using System;
using System.IO;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class User_Data : UserControl
    {
        private AutoScaler _scaler;

        //===========hud==========
        private HudControl _hud;
        // bit 7 của base_mode = ARMED (chuẩn MAVLink)
        private const byte MODE_FLAG_ARMED = 0x80;
        public User_Data()
        {
            InitializeComponent();

            // Khuyến nghị: tắt autoscale mặc định để tránh “double scale”
            this.AutoScaleMode = AutoScaleMode.None;

            // Nếu User_Data nằm trong panelMain → để Fill
            this.Dock = DockStyle.Fill;

            // Khởi tạo scaler
            _scaler = new AutoScaler(this);

            // Chụp layout gốc sau khi control đã tạo xong
            this.Load += (s, e) => _scaler.Capture();

            // Áp dụng scale khi kích thước thay đổi
            this.Resize += (s, e) => _scaler.Apply();

            //======================hud======================
            // ADD: Bật double-buffer cho panel hiển thị 3D để giảm flicker (không bắt buộc)
            pnl_data3d.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(pnl_data3d, true, null);

            // ADD: Khởi tạo HUD và nhúng vào pnl_data3d
            _hud = new HudControl { Dock = DockStyle.Fill };
            pnl_data3d.Controls.Add(_hud);

            // (tuỳ chọn) nếu chưa đặt Interval cho timer1
            if (timer1.Interval == 100) timer1.Interval = 50; // ~20Hz cho mượt

            //===============================================

            timer1.Start();


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblAltitude.Text = (TransmissionFrame.Gpi_RelAlt / 1000.0).ToString("0.00");

            double gs = TransmissionFrame.Vfr_Groundspeed > 0
                ? TransmissionFrame.Vfr_Groundspeed
                : Math.Sqrt(Math.Pow(TransmissionFrame.Gpi_Vx / 100.0, 2) + Math.Pow(TransmissionFrame.Gpi_Vy / 100.0, 2));
            lblGroundSpeed.Text = gs.ToString("0.00");

            double yawDeg = TransmissionFrame.Att_Yaw * 180.0 / Math.PI;
            lblYaw.Text = yawDeg.ToString("0");

            double vs = Math.Abs(TransmissionFrame.Vfr_Climb) > 1e-3
                ? TransmissionFrame.Vfr_Climb
                : -(TransmissionFrame.Gpi_Vz / 100.0);
            lblVerticalSpeed.Text = vs.ToString("0.00");

            float rollDeg = (float)(TransmissionFrame.Att_Roll * 180.0 / Math.PI);
            lblRoll.Text = rollDeg.ToString("0.00");
            float pitchDeg = (float)(TransmissionFrame.Att_Pitch * 180.0 / Math.PI);
            lblPitch.Text = pitchDeg.ToString("0.00");

            //======================hud==========================
            // ===== ADD: Cập nhật HUD từ TransmissionFrame =====

            // Dùng lại các biến bạn đã tính ngay ở trên
            float alt_m = (float)(TransmissionFrame.Gpi_RelAlt / 1000.0);
            if (alt_m == 0f) alt_m = TransmissionFrame.Vfr_Alt; // fallback

            double yaw360 = yawDeg;
            if (yaw360 < 0) yaw360 += 360.0;

            // Armed flag từ base_mode
            bool armed = (TransmissionFrame.Hb_base_mode & MODE_FLAG_ARMED) != 0;

            // Gán vào HUD
            _hud.RollDeg = (float)(TransmissionFrame.Att_Roll * 180.0 / Math.PI);
            _hud.PitchDeg = (float)(TransmissionFrame.Att_Pitch * 180.0 / Math.PI);
            _hud.YawDeg = (float)yaw360;
            _hud.Groundspeed = (float)gs;
            _hud.Airspeed = TransmissionFrame.Vfr_Airspeed;
            _hud.Altitude = alt_m;
            _hud.Climb = (float)vs;
            _hud.ThrottlePct = (int)TransmissionFrame.Vfr_Throttle;

            _hud.Armed = armed;
            _hud.ModeText = ((MAVLink.MAV_MODE_FLAG)TransmissionFrame.Hb_base_mode).ToString();
            _hud.GpsText = (TransmissionFrame.Gpi_Hdg == ushort.MaxValue) ? "No GPS" : "OK";

            // Pin: ưu tiên SYS_STATUS nếu có
            if (TransmissionFrame.Sys_Voltage_battery > 0)
            {
                var v = TransmissionFrame.Sys_Voltage_battery / 1000.0; // mV -> V
                var r = TransmissionFrame.Sys_Battery_remaining;
                _hud.BattText = $"Bat {v:0.00}V {r}%";
            }
            else
            {
                _hud.BattText = $"Bat {TransmissionFrame.Vfr_Throttle}%";
            }

            // Vẽ lại
            _hud.Invalidate();
            //==========================
        }

        private async void User_Data_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();

            webView21.CoreWebView2.PermissionRequested += (s, ev) =>
            {
                if (ev.PermissionKind ==
                    Microsoft.Web.WebView2.Core.CoreWebView2PermissionKind.Geolocation)
                    ev.State = Microsoft.Web.WebView2.Core.CoreWebView2PermissionState.Allow;
            };

            webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            webView21.CoreWebView2.NavigationCompleted += async (s, ev) =>
            {
                if (ev.IsSuccess && webView21.CoreWebView2 != null)
                    await webView21.CoreWebView2.ExecuteScriptAsync("requestCurrentLocation();");
            };

            var htmlFilePath = Path.Combine(Application.StartupPath, "map.html");
            if (File.Exists(htmlFilePath))
                webView21.Source = new Uri(htmlFilePath);
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            void Handle()
            {
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(e.WebMessageAsJson);
                string type = data.type ?? "";

                if (type == "currentLocation" || type == "watch")
                {
                    double lat = data.lat, lng = data.lng, acc = data.acc;
                    // cập nhật label nếu muốn
                    //lblLat.Text = lat.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
                    //lblLng.Text = lng.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
                    //lblAcc.Text = $"±{acc:0} m";

                }
                else if (type == "geoError")
                {
                    MessageBox.Show((string)data.message, "Geolocation");
                }
                // else: click map (lat/lng không có type) -> xử lý nếu cần
            }

            if (InvokeRequired) BeginInvoke((Action)Handle); else Handle();
        }

    }
}



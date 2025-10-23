using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace UAV_Timelapse
{
    public class MissionItem
    {
        public int STT { get; set; }
        public string Command { get; set; } = "WAYPOINT";
        public double Delay { get; set; }
        public double P2 { get; set; }
        public double P3 { get; set; }
        public double P4 { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double Alt { get; set; } = 100;
        public string Frame { get; set; } = "Relative";
    }

    public partial class User_Data : UserControl
    {
        private AutoScaler _scaler;

        //===========hud==========
        private HudControl _hud;
        private const byte MODE_FLAG_ARMED = 0x80;

        //=== Mission data binding
        private readonly BindingList<MissionItem> _items = new BindingList<MissionItem>();

        //=== DataGridView (tạo bằng code để tránh lỗi CS0103)
        private readonly DataGridView dgvMission = new DataGridView();

        public User_Data()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;

            // Scaler
            _scaler = new AutoScaler(this);
            this.Load += (s, e) => _scaler.Capture();
            this.Resize += (s, e) => _scaler.Apply();

            // HUD
            pnl_data3d.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(pnl_data3d, true, null);

            _hud = new HudControl { Dock = DockStyle.Fill };
            pnl_data3d.Controls.Add(_hud);

            if (timer1.Interval == 100) timer1.Interval = 50;
            timer1.Start();

            //===== DataGridView cấu hình nhanh
            SetupMissionGrid();
        }

        private void SetupMissionGrid()
        {
            dgvMission.Dock = DockStyle.Bottom;
            dgvMission.Height = 220;
            dgvMission.AutoGenerateColumns = false;
            dgvMission.AllowUserToAddRows = false;
            dgvMission.AllowUserToDeleteRows = true;
            dgvMission.RowHeadersVisible = false;

            // Cột
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "STT",
                DataPropertyName = "STT",
                Width = 45,
                ReadOnly = true
            });

            var colCmd = new DataGridViewComboBoxColumn
            {
                HeaderText = "Command",
                DataPropertyName = "Command",
                Width = 110,
                FlatStyle = FlatStyle.Flat
            };
            colCmd.Items.AddRange("WAYPOINT", "TAKEOFF", "LAND", "RTL", "LOITER_TIME");
            dgvMission.Columns.Add(colCmd);

            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Delay", DataPropertyName = "Delay", Width = 60 });
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "P2", DataPropertyName = "P2", Width = 55 });
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "P3", DataPropertyName = "P3", Width = 55 });
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "P4", DataPropertyName = "P4", Width = 55 });
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Lat", DataPropertyName = "Lat", Width = 120 });
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Long", DataPropertyName = "Long", Width = 120 });
            dgvMission.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Alt", DataPropertyName = "Alt", Width = 60 });

            var colFrame = new DataGridViewComboBoxColumn
            {
                HeaderText = "Frame",
                DataPropertyName = "Frame",
                Width = 90,
                FlatStyle = FlatStyle.Flat
            };
            colFrame.Items.AddRange("Relative", "Absolute", "Terrain");
            dgvMission.Columns.Add(colFrame);

            dgvMission.DataSource = _items;

            // Khi chỉnh lưới -> đẩy ngược lên map
            dgvMission.CellValueChanged += (s, e) => PushGridToMap();
            dgvMission.RowsRemoved += (s, e) => RenumberAndPush();
            dgvMission.UserAddedRow += (s, e) => RenumberAndPush();
            dgvMission.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvMission.IsCurrentCellDirty)
                {
                    dgvMission.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            this.Controls.Add(dgvMission);
        }

        private void RenumberAndPush()
        {
            for (int i = 0; i < _items.Count; i++) _items[i].STT = i + 1;
            dgvMission.Refresh();
            PushGridToMap();
        }

        private void PushGridToMap()
        {
            if (webView21?.CoreWebView2 == null) return;
            var payload = _items.Select(r => new { lat = r.Lat, lon = r.Long, alt = r.Alt });
            var msg = System.Text.Json.JsonSerializer.Serialize(new { type = "SET_WAYPOINTS", payload });

            webView21.CoreWebView2.PostWebMessageAsJson(msg);
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

            // HUD
            float alt_m = (float)(TransmissionFrame.Gpi_RelAlt / 1000.0);
            if (alt_m == 0f) alt_m = TransmissionFrame.Vfr_Alt;

            double yaw360 = yawDeg; if (yaw360 < 0) yaw360 += 360.0;
            bool armed = (TransmissionFrame.Hb_base_mode & MODE_FLAG_ARMED) != 0;

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

            if (TransmissionFrame.Sys_Voltage_battery > 0)
            {
                var v = TransmissionFrame.Sys_Voltage_battery / 1000.0;
                var r = TransmissionFrame.Sys_Battery_remaining;
                _hud.BattText = $"Bat {v:0.00}V {r}%";
            }
            else
            {
                _hud.BattText = $"Bat {TransmissionFrame.Vfr_Throttle}%";
            }

            _hud.Invalidate();
        }

        private async void User_Data_Load(object sender, EventArgs e)
        {
            // Khởi tạo WebView2
            await webView21.EnsureCoreWebView2Async(null);

            // Quyền geolocation nếu dùng
            webView21.CoreWebView2.PermissionRequested += (s, ev) =>
            {
                if (ev.PermissionKind == CoreWebView2PermissionKind.Geolocation)
                    ev.State = CoreWebView2PermissionState.Allow;
            };

            // Một handler duy nhất cho mọi message từ HTML
            webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            // Nạp map.html
            var htmlFilePath = Path.Combine(Application.StartupPath, "map.html");
            if (File.Exists(htmlFilePath))
                webView21.Source = new Uri(htmlFilePath);
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            void Handle()
            {
                using (var doc = System.Text.Json.JsonDocument.Parse(e.WebMessageAsJson))
                {
                    var root = doc.RootElement;

                    string type = root.TryGetProperty("type", out var t) ? (t.GetString() ?? "") : "";

                    if (string.Equals(type, "MISSION_CHANGED", StringComparison.OrdinalIgnoreCase))
                    {
                        var payload = root.GetProperty("payload");
                        var arr = payload.GetProperty("waypoints").EnumerateArray();

                        _items.RaiseListChangedEvents = false;
                        _items.Clear();
                        int i = 1;
                        foreach (var it in arr)
                        {
                            _items.Add(new MissionItem
                            {
                                STT = i++,
                                Lat = it.GetProperty("lat").GetDouble(),
                                Long = it.GetProperty("lon").GetDouble(),
                                Alt = it.GetProperty("alt").GetDouble(),
                                Command = "WAYPOINT",
                                Frame = "Relative"
                            });
                        }
                        _items.RaiseListChangedEvents = true;
                        dgvMission.Refresh();
                    }
                    else if (type == "currentLocation" || type == "watch")
                    {
                        // xử lý nếu cần
                    }
                    else if (type == "geoError")
                    {
                        string msg = root.TryGetProperty("message", out var m) ? (m.GetString() ?? "Geo error") : "Geo error";
                        MessageBox.Show(msg, "Geolocation");
                    }
                }
            }
            if (InvokeRequired) BeginInvoke((Action)Handle); else Handle();
        }

    }
}

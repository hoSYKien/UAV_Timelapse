using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace UAV_Timelapse
{
    public partial class User_Data : UserControl
    {
        private AutoScaler _scaler;

        //===========hud==========
        private HudControl _hud;
        private const byte MODE_FLAG_ARMED = 0x80;

        //=== Mission data binding
        private readonly BindingList<MissionItem> _items = new BindingList<MissionItem>();

        // LƯU Ý: KHÔNG tạo DataGridView mới. Dùng dataGridView2 + các cột dgr* đã có sẵn trong Designer.

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

            //===== DataGridView cấu hình: dùng dataGridView2 và cột dgr*
            SetupMissionGrid();
        }

        //GPS-------------------------------------------------------------------------------------
        private bool TryGetBestPose(out double latDeg, out double lonDeg, out double alt_m, out double hdg_deg)
        {
            // Mặc định
            latDeg = lonDeg = alt_m = double.NaN;
            hdg_deg = -1;

            // Ưu tiên GPI (đã hợp nhất EKF)
            bool hasGpi = (TransmissionFrame.Gpi_Lat != 0 || TransmissionFrame.Gpi_Lon != 0);
            if (hasGpi)
            {
                latDeg = TransmissionFrame.Gpi_LatDeg;       // deg
                lonDeg = TransmissionFrame.Gpi_LonDeg;       // deg
                alt_m = TransmissionFrame.Gpi_RelAlt_m > 0  // ưu tiên relAlt nếu có, không thì alt AMSL
                       ? TransmissionFrame.Gpi_RelAlt_m
                       : TransmissionFrame.Gpi_Alt_m;
                hdg_deg = TransmissionFrame.Gpi_Hdg_deg;     // -1 nếu unknown
            }

            // Fallback sang GPS raw nếu thiếu GPI hoặc GPI chưa hợp lệ
            bool needFallback = double.IsNaN(latDeg) || double.IsNaN(lonDeg) || (latDeg == 0 && lonDeg == 0);
            if (needFallback && (TransmissionFrame.Gps_Lat != 0 || TransmissionFrame.Gps_Lon != 0))
            {
                latDeg = TransmissionFrame.Gps_LatDeg;
                lonDeg = TransmissionFrame.Gps_LonDeg;
                if (double.IsNaN(alt_m) || alt_m <= 0) alt_m = TransmissionFrame.Gps_Alt_m;
                if (hdg_deg < 0) hdg_deg = TransmissionFrame.Gps_Cog_deg;
            }

            // Hợp lệ nếu có lat/lon khác 0 và không NaN
            return !double.IsNaN(latDeg) && !double.IsNaN(lonDeg) && !(Math.Abs(latDeg) < 1e-9 && Math.Abs(lonDeg) < 1e-9);
        }
        private void PushTelemetryToMap()
        {
            if (webView21?.CoreWebView2 == null) return;

            if (!TryGetBestPose(out double lat, out double lon, out double alt_m, out double hdg))
                return; // chưa có GPS

            // Vận tốc mặt đất (m/s) ưu tiên VFR_HUD, fallback GPI
            double gs = TransmissionFrame.Vfr_Groundspeed > 0
                ? TransmissionFrame.Vfr_Groundspeed
                : Math.Sqrt(Math.Pow(TransmissionFrame.Gpi_Vx_mps, 2) + Math.Pow(TransmissionFrame.Gpi_Vy_mps, 2));

            var payload = new
            {
                lat,
                lon,
                alt = alt_m,
                hdg = (hdg < 0) ? (double?)null : hdg, // null nếu unknown
                vx = TransmissionFrame.Gpi_Vx_mps,
                vy = TransmissionFrame.Gpi_Vy_mps,
                vz = TransmissionFrame.Gpi_Vz_mps,
                gs,
                armed = (TransmissionFrame.Hb_base_mode & 0x80) != 0
            };

            var json = System.Text.Json.JsonSerializer.Serialize(new { type = "VEHICLE_TELEMETRY", payload });
            webView21.CoreWebView2.PostWebMessageAsJson(json);
        }


        //-------------------------------------------------------------------------------------
        private void SetupMissionGrid()
        {
            var gv = dataGridView2; // dùng lưới sẵn có
            gv.AutoGenerateColumns = false;
            gv.AllowUserToAddRows = false;
            gv.AllowUserToDeleteRows = true;
            gv.RowHeadersVisible = false;

            // Gán DataPropertyName cho các cột đã tạo trong Designer
            // (các cột này phải tồn tại dưới dạng field: dgrSTT, dgrCommand, dgrDelay, dgrP2, dgrP3, dgrP4, dgrLat, dgrLong, dgrAlt, dgrFrame)
            dgrSTT.DataPropertyName = nameof(MissionItem.STT);
            dgrCommand.DataPropertyName = nameof(MissionItem.Command);
            dgrDelay.DataPropertyName = nameof(MissionItem.Delay);
            dgrDelay2.DataPropertyName = nameof(MissionItem.P2);
            dgrDelay3.DataPropertyName = nameof(MissionItem.P3);
            dgrDelay4.DataPropertyName = nameof(MissionItem.P4);
            dgrLat.DataPropertyName = nameof(MissionItem.Lat);
            dgrLong.DataPropertyName = nameof(MissionItem.Long);
            dgrAlt.DataPropertyName = nameof(MissionItem.Alt);
            dgrFrame.DataPropertyName = nameof(MissionItem.Frame);

            // Nạp item cho các ComboBoxColumn nếu có
            if (dgrCommand is DataGridViewComboBoxColumn cmd)
            {
                cmd.FlatStyle = FlatStyle.Flat;
                cmd.Items.Clear();
                cmd.Items.AddRange("WAYPOINT", "TAKEOFF", "LAND", "RTL", "LOITER_TIME");
            }
            if (dgrFrame is DataGridViewComboBoxColumn fr)
            {
                fr.FlatStyle = FlatStyle.Flat;
                fr.Items.Clear();
                fr.Items.AddRange("Relative", "Absolute", "Terrain");
            }

            gv.DataSource = _items;

            // Khi chỉnh lưới -> đẩy ngược lên map
            gv.CellValueChanged += (s, e) => PushGridToMap();
            gv.RowsRemoved += (s, e) => RenumberAndPush();
            gv.UserAddedRow += (s, e) => RenumberAndPush();
            gv.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (gv.IsCurrentCellDirty)
                    gv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            // KHÔNG Add gv vào Controls vì nó đã nằm trong form qua Designer
        }

        private void RenumberAndPush()
        {
            for (int i = 0; i < _items.Count; i++) _items[i].STT = i + 1;
            dataGridView2.Refresh();
            PushGridToMap();
        }

        private void PushGridToMap()
        {
            if (webView21?.CoreWebView2 == null) return;

            var payload = _items.Select(r => new { lat = r.Lat, lon = r.Long, alt = r.Alt });
            // PostWebMessageAsJson nhận string JSON -> Serialize thành chuỗi JSON
            var json = System.Text.Json.JsonSerializer.Serialize(new { type = "SET_WAYPOINTS", payload });
            webView21.CoreWebView2.PostWebMessageAsJson(json);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // ===== Altitude (ưu tiên RelAlt (mm) -> m), fallback VFR_Alt (m) =====
            double relAlt_m = TransmissionFrame.Gpi_RelAlt / 1000.0;
            double altForHud_m = (relAlt_m > 0.001) ? relAlt_m : TransmissionFrame.Vfr_Alt;
            lblAltitude.Text = altForHud_m.ToString("0.00");

            // ===== Ground speed =====
            double gs = TransmissionFrame.Vfr_Groundspeed > 0
                ? TransmissionFrame.Vfr_Groundspeed
                : Math.Sqrt(Math.Pow(TransmissionFrame.Gpi_Vx / 100.0, 2) + Math.Pow(TransmissionFrame.Gpi_Vy / 100.0, 2));
            lblGroundSpeed.Text = gs.ToString("0.00");

            // ===== Attitude / Yaw =====
            double yawDeg = TransmissionFrame.Att_Yaw * 180.0 / Math.PI;
            double yaw360 = yawDeg; if (yaw360 < 0) yaw360 += 360.0;
            lblYaw.Text = yawDeg.ToString("0");

            // ===== Vertical speed (ưu tiên VFR_Climb, fallback -Vz) =====
            double vs = Math.Abs(TransmissionFrame.Vfr_Climb) > 1e-3
                ? TransmissionFrame.Vfr_Climb
                : -(TransmissionFrame.Gpi_Vz / 100.0);
            lblVerticalSpeed.Text = vs.ToString("0.00");

            // ===== Roll/Pitch deg =====
            float rollDeg = (float)(TransmissionFrame.Att_Roll * 180.0 / Math.PI);
            float pitchDeg = (float)(TransmissionFrame.Att_Pitch * 180.0 / Math.PI);
            lblRoll.Text = rollDeg.ToString("0.00");
            lblPitch.Text = pitchDeg.ToString("0.00");

            // ===== HUD binding =====
            bool armed = (TransmissionFrame.Hb_base_mode & MODE_FLAG_ARMED) != 0;

            _hud.RollDeg = rollDeg;
            _hud.PitchDeg = pitchDeg;
            _hud.YawDeg = (float)yaw360;
            _hud.Groundspeed = (float)gs;
            _hud.Airspeed = TransmissionFrame.Vfr_Airspeed;
            _hud.Altitude = (float)altForHud_m;
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

            // ===== Push pose/velocity lên map.html =====
            PushTelemetryToMap();
        }


        private async void User_Data_Load(object sender, EventArgs e)
        {
            // Khởi tạo WebView2
            await webView21.EnsureCoreWebView2Async(null);

            // Cho phép geolocation
            webView21.CoreWebView2.PermissionRequested += (s, ev) =>
            {
                if (ev.PermissionKind == CoreWebView2PermissionKind.Geolocation)
                    ev.State = CoreWebView2PermissionState.Allow;
            };

            // Nhận message từ HTML
            webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            // Khi trang nạp xong -> tự động bấm Start GPS
            webView21.CoreWebView2.NavigationCompleted += (s, ev) =>
            {
                string js = @"
        (function () {
            if (window.__wv2_bound) return;
            window.__wv2_bound = true;

            // Lắng nghe message từ C#
            window.chrome.webview.addEventListener('message', function (ev) {
                // Có môi trường nhận chuỗi, có môi trường nhận object
                const msg = (typeof ev.data === 'string') ? JSON.parse(ev.data) : ev.data;
                if (!msg || !msg.type) return;

                if (msg.type === 'VEHICLE_TELEMETRY') {
                    const p = msg.payload; // {lat,lon,alt,hdg,vx,vy,vz,gs,armed}
                    // TODO: cập nhật marker máy bay, heading, pan map...
                    // updateVehicleMarker(p);
                } else if (msg.type === 'SET_WAYPOINTS') {
                    const wps = msg.payload; // mảng waypoint {lat,lon,alt}
                    // TODO: vẽ polyline/markers waypoints
                    // drawWaypoints(wps);
                }
            });

            console.log('WV2 listener bound');
        })();
    ";
                webView21.CoreWebView2.ExecuteScriptAsync(js);
            };


            // Nạp map.html
            var htmlFilePath = Path.Combine(Application.StartupPath, "map.html");
            if (File.Exists(htmlFilePath))
                webView21.Source = new Uri(htmlFilePath);
            else
                MessageBox.Show("Không tìm thấy map.html trong thư mục ứng dụng.", "Thông báo");
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
                        dataGridView2.Refresh();
                    }
                    else if (type == "GPS_UPDATE")
                    {
                        // Nếu cần log GPS từ trang map.html thì xử lý tại đây.
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
}

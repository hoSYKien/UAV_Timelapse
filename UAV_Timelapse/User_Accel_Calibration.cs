using System;
using System.Windows.Forms;
using static MAVLink;

namespace UAV_Timelapse
{
    public partial class User_Accel_Calibration : UserControl
    {
        private readonly Form_Main _main;
        private bool _incalibrate = false;
        private byte _count = 0;

        // FC gửi yêu cầu tư thế qua COMMAND_LONG.ACCELCAL_VEHICLE_POS, pos nằm ở param1
        private float _pos = 0;
        public User_Accel_Calibration(Form_Main main)
        {
            InitializeComponent();
            _main = main ?? throw new ArgumentNullException(nameof(main));

            // nhận message từ Form_Main (thay cho SubscribeToPacketType của MissionPlanner)
            _main.OnStatustext += Main_OnStatustext;
            _main.OnCommandLong += Main_OnCommandLong;

            ResetUi();
        }


        private void ResetUi()
        {
            _incalibrate = false;
            _count = 0;
            _pos = 0;

            btnCalibAccel.Text = "Calibrate Accel";
            btnCalibAccel.Enabled = true;

            btnCalibLevel.Text = "Calibrate Level";
            btnSimpleAccelCal.Text = "Simple Accel Cal";

            // đổi tên label nếu bạn đặt khác
            lblStatus.Text = "";
        }


        private void btnCalibAccel_Click(object sender, EventArgs e)
        {
            //if (!_main.IsConnected)
            //{
            //    MessageBox.Show("Chưa kết nối FCU (COM chưa mở).");
            //    return;
            //}
            if (!_main.IsConnected)
            {
                MessageBox.Show("Chưa kết nối FCU.\n" + _main.GetConnDebug());
                return;
            }

            // Giống MP: nếu đang calib thì mỗi click gửi ACCELCAL_VEHICLE_POS với param1=pos
            if (_incalibrate)
            {
                _count++;

                _main.SendCommandLong(Form_Main.ACCELCAL_VEHICLE_POS, p1: _pos);

                return;
            }

            // Bắt đầu accel calibration: PREFLIGHT_CALIBRATION với param5=1
            _count = 0;

            _main.SendCommandLong(Form_Main.PREFLIGHT_CALIBRATION,
                p1: 0, p2: 0, p3: 0, p4: 0,
                p5: 1,   // accel calib start
                p6: 0, p7: 0);

            _incalibrate = true;
            btnCalibAccel.Text = "Click when Done";
            lblStatus.Text = "Đang bắt đầu Accel Calibration... chờ FCU hướng dẫn đặt tư thế.";
        }

        private void btnCalibLevel_Click(object sender, EventArgs e)
        {
            if (!_main.IsConnected)
            {
                MessageBox.Show("Chưa kết nối FCU (COM chưa mở).");
                return;
            }

            // MP: PREFLIGHT_CALIBRATION param5=2
            _main.SendCommandLong(Form_Main.PREFLIGHT_CALIBRATION,
                p1: 0, p2: 0, p3: 0, p4: 0,
                p5: 2,
                p6: 0, p7: 0);

            btnCalibLevel.Text = "Completed";
        }

        private void btnSimpleAccelCal_Click(object sender, EventArgs e)
        {
            if (!_main.IsConnected)
            {
                MessageBox.Show("Chưa kết nối FCU (COM chưa mở).");
                return;
            }

            // MP: PREFLIGHT_CALIBRATION param5=4
            _main.SendCommandLong(Form_Main.PREFLIGHT_CALIBRATION,
                p1: 0, p2: 0, p3: 0, p4: 0,
                p5: 4,
                p6: 0, p7: 0);

            btnSimpleAccelCal.Text = "Completed";
        }
        private void Main_OnCommandLong(MAVLink.mavlink_command_long_t cl)
        {
            if (cl.command != Form_Main.ACCELCAL_VEHICLE_POS) return;

            int p = (int)Math.Round(cl.param1);

            // Lọc rác: ArduPilot thường dùng 1..6 cho 6 mặt
            if (p < 1 || p > 6) return;   // sẽ chặn 16777215

            _pos = p;
            lblStatus.Text = "Please place vehicle " + PosToText(p);
        }

        private void Main_OnStatustext(string msg)
        {
            if (!this.Visible) return;
            if (string.IsNullOrWhiteSpace(msg)) return;

            string m = msg.Trim();
            string low = m.ToLowerInvariant();

            // MP chỉ update label nếu có "place vehicle" hoặc "calibration"
            if (low.Contains("place vehicle") || low.Contains("calibration"))
            {
                lblStatus.Text = m;
            }

            // kết thúc
            if (low.Contains("calibration successful") || low.Contains("calibration failed"))
            {
                _incalibrate = false;
                btnCalibAccel.Text = "Done";
                btnCalibAccel.Enabled = false;
                lblStatus.Text = "Calibration successful";
            }
        }

        private static string PosToText(float pos)
        {
            int p = (int)Math.Round(pos);

            switch (p)
            {
                case 1: return "LEVEL (đặt phẳng)";
                case 2: return "LEFT (nghiêng trái)";
                case 3: return "RIGHT (nghiêng phải)";
                case 4: return "NOSE DOWN (cắm đầu xuống)";
                case 5: return "NOSE UP (ngửa đầu lên)";
                case 6: return "BACK/UPSIDE DOWN (lật ngửa)";
                default: return p.ToString();
            }
        }

    }
}

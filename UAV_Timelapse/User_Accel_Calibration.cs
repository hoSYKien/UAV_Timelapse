using System;
using System.Windows.Forms;
using static MAVLink;

namespace UAV_Timelapse
{
    public partial class User_Accel_Calibration : UserControl
    {
        private readonly Form_Main _main;

        // Thứ tự tư thế giống ArduPilot / Mission Planner
        private readonly Form_Main.AccelPose[] PoseOrder =
        {
            Form_Main.AccelPose.LEVEL,
            Form_Main.AccelPose.LEFT,
            Form_Main.AccelPose.RIGHT,
            Form_Main.AccelPose.NOSEUP,
            Form_Main.AccelPose.NOSEDOWN,
            Form_Main.AccelPose.BACK
        };

        private int _currentPoseIndex = -1;   // -1 = chưa vào step nào
        private bool _running = false;

        // timeout mỗi step (giống MP tầm 20 s)
        private readonly Timer _stepTimeout = new Timer { Interval = 20000 };

        public User_Accel_Calibration(Form_Main main)
        {
            InitializeComponent();
            _main = main;

            // đăng ký event từ Form_Main
            _main.OnStatusText += HandleStatusText;
            _main.OnCommandAck += HandleCommandAck;

            _stepTimeout.Tick += StepTimeout_Tick;

            lblStatus.Text = "Nhấn \"Calibrate Accel\" để bắt đầu.";
        }

        private void btnCalibAccel_Click(object sender, EventArgs e)
        {
            // TRẠNG THÁI 1: chưa chạy calib -> gửi PREFLIGHT_CALIBRATION
            if (!_running)
            {
                if (_main.IsArmed)
                {
                    MessageBox.Show("Vui lòng DISARM trước khi calib accelerometer.");
                    return;
                }

                _running = true;
                _currentPoseIndex = -1;
                rtxtDataRespond.Clear();

                lblStatus.Text = "Gửi lệnh PREFLIGHT_CALIBRATION (accel)...";
                btnCalibAccel.Text = "Click when DONE";

                _main.StartAccelCalibration();
                return;
            }

            // TRẠNG THÁI 2: đang calib -> nút đóng vai trò "Click when DONE"
            if (_currentPoseIndex < 0 || _currentPoseIndex >= PoseOrder.Length)
            {
                MessageBox.Show("Chưa nhận tư thế từ FC.");
                return;
            }

            var pose = PoseOrder[_currentPoseIndex];
            lblStatus.Text = $"Gửi kết thúc tư thế: {pose}...";
            _main.SendAccelCalVehiclePos(pose);
        }
        private void HandleCommandAck(ushort command, byte result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleCommandAck(command, result)));
                return;
            }

            rtxtDataRespond.AppendText(
                $"ACK cmd={command} result={(MAV_RESULT)result}\r\n");

            if (!_running) return;

            if (command == Form_Main.PREFLIGHT_CALIBRATION)
            {
                if (result == (byte)MAV_RESULT.ACCEPTED ||
                    result == (byte)MAV_RESULT.IN_PROGRESS)
                {
                    lblStatus.Text = "Đã nhận ACK calib. Đang chờ FC yêu cầu tư thế đầu tiên...";
                    // chờ STATUSTEXT first pose
                }
                else
                {
                    lblStatus.Text = $"FC từ chối calib (result={(MAV_RESULT)result}).";
                    _running = false;
                }
            }
            else if (command == Form_Main.ACCELCAL_VEHICLE_POS)
            {
                if (result == (byte)MAV_RESULT.ACCEPTED)
                {
                    // đã xong tư thế hiện tại, đợi STATUSTEXT thông báo tư thế tiếp theo
                    _stepTimeout.Stop();
                    _stepTimeout.Start();
                }
            }
        }

        private void HandleStatusText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandleStatusText(text)));
                return;
            }

            rtxtDataRespond.AppendText("STATUSTEXT: " + text + "\r\n");

            if (!_running) return;

            string lower = text.ToLowerInvariant();

            // thông báo thành công / lỗi
            if (lower.Contains("calibration successful"))
            {
                lblStatus.Text = "Calib accelerometer thành công.";
                _running = false;
                _currentPoseIndex = -1;
                _stepTimeout.Stop();
                return;
            }
            if (lower.Contains("failed"))
            {
                lblStatus.Text = "Calib accelerometer FAILED, xem lại tư thế hoặc reboot FC.";
                _running = false;
                _stepTimeout.Stop();
                return;
            }

            // parse text để xem FC đang yêu cầu tư thế nào
            if (lower.Contains("level"))
                SetPose(Form_Main.AccelPose.LEVEL);
            else if (lower.Contains("left"))
                SetPose(Form_Main.AccelPose.LEFT);
            else if (lower.Contains("right"))
                SetPose(Form_Main.AccelPose.RIGHT);
            else if (lower.Contains("nose up") || lower.Contains("nose-up"))
                SetPose(Form_Main.AccelPose.NOSEUP);
            else if (lower.Contains("nose down") || lower.Contains("nose-down"))
                SetPose(Form_Main.AccelPose.NOSEDOWN);
            else if (lower.Contains("back"))
                SetPose(Form_Main.AccelPose.BACK);
        }

        private void SetPose(Form_Main.AccelPose pose)
        {
            _currentPoseIndex = Array.IndexOf(PoseOrder, pose);
            if (_currentPoseIndex < 0) return;

            lblStatus.Text = $"Đặt máy ở tư thế: {pose} rồi nhấn \"Click when DONE\".";
            _stepTimeout.Stop();
            _stepTimeout.Start();
        }

        private void StepTimeout_Tick(object sender, EventArgs e)
        {
            _stepTimeout.Stop();
            if (!_running) return;

            MessageBox.Show("Hết thời gian cho bước này (20s). Kiểm tra kết nối hoặc xem FC có treo không.");

            _running = false;
            _currentPoseIndex = -1;
            btnCalibAccel.Text = "Calibrate Accel";
        }
    }
}

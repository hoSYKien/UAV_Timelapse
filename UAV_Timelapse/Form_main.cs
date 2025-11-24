using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static MAVLink;
namespace UAV_Timelapse
{
    public partial class Form_Main : Form
    {
        // Variable
        bool checkBtnOption = true;
        bool checkBtnData = true;

        private AutoScaler _scaler;

        // ====== GCS/FCU IDs ======
        private readonly byte GcsSysId = 255;
        private readonly byte GcsCompId = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
        // Sẽ update FcuSysId/FcuCompId sau khi thấy HEARTBEAT đầu tiên
        private byte FcuSysId = 1, FcuCompId = 1;

        // ====== Timers ======
        private System.Windows.Forms.Timer hbTimer = new System.Windows.Forms.Timer();
        private void HbTimer_Tick(object sender, EventArgs e) => SendHeartbeat();

        private const ushort MAV_CMD_SET_MESSAGE_INTERVAL = 511;
        //-------------------------------------

        //
        private SerialPort serialPort1;
        private MAVLink.MavlinkParse mavlink = new MAVLink.MavlinkParse();

        // Nullable structs để so sánh với null
        private MAVLink.mavlink_vfr_hud_t? hud;
        private MAVLink.mavlink_global_position_int_t? globalPos;
        private MAVLink.mavlink_heartbeat_t? heartbeat;
        private MAVLink.mavlink_attitude_t? attitude;
        private MAVLink.mavlink_highres_imu_t? imu;
        private MAVLink.mavlink_gps_raw_int_t? gpsRaw;  // thêm
        private MAVLink.mavlink_rc_channels_t? rcCh;

        // Buffer toàn cục để xử lý packet chưa đầy
        private List<byte> mavlinkBuffer = new List<byte>();

        User_RTK_GPS_Inject userRTK = new User_RTK_GPS_Inject();
        User_CAN_GPS_Order userCan = new User_CAN_GPS_Order();
        User_Joystick user_Joystick = new User_Joystick();
        User_Compass_Motor_Calid userComp = new User_Compass_Motor_Calid();
        User_Range_Finder user_Range_Finder = new User_Range_Finder();
        User_Optical_Flow_and_OSD user_Optical_Flow_And_OSD = new User_Optical_Flow_and_OSD();
        User_Camera_Gimbal user_Camera_Gimbal = new User_Camera_Gimbal();
        User_Motor_Test user_Motor_Test = new User_Motor_Test();
        User_Install_Firmware user_Install_Firmware = new User_Install_Firmware();
        User_Data user_Data = new User_Data();
        User_Frame_Type user_Frame_Type = new User_Frame_Type();
        User_Accel_Calibration user_Accel_Calibration;
        User_Compass user_Compass = new User_Compass();
        User_Radio_Calibration user_Radio_Calibration = new User_Radio_Calibration();
        User_Servo_Output user_Servo_Output = new User_Servo_Output();

        User_Full_Parameter_List user_Full_Parameter_List;
        /*------------------------------------------*/

        public Form_Main()
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
            serialPort1 = new SerialPort();
            serialPort1.DataReceived += SerialPort1_DataReceived;
            RefreshPorts();

            //this.fullS

            user_Accel_Calibration = new User_Accel_Calibration(this);
            user_Full_Parameter_List = new User_Full_Parameter_List(this);
        }

        private bool _isArmed = false;
        public bool IsArmed => _isArmed;
        const byte SAFETY_ARMED = 0x80; // 128
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytesToRead = serialPort1.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                serialPort1.Read(buffer, 0, bytesToRead);

                long bytesConsumed = 0;
                mavlinkBuffer.AddRange(buffer);

                using (var ms = new MemoryStream(mavlinkBuffer.ToArray()))
                {
                    MAVLink.MAVLinkMessage msg;
                    try
                    {
                        while ((msg = mavlink.ReadPacket(ms)) != null)
                        {
                            // Ghi nhận sysid/compid của FCU từ msg
                            FcuSysId = msg.sysid;
                            FcuCompId = msg.compid;

                            switch ((MAVLink.MAVLINK_MSG_ID)msg.msgid)
                            {
                                case MAVLink.MAVLINK_MSG_ID.HEARTBEAT:
                                    //heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
                                    //break;

                                    var hb = (MAVLink.mavlink_heartbeat_t)msg.data;
                                    heartbeat = hb;

                                    const byte SAFETY_ARMED = 0x80;           // MAV_MODE_FLAG_SAFETY_ARMED
                                    bool armed = (hb.base_mode & SAFETY_ARMED) != 0;

                                    // đẩy sang User_Data (instance bạn đã tạo: user_Data)
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        user_Data.SetArmed(armed);
                                        // nếu muốn: khóa/mở nút Calibrate khi đang armed
                                        btnAccelCalibration.Enabled = !armed;

                                        // lần đầu nhận heartbeat thì detect firmware
                                        if (string.IsNullOrEmpty(_pdefFileName))
                                        {
                                            _pdefFileName = GetPdefFileName(hb);

                                            // nếu muốn show lên UI:
                                            // lblFirmware.Text = _pdefFileName ?? "Unknown FW";
                                        }
                                    }));
                                    break;

                                case MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT:
                                    globalPos = (MAVLink.mavlink_global_position_int_t)msg.data;
                                    break;
                                case MAVLink.MAVLINK_MSG_ID.VFR_HUD:
                                    hud = (MAVLink.mavlink_vfr_hud_t)msg.data;
                                    break;
                                case MAVLink.MAVLINK_MSG_ID.ATTITUDE:
                                    attitude = (MAVLink.mavlink_attitude_t)msg.data;
                                    break;
                                case MAVLink.MAVLINK_MSG_ID.HIGHRES_IMU:
                                    imu = (MAVLink.mavlink_highres_imu_t)msg.data;
                                    break;
                                case MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT:
                                    gpsRaw = (MAVLink.mavlink_gps_raw_int_t)msg.data; // lưu lại để UpdateTelemetryDisplay dùng
                                    break;
                                case MAVLink.MAVLINK_MSG_ID.RC_CHANNELS:           // msg_id = 65
                                    rcCh = (MAVLink.mavlink_rc_channels_t)msg.data;
                                    UpdateRcFromChannels(rcCh.Value);
                                    break;
                                case MAVLink.MAVLINK_MSG_ID.STATUSTEXT:
                                    {
                                        var st = (MAVLink.mavlink_statustext_t)msg.data;
                                        string txt = Encoding.ASCII.GetString(st.text).TrimEnd('\0');

                                        // raise event cho User_Accel_Calibration
                                        RaiseStatusText(txt);
                                        break;
                                    }
                                case MAVLink.MAVLINK_MSG_ID.COMMAND_ACK:
                                    {
                                        var ack = (MAVLink.mavlink_command_ack_t)msg.data;
                                        RaiseCommandAck(ack.command, ack.result);
                                        break;
                                    }
                                case MAVLink.MAVLINK_MSG_ID.PARAM_VALUE:
                                    {
                                        var p = (MAVLink.mavlink_param_value_t)msg.data;
                                        RaiseParamValue(p);
                                        break;
                                    }

                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        // Packet chưa đầy, chờ thêm byte
                    }

                    bytesConsumed = ms.Position;
                }

                if (bytesConsumed > 0)
                    mavlinkBuffer = mavlinkBuffer.Skip((int)bytesConsumed).ToList();

                this.BeginInvoke(new Action(UpdateTelemetryDisplay));
            }
            catch { /* tránh crash */ }
        }

        private void UpdateTelemetryDisplay()
        {
            // ===== GPS_RAW_INT (nếu có) =====
            if (gpsRaw.HasValue)
            {
                var gr = gpsRaw.Value;

                // raw giữ nguyên đơn vị MAVLink
                TransmissionFrame.Gps_FixType = gr.fix_type;
                TransmissionFrame.Gps_SatellitesVisible = gr.satellites_visible;
                TransmissionFrame.Gps_Eph = gr.eph;   // cm
                TransmissionFrame.Gps_Epv = gr.epv;   // cm
                TransmissionFrame.Gps_Vel = gr.vel;   // cm/s
                TransmissionFrame.Gps_Cog = gr.cog;   // cdeg
                TransmissionFrame.Gps_Lat = gr.lat;   // 1e-7 deg
                TransmissionFrame.Gps_Lon = gr.lon;   // 1e-7 deg
                TransmissionFrame.Gps_Alt = gr.alt;   // mm

                // đổi đơn vị (tuỳ chọn, dễ hiển thị)
                TransmissionFrame.Gps_LatDeg = gr.lat * 1e-7;
                TransmissionFrame.Gps_LonDeg = gr.lon * 1e-7;
                TransmissionFrame.Gps_Alt_m = gr.alt / 1000.0;          // mm -> m
                TransmissionFrame.Gps_Spd_mps = gr.vel / 100.0;           // cm/s -> m/s
                TransmissionFrame.Gps_Cog_deg = (gr.cog == 65535) ? -1.0  // unknown
                                             : (gr.cog / 100.0);          // cdeg -> deg

                TransmissionFrame.Gps_HDOP = (gr.eph == ushort.MaxValue) ? double.NaN : gr.eph / 100.0;
                TransmissionFrame.Gps_VDOP = (gr.epv == ushort.MaxValue) ? double.NaN : gr.epv / 100.0;
            }

            // ===== GLOBAL_POSITION_INT + các khối khác chỉ cập nhật khi có =====
            if (globalPos.HasValue)
            {
                var g = globalPos.Value;

                // raw
                TransmissionFrame.Gpi_Lat = g.lat;
                TransmissionFrame.Gpi_Lon = g.lon;
                TransmissionFrame.Gpi_Alt = g.alt;
                TransmissionFrame.Gpi_RelAlt = g.relative_alt;
                TransmissionFrame.Gpi_Vx = g.vx;
                TransmissionFrame.Gpi_Vy = g.vy;
                TransmissionFrame.Gpi_Vz = g.vz;
                TransmissionFrame.Gpi_Hdg = g.hdg;

                // đổi đơn vị
                TransmissionFrame.Gpi_LatDeg = g.lat * 1e-7;
                TransmissionFrame.Gpi_LonDeg = g.lon * 1e-7;
                TransmissionFrame.Gpi_Alt_m = g.alt / 1000.0;           // mm -> m
                TransmissionFrame.Gpi_RelAlt_m = g.relative_alt / 1000.0;  // mm -> m
                TransmissionFrame.Gpi_Vx_mps = g.vx / 100.0;             // cm/s -> m/s
                TransmissionFrame.Gpi_Vy_mps = g.vy / 100.0;
                TransmissionFrame.Gpi_Vz_mps = g.vz / 100.0;
                TransmissionFrame.Gpi_Hdg_deg = (g.hdg == 65535) ? -1.0 : (g.hdg / 100.0);
            }

            if (heartbeat.HasValue)
            {
                var hb = heartbeat.Value;
                TransmissionFrame.Hb_type = hb.type;
                TransmissionFrame.Hb_autopilot = hb.autopilot;
                TransmissionFrame.Hb_base_mode = hb.base_mode;
                TransmissionFrame.Hb_custom_mode = hb.custom_mode;
                TransmissionFrame.Hb_system_status = hb.system_status;
            }

            if (hud.HasValue)
            {
                var h = hud.Value;
                TransmissionFrame.Vfr_Airspeed = h.airspeed;
                TransmissionFrame.Vfr_Groundspeed = h.groundspeed;
                TransmissionFrame.Vfr_Heading = (short)h.heading;
                TransmissionFrame.Vfr_Throttle = (ushort)h.throttle;
                TransmissionFrame.Vfr_Alt = h.alt;
                TransmissionFrame.Vfr_Climb = h.climb;
            }

            if (attitude.HasValue)
            {
                var at = attitude.Value;
                TransmissionFrame.Att_Roll = at.roll;
                TransmissionFrame.Att_Pitch = at.pitch;
                TransmissionFrame.Att_Yaw = at.yaw;
                TransmissionFrame.Att_Rollspeed = at.rollspeed;
                TransmissionFrame.Att_Pitchspeed = at.pitchspeed;
                TransmissionFrame.Att_Yawspeed = at.yawspeed;
            }

            if (imu.HasValue)
            {
                var im = imu.Value;
                TransmissionFrame.Imu_Xacc = im.xacc;
                TransmissionFrame.Imu_Yacc = im.yacc;
                TransmissionFrame.Imu_Zacc = im.zacc;
                TransmissionFrame.Imu_Xgyro = im.xgyro;
                TransmissionFrame.Imu_Ygyro = im.ygyro;
                TransmissionFrame.Imu_Zgyro = im.zgyro;
                TransmissionFrame.Imu_Xmag = im.xmag;
                TransmissionFrame.Imu_Ymag = im.ymag;
                TransmissionFrame.Imu_Zmag = im.zmag;
                TransmissionFrame.Imu_AbsPressure = im.abs_pressure;
                TransmissionFrame.Imu_DiffPressure = im.diff_pressure;
                TransmissionFrame.Imu_PressureAlt = im.pressure_alt;
                TransmissionFrame.Imu_Temperature = im.temperature;
            }
        }


        private void RefreshPorts()
        {
            comboBoxPorts.Items.Clear();
            var ports = SerialPort.GetPortNames();
            comboBoxPorts.Items.AddRange(ports);
            if (ports.Length > 0)
                comboBoxPorts.SelectedIndex = 0;
        }

        // Funcion
        private void addUserControl(UserControl usercontrol)
        {
            // Loại bỏ mọi control trước đó (tránh chồng chéo)
            panelMain.SuspendLayout();
            try
            {
                panelMain.Controls.Clear();

                // TUYỆT ĐỐI KHÔNG bật AutoSize ở đây
                usercontrol.AutoSize = false;
                usercontrol.Margin = Padding.Empty;
                usercontrol.Padding = Padding.Empty;
                usercontrol.Dock = DockStyle.Fill;

                panelMain.Padding = Padding.Empty;
                panelMain.Margin = Padding.Empty;

                panelMain.Controls.Add(usercontrol);
                usercontrol.BringToFront();
            }
            finally
            {
                panelMain.ResumeLayout();
            }
        }


        /*------------------------------------------*/

        private void btnOptional_Click(object sender, EventArgs e)
        {
            //int tmp = PanelOptional.Height;
            if (checkBtnOption)
            {
                for (int i = 420; i > 0; i -= 20)
                {
                    if (i != 0) PanelOptional.Size = new Size(160, i);
                }
                PanelOptional.Visible = false;
                checkBtnOption = false;
            }
            else
            {
                PanelOptional.Visible = true;
                for (int i = 0; i <= 420; i += 20)
                {
                    if (i != 0) PanelOptional.Size = new Size(160, i);
                }

                checkBtnOption = true;
            }
        }

        private void btnData_Click(object sender, EventArgs e)
        {
            //if (checkBtnData)
            //{
            // Thu panelSetup về 0 width
            for (int w = panelSetup.Width; w > 0; w -= 20)
            {
                panelSetup.Width = w;
                panelSetup.Refresh();
            }
            panelSetup.Visible = false;
            checkBtnData = false;

            addUserControl(user_Data);
            //}
            //else
            //{

            //}
        }


        private void btnRTK_GPS_Click(object sender, EventArgs e)
        {

            addUserControl(userRTK);
        }

        private void btnCAN_GPS_Click(object sender, EventArgs e)
        {

            addUserControl(userCan);
        }

        private void btnJoystick_Click(object sender, EventArgs e)
        {

            addUserControl(user_Joystick);
        }

        private void btnComp_Motor_Click(object sender, EventArgs e)
        {

            addUserControl(userComp);
        }

        private void btnRangerFinder_Click(object sender, EventArgs e)
        {

            addUserControl(user_Range_Finder);
        }

        private void btnOpticalFlow_OSD_Click(object sender, EventArgs e)
        {

            addUserControl(user_Optical_Flow_And_OSD);
        }

        private void btnCamGimbal_Click(object sender, EventArgs e)
        {

            addUserControl(user_Camera_Gimbal);
        }

        private void btnMotorTest_Click(object sender, EventArgs e)
        {

            addUserControl(user_Motor_Test);
        }

        private void btnInstallFirmware_Click(object sender, EventArgs e)
        {
            addUserControl(user_Install_Firmware);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = comboBoxPorts.SelectedItem.ToString();
                    serialPort1.BaudRate = int.Parse(comboBoxBaudrate.SelectedItem.ToString());
                    serialPort1.Open();
                    // 1) Gửi HEARTBEAT và khởi động timer
                    StartHeartbeatLoop();

                    // 2) Yêu cầu stream
                    //    Dùng 1 trong 2 cách (hoặc cả hai — nhiều firmware vẫn OK):
                    RequestByMessageIntervals();         // chính xác theo từng message

                    MessageBox.Show($"Đã kết nối {serialPort1.PortName} thành công!");
                }
                else
                {
                    hbTimer.Stop();
                    serialPort1.Close();
                    MessageBox.Show("Ngắt kết nối thành công.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private void RequestByMessageIntervals()
        {
            // Ép sang uint khi truyền
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.ATTITUDE, 20);
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT, 10);
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.VFR_HUD, 10);
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.HIGHRES_IMU, 50);
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT, 10); // ID 24
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.GPS2_RAW, 10);    // ID 124
            SetMessageInterval((uint)MAVLink.MAVLINK_MSG_ID.RC_CHANNELS, 20); // 20 Hz

        }

        private void SetMessageInterval(uint msgId, int hz)
        {
            float intervalUs = hz > 0 ? (1_000_000f / hz) : -1f; // -1: dùng default
            var cmd = new MAVLink.mavlink_command_long_t
            {
                target_system = FcuSysId,
                target_component = FcuCompId,
                command = MAV_CMD_SET_MESSAGE_INTERVAL,  // dùng hằng số ở trên
                confirmation = 0,
                param1 = msgId,       // message id
                param2 = intervalUs,  // microseconds
                                      // các param còn lại để 0
            };

            SendPacketV2(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }
        private void SendPacketV2(MAVLink.MAVLINK_MSG_ID msgId, object payload)
        {
            if (serialPort1 == null || !serialPort1.IsOpen) return;

            // messageType, payload, sign, sysid, compid
            byte[] frame = mavlink.GenerateMAVLinkPacket20(
                msgId,
                payload,
                false,
                GcsSysId,
                GcsCompId);

            serialPort1.Write(frame, 0, frame.Length);
        }
        private void StartHeartbeatLoop()
        {
            // Gửi ngay 1 nhịp
            SendHeartbeat();

            // Lặp 1 Hz
            hbTimer.Stop();
            hbTimer.Interval = 1000;
            hbTimer.Tick -= HbTimer_Tick;
            hbTimer.Tick += HbTimer_Tick;
            hbTimer.Start();
        }

        private void SendHeartbeat()
        {
            var hb = new MAVLink.mavlink_heartbeat_t
            {
                type = (byte)MAVLink.MAV_TYPE.GCS,                // GCS
                autopilot = (byte)MAVLink.MAV_AUTOPILOT.INVALID,  // không phải autopilot
                base_mode = 0,
                custom_mode = 0,
                system_status = (byte)MAVLink.MAV_STATE.ACTIVE,
                mavlink_version = 3
            };
            SendPacketV2(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb);
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            comboBoxPorts.Items.Clear();
            comboBoxPorts.Items.AddRange(SerialPort.GetPortNames());
            if (comboBoxPorts.Items.Count > 0)
                comboBoxPorts.SelectedIndex = 0;
        }

        private void btnFrameType_Click(object sender, EventArgs e)
        {
            addUserControl(user_Frame_Type);
        }

        private void btnAccelCalibration_Click(object sender, EventArgs e)
        {
            addUserControl(user_Accel_Calibration);
        }

        private void btnCompass_Click(object sender, EventArgs e)
        {
            addUserControl(user_Compass);
        }
        private void btnRadioCalib_Click(object sender, EventArgs e)
        {
            addUserControl(user_Radio_Calibration);
        }
        private void btnSetUp_Click(object sender, EventArgs e)
        {
            panelSetup.Visible = true;
            int target = btnInstallFirmware.Width + 5; // width mục tiêu
            for (int w = panelSetup.Width; w < target; w += 40)
            {
                panelSetup.Width = w;
                panelSetup.Refresh();
            }
            panelSetup.Width = target;
            checkBtnData = true;

            addUserControl(user_Install_Firmware);
        }

        private static ushort ClampUs(int v)
        {
            if (v < 0) return 0;          // 0 = kênh không hợp lệ/không dùng (ArduPilot có thể gửi 0)
            if (v < 1000) return 1000;
            if (v > 2000) return 2000;
            return (ushort)v;
        }

        private void btnServoOutput_Click(object sender, EventArgs e)
        {
            addUserControl(user_Servo_Output);
        }

        private void UpdateRcFromChannels(MAVLink.mavlink_rc_channels_t m)
        {
            TransmissionFrame.Rc_Ch1 = ClampUs(m.chan1_raw);
            TransmissionFrame.Rc_Ch2 = ClampUs(m.chan2_raw);
            TransmissionFrame.Rc_Ch3 = ClampUs(m.chan3_raw);
            TransmissionFrame.Rc_Ch4 = ClampUs(m.chan4_raw);
            TransmissionFrame.Rc_Ch5 = ClampUs(m.chan5_raw);
            TransmissionFrame.Rc_Ch6 = ClampUs(m.chan6_raw);
            TransmissionFrame.Rc_Ch7 = ClampUs(m.chan7_raw);
            TransmissionFrame.Rc_Ch8 = ClampUs(m.chan8_raw);
            TransmissionFrame.Rc_Ch9 = ClampUs(m.chan9_raw);
            TransmissionFrame.Rc_Ch10 = ClampUs(m.chan10_raw);
            TransmissionFrame.Rc_Ch11 = ClampUs(m.chan11_raw);
            TransmissionFrame.Rc_Ch12 = ClampUs(m.chan12_raw);
            TransmissionFrame.Rc_Ch13 = ClampUs(m.chan13_raw);
            TransmissionFrame.Rc_Ch14 = ClampUs(m.chan14_raw);
            TransmissionFrame.Rc_Ch15 = ClampUs(m.chan15_raw);
            TransmissionFrame.Rc_Ch16 = ClampUs(m.chan16_raw);
            TransmissionFrame.Rc_RSSI = m.rssi; // 0..255
        }
        public const ushort PREFLIGHT_CALIBRATION = 241;    // MAV_CMD_PREFLIGHT_CALIBRATION
        public const ushort ACCELCAL_VEHICLE_POS = 42429;  // MAV_CMD_ACCELCAL_VEHICLE_POS
        public enum AccelPose : int
        {
            LEVEL = 1,
            LEFT = 2,
            RIGHT = 3,
            NOSEUP = 4,
            NOSEDOWN = 5,
            BACK = 6
        }
        public event Action<string> OnStatusText;
        public event Action<ushort, byte> OnCommandAck;

        private void RaiseStatusText(string txt)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() => OnStatusText?.Invoke(txt)));
        }

        private void RaiseCommandAck(ushort cmd, byte result)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() => OnCommandAck?.Invoke(cmd, result)));
        }


        // Gửi lệnh bắt đầu calib accel (giống MP bấm "Calibrate Accel")
        public void StartAccelCalibration()
        {
            var cmd = new MAVLink.mavlink_command_long_t
            {
                target_system = FcuSysId,
                target_component = FcuCompId,
                command = PREFLIGHT_CALIBRATION,
                confirmation = 0,
                // param5 = 1 -> accel calib
                param1 = 0f,   // gyro
                param2 = 0f,   // mag
                param3 = 0f,   // baro
                param4 = 0f,   // RC
                param5 = 1f,   // accel
                param6 = 0f,
                param7 = 0f
            };

            SendPacketV2(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        // Gửi "Click when DONE" cho tư thế hiện tại
        public void SendAccelCalVehiclePos(AccelPose pose)
        {
            var cmd = new MAVLink.mavlink_command_long_t
            {
                target_system = FcuSysId,
                target_component = FcuCompId,
                command = ACCELCAL_VEHICLE_POS,
                confirmation = 0,
                param1 = (float)pose
            };

            SendPacketV2(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, cmd);
        }

        public event Action<mavlink_param_value_t> OnParamValue;

        private void btnParam_Click(object sender, EventArgs e)
        {
            // Chỉ load metadata 1 lần
            if (!_paramMetaLoaded)
            {
                string pdefFolder = Path.Combine(Application.StartupPath, "Pdef");

                // nếu chưa detect được thì mặc định dùng ArduCopter
                string fileName = _pdefFileName ?? "ArduCopter.apm.pdef.xml";
                string fullPath = Path.Combine(pdefFolder, fileName);

                ParamMetaStore.LoadFromXml(fullPath);
                _paramMetaLoaded = true;
            }

            addUserControl(user_Full_Parameter_List);

            // Sau khi show form thì yêu cầu FCU gửi toàn bộ params
            RequestAllParams();
        }

        private void RaiseParamValue(mavlink_param_value_t p)
        {
            if (!IsHandleCreated || IsDisposed) return;
            BeginInvoke(new Action(() => OnParamValue?.Invoke(p)));
        }
        public void RequestAllParams()
        {
            var req = new mavlink_param_request_list_t
            {
                target_system = FcuSysId,   // đã update khi nhận HEARTBEAT
                target_component = FcuCompId
            };

            SendPacketV2(MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST, req);
        }
        private bool _paramMetaLoaded = false;

        private string _pdefFileName = null;   // sẽ set sau khi nhận HEARTBEAT

        private string GetPdefFileName(MAVLink.mavlink_heartbeat_t hb)
        {
            // Chỉ xử lý ArduPilot
            if (hb.autopilot != (byte)MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA)
                return null;

            var type = (MAVLink.MAV_TYPE)hb.type;

            switch (type)
            {
                // === Plane ===
                case MAVLink.MAV_TYPE.FIXED_WING:
                    return "ArduPlane.apm.pdef.xml";

                // === Rover ===
                case MAVLink.MAV_TYPE.GROUND_ROVER:
                case MAVLink.MAV_TYPE.SURFACE_BOAT:
                    return "Rover.apm.pdef.xml";

                // === Sub ===
                case MAVLink.MAV_TYPE.SUBMARINE:
                    return "ArduSub.apm.pdef.xml";

                // === Blimp ===
                case MAVLink.MAV_TYPE.AIRSHIP:
                    return "Blimp.apm.pdef.xml";

                // === Antenna Tracker ===
                case MAVLink.MAV_TYPE.ANTENNA_TRACKER:
                    return "AntennaTracker.apm.pdef.xml";

                // === Heli ===
                case MAVLink.MAV_TYPE.HELICOPTER:
                    return "Heli.apm.pdef.xml";

                // === AP_Periph (board ngoại vi) ===
                case MAVLink.MAV_TYPE.ONBOARD_CONTROLLER:
                    return "AP_Periph.apm.pdef.xml";

                // === Còn lại: mặc định coi là Copter (quad, hex, octo, tri, coax, v.v.) ===
                default:
                    return "ArduCopter.apm.pdef.xml";
            }
        }

    }
}

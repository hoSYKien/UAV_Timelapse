using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        User_Accel_Calibration user_Accel_Calibration = new User_Accel_Calibration();
        User_Compass user_Compass = new User_Compass();
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
        }
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
                                    heartbeat = (MAVLink.mavlink_heartbeat_t)msg.data;
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        
        private void btnOptional_Click(object sender, EventArgs e)
        {
            //int tmp = PanelOptional.Height;
            if(checkBtnOption)
            {
                for(int i = 420; i > 0; i-=20)
                {
                    if (i != 0) PanelOptional.Size = new Size(160, i);
                }
                PanelOptional.Visible = false;
                checkBtnOption = false;
            }
            else
            {
                PanelOptional.Visible = true;
                for (int i = 0; i <= 420; i+=20)
                {
                    if (i != 0) PanelOptional.Size = new Size(160, i);
                }
                
                checkBtnOption = true;
            }
        }

        // Helper nhỏ để bật double-buffer cho panel setup (giảm flicker)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            panelSetup.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(panelSetup, true, null);
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
            
            addUserControl (user_Camera_Gimbal);
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

        private void StartHeartbeat()
        {
            hbTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            hbTimer.Tick += (s, e) =>
            {
                var hb = new mavlink_heartbeat_t
                {
                    type = (byte)MAV_TYPE.GCS,                 // <-- sửa
                    autopilot = (byte)MAV_AUTOPILOT.INVALID,   // <-- sửa
                    base_mode = 0,
                    custom_mode = 0,
                    system_status = (byte)MAV_STATE.ACTIVE,    // <-- sửa
                    mavlink_version = 3
                };
                SendPacket(MAVLINK_MSG_ID.HEARTBEAT, hb);
            };
            hbTimer.Start();
        }
        private void StopHeartbeat()
        {
            hbTimer?.Stop();
            hbTimer?.Dispose();
            hbTimer = null;
        }


        private const ushort CMD_SET_MESSAGE_INTERVAL = 511;  // MAV_CMD_SET_MESSAGE_INTERVAL

        private void RequestStreams()
        {
            void SetRate(uint msgId, int hz)
            {
                var cmd = new mavlink_command_long_t
                {
                    target_system = 1,
                    target_component = 1,
                    command = CMD_SET_MESSAGE_INTERVAL,  // dùng hằng số thay vì enum
                    confirmation = 0,
                    param1 = msgId,
                    param2 = hz > 0 ? 1_000_000f / hz : -1f
                };
                SendPacket(MAVLINK_MSG_ID.COMMAND_LONG, cmd);
            }

            SetRate(30, 20);  // ATTITUDE
            SetRate(74, 10);  // VFR_HUD
            SetRate(33, 10);  // GLOBAL_POSITION_INT
            SetRate(105, 5);  // HIGHRES_IMU
            SetRate(147, 1);  // BATTERY_STATUS
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

        private void SendPacket(MAVLINK_MSG_ID id, object payload)
        {
            if (!serialPort1.IsOpen) return;
            // Overload trong MAVLink.dll của bạn: (id, payload, bool sign=false)
            byte[] pkt = mavlink.GenerateMAVLinkPacket20(id, payload);
            serialPort1.Write(pkt, 0, pkt.Length);
        }
    }
}

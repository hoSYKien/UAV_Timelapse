using System;

namespace UAV_Timelapse
{
    /// <summary>
    /// Chỉ chứa biến (static fields) nhận từ MAVLink2 theo phong cách Mission Planner.
    /// Đơn vị được ghi chú ở từng nhóm.
    /// </summary>
    public static class TransmissionFrame
    {
        // ===== HEARTBEAT =====
        public static byte Hb_type;               // MAV_TYPE
        public static byte Hb_autopilot;          // MAV_AUTOPILOT
        public static byte Hb_base_mode;          // MAV_MODE_FLAG
        public static uint Hb_custom_mode;        // mode cụ thể
        public static byte Hb_system_status;      // MAV_STATE

        // ===== SYS_STATUS =====
        // Đơn vị: load 0..1000 (tức 0..100.0%), voltage mV, current cA (10*mA), remaining %
        public static ushort Sys_Load;
        public static ushort Sys_Voltage_battery;
        public static short Sys_Current_battery;
        public static sbyte Sys_Battery_remaining;

        // ===== BATTERY_STATUS =====
        // Đơn vị: voltages mV, current cA, remaining %
        public static short Batt_Current_battery;
        public static ushort Batt_Voltages_mv_0;   // kênh 0 (một số FCU dùng mảng 10 phần tử; bạn mở rộng nếu cần)
        public static sbyte Batt_Battery_remaining;

        // ===== GPS_RAW_INT =====
        // Đơn vị: lat/lon 1e-7 deg, alt mm AMSL, eph/epv cm, vel cm/s, cog cdeg
        public static byte Gps_FixType;
        public static byte Gps_SatellitesVisible;
        public static ushort Gps_Eph;
        public static ushort Gps_Epv;
        public static ushort Gps_Vel;
        public static ushort Gps_Cog;
        public static int Gps_Lat;
        public static int Gps_Lon;
        public static int Gps_Alt;

        // ===== GLOBAL_POSITION_INT =====
        // Đơn vị: lat/lon 1e-7 deg, alt/relAlt mm, vx/vy/vz cm/s (NED), hdg cdeg
        public static int Gpi_Lat;
        public static int Gpi_Lon;
        public static int Gpi_Alt;
        public static int Gpi_RelAlt;
        public static short Gpi_Vx;
        public static short Gpi_Vy;
        public static short Gpi_Vz;
        public static ushort Gpi_Hdg;

        // ===== VFR_HUD =====
        // Đơn vị: airspeed m/s, groundspeed m/s, heading deg, throttle %, alt m, climb m/s
        public static float Vfr_Airspeed;
        public static float Vfr_Groundspeed;
        public static short Vfr_Heading;
        public static ushort Vfr_Throttle;
        public static float Vfr_Alt;
        public static float Vfr_Climb;

        // ===== ATTITUDE =====
        // Đơn vị: roll/pitch/yaw rad, rollspeed/pitchspeed/yawspeed rad/s
        public static float Att_Roll;
        public static float Att_Pitch;
        public static float Att_Yaw;
        public static float Att_Rollspeed;
        public static float Att_Pitchspeed;
        public static float Att_Yawspeed;

        // ===== HIGHRES_IMU =====
        // Đơn vị: acc m/s^2, gyro rad/s, mag Gauss, pressure mbar, pressure_alt m, temp degC
        public static float Imu_Xacc;
        public static float Imu_Yacc;
        public static float Imu_Zacc;
        public static float Imu_Xgyro;
        public static float Imu_Ygyro;
        public static float Imu_Zgyro;
        public static float Imu_Xmag;
        public static float Imu_Ymag;
        public static float Imu_Zmag;
        public static float Imu_AbsPressure;
        public static float Imu_DiffPressure;
        public static float Imu_PressureAlt;
        public static float Imu_Temperature;

        // ===== NAV_CONTROLLER_OUTPUT =====
        // Đơn vị: nav_roll/pitch deg, bearing deg, dist m, lỗi alt m, lỗi airspeed m/s, xtrack m
        public static float Nav_NavRoll;
        public static float Nav_NavPitch;
        public static short Nav_AltError;
        public static short Nav_AspdError;
        public static short Nav_XtrackError;
        public static short Nav_NavBearing;
        public static short Nav_TargetBearing;
        public static ushort Nav_WpDist;

        // ===== RC_CHANNELS (1..16) =====
        // Đơn vị: microseconds (1000..2000), RSSI 0..255
        public static ushort Rc_Ch1, Rc_Ch2, Rc_Ch3, Rc_Ch4;
        public static ushort Rc_Ch5, Rc_Ch6, Rc_Ch7, Rc_Ch8;
        public static ushort Rc_Ch9, Rc_Ch10, Rc_Ch11, Rc_Ch12;
        public static ushort Rc_Ch13, Rc_Ch14, Rc_Ch15, Rc_Ch16;
        public static byte Rc_RSSI;

        // ===== HOME_POSITION =====
        // Đơn vị: lat/lon 1e-7 deg, alt mm
        public static int Home_Lat;
        public static int Home_Lon;
        public static int Home_Alt;

        // ===== STATUSTEXT =====
        public static byte Statustext_Severity;    // MAV_SEVERITY
        public static string Statustext_Text;        // chuỗi cuối cùng nhận được

        public static double DistToMav_m;
        // ===== (Tuỳ chọn) EKF_STATUS_REPORT (nếu bạn cần hiển thị) =====
        // public static uint Ekf_Flags;
        // public static float Ekf_VelRatio, Ekf_PosHorizRatio, Ekf_PosVertRatio, Ekf_CompassRatio, Ekf_TerrainAlt, Ekf_ConstPosMode, Ekf_PredHorizPosError, Ekf_PredVertPosError;

    }
}

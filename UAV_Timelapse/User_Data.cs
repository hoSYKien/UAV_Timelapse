using System;
using System.IO;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class User_Data : UserControl
    {
        private AutoScaler _scaler;

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
            timer1.Start();


        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblAltitude.Text = (TransmissionFrame.Gpi_RelAlt / 1000.0).ToString("0.00");

            double gs = TransmissionFrame.Vfr_Groundspeed > 0
                ? TransmissionFrame.Vfr_Groundspeed
                : Math.Sqrt(Math.Pow(TransmissionFrame.Gpi_Vx / 100.0, 2) + Math.Pow(TransmissionFrame.Gpi_Vy / 100.0, 2));
            lblGroundSpeed.Text = gs.ToString("0.00");

            lblDist2WP.Text = TransmissionFrame.Nav_WpDist.ToString();

            double yawDeg = TransmissionFrame.Att_Yaw * 180.0 / Math.PI;
            lblYaw.Text = yawDeg.ToString("0");

            double vs = Math.Abs(TransmissionFrame.Vfr_Climb) > 1e-3
                ? TransmissionFrame.Vfr_Climb
                : -(TransmissionFrame.Gpi_Vz / 100.0);
            lblVerticalSpeed.Text = vs.ToString("0.00");
        }

        private async void User_Data_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();           // OK vì đã async
            webView21.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            var htmlFilePath = Path.Combine(Application.StartupPath, "map.html");
            if (File.Exists(htmlFilePath))
                webView21.Source = new Uri(htmlFilePath);
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            string msg = e.WebMessageAsJson;
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(msg);
            double lat = data.lat;
            double lng = data.lng;

            MessageBox.Show($"Bạn đã nhấn tọa độ:\nLat: {lat}, Lng: {lng}");
        }
    }
}



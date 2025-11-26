using System;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class User_RTK_Base : UserControl
    {
        public User_RTK_Base()
        {
            InitializeComponent();
            InitWebview();
        }
        private async void InitWebview()
        {
            // Khởi tạo WebView2
            await webView21.EnsureCoreWebView2Async(null);

            // (Tuỳ chọn) Bật DevTools để debug
            webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;

            // 👉 Trỏ thẳng tới RTKBase đang chạy trong WSL
            webView21.Source = new Uri("http://192.168.48.129:2048");

            // Nếu sau này cần nhận message từ JS:
            webView21.CoreWebView2.WebMessageReceived += (s, e) =>
            {
                string msgJson = e.WebMessageAsJson;
                MessageBox.Show("JS gửi: " + msgJson);
            };
        }

    }
}

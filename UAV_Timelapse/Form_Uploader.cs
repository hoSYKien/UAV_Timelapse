using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class Form_Uploader : Form
    {
        public Form_Uploader()
        {
            InitializeComponent();
            Load += Form_Uploader_Load;
        }

        private void Form_Uploader_Load(object sender, EventArgs e)
        {
            comboBoxBaudrate.Items.AddRange(new object[] { "57600", "115200", "230400", "460800" });
            comboBoxBaudrate.SelectedIndex = 1; // default 115200
            RefreshPorts();
            Log("Bootloader started...");
            btnSelectFile.Click += BtnSelectFile_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnFlash.Click += BtnFlash_Click;
        }

        // === LOG ===
        private void Log(string msg)
        {
            listBoxStatus.Items.Add($"[{DateTime.Now:HH:mm:ss}] {msg}");
            listBoxStatus.TopIndex = listBoxStatus.Items.Count - 1;
        }

        // === COM PORT ===
        private void RefreshPorts()
        {
            comboBoxPorts.Items.Clear();
            var ports = SerialPort.GetPortNames().OrderBy(p => p).ToArray();
            comboBoxPorts.Items.AddRange(ports);
            if (ports.Length > 0) comboBoxPorts.SelectedIndex = 0;
        }

        private void BtnRefresh_Click(object sender, EventArgs e) => RefreshPorts();

        // === CHỌN FILE ===
        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Waypoints (*.waypoints;*.txt)|*.waypoints;*.txt|All files (*.*)|*.*";
                dlg.Title = "Chọn file waypoint";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBoxFile.Text = dlg.FileName;
                    lblFilePath.Text = Path.GetFileName(dlg.FileName);
                    Log($"Chọn file: {dlg.FileName}");
                }
            }
        }

        // === NẠP WAYPOINT ===
        private async void BtnFlash_Click(object sender, EventArgs e)
        {
            var file = textBoxFile.Text.Trim();
            if (!File.Exists(file))
            {
                MessageBox.Show("Chưa chọn file hợp lệ");
                return;
            }
            if (comboBoxPorts.SelectedItem == null)
            {
                MessageBox.Show("Chưa chọn cổng COM");
                return;
            }
            if (!int.TryParse(comboBoxBaudrate.Text, out int baud))
            {
                MessageBox.Show("Baudrate không hợp lệ");
                return;
            }

            btnFlash.Enabled = false;
            try
            {
                string com = comboBoxPorts.Text;
                Log($"Kết nối {com} @ {baud}...");
                await UploadMissionAsync(file, com, baud);
                Log("Hoàn tất nạp mission!");
            }
            catch (Exception ex)
            {
                Log("Lỗi: " + ex.Message);
            }
            finally
            {
                btnFlash.Enabled = true;
            }
        }

        // === Hàm chính nạp mission (.waypoints) ===
        private async Task UploadMissionAsync(string wplPath, string port, int baud)
        {
            var items = WplParser.Parse(wplPath);
            Log($"Đọc {items.Count} waypoint từ file.");

            using (var sp = new SerialPort(port, baud))
            {
                sp.Open();
                Log("Đang đợi HEARTBEAT từ FC...");

                await Task.Delay(1000); // giả lập delay (sau này thay parser MAVLink thật)
                Log("HEARTBEAT OK!");

                // == Gửi lệnh nạp mission ==
                Log("Gửi lệnh MISSION_CLEAR_ALL");
                await Task.Delay(100);

                Log($"Gửi MISSION_COUNT = {items.Count}");
                await Task.Delay(100);

                int sent = 0;
                foreach (var wp in items)
                {
                    Log($"Gửi MISSION_ITEM_INT {sent + 1}/{items.Count}");
                    await Task.Delay(50); // giả lập delay
                    sent++;
                }

                Log("Nhận MISSION_ACK");
                sp.Close();
            }
        }
    }

    // === CLASS ĐỌC FILE QGC WPL 110 ===
    public class WplItem
    {
        public int idx, current, frame, command, autocontinue;
        public float p1, p2, p3, p4, alt;
        public double lat, lon;
    }

    public static class WplParser
    {
        public static List<WplItem> Parse(string path)
        {
            var lines = File.ReadAllLines(path)
                            .Select(s => s.Trim())
                            .Where(s => s != "" && !s.StartsWith("#"))
                            .ToList();

            if (lines.Count == 0 || !lines[0].Contains("QGC WPL"))
                throw new Exception("Không phải file QGC WPL 110.");

            var items = new List<WplItem>();
            for (int i = 1; i < lines.Count; i++)
            {
                var t = lines[i].Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
                if (t.Length < 12) continue;

                items.Add(new WplItem
                {
                    idx = int.Parse(t[0]),
                    current = int.Parse(t[1]),
                    frame = int.Parse(t[2]),
                    command = int.Parse(t[3]),
                    p1 = float.Parse(t[4], CultureInfo.InvariantCulture),
                    p2 = float.Parse(t[5], CultureInfo.InvariantCulture),
                    p3 = float.Parse(t[6], CultureInfo.InvariantCulture),
                    p4 = float.Parse(t[7], CultureInfo.InvariantCulture),
                    lat = double.Parse(t[8], CultureInfo.InvariantCulture),
                    lon = double.Parse(t[9], CultureInfo.InvariantCulture),
                    alt = float.Parse(t[10], CultureInfo.InvariantCulture),
                    autocontinue = int.Parse(t[11])
                });
            }
            return items;
        }
    }
}

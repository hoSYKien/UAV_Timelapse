using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet;
using LibUsbDotNet.Main;


namespace UAV_Timelapse
{
    public partial class User_Install_Firmware : UserControl
    {
        private string selectedFile = string.Empty;
        private SerialPort serialPort1 = new SerialPort();
        public User_Install_Firmware()
        {
            InitializeComponent();
            serialPort1 = new SerialPort
            {
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            LoadBaudRates();
            Task.Run(() => SafeInvoke(() => btnRefresh_Click(null, null)));
        }

        private void LoadBaudRates()
        {
            comboBoxBaudrate.Items.Clear();
            comboBoxBaudrate.Items.Add("9600");
            comboBoxBaudrate.Items.Add("115200");
            comboBoxBaudrate.Items.Add("230400");
            comboBoxBaudrate.SelectedIndex = 1;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Firmware Files (*.hex;*.bin)|*.hex;*.bin";
                openFileDialog.Title = "Chọn file firmware STM32";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFile = openFileDialog.FileName;
                    lblFilePath.Text = $"Đã chọn: {Path.GetFileName(selectedFile)}";
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                SafeInvoke(() =>
                {
                    comboBoxPorts.Items.Clear();
                    listBoxStatus.Items.Add("🔄 Quét thiết bị...");
                });

                bool dfuDetected = IsDFUDeviceConnected();

                if (dfuDetected)
                {
                    SafeInvoke(() =>
                    {
                        comboBoxPorts.Items.Add("DFU Mode (USB)");
                        comboBoxPorts.SelectedIndex = 0;
                        listBoxStatus.Items.Add("✅ Phát hiện thiết bị ở DFU Mode (USB)");
                    });
                }
                else
                {
                    string[] ports = SerialPort.GetPortNames();

                    SafeInvoke(() =>
                    {
                        if (ports.Length > 0)
                        {
                            comboBoxPorts.Items.AddRange(ports);
                            comboBoxPorts.SelectedIndex = 0;
                            listBoxStatus.Items.Add("🟢 Các cổng COM: " + string.Join(", ", ports));
                        }
                        else
                        {
                            comboBoxPorts.Items.Add("(Không có COM / DFU)");
                            comboBoxPorts.SelectedIndex = 0;
                            listBoxStatus.Items.Add("⚠️ Không tìm thấy DFU hay COM port nào");
                        }
                    });
                }
            });
        }

        private bool IsDFUDeviceConnected()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity"))
                {
                    foreach (var device in searcher.Get())
                    {
                        string name = device["Name"]?.ToString() ?? "";
                        string desc = (device["Description"]?.ToString()) ?? "";

                        string lower = (name + " " + desc).ToLowerInvariant();
                        if (lower.Contains("dfu") || lower.Contains("stm32 bootloader") || lower.Contains("stm device"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SafeInvoke(() => listBoxStatus.Items.Add($"⚠️ Lỗi khi dò thiết bị: {ex.Message}"));
            }
            return false;
        }

        private void btnFlash_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFile) || !File.Exists(selectedFile))
            {
                MessageBox.Show("Vui lòng chọn file firmware (.hex hoặc .bin) trước khi nạp.");
                return;
            }

            string portText = comboBoxPorts.Text ?? "";

            if (portText.ToLower().Contains("dfu"))
            {
                SafeInvoke(() => listBoxStatus.Items.Add("🔌 Chế độ DFU được chọn. Nạp trực tiếp qua USB DFU..."));
                Task.Run(() => FlashViaNativeDFU(selectedFile));
            }
            else if (portText.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            {
                if (comboBoxBaudrate.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn tốc độ truyền (Baudrate)!");
                    return;
                }

                Task.Run(() => FlashViaSerial(portText, int.Parse(comboBoxBaudrate.Text), selectedFile));
            }
            else
            {
                MessageBox.Show("Chưa chọn DFU hay COM hợp lệ. Vui lòng làm mới và chọn lại.");
            }
        }

        private const int STM32_VID = 0x0483;
        private const int STM32_PID = 0xDF11;

        private void FlashViaNativeDFU(string firmwarePath)
        {
            try
            {
                UsbDeviceFinder finder = new UsbDeviceFinder(STM32_VID, STM32_PID);
                UsbDevice usbDevice = UsbDevice.OpenUsbDevice(finder);

                if (usbDevice == null)
                {
                    SafeInvoke(() => listBoxStatus.Items.Add("❌ Không tìm thấy thiết bị DFU STM32."));
                    return;
                }

                IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                if (wholeUsbDevice != null)
                    wholeUsbDevice.SetConfiguration(1);

                UsbSetupPacket setup;
                byte[] data = File.ReadAllBytes(firmwarePath);
                int total = data.Length;
                int sent = 0;
                int block = 2048;
                DateTime startTime = DateTime.Now;

                SafeInvoke(() => listBoxStatus.Items.Add($"📦 Bắt đầu nạp {total} bytes..."));

                while (sent < total)
                {
                    int len = Math.Min(block, total - sent);
                    byte[] chunk = new byte[len];
                    Array.Copy(data, sent, chunk, 0, len);

                    setup = new UsbSetupPacket(
                        (byte)(UsbCtrlFlags.Direction_Out | UsbCtrlFlags.RequestType_Class | UsbCtrlFlags.Recipient_Interface),
                        0x01,  // DFU_DNLOAD
                        (short)(sent / block),
                        0,
                        (short)len
                    );

                    int transferred;
                    if (!usbDevice.ControlTransfer(ref setup, chunk, len, out transferred))
                    {
                        SafeInvoke(() => listBoxStatus.Items.Add($"❌ Lỗi khi gửi block {sent / block}"));
                        break;
                    }

                    sent += len;
                    int percent = (sent * 100) / total;
                    double elapsed = (DateTime.Now - startTime).TotalSeconds;
                    double est = (elapsed / sent) * (total - sent);
                    SafeInvoke(() => listBoxStatus.Items.Add($"📤 Block {sent / block}: {percent}% ({sent}/{total}) - ETA {est:F1}s"));

                    Thread.Sleep(10);
                }

                // DFU_DNLOAD với length = 0 → kết thúc
                setup = new UsbSetupPacket(
                    (byte)(UsbCtrlFlags.Direction_Out | UsbCtrlFlags.RequestType_Class | UsbCtrlFlags.Recipient_Interface),
                    0x01, 0, 0, 0);
                int transferredZero;
                usbDevice.ControlTransfer(ref setup, new byte[0], 0, out transferredZero);

                SafeInvoke(() => listBoxStatus.Items.Add("✅ Đã nạp xong, đang thoát DFU mode."));
                usbDevice.Close();
            }
            catch (Exception ex)
            {
                SafeInvoke(() => listBoxStatus.Items.Add($"❌ Lỗi DFU native: {ex.Message}"));
            }
        }

        private void FlashViaSerial(string portName, int baudRate, string firmwarePath)
        {
            SafeInvoke(() => listBoxStatus.Items.Add($"🔌 Mở cổng {portName} @ {baudRate}"));

            try
            {
                serialPort1.PortName = portName;
                serialPort1.BaudRate = baudRate;

                if (!serialPort1.IsOpen)
                    serialPort1.Open();

                SafeInvoke(() => listBoxStatus.Items.Add("✅ Cổng serial đã mở. (Đang giả lập quá trình nạp...)"));

                for (int i = 0; i <= 100; i += 10)
                {
                    SafeInvoke(() => listBoxStatus.Items.Add($"Đang nạp qua UART... {i}%"));
                    Thread.Sleep(150);
                }

                SafeInvoke(() => listBoxStatus.Items.Add("✅ Nạp qua UART hoàn tất (giả lập)."));
            }
            catch (Exception ex)
            {
                SafeInvoke(() => listBoxStatus.Items.Add($"❌ Lỗi Serial: {ex.Message}"));
            }
            finally
            {
                try
                {
                    if (serialPort1.IsOpen)
                        serialPort1.Close();
                }
                catch { }
            }
        }
        private string FindExecutableInPath(string exeName)
        {
            try
            {
                if (File.Exists(exeName))
                    return Path.GetFullPath(exeName);

                var paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? new string[0];
                foreach (var p in paths)
                {
                    string candidate = Path.Combine(p, exeName);
                    if (File.Exists(candidate))
                        return candidate;

                    if (Path.GetExtension(candidate) == string.Empty)
                    {
                        string candExe = candidate + ".exe";
                        if (File.Exists(candExe))
                            return candExe;
                    }
                }
            }
            catch { }
            return null;
        }

        private void SafeInvoke(Action act)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (this.InvokeRequired)
                    this.Invoke(act);
                else
                    act();
            }
        }
    }
}

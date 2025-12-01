using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class User_Servo_Output : UserControl
    {
        private readonly Form_Main _main;

        private RcBar[] _bars;
        private CheckBox[] _rev;
        private ComboBox[] _func;
        private NumericUpDown[] _min;
        private NumericUpDown[] _trim;
        private NumericUpDown[] _max;

        private List<ServoFuncItem> _funcOptions;
        public User_Servo_Output(Form_Main main)
        {
            InitializeComponent();
            _main = main ?? throw new ArgumentNullException(nameof(main));

            BuildArrays();
            InitBars();
            InitFuncCombos();
            
            //nhận param_value
            _main.OnParamValue += Main_OnParamValue;
        }

        private void Main_OnParamValue(MAVLink.mavlink_param_value_t p)
        {
            // sự kiện này đã được RaiseParamValue() marshal sang UI thread rồi
            string name = Encoding.ASCII.GetString(p.param_id).TrimEnd('\0');

            // Chỉ quan tâm tới SERVOx_FUNCTION
            //if (!name.StartsWith("SERVO", StringComparison.OrdinalIgnoreCase) ||
            //    !name.EndsWith("_FUNCTION", StringComparison.OrdinalIgnoreCase))
            //    return;
            if (!name.StartsWith("SERVO", StringComparison.OrdinalIgnoreCase))
                return;

            // Tách chỉ số x trong SERVOx_FUNCTION
            int start = "SERVO".Length;
            int underscore = name.IndexOf('_', start);
            if (underscore < 0) return;

            string idxStr = name.Substring(start, underscore - start);
            if (!int.TryParse(idxStr, out int servoIndex)) return;
            if (servoIndex < 1 || servoIndex > 16) return;

            int i = servoIndex - 1;   // index trong mảng 0..15
            float fval = p.param_value;
            string suffix = name.Substring(underscore);

            // ===== FUNCTION =====
            if (suffix.Equals("_FUNCTION", StringComparison.OrdinalIgnoreCase))
            {
                int code = (int)fval;
                var cb = _func[i];
                var item = cb.Items.Cast<ServoFuncItem>()
                                   .FirstOrDefault(it => it.Code == code);
                if (item != null)
                    cb.SelectedItem = item;
            }
            // ===== REVERSED =====
            else if (suffix.Equals("_REVERSED", StringComparison.OrdinalIgnoreCase))
            {
                _rev[i].Checked = (Math.Abs(fval) > 0.5f);   // 0 = false, 1 = true
            }
            // ===== MIN =====
            else if (suffix.Equals("_MIN", StringComparison.OrdinalIgnoreCase))
            {
                SetNumericFromFloat(_min[i], fval);
            }
            // ===== TRIM =====
            else if (suffix.Equals("_TRIM", StringComparison.OrdinalIgnoreCase))
            {
                SetNumericFromFloat(_trim[i], fval);
            }
            // ===== MAX =====
            else if (suffix.Equals("_MAX", StringComparison.OrdinalIgnoreCase))
            {
                SetNumericFromFloat(_max[i], fval);
            }
        }
        private void SetNumericFromFloat(NumericUpDown nud, float value)
        {
            decimal v = (decimal)value;
            if (v < nud.Minimum) v = nud.Minimum;
            if (v > nud.Maximum) v = nud.Maximum;
            nud.Value = v;
        }

        #region build arrays
        private void BuildArrays()
        {
            _bars = new[]
            {
            rcbPos1, rcbPos2, rcbPos3, rcbPos4,
            rcbPos5, rcbPos6, rcbPos7, rcbPos8,
            rcbPos9, rcbPos10, rcbPos11, rcbPos12,
            rcbPos13, rcbPos14, rcbPos15, rcbPos16
            };

            _rev = new[]
            {
            chkRev1, chkRev2, chkRev3, chkRev4,
            chkRev5, chkRev6, chkRev7, chkRev8,
            chkRev9, chkRev10, chkRev11, chkRev12,
            chkRev13, chkRev14, chkRev15, chkRev16
            };

            _func = new[]
            {
            cbbFunc1, cbbFunc2, cbbFunc3, cbbFunc4,
            cbbFunc5, cbbFunc6, cbbFunc7, cbbFunc8,
            cbbFunc9, cbbFunc10, cbbFunc11, cbbFunc12,
            cbbFunc13, cbbFunc14, cbbFunc15, cbbFunc16
            };

            _min = new[]
            {
            nudMin1, nudMin2, nudMin3, nudMin4,
            nudMin5, nudMin6, nudMin7, nudMin8,
            nudMin9, nudMin10, nudMin11, nudMin12,
            nudMin13, nudMin14, nudMin15, nudMin16
            };

            _trim = new[]
            {
            nudTrim1, nudTrim2, nudTrim3, nudTrim4,
            nudTrim5, nudTrim6, nudTrim7, nudTrim8,
            nudTrim9, nudTrim10, nudTrim11, nudTrim12,
            nudTrim13, nudTrim14, nudTrim15, nudTrim16
            };

            _max = new[]
            {
            nudMax1, nudMax2, nudMax3, nudMax4,
            nudMax5, nudMax6, nudMax7, nudMax8,
            nudMax9, nudMax10, nudMax11, nudMax12,
            nudMax13, nudMax14, nudMax15, nudMax16
            };
        }
        #endregion

        #region init controls

        private void InitBars()
        {
            foreach (var bar in _bars)
            {
                bar.Minimum = 0;
                bar.Maximum = 2200;
                bar.CenterValue = 1500;
                bar.ShowCenterLine = true;
                bar.ShowText = true;
                bar.TextFormat = "{0}";      // hiển thị "1500" v.v.
            }
        }

        private void InitFuncCombos()
        {
            // đảm bảo đã load pdef
            _main.EnsureParamMetaLoaded();

            _funcOptions = BuildServoFunctionOptions();

            foreach (var cb in _func)
            {
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.DisplayMember = nameof(ServoFuncItem.Text);
                cb.ValueMember = nameof(ServoFuncItem.Code);

                // dùng list clone cho mỗi combobox
                cb.DataSource = new List<ServoFuncItem>(_funcOptions);
            }

            // (bước sau có thể đọc SERVOx_FUNCTION hiện tại rồi chọn SelectedValue)
        }

        private List<ServoFuncItem> BuildServoFunctionOptions()
        {
            // các SERVOx_FUNCTION dùng cùng 1 enum => lấy bất kỳ cái nào
            ParamMeta meta =
                ParamMetaStore.Get("SERVO1_FUNCTION") ??
                ParamMetaStore.Get("SERVO3_FUNCTION") ??
                ParamMetaStore.Get("SERVO10_FUNCTION");

            var list = new List<ServoFuncItem>();

            if (meta == null || string.IsNullOrWhiteSpace(meta.Values))
                return list;

            // meta.Values dạng "0:Disabled,1:RCIN1,33:Motor1,34:Motor2,..."
            var pairs = meta.Values.Split(',');
            foreach (var p in pairs)
            {
                var parts = p.Split(new[] { ':' }, 2);
                if (parts.Length < 2) continue;

                if (!int.TryParse(parts[0].Trim(), out int code))
                    continue;

                string text = parts[1].Trim();
                if (string.IsNullOrEmpty(text))
                    text = code.ToString();

                list.Add(new ServoFuncItem { Code = code, Text = text });
            }

            // sắp xếp cho đẹp: Disabled trước, còn lại theo text
            list = list
                .OrderBy(it => it.Code == 0 ? 0 : 1)
                .ThenBy(it => it.Text)
                .ToList();

            return list;
        }

        #endregion

        #region public API - được gọi từ Form_Main

        /// <summary>
        /// Cập nhật giá trị PWM lên 16 thanh RcBar.
        /// Có thể gọi từ thread SerialPort, hàm tự marshal về UI thread.
        /// </summary>
        public void UpdateServoPwm(ushort[] pwm)
        {
            if (pwm == null) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateServoPwm(pwm)));
                return;
            }

            int n = Math.Min(pwm.Length, _bars.Length);

            for (int i = 0; i < n; i++)
            {
                // func ko dùng xung -> pwm = 0
                var item = _func[i].SelectedItem as ServoFuncItem;
                if (IsNonPwmFunction(item))
                {
                    _bars[i].Value = 0;   // luôn hiển thị 0, thanh rỗng
                    continue;
                }

                // hiển thị pwm rcbar
                ushort v = pwm[i];
                if (v == 0) continue;   // 0 = kênh không dùng

                // clamp nhẹ để tránh value lố range
                if (v < _bars[i].Minimum) v = (ushort)_bars[i].Minimum;
                if (v > _bars[i].Maximum) v = (ushort)_bars[i].Maximum;

                _bars[i].Value = v;
            }
        }

        #endregion

        private bool IsNonPwmFunction(ServoFuncItem item)
        {
            if (item == null) return true;  // coi như không PWM

            // Disabled (code = 0)
            if (item.Code == 0)
                return true;

            string text = item.Text ?? "";

            // NeoPixel (NeoPixel1, NeoPixel2...)
            if (text.StartsWith("NeoPixel", StringComparison.OrdinalIgnoreCase))
                return true;

            // ko xung
            if (text.Contains("relay") || text.Contains("gpio"))
                return true;

            return false;  // các chức năng còn lại hiển thị PWM bình thường
        }

        private void btnWriteParams_Click(object sender, EventArgs e)
        {
            // Gửi SERVO1..SERVO16
            for (int i = 0; i < 16; i++)
            {
                int index = i + 1; // 1..16

                // ===== FUNCTION =====
                var funcItem = _func[i].SelectedItem as ServoFuncItem;
                if (funcItem != null)
                {
                    string pnameFunc = $"SERVO{index}_FUNCTION";
                    _main.SendServoParam(pnameFunc, funcItem.Code);
                }

                // ===== REVERSED =====
                string pnameRev = $"SERVO{index}_REVERSED";
                float revVal = _rev[i].Checked ? 1f : 0f;
                _main.SendServoParam(pnameRev, revVal);

                // ===== MIN =====
                string pnameMin = $"SERVO{index}_MIN";
                _main.SendServoParam(pnameMin, (float)_min[i].Value);

                // ===== TRIM =====
                string pnameTrim = $"SERVO{index}_TRIM";
                _main.SendServoParam(pnameTrim, (float)_trim[i].Value);

                // ===== MAX =====
                string pnameMax = $"SERVO{index}_MAX";
                _main.SendServoParam(pnameMax, (float)_max[i].Value);
            }

            MessageBox.Show(
                "Đã gửi SERVO1..SERVO16 (FUNCTION, REVERSED, MIN, TRIM, MAX) lên Flight Controller.",
                "Servo Output",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    // item cho combobox SERVOx_FUNCTION
    public class ServoFuncItem
    {
        [Browsable(false)]
        public int Code { get; set; }
        public string Text { get; set; }

        public override string ToString() => Text;
    }
}


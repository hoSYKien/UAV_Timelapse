using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class User_Motor_Test : UserControl
    {
        private readonly Form_Main _main;
        private readonly List<Button> _motorButtons = new List<Button>();
        public User_Motor_Test(Form_Main main)
        {
            InitializeComponent();
            _main = main;
        }
        public void ApplyFrame(int frameClass, int frameType)
        {
            lblClass.Text = "Dạng: " + GetFrameClassName(frameClass);
            lblType.Text = "Loại khung: "+ GetFrameTypeName(frameType);

            int motorCount = GetMotorCount(frameClass);
            BuildMotorButtons(motorCount);
        }
        private int GetMotorCount(int frameClass)
        {
            switch (frameClass)
            {
                case 1:  // Quad
                    return 4;
                case 2:  // Hexa
                    return 6;
                case 3:  // Octa
                case 4:  // OctaQuad
                    return 8;
                case 5:  // Y6
                    return 6;
                case 7:  // Heli
                    return 1;
                case 8:  // Tri
                    return 3;
                default:
                    return 4;   // fallback
            }
        }
        // Tên hiển thị
        private string GetFrameClassName(int frameClass)
        {
            switch (frameClass)
            {
                case 1: return "Quad";
                case 2: return "Hexa";
                case 3: return "Octa";
                case 4: return "OctaQuad";
                case 5: return "Y6";
                case 7: return "Heli";
                case 8: return "Tri";
                default: return "Không rõ";
            }
        }
        private string GetFrameTypeName(int frameType)
        {
            // tuỳ bạn map: 0=Plus, 1=X, 2=V, …
            switch (frameType)
            {
                case 0: return "Plus";
                case 1: return "X";
                default: return "Kiểu khác";
            }
        }
        // Tạo nút động cơ
        private void BuildMotorButtons(int motorCount)
        {
            flowMotors.Controls.Clear();
            _motorButtons.Clear();

            for (int i = 1; i <= motorCount; i++)
            {
                var btn = new Button
                {
                    Width = 82,
                    Height = 38,
                    Margin = new Padding(3),
                    Tag = i
                };

                string label = MotorLabel(i); // A, B, C… hoặc số
                btn.BackColor = Color.DarkCyan; 
                btn.Text = $"Kiểm tra động cơ {label}";

                btn.Click += MotorButton_Click;

                flowMotors.Controls.Add(btn);
                _motorButtons.Add(btn);
            }
        }
        private string MotorLabel(int index)
        {
            // 1->A, 2->B, ... 26->Z, >26 dùng số
            if (index >= 1 && index <= 26)
                return ((char)('A' + index - 1)).ToString();
            return index.ToString();
        }

        private void MotorButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            int motorId = (int)btn.Tag;

            float throttle = (float)numThrottle.Value;
            float duration = (float)numDuration.Value;

            _main.MotorTest(motorId, throttle, duration);
        }
        private void TestMotor(int motorId)
        {
            float throttle = (float)numThrottle.Value;   // NumericUpDown %
            float duration = (float)numDuration.Value;   // NumericUpDown s

            _main.MotorTest(motorId, throttle, duration);
        }
        private void btnTestMotorA_Click(object sender, EventArgs e)
        {
            TestMotor(1);
        }

        private void btnTestMotorB_Click(object sender, EventArgs e)
        {
            TestMotor(2);
        }

        private void btnTestMotorC_Click(object sender, EventArgs e)
        {
            TestMotor(3);
        }

        private void btnTestMotorD_Click(object sender, EventArgs e)
        {
            TestMotor(4);
        }

        private void btnTestAllMotor_Click(object sender, EventArgs e)
        {
            float throttle = (float)numThrottle.Value;
            float duration = (float)numDuration.Value;

            // numMotors = số nút hiện có
            for (int i = 1; i <= _motorButtons.Count; i++)
            {
                _main.MotorTest(i, throttle, duration);
            }
            
        }

        private void btnStopAllMotor_Click(object sender, EventArgs e)
        {
            _main.MotorTestStopAll();
        }

        private void btnTestSequenceMotor_Click(object sender, EventArgs e)
        {
            float throttle = (float)numThrottle.Value;
            float duration = (float)numDuration.Value;

            _main.MotorTest(1, throttle, duration, _motorButtons.Count, 1);
        }
    }
}

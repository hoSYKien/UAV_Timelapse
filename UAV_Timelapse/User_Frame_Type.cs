using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MAVLink;

namespace UAV_Timelapse
{
    public partial class User_Frame_Type : UserControl
    {
        private readonly Form_Main _main;
        private bool _suppressEvents = false;

        // lưu giá trị số giống param trên FC
        private int _currentFrameClass;
        private int _currentFrameType;

        // === Màu & state ===
        private readonly Color _normalColor = Color.Gainsboro;
        private readonly Color _selectedColor = SystemColors.GrayText;
        private readonly Color _lblNormal = Color.Gainsboro;
        private readonly Color _lblSelected = SystemColors.GrayText;

        private List<PictureBox> _framePics;

        public enum FrameClass { Undefined, Quad, Hexa, Octa, X8, X6, Tri, Heli }
        public FrameClass SelectedFrame { get; private set; } = FrameClass.Undefined;
        public User_Frame_Type(Form_Main main)
        {
            InitializeComponent();
            _main = main;

            _framePics = new List<PictureBox>
            {
                picUndefined, picQuad, picHexa, picOcta, picX8, picX6, picTri, picHeli
            };

            // map Tag -> enum để dùng sau
            picUndefined.Tag = FrameClass.Undefined;
            picQuad.Tag = FrameClass.Quad;
            picHexa.Tag = FrameClass.Hexa;
            picOcta.Tag = FrameClass.Octa;
            picX8.Tag = FrameClass.X8;
            picX6.Tag = FrameClass.X6;
            picTri.Tag = FrameClass.Tri;
            picHeli.Tag = FrameClass.Heli;

            // gán handler chung
            foreach (var pb in _framePics)
            {
                pb.Cursor = Cursors.Hand;
                pb.BackColor = _normalColor;
                pb.BorderStyle = BorderStyle.None;
                pb.Click += FramePic_Click;
            }

            lblUndefined.BackColor = _lblNormal;

            // handler cho các radio button frame type
            rbtPlus.CheckedChanged += FrameTypeRadio_CheckedChanged;
            rbtXY6A.CheckedChanged += FrameTypeRadio_CheckedChanged;
            rbtV.CheckedChanged += FrameTypeRadio_CheckedChanged;
            rbtVTail.CheckedChanged += FrameTypeRadio_CheckedChanged;
            rbtH.CheckedChanged += FrameTypeRadio_CheckedChanged;
            rbtY6B.CheckedChanged += FrameTypeRadio_CheckedChanged;
            rbtOther.CheckedChanged += FrameTypeRadio_CheckedChanged;

            _main.OnParamValue += Main_OnParamValue;
        }
        // === Hàm dùng chung: chọn 1 picturebox ===
        private void SelectFrame(PictureBox clicked)
        {
            // reset toàn bộ
            foreach (var pb in _framePics)
            {
                pb.BackColor = _normalColor;
                pb.BorderStyle = BorderStyle.None;
            }
            lblUndefined.BackColor = _lblNormal;

            // đánh dấu ô vừa chọn
            clicked.BackColor = _selectedColor;
            clicked.BorderStyle = BorderStyle.FixedSingle;

            // label chỉ đổi khi chọn Undefined
            if (clicked == picUndefined)
                lblUndefined.BackColor = _lblSelected;

            // cập nhật state
            if (clicked.Tag is FrameClass fc)
                SelectedFrame = fc;
        }

        // === Handler chung: chỉ việc gọi SelectFrame ===
        private void FramePic_Click(object sender, EventArgs e)
            => SelectFrame((PictureBox)sender);

        private void picUndefined_Click(object sender, EventArgs e)
        {
            SelectFrame(picUndefined);
            //grbFrameType enable false
        }

        private void picQuad_Click(object sender, EventArgs e)
        {
            SelectFrame(picQuad);
            //rbtY6B enable false
        }

        private void picHexa_Click(object sender, EventArgs e)
        {
            SelectFrame(picHexa);
            //rbt V VTail H Y6B enable false
        }

        private void picOcta_Click(object sender, EventArgs e)
        {
            SelectFrame(picOcta);
            //rbt Y6B enable false
        }

        private void picX8_Click(object sender, EventArgs e)
        {
            SelectFrame(picX8);
            //rbtH 
        }

        private void picX6_Click(object sender, EventArgs e)
        {
            SelectFrame(picX6);
        }

        private void picHeli_Click(object sender, EventArgs e)
        {
            SelectFrame(picHeli);
        }

        private void picTri_Click(object sender, EventArgs e)
        {
            SelectFrame(picTri);
        }
        public void InitializeFromMain()
        {
            // set UI theo giá trị đang lưu trong Form_Main (nếu có)
            _suppressEvents = true;
            SetFrameClassFromValue(_main.FrameClass);
            SetFrameTypeFromValue(_main.FrameType);
            _suppressEvents = false;

            // yêu cầu FC gửi FRAME_CLASS và FRAME_TYPE để sync
            _main.RequestParamByName("FRAME_CLASS");
            _main.RequestParamByName("FRAME_TYPE");
        }
        private void Main_OnParamValue(mavlink_param_value_t p)
        {
            string name = Encoding.ASCII.GetString(p.param_id).TrimEnd('\0');

            if (name == "FRAME_CLASS")
            {
                int v = (int)p.param_value;
                _currentFrameClass = v;
                _main.FrameClass = v;

                SetFrameClassFromValue(v);
                //BeginInvoke(new Action(() => SetFrameClassFromValue(v)));
            }
            else if (name == "FRAME_TYPE")
            {
                int v = (int)p.param_value;
                _currentFrameType = v;
                _main.FrameType = v;

                SetFrameTypeFromValue(v);
                //BeginInvoke(new Action(() => SetFrameTypeFromValue(v)));
            }
        }
        // ArduCopter FRAME_CLASS: 1=Quad,2=Hexa,3=Octa,4=OctaQuad,5=Y6,7=Heli,8=Tri,...
        private void SetFrameClassFromValue(int val)
        {
            PictureBox pb;
            switch (val)
            {
                case 1: pb = picQuad; break;
                case 2: pb = picHexa; break;
                case 3: pb = picOcta; break;
                case 4: pb = picX8; break;
                case 5: pb = picX6; break;  // ví dụ gán Y6 vào X6 hình
                case 7: pb = picHeli; break;
                case 8: pb = picTri; break;
                default: pb = picUndefined; break;
            }

            SelectFrame(pb);
        }

        // FRAME_TYPE: 0=Plus,1=X,2=V,3=VTail,4=H,10=Y6B, ...
        private void SetFrameTypeFromValue(int val)
        {
            _suppressEvents = true;

            rbtPlus.Checked = (val == 0);
            rbtXY6A.Checked = (val == 1);
            rbtV.Checked = (val == 2);
            rbtVTail.Checked = (val == 3);
            rbtH.Checked = (val == 4);
            rbtY6B.Checked = (val == 10);

            if (!rbtPlus.Checked && !rbtXY6A.Checked && !rbtV.Checked &&
                !rbtVTail.Checked && !rbtH.Checked && !rbtY6B.Checked)
            {
                rbtOther.Checked = true;
            }

            _suppressEvents = false;
        }
        private void FrameTypeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            var rb = (RadioButton)sender;
            if (!rb.Checked) return;

            if (rb == rbtPlus) _currentFrameType = 0;
            else if (rb == rbtXY6A) _currentFrameType = 1;
            else if (rb == rbtV) _currentFrameType = 2;
            else if (rb == rbtVTail) _currentFrameType = 3;
            else if (rb == rbtH) _currentFrameType = 4;
            else if (rb == rbtY6B) _currentFrameType = 10;
            else _currentFrameType = 0; // default

            _main.FrameType = _currentFrameType;
        }
        private int GetFrameClassValue()
        {
            switch (SelectedFrame)
            {
                case FrameClass.Quad: return 1;
                case FrameClass.Hexa: return 2;
                case FrameClass.Octa: return 3;
                case FrameClass.X8: return 4;
                case FrameClass.X6: return 5;  // Y6
                case FrameClass.Heli: return 7;
                case FrameClass.Tri: return 8;
                default: return 0;  // Undefined
            }
        }

        private void SaveFrameConfig()
        {
            int classVal = (SelectedFrame == FrameClass.Undefined)
                   ? _currentFrameClass
                   : GetFrameClassValue();   // map từ SelectedFrame -> 0,1,2,...
            int typeVal = _currentFrameType;      // đã set trong FrameTypeRadio_CheckedChanged

            // Lưu vào Form_Main
            _main.FrameClass = classVal;
            _main.FrameType = typeVal;

            // Gửi lên FC (PARAM_SET)
            _main.SendParamSet(new ParamItem
            {
                Name = "FRAME_CLASS",
                Value = classVal,
                MavType = MAV_PARAM_TYPE.INT8
            });

            _main.SendParamSet(new ParamItem
            {
                Name = "FRAME_TYPE",
                Value = typeVal,
                MavType = MAV_PARAM_TYPE.INT8
            });
        }
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            // Tránh gửi param khi chưa kết nối
            SaveFrameConfig();
        }
    }
}

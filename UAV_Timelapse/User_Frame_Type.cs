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

namespace UAV_Timelapse
{
    public partial class User_Frame_Type : UserControl
    {
        // === Màu & state ===
        private readonly Color _normalColor = Color.Gainsboro;
        private readonly Color _selectedColor = SystemColors.GrayText;
        private readonly Color _lblNormal = Color.Gainsboro;
        private readonly Color _lblSelected = SystemColors.GrayText;

        private List<PictureBox> _framePics;

        public enum FrameClass { Undefined, Quad, Hexa, Octa, X8, X6, Tri, Heli }
        public FrameClass SelectedFrame { get; private set; } = FrameClass.Undefined;
        public User_Frame_Type()
        {
            InitializeComponent();

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
    }
}

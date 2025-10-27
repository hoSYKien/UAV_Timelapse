using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public class RcBar : Control
    {
        public RcBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            Height = 20;
        }

        // ====== Properties ======
        private int _min = 1000, _max = 2000, _value = 1000;
        [Category("Behavior")] public int Minimum { get => _min; set { _min = value; if (_max <= _min) _max = _min + 1; Value = _value; Invalidate(); } }
        [Category("Behavior")] public int Maximum { get => _max; set { _max = Math.Max(value, _min + 1); Value = _value; Invalidate(); } }

        private bool _showCenter = true;
        [Category("Appearance")] public bool ShowCenterLine { get => _showCenter; set { _showCenter = value; Invalidate(); } }

        private int _center = 1500;
        [Category("Behavior")] public int CenterValue { get => _center; set { _center = value; Invalidate(); } }

        private Color _fill = Color.ForestGreen;
        [Category("Appearance")] public Color FillColor { get => _fill; set { _fill = value; Invalidate(); } }

        private Color _border = Color.Silver;
        [Category("Appearance")] public Color BorderColor { get => _border; set { _border = value; Invalidate(); } }

        private Color _centerColor = Color.LightGray;
        [Category("Appearance")] public Color CenterLineColor { get => _centerColor; set { _centerColor = value; Invalidate(); } }

        private bool _showText = false;
        [Category("Appearance")] public bool ShowText { get => _showText; set { _showText = value; Invalidate(); } }

        private string _textFmt = "{0}";
        [Category("Appearance")] public string TextFormat { get => _textFmt; set { _textFmt = value ?? "{0}"; Invalidate(); } }

        [Browsable(false)]
        public int Value
        {
            get => _value;
            set
            {
                int v = Math.Max(_min, Math.Min(_max, value));
                if (_value == v) return;
                _value = v;
                Invalidate();           // vẽ lại tức thì, không animation
            }
        }

        // ====== Painting ======
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // nền
            using (var back = new SolidBrush(BackColor))
                g.FillRectangle(back, ClientRectangle);

            // phần đầy
            float ratio = (_value - _min) / (float)(_max - _min);
            int w = (int)Math.Round((ClientSize.Width - 2) * ratio);
            using (var fill = new SolidBrush(_fill))
                g.FillRectangle(fill, 1, 1, Math.Max(0, w), ClientSize.Height - 2);

            // vạch giữa (tùy chọn)
            if (_showCenter && _center > _min && _center < _max)
            {
                float cxRatio = (_center - _min) / (float)(_max - _min);
                int cx = 1 + (int)Math.Round((ClientSize.Width - 2) * cxRatio);
                using (var pen = new Pen(_centerColor))
                    g.DrawLine(pen, cx, 1, cx, ClientSize.Height - 2);
            }

            // viền
            using (var pen = new Pen(_border))
                g.DrawRectangle(pen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

            // text (tùy chọn)
            if (_showText)
            {
                string txt = string.Format(_textFmt, _value);
                TextRenderer.DrawText(g, txt, Font, ClientRectangle, ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis);
            }
        }
    }
}

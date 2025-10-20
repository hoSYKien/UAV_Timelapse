using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public sealed class AutoScaler
    {
        private readonly Control _root;
        private Size _baseSize;
        private readonly Dictionary<Control, Rectangle> _baseBounds = new Dictionary<Control, Rectangle>();
        private readonly Dictionary<Control, float> _baseFont = new Dictionary<Control, float>();
        private bool _initialized;

        // Nếu muốn bỏ qua các control có Tag="noscale"
        private static bool Skip(Control c) =>
            (c.Tag is string s) && s.Equals("noscale", StringComparison.OrdinalIgnoreCase);

        public AutoScaler(Control root) => _root = root ?? throw new ArgumentNullException(nameof(root));

        /// Gọi sau khi InitializeComponent (ví dụ ở Load)
        public void Capture()
        {
            if (_initialized) return;
            _baseSize = _root.Size;
            _baseBounds.Clear();
            _baseFont.Clear();

            foreach (var c in GetAllControls(_root))
            {
                if (Skip(c)) continue;
                _baseBounds[c] = c.Bounds;
                _baseFont[c] = c.Font.SizeInPoints;
            }
            _initialized = true;
        }

        /// Gọi trong sự kiện Resize của root
        public void Apply()
        {
            if (!_initialized) return;
            if (_baseSize.Width <= 0 || _baseSize.Height <= 0) return;

            float sx = (float)_root.Width / _baseSize.Width;
            float sy = (float)_root.Height / _baseSize.Height;
            float sFont = Math.Min(sx, sy);

            _root.SuspendLayout();
            try
            {
                foreach (var kv in _baseBounds)
                {
                    var c = kv.Key;
                    if (c.IsDisposed) continue;

                    var r = kv.Value;
                    int x = (int)Math.Round(r.X * sx);
                    int y = (int)Math.Round(r.Y * sy);
                    int w = (int)Math.Round(r.Width * sx);
                    int h = (int)Math.Round(r.Height * sy);
                    c.SetBounds(x, y, w, h);

                    float baseSize = _baseFont[c];
                    float newSize = Math.Max(6f, baseSize * sFont); // tránh quá nhỏ
                    if (Math.Abs(c.Font.SizeInPoints - newSize) > 0.25f)
                        c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                }
            }
            finally
            {
                _root.ResumeLayout();
            }
        }

        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (var child in GetAllControls(c))
                    yield return child;
            }
        }
    }
}


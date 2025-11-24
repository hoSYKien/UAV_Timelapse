using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class HudControl : UserControl
    {
        // ===== Public telemetry properties =====
        [Browsable(false)] public float RollDeg { get; set; }      // +right
        [Browsable(false)] public float PitchDeg { get; set; }     // +nose up
        [Browsable(false)] public float YawDeg { get; set; }       // 0..360
        [Browsable(false)] public float Airspeed { get; set; }     // m/s
        [Browsable(false)] public float Groundspeed { get; set; }  // m/s
        [Browsable(false)] public float Altitude { get; set; }     // m AMSL or relative
        [Browsable(false)] public float Climb { get; set; }        // m/s
        [Browsable(false)] public int ThrottlePct { get; set; }  // 0..100
        [Browsable(false)] public string ModeText { get; set; } = "Stabilize";
        [Browsable(false)] public bool Armed { get; set; } = false;
        [Browsable(false)] public string GpsText { get; set; } = "No GPS";
        [Browsable(false)] public string BattText { get; set; } = "Bat --";

        // tuning
        private const float PitchPixelsPerDeg = 4.0f;  // đổi pitch->px (tăng nếu muốn nhạy hơn)
        private const int LadderMaxDeg = 30;

        public HudControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(25, 25, 25));

            var rect = ClientRectangle;
            if (rect.Width <= 10 || rect.Height <= 10) return;

            // khu vực “PFD”
            var pfd = Rectangle.Inflate(rect, -10, -10);
            var cx = pfd.Left + pfd.Width / 2f;
            var cy = pfd.Top + pfd.Height / 2f;

            // ====== Artificial Horizon (sky/ground) ======
            // cần có: using System.Drawing.Drawing2D;
            GraphicsState state = g.Save();
            try
            {
                g.TranslateTransform(cx, cy);
                g.RotateTransform(RollDeg);                         // nghiêng theo roll
                g.TranslateTransform(0, PitchDeg * PitchPixelsPerDeg); // tịnh tiến theo pitch

                // vẽ nền trời/đất
                var sky = new RectangleF(-pfd.Width, -pfd.Height * 2, pfd.Width * 2, pfd.Height * 2);
                var ground = new RectangleF(-pfd.Width, 0, pfd.Width * 2, pfd.Height * 2);

                using (var skyBrush = new SolidBrush(Color.FromArgb(80, 150, 220)))
                using (var grdBrush = new SolidBrush(Color.FromArgb(115, 85, 60)))
                using (var pen = new Pen(Color.White, 2))
                using (var tickPen = new Pen(Color.White, 1.5f))
                using (var txtBr = new SolidBrush(Color.White))
                using (var f = new Font(Font.FontFamily, 8f, FontStyle.Bold))
                {
                    g.FillRectangle(skyBrush, sky);
                    g.FillRectangle(grdBrush, ground);
                    g.DrawLine(pen, -pfd.Width, 0, pfd.Width, 0);

                    for (int deg = -30; deg <= 30; deg += 5)
                    {
                        if (deg == 0) continue;
                        float y = -deg * PitchPixelsPerDeg;
                        int half = (deg % 10 == 0) ? 35 : 20;
                        g.DrawLine(tickPen, -half, y, half, y);
                        if (deg % 10 == 0)
                        {
                            string t = Math.Abs(deg).ToString();
                            DrawTextCentered(g, f, txtBr, t, -half - 15, y);
                            DrawTextCentered(g, f, txtBr, t, half + 15, y);
                        }
                    }
                }
            }
            finally
            {
                g.Restore(state);
            }


            // ====== vòng bank marks ======
            DrawBankScale(g, new PointF(cx, cy), Math.Min(pfd.Width, pfd.Height) * 0.9f);

            // ====== flight director / center reference ======
            using (var pen = new Pen(Color.Red, 2))
            {
                g.DrawLine(pen, cx - 20, cy, cx - 5, cy);
                g.DrawLine(pen, cx + 5, cy, cx + 20, cy);
                g.DrawLine(pen, cx, cy - 5, cx, cy + 5);
            }

            // ====== Speed & Alt tapes ======
            DrawSpeedTape(g, new Rectangle(pfd.Left, pfd.Top, 70, pfd.Height));
            DrawAltTape(g, new Rectangle(pfd.Right - 70, pfd.Top, 70, pfd.Height));
            DrawHeading(g, new Rectangle(pfd.Left, pfd.Top - 26, pfd.Width, 22));

            // ====== Footer/status ======
            //DrawStatusBar(g, new Rectangle(pfd.Left, pfd.Bottom + 4, pfd.Width, 22));
            DrawStatusBar(g, new Rectangle(pfd.Left, pfd.Bottom - 22, pfd.Width, 22));
        }

        private void DrawBankScale(Graphics g, PointF c, float diameter)
        {
            float r = diameter / 2f;
            using (var pen = new Pen(Color.White, 2))
            {
                var arcRect = new RectangleF(c.X - r, c.Y - r, diameter, diameter);
                g.DrawArc(pen, arcRect, 210, 120); // vòng cung trên

                // vạch 10°/20°/30°/45°/60°
                int[] marks = { 10, 20, 30, 45, 60 };
                foreach (var deg in marks)
                {
                    float rad = (float)((-90 + deg) * Math.PI / 180.0);
                    float rad2 = (float)((-90 - deg) * Math.PI / 180.0);
                    DrawBankTick(g, c, r, rad);
                    DrawBankTick(g, c, r, rad2);
                }
                // bug marker theo Roll
                using (var red = new Pen(Color.Red, 2))
                {
                    float ang = (float)((-90 + RollDeg) * Math.PI / 180.0);
                    var p1 = new PointF(
                        c.X + (float)Math.Cos(ang) * (r - 5),
                        c.Y + (float)Math.Sin(ang) * (r - 5));
                    var p2 = new PointF(
                        c.X + (float)Math.Cos(ang) * (r + 10),
                        c.Y + (float)Math.Sin(ang) * (r + 10));
                    g.DrawLine(red, p1, p2);
                }
            }
        }
        private void DrawBankTick(Graphics g, PointF c, float r, float ang)
        {
            var p1 = new PointF(c.X + (float)Math.Cos(ang) * (r - 8),
                                c.Y + (float)Math.Sin(ang) * (r - 8));
            var p2 = new PointF(c.X + (float)Math.Cos(ang) * (r + 8),
                                c.Y + (float)Math.Sin(ang) * (r + 8));
            g.DrawLine(Pens.White, p1, p2);
        }

        private void DrawSpeedTape(Graphics g, Rectangle area)
        {
            using (var br = new SolidBrush(Color.FromArgb(30, 30, 30)))
            using (var pen = new Pen(Color.White, 1.5f))
            using (var f = new Font(Font.FontFamily, 10f, FontStyle.Bold))
            using (var txt = new SolidBrush(Color.White))
            {
                g.FillRectangle(br, area);
                g.DrawRectangle(pen, area);

                // cửa sổ giá trị chính giữa
                var win = new Rectangle(area.Left + 5, area.Top + area.Height / 2 - 14, area.Width - 10, 28);
                g.DrawRectangle(pen, win);
                DrawTextCentered(g, f, txt, $"{Groundspeed:0.0} m/s", win.Left + win.Width / 2, win.Top + win.Height / 2);

                // vạch lân cận mỗi 5 m/s
                float baseVal = (float)Math.Round(Groundspeed / 5f) * 5f;
                for (int k = -3; k <= 3; k++)
                {
                    float v = baseVal + k * 5f;
                    int dy = k * 20;
                    var y = win.Top + win.Height / 2 - dy;
                    g.DrawLine(pen, area.Right - 25, y, area.Right - 5, y);
                    var s = v.ToString("0");
                    g.DrawString(s, this.Font, txt, area.Left + 6, y - 8);
                }
            }
        }

        private void DrawAltTape(Graphics g, Rectangle area)
        {
            using (var br = new SolidBrush(Color.FromArgb(30, 30, 30)))
            using (var pen = new Pen(Color.White, 1.5f))
            using (var f = new Font(Font.FontFamily, 10f, FontStyle.Bold))
            using (var txt = new SolidBrush(Color.White))
            {
                g.FillRectangle(br, area);
                g.DrawRectangle(pen, area);

                var win = new Rectangle(area.Left + 5, area.Top + area.Height / 2 - 14, area.Width - 10, 28);
                g.DrawRectangle(pen, win);
                DrawTextCentered(g, f, txt, $"{Altitude:0.0} m", win.Left + win.Width / 2, win.Top + win.Height / 2);

                float baseVal = (float)Math.Round(Altitude / 10f) * 10f;
                for (int k = -3; k <= 3; k++)
                {
                    float v = baseVal + k * 10f;
                    int dy = k * 20;
                    var y = win.Top + win.Height / 2 - dy;
                    g.DrawLine(pen, area.Left + 5, y, area.Left + 25, y);
                    var s = v.ToString("0");
                    var sz = g.MeasureString(s, this.Font);
                    g.DrawString(s, this.Font, txt, area.Right - sz.Width - 6, y - 8);
                }
            }
        }

        private void DrawHeading(Graphics g, Rectangle area)
        {
            using (var br = new SolidBrush(Color.FromArgb(30, 30, 30)))
            using (var pen = new Pen(Color.White, 1.5f))
            using (var txt = new SolidBrush(Color.White))
            using (var f = new Font(Font.FontFamily, 10f, FontStyle.Bold))
            {
                g.FillRectangle(br, area);
                g.DrawRectangle(pen, area);

                // thước 0..360 (đơn giản quanh yaw)
                float center = YawDeg;
                for (int k = -6; k <= 6; k++)
                {
                    float hdg = (center + k * 5f + 360f) % 360f;
                    int x = area.Left + area.Width / 2 + k * 20;
                    if (x < area.Left || x > area.Right) continue;
                    int tick = (Math.Abs(k) % 2 == 0) ? 10 : 5;
                    g.DrawLine(pen, x, area.Bottom - tick, x, area.Bottom);

                    if (k % 2 == 0)
                    {
                        var label = ((int)Math.Round(hdg)).ToString("000");
                        var sz = g.MeasureString(label, f);
                        g.DrawString(label, f, txt, x - sz.Width / 2, area.Top + 2);
                    }
                }

                // caret giữa
                using (var red = new Pen(Color.Red, 2))
                {
                    int cx = area.Left + area.Width / 2;
                    g.DrawLine(red, cx, area.Bottom - 14, cx, area.Bottom);
                }
            }
        }

        private void DrawStatusBar(Graphics g, Rectangle area)
        {
            using (var br = new SolidBrush(Color.FromArgb(30, 30, 30)))
            using (var txtGreen = new SolidBrush(Color.Lime))
            using (var txtRed = new SolidBrush(Color.Red))
            using (var txtYellow = new SolidBrush(Color.Gold))
            using (var pen = new Pen(Color.White, 1))
            using (var f = new Font(Font.FontFamily, 9f, FontStyle.Bold))
            {
                g.FillRectangle(br, area);
                g.DrawRectangle(pen, area);

                // trái: pin
                g.DrawString(BattText, f, txtYellow, area.Left + 6, area.Top + 3);

                // giữa: armed/mode
                string center = Armed ? "ARMED" : "DISARMED";
                var b = Armed ? txtGreen : txtRed;
                var sz = g.MeasureString(center, f);
                g.DrawString(center, f, b,
                    area.Left + area.Width / 2 - sz.Width / 2, area.Top + 3);

                // phải: GPS + mode
                string right = $"GPS: {GpsText}   {ModeText}";
                var szr = g.MeasureString(right, f);
                g.DrawString(right, f, Brushes.White,
                    area.Right - szr.Width - 6, area.Top + 3);
            }
        }

        private static void DrawTextCentered(Graphics g, Font f, Brush br, string s, float x, float y)
        {
            var sz = g.MeasureString(s, f);
            g.DrawString(s, f, br, x - sz.Width / 2f, y - sz.Height / 2f);
        }
    }
}

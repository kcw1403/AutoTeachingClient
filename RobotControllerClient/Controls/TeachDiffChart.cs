using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using RobotControllerClient.Protocol;

namespace RobotControllerClient.Controls
{
    public class TeachDiffChart : Control
    {
        private static readonly Color BgColor = Color.FromArgb(24, 26, 31);
        private static readonly Color PanelColor = Color.FromArgb(32, 35, 42);
        private static readonly Color GridColor = Color.FromArgb(52, 56, 66);
        private static readonly Color TextColor = Color.FromArgb(214, 219, 228);
        private static readonly Color MutedColor = Color.FromArgb(138, 146, 160);
        private static readonly Color OriginColor = Color.FromArgb(120, 132, 150);
        private static readonly Color TeachColor = Color.FromArgb(64, 158, 255);
        private static readonly Color DevPosColor = Color.FromArgb(240, 90, 90);
        private static readonly Color DevNegColor = Color.FromArgb(88, 196, 132);

        private readonly string[] _axisLabels = { "X", "Y", "Z", "Yaw", "Pitch", "Roll" };
        private readonly string[] _axisUnits = { "mm", "mm", "mm", "deg", "deg", "deg" };

        private TeachDiffer _data;
        private DateTime _updatedAt;

        public TeachDiffChart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint, true);
            BackColor = BgColor;
            ForeColor = TextColor;
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(260, 320);
        }

        public void SetData(TeachDiffer data)
        {
            _data = data;
            _updatedAt = DateTime.Now;
            Invalidate();
        }

        public void Clear()
        {
            _data = null;
            Invalidate();
        }

        private double[] Origins()
        {
            return new[]
            {
                _data.Origin.X, _data.Origin.Y, _data.Origin.Z,
                _data.Origin.Yaw, _data.Origin.Pitch, _data.Origin.Roll
            };
        }

        private double[] Currents()
        {
            return new[]
            {
                _data.Current.X, _data.Current.Y, _data.Current.Z,
                _data.Current.Yaw, _data.Current.Pitch, _data.Current.Roll
            };
        }

        private double[] Deviations()
        {
            return new[]
            {
                _data.DeviationX, _data.DeviationY, _data.DeviationZ,
                _data.DeviationYaw, _data.DeviationPitch, _data.DeviationRoll
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(BgColor);

            int pad = 14;
            Rectangle content = new Rectangle(pad, pad, Width - pad * 2, Height - pad * 2);

            DrawHeader(g, ref content);

            if (_data == null)
            {
                DrawEmpty(g, content);
                return;
            }

            DrawLegend(g, ref content);
            DrawAxisRows(g, content);
        }

        private void DrawHeader(Graphics g, ref Rectangle content)
        {
            using (Font titleFont = new Font("Segoe UI Semibold", 11F, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(TextColor))
            {
                g.DrawString("Auto Teaching 편차", titleFont, titleBrush, content.Left, content.Top);
            }

            string sub = _data == null
                ? "WAFEREYETEACH 응답 대기 중"
                : string.Format("갱신: {0:HH:mm:ss}", _updatedAt);
            using (SolidBrush subBrush = new SolidBrush(MutedColor))
            {
                g.DrawString(sub, Font, subBrush, content.Left, content.Top + 22);
            }

            int used = 46;
            content = new Rectangle(content.Left, content.Top + used, content.Width, content.Height - used);
        }

        private void DrawEmpty(Graphics g, Rectangle content)
        {
            using (SolidBrush brush = new SolidBrush(MutedColor))
            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(
                    "WAFEREYETEACH 명령을 실행하면\n원래 위치 · 티칭 위치 · 편차가\n여기에 표시됩니다.",
                    Font, brush, content, sf);
            }
        }

        private void DrawLegend(Graphics g, ref Rectangle content)
        {
            int y = content.Top;
            int x = content.Left;
            x = DrawLegendItem(g, x, y, OriginColor, "원래 위치");
            x = DrawLegendItem(g, x + 14, y, TeachColor, "티칭 위치");
            DrawLegendItem(g, x + 14, y, DevPosColor, "편차");

            int used = 26;
            content = new Rectangle(content.Left, content.Top + used, content.Width, content.Height - used);
        }

        private int DrawLegendItem(Graphics g, int x, int y, Color color, string label)
        {
            int box = 11;
            using (SolidBrush b = new SolidBrush(color))
            {
                g.FillRectangle(b, x, y + 1, box, box);
            }
            using (SolidBrush t = new SolidBrush(MutedColor))
            {
                g.DrawString(label, Font, t, x + box + 4, y - 1);
            }
            SizeF size = g.MeasureString(label, Font);
            return x + box + 4 + (int)size.Width;
        }

        private void DrawAxisRows(Graphics g, Rectangle content)
        {
            int count = _axisLabels.Length;
            double[] origins = Origins();
            double[] currents = Currents();
            double[] deviations = Deviations();

            int rowGap = 10;
            int rowHeight = (content.Height - rowGap * (count - 1)) / count;
            if (rowHeight < 40)
            {
                rowHeight = 40;
            }

            for (int i = 0; i < count; i++)
            {
                Rectangle row = new Rectangle(
                    content.Left,
                    content.Top + i * (rowHeight + rowGap),
                    content.Width,
                    rowHeight);
                if (row.Bottom > content.Bottom + rowHeight)
                {
                    break;
                }
                DrawAxisRow(g, row, _axisLabels[i], _axisUnits[i], origins[i], currents[i], deviations[i]);
            }
        }

        private void DrawAxisRow(Graphics g, Rectangle row, string label, string unit,
            double origin, double current, double deviation)
        {
            using (SolidBrush panel = new SolidBrush(PanelColor))
            {
                FillRoundedRect(g, panel, row, 6);
            }

            int inner = 10;
            Rectangle body = new Rectangle(row.Left + inner, row.Top + inner,
                row.Width - inner * 2, row.Height - inner * 2);

            using (Font axisFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold))
            using (SolidBrush axisBrush = new SolidBrush(TextColor))
            {
                g.DrawString(label, axisFont, axisBrush, body.Left, body.Top);
            }

            string devText = string.Format(CultureInfo.InvariantCulture,
                "Δ {0:+0.000;-0.000;0.000} {1}", deviation, unit);
            Color devColor = Math.Abs(deviation) < 1e-9
                ? MutedColor
                : (deviation > 0 ? DevPosColor : DevNegColor);
            using (Font devFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold))
            using (SolidBrush devBrush = new SolidBrush(devColor))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Far })
            {
                g.DrawString(devText, devFont, devBrush,
                    new RectangleF(body.Left, body.Top, body.Width, 18), sf);
            }

            int barTop = body.Top + 22;
            int barAreaH = body.Bottom - barTop;
            int barH = Math.Max(7, (barAreaH - 6) / 2);

            double scale = Math.Max(Math.Abs(origin), Math.Abs(current));
            if (scale < 1e-9)
            {
                scale = 1.0;
            }

            Rectangle track1 = new Rectangle(body.Left, barTop, body.Width, barH);
            Rectangle track2 = new Rectangle(body.Left, barTop + barH + 6, body.Width, barH);

            DrawValueBar(g, track1, origin, scale, OriginColor, unit);
            DrawValueBar(g, track2, current, scale, TeachColor, unit);
        }

        private void DrawValueBar(Graphics g, Rectangle track, double value, double scale, Color color, string unit)
        {
            using (SolidBrush trackBrush = new SolidBrush(GridColor))
            {
                FillRoundedRect(g, trackBrush, track, track.Height / 2);
            }

            int mid = track.Left + track.Width / 2;
            using (Pen midPen = new Pen(Color.FromArgb(90, 96, 110)))
            {
                g.DrawLine(midPen, mid, track.Top, mid, track.Bottom);
            }

            double ratio = value / scale;
            if (ratio > 1) ratio = 1;
            if (ratio < -1) ratio = -1;
            int half = track.Width / 2 - 2;
            int barLen = (int)Math.Round(Math.Abs(ratio) * half);

            Rectangle bar;
            if (value >= 0)
            {
                bar = new Rectangle(mid, track.Top, barLen, track.Height);
            }
            else
            {
                bar = new Rectangle(mid - barLen, track.Top, barLen, track.Height);
            }
            if (bar.Width > 0)
            {
                using (SolidBrush b = new SolidBrush(color))
                {
                    FillRoundedRect(g, b, bar, track.Height / 2);
                }
            }

            string valText = string.Format(CultureInfo.InvariantCulture, "{0:0.000} {1}", value, unit);
            using (SolidBrush t = new SolidBrush(TextColor))
            using (StringFormat sf = new StringFormat
            {
                Alignment = value >= 0 ? StringAlignment.Near : StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle labelRect = value >= 0
                    ? new Rectangle(track.Left + 4, track.Top, track.Width / 2 - 6, track.Height)
                    : new Rectangle(track.Left + track.Width / 2 + 2, track.Top, track.Width / 2 - 6, track.Height);
                g.DrawString(valText, Font, t, labelRect, sf);
            }
        }

        private static void FillRoundedRect(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    g.FillRectangle(brush, rect);
                }
                return;
            }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }
    }
}

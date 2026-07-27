using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using RobotControllerClient.Protocol;

namespace RobotControllerClient.Controls
{
    public enum TrendMetric
    {
        X,
        Y,
        Z,
        Yaw,
        Pitch,
        Roll
    }

    public class TeachTrendChart : Control
    {
        private static readonly Color BgColor = Color.FromArgb(24, 26, 31);
        private static readonly Color PlotColor = Color.FromArgb(30, 33, 40);
        private static readonly Color GridColor = Color.FromArgb(48, 52, 62);
        private static readonly Color AxisColor = Color.FromArgb(90, 96, 110);
        private static readonly Color TextColor = Color.FromArgb(214, 219, 228);
        private static readonly Color MutedColor = Color.FromArgb(138, 146, 160);
        private static readonly Color ZeroColor = Color.FromArgb(120, 128, 142);
        private static readonly Color CurrentColor = Color.FromArgb(255, 196, 64);

        private static readonly Color[] MetricColors =
        {
            Color.FromArgb(64, 158, 255),
            Color.FromArgb(88, 196, 132),
            Color.FromArgb(240, 90, 90),
            Color.FromArgb(180, 140, 255),
            Color.FromArgb(255, 150, 80),
            Color.FromArgb(96, 208, 214)
        };

        private static readonly string[] MetricLabels = { "X", "Y", "Z", "Yaw", "Pitch", "Roll" };
        private static readonly string[] MetricUnits = { "mm", "mm", "mm", "deg", "deg", "deg" };

        private readonly bool[] _visible = { true, true, true, false, false, false };
        private List<TeachDiffRecord> _records = new List<TeachDiffRecord>();
        private int _station = -1;

        public TeachTrendChart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint, true);
            BackColor = BgColor;
            ForeColor = TextColor;
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(360, 260);
        }

        public void SetRecords(int station, List<TeachDiffRecord> records)
        {
            _station = station;
            _records = records ?? new List<TeachDiffRecord>();
            Invalidate();
        }

        public void SetMetricVisible(TrendMetric metric, bool visible)
        {
            _visible[(int)metric] = visible;
            Invalidate();
        }

        public bool IsMetricVisible(TrendMetric metric)
        {
            return _visible[(int)metric];
        }

        public static Color MetricColor(TrendMetric metric)
        {
            return MetricColors[(int)metric];
        }

        private static double Deviation(TeachDiffRecord r, int metric)
        {
            switch (metric)
            {
                case 0: return r.DeviationX;
                case 1: return r.DeviationY;
                case 2: return r.DeviationZ;
                case 3: return r.DeviationYaw;
                case 4: return r.DeviationPitch;
                default: return r.DeviationRoll;
            }
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
            DrawLegend(g, ref content);

            if (_records.Count == 0)
            {
                DrawEmpty(g, content);
                return;
            }

            int marginLeft = 58;
            int marginBottom = 40;
            Rectangle plot = new Rectangle(
                content.Left + marginLeft,
                content.Top,
                content.Width - marginLeft,
                content.Height - marginBottom);
            if (plot.Width < 40 || plot.Height < 40)
            {
                return;
            }

            using (SolidBrush plotBrush = new SolidBrush(PlotColor))
            {
                g.FillRectangle(plotBrush, plot);
            }

            double min, max;
            ComputeRange(out min, out max);
            DrawGridAndAxis(g, plot, min, max);
            DrawSeries(g, plot, min, max);
            DrawXLabels(g, plot);
        }

        private void DrawHeader(Graphics g, ref Rectangle content)
        {
            using (Font titleFont = new Font("Segoe UI Semibold", 11F, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(TextColor))
            {
                string title = _station >= 0
                    ? string.Format("Stage {0} 편차 트렌드", _station)
                    : "편차 트렌드";
                g.DrawString(title, titleFont, titleBrush, content.Left, content.Top);
            }

            string sub = _records.Count == 0
                ? "이력 없음"
                : string.Format("이력 {0}건 · 최근값 강조", _records.Count);
            using (SolidBrush subBrush = new SolidBrush(MutedColor))
            {
                g.DrawString(sub, Font, subBrush, content.Left, content.Top + 22);
            }

            int used = 46;
            content = new Rectangle(content.Left, content.Top + used, content.Width, content.Height - used);
        }

        private void DrawLegend(Graphics g, ref Rectangle content)
        {
            int x = content.Left;
            int y = content.Top;
            for (int m = 0; m < MetricLabels.Length; m++)
            {
                if (!_visible[m])
                {
                    continue;
                }
                x = DrawLegendItem(g, x, y, MetricColors[m], MetricLabels[m]) + 14;
            }

            int used = 24;
            content = new Rectangle(content.Left, content.Top + used, content.Width, content.Height - used);
        }

        private int DrawLegendItem(Graphics g, int x, int y, Color color, string label)
        {
            using (Pen pen = new Pen(color, 2.5F))
            {
                g.DrawLine(pen, x, y + 8, x + 18, y + 8);
            }
            using (SolidBrush dot = new SolidBrush(color))
            {
                g.FillEllipse(dot, x + 6, y + 4, 7, 7);
            }
            using (SolidBrush t = new SolidBrush(MutedColor))
            {
                g.DrawString(label, Font, t, x + 22, y);
            }
            SizeF size = g.MeasureString(label, Font);
            return x + 22 + (int)size.Width;
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
                    "선택한 Stage의 편차 이력이 없습니다.\nWAFEREYETEACH 명령을 실행하면\n이력이 누적됩니다.",
                    Font, brush, content, sf);
            }
        }

        private void ComputeRange(out double min, out double max)
        {
            min = 0;
            max = 0;
            bool any = false;
            foreach (TeachDiffRecord r in _records)
            {
                for (int m = 0; m < MetricLabels.Length; m++)
                {
                    if (!_visible[m])
                    {
                        continue;
                    }
                    double v = Deviation(r, m);
                    if (!any)
                    {
                        min = v;
                        max = v;
                        any = true;
                    }
                    else
                    {
                        if (v < min) min = v;
                        if (v > max) max = v;
                    }
                }
            }

            if (!any)
            {
                min = -1;
                max = 1;
                return;
            }

            if (min > 0) min = 0;
            if (max < 0) max = 0;

            double span = max - min;
            if (span < 1e-6)
            {
                min -= 1;
                max += 1;
            }
            else
            {
                double margin = span * 0.1;
                min -= margin;
                max += margin;
            }
        }

        private void DrawGridAndAxis(Graphics g, Rectangle plot, double min, double max)
        {
            int rows = 4;
            using (Pen gridPen = new Pen(GridColor))
            using (Pen zeroPen = new Pen(ZeroColor) { DashStyle = DashStyle.Dash })
            using (SolidBrush textBrush = new SolidBrush(MutedColor))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
            {
                for (int i = 0; i <= rows; i++)
                {
                    double value = max - (max - min) * i / rows;
                    int y = plot.Top + (plot.Height * i / rows);
                    bool isZero = Math.Abs(value) < (max - min) * 1e-6;
                    g.DrawLine(isZero ? zeroPen : gridPen, plot.Left, y, plot.Right, y);
                    string label = value.ToString("0.###", CultureInfo.InvariantCulture);
                    g.DrawString(label, Font, textBrush,
                        new RectangleF(plot.Left - 56, y - 8, 52, 16), sf);
                }
            }

            using (Pen axisPen = new Pen(AxisColor))
            {
                g.DrawLine(axisPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
                g.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);
            }
        }

        private void DrawSeries(Graphics g, Rectangle plot, double min, double max)
        {
            int n = _records.Count;
            double range = max - min;
            if (range < 1e-9)
            {
                range = 1;
            }

            for (int m = 0; m < MetricLabels.Length; m++)
            {
                if (!_visible[m])
                {
                    continue;
                }

                Color color = MetricColors[m];
                PointF[] points = new PointF[n];
                for (int i = 0; i < n; i++)
                {
                    double v = Deviation(_records[i], m);
                    float x = n == 1
                        ? plot.Left + plot.Width / 2f
                        : plot.Left + (float)i / (n - 1) * plot.Width;
                    float y = plot.Bottom - (float)((v - min) / range) * plot.Height;
                    points[i] = new PointF(x, y);
                }

                if (n >= 2)
                {
                    using (Pen linePen = new Pen(color, 2F))
                    {
                        g.DrawLines(linePen, points);
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    bool isLast = i == n - 1;
                    float d = isLast ? 5f : 3f;
                    using (SolidBrush b = new SolidBrush(color))
                    {
                        g.FillEllipse(b, points[i].X - d, points[i].Y - d, d * 2, d * 2);
                    }
                }

                float lastX = points[n - 1].X;
                float lastY = points[n - 1].Y;
                using (Pen ring = new Pen(CurrentColor, 2F))
                {
                    g.DrawEllipse(ring, lastX - 7, lastY - 7, 14, 14);
                }
            }

            DrawCurrentValues(g, plot);
        }

        private void DrawCurrentValues(Graphics g, Rectangle plot)
        {
            TeachDiffRecord last = _records[_records.Count - 1];

            List<string> lines = new List<string>();
            for (int m = 0; m < MetricLabels.Length; m++)
            {
                if (!_visible[m])
                {
                    continue;
                }
                lines.Add(string.Format(CultureInfo.InvariantCulture,
                    "{0} Δ {1:+0.000;-0.000;0.000} {2}", MetricLabels[m], Deviation(last, m), MetricUnits[m]));
            }
            if (lines.Count == 0)
            {
                return;
            }

            int boxW = 150;
            int lineH = 16;
            int boxH = lines.Count * lineH + 26;
            Rectangle box = new Rectangle(plot.Right - boxW - 8, plot.Top + 8, boxW, boxH);

            using (SolidBrush back = new SolidBrush(Color.FromArgb(220, 18, 20, 24)))
            {
                g.FillRectangle(back, box);
            }
            using (Pen border = new Pen(CurrentColor))
            {
                g.DrawRectangle(border, box);
            }

            using (Font hdr = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold))
            using (SolidBrush hb = new SolidBrush(CurrentColor))
            {
                g.DrawString(string.Format("현재값 ({0:MM-dd HH:mm})", last.Timestamp),
                    hdr, hb, box.Left + 6, box.Top + 5);
            }

            int y = box.Top + 24;
            int idx = 0;
            for (int m = 0; m < MetricLabels.Length; m++)
            {
                if (!_visible[m])
                {
                    continue;
                }
                using (SolidBrush tb = new SolidBrush(MetricColors[m]))
                {
                    g.DrawString(lines[idx], Font, tb, box.Left + 6, y);
                }
                y += lineH;
                idx++;
            }
        }

        private void DrawXLabels(Graphics g, Rectangle plot)
        {
            int n = _records.Count;
            using (SolidBrush brush = new SolidBrush(MutedColor))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                int maxLabels = Math.Max(2, plot.Width / 90);
                int step = Math.Max(1, (int)Math.Ceiling((double)n / maxLabels));
                for (int i = 0; i < n; i += step)
                {
                    float x = n == 1
                        ? plot.Left + plot.Width / 2f
                        : plot.Left + (float)i / (n - 1) * plot.Width;
                    string label = _records[i].Timestamp.ToString("MM-dd\nHH:mm", CultureInfo.InvariantCulture);
                    g.DrawString(label, Font, brush,
                        new RectangleF(x - 40, plot.Bottom + 4, 80, 30), sf);
                }
            }
        }
    }
}

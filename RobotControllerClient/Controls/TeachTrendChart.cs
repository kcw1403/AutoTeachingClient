using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotControllerClient.Protocol;
using ScottPlot;
using ScottPlot.Plottable;

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

    public class TeachTrendChart : UserControl
    {
        private static readonly Color FigureBg = Color.FromArgb(24, 26, 31);
        private static readonly Color DataBg = Color.FromArgb(30, 33, 40);
        private static readonly Color GridColor = Color.FromArgb(48, 52, 62);
        private static readonly Color TickColor = Color.FromArgb(138, 146, 160);
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

        private readonly FormsPlot _plot = new FormsPlot();
        private readonly bool[] _visible = { true, true, true, false, false, false };
        private readonly ScatterPlot[] _series = new ScatterPlot[6];

        private List<TeachDiffRecord> _records = new List<TeachDiffRecord>();
        private int _station = -1;

        public TeachTrendChart()
        {
            Dock = DockStyle.Fill;
            BackColor = FigureBg;
            _plot.Dock = DockStyle.Fill;
            Controls.Add(_plot);

            _plot.Plot.Style(figureBackground: FigureBg, dataBackground: DataBg,
                grid: GridColor, tick: TickColor, axisLabel: TickColor, titleLabel: TickColor);
            _plot.Plot.XAxis.DateTimeFormat(true);
            _plot.Plot.YAxis.Label("편차");
            _plot.Plot.Legend(true, Alignment.UpperRight);

            RenderEmpty();
        }

        public void SetRecords(int station, List<TeachDiffRecord> records)
        {
            _station = station;
            _records = records ?? new List<TeachDiffRecord>();
            Rebuild();
        }

        public void SetMetricVisible(TrendMetric metric, bool visible)
        {
            int idx = (int)metric;
            _visible[idx] = visible;
            if (_series[idx] != null)
            {
                _series[idx].IsVisible = visible;
            }
            _plot.Plot.AxisAuto();
            _plot.Refresh();
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

        private void Rebuild()
        {
            _plot.Plot.Clear();
            for (int i = 0; i < _series.Length; i++)
            {
                _series[i] = null;
            }

            if (_records.Count == 0)
            {
                RenderEmpty();
                return;
            }

            int n = _records.Count;
            double[] xs = new double[n];
            for (int i = 0; i < n; i++)
            {
                xs[i] = _records[i].Timestamp.ToOADate();
            }

            for (int m = 0; m < MetricLabels.Length; m++)
            {
                double[] ys = new double[n];
                for (int i = 0; i < n; i++)
                {
                    ys[i] = Deviation(_records[i], m);
                }

                ScatterPlot scatter = _plot.Plot.AddScatter(
                    xs, ys,
                    color: MetricColors[m],
                    lineWidth: 2,
                    markerSize: 5,
                    label: MetricLabels[m]);
                scatter.IsVisible = _visible[m];
                _series[m] = scatter;
            }

            AddCurrentMarkers(xs[n - 1]);

            _plot.Plot.Title(_station >= 0
                ? string.Format("Stage {0} 편차 트렌드 (이력 {1}건)", _station, n)
                : "편차 트렌드");
            _plot.Plot.XAxis.TickLabelStyle(rotation: 30);
            _plot.Plot.AxisAuto();
            _plot.Refresh();
        }

        private void AddCurrentMarkers(double lastX)
        {
            TeachDiffRecord last = _records[_records.Count - 1];
            for (int m = 0; m < MetricLabels.Length; m++)
            {
                if (!_visible[m])
                {
                    continue;
                }
                _plot.Plot.AddMarker(
                    lastX, Deviation(last, m),
                    MarkerShape.openCircle,
                    size: 14,
                    color: CurrentColor);
            }
        }

        private void RenderEmpty()
        {
            _plot.Plot.Clear();
            _plot.Plot.Title(_station >= 0
                ? string.Format("Stage {0} · 편차 이력 없음", _station)
                : "편차 트렌드");
            _plot.Plot.AxisAuto();
            _plot.Refresh();
        }
    }
}

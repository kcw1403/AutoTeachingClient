using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using RobotControllerClient.Controls;
using RobotControllerClient.Protocol;

namespace RobotControllerClient.Forms
{
    public class TeachTrendForm : Form
    {
        private static readonly Color BgColor = Color.FromArgb(24, 26, 31);
        private static readonly Color PanelColor = Color.FromArgb(32, 35, 42);
        private static readonly Color TextColor = Color.FromArgb(214, 219, 228);
        private static readonly Color GridColor = Color.FromArgb(52, 56, 66);

        private readonly TeachDiffStore _store;

        private ComboBox _cmbStation;
        private TeachTrendChart _chart;
        private DataGridView _grid;
        private FlowLayoutPanel _metricPanel;
        private Button _btnRefresh;
        private Button _btnClear;
        private Label _lblSummary;

        private readonly CheckBox[] _metricChecks = new CheckBox[6];
        private static readonly string[] MetricNames = { "X", "Y", "Z", "Yaw", "Pitch", "Roll" };

        public TeachTrendForm(TeachDiffStore store)
        {
            _store = store;
            BuildUi();
            LoadStations();
        }

        private void BuildUi()
        {
            Text = "Auto Teaching 편차 이력 - Stage별 XYZ 트렌드";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(940, 620);
            MinimumSize = new Size(720, 480);
            BackColor = BgColor;
            ForeColor = TextColor;
            Font = new Font("Segoe UI", 9F);

            Panel top = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = PanelColor,
                Padding = new Padding(12, 10, 12, 8)
            };

            Label lblStage = new Label
            {
                Text = "Stage",
                AutoSize = true,
                ForeColor = TextColor,
                Location = new Point(12, 16)
            };

            _cmbStation = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(60, 12),
                Width = 160,
                FlatStyle = FlatStyle.Flat,
                BackColor = BgColor,
                ForeColor = TextColor
            };
            _cmbStation.SelectedIndexChanged += (s, e) => RefreshChart();

            _btnRefresh = new Button
            {
                Text = "새로고침",
                Location = new Point(232, 11),
                Width = 84,
                Height = 26,
                FlatStyle = FlatStyle.Flat,
                ForeColor = TextColor
            };
            _btnRefresh.FlatAppearance.BorderColor = GridColor;
            _btnRefresh.Click += (s, e) => { LoadStations(); RefreshChart(); };

            _btnClear = new Button
            {
                Text = "이력 삭제",
                Location = new Point(322, 11),
                Width = 84,
                Height = 26,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(240, 120, 120)
            };
            _btnClear.FlatAppearance.BorderColor = Color.FromArgb(120, 60, 60);
            _btnClear.Click += BtnClear_Click;

            _lblSummary = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(138, 146, 160),
                Location = new Point(422, 16)
            };

            _metricPanel = new FlowLayoutPanel
            {
                Location = new Point(12, 46),
                Size = new Size(900, 26),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                WrapContents = false,
                AutoScroll = false
            };

            bool[] defaults = { true, true, true, false, false, false };
            for (int i = 0; i < MetricNames.Length; i++)
            {
                CheckBox chk = new CheckBox
                {
                    Text = MetricNames[i],
                    Checked = defaults[i],
                    AutoSize = true,
                    ForeColor = TeachTrendChart.MetricColor((TrendMetric)i),
                    Margin = new Padding(0, 3, 16, 0),
                    Tag = (TrendMetric)i
                };
                chk.CheckedChanged += Metric_CheckedChanged;
                _metricChecks[i] = chk;
                _metricPanel.Controls.Add(chk);
            }

            top.Controls.Add(_lblSummary);
            top.Controls.Add(_btnRefresh);
            top.Controls.Add(_btnClear);
            top.Controls.Add(_cmbStation);
            top.Controls.Add(lblStage);
            top.Controls.Add(_metricPanel);

            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                BackColor = BgColor,
                SplitterWidth = 6
            };

            _chart = new TeachTrendChart { Dock = DockStyle.Fill };
            split.Panel1.Controls.Add(_chart);
            split.Panel1MinSize = 220;

            _grid = BuildGrid();
            split.Panel2.Controls.Add(_grid);
            split.Panel2MinSize = 140;

            Controls.Add(split);
            Controls.Add(top);

            Load += (s, e) => { split.SplitterDistance = (int)(ClientSize.Height * 0.58); };
        }

        private DataGridView BuildGrid()
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = BgColor,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                GridColor = GridColor
            };

            grid.DefaultCellStyle.BackColor = PanelColor;
            grid.DefaultCellStyle.ForeColor = TextColor;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 74, 104);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 43, 52);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 26;

            AddColumn(grid, "시각");
            AddColumn(grid, "Slot");
            AddColumn(grid, "Arm");
            AddColumn(grid, "ΔX");
            AddColumn(grid, "ΔY");
            AddColumn(grid, "ΔZ");
            AddColumn(grid, "ΔYaw");
            AddColumn(grid, "ΔPitch");
            AddColumn(grid, "ΔRoll");

            return grid;
        }

        private static void AddColumn(DataGridView grid, string header)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            grid.Columns.Add(col);
        }

        private void Metric_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            _chart.SetMetricVisible((TrendMetric)chk.Tag, chk.Checked);
        }

        private void LoadStations()
        {
            int previous = -1;
            if (_cmbStation.SelectedItem is StationItem prevItem)
            {
                previous = prevItem.Station;
            }

            _cmbStation.Items.Clear();
            List<int> stations = _store.Stations();
            foreach (int st in stations)
            {
                _cmbStation.Items.Add(new StationItem { Station = st });
            }

            if (_cmbStation.Items.Count == 0)
            {
                _lblSummary.Text = "저장된 편차 이력이 없습니다.";
                _chart.SetRecords(-1, new List<TeachDiffRecord>());
                _grid.Rows.Clear();
                return;
            }

            int target = 0;
            for (int i = 0; i < _cmbStation.Items.Count; i++)
            {
                if (((StationItem)_cmbStation.Items[i]).Station == previous)
                {
                    target = i;
                    break;
                }
            }
            _cmbStation.SelectedIndex = target;
        }

        private void RefreshChart()
        {
            StationItem item = _cmbStation.SelectedItem as StationItem;
            if (item == null)
            {
                return;
            }

            List<TeachDiffRecord> records = _store.ForStation(item.Station);
            _chart.SetRecords(item.Station, records);
            FillGrid(records);

            _lblSummary.Text = string.Format("Stage {0} · 이력 {1}건", item.Station, records.Count);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            StationItem item = _cmbStation.SelectedItem as StationItem;
            if (item == null)
            {
                MessageBox.Show(this, "삭제할 이력이 없습니다.", "이력 삭제",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult choice = MessageBox.Show(
                this,
                string.Format(
                    "편차 이력을 삭제합니다.\n\n[예] 현재 Stage {0} 이력만 삭제\n[아니오] 전체 Stage 이력 삭제\n[취소] 취소",
                    item.Station),
                "이력 삭제",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (choice == DialogResult.Yes)
            {
                _store.ClearStation(item.Station);
            }
            else if (choice == DialogResult.No)
            {
                DialogResult confirm = MessageBox.Show(
                    this,
                    "모든 Stage의 편차 이력이 삭제됩니다. 계속하시겠습니까?",
                    "전체 이력 삭제 확인",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);
                if (confirm != DialogResult.OK)
                {
                    return;
                }
                _store.Clear();
            }
            else
            {
                return;
            }

            LoadStations();
            RefreshChart();
        }

        private void FillGrid(List<TeachDiffRecord> records)
        {
            _grid.Rows.Clear();
            for (int i = records.Count - 1; i >= 0; i--)
            {
                TeachDiffRecord r = records[i];
                int rowIndex = _grid.Rows.Add(
                    r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    r.Slot.ToString(CultureInfo.InvariantCulture),
                    string.IsNullOrEmpty(r.Arm) ? "-" : r.Arm,
                    F(r.DeviationX), F(r.DeviationY), F(r.DeviationZ),
                    F(r.DeviationYaw), F(r.DeviationPitch), F(r.DeviationRoll));

                if (i == records.Count - 1)
                {
                    _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(255, 196, 64);
                    _grid.Rows[rowIndex].DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
                }
            }
        }

        private static string F(double v)
        {
            return v.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private sealed class StationItem
        {
            public int Station { get; set; }
            public override string ToString()
            {
                return string.Format("Stage {0}", Station);
            }
        }
    }
}

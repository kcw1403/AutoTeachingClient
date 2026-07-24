using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using RobotControllerClient.Communication;
using RobotControllerClient.Controls;
using RobotControllerClient.Cycle;
using RobotControllerClient.Logging;
using RobotControllerClient.Protocol;

namespace RobotControllerClient.Forms
{
    public partial class MainForm : Form
    {
        private readonly RobotClient _client = new RobotClient();
        private readonly CommLogger _logger;
        private readonly CycleEngine _cycle;
        private readonly List<CycleStep> _steps = new List<CycleStep>();

        private int _runningStepIndex = -1;

        public MainForm()
        {
            InitializeComponent();

            _logger = new CommLogger(Path.Combine(Application.StartupPath, "logs"));
            _cycle = new CycleEngine(_client);

            _client.Log += Client_Log;
            _client.ConnectionChanged += Client_ConnectionChanged;

            _cycle.StepStarted += Cycle_StepStarted;
            _cycle.StepFinished += Cycle_StepFinished;
            _cycle.IterationCompleted += Cycle_IterationCompleted;
            _cycle.CycleStopped += Cycle_CycleStopped;

            BuildCommandButtons();
            UpdateConnectionUi(false);
        }

        private void BuildCommandButtons()
        {
            foreach (CommandDefinition def in CommandCatalog.All)
            {
                Button btn = new Button
                {
                    Text = def.DisplayName,
                    Tag = def,
                    Width = 168,
                    Height = 40,
                    Margin = new Padding(3),
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.System
                };
                btn.BackColor = KindColor(def.Kind);
                _toolTip.SetToolTip(btn, def.Description);
                btn.Click += CommandButton_Click;
                pnlCommands.Controls.Add(btn);
            }
        }

        private static Color KindColor(CommandKind kind)
        {
            switch (kind)
            {
                case CommandKind.Request:
                    return Color.FromArgb(225, 240, 255);
                case CommandKind.Set:
                    return Color.FromArgb(255, 245, 220);
                default:
                    return Color.FromArgb(230, 245, 230);
            }
        }

        private readonly ToolTip _toolTip = new ToolTip();

        private void CommandButton_Click(object sender, EventArgs e)
        {
            CommandDefinition def = (CommandDefinition)((Button)sender).Tag;

            string commandText;
            if (def.HasParameters)
            {
                using (CommandParamDialog dialog = new CommandParamDialog(def))
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                    commandText = dialog.ResultCommandText;
                }
            }
            else
            {
                commandText = def.Build(null);
            }

            AskAddToCycleOrSend(def, commandText);
        }

        private void AskAddToCycleOrSend(CommandDefinition def, string commandText)
        {
            DialogResult choice = MessageBox.Show(
                this,
                string.Format("명령: {0}\n\n[예] 사이클에 추가\n[아니오] 지금 바로 전송\n[취소] 취소", commandText),
                "명령 실행",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (choice == DialogResult.Yes)
            {
                AddStepToCycle(def.Id, commandText);
            }
            else if (choice == DialogResult.No)
            {
                SendOnce(commandText);
            }
        }

        private void AddStepToCycle(string commandId, string commandText)
        {
            int delay = PromptDelay();
            if (delay < 0)
            {
                return;
            }
            CycleStep step = new CycleStep(commandId, commandText, delay);
            _steps.Add(step);
            lstCycle.Items.Add(step);
        }

        private int PromptDelay()
        {
            using (Form form = new Form())
            {
                form.Text = "동작 후 딜레이";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.ClientSize = new Size(280, 110);

                Label label = new Label { Text = "다음 동작까지 대기 시간 (ms)", Left = 12, Top = 15, Width = 250 };
                NumericUpDown num = new NumericUpDown
                {
                    Left = 12,
                    Top = 40,
                    Width = 120,
                    Maximum = 3600000,
                    Minimum = 0,
                    Value = 500
                };
                Button ok = new Button { Text = "확인", Left = 112, Top = 75, Width = 70, DialogResult = DialogResult.OK };
                Button cancel = new Button { Text = "취소", Left = 190, Top = 75, Width = 70, DialogResult = DialogResult.Cancel };

                form.Controls.Add(label);
                form.Controls.Add(num);
                form.Controls.Add(ok);
                form.Controls.Add(cancel);
                form.AcceptButton = ok;
                form.CancelButton = cancel;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    return (int)num.Value;
                }
                return -1;
            }
        }

        private void SendOnce(string commandText)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show(this, "연결되어 있지 않습니다.", "전송 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            System.Threading.Tasks.Task.Run(() =>
            {
                CommandResult result = _client.SendCommand(commandText, System.Threading.CancellationToken.None);
                BeginInvoke((Action)(() => ShowResultSummary(result)));
            });
        }

        private void ShowResultSummary(CommandResult result)
        {
            UpdateTeachChart(result);

            switch (result.Status)
            {
                case ResultStatus.Success:
                    SetStatus(string.Format("완료: {0}", result.CommandText));
                    break;
                case ResultStatus.Nak:
                    SetStatus(string.Format("_NAK (잘못된 명령): {0}", result.CommandText));
                    break;
                case ResultStatus.Error:
                    SetStatus(string.Format("_ERR {0} - {1}", result.ErrorCode, result.ErrorDescription));
                    break;
                case ResultStatus.Timeout:
                    SetStatus(string.Format("타임아웃: {0}", result.CommandText));
                    break;
                case ResultStatus.NotConnected:
                    SetStatus("연결 끊김");
                    break;
            }
        }

        private void UpdateTeachChart(CommandResult result)
        {
            if (result == null || result.Responses == null)
            {
                return;
            }

            foreach (string line in result.Responses)
            {
                TeachDiffer differ;
                if (TeachDiffer.TryParse(line, out differ))
                {
                    teachChart.SetData(differ);
                    AppendLog(LogDirection.Info, string.Format(
                        "TEACH_DIFFER 수신 - 편차 X:{0:0.000} Y:{1:0.000} Z:{2:0.000}",
                        differ.DeviationX, differ.DeviationY, differ.DeviationZ));
                    return;
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int port;
            if (!int.TryParse(txtPort.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out port))
            {
                MessageBox.Show(this, "Port 번호가 올바르지 않습니다.", "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _client.Connect(txtHost.Text.Trim(), port);
            }
            catch (Exception ex)
            {
                AppendLog(LogDirection.Error, "연결 실패: " + ex.Message);
                MessageBox.Show(this, "연결 실패: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (_cycle.IsRunning)
            {
                _cycle.Stop();
            }
            _client.Disconnect();
        }

        private void btnManualSend_Click(object sender, EventArgs e)
        {
            SendManual();
        }

        private void txtManual_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendManual();
            }
        }

        private void SendManual()
        {
            string text = txtManual.Text.Trim();
            if (text.Length == 0)
            {
                return;
            }
            SendOnce(text);
            txtManual.Clear();
        }

        private void btnCycleRemove_Click(object sender, EventArgs e)
        {
            int idx = lstCycle.SelectedIndex;
            if (idx < 0)
            {
                return;
            }
            _steps.RemoveAt(idx);
            lstCycle.Items.RemoveAt(idx);
            if (lstCycle.Items.Count > 0)
            {
                lstCycle.SelectedIndex = Math.Min(idx, lstCycle.Items.Count - 1);
            }
        }

        private void btnCycleUp_Click(object sender, EventArgs e)
        {
            MoveStep(-1);
        }

        private void btnCycleDown_Click(object sender, EventArgs e)
        {
            MoveStep(1);
        }

        private void MoveStep(int direction)
        {
            int idx = lstCycle.SelectedIndex;
            int target = idx + direction;
            if (idx < 0 || target < 0 || target >= _steps.Count)
            {
                return;
            }

            CycleStep step = _steps[idx];
            _steps.RemoveAt(idx);
            _steps.Insert(target, step);

            lstCycle.Items.RemoveAt(idx);
            lstCycle.Items.Insert(target, step);
            lstCycle.SelectedIndex = target;
        }

        private void btnCycleClear_Click(object sender, EventArgs e)
        {
            _steps.Clear();
            lstCycle.Items.Clear();
        }

        private void chkInfinite_CheckedChanged(object sender, EventArgs e)
        {
            numIterations.Enabled = !chkInfinite.Checked;
        }

        private void btnCycleStart_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show(this, "연결되어 있지 않습니다.", "실행 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_steps.Count == 0)
            {
                MessageBox.Show(this, "사이클에 추가된 동작이 없습니다.", "실행 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _cycle.Start(_steps, (int)numIterations.Value, chkInfinite.Checked, chkStopOnError.Checked);
                SetCycleRunningUi(true);
                AppendLog(LogDirection.Info, "사이클 시작");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCycleStop_Click(object sender, EventArgs e)
        {
            _cycle.Stop();
            AppendLog(LogDirection.Info, "사이클 정지 요청");
        }

        private void btnLogClear_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        private void btnOpenLogFolder_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", _logger.LogDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "폴더를 열 수 없습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Client_Log(object sender, CommEventArgs e)
        {
            _logger.Write(e.Direction, e.Text);
            if (IsHandleCreated)
            {
                BeginInvoke((Action)(() => AppendLogLine(e.Direction, e.Text)));
            }
        }

        private void Client_ConnectionChanged(object sender, bool connected)
        {
            if (IsHandleCreated)
            {
                BeginInvoke((Action)(() => UpdateConnectionUi(connected)));
            }
        }

        private void Cycle_StepStarted(object sender, CycleProgressEventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                _runningStepIndex = e.StepIndex;
                lstCycle.Invalidate();
                string total = e.TotalIterations == 0 ? "∞" : e.TotalIterations.ToString();
                cycleStatusLabel.Text = string.Format("반복 {0}/{1}, 스텝 {2}/{3}: {4}",
                    e.Iteration, total, e.StepIndex + 1, _steps.Count, e.Step.CommandText);
            }));
        }

        private void Cycle_StepFinished(object sender, CycleProgressEventArgs e)
        {
            BeginInvoke((Action)(() => ShowResultSummary(e.Result)));
        }

        private void Cycle_IterationCompleted(object sender, int iteration)
        {
            BeginInvoke((Action)(() => AppendLog(LogDirection.Info, string.Format("반복 {0} 완료", iteration))));
        }

        private void Cycle_CycleStopped(object sender, string reason)
        {
            BeginInvoke((Action)(() =>
            {
                _runningStepIndex = -1;
                lstCycle.Invalidate();
                SetCycleRunningUi(false);
                cycleStatusLabel.Text = string.Empty;
                AppendLog(LogDirection.Info, reason);
            }));
        }

        private void lstCycle_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            bool isRunning = e.Index == _runningStepIndex;
            Color back = isRunning ? Color.FromArgb(255, 244, 200) : lstCycle.BackColor;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                back = SystemColors.Highlight;
            }

            using (SolidBrush backBrush = new SolidBrush(back))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            string text = lstCycle.Items[e.Index].ToString();
            if (isRunning)
            {
                text = "▶ " + text;
            }

            Color fore = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? SystemColors.HighlightText
                : SystemColors.WindowText;

            using (SolidBrush foreBrush = new SolidBrush(fore))
            using (Font font = isRunning ? new Font(e.Font, FontStyle.Bold) : new Font(e.Font, FontStyle.Regular))
            {
                e.Graphics.DrawString(text, font, foreBrush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        private void lstLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            string text = lstLog.Items[e.Index].ToString();

            Color color = SystemColors.WindowText;
            if (text.Contains("[SEND]")) color = Color.FromArgb(0, 0, 160);
            else if (text.Contains("[RECV]")) color = Color.FromArgb(0, 110, 0);
            else if (text.Contains("[ERR ]")) color = Color.FromArgb(190, 0, 0);
            else if (text.Contains("[INFO]")) color = Color.FromArgb(90, 90, 90);

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                color = SystemColors.HighlightText;
            }

            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        private void AppendLog(LogDirection direction, string text)
        {
            _logger.Write(direction, text);
            AppendLogLine(direction, text);
        }

        private void AppendLogLine(LogDirection direction, string text)
        {
            string line = string.Format("[{0:HH:mm:ss.fff}] {1} {2}", DateTime.Now, DirTag(direction), text);
            lstLog.Items.Add(line);
            if (lstLog.Items.Count > 5000)
            {
                lstLog.Items.RemoveAt(0);
            }
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        private static string DirTag(LogDirection direction)
        {
            switch (direction)
            {
                case LogDirection.Sent: return "[SEND]";
                case LogDirection.Received: return "[RECV]";
                case LogDirection.Error: return "[ERR ]";
                default: return "[INFO]";
            }
        }

        private void UpdateConnectionUi(bool connected)
        {
            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;
            txtHost.Enabled = !connected;
            txtPort.Enabled = !connected;

            lblStatus.Text = connected ? "● 연결됨" : "● 연결 끊김";
            lblStatus.ForeColor = connected ? Color.ForestGreen : Color.Firebrick;
            SetStatus(connected ? "연결됨" : "연결 안 됨");

            if (!connected && !_cycle.IsRunning)
            {
                SetCycleRunningUi(false);
            }
        }

        private void SetCycleRunningUi(bool running)
        {
            btnCycleStart.Enabled = !running;
            btnCycleStop.Enabled = running;
            btnCycleRemove.Enabled = !running;
            btnCycleUp.Enabled = !running;
            btnCycleDown.Enabled = !running;
            btnCycleClear.Enabled = !running;
            numIterations.Enabled = !running && !chkInfinite.Checked;
            chkInfinite.Enabled = !running;
            chkStopOnError.Enabled = !running;
        }

        private void SetStatus(string text)
        {
            statusLabel.Text = text;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cycle.IsRunning)
            {
                _cycle.Stop();
            }
            _client.Dispose();
            _logger.Dispose();
        }
    }
}

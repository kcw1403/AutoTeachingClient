namespace RobotControllerClient.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.leftSplit = new System.Windows.Forms.SplitContainer();
            this.grpCommands = new System.Windows.Forms.GroupBox();
            this.pnlCommands = new System.Windows.Forms.FlowLayoutPanel();
            this.grpCycle = new System.Windows.Forms.GroupBox();
            this.lstCycle = new System.Windows.Forms.ListBox();
            this.pnlCycleTop = new System.Windows.Forms.Panel();
            this.btnCycleClear = new System.Windows.Forms.Button();
            this.btnCycleDown = new System.Windows.Forms.Button();
            this.btnCycleUp = new System.Windows.Forms.Button();
            this.btnCycleRemove = new System.Windows.Forms.Button();
            this.pnlCycleBottom = new System.Windows.Forms.Panel();
            this.btnCycleStop = new System.Windows.Forms.Button();
            this.btnCycleStart = new System.Windows.Forms.Button();
            this.chkInfinite = new System.Windows.Forms.CheckBox();
            this.chkStopOnError = new System.Windows.Forms.CheckBox();
            this.numIterations = new System.Windows.Forms.NumericUpDown();
            this.lblIterations = new System.Windows.Forms.Label();
            this.rightSplit = new System.Windows.Forms.SplitContainer();
            this.grpTeach = new System.Windows.Forms.GroupBox();
            this.teachChart = new RobotControllerClient.Controls.TeachDiffChart();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.pnlLogBottom = new System.Windows.Forms.Panel();
            this.txtManual = new System.Windows.Forms.TextBox();
            this.btnManualSend = new System.Windows.Forms.Button();
            this.btnLogClear = new System.Windows.Forms.Button();
            this.btnOpenLogFolder = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cycleStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftSplit)).BeginInit();
            this.leftSplit.Panel1.SuspendLayout();
            this.leftSplit.Panel2.SuspendLayout();
            this.leftSplit.SuspendLayout();
            this.grpCommands.SuspendLayout();
            this.grpCycle.SuspendLayout();
            this.pnlCycleTop.SuspendLayout();
            this.pnlCycleBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
            this.rightSplit.Panel1.SuspendLayout();
            this.rightSplit.Panel2.SuspendLayout();
            this.rightSplit.SuspendLayout();
            this.grpTeach.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.pnlLogBottom.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // grpConnection
            //
            this.grpConnection.Controls.Add(this.lblStatus);
            this.grpConnection.Controls.Add(this.btnDisconnect);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.txtPort);
            this.grpConnection.Controls.Add(this.lblPort);
            this.grpConnection.Controls.Add(this.txtHost);
            this.grpConnection.Controls.Add(this.lblHost);
            this.grpConnection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpConnection.Location = new System.Drawing.Point(0, 0);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Padding = new System.Windows.Forms.Padding(8);
            this.grpConnection.Size = new System.Drawing.Size(1008, 64);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "접속";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblStatus.Location = new System.Drawing.Point(560, 30);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(59, 12);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "● 연결 끊김";
            //
            // btnDisconnect
            //
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(452, 24);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(90, 26);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "연결 해제";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            //
            // btnConnect
            //
            this.btnConnect.Location = new System.Drawing.Point(356, 24);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 26);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "연결";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            //
            // txtPort
            //
            this.txtPort.Location = new System.Drawing.Point(268, 27);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(72, 21);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "10110";
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(228, 30);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(33, 12);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port";
            //
            // txtHost
            //
            this.txtHost.Location = new System.Drawing.Point(80, 27);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(140, 21);
            this.txtHost.TabIndex = 1;
            this.txtHost.Text = "199.34.57.91";
            //
            // lblHost
            //
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(16, 30);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(57, 12);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "IP Address";
            //
            // mainSplit
            //
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(0, 64);
            this.mainSplit.Name = "mainSplit";
            //
            // mainSplit.Panel1
            //
            this.mainSplit.Panel1.Controls.Add(this.leftSplit);
            this.mainSplit.Panel1MinSize = 320;
            //
            // mainSplit.Panel2
            //
            this.mainSplit.Panel2.Controls.Add(this.rightSplit);
            this.mainSplit.Panel2MinSize = 300;
            this.mainSplit.Size = new System.Drawing.Size(1288, 665);
            this.mainSplit.SplitterDistance = 520;
            this.mainSplit.TabIndex = 1;
            //
            // rightSplit
            //
            this.rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplit.Location = new System.Drawing.Point(0, 0);
            this.rightSplit.Name = "rightSplit";
            //
            // rightSplit.Panel1
            //
            this.rightSplit.Panel1.Controls.Add(this.grpLog);
            this.rightSplit.Panel1MinSize = 320;
            //
            // rightSplit.Panel2
            //
            this.rightSplit.Panel2.Controls.Add(this.grpTeach);
            this.rightSplit.Panel2MinSize = 280;
            this.rightSplit.Size = new System.Drawing.Size(764, 665);
            this.rightSplit.SplitterDistance = 430;
            this.rightSplit.TabIndex = 0;
            //
            // grpTeach
            //
            this.grpTeach.Controls.Add(this.teachChart);
            this.grpTeach.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTeach.Location = new System.Drawing.Point(0, 0);
            this.grpTeach.Name = "grpTeach";
            this.grpTeach.Padding = new System.Windows.Forms.Padding(6);
            this.grpTeach.Size = new System.Drawing.Size(330, 665);
            this.grpTeach.TabIndex = 0;
            this.grpTeach.TabStop = false;
            this.grpTeach.Text = "티칭 편차 (XYZ 위치 · 편차)";
            //
            // teachChart
            //
            this.teachChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.teachChart.Location = new System.Drawing.Point(6, 18);
            this.teachChart.Name = "teachChart";
            this.teachChart.Size = new System.Drawing.Size(318, 641);
            this.teachChart.TabIndex = 0;
            //
            // leftSplit
            //
            this.leftSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSplit.Location = new System.Drawing.Point(0, 0);
            this.leftSplit.Name = "leftSplit";
            this.leftSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // leftSplit.Panel1
            //
            this.leftSplit.Panel1.Controls.Add(this.grpCommands);
            this.leftSplit.Panel1MinSize = 180;
            //
            // leftSplit.Panel2
            //
            this.leftSplit.Panel2.Controls.Add(this.grpCycle);
            this.leftSplit.Panel2MinSize = 240;
            this.leftSplit.Size = new System.Drawing.Size(560, 665);
            this.leftSplit.SplitterDistance = 300;
            this.leftSplit.TabIndex = 0;
            //
            // grpCommands
            //
            this.grpCommands.Controls.Add(this.pnlCommands);
            this.grpCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCommands.Location = new System.Drawing.Point(0, 0);
            this.grpCommands.Name = "grpCommands";
            this.grpCommands.Padding = new System.Windows.Forms.Padding(6);
            this.grpCommands.Size = new System.Drawing.Size(560, 300);
            this.grpCommands.TabIndex = 0;
            this.grpCommands.TabStop = false;
            this.grpCommands.Text = "명령 (버튼 클릭으로 실행)";
            //
            // pnlCommands
            //
            this.pnlCommands.AutoScroll = true;
            this.pnlCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCommands.Location = new System.Drawing.Point(6, 18);
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Padding = new System.Windows.Forms.Padding(2);
            this.pnlCommands.Size = new System.Drawing.Size(548, 276);
            this.pnlCommands.TabIndex = 0;
            //
            // grpCycle
            //
            this.grpCycle.Controls.Add(this.lstCycle);
            this.grpCycle.Controls.Add(this.pnlCycleTop);
            this.grpCycle.Controls.Add(this.pnlCycleBottom);
            this.grpCycle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCycle.Location = new System.Drawing.Point(0, 0);
            this.grpCycle.Name = "grpCycle";
            this.grpCycle.Padding = new System.Windows.Forms.Padding(6);
            this.grpCycle.Size = new System.Drawing.Size(560, 361);
            this.grpCycle.TabIndex = 0;
            this.grpCycle.TabStop = false;
            this.grpCycle.Text = "사이클 (동작을 순서대로 이어서 반복 구동)";
            //
            // lstCycle
            //
            this.lstCycle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCycle.FormattingEnabled = true;
            this.lstCycle.IntegralHeight = false;
            this.lstCycle.ItemHeight = 12;
            this.lstCycle.Location = new System.Drawing.Point(6, 50);
            this.lstCycle.Name = "lstCycle";
            this.lstCycle.Size = new System.Drawing.Size(548, 261);
            this.lstCycle.TabIndex = 1;
            this.lstCycle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstCycle.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstCycle_DrawItem);
            //
            // pnlCycleTop
            //
            this.pnlCycleTop.Controls.Add(this.btnCycleClear);
            this.pnlCycleTop.Controls.Add(this.btnCycleDown);
            this.pnlCycleTop.Controls.Add(this.btnCycleUp);
            this.pnlCycleTop.Controls.Add(this.btnCycleRemove);
            this.pnlCycleTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCycleTop.Location = new System.Drawing.Point(6, 18);
            this.pnlCycleTop.Name = "pnlCycleTop";
            this.pnlCycleTop.Size = new System.Drawing.Size(548, 32);
            this.pnlCycleTop.TabIndex = 0;
            //
            // btnCycleClear
            //
            this.btnCycleClear.Location = new System.Drawing.Point(273, 3);
            this.btnCycleClear.Name = "btnCycleClear";
            this.btnCycleClear.Size = new System.Drawing.Size(80, 25);
            this.btnCycleClear.TabIndex = 3;
            this.btnCycleClear.Text = "전체 삭제";
            this.btnCycleClear.UseVisualStyleBackColor = true;
            this.btnCycleClear.Click += new System.EventHandler(this.btnCycleClear_Click);
            //
            // btnCycleDown
            //
            this.btnCycleDown.Location = new System.Drawing.Point(184, 3);
            this.btnCycleDown.Name = "btnCycleDown";
            this.btnCycleDown.Size = new System.Drawing.Size(80, 25);
            this.btnCycleDown.TabIndex = 2;
            this.btnCycleDown.Text = "아래로 ▼";
            this.btnCycleDown.UseVisualStyleBackColor = true;
            this.btnCycleDown.Click += new System.EventHandler(this.btnCycleDown_Click);
            //
            // btnCycleUp
            //
            this.btnCycleUp.Location = new System.Drawing.Point(95, 3);
            this.btnCycleUp.Name = "btnCycleUp";
            this.btnCycleUp.Size = new System.Drawing.Size(80, 25);
            this.btnCycleUp.TabIndex = 1;
            this.btnCycleUp.Text = "위로 ▲";
            this.btnCycleUp.UseVisualStyleBackColor = true;
            this.btnCycleUp.Click += new System.EventHandler(this.btnCycleUp_Click);
            //
            // btnCycleRemove
            //
            this.btnCycleRemove.Location = new System.Drawing.Point(6, 3);
            this.btnCycleRemove.Name = "btnCycleRemove";
            this.btnCycleRemove.Size = new System.Drawing.Size(80, 25);
            this.btnCycleRemove.TabIndex = 0;
            this.btnCycleRemove.Text = "선택 삭제";
            this.btnCycleRemove.UseVisualStyleBackColor = true;
            this.btnCycleRemove.Click += new System.EventHandler(this.btnCycleRemove_Click);
            //
            // pnlCycleBottom
            //
            this.pnlCycleBottom.Controls.Add(this.btnCycleStop);
            this.pnlCycleBottom.Controls.Add(this.btnCycleStart);
            this.pnlCycleBottom.Controls.Add(this.chkStopOnError);
            this.pnlCycleBottom.Controls.Add(this.chkInfinite);
            this.pnlCycleBottom.Controls.Add(this.numIterations);
            this.pnlCycleBottom.Controls.Add(this.lblIterations);
            this.pnlCycleBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCycleBottom.Location = new System.Drawing.Point(6, 311);
            this.pnlCycleBottom.Name = "pnlCycleBottom";
            this.pnlCycleBottom.Size = new System.Drawing.Size(548, 44);
            this.pnlCycleBottom.TabIndex = 2;
            //
            // btnCycleStop
            //
            this.btnCycleStop.Enabled = false;
            this.btnCycleStop.Location = new System.Drawing.Point(462, 9);
            this.btnCycleStop.Name = "btnCycleStop";
            this.btnCycleStop.Size = new System.Drawing.Size(80, 28);
            this.btnCycleStop.TabIndex = 5;
            this.btnCycleStop.Text = "정지";
            this.btnCycleStop.UseVisualStyleBackColor = true;
            this.btnCycleStop.Click += new System.EventHandler(this.btnCycleStop_Click);
            //
            // btnCycleStart
            //
            this.btnCycleStart.Location = new System.Drawing.Point(376, 9);
            this.btnCycleStart.Name = "btnCycleStart";
            this.btnCycleStart.Size = new System.Drawing.Size(80, 28);
            this.btnCycleStart.TabIndex = 4;
            this.btnCycleStart.Text = "시작";
            this.btnCycleStart.UseVisualStyleBackColor = true;
            this.btnCycleStart.Click += new System.EventHandler(this.btnCycleStart_Click);
            //
            // chkStopOnError
            //
            this.chkStopOnError.AutoSize = true;
            this.chkStopOnError.Checked = true;
            this.chkStopOnError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStopOnError.Location = new System.Drawing.Point(234, 15);
            this.chkStopOnError.Name = "chkStopOnError";
            this.chkStopOnError.Size = new System.Drawing.Size(107, 16);
            this.chkStopOnError.TabIndex = 3;
            this.chkStopOnError.Text = "에러 시 정지";
            this.chkStopOnError.UseVisualStyleBackColor = true;
            //
            // chkInfinite
            //
            this.chkInfinite.AutoSize = true;
            this.chkInfinite.Location = new System.Drawing.Point(150, 15);
            this.chkInfinite.Name = "chkInfinite";
            this.chkInfinite.Size = new System.Drawing.Size(72, 16);
            this.chkInfinite.TabIndex = 2;
            this.chkInfinite.Text = "무한반복";
            this.chkInfinite.UseVisualStyleBackColor = true;
            this.chkInfinite.CheckedChanged += new System.EventHandler(this.chkInfinite_CheckedChanged);
            //
            // numIterations
            //
            this.numIterations.Location = new System.Drawing.Point(72, 12);
            this.numIterations.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIterations.Name = "numIterations";
            this.numIterations.Size = new System.Drawing.Size(70, 21);
            this.numIterations.TabIndex = 1;
            this.numIterations.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            //
            // lblIterations
            //
            this.lblIterations.AutoSize = true;
            this.lblIterations.Location = new System.Drawing.Point(6, 16);
            this.lblIterations.Name = "lblIterations";
            this.lblIterations.Size = new System.Drawing.Size(57, 12);
            this.lblIterations.TabIndex = 0;
            this.lblIterations.Text = "반복 횟수";
            //
            // grpLog
            //
            this.grpLog.Controls.Add(this.lstLog);
            this.grpLog.Controls.Add(this.pnlLogBottom);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(0, 0);
            this.grpLog.Name = "grpLog";
            this.grpLog.Padding = new System.Windows.Forms.Padding(6);
            this.grpLog.Size = new System.Drawing.Size(444, 665);
            this.grpLog.TabIndex = 0;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "통신 로그 (파일 자동 저장)";
            //
            // lstLog
            //
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstLog.FormattingEnabled = true;
            this.lstLog.HorizontalScrollbar = true;
            this.lstLog.IntegralHeight = false;
            this.lstLog.ItemHeight = 14;
            this.lstLog.Location = new System.Drawing.Point(6, 18);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(432, 569);
            this.lstLog.TabIndex = 1;
            this.lstLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstLog_DrawItem);
            //
            // pnlLogBottom
            //
            this.pnlLogBottom.Controls.Add(this.txtManual);
            this.pnlLogBottom.Controls.Add(this.btnManualSend);
            this.pnlLogBottom.Controls.Add(this.btnLogClear);
            this.pnlLogBottom.Controls.Add(this.btnOpenLogFolder);
            this.pnlLogBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLogBottom.Location = new System.Drawing.Point(6, 587);
            this.pnlLogBottom.Name = "pnlLogBottom";
            this.pnlLogBottom.Size = new System.Drawing.Size(432, 72);
            this.pnlLogBottom.TabIndex = 0;
            //
            // txtManual
            //
            this.txtManual.Location = new System.Drawing.Point(3, 8);
            this.txtManual.Name = "txtManual";
            this.txtManual.Size = new System.Drawing.Size(330, 21);
            this.txtManual.TabIndex = 0;
            this.txtManual.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtManual_KeyDown);
            //
            // btnManualSend
            //
            this.btnManualSend.Location = new System.Drawing.Point(339, 6);
            this.btnManualSend.Name = "btnManualSend";
            this.btnManualSend.Size = new System.Drawing.Size(88, 25);
            this.btnManualSend.TabIndex = 1;
            this.btnManualSend.Text = "수동 전송";
            this.btnManualSend.UseVisualStyleBackColor = true;
            this.btnManualSend.Click += new System.EventHandler(this.btnManualSend_Click);
            //
            // btnLogClear
            //
            this.btnLogClear.Location = new System.Drawing.Point(3, 39);
            this.btnLogClear.Name = "btnLogClear";
            this.btnLogClear.Size = new System.Drawing.Size(120, 27);
            this.btnLogClear.TabIndex = 2;
            this.btnLogClear.Text = "화면 로그 지우기";
            this.btnLogClear.UseVisualStyleBackColor = true;
            this.btnLogClear.Click += new System.EventHandler(this.btnLogClear_Click);
            //
            // btnOpenLogFolder
            //
            this.btnOpenLogFolder.Location = new System.Drawing.Point(129, 39);
            this.btnOpenLogFolder.Name = "btnOpenLogFolder";
            this.btnOpenLogFolder.Size = new System.Drawing.Size(120, 27);
            this.btnOpenLogFolder.TabIndex = 3;
            this.btnOpenLogFolder.Text = "로그 폴더 열기";
            this.btnOpenLogFolder.UseVisualStyleBackColor = true;
            this.btnOpenLogFolder.Click += new System.EventHandler(this.btnOpenLogFolder_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.cycleStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 729);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.TabIndex = 2;
            //
            // statusLabel
            //
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(57, 17);
            this.statusLabel.Text = "준비";
            //
            // cycleStatusLabel
            //
            this.cycleStatusLabel.Name = "cycleStatusLabel";
            this.cycleStatusLabel.Size = new System.Drawing.Size(0, 17);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1288, 751);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.grpConnection);
            this.Font = new System.Drawing.Font("굴림", 9F);
            this.MinimumSize = new System.Drawing.Size(1120, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Robot Controller Client - QUADRA 5 Axis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.leftSplit.Panel1.ResumeLayout(false);
            this.leftSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.leftSplit)).EndInit();
            this.leftSplit.ResumeLayout(false);
            this.grpCommands.ResumeLayout(false);
            this.grpCycle.ResumeLayout(false);
            this.pnlCycleTop.ResumeLayout(false);
            this.pnlCycleBottom.ResumeLayout(false);
            this.pnlCycleBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIterations)).EndInit();
            this.rightSplit.Panel1.ResumeLayout(false);
            this.rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
            this.rightSplit.ResumeLayout(false);
            this.grpTeach.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.pnlLogBottom.ResumeLayout(false);
            this.pnlLogBottom.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.SplitContainer leftSplit;
        private System.Windows.Forms.GroupBox grpCommands;
        private System.Windows.Forms.FlowLayoutPanel pnlCommands;
        private System.Windows.Forms.GroupBox grpCycle;
        private System.Windows.Forms.Panel pnlCycleTop;
        private System.Windows.Forms.Button btnCycleRemove;
        private System.Windows.Forms.Button btnCycleUp;
        private System.Windows.Forms.Button btnCycleDown;
        private System.Windows.Forms.Button btnCycleClear;
        private System.Windows.Forms.ListBox lstCycle;
        private System.Windows.Forms.Panel pnlCycleBottom;
        private System.Windows.Forms.Label lblIterations;
        private System.Windows.Forms.NumericUpDown numIterations;
        private System.Windows.Forms.CheckBox chkInfinite;
        private System.Windows.Forms.CheckBox chkStopOnError;
        private System.Windows.Forms.Button btnCycleStart;
        private System.Windows.Forms.Button btnCycleStop;
        private System.Windows.Forms.SplitContainer rightSplit;
        private System.Windows.Forms.GroupBox grpTeach;
        private RobotControllerClient.Controls.TeachDiffChart teachChart;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Panel pnlLogBottom;
        private System.Windows.Forms.TextBox txtManual;
        private System.Windows.Forms.Button btnManualSend;
        private System.Windows.Forms.Button btnLogClear;
        private System.Windows.Forms.Button btnOpenLogFolder;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel cycleStatusLabel;
    }
}

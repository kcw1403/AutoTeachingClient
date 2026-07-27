namespace RobotControllerClient.Forms
{
    partial class CommandParamDialog
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
            this.pnlParams = new System.Windows.Forms.TableLayoutPanel();
            this.lblPreviewCaption = new System.Windows.Forms.Label();
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.btnAddToCycle = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // pnlParams
            //
            this.pnlParams.ColumnCount = 2;
            this.pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.pnlParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlParams.Location = new System.Drawing.Point(12, 12);
            this.pnlParams.Name = "pnlParams";
            this.pnlParams.Size = new System.Drawing.Size(410, 200);
            this.pnlParams.TabIndex = 0;
            this.pnlParams.AutoSize = true;
            this.pnlParams.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            //
            // lblPreviewCaption
            //
            this.lblPreviewCaption.AutoSize = true;
            this.lblPreviewCaption.Location = new System.Drawing.Point(12, 224);
            this.lblPreviewCaption.Name = "lblPreviewCaption";
            this.lblPreviewCaption.Size = new System.Drawing.Size(78, 12);
            this.lblPreviewCaption.TabIndex = 1;
            this.lblPreviewCaption.Text = "전송 미리보기";
            //
            // txtPreview
            //
            this.txtPreview.Location = new System.Drawing.Point(12, 240);
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.ReadOnly = true;
            this.txtPreview.Size = new System.Drawing.Size(410, 21);
            this.txtPreview.TabIndex = 2;
            this.txtPreview.BackColor = System.Drawing.SystemColors.Info;
            //
            // btnAddToCycle
            //
            this.btnAddToCycle.Location = new System.Drawing.Point(170, 272);
            this.btnAddToCycle.Name = "btnAddToCycle";
            this.btnAddToCycle.Size = new System.Drawing.Size(90, 28);
            this.btnAddToCycle.TabIndex = 3;
            this.btnAddToCycle.Text = "사이클 추가";
            this.btnAddToCycle.UseVisualStyleBackColor = true;
            this.btnAddToCycle.Click += new System.EventHandler(this.btnAddToCycle_Click);
            //
            // btnSend
            //
            this.btnSend.Location = new System.Drawing.Point(266, 272);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 28);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(347, 272);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //
            // CommandParamDialog
            //
            this.AcceptButton = this.btnSend;
            this.CancelButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 312);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnAddToCycle);
            this.Controls.Add(this.txtPreview);
            this.Controls.Add(this.lblPreviewCaption);
            this.Controls.Add(this.pnlParams);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommandParamDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "명령 파라미터 입력";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel pnlParams;
        private System.Windows.Forms.Label lblPreviewCaption;
        private System.Windows.Forms.TextBox txtPreview;
        private System.Windows.Forms.Button btnAddToCycle;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnCancel;
    }
}

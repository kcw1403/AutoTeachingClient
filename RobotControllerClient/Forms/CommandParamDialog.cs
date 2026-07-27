using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RobotControllerClient.Protocol;

namespace RobotControllerClient.Forms
{
    public enum CommandAction
    {
        Send,
        AddToCycle
    }

    public partial class CommandParamDialog : Form
    {
        private readonly CommandDefinition _definition;
        private readonly Dictionary<string, Control> _inputs = new Dictionary<string, Control>();

        public string ResultCommandText { get; private set; }
        public CommandAction ResultAction { get; private set; }

        public CommandParamDialog(CommandDefinition definition)
        {
            _definition = definition;
            InitializeComponent();
            Text = "파라미터 입력 - " + definition.DisplayName;
            BuildInputs();
            UpdatePreview();

            if (definition.Id == CommandCatalog.DelayCommandId)
            {
                btnSend.Visible = false;
                btnAddToCycle.Text = "확인";
                AcceptButton = btnAddToCycle;
            }
        }

        private void BuildInputs()
        {
            pnlParams.Controls.Clear();
            pnlParams.RowStyles.Clear();
            pnlParams.RowCount = _definition.Parameters.Count;

            int row = 0;
            foreach (CommandParameter p in _definition.Parameters)
            {
                pnlParams.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));

                Label label = new Label
                {
                    Text = p.Name + " : " + p.Description,
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };

                Control input;
                if (p.Choices != null && p.Choices.Length > 0)
                {
                    ComboBox combo = new ComboBox
                    {
                        Dock = DockStyle.Fill,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    combo.Items.AddRange(p.Choices);
                    int idx = Array.IndexOf(p.Choices, p.DefaultValue);
                    combo.SelectedIndex = idx >= 0 ? idx : 0;
                    combo.SelectedIndexChanged += (s, e) => UpdatePreview();
                    input = combo;
                }
                else
                {
                    TextBox tb = new TextBox
                    {
                        Dock = DockStyle.Fill,
                        Text = p.DefaultValue
                    };
                    tb.TextChanged += (s, e) => UpdatePreview();
                    input = tb;
                }

                _inputs[p.Name] = input;
                pnlParams.Controls.Add(label, 0, row);
                pnlParams.Controls.Add(input, 1, row);
                row++;
            }
        }

        private Dictionary<string, string> CollectValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (KeyValuePair<string, Control> kv in _inputs)
            {
                values[kv.Key] = kv.Value.Text.Trim();
            }
            return values;
        }

        private void UpdatePreview()
        {
            txtPreview.Text = _definition.Build(CollectValues());
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Commit(CommandAction.Send);
        }

        private void btnAddToCycle_Click(object sender, EventArgs e)
        {
            Commit(CommandAction.AddToCycle);
        }

        private void Commit(CommandAction action)
        {
            foreach (KeyValuePair<string, Control> kv in _inputs)
            {
                if (string.IsNullOrEmpty(kv.Value.Text.Trim()))
                {
                    MessageBox.Show("빈 파라미터가 있습니다: " + kv.Key, "입력 확인",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            ResultCommandText = _definition.Build(CollectValues());
            ResultAction = action;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

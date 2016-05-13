using pb.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// problème : l'ordre des control de _tabPanel peut changer
// par exemple si on affiche panel 5 puis panel 4, l'ordre devient : panel 4, panel 1, panel 2, panel 3, panel 5

namespace Test.Test_Form.Test_Visible_01
{
    public class Form1 : Form
    {
        private IContainer _components = null;
        private Panel _tabPanel;
        private Panel _toolsPanel;
        private Panel _tracePanel;
        private TextBox _textBox;

        public Form1()
        {
            this.Load += Form1_Load;

            _tabPanel = zForm.CreatePanel(dockStyle: DockStyle.Top, backColor: Color.DodgerBlue, height: 300);
            _tabPanel.SuspendLayout();
            _toolsPanel = zForm.CreatePanel(dockStyle: DockStyle.Top, backColor: Color.DodgerBlue, height: 35);
            _toolsPanel.SuspendLayout();
            _tracePanel = zForm.CreatePanel(dockStyle: DockStyle.Fill);
            _tracePanel.SuspendLayout();

            CreateTabPanel();
            CreateToolsPanel();
            CreateTracePanel();

            this.SuspendLayout();

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 600);

            this.Controls.Add(_tracePanel);
            this.Controls.Add(_toolsPanel);
            this.Controls.Add(_tabPanel);

            ActiveTabPanel(4);

            _tracePanel.ResumeLayout(false);
            _toolsPanel.ResumeLayout(false);
            _tabPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            WriteMessage("form load");
            TraceTabPanel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        private Panel _panel1;
        private Panel _panel2;
        private Panel _panel3;
        private Panel _panel4;
        private Panel _panel5;
        private void CreateTabPanel()
        {
            _panel1 = CreatePanelWithText(SystemColors.Control, "panel 1");
            _panel1.Name = "panel 1";
            _panel2 = CreatePanelWithText(SystemColors.Control, "panel 2");
            _panel2.Name = "panel 2";
            _panel3 = CreatePanelWithText(SystemColors.Control, "panel 3");
            _panel3.Name = "panel 3";
            _panel4 = CreatePanelWithText(SystemColors.Control, "panel 4");
            _panel4.Name = "panel 4";
            _panel5 = CreatePanelWithText(SystemColors.Control, "panel 5");
            _panel5.Name = "panel 5";
            _tabPanel.Controls.Add(_panel1);
            _tabPanel.Controls.Add(_panel2);
            _tabPanel.Controls.Add(_panel3);
            _tabPanel.Controls.Add(_panel4);
            _tabPanel.Controls.Add(_panel5);
            _panel1.Visible = false;
            _panel2.Visible = false;
            _panel3.Visible = false;
            _panel4.Visible = false;
            _panel5.Visible = false;
        }

        private int _selectedIndex = -1;
        private void ActiveTabPanel(int index)
        {
            if (index == _selectedIndex)
                return;
            if (_selectedIndex != -1)
                _tabPanel.Controls[_selectedIndex].Visible = false;
            //_tabPanel.Controls[_selectedIndex].Hide();
            _tabPanel.Controls[index].Visible = true;
            //_tabPanel.Controls[index].Show();
            _selectedIndex = index;

            WriteMessage("active panel {0}", index);
            TraceTabPanel();
        }

        private void TraceTabPanel()
        {
            //WriteMessage("panel1 : index {0} visible {1}", _tabPanel.Controls.GetChildIndex(_panel1), _panel1.Visible);
            //WriteMessage("panel2 : index {0} visible {1}", _tabPanel.Controls.GetChildIndex(_panel2), _panel2.Visible);
            //WriteMessage("panel3 : index {0} visible {1}", _tabPanel.Controls.GetChildIndex(_panel3), _panel3.Visible);
            //WriteMessage("panel4 : index {0} visible {1}", _tabPanel.Controls.GetChildIndex(_panel4), _panel4.Visible);
            //WriteMessage("panel5 : index {0} visible {1}", _tabPanel.Controls.GetChildIndex(_panel5), _panel5.Visible);

            for (int index = 0; index < 5; index++)
            {
                Control panel = _tabPanel.Controls[index];
                WriteMessage("index {0} name \"{1}\" visible {2}", index, panel.Name, panel.Visible);
            }
            WriteMessage();
        }

        private static Panel CreatePanelWithText(Color color, string text)
        {
            Panel panel = zForm.CreatePanel(dockStyle: DockStyle.Fill, backColor: color);
            TextBox textBox = zForm.CreateTextBox(multiline: true, width: 500, height: 50);
            textBox.Text = text;
            panel.Controls.Add(textBox);
            return panel;
        }

        private void CreateToolsPanel()
        {
            ToolStrip toolStrip1 = new ToolStrip();
            toolStrip1.SuspendLayout();
            toolStrip1.Items.Add(zForm.CreateToolStripButton("panel 1", (sender, eventArgs) => ActiveTabPanel(0)));
            toolStrip1.Items.Add(zForm.CreateToolStripButton("panel 2", (sender, eventArgs) => ActiveTabPanel(1)));
            toolStrip1.Items.Add(zForm.CreateToolStripButton("panel 3", (sender, eventArgs) => ActiveTabPanel(2)));
            toolStrip1.Items.Add(zForm.CreateToolStripButton("panel 4", (sender, eventArgs) => ActiveTabPanel(3)));
            toolStrip1.Items.Add(zForm.CreateToolStripButton("panel 5", (sender, eventArgs) => ActiveTabPanel(4)));
            toolStrip1.ResumeLayout(false);

            _toolsPanel.Controls.Add(toolStrip1);
        }

        private void CreateTracePanel()
        {
            _textBox = zForm.CreateTextBox(dockStyle: DockStyle.Fill, multiline: true);
            _tracePanel.Controls.Add(_textBox);
        }

        private void WriteMessage(string message = null, params object[] prm)
        {
            if (_textBox == null)
                return;
            if (prm.Length > 0)
                message = string.Format(message, prm);
            if (message != null)
                _textBox.AppendText(message);
            _textBox.AppendText("\r\n");
            _textBox.Select(_textBox.TextLength, 0);
            _textBox.ScrollToCaret();
        }
    }
}

using pb.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Test.Test_RunSourceForm_01
{
    public partial class RunSourceFormBase_v2 : Form
    {
        private IContainer _components = null;
        private Panel _topToolsPanel;
        private Panel _editPanel;
        private PanelTabControl _resultTab;
        //private int _messageTabIndex = -1;
        private Panel _bottomToolsPanel;
        private ToolStrip _bottomToolStrip;
        //private int _formWidth = 300;
        //private int _formHeight = 300;

        // Form :
        //   _menu
        //   _toolsPanel
        //   _topPanel
        //   splitTop
        //   _bottomPanel
        //   _statusPanel
        //

        public RunSourceFormBase_v2()
        {
            CreateControls();
            this.Load += RunSourceFormBase_v2_Load;
        }

        private void RunSourceFormBase_v2_Load(object sender, System.EventArgs e)
        {
            //WriteMessage("form load");
            //TraceResultTab();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        //public int FormWidth { get { return _formWidth; } set { _formWidth = value; } }
        //public int FormHeight { get { return _formHeight; } set { _formHeight = value; } }

        public void Initialize()
        {
            this.SuspendLayout();

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            //this.ClientSize = new Size(_formWidth, _formHeight);

            this.Controls.Add(_bottomToolsPanel);
            this.Controls.Add(_resultTab);
            this.Controls.Add(zForm.CreateSplitter(DockStyle.Top));
            this.Controls.Add(_editPanel);
            this.Controls.Add(_topToolsPanel);
            this.Controls.Add(this.MainMenuStrip);

            //WriteMessage("before PerformLayout");
            //TraceResultTab();

            // active message tab
            //ActivePanResult(_messageTabIndex);

            this.MainMenuStrip.ResumeLayout(false);
            _topToolsPanel.ResumeLayout(false);
            _editPanel.ResumeLayout(false);
            _resultTab.ResumeLayout(false);
            _bottomToolStrip.ResumeLayout(false);
            _bottomToolsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            //WriteMessage("after PerformLayout");
            //TraceResultTab();
        }

        //private TextBox _textBox;
        private void CreateControls()
        {
            _topToolsPanel = zForm.CreatePanel(DockStyle.Top, Color.DodgerBlue, height: 35);
            _topToolsPanel.SuspendLayout();

            _editPanel = zForm.CreatePanel(DockStyle.Top, Color.DeepPink, height: 300);
            _editPanel.SuspendLayout();
            //_textBox = zForm.CreateTextBox(dockStyle: DockStyle.Fill, multiline: true);
            //_editPanel.Controls.Add(_textBox);

            //_bottomPanel = zForm.CreatePanel(DockStyle.Fill, Color.Brown);
            //_bottomPanel.SuspendLayout();
            //_resultTab = CreateResultTab();
            _resultTab = PanelTabControl.Create(DockStyle.Fill, Color.DarkOrange);
            _resultTab.SuspendLayout();

            //_statusPanel = zForm.CreatePanel(DockStyle.Bottom, Color.Chocolate, height: 20);
            //_statusPanel.SuspendLayout();
            //_bottomToolsPanel = CreateBottomToolsPanel();
            _bottomToolStrip = new ToolStrip();
            _bottomToolStrip.SuspendLayout();
            _bottomToolsPanel = zForm.CreatePanel(DockStyle.Bottom, Color.Chocolate, height: 20);
            _bottomToolsPanel.SuspendLayout();
            _bottomToolsPanel.Controls.Add(_bottomToolStrip);

            MenuStrip menu = new MenuStrip();
            menu.SuspendLayout();
            this.MainMenuStrip = menu;
        }

        //private Panel _panel1;
        //private Panel _panel2;
        //private Panel _panel3;
        //private Panel _panel4;
        //private Panel _panel5;
        //private PanelTabControl CreateResultTab()
        //{
        //    //PanelTabControl tabControl = PanelTabControl.Create(DockStyle.Top, Color.Blue, height: 300);
        //    PanelTabControl tabControl = PanelTabControl.Create(DockStyle.Fill, Color.DarkOrange);
        //    tabControl.SuspendLayout();
        //    tabControl.Controls.Add(zForm.CreatePanel(DockStyle.Fill, Color.Red));
        //    tabControl.Controls.Add(zForm.CreatePanel(DockStyle.Fill, Color.Green));
        //    tabControl.Controls.Add(zForm.CreatePanel(DockStyle.Fill, Color.Blue));
        //    tabControl.Controls.Add(zForm.CreatePanel(DockStyle.Fill, Color.Yellow));
        //    tabControl.Controls.Add(zForm.CreatePanel(DockStyle.Fill, Color.BlueViolet));
        //    _messageTabIndex = 4;

        //    //_panel1 = CreatePanelWithText(Color.Red, "result 1");
        //    //_panel2 = CreatePanelWithText(Color.Green, "result 2");
        //    //_panel3 = CreatePanelWithText(Color.Blue, "result 3");
        //    //_panel4 = CreatePanelWithText(Color.Yellow, "result 4");
        //    //_panel5 = CreatePanelWithText(Color.Brown, "message");
        //    //tabControl.Controls.Add(_panel1);
        //    //tabControl.Controls.Add(_panel2);
        //    //tabControl.Controls.Add(_panel3);
        //    //tabControl.Controls.Add(_panel4);
        //    //tabControl.Controls.Add(_panel5);
        //    //_panel1.Visible = false;
        //    //_panel2.Visible = false;
        //    //_panel3.Visible = false;
        //    //_panel4.Visible = false;
        //    //_panel5.Visible = false;
        //    return tabControl;
        //}

        //private static Panel CreatePanelWithText(Color color, string text)
        //{
        //    Panel panel = zForm.CreatePanel(DockStyle.Fill, color);
        //    TextBox textBox = zForm.CreateTextBox(multiline: true, width: 500, height: 50);
        //    textBox.Text = text;
        //    panel.Controls.Add(textBox);
        //    return panel;
        //}

        //private Panel CreateBottomToolsPanel()
        //{
        //    ToolStrip toolStrip1 = new ToolStrip();
        //    toolStrip1.SuspendLayout();
        //    toolStrip1.Items.Add(zForm.CreateToolStripButton("result 1", (sender, eventArgs) => ActivePanResult(0)));
        //    toolStrip1.Items.Add(zForm.CreateToolStripButton("result 2", (sender, eventArgs) => ActivePanResult(1)));
        //    toolStrip1.Items.Add(zForm.CreateToolStripButton("result 3", (sender, eventArgs) => ActivePanResult(2)));
        //    toolStrip1.Items.Add(zForm.CreateToolStripButton("result 4", (sender, eventArgs) => ActivePanResult(3)));
        //    toolStrip1.Items.Add(zForm.CreateToolStripButton("messages", (sender, eventArgs) => ActivePanResult(4)));
        //    toolStrip1.ResumeLayout(false);

        //    Panel statusPanel = zForm.CreatePanel(DockStyle.Bottom, Color.Chocolate, height: 20);
        //    statusPanel.SuspendLayout();
        //    statusPanel.Controls.Add(toolStrip1);

        //    return statusPanel;
        //}

        public void ActiveResultPanel(int index)
        {
            _resultTab.SelectedIndex = index;
            //WriteMessage("active panel {0}", selectedIndex);
            //TraceResultTab();
        }

        public Panel AddResultPanel(string buttonText, Color? backColor = null)
        {
            ToolStripButton button = zForm.CreateToolStripButton(buttonText);
            button.Tag = _resultTab.Controls.Count;
            button.Click += (sender, eventArgs) => ActiveResultPanel((int)button.Tag);
            _bottomToolStrip.Items.Add(button);

            Panel panel = zForm.CreatePanel(DockStyle.Fill, backColor);
            _resultTab.Controls.Add(panel);

            return panel;
        }

        //private void TraceResultTab()
        //{
        //    WriteMessage("panel1 : index {0} visible {1}", _resultTab.Controls.GetChildIndex(_panel1), _panel1.Visible);
        //    WriteMessage("panel2 : index {0} visible {1}", _resultTab.Controls.GetChildIndex(_panel2), _panel2.Visible);
        //    WriteMessage("panel3 : index {0} visible {1}", _resultTab.Controls.GetChildIndex(_panel3), _panel3.Visible);
        //    WriteMessage("panel4 : index {0} visible {1}", _resultTab.Controls.GetChildIndex(_panel4), _panel4.Visible);
        //    WriteMessage("panel5 : index {0} visible {1}", _resultTab.Controls.GetChildIndex(_panel5), _panel5.Visible);
        //    WriteMessage();
        //}

        //private void WriteMessage(string message = null, params object[] prm)
        //{
        //    if (_textBox == null)
        //        return;
        //    if (prm.Length > 0)
        //        message = string.Format(message, prm);
        //    if (message != null)
        //        _textBox.AppendText(message);
        //    _textBox.AppendText("\r\n");
        //    _textBox.Select(_textBox.TextLength, 0);
        //    _textBox.ScrollToCaret();
        //}
    }
}

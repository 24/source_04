using System.Drawing;
using System.Windows.Forms;
using pb.Windows.Forms;

namespace Test.Test_Form.Test_RunSourceForm.v3
{
    public class RunSourceForm_v3 : RunSourceFormBase_v3
    {
        private Button _executeButton;
        private Button _pauseButton;
        private Button _stopButton;
        private Label _progressLabel;
        private ProgressBar _progressBar;
        private ScintillaControl _source;
        private XtraGridControl _gridResult1;
        //private GridView _gridView1;
        private DataGridViewControl _gridResult2;
        private DataGrid _gridResult3;
        private TreeView _treeResult;
        private LogTextBox _logTextBox;

        public RunSourceForm_v3()
        {
            //this.FormWidth = 1060;
            //this.FormHeight = 650;
            this.ClientSize = new Size(1060, 650);
            CreateMenu();
            CreateTopTools();

            _source = ScintillaControl.Create(dockStyle: DockStyle.Fill);
            _editPanel.Controls.Add(_source);
            _source.SetFont("Consolas", 10);
            _source.ConfigureLexerCpp();
            _source.SetTabIndent(2, 0, false);
            _source.DisplayLineNumber(4);
            //_source.SelectionChanged += _source_SelectionChanged;
            _source.UpdateUI += _source_UpdateUI;

            Panel panel = AddResultPanel("result 1");
            _gridResult1 = XtraGridControl.Create(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_gridResult1);

            panel = AddResultPanel("result 2");
            _gridResult2 = DataGridViewControl.Create(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_gridResult2);

            panel = AddResultPanel("result 3");
            _gridResult3 = zForm.CreateDataGrid(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_gridResult3);

            panel = AddResultPanel("result 4");
            _treeResult = new TreeView();
            _treeResult.Dock = DockStyle.Fill;
            panel.Controls.Add(_treeResult);

            panel = AddResultPanel("message");
            _logTextBox = LogTextBox.Create(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_logTextBox);
            //ActiveResultPanel(4);
            SelectResultTab(4);
            this.BaseInitialize();
            this.Load += RunSourceForm_v3_Load;
        }

        private void _source_UpdateUI(object sender, ScintillaNET.UpdateUIEventArgs e)
        {
            pb.Trace.WriteLine("_source.SelectionStart {0} _source.SelectionEnd {1}", _source.SelectionStart, _source.SelectionEnd);
        }

        private void RunSourceForm_v3_Load(object sender, System.EventArgs e)
        {
            //_logTextBox.WriteMessage
            pb.Trace.WriteLine("Load");
            pb.Trace.WriteLine("_source {0}", _source.GetType().FullName);
            pb.Trace.WriteLine("_source is ScintillaNET.Scintilla {0}", _source is ScintillaNET.Scintilla);
            ScintillaNET.Scintilla scintilla = _source;
            pb.Trace.WriteLine("scintilla {0}", scintilla.GetType().FullName);
            //pb.Trace.WriteLine("_source.Selection.Start {0}", _source.Selection.Start);
        }

        //private void _source_SelectionChanged(object sender, System.EventArgs e)
        //{
        //    _logTextBox.WriteMessage("SelectionChanged : Start {0} End {1}", _source.Selection.Start, _source.Selection.End);
        //}

        public ScintillaControl Source { get { return _source; } }
        public XtraGridControl GridResult1 { get { return _gridResult1; } }
        public DataGridViewControl GridResult2 { get { return _gridResult2; } }
        public DataGrid GridResult3 { get { return _gridResult3; } }
        public TreeView TreeResult { get { return _treeResult; } }
        public LogTextBox LogTextBox { get { return _logTextBox; } }

        private void CreateMenu()
        {
            MenuStrip menu = this.MainMenuStrip;
            ToolStripMenuItem m_file = zForm.CreateMenuItem("&File");
            m_file.DropDownItems.AddRange(new ToolStripItem[] {
                zForm.CreateMenuItem("&New (Ctrl-N)"),
                zForm.CreateMenuItem("&Open (Ctrl-O)"),
                zForm.CreateMenuItem("&Save (Ctrl-S)"),
                zForm.CreateMenuItem("Save &as (Ctrl-A)"),
                //new ToolStripSeparator(),
                //zForm.CreateMenuItem("&Execute (F5)", onClick: m_execute_Click),
                //zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
                //zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
                //zForm.CreateMenuItem("Execute &without project (ctrl + F5)", onClick: m_execute_without_project_Click),
                //zForm.CreateMenuItem("&Compile (Shift-Ctrl-B)", onClick: m_compile_Click),
                //new ToolStripSeparator(),
                //zForm.CreateMenuItem("Compile and &restart \"Run source\" (Shift-Ctrl-U)", onClick: m_update_runsource_Click),
                //zForm.CreateMenuItem("C&ompile \"Run source\"  (Shift-Ctrl-C)", onClick: m_compile_runsource_Click),
                //zForm.CreateMenuItem("&Restart \"Run source\"  (Shift-Ctrl-R)", onClick:  m_restart_runsource_Click),
                //new ToolStripSeparator(),
                //zForm.CreateMenuItem("&Quit", onClick: m_quit_Click),
            });

            ToolStripMenuItem m_options = zForm.CreateMenuItem("&Options");

            //m_view_source_line_number = zForm.CreateMenuItem("View source line number", checkOnClick: true, @checked: true, onClick: m_view_source_line_number_Click);
            //m_run_init = zForm.CreateMenuItem("Run &init", checkOnClick: true, @checked: true, onClick: m_run_init_Click);
            //m_allow_multiple_execution = zForm.CreateMenuItem("&Allow multiple execution", checkOnClick: true, @checked: true, onClick: m_allow_multiple_execution_Click);

            m_options.DropDownItems.AddRange(new ToolStripItem[] {
                zForm.CreateMenuItem("Set grid &max width height", checkOnClick: true, @checked: true),
                zForm.CreateMenuItem("Resize data table images", checkOnClick: true, @checked: true)
                //m_view_source_line_number,
                //new ToolStripSeparator(),
                //m_run_init,
                //m_allow_multiple_execution
            });

            menu.Items.AddRange(new ToolStripItem[] { m_file, m_options });
        }

        private void CreateTopTools()
        {
            _executeButton = zForm.CreateButton("&Run", x: 3, y: 3, width: 75, height: 23);
            _pauseButton = zForm.CreateButton("&Pause", x: 87, y: 3, width: 75, height: 23);
            _stopButton = zForm.CreateButton("&Stop", x: 171, y: 3, width: 75, height: 23);
            // width: 47 
            _progressLabel = zForm.CreateLabel("progress", x: 624, y: 1, height: 13);
            _progressBar = zForm.CreateProgressBar(x: 622, y: 16, width: 422, height: 16);
            _topToolsPanel.SuspendLayout();
            _topToolsPanel.Controls.Add(_executeButton);
            _topToolsPanel.Controls.Add(_pauseButton);
            _topToolsPanel.Controls.Add(_stopButton);
            _topToolsPanel.Controls.Add(_progressLabel);
            _topToolsPanel.Controls.Add(_progressBar);
            _topToolsPanel.ResumeLayout(false);
        }
    }
}

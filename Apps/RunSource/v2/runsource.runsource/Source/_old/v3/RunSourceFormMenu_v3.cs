using pb.Windows.Forms;
using System;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm_v3
    {
        private ToolStripMenuItem _menuViewSourceLineNumber;
        private ToolStripMenuItem _menuRunInit;
        private ToolStripMenuItem _menuAllowMultipleExecution;
        private ToolStripMenuItem _menuGridSetMaxWidthHeight;
        private ToolStripMenuItem _menuResizeDatatableImages;
        private Button _executeButton;
        private Button _pauseButton;
        private Button _stopButton;
        private Label _progressLabel;
        private ProgressBar _progressBar;
        private string _progressText = null;

        private void CreateMenu()
        {
            MenuStrip menu = this.MainMenuStrip;
            ToolStripMenuItem m_file = zForm.CreateMenuItem("&File");
            m_file.DropDownItems.AddRange(new ToolStripItem[] {
                zForm.CreateMenuItem("&New (Ctrl-N)", onClick: menuNew_Click),
                zForm.CreateMenuItem("&Open (Ctrl-O)", onClick: menuOpen_Click),
                zForm.CreateMenuItem("&Save (Ctrl-S)", onClick: menuSave_Click),
                zForm.CreateMenuItem("Save &as (Ctrl-A)", onClick: menuSaveAs_Click),
                new ToolStripSeparator(),
                zForm.CreateMenuItem("&Execute (F5)", onClick: menuExecute_Click),
                zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: menuExecuteOnMainThread_Click),
                zForm.CreateMenuItem("Execute &without project (ctrl + F5)", onClick: menuExecuteWithoutProject_Click),
                zForm.CreateMenuItem("&Compile (Shift-Ctrl-B)", onClick: menuCompile_Click),
                new ToolStripSeparator(),
                zForm.CreateMenuItem("Compile and &restart \"Run source\" (Shift-Ctrl-U)", onClick: menuUpdateRunsource_Click),
                zForm.CreateMenuItem("C&ompile \"Run source\"  (Shift-Ctrl-C)", onClick: menuCompileRunsource_Click),
                zForm.CreateMenuItem("&Restart \"Run source\"  (Shift-Ctrl-R)", onClick:  menuRestartRunsource_Click),
                new ToolStripSeparator(),
                zForm.CreateMenuItem("&Quit", onClick: menuQuit_Click),
            });

            ToolStripMenuItem m_options = zForm.CreateMenuItem("&Options");

            _menuViewSourceLineNumber = zForm.CreateMenuItem("View source line number", checkOnClick: true, @checked: true, onClick: menuViewSourceLineNumber_Click);
            _menuRunInit = zForm.CreateMenuItem("Run &init", checkOnClick: true, @checked: true, onClick: menuRunInit_Click);
            _menuAllowMultipleExecution = zForm.CreateMenuItem("&Allow multiple execution", checkOnClick: true, @checked: true, onClick: menuAllowMultipleExecution_Click);
            _menuGridSetMaxWidthHeight = zForm.CreateMenuItem("Set grid &max width height", checkOnClick: true, @checked: true, onClick: menuGridSetMaxWidthHeight_Click);
            _menuResizeDatatableImages = zForm.CreateMenuItem("Resize data table images", checkOnClick: true, @checked: true, onClick: menuResizeDatatableImages_Click);

            m_options.DropDownItems.AddRange(new ToolStripItem[] {
                _menuGridSetMaxWidthHeight,
                _menuResizeDatatableImages,
                _menuViewSourceLineNumber,
                new ToolStripSeparator(),
                _menuRunInit,
                _menuAllowMultipleExecution
            });

            menu.Items.AddRange(new ToolStripItem[] { m_file, m_options });
        }

        private void InitMenu()
        {
            ScintillaViewLineNumber(_menuViewSourceLineNumber.Checked);
            _menuRunInit.Checked = GetCallInit();
            _menuAllowMultipleExecution.Checked = GetAllowMultipleExecution();
            _menuGridSetMaxWidthHeight.Checked = GetGridMaxColumnsWidthAndRowsHeight();
            _menuResizeDatatableImages.Checked = GetResizeDataTableImages();
        }

        private void CreateTopTools()
        {
            _executeButton = zForm.CreateButton("&Run", x: 3, y: 3, width: 75, height: 23, onClick: buttonExecute_Click);
            _pauseButton = zForm.CreateButton("&Pause", x: 87, y: 3, width: 75, height: 23, onClick: buttonPause_Click);
            _stopButton = zForm.CreateButton("&Stop", x: 171, y: 3, width: 75, height: 23, onClick: buttonStop_Click);
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

        private void menuNew_Click(object sender, EventArgs e)
        {
            Try(New);
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            Try(Open);
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            Try(Save);
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            Try(SaveAs);
        }

        private void menuExecute_Click(object sender, EventArgs e)
        {
            Try(() => _RunCode());
        }

        private void menuExecuteOnMainThread_Click(object sender, EventArgs e)
        {
            Try(() => _RunCode(useNewThread: false));
        }

        private void menuExecuteWithoutProject_Click(object sender, EventArgs e)
        {
            Try(() => _RunCode(compileWithoutProject: true));
        }

        private void menuCompile_Click(object sender, EventArgs e)
        {
            Try(CompileCode);
        }

        private void menuUpdateRunsource_Click(object sender, EventArgs e)
        {
            Try(_UpdateRunSource);
        }

        private void menuCompileRunsource_Click(object sender, EventArgs e)
        {
            Try(_CompileRunSource);
        }

        private void menuRestartRunsource_Click(object sender, EventArgs e)
        {
            Try(_RestartRunSource);
        }

        private void menuQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuGridSetMaxWidthHeight_Click(object sender, EventArgs e)
        {
            SetGridMaxColumnsWidthAndRowsHeight(_menuGridSetMaxWidthHeight.Checked);
        }

        private void menuResizeDatatableImages_Click(object sender, EventArgs e)
        {
            SetResizeDataTableImages(_menuResizeDatatableImages.Checked);
        }

        private void menuViewSourceLineNumber_Click(object sender, EventArgs e)
        {
            ScintillaViewLineNumber(_menuViewSourceLineNumber.Checked);
        }

        private void menuRunInit_Click(object sender, EventArgs e)
        {
            SetCallInit(_menuRunInit.Checked);
        }

        private void menuAllowMultipleExecution_Click(object sender, EventArgs e)
        {
            SetAllowMultipleExecution(_menuAllowMultipleExecution.Checked);
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            if (!_runSource.IsRunning())
                Try(() => _RunCode());
            else
            {
                DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "Run source", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.OK)
                    Try(AbortThreadExecution);
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            bool pause = !_runSource.IsExecutionPaused();
            _runSource.PauseExecution(pause);
            if (pause)
                _pauseButton.Text = "&Continue";
            else
                _pauseButton.Text = "&Pause";
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (_runSource.IsRunning())
            {
                DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du/des programme(s) ?", "Run source", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.OK)
                    Try(AbortThreadExecution);
            }
        }
    }
}

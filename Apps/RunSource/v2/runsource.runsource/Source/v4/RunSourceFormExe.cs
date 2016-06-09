using System;
using System.Drawing;
using System.Windows.Forms;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using pb.Windows.Forms;

// - améliorer ctrl-f prendre selection et ds la dlg sel le text
// - faire ctrl-m + ctrl-m
// - faire ctrl-g


namespace runsourced
{
    public partial class RunSourceFormExe : RunSourceForm
    {
        private ITrace _trace = null;
        private XmlConfig _config = null;
        private RunSourceRestartParameters _runSourceParameters = null;
        private string _title = "Run source";

        public RunSourceFormExe(IRunSource runSource, ITrace trace, XmlConfig config, RunSourceRestartParameters runSourceParameters)
        {
            _runSource = runSource;
            _trace = trace;
            _trace.SetViewer(TraceWrited);
            _config = config;
            _runSourceParameters = runSourceParameters;

            _source.TextChanged += source_TextChanged;

            CreateMenu();
            CreateTopTools();
            this.InitializeForm();
            InitExe();
            InitMenu();
            UpdateRunSourceStatus();
            Try(SetKeyboardShortcuts);
            this.KeyPreview = true;
            this.Load += RunSourceForm_Load;
            this.FormClosing += RunSourceForm_FormClosing;
            this.FormClosed += RunSourceForm_FormClosed;
            //this.KeyDown += RunSourceForm_KeyDown;
        }

        private void SetKeyboardShortcuts()
        {
            // ctrl-N : New (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.N, () => PostMessage(_hwnd, WM_CUSTOM_NEW, 0, 0));
            // ctrl-O : Open (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.O, () => PostMessage(_hwnd, WM_CUSTOM_OPEN_FILE, 0, 0));
            // ctrl-S : Save (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.S, () => PostMessage(_hwnd, WM_CUSTOM_SAVE, 0, 0));
            // ctrl-A : SaveAs (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.A, () => PostMessage(_hwnd, WM_CUSTOM_SAVE_AS, 0, 0));

            // F5 : RunCode (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.F5, () => PostMessage(_hwnd, WM_CUSTOM_RUN_CODE, 0, 0));
            // shift-F5 : RunCodeOnMainThread (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Shift | Keys.F5, () => PostMessage(_hwnd, WM_CUSTOM_RUN_CODE_ON_MAIN_THREAD, 0, 0));
            // ctrl-F5 : RunCodeWithoutProject (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.F5, () => PostMessage(_hwnd, WM_CUSTOM_RUN_CODE_WITHOUT_PROJECT, 0, 0));
            // shift-ctrl-B : CompileCode (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Shift | Keys.Control | Keys.B, () => PostMessage(_hwnd, WM_CUSTOM_COMPILE_CODE, 0, 0));
            // ctrl-C : AbortExecution (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.C, () => PostMessage(_hwnd, WM_CUSTOM_ABORT_EXECUTION, 0, 0));

            // shift-ctrl-C : CompileRunSource (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Shift | Keys.Control | Keys.C, () => PostMessage(_hwnd, WM_CUSTOM_COMPILE_RUNSOURCE, 0, 0));
            // shift-ctrl-R : RestartRunSource (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Shift | Keys.Control | Keys.R, () => PostMessage(_hwnd, WM_CUSTOM_RESTART_RUNSOURCE, 0, 0));
            // shift-ctrl-U : UpdateRunSource (PostMessage)
            _formKeyboard.SetSimpleKey(Keys.Shift | Keys.Control | Keys.U, () => PostMessage(_hwnd, WM_CUSTOM_UPDATE_RUNSOURCE, 0, 0));
        }

        private void RunSourceForm_Load(object sender, EventArgs e)
        {
            Try(_RunSourceForm_Load, errorMessageBox: true);
        }

        private void _RunSourceForm_Load()
        {
            LoadSettings();
            if (_runSourceParameters != null)
            {
                if (_runSourceParameters.SourceFile != null)
                    OpenSourceFile(_runSourceParameters.SourceFile);
                _source.SelectionStart = _runSourceParameters.SelectionStart;
                _source.SelectionEnd = _runSourceParameters.SelectionEnd;
                _source.ScrollCaret();
            }

            // il faut afficher une fois le _tabResultGrid sinon _gridResult2.AutoResizeColumns() et _gridResult2.AutoResizeRows() ne marche pas la 1ère fois
            SelectGridResultTab();
            SelectMessageResultTab();
            ActiveControl = _source;
        }

        private void RunSourceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool cancel;

            if (_runSource.IsRunning())
            {
                DialogResult r = MessageBox.Show("Un programme est en cours d'exécution. Voulez-vous l'interrompre ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                AbortThreadExecution(out cancel);
                if (cancel)
                {
                    e.Cancel = cancel;
                    return;
                }
            }
            ControlSave(out cancel);
            e.Cancel = cancel;
        }

        private void RunSourceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Try(_RunSourceForm_FormClosed, errorMessageBox: true);
        }

        private void _RunSourceForm_FormClosed()
        {
            SaveSettings();
            EndRunSource();
            _trace.SetViewer(null);
        }

        private void InitExe()
        {
            // http://stackoverflow.com/questions/9056418/resources-getobjectthis-icon-crashes-application-on-windows-xp
            // pour ne pas avoir une exection avec :
            // this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            // Thread.Sleep(100);
            // dont work
            Try(() => this.Icon = Properties.Resources.app);
            InitTitle();
            InitResult();
            InitRunSource();
            SetFileSaved();
        }

        private void InitTitle()
        {
            string title = _config.Get("RunsourceTitle");
            if (title != null)
                _title = title;
        }

        private void SetFormTitle()
        {
            string title = _title;
            if (_runSource.SourceFile != null)
                title += " : " + zPath.GetFileName(_runSource.SourceFile);
            if (!_fileSaved)
                title += "*";
            if (_runSource.ProjectFile != null)
            {
                title += " (" + zPath.GetFileName(_runSource.ProjectFile);
                if (!zFile.Exists(_runSource.ProjectFile))
                    title += "(*)";
                title += ")";
            }
            if (_runSource.Progress_PutProgressMessageToWindowsTitle && _progressText != null)
                title += " - " + _progressText;
            this.Text = title;
        }

        private void source_TextChanged(object sender, EventArgs e)
        {
            SetFileNotSaved();
        }

        private string GetCode()
        {
            return _source.SelectedText;
        }

        private void LoadSettings()
        {
            XmlSerializer xmlSerializer = XmlSerializer.Load(GetSettingsFile());

            if (xmlSerializer.OpenElement("RunSource_Form") != null)
            {
                this.WindowState = xmlSerializer.GetValue("WindowState", FormWindowState.Normal);
                int locationX = xmlSerializer.GetValue("Location_X", this.Location.X);
                int locationY = xmlSerializer.GetValue("Location_Y", this.Location.Y);
                this.Location = new Point(locationX, locationY);
                int sizeWidth = xmlSerializer.GetValue("Width", this.Size.Width);
                int sizeHeight = xmlSerializer.GetValue("Height", this.Size.Height);
                this.Size = new Size(sizeWidth, sizeHeight);
                int panTopHeight = xmlSerializer.GetValue("pan_top_Height", _editPanel.Size.Height);
                _editPanel.Size = new Size(_editPanel.Size.Width, panTopHeight);
                xmlSerializer.CloseElement();
            }

            if (xmlSerializer.OpenElement("RunSource") != null)
            {
                _settingsProjectDirectory = xmlSerializer.GetValue<string>("ProjectDirectory");
                xmlSerializer.CloseElement();
            }
        }

        private void SaveSettings()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(rootName: "settings");

            xmlSerializer.AddElement("RunSource_Form");
            xmlSerializer.AddValue("WindowState", this.WindowState);
            xmlSerializer.AddValue("Location_X", this.Location.X);
            xmlSerializer.AddValue("Location_Y", this.Location.Y);
            xmlSerializer.AddValue("Width", this.Size.Width);
            xmlSerializer.AddValue("Height", this.Size.Height);
            xmlSerializer.AddValue("pan_top_Height", _editPanel.Size.Height);
            xmlSerializer.CloseElement();

            xmlSerializer.AddElement("RunSource");
            xmlSerializer.AddValue("ProjectDirectory", _runSource.ProjectDirectory ?? _settingsProjectDirectory);
            xmlSerializer.CloseElement();

            xmlSerializer.Save(GetSettingsFile());
        }

        private string GetSettingsFile()
        {
            return zPath.Combine(zapp.GetLocalSettingsDirectory(_config.Get("RunsourceProductName")), "settings.xml");
        }

        private void Try(Action action, bool errorMessageBox = false)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _trace.WriteError(ex);
                if (errorMessageBox)
                    zerrf.ErrorMessageBox(ex);
            }
        }
    }
}

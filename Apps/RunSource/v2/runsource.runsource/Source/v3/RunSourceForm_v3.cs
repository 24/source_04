using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using pb.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

// todo :
// ok - manage run init
// ok - manage allow multiple run
// ok - correct pb with modal dialog, scintilla send code character
//    - add status bar, line column, nb of thread running
//    - take text selection when open find form

namespace runsourced
{
    public partial class RunSourceForm_v3 : RunSourceFormBase_v3
    {
        private string _title = "Run source";
        private XmlConfig _config = null;
        private RunSourceRestartParameters _runSourceParameters = null;
        private ITrace _trace = null;

        public RunSourceForm_v3(IRunSource runSource, ITrace trace, XmlConfig config, RunSourceRestartParameters runSourceParameters)
        {
            _runSource = runSource;
            _config = config;
            _runSourceParameters = runSourceParameters;
            _trace = trace;
            _trace.SetViewer(TraceWrited);

            this.ClientSize = new Size(1060, 650);
            CreateMenu();
            CreateTopTools();
            CreateScintillaControl();
            _editPanel.Controls.Add(_source);
            CreateResultControls();
            this.BaseInitialize();
            InitExe();
            InitMenu();
            this.KeyPreview = true;
            this.Load += RunSourceForm_Load;
            this.FormClosing += RunSourceForm_FormClosing;
            this.FormClosed += RunSourceForm_FormClosed;
            this.KeyDown += RunSourceForm_KeyDown;
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
            //tc_result.SelectedTab = _tabResultGrid;
            SelectGridResultTab();
            //tc_result.SelectedTab = _tabResultMessage;
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
            this.Icon = Properties.Resources.app;
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

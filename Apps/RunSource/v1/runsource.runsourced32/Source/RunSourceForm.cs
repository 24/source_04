using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;
using pb;
using PB_Util;
using PB_Grid;
using pb.Data.Xml;

namespace runsourced
{
    public class RunSourceRestartParameters
    {
        public string SourceFile;
        public int SelectionStart;
        public int SelectionLength;
    }

    //public delegate void UpdateRunsourceFilesEvent(Dictionary<string, List<string>> projectFiles, RunSourceParameters runSourceParameters);
    public delegate void UpdateRunsourceFilesEvent(Dictionary<string, List<string>> projectFiles);
    public delegate void SetRestartRunsourceEvent(RunSourceRestartParameters runSourceParameters);

    public partial class RunSourceForm : Form
    {
        private static string __title = "Run source";
        public delegate void fExe();
        private bool _disableMessage = false;
        private DataTable _dataTableResult = null;
        private DataSet _dataSetResult = null;
        private string _xmlResultFormat = null;
        private bool _newDataTableResult = false;       // true si il y a un nouveau r�sultat DataTable ou DataSet � afficher
        private bool _newTreeViewResult = false;        // true si il y a un nouveau r�sultat TreeView � afficher
        private bool _selectTreeViewResult = false;     // true si il faut s�lectionner le r�sultat du TreeView
        private bool _errorResult = false;              // true si le r�sultat est une liste d'erreurs
        //private bool _newMessage = false;             // true si il y a un nouveau message � afficher
        private string _sourcePath = null;
        private bool _fileSaved = true;
        private string _sourceFilter = "source files (*.cs)|*.cs|All files (*.*)|*.*";
        private string _progressText = null;
        private bool _setGridMaxColumnsWidthAndRowsHeight = true;
        private int _gridMaxWidth = 0;
        private int _gridMaxHeight = 0;
        private bool _resizeDataTableImages = true;
        private int _dataTableMaxImageWidth = 0;
        private int _dataTableMaxImageHeight = 0;

        private XmlConfig _config = null;
        private IRunSource _runSource = null;
        private RunSourceRestartParameters _runSourceParameters = null;

        private delegate void EventMessageSendCallback(string sMsg);
        private delegate void SetDataTableEventCallback(DataTable dt, string sXmlFormat);
        private delegate void EventGridResultSetDataSetCallback(DataSet ds, string sXmlFormat);
        //private delegate void EventTreeViewResultClearCallback();
        //private delegate void EventTreeViewResultAddCallback(string nodeName, XElement xmlElement, XFormat xFormat);
        private delegate void EventProgressChangeCallback(int iCurrent, int iTotal, string sMessage, params object[] prm);
        private delegate void EventEndRunCallback(bool bError);
        //public UpdateRunsourceFilesEvent UpdateRunsourceFiles;
        public SetRestartRunsourceEvent SetRestartRunsource;

        public RunSourceForm(IRunSource runSource, XmlConfig config, RunSourceRestartParameters runSourceParameters)
        {
            _runSource = runSource;
            _config = config;
            _runSourceParameters = runSourceParameters;

            // http://stackoverflow.com/questions/9056418/resources-getobjectthis-icon-crashes-application-on-windows-xp
            // pour ne pas avoir une exection avec :
            // this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            // Thread.Sleep(100);
            // dont work

            InitializeComponent();
            this.Icon = Properties.Resources.app;
            initGrid();
            //initTestMenu();
            tb_source.ConfigurationManager.CustomLocation = @"ScintillaNET.xml";
            tb_source.ConfigurationManager.Language = "cs";
            tc_result.SelectedTab = tab_message;
            //ActiveControl = me_source;
            ActiveControl = tb_source;

            cGrid.Culture = CultureInfo.CurrentUICulture;

            //_gridMaxWidth = _config.Get<int>("GridMaxWidth");
            _gridMaxWidth = _config.Get("GridMaxWidth").zParseAs<int>();
            _gridMaxHeight = _config.Get("GridMaxHeight").zParseAs<int>();
            _dataTableMaxImageWidth = _config.Get("DataTableMaxImageWidth").zParseAs<int>();
            _dataTableMaxImageHeight = _config.Get("DataTableMaxImageHeight").zParseAs<int>();

            initRunSource();
            InitLog();
            SetFileSaved();
        }

        private System.Windows.Forms.DataGridView _gridResult2;
        private void initGrid()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._gridResult2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._gridResult2)).BeginInit();
            this.tab_result2.Controls.Add(this._gridResult2);
            this._gridResult2.AllowUserToAddRows = false;
            this._gridResult2.AllowUserToDeleteRows = false;
            this._gridResult2.AllowUserToOrderColumns = true;
            this._gridResult2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewCellStyle1.NullValue = "(null)";
            this._gridResult2.DefaultCellStyle = dataGridViewCellStyle1;
            this._gridResult2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridResult2.Location = new System.Drawing.Point(0, 0);
            this._gridResult2.Name = "grid_result2";
            this._gridResult2.ReadOnly = true;
            this._gridResult2.Size = new System.Drawing.Size(1048, 364);
            this._gridResult2.TabIndex = 0;
            this._gridResult2.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.grid_result2_RowPostPaint);
            ((System.ComponentModel.ISupportInitialize)(this._gridResult2)).EndInit();
        }

        private void RunSourceForm_Load(object sender, EventArgs e)
        {
            LoadParameters();
            if (_runSourceParameters != null)
            {
                if (_runSourceParameters.SourceFile != null)
                    OpenSourceFile(_runSourceParameters.SourceFile);
                //me_source.Select(_runSourceParameters.SelectionStart, _runSourceParameters.SelectionLength);
                //me_source.ScrollToCaret();
                tb_source.Selection.Start = _runSourceParameters.SelectionStart;
                tb_source.Selection.Length = _runSourceParameters.SelectionLength;
                tb_source.Scrolling.ScrollToCaret();
            }

            // il faut afficher une fois le tab_result2 sinon _gridResult2.AutoResizeColumns() et _gridResult2.AutoResizeRows() ne marche pas la 1�re fois
            tc_result.SelectedTab = tab_result2;
            tc_result.SelectedTab = tab_message;
            //ActiveControl = me_source;
            ActiveControl = tb_source;
        }

        private void RunSourceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            endRunSource();
        }

        private void RunSourceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool bCancel;

            if (_runSource.IsRunning())
            {
                DialogResult r = MessageBox.Show("Un programme est en cours d'ex�cution. Voulez-vous l'interrompre ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                AbortThreadExecution(out bCancel);
                if (bCancel)
                {
                    e.Cancel = bCancel;
                    return;
                }
            }
            ControlSave(out bCancel);
            e.Cancel = bCancel;
        }

        private void initRunSource()
        {
            _runSource.SetRunSourceConfig(_config.ConfigPath);
            _runSource.Trace.Writed += new WritedEvent(EventWrited);
            _runSource.DisableMessageChanged += new DisableMessageChangedEvent(EventDisableMessageChanged);
            _runSource.GridResultSetDataTable += new SetDataTableEvent(EventGridResultSetDataTable);
            _runSource.GridResultSetDataSet += new SetDataSetEvent(EventGridResultSetDataSet);
            //_runSource.TreeViewResultAdd += new TreeViewResultAddEvent(EventTreeViewResultAdd);
            //_runSource.TreeViewResultSelect += new TreeViewResultSelectEvent(EventTreeViewResultSelect);
            _runSource.ErrorResultSet += new SetDataTableEvent(EventErrorResultSet);
            _runSource.ProgressChange += new ProgressChangeEvent(EventProgressChange);
            _runSource.EndRun += new EndRunEvent(EventEndRun);

            //_runSource.SourceDir = _config.GetRootSubDir("SourceDir", "run", true);
            _runSource.SourceDir = _config.Get("SourceDir", "run").zSetRootDirectory();
            //_runSource.LoadParameters();

            string s = _config.Get("ProgressMinimumMillisecondsBetweenMessage");
            int progressMinimumMillisecondsBetweenMessage;
            if (s != null && int.TryParse(s, out progressMinimumMillisecondsBetweenMessage))
                _runSource.Progress_MinimumMillisecondsBetweenMessage = progressMinimumMillisecondsBetweenMessage;
        }

        private void endRunSource()
        {
            if (_runSource != null)
            {
                SaveParameters();
                //_runSource.SaveParameters();
                _runSource.Trace.Writed -= new WritedEvent(EventWrited);
                _runSource.DisableMessageChanged -= new DisableMessageChangedEvent(EventDisableMessageChanged);
                _runSource.GridResultSetDataTable -= new SetDataTableEvent(EventGridResultSetDataTable);
                _runSource.GridResultSetDataSet -= new SetDataSetEvent(EventGridResultSetDataSet);
                //_runSource.TreeViewResultAdd -= new TreeViewResultAddEvent(EventTreeViewResultAdd);
                //_runSource.TreeViewResultSelect -= new TreeViewResultSelectEvent(EventTreeViewResultSelect);
                _runSource.ErrorResultSet -= new SetDataTableEvent(EventErrorResultSet);
                _runSource.ProgressChange -= new ProgressChangeEvent(EventProgressChange);
                _runSource.EndRun -= new EndRunEvent(EventEndRun);
                _runSource = null;
            }
        }

        private void LoadParameters()
        {
            XmlParameters xp = _runSource.LoadParameters();
            string name = "RunSource_Form_";
            object o1, o2;
            o1 = xp.Get(name + "Form_WindowState");
            if (o1 != null && o1 is int)
                this.WindowState = (FormWindowState)(int)o1;
            o1 = xp.Get(name + "Form_Location_X");
            o2 = xp.Get(name + "Form_Location_Y");
            if (o1 != null && o1 is int && o2 != null && o2 is int)
                this.Location = new Point((int)o1, (int)o2);
            o1 = xp.Get(name + "Form_Size_Width");
            o2 = xp.Get(name + "Form_Size_Height");
            if (o1 != null && o1 is int && o2 != null && o2 is int)
                this.Size = new Size((int)o1, (int)o2);
            //o1 = xp.Get(name + "tc_result_Size_Height");
            //if (o1 != null && o1 is int)
            //    tc_result.Size = new Size(tc_result.Size.Width, (int)o1);
            o1 = xp.Get(name + "pan_top_Size_Height");
            if (o1 != null && o1 is int)
                pan_top.Size = new Size(pan_top.Size.Width, (int)o1);
        }

        private void SaveParameters()
        {
            XmlParameters xp = _runSource.CreateParameters();
            string name = "RunSource_Form_";
            xp.Set(name + "Form_WindowState", this.WindowState);
            xp.Set(name + "Form_Location_X", this.Location.X);
            xp.Set(name + "Form_Location_Y", this.Location.Y);
            xp.Set(name + "Form_Size_Width", this.Size.Width);
            xp.Set(name + "Form_Size_Height", this.Size.Height);
            //xp.Set(name + "tc_result_Size_Height", tc_result.Size.Height);
            xp.Set(name + "pan_top_Size_Height", pan_top.Size.Height);
            _runSource.SaveParameters();
        }

        private void fWRun_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(string.Format("fWRun_KeyDown : {0} Ctrl={1} Shift={2} Alt={3}", e.KeyCode.ToString(), e.Control, e.Shift, e.Alt));
            //Trace.WriteLine("fWRun_KeyDown : {0} Ctrl={1} Shift={2} Alt={3}", e.KeyCode.ToString(), e.Control, e.Shift, e.Alt);
            switch (e.KeyCode)
            {
                case Keys.F5:
                    //if (!e.Alt && !e.Control && !e.Shift)
                    //    Exe(new fExe(RunSource));
                    if (!e.Alt && !e.Control)
                    {
                        if (!e.Shift)
                            Exe(new fExe(RunSource));
                        else
                            Exe(new fExe(RunSourceOnMainThread));
                    }
                    break;
                //case Keys.Escape:
                //    if (!e.Alt && !e.Control && !e.Shift && gbThreadExecutionRunning)
                //    {
                //        DialogResult r = MessageBox.Show("Voulez-vous interrompre l'ex�cution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                //        if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
                //    }
                //    break;
                case Keys.A:
                    if (!e.Alt && e.Control && !e.Shift)
                        Exe(new fExe(SaveAs));
                    break;
                case Keys.B:
                    if (!e.Alt && e.Control && e.Shift)
                        Exe(new fExe(CompileSource));
                    break;
                case Keys.C:
                    // Ctrl-C
                    if (!e.Alt && e.Control && !e.Shift && _runSource.IsRunning())
                    {
                        DialogResult r = MessageBox.Show("Voulez-vous interrompre l'ex�cution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                        if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
                    }
                    // Shift-Ctrl-C
                    else if (!e.Alt && e.Control && e.Shift)
                        Exe(new fExe(_CompileRunSource));
                    break;
                case Keys.N:
                    if (!e.Alt && e.Control && !e.Shift)
                        Exe(new fExe(New));
                    break;
                case Keys.O:
                    if (!e.Alt && e.Control && !e.Shift)
                        Exe(new fExe(Open));
                    break;
                case Keys.R:  // d�sactiver Shift-Ctrl-R dans 4t Tray minimizer 02/10/2014
                    // Shift-Ctrl-R
                    if (!e.Alt && e.Control && e.Shift)
                        Exe(new fExe(_RestartRunSource));
                    break;
                case Keys.S:
                    if (!e.Alt && e.Control && !e.Shift)
                        Exe(new fExe(Save));
                    break;
                case Keys.U:
                    if (!e.Alt && e.Control && e.Shift)
                        Exe(new fExe(_UpdateRunSource));
                    break;
            }
        }

        private void me_source_TextChanged(object sender, EventArgs e)
        {
            SetFileNotSaved();
        }

        private void m_new_Click(object sender, EventArgs e)
        {
            Exe(new fExe(New));
        }

        private void m_open_Click(object sender, EventArgs e)
        {
            Exe(new fExe(Open));
        }

        private void m_save_Click(object sender, EventArgs e)
        {
            Exe(new fExe(Save));
        }

        private void m_save_as_Click(object sender, EventArgs e)
        {
            //cMenu.Exe(this, new cMenu.fExe(SaveAs));
            Exe(new fExe(SaveAs));
        }

        private void m_execute_Click(object sender, EventArgs e)
        {
            Exe(new fExe(RunSource));
        }

        private void m_execute_on_main_thread_Click(object sender, EventArgs e)
        {
            Exe(new fExe(RunSourceOnMainThread));
        }

        private void m_compile_Click(object sender, EventArgs e)
        {
            Exe(new fExe(CompileSource));
        }

        private void m_update_runsource_Click(object sender, EventArgs e)
        {
            Exe(new fExe(_UpdateRunSource));
        }

        private void m_compile_runsource_Click(object sender, EventArgs e)
        {
            Exe(new fExe(_CompileRunSource));
        }

        private void m_restart_runsource_Click(object sender, EventArgs e)
        {
            Exe(new fExe(_RestartRunSource));
        }

        private void m_quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void m_grid_set_max_width_height_Click(object sender, EventArgs e)
        {
            _setGridMaxColumnsWidthAndRowsHeight = !_setGridMaxColumnsWidthAndRowsHeight;
        }

        private void m_resize_datatable_images_Click(object sender, EventArgs e)
        {
            _resizeDataTableImages = !_resizeDataTableImages;
        }

        private void bt_execute_Click(object sender, EventArgs e)
        {
            if (!_runSource.IsRunning())
                Exe(new fExe(RunSource));
            else
            {
                DialogResult r = MessageBox.Show("Voulez-vous interrompre l'ex�cution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
            }
        }

        private void bt_pause_Click(object sender, EventArgs e)
        {
            bool pause = !_runSource.IsExecutionPaused();
            _runSource.PauseExecution(pause);
            if (pause)
                bt_pause.Text = "&Continue";
            else
                bt_pause.Text = "&Pause";
        }

        private void InitLog()
        {
            //string path = _config.GetRootSubPath("Log", null);
            string path = _config.Get("Log").zSetRootDirectory();
            if (path != null)
                _runSource.Trace.SetLogFile(path, LogOptions.IndexedFile);
        }

        private void New()
        {
            bool bCancel;

            ControlSave(out bCancel);
            if (bCancel) return;
            OpenSourceFile(null);
        }

        private void Open()
        {
            bool cancel;
            ControlSave(out cancel);
            if (cancel) return;
            string path = SelectOpenFile(_sourcePath);
            if (path != null)
            {
                OpenSourceFile(path);
            }
        }

        private void Save()
        {
            bool bCancel;
            Save(out bCancel);
        }

        private void Save(out bool bCancel)
        {
            bCancel = false;
            if (_sourcePath == null) SetPathSourceFile(SelectSaveFile(_sourcePath));
            if (_sourcePath == null)
            {
                bCancel = true;
                return;
            }
            if (!WriteSourceFile())
                bCancel = true;
        }

        private void SaveAs()
        {
            string sPath;

            sPath = SelectSaveFile(_sourcePath);
            if (sPath != null)
            {
                SetPathSourceFile(sPath);
                WriteSourceFile();
            }
        }

        private void ReadSourceFile()
        {
            try
            {
                if (_sourcePath != null && File.Exists(_sourcePath))
                    //me_source.Text = zfile.ReadFile(_sourcePath);
                    tb_source.Text = zfile.ReadAllText(_sourcePath);
                else
                    //me_source.Text = "";
                    tb_source.Text = "";
                //me_source.Select(0, 0);
                tb_source.Selection.Start = 0;
                tb_source.Selection.Length = 0;
                SetFileSaved();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
            }
        }

        private bool WriteSourceFile()
        {
            if (_fileSaved) return true;
            try
            {
                //zfile.WriteFile(_sourcePath, me_source.Text);
                zfile.WriteFile(_sourcePath, tb_source.Text);
                SetFileSaved();
                return true;
            }
            catch (Exception ex)
            {
                _runSource.Trace.WriteError(ex);
                MessageBox.Show("Error saving file : \r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ControlSave(out bool bCancel)
        {
            DialogResult dr;

            bCancel = false;
            if (!_fileSaved)
            {
                dr = MessageBox.Show("Le source a �t� modifi�.\r\nVoulez vous le sauvegarder ?", "Sauvegarde du source",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Cancel)
                    bCancel = true;
                else if (dr == DialogResult.Yes)
                {
                    Save(out bCancel);
                }
            }
        }

        private string SelectSaveFile(string sPathDefaut)
        {
            string sPath;
            SaveFileDialog sfDlg;
            DialogResult dr;

            sfDlg = new SaveFileDialog();
            sfDlg.InitialDirectory = _runSource.SourceDir;
            sfDlg.Filter = _sourceFilter;
            sfDlg.FilterIndex = 1;
            sfDlg.RestoreDirectory = true;
            sfDlg.FileName = sPathDefaut;
            dr = sfDlg.ShowDialog();

            sPath = null;
            if (dr == DialogResult.OK)
            {
                sPath = sfDlg.FileName;
            }
            return sPath;
        }

        private string SelectOpenFile(string sPathDefaut)
        {
            string sPath;
            OpenFileDialog ofDlg;
            DialogResult dr;

            ofDlg = new OpenFileDialog();
            ofDlg.InitialDirectory = _runSource.SourceDir;
            ofDlg.Filter = _sourceFilter;
            ofDlg.FilterIndex = 1;
            ofDlg.RestoreDirectory = true;
            ofDlg.FileName = sPathDefaut;
            ofDlg.CheckFileExists = false;
            dr = ofDlg.ShowDialog();

            sPath = null;
            if (dr == DialogResult.OK)
            {
                sPath = ofDlg.FileName;
            }
            return sPath;
        }

        private string GetSource()
        {
            //return me_source.SelectedText;
            return tb_source.Selection.Text;
        }

        private void SetFileSaved()
        {
            _fileSaved = true;
            SetFormTitle();
        }

        private void SetFileNotSaved()
        {
            _fileSaved = false;
            SetFormTitle();
        }

        private void SetFormTitle()
        {
            this.Text = __title;
            if (_sourcePath != null)
                this.Text += " : " + Path.GetFileName(_sourcePath);
            if (!_fileSaved)
                this.Text += "*";
            if (_runSource.Progress_PutProgressMessageToWindowsTitle && _progressText != null)
                this.Text += " - " + _progressText;
        }

        private void RunSource()
        {
            _RunSource();
        }

        private void RunSourceOnMainThread()
        {
            _RunSource(false);
        }

        private void _RunSource(bool useNewThread = true)
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est d�j� en cours d'ex�cution !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //if (me_source.SelectedText == "")
            if (tb_source.Selection.Text == "")
            {
                MessageBox.Show("Aucune instruction s�lectionn�e !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_sourcePath != null)
            {
                FileInfo file = new FileInfo(_sourcePath);
                if ((file.Attributes & FileAttributes.ReadOnly) == 0)
                    Save();
            }

            RazResult();
            RazProgress();

            bt_execute.Text = "&Stop";
            string s = GetSource();
            _runSource.Run(s, useNewThread);
        }

        private void CompileSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est d�j� en cours d'ex�cution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (_sourcePath == null) SetPathSourceFile(SelectSaveFile(_sourcePath));
            if (_sourcePath != null)
            {
                FileInfo file = new FileInfo(_sourcePath);
                if ((file.Attributes & FileAttributes.ReadOnly) == 0)
                    Save();
            }

            RazResult();
            RazProgress();
            bt_execute.Enabled = false;
            string s = GetSource();
            _runSource.GenerateWRSourceAndCompile(s);
            EventEndRun(false);
        }

        private void _UpdateRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est d�j� en cours d'ex�cution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            CompileRunSource();
            RestartRunSource();
        }

        private void _CompileRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est d�j� en cours d'ex�cution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            CompileRunSource();
        }

        private void _RestartRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est d�j� en cours d'ex�cution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            RestartRunSource();
        }

        private void CompileRunSource()
        {
            string updateDir = _config.GetExplicit("UpdateRunSource/UpdateDirectory");
            if (!Path.IsPathRooted(updateDir))
                updateDir = Path.Combine(zapp.GetAppDirectory(), updateDir);
            //Trace.WriteLine("CompileRunSource() : UpdateDirectory \"{0}\"", updateDir);
            string projectDir = _config.GetExplicit("UpdateRunSource/ProjectDirectory");
            Dictionary<string, List<string>> projectFiles = new Dictionary<string, List<string>>();

            //foreach (string project in _config.GetValues("UpdateRunSource/Project"))
            //{
            //    ICompiler compiler = Compile_Project(Path.Combine(projectDir, project));
            //    if (compiler.HasError)
            //        return;
            //    List<string> files = CopyProjectFiles(compiler, updateDir);
            //    projectFiles.Add(Path.GetFileName(project), files);
            //}

            foreach (XElement project in _config.GetElements("UpdateRunSource/Project"))
            {
                ICompiler compiler = Compile_Project(Path.Combine(projectDir, project.zExplicitAttribValue("value")));
                if (compiler.HasError)
                    return;
                //List<string> files = CopyProjectFiles(compiler, updateDir);
                //projectFiles.Add(Path.GetFileName(project), files);
                string copyOutput = project.zAttribValue("copyOutput");
                if (copyOutput != null)
                {
                    if (!Path.IsPathRooted(copyOutput))
                        copyOutput = Path.Combine(zapp.GetAppDirectory(), copyOutput);
                    Trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                    compiler.CopyResultFilesToDirectory(copyOutput);
                }
                Trace.WriteLine("  copy result files to directory \"{0}\"", updateDir);
                compiler.CopyResultFilesToDirectory(updateDir);
            }

            //foreach (string project in _config.GetValues("UpdateRunSource/ProjectRunSourceLaunch"))
            //{
            //    ICompiler compiler = Compile_Project(Path.Combine(projectDir, project));
            //    if (compiler.HasError)
            //        return;
            //    //CopyProjectFiles(compiler, updateDir);
            //}

            foreach (XElement project in _config.GetElements("UpdateRunSource/ProjectRunSourceLaunch"))
            {
                ICompiler compiler = Compile_Project(Path.Combine(projectDir, project.zExplicitAttribValue("value")));
                if (compiler.HasError)
                    return;
                string copyOutput = project.zAttribValue("copyOutput");
                if (copyOutput != null)
                {
                    if (!Path.IsPathRooted(copyOutput))
                        copyOutput = Path.Combine(zapp.GetAppDirectory(), copyOutput);
                    Trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                    compiler.CopyResultFilesToDirectory(copyOutput);
                }
            }

            //if (UpdateRunsourceFiles != null)
            //    UpdateRunsourceFiles(projectFiles);
        }

        private ICompiler Compile_Project(string projectPath)
        {
            ICompiler compiler = _runSource.Compile_Project(projectPath);
            if (compiler.HasError)
            {
                DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
                if (dtMessage != null)
                    SetResult(dtMessage, null);
                EventEndRun(false);
                //return null;
            }
            return compiler;
        }

        //private List<string> CopyProjectFiles(ICompiler compiler, string updateDir)
        //{
        //    List<string> outputFiles;
        //    if (compiler.CopyOutputFiles.Count > 0)
        //        outputFiles = compiler.CopyOutputFiles.First().Value;
        //    else
        //        outputFiles = compiler.OutputFiles;
        //    List<string> files = new List<string>();
        //    foreach (string file in outputFiles)
        //    {
        //        //string file2 = zfile.CopyFileToDirectory(file, updateDir, overwrite: true);
        //        string file2 = zfile.CopyFileToDirectory(file, updateDir, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //        if (file2 != null)
        //            files.Add(file2);
        //    }
        //    return files;
        //}

        private void RestartRunSource()
        {
            if (SetRestartRunsource != null)
                //SetRestartRunsource(new RunSourceRestartParameters { SourceFile = _sourcePath, SelectionStart = me_source.SelectionStart, SelectionLength = me_source.SelectionLength });
                SetRestartRunsource(new RunSourceRestartParameters { SourceFile = _sourcePath, SelectionStart = tb_source.Selection.Start, SelectionLength = tb_source.Selection.Length });
            this.Close();
        }

        private void AbortThreadExecution()
        {
            bool bCancel;
            AbortThreadExecution(out bCancel);
        }

        private void AbortThreadExecution(out bool bCancel)
        {
            bCancel = false;
            int iTimeout = 30;
            string sTimeout = _config.Get("AbortThreadExecutionTimeout");
            if (!int.TryParse(sTimeout, out iTimeout)) iTimeout = 30;
            _runSource.Trace.WriteLine("Abort execution process (Timeout {0} sec)", iTimeout);
            _runSource.AbortExecution(true);
            DateTime dt = DateTime.Now;
            while (true)
            {
                while (true)
                {
                    Application.DoEvents();
                    if (!_runSource.IsRunning())
                    {
                        _runSource.Trace.WriteLine("Execution process aborted");
                        return;
                    }
                    TimeSpan ts = DateTime.Now.Subtract(dt);
                    if (ts.Seconds > iTimeout) break;
                    Thread.Sleep(50);
                }
                DialogResult r = MessageBox.Show("Le programme est toujours en cours d'ex�cution. Voulez-vous forcer l'interruption ?", "RunSource",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Cancel)
                {
                    _runSource.AbortExecution(false);
                    bCancel = true;
                    return;
                }
                if (r == DialogResult.Yes)
                    break;
            }
            if (!_runSource.IsRunning())
            {
                _runSource.Trace.WriteLine("No thread execution process");
                return;
            }
            //WriteMessageLine("execution process : IsAlive = {0}, ThreadState = {1}", thread.IsAlive, thread.ThreadState.ToString());
            iTimeout = 30;
            sTimeout = _config.Get("ForceAbortThreadExecutionTimeout");
            if (!int.TryParse(sTimeout, out iTimeout)) iTimeout = 30;
            _runSource.Trace.WriteLine("Force abort of execution process (Timeout {0})", iTimeout);
            _runSource.ForceAbortExecution();
            dt = DateTime.Now;
            while (true)
            {
                //WriteMessageLine("execution process : IsAlive = {0}, ThreadState = {1}", thread.IsAlive, thread.ThreadState.ToString());
                Application.DoEvents();
                if (!_runSource.IsExecutionAlive())
                    break;
                Thread.Sleep(100);
                TimeSpan ts = DateTime.Now.Subtract(dt);
                if (ts.Seconds > iTimeout)
                {
                    _runSource.Trace.WriteLine("Impossible to abort execution process");
                    MessageBox.Show("Impossible to abort execution process.", "RunSource", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    bCancel = true;
                    return;
                }
            }
            //WriteMessageLine("execution process : IsAlive = {0}, ThreadState = {1}", thread.IsAlive, thread.ThreadState.ToString());
            _runSource.Trace.WriteLine("Execution process aborted");
        }

        private void EventEndRun(bool bError)
        {
            if (InvokeRequired)
            {
                EventEndRunCallback endRunSource = new EventEndRunCallback(EventEndRun);
                Invoke(endRunSource, bError);
            }
            else
            {
                try
                {
                    _runSource.Trace.WriteLine("Process completed {0}", _runSource.ExecutionChrono.TotalTimeString);
                    //_runSource.Trace.WriteLine("Process completed {0} (is running {1})", _runSource.ExecutionChrono.TotalTimeString, _runSource.IsRunning());
                }
                catch (Exception ex)
                {
                    zerrf.ErrorMessageBox(ex);
                }
                Control activeControl = ActiveControl;

                if (_newDataTableResult)
                    ViewDataTableResult();
                if (_newTreeViewResult)
                    ViewTreeViewResult();

                // on s�lectionne l'onglet sauf si :
                //   - DontSelectResultTab = true, gbErrorResult = false et bError = false
                if (!_runSource.DontSelectResultTab || _errorResult || bError)
                {
                    if (_selectTreeViewResult && !bError)
                        tc_result.SelectedTab = tab_result4;
                    else if (_newDataTableResult && !bError)
                    {
                        if (_xmlResultFormat != null || _dataSetResult != null)
                            tc_result.SelectedTab = tab_result;
                        else
                            tc_result.SelectedTab = tab_result2;
                    }
                    else if (_newTreeViewResult && !bError)
                        tc_result.SelectedTab = tab_result4;
                    else
                        tc_result.SelectedTab = tab_message;
                }

                ActiveControl = activeControl;
                bt_execute.Text = "&Run";
                bt_execute.Enabled = true;
                bt_pause.Text = "&Pause";
            }
        }

        public void OpenSourceFile(string path)
        {
            SetPathSourceFile(path);
            ReadSourceFile();
        }

        private void SetPathSourceFile(string sPathSource)
        {
            _sourcePath = sPathSource;
            _runSource.SourceDir = Path.GetDirectoryName(sPathSource);
            Directory.SetCurrentDirectory(_runSource.SourceDir);
            _runSource.ProjectName = sPathSource;
        }

        private void EventWrited(string sMsg)
        {
            if (InvokeRequired)
            {
                EventMessageSendCallback callback = new EventMessageSendCallback(EventWrited);
                Invoke(callback, sMsg);
            }
            else
            {
                Control activeControl = ActiveControl;
                tc_result.SelectedTab = tab_message;
                ActiveControl = activeControl;
                WriteMessage(sMsg);
            }
        }

        private void WriteMessage(string sMsg, params object[] prm)
        {
            if (_disableMessage)
                return;
            if (tb_message.Lines.Length > 1000)
            {
                tb_message.SuspendLayout();
                string[] sLines = new string[900];
                Array.Copy(tb_message.Lines, tb_message.Lines.Length - 900, sLines, 0, 900);
                tb_message.Lines = sLines;
                tb_message.ResumeLayout();
            }
            if (prm.Length != 0) sMsg = string.Format(sMsg, prm);
            tb_message.AppendText(sMsg);
        }

        private void EventDisableMessageChanged(bool disableMessage)
        {
            _disableMessage = disableMessage;
        }

        private void EventGridResultSetDataTable(DataTable dt, string sXmlFormat)
        {
            if (InvokeRequired)
            {
                SetDataTableEventCallback callback = new SetDataTableEventCallback(EventGridResultSetDataTable);
                Invoke(callback, dt, sXmlFormat);
            }
            else
            {
                SetResult(dt, sXmlFormat);
            }
        }

        private void EventGridResultSetDataSet(DataSet ds, string sXmlFormat)
        {
            if (InvokeRequired)
            {
                EventGridResultSetDataSetCallback callback = new EventGridResultSetDataSetCallback(EventGridResultSetDataSet);
                Invoke(callback, ds, sXmlFormat);
            }
            else
            {
                SetResult(ds, sXmlFormat);
            }
        }

        //private void EventTreeViewResultClear()
        //{
        //    if (InvokeRequired)
        //    {
        //        EventTreeViewResultClearCallback callback = new EventTreeViewResultClearCallback(EventTreeViewResultClear);
        //        Invoke(callback);
        //    }
        //    else
        //    {
        //        tree_result = new TreeView();
        //        tree_result.SuspendLayout();
        //    }
        //}

        //private void EventTreeViewResultAdd(string nodeName, XElement xmlElement, XFormat xFormat)
        //{
        //    if (InvokeRequired)
        //    {
        //        EventTreeViewResultAddCallback callback = new EventTreeViewResultAddCallback(EventTreeViewResultAdd);
        //        Invoke(callback, nodeName, xmlElement, xFormat);
        //    }
        //    else
        //    {
        //        tree_result.SuspendLayout();
        //        tree_result.zAddNode(nodeName, xmlElement, xFormat);
        //        _newTreeViewResult = true;
        //    }
        //}

        //private void EventTreeViewResultSelect()
        //{
        //    _selectTreeViewResult = true;
        //}

        private void EventErrorResultSet(DataTable dt, string sXmlFormat)
        {
            if (InvokeRequired)
            {
                SetDataTableEventCallback callback = new SetDataTableEventCallback(EventErrorResultSet);
                Invoke(callback, dt, sXmlFormat);
            }
            else
            {
                SetErrorResult(dt, sXmlFormat);
            }
        }

        private void SetResult(DataTable dt, string sXmlFormat)
        {
            _newDataTableResult = true;
            _dataTableResult = dt;
            _xmlResultFormat = sXmlFormat;
            _dataSetResult = null;
        }

        private void SetResult(DataSet ds, string sXmlFormat)
        {
            _newDataTableResult = true;
            _dataSetResult = ds;
            _xmlResultFormat = sXmlFormat;
            _dataTableResult = null;
        }

        private void SetErrorResult(DataTable dt, string sXmlFormat)
        {
            _newDataTableResult = true;
            _errorResult = true;
            _dataTableResult = dt;
            _xmlResultFormat = sXmlFormat;
            _dataSetResult = null;
        }

        private void RazResult()
        {
            // le r�sultat est conserv� d'un appel sur l'autre
            if (_errorResult)
            {
                _dataTableResult = null;
                _dataSetResult = null;
                ViewDataTableResult();
                _errorResult = false;
            }
            RazTreeViewResult();
            _newDataTableResult = false;
            _newTreeViewResult = false;
            _selectTreeViewResult = false;
        }

        private void ViewDataTableResult()
        {
            if (_dataTableResult != null)
            {
                ResizeDataTableImages(_dataTableResult);
                cGrid.GridSetDataSource(grid_result, _dataTableResult, _xmlResultFormat);

                _gridResult2.SuspendLayout();
                _gridResult2.DataSource = null;
                if (_gridResult2.DataSource != _dataTableResult.DefaultView)
                {
                    _gridResult2.DataSource = _dataTableResult.DefaultView;
                    _gridResult2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    _gridResult2.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    SetGridMaxColumnsWidthAndRowsHeight();
                    _gridResult2.AllowUserToOrderColumns = true;
                    _gridResult2.ReadOnly = false;
                }
                _gridResult2.ResumeLayout();
                grid_result3.DataSource = _dataTableResult.DefaultView;
            }
            else if (_dataSetResult != null)
            {
                cGrid.GridSetDataSource(grid_result, _dataSetResult.Tables[0], _xmlResultFormat);

                _gridResult2.SuspendLayout();
                _gridResult2.DataSource = _dataSetResult.Tables[0];
                _gridResult2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                _gridResult2.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                _gridResult2.AllowUserToOrderColumns = true;
                _gridResult2.ReadOnly = false;
                _gridResult2.ResumeLayout();

                grid_result3.DataSource = _dataSetResult;
            }
            else
            {
                cGrid.GridClearDataSource(grid_result);
                _gridResult2.DataSource = null;
                grid_result3.DataSource = null;
            }
        }

        private void ResizeDataTableImages(DataTable dataTable)
        {
            if (!_resizeDataTableImages || (_dataTableMaxImageWidth == 0 && _dataTableMaxImageHeight == 0))
                return;
            List<int> imageColumns = new List<int>();
            foreach (DataColumn column in dataTable.Columns)
            {
                //Trace.WriteLine(column.DataType.FullName);
                if (column.DataType == typeof(Image) || column.DataType.BaseType == typeof(Image))
                {
                    imageColumns.Add(column.Ordinal);
                }
            }

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (int indexColumn in imageColumns)
                {
                    object value = row[indexColumn];
                    if (value is Image || value.GetType().BaseType == typeof(Image))
                    //if (value is Bitmap)
                    {
                        Image image = (Image)value;
                        if (_dataTableMaxImageHeight != 0 && image.Height > _dataTableMaxImageHeight) // 200
                        {
                            row[indexColumn] = image.zResize(height: _dataTableMaxImageHeight); // 200
                        }
                        else if (_dataTableMaxImageWidth != 0 && image.Width > _dataTableMaxImageWidth)
                        {
                            row[indexColumn] = image.zResize(width: _dataTableMaxImageWidth);
                        }
                    }
                }
            }
        }

        private void SetGridMaxColumnsWidthAndRowsHeight()
        {
            if (!_setGridMaxColumnsWidthAndRowsHeight)
                return;
            if (_gridMaxWidth != 0)
            {
                foreach (DataGridViewColumn col in _gridResult2.Columns)
                {
                    if (col.Width > _gridMaxWidth)
                        col.Width = _gridMaxWidth;
                }
            }
            if (_gridMaxHeight != 0)
            {
                foreach (DataGridViewRow row in _gridResult2.Rows)
                {
                    if (row.Height > _gridMaxHeight)
                        row.Height = _gridMaxHeight;
                }
            }
        }

        private void RazTreeViewResult()
        {
            foreach (Control control in tab_result4.Controls)
                control.Dispose();
            tab_result4.Controls.Clear();
            tree_result = new TreeView();
            //tab_result4.Controls.Add(tree_result);
            tree_result.Dock = System.Windows.Forms.DockStyle.Fill;
            tree_result.Location = new System.Drawing.Point(0, 0);
            tree_result.Name = "tree_result";
            tree_result.Size = new System.Drawing.Size(1048, 364);
            tree_result.TabIndex = 0;
        }

        private void ViewTreeViewResult()
        {
            tree_result.ResumeLayout();
            tab_result4.Controls.Add(tree_result);

            //foreach (Control control in tab_result4.Controls)
            //    control.Dispose();
            //tab_result4.Controls.Clear();

            //tab_result4.Controls.Add(tree_result);
            //tree_result.Dock = System.Windows.Forms.DockStyle.Fill;
            //tree_result.Location = new System.Drawing.Point(0, 0);
            //tree_result.Name = "tree_result";
            //tree_result.Size = new System.Drawing.Size(1048, 364);
            //tree_result.TabIndex = 0;
        }

        private void EventProgressChange(int iCurrent, int iTotal, string sMessage, params object[] prm)
        {
            if (InvokeRequired)
            {
                EventProgressChangeCallback progress = new EventProgressChangeCallback(EventProgressChange);
                Invoke(progress, iCurrent, iTotal, sMessage, prm);
            }
            else
            {
                Progress(iCurrent, iTotal, sMessage, prm);
            }
        }

        private void RazProgress()
        {
            _progressText = null;
            lb_progress_label.Text = "";
            pb_progress_bar.Value = 0;
        }

        private void Progress(int iCurrent, int iTotal, string sMessage, params object[] prm)
        {
            if (sMessage != null)
            {
                //gsProgressText = string.Format(sMessage, prm) + string.Format(" ({0} / {1})", iCurrent, iTotal);
                _progressText = string.Format(sMessage, prm);
                lb_progress_label.Text = _progressText;
                if (_runSource.Progress_PutProgressMessageToWindowsTitle) SetFormTitle();
            }
            if (iTotal != 0)
                pb_progress_bar.Value = (int)((float)iCurrent / iTotal * 100 + 0.5);
            else
                pb_progress_bar.Value = 0;
        }

        private void grid_result2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Paint the row number on the row header.
            // The using statement automatically disposes the brush.
            using (SolidBrush b = new SolidBrush(_gridResult2.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }

        public void Exe(fExe f)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                f();
            }
            catch (Exception ex)
            {
                _runSource.Trace.WriteError(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        //private void initTestMenu()
        //{
        //    System.Windows.Forms.ToolStripMenuItem m_test = new System.Windows.Forms.ToolStripMenuItem();
        //    m_test.Name = "m_test";
        //    m_test.Size = new System.Drawing.Size(35, 20);
        //    m_test.Text = "&Test";
        //    this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { m_test });

        //    System.Windows.Forms.ToolStripMenuItem m_test1 = new System.Windows.Forms.ToolStripMenuItem();
        //    m_test1.Name = "m_test1";
        //    m_test1.Size = new System.Drawing.Size(186, 22);
        //    m_test1.Text = "Auto resize columns and rows from result 2 grid";
        //    m_test1.Click += new System.EventHandler(this.m_test1_Click);

        //    m_test.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { m_test1 });
        //}

        //private void m_test1_Click(object sender, EventArgs e)
        //{
        //    Exe(new fExe(test1));
        //}

        //private void test1()
        //{
        //    _gridResult2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        //    _gridResult2.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        //}
    }
}

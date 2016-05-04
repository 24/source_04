using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;
using pb;
using pb.Data;
using pb.IO;
using pb.Data.Xml;
using pb.Compiler;
using pb.Windows.Forms;
using ScintillaNET;

// todo :
//   - manage run init
//   - manage allow multiple run
//   - correct pb with modal dialog, scintilla send code character
//   - add status bar, line column, nb of thread running

namespace runsourced
{
    public partial class RunSourceForm : Form
    {
        private static string __title = "Run source";
        public delegate void fExe();
        private bool _disableMessage = false;
        private DataTable _dataTableResult = null;
        private DataSet _dataSetResult = null;
        private string _xmlResultFormat = null;
        private bool _newDataTableResult = false;       // true si il y a un nouveau résultat DataTable ou DataSet à afficher
        private bool _newTreeViewResult = false;        // true si il y a un nouveau résultat TreeView à afficher
        private bool _selectTreeViewResult = false;     // true si il faut sélectionner le résultat du TreeView
        // $$todo à supprimer
        //private bool _errorResult = false;              // true si le résultat est une liste d'erreurs
        //private bool _newMessage = false;             // true si il y a un nouveau message à afficher
        //private string _sourceFile = null;
        private bool _fileSaved = true;
        private string _sourceFilter = "source files (*.cs)|*.cs|All files (*.*)|*.*";
        private string _progressText = null;
        private bool _setGridMaxColumnsWidthAndRowsHeight = true;
        private int _gridMaxWidth = 0;
        private int _gridMaxHeight = 0;
        private bool _resizeDataTableImages = true;
        private int _dataTableMaxImageWidth = 0;
        private int _dataTableMaxImageHeight = 0;
        private ScintillaForm _scintillaForm = null;

        private IRunSource _runSource = null;
        private ITrace _trace = null;
        private XmlConfig _config = null;
        private RunSourceRestartParameters _runSourceParameters = null;
        private string _settingsProjectDirectory = null;

        public SetRestartRunsourceEvent SetRestartRunsource;

        public RunSourceForm(IRunSource runSource, ITrace trace, XmlConfig config, RunSourceRestartParameters runSourceParameters)
        {
            _runSource = runSource;
            _trace = trace;
            _config = config;
            _runSourceParameters = runSourceParameters;

            try
            {
                // http://stackoverflow.com/questions/9056418/resources-getobjectthis-icon-crashes-application-on-windows-xp
                // pour ne pas avoir une exection avec :
                // this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                // Thread.Sleep(100);
                // dont work

                InitializeComponent();
                this.Icon = Properties.Resources.app;
                string title = config.Get("RunsourceTitle");
                if (title != null)
                    __title = title;
                InitGrid();
                //tb_source.ConfigurationManager.CustomLocation = "ScintillaNET.xml";
                //tb_source.ConfigurationManager.Language = "cs";
                //InitScintilla();
                Try(InitScintilla);

                tc_result.SelectedTab = tab_message;
                ActiveControl = tb_source;

                cGrid.Culture = CultureInfo.CurrentUICulture;

                _gridMaxWidth = _config.Get("GridMaxWidth").zParseAs<int>();
                _gridMaxHeight = _config.Get("GridMaxHeight").zParseAs<int>();
                _dataTableMaxImageWidth = _config.Get("DataTableMaxImageWidth").zParseAs<int>();
                _dataTableMaxImageHeight = _config.Get("DataTableMaxImageHeight").zParseAs<int>();

                initRunSource();
                SetFileSaved();
            }
            catch (Exception ex)
            {
                _trace.WriteError(ex);
                zerrf.ErrorMessageBox(ex);
            }
        }

        private void InitScintilla()
        {
            _scintillaForm = new ScintillaForm(tb_source);
            _scintillaForm.SetFont("Consolas", 10);
            _scintillaForm.ConfigureLexerCpp();
            _scintillaForm.SetTabIndent(2, 0, false);
            _scintillaForm.DisplayLineNumber(4);
        }

        private void scintilla_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            switch (e.Text)
            {
                // remove ctrl-A code 0x01 (SaveAs)
                case "\u0001":
                // remove ctrl-B code 0x02 (CompileCode)
                case "\u0002":
                // remove ctrl-C code 0x03 (AbortThreadExecution)
                case "\u0003":
                // remove ctrl-N code 0x03 (New)
                case "\u000E":
                // remove ctrl-O code 0x0F (Open)
                case "\u000F":
                // remove shift-ctrl-R code 0x12 (_RestartRunSource)
                case "\u0012":
                // remove ctrl-S code 0x12 (Save)
                case "\u0013":
                // remove ctrl-U code 0x14 (_UpdateRunSource)
                case "\u0014":
                    e.Text = null;
                    break;
            }
        }

        private void ScintillaViewLineNumber()
        {
            int marginWidth = 0;
            if (m_view_source_line_number.Checked)
            {
                // Displaying Line Numbers https://github.com/jacobslusser/ScintillaNET/wiki/Displaying-Line-Numbers
                //tb_source.Margins[0].Width = 16;
                // Calculate the width required to display the last line number
                // and include some padding for good measure.
                const int padding = 2;
                const int maxLineNumberCharLength = 4;
                marginWidth = tb_source.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            }
            tb_source.Margins[0].Width = marginWidth;
        }

        private System.Windows.Forms.DataGridView _gridResult2;
        private void InitGrid()
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
            try
            {
                LoadSettings();
                if (_runSourceParameters != null)
                {
                    if (_runSourceParameters.SourceFile != null)
                        OpenSourceFile(_runSourceParameters.SourceFile);
                    //tb_source.Selection.Start = _runSourceParameters.SelectionStart;
                    //tb_source.Selection.Length = _runSourceParameters.SelectionLength;
                    //tb_source.Scrolling.ScrollToCaret();
                    tb_source.SelectionStart = _runSourceParameters.SelectionStart;
                    tb_source.SelectionEnd = _runSourceParameters.SelectionEnd;
                    tb_source.ScrollCaret();
                }

                // il faut afficher une fois le tab_result2 sinon _gridResult2.AutoResizeColumns() et _gridResult2.AutoResizeRows() ne marche pas la 1ère fois
                tc_result.SelectedTab = tab_result2;
                tc_result.SelectedTab = tab_message;
                ActiveControl = tb_source;
            }
            catch(Exception ex)
            {
                //_runSource.Trace.WriteError(ex);
                _trace.WriteError(ex);
                zerrf.ErrorMessageBox(ex);
            }
        }

        private void RunSourceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SaveSettings();
                EndRunSource();
            }
            catch (Exception ex)
            {
                //_runSource.Trace.WriteError(ex);
                _trace.WriteError(ex);
                zerrf.ErrorMessageBox(ex);
            }
        }

        private void RunSourceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool bCancel;

            if (_runSource.IsRunning())
            {
                DialogResult r = MessageBox.Show("Un programme est en cours d'exécution. Voulez-vous l'interrompre ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
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
            _trace.SetViewer(TraceWrited);
            _runSource.SetRunSourceConfig(_config.ConfigFile);
            _runSource.StartAssemblyResolve();
            _runSource.DeleteGeneratedAssemblies();
            //_trace.Writed += new WritedEvent(TraceWrited);
            //_trace.AddOnWrite("RunSourceForm", TraceWrited);
            //_trace.SetViewer(TraceWrited);
            _runSource.DisableMessageChanged += new DisableMessageChangedEvent(EventDisableMessageChanged);
            _runSource.GridResultSetDataTable += new SetDataTableEvent(EventGridResultSetDataTable);
            _runSource.GridResultSetDataSet += new SetDataSetEvent(EventGridResultSetDataSet);
            //_runSource.TreeViewResultAdd += new TreeViewResultAddEvent(EventTreeViewResultAdd);
            //_runSource.TreeViewResultSelect += new TreeViewResultSelectEvent(EventTreeViewResultSelect);
            //_runSource.ErrorResultSet += new SetDataTableEvent(EventErrorResultSet);
            _runSource.ProgressChange += new ProgressChangeEvent(EventProgressChange);
            //_runSource.EndRun += new EndRunEvent(EventEndRun);
            _runSource.EndRunCode += EventEndRunCode;

            //_runSource.SourceDir = _config.Get("SourceDir", "run").zSetRootDirectory();
            //_runSource.LoadParameters();

            string s = _config.Get("ProgressMinimumMillisecondsBetweenMessage");
            int progressMinimumMillisecondsBetweenMessage;
            if (s != null && int.TryParse(s, out progressMinimumMillisecondsBetweenMessage))
                _runSource.Progress_MinimumMillisecondsBetweenMessage = progressMinimumMillisecondsBetweenMessage;
            m_run_init.Checked = _runSource.CallInit;
            m_allow_multiple_execution.Checked = _runSource.AllowMultipleExecution;
        }

        private void EndRunSource()
        {
            if (_runSource != null)
            {
                _trace.SetViewer(null);
                _runSource.DisableMessageChanged -= new DisableMessageChangedEvent(EventDisableMessageChanged);
                _runSource.GridResultSetDataTable -= new SetDataTableEvent(EventGridResultSetDataTable);
                _runSource.GridResultSetDataSet -= new SetDataSetEvent(EventGridResultSetDataSet);
                _runSource.ProgressChange -= new ProgressChangeEvent(EventProgressChange);
                _runSource.EndRunCode -= EventEndRunCode;
                _runSource = null;
            }
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
                int panTopHeight = xmlSerializer.GetValue("pan_top_Height", this.pan_top.Size.Height);
                this.pan_top.Size = new Size(this.pan_top.Size.Width, panTopHeight);

                //_sourceDirectory = xmlSerializer.GetValue("SourceDirectory", _sourceDirectory);

                xmlSerializer.CloseElement();
            }

            if (xmlSerializer.OpenElement("RunSource") != null)
            {
                //_runSource.ProjectDirectory = xmlSerializer.GetValue("ProjectDirectory", _runSource.ProjectDirectory);
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
            xmlSerializer.AddValue("pan_top_Height", this.pan_top.Size.Height);

            //xmlSerializer.AddValue("SourceDirectory", _sourceDirectory);

            xmlSerializer.CloseElement();

            xmlSerializer.AddElement("RunSource");
            //xmlSerializer.AddValue("SourceDir", _runSource.SourceDir);
            xmlSerializer.AddValue("ProjectDirectory", _runSource.ProjectDirectory ?? _settingsProjectDirectory);
            xmlSerializer.CloseElement();

            xmlSerializer.Save(GetSettingsFile());
        }

        private string GetSettingsFile()
        {
            return zPath.Combine(zapp.GetLocalSettingsDirectory(_config.Get("RunsourceProductName")), "settings.xml");
        }

        private void RunSourceForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(string.Format("fWRun_KeyDown : {0} Ctrl={1} Shift={2} Alt={3}", e.KeyCode.ToString(), e.Control, e.Shift, e.Alt));
            //Trace.WriteLine("fWRun_KeyDown : {0} Ctrl={1} Shift={2} Alt={3}", e.KeyCode.ToString(), e.Control, e.Shift, e.Alt);
            switch (e.KeyCode)
            {
                case Keys.F5:
                    //if (!e.Alt && !e.Control && !e.Shift)
                    //    Exe(new fExe(RunSource));
                    if (!e.Alt && !e.Control && !e.Shift)
                        Exe(new fExe(RunCode));
                    else if (!e.Alt && !e.Control && e.Shift)
                        Exe(new fExe(RunCodeOnMainThread));
                    else if (!e.Alt && e.Control && !e.Shift)
                        Exe(new fExe(RunCodeWithoutProject));
                    break;
                //case Keys.Escape:
                //    if (!e.Alt && !e.Control && !e.Shift && gbThreadExecutionRunning)
                //    {
                //        DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                //        if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
                //    }
                //    break;
                case Keys.A:
                    if (!e.Alt && e.Control && !e.Shift)
                        Exe(new fExe(SaveAs));
                    break;
                case Keys.B:
                    if (!e.Alt && e.Control && e.Shift)
                        Exe(new fExe(CompileCode));
                    break;
                case Keys.C:
                    // Ctrl-C
                    if (!e.Alt && e.Control && !e.Shift && _runSource.IsRunning())
                    {
                        DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
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
                case Keys.R:  // désactiver Shift-Ctrl-R dans 4t Tray minimizer 02/10/2014
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

        private void tb_source_TextChanged(object sender, EventArgs e)
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
            Exe(new fExe(RunCode));
        }

        private void m_execute_on_main_thread_Click(object sender, EventArgs e)
        {
            Exe(new fExe(RunCodeOnMainThread));
        }

        private void m_execute_without_project_Click(object sender, EventArgs e)
        {
            Exe(new fExe(RunCodeWithoutProject));
        }

        private void m_compile_Click(object sender, EventArgs e)
        {
            Exe(new fExe(CompileCode));
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

        private void m_view_source_line_number_Click(object sender, EventArgs e)
        {
            ScintillaViewLineNumber();
        }

        private void bt_execute_Click(object sender, EventArgs e)
        {
            if (!_runSource.IsRunning())
                Exe(new fExe(RunCode));
            else
            {
                DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
            }
        }

        private void bt_stop_Click(object sender, EventArgs e)
        {
            if (_runSource.IsRunning())
            {
                DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du/des programme(s) ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
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

        //private void InitLog()
        //{
        //    string path = _config.Get("Log").zSetRootDirectory();
        //    if (path != null)
        //        _runSource.Trace.SetLogFile(path, LogOptions.IndexedFile);
        //}

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
            string path = SelectOpenFile(_runSource.SourceFile);
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
            if (_runSource.SourceFile == null)
                SetSourceFile(SelectSaveFile(_runSource.SourceFile));
            if (_runSource.SourceFile == null)
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

            sPath = SelectSaveFile(_runSource.SourceFile);
            if (sPath != null)
            {
                SetSourceFile(sPath);
                WriteSourceFile();
            }
        }

        private void ReadSourceFile()
        {
            try
            {
                if (_runSource.SourceFile != null && zFile.Exists(_runSource.SourceFile))
                    tb_source.Text = zfile.ReadAllText(_runSource.SourceFile);
                else
                    tb_source.Text = "";
                //tb_source.Selection.Start = 0;
                //tb_source.Selection.Length = 0;
                tb_source.SelectionStart = 0;
                tb_source.SelectionEnd = 0;
                SetFileSaved();
            }
            catch (Exception ex)
            {
                _trace.WriteError(ex);
                zerrf.ErrorMessageBox(ex);
            }
        }

        private bool WriteSourceFile()
        {
            if (_fileSaved) return true;
            try
            {
                zfile.WriteFile(_runSource.SourceFile, tb_source.Text);
                SetFileSaved();
                return true;
            }
            catch (Exception ex)
            {
                //_runSource.Trace.WriteError(ex);
                _trace.WriteError(ex);
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
                dr = MessageBox.Show("Le source a été modifié.\r\nVoulez vous le sauvegarder ?", "Sauvegarde du source",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.Cancel)
                    bCancel = true;
                else if (dr == DialogResult.Yes)
                {
                    Save(out bCancel);
                }
            }
        }

        private string SelectSaveFile(string defaultFile)
        {
            string file;
            SaveFileDialog saveFileDialog;
            DialogResult dr;

            saveFileDialog = new SaveFileDialog();
            //sfDlg.InitialDirectory = _runSource.SourceDir;
            //saveFileDialog.InitialDirectory = _sourceDirectory;
            saveFileDialog.InitialDirectory = _runSource.ProjectDirectory ?? _settingsProjectDirectory;
            saveFileDialog.Filter = _sourceFilter;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = defaultFile;
            dr = saveFileDialog.ShowDialog();

            file = null;
            if (dr == DialogResult.OK)
                file = saveFileDialog.FileName;
            return file;
        }

        private string SelectOpenFile(string defaultFile)
        {
            string file;
            OpenFileDialog openFileDialog;
            DialogResult dr;

            openFileDialog = new OpenFileDialog();
            //ofDlg.InitialDirectory = _runSource.SourceDir;
            //openFileDialog.InitialDirectory = _sourceDirectory;
            openFileDialog.InitialDirectory = _runSource.ProjectDirectory ?? _settingsProjectDirectory;
            openFileDialog.Filter = _sourceFilter;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileName = defaultFile;
            openFileDialog.CheckFileExists = false;
            dr = openFileDialog.ShowDialog();

            file = null;
            if (dr == DialogResult.OK)
                file = openFileDialog.FileName;
            return file;
        }

        private string GetCode()
        {
            //return tb_source.Selection.Text;
            return tb_source.SelectedText;
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
            string title = __title;
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

        private void RunCode()
        {
            _RunCode();
        }

        private void RunCodeOnMainThread()
        {
            _RunCode(useNewThread: false);
        }

        private void RunCodeWithoutProject()
        {
            _RunCode(compileWithoutProject: true);
        }

        private void _RunCode(bool useNewThread = true, bool compileWithoutProject = false)
        {
            if (_runSource.IsRunning() && !_runSource.AllowMultipleExecution)
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //if (tb_source.Selection.Text == "")
            if (GetCode() == "")
            {
                MessageBox.Show("Aucune instruction sélectionnée !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_runSource.SourceFile != null)
            {
                //FileInfo file = new FileInfo(_sourceFile);
                var file = zFile.CreateFileInfo(_runSource.SourceFile);
                if ((file.Attributes & FileAttributes.ReadOnly) == 0)
                    Save();
            }

            RazResult();
            RazProgress();

            //bt_execute.Text = "&Stop";
            bt_stop.Enabled = true;
            string s = GetCode();
            _runSource.RunCode(s, useNewThread, compileWithoutProject);
        }

        private void CompileCode()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (_runSource.SourceFile == null)
                SetSourceFile(SelectSaveFile(_runSource.SourceFile));
            if (_runSource.SourceFile != null)
            {
                //FileInfo file = new FileInfo(_sourceFile);
                var file = zFile.CreateFileInfo(_runSource.SourceFile);
                if ((file.Attributes & FileAttributes.ReadOnly) == 0)
                    Save();
            }

            RazResult();
            RazProgress();
            bt_execute.Enabled = false;
            string s = GetCode();
            _runSource.CompileCode(s);
            //EventEndRunCode(false);
            //EventEndRunCode(new EndRunCodeInfo { Error = false });
        }

        private void _UpdateRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            RestartRunSource();
        }

        private void CompileRunSource()
        {
            Chrono chrono = new Chrono();
            chrono.Start();
            int nbProject = 0;
            try
            {
                string updateDir = _config.GetExplicit("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
                Dictionary<string, List<string>> projectFiles = new Dictionary<string, List<string>>();

                foreach (XElement project in _config.GetElements("UpdateRunSource/Project"))
                {
                    ICompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
                    if (compiler.HasError())
                        return;
                    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
                    if (copyOutput != null)
                    {
                        //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                        compiler.CopyResultFilesToDirectory(copyOutput);
                    }
                    //_trace.WriteLine("  copy result files to directory \"{0}\"", updateDir);
                    compiler.CopyResultFilesToDirectory(updateDir);
                    nbProject++;
                }

                foreach (XElement project in _config.GetElements("UpdateRunSource/ProjectRunSourceLaunch"))
                {
                    ICompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
                    if (compiler.HasError())
                        return;
                    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
                    if (copyOutput != null)
                    {
                        //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                        compiler.CopyResultFilesToDirectory(copyOutput);
                    }
                    nbProject++;
                }
            }
            finally
            {
                chrono.Stop();
                _trace.WriteLine("{0} project(s) compiled", nbProject);
                _trace.WriteLine("Process completed {0}", chrono.TotalTimeString);
            }
        }

        private ICompiler CompileProject(string projectPath)
        {
            ICompiler compiler = _runSource.CompileProject(projectPath);
            if (compiler.HasError())
            {
                DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
                if (dtMessage != null)
                    SetResult(dtMessage, null);
                //EventEndRunCode(false);
                EventEndRunCode(new EndRunCodeInfo { Error = false });
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
                //SetRestartRunsource(new RunSourceRestartParameters { SourceFile = _runSource.SourceFile, SelectionStart = tb_source.Selection.Start, SelectionLength = tb_source.Selection.Length });
                SetRestartRunsource(new RunSourceRestartParameters { SourceFile = _runSource.SourceFile, SelectionStart = tb_source.SelectionStart, SelectionEnd = tb_source.SelectionEnd });
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
            //_runSource.Trace.WriteLine("Abort execution process (Timeout {0} sec)", iTimeout);
            _trace.WriteLine("Abort execution process (Timeout {0} sec)", iTimeout);
            _runSource.AbortExecution(true);
            DateTime dt = DateTime.Now;
            while (true)
            {
                while (true)
                {
                    Application.DoEvents();
                    if (!_runSource.IsRunning())
                    {
                        //_runSource.Trace.WriteLine("Execution process aborted");
                        _trace.WriteLine("Execution process aborted");
                        return;
                    }
                    TimeSpan ts = DateTime.Now.Subtract(dt);
                    if (ts.Seconds > iTimeout) break;
                    Thread.Sleep(50);
                }
                DialogResult r = MessageBox.Show("Le programme est toujours en cours d'exécution. Voulez-vous forcer l'interruption ?", "RunSource",
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
                //_runSource.Trace.WriteLine("No thread execution process");
                _trace.WriteLine("No thread execution process");
                return;
            }
            //WriteMessageLine("execution process : IsAlive = {0}, ThreadState = {1}", thread.IsAlive, thread.ThreadState.ToString());
            iTimeout = 30;
            sTimeout = _config.Get("ForceAbortThreadExecutionTimeout");
            if (!int.TryParse(sTimeout, out iTimeout)) iTimeout = 30;
            //_runSource.Trace.WriteLine("Force abort of execution process (Timeout {0})", iTimeout);
            _trace.WriteLine("Force abort of execution process (Timeout {0})", iTimeout);
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
                    //_runSource.Trace.WriteLine("Impossible to abort execution process");
                    _trace.WriteLine("Impossible to abort execution process");
                    MessageBox.Show("Impossible to abort execution process.", "RunSource", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    bCancel = true;
                    return;
                }
            }
            //WriteMessageLine("execution process : IsAlive = {0}, ThreadState = {1}", thread.IsAlive, thread.ThreadState.ToString());
            //_runSource.Trace.WriteLine("Execution process aborted");
            _trace.WriteLine("Execution process aborted");
        }

        //private void EventEndRun(bool bError)
        private void EventEndRunCode(EndRunCodeInfo endRunCodeInfo)
        {
            if (InvokeRequired)
            {
                //EventEndRunCallback endRunSource = new EventEndRunCallback(EventEndRun);
                //Invoke(endRunSource, bError);

                // Error 13 Cannot convert lambda expression to type 'System.Delegate' because it is not a delegate type
                //Invoke(() => { EventEndRunCode(endRunCodeInfo); });

                Invoke((Action)(() => EventEndRunCode(endRunCodeInfo)));
            }
            else
            {
                try
                {
                    //_runSource.Trace.WriteLine("Process completed {0}", _runSource.ExecutionChrono.TotalTimeString);
                    //_trace.WriteLine("Process completed {0}", _runSource.RunCodeChrono.TotalTimeString);
                    //_trace.WriteLine("Process completed {0}", endRunCodeInfo.RunCodeChrono.TotalTimeString);
                    _trace.Write("Process completed ");
                    if (endRunCodeInfo.RunCodeChrono != null)
                        _trace.Write(endRunCodeInfo.RunCodeChrono.TotalTimeString);
                    else
                        _trace.Write("----");
                    _trace.WriteLine();
                    //_runSource.Trace.WriteLine("Process completed {0} (is running {1})", _runSource.ExecutionChrono.TotalTimeString, _runSource.IsRunning());
                }
                catch (Exception ex)
                {
                    _trace.WriteError(ex);
                    zerrf.ErrorMessageBox(ex);
                }
                Control activeControl = ActiveControl;

                try
                {

                    if (_newDataTableResult)
                        ViewDataTableResult();
                    if (_newTreeViewResult)
                        ViewTreeViewResult();

                    bool error = endRunCodeInfo.Error;

                    // on sélectionne l'onglet sauf si :
                    //   - DontSelectResultTab = true, gbErrorResult = false et bError = false
                    //if (!_runSource.DontSelectResultTab || _errorResult || error)
                    if (!_runSource.DontSelectResultTab || error)
                    {
                        if (_selectTreeViewResult && !error)
                            tc_result.SelectedTab = tab_result4;
                        else if (_newDataTableResult && !error)
                        {
                            if (_xmlResultFormat != null || _dataSetResult != null)
                                tc_result.SelectedTab = tab_result;
                            else
                                tc_result.SelectedTab = tab_result2;
                        }
                        else if (_newTreeViewResult && !error)
                            tc_result.SelectedTab = tab_result4;
                        else
                            tc_result.SelectedTab = tab_message;
                    }
                }
                catch(Exception ex)
                {
                    _trace.WriteError(ex);
                    //zerrf.ErrorMessageBox(ex);
                }

                ActiveControl = activeControl;
                //bt_execute.Text = "&Run";
                bt_execute.Enabled = true;
                bt_pause.Text = "&Pause";
                bt_stop.Enabled = _runSource.IsRunning();

                SetFormTitle();
            }
        }

        public void OpenSourceFile(string path)
        {
            SetSourceFile(path);
            ReadSourceFile();
        }

        //private void SetPathSourceFile(string sPathSource)
        //{
        //    _sourcePath = sPathSource;
        //    _runSource.SourceDir = zPath.GetDirectoryName(sPathSource);
        //    Directory.SetCurrentDirectory(_runSource.SourceDir);
        //    _runSource.ProjectName = sPathSource;
        //}

        private void SetSourceFile(string file)
        {
            //_sourceFile = file;
            _runSource.SourceFile = file;
            _runSource.SetProject(file);
        }

        private void TraceWrited(string msg)
        {
            if (InvokeRequired)
            {
                //EventMessageSendCallback callback = new EventMessageSendCallback(TraceWrited);
                //Invoke(callback, msg);

                Invoke((Action)(() => TraceWrited(msg)));
            }
            else
            {
                if (!_newDataTableResult)
                {
                    Control activeControl = ActiveControl;
                    tc_result.SelectedTab = tab_message;
                    ActiveControl = activeControl;
                }
                WriteMessage(msg);
            }
        }

        private void WriteMessage(string msg, params object[] prm)
        {
            if (_disableMessage || msg == null)
                return;
            if (tb_message.Lines.Length > 1000)
            {
                tb_message.SuspendLayout();
                string[] sLines = new string[900];
                Array.Copy(tb_message.Lines, tb_message.Lines.Length - 900, sLines, 0, 900);
                tb_message.Lines = sLines;
                tb_message.ResumeLayout();
            }
            if (prm.Length != 0)
                msg = string.Format(msg, prm);
            tb_message.AppendText(msg);
        }

        private void EventDisableMessageChanged(bool disableMessage)
        {
            _disableMessage = disableMessage;
        }

        private void EventGridResultSetDataTable(DataTable dt, string sXmlFormat)
        {
            if (InvokeRequired)
            {
                //SetDataTableEventCallback callback = new SetDataTableEventCallback(EventGridResultSetDataTable);
                //Invoke(callback, dt, sXmlFormat);

                Invoke((Action)(() => EventGridResultSetDataTable(dt, sXmlFormat)));
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
                //EventGridResultSetDataSetCallback callback = new EventGridResultSetDataSetCallback(EventGridResultSetDataSet);
                //Invoke(callback, ds, sXmlFormat);

                Invoke((Action)(() => EventGridResultSetDataSet(ds, sXmlFormat)));
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

        //private void EventErrorResultSet(DataTable dt, string sXmlFormat)
        //{
        //    if (InvokeRequired)
        //    {
        //        //SetDataTableEventCallback callback = new SetDataTableEventCallback(EventErrorResultSet);
        //        //Invoke(callback, dt, sXmlFormat);

        //        Invoke((Action)(() => EventErrorResultSet(dt, sXmlFormat)));
        //    }
        //    else
        //    {
        //        SetErrorResult(dt, sXmlFormat);
        //    }
        //}

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

        //private void SetErrorResult(DataTable dt, string sXmlFormat)
        //{
        //    _newDataTableResult = true;
        //    _errorResult = true;
        //    _dataTableResult = dt;
        //    _xmlResultFormat = sXmlFormat;
        //    _dataSetResult = null;
        //}

        private void RazResult()
        {
            // le résultat est conservé d'un appel sur l'autre
            //if (_errorResult)
            //{
            //    _dataTableResult = null;
            //    _dataSetResult = null;
            //    ViewDataTableResult();
            //    _errorResult = false;
            //}
            _dataTableResult = null;
            _dataSetResult = null;
            ViewDataTableResult();

            RazTreeViewResult();
            _newDataTableResult = false;
            _newTreeViewResult = false;
            _selectTreeViewResult = false;
        }

        private void ViewDataTableResult()
        {
            if (_dataTableResult != null)
            {
                Try(() => ResizeDataTableImages(_dataTableResult));
                Try(() => cGrid.GridSetDataSource(grid_result, _dataTableResult, _xmlResultFormat));

                Try(() =>
                {
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
                });

                Try(() => grid_result3.DataSource = _dataTableResult.DefaultView);
            }
            else if (_dataSetResult != null)
            {
                Try(() => cGrid.GridSetDataSource(grid_result, _dataSetResult.Tables[0], _xmlResultFormat));

                Try(() =>
                {
                    _gridResult2.SuspendLayout();
                    _gridResult2.DataSource = _dataSetResult.Tables[0];
                    _gridResult2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    _gridResult2.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    _gridResult2.AllowUserToOrderColumns = true;
                    _gridResult2.ReadOnly = false;
                    _gridResult2.ResumeLayout();
                });

                Try(() => grid_result3.DataSource = _dataSetResult);
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
                //EventProgressChangeCallback progress = new EventProgressChangeCallback(EventProgressChange);
                //Invoke(progress, iCurrent, iTotal, sMessage, prm);

                Invoke((Action)(() => EventProgressChange(iCurrent, iTotal, sMessage, prm)));
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
                if (_runSource.Progress_PutProgressMessageToWindowsTitle)
                    SetFormTitle();
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
                //_runSource.Trace.WriteError(ex);
                _trace.WriteError(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _trace.WriteError(ex);
            }
        }

        private void m_run_init_Click(object sender, EventArgs e)
        {
            _runSource.CallInit = m_run_init.Checked;
        }

        private void m_allow_multiple_execution_Click(object sender, EventArgs e)
        {
            _runSource.AllowMultipleExecution = m_allow_multiple_execution.Checked;
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

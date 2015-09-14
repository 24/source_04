using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;

namespace Test_RunSource
{
    public partial class Test_RunSourceForm : Form
    {
        #region variable
        public delegate void fExe();
        private bool _disableMessage = false;
        //private DataTable _resultTable = null;
        //private DataSet _resultSet = null;
        //private string _xmlResultFormat = null;
        //private bool _dataTableResult = false;       // true si il y a un nouveau résultat DataTable ou DataSet à afficher
        //private bool _treeViewResult = false;        // true si il y a un nouveau résultat TreeView à afficher
        //private bool _selectTreeViewResult = false;  // true si il faut sélectionner le résultat du TreeView
        //private bool _errorResult = false;           // true si le résultat est une liste d'erreurs
        //private string _pathSource = null;
        //private bool _fileSaved = true;
        //private string _sourceFilter = "source files (*.cs)|*.cs|All files (*.*)|*.*";
        //private string _progressText = null;

        //private RemoteRunSource _remoteRunSource = null;
        //private IRunSource _runSource = null;
        //private bool _threadExecutionRunning = false;

        private delegate void EventMessageSendCallback(string msg, params object[] prm);
        private delegate void SetDataTableEventCallback(DataTable dt, string xmlFormat);
        private delegate void EventGridResultSetDataSetCallback(DataSet ds, string xmlFormat);
        private delegate void EventTreeViewResultClearCallback();
        private delegate void EventTreeViewResultAddCallback(string nodeName, XElement xmlElement, XFormat format);
        private delegate void EventProgressChangeCallback(int current, int total, string message, params object[] prm);
        private delegate void EventEndRunCallback(bool error);
        #endregion

        public Test_RunSourceForm()
        {
            InitializeComponent();
            tb_source.Text = "RunSource.CurrentDomainRunSource.Trace.WriteLine(DateTime.Now.ToString());\r\n";
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            tc_result.SelectedTab = tab_message;
            ActiveControl = tb_source;
            //cGrid.Culture = CultureInfo.CurrentUICulture;
            //initRunSource();
        }

        private void Test_RunSourceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //endRunSource();
        }

        private void Test_RunSourceForm_Load(object sender, EventArgs e)
        {
        }

        //private void initRunSource()
        //{
        //    endRunSource();
        //    _remoteRunSource = new RemoteRunSource();
        //    _runSource = _remoteRunSource.GetRunSource();
        //    _runSource.SetRunSourceConfig(Program.Config.ConfigPath);
        //    //_runSource.Trace.Writed += new WritedEvent(EventWrited);
        //    _runSource.Writed += new WritedEvent2(EventWrited2);
        //    //_runSource.DisableMessageChanged += new DisableMessageChangedEvent(EventDisableMessageChanged);
        //    //_runSource.GridResultSetDataTable += new SetDataTableEvent(EventGridResultSetDataTable);
        //    //_runSource.GridResultSetDataSet += new SetDataSetEvent(EventGridResultSetDataSet);
        //    //_runSource.TreeViewResultAdd += new TreeViewResultAddEvent(EventTreeViewResultAdd);
        //    //_runSource.TreeViewResultSelect += new TreeViewResultSelectEvent(EventTreeViewResultSelect);
        //    //_runSource.ErrorResultSet += new SetDataTableEvent(EventErrorResultSet);
        //    //_runSource.ProgressChange += new ProgressChangeEvent(EventProgressChange);
        //    //_runSource.EndRun += new EndRunEvent(EventEndRun);

        //    _runSource.SourceDir = Program.Config.GetRootSubDir("SourceDir", "WRun", true);
        //    _runSource.LoadParameters();

        //    //string s = Program.Config.Get("ProgressMinimumMillisecondsBetweenMessage/@value");
        //    string s = Program.Config.Get("ProgressMinimumMillisecondsBetweenMessage");
        //    int progressMinimumMillisecondsBetweenMessage;
        //    if (s != null && int.TryParse(s, out progressMinimumMillisecondsBetweenMessage)) _runSource.Progress_MinimumMillisecondsBetweenMessage = progressMinimumMillisecondsBetweenMessage;
        //}

        //private void endRunSource()
        //{
        //    if (_remoteRunSource != null)
        //    {
        //        _runSource.SaveParameters();
        //        //_runSource.Trace.Writed -= new WritedEvent(EventWrited);
        //        _runSource.Writed -= new WritedEvent2(EventWrited2);
        //        //_runSource.DisableMessageChanged -= new DisableMessageChangedEvent(EventDisableMessageChanged);
        //        //_runSource.GridResultSetDataTable -= new SetDataTableEvent(EventGridResultSetDataTable);
        //        //_runSource.GridResultSetDataSet -= new SetDataSetEvent(EventGridResultSetDataSet);
        //        //_runSource.TreeViewResultAdd -= new TreeViewResultAddEvent(EventTreeViewResultAdd);
        //        //_runSource.TreeViewResultSelect -= new TreeViewResultSelectEvent(EventTreeViewResultSelect);
        //        //_runSource.ErrorResultSet -= new SetDataTableEvent(EventErrorResultSet);
        //        //_runSource.ProgressChange -= new ProgressChangeEvent(EventProgressChange);
        //        //_runSource.EndRun -= new EndRunEvent(EventEndRun);

        //        _remoteRunSource.Dispose();
        //        _remoteRunSource = null;
        //        _runSource = null;
        //    }
        //}

        private void Test_RunSourceForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    //if (!e.Alt && !e.Control && !e.Shift)
                    //    Exe(new fExe(RunSource));
                    break;
                //case Keys.Escape:
                //    if (!e.Alt && !e.Control && !e.Shift && gbThreadExecutionRunning)
                //    {
                //        DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                //        if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
                //    }
                //    break;
                case Keys.A:
                    //if (!e.Alt && e.Control && !e.Shift)
                    //    Exe(new fExe(SaveAs));
                    break;
                case Keys.B:
                    //if (!e.Alt && e.Control && e.Shift)
                    //    Exe(new fExe(CompileSource));
                    break;
                case Keys.C:
                    //if (!e.Alt && e.Control && !e.Shift && _threadExecutionRunning)
                    //{
                    //    DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                    //    if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
                    //}
                    break;
                case Keys.N:
                    //if (!e.Alt && e.Control && !e.Shift)
                    //    Exe(new fExe(New));
                    break;
                case Keys.O:
                    //if (!e.Alt && e.Control && !e.Shift)
                    //    Exe(new fExe(Open));
                    break;
                case Keys.S:
                    //if (!e.Alt && e.Control && !e.Shift)
                    //    Exe(new fExe(Save));
                    break;
            }
        }

        private void EventWrited0(string msg, params object[] prm)
        {
            if (InvokeRequired)
            {
                //EventMessageSendCallback callback = new EventMessageSendCallback(EventWrited);
                //Invoke(callback, msg, prm);
            }
            else
            {
                Control activeControl = ActiveControl;
                tc_result.SelectedTab = tab_message;
                ActiveControl = activeControl;
                WriteMessage(msg, prm);
            }
        }

        private void EventDisableMessageChanged(bool disableMessage)
        {
            _disableMessage = disableMessage;
        }

        private void EventGridResultSetDataTable(DataTable dt, string xmlFormat)
        {
            if (InvokeRequired)
            {
                SetDataTableEventCallback callback = new SetDataTableEventCallback(EventGridResultSetDataTable);
                Invoke(callback, dt, xmlFormat);
            }
            else
            {
                //SetResult(dt, xmlFormat);
            }
        }

        private void EventGridResultSetDataSet(DataSet ds, string xmlFormat)
        {
            if (InvokeRequired)
            {
                EventGridResultSetDataSetCallback callback = new EventGridResultSetDataSetCallback(EventGridResultSetDataSet);
                Invoke(callback, ds, xmlFormat);
            }
            else
            {
                //SetResult(ds, xmlFormat);
            }
        }

        private void EventTreeViewResultAdd(string nodeName, XElement xmlElement, XFormat format)
        {
            if (InvokeRequired)
            {
                EventTreeViewResultAddCallback callback = new EventTreeViewResultAddCallback(EventTreeViewResultAdd);
                Invoke(callback, nodeName, xmlElement, format);
            }
            //else
            //{
            //    //tree_result.SuspendLayout();
            //    //tree_result.zAddNode(nodeName, xmlElement, format);
            //    _treeViewResult = true;
            //}
        }

        //private void EventTreeViewResultSelect()
        //{
        //    _selectTreeViewResult = true;
        //}

        private void EventErrorResultSet(DataTable dt, string xmlFormat)
        {
            if (InvokeRequired)
            {
                SetDataTableEventCallback callback = new SetDataTableEventCallback(EventErrorResultSet);
                Invoke(callback, dt, xmlFormat);
            }
            else
            {
                //SetErrorResult(dt, xmlFormat);
            }
        }

        private void EventProgressChange(int current, int total, string message, params object[] prm)
        {
            if (InvokeRequired)
            {
                EventProgressChangeCallback progress = new EventProgressChangeCallback(EventProgressChange);
                Invoke(progress, current, total, message, prm);
            }
            else
            {
                //Progress(current, total, message, prm);
            }
        }

        private void EventEndRun(bool error)
        {
        }

        //private void EventEndRun(bool error)
        //{
        //    if (InvokeRequired)
        //    {
        //        EventEndRunCallback endRunSource = new EventEndRunCallback(EventEndRun);
        //        Invoke(endRunSource, error);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            _runSource.Trace.WriteLine("Process completed {0}", _runSource.ExecutionChrono.TotalTimeString);
        //        }
        //        catch (Exception ex)
        //        {
        //            cf.ErrorMessageBox(ex);
        //        }
        //        Control activeControl = ActiveControl;

        //        //if (_dataTableResult)
        //        //    ViewDataTableResult();
        //        //if (_treeViewResult)
        //        //    ViewTreeViewResult();


        //        // on sélectionne l'onglet sauf si :
        //        //   - DontSelectResultTab = true, gbErrorResult = false et bError = false
        //        if (!_runSource.DontSelectResultTab || _errorResult || error)
        //        {
        //            if (_selectTreeViewResult && !error)
        //                tc_result.SelectedTab = tab_result4;
        //            else if (_dataTableResult && !error)
        //            {
        //                if (_xmlResultFormat != null || _resultSet != null)
        //                    tc_result.SelectedTab = tab_result1;
        //                else
        //                    tc_result.SelectedTab = tab_result2;
        //            }
        //            else if (_treeViewResult && !error)
        //                tc_result.SelectedTab = tab_result4;
        //            else
        //                tc_result.SelectedTab = tab_message;
        //        }



        //        ActiveControl = activeControl;
        //        _threadExecutionRunning = false;
        //        bt_run.Text = "&Run";
        //        bt_run.Enabled = true;
        //        _runSource.Pause = false;
        //        bt_pause.Text = "&Pause";
        //    }
        //}

        //private void RunSource()
        //{
        //    if (_threadExecutionRunning)
        //    {
        //        MessageBox.Show("Un programme est déjà en cours d'exécution !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }

        //    //if (_pathSource != null)
        //    //{
        //    //    FileInfo file = new FileInfo(_pathSource);
        //    //    if ((file.Attributes & FileAttributes.ReadOnly) == 0)
        //    //        Save();
        //    //}

        //    //RazResult();
        //    //RazProgress();

        //    //_threadExecutionRunning = true;
        //    bt_run.Text = "&Stop";
        //    string s = GetSource();
        //    _runSource.Run(s);
        //}

        private string GetSource()
        {
            string cmd = tb_source.SelectedText;
            if (cmd == "") cmd = tb_source.Text;
            return cmd;
        }

        //public void Exe(fExe f)
        //{
        //    try
        //    {
        //        this.Cursor = Cursors.WaitCursor;
        //        f();
        //    }
        //    catch (Exception ex)
        //    {
        //        //_runSource.Trace.WriteError(ex);
        //        WriteMessage(ex.ToString() + "\r\n");
        //    }
        //    finally
        //    {
        //        this.Cursor = Cursors.Default;
        //    }
        //}

        private void WriteMessage(string msg, params object[] prm)
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
            if (prm.Length != 0) msg = string.Format(msg, prm);
            tb_message.AppendText(msg);
        }

        private void bt_run_Click(object sender, EventArgs e)
        {
            //Test_RunSource();
            Test_RunSource_Thread();
        }

        private static string TimeSpanToString(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        private void EventWrited2(string msg)
        {
        }

        private void Test_RunSource_Thread()
        {
            Thread thread = new Thread(new ThreadStart(Test_RunSource));
            thread.Start();
        }

        private void Test_RunSource()
        {
            //private RemoteRunSource _remoteRunSource = null;
            //private IRunSource _runSource = null;
            RemoteRunSource remoteRunSource = new RemoteRunSource();
            IRunSource runSource = remoteRunSource.GetRunSource();
            //runSource.SetRunSourceConfig(Program.Config.ConfigPath);
            //Trace.CurrentTrace.Writed += new WritedEvent(EventWrited2);
            Trace.CurrentTrace.SetViewer(EventWrited2);
            //_runSource.DisableMessageChanged += new DisableMessageChangedEvent(EventDisableMessageChanged);
            //_runSource.GridResultSetDataTable += new SetDataTableEvent(EventGridResultSetDataTable);
            //_runSource.GridResultSetDataSet += new SetDataSetEvent(EventGridResultSetDataSet);
            //_runSource.TreeViewResultAdd += new TreeViewResultAddEvent(EventTreeViewResultAdd);
            //_runSource.TreeViewResultSelect += new TreeViewResultSelectEvent(EventTreeViewResultSelect);
            //_runSource.ErrorResultSet += new SetDataTableEvent(EventErrorResultSet);
            //_runSource.ProgressChange += new ProgressChangeEvent(EventProgressChange);
            runSource.EndRunCode += new EndRunEvent(EventEndRun);

            //runSource.SourceDir = Program.Config.GetRootSubDir("SourceDir", "WRun", true);
            //runSource.SourceDir = Program.Config.Get("SourceDir", "WRun").zRootPath(zapp.GetAppDirectory());
            runSource.ProjectDirectory = Program.Config.Get("SourceDir", "WRun").zRootPath(zapp.GetAppDirectory());
            //runSource.LoadParameters();

            Test_RunSource1(runSource);
            //Test_RunSource2(runSource);
            //Test_RunSource3(runSource);

            //runSource.SaveParameters();
            //Trace.CurrentTrace.Writed -= new WritedEvent(EventWrited2);
            Trace.CurrentTrace.SetViewer(null);
            //_runSource.DisableMessageChanged -= new DisableMessageChangedEvent(EventDisableMessageChanged);
            //_runSource.GridResultSetDataTable -= new SetDataTableEvent(EventGridResultSetDataTable);
            //_runSource.GridResultSetDataSet -= new SetDataSetEvent(EventGridResultSetDataSet);
            //_runSource.TreeViewResultAdd -= new TreeViewResultAddEvent(EventTreeViewResultAdd);
            //_runSource.TreeViewResultSelect -= new TreeViewResultSelectEvent(EventTreeViewResultSelect);
            //_runSource.ErrorResultSet -= new SetDataTableEvent(EventErrorResultSet);
            //_runSource.ProgressChange -= new ProgressChangeEvent(EventProgressChange);
            runSource.EndRunCode -= new EndRunEvent(EventEndRun);

            remoteRunSource.Dispose();
            remoteRunSource = null;
            runSource = null;
        }

        private void Test_RunSource1(IRunSource runSource)
        {
            string s = GetSource();
            DateTime dt1 = DateTime.Now;
            WriteMessage("{0:HH:mm:ss} RunSource\r\n", dt1);
            //Exe(new fExe(RunSource));
            Test_RunSource(runSource, s);

            WriteMessage("wait 6 min\r\n");
            Thread.Sleep(360000);

            DateTime dt2 = DateTime.Now;
            WriteMessage("{0:HH:mm:ss}-{1} RunSource\r\n", dt2, TimeSpanToString(dt2.Subtract(dt1)));
            //Exe(new fExe(RunSource));
            Test_RunSource(runSource, s);
        }

        private void Test_RunSource2(IRunSource runSource)
        {
            string s = GetSource();
            DateTime dt1 = DateTime.Now;
            WriteMessage("{0:HH:mm:ss} RunSource\r\n", dt1);
            //Exe(new fExe(RunSource));
            Test_RunSource(runSource, s);

            for (int i = 0; i < 10; i++)
            {
                WriteMessage("wait 1 min\r\n");
                Thread.Sleep(60000);

                DateTime dt2 = DateTime.Now;
                WriteMessage("{0:HH:mm:ss}-{1} RunSource\r\n", dt2, TimeSpanToString(dt2.Subtract(dt1)));
                //Exe(new fExe(RunSource));
                Test_RunSource(runSource, s);
            }
        }

        private void Test_RunSource3(IRunSource runSource)
        {
            string s = GetSource();
            DateTime dt1 = DateTime.Now;
            WriteMessage("{0:HH:mm:ss} RunSource\r\n", dt1);
            //Exe(new fExe(RunSource));
            Test_RunSource(runSource, s);

            DateTime dt2;

            for (int i = 0; i < 6; i++)
            {
                WriteMessage("wait 1 min\r\n");
                Thread.Sleep(60000);
                dt2 = DateTime.Now;
                WriteMessage("{0:HH:mm:ss}-{1} KeepAlive\r\n", dt2, TimeSpanToString(dt2.Subtract(dt1)));
                //runSource.KeepAlive();
            }

            dt2 = DateTime.Now;
            WriteMessage("{0:HH:mm:ss}-{1} RunSource\r\n", dt2, TimeSpanToString(dt2.Subtract(dt1)));
            //Exe(new fExe(RunSource));
            Test_RunSource(runSource, s);
        }

        private void Test_RunSource(IRunSource runSource, string s)
        {
            try
            {
                runSource.RunCode(s);
            }
            catch (Exception ex)
            {
                WriteMessage(ex.ToString() + "\r\n");
            }
        }

        private void Test_RunSourceForm_Shown(object sender, EventArgs e)
        {
            //Test_RunSource();
        }
    }
}

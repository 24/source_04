using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pb;
using pb.Windows.Forms;

namespace Pib
{
    public partial class MainForm : Form //FormWinProc
    {
        private delegate void FormCallback();
        private delegate void ProgressCallback(Label label, ProgressBar progressBar, long current, long total, string message);
        private delegate void WriteLogCallback(TextBox tb, string msg);
        private string gsTitle = null;
        private string gsProgressText = null;
        private bool gbPutMessageToWindowsTitle = false;
        private bool gbSetFormTitleInProgress = false;
        private ITrace gTrace = null;
        private ProgressControl gTask1ProgressControl = null;
        private ProgressControl gTask1DetailProgressControl = null;

        #region constructor
        public MainForm()
        {
            gTrace = Trace.CurrentTrace;
            Init();
        }

        //public MainForm(cTraced trace)
        public MainForm(ITrace trace)
        {
            gTrace = trace;
            Init();
        }
        #endregion

        #region PutMessageToWindowsTitle
        public bool PutMessageToWindowsTitle
        {
            get { return gbPutMessageToWindowsTitle; }
            set { gbPutMessageToWindowsTitle = value; }
        }
        #endregion

        #region Task1ProgressControl
        public ProgressControl Task1ProgressControl
        {
            get { return gTask1ProgressControl; }
        }
        #endregion

        #region Task1DetailProgressControl
        public ProgressControl Task1DetailProgressControl
        {
            get { return gTask1DetailProgressControl; }
        }
        #endregion

        public void Init()
        {
            InitializeComponent();
            this.Icon = main.gIcon;
            //this.Owner

            //gTrace.Writed += new WritedEvent(WriteLog);
            gTrace.SetViewer(WriteLog);

            //string logFile = gTrace.GetLogFile();
            //if (logFile != null && File.Exists(logFile))
            //    WriteLog(zfile.ReadAllText(logFile));
            gsTitle = this.Text;
            this.TextChanged += new EventHandler(MainForm_TextChanged);
            gTask1ProgressControl = new ProgressControl(task1_lb_progress_label1, task1_pb_progress_bar1);
            gTask1DetailProgressControl = new ProgressControl(task1_lb_progress_label2, task1_pb_progress_bar2);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //gTrace.Writed -= new WritedEvent(WriteLog);
            gTrace.SetViewer(null);
        }

        #region MainForm_TextChanged
        private void MainForm_TextChanged(object sender, EventArgs e)
        {
            if (gbSetFormTitleInProgress) return;
            gsTitle = this.Text;
            SetFormTitle();
        }
        #endregion

        public void WriteLog(string msg)
        {
            _WriteLog(tb_log, msg);
        }

        #region ShowTask1
        public void ShowTask1()
        {
            task1_lb_progress_label1.Text = null;
            task1_pb_progress_bar1.Value = 0;
            task1_lb_progress_label2.Text = null;
            task1_pb_progress_bar2.Value = 0;
            split_task1.Visible = true;
            pan_task1.Visible = true;
        }
        #endregion

        #region HideTask1
        public void HideTask1()
        {
            pan_task1.Visible = false;
            split_task1.Visible = false;
        }
        #endregion

        #region //Task1SetProgressText
        //public void Task1SetProgressText(string text)
        //{
        //    gsProgressText = text;
        //    //gbPutMessageToWindowsTitle = putMessageToWindowsTitle;
        //    SetProgressText(task1_lb_progress_label1, task1_pb_progress_bar1, text);
        //    if (gbPutMessageToWindowsTitle) SetFormTitle();
        //}
        #endregion

        #region //Task1SetProgress
        //public void Task1Progress(long current, long total, string message, bool putMessageToWindowsTitle)
        //public void Task1SetProgress(long current, long total)
        //{
        //    //gsProgressText = message;
        //    //gbPutMessageToWindowsTitle = putMessageToWindowsTitle;
        //    SetProgress(task1_lb_progress_label1, task1_pb_progress_bar1, current, total);
        //    if (gbPutMessageToWindowsTitle) SetFormTitle();
        //}
        #endregion

        #region //Task1ProgressDetail
        //public void Task1ProgressDetail(long current, long total, string message, bool putMessageToWindowsTitle)
        //public void Task1ProgressDetail(long current, long total, string message)
        //{
        //    SetProgress(task1_lb_progress_label2, task1_pb_progress_bar2, current, total, message);
        //}
        #endregion

        #region //SetProgress
        //public static void SetProgress(Label label, ProgressBar progressBar, long current, long total, string message)
        //{
        //    if (progressBar.InvokeRequired)
        //    {
        //        ProgressCallback progress = new ProgressCallback(SetProgress);
        //        progressBar.Invoke(progress, label, progressBar, current, total, message);
        //    }
        //    else
        //    {
        //        //gsProgressText = sMessage;
        //        //gbPutMessageToWindowsTitle = bPutMessageToWindowsTitle;
        //        label.Text = message;
        //        //if (bPutMessageToWindowsTitle) SetFormTitle();
        //        if (total != 0)
        //            progressBar.Value = (int)((float)current / total * 100 + 0.5);
        //        else
        //            progressBar.Value = 0;
        //    }
        //}
        #endregion

        #region //RemoveProgressText
        //public void RemoveProgressText()
        //{
        //    gsProgressText = null;
        //    if (gbPutMessageToWindowsTitle) SetFormTitle();
        //}
        #endregion

        #region Task1WriteLog
        public void Task1WriteLog(string msg)
        {
            _WriteLog(task1_tb_log, msg);
        }
        #endregion

        #region _WriteLog
        private static void _WriteLog(TextBox tb, string msg)
        {
            if (tb.InvokeRequired)
            {
                WriteLogCallback writeLog = new WriteLogCallback(_WriteLog);
                tb.Invoke(writeLog, tb, msg);
            }
            else
            {
                if (msg == null) return;
                if (tb.Lines.Length > 1000)
                {
                    tb.SuspendLayout();
                    string[] sLines = new string[900];
                    Array.Copy(tb.Lines, tb.Lines.Length - 900, sLines, 0, 900);
                    tb.Lines = sLines;
                    tb.ResumeLayout();
                }
                tb.AppendText(msg);
            }
        }
        #endregion

        #region SetFormTitle
        private void SetFormTitle()
        {
            if (InvokeRequired)
            {
                FormCallback call = new FormCallback(SetFormTitle);
                Invoke(call);
            }
            else
            {
                gbSetFormTitleInProgress = true;
                string sTitle = gsTitle;
                if (gbPutMessageToWindowsTitle && gsProgressText != null) sTitle += " - " + gsProgressText;
                Text = sTitle;
                gbSetFormTitleInProgress = false;
            }
        }
        #endregion

        private void m_trace_winproc_Click(object sender, EventArgs e)
        {
            WinProc.TraceWinProcList();
        }
    }
}

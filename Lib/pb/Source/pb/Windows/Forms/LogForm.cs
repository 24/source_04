using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public partial class LogForm : Form
    {
        private delegate void ProgressCallback(int iCurrent, int iTotal, string sMessage, bool bPutMessageToWindowsTitle);
        private string gsTitle = null;
        private string gsProgressText = null;
        private bool gbPutMessageToWindowsTitle = false;
        //private ITrace gTrace = null;

        public LogForm()
        {
            //gTrace = Trace.CurrentTrace;
            Init();
        }

        //public LogForm(ITrace trace)
        //{
        //    gTrace = trace;
        //    Init();
        //}

        public void Init()
        {
            InitializeComponent();

            //Trace.CurrentTrace.Writed += new WritedEvent(WriteLog);
            Trace.CurrentTrace.SetViewer(WriteLog);
            //string logFile = gTrace.GetLogFile();
            //if (logFile != null && File.Exists(logFile))
            //    WriteLog(zfile.ReadAllText(logFile));
            gsTitle = this.Text;
            this.TextChanged += new EventHandler(LogForm_TextChanged);
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Trace.CurrentTrace.Writed -= new WritedEvent(WriteLog);
            Trace.CurrentTrace.SetViewer(null);
        }

        #region LogForm_TextChanged
        void LogForm_TextChanged(object sender, EventArgs e)
        {
            gsTitle = this.Text;
            SetFormTitle();
        }
        #endregion

        #region WriteLog
        public void WriteLog(string msg)
        {
            if (msg == null) return;
            if (tb_log.Lines.Length > 1000)
            {
                tb_log.SuspendLayout();
                string[] sLines = new string[900];
                Array.Copy(tb_log.Lines, tb_log.Lines.Length - 900, sLines, 0, 900);
                tb_log.Lines = sLines;
                tb_log.ResumeLayout();
            }
            tb_log.AppendText(msg);
        }
        #endregion

        #region ShowProgress
        public void ShowProgress()
        {
            pan_progress.Visible = true;
        }
        #endregion

        #region HideProgress
        public void HideProgress()
        {
            pan_progress.Visible = false;
        }
        #endregion

        #region Progress
        public void Progress(int iCurrent, int iTotal, string sMessage, bool bPutMessageToWindowsTitle)
        {
            if (InvokeRequired)
            {
                ProgressCallback progress = new ProgressCallback(Progress);
                Invoke(progress, iCurrent, iTotal, sMessage, bPutMessageToWindowsTitle);
            }
            else
            {
                gsProgressText = sMessage;
                gbPutMessageToWindowsTitle = bPutMessageToWindowsTitle;
                lb_progress_label.Text = sMessage;
                if (bPutMessageToWindowsTitle) SetFormTitle();
                if (iTotal != 0)
                    pb_progress_bar.Value = (int)((float)iCurrent / iTotal * 100 + 0.5);
                else
                    pb_progress_bar.Value = 0;
            }
        }
        #endregion

        #region SetFormTitle
        private void SetFormTitle()
        {
            string sTitle = gsTitle;
            if (gbPutMessageToWindowsTitle && gsProgressText != null) sTitle += " - " + gsProgressText;
            Text = sTitle;
        }
        #endregion
    }
}

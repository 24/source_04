using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pb
{
    public class Progress
    {
        #region variable
        private long glValue = 0;
        private long glTotal = 0;
        private string gsProgressText = null;
        //private string gsFinalProgressMessage = null;
        //private bool gbAddValueToMessage = true;
        //private bool gbPutMessageToWindowsTitle = true;
        private int giMinimumTimeBetweenMessage = 0; // in milliseconds
        private DateTime gLastProgressUpdateTime;

        //private IProgressControl gProgressControl = null;

        //public delegate void ProgressControlChangedEventHandler(IProgressControl progressControl);
        //public event ProgressControlChangedEventHandler ProgressControlChanged;

        public delegate void ProgressChangedEventHandler(long current, long total);
        public event ProgressChangedEventHandler ProgressChanged;
        public delegate void ProgressTextChangedEventHandler(string progressMessage);
        public event ProgressTextChangedEventHandler ProgressTextChanged;
        #endregion

        #region constructor
        public Progress()
        {
        }

        //public Progress(IProgressControl progressControl)
        //{
        //    gProgressControl = progressControl;
        //}
        #endregion

        #region property ...
        #region //Value
        //public long Value
        //{
        //    get { return glValue; }
        //    set
        //    {
        //        glValue = value;
        //        UpdateProgress();
        //    }
        //}
        #endregion

        #region //Total
        //public long Total
        //{
        //    get { return glTotal; }
        //    set
        //    {
        //        glTotal = value;
        //        UpdateProgress();
        //    }
        //}
        #endregion

        #region //AddValueToMessage
        //public bool AddValueToMessage
        //{
        //    get { return gbAddValueToMessage; }
        //    set
        //    {
        //        gbAddValueToMessage = value;
        //        UpdateProgress();
        //    }
        //}
        #endregion

        #region //PutMessageToWindowsTitle
        //public bool PutMessageToWindowsTitle
        //{
        //    get { return gbPutMessageToWindowsTitle; }
        //    set
        //    {
        //        gbPutMessageToWindowsTitle = value;
        //        UpdateProgress();
        //    }
        //}
        #endregion

        #region MinimumTimeBetweenMessage
        public int MinimumTimeBetweenMessage
        {
            get { return giMinimumTimeBetweenMessage; }
            set { giMinimumTimeBetweenMessage = value; }
        }
        #endregion

        #region //ProgressControl
        //public IProgressControl ProgressControl
        //{
        //    get { return gProgressControl; }
        //    set
        //    {
        //        gProgressControl = value;
        //        if (ProgressControlChanged != null) ProgressControlChanged(value);
        //    }
        //}
        #endregion
        #endregion

        #region SetProgressText(string text)
        public void SetProgressText(string text)
        {
            gsProgressText = text;
            UpdateProgressText();
        }
        #endregion

        #region SetProgressText(string text, params object[] prm)
        public void SetProgressText(string text, params object[] prm)
        {
            if (text != null && prm.Length > 0) text = string.Format(text, prm);
            gsProgressText = text;
            UpdateProgressText();
        }
        #endregion

        #region //SetProgress
        //public void SetProgress(long value, long total, string text, params object[] prm)
        //{
        //    glValue = value;
        //    glTotal = total;
        //    if (text != null && prm.Length > 0) text = string.Format(text, prm);
        //    gsProgressText = text;
        //    UpdateProgress();
        //}
        #endregion

        #region SetProgress(long value, long total)
        public void SetProgress(long value, long total)
        {
            glValue = value;
            glTotal = total;
            UpdateProgress();
        }
        #endregion

        #region UpdateProgress
        private void UpdateProgress()
        {
            if (giMinimumTimeBetweenMessage > 0)
            {
                DateTime t = DateTime.Now;
                if (t.Subtract(gLastProgressUpdateTime).TotalMilliseconds < giMinimumTimeBetweenMessage) return;
                gLastProgressUpdateTime = t;
            }
            if (glTotal < glValue) glTotal = glValue;
            if (ProgressChanged != null) ProgressChanged(glValue, glTotal);
            //if (gProgressControl != null) gProgressControl.SetProgress(glValue, glTotal);
        }
        #endregion

        #region UpdateProgressText
        private void UpdateProgressText()
        {
            if (ProgressTextChanged != null) ProgressTextChanged(gsProgressText);
            //if (gProgressControl != null) gProgressControl.SetProgressText(gsProgressText);
        }
        #endregion
    }
}

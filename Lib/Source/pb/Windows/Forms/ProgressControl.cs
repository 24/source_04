using System;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class ProgressControl
    {
        private Label gLabel = null;
        private ProgressBar gProgressBar = null;
        private string gText = null;
        private string gFinalText = null;
        private long gProgressValue = 0;
        private long gProgressTotal = 0;
        private bool gbAddProgressValueToText = true;

        public delegate void ProgressChangedEventHandler(long progressValue, long progressTotal);
        public event ProgressChangedEventHandler ProgressChanged;
        public delegate void ProgressTextChangedEventHandler(string progressText);
        public event ProgressTextChangedEventHandler ProgressTextChanged;

        private delegate void Callback();

        public ProgressControl(Label label, ProgressBar progressBar)
        {
            gLabel = label;
            gProgressBar = progressBar;
        }

        public bool AddProgressValueToText
        {
            get { return gbAddProgressValueToText; }
            set
            {
                gbAddProgressValueToText = value;
                SetText();
            }
        }

        public void SetProgressText(string text)
        {
            gText = text;
            SetText();
        }

        public void SetProgressText(string text, params object[] prm)
        {
            if (text != null && prm.Length > 0) text = string.Format(text, prm);
            gText = text;
            SetText();
        }

        private void SetText()
        {
            gFinalText = gText;
            if (gbAddProgressValueToText && gProgressTotal != 0)
                gFinalText += string.Format(" ({0} / {1})", gProgressValue, gProgressTotal);
            SetTextLabel();
            if (ProgressTextChanged != null) ProgressTextChanged(gFinalText);
        }

        private void SetTextLabel()
        {
            if (gLabel.InvokeRequired)
                gLabel.Invoke(new Callback(SetTextLabel));
            else
                gLabel.Text = gFinalText;
        }

        public void SetProgress(long progressValue, long progressTotal)
        {
            gProgressValue = progressValue;
            gProgressTotal = progressTotal;
            SetProgressBar();
            if (gbAddProgressValueToText)
                SetText();
            if (ProgressChanged != null) ProgressChanged(progressValue, progressTotal);
        }

        public void SetProgressBar()
        {
            if (gProgressBar.InvokeRequired)
                gProgressBar.Invoke(new Callback(SetProgressBar));
            else
            {
                if (gProgressTotal != 0)
                    gProgressBar.Value = (int)((float)gProgressValue / gProgressTotal * 100 + 0.5);
                else
                    gProgressBar.Value = 0;
            }
        }
    }
}

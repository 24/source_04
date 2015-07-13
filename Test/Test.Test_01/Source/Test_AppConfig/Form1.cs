using System;
//using System.Linq;
using System.Windows.Forms;
using pb;

// http://localhost:8701/Test_wcf_service_3/Service1/
// http://localhost:8701/Test_wcf_service_3/Service1/GetString_Get

namespace Test.Test_AppConfig
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Trace.CurrentTrace.SetViewer(WriteText);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Trace.CurrentTrace.SetViewer(null);
        }

        private void Go()
        {
            Exec(Test_AppConfig.Test);
        }

        private void Exec(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        private void WriteText(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(WriteText), msg);
            }
            else
            {
                this.tb_log.AppendText(msg);
            }
        }

        private void bt_go_Click(object sender, EventArgs e)
        {
            Go();
        }
    }
}

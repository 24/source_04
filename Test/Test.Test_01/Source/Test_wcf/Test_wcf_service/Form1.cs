using System;
//using System.Linq;
using System.Windows.Forms;
using pb;

// http://localhost:8701/Test_wcf_service_3/Service1/
// http://localhost:8701/Test_wcf_service_3/Service1/GetString_Get

namespace Test_wcf_service
{
    public partial class Form1 : Form
    {
        private ServiceHostManager _host = null;

        public Form1()
        {
            InitializeComponent();
            Trace.CurrentTrace.SetViewer(WriteText);
            _host = new ServiceHostManager();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Trace.CurrentTrace.SetViewer(null);
        }

        private void StartService()
        {
            StopService();
            Exec(_host.StartService);
        }

        private void StopService()
        {
            Exec(_host.StopService);
        }

        private void Exec(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex.Message);
                //Trace.WriteLine(ex.StackTrace);
                Trace.WriteLine(Error.GetErrorMessage(ex, true, true));
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

        private void bt_start_service_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void bt_stop_service_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void bt_trace_messages_Click(object sender, EventArgs e)
        {
            Exec(() =>
            {
                CorsMessageInspector.Trace = !CorsMessageInspector.Trace;
                bt_trace_messages.Text = CorsMessageInspector.Trace ? "Stop trace messages" : "Start trace messages";
            });
        }

        private void bt_trace_service_host_Click(object sender, EventArgs e)
        {
            Exec(() =>
            {
                ServiceHostManager.Trace = !ServiceHostManager.Trace;
                bt_trace_service_host.Text = ServiceHostManager.Trace ? "Stop trace service host" : "Start trace service host";
            });
        }
    }
}

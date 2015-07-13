using System;
using System.Linq;
using System.Windows.Forms;

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
            Trace.OnWrite += WriteText;
            _host = new ServiceHostManager();
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
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        //private delegate void WriteTextCallback(string msg);
        private void WriteText(string msg)
        {
            if (InvokeRequired)
            {
                //EventMessageSendCallback callback = new EventMessageSendCallback(EventWrited);
                //Invoke(() => { WriteText(); }, msg);
                //Invoke(() => { WriteText(msg); });
                //Invoke(WriteText, msg);
                //Invoke(new WriteTextCallback(WriteText), msg);
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
    }
}

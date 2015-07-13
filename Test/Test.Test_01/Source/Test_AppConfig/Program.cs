using System;
using System.Windows.Forms;
using pb;

namespace Test.Test_AppConfig
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Trace.CurrentTrace.SetWriter("_log.txt");
            Trace.WriteLine("Test_AppConfig : {0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            Trace.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

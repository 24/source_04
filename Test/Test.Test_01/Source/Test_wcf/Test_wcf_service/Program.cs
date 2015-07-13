using System;
using System.Windows.Forms;
using pb;
using pb.IO;

namespace Test_wcf_service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Trace.CurrentTrace.SetWriter("log\\_log.txt", FileOption.IndexedFile);
            Trace.WriteLine("Test_wcf_service : {0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            Trace.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

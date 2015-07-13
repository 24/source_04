using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            WriteToFile writeToFile = new WriteToFile("log.txt");
            Trace.OnWrite += writeToFile.Write;
            Trace.WriteLine("Test_wcf_service : {0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);
            Trace.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

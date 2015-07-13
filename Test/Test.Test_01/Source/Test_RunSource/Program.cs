using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using pb;
using pb.Data.Xml;
using pb.Windows.Forms;

namespace Test_RunSource
{
    static class Program
    {
        private static XmlConfig _config = null;

        [STAThread]
        static void Main()
        {
            try
            {
                _config = new XmlConfig();
                FormatInfo.SetInvariantCulture();
                Application.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Test_RunSourceForm());
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
            }
        }

        public static XmlConfig Config { get { return _config; } }
    }
}
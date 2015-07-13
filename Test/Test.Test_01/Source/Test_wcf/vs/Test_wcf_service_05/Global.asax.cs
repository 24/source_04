using System;
using pb;
using pb.IO;

namespace Test_wcf_service
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            CorsMessageInspector.Trace = true;
            Trace.CurrentTrace.SetWriter("log\\_log.txt", FileOption.IndexedFile);
            Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - Test_wcf_service :", DateTime.Now);
            Trace.WriteLine();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Trace.CurrentTrace.Close();
        }
    }
}

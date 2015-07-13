using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using pb;
using pb.IO;

namespace Test_wcf_service
{
    public class Global : System.Web.HttpApplication
    {
        private static bool __trace = true;
        private static bool __traceOption = true;
        private static string __traceOptionFile = null;
        private static bool __traceToMemory = true;
        private static string __traceToMemoryFile = null;
        private static bool __OptionsMessage = false;

        protected void Application_Start(object sender, EventArgs e)
        {
            Trace.CurrentTrace.SetWriter("log\\_log.txt", FileOption.IndexedFile);
            Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - Test_wcf_service :", DateTime.Now);
            Trace.WriteLine();
            __traceOptionFile = zfile.GetNewIndexedFileName(Path.Combine(zapp.GetAppDirectory(), "log"), "_log_options.txt");
            File.Create(__traceOptionFile).Close();
            __traceToMemoryFile = zfile.GetNewIndexedFileName(Path.Combine(zapp.GetAppDirectory(), "log"), "_log_memory.txt");
            File.Create(__traceToMemoryFile).Close();
            Trace.WriteLine("  Trace                : {0}", __trace);
            Trace.WriteLine("  Trace file           : \"{0}\"", Trace.CurrentTrace.GetWriterFile());
            Trace.WriteLine("  Trace option         : {0}", __traceOption);
            Trace.WriteLine("  Trace option file    : \"{0}\"", __traceOptionFile);
            Trace.WriteLine("  Trace to memory      : {0}", __traceToMemory);
            Trace.WriteLine("  Trace to memory file : \"{0}\"", __traceToMemoryFile);
            if (__trace)
            {
                Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - Application_Start", DateTime.Now);
                Trace.WriteLine();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;
            HttpResponse httpResponse = HttpContext.Current.Response;

            foreach (var header in CorsHeaders.CurrentCorsHeaders.AllMessageHeaders)
                httpResponse.AddHeader(header.Key, header.Value);

            if (__trace)
            {
                Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - Application_BeginRequest :", DateTime.Now);
            }
            if (__traceToMemory)
                TraceToMemory.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - Application_BeginRequest :", DateTime.Now);

            if (httpRequest.HttpMethod == "OPTIONS")
            {
                __OptionsMessage = true;

                if (__trace)
                {
                    Trace.WriteLine("  set options headers : {0}", httpRequest.HttpMethod);
                }
                if (__traceOption)
                    zfile.WriteFile(__traceOptionFile, string.Format("{0:dd/MM/yyyy HH:mm:ss} - set options headers : {1}\r\n", DateTime.Now, httpRequest.HttpMethod), append: true);

                if (__traceToMemory)
                    TraceToMemory.WriteLine("  set options headers : {0}", httpRequest.HttpMethod);

                foreach (var header in CorsHeaders.CurrentCorsHeaders.OptionsMessageHeaders)
                    httpResponse.AddHeader(header.Key, header.Value);
            }

            if (__trace)
            {
                TraceMessage(HttpContext.Current);
                Trace.WriteLine();
            }
            if (__traceToMemory)
                TraceToMemory.WriteLine();

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                httpResponse.End();
            }
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
            if (__trace)
            {
                Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} - Application_End", DateTime.Now);
                Trace.WriteLine("  OptionsMessage {0}", __OptionsMessage);
                Trace.WriteLine();
            }
            if (__traceToMemory)
                zfile.WriteFile(__traceToMemoryFile, TraceToMemory.GetTrace(), append: true);
            Trace.CurrentTrace.Close();
        }

        private static void TraceMessage(HttpContext httpContext)
        {
            HttpRequest httpRequest = httpContext.Request;
            Trace.WriteLine("  HttpRequest :");
            Trace.WriteLine("    Url {0}", httpRequest.Url);
            Trace.WriteLine("    Method {0}", httpRequest.HttpMethod);
            foreach (string header in httpRequest.Headers)
                Trace.WriteLine("    header \"{0}\" = \"{1}\"", header, httpRequest.Headers[header]);

            HttpResponse httpResponse = httpContext.Response;
            Trace.WriteLine("  httpResponse :");
            Trace.WriteLine("    StatusCode {0} - {1}", (int)httpResponse.StatusCode, httpResponse.StatusCode);
            Trace.WriteLine("    StatusDescription {0}", httpResponse.StatusDescription);
            foreach (string header in httpResponse.Headers)
                Trace.WriteLine("    header \"{0}\" = \"{1}\"", header, httpResponse.Headers[header]);
        }
    }
}
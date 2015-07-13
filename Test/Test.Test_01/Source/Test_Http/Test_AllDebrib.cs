using System;
using System.IO;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.Web;

namespace Test.Test_Http
{
    public static class Test_AllDebrib
    {
        public static void Test_AllDebrib_01()
        {
            GetAllDebribAccount();
            Trace.WriteLine();
            Test_AllDebrib_Link_01("http://uptobox.com/0485eztzwh1y");
            Trace.WriteLine();
            Test_AllDebrib_Link_01("http://www.oboom.com/36U5SZ6H");
        }

        public static void Test_AllDebrib_Link_01(string link)
        {
            //string link = "http://ul.to/g4gtl5j3";
            //http://uptobox.com/0485eztzwh1y
            //http://www.alldebrid.fr/service.php?link=http%3A%2F%2Ful.to%2Fg4gtl5j3&nb=0&json=true&pw=
            //string url = "http://www.alldebrid.fr/service.php?link={0}&nb=0&json=true&pw=";
            //url = string.Format(url, link);
            //string url = "http://www.alldebrid.com/service.php?pseudo=&password=&link=http%3A%2F%2Fuptobox.com%2F0485eztzwh1y&view=1";
            //string url = "http://www.alldebrid.com/service.php?pseudo=&password=&link=http%3A%2F%2Fuptobox.com%2F0485eztzwh1y";
            //string url = "http://www.alldebrid.com/service.php?pseudo=&password=&link=http%3A%2F%2Fuptobox.com%2F0485eztzwh1y&json=true";

            GetAllDebribAccount();

            //int traceLevel = Http2.HtmlReader.Trace.TraceLevel;
            //Http2.HtmlReader.Trace.TraceLevel = 0;

            //try
            //{
                string url = GetAllDebribUrl(link);
                string urlWithoutAccount = GetAllDebribUrl(link, false);
                Trace.WriteLine("Load(\"{0}\")", urlWithoutAccount);
                //Http2.LoadUrl(url);
                Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url });
                //Trace.WriteLine("result : {0}", Http2.HtmlReader.http.TextResult);
                Trace.WriteLine("result : {0}", http.ResultText);

                Trace.WriteLine("Load(\"{0}\")", urlWithoutAccount + "&view=1");
                //Http2.LoadUrl(url + "&view=1");
                http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url + "&view=1" });
                Trace.WriteLine("result : {0}", http.ResultText);

                Trace.WriteLine("Load(\"{0}\")", urlWithoutAccount + "&json=true");
                //Http2.LoadUrl(url + "&json=true");
                http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url + "&json=true" });
                Trace.WriteLine("result : {0}", http.ResultText);
            //}
            //finally
            //{
            //    Http2.HtmlReader.Trace.TraceLevel = traceLevel;
            //}

        }

        private static string __login = null;
        private static string __password = null;
        public static void GetAllDebribAccount()
        {
            if (__login != null)
                return;
            string LocalConfigFile = XmlConfig.CurrentConfig.Get("LocalConfig");
            if (!File.Exists(LocalConfigFile))
                throw new PBException("error LocalConfig not found \"{0}\"", LocalConfigFile);
            XmlConfig localConfig = new XmlConfig(LocalConfigFile);
            //Trace.WriteLine("LocalConfig : \"{0}\"", RunSource.CurrentRunSource.Config.Get("LocalConfig"));
            __login = localConfig.GetExplicit("DownloadAutomateManager/Alldebrid/Login");
            __password = localConfig.GetExplicit("DownloadAutomateManager/Alldebrid/Password");
            //Trace.WriteLine("login       : {0}", __login == null ? "login is null" : "login is not null");
            //Trace.WriteLine("password    : {0}", __password == null ? "password is null" : "password is not null");
        }

        public static string GetAllDebribUrl(string link, bool withAccount = true)
        {
            string login = __login;
            string password = __password;
            if (!withAccount)
            {
                login = "";
                password = "";
            }
            return string.Format("http://www.alldebrid.com/service.php?pseudo={0}&password={1}&link={2}", login, password, link);
        }
    }
}

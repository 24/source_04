using System;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;

namespace Test.Test_Http
{
    public static class Test_DebridLink
    {
        public static void Test_DebridLink_01()
        {
            //HttpRun.Load("https://api.debrid-link.fr/rest/token/1R6858wC6lO15X8i/new");
            string urlBase = "https://api.debrid-link.fr/rest/";
            //string login = RunSource.CurrentRunSource.Config.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/DebridLink/Login");
            string login = XmlConfig.CurrentConfig.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/DebridLink/Login");
            string password = XmlConfig.CurrentConfig.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/DebridLink/Password");
            //string publickey = "1R6858wC6lO15X8i";
            string publickey = XmlConfig.CurrentConfig.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/DebridLink/PublicKey");
            //string sessidTime = "all";
            string sessidTime = XmlConfig.CurrentConfig.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/DebridLink/SessidTime");
            string url = urlBase + string.Format("token/{0}/new", publickey);
            HttpRequestParameters requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url }, requestParameters);
            DateTime dt = DateTime.Now;
            http.ResultText.zTraceJson();
            BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(http.ResultText);
            string token = doc.zGet("value.token").zAsString();
            string validTokenUrl = doc.zGet("value.validTokenUrl").zAsString();
            string key = doc.zGet("value.key").zAsString();
            int ts = doc.zGet("ts").zAsInt();
            Trace.WriteLine("request time   : \"{0:dd/MM/yyyy HH:mm:ss}\"", dt);
            Trace.WriteLine("result         : \"{0}\"", doc.zGet("result").zAsString());
            Trace.WriteLine("token          : \"{0}\"", token);
            Trace.WriteLine("validTokenUrl  : \"{0}\"", validTokenUrl);
            Trace.WriteLine("key            : \"{0}\"", key);
            Trace.WriteLine("ts             : \"{0}\"", ts);
            Trace.WriteLine("ts             : \"{0:dd/MM/yyyy HH:mm:ss}\"", zdate.UnixTimeStampToDateTime(ts));
            Trace.WriteLine("ts             : \"{0}\"", zdate.UnixTimeStampToDateTime(ts) - dt);

            // validTokenUrl : "https://secure.debrid-link.fr/user/2_2d481d8991e4db60f43d24d9d387b75699db7a0157182967/login"
            http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = validTokenUrl }, requestParameters);

            // <script>if (window!=window.top) { top.location.href='https://secure.debrid-link.fr/login'; }</script>
            // <form action='' method='POST' class='form-horizontal'>
            // <input type='text' class='form-control' name='user'>
            // <input type='password' class='form-control' name='password'>
            // <input type='hidden' value='10_a3a206c4398f195283a4843d44f017f3211275e443747173' name='token'>
            // <input type='submit' style='display:none'>
            // <button type='submit' name='authorizedToken' value='1' class='btn btn-dl'>Envoyer</button>

            XXElement xeSource = http.zGetXDocument().zXXElement();

            // script : if (window!=window.top) { top.location.href='https://secure.debrid-link.fr/login'; }
            string script = xeSource.XPathValue("//head//script//text()");
            if (script == null)
            {
                Trace.WriteLine("//head//script not found");
                return;
            }
            Trace.WriteLine("script : \"{0}\"", script);
            Regex rg = new Regex("top\\.location\\.href=[\"'](.*)[\"']", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            Match match = rg.Match(script);
            if (!match.Success)
            {
                Trace.WriteLine("top.location.href='...' not found in script");
                return;
            }
            url = match.Groups[1].Value;
            Trace.WriteLine("login url : \"{0}\"", url);

            XXElement xeForm = xeSource.XPathElement("//form");
            string action = xeForm.AttribValue("action");
            Trace.WriteLine("form action : \"{0}\"", action);
            if (action != null && action != "")
                url = action;
            HttpRequestMethod method = Http.GetHttpRequestMethod(xeForm.AttribValue("method"));
            Trace.WriteLine("form method : {0}", method);

            //XmlConfig localConfig = new XmlConfig(RunSource.CurrentRunSource.Config.GetExplicit("LocalConfig"));
            //string login = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Login");
            //string password = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Password");
            StringBuilder content = new StringBuilder();
            bool first = true;
            string name, value;
            //foreach (XXElement xe in xeForm.XPathElements(".//input"))
            foreach (XXElement xe in xeForm.DescendantFormItems())
            {
                name = xe.AttribValue("name");
                if (name == null)
                    continue;
                if (name == "user")
                    value = login;
                else if (name == "password")
                    value = password;
                else if (name == "sessidTime")
                    value = sessidTime;
                else
                    value = xe.AttribValue("value");
                if (!first)
                    content.Append('&');
                content.AppendFormat("{0}={1}", name, value);
                Trace.WriteLine("{0}={1}", name, value);
                first = false;
            }
            //XXElement xeButton = xeForm.XPathElement(".//button");
            //name = xeButton.AttribValue("name");
            //value = xeButton.AttribValue("value");
            //if (name != null && value != null)
            //{
            //    content.AppendFormat("&{0}={1}", name, value);
            //    Trace.WriteLine("{0}={1}", name, value);
            //}

            // "user=la_beuze&password=xxxxxx&sessidTime=all&token=10_56b51ee12ad5dabcac620230cda436cab94bd37154742765&authorizedToken=1"
            //  user=la_beuze&password=pbeuz0&sessidTime=all&token=10_3205776c76bb0479b1d57e9bf834b38ae2c5d10669848384&authorizedToken=1
            Trace.WriteLine("content : \"{0}\"", content.ToString());
            http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url, Method = method, Content = content.ToString() }, requestParameters);

            // <div class='panel-body'>
            // <div class='alert alert-success'>
            // La session a bien été activée. Vous pouvez utiliser l'application API Test
            // </div>
            // </div>
            xeSource = http.zGetXDocument().zXXElement();
            string loginMessage = xeSource.XPathValue("//div[@class='panel-body']//text()").Trim();
            Trace.WriteLine("login message : \"{0}\"", loginMessage);
        }
    }
}

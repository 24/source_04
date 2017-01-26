using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Web.Http;
using System;
using System.Text;

namespace pb.Web.Data.Test
{
    public static class Test_DebridLinkFr
    {
        public static BsonDocument Test_DebridLinkFr_01()
        {
            DebridLinkFr debridLinkFr = CreateDebridLinkFr();
            return debridLinkFr.GetAccountInfos();
        }

        public static void Test_Connexion_01()
        {
            string exportDirectory = @"c:\pib\drive\google\dev_data\exe\runsource\download\sites\debrid-link.fr\model\login\new";

            bool trace = DebridLinkFr.Trace;
            DebridLinkFr.Trace = false;

            Trace.WriteLine("test connexion to debrid-link.fr");
            Trace.WriteLine("  export directory      : \"{0}\"", exportDirectory);

            XmlConfig localConfig = GetLocalConfig();

            string publicKey = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/PublicKey");
            Trace.WriteLine("  publicKey             : \"{0}\"", publicKey);
            Trace.WriteLine();

            string newTokenUrl = string.Format("https://debrid-link.fr/api/token/{0}/new", publicKey);
            string exportFile = "01_debrid-link.fr_api_new_token.txt";
            Trace.WriteLine("  get new token key     : \"{0}\"", newTokenUrl);
            Trace.WriteLine("  export to file        : \"{0}\"", exportFile);
            DateTime requestTime = DateTime.Now;
            Http.Http http = Http.Http.LoadAsText(new HttpRequest { Url = newTokenUrl }, exportFile: zPath.Combine(exportDirectory, exportFile));
            BsonDocument result = BsonDocument.Parse(http.ResultText);
            int serverTs = result.zGet("ts").zAsInt();
            DateTime serverTime = zdate.UnixTimeStampToDateTime(serverTs);
            TimeSpan serverTimeGap = serverTime - requestTime;
            Trace.WriteLine("  server time           : request time {0} server timestamp {1} server time {2} gap {3}", requestTime, serverTs, serverTime, serverTimeGap);
            Trace.WriteLine("  result                :");
            Trace.WriteLine(result.zToJson());
            Trace.WriteLine();

            string validTokenUrl = result.zGet("value.validTokenUrl").zAsString();
            exportFile = "02_debrid-link.fr_api_valid_token.html";
            Trace.WriteLine("  load valid token url  : \"{0}\"", validTokenUrl);
            Trace.WriteLine("  export to file        : \"{0}\"", exportFile);
            HttpRequestParameters httpRequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            http = Http.Http.LoadAsText(new HttpRequest { Url = validTokenUrl }, httpRequestParameters, exportFile: zPath.Combine(exportDirectory, exportFile));
            Trace.WriteLine();

            string loginUrl = "https://debrid-link.fr/login";
            exportFile = "03_debrid-link.fr_login.html";
            Trace.WriteLine("  send login info       : \"{0}\"", loginUrl);
            Trace.WriteLine("  export to file        : \"{0}\"", exportFile);
            string content = string.Format("user={0}&password={1}&understand=true", localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Login"), localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Password"));
            string traceContent = string.Format("user={0}&password={1}&understand=true", "xxxxxx", "xxxxxx");
            Trace.WriteLine("  content               : \"{0}\"", traceContent);
            http = Http.Http.LoadAsText(new HttpRequest { Url = loginUrl, Method = HttpRequestMethod.Post, Content = content }, httpRequestParameters, exportFile: zPath.Combine(exportDirectory, exportFile));
            Trace.WriteLine();

            XXElement xe = http.zGetXDocument().zXXElement();
            //<div class="alert alert-success">
            xe = xe.XPathElement("//div[@class='alert alert-success']");
            Trace.WriteLine("  verify login          : {0}", xe.XElement != null ? "login ok" : "login error");
            Trace.WriteLine();

            string request = "/account/infos";
            string urlRequest = "https://debrid-link.fr/api" + request;
            exportFile = "04_debrid-link.fr_account_infos.txt";
            Trace.WriteLine("  get account infos     : \"{0}\"", urlRequest);
            Trace.WriteLine("  export to file        : \"{0}\"", exportFile);
            string key = result.zGet("value.key").zAsString();
            DateTime time = DateTime.Now + serverTimeGap;
            int timestamp = zdate.DateTimeToUnixTimeStamp(time);
            string signature = DebridLinkFr.GetSignature(timestamp, request, key);
            Trace.WriteLine("  signature             : timestamp {0} request \"{1}\" key \"{2}\" signature \"{3}\"", timestamp, request, key, signature);
            string token = result.zGet("value.token").zAsString();
            httpRequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            httpRequestParameters.Headers["x-dl-token"] = token;
            httpRequestParameters.Headers["x-dl-sign"] = signature;
            httpRequestParameters.Headers["x-dl-ts"] = timestamp.ToString();
            Trace.WriteLine("  set header            : \"{0}\" = \"{1}\"", "x-dl-token", token);
            Trace.WriteLine("  set header            : \"{0}\" = \"{1}\"", "x-dl-sign", signature);
            Trace.WriteLine("  set header            : \"{0}\" = \"{1}\"", "x-dl-ts", timestamp);
            DateTime dt = DateTime.Now;
            http = Http.Http.LoadAsText(new HttpRequest { Url = urlRequest }, httpRequestParameters, exportFile: zPath.Combine(exportDirectory, exportFile));
            result = BsonDocument.Parse(http.ResultText);
            // control server time
            int newTimestamp = result.zGet("ts").zAsInt();
            DateTime newServerTime = zdate.UnixTimeStampToDateTime(newTimestamp);
            TimeSpan newServerTimeGap = newServerTime - dt;
            Trace.WriteLine("  new server time       : {0} gap {1} timestamp {2} timestamp gap {3}", newServerTime, newServerTimeGap, newTimestamp, timestamp - newTimestamp);
            Trace.WriteLine("  result                :");
            Trace.WriteLine(result.zToJson());
            Trace.WriteLine();

            DebridLinkFr.Trace = trace;
        }

        public static DebridLinkFr CreateDebridLinkFr(bool trace = false)
        {
            //XmlConfig config;
            XmlConfig localConfig = GetLocalConfig();
            DebridLinkFr debrider = new DebridLinkFr();
            debrider.Login = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Login");
            debrider.Password = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Password");
            debrider.PublicKey = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/PublicKey");
            debrider.ConnexionLifetime = DebridLinkFr.GetConnexionLifetime(localConfig.GetExplicit("DownloadAutomateManager/DebridLink/ConnexionLifetime"));
            debrider.ConnexionFile = @"c:\pib\dev_data\exe\runsource\download\sites\debrid-link.fr\connexion\Connexion.txt";
            //debrider.ServerTimeFile = XmlConfig.CurrentConfig.GetExplicit("DebridLink/ServerTimeFile");
            DebridLinkFr.Trace = trace;
            return debrider;
        }

        public static XmlConfig GetLocalConfig()
        {
            return new XmlConfig(@"c:\pib\drive\google\dev_data\exe\runsource\download\config\download.config.local.xml");
        }
    }
}

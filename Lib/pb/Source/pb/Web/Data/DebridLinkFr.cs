using System;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Data.TraceData;

namespace pb.Web
{
    public enum DebridLinkConnexionLifetime
    {
        All = 1,
        OneHour,
        SixHours,
        TwelveHours,
        TwentyFourHours
    }

    public class DebridLinkConnexion
    {
        public string Token;
        public string Key;
        public DebridLinkConnexionLifetime ConnexionLifetime;
        public DateTime ConnexionTime;
        public DateTime? EndConnexionTime;
        public DateTime ClientTime;
        public DateTime ServerTime;
        public TimeSpan ServerTimeGap;         // ConnexionTime - ServerConnexionTime
    }

    //public class DebridLinkServerTime
    //{
    //    public DateTime ConnexionTime;
    //    public DateTime ServerConnexionTime;
    //    public TimeSpan ServerTimeGap;         // ConnexionTime - ServerConnexionTime
    //}

    public class DebridLinkFrInfo : DebridLinkInfo
    {
        public string ErrorCode;

        public override string GetErrorMessage()
        {
            if (ErrorCode == null)
                return null;
            switch (ErrorCode.ToLower())
            {
                case "maxlinkhost":
                    return string.Format("warning : nombre de liens maximum atteint pour cet hébergeur, link \"{0}\"", Link);
                case "hostnotvalid":
                    return string.Format("warning : host not valid, link \"{0}\"", Link);
                default:
                    return string.Format("unknow error code \"{0}\"", ErrorCode);
            }
        }
    }

    public class DebridLinkFr : ITraceData
    {
        private static bool __trace = false;
        private static string __url = "https://api.debrid-link.fr/rest";
        private TraceData _traceData = null;
        private string _login = null;
        private string _password = null;
        private string _publicKey = null;
        private DebridLinkConnexionLifetime _connexionLifetime = DebridLinkConnexionLifetime.OneHour;
        private string _connexionFile = null;
        //private string _serverTimeFile = null;
        private DebridLinkConnexion _connexion = null;
        //private DebridLinkServerTime _serverTime = null;

        //public DebriderDebridLink(string login, string password)
        //{
        //    if (login == null || login == "" || password == null || password == "")
        //        throw new PBException("error debrid-link.fr missing login or password");
        //    _login = login;
        //    _password = password;
        //}

        public DebridLinkFr()
        {
            TraceDataRegistry.CurrentTraceDataRegistry.Register("DebridLinkFr", this);
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public string Login { get { return _login; } set { _login = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string PublicKey { get { return _publicKey; } set { _publicKey = value; } }
        public DebridLinkConnexionLifetime ConnexionLifetime { get { return _connexionLifetime; } set { _connexionLifetime = value; } }
        public string ConnexionFile { get { return _connexionFile; } set { _connexionFile = value; } }
        //public string ServerTimeFile { get { return _serverTimeFile; } set { _serverTimeFile = value; } }

        public void ActivateTraceData(TraceData traceData)
        {
            _traceData = traceData;
        }

        public void DesactivateTraceData()
        {
            _traceData = null;
        }

        public void Connexion()
        {
            if (_connexionFile == null)
                throw new PBException("DebriderDebridLink connexion file is null");
            //if (_serverTimeFile == null)
            //    throw new PBException("DebriderDebridLink server time file is null");

            pb.Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - new connexion to debrid-link.fr", DateTime.Now);

            if (__trace)
            {
                pb.Trace.WriteLine("DebriderDebridLink.Connexion() :");
            }

            string url = __url + string.Format("/token/{0}/new", _publicKey);

            HttpRequestParameters requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            DateTime dt = DateTime.Now;
            Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url }, requestParameters);
            BsonDocument result = BsonSerializer.Deserialize<BsonDocument>(http.ResultText);
            if (__trace)
            {
                pb.Trace.WriteLine("  result                      :");
                pb.Trace.WriteLine(result.zToJson());
            }
            DebridLinkConnexion connexion = new DebridLinkConnexion();
            //DebridLinkServerTime serverTime = new DebridLinkServerTime();
            connexion.ConnexionTime = dt;
            //string token = doc.zGet("value.token").zAsString();
            connexion.Token = result.zGet("value.token").zAsString();
            string validTokenUrl = result.zGet("value.validTokenUrl").zAsString();
            //string key = doc.zGet("value.key").zAsString();
            connexion.Key = result.zGet("value.key").zAsString();
            int ts = result.zGet("ts").zAsInt();
            connexion.ClientTime = dt;
            connexion.ServerTime = zdate.UnixTimeStampToDateTime(ts);
            connexion.ServerTimeGap = connexion.ServerTime - dt;
            connexion.ConnexionLifetime = _connexionLifetime;
            connexion.EndConnexionTime = connexion.ConnexionTime + GetConnexionTimespan(connexion.ConnexionLifetime) - TimeSpan.FromMinutes(5);
            if (__trace)
            {
                pb.Trace.WriteLine("  request time                : \"{0:dd/MM/yyyy HH:mm:ss}\"", dt);
                pb.Trace.WriteLine("  result                      : \"{0}\"", result.zGet("result").zAsString());
                pb.Trace.WriteLine("  token                       : \"{0}\"", connexion.Token);
                pb.Trace.WriteLine("  validTokenUrl               : \"{0}\"", validTokenUrl);
                pb.Trace.WriteLine("  key                         : \"{0}\"", connexion.Key);
                pb.Trace.WriteLine("  server time                 : {0} - {1:dd/MM/yyyy HH:mm:ss}", ts, connexion.ServerTime);
                pb.Trace.WriteLine("  server time gap             : {0}", connexion.ServerTimeGap);
            }

            // validTokenUrl : "https://secure.debrid-link.fr/user/2_2d481d8991e4db60f43d24d9d387b75699db7a0157182967/login"
            http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = validTokenUrl }, requestParameters);

            // <script>if (window!=window.top) { top.location.href='https://secure.debrid-link.fr/login'; }</script>
            // <form action='' method='POST' class='form-horizontal'>
            // <input type='text' class='form-control' name='user'>
            // <input type='password' class='form-control' name='password'>
            // <select name='sessidTime' class='form-control'>
            // <option value='all'  selected='selected'> Toujours</option>
            // ...
            // </select>
            // <input type='hidden' value='10_a3a206c4398f195283a4843d44f017f3211275e443747173' name='token'>
            // <button type='submit' name='authorizedToken' value='1' class='btn btn-dl'>Envoyer</button>
            // <input type='submit' style='display:none'>

            XXElement xeSource = http.zGetXDocument().zXXElement();

            // le script n'est plus là dans la page html 24/03/2015
            // script : if (window!=window.top) { top.location.href='https://secure.debrid-link.fr/login'; }
            //string script = xeSource.XPathValue("//head//script//text()");
            //if (script == null)
            //{
            //    //Trace.WriteLine("//head//script not found");
            //    //return;
            //    throw new PBException("DebriderDebridLink.Connect() : //head//script not found");
            //}
            //if (__trace)
            //    pb.Trace.WriteLine("  script                      : \"{0}\"", script);
            //Regex rg = new Regex("top\\.location\\.href=[\"'](.*)[\"']", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            //Match match = rg.Match(script);
            //if (!match.Success)
            //{
            //    //Trace.WriteLine("top.location.href='...' not found in script");
            //    //return;
            //    throw new PBException("DebriderDebridLink.Connect() : top.location.href='...' not found in script");
            //}
            //url = match.Groups[1].Value;

            url = "https://secure.debrid-link.fr/login";
            if (__trace)
                pb.Trace.WriteLine("  login url                   : \"{0}\"", url);

            XXElement xeForm = xeSource.XPathElement("//form");
            string action = xeForm.AttribValue("action");
            if (__trace)
                pb.Trace.WriteLine("  form action                 : \"{0}\"", action);
            if (action != null && action != "")
                url = action;
            HttpRequestMethod method = Http.GetHttpRequestMethod(xeForm.AttribValue("method"));
            if (__trace)
                pb.Trace.WriteLine("  form method                 : {0}", method);

            StringBuilder content = new StringBuilder();
            bool first = true;
            string name, value;
            foreach (XXElement xe in xeForm.DescendantFormItems())
            {
                name = xe.AttribValue("name");
                if (name == null)
                    continue;
                if (name == "user")
                    value = _login;
                else if (name == "password")
                    value = _password;
                else if (name == "sessidTime")
                    value = GetConnexionLifetime(_connexionLifetime);
                else
                    value = xe.AttribValue("value");
                if (!first)
                    content.Append('&');
                content.AppendFormat("{0}={1}", name, value);
                if (__trace)
                {
                    if (name != "password")
                        pb.Trace.WriteLine("  {0}={1}", name, value);
                    else
                        pb.Trace.WriteLine("  {0}=xxx", name);
                }
                first = false;
            }

            // "user=la_beuze&password=xxxxxx&sessidTime=all&token=10_56b51ee12ad5dabcac620230cda436cab94bd37154742765&authorizedToken=1"
            //if (__trace)
            //    pb.Trace.WriteLine("content : \"{0}\"", content.ToString());

            http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url, Method = method, Content = content.ToString() }, requestParameters);

            // <div class='panel-body'>
            // <div class='alert alert-success'>
            // La session a bien été activée. Vous pouvez utiliser l'application API Test
            // </div>
            // </div>
            xeSource = http.zGetXDocument().zXXElement();
            //string loginMessage = xeSource.ExplicitXPathValue("//div[@class='panel-body']//text()");
            string loginMessage = xeSource.ExplicitXPathValue("//div[@class='alert alert-success']//text()");
            if (__trace)
                pb.Trace.WriteLine("  login message               : \"{0}\"", loginMessage);
            //if (loginMessage == null || !loginMessage.Trim().StartsWith("La session a bien été activée", StringComparison.InvariantCultureIgnoreCase))
            if (loginMessage == null || !loginMessage.Trim().StartsWith("Vous avez été connecté avec succès", StringComparison.InvariantCultureIgnoreCase))
                throw new PBException("DebriderDebridLink.Connect() : wrong login message \"{0}\"", loginMessage);


            connexion.zSave(_connexionFile);
            _connexion = connexion;
            //serverTime.zSave(_serverTimeFile);
            //_serverTime = serverTime;
        }

        public bool IsConnected()
        {
            if (_connexion == null)
            {
                if (_connexionFile == null)
                    throw new PBException("DebriderDebridLink connexion file is null");

                if (!zFile.Exists(_connexionFile))
                    return false;

                _connexion = zmongo.ReadFileAs<DebridLinkConnexion>(_connexionFile);
            }
            if (_connexion.EndConnexionTime != null && DateTime.Now >= _connexion.EndConnexionTime)
            {
                // la connection n'est plus valable, il faut se reconnecter
                return false;
            }

            //if (_serverTime == null)
            //{

            //    if (_serverTimeFile == null)
            //        throw new PBException("DebriderDebridLink server time file is null");

            //    if (!zFile.Exists(_serverTimeFile))
            //        return false;

            //    _serverTime = zmongo.ReadFileAs<DebridLinkServerTime>(_serverTimeFile);
            //}

            return true;
        }

        public BsonDocument GetAccountInfos()
        {
            return ExecutePostCommand("/account/infos");
            // https://api.debrid-link.fr/rest/account/infos
            // {
            // "result":"OK",
            // "value":{
            //    "pseudo":"la_beuze",
            //    "email":"labe***euz@gmai**com",
            //    "accountType":1,
            //    "premiumLeft":1984656,
            //    "pts":195,
            //    "trafishare":32212254720,
            //    "vouchersUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/create_voucher",
            //    "editPasswordUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/editPassword",
            //    "editEmailUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/editEmail",
            //    "viewSessidUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/sessid",
            //    "upgradeAccountUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/premium",
            //    "upgradePtsUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/premium\/pts",
            //    "upgradeTraficshareUrl":"https:\/\/secure.debrid-link.fr\/user\/2_f843bb5a464171c9bc6304db373e5c9956d7f2b250566148\/premium\/traficshare",
            //    "registerDate":"2013-02-08"
            //    },
            // "ts":1425823512
            // }
        }

        public BsonDocument GetTorrentActivity()
        {
            return ExecutePostCommand("/seedbox/activity");
        }

        public BsonDocument GetTorrentStats()
        {
            return ExecutePostCommand("/seedbox/stats");
        }

        public BsonDocument GetTorrentPoints()
        {
            return ExecutePostCommand("/seedbox/points");
            // {
            //   "result" : "OK",
            //   "value" : {
            //     "duration" : 4007211,
            //     "usage_day" : 4,
            //     "usage_next_day" : 4
            //   },
            //   "ts" : 1425913089
            // }
        }

        public BsonDocument GetTorrentList()
        {
            return ExecutePostCommand("/seedbox/list");
        }

        public BsonDocument AddTorrent(string torrentUrl)
        {
            return ExecutePostCommand("/seedbox/add", "torrentUrl=" + torrentUrl);
        }

        public BsonDocument StopTorrents(params string[] torrentIds)
        {
            return ExecutePostCommand("/seedbox/ids/" + GetJsonValue(torrentIds) + "/stop");
            // {
            //   "result" : "OK",
            //   "value" : ["44654f844ce2ad4c678"],
            //   "ts" : 1425904891
            // }
        }

        public BsonDocument ResumeTorrents(params string[] torrentIds)
        {
            return ExecutePostCommand("/seedbox/ids/" + GetJsonValue(torrentIds) + "/resume");
            // {
            //   "result" : "OK",
            //   "value" : ["44654f844ce2ad4c678"],
            //   "ts" : 1425904891
            // }
        }

        public BsonDocument RemoveTorrents(params string[] torrentIds)
        {
            return ExecutePostCommand("/seedbox/ids/" + GetJsonValue(torrentIds) + "/remove");
            // {
            //   "result" : "OK",
            //   "value" : ["44654f844ce2ad4c678"],
            //   "ts" : 1425904891
            // }
        }

        public BsonDocument GetDownloaderStatus()
        {
            return ExecuteGetCommand("/downloader/status");
        }

        public BsonDocument GetDownloaderTraffic()
        {
            return ExecutePostCommand("/downloader/traffic");
        }

        public BsonDocument GetDownloaderStats()
        {
            return ExecutePostCommand("/downloader/stats");
        }

        public BsonDocument GetDownloaderList()
        {
            return ExecutePostCommand("/downloader/list");
        }

        public BsonDocument DownloaderAdd(string link)
        {
            BsonDocument result = ExecutePostCommand("/downloader/add", "link=" + link);
            if (result.zGet("result").zAsString() == "KO")
            {
                if (result.zGet("ERR").zAsString() == "maxLinkHost")
                {
                    pb.Trace.WriteLine("warning : nombre de liens maximum atteint pour cet hébergeur. link \"{0}\"", link);
                }
            }
            if (_traceData != null)
            {
                _traceData.Trace(new BsonDocument { { "OpeType", "Debrider" }, { "Ope", "DebridLinkFr" }, { "Link", link }, { "Result", result } });
            }
            return result;
            // {
            //   "result" : "OK",
            //   "value" : {
            //     "time" : 1425914492,
            //     "id" : "39ffe0467b5e56d85f8b50aea2e6be0795bad733",
            //     "link" : "http://uploaded.net/file/2o4ntzs4",
            //     "downloadLink" : "http://dl7-5.debrid-link.fr/dl/10_8ce50454790e4b66d5c40812b0a2860e67af51df88019505/39ffe0467b5e56d85f8b50aea2e6be0795bad733/j090315-ED.rar",
            //     "filename" : "j090315-ED.rar",
            //     "chunk" : 16,
            //     "resume" : true
            //   },
            //   "ts" : 1425914492
            // }
        }

        public BsonDocument DownloaderRemove(params string[] idLinks)
        {
            return ExecutePostCommand("/downloader/ids/" + GetJsonValue(idLinks) + "/remove");
            // {
            //   "result" : "OK",
            //   "value" : ["a0bbc06d1b2762166d9139f3d2b81e89b7e78af5"],
            //   "ts" : 1425916912
            // }
        }

        public BsonDocument GetRemoteHostList()
        {
            return ExecutePostCommand("/remote/host/list");
        }

        private void Connect(bool forceNewConnexion = false)
        {
            if (forceNewConnexion || !IsConnected())
                Connexion();
        }

        public BsonDocument ExecuteGetCommand(string requestPath)
        {
            string url = __url + requestPath;
            if (__trace)
                pb.Trace.WriteLine("DebriderDebridLink.ExecuteGetCommand() :");
            HttpRequestParameters requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };

            Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url }, requestParameters);
            BsonDocument result = BsonDocument.Parse(http.ResultText);
            if (__trace)
            {
                pb.Trace.WriteLine("  result                      :");
                pb.Trace.WriteLine(result.zToJson());
            }
            return result;
        }

        public BsonDocument ExecutePostCommand(string requestPath, string content = null)
        {
            Connect();
            string url = __url + requestPath;
            if (__trace)
                pb.Trace.WriteLine("DebriderDebridLink.ExecutePostCommand() :");
            HttpRequestParameters requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.AppendFormat("token={0}", _connexion.Token);

            int timestamp = GetServerTimestamp();

            // test de décalage du timestamp
            //timestamp -= 15;  // KO avec -15 ou +15
            //if (__trace)
            //    pb.Trace.WriteLine("  server time + 60 sec        : {0}", zdate.UnixTimeStampToDateTime(timestamp));

            contentBuilder.AppendFormat("&sign={0}", GetSignature(_connexion.Key, timestamp, requestPath));
            if (content != null)
            {
                contentBuilder.Append('&');
                contentBuilder.Append(content);
            }
            contentBuilder.AppendFormat("&ts={0}", timestamp);
            if (__trace)
                pb.Trace.WriteLine("  content                     : \"{0}\"", contentBuilder.ToString());
            DateTime dt = DateTime.Now;
            Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url, Method = HttpRequestMethod.Post, Content = contentBuilder.ToString() }, requestParameters);
            BsonDocument result = BsonDocument.Parse(http.ResultText);
            //if (__trace)
            //{
            int newTimestamp = result.zGet("ts").zAsInt();
            DateTime newServerTime = zdate.UnixTimeStampToDateTime(newTimestamp);
            TimeSpan newServerTimeGap = newServerTime - dt;
            // gap gap {2}    newServerTimeGap - _connexion.ServerTimeGap
            pb.Trace.WriteLine("  new server time             : {0} gap {1} timestamp {2} timestamp gap {3}", newServerTime, newServerTimeGap, newTimestamp, timestamp - newTimestamp);
            //}
            if (__trace)
            {
                pb.Trace.WriteLine("  result                      :");
                pb.Trace.WriteLine(result.zToJson());
            }
            return result;
        }

        private int GetServerTimestamp()
        {
            DateTime time = DateTime.Now + _connexion.ServerTimeGap;
            int timestamp = zdate.DateTimeToUnixTimeStamp(time);
            if (__trace)
            {
                //pb.Trace.WriteLine("  time                        : {0}", DateTime.Now);
                pb.Trace.WriteLine("  calculated server time      : {0} gap {1} timestamp {2}", time, _connexion.ServerTimeGap, timestamp);
            }
            return timestamp;
        }

        private static string GetSignature(string key, int timestamp, string requestPath)
        {
            // The signature is the SHA1 result of (Timestamp + Route + key) (from http://debrid-link.fr/api_doc/#/home)
            // Timestamp : server unix timestamp
            // Route : path of the request
            // key : key returned by authenticating
            // example Timestamp + Route + key : 1418758917/account/infosi619yOI4Kt8WB02g
            // SHA1 result : ab90fa6a2c9f1bc2bbd7988ff266971b5c10583c
            string signature = timestamp.ToString() + requestPath + key;

            string hash = Crypt.ComputeSHA1Hash(signature).zToHex(lowercase: true);
            //if (__trace)
            //{
            //    pb.Trace.WriteLine("  signature                   : \"{0}\"", signature);
            //    pb.Trace.WriteLine("  signature sha1              : \"{0}\"", hash);
            //}
            return hash;
        }

        private string GetJsonValue(string[] torrentIds)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            bool first = true;
            foreach (string torrentId in torrentIds)
            {
                if (!first)
                    sb.Append(',');
                first = false;
                sb.AppendFormat("\"{0}\"", torrentId);
            }
            sb.Append(']');
            return sb.ToString();
        }

        public static DebridLinkConnexionLifetime GetConnexionLifetime(string connexionLifetime)
        {
            switch (connexionLifetime)
            {
                case "all":
                    return DebridLinkConnexionLifetime.All;
                case "1":
                    return DebridLinkConnexionLifetime.OneHour;
                case "6":
                    return DebridLinkConnexionLifetime.SixHours;
                case "12":
                    return DebridLinkConnexionLifetime.TwelveHours;
                case "24":
                    return DebridLinkConnexionLifetime.TwentyFourHours;
                default:
                    throw new PBException("unknow DebridLink connexion lifetime \"{0}\"", connexionLifetime);
            }
        }

        public static string GetConnexionLifetime(DebridLinkConnexionLifetime connexionLifetime)
        {
            switch (connexionLifetime)
            {
                case DebridLinkConnexionLifetime.All:
                    return "all";
                case DebridLinkConnexionLifetime.OneHour:
                    return "1";
                case DebridLinkConnexionLifetime.SixHours:
                    return "6";
                case DebridLinkConnexionLifetime.TwelveHours:
                    return "12";
                case DebridLinkConnexionLifetime.TwentyFourHours:
                    return "24";
                default:
                    throw new PBException("unknow DebridLink connexion lifetime {0}", connexionLifetime);
            }
        }

        public static TimeSpan? GetConnexionTimespan(DebridLinkConnexionLifetime connexionLifetime)
        {
            switch (connexionLifetime)
            {
                case DebridLinkConnexionLifetime.All:
                    return null;
                case DebridLinkConnexionLifetime.OneHour:
                    return TimeSpan.FromHours(1);
                case DebridLinkConnexionLifetime.SixHours:
                    return TimeSpan.FromHours(6);
                case DebridLinkConnexionLifetime.TwelveHours:
                    return TimeSpan.FromHours(12);
                case DebridLinkConnexionLifetime.TwentyFourHours:
                    return TimeSpan.FromHours(24);
                default:
                    throw new PBException("unknow DebridLink connexion lifetime {0}", connexionLifetime);
            }
        }

        public static int GetLinkRate(string server)
        {
            // http://uptobox.com/oiyprfxyfn1v
            // http://www.uploadable.ch/file/BgeEV6KxnCbB/VPFN118.rar
            // http://rapidgator.net/file/096cca55aba4d9e9dac902b9508a23b1/MiHN65.rar.html
            // http://turbobit.net/15cejdxrzleh.html
            // http://6i5mqc65bc.1fichier.com/
            // http://ul.to/0cqaq9ou
            // http://uploaded.net/file/t40jl73t
            switch (server.ToLower())
            {
                case "uptobox":
                case "uptobox.com":
                    return 30;
                case "uploadable":
                case "uploadable.ch":
                    return 40;
                case "rapidgator":
                case "rapidgator.net":
                    return 50;
                case "turbobit":
                case "turbobit.net":
                    return 60;
                case "1fichier":
                case "1fichier.com":
                    return 70;
                case "letitbit":
                case "letitbit.net":
                    return 80;
                case "ul":
                case "ul.to":
                    return 90;
                case "uploaded":
                case "uploaded.net":
                    return 95;
                default:
                    return 999;
            }
        }
    }
}

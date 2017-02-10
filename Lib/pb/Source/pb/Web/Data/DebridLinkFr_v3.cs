using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using pb.Data.Mongo;
using pb.Data.TraceData;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Web.Http;
using System;
using System.Text;

namespace pb.Web.Data
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

    public class DebridLinkFrRequestResult
    {
        public DateTime RequestTime;
        public BsonDocument Result;
    }

    // from DebridLinkFr_v2 using Http_v2 and HttpManager_v2
    public class DebridLinkFr_v3 : ITraceData
    {
        private static bool __trace = false;
        private static string __url = "https://debrid-link.fr/api";
        private static string __loginUrl = "https://debrid-link.fr/login";
        private TraceData _traceData = null;
        private string _login = null;
        private string _password = null;
        private string _publicKey = null;
        private DebridLinkConnexionLifetime _connexionLifetime = DebridLinkConnexionLifetime.OneHour;
        private string _connexionFile = null;
        private DebridLinkConnexion _connexion = null;
        // used in : Connexion()
        //private HttpRequestParameters _requestParameters = null;
        // used in : ExecuteCommand()
        //private HttpRequestParameters _authenticateRequestParameters = null;
        // HttpManager used in Connexion()
        private HttpManager_v2 _httpManagerConnexion = null;
        // HttpManager used in ExecuteCommand()
        private HttpManager_v2 _httpManagerCommand = null;

        public DebridLinkFr_v3()
        {
            TraceDataRegistry.CurrentTraceDataRegistry.Register("DebridLinkFr_v3", this);
            //_requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            //_authenticateRequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
            _httpManagerConnexion = new HttpManager_v2 { RequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 } };
            _httpManagerCommand = new HttpManager_v2 { RequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 } };
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public string Login { get { return _login; } set { _login = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string PublicKey { get { return _publicKey; } set { _publicKey = value; } }
        public DebridLinkConnexionLifetime ConnexionLifetime { get { return _connexionLifetime; } set { _connexionLifetime = value; } }
        public string ConnexionFile { get { return _connexionFile; } set { _connexionFile = value; } }

        public void ActivateTraceData(TraceData traceData)
        {
            _traceData = traceData;
        }

        public void DesactivateTraceData()
        {
            _traceData = null;
        }

        public BsonDocument GetAccountInfos()
        {
            //return ExecutePostCommand("/account/infos");
            BsonDocument result = ExecuteCommand("/account/infos", HttpRequestMethod.Get);
            if (_traceData != null)
                _traceData.Trace(new BsonDocument { { "OpeType", "Debrider.DebridLinkFr" }, { "Ope", "AccountInfos" }, { "Param", new BsonDocument { } }, { "Result", result } });
            return result;

            //https://debrid-link.fr/api/account/infos
            //{
            //  "result" : "OK",
            //  "value" : {
            //    "pseudo" : "la_beuze",
            //    "email" : "la*****euz@gm*****om",
            //    "accountType" : 1,
            //    "premiumLeft" : 71018,
            //    "pts" : 411,
            //    "trafishare" : "7663028224",
            //    "vouchersUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/account/vouchers",
            //    "editPasswordUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/account/edit_password",
            //    "editEmailUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/account/config_email",
            //    "viewSessidUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/account/view_sessid",
            //    "upgradeAccountUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/premium",
            //    "upgradePtsUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/premium/pts",
            //    "upgradeTraficshareUrl" : "https://debrid-link.fr/user/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/premium/traficshare",
            //    "registerDate" : ""
            //  },
            //  "ts" : 1459705150
            //}

        }

        public BsonDocument DownloaderAdd(string link)
        {
            BsonDocument result = ExecuteCommand("/downloader/add", HttpRequestMethod.Post, "link=" + link);
            if (result.zGet("result").zAsString() == "KO")
            {
                if (result.zGet("ERR").zAsString() == "maxLinkHost")
                {
                    pb.Trace.WriteLine("warning : nombre de liens maximum atteint pour cet hébergeur. link \"{0}\"", link);
                }
            }
            if (_traceData != null)
                _traceData.Trace(new BsonDocument { { "OpeType", "Debrider.DebridLinkFr" }, { "Ope", "DebridLink" }, { "Param", new BsonDocument { { "Link", link } } }, { "Result", result } });
            return result;
            //{
            //  "result" : "OK",
            //  "value" : {
            //    "time" : 1459711231,
            //    "id" : "b386ad062459387f7b5139d26dff594ebc34dae9",
            //    "link" : "http://turbobit.net/ck3xrhxjx7d9.html",
            //    "downloadLink" : "http://dl7-3.debrid-link.fr/dl/10_af8bb87b19a90cbda5d5e387c55b45be83151a4989898437/b386ad062459387f7b5139d26dff594ebc34dae9/Society28.pdf",
            //    "filename" : "Society28.pdf",
            //    "otherLinks" : []
            //  },
            //  "ts" : 1459711232
            //}

            // old version
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

        public BsonDocument ExecuteCommand(string requestPath, HttpRequestMethod method, string content = null)
        {
            // from https://debrid-link.fr/api_doc/#/home
            // URL : https://debrid-link.fr/api/account/infos
            // X-DL-TOKEN : 10_d82555567e30c1e7137828eee6bf35429706d27e43319312
            // X-DL-SIGN : ab90fa6a2c9f1bc2bbd7988ff266971b5c10583c
            // X-DL-TS : 1418758917
            // Sign decoded : 1418758917/account/infosi619yOI4Kt8WB02g

            Connect();
            string url = __url + requestPath;
            if (__trace)
                pb.Trace.WriteLine("DebriderDebridLink.ExecuteGetCommand() :");
            //HttpRequestParameters requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };

            // test de décalage du timestamp
            //timestamp -= 15;  // KO avec -15 ou +15
            //if (__trace)
            //    pb.Trace.WriteLine("  server time + 60 sec        : {0}", zdate.UnixTimeStampToDateTime(timestamp));
            int timestamp = GetServerTimestamp();
            string signature = GetSignature(timestamp, requestPath, _connexion.Key);

            // set token, signature and timestamp as param in url
            //StringBuilder paramBuilder = new StringBuilder();
            //paramBuilder.Append('?');
            //paramBuilder.AppendFormat("x-dl-token={0}", _connexion.Token);
            //paramBuilder.AppendFormat("&x-dl-sign={0}", signature);
            //paramBuilder.AppendFormat("&x-dl-ts={0}", timestamp);
            //url += paramBuilder.ToString();

            // set token, signature and timestamp as headers of http request
            HttpRequestParameters requestParameters = _httpManagerCommand.RequestParameters;
            //_authenticateRequestParameters.Headers["x-dl-token"] = _connexion.Token;
            //_authenticateRequestParameters.Headers["x-dl-sign"] = signature;
            //_authenticateRequestParameters.Headers["x-dl-ts"] = timestamp.ToString();
            requestParameters.Headers["x-dl-token"] = _connexion.Token;
            requestParameters.Headers["x-dl-sign"] = signature;
            requestParameters.Headers["x-dl-ts"] = timestamp.ToString();

            if (__trace)
            {
                pb.Trace.WriteLine("  http method                 : {0}", method);
                pb.Trace.WriteLine("  http header                 : \"{0}\" = \"{1}\"", "x-dl-token", requestParameters.Headers["x-dl-token"]);
                pb.Trace.WriteLine("  http header                 : \"{0}\" = \"{1}\"", "x-dl-sign", requestParameters.Headers["x-dl-sign"]);
                pb.Trace.WriteLine("  http header                 : \"{0}\" = \"{1}\"", "x-dl-ts", requestParameters.Headers["x-dl-ts"]);
                if (content != null)
                    pb.Trace.WriteLine("  http content                : \"{0}\"", content);
            }

            DateTime dt = DateTime.Now;
            //Http.Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url, Method = method, Content = content }, _authenticateRequestParameters);
            HttpResult<string> httpResult = _httpManagerCommand.LoadText(new HttpRequest { Url = url, Method = method, Content = content });

            //BsonDocument result = BsonDocument.Parse(http.ResultText);
            BsonDocument result = BsonDocument.Parse(httpResult.Data);

            //if (__trace)
            //{
            int newTimestamp = result.zGet("ts").zAsInt();
            DateTime newServerTime = zdate.UnixTimeStampToDateTime(newTimestamp);
            TimeSpan newServerTimeGap = newServerTime - dt;
            // gap gap {2}    newServerTimeGap - _connexion.ServerTimeGap
            pb.Trace.WriteLine("  new server time             : {0} gap {1} timestamp {2} timestamp gap {3}", newServerTime, newServerTimeGap, newTimestamp, timestamp - newTimestamp);
            //}

            //if (__trace)
            //{
            //    pb.Trace.WriteLine("  result                      :");
            //    pb.Trace.WriteLine(result.zToJson());
            //}
            return result;
        }

        private void Connect(bool forceNewConnexion = false)
        {
            if (forceNewConnexion || !IsConnected())
                Connexion();
        }

        public bool IsConnected()
        {
            if (_connexion == null)
            {
                if (_connexionFile == null)
                    throw new PBException("DebriderDebridLink connexion file is null");

                if (!zFile.Exists(_connexionFile))
                    return false;

                _connexion = zMongo.ReadFileAs<DebridLinkConnexion>(_connexionFile);
            }
            if (_connexion.EndConnexionTime != null && DateTime.Now >= _connexion.EndConnexionTime)
            {
                // la connection n'est plus valable, il faut se reconnecter
                return false;
            }
            return true;
        }

        public void Connexion()
        {
            pb.Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - new connexion to debrid-link.fr", DateTime.Now);

            if (__trace)
            {
                pb.Trace.WriteLine("DebriderDebridLink.Connexion() :");
            }
            DebridLinkFrRequestResult tokenResult = GetNewToken();
            DebridLinkConnexion connexion = CreateDebridLinkConnexion(tokenResult);
            string validTokenUrl = tokenResult.Result.zGet("value.validTokenUrl").zAsString();

            if (__trace)
            {
                pb.Trace.WriteLine("  request time                : \"{0:dd/MM/yyyy HH:mm:ss}\"", tokenResult.RequestTime);
                pb.Trace.WriteLine("  result                      : \"{0}\"", tokenResult.Result.zGet("result").zAsString());
                pb.Trace.WriteLine("  token                       : \"{0}\"", connexion.Token);
                pb.Trace.WriteLine("  validTokenUrl               : \"{0}\"", validTokenUrl);
                pb.Trace.WriteLine("  key                         : \"{0}\"", connexion.Key);
                pb.Trace.WriteLine("  server time                 : {0} - {1:dd/MM/yyyy HH:mm:ss}", tokenResult.Result.zGet("ts").zAsInt(), connexion.ServerTime);
                pb.Trace.WriteLine("  server time gap             : {0}", connexion.ServerTimeGap);
            }

            _Login(validTokenUrl);

            connexion.zSave(_connexionFile);
            _connexion = connexion;

            if (_traceData != null)
                _traceData.Trace(new BsonDocument { { "OpeType", "Debrider.DebridLinkFr" }, { "Ope", "Login" }, { "Param", new BsonDocument { } }, { "Result", connexion.ToBsonDocument() } });
        }

        private DebridLinkFrRequestResult GetNewToken()
        {
            // request : https://debrid-link.fr/api/token/:publickey/new
            // result  :
            //{
            //  "result":"OK",
            //  "value":
            //  {
            //    "token":"10_2d600afa935e73e12898abac9ff075673c20625d99452393",
            //    "validTokenUrl":"https:\/\/debrid-link.fr\/user\/10_2d600afa935e73e12898abac9ff075673c20625d99452393\/login",
            //    "key":"sQ0bhf6k2A3yav5I"
            //  },
            //  "ts":1458902940
            //}

            string url = __url + string.Format("/token/{0}/new", _publicKey);
            DateTime requestTime = DateTime.Now;
            BsonDocument result = null;
            Exception ex = null;
            try
            {
                //Http.Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url }, _requestParameters);
                HttpResult<string> httpResult = _httpManagerConnexion.LoadText(new HttpRequest { Url = url });
                //result = BsonSerializer.Deserialize<BsonDocument>(http.ResultText);
                result = BsonSerializer.Deserialize<BsonDocument>(httpResult.Data);
            }
            catch (Exception ex2)
            {
                ex = ex2;
                throw;
            }
            finally
            {
                if (_traceData != null)
                {
                    _traceData.Trace(new BsonDocument { { "Category", "DebridLinkFr_v3" }, { "Ope", "GetNewToken" }, { "Key", "HttpRequest" }, { "Data", new BsonDocument { { "Url", url }, { "Result", result } } } }, ex);
                }
                if (__trace)
                {
                    pb.Trace.WriteLine("  get new token               : \"{0}\"", url);
                    pb.Trace.WriteLine("  result                      :");
                    pb.Trace.WriteLine(result.zToJson());
                }
            }

            return new DebridLinkFrRequestResult { RequestTime = requestTime, Result = result };
        }

        private DebridLinkConnexion CreateDebridLinkConnexion(DebridLinkFrRequestResult tokenResult)
        {
            DebridLinkConnexion connexion = new DebridLinkConnexion();
            connexion.ConnexionTime = tokenResult.RequestTime;
            connexion.Token = tokenResult.Result.zGet("value.token").zAsString();
            connexion.Key = tokenResult.Result.zGet("value.key").zAsString();
            int ts = tokenResult.Result.zGet("ts").zAsInt();
            connexion.ClientTime = tokenResult.RequestTime;
            connexion.ServerTime = zdate.UnixTimeStampToDateTime(ts);
            connexion.ServerTimeGap = connexion.ServerTime - tokenResult.RequestTime;
            connexion.ConnexionLifetime = _connexionLifetime;
            connexion.EndConnexionTime = connexion.ConnexionTime + GetConnexionTimespan(connexion.ConnexionLifetime) - TimeSpan.FromMinutes(5);
            return connexion;
        }

        private void _Login(string url)
        {
            // https://debrid-link.fr/user/2_21c744ba958f13fac08ee5c8855f72ab9a3b3e3224789126/login
            //Http.Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url }, _requestParameters);
            HttpResult<string> httpResult = _httpManagerConnexion.LoadText(new HttpRequest { Url = url });
            //XXElement xeSource = HttpManager.CurrentHttpManager.GetXDocument(http).zXXElement();
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();

            string loginUrl = __loginUrl;
            XXElement xeForm = xeSource.XPathElement("//form");

            string action = xeForm.AttribValue("action");
            if (action != null && action != "")
                loginUrl = action;

            string method = xeForm.AttribValue("method");
            HttpRequestMethod httpMethod = HttpRequestMethod.Get;
            if (method != null && method != "")
                httpMethod = HttpTools.GetHttpRequestMethod(method);

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
                //else if (name == "sessidTime")
                //    value = GetConnexionLifetime(_connexionLifetime);
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

            if (__trace)
            {
                pb.Trace.WriteLine("  form login url              : \"{0}\"", loginUrl);
                pb.Trace.WriteLine("  form action                 : \"{0}\"", action);
                pb.Trace.WriteLine("  form method                 : {0}", httpMethod);
                //pb.Trace.WriteLine("  form values                 : {0}", content.ToString());
            }

            //http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = loginUrl, Method = httpMethod, Content = content.ToString() }, _requestParameters);
            httpResult = _httpManagerConnexion.LoadText(new HttpRequest { Url = loginUrl, Method = httpMethod, Content = content.ToString() });

            //xeSource = http.zGetXDocument().zXXElement();
            xeSource = httpResult.zGetXDocument().zXXElement();
            //<div class="alert alert-success">
            XXElement xeLogin = xeSource.XPathElement("//div[@class='alert alert-success']");
            if (xeLogin.XElement == null)
                throw new PBException("can't login to debrid-link.fr");
        }

        //public BsonDocument ExecutePostCommand(string requestPath, string content = null)
        //{
        //    // from https://debrid-link.fr/api_doc/#/home
        //    // URL : https://debrid-link.fr/api/account/infos
        //    // X-DL-TOKEN : 10_d82555567e30c1e7137828eee6bf35429706d27e43319312
        //    // X-DL-SIGN : ab90fa6a2c9f1bc2bbd7988ff266971b5c10583c
        //    // X-DL-TS : 1418758917
        //    // Sign decoded : 1418758917/account/infosi619yOI4Kt8WB02g

        //    Connect();
        //    string url = __url + requestPath;
        //    if (__trace)
        //        pb.Trace.WriteLine("DebriderDebridLink.ExecutePostCommand() :");
        //    HttpRequestParameters requestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };
        //    StringBuilder contentBuilder = new StringBuilder();
        //    //contentBuilder.AppendFormat("token={0}", _connexion.Token);
        //    contentBuilder.AppendFormat("x-dl-token={0}", _connexion.Token);

        //    int timestamp = GetServerTimestamp();

        //    // test de décalage du timestamp
        //    //timestamp -= 15;  // KO avec -15 ou +15
        //    //if (__trace)
        //    //    pb.Trace.WriteLine("  server time + 60 sec        : {0}", zdate.UnixTimeStampToDateTime(timestamp));

        //    //contentBuilder.AppendFormat("&sign={0}", GetSignature(_connexion.Key, timestamp, requestPath));
        //    contentBuilder.AppendFormat("&x-dl-sign={0}", GetSignature(timestamp, requestPath, _connexion.Key));

        //    if (content != null)
        //    {
        //        contentBuilder.Append('&');
        //        contentBuilder.Append(content);
        //    }

        //    //contentBuilder.AppendFormat("&ts={0}", timestamp);
        //    contentBuilder.AppendFormat("&x-dl-ts={0}", timestamp);

        //    if (__trace)
        //        pb.Trace.WriteLine("  content                     : \"{0}\"", contentBuilder.ToString());
        //    DateTime dt = DateTime.Now;
        //    Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url, Method = HttpRequestMethod.Post, Content = contentBuilder.ToString() }, requestParameters);
        //    BsonDocument result = BsonDocument.Parse(http.ResultText);
        //    //if (__trace)
        //    //{
        //    int newTimestamp = result.zGet("ts").zAsInt();
        //    DateTime newServerTime = zdate.UnixTimeStampToDateTime(newTimestamp);
        //    TimeSpan newServerTimeGap = newServerTime - dt;
        //    // gap gap {2}    newServerTimeGap - _connexion.ServerTimeGap
        //    pb.Trace.WriteLine("  new server time             : {0} gap {1} timestamp {2} timestamp gap {3}", newServerTime, newServerTimeGap, newTimestamp, timestamp - newTimestamp);
        //    //}
        //    if (__trace)
        //    {
        //        pb.Trace.WriteLine("  result                      :");
        //        pb.Trace.WriteLine(result.zToJson());
        //    }
        //    return result;
        //}

        private int GetServerTimestamp()
        {
            DateTime time = DateTime.Now + _connexion.ServerTimeGap;
            int timestamp = zdate.DateTimeToUnixTimeStamp(time);
            if (__trace)
            {
                pb.Trace.WriteLine("  calculated server time      : {0} gap {1} timestamp {2}", time, _connexion.ServerTimeGap, timestamp);
            }
            return timestamp;
        }

        public static string GetSignature(int timestamp, string requestPath, string key)
        {
            // The signature is the SHA1 result of (Timestamp + Route + key) (from http://debrid-link.fr/api_doc/#/home)
            // Timestamp : server unix timestamp
            // Route : path of the request
            // key : key returned by authenticating
            // example Timestamp + Route + key : 1418758917/account/infosi619yOI4Kt8WB02g
            // SHA1 result : ab90fa6a2c9f1bc2bbd7988ff266971b5c10583c
            string signature = timestamp.ToString() + requestPath + key;

            string hash = Crypt.ComputeSHA1Hash(signature).zToHex(lowercase: true);

            if (__trace)
                pb.Trace.WriteLine("  signature                   : timestamp+request+key \"{0}\" = \"{1}\"", signature, hash);

            //if (__trace)
            //{
            //    pb.Trace.WriteLine("  signature                   : \"{0}\"", signature);
            //    pb.Trace.WriteLine("  signature sha1              : \"{0}\"", hash);
            //}
            return hash;
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

        private static TimeSpan? GetConnexionTimespan(DebridLinkConnexionLifetime connexionLifetime)
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

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using pb;
using pb.Data;
using pb.IO;
using pb.Text;

namespace pb.Web
{
    public class HttpLog
    {
    }

    public class HttpRequestParameters
    {
        //public bool UseWebClient = false;                                // static use System.Net.WebClient or System.Net.WebRequest
        public Encoding Encoding = null;                                 // static
        //public HttpRequestMethod Method = HttpRequestMethod.Get;         // request
        public string UserAgent = "Mozilla/5.0 Pib";                     // static
        public string Accept = null;                                     // request
        //public string Referer = null;                                    // request
        public DecompressionMethods? AutomaticDecompression = null;      // static
        public NameValueCollection Headers = new NameValueCollection();  // request
        public string ContentType = "application/x-www-form-urlencoded"; // static    valeur par defaut car obligatoire sur certain serveur (ex: http://www.handeco.org/fournisseurs/rechercher)
        //public string Content = null;                                    // request
        public CookieContainer Cookies = new CookieContainer();          // static
        public bool Expect100Continue = false;                           // false permet d'éviter que le content soit envoyé séparément avec Expect: 100-continue dans l'entete du 1er paquet
    }

    public class HttpManager
    {
        private static HttpManager __currentHttpManager = new HttpManager();
        //private HtmlXmlReader _hxr = null;
        //private ITrace _trace = pb.Trace.CurrentTrace;
        private bool _traceException = false;
        private int _loadRepeatIfError = 1;
        private int _loadRetryTimeout = 10;                                // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private bool _exportResult = false;
        private string _exportDirectory = null;

        public static HttpManager CurrentHttpManager { get { return __currentHttpManager; } }

        //public ITrace Trace { get { return _trace; } set { _trace = value; } }
        public bool TraceException { get { return _traceException; } set { _traceException = value; } }
        public int LoadRepeatIfError { get { return _loadRepeatIfError; } set { _loadRepeatIfError = value; } }
        public int LoadRetryTimeout { get { return _loadRetryTimeout; } set { _loadRetryTimeout = value; } }
        public bool ExportResult { get { return _exportResult; } set { _exportResult = value; } }
        public string ExportDirectory { get { return _exportDirectory; } set { _exportDirectory = value; } }

        public Http Load(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            try
            {
                //_hxr.Load(url, requestParameters);
                for (int i = 0; i < _loadRepeatIfError - 1; i++)
                {
                    try
                    {
                        return _Load(httpRequest, requestParameters);
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
                            // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                            if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                                throw;
                        }
                        if (ex is ProtocolViolationException)
                            throw;
                        //if (Trace.CurrentTrace.TraceLevel >= 1)
                        //    Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                        Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    }
                }
                return _Load(httpRequest, requestParameters);
            }
            catch (Exception ex)
            {
                //Load("http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html");
                //15/08/2013 12:00:32 Error : A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 5.199.168.178:80 (System.Net.Sockets.SocketException)
                //Unable to connect to the remote server (System.Net.WebException)
                //----------------------
                //   at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
                //   at System.Net.ServicePoint.ConnectSocketInternal(Boolean connectFailure, Socket s4, Socket s6, Socket& socket, IPAddress& address, ConnectSocketState state, IAsyncResult asyncResult, Int32 timeout, Exception& exception)
                //----------------------
                //   at System.Net.HttpWebRequest.GetResponse()
                //   at pb.old.Http.OpenWebRequest() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 911
                //   at pb.old.Http.Open() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 780
                //   at pb.old.Http.Load() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 503
                //   at pb.old.HtmlXmlReader.Load(String sUrl) in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\HtmlXmlReader.cs:line 426
                //   at Print.download.w.Test_frboard_02()
                //   at Print.download.w.Run()

                if (_traceException)
                    //Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    Trace.CurrentTrace.WriteError(ex);
                else
                    throw;
                return null;
            }
        }

        private Http _Load(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            //if (Trace.CurrentTrace.TraceLevel >= 1)
            //    Trace.WriteLine("Load(\"{0}\");", httpRequest.Url);
            Trace.WriteLine(1, "Load(\"{0}\");", httpRequest.Url);
            Http http = CreateHttp(httpRequest, requestParameters);

            http.Load();

            return http;
        }

        public bool LoadToFile(HttpRequest httpRequest, string file, HttpRequestParameters requestParameters = null)
        {
            try
            {
                //_hxr.LoadToFile(url, file, requestParameters);
                for (int i = 0; i < _loadRepeatIfError - 1; i++)
                {
                    try
                    {
                        return _LoadToFile(httpRequest, file, requestParameters);
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
                            // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                            if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                                throw;
                        }
                        //if (Trace.CurrentTrace.TraceLevel >= 1)
                        //    Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                        Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    }
                }
                return _LoadToFile(httpRequest, file, requestParameters);
            }
            catch (Exception ex)
            {
                if (_traceException)
                    Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                else
                    throw;
                return false;
            }
        }

        private bool _LoadToFile(HttpRequest httpRequest, string file, HttpRequestParameters requestParameters = null)
        {
            //if (Trace.CurrentTrace.TraceLevel >= 1)
            //    Trace.WriteLine("LoadToFile(\"{0}\", \"{1}\");", httpRequest.Url, file);
            Trace.WriteLine(1, "LoadToFile(\"{0}\", \"{1}\");", httpRequest.Url, file);
            Http http = CreateHttp(httpRequest, requestParameters);

            return http.LoadToFile(file);
        }

        public Image LoadImage(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            try
            {
                //Image image = _hxr.LoadImage(url, requestParameters);
                //return image;
                for (int i = 0; i < _loadRepeatIfError - 1; i++)
                {
                    try
                    {
                        return _LoadImage(httpRequest, requestParameters);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.GetType().FullName.StartsWith("System.Net."))
                            throw;
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
                            // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                            if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                                throw;
                        }
                        //if (Trace.CurrentTrace.TraceLevel >= 1)
                        //    Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                        Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    }
                }
                return _LoadImage(httpRequest, requestParameters);
            }
            catch (Exception ex)
            {
                pb.Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                return null;
            }
        }

        private Image _LoadImage(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            //if (Trace.CurrentTrace.TraceLevel >= 1)
            //    Trace.WriteLine("LoadImage(\"{0}\");", httpRequest.Url);
            Trace.WriteLine(1, "LoadImage(\"{0}\");", httpRequest.Url);
            if (httpRequest.Url.StartsWith("http://"))
            {
                return CreateHttp(httpRequest, requestParameters).LoadImage();
            }
            else
            {
                try
                {
                    return zimg.LoadFromFile(httpRequest.Url);
                }
                catch (Exception ex)
                {
                    pb.Trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    return null;
                }
            }
        }

        public Http CreateHttp(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            Http http = new Http(httpRequest, requestParameters);
            //http.HttpRetry += new Http.fnHttpRetry(LoadRetryEvent);
            http.LoadRetryTimeout = _loadRetryTimeout;
            //http.ReadCommentInText = _webReadCommentInText;
            //_webExportPath = null;
            //_webXmlExportPath = null;
            //if (Trace.CurrentTrace.TraceLevel >= 2)
            //    http.TraceDirectory = Trace.CurrentTrace.TraceDir;
            http.ExportResult = _exportResult;
            http.ExportDirectory = _exportDirectory;
            return http;
        }
    }

    public class Http : IDisposable
    {
        private static bool __trace = false;
        // http request
        private HttpRequest _httpRequest = null;
        private HttpRequestParameters _requestParameters = null;
        // parameters
        private int _loadRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private bool _exportResult = false;
        private string _exportDirectory = null;
        // work variables
        private Progress _progress = null;
        private System.Net.WebRequest _webRequest = null;
        private WebResponse _webResponse = null;
        private Stream _stream = null;
        private StreamReader _streamReader = null;
        private string _resultCharset = null;
        private string _resultContentType = null;
        private long _resultContentLength = -1;
        private string _resultText = null;
        private string _exportFile = null;

        public Http(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            _httpRequest = httpRequest;
            if (requestParameters != null)
                _requestParameters = requestParameters;
            else
                _requestParameters = new HttpRequestParameters();
            _progress = new Progress();
        }

        public void Dispose()
        {
            Close();
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public HttpRequest HttpRequest { get { return _httpRequest; } }
        public HttpRequestParameters RequestParameters { get { return _requestParameters; } }
        public int LoadRetryTimeout { get { return _loadRetryTimeout; } set { _loadRetryTimeout = value; } }
        public bool ExportResult { get { return _exportResult; } set { _exportResult = value; } }
        public string ExportDirectory { get { return _exportDirectory; } set { _exportDirectory = value; } }
        public string ExportFile { get { return _exportFile; } }
        public string ResultCharset { get { return _resultCharset; } }
        public string ResultContentType { get { return _resultContentType; } }
        public long ResultContentLength { get { return _resultContentLength; } }
        public string ResultText { get { return _resultText; } }

        public void Load()
        {
            try
            {
                if (__trace)
                    pb.Trace.WriteLine("Http.Load()");
                Open();
                if (_resultContentType.StartsWith("text") || _resultContentType == "application/json")
                {
                    _LoadText();
                    if (_exportResult && _exportDirectory != null)
                        _exportFile = GetNewHttpFileName(_exportDirectory, GetContentFileExtension(_resultContentType));
                    else if (_exportFile != null)
                    {
                        if (zPath.GetExtension(_exportFile) == "")
                            _exportFile = zpath.PathSetExtension(_exportFile, GetContentFileExtension(_resultContentType));
                    }
                    if (_exportFile != null)
                        zfile.WriteFile(_exportFile, _resultText);
                }
            }
            finally
            {
                Close();
            }
        }

        private void _LoadText()
        {
            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    CreateStreamReader();
                    _resultText = _streamReader.ReadToEnd();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                        throw;
                    if (ex is ThreadAbortException)
                        throw;
                    if (_loadRetryTimeout == 0)
                        throw;

                    if (dtFirstCatch.Ticks == 0)
                    {
                        dtFirstCatch = DateTime.Now;
                    }
                    else if (_loadRetryTimeout != -1)
                    {
                        dtFirstCatch = DateTime.Now;
                        TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                        if (ts.Seconds > _loadRetryTimeout)
                            throw;
                    }
                    //if (HttpRetry != null && !HttpRetry(ex))
                    //    throw;

                    Close();
                    Open();
                }
            }
        }

        public bool LoadToFile(string file)
        {
            bool ret = false;
            FileStream fs = null;
            try
            {
                if (__trace)
                    pb.Trace.WriteLine("Http.LoadToFile()");
                Open();
                zfile.CreateFileDirectory(file);
                fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);

                DateTime dtFirstCatch = new DateTime(0);
                while (true)
                {
                    try
                    {
                        //if (_abortTransfer)
                        //{
                        //    ret = false;
                        //    break;
                        //}
                        StreamTransfer streamTransfer = new StreamTransfer();
                        streamTransfer.SourceLength = _resultContentLength;
                        streamTransfer.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(StreamTransferProgressChange);
                        ret = streamTransfer.Transfer(_stream, fs);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException)
                            throw;
                        if (ex is ThreadAbortException)
                            throw;
                        if (_loadRetryTimeout == 0)
                            throw;

                        if (dtFirstCatch.Ticks == 0)
                        {
                            dtFirstCatch = DateTime.Now;
                        }
                        else if (_loadRetryTimeout != -1)
                        {
                            dtFirstCatch = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                            if (ts.Seconds > _loadRetryTimeout)
                                throw;
                        }
                        //if (HttpRetry != null && !HttpRetry(ex))
                        //    throw;

                        Close();
                        Open();
                        FileStream fs2 = fs;
                        fs = null;
                        fs2.Close();
                        fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                }
            }
            finally
            {
                //_abortTransfer = false;
                if (fs != null)
                    fs.Close();
                Close();
            }
            return ret;
        }

        public Image LoadImage()
        {
            try
            {
                if (__trace)
                    pb.Trace.WriteLine("Http.LoadImage()");
                Image image = null;
                Open();
                DateTime dtFirstCatch = new DateTime(0);
                while (true)
                {
                    try
                    {
                        //if (_abortTransfer)
                        //{
                        //    break;
                        //}
                        image = Image.FromStream(_stream);
                        break;
                    }
                    catch (Exception ex)
                    {
                        //if (ex is IOException)
                        //    throw;
                        if (!ex.GetType().FullName.StartsWith("System.Net."))
                            throw;
                        if (ex is ThreadAbortException)
                            throw;
                        if (_loadRetryTimeout == 0)
                            throw;

                        if (dtFirstCatch.Ticks == 0)
                        {
                            dtFirstCatch = DateTime.Now;
                        }
                        else if (_loadRetryTimeout != -1)
                        {
                            //dtFirstCatch = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                            if (ts.Seconds > _loadRetryTimeout)
                                throw;
                        }
                        //if (HttpRetry != null && !HttpRetry(ex))
                        //    throw;

                        Close();
                        Open();
                    }
                }
                return image;
            }
            finally
            {
                //_abortTransfer = false;
                Close();
            }
        }

        private void StreamTransferProgressChange(long current, long total)
        {
            _progress.SetProgress(current, total);
        }

        private void CreateStreamReader()
        {
            Encoding encoding;
            if (_requestParameters.Encoding != null)
                encoding = _requestParameters.Encoding;
            else
            {
                encoding = zconvert.GetEncoding(_resultCharset);
                //Trace.CurrentTrace.WriteLine("Http : charset encoding \"{0}\"", encoding.zToStringOrNull());
                if (encoding == null)
                    encoding = Encoding.Default;
            }
            //Trace.CurrentTrace.WriteLine("Http : encoding \"{0}\"", encoding.zToStringOrNull());
            _streamReader = new StreamReader(_stream, encoding);
        }

        private void Open()
        {
            //if (_opened)
            //    return;
            if (__trace)
                pb.Trace.WriteLine("Http.Open()");
            //if (_webRequest != null)
            //    return;
            //if (_url == null)
            //    return;

            //cTrace.Trace("{0} Http.OpenWebRequest() : gWebRequest = WebRequest.Create()", giOpenWebRequest++);
            _webRequest = System.Net.WebRequest.Create(_httpRequest.Url);
            if (_webRequest is HttpWebRequest)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)_webRequest;

                if (_requestParameters.UserAgent != null)
                    httpWebRequest.UserAgent = _requestParameters.UserAgent;
                if (_requestParameters.AutomaticDecompression != null)
                    httpWebRequest.AutomaticDecompression = (DecompressionMethods)_requestParameters.AutomaticDecompression;

                if (_httpRequest.Method == HttpRequestMethod.Get)
                    httpWebRequest.Method = "GET";
                else
                    httpWebRequest.Method = "POST";
                if (_requestParameters.Accept != null)
                    httpWebRequest.Accept = _requestParameters.Accept;
                if (_httpRequest.Referer != null)
                    httpWebRequest.Referer = _httpRequest.Referer;
                httpWebRequest.Headers.Add(_requestParameters.Headers);
                if (_requestParameters.Cookies == null)
                    _requestParameters.Cookies = new CookieContainer();
                httpWebRequest.CookieContainer = _requestParameters.Cookies;
                //Trace.WriteLine("set HttpWebRequest.ServicePoint.Expect100Continue = false");
                httpWebRequest.ServicePoint.Expect100Continue = _requestParameters.Expect100Continue;
                if (_httpRequest.Content != null)
                {
                    httpWebRequest.ContentType = _requestParameters.ContentType;
                    Encoding encoding;
                    if (_requestParameters.Encoding != null)
                        encoding = _requestParameters.Encoding;
                    else
                        encoding = Encoding.Default;
                    byte[] bytes = encoding.GetBytes(_httpRequest.Content);
                    httpWebRequest.ContentLength = bytes.LongLength;
                    Stream stream = httpWebRequest.GetRequestStream();
                    using (BinaryWriter w = new BinaryWriter(stream))
                    {
                        w.Write(bytes);
                    }
                }
            }

            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    _webResponse = _webRequest.GetResponse();
                    _stream = _webResponse.GetResponseStream();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                        throw;
                    if (ex is ThreadAbortException)
                        throw;

                    // error from frboard.com
                    // si pas de throw sur WebException ça boucle sans fin
                    // 16/08/2013 09:17:27 Error : A connection attempt failed because the connected party did not properly respond after a period of time,
                    //   or established connection failed because connected host has failed to respond 5.199.168.178:80 (System.Net.Sockets.SocketException)
                    // Unable to connect to the remote server (System.Net.WebException)
                    if (ex is WebException)
                    {
                        //WebException wex = (WebException)ex;
                        //if (   wex.Status != WebExceptionStatus.ConnectFailure
                        //    && wex.Status != WebExceptionStatus.PipelineFailure
                        //    && wex.Status != WebExceptionStatus.ProtocolError
                        //    && wex.Status != WebExceptionStatus.ReceiveFailure
                        //    && wex.Status != WebExceptionStatus.SendFailure
                        //    && wex.Status != WebExceptionStatus.ServerProtocolViolation
                        //    && wex.Status != WebExceptionStatus.Timeout
                        //    && wex.Status != WebExceptionStatus.UnknownError
                        //    )
                        throw;
                    }

                    if (_loadRetryTimeout == 0)
                        throw;

                    if (dtFirstCatch.Ticks == 0)
                    {
                        dtFirstCatch = DateTime.Now;
                    }
                    else if (_loadRetryTimeout != -1)
                    {
                        dtFirstCatch = DateTime.Now;
                        TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                        if (ts.Seconds > _loadRetryTimeout)
                            throw;
                    }
                    //if (HttpRetry != null && !HttpRetry(ex))
                    //    throw;
                    //WriteLine("error in OpenWebRequest : ", ex.Message);
                }
            }

            GetWebRequestHeaderValues();
            //_opened = true;
            //_result = true;
        }

        public void Close()
        {
            //if (!_opened) return;
            if (__trace)
                pb.Trace.WriteLine("Http.Close()");
            if (_streamReader != null)
            {
                _streamReader.Close();
                _streamReader = null;
            }
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
            if (_webResponse != null)
            {
                _webResponse.Close();
                _webResponse = null;
            }
            _webRequest = null;
            //_opened = false;
        }

        private void GetWebRequestHeaderValues()
        {
            if (_webResponse is HttpWebResponse)
            {

                HttpWebResponse httpResponse = (HttpWebResponse)_webResponse;
                _resultCharset = httpResponse.CharacterSet;
                if (_resultCharset != null) _resultCharset = _resultCharset.ToLower();

                string s = httpResponse.ContentType.ToLower();
                string[] s2 = zsplit.Split(s, ';', true);
                if (s2.Length > 0) _resultContentType = s2[0];

                _resultContentLength = httpResponse.ContentLength;
            }
            else if (_webResponse is FileWebResponse)
            {
                Uri uri = new Uri(_httpRequest.Url);
                string ext = zPath.GetExtension(uri.LocalPath).ToLower();
                switch (ext)
                {
                    case ".xml":
                        _resultContentType = "text/xml";
                        break;
                    case ".htm":
                    case ".html":
                    case ".asp":
                    case ".php":
                        _resultContentType = "text/html";
                        break;
                    case ".txt":
                        _resultContentType = "text/txt";
                        break;
                    default:
                        if (ext.Length > 1)
                            _resultContentType = "/" + ext.Substring(1);
                        break;
                }
            }
        }

        public string GetNewHttpFileName(string dir, string ext)
        {
            return GetNewUrlFileName(dir, _httpRequest.Url, ext);
        }

        private static string GetNewUrlFileName(string dir, string url, string ext)
        {
            //string file = UrlToFileName(url, ext);
            //return zfile.GetNewIndexedFileName(zPath.Combine(dir, "{0:0000}")) + "_" + file;
            //return zfile.GetNewIndexedFileName(dir) + "_" + UrlToFileName(url, ext);
            return zfile.GetNewIndexedFileName(dir) + "_" + zurl.UrlToFileName(url, UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query, ext);
        }

        //public static string UrlToFileName(string url, string ext, UrlFileNameType type = UrlFileNameType.FileName)
        //{
        //    string file;
        //    Uri uri = new Uri(url);
        //    if (type == UrlFileNameType.FileName)
        //    {
        //        file = uri.LocalPath;
        //        while (file.StartsWith("/")) file = file.Remove(0, 1);
        //        file = file.Replace("/", "_");
        //        if (file != "")
        //            file = uri.Host + "_" + file;
        //        else
        //            file = uri.Host;
        //        if (ext != null) file = file + ext;
        //    }
        //    else
        //    {
        //        if (type == UrlFileNameType.Path)
        //            file = uri.AbsolutePath;
        //        else if (type == UrlFileNameType.Query)
        //            file = uri.Query;
        //        else
        //            return null;
        //        int i = file.IndexOf("://");
        //        if (i != -1) file = zstr.right(file, file.Length - i - 3);
        //        i = file.IndexOf('?');
        //        if (i != -1) file = zstr.left(file, i);
        //        file = file.Replace('/', '_');
        //        file = file.Replace('%', '_');
        //        if (ext != null) file = file + ext;
        //    }
        //    file = zPath.GetFileName(file);
        //    return file;
        //}

        private static string GetContentFileExtension(string contentType)
        {
            contentType = contentType.ToLower();
            // text/html
            if (contentType.EndsWith("/html"))
                return ".html";
            // text/xml application/xml
            else if (contentType.EndsWith("/xml"))
                return ".xml";
            else
                return ".txt";
        }

        public static HttpRequestMethod GetHttpRequestMethod(string method)
        {
            switch (method.ToLower())
            {
                case "get":
                    return HttpRequestMethod.Get;
                case "post":
                    return HttpRequestMethod.Post;
                default:
                    throw new PBException("Error unknow HttpRequestMethod \"{0}\"", method);
            }
        }
    }
}

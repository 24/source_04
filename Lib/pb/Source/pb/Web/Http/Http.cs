using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using pb.IO;
using pb.Text;
using pb.Data.Mongo;

namespace pb.Web.Http
{
    //public class HttpResponseLog
    //{
    //    public int StatusCode;
    //    public WebHeaderCollection Headers;
    //    public HttpRequestLog Request;

    //    public HttpResponseLog(WebRequest webRequest, string webRequestContent, WebResponse webResponse)
    //    {
    //        if (webResponse is HttpWebResponse)
    //        {
    //            HttpWebResponse httpWebResponse = (HttpWebResponse)webResponse;
    //            StatusCode = (int)httpWebResponse.StatusCode;
    //            Headers = httpWebResponse.Headers;
    //            Request = new HttpRequestLog(webRequest, webRequestContent);
    //        }
    //    }
    //}

    public class Http : IDisposable
    {
        private static bool __trace = false;
        // http request
        private HttpRequest _httpRequest = null;
        private HttpRequestParameters _requestParameters = null;
        // parameters
        private int _loadRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        //private bool _exportResult = false;
        //private string _exportDirectory = null;
        // work variables
        private Progress _progress = null;
        private WebRequest _webRequest = null;
        private DateTime _webRequestTime;
        private TimeSpan _webRequestDuration;
        private WebResponse _webResponse = null;
        private Stream _stream = null;
        private StreamReader _streamReader = null;
        private string _resultCharset = null;
        private string _resultContentType = null;
        private long _resultContentLength = -1;
        private string _resultText = null;
        private string _exportFile = null;
        private bool _exportRequest = true;
        private bool _setExportFileExtension = false;

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
        //public bool ExportResult { get { return _exportResult; } set { _exportResult = value; } }
        //public string ExportDirectory { get { return _exportDirectory; } set { _exportDirectory = value; } }
        public string ExportFile { get { return _exportFile; } set { _exportFile = value; } }
        public bool SetExportFileExtension { get { return _setExportFileExtension; } set { _setExportFileExtension = value; } }
        public string ResultCharset { get { return _resultCharset; } }
        public string ResultContentType { get { return _resultContentType; } }
        public long ResultContentLength { get { return _resultContentLength; } }
        public string ResultText { get { return _resultText; } }

        //public void Load()
        public void LoadAsText()
        {
            try
            {
                if (__trace)
                    pb.Trace.WriteLine("Http.LoadAsText()");
                Open();
                if (_resultContentType != null && (_resultContentType.StartsWith("text") || _resultContentType == "application/json"))
                {
                    _LoadText();
                    //if (_exportResult && _exportDirectory != null)
                    //    _exportFile = GetNewHttpFileName(_exportDirectory, GetFileExtensionFromContentType(_resultContentType));
                    //else if (_exportFile != null)
                    //{
                    //    if (zPath.GetExtension(_exportFile) == "")
                    //        _exportFile = zpath.PathSetExtension(_exportFile, GetFileExtensionFromContentType(_resultContentType));
                    //}
                    if (_exportFile != null)
                    {
                        if (_setExportFileExtension)
                            _exportFile = zpath.PathSetExtension(_exportFile, HttpTools.GetFileExtensionFromContentType(_resultContentType));
                        zfile.WriteFile(_exportFile, _resultText);
                        if (_exportRequest)
                            ExportRequest(_exportFile);
                    }
                }
            }
            finally
            {
                Close();
            }
        }

        private void ExportRequest(string file)
        {
            //new HttpResponseLog(_webRequest, _httpRequest.Content, _webResponse).zSave(zpath.PathSetExtension(file, ".request.json"));
            new HttpLog { Request = new HttpRequestLog(_webRequest, _httpRequest.Content), Response = new HttpResponseLog(_webResponse) }.zSave(zpath.PathSetExtension(file, ".request.json"), jsonIndent: true);
        }

        private void _LoadText()
        {
            //DateTime dtFirstCatch = new DateTime(0);
            DateTime dtFirstCatch = DateTime.Now;
            while (true)
            {
                try
                {
                    CreateStreamReader();
                    _resultText = _streamReader.ReadToEnd();
                    _webRequestDuration = DateTime.Now - _webRequestTime;
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

        public bool LoadToFile(string file, bool exportRequest = false)
        {
            bool ret = false;
            FileStream fs = null;
            try
            {
                if (__trace)
                    pb.Trace.WriteLine("Http.LoadToFile()");
                Open();
                zfile.CreateFileDirectory(file);
                //fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read);

                //DateTime dtFirstCatch = new DateTime(0);
                DateTime dtFirstCatch = DateTime.Now;
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
                        _webRequestDuration = DateTime.Now - _webRequestTime;
                        if (exportRequest)
                            ExportRequest(file);
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
                        //fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                        fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read);
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
                //DateTime dtFirstCatch = new DateTime(0);
                DateTime dtFirstCatch = DateTime.Now;
                while (true)
                {
                    try
                    {
                        //if (_abortTransfer)
                        //{
                        //    break;
                        //}
                        image = Image.FromStream(_stream);
                        _webRequestDuration = DateTime.Now - _webRequestTime;
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
                    // modif le 05/04/2016 set default encoding to utf8
                    // attention ne pas changer l'encoding par défaut, il est utilisé par le cache
                    encoding = Encoding.Default;
            }
            //Trace.CurrentTrace.WriteLine("Http : encoding \"{0}\"", encoding.zToStringOrNull());
            _streamReader = new StreamReader(_stream, encoding);
        }

        private void Open()
        {
            if (__trace)
                pb.Trace.WriteLine("Http.Open()");

            _webRequest = WebRequest.Create(_httpRequest.Url);
            if (_webRequest is HttpWebRequest)
            {
                // HttpWebRequest Class https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest(v=vs.110).aspx

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

            //DateTime dtFirstCatch = new DateTime(0);
            DateTime dtFirstCatch = DateTime.Now;
            while (true)
            {
                try
                {
                    _webRequestTime = DateTime.Now;
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
                //Uri uri = new Uri(_httpRequest.Url);
                //string ext = zPath.GetExtension(uri.LocalPath).ToLower();
                //switch (ext)
                //{
                //    case ".xml":
                //        _resultContentType = "text/xml";
                //        break;
                //    case ".htm":
                //    case ".html":
                //    case ".asp":
                //    case ".php":
                //        _resultContentType = "text/html";
                //        break;
                //    case ".txt":
                //        _resultContentType = "text/txt";
                //        break;
                //    default:
                //        if (ext.Length > 1)
                //            _resultContentType = "/" + ext.Substring(1);
                //        break;
                //}
                // modif le 09/11/2015 ne gère plus les extensions inconnues ("/unknow_ext")
                _resultContentType = HttpTools.GetContentTypeFromFileExtension(zPath.GetExtension(new Uri(_httpRequest.Url).LocalPath));
            }
        }

        //public string GetNewHttpFileName(string dir, string ext)
        //{
        //    return GetNewUrlFileName(dir, _httpRequest.Url, ext);
        //}

        //private static string GetNewUrlFileName(string dir, string url, string ext)
        //{
        //    //string file = UrlToFileName(url, ext);
        //    //return zfile.GetNewIndexedFileName(zPath.Combine(dir, "{0:0000}")) + "_" + file;
        //    //return zfile.GetNewIndexedFileName(dir) + "_" + UrlToFileName(url, ext);
        //    return zfile.GetNewIndexedFileName(dir) + "_" + zurl.UrlToFileName(url, UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query, ext);
        //}

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

        public static Http LoadAsText(HttpRequest httpRequest, HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false)
        {
            Http http = new Http(httpRequest, requestParameters);
            http.ExportFile = exportFile;
            http.SetExportFileExtension = setExportFileExtension;
            http.LoadAsText();
            return http;
        }
    }
}

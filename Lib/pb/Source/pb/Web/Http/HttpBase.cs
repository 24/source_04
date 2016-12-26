using pb.Data.Mongo;
using pb.IO;
using pb.Text;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

// save encoding
namespace pb.Web
{
    public class HttpLog
    {
        public HttpRequestLog Request;
        public HttpResponseLog Response;
        public HttpValuesLog Values;
    }

    public class HttpRequestLog
    {
        public Uri Uri;
        public string Method;
        public WebHeaderCollection Headers;
        public string Content;

        public HttpRequestLog(WebRequest webRequest, string webRequestContent)
        {
            //if (webRequest is HttpWebRequest)
            //{
            //HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
            Uri = webRequest.RequestUri;
            Method = webRequest.Method;
            Headers = webRequest.Headers;
            Content = webRequestContent;
            //}
        }
    }

    public class HttpResponseLog
    {
        public int StatusCode;
        public WebHeaderCollection Headers;

        public HttpResponseLog(WebResponse webResponse)
        {
            if (webResponse is HttpWebResponse)
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)webResponse;
                StatusCode = (int)httpWebResponse.StatusCode;
                Headers = httpWebResponse.Headers;
            }
        }
    }

    public class HttpValuesLog
    {
        public DateTime RequestTime;
        public TimeSpan RequestDuration;
        public string Charset;
        public string ContentType;
        //public UrlCachePathResult UrlCachePath = null;
        public string CacheFile = null;      // sub-path of cache file
    }

    public partial class HttpBase : IDisposable
    {
        protected static bool __trace = false;

        // http request
        protected HttpRequest _httpRequest = null;
        protected HttpRequestParameters _requestParameters = null;
        //private bool _cacheFile = false;

        private string _exportFile = null;
        private bool _exportRequest = true;
        private bool _setExportFileExtension = false;

        // parameters
        protected int _loadRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout

        // WebRequest
        private WebRequest _webRequest = null;
        private WebResponse _webResponse = null;
        private bool _webResponseClosed = false;

        private HttpRequestLog _httpRequestLog = null;
        private HttpResponseLog _httpResponseLog = null;

        private Stream _stream = null;
        private DateTime _requestTime;
        private TimeSpan _requestDuration;

        protected string _resultCharset = null;
        protected string _resultContentType = null;
        protected long _resultContentLength = -1;

        // bool cacheFile = false
        public HttpBase(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            _httpRequest = httpRequest;
            if (requestParameters != null)
                _requestParameters = requestParameters;
            else
                _requestParameters = new HttpRequestParameters();
            //_cacheFile = cacheFile;
            //_progress = new Progress();
        }

        public HttpBase(HttpLog httpLog)
        {
            ImportRequest(httpLog);
            SetHttpRequest(httpLog);
        }

        public virtual void Dispose()
        {
            Close();
        }

        public HttpRequest HttpRequest { get { return _httpRequest; } }
        public int LoadRetryTimeout { get { return _loadRetryTimeout; } set { _loadRetryTimeout = value; } }
        public string ExportFile { get { return _exportFile; } set { _exportFile = value; } }
        public bool SetExportFileExtension { get { return _setExportFileExtension; } set { _setExportFileExtension = value; } }
        public DateTime RequestTime { get { return _requestTime; } }
        public TimeSpan RequestDuration { get { return _requestDuration; } }

        public string ResultCharset { get { return _resultCharset; } }
        public string ResultContentType { get { return _resultContentType; } }
        public long ResultContentLength { get { return _resultContentLength; } }

        protected void Try(Action action, Action retry = null, Action final = null)
        {
            try
            {
                Open();

                DateTime dtFirstCatch = DateTime.Now;
                while (true)
                {
                    try
                    {
                        action();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (_loadRetryTimeout == 0)
                            throw;
                        if (!ex.GetType().FullName.StartsWith("System.Net."))
                            throw;
                        if (ex is IOException)
                            throw;
                        if (ex is ThreadAbortException)
                            throw;
                        // from HttpManager.Load()
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

                        // if not throw trace exception
                        Trace.WriteLine("Error : loading \"{0}\" ({1}) {2}", _httpRequest.UrlCachePath != null ? _httpRequest.UrlCachePath.Path : _httpRequest.Url, _httpRequest.Method, ex.Message);

                        Close();
                        Open();

                        if (retry != null)
                            retry();
                    }
                }
            }
            finally
            {
                Close();

                if (final != null)
                    final();
            }
        }

        protected void SetRequestDuration(TimeSpan duration)
        {
            //if (!_httpRequest.CacheFile)
            if (_httpRequest.UrlCachePath == null)
                _requestDuration = duration;
        }

        protected void Open()
        {
            if (__trace)
                pb.Trace.WriteLine("Http.Open()");
            //if (_httpRequest.CacheFile)
            if (_httpRequest.UrlCachePath != null)
                OpenCacheFile();
            else
            {
                //OpenWebRequest();
                CreateWebRequest();
                OpenWebRequest();
            }
        }

        private void OpenCacheFile()
        {
            // set _cacheFile, _httpRequestLog, _httpResponseLog, _webRequestTime, _webRequestDuration, _resultCharset, _resultContentType, _resultContentLength

            //_cacheFile = true;
            //ImportRequest(_httpRequest.Url);
            ImportRequest(_httpRequest.UrlCachePath.Path);
        }

        private void CreateWebRequest()
        {
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
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(bytes);
                    }
                }
            }
            //_httpRequestLog = new HttpRequestLog(_webRequest, _httpRequest.Content);
        }

        private void OpenWebRequest()
        {
            try
            {
                _requestTime = DateTime.Now;
                _webResponseClosed = false;
                _webResponse = _webRequest.GetResponse();
                //_httpResponseLog = new HttpResponseLog(_webResponse);
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException wex = (WebException)ex;
                    _webResponse = wex.Response;
                }
                throw;
            }
            finally
            {
                GetWebRequestHeaderValues();
            }
        }

        //private void OpenWebRequest_v1()
        //{
        //    // set _cacheFile, _webRequest, _httpRequestLog, _webResponse, _httpResponseLog, _webRequestTime, _resultCharset, _resultContentType, _resultContentLength

        //    //_cacheFile = false;
        //    _webRequest = WebRequest.Create(_httpRequest.Url);
        //    if (_webRequest is HttpWebRequest)
        //    {
        //        // HttpWebRequest Class https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest(v=vs.110).aspx

        //        HttpWebRequest httpWebRequest = (HttpWebRequest)_webRequest;

        //        if (_requestParameters.UserAgent != null)
        //            httpWebRequest.UserAgent = _requestParameters.UserAgent;
        //        if (_requestParameters.AutomaticDecompression != null)
        //            httpWebRequest.AutomaticDecompression = (DecompressionMethods)_requestParameters.AutomaticDecompression;

        //        if (_httpRequest.Method == HttpRequestMethod.Get)
        //            httpWebRequest.Method = "GET";
        //        else
        //            httpWebRequest.Method = "POST";
        //        if (_requestParameters.Accept != null)
        //            httpWebRequest.Accept = _requestParameters.Accept;
        //        if (_httpRequest.Referer != null)
        //            httpWebRequest.Referer = _httpRequest.Referer;
        //        httpWebRequest.Headers.Add(_requestParameters.Headers);
        //        if (_requestParameters.Cookies == null)
        //            _requestParameters.Cookies = new CookieContainer();
        //        httpWebRequest.CookieContainer = _requestParameters.Cookies;
        //        httpWebRequest.ServicePoint.Expect100Continue = _requestParameters.Expect100Continue;
        //        if (_httpRequest.Content != null)
        //        {
        //            httpWebRequest.ContentType = _requestParameters.ContentType;
        //            Encoding encoding;
        //            if (_requestParameters.Encoding != null)
        //                encoding = _requestParameters.Encoding;
        //            else
        //                encoding = Encoding.Default;
        //            byte[] bytes = encoding.GetBytes(_httpRequest.Content);
        //            httpWebRequest.ContentLength = bytes.LongLength;
        //            Stream stream = httpWebRequest.GetRequestStream();
        //            using (BinaryWriter writer = new BinaryWriter(stream))
        //            {
        //                writer.Write(bytes);
        //            }
        //        }
        //    }
        //    _httpRequestLog = new HttpRequestLog(_webRequest, _httpRequest.Content);

        //    DateTime dtFirstCatch = DateTime.Now;
        //    while (true)
        //    {
        //        try
        //        {
        //            _requestTime = DateTime.Now;
        //            _webResponse = _webRequest.GetResponse();
        //            _httpResponseLog = new HttpResponseLog(_webResponse);
        //            ////////////////////////_stream = _webResponse.GetResponseStream();
        //            break;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (ex is IOException)
        //                throw;
        //            if (ex is ThreadAbortException)
        //                throw;

        //            // error from frboard.com
        //            // si pas de throw sur WebException ça boucle sans fin
        //            // 16/08/2013 09:17:27 Error : A connection attempt failed because the connected party did not properly respond after a period of time,
        //            //   or established connection failed because connected host has failed to respond 5.199.168.178:80 (System.Net.Sockets.SocketException)
        //            // Unable to connect to the remote server (System.Net.WebException)
        //            if (ex is WebException)
        //            {
        //                //WebException wex = (WebException)ex;
        //                //if (   wex.Status != WebExceptionStatus.ConnectFailure
        //                //    && wex.Status != WebExceptionStatus.PipelineFailure
        //                //    && wex.Status != WebExceptionStatus.ProtocolError
        //                //    && wex.Status != WebExceptionStatus.ReceiveFailure
        //                //    && wex.Status != WebExceptionStatus.SendFailure
        //                //    && wex.Status != WebExceptionStatus.ServerProtocolViolation
        //                //    && wex.Status != WebExceptionStatus.Timeout
        //                //    && wex.Status != WebExceptionStatus.UnknownError
        //                //    )
        //                throw;
        //            }

        //            if (_loadRetryTimeout == 0)
        //                throw;

        //            if (dtFirstCatch.Ticks == 0)
        //            {
        //                dtFirstCatch = DateTime.Now;
        //            }
        //            else if (_loadRetryTimeout != -1)
        //            {
        //                dtFirstCatch = DateTime.Now;
        //                TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
        //                if (ts.Seconds > _loadRetryTimeout)
        //                    throw;
        //            }
        //            //if (HttpRetry != null && !HttpRetry(ex))
        //            //    throw;
        //            //WriteLine("error in OpenWebRequest : ", ex.Message);
        //        }
        //    }

        //    GetWebRequestHeaderValues();
        //}

        public virtual void Close()
        {
            if (__trace)
                pb.Trace.WriteLine("Http.Close()");

            //if (_streamReader != null)
            //{
            //    _streamReader.Close();
            //    _streamReader = null;
            //}

            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (_webResponse != null && !_webResponseClosed)
            {
                _webResponseClosed = true;
                _webResponse.Close();
                //_webResponse = null;
            }
            //_webRequest = null;
        }

        protected Stream GetResponseStream()
        {
            if (_stream == null)
            {
                //if (_httpRequest.CacheFile)
                if (_httpRequest.UrlCachePath != null)
                    //_stream = zFile.OpenRead(_httpRequest.Url);
                    _stream = zFile.OpenRead(_httpRequest.UrlCachePath.Path);
                else
                    _stream = _webResponse.GetResponseStream();
            }
            return _stream;
        }

        protected Encoding GetResponseStreamEncoding()
        {
            Encoding encoding;
            //if (_httpRequest.CacheFile)
            if (_httpRequest.UrlCachePath != null)
            {
                // cache encoding is always UTF8
                encoding = Encoding.UTF8;
            }
            else
            {
                if (_requestParameters.Encoding != null)
                    encoding = _requestParameters.Encoding;
                else
                {
                    encoding = zconvert.GetEncoding(_resultCharset);
                    //Trace.CurrentTrace.WriteLine("Http : charset encoding \"{0}\"", encoding.zToStringOrNull());
                    if (encoding == null)
                        // modif le 05/04/2016 set default encoding to utf8
                        // attention ne pas changer l'encoding par défaut, il est utilisé par le cache
                        // maintenant on peut le changer
                        encoding = Encoding.UTF8;
                }
            }
            return encoding;
        }

        private void GetWebRequestHeaderValues()
        {
            // set _resultCharset, _resultContentType, _resultContentLength

            if (_webResponse is HttpWebResponse)
            {

                HttpWebResponse httpResponse = (HttpWebResponse)_webResponse;

                _resultCharset = httpResponse.CharacterSet;
                if (_resultCharset != null)
                    _resultCharset = _resultCharset.ToLower();

                string s = httpResponse.ContentType.ToLower();
                string[] s2 = zsplit.Split(s, ';', true);
                if (s2.Length > 0)
                    _resultContentType = s2[0];

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
                _resultContentType = GetContentTypeFromFileExtension(zPath.GetExtension(new Uri(_httpRequest.Url).LocalPath));
            }
        }

        protected void Export(string text)
        {
            if (_exportFile != null)
            {
                if (_setExportFileExtension)
                    _exportFile = zpath.PathSetExtension(_exportFile, GetFileExtensionFromContentType(_resultContentType));
                zfile.WriteFile(_exportFile, text);
                if (_exportRequest)
                    ExportRequest(_exportFile);
            }
        }

        public HttpLog GetHttpLog()
        {
            return new HttpLog
            {
                //Request = _httpRequestLog,
                Request = GetHttpRequestLog(),
                //Response = _httpResponseLog,
                Response = GetHttpResponseLog(),
                //Values = new HttpValuesLog { RequestTime = _requestTime, RequestDuration = _requestDuration, Charset = _resultCharset, ContentType = _resultContentType, UrlCachePath = _httpRequest.UrlCachePath }
                Values = new HttpValuesLog { RequestTime = _requestTime, RequestDuration = _requestDuration, Charset = _resultCharset, ContentType = _resultContentType, CacheFile = _httpRequest.UrlCachePath != null ? _httpRequest.UrlCachePath.SubPath : null }
            };
        }

        private HttpRequestLog GetHttpRequestLog()
        {
            if (_httpRequestLog == null)
            {
                if (_webRequest == null)
                    throw new PBException("WebRequest is not defined");
                _httpRequestLog = new HttpRequestLog(_webRequest, _httpRequest.Content);
            }
            return _httpRequestLog;
        }

        private HttpResponseLog GetHttpResponseLog()
        {
            if (_httpResponseLog == null)
            {
                if (_webResponse == null)
                    throw new PBException("WebResponse is not defined");
                _httpResponseLog = new HttpResponseLog(_webResponse);
            }
            return _httpResponseLog;
        }

        protected void ExportRequest(string file)
        {
            //new HttpResponseLog(_webRequest, _httpRequest.Content, _webResponse).zSave(zpath.PathSetExtension(file, ".request.json"));
            GetHttpLog().zSave(zpath.PathSetExtension(file, ".request.json"), jsonIndent: true);
        }

        private void ImportRequest(string file)
        {
            file = zpath.PathSetExtension(file, ".request.json");
            if (!zFile.Exists(file))
                throw new PBException($"request file not found \"{file}\"");
            ImportRequest(zmongo.ReadFileAs<HttpLog>(file));
        }

        private void ImportRequest(HttpLog httpLog)
        {
            _httpRequestLog = httpLog.Request;
            _httpResponseLog = httpLog.Response;
            HttpValuesLog values = httpLog.Values;
            _requestTime = values.RequestTime;
            _requestDuration = values.RequestDuration;
            _resultCharset = values.Charset;
            _resultContentType = values.ContentType;
            //public UrlCachePathResult UrlCachePath = null;
            //public string CacheFile = null;      // sub-path of cache file
        }

        private void SetHttpRequest(HttpLog httpLog)
        {
            _httpRequest = new HttpRequest();
            _httpRequest.Url = httpLog.Request.Uri.ToString();
            //_httpRequest.Method = _httpRequestLog.Method;
            //_httpRequest.Referer
            _httpRequest.Content = httpLog.Request.Content;
            _httpRequest.UrlCachePath = new UrlCachePathResult { SubPath = httpLog.Values.CacheFile };
        }

        private static string GetFileExtensionFromContentType(string contentType)
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

        public static string GetContentTypeFromFileExtension(string ext)
        {
            switch (ext.ToLower())
            {
                case ".xml":
                    return "text/xml";
                case ".htm":
                case ".html":
                case ".asp":
                case ".php":
                    return "text/html";
                case ".txt":
                    return "text/txt";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".tiff":
                    return "image/tiff";
                case ".bmp":
                    return "image/bmp";
                default:
                    return null;
                    //default:
                    //    if (ext.Length > 1)
                    //        _resultContentType = "/" + ext.Substring(1);
                    //    break;
            }
        }
    }
}

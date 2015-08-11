using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Web;
using pb.Web.old;

namespace pb.old
{
    public class Http_v1 : IDisposable
    {
        #region doc
        // - le fait de modifier un paramètre ferme la session en cours (appel de Close())
        // - les paramètres suivants sont remis à zéro à chaque fin de session (Close() appel ResetParameters())
        //   gMethod, gsReferer, gHeaders, gsRequestContentType, gsContent, gsExportPath
        #endregion

        #region variable
        private static bool _trace = false;

        // paramètre :
        private string _url = null;

        //private HttpStaticRequestParameters _staticRequestParameters = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        //private bool _useWebClient = false; // use System.Net.WebClient or System.Net.WebRequest
        //private Encoding _encoding = null;
        //private HttpRequestMethod _method;
        //private string _userAgent = "Pib";
        //private string _accept = null;
        //private string _referer = null;
        //private DecompressionMethods? _automaticDecompression = null;
        //private NameValueCollection _headers = new NameValueCollection();
        //private string _requestContentType = null;
        //private string _content = null;
        //private CookieContainer _cookies = null;

        private int _loadRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private string _traceDirectory = null;
        private string _textExportPath = null;
        private string _xmlExportPath = null;
        private bool _readCommentInText = false;

        // résultat de la requete :
        private string _resultText = null;
        private string _resultContentType = null;
        private string _resultCharset = null;
        private long _resultContentLength = -1;

        private WebClient _webClient = null;

        private System.Net.WebRequest _webRequest = null;
        private WebResponse _webResponse = null;

        private Progress _progress = null;
        private StreamTransfer _streamTransfer = null;
        private bool _abortTransfer = false;
        private Stream _stream = null;
        private StreamReader _webStream = null;
        private Encoding _webStreamEncoding = null;
        private bool _opened = false;
        private bool _result = false;
        public delegate bool fnHttpRetry(Exception ex);
        /// <summary>
        /// valeur retournée : true pour recommencer, false pour arrêter
        /// </summary>
        public fnHttpRetry HttpRetry = null;

        private static Regex _translate1 = new Regex(@"&([a-zA-Z]+)\w*;?", RegexOptions.Compiled);
        private static Regex _translate2 = new Regex(@"&#([0-9]+);", RegexOptions.Compiled);
        private static Regex _translate3 = new Regex(@"&#x([a-fA-F0-9]+);", RegexOptions.Compiled);

        #endregion

        public Http_v1()
        {
            Init();
        }

        //public Http(string url)
        //{
        //    _url = url;
        //    Init();
        //}

        //HttpStaticRequestParameters staticRequestParameters = null
        public Http_v1(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            _url = url;
            if (requestParameters != null)
                _requestParameters = requestParameters;
            else
                _requestParameters = new HttpRequestParameters_v1();
            //if (staticRequestParameters != null)
            //    _staticRequestParameters = staticRequestParameters;
            //else
            //    _staticRequestParameters = new HttpStaticRequestParameters();
            Init();
        }

        //public Http(string sUrl, CookieContainer cookies)
        //{
        //    _url = sUrl;
        //    _cookies = cookies;
        //    Init();
        //}

        public void Dispose()
        {
            Close();
        }

        #region property ...
        public static bool Trace { get { return _trace; } set { _trace = value; } }

        public string Url
        {
            get { return _url; }
            set
            {
                Reset();
                _url = zurl.GetUrl(_url, value);
            }
        }

        //public HttpStaticRequestParameters StaticRequestParameters
        //{
        //    get { return _staticRequestParameters; }
        //    set
        //    {
        //        Reset();
        //        _staticRequestParameters = value;
        //    }
        //}

        //public HttpRequestParameters RequestParameters
        //{
        //    get { return _requestParameters; }
        //    set
        //    {
        //        Reset();
        //        _requestParameters = value;
        //    }
        //}


        //public HttpRequestMethod Method
        //{
        //    get { return _method; }
        //    set
        //    {
        //        Reset();
        //        _method = value;
        //    }
        //}

        //public string UserAgent
        //{
        //    get { return _userAgent; }
        //    set
        //    {
        //        Reset();
        //        _userAgent = value;
        //    }
        //}

        //public string Accept
        //{
        //    get { return _accept; }
        //    set
        //    {
        //        Reset();
        //        _accept = value;
        //    }
        //}

        //public string Referer
        //{
        //    get { return _referer; }
        //    set
        //    {
        //        Reset();
        //        _referer = value;
        //    }
        //}

        //public DecompressionMethods? AutomaticDecompression
        //{
        //    get { return _automaticDecompression; }
        //    set
        //    {
        //        Reset();
        //        _automaticDecompression = value;
        //    }
        //}

        //public NameValueCollection Headers
        //{
        //    get { return _headers; }
        //    set
        //    {
        //        Reset();
        //        _headers = value;
        //    }
        //}

        //public string RequestContentType
        //{
        //    get { return _requestContentType; }
        //    set
        //    {
        //        Reset();
        //        _requestContentType = value;
        //    }
        //}

        //public string Content
        //{
        //    get { return _content; }
        //    set
        //    {
        //        Reset();
        //        _content = value;
        //    }
        //}

        //public Encoding Encoding
        //{
        //    get { return _encoding; }
        //    set
        //    {
        //        Reset();
        //        _encoding = value;
        //    }
        //}

        //public bool UseWebClient
        //{
        //    get { return _useWebClient; }
        //    set
        //    {
        //        Reset();
        //        _useWebClient = value;
        //    }
        //}

        public WebClient WebClient
        {
            get
            {
                Open();
                return _webClient;
            }
        }

        public System.Net.WebRequest Request
        {
            get
            {
                Open();
                return _webRequest;
            }
        }

        public WebResponse Response
        {
            get
            {
                Open();
                return _webResponse;
            }
        }

        //public CookieContainer Cookies
        //{
        //    get
        //    {
        //        Open();
        //        return _cookies;
        //    }
        //    set
        //    {
        //        Reset();
        //        _cookies = value;
        //    }
        //}

        public StreamReader WebStream
        {
            get
            {
                Open();
                CreateStreamReader();
                return _webStream;
            }
        }

        public Encoding WebStreamEncoding
        {
            get
            {
                //Open();
                return _webStreamEncoding;
            }
        }

        public string TextResult
        {
            get { return _resultText; }
        }

        public string ContentType
        {
            get
            {
                Open();
                return _resultContentType;
            }
        }

        public string Charset
        {
            get
            {
                Open();
                return _resultCharset;
            }
        }

        public long ContentLength
        {
            get
            {
                Open();
                return _resultContentLength;
            }
        }

        /// <summary>
        /// timeout in seconds, 0 = no timeout, -1 = endless timeout
        /// </summary>
        public int LoadRetryTimeout
        {
            get { return _loadRetryTimeout; }
            set
            {
                Reset();
                _loadRetryTimeout = value;
            }
        }

        public string TraceDirectory
        {
            get { return _traceDirectory; }
            set
            {
                Reset();
                _traceDirectory = value;
            }
        }

        public string TextExportPath
        {
            get { return _textExportPath; }
            set
            {
                Reset();
                _textExportPath = value;
            }
        }

        public string XmlExportPath
        {
            get { return _xmlExportPath; }
            set
            {
                Reset();
                _xmlExportPath = value;
            }
        }

        public bool ReadCommentInText
        {
            get { return _readCommentInText; }
            set
            {
                Reset();
                _readCommentInText = value;
            }
        }

        public Progress Progress
        {
            get { return _progress; }
        }

        //public static bool Trace { get { return _trace; } set { _trace = value; } }
        #endregion

        private void Init()
        {
            _progress = new Progress();
            //gProgress.ProgressControlChanged += new Progress.ProgressControlChangedEventHandler(ProgressControlChanged);
        }

        public void AbortTransfer()
        {
            _abortTransfer = true;
            if (_streamTransfer != null) _streamTransfer.AbortTransfer();
        }

        public void CancelAbortTransfer()
        {
            _abortTransfer = false;
            if (_streamTransfer != null) _streamTransfer.CancelAbortTransfer();
        }

        public void Load(string url)
        {
            //Load(url, HttpRequestMethod.Get, null, null);
            Reset();
            _url = zurl.GetUrl(_url, url);
            Load();
        }

        //public void Load(string url, HttpRequestMethod method, string content)
        //{
        //    //Load(url, method, content, null);
        //    Reset();
        //    _url = zurl.GetUrl(_url, url);
        //    _method = method;
        //    _content = content;
        //    Load();
        //}

        //public void Load(string url, HttpRequestMethod method, string content, string referer)
        //{
        //    Reset();
        //    _url = zurl.GetUrl(_url, url);
        //    _method = method;
        //    _content = content;
        //    _referer = referer;
        //    Load();
        //}

        public void Load()
        {
            try
            {
                if (_trace)
                    pb.Trace.WriteLine("Http.Load()");
                Open();
                if (_resultContentType.StartsWith("text"))
                {
                    _LoadText();
                    if (_traceDirectory != null)
                        _textExportPath = GetNewHttpFileName(_traceDirectory, GetContentFileExtension(_resultContentType));
                    else if (_textExportPath != null)
                    {
                        if (zPath.GetExtension(_textExportPath) == "")
                            _textExportPath = zpath.PathSetExtension(_textExportPath, GetContentFileExtension(_resultContentType));
                    }
                    if (_textExportPath != null)
                        zfile.WriteFile(_textExportPath, _resultText);
                }
            }
            finally
            {
                // modif le 04/04/2014 mais à quoi sert ce close, cela fait que les pages sont chargées 2 fois
                // modif 06/11/2014 retry to Close()
                Close();
            }
        }

        public Image LoadImage(string url)
        {
            Reset();
            _url = zurl.GetUrl(_url, url);
            return LoadImage();
        }

        public Image LoadImage()
        {
            try
            {
                if (_trace)
                    pb.Trace.WriteLine("Http.LoadImage()");
                Image image = null;
                Open();
                DateTime dtFirstCatch = new DateTime(0);
                while (true)
                {
                    try
                    {
                        if (_abortTransfer)
                        {
                            break;
                        }
                        //gStreamTransfer = new StreamTransfer();
                        //gStreamTransfer.SourceLength = gContentLength;
                        //gStreamTransfer.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(StreamTransferProgressChange);
                        //ret = gStreamTransfer.Transfer(gStream, fs);
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
                        if (HttpRetry != null && !HttpRetry(ex))
                            throw;

                        Close();
                        Open();
                    }
                }
                return image;
            }
            finally
            {
                _abortTransfer = false;
                Close();
            }
        }

        //public bool LoadToFile(string path, string url)
        //{
        //    return LoadToFile(path, url, HttpRequestMethod.Get, null, null);
        //}

        //public bool LoadToFile(string path, string url, HttpRequestMethod method, string content)
        //{
        //    return LoadToFile(path, url, method, content, null);
        //}

        //public bool LoadToFile(string path, string url, HttpRequestMethod method, string content, string referer)
        public bool LoadToFile(string path, string url)
        {
            Reset();
            _url = zurl.GetUrl(_url, url);
            //_method = method;
            //_content = content;
            //_referer = referer;
            return LoadToFile(path);
        }

        public bool LoadToFile(string path)
        {
            bool ret = false;
            FileStream fs = null;
            try
            {
                if (_trace)
                    pb.Trace.WriteLine("Http.LoadToFile()");
                Open();
                zfile.CreateFileDirectory(path);
                fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);

                DateTime dtFirstCatch = new DateTime(0);
                while (true)
                {
                    try
                    {
                        //cu.StreamWrite(gStream, fs);
                        if (_abortTransfer)
                        {
                            ret = false;
                            break;
                        }
                        //gStreamTransfer = new StreamTransfer(gProgress.ProgressControl);
                        _streamTransfer = new StreamTransfer();
                        _streamTransfer.SourceLength = _resultContentLength;
                        _streamTransfer.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(StreamTransferProgressChange);
                        ret = _streamTransfer.Transfer(_stream, fs);
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
                            if (ts.Seconds > _loadRetryTimeout) throw;
                        }
                        if (HttpRetry != null && !HttpRetry(ex)) throw;

                        Close();
                        Open();
                        FileStream fs2 = fs;
                        fs = null;
                        fs2.Close();
                        fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                }
            }
            finally
            {
                _abortTransfer = false;
                if (fs != null) fs.Close();
                Close();
            }
            return ret;
        }

        private void StreamTransferProgressChange(long current, long total)
        {
            _progress.SetProgress(current, total);
        }

        private void _LoadText()
        {
            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    CreateStreamReader();
                    StreamReader sr = _webStream;
                    _resultText = sr.ReadToEnd();
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
                        if (ts.Seconds > _loadRetryTimeout) throw;
                    }
                    if (HttpRetry != null && !HttpRetry(ex)) throw;

                    Close();
                    Open();
                }
            }
        }

        public XmlDocument GetXmlDocumentResult()
        {
            if (_trace)
                pb.Trace.WriteLine("Http.GetXmlDocumentResult()");
            XmlDocument xml = null;
            if (_resultContentType == "text/html")
            {
                HtmlToXml hx = new HtmlToXml(new StringReader(_resultText));
                hx.ReadCommentInText = _readCommentInText;
                xml = hx.GenerateXmlDocument();
            }
            else if (_resultContentType == "text/xml")
            {
                xml = new XmlDocument();
                xml.LoadXml(_resultText);
                //return xml;
            }
            else
                throw new PBException("Error can't transform \"{0}\" content to xml", _resultContentType);


            if (_traceDirectory != null)
                _xmlExportPath = GetNewHttpFileName(_traceDirectory, ".xml");
            else if (_xmlExportPath != null)
            {
                if (zPath.GetExtension(_textExportPath) == "")
                    _xmlExportPath = zpath.PathSetExtension(_textExportPath, ".xml");
            }
            if (_xmlExportPath != null)
                xml.Save(_xmlExportPath);
            return xml;
        }

        public XDocument GetXDocumentResult()
        {
            if (_trace)
                pb.Trace.WriteLine("Http.GetXDocumentResult()");
            XDocument xml = null;
            if (_resultContentType == "text/html")
            {
                HtmlToXml hx = new HtmlToXml(new StringReader(_resultText));
                hx.ReadCommentInText = _readCommentInText;
                xml = hx.GenerateXDocument();
            }
            else if (_resultContentType == "text/xml")
            {
                xml = XDocument.Parse(_resultText, LoadOptions.PreserveWhitespace);
            }
            else
                throw new PBException("Error can't transform \"{0}\" content to xml", _resultContentType);

            if (_traceDirectory != null)
                _xmlExportPath = GetNewHttpFileName(_traceDirectory, ".xml");
            else if (_xmlExportPath != null)
            {
                if (zPath.GetExtension(_textExportPath) == "")
                    _xmlExportPath = zpath.PathSetExtension(_textExportPath, ".xml");
            }
            if (_xmlExportPath != null)
                xml.Save(_xmlExportPath);
            return xml;
        }

        public void SaveResult(string path)
        {
            if (_trace)
                pb.Trace.WriteLine("Http.SaveResult()");
            if (_resultText == null)
                throw new PBException("Http error, there is no result to save (file \"{0}\")", path);
            zfile.WriteFile(path, _resultText);
        }

        public void Open()
        {
            if (_opened)
                return;
            if (_trace)
                pb.Trace.WriteLine("Http.Open() : _opened {0}", _opened);
            if (_requestParameters.useWebClient)
                OpenWebClient();
            else
                OpenWebRequest();
        }

        private void OpenWebClient()
        {
            if (_webClient != null) return;
            if (_url == null) return;
            _webClient = new WebClient();
            _webClient.Headers.Add(HttpRequestHeader.UserAgent, "pb.old");

            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    _stream = _webClient.OpenRead(_url);
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
                        if (ts.Seconds > _loadRetryTimeout) throw;
                    }
                    if (HttpRetry != null && !HttpRetry(ex)) throw;
                }
            }

            GetWebClientHeaderValues();
            //CreateStreamReader();
            _opened = true;
        }

        private void OpenWebRequest()
        {
            if (_webRequest != null)
                return;
            if (_url == null)
                return;

            //cTrace.Trace("{0} Http.OpenWebRequest() : gWebRequest = WebRequest.Create()", giOpenWebRequest++);
            _webRequest = System.Net.WebRequest.Create(_url);
            if (_webRequest is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)_webRequest;

                if (_requestParameters.userAgent != null)
                    httpRequest.UserAgent = _requestParameters.userAgent;
                if (_requestParameters.automaticDecompression != null)
                    httpRequest.AutomaticDecompression = (DecompressionMethods)_requestParameters.automaticDecompression;

                if (_requestParameters.method == HttpRequestMethod.Get)
                    httpRequest.Method = "GET";
                else
                    httpRequest.Method = "POST";
                if (_requestParameters.accept != null)
                    httpRequest.Accept = _requestParameters.accept;
                if (_requestParameters.referer != null)
                    httpRequest.Referer = _requestParameters.referer;
                httpRequest.Headers.Add(_requestParameters.headers);
                if (_requestParameters.cookies == null)
                    _requestParameters.cookies = new CookieContainer();
                httpRequest.CookieContainer = _requestParameters.cookies;
                //Trace.WriteLine("set HttpWebRequest.ServicePoint.Expect100Continue = false");
                httpRequest.ServicePoint.Expect100Continue = _requestParameters.Expect100Continue;
                if (_requestParameters.content != null)
                {
                    httpRequest.ContentType = _requestParameters.contentType;
                    Encoding encoding;
                    if (_requestParameters.encoding != null)
                        encoding = _requestParameters.encoding;
                    else
                        encoding = Encoding.Default;
                    byte[] bytes = encoding.GetBytes(_requestParameters.content);
                    httpRequest.ContentLength = bytes.LongLength;
                    Stream stream = httpRequest.GetRequestStream();
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
                    if (HttpRetry != null && !HttpRetry(ex))
                        throw;
                    //WriteLine("error in OpenWebRequest : ", ex.Message);
                }
            }

            GetWebRequestHeaderValues();
            //CreateStreamReader();
            _opened = true;
            _result = true;
        }

        private void CreateStreamReader()
        {
            //Trace.CurrentTrace.WriteLine("Http : charset \"{0}\"", _charset.zToStringOrNull());
            //Encoding encoding = zconvert.GetEncoding(_charset);
            //Trace.CurrentTrace.WriteLine("Http : charset encoding \"{0}\"", encoding.zToStringOrNull());
            //Trace.CurrentTrace.WriteLine("Http : _encoding \"{0}\"", _encoding.zToStringOrNull());
            //if (encoding == null)
            //{
            //    if (_encoding != null)
            //        encoding = _encoding;
            //    else
            //        encoding = Encoding.Default;
            //}
            //_webStreamEncoding = encoding;
            //Trace.CurrentTrace.WriteLine("Http : encoding \"{0}\"", encoding.zToStringOrNull());

            //Trace.CurrentTrace.WriteLine("Http : _encoding \"{0}\"", _encoding.zToStringOrNull());
            //Trace.CurrentTrace.WriteLine("Http : charset \"{0}\"", _charset.zToStringOrNull());
            Encoding encoding;
            if (_requestParameters.encoding != null)
                encoding = _requestParameters.encoding;
            else
            {
                encoding = zconvert.GetEncoding(_resultCharset);
                //Trace.CurrentTrace.WriteLine("Http : charset encoding \"{0}\"", encoding.zToStringOrNull());
                if (encoding == null)
                    encoding = Encoding.Default;
            }
            //Trace.CurrentTrace.WriteLine("Http : encoding \"{0}\"", encoding.zToStringOrNull());
            _webStream = new StreamReader(_stream, encoding);
        }

        private void Reset()
        {
            Close();
            ResetParameters();
        }

        public void Close()
        {
            if (!_opened) return;
            if (_trace)
                pb.Trace.WriteLine("Http.Close() : _opened {0}", _opened);
            if (_webStream != null)
            {
                _webStream.Close();
                _webStream = null;
            }
            _webStreamEncoding = null;
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
            if (_webResponse != null)
            {
                _webResponse.Close();
                _webResponse = null;
            }
            _webRequest = null;
            _opened = false;
        }

        public void ResetParameters()
        {
            if (_result)
            {
                //_method = HttpRequestMethod.Get;
                //_referer = null;
                //_headers = new NameValueCollection();
                //_requestContentType = null;
                //_content = null;
                //_requestParameters.method = HttpRequestMethod.Get;
                //_requestParameters.referer = null;
                //_requestParameters.headers = new NameValueCollection();
                //_requestParameters.contentType = null;
                //_requestParameters.content = null;
                _textExportPath = null;

                _resultText = null;
                _resultContentType = null;
                _resultCharset = null;

                _result = false;
            }
        }

        private void GetWebClientHeaderValues()
        {
            if (_url.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase))
            {
                string s = _webClient.ResponseHeaders[HttpResponseHeader.ContentType].ToLower();
                string[] s2 = zsplit.Split(s, ';', true);
                if (s2.Length > 0)
                {
                    _resultContentType = s2[0];
                }
                for (int i = 1; i < s2.Length; i++)
                {
                    string[] s3 = zsplit.Split(s2[i], '=', true);
                    if (s3.Length > 1)
                    {
                        if (s3[0] == "charset") _resultCharset = s3[1];
                    }

                }
            }
            else if (_url.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase))
            {
                Uri uri = new Uri(_url);
                string sExt = zPath.GetExtension(uri.LocalPath).ToLower();
                switch (sExt)
                {
                    case ".xml":
                        _resultContentType = "text/xml";
                        break;
                    case ".htm":
                    case ".html":
                    case ".asp":
                        _resultContentType = "text/html";
                        break;
                    case ".txt":
                        _resultContentType = "text/txt";
                        break;
                    default:
                        if (sExt.Length > 1)
                            _resultContentType = "/" + sExt.Substring(1);
                        break;
                }
            }
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

                //string contentLength = httpResponse.Headers[HttpResponseHeader.ContentLength];
                //int l = 0;
                //if (contentLength != null) l = int.Parse(contentLength);
                _resultContentLength = httpResponse.ContentLength;
            }
            else if (_webResponse is FileWebResponse)
            {
                Uri uri = new Uri(_url);
                string sExt = zPath.GetExtension(uri.LocalPath).ToLower();
                switch (sExt)
                {
                    case ".xml":
                        _resultContentType = "text/xml";
                        break;
                    case ".htm":
                    case ".html":
                    case ".asp":
                        _resultContentType = "text/html";
                        break;
                    case ".txt":
                        _resultContentType = "text/txt";
                        break;
                    default:
                        if (sExt.Length > 1)
                            _resultContentType = "/" + sExt.Substring(1);
                        break;
                }
            }
        }

        public string GetNewHttpFileName(string dir)
        {
            return GetNewUrlFileName(dir, _url);
        }

        public string GetNewHttpFileName(string dir, string ext)
        {
            return GetNewUrlFileName(dir, _url, ext);
        }

        public DataTable GetInfo()
        {
            DataTable dt = pb.Data.zdt.Create("From, Name, Value1, Value2");
            //if (wr.http == null)
            //{
            //    dt.Rows.Add("", "", "pas de page chargée");
            //    return;
            //}
            GetHttpInfo(dt);
            GetWebClientInfo(dt);
            GetWebRequestInfo(dt);
            GetWebResponseInfo(dt);
            return dt;
        }

        public void GetHttpInfo(DataTable dt)
        {
            dt.Rows.Add("Http input", "Accept", _requestParameters.accept);
            dt.Rows.Add("Http input", "AutomaticDecompression", _requestParameters.automaticDecompression.ToString());
            dt.Rows.Add("Http input", "Content", _requestParameters.content);
            string s = ""; if (_requestParameters.encoding != null) s = _requestParameters.encoding.EncodingName;
            dt.Rows.Add("Http input", "Encoding", s);
            dt.Rows.Add("Http input", "LoadXmlRetryTimeout", this.LoadRetryTimeout.ToString());
            dt.Rows.Add("Http input", "Method", _requestParameters.method);
            dt.Rows.Add("Http input", "Referer", _requestParameters.referer);
            dt.Rows.Add("Http input", "RequestContentType", _requestParameters.contentType);
            dt.Rows.Add("Http input", "Url", this.Url);
            dt.Rows.Add("Http input", "UseWebClient", _requestParameters.useWebClient.ToString());
            dt.Rows.Add("Http input", "Headers", "Count", _requestParameters.headers.Count);
            for (int i = 0; i < _requestParameters.headers.Count; i++)
                dt.Rows.Add("Http input", "Headers", _requestParameters.headers.Keys[i], _requestParameters.headers[i]);

            dt.Rows.Add("Http output", "Charset", this.Charset);
            dt.Rows.Add("Http output", "ContentType", this.ContentType);
        }

        public DataTable GetWebClientInfo()
        {
            DataTable dt = pb.Data.zdt.Create("From, Name, Value1, Value2");
            GetWebClientInfo(dt);
            return dt;
        }

        public void GetWebClientInfo(DataTable dt)
        {
            WebClient client = WebClient;
            if (client == null)
            {
                dt.Rows.Add("WebClient", "", "pas de WebClient");
                return;
            }
            dt.Rows.Add("WebClient", "BaseAddress", client.BaseAddress);
            dt.Rows.Add("WebClient", "IsBusy", client.IsBusy.ToString());
            //dt.Rows.Add("WebClient", "", client.);
            dt.Rows.Add("WebClient", "Headers", "Count", client.Headers.Count);
            for (int i = 0; i < client.Headers.Count; i++)
                dt.Rows.Add("WebClient", "Headers", client.Headers.Keys[i], client.Headers[i]);
            dt.Rows.Add("WebClient", "ResponseHeaders", "Count", client.ResponseHeaders.Count);
            for (int i = 0; i < client.ResponseHeaders.Count; i++)
                dt.Rows.Add("WebClient", "ResponseHeaders", client.ResponseHeaders.Keys[i], client.ResponseHeaders[i]);
        }

        public DataTable GetWebRequestInfo()
        {
            DataTable dt = pb.Data.zdt.Create("From, Name, Value1, Value2");
            GetWebRequestInfo(dt);
            return dt;
        }

        public void GetWebRequestInfo(DataTable dt)
        {
            System.Net.WebRequest request = Request;
            if (request == null)
            {
                dt.Rows.Add("WebRequest", "", "pas de WebRequest");
                return;
            }
            dt.Rows.Add("WebRequest", "ContentType", request.ContentType);
            dt.Rows.Add("WebRequest", "ContentLength", request.ContentLength.ToString());
            dt.Rows.Add("WebRequest", "ConnectionGroupName", request.ConnectionGroupName);
            dt.Rows.Add("WebRequest", "Method", request.Method);
            dt.Rows.Add("WebRequest", "RequestUri", request.RequestUri.AbsoluteUri);
            dt.Rows.Add("WebRequest", "Timeout", request.Timeout.ToString());
            //dt.Rows.Add("WebRequest", "", request.);
            //request.Headers;
            if (request is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)request;
                dt.Rows.Add("HttpWebRequest", "Accept", httpRequest.Accept);
                dt.Rows.Add("HttpWebRequest", "Address", httpRequest.Address.AbsoluteUri);
                dt.Rows.Add("HttpWebRequest", "AllowAutoRedirect", httpRequest.AllowAutoRedirect.ToString());
                dt.Rows.Add("HttpWebRequest", "AllowWriteStreamBuffering", httpRequest.AllowWriteStreamBuffering.ToString());
                dt.Rows.Add("HttpWebRequest", "AutomaticDecompression", httpRequest.AutomaticDecompression.ToString());
                dt.Rows.Add("HttpWebRequest", "Connection", httpRequest.Connection);
                dt.Rows.Add("HttpWebRequest", "ConnectionGroupName", httpRequest.ConnectionGroupName);
                dt.Rows.Add("HttpWebRequest", "ContentLength", httpRequest.ContentLength.ToString());
                dt.Rows.Add("HttpWebRequest", "ContentType", httpRequest.ContentType);
                dt.Rows.Add("HttpWebRequest", "Expect", httpRequest.Expect);
                dt.Rows.Add("HttpWebRequest", "HaveResponse", httpRequest.HaveResponse.ToString());
                dt.Rows.Add("HttpWebRequest", "IfModifiedSince", httpRequest.IfModifiedSince.ToString());
                dt.Rows.Add("HttpWebRequest", "KeepAlive", httpRequest.KeepAlive.ToString());
                dt.Rows.Add("HttpWebRequest", "MaximumAutomaticRedirections", httpRequest.MaximumAutomaticRedirections.ToString());
                dt.Rows.Add("HttpWebRequest", "MaximumResponseHeadersLength", httpRequest.MaximumResponseHeadersLength.ToString());
                dt.Rows.Add("HttpWebRequest", "MediaType", httpRequest.MediaType);
                dt.Rows.Add("HttpWebRequest", "Method", httpRequest.Method);
                dt.Rows.Add("HttpWebRequest", "Pipelined", httpRequest.Pipelined.ToString());
                dt.Rows.Add("HttpWebRequest", "PreAuthenticate", httpRequest.PreAuthenticate.ToString());
                dt.Rows.Add("HttpWebRequest", "ProtocolVersion", httpRequest.ProtocolVersion.ToString(2));
                dt.Rows.Add("HttpWebRequest", "ReadWriteTimeout", httpRequest.ReadWriteTimeout.ToString());
                dt.Rows.Add("HttpWebRequest", "Referer", httpRequest.Referer);
                dt.Rows.Add("HttpWebRequest", "RequestUri", httpRequest.RequestUri.AbsoluteUri);
                dt.Rows.Add("HttpWebRequest", "SendChunked", httpRequest.SendChunked.ToString());
                dt.Rows.Add("HttpWebRequest", "Timeout", httpRequest.Timeout.ToString());
                dt.Rows.Add("HttpWebRequest", "TransferEncoding", httpRequest.TransferEncoding);
                dt.Rows.Add("HttpWebRequest", "UnsafeAuthenticatedConnectionSharing", httpRequest.UnsafeAuthenticatedConnectionSharing.ToString());
                dt.Rows.Add("HttpWebRequest", "UseDefaultCredentials", httpRequest.UseDefaultCredentials.ToString());
                dt.Rows.Add("HttpWebRequest", "UserAgent", httpRequest.UserAgent);
                //dt.Rows.Add("HttpWebRequest", "", httpRequest.);
                WebHeaderCollection headers = httpRequest.Headers;
                dt.Rows.Add("HttpWebRequest", "Headers", "Count", headers.Count);
                for (int i = 0; i < headers.Count; i++)
                {
                    dt.Rows.Add("HttpWebRequest", "Headers", headers.GetKey(i), headers.Get(i));
                }
                //httpRequest.ClientCertificates;
                //httpRequest.ServicePoint
            }
        }

        public DataTable GetWebResponseInfo()
        {
            DataTable dt = pb.Data.zdt.Create("From, Name, Value1, Value2");
            GetWebResponseInfo(dt);
            return dt;
        }

        public void GetWebResponseInfo(DataTable dt)
        {
            WebResponse response = Response;
            if (response == null)
            {
                dt.Rows.Add("WebResponse", "", "pas de WebResponse");
                return;
            }
            dt.Rows.Add("WebResponse", "ContentLength", response.ContentLength.ToString());
            dt.Rows.Add("WebResponse", "ContentType", response.ContentType);
            dt.Rows.Add("WebResponse", "IsFromCache", response.IsFromCache.ToString());
            dt.Rows.Add("WebResponse", "IsMutuallyAuthenticated", response.IsMutuallyAuthenticated.ToString());
            dt.Rows.Add("WebResponse", "ResponseUri", response.ResponseUri.AbsoluteUri);
            //dt.Rows.Add("WebResponse", "", response.);
            // response.Headers
            if (response is HttpWebResponse)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                dt.Rows.Add("HttpWebResponse", "CharacterSet", httpResponse.CharacterSet);
                dt.Rows.Add("HttpWebResponse", "ContentEncoding", httpResponse.ContentEncoding);
                dt.Rows.Add("HttpWebResponse", "ContentLength", httpResponse.ContentLength.ToString());
                dt.Rows.Add("HttpWebResponse", "ContentType", httpResponse.ContentType);
                dt.Rows.Add("HttpWebResponse", "IsFromCache", httpResponse.IsFromCache.ToString());
                dt.Rows.Add("HttpWebResponse", "IsMutuallyAuthenticated", httpResponse.IsMutuallyAuthenticated.ToString());
                dt.Rows.Add("HttpWebResponse", "LastModified", httpResponse.LastModified.ToString());
                dt.Rows.Add("HttpWebResponse", "Method", httpResponse.Method);
                dt.Rows.Add("HttpWebResponse", "ProtocolVersion", httpResponse.ProtocolVersion.ToString(2));
                dt.Rows.Add("HttpWebResponse", "ResponseUri", httpResponse.ResponseUri.AbsoluteUri);
                dt.Rows.Add("HttpWebResponse", "Server", httpResponse.Server);
                dt.Rows.Add("HttpWebResponse", "StatusCode", httpResponse.StatusCode.ToString());
                dt.Rows.Add("HttpWebResponse", "StatusDescription", httpResponse.StatusDescription);
                //dt.Rows.Add("HttpWebResponse", "", httpResponse.);
                WebHeaderCollection headers = httpResponse.Headers;
                dt.Rows.Add("HttpWebResponse", "Headers", "Count", headers.Count);
                for (int i = 0; i < headers.Count; i++)
                {
                    dt.Rows.Add("HttpWebResponse", "Headers", headers.GetKey(i), headers.Get(i));
                }
            }
        }

        public static string GetNewUrlFileName(string dir, string url)
        {
            return GetNewUrlFileName(dir, url, null);
        }

        public static string GetNewUrlFileName(string dir, string url, string ext)
        {
            string sFile = UrlToFileName(url, ext);
            //return zfile.GetNewIndexedFileName(zPath.Combine(dir, "{0:0000}")) + "_" + sFile;
            return zfile.GetNewIndexedFileName(dir) + "_" + sFile;
        }

        public static string UrlToFileName(string sUrl)
        {
            return UrlToFileName(sUrl, null);
        }

        public static string UrlToFileName(string sUrl, string sExt)
        {
            return UrlToFileName(sUrl, sExt, UrlFileNameType.FileName);
        }

        public static string UrlToFileName(string sUrl, string sExt, UrlFileNameType type)
        {
            string sFile;
            Uri uri = new Uri(sUrl);
            if (type == UrlFileNameType.FileName)
            {
                sFile = uri.LocalPath;
                while (sFile.StartsWith("/")) sFile = sFile.Remove(0, 1);
                sFile = sFile.Replace("/", "_");
                if (sFile != "")
                    sFile = uri.Host + "_" + sFile;
                else
                    sFile = uri.Host;
                if (sExt != null) sFile = sFile + sExt;
            }
            else
            {
                //sFile = uri.AbsoluteUri;
                //sFile = sUrl;
                if (type == UrlFileNameType.Path)
                    sFile = uri.AbsolutePath;
                //else if (type == UrlFileNameType.PathAndQuery)
                //    sFile = uri.PathAndQuery;
                else if (type == UrlFileNameType.Query)
                    sFile = uri.Query;
                else
                    return null;
                int i = sFile.IndexOf("://");
                if (i != -1) sFile = zstr.right(sFile, sFile.Length - i - 3);
                i = sFile.IndexOf('?');
                if (i != -1) sFile = zstr.left(sFile, i);
                sFile = sFile.Replace('/', '_');
                //sFile = sFile.Replace('?', '_');
                sFile = sFile.Replace('%', '_');
                //sFile = cu.PathGetFile(sFile) + ".html";
                if (sExt != null) sFile = sFile + sExt;
            }
            sFile = zPath.GetFileName(sFile);
            return sFile;
        }

        //public static string LoadText(string url)
        //{
        //    return LoadText(url, HttpRequestMethod.Get, null, null);
        //}

        //public static string LoadText(string url, HttpRequestMethod method, string content)
        //{
        //    return LoadText(url, method, content, null);
        //}

        //public static string LoadText(string url, HttpRequestMethod method, string content, string referer)
        //{
        //    Http http = new Http();
        //    http.Load(url, method, content, referer);
        //    return http.TextResult;
        //}

        //public static XmlDocument LoadToXmlDocument(string url)
        //{
        //    return LoadToXmlDocument(url, null, false);
        //}

        //public static XmlDocument LoadToXmlDocument(string url, CookieContainer cookies, bool UseWebClient)
        //{
        //    Http http = new Http(url, cookies);
        //    http.UseWebClient = UseWebClient;
        //    try
        //    {
        //        http.Load();
        //        return http.GetXmlDocumentResult();
        //    }
        //    finally
        //    {
        //        http.Dispose();
        //    }
        //}

        //public static XDocument LoadToXDocument(string url)
        //{
        //    return LoadToXDocument(url, null, false);
        //}

        //public static XDocument LoadToXDocument(string url, CookieContainer cookies, bool UseWebClient)
        //{
        //    Http http = new Http(url, cookies);
        //    http.UseWebClient = UseWebClient;
        //    try
        //    {
        //        http.Load();
        //        return http.GetXDocumentResult();
        //    }
        //    finally
        //    {
        //        http.Dispose();
        //    }
        //}

        //public static void DownLoad(string path, string url)
        //{
        //    DownLoad(path, url, HttpRequestMethod.Get, null, null);
        //}

        //public static void DownLoad(string path, string url, HttpRequestMethod method, string content)
        //{
        //    DownLoad(path, url, method, content, null);
        //}

        //public static void DownLoad(string path, string url, HttpRequestMethod method, string content, string referer)
        //{
        //    Http http = new Http();
        //    http.LoadToFile(path, url, method, content, referer);
        //}

        public static string GetContentFileExtension(string sContentType)
        {
            sContentType = sContentType.ToLower();
            //if (sContentType == "text/html")
            if (sContentType.EndsWith("/html"))
                return ".html";
            //else if (sContentType == "text/xml" || sContentType == "application/xml")
            else if (sContentType.EndsWith("/xml"))
                return ".xml";
            else
                return ".txt";
        }

        public static string UrlLocalPath(string sUrl)
        {
            Uri uri = new Uri(sUrl);
            return uri.LocalPath;
        }

        //public static string GetUrl(string sBaseUrl, string sUrl)
        //{
        //    if (sUrl == null) return null;
        //    Uri uri;
        //    if (sBaseUrl != null)
        //    {
        //        Uri baseUri = new Uri(sBaseUrl);
        //        uri = new Uri(baseUri, sUrl);
        //    }
        //    else
        //        uri = new Uri(sUrl);
        //    return uri.AbsoluteUri;
        //}

        /// <summary>
        /// translate html code like "&gt;" or &#62; or &#xB7;
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static string TranslateCode(string sValue)
        {
            if (sValue == null) return null;
            // &gt;  &aaa;
            int i = 0;
            while (true)
            {
                Match match = _translate1.Match(sValue, i);
                if (!match.Success) break;
                //string sName = match.Value.Substring(1, match.Value.Length - 2);
                string sName = match.Groups[1].Value;
                //char c = HtmlCharList.GetHtmlChar(sName).c;
                HtmlCharCode htmlChar = HtmlCharCodes.GetHtmlChar(sName);
                if (htmlChar != null)
                {
                    char c = htmlChar.Char;
                    sValue = sValue.Substring(0, match.Index) + c.ToString() + sValue.Substring(match.Index + match.Length, sValue.Length - match.Index - match.Length);
                }
                i = match.Index + 1;
            }

            // &#62; &#nnn;
            i = 0;
            while (true)
            {
                Match match = _translate2.Match(sValue, i);
                if (!match.Success) break;
                //string sCode = match.Value.Substring(2, match.Value.Length - 3);
                string sCode = match.Groups[1].Value;
                int iCode = int.Parse(sCode);
                char c = (char)iCode;
                sValue = sValue.Substring(0, match.Index) + c.ToString() + sValue.Substring(match.Index + match.Length, sValue.Length - match.Index - match.Length);
                i = match.Index + 1;
            }

            // &#xB7; &#xnn;
            i = 0;
            while (true)
            {
                Match match = _translate3.Match(sValue, i);
                if (!match.Success) break;
                //string sCode = match.Value.Substring(3, match.Value.Length - 4);
                string sCode = match.Groups[1].Value;
                int iCode = int.Parse(sCode, System.Globalization.NumberStyles.AllowHexSpecifier);
                char c = (char)iCode;
                sValue = sValue.Substring(0, match.Index) + c.ToString() + sValue.Substring(match.Index + match.Length, sValue.Length - match.Index - match.Length);
                i = match.Index + 1;
            }

            return sValue;
        }

        //public static HttpRequestMethod GetHttpRequestMethod(string method)
        //{
        //    switch (method.ToLower())
        //    {
        //        case "get":
        //            return HttpRequestMethod.Get;
        //        case "post":
        //            return HttpRequestMethod.Post;
        //        default:
        //            throw new PBException("Error unknow HttpRequestMethod \"{0}\"", method);
        //    }
        //}
    }

    public enum HtmlFormControlType
    {
        Unknown = 0,
        SubmitButton,
        ResetButton,
        Button,
        Checkbox,
        Radio,
        ImageButton,
        File,
        Text,
        Password,
        Hidden,
        TextArea,
        Select
    }

    public class HtmlControlOption
    {
        public string Value;
        public string Text;
        public bool Selected = false;
    }

    public class HtmlFormControl
    {
        public string ControlName;
        public HtmlFormControlType Type;
        public string TypeName;
        public string Name;
        public string Value;
        public bool Checked = false;
        public string ImageSource;
        //public Align;
        public int Size;
        public int NbRow;
        public int NbCol;
        public List<HtmlControlOption> ControlOption;
        //public string OptionSelected;

    }

    public class HtmlForm
    {
        public string Code;
        //public string Method;
        public HttpRequestMethod Method;
        public string Action;
        public string Path;
        //public List<HtmlFormControl> Controls;
        public SortedList<string, HtmlFormControl> Controls;

        #region AddControl(XmlNode xmlControl, string sName, string sControlName, string sType, string sValue, bool bChecked)
        public void AddControl(XmlNode xmlControl, string sName, string sControlName, string sType, string sValue, bool bChecked)
        {
            HtmlFormControlType type = GetInputType(sControlName, sType);
            if (sName != null) sName = sName.Replace(" ", "+"); else sName = "";
            HtmlFormControl control = GetControl(type, sName);
            //HtmlFormControl control = new HtmlFormControl();
            control.ControlName = sControlName;
            //control.Type = type;
            control.TypeName = sType;
            //control.Name = sName;
            if (sValue != null) sValue = sValue.Replace(" ", "+"); else sValue = "";
            //control.Checked = bChecked;
            if (type == HtmlFormControlType.Radio)
            {
                if (bChecked) control.Value = sValue;
                if (control.ControlOption == null) control.ControlOption = new List<HtmlControlOption>();
                HtmlControlOption option = new HtmlControlOption();
                option.Value = sValue;
                option.Selected = bChecked;
                // Xml.GetValue(xmlControl, "text()")
                option.Text = xmlControl.zGetValue("text()");
                control.ControlOption.Add(option);
            }
            else if (control.Type == HtmlFormControlType.Checkbox)
            {
                control.Value = sValue;
                control.Checked = bChecked;
            }
            else if (type == HtmlFormControlType.Select)
                GetSelectOptions(control, xmlControl);
            else
                control.Value = sValue;

            //form.Controls.Add(sControlName, control);
        }
        #endregion

        #region AddControl(XElement xmlControl, string sName, string sControlName, string sType, string sValue, bool bChecked)
        public void AddControl(XElement xmlControl, string sName, string sControlName, string sType, string sValue, bool bChecked)
        {
            HtmlFormControlType type = GetInputType(sControlName, sType);
            if (sName != null) sName = sName.Replace(" ", "+"); else sName = "";
            HtmlFormControl control = GetControl(type, sName);
            control.ControlName = sControlName;
            control.TypeName = sType;
            if (sValue != null) sValue = sValue.Replace(" ", "+"); else sValue = "";
            if (type == HtmlFormControlType.Radio)
            {
                if (bChecked) control.Value = sValue;
                if (control.ControlOption == null) control.ControlOption = new List<HtmlControlOption>();
                HtmlControlOption option = new HtmlControlOption();
                option.Value = sValue;
                option.Selected = bChecked;
                //option.Text = Xml.GetValue(xmlControl, "text()");
                //option.Text = xmlControl.zFirstValue();
                option.Text = xmlControl.zDescendantTexts().FirstOrDefault();
                control.ControlOption.Add(option);
            }
            else if (control.Type == HtmlFormControlType.Checkbox)
            {
                control.Value = sValue;
                control.Checked = bChecked;
            }
            else if (type == HtmlFormControlType.Select)
                //GetSelectOptions(control, xmlControl);
                GetSelectOptions(control, xmlControl);
            else
                control.Value = sValue;
        }
        #endregion

        #region GetControl
        private HtmlFormControl GetControl(HtmlFormControlType type, string sName)
        {
            HtmlFormControl control;
            int i = 1;
            string sName2 = sName;
            if (sName2 == null || sName2 == "")
            {
                sName2 = "control";
                i = 0;
            }
            else
            {
                int iControl = Controls.IndexOfKey(sName);
                if (iControl == -1)
                {
                    control = new HtmlFormControl();
                    control.Name = sName;
                    control.Type = type;
                    Controls.Add(sName, control);
                    return control;
                }
                if (type == HtmlFormControlType.Radio)
                    return Controls.Values[iControl];
            }

            string sControlName2;
            do
            {
                sControlName2 = sName2 + "_" + (++i).ToString();
            }
            while (Controls.ContainsKey(sControlName2));

            control = new HtmlFormControl();
            control.Name = sName;
            control.Type = type;
            Controls.Add(sControlName2, control);
            return control;
        }
        #endregion

        #region GetInputType
        private static HtmlFormControlType GetInputType(string sControlName, string sInputType)
        {
            switch (sControlName.ToLower())
            {
                case "button":
                    return HtmlFormControlType.Button;
                case "select":
                    return HtmlFormControlType.Select;
                case "textarea":
                    return HtmlFormControlType.TextArea;
                case "input":
                    if (sInputType == null) return HtmlFormControlType.Text;
                    switch (sInputType.ToLower())
                    {
                        case "text":
                            return HtmlFormControlType.Text;
                        case "password":
                            return HtmlFormControlType.Password;
                        case "checkbox":
                            return HtmlFormControlType.Checkbox;
                        case "radio":
                            return HtmlFormControlType.Radio;
                        case "submit":
                            return HtmlFormControlType.SubmitButton;
                        case "reset":
                            return HtmlFormControlType.ResetButton;
                        case "file":
                            return HtmlFormControlType.File;
                        case "hidden":
                            return HtmlFormControlType.Hidden;
                        case "image":
                            return HtmlFormControlType.ImageButton;
                        case "button":
                            return HtmlFormControlType.Button;
                    }
                    break;
                //default:
                //    return HtmlInputType.Unknown;
            }
            return HtmlFormControlType.Unknown;
            //throw new PBException("HtmlXml.GetInputType unknow type : {0}", sInputType);
        }
        #endregion

        #region GetSelectOptions(HtmlFormControl control, XmlNode xmlControl)
        private static void GetSelectOptions(HtmlFormControl control, XmlNode xmlControl)
        {
            control.ControlOption = new List<HtmlControlOption>();
            XmlNodeList nodes = xmlControl.SelectNodes("./option");
            foreach (XmlNode node in nodes)
            {
                HtmlControlOption option = new HtmlControlOption();
                // Xml.GetValue(node, "@value")
                option.Value = node.zGetValue("@value");
                // Xml.GetValue(node, "text()")
                option.Text = node.zGetValue("text()");
                // Xml.GetValue(node, "@selected")
                string s = node.zGetValue("@selected");
                if (s != null)
                {
                    option.Selected = true;
                    control.Value = option.Value;
                }
                control.ControlOption.Add(option);
            }
        }
        #endregion

        #region GetSelectOptions(HtmlFormControl control, XElement xmlControl)
        private static void GetSelectOptions(HtmlFormControl control, XElement xmlControl)
        {
            control.ControlOption = new List<HtmlControlOption>();
            //XmlNodeList nodes = xmlControl.SelectNodes("./option");
            var nodes = from n in xmlControl.Elements("option") select n;
            //foreach (XmlNode node in nodes)
            foreach (XElement node in nodes)
            {
                HtmlControlOption option = new HtmlControlOption();
                //option.Value = Xml.GetValue(node, "@value");
                option.Value = node.zAttribValue("value");
                //option.Text = Xml.GetValue(node, "text()");
                //option.Text = node.zFirstValue();
                option.Text = node.zDescendantTexts().FirstOrDefault();
                //string s = Xml.GetValue(node, "@selected");
                string s = node.zAttribValue("selected");
                if (s != null)
                {
                    option.Selected = true;
                    control.Value = option.Value;
                }
                control.ControlOption.Add(option);
            }
        }
        #endregion

        #region GetValues
        public string[] GetValues(string sName)
        {
            int i = Controls.IndexOfKey(sName);
            if (i == -1) return new string[0];
            HtmlFormControl control = Controls.Values[i];
            List<HtmlControlOption> option = control.ControlOption;
            if (option == null)
                return new string[] { control .Value };
            string[] sValues = new string[option.Count];
            for (i = 0; i < option.Count; i++)
                sValues[i] = option[i].Value;
            return sValues;
        }
        #endregion

        #region SetValue
        public void SetValue(string sName, string sValue)
        {
            if (!TrySetValue(sName, sValue))
                throw new PBException("input \"{0}\" not found in form, value\"{1}\" (HtmlXml.Form.SetValue())", sName, sValue);
        }
        #endregion

        #region TrySetValue
        public bool TrySetValue(string sName, string sValue)
        {
            //bool bFound = false;
            int i = Controls.IndexOfKey(sName);
            if (i == -1) return false;
            HtmlFormControl control = Controls.Values[i];
            control.Value = sValue.Replace(" ", "+");
            return true;
            //foreach (HtmlFormControl control in Controls)
            //{
            //    if (control.Name != sName) continue;
            //    if (control.Type == HtmlFormControlType.Radio)
            //    {
            //        if (control.Value == sValue)
            //        {
            //            control.Checked = true;
            //            bFound = true;
            //        }
            //        else
            //            control.Checked = false;
            //    }
            //    else
            //    {
            //        control.Value = sValue.Replace(" ", "+");
            //        bFound = true;
            //        break;
            //    }
            //}
            //return bFound;
        }
        #endregion

        #region GetRequest
        public string GetRequest()
        {
            return Action + "?" + GetRequestParameters();
        }
        #endregion

        #region GetRequestParameters
        public string GetRequestParameters()
        {
            string sParam = "";
            foreach (HtmlFormControl control in Controls.Values)
            {
                if (control.Type == HtmlFormControlType.ResetButton) continue;
                //if (control.Type == HtmlFormControlType.Radio && !control.Checked) continue;
                if (control.Type == HtmlFormControlType.Checkbox && !control.Checked) continue;
                if (control.Name != null && control.Name != "")
                {
                    if (sParam != "") sParam += "&";
                    sParam += control.Name + "=" + control.Value;
                }
            }
            return sParam;
        }
        #endregion

        #region GetFormMethod
        public static string GetFormMethod(XmlNode form)
        {
            // Xml.GetValue(form, "@method")
            return form.zGetValue("@method");
        }
        #endregion

        #region GetFormRequest
        public static string GetFormRequest(XmlNode xmlForm)
        {
            //string sAction = cXml.SelectValue(form, "@action");
            //XmlNodeList inputs = form.SelectNodes(".//input");
            //string sParam = "";
            //foreach (XmlNode input in inputs)
            //{
            //    if (sParam != "") sParam += "&";
            //    string sName = cXml.SelectValue(input, "@name");
            //    if (sName != null) sName = sName.Replace(" ", "+"); else sName = "";
            //    string sValue = cXml.SelectValue(input, "@value");
            //    if (sValue != null) sValue = sValue.Replace(" ", "+"); else sValue = "";
            //    sParam += sName + "=" + sValue;
            //}
            //return sAction + "?" + sParam;
            HtmlForm form = GetForm(xmlForm);
            return form.GetRequest();
        }
        #endregion

        #region GetForm(XmlNode formNode)
        public static HtmlForm GetForm(XmlNode formNode)
        {
            return GetForm(formNode, null);
        }
        #endregion

        #region GetForm(XmlNode formNode, HtmlXmlTables tables)
        public static HtmlForm GetForm(XmlNode formNode, HtmlXmlTables tables)
        {
            if (formNode == null) return null;
            HtmlForm form = new HtmlForm();
            //form.Method = Xml.GetValue(formNode, "@method");
            // Xml.GetValue(formNode, "@method")
            form.Method = Http.GetHttpRequestMethod(formNode.zGetValue("@method"));
            // Xml.GetValue(formNode, "@action")
            form.Action = formNode.zGetValue("@action");
            // Xml.GetNodePath(formNode)
            form.Path = XmlSelect.XPathToTranslatedXPath(formNode.zGetPath(), tables);
            //form.Controls = new List<HtmlFormControl>();
            form.Controls = new SortedList<string, HtmlFormControl>();
            //XmlNodeList nodes = xmlForm.SelectNodes(".//input");
            XmlNodeList nodes = formNode.SelectNodes(".//input | .//button | .//select | .//textarea");
            foreach (XmlNode xmlControl in nodes)
            {
                string sControlName = xmlControl.Name;
                // Xml.GetValue(xmlControl, "@type")
                string sType = xmlControl.zGetValue("@type");
                // Xml.GetValue(xmlControl, "@name")
                string sName = xmlControl.zGetValue("@name");
                // Xml.GetValue(xmlControl, "@value")
                string sValue = xmlControl.zGetValue("@value");
                bool bChecked = false;
                // Xml.GetValue(xmlControl, "@checked")
                if (xmlControl.zGetValue("@checked") != null)
                    bChecked = true;

                form.AddControl(xmlControl, sName, sControlName, sType, sValue, bChecked);

                //HtmlFormControlType type = GetInputType(sControlName, sType);
                //string sControlName = GetControlName(form.Controls, xmlControl.Name);
                //if (sName != null) sName = sName.Replace(" ", "+"); else sName = "";
                //HtmlFormControl control = new HtmlFormControl();
                //control.ControlName = sControlName;
                //control.Type = type;
                //control.TypeName = sType;
                //control.Name = sName;
                //if (sValue != null) sValue = sValue.Replace(" ", "+"); else sValue = "";
                //control.Value = sValue;
                //if (Xml.GetValue(xmlControl, "@checked") != null) control.Checked = true;
                //if (type == HtmlFormControlType.Select) GetSelectOptions(control, xmlControl);
                //form.Controls.Add(sControlName, control);
            }
            return form;
        }
        #endregion

        #region GetForm(XElement formNode)
        public static HtmlForm GetForm(XElement formNode)
        {
            if (formNode == null) return null;
            HtmlForm form = new HtmlForm();
            //form.Method = Xml.GetValue(formNode, "@method");
            //form.Method = formNode.zAttribValue("method");
            form.Method = Http.GetHttpRequestMethod(formNode.zAttribValue("method"));
            //form.Action = Xml.GetValue(formNode, "@action");
            form.Action = formNode.zAttribValue("action");
            //form.Path = XmlSelect.XPathToTranslatedXPath(Xml.GetNodePath(formNode), tables);
            form.Controls = new SortedList<string, HtmlFormControl>();
            //XmlNodeList nodes = formNode.SelectNodes(".//input | .//button | .//select | .//textarea");
            var nodes = from n in formNode.Descendants()
                        where n.Name.LocalName.Equals("input", StringComparison.InvariantCultureIgnoreCase)
                        || n.Name.LocalName.Equals("button", StringComparison.InvariantCultureIgnoreCase)
                        || n.Name.LocalName.Equals("select", StringComparison.InvariantCultureIgnoreCase)
                        || n.Name.LocalName.Equals("textarea", StringComparison.InvariantCultureIgnoreCase)
                        select n;
            //foreach (XmlNode xmlControl in nodes)
            foreach (XElement xmlControl in nodes)
            {
                string sControlName = xmlControl.Name.LocalName;
                //string sType = Xml.GetValue(xmlControl, "@type");
                string sType = xmlControl.zAttribValue("type");
                //string sName = Xml.GetValue(xmlControl, "@name");
                string sName = xmlControl.zAttribValue("name");
                //string sValue = Xml.GetValue(xmlControl, "@value");
                string sValue = xmlControl.zAttribValue("value");
                //bool bChecked = false; if (Xml.GetValue(xmlControl, "@checked") != null) bChecked = true;
                bool bChecked = false; if (xmlControl.zAttribValue("checked") != null) bChecked = true;

                form.AddControl(xmlControl, sName, sControlName, sType, sValue, bChecked);
            }
            return form;
        }
        #endregion
    }

    public class HtmlXmlTableException : Exception
    {
        public HtmlXmlTableException(string sMessage) : base(sMessage) { }
        public HtmlXmlTableException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public HtmlXmlTableException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public HtmlXmlTableException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }

    public class HtmlXmlTable
    {
        #region variable
        public XmlNode Node = null;
        public string Id = null;
        public string Code = null;
        public string Code2 = null;
        public string AbsoluteTablePath = null;
        public string TablePath = null;
        public HtmlXmlTable ParentTable = null;
        public string ParentTablePath = null;
        public int ParentTableRow = -1;                            // Position de la table dans la table parent : no de ligne
        public int ParentTableColumn = -1;                         // Position de la table dans la table parent : no de colonne
        // Liste des tables par ligne, liste des tables par colonne, liste des tables par cellule
        public SortedList<int, SortedList<int, List<HtmlXmlTable>>> RowChilds = new SortedList<int, SortedList<int, List<HtmlXmlTable>>>();
        // Liste des tables par colonne, liste des tables par ligne, liste des tables par cellule
        public SortedList<int, SortedList<int, List<HtmlXmlTable>>> ColChilds = new SortedList<int, SortedList<int, List<HtmlXmlTable>>>();
        public int Level = 1;                                      // Niveau d'imbrication de la table : 1 = root table
        public int NbRow = 1;
        public int ColMaxNbRow = 0;
        public int RowNumber = -1;
        public int NbColumn = 1;                                   // nb colonne des enfants de la table
        public int ColumnNumber = -1;
        public HtmlXmlTable PreviousRootTable = null;              // si table root, table root précédente
        public HtmlXmlTable NextRootTable = null;                  // si table root, table root suivante
        public HtmlXmlTable FirstChildByRow = null;
        public HtmlXmlTable FirstChildByCol = null;
        // par ligne
        public HtmlXmlTable ParentByRowFirstRow = null;            // table de la 1ère ligne de la table parent
        public HtmlXmlTable ParentByRowPreviousRow = null;         // table de la ligne précédente de la table parent
        public HtmlXmlTable ParentByRowNextRow = null;             // table de la ligne suivante de la table parent
        public HtmlXmlTable ParentByRowFirstColumn = null;         // table de la 1ère colonne de la table parent
        public HtmlXmlTable ParentByRowPreviousColumn = null;      // table de la colonne précédente de la table parent
        public HtmlXmlTable ParentByRowNextColumn = null;          // table de la colonne suivante de la table parent
        // par colonne
        public HtmlXmlTable ParentByColFirstColumn = null;         // table de la 1ère colonne de la table parent
        public HtmlXmlTable ParentByColPreviousColumn = null;      // table de la colonne précédente de la table parent
        public HtmlXmlTable ParentByColNextColumn = null;          // table de la colonne suivante de la table parent
        public HtmlXmlTable ParentByColFirstRow = null;            // table de la 1ère ligne de la table parent
        public HtmlXmlTable ParentByColPreviousRow = null;         // table de la ligne précédente de la table parent
        public HtmlXmlTable ParentByColNextRow = null;             // table de la ligne suivante de la table parent
        // par cellule
        public HtmlXmlTable ParentFirstCell = null;                // 1ère table de la cellule de la table parent
        public HtmlXmlTable ParentPreviousCell = null;             // table précédente dans la cellule de la table parent
        public HtmlXmlTable ParentNextCell = null;                 // table suivante dans la cellule de la table parent
        #endregion

        #region doc
        //
        //
        //
        //     Liste des fils de t1 par ligne
        //         ParentByRowFirstRow t02, ParentByRowPreviousRow - ParentByRowNextRow t02, t06, t09, t13
        //         ParentByRowFirstColumn, ParentByRowPreviousColumn, ParentByRowNextColumn
        //     t1
        //     
        //         t02------t03------t04------t05
        //            \                            
        //             t06------t07------t08
        //            /
        //         t09------t10------t11------t12
        //            \
        //             t13------t14------t15
        //
        //
        //     Liste des fils de t1 par colonne
        //         ParentByColumnFirstColumn t02, ParentByColumnPreviousColumn, ParentByColumnNextColumn t02, t06, t03, t07, t04, t08, t05
        //         ParentByColumnFirstRow, ParentByColumnPreviousRow, ParentByColumnNextRow
        //     t1
        //     
        //         t02      t03      t04      t05
        //          | \    / | \    / | \    / |
        //          |  t06   |  t07   |  t08   |
        //          |   |    |   |    |   |    |
        //         t09  |   t10  |   t11  |   t12
        //              |        |        |
        //             t13      t14      t15
        //
        //
        //
        //     Liste des fils de t1 par ligne
        //         ParentByRowFirstRow t02, ParentByRowPreviousRow - ParentByRowNextRow t02, t05, t09, t12
        //         ParentByRowFirstColumn, ParentByRowPreviousColumn, ParentByRowNextColumn
        //     t1
        //     
        //             t02------t03------t04
        //            /                            
        //         t05------t06------t07------t08
        //            \
        //             t09------t10------t11
        //            /
        //         t12------t13------t14------t15
        //
        //
        //     Liste des fils de t1 par colonne
        //         ParentByColumnFirstColumn t02, ParentByColumnPreviousColumn, ParentByColumnNextColumn t02, t06, t03, t07, t04, t08, t05
        //         ParentByColumnFirstRow, ParentByColumnPreviousRow, ParentByColumnNextRow
        //     t1
        //     
        //             t02     t03     t04
        //            / | \   / | \   / | \
        //         t05  |  t06  |  t07  |  t08
        //          |   |   |   |   |   |   |
        //          |  t09  |  t10  |  t11  |
        //          |       |       |       |
        //         t12     t13     t14     t15
        //
        //
        //
        #endregion

        #region constructor
        #region HtmlXmlTable(string sAbsoluteTablePath)
        public HtmlXmlTable(string sAbsoluteTablePath)
        {
            AbsoluteTablePath = sAbsoluteTablePath;
            Create();
        }
        #endregion

        #region HtmlXmlTable(XmlNode node, string sAbsoluteTablePath)
        public HtmlXmlTable(XmlNode node, string sAbsoluteTablePath)
        {
            Node = node;
            AbsoluteTablePath = sAbsoluteTablePath;
            Create();
        }
        #endregion
        #endregion

        #region Create
        private void Create()
        {
            if (AbsoluteTablePath == null) throw new HtmlXmlTableException("error AbsoluteTablePath is null");
            string sPath = AbsoluteTablePath.ToLower();
            // /xml[1]/html[1]/body[1]/table[4]/tbody[1]/tr[1]/td[1]/div[2]/div[2]/div[1]/table[1]
            int i = sPath.Length - 1;
            if (sPath[i--] != ']') goto err;
            for (; i >= 0; i--) if (!char.IsDigit(sPath[i])) break;
            if (sPath[i--] != '[') goto err;
            if (i < 4 || sPath.Substring(i - 4, 5) != "table") goto err;
            i -= 5;
            if (sPath[i--] != '/') goto err;
            int i2 = sPath.Substring(0, i + 1).LastIndexOf("table");
            if (i2 == -1)
            {
                TablePath = AbsoluteTablePath;
                return;
            }
            i2 += 5;
            if (sPath[i2++] != '[') goto err;
            for (; i2 < sPath.Length; i2++) if (!char.IsDigit(sPath[i2])) break;
            if (sPath[i2++] != ']') goto err;
            ParentTablePath = AbsoluteTablePath.Substring(0, i2);
            ParentTableRow = 0;
            ParentTableColumn = 0;
            if (sPath[i2++] != '/') goto err;
            TablePath = AbsoluteTablePath.Substring(i2, AbsoluteTablePath.Length - i2);

            int i3 = sPath.IndexOf("tr", i2);
            int i4;
            if (i3 != -1)
            {
                i3 += 2;
                if (sPath[i3++] != '[') goto err;
                for (i4 = i3; i4 < sPath.Length; i4++) if (!char.IsDigit(sPath[i4])) break;
                if (i4 == i3 || sPath[i4] != ']') goto err;
                ParentTableRow = int.Parse(sPath.Substring(i3, i4 - i3)) - 1;
            }

            i3 = sPath.IndexOf("td", i2);
            if (i3 != -1)
            {
                i3 += 2;
                if (sPath[i3++] != '[') goto err;
                for (i4 = i3; i4 < sPath.Length; i4++) if (!char.IsDigit(sPath[i4])) break;
                if (i4 == i3 || sPath[i4] != ']') goto err;
                ParentTableColumn = int.Parse(sPath.Substring(i3, i4 - i3)) - 1;
            }

            return;
        err:
            throw new HtmlXmlTableException("error AbsoluteTablePath is not correct : {0}", AbsoluteTablePath);
        }
        #endregion
    }

    public class HtmlXmlTables : SortedList<string, HtmlXmlTable>, IDisposable
    {
        #region variable
        public List<HtmlXmlTable> RootTables = new List<HtmlXmlTable>();
        private SortedList<string, HtmlXmlTable> gTableCode = null;
        #endregion

        #region constructor
        #region HtmlXmlTables(XmlDocument xml)
        public HtmlXmlTables(XmlDocument xml)
        {
            Create(xml);
        }
        #endregion

        #region HtmlXmlTables(string[] tablesPath)
        public HtmlXmlTables(string[] tablesPath)
        {
            Create(tablesPath);
        }
        #endregion
        #endregion

        #region Dispose
        public void Dispose()
        {
            RootTables = null;
            gTableCode = null;
        }
        #endregion

        #region Create(XmlDocument xml)
        private void Create(XmlDocument xml)
        {
            XmlNodeList tables = xml.SelectNodes("//table");
            gTableCode = new SortedList<string, HtmlXmlTable>();
            int iRootRow = 1;
            int iTable = 1;
            foreach (XmlNode table in tables)
            {
                // Xml.GetNodePath(table)
                HtmlXmlTable xmlTable = new HtmlXmlTable(table, table.zGetPath());
                //xmlTable.Id = cXml.GetValue(table, "id");
                // Xml.GetValue(table, "@id")
                xmlTable.Id = table.zGetValue("@id");
                if (xmlTable.Id == null) xmlTable.Id = "t" + iTable++.ToString();
                AddTable(xmlTable, ref iRootRow);
                this.Add(xmlTable.AbsoluteTablePath, xmlTable);
            }
            SetColChilds();
            SetColumnNumber();
            foreach (HtmlXmlTable xmlTable in this.Values)
            {
                if (xmlTable.Code != null) gTableCode.Add(xmlTable.Code, xmlTable);
                if (xmlTable.RowChilds.Count != 0)
                    xmlTable.FirstChildByRow = xmlTable.RowChilds.Values[0].Values[0][0];
                if (xmlTable.ColChilds.Count != 0)
                    xmlTable.FirstChildByCol = xmlTable.ColChilds.Values[0].Values[0][0];
            }
        }
        #endregion

        #region Create(string[] tablesPath)
        private void Create(string[] tablesPath)
        {
            int iRootRow = 1;
            int iTable = 1;
            foreach (string tablePath in tablesPath)
            {
                HtmlXmlTable xmlTable = new HtmlXmlTable(tablePath);
                xmlTable.Id = "t" + iTable++.ToString();
                AddTable(xmlTable, ref iRootRow);
                this.Add(xmlTable.AbsoluteTablePath, xmlTable);
            }
            SetColChilds();
            SetColumnNumber();
            foreach (HtmlXmlTable xmlTable in this.Values)
                if (xmlTable.Code != null) gTableCode.Add(xmlTable.Code, xmlTable);
        }
        #endregion

        #region AddTable
        private void AddTable(HtmlXmlTable xmlTable, ref int iRootRow)
        {
            //cTrace.Trace("AddTable : {0}", xmlTable.Id);
            if (xmlTable.ParentTablePath != null)
            {
                int iParent = this.IndexOfKey(xmlTable.ParentTablePath);
                if (iParent == -1) throw new HtmlXmlTableException("error parent table {0} does'nt exist", xmlTable.ParentTablePath);
                xmlTable.ParentTable = this.Values[iParent];
                xmlTable.Level = xmlTable.ParentTable.Level + 1;

                // recherche les tables sur la même ligne
                int iRowNumber = xmlTable.ParentTable.RowNumber + 1;
                SortedList<int, List<HtmlXmlTable>> rowChildsCol;
                SortedList<int, SortedList<int, List<HtmlXmlTable>>> parentRowChilds = xmlTable.ParentTable.RowChilds;
                int iRow = parentRowChilds.IndexOfKey(xmlTable.ParentTableRow);
                if (iRow == -1)
                {
                    if (parentRowChilds.Count != 0)
                    {
                        HtmlXmlTable previousRowChild = parentRowChilds.Values[parentRowChilds.Count - 1].Values[0][0];
                        previousRowChild.ParentByRowNextRow = xmlTable;
                        xmlTable.ParentByRowPreviousRow = previousRowChild;

                        iRowNumber = previousRowChild.RowNumber + previousRowChild.NbRow;
                    }
                    rowChildsCol = new SortedList<int, List<HtmlXmlTable>>();
                    iRow = parentRowChilds.Count;
                    parentRowChilds.Add(xmlTable.ParentTableRow, rowChildsCol);
                }
                else
                {
                    rowChildsCol = parentRowChilds.Values[iRow];
                    iRowNumber = rowChildsCol.Values[0][0].RowNumber;
                }

                List<HtmlXmlTable> cellChilds;
                int iCol;
                iCol = rowChildsCol.IndexOfKey(xmlTable.ParentTableColumn);
                if (iCol == -1)
                {
                    if (rowChildsCol.Count != 0)
                    {
                        HtmlXmlTable previousColumnChild = rowChildsCol.Values[rowChildsCol.Count - 1][0];
                        previousColumnChild.ParentByRowNextColumn = xmlTable;
                        xmlTable.ParentByRowPreviousColumn = previousColumnChild;
                    }
                    cellChilds = new List<HtmlXmlTable>();
                    iCol = rowChildsCol.Count;
                    rowChildsCol.Add(xmlTable.ParentTableColumn, cellChilds);
                }
                else
                {
                    cellChilds = rowChildsCol.Values[iCol];
                    HtmlXmlTable previousCellChild = cellChilds[cellChilds.Count - 1];
                    previousCellChild.ParentNextCell = xmlTable;
                    xmlTable.ParentPreviousCell = previousCellChild;

                    iRowNumber = previousCellChild.RowNumber + previousCellChild.NbRow;
                }

                // recherche les tables sur la même colonne
                SortedList<int, List<HtmlXmlTable>> colChildsRow;
                SortedList<int, SortedList<int, List<HtmlXmlTable>>> parentColChilds = xmlTable.ParentTable.ColChilds;
                int iCol2 = parentColChilds.IndexOfKey(xmlTable.ParentTableColumn);
                if (iCol2 == -1)
                {
                    colChildsRow = new SortedList<int, List<HtmlXmlTable>>();
                    iCol2 = parentColChilds.Count;
                    parentColChilds.Add(xmlTable.ParentTableColumn, colChildsRow);
                }
                else
                    colChildsRow = parentColChilds.Values[iCol2];


                List<HtmlXmlTable> cellChilds2;
                int iRow2;
                iRow2 = colChildsRow.IndexOfKey(xmlTable.ParentTableRow);
                if (iRow2 == -1)
                {
                    cellChilds2 = new List<HtmlXmlTable>();
                    iRow2 = colChildsRow.Count;
                    colChildsRow.Add(xmlTable.ParentTableRow, cellChilds2);
                }
                else
                    cellChilds2 = colChildsRow.Values[iRow2];

                xmlTable.Code2 = xmlTable.ParentTable.Code2 + string.Format("-{0}.{1}.{2}", iRow + 1, iCol + 1, cellChilds.Count + 1);
                xmlTable.RowNumber = iRowNumber;
                cellChilds.Add(xmlTable);
                cellChilds2.Add(xmlTable);
                xmlTable.ParentFirstCell = cellChilds[0];
                xmlTable.ParentByRowFirstColumn = rowChildsCol.Values[0][0];
                xmlTable.ParentByRowFirstRow = parentRowChilds.Values[0].Values[0][0];
                if (xmlTable.ParentFirstCell != xmlTable) xmlTable.ParentFirstCell.NbRow++;
                if (xmlTable.ParentFirstCell.NbRow > xmlTable.ParentByRowFirstColumn.ColMaxNbRow)
                {
                    int iNbRow = xmlTable.ParentFirstCell.NbRow - xmlTable.ParentByRowFirstColumn.ColMaxNbRow;
                    xmlTable.ParentByRowFirstColumn.ColMaxNbRow = xmlTable.ParentFirstCell.NbRow;
                    xmlTable.ParentTable.NbRow += iNbRow;
                    // Ajoute iNbRow aux tables parentes de xmlTable
                    AddRow(xmlTable.ParentTable, iNbRow);
                }
            }
            else
            {
                xmlTable.Code2 = string.Format("T{0}", iRootRow++);
                xmlTable.ParentFirstCell = xmlTable;
                xmlTable.ParentByRowFirstColumn = xmlTable;
                xmlTable.ParentByRowFirstRow = xmlTable;
                if (RootTables.Count != 0)
                {
                    HtmlXmlTable previousRootTable = RootTables[RootTables.Count - 1];
                    previousRootTable.NextRootTable = xmlTable;
                    xmlTable.PreviousRootTable = previousRootTable;
                    xmlTable.RowNumber = previousRootTable.RowNumber + previousRootTable.NbRow;
                }
                else
                    xmlTable.RowNumber = 0;
                RootTables.Add(xmlTable);
            }
        }
        #endregion

        /// <summary>
        /// Ajoute iNbRow aux tables parentes de xmlTable
        /// </summary>
        private static void AddRow(HtmlXmlTable xmlTable, int iNbRow)
        {
            while (xmlTable.ParentTable != null)
            {
                if (xmlTable.ParentFirstCell != xmlTable) xmlTable.ParentFirstCell.NbRow += iNbRow;
                iNbRow = xmlTable.ParentFirstCell.NbRow - xmlTable.ParentByRowFirstColumn.ColMaxNbRow;
                if (iNbRow <= 0) break;
                xmlTable.ParentByRowFirstColumn.ColMaxNbRow = xmlTable.ParentFirstCell.NbRow;
                xmlTable.ParentTable.NbRow += iNbRow;
                xmlTable = xmlTable.ParentTable;
            }
        }

        #region SetColChilds
        private void SetColChilds()
        {
            // Liste des tables par colonne, liste des tables par ligne, liste des tables par cellule
            //public SortedList<int, SortedList<int, List<HtmlXmlTable>>> ColChilds = new SortedList<int, SortedList<int, List<HtmlXmlTable>>>();

            foreach (HtmlXmlTable rootTable in RootTables)
            {
                HtmlXmlTable xmlTable = rootTable;
                do
                {
                    //cTrace.Trace("SetColChilds : {0}", xmlTable.Id);
                    if (xmlTable.ColChilds.Count != 0)
                    {
                        HtmlXmlTable ParentByColFirstColumn = xmlTable.ColChilds.Values[0].Values[0][0];
                        HtmlXmlTable ParentByColPreviousColumn = null;
                        for (int i1 = 0; i1 < xmlTable.ColChilds.Count; i1++)
                        {
                            SortedList<int, List<HtmlXmlTable>> rowTables = xmlTable.ColChilds.Values[i1];
                            HtmlXmlTable ParentByColPreviousRow = null;
                            HtmlXmlTable ParentByColFirstRow = rowTables.Values[0][0];
                            for (int i2 = 0; i2 < rowTables.Count; i2++)
                            {
                                List<HtmlXmlTable> colTables = rowTables.Values[i2];
                                for (int i3 = 0; i3 < colTables.Count; i3++)
                                {
                                    HtmlXmlTable xmlTable2 = colTables[i3];
                                    xmlTable2.ParentByColFirstColumn = ParentByColFirstColumn;
                                    xmlTable2.ParentByColFirstRow = ParentByColFirstRow;
                                    if (i3 == 0)
                                    {
                                        if (i2 == 0)
                                        {
                                            if (ParentByColPreviousColumn != null)
                                            {
                                                ParentByColPreviousColumn.ParentByColNextColumn = xmlTable2;
                                                xmlTable2.ParentByColPreviousColumn = ParentByColPreviousColumn;
                                            }
                                            ParentByColPreviousColumn = xmlTable2;
                                        }
                                        if (ParentByColPreviousRow != null)
                                        {
                                            ParentByColPreviousRow.ParentByColNextRow = xmlTable2;
                                            xmlTable2.ParentByColPreviousRow = ParentByColPreviousRow;
                                        }
                                        ParentByColPreviousRow = xmlTable2;
                                    }

                                }
                            }
                        }
                    }
                    xmlTable = SetColChildsGetNextTable(xmlTable);
                } while (xmlTable != null);
            }
        }
        #endregion

        #region SetColChildsGetNextTable
        private HtmlXmlTable SetColChildsGetNextTable(HtmlXmlTable xmlTable)
        {
            if (xmlTable.RowChilds.Count != 0)
                return xmlTable.RowChilds.Values[0].Values[0][0];
            HtmlXmlTable xmlTable2 = xmlTable;
            do
            {
                do
                {
                    do
                    {
                        do
                        {
                            if (xmlTable2 != null)
                            {
                                xmlTable = xmlTable2;
                                if (xmlTable.RowChilds.Count != 0)
                                    return xmlTable;
                            }

                            xmlTable2 = xmlTable.ParentNextCell;
                        } while (xmlTable2 != null);

                        xmlTable = xmlTable.ParentFirstCell;
                        xmlTable2 = xmlTable.ParentByRowNextColumn;
                    } while (xmlTable2 != null);

                    xmlTable = xmlTable.ParentByRowFirstColumn;
                    xmlTable2 = xmlTable.ParentByRowNextRow;
                } while (xmlTable2 != null);

                xmlTable = xmlTable.ParentTable;
            } while (xmlTable != null);
            return null;
        }
        #endregion

        #region SetColumnNumber
        private void SetColumnNumber()
        {
            foreach (HtmlXmlTable rootTable in RootTables)
            {
                int iColumnNumber = 0;
                HtmlXmlTable xmlTable = rootTable;
                do
                {
                    //cTrace.Trace("SetColumnNumber {0}", xmlTable.Id);
                    xmlTable.ColumnNumber = iColumnNumber;
                    xmlTable.Code = string.Format("T{0}.{1}.{2}", xmlTable.Level, xmlTable.RowNumber + 1, xmlTable.ColumnNumber + 1);
                    HtmlXmlTable parentTable = xmlTable.ParentTable;
                    while (parentTable != null)
                    {
                        int iNbColumn = iColumnNumber - parentTable.ColumnNumber + 1;
                        if (iNbColumn > parentTable.NbColumn) parentTable.NbColumn = iNbColumn;
                        parentTable = parentTable.ParentTable;
                    }
                    xmlTable = ColumnNumberGetNextTable(xmlTable, ref iColumnNumber);
                } while (xmlTable != null);
            }
        }
        #endregion

        private static HtmlXmlTable ColumnNumberGetNextTable(HtmlXmlTable xmlTable, ref int iColumnNumber)
        {
            if (xmlTable.ColChilds.Count != 0)
            {
                return xmlTable.ColChilds.Values[0].Values[0][0];
            }
            while (xmlTable.ParentTable != null)
            {
                iColumnNumber = xmlTable.ColumnNumber;
                if (xmlTable.ParentNextCell != null)
                {
                    return xmlTable.ParentNextCell;
                }
                xmlTable = xmlTable.ParentFirstCell;
                if (xmlTable.ParentByColNextRow != null)
                {
                    return xmlTable.ParentByColNextRow;
                }
                xmlTable = xmlTable.ParentByColFirstRow;
                if (xmlTable.ParentByColNextColumn != null)
                {
                    iColumnNumber = xmlTable.ParentTable.ColumnNumber + xmlTable.ParentTable.NbColumn;
                    return xmlTable.ParentByColNextColumn;
                }
                xmlTable = xmlTable.ParentTable;
            }
            return null;
        }

        #region GetTable(string sTableCode)
        public HtmlXmlTable GetTable(string sTableCode)
        {
            int i = gTableCode.IndexOfKey(sTableCode.ToUpper());
            if (i == -1) return null;
            return gTableCode.Values[i];
        }
        #endregion

        #region GetTable(XmlNode node)
        public HtmlXmlTable GetTable(XmlNode node)
        {
            //Xml.GetNodePath(node)
            string sPathNode = node.zGetPath();
            HtmlXmlTable table;
            XPathToTranslatedXPath(sPathNode, out table);
            return table;
        }
        #endregion

        #region TableCodeExist
        public bool TableCodeExist(string sTableCode)
        {
            return gTableCode.ContainsKey(sTableCode.ToUpper());
        }
        #endregion

        #region XPathToTranslatedXPath(string sNodePath, out HtmlXmlTable table)
        public string XPathToTranslatedXPath(string sNodePath, out HtmlXmlTable table)
        {
            table = null;
            //if (gSelectPrm.Tables == null) return sNodePath;
            //if (tables == null) return sNodePath;

            int i = sNodePath.LastIndexOf("table", StringComparison.InvariantCultureIgnoreCase);
            if (i != -1)
            {
                i = sNodePath.IndexOf(']', i + 5);
                if (i != -1)
                {
                    table = this[sNodePath.Substring(0, i + 1)];
                    string sNodePath2 = table.Code + "(" + table.Id + ")";
                    if (sNodePath.Length > i + 1)
                        sNodePath2 += sNodePath.Substring(i + 1, sNodePath.Length - i - 1);
                    return sNodePath2;
                }
            }
            return sNodePath;
        }
        #endregion


        public DataTable ToDataTable()
        {
            string s = "";
            s += " Id string, Code string, Parent string, Row int, Col int, Level int, NbRow int, NbCol int,";
            s += " PrevRoot string, NextRoot string,";
            s += " RowFirstRow string, RowPrevRow string, RowNextRow string,";
            s += " RowFirstCol string, RowPrevCol string, RowNextCol string,";
            s += " ColFirstCol string, ColPrevCol string, ColNextCol string,";
            s += " ColFirstRow string, ColPrevRow string, ColNextRow string,";
            s += " FirstCell string, PrevCell string, NextCell string,";
            s += " Code2 string, AbsolutePath string, Path string,";
            s += " ParentPath string, ParentRow int, ParentCol int";
            DataTable dt = pb.Data.zdt.Create(s);

            foreach (HtmlXmlTable table in this.Values)
            {
                DataRow row = dt.NewRow();
                row["Id"] = table.Id;
                row["Code"] = table.Code;
                row["Level"] = table.Level;
                row["Row"] = table.RowNumber;
                row["NbRow"] = table.NbRow;
                row["Col"] = table.ColumnNumber;
                row["NbCol"] = table.NbColumn;

                if (table.PreviousRootTable != null) row["PrevRoot"] = table.PreviousRootTable.Id;
                if (table.NextRootTable != null) row["NextRoot"] = table.NextRootTable.Id;

                if (table.ParentTable != null) row["Parent"] = table.ParentTable.Id;

                if (table.ParentByRowFirstRow != null) row["RowFirstRow"] = table.ParentByRowFirstRow.Id;
                if (table.ParentByRowPreviousRow != null) row["RowPrevRow"] = table.ParentByRowPreviousRow.Id;
                if (table.ParentByRowNextRow != null) row["RowNextRow"] = table.ParentByRowNextRow.Id;

                if (table.ParentByRowFirstColumn != null) row["RowFirstCol"] = table.ParentByRowFirstColumn.Id;
                if (table.ParentByRowPreviousColumn != null) row["RowPrevCol"] = table.ParentByRowPreviousColumn.Id;
                if (table.ParentByRowNextColumn != null) row["RowNextCol"] = table.ParentByRowNextColumn.Id;

                if (table.ParentByColFirstColumn != null) row["ColFirstCol"] = table.ParentByColFirstColumn.Id;
                if (table.ParentByColPreviousColumn != null) row["ColPrevCol"] = table.ParentByColPreviousColumn.Id;
                if (table.ParentByColNextColumn != null) row["ColNextCol"] = table.ParentByColNextColumn.Id;

                if (table.ParentByColFirstRow != null) row["ColFirstRow"] = table.ParentByColFirstRow.Id;
                if (table.ParentByColPreviousRow != null) row["ColPrevRow"] = table.ParentByColPreviousRow.Id;
                if (table.ParentByColNextRow != null) row["ColNextRow"] = table.ParentByColNextRow.Id;

                if (table.ParentFirstCell != null) row["FirstCell"] = table.ParentFirstCell.Id;
                if (table.ParentPreviousCell != null) row["PrevCell"] = table.ParentPreviousCell.Id;
                if (table.ParentNextCell != null) row["NextCell"] = table.ParentNextCell.Id;

                row["Code2"] = table.Code2;
                row["AbsolutePath"] = table.AbsoluteTablePath;
                row["Path"] = table.TablePath;
                row["ParentPath"] = table.ParentTablePath;
                if (table.ParentTableRow != -1) row["ParentRow"] = table.ParentTableRow;
                if (table.ParentTableColumn != -1) row["ParentCol"] = table.ParentTableColumn;
                dt.Rows.Add(row);
            }
            dt.DefaultView.Sort = "Row, Col";
            //dt.DefaultView.RowFilter = "Row, Col";
            return dt;
        }

        public DataTable ToDataTable0()
        {
            string s = "";
            s += " Id string, Code string,";
            s += " PreviousRootTable string, NextRootTable string,";
            s += " ParentTable string,";
            s += " ParentFirstRow string, ParentPreviousRow string, ParentNextRow string,";
            s += " ParentFirstColumn string, ParentPreviousColumn string, ParentNextColumn string,";
            s += " ParentFirstCell string, ParentPreviousCell string, ParentNextCell string,";
            s += " Level int, RowNumber int, ColumnNumber int, NbRow int, NbColumn int, Code2 string, AbsoluteTablePath string, TablePath string,";
            s += " ParentTablePath string, ParentTableRow int, ParentTableColumn int";
            DataTable dt = pb.Data.zdt.Create(s);

            foreach (HtmlXmlTable table in this.Values)
            {
                DataRow row = dt.NewRow();
                row["Id"] = table.Id;
                row["Code"] = table.Code;

                if (table.PreviousRootTable != null) row["PreviousRootTable"] = table.PreviousRootTable.Id;
                if (table.NextRootTable != null) row["NextRootTable"] = table.NextRootTable.Id;

                if (table.ParentTable != null) row["ParentTable"] = table.ParentTable.Id;

                if (table.ParentByRowFirstRow != null) row["ParentFirstRow"] = table.ParentByRowFirstRow.Id;
                if (table.ParentByRowPreviousRow != null) row["ParentPreviousRow"] = table.ParentByRowPreviousRow.Id;
                if (table.ParentByRowNextRow != null) row["ParentNextRow"] = table.ParentByRowNextRow.Id;

                if (table.ParentByRowFirstColumn != null) row["ParentFirstColumn"] = table.ParentByRowFirstColumn.Id;
                if (table.ParentByRowPreviousColumn != null) row["ParentPreviousColumn"] = table.ParentByRowPreviousColumn.Id;
                if (table.ParentByRowNextColumn != null) row["ParentNextColumn"] = table.ParentByRowNextColumn.Id;

                if (table.ParentFirstCell != null) row["ParentFirstCell"] = table.ParentFirstCell.Id;
                if (table.ParentPreviousCell != null) row["ParentPreviousCell"] = table.ParentPreviousCell.Id;
                if (table.ParentNextCell != null) row["ParentNextCell"] = table.ParentNextCell.Id;

                row["Level"] = table.Level;
                row["RowNumber"] = table.RowNumber;
                row["ColumnNumber"] = table.ColumnNumber;
                row["NbRow"] = table.NbRow;
                row["NbColumn"] = table.NbColumn;
                row["Code2"] = table.Code2;
                row["AbsoluteTablePath"] = table.AbsoluteTablePath;
                row["TablePath"] = table.TablePath;
                row["ParentTablePath"] = table.ParentTablePath;
                if (table.ParentTableRow != -1) row["ParentTableRow"] = table.ParentTableRow;
                if (table.ParentTableColumn != -1) row["ParentTableColumn"] = table.ParentTableColumn;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static HtmlXmlTables GetTables(XmlDocument xml)
        {
            return new HtmlXmlTables(xml);
        }

        public static HtmlXmlTables GetTables(string[] tablesPath)
        {
            return new HtmlXmlTables(tablesPath);
        }
    }
}

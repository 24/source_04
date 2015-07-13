using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
using pb.Web.old;

namespace pb.old
{
    public class HtmlXmlReaderException : Exception
    {
        public HtmlXmlReaderException(string sMessage) : base(sMessage) { }
        public HtmlXmlReaderException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public HtmlXmlReaderException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public HtmlXmlReaderException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }

    //public delegate void SetResultEvent(DataTable dt);
    public delegate void SetResultEvent(DataTable dt, string xmlFormat = null);

    public class HtmlXmlReader : IDisposable
    {
        private static HtmlXmlReader _currentHtmlXmlReader = null;
        //private ITrace _trace = null;
        private bool _traceFunction = false;
        private bool _abort = false;

        private string _dir = null;

        private string _url = null;
        private Http_v1 _http = null;

        //// paramètres http
        //private HttpStaticRequestParameters _staticRequestParameters = new HttpStaticRequestParameters();
        //private HttpRequestParameters _requestParameters = new HttpRequestParameters();
        //private bool _useWebClient = false;
        //private Encoding _webEncoding = null;
        //private string _webRequestUserAgent = "Pib";
        //private CookieContainer _cookies = new CookieContainer();
        //// paramètres http réinitialisés après chaque chargement http
        //private HttpRequestMethod _webRequestMethod;
        //private string _webRequestAccept = null;
        //private string _webRequestReferer = null;
        //private NameValueCollection _webRequestHeaders = new NameValueCollection();
        //private string _webRequestContentType = null;
        //private string _webRequestContent = null;

        //private int _loadRetryTimeout = 120; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private int _loadRetryTimeout = 10; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private int _loadRepeatIfError = 1;
        private bool _webReadCommentInText = false;

        private string _webExportPath = null;
        private string _webXmlExportPath = null;

        private XmlDocumentSourceType _xmlDocumentSourceType;
        private string _xmlFileSourcePath = null;
        private string _xmlHtmlSource = null;
        private XmlDocument _xmlDocument = null;
        private XDocument _xDocument = null;
        //private XLDocument _xlDocument = null;
        //public delegate void XmlDocumentLoadedEvent(XmlDocument xml, string sUrl, Http http);
        //public event XmlDocumentLoadedEvent XmlDocumentLoaded = null;

        private HtmlXmlTables _htmlXmlTables = null;
        private bool _nodePathWithTableCode = true;
        private XmlNode _node = null;
        private XElement _xNode = null;
        private XmlNode[] _nodes = null;
        private HtmlForm _form = null;
        private XmlNode _formNode = null;
        private HtmlXmlTable _table = null;
        private XElement _xTable = null;

        public event SetResultEvent SetResult;

        public HtmlXmlReader()
        {
            //_trace = pb.Trace.CurrentTrace;
        }

        public void Dispose()
        {
            DisposeHttp();
        }

        public static HtmlXmlReader CurrentHtmlXmlReader
        {
            get
            {
                if (_currentHtmlXmlReader == null) _currentHtmlXmlReader = new HtmlXmlReader();
                return _currentHtmlXmlReader;
            }
            set { _currentHtmlXmlReader = value; }
        }

        //public ITrace Trace
        //{
        //    get { return _trace; }
        //    set { _trace = value; }
        //}

        public bool TraceFunction
        {
            get { return _traceFunction; }
            set { _traceFunction = value; }
        }

        public bool Abort
        {
            get { return _abort; }
            set { _abort = value; }
        }

        public string Dir
        {
            get { return _dir; }
            set { _dir = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public Http_v1 http
        {
            get { return _http; }
        }

        //public HttpStaticRequestParameters StaticRequestParameters
        //{
        //    get { return _staticRequestParameters; }
        //    set { _staticRequestParameters = value; }
        //}

        //public HttpRequestParameters RequestParameters
        //{
        //    get { return _requestParameters; }
        //    set { _requestParameters = value; }
        //}

        //public bool UseWebClient
        //{
        //    get { return _useWebClient; }
        //    set { _useWebClient = value; }
        //}

        //public string WebRequestUserAgent
        //{
        //    get { return _webRequestUserAgent; }
        //    set { _webRequestUserAgent = value; }
        //}

        //public HttpRequestMethod WebRequestMethod
        //{
        //    get { return _webRequestMethod; }
        //    set { _webRequestMethod = value; }
        //}

        //public string WebRequestAccept
        //{
        //    get { return _webRequestAccept; }
        //    set { _webRequestAccept = value; }
        //}

        //public string WebRequestReferer
        //{
        //    get { return _webRequestReferer; }
        //    set { _webRequestReferer = value; }
        //}

        //public NameValueCollection WebRequestHeaders
        //{
        //    get { return _webRequestHeaders; }
        //    set { _webRequestHeaders = value; }
        //}

        //public string WebRequestContentType
        //{
        //    get { return _webRequestContentType; }
        //    set { _webRequestContentType = value; }
        //}

        //public string WebRequestContent
        //{
        //    get { return _webRequestContent; }
        //    set { _webRequestContent = value; }
        //}

        //public string WebExportPath
        //{
        //    get { return _webExportPath; }
        //}

        //public string WebXmlExportPath
        //{
        //    get { return _webXmlExportPath; }
        //}

        //public CookieContainer Cookies
        //{
        //    get { return _cookies; }
        //    set { _cookies = value; }
        //}

        //public int LoadRetryTimeout
        //{
        //    get { return _loadRetryTimeout; }
        //    set { _loadRetryTimeout = value; }
        //}

        public int LoadRepeatIfError
        {
            get { return _loadRepeatIfError; }
            set { _loadRepeatIfError = value; }
        }

        //public Encoding WebEncoding
        //{
        //    get { return _webEncoding; }
        //    set { _webEncoding = value; }
        //}

        //public bool WebReadCommentInText
        //{
        //    get { return _webReadCommentInText; }
        //    set { _webReadCommentInText = value; }
        //}

        public XmlDocument XmlDocument
        {
            get
            {
                GetXmlDocument();
                return _xmlDocument;
            }
        }

        public XDocument XDocument
        {
            get
            {
                GetXDocument();
                return _xDocument;
            }
            //set { gXDocument = value; }
        }

        //public XLDocument XLDocument
        //{
        //    get
        //    {
        //        GetXLDocument();
        //        return _xlDocument;
        //    }
        //    //set { gXLDocument = value; }
        //}

        //public XmlNode Node
        //{
        //    get { return _node; }
        //    set { _node = value; }
        //}

        //public XElement XNode
        //{
        //    get { return _xNode; }
        //    set { _xNode = value; }
        //}

        //public XmlNode[] Nodes
        //{
        //    get { return _nodes; }
        //    set { _nodes = value; }
        //}

        //public HtmlForm Form
        //{
        //    get { return _form; }
        //    set { _form = value; }
        //}

        //public XmlNode FormNode
        //{
        //    get { return _formNode; }
        //    set { _formNode = value; }
        //}

        //public HtmlXmlTable Table
        //{
        //    get { return _table; }
        //    set { _table = value; }
        //}

        //public XElement XTable
        //{
        //    get { return _xTable; }
        //    set { _xTable = value; }
        //}

        //public HtmlXmlTables Tables
        //{
        //    get { return _htmlXmlTables; }
        //    set { _htmlXmlTables = value; }
        //}

        //public bool NodePathWithTableCode
        //{
        //    get { return _nodePathWithTableCode; }
        //    set { _nodePathWithTableCode = value; }
        //}

        private void RazXmlDocument()
        {
            //_xmlDocument = null;
            _xDocument = null;
            //_xlDocument = null;

            _xmlDocumentSourceType = XmlDocumentSourceType.NoSource;
            _xmlFileSourcePath = null;
            _xmlHtmlSource = null;
        }

        private void GetXmlDocument()
        {
            if (_xmlDocument != null) return;
            if (_xmlDocumentSourceType == XmlDocumentSourceType.Http)
            {
                _xmlDocument = _http.GetXmlDocumentResult();
                //if (_trace.TraceLevel >= 2)
                //{
                //    _webXmlExportPath = _http.XmlExportPath;
                //}
                _webXmlExportPath = _http.XmlExportPath;
            }
            else if (_xmlDocumentSourceType == XmlDocumentSourceType.XmlFile)
            {
                _xmlDocument = new XmlDocument();
                _xmlDocument.Load(_xmlFileSourcePath);
            }
            else if (_xmlDocumentSourceType == XmlDocumentSourceType.HtmlString)
            {
                if (_xmlHtmlSource != null)
                {
                    using (StringReader sr = new StringReader(_xmlHtmlSource))
                    {
                        HtmlToXml hx = new HtmlToXml(sr);
                        hx.ReadCommentInText = _webReadCommentInText;
                        _xmlDocument = hx.GenerateXmlDocument();
                    }

                    //if (_trace.TraceLevel >= 2)
                    //{
                    //    string sPath = GetNewFileName();
                    //    if (_xmlDocument != null && sPath != null) _xmlDocument.Save(sPath + "_LoadHtmlString.xml");
                    //}
                    string path = GetNewFileName();
                    if (_xmlDocument != null && path != null)
                        _xmlDocument.Save(path + "_LoadHtmlString.xml");
                }
            }
        }

        private void GetXDocument()
        {
            if (_xDocument != null) return;
            if (_xmlDocumentSourceType == XmlDocumentSourceType.Http)
            {
                _xDocument = _http.GetXDocumentResult();
                //if (_trace.TraceLevel >= 2)
                //{
                //    _webXmlExportPath = _http.XmlExportPath;
                //}
                _webXmlExportPath = _http.XmlExportPath;
            }
            else if (_xmlDocumentSourceType == XmlDocumentSourceType.XmlFile)
                _xDocument = XDocument.Load(_xmlFileSourcePath);
            else if (_xmlDocumentSourceType == XmlDocumentSourceType.HtmlString)
            {
                if (_xmlHtmlSource != null)
                {
                    using (StringReader sr = new StringReader(_xmlHtmlSource))
                    {
                        HtmlToXml hx = new HtmlToXml(sr);
                        hx.ReadCommentInText = _webReadCommentInText;
                        _xDocument = hx.GenerateXDocument();
                    }

                    //if (_trace.TraceLevel >= 2)
                    //{
                    //    string sPath = GetNewFileName();
                    //    if (_xDocument != null && sPath != null) _xDocument.Save(sPath + "_LoadHtmlString.xml");
                    //}
                    string path = GetNewFileName();
                    if (_xDocument != null && path != null)
                        _xDocument.Save(path + "_LoadHtmlString.xml");
                }
            }
        }

        //private void GetXLDocument()
        //{
        //    if (_xlDocument != null) return;
        //    GetXDocument();
        //    _xlDocument = new XLDocument(_xDocument);
        //}

        public void Load(string baseUrl, string url, HttpRequestParameters_v1 requestParameters = null)
        {
            //return Load(GetUrl(sBaseUrl, sUrl));
            Load(GetUrl(baseUrl, url), requestParameters);
        }

        public void Load(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            for (int i = 0; i < _loadRepeatIfError - 1; i++)
            {
                try
                {
                    _Load(url, requestParameters);
                    return;
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
                    //if (_trace.TraceLevel >= 1)
                    //    _trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                }
            }
            _Load(url, requestParameters);
        }

        private void _Load(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                //if (_trace.TraceLevel >= 1)
                //    _trace.WriteLine("Load(\"{0}\");", url);
                Trace.WriteLine(1, "Load(\"{0}\");", url);
                _url = GetUrl(url);

                //cTrace.StartLevel("Load Html");
                if (_htmlXmlTables != null)
                {
                    _htmlXmlTables.Dispose();
                    _htmlXmlTables = null;
                }

                DisposeHttp();
                _http = CreateHttp(_url, requestParameters);

                //cTrace.StopLevel("Load Html");
                //string sPath = null;
                //if (giTraceLevel >= 2)
                //{
                //    sPath = GetNewUrlFileName(gsUrl);
                //    gHttp.TextExportPath = sPath;
                //}
                //cTrace.StartLevel("Load Html");
                //gXml = gHttp.LoadXml();
                _http.Load();

                //gXmlDocument = null;
                //gXLDocument = null;
                RazXmlDocument();
                _xmlDocumentSourceType = XmlDocumentSourceType.Http;

                //if (gbGenerateXmlDocument) gXml = gHttp.GetXmlDocumentResult0();
                //if (gbGenerateXmlDocument) gXmlDocument = gHttp.GetXmlDocumentResult();
                //if (gbGenerateXLDocument) gXLDocument = new XLDocument(gHttp.GetXDocumentResult());
                //SetXml2();
                //cTrace.StopLevel("Load Html");
                //if (_trace.TraceLevel >= 2)
                //{
                //    string sPath = _http.TextExportPath;
                //    if (sPath != null)
                //    {
                //        string sContentType = _http.ContentType;
                //        string sPath2 = sPath;
                //        //sPath = gHttp.TextExportPath;
                //        string sExt = Http_v1.GetContentFileExtension(sContentType);
                //        if (sExt == ".xml")
                //        {
                //            sPath2 = zpath.PathSetFileName(sPath, Path.GetFileNameWithoutExtension(sPath) + "_source.xml");
                //            File.Move(sPath, sPath2);
                //        }
                //        //else
                //        //    sPath2 = cu.PathSetExt(sPath, sExt);
                //        //File.Move(sPath, sPath2);
                //        _webExportPath = sPath2;
                //    }

                //    //    sPath = GetNewUrlFileName(gsUrl, ".xml");
                //    //    if (gXmlDocument != null && sPath != null)
                //    //    {
                //    //        gXmlDocument.Save(sPath);
                //    //        gsWebXmlExportPath = sPath;
                //    //    }
                //}

                string path = _http.TextExportPath;
                if (path != null)
                {
                    string sContentType = _http.ContentType;
                    string path2 = path;
                    //sPath = gHttp.TextExportPath;
                    string ext = Http_v1.GetContentFileExtension(sContentType);
                    if (ext == ".xml")
                    {
                        path2 = zpath.PathSetFileName(path, Path.GetFileNameWithoutExtension(path) + "_source.xml");
                        File.Move(path, path2);
                    }
                    _webExportPath = path2;
                }

                //if (XmlDocumentLoaded != null) XmlDocumentLoaded(gXmlDocument, gsUrl, gHttp);
                //return gXml;
            }
            finally
            {
                // modif le 05/02/2014
                //ResetHttpParameters();
                EndLoadingHttp();
            }
        }

        public Image LoadImage(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            for (int i = 0; i < _loadRepeatIfError - 1; i++)
            {
                try
                {
                    return _LoadImage(url, requestParameters);
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
                    //if (_trace.TraceLevel >= 1)
                    //    _trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                }
            }
            return _LoadImage(url, requestParameters);
        }

        private Image _LoadImage(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                //if (_trace.TraceLevel >= 1)
                //    _trace.WriteLine("LoadImage(\"{0}\");", url);
                Trace.WriteLine(1, "LoadImage(\"{0}\");", url);
                _url = GetUrl(url);

                DisposeHttp();
                _http = CreateHttp(_url, requestParameters);

                return _http.LoadImage();

            }
            finally
            {
                // modif le 05/02/2014
                //ResetHttpParameters();
                EndLoadingHttp();
            }
        }

        public bool LoadToFile(string url, string path, HttpRequestParameters_v1 requestParameters = null)
        {
            for (int i = 0; i < _loadRepeatIfError - 1; i++)
            {
                try
                {
                    return _LoadToFile(url, path, requestParameters);
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
                    //if (_trace.TraceLevel >= 1)
                    //    _trace.WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                    Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
                }
            }
            return _LoadToFile(url, path, requestParameters);
        }

        private bool _LoadToFile(string url, string path, HttpRequestParameters_v1 requestParameters = null)
        {
            try
            {
                //if (_trace.TraceLevel >= 1)
                //    _trace.WriteLine("LoadToFile(\"{0}\", \"{1}\");", url, path);
                Trace.WriteLine(1, "LoadToFile(\"{0}\", \"{1}\");", url, path);
                _url = GetUrl(url);

                DisposeHttp();
                _http = CreateHttp(_url, requestParameters);

                return _http.LoadToFile(path);

            }
            finally
            {
                // modif le 05/02/2014
                //ResetHttpParameters();
                EndLoadingHttp();
            }
        }

        public Http_v1 CreateHttp(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            Http_v1 http = new Http_v1(url, requestParameters);
            http.HttpRetry += new Http_v1.fnHttpRetry(LoadRetryEvent);
            http.LoadRetryTimeout = _loadRetryTimeout;
            http.ReadCommentInText = _webReadCommentInText;
            _webExportPath = null;
            _webXmlExportPath = null;
            //if (_trace.TraceLevel >= 2)
            //    http.TraceDirectory = _trace.TraceDir;
            return http;
        }

        private void EndLoadingHttp()
        {
            http.HttpRetry -= new Http_v1.fnHttpRetry(LoadRetryEvent);
        }

        // modif le 05/02/2014
        //private void ResetHttpParameters()
        //{
        //    //_webRequestMethod = HttpRequestMethod.Get;
        //    //_webRequestAccept = null;
        //    //_webRequestReferer = null;
        //    //_webRequestHeaders = new NameValueCollection();
        //    //_webRequestContentType = null;
        //    //_webRequestContent = null;
        //    _requestParameters.method = HttpRequestMethod.Get;
        //    _requestParameters.accept = null;
        //    _requestParameters.referer = null;
        //    _requestParameters.headers = new NameValueCollection();
        //    _requestParameters.contentType = null;
        //    _requestParameters.content = null;
        //}

        public void ResetCookies()
        {
            //_cookies = new CookieContainer();
            //_requestParameters.cookies = new CookieContainer();
        }

        private void DisposeHttp()
        {
            if (_http != null)
            {
                _http.Dispose();
                _http = null;
            }
        }

        public void LoadXml(string sPath)
        {
            //if (_trace.TraceLevel >= 1)
            //    _trace.WriteLine("LoadXml(\"{0}\");", sPath);
            Trace.WriteLine(1, "LoadXml(\"{0}\");", sPath);

            if (_htmlXmlTables != null)
            {
                _htmlXmlTables.Dispose();
                _htmlXmlTables = null;
            }

            //gXmlDocument = null;
            //gXLDocument = null;
            RazXmlDocument();
            _xmlFileSourcePath = sPath;
            _xmlDocumentSourceType = XmlDocumentSourceType.XmlFile;

            //if (gbGenerateXmlDocument)
            //{
            //    gXmlDocument = new XmlDocument();
            //    gXmlDocument.Load(sPath);
            //}
            //if (gbGenerateXLDocument)
            //    gXLDocument = new XLDocument(XDocument.Load(sPath));

            //SetXml2();
            //if (XmlDocumentLoaded != null) XmlDocumentLoaded(gXmlDocument, sPath, null);
        }

        public void LoadHtmlString(string sHtml)
        {
            //if (_trace.TraceLevel >= 1)
            //{
            //    if (sHtml == null)
            //        _trace.WriteLine("LoadLoadHtmlString(null);");
            //    else
            //        _trace.WriteLine("LoadLoadHtmlString();");
            //}
            if (sHtml == null)
                Trace.WriteLine(1, "LoadLoadHtmlString(null);");
            else
                Trace.WriteLine(1, "LoadLoadHtmlString();");

            if (_htmlXmlTables != null)
            {
                _htmlXmlTables.Dispose();
                _htmlXmlTables = null;
            }

            //gXmlDocument = null;
            //gXLDocument = null;
            RazXmlDocument();
            _xmlHtmlSource = sHtml;
            _xmlDocumentSourceType = XmlDocumentSourceType.HtmlString;

            //if (sHtml == null) return;

            if (sHtml != null)
            {
                //string sPath;
                //if (_trace.TraceLevel >= 2)
                //{
                //    sPath = GetNewFileName();
                //    if (sPath != null) zfile.WriteFile(sPath + "_LoadHtmlString.html", sHtml);
                //}
                string path = GetNewFileName();
                if (path != null)
                    zfile.WriteFile(path + "_LoadHtmlString.html", sHtml);
            }

            //if (gbGenerateXmlDocument)
            //{
            //    using (StringReader sr = new StringReader(sHtml))
            //    {
            //        HtmlXml hx = new HtmlXml(sr);
            //        hx.ReadCommentInText = gbWebReadCommentInText;
            //        gXmlDocument = hx.GenerateXmlDocument();
            //    }
            //}
            //if (gbGenerateXLDocument)
            //{
            //    using (StringReader sr = new StringReader(sHtml))
            //    {
            //        HtmlXml hx = new HtmlXml(sr);
            //        hx.ReadCommentInText = gbWebReadCommentInText;
            //        gXLDocument = new XLDocument(hx.GenerateXDocument());
            //    }
            //}

            //if (giTraceLevel >= 2)
            //{
            //    sPath = GetNewFileName();
            //    if (gXmlDocument != null && sPath != null) gXmlDocument.Save(sPath + "_LoadHtmlString.xml");
            //}

            //if (XmlDocumentLoaded != null) XmlDocumentLoaded(gXmlDocument, null, null);
        }

        public void Save(string url)
        {
            string path = Http_v1.UrlToFileName(url);
            //if (Dir != null) sPath = Dir + sPath;
            path = GetPathFichier(path);
            Save(url, path, true);
        }

        public void Save(string url, string path, bool setExtFromHttpContentType = false)
        {
            //string sUrl2 = GetUrl(url);

            Http_v1 http = CreateHttp(url);

            try
            {
                string sContentType = http.ContentType;
                string sExt = Http_v1.GetContentFileExtension(sContentType);
                if (setExtFromHttpContentType)
                    path = zpath.PathSetExtension(path, sExt);
                //if (_trace.TraceLevel >= 1)
                //    _trace.WriteLine("Save(\"{0}\"); -> \"{1}\"", url, path);
                Trace.WriteLine(1, "Save(\"{0}\"); -> \"{1}\"", url, path);
                //http.LoadToFile(sPath);
                http.Load();
                http.SaveResult(path);
            }
            catch (Exception ex)
            {
                if (!(ex is WebException)) throw;
                WebException wex = (WebException)ex;
                // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
                if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
                    throw;
            }
            finally
            {
                http.Dispose();
                // modif le 05/02/2014
                //ResetHttpParameters();
                EndLoadingHttp();
            }

        }

        private bool LoadRetryEvent(Exception ex)
        {
            //_trace.WriteLine("Error loading html  : \"{0}\"", _url);
            //_trace.WriteError(ex);
            Trace.WriteLine("Error loading html  : \"{0}\"", _url);
            Trace.CurrentTrace.WriteError(ex);
            return !_abort;
        }

        public DataTable ReadSelect(string sXPath, params string[] sValues)
        {
            return _ReadSelect(XmlDocument, sXPath, sValues);
        }

        public DataTable ReadSelect(XmlNode node, string sXPath, params string[] sValues)
        {
            return _ReadSelect(node, sXPath, sValues);
        }

        private DataTable _ReadSelect(XmlNode node, string sXPath, params string[] sValues)
        {
            if (node == null) { _SetResult(null); return null; }
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no xml data loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            if (sValues.Length == 0) sValues = new string[] { ":.:NodeValue" };
            XmlSelect select = XmlForHtmlXmlReader.Select(node, new XmlSelectParameters(t, _url, _traceFunction), sXPath, sValues);
            DataTable dt = XmlForHtmlXmlReader.ReadSelect(select);
            _SetResult(dt);
            return dt;
        }

        public XmlSelect Select(string sXPath, params string[] sValues)
        {
            return Select(XmlDocument, sXPath, sValues);
        }

        public XmlSelect Select(XmlNode node, string sXPath, params string[] sValues)
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no xml data loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            if (sValues.Length == 0) sValues = new string[] { ":.:NodeValue" };

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = new XmlSelectParameters(t, _url, _traceFunction);
            xSelect.SourceXPathNode = sXPath;
            xSelect.SourceXPathValues = sValues;
            return xSelect;
        }

        public string SelectValue(string sXPath, params string[] sValues)
        {
            //Message("SelectValue(\"{0}\");", sXPath);
            return _SelectValue(XmlDocument, sXPath, sValues);
        }

        public string SelectValue(XmlNode node, string sXPath, params string[] sValues)
        {
            //Message("SelectValue(node, \"{0}\");", sXPath);
            return _SelectValue(node, sXPath, sValues);
        }

        private string _SelectValue(XmlNode node, string sXPath, params string[] sValues)
        {
            if (node == null) return null;
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            return XmlForHtmlXmlReader.SelectValue(node, new XmlSelectParameters(t, _url, _traceFunction), sXPath, sValues);
        }

        public string[] SelectValues(string sXPath, params string[] sValues)
        {
            //Message("SelectValues(\"{0}\");", sXPath);
            return _SelectValues(XmlDocument, sXPath, sValues);
        }

        public string[] SelectValues(XmlNode node, string sXPath, params string[] sValues)
        {
            //Message("SelectValues(\"{0}\");", sXPath);
            return _SelectValues(node, sXPath, sValues);
        }

        private string[] _SelectValues(XmlNode node, string sXPath, params string[] sValues)
        {
            if (node == null) return new string[0];
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            return XmlForHtmlXmlReader.SelectValues(node, new XmlSelectParameters(t, _url, _traceFunction), sXPath, sValues);
        }

        public XmlNode SelectNode(string sXPath, params string[] sValues)
        {
            return _SelectNode(XmlDocument, sXPath, sValues);
        }

        public XmlNode SelectNode(XmlNode node, string sXPath, params string[] sValues)
        {
            return _SelectNode(node, sXPath, sValues);
        }

        private XmlNode _SelectNode(XmlNode node, string sXPath, params string[] sValues)
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            XmlSelectParameters selectPrm = new XmlSelectParameters(t, _url, _traceFunction);
            selectPrm.EmptyRow = true;
            _node = XmlForHtmlXmlReader.SelectNode(node, selectPrm, sXPath, sValues);
            return _node;
        }

        public XElement SelectXNode(IEnumerable<XElement> xelms)
        {
            _xNode = xelms.FirstOrDefault();
            return _xNode;
        }

        public XElement SelectXNode(XElement xelm)
        {
            _xNode = xelm;
            return _xNode;
        }

        public XmlNode[] SelectNodes(string sXPath, params string[] sValues)
        {
            return _SelectNodes(XmlDocument, sXPath, sValues);
        }

        public XmlNode[] SelectNodes(XmlNode node, string sXPath, params string[] sValues)
        {
            return _SelectNodes(node, sXPath, sValues);
        }

        private XmlNode[] _SelectNodes(XmlNode node, string sXPath, params string[] sValues)
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            XmlSelectParameters selectPrm = new XmlSelectParameters(t, _url, _traceFunction);
            selectPrm.EmptyRow = true;
            _nodes = XmlForHtmlXmlReader.SelectNodes(node, selectPrm, sXPath, sValues);
            //
            return _nodes;
        }

        public HtmlForm SelectForm(string sXPath)
        {
            return _SelectForm(XmlDocument, sXPath, null, null);
        }

        public HtmlForm SelectForm(string sXPath, string sAction, string sMethod, params string[] sValues)
        {
            return _SelectForm(XmlDocument, sXPath, sAction, sMethod, sValues);
        }

        public HtmlForm SelectForm(string sAction, string sMethod)
        {
            return _SelectForm(XmlDocument, null, sAction, sMethod);
        }

        public HtmlForm SelectForm(XmlNode node, string sXPath)
        {
            return _SelectForm(node, sXPath, null, null);
        }

        public HtmlForm SelectForm(XmlNode node, string sXPath, string sAction, string sMethod, params string[] sValues)
        {
            return _SelectForm(node, sXPath, sAction, sMethod, sValues);
        }

        public HtmlForm SelectForm(XmlNode node, string sAction, string sMethod)
        {
            return _SelectForm(node, null, sAction, sMethod);
        }

        private HtmlForm _SelectForm(XmlNode node, string sPath, string sAction, string sMethod, params string[] sValues)
        {
            if (node == null) return null;
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            XmlSelectParameters selectPrm = new XmlSelectParameters(t, _url, _traceFunction);
            selectPrm.EmptyRow = true;
            if (sPath == null || sPath == "") sPath = ".//form";
            if (sAction != null && sAction != "") sPath += string.Format(":FindAttrib(action={0})", sAction);
            if (sMethod != null && sMethod != "") sPath += string.Format(":FindAttrib(method={0})", sMethod);

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = sPath;
            if (sValues.Length == 0)
                xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
            else
                xSelect.SourceXPathValues = sValues;

            if (!xSelect.Get()) return null;
            _formNode = xSelect.CurrentNode;
            _table = xSelect.CurrentTable;

            _form = HtmlForm.GetForm(_formNode);
            return _form;
        }

        public DataTable GetForms()
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            HtmlXmlTables t = null;
            if (_nodePathWithTableCode)
            {
                if (_htmlXmlTables == null)
                    _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
                t = _htmlXmlTables;
            }
            HtmlForms gForms = HtmlForms.GetForms(XmlDocument, t);

            DataTable dt = pb.Data.zdt.Create("Code, Method, Action, Path");
            foreach (var form in gForms.Values)
                dt.Rows.Add(form.Code, form.Method, form.Action, form.Path);
            //Result = dt;
            _SetResult(dt);
            return dt;
        }

        public string[] FormGetValues(string sName)
        {
            if (_form == null) throw new HtmlXmlReaderException("error no form selected");
            return _form.GetValues(sName);
        }

        public void FormSetValue(string sName, string sValue)
        {
            if (_form == null) throw new HtmlXmlReaderException("error no form selected");
            _form.TrySetValue(sName, sValue);
        }

        public void FormRequest()
        {
            if (_form == null) throw new HtmlXmlReaderException("error no form selected");
            string sRequest = _form.GetRequest();
            Load(GetUrl(sRequest));
        }

        public void FormGetValueList()
        {
            if (_form == null) throw new HtmlXmlReaderException("error no form selected");
            DataTable dt = pb.Data.zdt.Create("Control string, Type string, Value string, Text string, Checked bool, NbCol int, NbRow int, Size int, ImageSource string, ControlName string, TypeName string");
            foreach (HtmlFormControl control in _form.Controls.Values)
            {
                DataRow row = dt.NewRow();
                row["Control"] = control.Name;
                row["Type"] = control.Type.ToString();
                row["Value"] = control.Value;
                //row["OptionSelected"] = control.OptionSelected;
                row["Checked"] = control.Checked;
                row["NbCol"] = control.NbCol;
                row["NbRow"] = control.NbRow;
                row["Size"] = control.Size;
                row["ImageSource"] = control.ImageSource;
                row["ControlName"] = control.ControlName;
                row["TypeName"] = control.TypeName;
                dt.Rows.Add(row);
                //if (control.Type == HtmlFormControlType.Select)
                if (control.ControlOption != null)
                {
                    foreach (HtmlControlOption option in control.ControlOption)
                    {
                        row = dt.NewRow();
                        row["Value"] = option.Value;
                        row["Text"] = option.Text;
                        row["Checked"] = option.Selected;
                        dt.Rows.Add(row);
                    }
                }
            }
            //Result = dt;
            _SetResult(dt);
        }

        public HtmlXmlTable SelectTable()
        {
            return SelectTable(_node);
        }

        public HtmlXmlTable SelectTable(XmlNode node)
        {
            return _SelectTable(node);
        }

        public HtmlXmlTable SelectTable(string sXPath, params string[] sValues)
        {
            return _SelectTable(XmlDocument, sXPath, sValues);
        }

        public HtmlXmlTable SelectTable(XmlNode node, string sXPath, params string[] sValues)
        {
            return _SelectTable(node, sXPath, sValues);
        }

        public XElement SelectTable(XElement xe)
        {
            _xTable = xe.AncestorsAndSelf("table").FirstOrDefault();
            return _xTable;
        }

        private HtmlXmlTable _SelectTable(XmlNode node)
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            if (_htmlXmlTables == null)
                _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
            _table = _htmlXmlTables.GetTable(node);
            return _table;
        }

        private HtmlXmlTable _SelectTable(XmlNode node, string sXPath, params string[] sValues)
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            if (_htmlXmlTables == null)
                _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
            XmlSelectParameters selectPrm = new XmlSelectParameters(_htmlXmlTables, _url, _traceFunction);
            selectPrm.EmptyRow = true;
            _table = __SelectTable(node, selectPrm, sXPath, sValues);
            return _table;
        }

        private static HtmlXmlTable __SelectTable(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        {
            if (node == null || sXPath == null) return null;

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = sXPath;
            if (sValues.Length == 0)
                xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
            else
                xSelect.SourceXPathValues = sValues;

            if (xSelect.Get())
            {
                if (xSelect.LastValueTable != null)
                    return xSelect.CurrentTable;
                else
                    return xSelect.CurrentTable;
            }
            return null;
        }

        public DataTable TableSelect(string sXPath, params string[] sValues)
        {
            XmlNode node = null;
            if (_table != null) node = _table.Node;
            return _ReadSelect(node, sXPath, sValues);
        }

        public string TableSelectValue(string sXPath, params string[] sValues)
        {
            XmlNode node = null;
            if (_table != null) node = _table.Node;
            return _SelectValue(node, sXPath, sValues);
        }

        public string[] TableSelectValues(string sXPath, params string[] sValues)
        {
            XmlNode node = null;
            if (_table != null) node = _table.Node;
            return _SelectValues(node, sXPath, sValues);
        }

        public HtmlXmlTable TableGetParent()
        {
            if (_table == null) return null;
            _table = _table.ParentTable;
            return _table;
        }

        public HtmlXmlTable TableGetFirstChild()
        {
            if (_table == null) return null;
            _table = _table.FirstChildByRow;
            return _table;
        }

        public HtmlXmlTable TableGetFirstRow()
        {
            if (_table == null) return null;
            _table = _table.ParentByRowFirstRow;
            return _table;
        }

        public HtmlXmlTable TableGetNextRow()
        {
            if (_table == null) return null;
            _table = _table.ParentByRowNextRow;
            return _table;
        }

        public HtmlXmlTable TableGetPreviousRow()
        {
            if (_table == null) return null;
            _table = _table.ParentByRowPreviousRow;
            return _table;
        }

        public HtmlXmlTable TableGetFirstColumn()
        {
            if (_table == null) return null;
            _table = _table.ParentByRowFirstColumn;
            return _table;
        }

        public HtmlXmlTable TableGetNextColumn()
        {
            if (_table == null) return null;
            _table = _table.ParentByRowNextColumn;
            return _table;
        }

        public HtmlXmlTable TableGetPreviousColumn()
        {
            if (_table == null) return null;
            _table = _table.ParentByRowPreviousColumn;
            return _table;
        }

        public HtmlXmlTable TableGetFirstCell()
        {
            if (_table == null) return null;
            _table = _table.ParentFirstCell;
            return _table;
        }

        public HtmlXmlTable TableGetNextCell()
        {
            if (_table == null) return null;
            _table = _table.ParentNextCell;
            return _table;
        }

        public HtmlXmlTable TableGetPreviousCell()
        {
            if (_table == null) return null;
            _table = _table.ParentPreviousCell;
            return _table;
        }

        public HtmlXmlTable TableGetNextRootTable()
        {
            if (_table == null) return null;
            _table = _table.NextRootTable;
            return _table;
        }

        public HtmlXmlTable TableGetPreviousRootTable()
        {
            if (_table == null) return null;
            _table = _table.PreviousRootTable;
            return _table;
        }

        public bool TableExist(string sTableCode)
        {
            if (_htmlXmlTables == null)
                _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
            return _htmlXmlTables.TableCodeExist(sTableCode);
        }

        public HtmlXmlTables GetTables()
        {
            if (XmlDocument == null) throw new HtmlXmlReaderException("error no html/xml page loaded");
            if (_htmlXmlTables == null)
                _htmlXmlTables = HtmlXmlTables.GetTables(XmlDocument);
            //Result = gHtmlXmlTables.ToDataTable();
            _SetResult(_htmlXmlTables.ToDataTable());
            return _htmlXmlTables;
        }

        public HtmlXmlTables GetTables(string[] tablesPath)
        {
            _htmlXmlTables = HtmlXmlTables.GetTables(tablesPath);
            //Result = gHtmlXmlTables.ToDataTable();
            _SetResult(_htmlXmlTables.ToDataTable());
            return _htmlXmlTables;
        }

        public string GetUrl(string sBaseUrl, string sUrl)
        {
            return zurl.GetUrl(sBaseUrl, sUrl);
        }

        public string GetUrl(string sUrl)
        {
            return zurl.GetUrl(_url, sUrl);
        }

        public string GetNewFileName()
        {
            //return zfile.GetNewIndexedFileName(_trace.TraceDir);
            return null;
        }

        public string GetNewUrlFileName(string url)
        {
            //return Http_v1.GetNewUrlFileName(_trace.TraceDir, url);
            return null;
        }

        public string GetNewUrlFileName(string url, string ext)
        {
            //return Http_v1.GetNewUrlFileName(_trace.TraceDir, url, ext);
            return null;
        }

        public string GetPathFichier(string sFileName)
        {
            if (!Path.IsPathRooted(sFileName))
            {
                string sDir = Dir;
                if (sDir != null) sFileName = sDir + sFileName;
            }
            return sFileName;
        }

        public void _SetResult(DataTable dt)
        {
            if (SetResult != null)
                SetResult(dt);
        }
    }

    public static class XmlForHtmlXmlReader
    {
        public static string SelectValue(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        {
            if (node == null || sXPath == null) return null;

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = sXPath;
            int iValue = 0;
            if (sValues.Length == 0)
                xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
            else
            {
                xSelect.SourceXPathValues = sValues;
                iValue = sValues.Length - 1;
            }

            if (xSelect.Get())
                return xSelect.Values[iValue];
            return null;
        }

        public static string[] SelectValues(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        {
            if (node == null || sXPath == null) return new string[0];

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = sXPath;
            //xSelect.SourceXPathValues = new string[] { sValue };
            int iValue = 0;
            if (sValues.Length == 0)
                //xSelect.SourceXPathValues = new string[] { "text()" };
                xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
            else
            {
                xSelect.SourceXPathValues = sValues;
                iValue = sValues.Length - 1;
            }

            List<string> sResultValues = new List<string>();
            while (xSelect.Get())
                sResultValues.Add(xSelect.Values[iValue]);
            string[] sResultValues2 = new string[sResultValues.Count];
            sResultValues.CopyTo(sResultValues2);
            return sResultValues2;
        }

        public static XmlSelect Select(XmlNode node, XmlSelectParameters selectPrm, string xpath, params string[] values)
        {
            if (xpath == null)
                return null;

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = xpath;
            xSelect.SourceXPathValues = values;
            return xSelect;
        }

        public static DataTable ReadSelect(XmlSelect select)
        {
            DataTable dtResult = CreateSelectDatatable(select);

            while (select.Get())
            {
                DataRow row = dtResult.NewRow();
                row[0] = select.TranslatedPathCurrentNode;
                row[1] = select.Values[0];
                for (int i = 1; i < select.SourceXPathValues.Length; i++)
                    row[i + 1] = select.GetValue(i);
                dtResult.Rows.Add(row);
            }
            return dtResult;
        }

        public static DataTable CreateSelectDatatable(XmlSelect select)
        {
            DataTable dtResult = new DataTable();

            dtResult.Columns.Add("node", typeof(string));
            string sColumnName = "text";
            if (select.XPathNode.IsNameDefined) sColumnName = select.XPathNode.Name;
            if (select.XPathValues.Length == 0) dtResult.Columns.Add(sColumnName, typeof(string));
            int i = 0;
            foreach (XPath xPathValue in select.XPathValues)
            {
                sColumnName = xPathValue.Name;
                if (i == 0 && !xPathValue.IsNameDefined && select.XPathNode.IsNameDefined) sColumnName = select.XPathNode.Name;
                sColumnName = pb.Data.zdt.GetNewColumnName(dtResult, sColumnName);
                dtResult.Columns.Add(sColumnName);
                i++;
            }

            return dtResult;
        }

        public static XmlNode SelectNode(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        {
            if (node == null || sXPath == null) return null;

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = sXPath;
            if (sValues.Length == 0)
                xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
            else
                xSelect.SourceXPathValues = sValues;

            if (xSelect.Get())
                return xSelect.CurrentNode;
            return null;
        }

        public static XmlNode[] SelectNodes(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        {
            if (node == null || sXPath == null) return new XmlNode[0];

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            xSelect.SelectPrm = selectPrm;
            xSelect.SourceXPathNode = sXPath;
            if (sValues.Length == 0)
                xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
            else
                xSelect.SourceXPathValues = sValues;

            List<XmlNode> nodes = new List<XmlNode>();
            while (xSelect.Get())
                nodes.Add(xSelect.CurrentNode);
            XmlNode[] nodes2 = new XmlNode[nodes.Count];
            nodes.CopyTo(nodes2);
            return nodes2;
        }
    }

    public class HtmlForms : SortedList<string, HtmlForm>
    {
        public static HtmlForms GetForms(XmlNode node, HtmlXmlTables tables)
        {
            XmlSelectParameters selectPrm = new XmlSelectParameters();
            selectPrm.EmptyRow = true;
            var formNodes = XmlForHtmlXmlReader.SelectNodes(node, selectPrm, ".//form");
            var forms = new HtmlForms();
            int iCode = 1;
            foreach (var formNode in formNodes)
            {
                var form = HtmlForm.GetForm(formNode, tables);
                form.Code = "f" + iCode.ToString();
                forms.Add(form.Code, form);
                iCode++;
            }
            return forms;
        }
    }
}

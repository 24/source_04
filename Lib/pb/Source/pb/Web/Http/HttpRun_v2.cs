using pb.Web.Html;
using System.IO;
using System.Xml.Linq;

namespace pb.Web.Http
{
    public static class HttpRun
    {
        private static HttpManager_v2 _httpManager = null;
        private static XmlDocumentSourceType _xdocumentSourceType = XmlDocumentSourceType.NoSource;
        private static XDocument _xdocument = null;

        // http
        private static HttpResult<string> _httpResult = null;

        // xml file
        private static string _xmlFile = null;

        // html string
        private static string _html = null;

        static HttpRun()
        {
            _httpManager = new HttpManager_v2();
            _httpManager.TraceException = true;
        }

        public static HttpManager_v2 HttpManager { get { return _httpManager; } }

        public static void SetRequestParameters(HttpRequestParameters requestParameters)
        {
            _httpManager.RequestParameters = requestParameters;
        }

        public static HttpResult<string> LoadText(string url)
        {
            return LoadText(new HttpRequest { Url = url });
        }

        public static HttpResult<string> LoadText(HttpRequest httpRequest)
        {
            RazSource();
            _httpResult = _httpManager.LoadText(httpRequest);
            if (_httpResult.Success)
                _xdocumentSourceType = XmlDocumentSourceType.Http;
            return _httpResult;
        }

        //public static HttpResult<string> LoadText_v1(HttpRequest httpRequest, string exportFile = null, bool setExportFileExtension = false, bool exportRequest = false)
        //{
        //    RazSource();
        //    ////__http = __httpManager.Load(httpRequest, requestParameters);
        //    //_http = _httpManager.LoadText(httpRequest, exportFile, setExportFileExtension);
        //    //return _http;
        //    TraceLevel.WriteLine(1, $"Load \"{httpRequest.Url}\" ({httpRequest.Method})");
        //    Http_v2 http = _httpManager.CreateHttp(httpRequest);
        //    http.ExportFile = exportFile;
        //    http.ExportRequest = exportRequest;
        //    http.SetExportFileExtension = setExportFileExtension;
        //    bool success = false;
        //    string text = null;
        //    try
        //    {
        //        text = http.LoadText();
        //        success = true;
        //        _xdocumentSourceType = XmlDocumentSourceType.Http;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteError(ex);
        //    }
        //    _httpResult = new HttpResult<string> { Success = success, LoadFromWeb = true, Http = http, Data = text };
        //    return _httpResult;
        //}

        public static void LoadXmlFile(string file)
        {
            RazSource();
            _xdocumentSourceType = XmlDocumentSourceType.XmlFile;
            _xmlFile = file;
        }

        public static void LoadHtmlString(string html)
        {
            RazSource();
            _xdocumentSourceType = XmlDocumentSourceType.XmlFile;
            _html = html;
        }

        public static XDocument GetXDocument()
        {
            if (_xdocument == null)
            {
                if (_xdocumentSourceType == XmlDocumentSourceType.Http)
                {
                    //__xDocument = __http.zGetXDocument();
                    //_xdocument = HttpManager.CurrentHttpManager.GetXDocument(__http);
                    _xdocument = _httpResult.zGetXDocument();
                }
                else if (_xdocumentSourceType == XmlDocumentSourceType.XmlFile)
                    _xdocument = XDocument.Load(_xmlFile);
                else if (_xdocumentSourceType == XmlDocumentSourceType.HtmlString)
                {
                    if (_html != null)
                    {
                        using (StringReader sr = new StringReader(_html))
                        {
                            HtmlToXml_v3 hx = new HtmlToXml_v3(new HtmlReader_v4(sr));
                            //hx.ReadCommentInText = _webReadCommentInText;
                            _xdocument = hx.CreateXml();
                        }
                    }
                }
            }
            return _xdocument;
        }

        private static void RazSource()
        {
            _xdocumentSourceType = XmlDocumentSourceType.NoSource;
            _xdocument = null;
            _httpResult = null;
            _xmlFile = null;
            _html = null;
        }
    }
}

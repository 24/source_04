using pb.IO;
using pb.Web.Http;
using System.IO;
using System.Xml.Linq;

namespace pb.Web.Html
{
    public class HtmlToXmlManager
    {
        private static HtmlToXmlManager _current = new HtmlToXmlManager();
        private bool _exportXml = false;

        public static HtmlToXmlManager Current { get { return _current; } }
        public bool ExportXml { get { return _exportXml; } set { _exportXml = value; } }

        public XDocument GetXDocument(HttpResult<string> httpResult)
        {
            //Trace.WriteLine($"HtmlToXmlManager.GetXDocument() ExportXml {_exportXml}");

            XDocument xml = null;
            if (httpResult.Http.ResultContentType == "text/html")
            {
                HtmlToXml_v3 hx = new HtmlToXml_v3(new HtmlReader_v4(new StringReader(httpResult.Data)));
                //hx.ReadCommentInText = _readCommentInText;
                xml = hx.CreateXml();
            }
            else if (httpResult.Http.ResultContentType == "text/xml")
            {
                xml = XDocument.Parse(httpResult.Data, LoadOptions.PreserveWhitespace);
            }
            else
                throw new PBException("Error can't transform http content \"{0}\" to xml", httpResult.Http.ResultContentType);

            //if (http.ExportResult && http.ExportDirectory != null)
            //{
            //    string xmlExportPath = http.GetNewHttpFileName(http.ExportDirectory, ".xml");
            //    xml.Save(xmlExportPath);
            //}

            if (_exportXml)
            {
                string path = httpResult.Http.HttpRequest.UrlCachePath?.Path;
                if (path != null)
                {
                    //string ext = zPath.GetExtension(path);
                    path += ".xml";
                    if (zFile.Exists(path))
                        zFile.Delete(path);
                    xml.Save(path);
                }
            }

            return xml;
        }
    }
}

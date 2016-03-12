using System.IO;
using System.Xml.Linq;

namespace pb.Web
{
    public static partial class GlobalExtension
    {
        public static XDocument zGetXDocument(this Http http)
        {
            //if (Http_new.Trace)
            //    pb.old.Trace.WriteLine("GlobalExtension.zGetXmlDocument()");
            XDocument xml = null;
            //Http_new http = loadDataFromWeb.Http;
            if (http.ResultContentType == "text/html")
            {
                HtmlToXml hx = new HtmlToXml(new StringReader(http.ResultText));
                //hx.ReadCommentInText = _readCommentInText;
                xml = hx.GenerateXDocument();
            }
            else if (http.ResultContentType == "text/xml")
            {
                xml = XDocument.Parse(http.ResultText, LoadOptions.PreserveWhitespace);
            }
            else
                throw new PBException("Error can't transform http content \"{0}\" to xml", http.ResultContentType);

            if (http.ExportResult && http.ExportDirectory != null)
            {
                string xmlExportPath = http.GetNewHttpFileName(http.ExportDirectory, ".xml");
                xml.Save(xmlExportPath);
            }
            //else if (_xmlExportPath != null)
            //{
            //    if (zpath.PathGetExtension(_textExportPath) == "")
            //        _xmlExportPath = zpath.PathSetExtension(_textExportPath, ".xml");
            //}
            return xml;
        }
    }
}

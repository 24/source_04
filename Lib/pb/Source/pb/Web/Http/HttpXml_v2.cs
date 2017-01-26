using pb.Web.Html;
using System.IO;
using System.Xml.Linq;

namespace pb.Web.Http
{
    public static partial class HttpExtension
    {
        public static XDocument zGetXDocument(this HttpResult<string> httpResult)
        {
            XDocument xml = null;
            if (httpResult.Http.ResultContentType == "text/html")
            {
                //HtmlToXml_v2 hx = new HtmlToXml_v2(new StringReader(httpResult.Data));
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
            return xml;
        }
    }
}

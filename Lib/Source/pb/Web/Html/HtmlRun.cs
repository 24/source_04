using System;
using System.Data;
using System.Xml.Linq;
using pb.Data.Xml;

namespace pb.Web
{
    public static class HtmlRun
    {
        //private static XmlDocumentSourceType __xmlDocumentSourceType = XmlDocumentSourceType.NoSource;

        // http
        //private static HttpManager __httpManager = new HttpManager();
        //private static Http_new __http = null;
        //private static XDocument __xDocument = null;

        // xml file
        //private static string __xmlFile = null;

        // html
        //private static string __html = null;

        public static Action<DataTable> SetResult = null;

        public static DataTable Select(string xpath, params string[] values)
        {
            return _Select(HttpRun.GetXDocument(), xpath, values);
        }

        private static DataTable _Select(XNode node, string xpath, params string[] values)
        {
            if (node == null)
            {
                _SetResult(null);
                return null;
            }
            //if (XmlDocument == null) throw new HtmlXmlReaderException("error no xml data loaded");
            //HtmlXmlTables t = null;
            //if (_nodePathWithTableCode)
            //{
            //    if (_htmlXmlTables == null) _htmlXmlTables = HtmlXml.GetTables(XmlDocument);
            //    t = _htmlXmlTables;
            //}
            if (values.Length == 0)
                values = new string[] { ":.:NodeValue" };

            //XmlSelect select = pb.old.Xml.Select(node, new XmlSelectParameters(t, _url, _traceFunction), xpath, values);
            if (xpath == null)
                return null;

            XmlSelect xmlSelect = new XmlSelect();
            xmlSelect.SourceNode = node;
            xmlSelect.SelectPrm = new XmlSelectParameters();
            xmlSelect.SourceXPathNode = xpath;
            xmlSelect.SourceXPathValues = values;

            //DataTable dt = pb.old.Xml.ReadSelect(select);
            DataTable dt = xmlSelect.zToDataTable();

            _SetResult(dt);
            return dt;
        }

        private static void _SetResult(DataTable table)
        {
            if (SetResult != null)
                SetResult(table);
        }
    }
}

using pb.Data.Xml;
using pb.Web.Http;
using System;
using System.Data;
using System.Xml.Linq;

namespace pb.Web.Html
{
    public static partial class HtmlRun
    {
        public static Action<DataTable> SetResult = null;

        public static DataTable Select(string xpath, params string[] values)
        {
            return _Select(HttpRun.GetXDocument(), xpath, values);
        }

        private static DataTable _Select(XNode node, string xpath, params string[] values)
        {
            if (node == null)
            {
                Trace.WriteLine("warning no xml");
                //_SetResult(null);
                return null;
            }
            if (values.Length == 0)
                values = new string[] { ":.:NodeValue" };

            //XmlSelect select = pb.old.Xml.Select(node, new XmlSelectParameters(t, _url, _traceFunction), xpath, values);
            if (xpath == null)
            {
                Trace.WriteLine("warning no xpath");
                return null;
            }

            XmlSelect xmlSelect = new XmlSelect();
            xmlSelect.SourceNode = node;
            xmlSelect.SelectPrm = new XmlSelectParameters();
            xmlSelect.SourceXPathNode = xpath;
            xmlSelect.SourceXPathValues = values;

            //DataTable dt = pb.old.Xml.ReadSelect(select);
            DataTable dt = xmlSelect.zToDataTable();
            Trace.WriteLine("found {0} elements", dt.Rows.Count);

            _SetResult(dt);
            return dt;
        }

        private static void _SetResult(DataTable table)
        {
            SetResult?.Invoke(table);
        }
    }
}

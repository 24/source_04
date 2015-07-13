using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;
using pb.Data.Xml;
using pb.Web;

namespace Test.Test_Xml
{
    public static class Test_Xml_f
    {
        public static void Test_Select_01()
        {
            string xpath1 = "//ol[@id='forums']";
            string xpath2 = ".//text()";

            Trace.WriteLine("HttpRun.GetXDocument()");
            XDocument doc = HttpRun.GetXDocument();
            if (doc == null)
            {
                Trace.WriteLine("XDocument is null");
                return;
            }
            XNode sourceNode = doc.Root;

            Trace.WriteLine("xpath1 \"{0}\"", xpath1);
            //XObject[] nodes = sourceNode.zXPath(xpath1).ToArray();
            object o = sourceNode.XPathEvaluate(xpath1);
            if (o == null)
            {
                Trace.WriteLine("result is null");
                return;
            }
            Trace.WriteLine("result : \"{0}\"", o.GetType().zGetTypeName());
            if (!(o is IEnumerable))
            {
                Trace.WriteLine("result is not IEnumerable");
                return;
            }

            IEnumerator enumerator = ((IEnumerable)o).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                Trace.WriteLine("no value in result");
                return;
            }

            o = enumerator.Current;
            Trace.WriteLine("first value : \"{0}\"", o.GetType().zGetTypeName());

            if (!(o is XObject))
            {
                Trace.WriteLine("first value is not XObject");
                return;
            }
            XObject node = (XObject)o;

            if (!(node is XNode))
            {
                Trace.WriteLine("first value is not XNode");
                return;
            }

            //IEnumerable<XObject> nodes2 = ((XNode)node).zXPath(xpath2);
            Trace.WriteLine("xpath2 \"{0}\"", xpath2);
            o = ((XNode)node).XPathEvaluate(xpath2);
            if (o== null)
            {
                Trace.WriteLine("result is null");
                return;
            }
            Trace.WriteLine("result : \"{0}\"", o.GetType().zGetTypeName());
            if (!(o is IEnumerable))
            {
                Trace.WriteLine("result is not IEnumerable");
                return;
            }

            enumerator = ((IEnumerable)o).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                Trace.WriteLine("no value in result");
                return;
            }

            o = enumerator.Current;
            Trace.WriteLine("first value : \"{0}\"", o.GetType().zGetTypeName());
        }

        public static void Test_Select_02()
        {
            string xpath1 = "//ol[@id='forums']";
            string xpath2 = ".//text()";

            Trace.WriteLine("HttpRun.GetXDocument()");
            XNode sourceNode = HttpRun.GetXDocument();
            if (sourceNode == null)
            {
                Trace.WriteLine("XDocument is null");
                return;
            }
            //XNode sourceNode = doc.Root;

            Trace.WriteLine("xpath1 \"{0}\"", xpath1);
            XObject[] nodes = sourceNode.zXPathNodes(xpath1).ToArray();
            Trace.WriteLine("result : {0} XObject", nodes.Length);

            if (nodes.Length == 0)
            {
                Trace.WriteLine("no node in result");
                return;
            }
            XObject node = nodes[0];

            if (!(node is XNode))
            {
                Trace.WriteLine("first node is not XNode");
                return;
            }

            Trace.WriteLine("xpath2 \"{0}\"", xpath2);
            IEnumerable<XObject> nodes2 = ((XNode)node).zXPathNodes(xpath2);

            Trace.WriteLine("result :");
            foreach (XObject node3 in nodes2)
            {
                Trace.WriteLine("node \"{0}\"", node3.GetType().zGetTypeName());
            }
        }
    }
}

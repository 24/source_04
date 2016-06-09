using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace pb.Data.Xml
{
    public class XXElement
    {
        private static int _traceLevel = 1;
        private XElement _xelement = null;
        private string _xpath = null;

        public XXElement(XElement xelement, string xpath = "/")
        {
            _xelement = xelement;
            _xpath = xpath;
        }

        public static int TraceLevel { get { return _traceLevel; } }
        public XElement XElement { get { return _xelement; } }
        public string XPath { get { return _xpath; } }

        public string GetPath()
        {
            if (_xelement != null)
                return _xelement.zGetPath();
            else
                return null;
        }

        public string ExplicitXPathValue(string xpath)
        {
            return _XPathValue(xpath, true);
        }

        public string XPathValue(string xpath)
        {
            return _XPathValue(xpath, false);
        }

        private string _XPathValue(string xpath, bool xplicit)
        {
            string value = null;
            if (_xelement != null)
            {
                value = _xelement.zXPathValue(xpath);
                if (value != null)
                    WriteLine(2, "get value \"{0}\" from element \"{1}\" (\"{2}\")", xpath, _xpath, value);
                else if (xplicit)
                    WriteLine(1, "error value not found \"{0}\" from element \"{1}\"", xpath, _xpath);
            }
            else if (xplicit)
                WriteLine(1, "error value not found \"{0}\" from null element \"{1}\"", xpath, _xpath);
            return value;
        }

        public IEnumerable<string> XPathValues(string xpath)
        {
            //string[] values = null;
            IEnumerable<string> values = null;
            if (_xelement != null)
                //return _xelement.zXPathValues(xpath);
                values = _xelement.zXPathValues(xpath);
            else
                values = new string[0];
            WriteLine(2, "get values \"{0}\" from element \"{1}\" (\"{2}\")", xpath, _xpath, values.zToStringValues());
            return values;
        }

        public string ExplicitAttribValue(XName attribName, string defaultValue = null)
        {
            return _AttribValue(attribName, true);
        }

        public string AttribValue(XName attribName, string defaultValue = null)
        {
            string value = _AttribValue(attribName, false);
            if (value != null)
                return value;
            else
                return defaultValue;
        }

        private string _AttribValue(XName attribName, bool xplicit)
        {
            string value = null;
            if (_xelement != null)
            {
                value = _xelement.zAttribValue(attribName);
                if (value != null)
                    WriteLine(2, "get attrib \"{0}\" from element \"{1}\" (\"{2}\")", attribName, _xpath, value);
                else if (xplicit)
                    WriteLine(1, "error attrib not found \"{0}\" from element \"{1}\"", attribName, _xpath);
            }
            else if (xplicit)
                WriteLine(1, "error attrib not found \"{0}\" from null element \"{1}\"", attribName, _xpath);
            return value;
        }

        public XXElement XPathElement(string xpath, bool writeError = true)
        {
            XElement xe = null;
            if (_xelement != null)
            {
                xe = _xelement.XPathSelectElement(xpath);
                if (xe == null && writeError)
                    WriteLine(1, "error element not found xpath \"{0}\" from element \"{1}\"", xpath, _xpath);
            }
            WriteLine(2, "select element \"{0}\" from element \"{1}\" ({2})", xpath, _xpath, xe != null ? "found " + xe.zGetPath() : "not found");
            return new XXElement(xe, CombineXPath(_xpath, xpath));
        }

        public IEnumerable<XXElement> XPathElements(string xpath)
        {
            WriteLine(2, "select elements \"{0}\" from element \"{1}\"", xpath, _xpath);
            if (_xelement != null)
            {
                //return new XXElements(_xelement.XPathSelectElements(xpath), CombineXPath(_xpath, xpath), elementFilter);
                return _xelement.zXPathElements(xpath).Select(xe => new XXElement(xe, CombineXPath(_xpath, xpath)));
            }
            return new XXElement[0];
        }

        public IEnumerable<XNode> DescendantNodes(Func<XNode, XNodeFilter> filter = null)
        {
            if (_xelement != null)
                return _xelement.zDescendantNodes(filter);
            else
                return new XNode[0];
        }

        public IEnumerable<XNodeInfo> DescendantNodesInfos(Func<XNode, XNodeFilter> filter = null)
        {
            if (_xelement != null)
                return _xelement.zDescendantNodesInfos(filter);
            else
                return new XNodeInfo[0];
        }

        public IEnumerable<string> DescendantTexts(Func<XNode, XNodeFilter> filter = null)
        {
            if (_xelement != null)
                return _xelement.zDescendantTexts(filter);
            else
                return new string[0];
        }

        public IEnumerable<XText> DescendantTextNodes(Func<XNode, XNodeFilter> filter = null)
        {
            if (_xelement != null)
                return _xelement.zDescendantTextNodes(filter);
            else
                return new XText[0];
        }

        public IEnumerable<XXElement> DescendantFormItems(Func<XNode, XNodeFilter> filter = null)
        {
            if (_xelement != null)
                return _xelement.zDescendantFormItems(filter).Select(xe => new XXElement(xe));
            else
                return new XXElement[0];
        }

        private static string CombineXPath(string xpath1, string xpath2)
        {
            if (xpath2 == null)
                return xpath1;
            else if (xpath1 == null || xpath2.StartsWith("/"))
                return xpath2;
            else if (xpath2.StartsWith("./"))
                return xpath1 + xpath2.Substring(1);
            else
                return xpath1 + "/" + xpath2;
        }

        // c# inline function
        // http://stackoverflow.com/questions/473782/inline-functions-in-c
        // Finally in .NET 4.5, the CLR allows one to force method inlining using MethodImplOptions.AggressiveInlining value. It is also available in the Mono's trunk (committed today).
        // using System.Runtime.CompilerServices;
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _traceLevel)
                Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public static partial class GlobalExtension
    {
        public static XXElement zXXElement(this XDocument xdocument)
        {
            if (xdocument != null)
                return new XXElement(xdocument.Root);
            else
                return new XXElement(null);
        }

        public static XXElement zXXElement(this XElement xelement)
        {
            return new XXElement(xelement);
        }

        public static IEnumerable<XElement> XElements(this IEnumerable<XXElement> xxelements)
        {
            foreach (XXElement xxelement in xxelements)
            {
                yield return xxelement.XElement;
            }
        }

        public static IEnumerable<XNode> DescendantNodes(this IEnumerable<XXElement> xxelements, Func<XNode, XNodeFilter> filter = null)
        {
            return xxelements.XElements().zDescendantNodes(filter);
        }

        public static IEnumerable<XNodeInfo> DescendantNodesInfos(this IEnumerable<XXElement> xxelements, Func<XNode, XNodeFilter> filter = null)
        {
            return xxelements.XElements().zDescendantNodesInfos(filter);
        }

        public static IEnumerable<string> DescendantTexts(this IEnumerable<XXElement> xxelements, Func<XNode, XNodeFilter> filter = null)
        {
            return xxelements.XElements().zDescendantTexts(filter);
        }

        public static IEnumerable<XText> DescendantTextNodes(this IEnumerable<XXElement> xxelements, Func<XNode, XNodeFilter> filter = null)
        {
            return xxelements.XElements().zDescendantTextNodes(filter);
        }

        public static IEnumerable<XXElement> zFilterElements(this IEnumerable<XXElement> elements, Func<XXElement, XNodeFilter> filter)
        {
            foreach (XXElement element in elements)
            {
                XNodeFilter xNodeFilter = filter(element);
                if ((xNodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                    break;
                if ((xNodeFilter & XNodeFilter.DontSelectNode) != XNodeFilter.DontSelectNode)
                    yield return element;
            }
        }
    }
}

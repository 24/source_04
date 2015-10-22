using System;
using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Data.Xml;

// for Gesat and Handeco

namespace Download
{
    public static class XmlExtension
    {
        public static string XPathConcatText(this XXElement xxelement, string xpath, string separator = null, Func<string, string> resultFunc = null, Func<string, string> itemFunc = null)
        {
            return _XPathConcatText(xxelement, xpath, separator, false, resultFunc, itemFunc);
        }

        private static string _XPathConcatText(XXElement xxelement, string xpath, string separator, bool xplicit, Func<string, string> resultFunc = null, Func<string, string> itemFunc = null)
        {
            string value = null;
            XElement xelement = xxelement.XElement;
            if (xelement != null)
            {
                object o = xelement.XPathEvaluate(xpath);
                if (XPathResultConcatText(o, out value, separator, resultFunc, itemFunc))
                    XXElement.WriteLine(2, "get value \"{0}\" from element \"{1}\" (\"{2}\")", xpath, xxelement.XPath, value);
                else if (xplicit)
                    XXElement.WriteLine(1, "error value not found \"{0}\" from element \"{1}\"", xpath, xxelement.XPath);
            }
            else if (xplicit)
                XXElement.WriteLine(1, "error value not found \"{0}\" from null element \"{1}\"", xpath, xxelement.XPath);
            return value;
        }

        private static bool XPathResultConcatText(object o, out string value, string separator = null, Func<string, string> resultFunc = null, Func<string, string> itemFunc = null)
        {
            value = null;
            if (o is IEnumerable)
            {
                bool found = false;
                string concat = "";
                foreach (object v in (IEnumerable)o)
                {
                    if (v is XText)
                    {
                        found = true;
                        if (concat != "" && separator != null)
                            concat += separator;
                        string item = (v as XText).Value;
                        if (itemFunc != null)
                            item = itemFunc(item);
                        concat += item;
                    }
                    if (resultFunc != null)
                        concat = resultFunc(concat);
                    value = concat;
                }
                return found;
            }
            else if (o is XText)
                value = (o as XText).Value;
            else
                return false;
            return true;
        }
    }
}

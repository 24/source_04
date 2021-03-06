﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Reflection;

namespace pb.Data.Xml
{
    public static class zxml
    {
        //public static string XPathValue(XElement xe, string xpath, string defaultValue = null)
        public static string XPathValue(XNode node, string xpath, string defaultValue = null, bool acceptCData = false)
        {
            if (node == null)
                return defaultValue;

            object xpathResult = node.XPathEvaluate(xpath);
            if (xpathResult is IEnumerable)
            {
                object xpathResult2 = XPathResultGetFirstValue(xpathResult as IEnumerable);
                if (xpathResult2 != null)
                    xpathResult = xpathResult2;
            }
            string value = XPathResultGetValue(xpathResult, acceptCData);
            if (value != null)
                return value;
            else
                return defaultValue;
        }

        //public static IEnumerable<string> XPathValues(XElement xe, string xpath)
        public static IEnumerable<string> XPathValues(XNode node, string xpath)
        {
            if (node == null)
                return new string[0];

            return XPathResultGetValues(node.XPathEvaluate(xpath));
        }

        //public static XElement XPathElement(XElement xe, string xpath)
        public static XElement XPathElement(XNode node, string xpath)
        {
            if (node == null)
                return null;

            object xpathResult = node.XPathEvaluate(xpath);
            if (xpathResult is IEnumerable)
            {
                object xpathResult2 = XPathResultGetFirstValue(xpathResult as IEnumerable);
                if (xpathResult2 != null)
                    xpathResult = xpathResult2;
            }
            if (xpathResult is XElement)
                return (XElement)xpathResult;
            else
                return null;
        }

        // "\$([_a-z][_a-z0-9/]*)\$"
        private static Regex __rgVariables = new Regex(@"\$([_a-z0-9@\[\]/]+)\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // get variable "$Ebookdz/DataDir$" value or "$//Root$"
        // function getValue() return constant value like AppDirectory
        //public static string zGetVariableValue(this XElement xe, string value, Func<string, string> getValue = null)
        //{
        //    if (xe == null || value == null)
        //        return null;
        //    return __rgVariables.Replace(value, new MatchEvaluator(
        //        match =>
        //        {
        //            string xpath = match.Groups[1].Value;
        //            XElement parent = xe.Parent;
        //            string var = null;
        //            if (getValue != null)
        //                var = getValue(xpath);
        //            if (var == null && parent != null && xpath != xe.Name.LocalName)
        //                var = parent.zXPathValue(xpath);
        //            if (var == null)
        //                var = xe.Document.Root.zXPathValue(xpath);
        //            if (var == null)
        //                pb.Trace.WriteLine("can't find xml variable \"${0}$\" from {1}", xpath, xe.zGetPath());
        //            return var;
        //        }));
        //}

        public static string zGetVariableValue(this XElement xe, string value, Func<string, string> getValue = null, bool traceError = false)
        {
            string newValue;
            xe.zTryGetVariableValue(value, out newValue, getValue, traceError: traceError);
            return newValue;
        }

        public static bool zTryGetVariableValue(this XElement xe, string value, out string newValue, Func<string, string> getValue = null, bool traceError = false)
        {
            //if (xe == null || value == null)
            if (value == null)
            {
                newValue = null;
                return true;
            }
            bool valueNotFound = false;
            newValue = __rgVariables.Replace(value, new MatchEvaluator(
                match =>
                {
                    string xpath = match.Groups[1].Value;
                    //XElement parent = xe.Parent;
                    //string var = null;
                    string var;
                    if (getValue != null)
                    {
                        var = getValue(xpath);
                        if (var != null)
                            return var;
                    }
                    //if (var == null && parent != null && xpath != xe.Name.LocalName)
                    if (xe != null)
                    {
                        XElement parent = xe.Parent;
                        if (parent != null && xpath != xe.Name.LocalName)
                        {
                            var = parent.zXPathValue(xpath);
                            if (var != null)
                                return var;
                        }
                        var = xe.Document.Root.zXPathValue(xpath);
                        if (var != null)
                            return var;
                    }
                    valueNotFound = true;
                    if (traceError)
                        pb.Trace.WriteLine("can't find xml variable \"${0}$\" from {1}", xpath, xe.zGetPath());
                    return null;
                }));
            return !valueNotFound;
        }

        public static void zSetAllVariablesValues(this XDocument xdocument, Func<string, string> getValue = null)
        {
            if (xdocument == null)
                return;
            foreach (XElement xe in xdocument.Root.DescendantsAndSelf())
            {
                foreach (XAttribute xa in xe.Attributes())
                    xa.Value = xe.zGetVariableValue(xa.Value, getValue, traceError: true);
            }
        }

        //private static bool XPathResultGetValue(object o, out string value)
        //{
        //    value = null;
        //    if (o is IEnumerable)
        //    {
        //        IEnumerator e = (o as IEnumerable).GetEnumerator();
        //        if (!e.MoveNext())
        //            return false;
        //        o = e.Current;
        //    }
        //    if (o is string)
        //        value = o as string;
        //    else if (o is XAttribute)
        //        value = (o as XAttribute).Value;
        //    // modif le 09/02/2015
        //    //else if (o is XElement)
        //    //    value = (o as XElement).zTextOrAttribValue();
        //    else if (o is XText)
        //        value = (o as XText).Value;
        //    else
        //        return false;
        //    return true;
        //}

        //private static string XPathResultGetValue(object xpathResult)
        //{
        //    if (xpathResult is IEnumerable)
        //    {
        //        object xpathResult2 = EnumGetFirstValue(xpathResult as IEnumerable);
        //        if (xpathResult2 != null)
        //            xpathResult = xpathResult2;
        //    }
        //    return GetValue(xpathResult);
        //}

        private static object XPathResultGetFirstValue(IEnumerable values)
        {
            IEnumerator enumerator = values.GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current;
            else
                return null;
        }

        private static string XPathResultGetValue(object xpathResult, bool acceptCData = false)
        {
            if (xpathResult is string)
                return xpathResult as string;
            else if (xpathResult is XAttribute)
                return (xpathResult as XAttribute).Value;
            // modif le 09/02/2015
            else if (xpathResult is XText)
                return (xpathResult as XText).Value;
            else if (xpathResult is XElement)
            {
                //value = (xpathResult as XElement).zTextOrAttribValue();
                XElement xe = (XElement)xpathResult;
                XAttribute attrib = xe.Attribute("value");
                if (attrib != null)
                    return attrib.Value;
                if (acceptCData)
                {
                    XCData cdata = GetUniqueCDataChild(xe);
                    if (cdata != null)
                        return cdata.Value;
                }
            }
            return null;
        }

        private static XCData GetUniqueCDataChild(XElement xe)
        {
            XCData cdata = null;
            foreach (XNode node in xe.Nodes())
            {
                if (node is XCData)
                {
                    // if multiple cdata return null
                    if (cdata != null)
                        return null;
                    cdata = (XCData)node;
                }
            }
            return cdata;
        }

        //private static string[] zXPathValues(object o)
        //{
        //    List<string> sValues = new List<string>();
        //    if (o is IEnumerable)
        //    {
        //        foreach (object o2 in o as IEnumerable)
        //        {
        //            string s = null;
        //            if (o2 is string)
        //                s = o2 as string;
        //            if (o2 is XAttribute)
        //                s = (o2 as XAttribute).Value;
        //            if (o2 is XElement)
        //                s = (o2 as XElement).zTextOrAttribValue();
        //            if (s != null) sValues.Add(s);
        //        }
        //    }
        //    else
        //    {
        //        string s = null;
        //        if (o is string)
        //            s = o as string;
        //        if (o is XAttribute)
        //            s = (o as XAttribute).Value;
        //        if (o is XElement)
        //            s = (o as XElement).zTextOrAttribValue();
        //        if (s != null) sValues.Add(s);
        //    }
        //    return sValues.ToArray();
        //}

        private static IEnumerable<string> XPathResultGetValues(object xpathResult)
        {
            //List<string> sValues = new List<string>();
            if (xpathResult is IEnumerable)
            {
                foreach (object value in xpathResult as IEnumerable)
                {
                    //string s = null;
                    //if (o2 is string)
                    //    s = o2 as string;
                    //if (o2 is XAttribute)
                    //    s = (o2 as XAttribute).Value;
                    //if (o2 is XElement)
                    //    s = (o2 as XElement).zTextOrAttribValue();
                    //if (s != null) sValues.Add(s);
                    yield return XPathResultGetValue(value);
                }
            }
            else
            {
                //string s = null;
                //if (o is string)
                //    s = o as string;
                //if (o is XAttribute)
                //    s = (o as XAttribute).Value;
                //if (o is XElement)
                //    s = (o as XElement).zTextOrAttribValue();
                //if (s != null) sValues.Add(s);
                yield return XPathResultGetValue(xpathResult);
            }
            //return sValues.ToArray();
        }
    }

    public static partial class GlobalExtension
    {
        //public static string zXPathValue(this XElement xe, string xpath, string defaultValue = null)
        public static string zXPathValue(this XNode node, string xpath, string defaultValue = null, bool acceptCData = false)
        {
            return zxml.XPathValue(node, xpath, defaultValue, acceptCData);
        }

        //public static string zXPathExplicitValue(this XElement xe, string xpath)
        public static string zXPathExplicitValue(this XNode node, string xpath)
        {
            if (node == null)
                throw new Exception(string.Format("XNode is null xpath \"{0}\" not found", xpath));

            string value = zxml.XPathValue(node, xpath);
            if (value == null)
                throw new PBException("xpath value not found, xpath \"{0}\" XNode \"{1}\"", xpath, node.zGetPath());
            return value;
        }

        //public static IEnumerable<string> zXPathValues(this XElement xe, string xpath)
        public static IEnumerable<string> zXPathValues(this XNode node, string xpath)
        {
            return zxml.XPathValues(node, xpath);
        }

        //public static XElement zXPathElement(this XElement xe, string xpath)
        public static XElement zXPathElement(this XNode node, string xpath)
        {
            return zxml.XPathElement(node, xpath);
        }

        //public static IEnumerable<XElement> zXPathElements(this XElement xe, string xpath)
        public static IEnumerable<XElement> zXPathElements(this XNode node, string xpath)
        {
            if (node != null)
                return node.XPathSelectElements(xpath);
            else
                return new XElement[0];
        }

        public static IEnumerable<XObject> zXPathNodes(this XNode node, string xpath)
        {
            foreach (object o in node.XPathEvaluate(xpath) as IEnumerable)
            {
                yield return (XObject)o;
            }
        }

        public static string zAttribValue(this XElement xe, XName attribName, string defaultValue = null)
        {
            string value = defaultValue;
            if (xe != null && attribName != null)
            {
                XAttribute attrib = xe.Attribute(attribName);
                if (attrib != null)
                    value = attrib.Value;
            }
            return value;
        }

        public static string zAttribValue(this XObject xobject, XName attribName, string defaultValue = null)
        {
            if (xobject is XElement)
                return ((XElement)xobject).zAttribValue(attribName, defaultValue);
            throw new PBException("no attrib on XObject type {0}", xobject.GetType().zGetTypeName());
        }

        public static string zExplicitAttribValue(this XElement xe, XName attribute)
        {
            if (xe == null)
                throw new Exception(string.Format("XElement is null attribute \"{0}\" not found", attribute));
            if (attribute == null)
                throw new Exception(string.Format("attribute name is null"));
            XAttribute xattribute = xe.Attribute(attribute);
            if (xattribute == null)
                throw new Exception(string.Format("attribute \"{0}\" not found", attribute));
            return xattribute.Value;
        }

        // used in XmlSelect
        public static string zGetValue(this XObject xobject)
        {
            if (xobject is XElement)
                return ((XElement)xobject).Value;
            else if (xobject is XAttribute)
                return ((XAttribute)xobject).Value;
            else if (xobject is XText)
                return ((XText)xobject).Value;
            else if (xobject is XComment)
                return ((XComment)xobject).Value;
            else
                return null;
        }

        public static string zGetName(this XObject xobject)
        {
            if (xobject is XElement)
                return ((XElement)xobject).Name.LocalName;
            else if (xobject is XAttribute)
                return ((XAttribute)xobject).Name.LocalName;
            else
                return null;
        }

        public static string zGetPath(this XObject xobject)
        {
            if (xobject is XElement)
                return ((XElement)xobject).zGetPath();
            if (xobject is XAttribute)
                return xobject.Parent.zGetPath() + "/@" + ((XAttribute)xobject).Name.LocalName;
            if (xobject is XComment)
                return xobject.Parent.zGetPath() + "/comment()";
            if (xobject is XText)
                return xobject.Parent.zGetPath() + "/text()";
            if (xobject is XDocument)
                return "/";
            if (xobject is XDocumentType)
                return "/DocumentType()";
            if (xobject is XProcessingInstruction)
                return "/ProcessingInstruction()";
            throw new PBException("unknow XNode type {0}", xobject.GetType().zGetTypeName());
        }

        public static string zGetPath(this XElement xe)
        {
            if (xe == null)
                return null;
            string path = "";
            while (xe != null)
            {
                path = "/" + xe.Name + "[" + (xe.zGetElementIndex() + 1).ToString() + "]" + path;
                xe = xe.Parent;
            }
            return path;
        }

        public static string zToString(this XObject xobject, bool attrib = false, bool close = false)
        {
            if (xobject is XElement)
            {
                if (close)
                    return "</" + ((XElement)xobject).Name.LocalName + ">";
                else if (attrib)
                {
                    XElement xe = (XElement)xobject;
                    string s = "<" + xe.Name.LocalName;
                    foreach (XAttribute attribute in xe.Attributes())
                        s += " " + attribute.Name.LocalName + "=" + attribute.Value;
                    s += ">";
                    return s;
                }
                else
                    return "<" + ((XElement)xobject).Name.LocalName + ">";
            }
            else if (xobject is XAttribute)
            {
                XAttribute attribute = (XAttribute)xobject;
                return attribute.Name.LocalName + "=" + attribute.Value;
            }
            else if (xobject is XText)
            {
                return "\"" + ((XText)xobject).Value + "\"";
            }
            else
                return null;
        }

        // used in zGetPath()
        public static int zGetElementIndex(this XElement element)
        {
            if (element == null)
                return 0;
            return element.ElementsBeforeSelf(element.Name).Count();
        }

        public static string zGetPath(this XAttribute xa)
        {
            if (xa == null)
                return null;
            //if (xa is pb.old.XLAttribute)
            //    return ((XLAttribute)xa).SourceAttribute.Parent.zGetPath() + "/@" + xa.Name;
            else
                return xa.Parent.zGetPath() + "/@" + xa.Name;
        }
    }
}

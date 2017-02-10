using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;

namespace pb.old
{

    /// <summary>
    /// XAttribute + SourceAttribute
    /// </summary>
    //public class XLAttribute : XAttribute
    //{
    //    public XAttribute SourceAttribute;

    //    public XLAttribute(XAttribute a) : base(a)
    //    {
    //        SourceAttribute = a;
    //    }
    //}

    //public static partial class XmlExtension
    //{
        // => pb.Data.Xml.zxml :
        //   public static string zXPathValue(this XElement xe, string xpath, string defaultValue = null)
        //   public static string[] zXPathValues(this XElement xe, string sXPath)
        //   public static XElement zXPathElement(this XElement xe, string sXPath)
        //   public static string zAttribValue(this XElement xe, XName attribName, string defaultValue = null)
        //   public static string zAttribValue(this XObject xobject, XName attribName, string defaultValue = null)
        //   public static int zGetElementIndex(this XElement element)
        //   public static string zGetPath(this XObject xobject)
        //   public static string zGetPath(this XElement xe)
        //   public static string zGetPath(this XAttribute xa)
        //   public static string zGetVariableValue(this XElement xe, string value, Func<string, string> getValue = null)

        //public static string zTextOrAttribValue(this XElement xe)
        //{
        //    if (xe == null) return null;
        //    foreach (XNode node in xe.Nodes())
        //    {
        //        if (node is XText) return ((XText)node).Value;
        //    }
        //    foreach (XAttribute attrib in xe.Attributes())
        //        return attrib.Value;
        //    return null;
        //}


        //public static string[] zValueList(this XElement xe)
        //{
        //    return xe.zValueList(SplitOption.None);
        //}

        //public static string[] zValueList(this XElement xe, SplitOption splitOption)
        //{
        //    if (xe == null) return null;
        //    List<string> values = new List<string>();
        //    //foreach (XNode node in xe.DescendantNodes())
        //    //{
        //    //    if (node is XText)
        //    //    {
        //    //foreach (string s in xe.zDescendantTextList())
        //    foreach (string s in xe.zDescendantTexts())
        //    {
        //        //string s = ((XText)node).Value;
        //        if (splitOption != SplitOption.None)
        //        {
        //            string[] ss = zsplit.Split(s, splitOption);
        //            foreach (string s2 in ss) values.Add(s2);
        //        }
        //        else
        //            values.Add(s);
        //    //    }
        //    }
        //    return values.ToArray();
        //}

        //public static IEnumerable<XText> zDescendantTextNodes(this XElement xe)
        //{
        //    return from n in xe.DescendantNodes() where n.NodeType == XmlNodeType.Text select n as XText;
        //}

        //public static IEnumerable<string> zDescendantTextList1(this XElement xe)
        //{
        //    return from n in xe.zDescendantTextNodes() select n.Value;
        //}

        //public static IEnumerable<string> zDescendantTextList(this XElement xe)
        //{
        //    return xe.DescendantNodes().zWhereSelect(SelectText);
        //}

        //public static IEnumerable<string> zDescendantTextList(this XElement element, Predicate<XNode> nodeFilter = null, Func<string, string> func = null)
        //{
        //    //return from xtext in new XnodeDescendants(element, nodeFilter, node => node is XText) select ((XText)xtext).Value;
        //    if (func != null)
        //        return from xtext in new XNodeDescendants(element, nodeFilter, node => node is XText) select func(((XText)xtext).Value);
        //    else
        //        return from xtext in new XNodeDescendants(element, nodeFilter, node => node is XText) select ((XText)xtext).Value;
        //}

        //public static string SelectText(XNode node)
        //{
        //    if (node.NodeType == XmlNodeType.Text)
        //        return ((XText)node).Value;
        //    else if (node.NodeType == XmlNodeType.Element && ((XElement)node).Name == "br")
        //        return "\r\n";
        //    else
        //        return null;
        //}

        //public static string zFirstValue(this XElement xe)
        //{
        //    if (xe == null) return null;
        //    return xe.DescendantNodes().zFirstValue(0, StringFormatOption.None);
        //}

        //public static string zFirstValue(this XElement xe, StringFormatOption stringFormatOption)
        //{
        //    if (xe == null) return null;
        //    return xe.DescendantNodes().zFirstValue(0, stringFormatOption);
        //}

        //public static string zFirstValue(this XElement xe, int indexFirstValue)
        //{
        //    if (xe == null) return null;
        //    return xe.DescendantNodes().zFirstValue(indexFirstValue, StringFormatOption.None);
        //}

        //public static string zFirstValue(this XElement xe, int indexFirstValue, StringFormatOption stringFormatOption)
        //{
        //    if (xe == null) return null;
        //    return xe.DescendantNodes().zFirstValue(indexFirstValue, stringFormatOption);
        //}

        //public static string zFirstValue(this XElement xe, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(nodeFilter).zFirstValue(0, StringFormatOption.None);
        //}

        //public static string zFirstValue(this XElement xe, StringFormatOption stringFormatOption, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(nodeFilter).zFirstValue(0, stringFormatOption);
        //}

        //public static string zFirstValue(this XElement xe, int indexFirstValue, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(nodeFilter).zFirstValue(indexFirstValue, StringFormatOption.None);
        //}

        //public static string zFirstValue(this XElement xe, int indexFirstValue, StringFormatOption stringFormatOption, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(nodeFilter).zFirstValue(indexFirstValue, stringFormatOption);
        //}

        //public static string zFirstValue(this XElement xe, params XName[] excludeElements)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(excludeElements).zFirstValue(0, StringFormatOption.None);
        //}

        //public static string zFirstValue(this XElement xe, StringFormatOption stringFormatOption, params XName[] excludeElements)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(excludeElements).zFirstValue(0, stringFormatOption);
        //}

        //public static string zFirstValue(this XElement xe, int indexFirstValue, params XName[] excludeElements)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(excludeElements).zFirstValue(indexFirstValue, StringFormatOption.None);
        //}

        //public static string zFirstValue(this XElement xe, int indexFirstValue, StringFormatOption stringFormatOption, params XName[] excludeElements)
        //{
        //    if (xe == null) return null;
        //    return xe.zDescendantNodes(excludeElements).zFirstValue(indexFirstValue, stringFormatOption);
        //}

        //private static string zFirstValue(this IEnumerable<XNode> enumNode, int indexFirstValue, StringFormatOption stringFormatOption)
        //{
        //    foreach (XNode node in enumNode)
        //    {
        //        if (node is XText)
        //        {
        //            if (indexFirstValue-- == 0)
        //            {
        //                //return ((XText)node).Value;
        //                return ((XText)node).Value.zFormat(stringFormatOption);
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.DescendantNodes().zFirstValues(nbValues, 0, StringFormatOption.None);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, StringFormatOption stringFormatOption)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.DescendantNodes().zFirstValues(nbValues, 0, stringFormatOption);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, int indexFirstValue)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.DescendantNodes().zFirstValues(nbValues, indexFirstValue, StringFormatOption.None);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, int indexFirstValue, StringFormatOption stringFormatOption)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.DescendantNodes().zFirstValues(nbValues, indexFirstValue, stringFormatOption);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(nodeFilter).zFirstValues(nbValues, 0, StringFormatOption.None);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, StringFormatOption stringFormatOption, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(nodeFilter).zFirstValues(nbValues, 0, stringFormatOption);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, int indexFirstValue, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(nodeFilter).zFirstValues(nbValues, indexFirstValue, StringFormatOption.None);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, int indexFirstValue, StringFormatOption stringFormatOption, Func<XNode, bool> nodeFilter)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(nodeFilter).zFirstValues(nbValues, indexFirstValue, stringFormatOption);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, params XName[] excludeElements)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(excludeElements).zFirstValues(nbValues, 0, StringFormatOption.None);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, StringFormatOption stringFormatOption, params XName[] excludeElements)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(excludeElements).zFirstValues(nbValues, 0, stringFormatOption);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, int indexFirstValue, params XName[] excludeElements)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(excludeElements).zFirstValues(nbValues, indexFirstValue, StringFormatOption.None);
        //}

        //public static string[] zFirstValues(this XElement xe, int nbValues, int indexFirstValue, StringFormatOption stringFormatOption, params XName[] excludeElements)
        //{
        //    if (xe == null || nbValues <= 0) return new string[0];
        //    return xe.zDescendantNodes(excludeElements).zFirstValues(nbValues, indexFirstValue, stringFormatOption);
        //}

        //public static string[] zFirstValues(this IEnumerable<XNode> enumNode, int nbValues, int indexFirstValue, StringFormatOption stringFormatOption)
        //{
        //    if (nbValues <= 0) return new string[0];
        //    List<string> values = new List<string>();
        //    foreach (XNode node in enumNode)
        //    {
        //        if (node is XText)
        //        {
        //            if (indexFirstValue == 0)
        //            {
        //                values.Add(((XText)node).Value.zFormat(stringFormatOption));
        //                if (--nbValues == 0) break;
        //            }
        //            else
        //                indexFirstValue--;
        //        }
        //    }
        //    return values.ToArray();
        //}

        //public static IEnumerable<XNode> zDescendantNodes(this XElement xe, Func<XNode, bool> nodeFilter)
        //{
        //    return XmlFilteredNode.DescendantNodes(xe, nodeFilter);
        //}

        //public static IEnumerable<XNode> zDescendantNodes(this XElement xe, params XName[] excludeElements)
        //{
        //    return XmlFilteredNode.DescendantNodes(xe, excludeElements);
        //}

        /// <param name="nodeFilter">filtre les noeuds à parcourir</param>
        /// <param name="returnNodeFilter">sélectionne les noeuds à retourner</param>
        //public static IEnumerable<XNode> zDescendantNodes(this XElement element, Predicate<XNode> nodeFilter = null, Predicate<XNode> returnNodeFilter = null)
        //{
        //    return new XNodeDescendants(element, nodeFilter, returnNodeFilter);
        //}

        //public static bool zHasValue(this XElement xe)
        //{
        //    if (xe == null) return false;
        //    string sValue = xe.Value;
        //    if (sValue == null) return false;
        //    if (sValue.Trim() == "") return false;
        //    return true;
        //}

        //public static string zAttribValue(this XElement xe, XName Attrib)
        //{
        //    return xe.zAttribValue(Attrib, null);
        //}

        //public static string zAttribValue(this XElement xe, XName attribName, string defaultValue = null)
        //{
        //    string value = defaultValue;
        //    if (xe != null && attribName != null)
        //    {
        //        XAttribute attrib = xe.Attribute(attribName);
        //        if (attrib != null)
        //            value = attrib.Value;
        //    }
        //    return value;
        //}

        //public static string zAttribValue(this XObject xobject, XName attribName, string defaultValue = null)
        //{
        //    if (xobject is XElement)
        //        return ((XElement)xobject).zAttribValue(attribName, defaultValue);
        //    throw new PBException("error no attrib on XObject type {0}", xobject.GetType().zGetName());
        //}

        //public static int zAttribValueInt(this XElement xe, XName Attrib, int iDefault)
        //{
        //    return zconvert.Int(xe.zAttribValue(Attrib), iDefault);
        //}

        //public static bool zAttribValueBool(this XElement xe, XName Attrib, bool bDefault)
        //{
        //    return zconvert.Parse(xe.zAttribValue(Attrib), bDefault);
        //}

        //public static string zXPathAttribValue(this XElement xe, string sXPath, XName Attrib)
        //{
        //    return xe.zXPathAttribValue(sXPath, Attrib, null);
        //}

        //public static string zXPathAttribValue(this XElement xe, string sXPath, XName Attrib, string sDefault)
        //{
        //    if (xe == null) return sDefault;
        //    xe = xe.XPathSelectElement(sXPath);
        //    return xe.zAttribValue(Attrib, sDefault);
        //}

        //public static string zXPathAttribValue(this XDocument xd, string sXPath, XName Attrib)
        //{
        //    if (xd == null) return null;
        //    return xd.Root.zXPathAttribValue(sXPath, Attrib, null);
        //}

        //public static string zXPathAttribValue(this XDocument xd, string sXPath, XName Attrib, string sDefault)
        //{
        //    if (xd == null) return sDefault;
        //    return xd.Root.zXPathAttribValue(sXPath, Attrib, sDefault);
        //}

        //public static int zXPathAttribValueInt(this XElement xe, string sXPath, XName Attrib, int iDefault)
        //{
        //    return zconvert.Int(xe.zXPathAttribValue(sXPath, Attrib), iDefault);
        //}

        //public static int zXPathAttribValueInt(this XDocument xd, string sXPath, XName Attrib, int iDefault)
        //{
        //    if (xd == null) return iDefault;
        //    //return cu.Int(xd.zXPathAttribValue(sXPath, Attrib), iDefault);
        //    return xd.Root.zXPathAttribValueInt(sXPath, Attrib, iDefault);
        //}

        //public static bool zXPathAttribValueBool(this XElement xe, string sXPath, XName Attrib, bool bDefault)
        //{
        //    return zconvert.Parse(xe.zXPathAttribValue(sXPath, Attrib), bDefault);
        //}

        //public static bool zXPathAttribValueBool(this XDocument xd, string sXPath, XName Attrib, bool bDefault)
        //{
        //    if (xd == null) return bDefault;
        //    //return cu.Parse(xd.zXPathAttribValue(sXPath, Attrib), bDefault);
        //    return xd.Root.zXPathAttribValueBool(sXPath, Attrib, bDefault);
        //}

        //public static Dictionary<string, string> zAttribs(this XElement xe)
        //{
        //    return xe.Attributes().zAttribs();
        //}

        //public static T zXPathValue<T>(this XElement xe, string xpath)
        //{
        //    T value;
        //    if (xe.zXPathValue<T>(xpath, out value))
        //        return value;
        //    else
        //        return default(T);
        //}

        //public static T zXPathValue<T>(this XElement xe, string xpath, T defaultValue)
        //{
        //    T value;
        //    if (xe.zXPathValue<T>(xpath, out value))
        //        return value;
        //    else
        //        return defaultValue;
        //}

        //public static bool zXPathValue<T>(this XElement xe, string xpath, out T value)
        //{
        //    value = default(T);
        //    if (xe == null) return false;
        //    object o = xe.XPathEvaluate(xpath);
        //    string stringValue;
        //    if (!XPathEvaluateToString(o, out stringValue))
        //        return false;
        //    return stringValue.zStringTo<T>(out value);
        //}

        //public static string zXPathValue(this XElement xe, string sXPath)
        //{
        //    return xe.zXPathValue(sXPath, null);
        //}

        //public static string zXPathValue(this XDocument xd, string sXPath)
        //{
        //    if (xd == null) return null;
        //    return xd.Root.zXPathValue(sXPath, null);
        //}

        //public static string zXPathValue(this XDocument xd, string xpath, string defaultValue = null)
        //{
        //    if (xd != null)
        //        return xd.Root.zXPathValue(xpath, defaultValue);
        //    else
        //        return defaultValue;
        //}

        //public static bool XPathEvaluateToString(object o, out string value)
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
        //    else if (o is XElement)
        //        value = (o as XElement).zTextOrAttribValue();
        //    else if (o is XText)
        //        value = (o as XText).Value;
        //    else
        //        return false;
        //    return true;
        //}

        //public static int zXPathValueInt(this XDocument xd, string sXPath, int iDefault)
        //{
        //    if (xd == null) return iDefault;
        //    return xd.Root.zXPathValueInt(sXPath, iDefault);
        //}

        //public static int zXPathValueInt(this XElement xe, string sXPath, int iDefault)
        //{
        //    return zconvert.Int(xe.zXPathValue(sXPath), iDefault);
        //}

        //public static bool zXPathValueBool(this XDocument xd, string sXPath, bool bDefault)
        //{
        //    if (xd == null) return bDefault;
        //    return xd.Root.zXPathValueBool(sXPath, bDefault);
        //}

        //public static bool zXPathValueBool(this XElement xe, string sXPath, bool bDefault)
        //{
        //    return zconvert.Parse(xe.zXPathValue(sXPath), bDefault);
        //}

        //public static string[] zXPathValues(this XDocument xd, string sXPath)
        //{
        //    if (xd == null) return new string[0];
        //    return zXPathValues(xd.Root.XPathEvaluate(sXPath));
        //}

        //public static string[] zXPathValues(this XElement xe, string sXPath)
        //{
        //    if (xe == null) return new string[0];
        //    return zXPathValues(xe.XPathEvaluate(sXPath));
        //}

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

        //public static object zXPathNode(this XElement xe, string sXPath)
        //{
        //    if (xe == null) return null;
        //    object o = xe.XPathEvaluate(sXPath);
        //    if (o is IEnumerable)
        //    {
        //        IEnumerator e = (o as IEnumerable).GetEnumerator();
        //        if (e.MoveNext())
        //            o = e.Current;
        //        else
        //            o = null;
        //    }
        //    if (o is XAttribute || o is XElement)
        //        return o;
        //    return null;
        //}

        //public static object zXPathNode(this XDocument xd, string sXPath)
        //{
        //    if (xd == null) return null;
        //    return xd.Root.zXPathNode(sXPath);
        //}

        //public static XElement zXPathElement(this XElement xe, string sXPath)
        //{
        //    if (xe == null) return null;
        //    object o = xe.XPathEvaluate(sXPath);
        //    if (o is IEnumerable)
        //    {
        //        IEnumerator e = (o as IEnumerable).GetEnumerator();
        //        if (e.MoveNext())
        //            o = e.Current;
        //        else
        //            o = null;
        //    }
        //    if (o is XElement)
        //        return (XElement)o;
        //    return null;
        //}

        //public static XElement zXPathElement(this XDocument xd, string sXPath)
        //{
        //    if (xd == null) return null;
        //    return xd.Root.zXPathElement(sXPath);
        //}

        //public static object zXPathAddNode(this XElement xe, string sXPath)
        //{
        //    if (xe == null) return null;
        //    string sElement;
        //    XElement xe2;
        //    char[] cSeparator = { '/', '\\' };
        //    string[] sElements = zsplit.Split(sXPath, cSeparator, true);
        //    for (int i = 0; i < sElements.Length - 1; i++)
        //    {
        //        sElement = sElements[i];
        //        xe2 = xe.Element(sElement);
        //        if (xe2 == null)
        //        {
        //            xe2 = new XElement(sElement);
        //            xe.Add(xe2);
        //        }
        //        xe = xe2;
        //    }
        //    sElement = sElements[sElements.Length - 1];
        //    xe2 = xe.Element(sElement);
        //    if (xe2 != null) return xe2;
        //    if (sElement.StartsWith("@")) sElement = sElement.Remove(0, 1);
        //    XAttribute xa = xe.Attribute(sElement);
        //    if (xa == null)
        //    {
        //        xa = new XAttribute(sElement, "");
        //        xe.Add(xa);
        //    }
        //    return xa;
        //}

        //public static void zSetValue(this XElement xe, string sXPath, string sValue)
        //{
        //    if (xe == null) return;
        //    object xn = xe.zXPathNode(sXPath);
        //    if (xn == null)
        //    {
        //        xn = xe.zXPathAddNode(sXPath);
        //    }
        //    XAttribute xa = null;
        //    if (xn is XElement)
        //    {
        //        XElement xe2 = xn as XElement;
        //        xa = xe2.Attributes().FirstOrDefault();
        //        if (xa == null)
        //        {
        //            xa = new XAttribute("value", sValue);
        //            xe2.Add(xa);
        //        }
        //    }
        //    else if (xn is XAttribute)
        //        xa = xn as XAttribute;
        //    xa.Value = sValue;
        //}

        //public static void zSetValue(this XDocument xd, string sXPath, string sValue)
        //{
        //    if (xd == null) return;
        //    xd.Root.zSetValue(sXPath, sValue);
        //}

        //public static IEnumerable<XElement> zElements(this XElement xe, string sXPath)
        //{
        //    return new XElementXPath(xe, sXPath);
        //}

        //public static IEnumerable<XElement> zElements(this XDocument xd, string sXPath)
        //{
        //    return new XElementXPath(xd.Root, sXPath);
        //}

        //public static DataTable zXmlToDataTable(this XElement xe)
        //{
        //    DataSet ds = xe.zXmlToDataSet();
        //    if (ds.Tables.Count > 0) return ds.Tables[0];
        //    return null;
        //}

        // used in :
        //   PB_Grid.XtraGridTools.SetColumnEditLookUp()        (PB_Tools\PB_Form\XtraGrid.cs)
        //   PB_Grid.XtraGridTools.SetGridColumnEditCombo()     (PB_Tools\PB_Form\XtraGrid.cs)
        //   PB_Grid.DataContainer.CreateDataTable()            (PB_Tools\PB_Form\cGrid.cs)

        // used in : pb.old.TreeViewExtension.TreeViewAdd() (PB_Tools\PB_Form\TreeView.cs)

        //public static DataTable zTranslateToDataTable<T>(this IEnumerable<T> q, string sFormatColumnName)
        //{
        //    Type t = typeof(T);
        //    DataTable dt = new DataTable();
        //    DataRow row = dt.NewRow();
        //    int i = 1;
        //    foreach (T v in q)
        //    {
        //        string sColumnName = string.Format(sFormatColumnName, i++);
        //        DataColumn col = dt.Columns.Add(sColumnName, t);
        //        row[col] = v;
        //    }
        //    dt.Rows.Add(row);
        //    return dt;
        //}

        //private static Regex _rgVariables = new Regex(@"\$([_a-z][_a-z0-9/]*)\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //// get variable $Ebookdz/DataDir$ value
        //public static string zGetVariableValue(this XElement xe, string value, Func<string, string> getValue = null)
        //{
        //    if (value == null || xe == null)
        //        return null;
        //    return _rgVariables.Replace(value, new MatchEvaluator(
        //        match =>
        //        {
        //            string xpath = match.Groups[1].Value;
        //            XElement parent = xe.Parent;
        //            string var = null;
        //            if (getValue != null)
        //                var = getValue(xpath);
        //            if (parent != null && xpath != xe.Name.LocalName)
        //                var = parent.zXPathValue(xpath);
        //            if (var == null)
        //                var = xe.Document.Root.zXPathValue(xpath);
        //            if (var == null)
        //                Trace.WriteLine("error can't find xml variable \"${0}$\" from {1}", xpath, xe.zGetPath());
        //            return var;
        //        }));
        //}

        // zContains() not used
        //public static bool zContains(this XElement xe, params string[] sFinds)
        //{
        //    if (xe == null) return false;
        //    //string sValue = xe.zGetFirstValue();
        //    string sValue = xe.Value;
        //    foreach (string sFind in sFinds)
        //    {
        //        Regex rx = new Regex(sFind, RegexOptions.IgnoreCase);
        //        if (rx.IsMatch(sValue)) return true;
        //    }
        //    return false;
        //}

        //public static bool zContains(this XElement xe, params Regex[] rxs)
        //{
        //    if (xe == null) return false;
        //    //return rx.IsMatch(xe.zGetFirstValue());
        //    string sValue = xe.Value;
        //    foreach (Regex rx in rxs)
        //    {
        //        if (rx.IsMatch(sValue)) return true;
        //    }
        //    return false;
        //}

        //public static bool zContains(this XAttribute xa, params string[] sFinds)
        //{
        //    if (xa == null) return false;
        //    string sValue = xa.Value;
        //    foreach (string sFind in sFinds)
        //    {
        //        Regex rx = new Regex(sFind, RegexOptions.IgnoreCase);
        //        if (rx.IsMatch(sValue)) return true;
        //    }
        //    return false;
        //}

        //public static bool zContains(this XAttribute xa, params Regex[] rxs)
        //{
        //    if (xa == null) return false;
        //    string sValue = xa.Value;
        //    foreach (Regex rx in rxs)
        //    {
        //        if (rx.IsMatch(sValue)) return true;
        //    }
        //    return false;
        //}
    //}

    //public class XElementXPath : IEnumerable<XElement>, IEnumerator<XElement>, IEnumerable
    //{
    //    private XElement gxeSource = null;
    //    private string gsXPathSource = null;

    //    private string[] gsElements = null;
    //    private IEnumerator<XElement>[] gXElements = null;
    //    private XElement gxeCurrent = null;
    //    private bool gbEndOfList = false;

    //    public XElementXPath(XElement xe, string sXPath)
    //    {
    //        gxeSource = xe;
    //        gsXPathSource = sXPath;
    //        Init();
    //    }

    //    public void Dispose()
    //    {
    //    }

    //    private void Init()
    //    {
    //        gsElements = zsplit.Split(gsXPathSource, '/');
    //        gXElements = new IEnumerator<XElement>[gsElements.Length];
    //        if (gxeSource != null)
    //            gXElements[0] = gxeSource.Elements(gsElements[0]).GetEnumerator();
    //        else
    //            gXElements[0] = new XElement[0].AsEnumerable().GetEnumerator();
    //    }

    //    public IEnumerator<XElement> GetEnumerator()
    //    {
    //        return this;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this;
    //    }

    //    public XElement Current
    //    {
    //        get { return gxeCurrent; }
    //    }

    //    object IEnumerator.Current
    //    {
    //        get { return gxeCurrent; }
    //    }

    //    public bool MoveNext()
    //    {
    //        if (gbEndOfList) return false;

    //        while (true)
    //        {
    //            bool bNext = false;
    //            IEnumerator<XElement> list = null;
    //            int i;
    //            for (i = gXElements.Length - 1; i >= 0; i--)
    //            {
    //                list = gXElements[i];
    //                if (list != null)
    //                {
    //                    bNext = list.MoveNext();
    //                    if (bNext) break;
    //                }
    //            }

    //            if (!bNext)
    //            {
    //                gbEndOfList = true;
    //                return false;
    //            }
    //            else if (i == gXElements.Length - 1)
    //            {
    //                gxeCurrent = list.Current;
    //                return true;
    //            }


    //            for (i++; i < gXElements.Length; i++)
    //            {
    //                list = list.Current.Elements(gsElements[i]).GetEnumerator();
    //                gXElements[i] = list;
    //                bNext = list.MoveNext();
    //                if (!bNext) break;
    //            }
    //            if (bNext)
    //            {
    //                gxeCurrent = list.Current;
    //                return true;
    //            }
    //        }
    //    }

    //    public void Reset()
    //    {
    //        gbEndOfList = false;
    //        gXElements[0].Reset();
    //    }
    //}

    //public class AutoNumber
    //{
    //    private int gi;

    //    #region constructor
    //    public AutoNumber(int i)
    //    {
    //        gi = i;
    //    }
    //    #endregion

    //    #region Value
    //    public int Value
    //    {
    //        get { return gi++; }
    //        set { gi = value; }
    //    }
    //    #endregion

    //    #region ToString
    //    public override string ToString()
    //    {
    //        return (gi++).ToString();
    //    }
    //    #endregion
    //}

    //public class XmlFilteredNode : IEnumerable<XNode>, IEnumerator<XNode>
    //{
    //    private XElement gSourceElement = null;
    //    private XName[] gExcludeElements = null;
    //    private SortedList<string, object> gExcludeElements2 = null;
    //    private Func<XNode, bool> gFilter = null;
    //    private XNode gCurrentNode = null;

    //    public XNode Current
    //    {
    //        get { return gCurrentNode; }
    //    }

    //    object IEnumerator.Current
    //    {
    //        get { return gCurrentNode; }
    //    }

    //    public XName[] ExcludeElements
    //    {
    //        get { return gExcludeElements; }
    //        set
    //        {
    //            gExcludeElements = value;
    //            gExcludeElements2 = new SortedList<string, object>();
    //            foreach (XName xname in value)
    //            {
    //                if (!gExcludeElements2.ContainsKey(xname.LocalName))
    //                    gExcludeElements2.Add(xname.LocalName, null);
    //            }
    //        }
    //    }

    //    public XmlFilteredNode(XElement sourceElement)
    //    {
    //        gSourceElement = sourceElement;
    //    }

    //    public XmlFilteredNode(XElement sourceElement, params XName[] excludeElements)
    //    {
    //        gSourceElement = sourceElement;
    //        ExcludeElements = excludeElements;
    //    }

    //    public void Dispose()
    //    {
    //    }

    //    public IEnumerator<XNode> GetEnumerator()
    //    {
    //        return this;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this;
    //    }

    //    public bool MoveNext()
    //    {
    //        XNode node, node2;
    //        node = gCurrentNode;
    //        if (node == null)
    //        {
    //            node = gSourceElement.FirstNode;
    //            if (node != null)
    //            {
    //                if (Filter(node))
    //                {
    //                    gCurrentNode = node;
    //                    return true;
    //                }
    //            }
    //            else
    //                return false;
    //        }
    //        else if (node is XElement)
    //        {
    //            XElement xe = (XElement)node;
    //            node2 = xe.FirstNode;
    //            if (node2 != null)
    //            {
    //                if (Filter(node2))
    //                {
    //                    gCurrentNode = node2;
    //                    return true;
    //                }
    //                else
    //                    node = node2;
    //            }
    //        }
    //        while (true)
    //        {
    //            while (true)
    //            {
    //                node2 = node.NextNode;
    //                if (node2 == null) break;
    //                if (Filter(node2))
    //                {
    //                    gCurrentNode = node2;
    //                    return true;
    //                }
    //                else
    //                    node = node2;
    //            }
    //            node = node.Parent;
    //            if (node == gSourceElement) return false;
    //        }
    //    }

    //    public void Reset()
    //    {
    //        gCurrentNode = null;
    //    }

    //    public bool Filter(XNode node)
    //    {
    //        if (gExcludeElements2 != null)
    //        {
    //            if (node is XElement)
    //            {
    //                XElement xe = (XElement)node;
    //                if (gExcludeElements2.ContainsKey(xe.Name.LocalName))
    //                    return false;
    //            }
    //        }
    //        if (gFilter != null)
    //        {
    //            if (!gFilter(node))
    //                return false;
    //        }
    //        return true;
    //    }

    //    public static IEnumerable<XNode> DescendantNodes(XElement xe, Func<XNode, bool> nodeFilter)
    //    {
    //        XmlFilteredNode filteredNode = new XmlFilteredNode(xe);
    //        filteredNode.gFilter = nodeFilter;
    //        return filteredNode;
    //    }

    //    public static IEnumerable<XNode> DescendantNodes(XElement xe, params XName[] excludeElements)
    //    {
    //        XmlFilteredNode filteredNode = new XmlFilteredNode(xe);
    //        filteredNode.ExcludeElements = excludeElements;
    //        return filteredNode;
    //    }
    //}

    //public class XNodeDescendants : IEnumerable<XNode>, IEnumerator<XNode>
    //{
    //    private static bool __trace = false;
    //    protected XNode _sourceNode = null;
    //    protected XNode _node = null;
    //    //protected int _level;
    //    protected Predicate<XNode> _nodeFilter = null;
    //    protected Predicate<XNode> _returnNodeFilter = null;

    //    /// <param name="nodeFilter">filtre les noeuds à parcourir</param>
    //    /// <param name="returnNodeFilter">sélectionne les noeuds à retourner</param>
    //    public XNodeDescendants(XElement element, Predicate<XNode> nodeFilter = null, Predicate<XNode> returnNodeFilter = null)
    //    {
    //        _sourceNode = element;
    //        _node = element;
    //        //_level = 0;
    //        _nodeFilter = nodeFilter;
    //        _returnNodeFilter = returnNodeFilter;
    //    }

    //    public void Dispose()
    //    {
    //    }

    //    public static bool Trace { get { return __trace; } set { __trace = value; } }

    //    public IEnumerator<XNode> GetEnumerator()
    //    {
    //        return this;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this;
    //    }

    //    //public XNode Current { get { return _node; } }
    //    public XNode Current
    //    {
    //        get
    //        {
    //            if (__trace)
    //                pb.old.Trace.WriteLine("XNodeDescendants node : {0}", _node.zGetPath());
    //            return _node;
    //        }
    //    }

    //    object IEnumerator.Current { get { return _node; } }

    //    public bool MoveNext()
    //    {
    //        while (true)
    //        {
    //            // get child
    //            if (_node is XElement)
    //            {
    //                XElement xe = (XElement)_node;
    //                // first child node
    //                XNode node = xe.FirstNode;
    //                if (node != null && (_nodeFilter == null || _nodeFilter(node)))
    //                {
    //                    _node = node;
    //                    //_level++;
    //                    if (_returnNodeFilter == null || _returnNodeFilter(node))
    //                        return true;
    //                    else
    //                        continue;
    //                }
    //            }

    //            // get next sibling node or next sibling node of parent node
    //            bool cont = false;
    //            while (true)
    //            {
    //                if (_node == _sourceNode)
    //                    return false;
    //                // next sibling node
    //                XNode node = _node.NextNode;
    //                while (true)
    //                {
    //                    if (node == null)
    //                        break;
    //                    if (_nodeFilter == null || _nodeFilter(node))
    //                    {
    //                        _node = node;
    //                        if (_returnNodeFilter == null || _returnNodeFilter(node))
    //                            return true;
    //                        else
    //                        {
    //                            cont = true;
    //                            break;
    //                        }
    //                    }
    //                    node = node.NextNode;
    //                }
    //                if (cont)
    //                    break;
    //                // parent node
    //                _node = _node.Parent;
    //                //_level--;
    //            }
    //        }
    //    }

    //    public void Reset()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    /// <param name="nodeFilter">filtre les noeuds à parcourir</param>
    //    /// <param name="returnNodeFilter">sélectionne les noeuds à retourner</param>
    //    public static IEnumerable<XNode> DescendantNodes(XElement element, Predicate<XNode> nodeFilter = null, Predicate<XNode> returnNodeFilter = null)
    //    {
    //        return new XNodeDescendants(element, nodeFilter, returnNodeFilter);
    //    }
    //}
}

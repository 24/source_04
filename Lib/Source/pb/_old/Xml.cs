using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using pb.Data.Xml;

namespace pb.old
{
    //public class XmlException : Exception
    //{
    //    public XmlException(string sMessage) : base(sMessage) { }
    //    public XmlException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
    //    public XmlException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
    //    public XmlException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    //}

    //public class Xml
    //{
        //private static Encoding gIsoEncoding = Encoding.GetEncoding("iso-8859-1");
        //private static Regex grxTranslate1 = new Regex(@"&([a-zA-Z]+)\w*;?", RegexOptions.Compiled);
        //private static Regex grxTranslate2 = new Regex(@"&#([0-9]+);", RegexOptions.Compiled);
        //private static Regex grxTranslate3 = new Regex(@"&#x([a-fA-F0-9]+);", RegexOptions.Compiled);

        //public static string GetValue(XmlNode node)
        //{
        //    return GetValue(node, null);
        //}

        //public static string[] GetValues(XmlNode node, string sXPath)
        //{
        //    if (node == null || sXPath == null) return new string[0];
        //    XmlNodeList nodes = node.SelectNodes(sXPath);
        //    ArrayList al = new ArrayList();
        //    foreach (XmlNode node2 in nodes)
        //        if (node2.Value != null) al.Add(node2.Value);
        //    string[] sValues = new string[al.Count];
        //    for (int i = 0; i < al.Count; i++) sValues[i] = (string)al[i];
        //    return sValues;
        //}

        //public static string SelectValue(XmlNode node, string sXPath, params string[] sValues)
        //{
        //    return SelectValue(node, new XmlSelectParameters(), sXPath, sValues);
        //}

        //public static string[] SelectValues(XmlNode node, string sXPath, params string[] sValues)
        //{
        //    return SelectValues(node, new XmlSelectParameters(), sXPath, sValues);
        //}

        //public static DataTable Select0(XmlNode node, string sXPath, params string[] sValues)
        //{
        //    return Select0(node, new XmlSelectParameters(), sXPath, sValues);
        //}

        //public static DataTable Select0(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        //{
        //    DataTable dtResult = new DataTable();
        //    if (sXPath == null) return dtResult;

        //    XmlSelect xSelect = new XmlSelect();
        //    xSelect.SourceNode = node;
        //    xSelect.SelectPrm = selectPrm;
        //    xSelect.SourceXPathNode = sXPath;
        //    xSelect.SourceXPathValues = sValues;

        //    dtResult.Columns.Add("node", typeof(string));
        //    //if (xSelect.XPathValues.Length == 0) dtResult.Columns.Add("text", typeof(string));
        //    string sColumnName = "text";
        //    if (xSelect.XPathNode.IsNameDefined) sColumnName = xSelect.XPathNode.Name;
        //    if (xSelect.XPathValues.Length == 0) dtResult.Columns.Add(sColumnName, typeof(string));
        //    int i = 0;
        //    foreach (XPath xPathValue in xSelect.XPathValues)
        //    {
        //        sColumnName = xPathValue.Name;
        //        if (i == 0 && !xPathValue.IsNameDefined && xSelect.XPathNode.IsNameDefined) sColumnName = xSelect.XPathNode.Name;
        //        sColumnName = cdt.GetNewColumnName(dtResult, sColumnName);
        //        Type type = typeof(string);
        //        if (xPathValue.TypeValue != null) type = xPathValue.TypeValue;
        //        dtResult.Columns.Add(sColumnName, type);
        //        i++;
        //    }

        //    while (xSelect.Get())
        //    {
        //        DataRow row = dtResult.NewRow();
        //        row[0] = xSelect.TranslatedPathCurrentNode;
        //        row[1] = xSelect.Values[0];
        //        for (i = 1; i < sValues.Length; i++)
        //            //row[i + 1] = xSelect.Values[i];
        //            row[i + 1] = xSelect.GetValue(i);
        //        dtResult.Rows.Add(row);
        //    }
        //    return dtResult;
        //}

        //public static XmlSelect Select(XmlNode node, string sXPath, params string[] sValues)
        //{
        //    return Select(node, new XmlSelectParameters(), sXPath, sValues);
        //}

        //public static HtmlXmlTable SelectTable(XmlNode node, XmlSelectParameters selectPrm, string sXPath, params string[] sValues)
        //{
        //    if (node == null || sXPath == null) return null;

        //    XmlSelect xSelect = new XmlSelect();
        //    xSelect.SourceNode = node;
        //    xSelect.SelectPrm = selectPrm;
        //    xSelect.SourceXPathNode = sXPath;
        //    if (sValues.Length == 0)
        //        xSelect.SourceXPathValues = new string[] { ":.:NodeValue" };
        //    else
        //        xSelect.SourceXPathValues = sValues;

        //    if (xSelect.Get())
        //    {
        //        if (xSelect.LastValueTable != null)
        //            return xSelect.CurrentTable;
        //        else
        //            return xSelect.CurrentTable;
        //    }
        //    return null;
        //}


        // Xml.Linq ***********************************************************************************************

        //public static string GetAttribValue(XElement xe, XName Attrib)
        //{
        //    if (xe == null) return null;
        //    return xe.zAttribValue(Attrib, null);
        //}

        //public static string GetAttribValue(XElement xe, XName Attrib, string sDefault)
        //{
        //    if (xe == null) return sDefault;
        //    return xe.zAttribValue(Attrib, sDefault);
        //}

        //public static int GetAttribIntValue(XElement xe, XName Attrib, int iDefault)
        //{
        //    if (xe == null) return iDefault;
        //    return xe.zAttribValueInt(Attrib, iDefault);
        //}

        //public static void Export(string path, XDocument xd)
        //{
        //    XmlExport.Export(path, xd.Root);
        //}

        //public static void Export(string path, XElement xe)
        //{
        //    XmlExport.Export(path, xe);
        //}

        //public static void XslTransformXmlFile(string xslPath, string xmlPath, string outputPath)
        //{
        //    XslCompiledTransform xsl = new XslCompiledTransform();
        //    xsl.Load(xslPath);
        //    xsl.Transform(xmlPath, outputPath);
        //}

        //public static string XslTransformXmlFile(string xslPath, string xmlPath)
        //{
        //    XslCompiledTransform xsl = new XslCompiledTransform();
        //    xsl.Load(xslPath);
        //    StringWriter sw = new StringWriter();
        //    xsl.Transform(xmlPath, null, sw);
        //    string result = sw.ToString();
        //    sw.Close();
        //    return result;
        //}

        //public static void XslTransformXmlString(string xslPath, string xml, string outputPath)
        //{
        //    XslCompiledTransform xsl = new XslCompiledTransform();
        //    xsl.Load(xslPath);
        //    XmlTextReader xr = new XmlTextReader(new StringReader(xml));
        //    StringWriter sw = new StringWriter();
        //    xsl.Transform(xr, null, sw);
        //    zfile.WriteFile(outputPath, sw.ToString());
        //    sw.Close();
        //    xr.Close();
        //}

        //public static string XslTransformXmlString(string xslPath, string xml)
        //{
        //    XslCompiledTransform xsl = new XslCompiledTransform();
        //    xsl.Load(xslPath);
        //    XmlTextReader xr = new XmlTextReader(new StringReader(xml));
        //    StringWriter sw = new StringWriter();
        //    xsl.Transform(xr, null, sw);
        //    string result = sw.ToString();
        //    sw.Close();
        //    xr.Close();
        //    return result;
        //}

        //public static void XslTransformXmlReader(string xslPath, XmlReader xmlReader, string outputPath)
        //{
        //    XslCompiledTransform xsl = new XslCompiledTransform();
        //    xsl.Load(xslPath);
        //    StringWriter sw = new StringWriter();
        //    xsl.Transform(xmlReader, null, sw);
        //    zfile.WriteFile(outputPath, sw.ToString());
        //    sw.Close();
        //}

        //public static string XslTransformXmlReader(string xslPath, XmlReader xmlReader)
        //{
        //    XslCompiledTransform xsl = new XslCompiledTransform();
        //    xsl.Load(xslPath);
        //    StringWriter sw = new StringWriter();
        //    xsl.Transform(xmlReader, null, sw);
        //    string result = sw.ToString();
        //    sw.Close();
        //    return result;
        //}
    //}

    //public class XmlExport
    //{
    //    private StreamWriter gsw = null;

    //    public static void Export(string path, XDocument xd)
    //    {
    //        XmlExport export = new XmlExport();
    //        export._Export(path, xd.Root);
    //    }

    //    public static void Export(string path, XElement xe)
    //    {
    //        XmlExport export = new XmlExport();
    //        export._Export(path, xe);
    //    }

    //    public void _Export(string path, XElement xe)
    //    {
    //        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
    //        using (gsw = new StreamWriter(fs))
    //        {
    //            _Export(xe, 0);
    //        }
    //    }

    //    private void _Export(XElement xe, int tab)
    //    {
    //        Write(tab, "<{0}", xe.Name);
    //        foreach (XAttribute xa in xe.Attributes())
    //            gsw.Write(" {0}=\"{1}\"", xa.Name, xa.Value);
    //        if (xe.IsEmpty)
    //            gsw.WriteLine("/>");
    //        else
    //        {
    //            gsw.WriteLine(">");
    //            int tab2 = tab + 1;
    //            foreach (XNode node in xe.Nodes())
    //            {
    //                if (node is XElement)
    //                    _Export((XElement)node, tab2);
    //                else if (node is XText)
    //                    WriteLine(tab2, zstr.ReplaceControl(((XText)node).Value));
    //                else if (node is XComment)
    //                    WriteLine(tab2, zstr.ReplaceControl(((XComment)node).Value));
    //            }
    //            Write(tab, "</{0}>", xe.Name);
    //            gsw.WriteLine();
    //        }

    //    }

    //    private void Write(int tab, string text, params object[] param)
    //    {
    //        for (int i = 0; i < tab; i++) gsw.Write('\t');
    //        if (param.Length != 0) text = string.Format(text, param);
    //        gsw.Write(text);
    //    }

    //    private void WriteLine(int tab, string text, params object[] param)
    //    {
    //        Write(tab, text, param);
    //        gsw.WriteLine();
    //    }
    //}
}

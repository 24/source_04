using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace pb.old
{
    //public static class zxml
    //{
        //public static void XmlSerialize<T>(T value, string file)
        //{
        //    XmlSerializer xs = new XmlSerializer(typeof(T));
        //    using (StreamWriter wr = new StreamWriter(file))
        //    {
        //        xs.Serialize(wr, value);
        //    }
        //}

        //public static T XmlDeserialize<T>(string file)
        //{
        //    XmlSerializer xs = new XmlSerializer(typeof(T));
        //    using (StreamReader sr = new StreamReader(file))
        //    {
        //        return (T)xs.Deserialize(sr);
        //    }
        //}

        //public static bool XPathResultToString(object o, out string value)
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

        // concat text only for XText
        //public static bool XPathResultConcatText(object o, out string value, string separator = null, Func<string, string> resultFunc = null, Func<string, string> itemFunc = null)
        //{
        //    value = null;
        //    if (o is IEnumerable)
        //    {
        //        bool found = false;
        //        string concat = "";
        //        foreach (object v in (IEnumerable)o)
        //        {
        //            if (v is XText)
        //            {
        //                found = true;
        //                if (concat != "" && separator != null)
        //                    concat += separator;
        //                string item = (v as XText).Value;
        //                if (itemFunc != null)
        //                    item = itemFunc(item);
        //                concat += item;
        //            }
        //            if (resultFunc != null)
        //                concat = resultFunc(concat);
        //            value = concat;
        //        }
        //        return found;
        //    }
        //    //if (o is string)
        //    //    value = o as string;
        //    //else if (o is XAttribute)
        //    //    value = (o as XAttribute).Value;
        //    //else if (o is XElement)
        //    //    value = (o as XElement).zTextOrAttribValue();
        //    else if (o is XText)
        //        value = (o as XText).Value;
        //    else
        //        return false;
        //    return true;
        //}

        //public static IEnumerable<string> XmlFileXPathEvaluate(string file, string xpath)
        //{
        //    XDocument xd = XDocument.Load(file);
        //    return new XPathEvaluate(xd.Root, xpath);
        //}

        //public static IEnumerable<FileValue> XmlFilesXPathEvaluate(IEnumerable<string> files, string xpath)
        //{
        //    return (from file in files select from value in XmlFileXPathEvaluate(file, xpath) select new FileValue { file = file, value = value }).Flatten();
        //}
    //}

    //public class FileValue
    //{
    //    public string file;
    //    public string value;
    //}

    //public class XPathEvaluate : IEnumerable<string>, IEnumerator<string>
    //{
    //    protected string _current = null;
    //    protected object _xpathResult = null;
    //    protected IEnumerator _enumerator = null;

    //    public XPathEvaluate(XElement xeSource, string xpath)
    //    {
    //        _xpathResult = xeSource.XPathEvaluate(xpath);
    //        if (_xpathResult is IEnumerable)
    //            _enumerator = (_xpathResult as IEnumerable).GetEnumerator();
    //    }

    //    public void Dispose()
    //    {
    //    }

    //    public IEnumerator<string> GetEnumerator()
    //    {
    //        return this;
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return this;
    //    }

    //    public string Current
    //    {
    //        get { return _current; }
    //    }

    //    object System.Collections.IEnumerator.Current
    //    {
    //        get { return _current; }
    //    }

    //    public bool MoveNext()
    //    {
    //        if (_enumerator != null)
    //        {
    //            if (_enumerator.MoveNext())
    //                _xpathResult = _enumerator.Current;
    //            else
    //                _xpathResult = null;
    //        }
    //        if (_xpathResult != null)
    //        {
    //            string value;
    //            if (zxml.XPathResultToString(_xpathResult, out value))
    //            {
    //                _current = value;
    //                _xpathResult = null;
    //                return true;
    //            }
    //            _xpathResult = null;
    //        }
    //        return false;
    //    }

    //    public void Reset()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace pb.Data.Xml.Test
{
    public static class Test_Xpath
    {
        public static void Test_01()
        {
//            string markup = @"
//<aw:Root xmlns:aw='http://www.adventure-works.com'>
//    <aw:Child1>child one data 1</aw:Child1>
//    <aw:Child1>child one data 2</aw:Child1>
//    <aw:Child1>child one data 3</aw:Child1>
//    <aw:Child2>child two data 4</aw:Child2>
//    <aw:Child2>child two data 5</aw:Child2>
//    <aw:Child2>child two data 6</aw:Child2>
//</aw:Root>";
            string markup = @"
<Root xmlns='http://www.adventure-works.com'>
    <Child1>child one data 1</Child1>
    <Child1>child one data 2</Child1>
    <Child1>child one data 3</Child1>
    <Child2>child two data 4</Child2>
    <Child2>child two data 5</Child2>
    <Child2>child two data 6</Child2>
</Root>";
            XmlReader reader = XmlReader.Create(new StringReader(markup));
            XElement root = XElement.Load(reader);
            XmlNameTable nameTable = reader.NameTable;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            //namespaceManager.AddNamespace("aw", "http://www.adventure-works.com");
            namespaceManager.AddNamespace(null, "http://www.adventure-works.com");
            //IEnumerable<XElement> elements = root.XPathSelectElements("./aw:Child1", namespaceManager);
            IEnumerable<XElement> elements = root.XPathSelectElements("./Child1", namespaceManager);
            foreach (XElement el in elements)
                //Console.WriteLine(el);
                Trace.WriteLine(el.Name.LocalName);
        }
    }
}

using System.Collections.Generic;
using System.Xml.Linq;

namespace pb.Data.Xml
{
    //public enum XmlElementType
    //{
    //    Root = 1,
    //    Html,
    //    Head,
    //    Title,
    //    Body
    //}

    public class XLog
    {
    }

    public class XLogElement : XLog
    {
        public string Name;
        public string Path;
    }

    public class XLogAttribute : XLog
    {
        public string Name;
        public string Value;
        public string Path;
    }

    public class XLogText : XLog
    {
        public string Text;
        public string Path;
    }

    public class XLogComment : XLog
    {
        public string Comment;
        public string Path;
    }

    public class XDocumentCreator
    {
        private XDocument _xdocument = null;
        //private Dictionary<XmlElementType, XElement> _elements = null;
        //private XElement _currentElement = null;
        private List<XLog> _log = null;

        public XDocumentCreator()
        {
            _xdocument = new XDocument();
            //_elements = new Dictionary<XmlElementType, XElement>();
            _log = new List<XLog>();
        }

        public XDocument XDocument { get { return _xdocument; } }
        public IEnumerable<XLog> Log { get { return _log; } }

        public XElement AddRootElement(string name)
        {
            XElement element = new XElement(name);
            _xdocument.Add(element);
            _log.Add(new XLogElement { Name = name, Path = element.zGetPath() });
            return element;
        }

        public void AddRootElement(XElement element)
        {
            _xdocument.Add(element);
            _log.Add(new XLogElement { Name = element.Name.LocalName, Path = element.zGetPath() });
        }

        public XElement AddElement(XElement parent, string name)
        {
            XElement element = new XElement(name);
            parent.Add(element);
            _log.Add(new XLogElement { Name = name, Path = element.zGetPath() });
            return element;
        }

        public void AddElement(XElement parent, XElement element)
        {
            parent.Add(element);
            _log.Add(new XLogElement { Name = element.Name.LocalName, Path = element.zGetPath() });
        }

        public void AddAttribute(XElement parent, string name, string value)
        {
            //XElement xeParent = (XElement)parent;
            if (parent.Attribute(name) == null)
            {
                if (value == null)
                    value = "";
                parent.Add(new XAttribute(name, value));
            }
            _log.Add(new XLogAttribute { Name = name, Value = value, Path = parent.zGetPath() });
        }

        public void AddText(XElement parent, string text)
        {
            parent.Add(new XText(text));
            _log.Add(new XLogText { Text = text, Path = parent.zGetPath() });
        }

        public void AddComment(XElement parent, string comment)
        {
            parent.Add(new XComment(comment));
            _log.Add(new XLogComment { Comment = comment, Path = parent.zGetPath() });
        }

        //public XElement AddElement(string name)
        //{
        //    XElement element = new XElement(name);
        //    if (_currentElement != null)
        //        _currentElement.Add(element);
        //    else
        //    {
        //        _xdocument.Add(element);
        //    }
        //    _currentElement = element;
        //    return element;
        //}

        //public void AddElement(XmlElementType type)
        //{
        //    if (_elements.ContainsKey(type))
        //        throw new PBException($"element already added {type}");
        //}
    }
}

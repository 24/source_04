using System;
using System.IO;
using System.Xml.Linq;
using pb.IO;

namespace pb.Data.Xml
{
    public class XmlSerializer
    {
        private XDocument _document = null;
        private XElement _currentElement = null;
        private TextSerializer _textSerializer = null;

        private XmlSerializer()
        {
        }

        public XmlSerializer(string rootName = "root", TextSerializer textSerializer = null)
        {
            SetTextSerializer(textSerializer);
            _document = new XDocument();
            CreateRoot(rootName);
            _currentElement = _document.Root;
        }

        public XDocument Document { get { return _document; } }

        private void SetTextSerializer(TextSerializer textSerializer)
        {
            if (textSerializer != null)
                _textSerializer = textSerializer;
            else
                _textSerializer = TextSerializer.CurrentTextSerializer;
        }

        private void CreateRoot(string rootName)
        {
            if (_document.Root == null)
                _document.Add(new XElement(rootName));
        }

        private void _Load(string file, string rootName)
        {
            if (File.Exists(file))
                _document = XDocument.Load(file);
            else
                _document = new XDocument();
            CreateRoot(rootName);
            _currentElement = _document.Root;
        }

        public void Save(string file)
        {
            zfile.CreateFileDirectory(file);
            _document.Save(file);
        }

        //public void AddValue(string name, string value)
        //{
        //    _currentElement.Add(new XElement(name, new XAttribute("value", value)));
        //}

        public void AddValue<T>(string name, T value)
        {
            string textValue = _textSerializer.Serialize(value);
            // save null as empty string
            if (textValue == null)
                textValue = "";
            _currentElement.Add(new XElement(name, new XAttribute("value", textValue)));
        }

        public T GetValue<T>(string name, T defaultValue = default(T))
        {
            XElement xe = _currentElement.Element(name);
            if (xe != null)
            {
                XAttribute xa = xe.Attribute("value");
                if (xa != null)
                {
                    string textValue = xa.Value;
                    // empty string is treated as null
                    if (textValue == "")
                        textValue = null;
                    return _textSerializer.Deserialize<T>(textValue, defaultValue, true);
                }
            }
            return defaultValue;
        }

        public T GetExplicitValue<T>(string name, T defaultValue = default(T))
        {
            XAttribute xa = _currentElement.Attribute(name);
            if (xa != null)
                return _textSerializer.Deserialize<T>(xa.Value, defaultValue, false);
            else
                throw new PBException("XmlSerializer value \"{0}\" not found from element \"{1}\"", name, _currentElement.zGetPath());
        }

        public XElement AddElement(string name)
        {
            XElement element = new XElement(name);
            _currentElement.Add(element);
            _currentElement = element;
            return _currentElement;
        }

        public XElement OpenElement(string name)
        {
            XElement element = _currentElement.Element(name);
            if (element != null)
            {
                _currentElement = element;
                return _currentElement;
            }
            else
                return null;
        }

        public XElement CloseElement()
        {
            _currentElement = _currentElement.Parent;
            return _currentElement;
        }

        public static XmlSerializer Load(string file, string rootName = "root", TextSerializer textSerializer = null)
        {
            XmlSerializer xmlSerializer = new XmlSerializer();
            xmlSerializer.SetTextSerializer(textSerializer);
            xmlSerializer._Load(file, rootName);
            return xmlSerializer;
        }
    }
}

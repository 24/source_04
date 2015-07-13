using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

//namespace Print.download
namespace Download.Print
{
    public class AttrToElementReader : XmlTextReader
    {
        private bool _ReadAttributes = false;
        private bool _ReadAttributeValue = false;
        private string _AttributeName = String.Empty;
        private XmlNodeType _NodeType = XmlNodeType.None;

        public AttrToElementReader(String fileName)
            : base(fileName)
        {
            base.WhitespaceHandling = WhitespaceHandling.None;
            // Intentionally Empty.
        }

        public override bool Read()
        {
            if (!_ReadAttributes)
            {
                bool baseRead = base.Read();
                _ReadAttributes = (baseRead && XmlNodeType.Element == base.NodeType && 0 < base.AttributeCount);
                return baseRead;
            }

            // Reading attribues;
            if (_ReadAttributeValue)
            {
                _ReadAttributeValue = base.ReadAttributeValue();
                if (!_ReadAttributeValue)
                {
                    // End of attribute.
                    // End element.
                    _NodeType = XmlNodeType.EndElement;
                }
                else
                {
                    _NodeType = XmlNodeType.None;
                }
                return true;
            }

            _ReadAttributes = base.MoveToNextAttribute();
            if (_ReadAttributes)
            {
                _ReadAttributeValue = true;
                _NodeType = XmlNodeType.Element;
                _AttributeName = base.Name;
                return true;
            }
            else
            {
                _ReadAttributeValue = false;
                _NodeType = XmlNodeType.None;
                _AttributeName = String.Empty;
                return Read();
            }
        }


        public override XmlNodeType NodeType
        {
            get
            {
                if (XmlNodeType.None == _NodeType)
                {
                    return base.NodeType;
                }

                return _NodeType;
            }
        }

        public override String Value
        {
            get
            {
                if (XmlNodeType.None == _NodeType)
                {
                    return base.Value;
                }

                return String.Empty;
            }
        }

        public override String Name
        {
            get
            {
                if (XmlNodeType.None == _NodeType)
                {
                    return base.Name;
                }

                return _AttributeName;
            }
        }

    }//AttrToElementReader
}

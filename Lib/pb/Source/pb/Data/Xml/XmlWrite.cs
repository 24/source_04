using System;
using System.Xml;

namespace pb.Data.Xml
{
    public static partial class GlobalExtension
    {
        public static void zWriteElementText(this XmlWriter xmlWriter, string elementName, string text)
        {
            xmlWriter.WriteStartElement(elementName);
            xmlWriter.WriteString(text);
            xmlWriter.WriteEndElement();
        }

        public static void zWriteElementWithAttributes(this XmlWriter xmlWriter, string elementName, params string[] attributes)
        {
            xmlWriter.WriteStartElement(elementName);
            string attributeName = null;
            foreach (string attribute in attributes)
            {
                if (attributeName == null)
                    attributeName = attribute;
                else
                {
                    xmlWriter.WriteAttributeString(attributeName, attribute);
                    attributeName = null;
                }
            }
            if (attributeName != null)
                xmlWriter.WriteAttributeString(attributeName, "");
            xmlWriter.WriteEndElement();
        }
    }
}

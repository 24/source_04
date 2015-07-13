using System;
using System.Xml;

namespace pb.Data.Xml.old
{
    public static partial class GlobalExtension
    {
        public static string zReadAttributeString(this XmlReader xmlReader, string name = null)
        {
            if (name != null)
                xmlReader.MoveToAttribute(name);
            else
                xmlReader.MoveToFirstAttribute();
            return xmlReader.Value;
        }
    }
}

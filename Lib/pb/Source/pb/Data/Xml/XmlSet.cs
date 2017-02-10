using pb.Text;
using System.Xml.Linq;

namespace pb.Data.Xml
{
    public static class XmlSet
    {
        //public static void SetValue(XDocument document, string xpath, string value)
        public static void SetValue(XElement xe, string xpath, string value)
        {
            XElement xe2 = xe.zXPathElement(xpath);
            if (xe2 == null)
                xe2 = CreateElement(xe, xpath);
            SetAttribute(xe2, "value", value);
        }

        //public static XElement CreateElement(XDocument document, string xpath)
        public static XElement CreateElement(XElement xe, string xpath)
        {
            string[] elements = zsplit.Split(xpath, '/');
            //XElement xe1 = document.Root;
            XElement xe1 = xe;
            foreach (string element in elements)
            {
                XElement xe2 = xe1.Element(element);
                if (xe2 == null)
                {
                    xe2 = new XElement(element);
                    xe1.Add(xe2);
                }
                xe1 = xe2;
            }
            return xe1;
        }

        public static void SetAttribute(XElement xe, string attributeName, string value)
        {
            XAttribute attribute = xe.Attribute(attributeName);
            if (attribute != null)
                attribute.Value = value;
            else
                xe.Add(new XAttribute(attributeName, value));

        }
    }

    public static class XmlSetExtension
    {
        //public static void zSetValue(this XDocument document, string xpath, string value)
        public static void zSetValue(this XElement xe, string xpath, string value)
        {
            XmlSet.SetValue(xe, xpath, value);
        }
    }
}

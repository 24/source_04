using System;
using System.IO;
using System.Xml.Serialization;

namespace pb.Data.Xml
{
    public static partial class GlobalExtension
    {
        public static string zXmlSerialize<T>(this T value)
        {
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            {
                xs.Serialize(sw, value);
                return sw.ToString();
            }
        }

        public static T zXmlDeserialize<T>(this string xml)
        {
            if (xml == null)
                return default(T);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(xml))
            {
                return (T)xs.Deserialize(sr);
            }
        }
    }
}

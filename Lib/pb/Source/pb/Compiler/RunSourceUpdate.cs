using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data.Xml;

namespace pb.Source.pb.Compiler
{
    public class RunSourceUpdateDirectory
    {
        public string sourceDirectory;
        public string destinationDirectory;
    }

    class RunSourceUpdate
    {
        public static IEnumerable<RunSourceUpdateDirectory> GetFromXml(string xml)
        {
            XDocument xdocument = XDocument.Parse(xml);
            foreach (XElement xe in xdocument.Root.zXPathElements("UpdateRunSource/Directory"))
            {
                yield return new RunSourceUpdateDirectory { sourceDirectory = xe.zAttribValue("source"), destinationDirectory = xe.zAttribValue("destination") };
            }
        }
    }
}

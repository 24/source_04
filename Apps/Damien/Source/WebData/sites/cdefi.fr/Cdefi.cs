using System;
using System.Text;
using pb.Data.Xml;
using pb.Text;
using pb.Web;

namespace hts.WebData
{
    public static class Cdefi
    {
        private static Func<string, string> __trim = text => text != null ? text.Trim() : null;
        private static HttpRequestParameters __httpRequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };

        public static Func<string, string> Trim { get { return __trim; } }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return __httpRequestParameters;
        }

        public static XmlExportDefinition GetXmlExportDefinition()
        {
            XmlExportDefinition xmlDefinition = new XmlExportDefinition();
            xmlDefinition.RootName = "Cdefi";
            xmlDefinition.ElementName = "Etablissement";
            xmlDefinition.ValuesDefinition = new XmlValueDefinition[] {
                new XmlValueDefinition { ElementName = "établissement", ValueName = "Institution" },
                new XmlValueDefinition { ElementName = "adresse", ValueName = "Address" },
                new XmlValueDefinition { ElementName = "département", ValueName = "Department" },
                new XmlValueDefinition { ElementName = "tel", ValueName = "Tel" },
                new XmlValueDefinition { ElementName = "mail", ValueName = "Mail" },
                new XmlValueDefinition { ElementName = "site", ValueName = "WebSite" },
                new XmlValueDefinition { ElementName = "type-établissement", ValueName = "InstitutionType" },
                new XmlValueDefinition { ElementName = "directeur", ValueName = "Director" },
                };
            return xmlDefinition;
        }
    }
}

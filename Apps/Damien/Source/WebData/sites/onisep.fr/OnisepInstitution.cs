using System;
using System.Text;
using pb.Data.Xml;
using pb.Text;
using pb.Web;

namespace hts.WebData
{
    public static class OnisepInstitution
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
            xmlDefinition.RootName = "Onisep";
            xmlDefinition.ElementName = "Institution";
            xmlDefinition.ValuesDefinition = new XmlValueDefinition[] {
                new XmlValueDefinition { ElementName = "établissement", ValueName = "Institution" },
                new XmlValueDefinition { ElementName = "code-UAI", ValueName = "UAICode" },
                new XmlValueDefinition { ElementName = "adresse", ValueName = "Address" },
                new XmlValueDefinition { ElementName = "code-postal", ValueName = "PostalCode" },
                new XmlValueDefinition { ElementName = "ville", ValueName = "City" },
                new XmlValueDefinition { ElementName = "tel", ValueName = "Tel" },
                new XmlValueDefinition { ElementName = "fax", ValueName = "Fax" },
                new XmlValueDefinition { ElementName = "mail", ValueName = "Mail" },
                new XmlValueDefinition { ElementName = "site", ValueName = "WebSite" },
                new XmlValueDefinition { ElementName = "statut", ValueName = "InstitutionStatus" },
                new XmlValueDefinition { ElementName = "hébergement", ValueName = "Lodging" },
                new XmlValueDefinition { ElementName = "niveau-étude", ValueName = "StudyLevels", TransformValue = studyLevels => ((string[])studyLevels).zConcatStrings(", ")},
                new XmlValueDefinition { ElementName = "bac", ValueName = "BacLevel" },
                new XmlValueDefinition { ElementName = "url", ValueName = "SourceUrl" }
                };
            return xmlDefinition;
        }
    }
}

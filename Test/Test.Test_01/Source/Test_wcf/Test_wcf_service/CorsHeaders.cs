using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;

namespace Test_wcf_service
{
    public class CorsHeaders
    {
        private static CorsHeaders __currentCorsHeaders = new CorsHeaders();
        private Dictionary<string, string> _allMessageHeaders = null;
        private Dictionary<string, string> _optionsMessageHeaders = null;

        public static CorsHeaders CurrentCorsHeaders { get { return __currentCorsHeaders; } }
        public Dictionary<string, string> AllMessageHeaders { get { return _allMessageHeaders; } }
        public Dictionary<string, string> OptionsMessageHeaders { get { return _optionsMessageHeaders; } }

        public CorsHeaders()
        {
            _allMessageHeaders = new Dictionary<string, string>();
            _optionsMessageHeaders = new Dictionary<string, string>();

            // in web application (WCF Service Application) ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None) throw an exception
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            if (configFile != null)
            {
                XDocument xdocument = XDocument.Load(configFile);
                XElement section = xdocument.Root.XPathSelectElement("corsHttpHeaders");
                if (section == null)
                {
                    Trace.WriteLine("corsHttpHeaders section not found in config file");
                    return;
                }
                XElement xelement = section.Element("allMessage");
                if (xelement != null)
                {
                    foreach (XElement xelement2 in xelement.Elements("corsHttpHeader"))
                        _allMessageHeaders.Add(xelement2.Attribute("name").Value, xelement2.Attribute("value").Value);
                }
                xelement = section.Element("optionsMessage");
                if (xelement != null)
                {
                    foreach (XElement xelement2 in xelement.Elements("corsHttpHeader"))
                        _optionsMessageHeaders.Add(xelement2.Attribute("name").Value, xelement2.Attribute("value").Value);
                }
            }
        }
    }
}
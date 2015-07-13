using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;

namespace Test.Test_AppConfig
{
    public static class Test_AppConfig
    {
        public static void Test()
        {
            Test_01();
            Test_02();
            Test_03();
            Test_04();
        }

        public static void Test_01()
        {
            string sectionName = "ConnectionManagerDatabaseServers";
            Trace.WriteLine("read section \"{0}\"", sectionName);
            object section = ConfigurationManager.GetSection(sectionName);
            if (section == null)
                Trace.WriteLine("element not found");
            else
                Trace.WriteLine("found element");
            if (section != null)
            {
                NameValueCollection connectionManagerDatabaseServers = section as NameValueCollection;
                //Trace.WriteLine("\"Dev\" \"{0}\"", connectionManagerDatabaseServers["Dev"].ToString());
                foreach (string key in connectionManagerDatabaseServers.AllKeys)
                    Trace.WriteLine("  name \"{0}\" = \"{1}\"", key, connectionManagerDatabaseServers[key]);
            }
            Trace.WriteLine();
        }

        public static void Test_02()
        {
            string sectionName = "corsHttpHeaders";
            Trace.WriteLine("read section \"{0}\"", sectionName);
            object section = ConfigurationManager.GetSection(sectionName);
            if (section == null)
                Trace.WriteLine("element not found");
            else
                Trace.WriteLine("found element");
            Trace.WriteLine();
        }

        public static void Test_03()
        {
            string sectionName = "corsHttpHeaders";
            Trace.WriteLine("read section \"{0}\"", sectionName);
            ConfigurationSection section = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection(sectionName);
            if (section == null)
            {
                Trace.WriteLine("element not found");
                Trace.WriteLine();
                return;
            }
            else
                Trace.WriteLine("found element");
            Trace.WriteLine("section.ElementInformation.Type : {0}", section.ElementInformation.Type);
            Trace.WriteLine();
        }

        public static void Test_04()
        {
            string xpath = "corsHttpHeaders";
            string elementName = "corsHttpHeader";
            Trace.WriteLine("load config file \"{0}\"", ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
            XDocument xdocument = XDocument.Load(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
            Trace.WriteLine("get xpath element \"{0}\"", xpath);
            XElement xelement = xdocument.Root.XPathSelectElement(xpath);
            if (xelement == null)
            {
                Trace.WriteLine("element not found");
                Trace.WriteLine();
                return;
            }
            else
                Trace.WriteLine("found element");

            Trace.WriteLine("enumerate elements \"{0}\"", elementName);
            foreach (XElement xelement2 in xelement.Elements(elementName))
                Trace.WriteLine("element \"{0}\" : name \"{1}\" value \"{2}\"", elementName, xelement2.Attribute("name"), xelement2.Attribute("value"));
            
            Trace.WriteLine();
        }
    }
}

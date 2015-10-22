using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;
using PB_Util;
//using PB_Tools;

//   taille du code :
//      Unea.cs                                 763 lignes
//      Unea_DetailCompany1.cs                  625 lignes
//      Unea_DetailCompany2.cs                  285 lignes
//      Unea_HeaderCompany.cs                   336 lignes
//      Unea_f.cs                               122 lignes
//                                            2 131 lignes
//
//      Gesat.cs                                802 lignes
//      Gesat_f.cs                               80 lignes
//                                              882 lignes

namespace Download.Damien
{
    static partial class w
    {
        private static RunSource _rs = RunSource.CurrentRunSource;
        private static ITrace _tr = _rs.Trace;
        private static HtmlXmlReader _hxr = HtmlXmlReader.CurrentHtmlXmlReader;
        private static string _dataDir = null;

        public static void Init()
        {
            _rs.InitConfig("damien");
            string log = _rs.Config.GetRootSubPath("Log", null);
            if (log != null) _tr.SetLogFile(log, LogOptions.IndexedFile);
            _dataDir = _rs.Config.GetExplicit("DataDir");
            string trace = _rs.Config.GetRootSubPath("Trace", null);
            if (trace != null && trace != "") _rs.Trace.SetTraceDirectory(trace);
            _hxr.SetResult += new SetResultEvent(_rs.SetResult);
        }

        public static void End()
        {
            _hxr.SetResult -= new SetResultEvent(_rs.SetResult);
        }

        public static string GetPath(string file)
        {
            return Path.Combine(_dataDir, file);
        }

        public static void Test_UrlToFileName_01()
        {
            string url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
            string request = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4=";
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = request;
            //_tr.WriteLine(zurl.UrlToFileName(url, UrlFileNameType.FileName | UrlFileNameType.Content, null, requestParameters));
            _tr.WriteLine(zurl.UrlToFileName(new HttpRequest { Url = url, Content = requestParameters.content }, UrlFileNameType.FileName | UrlFileNameType.Content));
        }

        public static void Test_Xml_01()
        {
            XElement xe = _hxr.XDocument.Root;
            string xpath = "//span[text() = 'Documents téléchargeables : ']";
            //string xpath = "//text()";
            //string xpath = "//*[text()";
            ////*[starts-with(text(), 'Phase ')]/ancestor::table[position()=1]//a
            object o = xe.XPathEvaluate(xpath);
            _tr.WriteLine("{0}", o.GetType());
        }

        public static void Test_XmlFile_01()
        {
            string file = @"c:\pib\dev_data\exe\wrun\damien\test\test.xml";
            string activity = "Gestion, administration, informatique : Numérisation -  -";
            zfile.CreateFileDirectory(file);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement("root");
                xw.WriteStartElement("data");
                xw.zWriteElementText("toto", "zozo");
                xw.zWriteElementText("tata", "zaza");
                xw.zWriteElementText("activité", activity);
                xw.zWriteElementText("tutu", "zuzu");
                xw.WriteEndElement();
                xw.WriteEndElement();
            }
        }
    }
}

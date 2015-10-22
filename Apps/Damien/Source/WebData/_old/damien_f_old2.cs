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
using PB_Util;
//using PB_Tools;
using Download.Gesat;

namespace Download.Damien
{
    static partial class w
    {
        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Bug_Path_01()
        {
            // bug from GetNewIndexedFileName() in File.cs
            _tr.WriteLine("Bug_Path_01");
            string file = "C:\\pib\\prog\\tools\\runsource\\run\\RunSource_{0:00000}.*";
            _tr.WriteLine("file \"{0}\"", file);
            try
            {
                // Path.GetDirectoryName(file); génère une exception System.NotSupportedException qui n'est pas capturé par le catch
                // et Path.GetDirectoryName est correctement exécuté
                //System.NotSupportedException occurred
                //  HResult=-2146233067
                //  Message=The given path's format is not supported.
                //  Source=mscorlib
                //  StackTrace:
                //       at System.Security.Util.StringExpressionSet.CanonicalizePath(String path, Boolean needFullPath)
                //  InnerException: 

                string dir = Path.GetDirectoryName(file);
                _tr.WriteLine("Path.GetDirectoryName(file) \"{0}\"", dir);
            }
            catch (NotSupportedException ex)
            {
                _tr.WriteLine("Path.GetDirectoryName(file) error NotSupportedException \"{0}\"", ex.Message);
            }
            catch (Exception ex)
            {
                _tr.WriteLine("Path.GetDirectoryName(file) error \"{0}\"", ex.Message);
            }
        }

        public static void Test_Download_Url_01()
        {
            //_tr.WriteLine("Test_Download_Url_01");
            string url = "http://www.reseau-gesat.com/Gesat/";
            //string url = "";
            //string url = "";
            //string url = "";
            //string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
            //_tr.WriteLine("download url \"{0}\" to \"{1}\"", url, file);

            _tr.WriteLine("download url \"{0}\"", url);
            Http_v2.LoadUrl(url);
        }

        public static void Test_Uri_01()
        {
            string url = "http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/";
            _tr.WriteLine("url \"{0}\"", url);
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            _tr.WriteLine("last segment \"{0}\"", file);
        }

        public static void Test_Uri_02()
        {
            //http://www.telechargement-plus.com/index.php?name=search
            string url = "http://www.telechargement-plus.com/toto/tata/index.php?name=search";
            _tr.WriteLine("url                  \"{0}\"", url);
            Uri uri = new Uri(url);
            _tr.WriteLine("uri.OriginalString   \"{0}\"", uri.OriginalString);
            _tr.WriteLine("uri.AbsoluteUri      \"{0}\"", uri.AbsoluteUri);
            _tr.WriteLine("uri.PathAndQuery     \"{0}\"", uri.PathAndQuery);
            _tr.WriteLine("uri.AbsolutePath     \"{0}\"", uri.AbsolutePath);
            _tr.WriteLine("uri.LocalPath        \"{0}\"", uri.LocalPath);
            _tr.WriteLine("uri.Query            \"{0}\"", uri.Query);
            _tr.WriteLine("uri.Segments         \"{0}\"", uri.Segments.zToStringValues());
            _tr.WriteLine();
            _tr.WriteLine("uri.Authority        \"{0}\"", uri.Authority);
            _tr.WriteLine("uri.DnsSafeHost      \"{0}\"", uri.DnsSafeHost);
            _tr.WriteLine("uri.Host             \"{0}\"", uri.Host);
            _tr.WriteLine("uri.HostNameType     \"{0}\"", uri.HostNameType);
            _tr.WriteLine();
            _tr.WriteLine("uri.IsAbsoluteUri    \"{0}\"", uri.IsAbsoluteUri);
            _tr.WriteLine("uri.IsDefaultPort    \"{0}\"", uri.IsDefaultPort);
            _tr.WriteLine("uri.IsFile           \"{0}\"", uri.IsFile);
            _tr.WriteLine("uri.IsLoopback       \"{0}\"", uri.IsLoopback);
            _tr.WriteLine("uri.IsUnc            \"{0}\"", uri.IsUnc);
            _tr.WriteLine("uri.UserEscaped      \"{0}\"", uri.UserEscaped);
            _tr.WriteLine();
            _tr.WriteLine("uri.Port             \"{0}\"", uri.Port);
            _tr.WriteLine("uri.Scheme           \"{0}\"", uri.Scheme);
            _tr.WriteLine();
            _tr.WriteLine("uri.Fragment         \"{0}\"", uri.Fragment);
            _tr.WriteLine("uri.UserInfo         \"{0}\"", uri.UserInfo);
        }

        public static void Test_XmlWriter_01()
        {
            string file = @"c:\pib\dev_data\exe\wrun\damien\export\test.xml";
            _tr.WriteLine("Test_XmlWriter_01 \"{0}\"", file);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement("Gesat");

                xw.WriteStartElement("Company");
                xw.zWriteElementText("Name", "ESAT BETTY LAUNAY-MOULIN VERT");
                xw.zWriteElementText("type", "E.S.A.T.");
                xw.zWriteElementText("activity", "Conditionnement, travaux à façon");
                xw.zWriteElementText("activity", "Assemblage, montage");
                xw.zWriteElementText("activity", "Mise sous pli, mailing, routage");
                xw.WriteEndElement();

                xw.WriteStartElement("Company");
                xw.zWriteElementText("Name", "ESAT COTRA");
                xw.zWriteElementText("type", "E.A.");
                xw.zWriteElementText("activity", "Toutes activités en entreprise");
                xw.zWriteElementText("activity", "Entretien et création d'espaces verts");
                xw.zWriteElementText("activity", "Petits travaux de bâtiment");
                xw.WriteEndElement();

                xw.WriteEndElement();
            }
        }

        // pas à jour utiliser Test_Gesat_loadHeader_01
        //public static void Test_Gesat_loadHeader_02(int startPage = 1, int maxPage = 1, bool loadImage = false)
        //{
        //    GesatInit();
        //    Gesat_LoadHeaderFromWebPages2 load = new Gesat_LoadHeaderFromWebPages2(startPage, maxPage, loadImage);
        //    _rs.View(load);
        //}

        public static void GesatInit()
        {
            /////////////////Http2.HtmlReader.WebEncoding = Encoding.UTF8;
            Gesat_LoadHeader.ClassInit(_rs.Config.GetElement("GesatHeader"));
            Gesat_LoadCompany.ClassInit(_rs.Config.GetElement("GesatCompany"));
        }


        //public static void Test_Gesat_ExportXmlCompanyList_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        //{
        //    _tr.WriteLine("Test_Gesat_ExportXmlCompanyList_01");
        //    GesatInit();
        //    Test_ExportXmlGesat_CompanyList_01(from header in new Gesat_LoadHeaderPages(startPage: startPage, maxPage: maxPage, reload: reload, loadImage: loadImage) select Gesat_LoadCompany.LoadCompany(header.url, header, reload: reload, loadImage: loadImage));
        //}

        public static void Test_Gesat_ExportXmlCompanyListFromWeb_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            _tr.WriteLine("Test_Gesat_ExportXmlCompanyListFromWeb_01");
            GesatInit();
            //Test_ExportXmlGesat_CompanyList_01(from header in new Gesat_LoadHeaderFromWebPages1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompanyFromWeb.LoadCompany(header.url, header, null, false, loadImage));
        }

        public static void Test_Gesat_ExportTextCompanyListFromWeb_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            _tr.WriteLine("Test_Gesat_ExportTextCompanyListFromWeb_01");
            GesatInit();
            //Test_ExportTextGesat_CompanyList_01(from header in new Gesat_LoadHeaderFromWebPages1(startPage: startPage, maxPage: maxPage) select Gesat_LoadCompanyFromWeb.LoadCompany(header.url, header, null, false, loadImage));
        }







        public static void Test_Gesat_loadCompany_01(string url, bool reload = false, bool loadImage = false)
        {
            //http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            _tr.WriteLine("Test_Gesat_loadCompany_01");
            GesatInit();
            Gesat_Company data = Gesat_LoadCompany.LoadCompany(url, null, reload, loadImage);
            //_rs.View(data);
            data.zView();
        }

        public static void Test_Gesat_LoadCompanyFromWeb_01()
        {
            //http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            _tr.WriteLine("Test_Gesat_LoadCompanyFromWeb_01");
            GesatInit();
            string url = "http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/";
            string urlFile = @"c:\pib\dev_data\exe\wrun\damien\cache\Gesat\esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837.html";
            bool reload = false;
            bool loadImage = false;
            Gesat_LoadCompanyFromWeb load = new Gesat_LoadCompanyFromWeb(url, null, urlFile, reload, loadImage);
            load.Load();
            //_rs.View(load.Data);
            load.Data.zView();
        }

        public static void Test_Gesat_LoadHeaderFromWeb_01()
        {
            //http://www.reseau-gesat.com/Gesat/
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            _tr.WriteLine("Test_Gesat_LoadHeaderFromWeb_01");
            GesatInit();
            string url = "http://www.reseau-gesat.com/Gesat/";
            //string urlFile = @"c:\pib\dev_data\exe\wrun\damien\cache\Gesat\esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837.html";
            string urlFile = null;
            bool reload = false;
            bool loadImage = false;
            Gesat_LoadHeaderFromWeb load = new Gesat_LoadHeaderFromWeb(url, urlFile, reload, loadImage);
            load.Load();
            //_rs.View(load);
            load.zView();
        }

        public static void Test_Gesat_LoadHeader_01()
        {
            //http://www.reseau-gesat.com/Gesat/
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            _tr.WriteLine("Test_Gesat_LoadHeader_01");
            GesatInit();
            string url = "http://www.reseau-gesat.com/Gesat/";
            //string urlFile = @"c:\pib\dev_data\exe\wrun\damien\cache\Gesat\esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837.html";
            bool reload = false;
            bool loadImage = false;
            Gesat_LoadHeader load = new Gesat_LoadHeader(url);
            load.Load(reload, loadImage);
            //_rs.View(load);
            load.zView();
        }

        public static void Test_Gesat_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            _tr.WriteLine("Test_Gesat_LoadHeader_01");
            GesatInit();
            Gesat_LoadHeaderPages load = new Gesat_LoadHeaderPages(startPage, maxPage, reload, loadImage);
            //_rs.View(load);
            load.zView();
        }

        public static void Test_Gesat_loadCompanyList_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            _tr.WriteLine("Test_Gesat_loadCompanyList_01");
            GesatInit();
            //_rs.View(from header in new Gesat_LoadHeaderPages(startPage: startPage, maxPage: maxPage, reload: reload, loadImage: loadImage) select Gesat_LoadCompany.LoadCompany(header.url, header, reload: reload, loadImage: loadImage));
            (from header in new Gesat_LoadHeaderPages(startPage: startPage, maxPage: maxPage, reload: reload, loadImage: loadImage) select Gesat_LoadCompany.LoadCompany(header.url, header, reload: reload, loadImage: loadImage)).zView();
        }
    }
}

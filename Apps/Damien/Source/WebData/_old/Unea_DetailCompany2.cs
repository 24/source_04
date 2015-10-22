using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;
using PB_Util;

namespace Download.Unea
{
    public class Unea_DetailCompany2
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;

        public string name = null;
        public string presentation = null;
        public SortedDictionary<string, string> activities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public SortedDictionary<string, Unea_Document> downloadDocuments = new SortedDictionary<string, Unea_Document>();

        public SortedDictionary<string, string> photos = new SortedDictionary<string, string>();

        public string address = null;
        public string phone = null;
        public string fax = null;
        public string email = null;
        public string webSite = null;

        public string leader = null; // dirigeant
        public int? employeNumber = null; // nombre de salarié
        public string lastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string siret = null;
        public string certification = null; // certification
        public string clients = null;

        public List<string> unknowInfos = new List<string>();
    }

    public class Unea_LoadDetailCompany2FromWeb : LoadDataFromWeb_v1<Unea_DetailCompany2>
    {
        // http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm
        private bool _loadImage = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path | UrlFileNameType.Query;
        //private static Func<string, string> __trimFunc1 = text => text.Trim();
        //private static Func<string, string> __trimFunc2 = text => text.Trim(' ', ':', '?', '\xA0', '-', '\t', '\r', '\n');
        //private static Func<string, string> __trimFunc2 = text => text.Trim(' ', '\xA0', '\t', '\r', '\n', ':', '?', '-');
        private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        private static Func<string, string> __trimFunc2 = text =>
        {
            text = text.Trim(' ', '\xA0', '\t', '\r', '\n', ':', '?', '-');
            text = __badCharacters.Replace(text, " ");
            return text;
        };

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
        }

        public Unea_LoadDetailCompany2FromWeb(string url, bool reload = false, bool loadImage = false)
            : base(url, reload: reload)
        {
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Unea_DetailCompany2 GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            Unea_DetailCompany2 data = new Unea_DetailCompany2();
            data.sourceUrl = Url;
            data.loadFromWebDate = DateTime.Now;

            // <div class='ctn_content-article'>
            XXElement xeContent = xeSource.XPathElement(".//div[@class='ctn_content-article']");

            //IEnumerator<string> texts = xeContent.DescendantTextList(nodeFilter: node => !(node is XElement) || (((XElement)node).Name != "script" && ((XElement)node).Name != "table"), func: __trimFunc2).GetEnumerator();
            IEnumerator<string> texts = xeContent.DescendantTexts(node => !(node is XElement) || (((XElement)node).Name != "script" && ((XElement)node).Name != "table") ? XNodeFilter.SelectNode : XNodeFilter.SkipNode).Select(__trimFunc2).GetEnumerator();

            // <h1>
            // <img src="http://unea.griotte.biz/BaseDocumentaire/Docs/Public/4017/LOGOAmpouleC.JPG" style='border-width:2px;border-color:#5593C9;' height='60px' /> 
            // <span>Entreprise Adapt&eacute;e</span><br />
            // ALSACE ENTREPRISE ADAPTEE
            // </h1>
            if (texts.MoveNext() && texts.MoveNext())
                data.name = texts.Current;

            // <h2>ALSACE ENTREPRISE ADAPTEE est implant&eacute;e sur les sites de Colmar et Mulhouse avec un effectif de 106 salari&eacute;s, avec les activit&eacute;s sous-traitance : assemblage de pi&egrave;ces, cintrage de tuyaux, montage complexe, ainsi qu'une activit&eacute; prestation de service en espaces verts, m&eacute;nage et transport.</h2>
            if (texts.MoveNext())
                data.presentation = texts.Current;

            Unea_TextType textType = Unea_TextType.unknow;
            //foreach (XText xtext in xeContent.DescendantTextNodeList(".//table"))
            foreach (XText xtext in xeContent.XPathElements(".//table").DescendantTextNodes())
            {
                string text = __trimFunc2(xtext.Value);
                if (text == "")
                    continue;
                if (text.Equals("NOS ACTIVITES", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.activity;
                else if (text.Equals("FILIERES METIER UNEA", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.sector;
                else if (text.Equals("DOCUMENTS TÉLÉCHARGEABLES", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (XXElement xe2 in new XXElement(xtext.Parent).XPathElements("following-sibling::ul//a"))
                    {
                        string url = xe2.XPathValue("@href");
                        //string name = name = xe2.XPathValue(".//text()", __trimFunc2);
                        string name = __trimFunc2(xe2.XPathValue(".//text()"));
                        if (!data.downloadDocuments.ContainsKey(url))
                            data.downloadDocuments.Add(url, new Unea_Document() { name = name, url = url });
                        else
                            Trace.CurrentTrace.WriteLine("warning download document already exists \"{0}\" \"{1}\"", name, url);
                    }
                    // textType = novalues pour ne pas avoir Plaquette_AEA.pdf dans unknowInfos
                    textType = Unea_TextType.novalues;
                }
                else if (text.Equals("NOUS CONTACTER", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.novalue;
                else if (text.Equals("ADRESSE", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.address;
                else if (text.Equals("TELEPHONE", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.phone;
                else if (text.Equals("FAX", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.fax;
                else if (text.Equals("EMAIL", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.email;
                else if (text.Equals("SITE", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.webSite;
                else if (text.Equals("QUI SOMMES NOUS", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.novalue;
                else if (text.Equals("DIRIGEANT", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.leader;
                else if (text.Equals("NOMBRE DE SALARIÉS", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.employeNumber;
                else if (text.Equals("CHIFFRE D'AFFAIRE DE L'ANNÉE ÉCOULÉE", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.lastYearRevenue;
                else if (text.Equals("NUMÉRO SIRET", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.siret;
                else if (text.Equals("CERTIFICATION", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.certification;
                else if (text.Equals("PRINCIPAUX CLIENTS", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.client;
                else
                {
                    switch (textType)
                    {
                        case Unea_TextType.activity:
                            if (!data.activities.ContainsKey(text))
                                data.activities.Add(text, null);
                            else
                                Trace.CurrentTrace.WriteLine("warning activity already exists \"{0}\"", text);
                            break;
                        case Unea_TextType.sector:
                            //data.sectors.Add(text);
                            if (!data.sectors.ContainsKey(text))
                                data.sectors.Add(text, null);
                            else
                                Trace.CurrentTrace.WriteLine("warning sector already exists \"{0}\"", text);
                            break;
                        case Unea_TextType.address:
                            if (data.address == null)
                                data.address = text;
                            else
                                data.address += " " + text;
                            break;
                        case Unea_TextType.phone:
                            data.phone = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.fax:
                            data.fax = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.email:
                            data.email = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.webSite:
                            data.webSite = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.leader:
                            data.leader = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.employeNumber:
                            int employeNumber;
                            if (int.TryParse(text, out employeNumber))
                                data.employeNumber = employeNumber;
                            else
                                Trace.CurrentTrace.WriteLine("error unknow employe number \"{0}\"", text);
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.lastYearRevenue:
                            if (text != "€")
                                data.lastYearRevenue = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.siret:
                            data.siret = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.certification:
                            data.certification = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.client:
                            data.clients = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.novalues:
                            break;
                        default:
                            data.unknowInfos.Add(text);
                            break;
                    }
                }
            }

            foreach (XXElement xe in xeContent.XPathElements(".//table//td/a/img"))
            {
                string url = xe.XPathValue("@src");
                if (!data.photos.ContainsKey(url))
                    data.photos.Add(url, null);
                else
                    Trace.CurrentTrace.WriteLine("warning photo already exists \"{0}\"", url);
            }

            return data;
        }

        public static Unea_DetailCompany2 LoadCompany(string url, bool reload = false, bool loadImage = false)
        {
            Unea_LoadDetailCompany2FromWeb load = new Unea_LoadDetailCompany2FromWeb(url, reload, loadImage);
            load.Load();
            return load.Data;
        }
    }

    public class Unea_LoadDetailCompany2 : LoadWebData_v1<Unea_DetailCompany2>
    {
        protected static bool __useXml = false;
        protected static bool __useMongo = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            //__imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            //__useXml = xe.zXPathValueBool("UseXml", __useXml);
            __useXml = xe.zXPathValue("UseXml").zTryParseAs(__useXml);
            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        protected Unea_LoadDetailCompany2(string url)
            : base(url)
        {
            //_imageCacheDirectory = __imageCacheDirectory;
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "Unea detail company 2";
        }

        protected override Unea_DetailCompany2 LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            return Unea_LoadDetailCompany2FromWeb.LoadCompany(Url, reload, loadImage);
        }

        public static Unea_DetailCompany2 LoadCompany(string url, bool reload = false, bool loadImage = false)
        {
            Unea_LoadDetailCompany2 load = new Unea_LoadDetailCompany2(url);
            load.Load(reload, loadImage);
            return load.Data;
        }
    }
}

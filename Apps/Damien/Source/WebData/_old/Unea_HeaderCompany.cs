using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;
using PB_Util;

namespace Download.Unea
{
    public class Unea_HeaderCompany
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;
        public string urlDetail1 = null;
        public string urlDetail2 = null;
        public string name = null;
        public string location = null;
        public string phone = null;
        public string fax = null;
        public string email = null;
        public SortedDictionary<string, string> activities = new SortedDictionary<string, string>();
        public List<string> unknowInfos = new List<string>();
    }

    public class Unea_LoadHeaderFromWeb : LoadDataFromWeb_v1<Unea_HeaderCompany[]>
    {
        private bool _loadImage = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.FileName | UrlFileNameType.Content;
        //private static Func<string, string> __trimFunc1 = text => text.Trim();
        private static Func<string, string> __trimFunc2 = text => text.Trim(' ', ':', '\xA0', '\t', '\r', '\n');
        //private static string __url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
        //private static string __referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true";
        //private static string __requestContentType = "application/x-www-form-urlencoded";

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
            //Trace.CurrentTrace.WriteLine("Unea_LoadHeaderFromWeb.ClassInit __useUrlCache {0}", __useUrlCache);
            //Trace.CurrentTrace.WriteLine("Unea_LoadHeaderFromWeb.ClassInit __cacheDirectory {0}", __cacheDirectory);
        }

        //public Unea_LoadHeaderFromWeb(string request, bool reload = false, bool loadImage = false)
        public Unea_LoadHeaderFromWeb(string url, HttpRequestParameters_v1 requestParameters, bool reload = false, bool loadImage = false)
            : base(url, requestParameters, reload)
        {
            //HttpRequestParameters requestParameters = new HttpRequestParameters();
            //requestParameters.method = HttpRequestMethod.Post;
            //requestParameters.content = request;
            //requestParameters.contentType = __requestContentType;
            //requestParameters.referer = __referer;
            //SetRequestParameters(requestParameters);
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Unea_HeaderCompany[] GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            string url = Url;
            // <div class="ctn_result">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class = 'ctn_result']");
            List<Unea_HeaderCompany> headers = new List<Unea_HeaderCompany>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Unea_HeaderCompany header = new Unea_HeaderCompany();
                header.sourceUrl = url;
                header.loadFromWebDate = DateTime.Now;

                // <div class="ctn_result-header">
                XXElement xe = xeHeader.XPathElement(".//div[@class='ctn_result-header']");

                // <div class="lien"><a href="http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4583/ACCAA TAKTIM.htm" target="_blank"><strong>></strong> Voir la fiche détaillée</a></div>
                header.urlDetail2 = zurl.GetUrl(url, xe.ExplicitXPathValue(".//div[@class = 'lien']//a/@href"));

                // <iframe src="detail.asp?id=4583" width="420" height="800" frameborder="0" scrolling="auto" marginheight="0" marginwidth="0"></iframe>
                header.urlDetail1 = zurl.GetUrl(url, xe.ExplicitXPathValue(".//iframe/@src"));

                // <h4><a href="http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4583/ACCAA TAKTIM.htm"  target="_blank">&nbsp;</a><span>|</span> ACCAA TAKTIM</h4>
                //header.name = xe.DescendantTextList(func: __trimFunc2).LastOrDefault();
                header.name = xe.DescendantTexts().Select(__trimFunc2).LastOrDefault();

                // <div class="ctn_result-content clearfix">
                // ...
                // <p>
                // <strong>Activités:</strong> TRAVAUX PAYSAGERS<br>PROPRETE<br>PRESTATION DE SERVICES<br>SOUS TRAITANCE INDUSTRIELLE<br>MECANIQUE<br>AUTOMOBILE<br>METALLURGIE<br />
                // <strong>Région - Département:</strong> Alsace - HAUT RHIN (68)<br />
                // <strong>Téléphone:</strong> 0389570210&nbsp;&nbsp;&nbsp;&nbsp;
                // <strong>Fax:</strong> 0389571761&nbsp;&nbsp;&nbsp;&nbsp;
                // <strong>Adresse e-mail:</strong>
                // <a href="mailto:info@alsace-ea.com">info@alsace-ea.com</a>
                // </p>
                // </div>
                Unea_TextType textType = Unea_TextType.unknow;
                //foreach (string s in xeHeader.DescendantTextList(".//div[@class = 'ctn_result-content clearfix']", func: __trimFunc2))
                foreach (string s in xeHeader.XPathElements(".//div[@class = 'ctn_result-content clearfix']").DescendantTexts().Select(__trimFunc2))
                {
                    if (s.Equals("Activités", StringComparison.InvariantCultureIgnoreCase))
                        textType = Unea_TextType.activity;
                    else if (s.Equals("Région - Département", StringComparison.InvariantCultureIgnoreCase))
                        textType = Unea_TextType.location;
                    else if (s.Equals("Téléphone", StringComparison.InvariantCultureIgnoreCase))
                        textType = Unea_TextType.phone;
                    else if (s.Equals("Fax", StringComparison.InvariantCultureIgnoreCase))
                        textType = Unea_TextType.fax;
                    else if (s.Equals("Adresse e-mail", StringComparison.InvariantCultureIgnoreCase))
                        textType = Unea_TextType.email;
                    else
                    {
                        switch (textType)
                        {
                            case Unea_TextType.activity:
                                if (!header.activities.ContainsKey(s))
                                    header.activities.Add(s, null);
                                break;
                            case Unea_TextType.location:
                                header.location = s;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.phone:
                                header.phone = s;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.fax:
                                header.fax = s;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.email:
                                header.email = s;
                                textType = Unea_TextType.unknow;
                                break;
                            default:
                                header.unknowInfos.Add(s);
                                break;
                        }
                    }
                }

                headers.Add(header);
            }
            return headers.ToArray();
        }
    }

    public class Unea_LoadHeader : LoadWebData_v1<Unea_HeaderCompany[]>
    {
        //protected static Regex __KeyRegex = new Regex("/Gesat/(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //protected static string __imageCacheDirectory = "image";
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

        public Unea_LoadHeader(string url, HttpRequestParameters_v1 requestParameters)
            : base(url, requestParameters)
        {
            //_imageCacheDirectory = __imageCacheDirectory;
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "Unea header";
        }

        protected override Unea_HeaderCompany[] LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            Unea_LoadHeaderFromWeb loadFromWeb = new Unea_LoadHeaderFromWeb(Url, RequestParameters, reload, loadImage);
            loadFromWeb.Load();
            return loadFromWeb.Data;
        }

        //protected override Unea_HeaderCompany[] LoadDocumentFromXml(string file, bool loadImage = false)
        //{
        //    //_data = loadFromXml.Data;
        //    throw new NotImplementedException();
        //}

        //protected override void SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        //{
        //    //string imageFile = null;
        //    //string fileDirectory = null;
        //    //if (saveImage)
        //    //{
        //    //    fileDirectory = GetFileDirectory();
        //    //    imageFile = GetImageFile();
        //    //}

        //    //_data.DocumentXmlSave(xw);
        //    throw new NotImplementedException();
        //}

        //protected override Unea_HeaderCompany[] LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        //{
        //    ////Trace.CurrentTrace.WriteLine("_DocumentMongoLoad loadImage {0}", loadImage);
        //    //_data = new Gesat_Company();
        //    //_data.DocumentMongoLoad(doc);
        //    throw new NotImplementedException();
        //}

        //protected override void SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        //{
        //    //string imageFile = null;
        //    //string fileDirectory = null;
        //    //if (saveImage)
        //    //{
        //    //    fileDirectory = GetFileDirectory();
        //    //    imageFile = GetImageFile();
        //    //}
        //    //_data.DocumentMongoSave(doc);
        //    throw new NotImplementedException();
        //}

        //protected override object GetDocumentKey()
        //{
        //    //http://www.reseau-gesat.com/Gesat/
        //    //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
        //    //Match match = __KeyRegex.Match(Url);
        //    //if (!match.Success)
        //    //    throw new PB_Util_Exception("key not found in url \"{0}\"", Url);
        //    //string key = match.Groups[1].Value;
        //    //if (key.EndsWith("/"))
        //    //    key = key.Substring(0, key.Length - 1);
        //    //if (key == "")
        //    //    key = "EtablissementList-0-10.html";
        //    //Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
        //    //return key;
        //    throw new NotImplementedException();
        //}
    }

    public class Unea_LoadHeaderPages : LoadWebDataPages_v1<Unea_HeaderCompany>
    {
        private IEnumerator<Unea_HeaderCompany> _enumerator = null;
        private int _page;
        //protected static string _urlHeader = "http://www.reseau-gesat.com/Gesat/";
        //protected static int _headersNumberByPage = 10;
        private static string __url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
        private static string __referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true";
        // default value of HttpRequestParameters.contentType is "application/x-www-form-urlencoded"
        //private static string __requestContentType = "application/x-www-form-urlencoded";
        private static string __sectorRequest = "hiddenValider=true&txtRecherche=&txtRecherche1={0}&txtRecherche2=&txtRecherche3=&txtRecherche4=";
        private static string[] __sectorCodes = { "Z" /*Agriculture*/, "1" /*Artisanat*/, "2" /*Automobile*/, "3" /*Bâtiment*/, "A" /*Blanchisserie*/, "B" /*Bois/ menuiserie*/,
                                                  "C" /*Bureautique / Prestation tertiaire*/, "D" /*Centre d'appel*/, "4" /*Commerce*/, "$" /*Communication*/,
                                                  "E" /*Conditionnement et logistique*/, "F" /*Contrôle / Qualité*/, "7" /*DEEE*/, "G" /*Electrique*/, "8" /*Electronique*/,
                                                  "9" /*Electrotechnique*/, "H" /*Gestion Electronique des Documents*/, "I" /*Hôtellerie / Restauration*/, "J" /*Impression*/,
                                                  "K" /*Industrie*/, "0" /*Informatique*/, "L" /*Location de salle*/, "X" /*Loisirs*/, "M" /*Maintenance*/,
                                                  "N" /*Marquage sur objet / textile*/, "O" /*Mécanique*/, "P" /*Métallurgie*/, "5" /*Plasturgie*/, "R" /*Prestation de services*/,
                                                  "S" /*Produits du terroir*/, "Q" /*Propreté*/, "T" /*Recyclage / tri*/, "U" /*Service funéraire*/, "6" /*Signalétique*/,
                                                  "V" /*Sous-traitance industrielle*/, "W" /*Textile*/, "Y" /*Travaux paysagers*/ };
        private static string __regionRequest = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2={0}&txtRecherche3=&txtRecherche4=";
        private static string[] __regionCodes = { "1" /*Alsace*/, "2" /*Aquitaine*/, "3" /*Auvergne*/, "4" /*Basse-Normandie*/, "5" /*Bourgogne*/, "6" /*Bretagne*/,
                                                  "7" /*Centre*/, "8" /*Champagne-Ardenne*/, "9" /*Corse*/, "10" /*Franche-Comté*/, "23" /*Guadeloupe*/,
                                                  "11" /*Haute-Normandie*/, "12" /*Ile-de-France*/, "25" /*La Réunion*/, "13" /*Languedoc-Roussillon*/, "14" /*Limousin*/,
                                                  "15" /*Lorraine*/, "26" /*Martinique*/, "16" /*Midi-Pyrénées*/, "17" /*Nord-Pas-de-Calais*/, "18" /*Pays-de-la-Loire*/,
                                                  "19" /*Picardie*/, "20" /*Poitou-Charentes*/, "21" /*Provence-Alpes-Côte-d'Azur*/, "22" /*Rhône-Alpes*/, "98" /*Territoires d'Outre-Mer*/ };
        private static string __departmentRequest = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=&txtRecherche3={0}&txtRecherche4=";
        private static string[] __departmentCodes = { "01" /*AIN*/, "02" /*AISNE*/, "03" /*ALLIER*/, "04" /*ALPES DE HAUTE PROVENCE*/, "06" /*ALPES MARITIMES*/, "07" /*ARDECHE*/,
                                                      "08" /*ARDENNES*/, "09" /*ARIEGE*/, "10" /*AUBE*/, "11" /*AUDE*/, "12" /*AVEYRON*/, "67" /*BAS RHIN*/, "13" /*BOUCHES DU RHONE*/,
                                                      "14" /*CALVADOS*/, "15" /*CANTAL*/, "16" /*CHARENTE*/, "17" /*CHARENTE MARITIME*/, "18" /*CHER*/, "19" /*CORREZE*/, "20" /*CORSE*/,
                                                      "21" /*COTE D'OR*/, "22" /*COTES D'ARMOR*/, "23" /*CREUSE*/, "79" /*DEUX SEVRES*/, "24" /*DORDOGNE*/, "25" /*DOUBS*/, "26" /*DROME*/,
                                                      "91" /*ESSONNE*/, "27" /*EURE*/, "28" /*EURE ET LOIR*/, "29" /*FINISTERE*/, "30" /*GARD*/, "32" /*GERS*/, "33" /*GIRONDE*/,
                                                      "971" /*GUADELOUPE*/, "68" /*HAUT RHIN*/, "31" /*HAUTE GARONNE*/, "43" /*HAUTE LOIRE*/, "52" /*HAUTE MARNE*/, "70" /*HAUTE SAONE*/,
                                                      "74" /*HAUTE SAVOIE*/, "87" /*HAUTE VIENNE*/, "05" /*HAUTES ALPES*/, "65" /*HAUTES PYRENEES*/, "92" /*HAUTS DE SEINE*/,
                                                      "34" /*HERAULT*/, "35" /*ILLE ET VILAINE*/, "36" /*INDRE*/, "37" /*INDRE ET LOIRE*/, "38" /*ISERE*/, "39" /*JURA*/,
                                                      "974" /*LA REUNION*/, "40" /*LANDES*/, "41" /*LOIR ET CHER*/, "42" /*LOIRE*/, "44" /*LOIRE ATLANTIQUE*/, "45" /*LOIRET*/,
                                                      "46" /*LOT*/, "47" /*LOT ET GARONNE*/, "48" /*LOZERE*/, "49" /*MAINE ET LOIRE*/, "50" /*MANCHE*/, "51" /*MARNE*/,
                                                      "972" /*MARTINIQUE*/, "53" /*MAYENNE*/, "54" /*MEURTHE ET MOSELLE*/, "56" /*MORBIHAN*/, "57" /*MOSELLE*/, "58" /*NIEVRE*/,
                                                      "59" /*NORD*/, "60" /*OISE*/, "61" /*ORNE*/, "75" /*PARIS*/, "62" /*PAS DE CALAIS*/, "987" /*POLYNÉSIE FRANÇAISE*/,
                                                      "63" /*PUY DE DOME*/, "64" /*PYRENEES ATLANTIQUES*/, "66" /*PYRENEES ORIENTALES*/, "69" /*RHONE*/, "71" /*SAONE ET LOIRE*/,
                                                      "72" /*SARTHE*/, "73" /*SAVOIE*/, "77" /*SEINE ET MARNE*/, "76" /*SEINE MARITIME*/, "93" /*SEINE SAINT DENIS*/, "80" /*SOMME*/,
                                                      "81" /*TARN*/, "82" /*TARN ET GARONNE*/, "90" /*TERRITOIRE DE BELFORT*/, "95" /*VAL D'OISE*/, "94" /*VAL DE MARNE*/, "83" /*VAR*/,
                                                      "84" /*VAUCLUSE*/, "85" /*VENDEE*/, "86" /*VIENNE*/, "88" /*VOSGES*/, "89" /*YONNE*/, "78" /*YVELINES*/ };
        private static string __activityRequest = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=&txtRecherche3=&txtRecherche4={0}";
        private static string[] __activityCodes = { "1" /*Blanchisserie*/, "9" /*Centre d'Appels*/, "2" /*Conditionnement et Logistique*/, "3" /*DEEE*/, "4" /*Electrique*/,
                                                    "5" /*Gestion Electronique des Documents*/, "6" /*Impression*/, "7" /*Métallurgie*/, "10" /*Propreté et services associés*/,
                                                    "8" /*Travaux paysagers*/ };

        public Unea_LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
            : base(startPage, maxPage, reload, loadImage)
        {
        }

        protected override void GetUrlPage(int page, out string url, out HttpRequestParameters_v1 requestParameters)
        {
            if (page > __regionCodes.Length)
                throw new PB_Util_Exception("error loading Unea header page {0} dont exist", page);
            _page = page;
            url = __url;
            requestParameters = new HttpRequestParameters_v1();
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = string.Format(__regionRequest, __regionCodes[page - 1]);
            //requestParameters.contentType = __requestContentType;
            requestParameters.referer = __referer;
        }

        protected override void Load()
        {
            Unea_LoadHeader load = new Unea_LoadHeader(Url, RequestParameters);
            load.Load(Reload, LoadImage);
            _enumerator = load.Data.AsEnumerable<Unea_HeaderCompany>().GetEnumerator();
        }

        protected override bool GetNextItem()
        {
            return _enumerator.MoveNext();
        }

        protected override Unea_HeaderCompany GetCurrentItem()
        {
            return _enumerator.Current;
        }

        protected override bool GetUrlNextPage(out string url, out HttpRequestParameters_v1 requestParameters)
        {
            url = null;
            requestParameters = null;
            if (++_page > __regionCodes.Length)
                return false;
            GetUrlPage(_page, out url, out requestParameters);
            return true;
        }
    }
}

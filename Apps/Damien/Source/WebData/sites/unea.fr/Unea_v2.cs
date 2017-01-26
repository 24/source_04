using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Text;
using pb.Web.Data;
using pb.Web.Html;
using pb.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

// unea.fr
//   - recherche :
//     - http://unea.fr/
//     - clic sur "rechercher" : http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire-unea.htm?idRechAnnuaire=Trouvez%20une%20EA...
//     - select alsace et clic sur "rechercher" :
//       url          : http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp
//       method       : "POST"
//       Content-Type : application/x-www-form-urlencoded
//       Referer      : http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=&hiddenValider=true
//       Content      : hiddenValider=true&txtRecherche=Par+Nom+Entreprise&txtRecherche1=&txtRecherche6=&txtRecherche5=&txtRecherche2=1&txtRecherche3=&txtRecherche4=
//                      txtRecherche : nom entreprise, txtRecherche1 : secteur, txtRecherche6 : section, txtRecherche5 : activité, txtRecherche2 : région, txtRecherche3 : département, txtRecherche4 : filière métier
//
//   - problème detail2 n'est pas accessible avec '+ DE VERT - Alsace - HAUT RHIN (68) http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=5106
//     http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/5106/'+_de_vert.htm
//     HTTP Error 404.11 - Not Found
//     envoi mail à info@unea.fr

namespace hts.WebData.Unea
{
    public class Unea_Header_v2 : IKeyDetail, INamedHttpRequest
    {
        //public string SourceUrl;
        public HttpRequest SourceHttpRequest;
        public DateTime? LoadFromWebDate = null;
        public string UrlDetail1 = null;              // http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4011
        public string UrlDetail2 = null;              // http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4011/adapei_du_bas-rhin_asea.htm
        public string Name = null;
        public string Location = null;
        public string Phone = null;
        public string Fax = null;
        public string Email = null;
        //public SortedDictionary<string, string> Activities = new SortedDictionary<string, string>();
        public Unea_Activity[] Activities = null;
        public List<string> UnknowInfos = new List<string>();

        //private static Regex _detailKey = new Regex(@"\?id=([0-9]+)", RegexOptions.Compiled);
        //public BsonValue GetDetailKey()
        //{
        //    // UrlDetail1 : http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4011
        //    // UrlDetail2 : http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4011/adapei_du_bas-rhin_asea.htm
        //    Match match = _detailKey.Match(new Uri(UrlDetail1).Query);
        //    if (!match.Success)
        //        throw new PBException($"can't find detail key in \"{UrlDetail1}\"");
        //    return int.Parse(match.Groups[1].Value);
        //}

        // IKeyDetail
        public BsonValue GetDetailKey()
        {
            return Unea_v2.GetDetailKey1(UrlDetail1);
        }

        // INamedHttpRequest
        public HttpRequest GetHttpRequest(string name)
        {
            if (name == "detail1")
                return new HttpRequest { Url = UrlDetail1 };
            else if (name == "detail2")
                return new HttpRequest { Url = UrlDetail2 };
            else
                throw new PBException($"unknow http request name \"{name}\"");
        }
    }

    public class Unea_HeaderDataPages : DataPages_v2<Unea_Header_v2>, IKeyData
    {
        public int Id;

        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class Unea_Detail_v2 : IKeyData
    {
        public int Id;
        public Unea_Detail1_v2 Detail1;
        public Unea_Detail2_v2 Detail2;

        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class Unea_Detail1_v2
    {
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        //   fiche_bloc no 1 : name, location, activities, sectors, unknowInfos
        public string Name = null;
        public string Location = null;
        //public SortedDictionary<string, string> Activities = new SortedDictionary<string, string>();
        public Unea_Activity[] Activities = null;
        public SortedDictionary<string, string> Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA

        //   fiche_bloc no 2 : presentation, clients, leader, employeNumber, lastYearRevenue, certification, siret, downloadDocuments, unknowInfos
        public string Presentation = null;
        public string Clients = null;
        public string Leader = null; // dirigeant
        public int? EmployeNumber = null; // nombre de salarié
        public string LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string Certification = null; // certification
        public string Siret = null;
        public SortedDictionary<string, string> Photos = new SortedDictionary<string, string>();
        public SortedDictionary<string, Unea_Document> DownloadDocuments = new SortedDictionary<string, Unea_Document>();

        //   fiche_bloc no 3 : address, phone, fax, email, webSite
        public string Address = null;
        public string Phone = null;
        public string Fax = null;
        public string Email = null;
        public string WebSite = null;

        public List<string> UnknowInfos = new List<string>();
    }

    public class Unea_Detail2_v2
    {
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Name = null;
        public string Presentation = null;
        //public SortedDictionary<string, string> Activities = new SortedDictionary<string, string>();
        public Unea_Activity[] Activities = null;
        public SortedDictionary<string, string> Sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA
        public SortedDictionary<string, Unea_Document> DownloadDocuments = new SortedDictionary<string, Unea_Document>();

        public SortedDictionary<string, string> Photos = new SortedDictionary<string, string>();

        public string Address = null;
        public string Phone = null;
        public string Fax = null;
        public string Email = null;
        public string WebSite = null;

        public string Leader = null; // dirigeant
        public int? EmployeNumber = null; // nombre de salarié
        public string LastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string Siret = null;
        public string Certification = null; // certification
        public string Clients = null;

        public List<string> UnknowInfos = new List<string>();
    }

    public enum Unea_TextType
    {
        unknow = 0,
        novalue = 1,
        novalues,
        activity,
        location,
        phone,
        fax,
        email,
        sector,
        presentation,
        client,
        leader,
        employeNumber,
        lastYearRevenue,
        certification,
        siret,
        //downloadDocument,
        address,
        webSite
    }

    public class Unea_Document
    {
        public string Name = null;
        public string Url = null;
    }

    public class Unea_Activity
    {
        public string Level1;
        public string Level2;
        public string Activity;
    }

    public class Unea_v2 : WebHeaderDetailMongoManagerBase_v3<Unea_Header_v2, Unea_Detail_v2>
    {
        private static string _configName = "Unea";
        private static Unea_v2 _current = null;
        private static string _urlMainPage = "http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
        private static Func<string, string> _trimFunc = text => text.Trim(' ', ':', '\xA0', '\t', '\r', '\n');
        private static Regex _replaceChars = new Regex("[\r\n]+", RegexOptions.Compiled);
        //private static Func<string, string> _replaceFunc = text => text.Replace('\x00', ' ');
        private static Func<string, string> _replaceFunc = text => _replaceChars.Replace(text, " ");

        //private static string _url = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp";
        //private static string _referer = "http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true";
        private static string _referer = "http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=&hiddenValider=true";
        //private static string _sectorRequest = "hiddenValider=true&txtRecherche=&txtRecherche1={0}&txtRecherche2=&txtRecherche3=&txtRecherche4=";
        //private static string[] _sectorCodes = { "Z" /*Agriculture*/, "1" /*Artisanat*/, "2" /*Automobile*/, "3" /*Bâtiment*/, "A" /*Blanchisserie*/, "B" /*Bois/ menuiserie*/,
        //                                         "C" /*Bureautique / Prestation tertiaire*/, "D" /*Centre d'appel*/, "4" /*Commerce*/, "$" /*Communication*/,
        //                                         "E" /*Conditionnement et logistique*/, "F" /*Contrôle / Qualité*/, "7" /*DEEE*/, "G" /*Electrique*/, "8" /*Electronique*/,
        //                                         "9" /*Electrotechnique*/, "H" /*Gestion Electronique des Documents*/, "I" /*Hôtellerie / Restauration*/, "J" /*Impression*/,
        //                                         "K" /*Industrie*/, "0" /*Informatique*/, "L" /*Location de salle*/, "X" /*Loisirs*/, "M" /*Maintenance*/,
        //                                         "N" /*Marquage sur objet / textile*/, "O" /*Mécanique*/, "P" /*Métallurgie*/, "5" /*Plasturgie*/, "R" /*Prestation de services*/,
        //                                         "S" /*Produits du terroir*/, "Q" /*Propreté*/, "T" /*Recyclage / tri*/, "U" /*Service funéraire*/, "6" /*Signalétique*/,
        //                                         "V" /*Sous-traitance industrielle*/, "W" /*Textile*/, "Y" /*Travaux paysagers*/ };
        private static string _regionRequest = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2={0}&txtRecherche3=&txtRecherche4=";
        private static string[] _regionCodes = { "1" /*Alsace*/, "2" /*Aquitaine*/, "3" /*Auvergne*/, "4" /*Basse-Normandie*/, "5" /*Bourgogne*/, "6" /*Bretagne*/,
                                                 "7" /*Centre*/, "8" /*Champagne-Ardenne*/, "9" /*Corse*/, "10" /*Franche-Comté*/, "23" /*Guadeloupe*/,
                                                 "11" /*Haute-Normandie*/, "12" /*Ile-de-France*/, "25" /*La Réunion*/, "13" /*Languedoc-Roussillon*/, "14" /*Limousin*/,
                                                 "15" /*Lorraine*/, "26" /*Martinique*/, "16" /*Midi-Pyrénées*/, "17" /*Nord-Pas-de-Calais*/, "18" /*Pays-de-la-Loire*/,
                                                 "19" /*Picardie*/, "20" /*Poitou-Charentes*/, "21" /*Provence-Alpes-Côte-d'Azur*/, "22" /*Rhône-Alpes*/, "98" /*Territoires d'Outre-Mer*/ };
        //private static string _departmentRequest = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=&txtRecherche3={0}&txtRecherche4=";
        //private static string[] _departmentCodes = { "01" /*AIN*/, "02" /*AISNE*/, "03" /*ALLIER*/, "04" /*ALPES DE HAUTE PROVENCE*/, "06" /*ALPES MARITIMES*/, "07" /*ARDECHE*/,
        //                                             "08" /*ARDENNES*/, "09" /*ARIEGE*/, "10" /*AUBE*/, "11" /*AUDE*/, "12" /*AVEYRON*/, "67" /*BAS RHIN*/, "13" /*BOUCHES DU RHONE*/,
        //                                             "14" /*CALVADOS*/, "15" /*CANTAL*/, "16" /*CHARENTE*/, "17" /*CHARENTE MARITIME*/, "18" /*CHER*/, "19" /*CORREZE*/, "20" /*CORSE*/,
        //                                             "21" /*COTE D'OR*/, "22" /*COTES D'ARMOR*/, "23" /*CREUSE*/, "79" /*DEUX SEVRES*/, "24" /*DORDOGNE*/, "25" /*DOUBS*/, "26" /*DROME*/,
        //                                             "91" /*ESSONNE*/, "27" /*EURE*/, "28" /*EURE ET LOIR*/, "29" /*FINISTERE*/, "30" /*GARD*/, "32" /*GERS*/, "33" /*GIRONDE*/,
        //                                             "971" /*GUADELOUPE*/, "68" /*HAUT RHIN*/, "31" /*HAUTE GARONNE*/, "43" /*HAUTE LOIRE*/, "52" /*HAUTE MARNE*/, "70" /*HAUTE SAONE*/,
        //                                             "74" /*HAUTE SAVOIE*/, "87" /*HAUTE VIENNE*/, "05" /*HAUTES ALPES*/, "65" /*HAUTES PYRENEES*/, "92" /*HAUTS DE SEINE*/,
        //                                             "34" /*HERAULT*/, "35" /*ILLE ET VILAINE*/, "36" /*INDRE*/, "37" /*INDRE ET LOIRE*/, "38" /*ISERE*/, "39" /*JURA*/,
        //                                             "974" /*LA REUNION*/, "40" /*LANDES*/, "41" /*LOIR ET CHER*/, "42" /*LOIRE*/, "44" /*LOIRE ATLANTIQUE*/, "45" /*LOIRET*/,
        //                                             "46" /*LOT*/, "47" /*LOT ET GARONNE*/, "48" /*LOZERE*/, "49" /*MAINE ET LOIRE*/, "50" /*MANCHE*/, "51" /*MARNE*/,
        //                                             "972" /*MARTINIQUE*/, "53" /*MAYENNE*/, "54" /*MEURTHE ET MOSELLE*/, "56" /*MORBIHAN*/, "57" /*MOSELLE*/, "58" /*NIEVRE*/,
        //                                             "59" /*NORD*/, "60" /*OISE*/, "61" /*ORNE*/, "75" /*PARIS*/, "62" /*PAS DE CALAIS*/, "987" /*POLYNÉSIE FRANÇAISE*/,
        //                                             "63" /*PUY DE DOME*/, "64" /*PYRENEES ATLANTIQUES*/, "66" /*PYRENEES ORIENTALES*/, "69" /*RHONE*/, "71" /*SAONE ET LOIRE*/,
        //                                             "72" /*SARTHE*/, "73" /*SAVOIE*/, "77" /*SEINE ET MARNE*/, "76" /*SEINE MARITIME*/, "93" /*SEINE SAINT DENIS*/, "80" /*SOMME*/,
        //                                             "81" /*TARN*/, "82" /*TARN ET GARONNE*/, "90" /*TERRITOIRE DE BELFORT*/, "95" /*VAL D'OISE*/, "94" /*VAL DE MARNE*/, "83" /*VAR*/,
        //                                             "84" /*VAUCLUSE*/, "85" /*VENDEE*/, "86" /*VIENNE*/, "88" /*VOSGES*/, "89" /*YONNE*/, "78" /*YVELINES*/ };
        //private static string _activityRequest = "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=&txtRecherche3=&txtRecherche4={0}";
        //private static string[] _activityCodes = { "1" /*Blanchisserie*/, "9" /*Centre d'Appels*/, "2" /*Conditionnement et Logistique*/, "3" /*DEEE*/, "4" /*Electrique*/,
        //                                           "5" /*Gestion Electronique des Documents*/, "6" /*Impression*/, "7" /*Métallurgie*/, "10" /*Propreté et services associés*/,
        //                                           "8" /*Travaux paysagers*/ };

        public static string ConfigName { get { return _configName; } }
        public static Unea_v2 Current { get { return _current; } }

        //public static Unea_v2 Create(bool test)
        public static Unea_v2 Create(XElement xe)
        {
            //if (test)
            //    Trace.WriteLine("{0} init for test", _configName);
            //XElement xe = GetConfigElement(test);

            _current = new Unea_v2();
            _current.HeaderPageNominalType = typeof(Unea_HeaderDataPages);
            _current.Create(xe, new NamedGetData<Unea_Detail_v2>[] { new NamedGetData<Unea_Detail_v2> { Name = "detail1", GetData = _current.GetDetailData1 }, new NamedGetData<Unea_Detail_v2> { Name = "detail2", GetData = _current.GetDetailData2 } });
            return _current;
        }

        //public static XElement GetConfigElement(bool test = false)
        //{
        //    string configName = _configName;
        //    if (test)
        //        configName += "_Test";
        //    return XmlConfig.CurrentConfig.GetElement(configName);
        //}

        // header get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override IEnumDataPages<Unea_Header_v2> GetHeaderPageData(HttpResult<string> httpResult)
        {
            //XXElement xeSource = httpResult.zGetXDocument().zXXElement();
            XXElement xeSource = HtmlToXmlManager.Current.GetXDocument(httpResult).zXXElement();
            string url = httpResult.Http.HttpRequest.Url;
            Unea_HeaderDataPages data = new Unea_HeaderDataPages();
            data.SourceHttpRequest = httpResult.Http.HttpRequest;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = GetPageKey(httpResult.Http.HttpRequest);


            // <div class="paginationControl">
            // page n    : <a href="/fournisseurs/rechercher/page/2#resultats">&gt;</a> |
            // last page : <span class="disabled">&gt;</span> |
            //data.UrlNextPage = zurl.RemoveFragment(zurl.GetUrl(url, xeSource.XPathValue("//div[@class='paginationControl']//*[position()=last()-1]/@href")));
            //HttpRequest httpRequest
            data.HttpRequestNextPage = GetHttpRequestNextPage(httpResult.Http.HttpRequest);

            // <div class="ctn_result">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class = 'ctn_result']");
            List<Unea_Header_v2> headers = new List<Unea_Header_v2>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Unea_Header_v2 header = new Unea_Header_v2();
                //header.SourceUrl = url;
                header.SourceHttpRequest = httpResult.Http.HttpRequest;
                header.LoadFromWebDate = DateTime.Now;

                // <div class="ctn_result-header">
                XXElement xe = xeHeader.XPathElement(".//div[@class='ctn_result-header']");

                // <div class="lien"><a href="http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4583/ACCAA TAKTIM.htm" target="_blank"><strong>></strong> Voir la fiche détaillée</a></div>
                header.UrlDetail2 = zurl.GetUrl(url, xe.ExplicitXPathValue(".//div[@class = 'lien']//a/@href"));

                // <iframe src="detail.asp?id=4583" width="420" height="800" frameborder="0" scrolling="auto" marginheight="0" marginwidth="0"></iframe>
                header.UrlDetail1 = zurl.GetUrl(url, xe.ExplicitXPathValue(".//iframe/@src"));

                // <h4><a href="http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4583/ACCAA TAKTIM.htm"  target="_blank">&nbsp;</a><span>|</span> ACCAA TAKTIM</h4>
                header.Name = xe.DescendantTexts().Select(_trimFunc).LastOrDefault();

                GetHeaderTextInfos(header, xeHeader.XPathElements(".//div[@class = 'ctn_result-content clearfix']").DescendantTexts().Select(_trimFunc).Select(_replaceFunc));

                headers.Add(header);
            }
            data.Data = headers.ToArray();
            return data;
        }

        private void GetHeaderTextInfos(Unea_Header_v2 header, IEnumerable<string> texts)
        {
            Unea_TextType textType = Unea_TextType.unknow;
            List<Unea_Activity> activities = new List<Unea_Activity>();
            foreach (string text in texts)
            {
                if (text.Equals("Activités", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.activity;
                else if (text.Equals("Région - Département", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.location;
                else if (text.Equals("Téléphone", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.phone;
                else if (text.Equals("Fax", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.fax;
                else if (text.Equals("Adresse e-mail", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.email;
                else
                {
                    switch (textType)
                    {
                        case Unea_TextType.activity:
                            //if (!header.Activities.ContainsKey(text))
                            //    header.Activities.Add(text, null);
                            activities.Add(new Unea_Activity { Level1 = text });
                            break;
                        case Unea_TextType.location:
                            header.Location = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.phone:
                            header.Phone = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.fax:
                            header.Fax = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.email:
                            header.Email = text;
                            textType = Unea_TextType.unknow;
                            break;
                        default:
                            header.UnknowInfos.Add(text);
                            break;
                    }
                }
            }
            header.Activities = activities.ToArray();
        }

        // header get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        private static Regex _pageKey = new Regex("&txtRecherche2=([0-9]+)", RegexOptions.Compiled);
        private static int GetPageKey(HttpRequest httpRequest)
        {
            // content : "hiddenValider=true&txtRecherche=&txtRecherche1=&txtRecherche2=1&txtRecherche3=&txtRecherche4="
            // txtRecherche2 contain region code
            if (httpRequest.Url != _urlMainPage)
                throw new PBException($"bad header page url \"{httpRequest.Url}\"");
            Match match = _pageKey.Match(httpRequest.Content);
            if (!match.Success)
                throw new PBException($"bad header page content \"{httpRequest.Content}\"");
            return int.Parse(match.Groups[1].Value);
        }

        // header get url page, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            if (page < 1 || page > _regionCodes.Length)
                throw new PBException($"wrong page number {page}");
            return new HttpRequest { Url = _urlMainPage, Method = HttpRequestMethod.Post, Referer = _referer, Content = string.Format(_regionRequest, _regionCodes[page - 1]) };
        }

        private HttpRequest GetHttpRequestNextPage(HttpRequest httpRequest)
        {
            //if (request.Url != _urlMainPage)
            //    throw new PBException($"wrong url can't get next page \"{request.Url}\"");
            string key = GetPageKey(httpRequest).ToString();
            int page = 0;
            for (int i = 0; i < _regionCodes.Length; i++)
            {
                if (_regionCodes[i] == key)
                {
                    page = i + 1;
                    break;
                }
            }
            if (page == 0)
                throw new PBException($"wrong region code can't get next page \"{key}\"");
            if (page < _regionCodes.Length)
                return GetHttpRequestPage(page + 1);
            else
                return null;
        }

        // detail cache get sub-directory, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        // attention GetDetailCacheUrlSubDirectory() est appelé pour tous les types de detail
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return (GetDetailKey(httpRequest.Url) / 100 * 100).ToString();
        }

        protected override Unea_Detail_v2 CreateDetailData()
        {
            return new Unea_Detail_v2();
        }

        private void GetDetailData1(Unea_Detail_v2 data, HttpResult<string> httpResult)
        {
            // UrlDetail1 : http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4011

            //Trace.WriteLine($"Unea_v2.GetDetailData1() : url \"{httpResult.Http.HttpRequest.Url}\"");

            //XXElement xeSource = httpResult.zGetXDocument().zXXElement();
            XXElement xeSource = HtmlToXmlManager.Current.GetXDocument(httpResult).zXXElement();

            Unea_Detail1_v2 detail1 = new Unea_Detail1_v2();
            detail1.SourceUrl = httpResult.Http.HttpRequest.Url;
            detail1.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = GetDetailKey1(httpResult.Http.HttpRequest.Url);

            // <div class="fiche">
            XXElement xeFiche = xeSource.XPathElement(".//div[@class='fiche']");
            GetDetail1TextInfos(detail1, xeFiche.XPathElements(".//div[@class='fiche_bloc']").DescendantTextNodes());

            //Trace.WriteLine($"    Name : \"{detail1.Name}\"");
            //Trace.WriteLine($"    Presentation : \"{detail1.Presentation}\"");

            data.Detail1 = detail1;
        }

        private void GetDetail1TextInfos(Unea_Detail1_v2 data, IEnumerable<XText> textNodes)
        {
            bool firstText = true;
            Unea_TextType textType = Unea_TextType.unknow;
            List<Unea_Activity> activities = new List<Unea_Activity>();
            foreach (XText textNode in textNodes)
            {
                string text = _trimFunc(textNode.Value);
                text = _replaceFunc(text);
                if (text == "")
                    continue;

                // fiche_bloc no 1
                if (text.Equals("Activités", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.activity;
                else if (text.Equals("Région - Département", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.location;
                else if (text.Equals("Filières Métiers UNEA", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.sector;
                // fiche_bloc no 2
                else if (text.Equals("Présentation de l'Entreprise Adaptée", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.unknow;
                else if (text.Equals("Présentation", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.presentation;
                else if (text.Equals("Principaux clients", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.client;
                else if (text.Equals("Dirigeant", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.leader;
                else if (text.Equals("Nombre de salariés", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.employeNumber;
                else if (text.Equals("Chiffre d'affaire de l'année écoulée", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.lastYearRevenue;
                else if (text.Equals("Certification", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.certification;
                else if (text.Equals("Numéro SIRET", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.siret;
                else if (text.Equals("Photos", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (XXElement xe2 in new XXElement(textNode.Parent).XPathElements("ancestor::p/following-sibling::p//img"))
                    {
                        string url = xe2.XPathValue("@src");
                        if (!data.Photos.ContainsKey(url))
                            data.Photos.Add(url, null);
                        else
                            Trace.WriteLine("warning photo already exists \"{0}\"", url);
                    }
                    textType = Unea_TextType.novalues;
                }
                else if (text.Equals("Documents téléchargeables", StringComparison.InvariantCultureIgnoreCase))
                {
                    bool stop = false;
                    foreach (XXElement xe2 in new XXElement(textNode.Parent).XPathElements("ancestor::p/following-sibling::p").Where(
                        e =>
                        {
                            if (e.XElement.Value.StartsWith("Photos", StringComparison.InvariantCultureIgnoreCase))
                                stop = true;
                            return !stop;
                        }))
                    {
                        XXElement xe3 = xe2.XPathElement(".//a", writeError: false);
                        if (xe3.XElement == null)
                            continue;
                        string url = xe3.XPathValue("@href");
                        string name = _trimFunc(xe3.XPathValue(".//text()"));
                        if (!data.DownloadDocuments.ContainsKey(url))
                            data.DownloadDocuments.Add(url, new Unea_Document() { Name = name, Url = url });
                        else
                            Trace.WriteLine("warning download document already exists \"{0}\" \"{1}\"", name, url);
                    }
                    // textType = novalues pour ne pas avoir Plaquette_AEA.pdf dans unknowInfos
                    textType = Unea_TextType.novalues;
                }
                // fiche_bloc no 3
                else if (text.Equals("Nous contacter", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.unknow;
                else if (text.Equals("Adresse", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.address;
                else if (text.Equals("Téléphone", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.phone;
                else if (text.Equals("Fax", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.fax;
                else if (text.Equals("Adresse e-mail", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.email;
                else if (text.Equals("Site internet", StringComparison.InvariantCultureIgnoreCase))
                    textType = Unea_TextType.webSite;
                else
                {
                    switch (textType)
                    {
                        // fiche_bloc no 1
                        case Unea_TextType.activity:
                            //Unea_Activity activities Activities2
                            //if (!data.Activities.ContainsKey(text))
                            //    data.Activities.Add(text, null);
                            //else
                            //    Trace.WriteLine("warning activity already exists \"{0}\"", text);
                            string[] activity = zsplit.Split(text, ':', true);
                            if (activity.Length == 2)
                                activities.Add(new Unea_Activity { Level1 = activity[0], Activity = activity[1] });
                            else
                                Trace.WriteLine($"warning wrong activity format \"{text}\"");
                            break;
                        case Unea_TextType.location:
                            data.Location = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.sector:
                            if (!data.Sectors.ContainsKey(text))
                                data.Sectors.Add(text, null);
                            else
                                Trace.WriteLine("warning sector already exists \"{0}\"", text);
                            break;
                        // fiche_bloc no 2
                        case Unea_TextType.presentation:
                            data.Presentation = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.client:
                            data.Clients = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.leader:
                            data.Leader = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.employeNumber:
                            int employeNumber;
                            if (int.TryParse(text, out employeNumber))
                                data.EmployeNumber = employeNumber;
                            else
                                Trace.WriteLine("error unknow employe number \"{0}\"", text);
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.lastYearRevenue:
                            data.LastYearRevenue = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.certification:
                            data.Certification = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.siret:
                            data.Siret = text;
                            textType = Unea_TextType.unknow;
                            break;
                        // fiche_bloc no 3
                        case Unea_TextType.address:
                            if (data.Address == null)
                                data.Address = text;
                            else
                                data.Address += " " + text;
                            break;
                        case Unea_TextType.phone:
                            data.Phone = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.fax:
                            data.Fax = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.email:
                            data.Email = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.webSite:
                            data.WebSite = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.novalue:
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.novalues:
                            break;
                        default:
                            if (firstText)
                            {
                                data.Name = text;
                                firstText = false;
                            }
                            else
                                data.UnknowInfos.Add(text);
                            break;
                    }
                }
            }
            data.Activities = activities.ToArray();
        }

        private void GetDetailData2(Unea_Detail_v2 data, HttpResult<string> httpResult)
        {
            // UrlDetail2 : http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4011/adapei_du_bas-rhin_asea.htm

            //Trace.WriteLine($"Unea_v2.GetDetailData2() : url \"{httpResult.Http.HttpRequest.Url}\"");

            //XXElement xeSource = httpResult.zGetXDocument().zXXElement();
            XXElement xeSource = HtmlToXmlManager.Current.GetXDocument(httpResult).zXXElement();

            Unea_Detail2_v2 detail2 = new Unea_Detail2_v2();
            detail2.SourceUrl = httpResult.Http.HttpRequest.Url;
            detail2.LoadFromWebDate = httpResult.Http.RequestTime;

            // <div class='ctn_content-article'>
            XXElement xeContent = xeSource.XPathElement("//div[@class='ctn_content-article']");

            //GetDetailData2_Header_v1(detail2, xeContent);
            GetDetailData2_Header_v2(detail2, xeContent);

            detail2.Activities = GetDetail2Activities(xeContent.XPathElement(".//table//*[text() = 'NOS ACTIVITES']/following::div/ul"));

            GetDetail2TextInfos(detail2, xeContent.XPathElements(".//table").DescendantTextNodes());

            foreach (XXElement xe in xeContent.XPathElements(".//table//td/a/img"))
            {
                string url = xe.XPathValue("@src");
                if (!detail2.Photos.ContainsKey(url))
                    detail2.Photos.Add(url, null);
                else
                    Trace.WriteLine("warning photo already exists \"{0}\"", url);
            }

            data.Detail2 = detail2;
        }

        // public used to test
        public static Unea_Activity[] GetDetail2Activities(XXElement xe)
        {
            List<Unea_Activity> activities = new List<Unea_Activity>();
            //Unea_Activity activity = null;
            string level1 = null;
            bool level1Used = false;
            foreach (XXElement xe2 in xe.XPathElements("*"))
            {
                if (xe2.XElement.Name == "li")
                {
                    //if (activity != null)
                    //    activities.Add(activity);
                    //activity = new Unea_Activity();
                    //activity.Level1 = xe2.XElement.Value;

                    if (level1 != null && !level1Used)
                        activities.Add(new Unea_Activity { Level1 = level1 });
                    level1 = xe2.XElement.Value;
                    level1Used = false;
                }
                else if (xe2.XElement.Name == "ul")
                {
                    //if (activity == null)
                    //{
                    //    Trace.WriteLine("warning reading detail2 activity missing <li>");
                    //    activity = new Unea_Activity();
                    //}
                    string level2 = null;
                    bool level2Used = false;
                    foreach (XXElement xe3 in xe2.XPathElements("*"))
                    {
                        if (xe3.XElement.Name == "li")
                        {
                            //activity.Level2 = xe3.XElement.Value;
                            if (level2 != null && !level2Used)
                            {
                                activities.Add(new Unea_Activity { Level1 = level1, Level2 = level2 });
                                level1Used = true;
                            }
                            level2 = xe3.XElement.Value;
                            level2Used = false;
                        }
                        else if (xe3.XElement.Name == "ul")
                        {
                            //activity.Activity = xe3.XPathValue("li//text()");
                            foreach (XXElement xe4 in xe3.XPathElements("li"))
                            {
                                activities.Add(new Unea_Activity { Level1 = level1, Level2 = level2, Activity = xe4.XElement.Value });
                                level1Used = true;
                                level2Used = true;
                            }
                        }
                    }
                    if (level2 != null && !level2Used)
                    {
                        activities.Add(new Unea_Activity { Level1 = level1, Level2 = level2 });
                        level1Used = true;
                    }
                }
            }
            if (level1 != null && !level1Used)
                activities.Add(new Unea_Activity { Level1 = level1 });
            return activities.ToArray();
        }

        private void GetDetailData2_Header_v2(Unea_Detail2_v2 detail2, XXElement xeContent)
        {
            IEnumerator<string> texts = xeContent.XPathElement("h1").DescendantTexts().Select(_trimFunc).Select(_replaceFunc).GetEnumerator();
            if (texts.MoveNext())
            {
                if (string.Compare(texts.Current, "Entreprise Adaptée", true) == 0)
                {
                    if (texts.MoveNext())
                        detail2.Name = texts.Current;
                }
                else
                    detail2.Name = texts.Current;
            }
            detail2.Presentation = xeContent.XPathValue("h2/text()");
            if (detail2.Presentation != null)
                detail2.Presentation = _replaceFunc(_trimFunc(detail2.Presentation));
        }

        private void GetDetailData2_Header_v1(Unea_Detail2_v2 detail2, XXElement xeContent)
        {
            //IEnumerator<string> texts = xeContent.DescendantTextList(nodeFilter: node => !(node is XElement) || (((XElement)node).Name != "script" && ((XElement)node).Name != "table"), func: __trimFunc2).GetEnumerator();
            IEnumerator<string> texts = xeContent.DescendantTexts(node => !(node is XElement) || (((XElement)node).Name != "script" && ((XElement)node).Name != "table") ? XNodeFilter.SelectNode : XNodeFilter.SkipNode)
                .Select(_trimFunc).Select(_replaceFunc).GetEnumerator();

            // <h1>
            // <img src="http://unea.griotte.biz/BaseDocumentaire/Docs/Public/4017/LOGOAmpouleC.JPG" style='border-width:2px;border-color:#5593C9;' height='60px' /> 
            // <span>Entreprise Adapt&eacute;e</span><br />
            // ALSACE ENTREPRISE ADAPTEE
            // </h1>
            if (texts.MoveNext() && texts.MoveNext())
                detail2.Name = texts.Current;

            //Trace.WriteLine($"    Name : \"{detail2.Name}\"");

            // <h2>ALSACE ENTREPRISE ADAPTEE est implant&eacute;e sur les sites de Colmar et Mulhouse avec un effectif de 106 salari&eacute;s, avec les activit&eacute;s sous-traitance : assemblage de pi&egrave;ces, cintrage de tuyaux, montage complexe, ainsi qu'une activit&eacute; prestation de service en espaces verts, m&eacute;nage et transport.</h2>
            if (texts.MoveNext())
                detail2.Presentation = texts.Current;
        }

        private Regex _yearRevenuePattern = new Regex(@"CHIFFRE\s*D'AFFAIRE\s*([0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private int? _yearRevenue = null;
        private void GetDetail2TextInfos(Unea_Detail2_v2 data, IEnumerable<XText> textNodes)
        {
            Unea_TextType textType = Unea_TextType.unknow;
            foreach (XText xtext in textNodes)
            {
                string text = _trimFunc(xtext.Value);
                text = _replaceFunc(text);
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
                        string name = _trimFunc(xe2.XPathValue(".//text()"));
                        if (!data.DownloadDocuments.ContainsKey(url))
                            data.DownloadDocuments.Add(url, new Unea_Document() { Name = name, Url = url });
                        else
                            Trace.WriteLine("warning download document already exists \"{0}\" \"{1}\"", name, url);
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
                else if (text.Equals("QUI SOMMES NOUS", StringComparison.InvariantCultureIgnoreCase) || text.Equals("QUI SOMMES NOUS ?", StringComparison.InvariantCultureIgnoreCase))
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
                    Match match = _yearRevenuePattern.Match(text);
                    if (match.Success)
                    {
                        textType = Unea_TextType.lastYearRevenue;
                        _yearRevenue = int.Parse(match.Groups[1].Value);
                    }
                    else
                    {
                        switch (textType)
                        {
                            case Unea_TextType.activity:
                                //if (!data.Activities.ContainsKey(text))
                                //    data.Activities.Add(text, null);
                                //else
                                //    Trace.WriteLine("warning activity already exists \"{0}\"", text);
                                break;
                            case Unea_TextType.sector:
                                //data.sectors.Add(text);
                                if (!data.Sectors.ContainsKey(text))
                                    data.Sectors.Add(text, null);
                                else
                                    Trace.WriteLine("warning sector already exists \"{0}\"", text);
                                break;
                            case Unea_TextType.address:
                                if (data.Address == null)
                                    data.Address = text;
                                else
                                    data.Address += " " + text;
                                break;
                            case Unea_TextType.phone:
                                data.Phone = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.fax:
                                data.Fax = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.email:
                                data.Email = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.webSite:
                                data.WebSite = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.leader:
                                data.Leader = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.employeNumber:
                                int employeNumber;
                                if (int.TryParse(text, out employeNumber))
                                    data.EmployeNumber = employeNumber;
                                else
                                    Trace.WriteLine("error unknow employe number \"{0}\"", text);
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.lastYearRevenue:
                                if (text != "€")
                                {
                                    if (_yearRevenue != null)
                                        text = _yearRevenue.ToString() + " : " + text;
                                    data.LastYearRevenue = text;
                                }
                                textType = Unea_TextType.unknow;
                                _yearRevenue = null;
                                break;
                            case Unea_TextType.siret:
                                data.Siret = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.certification:
                                data.Certification = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.client:
                                data.Clients = text;
                                textType = Unea_TextType.unknow;
                                break;
                            case Unea_TextType.novalues:
                                break;
                            default:
                                data.UnknowInfos.Add(text);
                                break;
                        }
                    }
                }
            }
        }

        // detail get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        //protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        //{
        //    return _GetDetailKey(httpRequest);
        //}

        private static int GetDetailKey(string url)
        {
            Uri uri = new Uri(url);
            if (uri.AbsolutePath.StartsWith("/union-nationale-entreprises-adaptees/annuaire-unea/71/71/"))
                return GetDetailKey1(url);
            else
                return GetDetailKey2(url);
        }

        private static Regex _detailKey1 = new Regex(@"\?id=([0-9]+)", RegexOptions.Compiled);
        // IKeyDetail
        public static int GetDetailKey1(string url)
        {
            // UrlDetail1 : http://unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4011
            Match match = _detailKey1.Match(new Uri(url).Query);
            if (!match.Success)
                throw new PBException($"can't find detail key 1 in \"{url}\"");
            return int.Parse(match.Groups[1].Value);
        }

        private static int GetDetailKey2(string url)
        {
            // UrlDetail2 : http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4011/adapei_du_bas-rhin_asea.htm
            Uri uri = new Uri(url);
            string segment = uri.Segments[uri.Segments.Length - 2];
            if (segment.EndsWith("/"))
                segment = segment.Substring(0, segment.Length - 1);
            int id;
            if (uri.Segments.Length >= 2 && int.TryParse(segment, out id))
                return id;
            else
                throw new PBException($"unable to get id from \"{url}\"");

        }
    }
}

using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Web.Data;
using pb.Web.Data.Mongo;
using pb.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace hts.WebData.Handeco
{
    public class Handeco_Header_v2 : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        //public string Title;
        public string UrlDetail;

        //public WebImage[] Images;

        public string Name = null;
        //public string Siret = null;
        public string Type = null;
        public string[] Groupes = null;
        public string[] Activités = null;
        public string PostalCode = null;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class Handeco_HeaderDataPages : DataPages<Handeco_Header_v2>, IKeyData
    {
        public int Id;

        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class Activity
    {
        public string Type;                               // html tag select
        public string Description;                        // description
        public string MoyensTechniquesDisponibles;        // moyens techniques disponibles
        public string EffectifTotalMobilisable;           // effectif total mobilisable (etp)
        public string ModalitésPratiques;                 // modalités pratiques
        public string CouvertureGéographique;             // couverture géographique
    }

    public class Contact
    {
        public string Description;       // html tag select
        public string Nom;               // prénom et nom
        public string Fonction;          // fonction
        public string Tel;               // téléphone
        public string Mobile;            // mobile
        public string Email;             // e-mail
    }

    public class Handeco_Detail_v2 : IKeyData  //, IHttpRequestData // IKeyData_v4<int> // IPost, IWebData
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public DateTime? DernièreMiseàjour;                                    // Dernière mise à jour le 18-01-2013

        // LES INFOS CLES (html)
        public string Logo;                                                     // logo
        public string RaisonSociale;                                            // raison sociale
        public string DateCréation;                                             // date de création
        public string StatutJuridique;                                          // statut juridique
        public string TypeStructure;                                            // type de structure
        public string SiteWeb;                                                  // site web
        public string Siret;                                                    // n° siret
        public string Localisation;                                             // localisation géographique
        public string Normes;                                                   // normes, habilitations et certifications
        public string ChiffreAffairesAnnuel;                                    // chiffre d'affaires annuel
        public string EffectifTotal;                                            // effectif total (etp)
        public string EffectifProduction;                                       // effectif de production (etp)
        public string EffectifEncadrement;                                      // effectif d'encadrement (etp)
        public string NombreTravailleursHandicapés;                             // nombre de travailleurs handicapés (etp)
        public string NombreHandicapéAccompagné;                                // nombre de personnes handicapées accompagnées par an

        // RESEAUX ET PARTENAIRES (html)
        //public string[] adhésionGroupements;                                    // adhésion à des groupements et fédérations professionnels
        public string[] Groupes;                                                // adhésion à des groupements et fédérations professionnels
        public string AppartenanceGroupe;                                       // appartenance à un groupe
        public string PrésentationGroupe;                                       // présentation du groupe
        public string SiteWebGroupe;                                            // site web du groupe
        public string AdhésionRéseauxHandicap;                                  // adhésion à des réseaux du handicap
        //public string adhésionGroupement;                                     // adhésion à des groupements et fédérations professionnels
        public string Cotraitance;                                              // expérience de co-traitance ou de gme avec

        // NOS COORDONNEES (html)
        public string AdressePrincipale;                                        // adresse principale
        public string AdresseSiège;                                             // adresse du siège
        public string AdresseAntennes;                                          // adresse des antennes
        public string Email;                                                    // e-mail
        public string Tel;                                                      // tél
        public string Fax;                                                      // fax
        public string CodeApe;                                                  // code ape
        public string NumeroFiness;                                             // n° finess

        public Activity[] Activités;
        public Contact[] Contacts;
        public List<string> UnknowInfos = new List<string>();

        // IKeyData
        public BsonValue GetKey()
        {
            return Id;
        }
    }

    public class Handeco_v2 : WebHeaderDetailMongoManagerBase_v2<Handeco_Header_v2, Handeco_Detail_v2>
    {
        private static string _configName = "Handeco";
        private static Handeco_v2 _current = null;
        private static string _urlMainPage = "https://www.handeco.org/fournisseurs/rechercher";
        private static string _headerRequestContent = "raisonSociale=&SIRET=&departements%5B%5D=67&departements%5B%5D=68&departements%5B%5D=24&departements%5B%5D=33&departements%5B%5D=40&departements%5B%5D=47&departements%5B%5D=64&departements%5B%5D=03&departements%5B%5D=15&departements%5B%5D=43&departements%5B%5D=63&departements%5B%5D=14&departements%5B%5D=50&departements%5B%5D=61&departements%5B%5D=21&departements%5B%5D=58&departements%5B%5D=71&departements%5B%5D=89&departements%5B%5D=22&departements%5B%5D=29&departements%5B%5D=35&departements%5B%5D=56&departements%5B%5D=18&departements%5B%5D=28&departements%5B%5D=36&departements%5B%5D=37&departements%5B%5D=41&departements%5B%5D=45&departements%5B%5D=08&departements%5B%5D=10&departements%5B%5D=51&departements%5B%5D=52&departements%5B%5D=2A&departements%5B%5D=2B&departements%5B%5D=25&departements%5B%5D=39&departements%5B%5D=70&departements%5B%5D=90&departements%5B%5D=27&departements%5B%5D=76&departements%5B%5D=75&departements%5B%5D=77&departements%5B%5D=78&departements%5B%5D=91&departements%5B%5D=92&departements%5B%5D=93&departements%5B%5D=94&departements%5B%5D=95&departements%5B%5D=11&departements%5B%5D=30&departements%5B%5D=34&departements%5B%5D=48&departements%5B%5D=66&departements%5B%5D=19&departements%5B%5D=23&departements%5B%5D=87&departements%5B%5D=54&departements%5B%5D=55&departements%5B%5D=57&departements%5B%5D=88&departements%5B%5D=09&departements%5B%5D=12&departements%5B%5D=31&departements%5B%5D=32&departements%5B%5D=46&departements%5B%5D=65&departements%5B%5D=81&departements%5B%5D=82&departements%5B%5D=59&departements%5B%5D=62&departements%5B%5D=44&departements%5B%5D=49&departements%5B%5D=53&departements%5B%5D=72&departements%5B%5D=85&departements%5B%5D=02&departements%5B%5D=60&departements%5B%5D=80&departements%5B%5D=16&departements%5B%5D=17&departements%5B%5D=79&departements%5B%5D=86&departements%5B%5D=04&departements%5B%5D=05&departements%5B%5D=06&departements%5B%5D=13&departements%5B%5D=83&departements%5B%5D=84&departements%5B%5D=01&departements%5B%5D=07&departements%5B%5D=26&departements%5B%5D=38&departements%5B%5D=42&departements%5B%5D=69&departements%5B%5D=73&departements%5B%5D=74&departements%5B%5D=971&departements%5B%5D=973&departements%5B%5D=972&departements%5B%5D=974&departements%5B%5D=988&departements%5B%5D=987&departements%5B%5D=975&departements%5B%5D=976&departements%5B%5D=986&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher";
        private static Regex _lastUpdateRegex = new Regex("[0-9]{2}-[0-9]{2}-[0-9]{4}", RegexOptions.Compiled);  // Dernière mise à jour le 18-01-2013
        private static Regex _badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        private static Regex _email1Regex = new Regex("email1\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email1 = "jeu-ser"
        private static Regex _email2Regex = new Regex("email2\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email2 = "wanadoo.fr"

        public static Handeco_v2 Current { get { return _current; } }

        public static Handeco_v2 Create(bool test)
        {
            if (test)
                Trace.WriteLine("{0} init for test", _configName);
            XElement xe = GetConfigElement(test);

            _current = new Handeco_v2();
            _current.HeaderPageNominalType = typeof(Handeco_HeaderDataPages);
            _current.Create(xe);
            return _current;
        }

        public static XElement GetConfigElement(bool test = false)
        {
            if (!test)
                return XmlConfig.CurrentConfig.GetElement(_configName);
            else
                return XmlConfig.CurrentConfig.GetElement(_configName + "_Test");
        }

        // header get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override IEnumDataPages<Handeco_Header_v2> GetHeaderPageData(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();
            string url = httpResult.Http.HttpRequest.Url;
            Handeco_HeaderDataPages data = new Handeco_HeaderDataPages();
            data.SourceUrl = url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = GetPageKey(httpResult.Http.HttpRequest);


            // <div class="paginationControl">
            // page n    : <a href="/fournisseurs/rechercher/page/2#resultats">&gt;</a> |
            // last page : <span class="disabled">&gt;</span> |
            data.UrlNextPage = zurl.RemoveFragment(zurl.GetUrl(url, xeSource.XPathValue("//div[@class='paginationControl']//*[position()=last()-1]/@href")));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table//tr[position() > 1]");
            List<Handeco_Header_v2> headers = new List<Handeco_Header_v2>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Handeco_Header_v2 header = new Handeco_Header_v2();
                header.SourceUrl = url;
                header.LoadFromWebDate = DateTime.Now;
                header.Name = Handeco.Trim(xeHeader.XPathValue(".//td[1]//text()"));
                header.UrlDetail = zurl.RemoveFragment(zurl.GetUrl(url, xeHeader.XPathValue(".//td[1]//a/@href")));
                //header.Siret = Handeco.Trim(xeHeader.XPathValue(".//td[2]//text()"));
                header.Type = Handeco.Trim(xeHeader.XPathValue(".//td[2]//text()"));
                header.Groupes = xeHeader.XPathValues(".//td[3]//text()").Select(Handeco.Trim).ToArray();
                header.Activités = xeHeader.XPathValues(".//td[4]//text()").Select(Handeco.Trim).ToArray();
                header.PostalCode = Handeco.Trim(xeHeader.XPathValue(".//td[5]//text()"));
                headers.Add(header);
            }
            data.Data = headers.ToArray();
            return data;
        }

        // header get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : https://www.handeco.org/fournisseurs/rechercher
            // page 2 : https://www.handeco.org/fournisseurs/rechercher/page/2
            string url = httpRequest.Url;
            if (url == _urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = uri.Segments[uri.Segments.Length - 1];
            if (lastSegment.EndsWith("/"))
                lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            int page;
            if (!int.TryParse(lastSegment, out page))
                throw new PBException("header page key not found in url \"{0}\"", url);
            return page;
        }

        // header get url page, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : https://www.handeco.org/fournisseurs/rechercher
            // page 2 : https://www.handeco.org/fournisseurs/rechercher/page/2
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page != 1)
                throw new PBException("error impossible to load directly page {0}, only page 1 must be loaded first", page);
            //string url = __urlMainPage;
            //if (page > 1)
            //    url += string.Format("page/{0}/", page);
            //return new HttpRequest { Url = url };

            //_page = page;
            //string url = __urlMainPage;
            //requestParameters = new HttpRequestParameters_v1();
            //requestParameters.method = HttpRequestMethod.Post;
            //requestParameters.content = __content;
            //requestParameters.encoding = Encoding.UTF8;
            // private static string __urlMainPage = "https://www.handeco.org/fournisseurs/rechercher";
            // referer                               "https://www.handeco.org/fournisseurs/rechercher"
            return new HttpRequest { Url = _urlMainPage, Method = HttpRequestMethod.Post, Content = _headerRequestContent };
        }

        // detail cache get sub-directory, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return (_GetDetailKey(httpRequest) / 1000 * 1000).ToString();
        }

        // detail image cache get sub-directory, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        //protected override string GetDetailImageCacheUrlSubDirectory(WebData<Handeco_Detail_v2> data)
        //{
        //    string subPath = null;
        //    subPath = data.Result_v2.Http.HttpRequest.UrlCachePath.SubPath;
        //    return zpath.PathSetExtension(subPath, null);
        //}

        // detail get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override Handeco_Detail_v2 GetDetailData(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();

            Handeco_Detail_v2 data = new Handeco_Detail_v2();
            data.SourceUrl = httpResult.Http.HttpRequest.Url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = _GetDetailKey(httpResult.Http.HttpRequest);

            _GetDetailData(xeSource, data);

            return data;
        }

        protected void _GetDetailData(XXElement xeSource, Handeco_Detail_v2 data)
        {
            //<div style="text-align: right; font-size: 10px;">
            //<em>Dernière mise à jour le 18-01-2013</em>
            //</div>
            string lastUpdate = Handeco.Trim(xeSource.XPathValue("//em[starts-with(text(), 'Dernière mise à jour')]/text()"));
            if (lastUpdate != null)
            {
                Match match = _lastUpdateRegex.Match(lastUpdate);
                DateTime date;
                if (match.Success && DateTime.TryParseExact(match.Value, "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
                    data.DernièreMiseàjour = date;
                else
                    data.UnknowInfos.Add(lastUpdate);
            }
            else
                pb.Trace.WriteLine("error \"Dernière mise à jour\" not found");

            // NOTRE OFFRE - activities - multiple
            //<select style="width: 200px; display: none;" onchange="change_activite(this.selectedIndex);" id="select_activites">
            //    <option>Sous-traitance industrielle - Autre</option>
            //    <option>Assemblage mécanique</option>
            //    <option>Energie renouvelable - Autre</option>
            //</select>
            string[] activityTypes = xeSource.XPathValues("//select[@id = 'select_activites']/option/text()").Select(Handeco.Trim).ToArray();

            // CONTACTS - multiple
            //<select style="width: 200px; display: none;" onchange="change_contact(this.selectedIndex);" id="select_contacts">
            //    <option>Jacky STEINLE (Chef d'atelier)</option>
            //</select>
            string[] contactDescriptions = xeSource.XPathValues("//select[@id = 'select_contacts']/option/text()").Select(Handeco.Trim).ToArray();

            int indexActivityType = 0;
            int indexContactDescription = 0;
            List<Activity> activities = new List<Activity>();
            List<Contact> contacts = new List<Contact>();
            foreach (XXElement xxe in xeSource.XPathElements("//table[@class = 'fiche organisation']"))
            {
                //string id = xxe.XPathValue("@id").ToLower();
                string id = xxe.XPathValue("@id");
                if (id != null)
                    id = id.ToLower();

                //if (__trace)
                //    pb.Trace.WriteLine("table id = \"{0}\"", id);

                Activity activity = null;
                Contact contact = null;
                if (id != null && id.StartsWith("fiche_activite_"))
                {
                    activity = new Activity();
                    activities.Add(activity);
                    if (indexActivityType < activityTypes.Length)
                        activity.Type = activityTypes[indexActivityType++];
                    else
                        pb.Trace.WriteLine("warning miss an activity type in html (<select id='select_activites'>)");
                }
                else if (id != null && id.StartsWith("fiche_contact_"))
                {
                    contact = new Contact();
                    contacts.Add(contact);
                    if (indexContactDescription < contactDescriptions.Length)
                        contact.Description = contactDescriptions[indexContactDescription++];
                    else
                        pb.Trace.WriteLine("warning miss an activity type in html (<select id='select_contacts'>)");
                }

                foreach (XXElement xxe2 in xxe.XPathElements(".//tr"))
                {
                    string valueName = Handeco.Trim(xxe2.XPathValue(".//th//text()"));
                    //_currentElement = xxe2;
                    XXElement currentElement = xxe2;

                    if (valueName == null)
                        continue;

                    if (activity != null)
                    {
                        //if (__trace)
                        //    pb.Trace.Write("activité ");
                        if (!SetActivityValue(activity, valueName, currentElement))
                        {
                            //if (__trace)
                            //    pb.Trace.Write("error ");
                            data.UnknowInfos.Add("valeur activité inconnu : " + valueName + " = " + GetTextValue(currentElement));
                        }
                        //else if (__trace)
                        //    pb.Trace.Write("      ");
                        //if (__trace)
                        //    pb.Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue(currentElement));
                    }
                    else if (contact != null)
                    {
                        //if (__trace)
                        //    pb.Trace.Write("contact  ");
                        if (!SetContactValue(contact, valueName, currentElement))
                        {
                            //if (__trace)
                            //    pb.Trace.Write("error ");
                            data.UnknowInfos.Add("valeur contact inconnu : " + valueName + " = " + GetTextValue(currentElement));
                        }
                        //else if (__trace)
                        //    pb.Trace.Write("      ");
                        //if (__trace)
                        //    pb.Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue(currentElement));
                    }
                    else
                    {
                        //if (__trace)
                        //    pb.Trace.Write("société  ");
                        if (!SetValue(data, valueName, currentElement))
                        {
                            //if (__trace)
                            //    pb.Trace.Write("error ");
                            data.UnknowInfos.Add("valeur inconnu : " + valueName + " = " + GetTextValue(currentElement));
                        }
                        //else if (__trace)
                        //    pb.Trace.Write("      ");
                        //if (__trace)
                        //    pb.Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue(currentElement));
                    }
                }
            }
            data.Activités = activities.ToArray();
            data.Contacts = contacts.ToArray();
        }

        private static bool SetActivityValue(Activity activity, string valueName, XXElement xe)
        {
            bool ret = true;
            switch (valueName.ToLower())
            {
                // NOTRE OFFRE (html)
                case "description":
                    activity.Description = GetTextValue(xe);
                    break;
                case "moyens techniques disponibles":
                    activity.MoyensTechniquesDisponibles = GetTextValue(xe);
                    break;
                case "effectif total mobilisable (etp)":
                    activity.EffectifTotalMobilisable = GetTextValue(xe);
                    break;
                case "modalités pratiques":
                    activity.ModalitésPratiques = GetTextValue(xe);
                    break;
                case "couverture géographique":
                    activity.CouvertureGéographique = GetTextValue(xe);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        private static bool SetContactValue(Contact contact, string valueName, XXElement xe)
        {
            bool ret = true;
            switch (valueName.ToLower())
            {
                // CONTACTS (html)
                case "prénom et nom":
                    contact.Nom = GetTextValue(xe);
                    break;
                case "fonction":
                    contact.Fonction = GetTextValue(xe);
                    break;
                case "téléphone":
                    contact.Tel = GetTextValue(xe);
                    break;
                case "mobile":
                    contact.Mobile = GetTextValue(xe);
                    break;
                case "e-mail":
                    contact.Email = GetEmail(GetTextValue(xe));
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public static string GetEmail(string html)
        {
            // var lien1 = '<a href="mailto:';
            // var email1 = "jeu-ser";
            // var lien2 = '">';
            // var email2 = "wanadoo.fr";
            // var lien3 = '</a>';
            // document.write(lien1 + email1 + "@" + email2 + lien2 + email1 + "@" + email2 + lien3);"
            if (html == null)
                return null;
            Match match1 = _email1Regex.Match(html);
            Match match2 = _email2Regex.Match(html);
            if (match1.Success && match2.Success)
                return match1.Groups[1].Value + "@" + match2.Groups[1].Value;
            else
            {
                pb.Trace.WriteLine("warning cant extract email from\r\n{0}", html);
                return null;
            }
        }

        private static bool SetValue(Handeco_Detail_v2 detail, string valueName, XXElement xe)
        {
            bool ret = true;
            switch (valueName.ToLower())
            {
                // LES INFOS CLES (html)
                case "logo":
                    detail.Logo = zurl.GetUrl(detail.SourceUrl, xe.XPathValue(".//td//img/@src"));
                    break;
                case "raison sociale":
                    detail.RaisonSociale = GetTextValue(xe);
                    break;
                case "date de création":
                    detail.DateCréation = GetTextValue(xe);
                    break;
                case "statut juridique":
                    detail.StatutJuridique = GetTextValue(xe);
                    break;
                case "type de structure":
                    detail.TypeStructure = GetTextValue(xe);
                    break;
                case "site web":
                    detail.SiteWeb = GetTextValue(xe);
                    break;
                case "n° siret":
                    detail.Siret = GetTextValue(xe);
                    break;
                case "localisation géographique":
                    detail.Localisation = GetTextValue(xe);
                    break;
                case "normes, habilitations et certifications":
                    detail.Normes = GetTextValue(xe);
                    break;
                case "chiffre d'affaires annuel":
                    detail.ChiffreAffairesAnnuel = GetTextValue(xe);
                    break;
                case "effectif total (etp)":
                    detail.EffectifTotal = GetTextValue(xe);
                    break;
                case "effectif de production (etp)":
                    detail.EffectifProduction = GetTextValue(xe);
                    break;
                case "effectif d'encadrement (etp)":
                    detail.EffectifEncadrement = GetTextValue(xe);
                    break;
                case "nombre de travailleurs handicapés (etp)":
                    detail.NombreTravailleursHandicapés = GetTextValue(xe);
                    break;
                case "nombre de personnes handicapées accompagnées par an":
                    detail.NombreHandicapéAccompagné = GetTextValue(xe);
                    break;
                // RESEAUX ET PARTENAIRES (html)
                case "appartenance à un groupe":
                    detail.AppartenanceGroupe = GetTextValue(xe);
                    break;
                case "présentation du groupe":
                    detail.PrésentationGroupe = GetTextValue(xe);
                    break;
                case "site web du groupe":
                    detail.SiteWebGroupe = GetTextValue(xe);
                    break;
                case "adhésion à des réseaux du handicap":
                    detail.AdhésionRéseauxHandicap = GetTextValue(xe);
                    break;
                case "adhésion à des groupements et fédérations professionnels":
                    detail.Groupes = GetTextValues(xe);
                    break;
                case "expérience de co-traitance ou de gme avec":
                    detail.Cotraitance = GetTextValue(xe);
                    break;
                // NOS COORDONNEES (html)
                case "adresse principale":
                    detail.AdressePrincipale = GetTextValue(xe);
                    break;
                case "adresse du siège":
                    detail.AdresseSiège = GetTextValue(xe);
                    break;
                case "adresse des antennes":
                    detail.AdresseAntennes = GetTextValue(xe);
                    break;
                case "e-mail":
                    detail.Email = GetEmail(GetTextValue(xe));
                    break;
                case "tél":
                    detail.Tel = GetTextValue(xe);
                    break;
                case "fax":
                    detail.Fax = GetTextValue(xe);
                    break;
                case "code ape":
                    detail.CodeApe = GetTextValue(xe);
                    break;
                case "n° finess":
                    detail.NumeroFiness = GetTextValue(xe);
                    break;
                default:
                    //company.unknowInfos.Add(valueName + " : " + value);
                    ret = false;
                    break;
            }
            return ret;
        }

        private static string GetTextValue(XXElement xe)
        {
            return xe.XPathValues(".//td//text()").Select(s => Handeco.Trim(_badCharacters.Replace(s, " "))).zToStringValues(" ");
        }

        private static string[] GetTextValues(XXElement xe)
        {
            return xe.XPathValues(".//td//text()").Select(s => Handeco.Trim(_badCharacters.Replace(s, " "))).ToArray(); ;
        }

        // detail get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            // https://www.handeco.org/fournisseur/consulter/id/1270
            Uri uri = new Uri(httpRequest.Url);
            string code = uri.Segments[uri.Segments.Length - 1];
            int id;
            if (int.TryParse(code, out id))
                return id;
            throw new PBException("key not found in url \"{0}\"", httpRequest.Url);
        }
    }
}

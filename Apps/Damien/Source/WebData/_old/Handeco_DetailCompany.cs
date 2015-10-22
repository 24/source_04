using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;
using PB_Util;

namespace Download.Handeco
{
    public class Activity
    {
        public string type;                               // html tag select
        public string description;                        // description
        public string moyensTechniquesDisponibles;        // moyens techniques disponibles
        public string effectifTotalMobilisable;           // effectif total mobilisable (etp)
        public string modalitésPratiques;                 // modalités pratiques
        public string couvertureGéographique;             // couverture géographique
    }

    public class Contact
    {
        public string description;       // html tag select
        public string nom;               // prénom et nom
        public string fonction;          // fonction
        public string tel;               // téléphone
        public string mobile;            // mobile
        public string email;             // e-mail
    }

    public class Handeco_DetailCompany
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;

        public DateTime?  dernièreMiseàjour;                                    // Dernière mise à jour le 18-01-2013

        // LES INFOS CLES (html)
        public string logo;                                                     // logo
        public string raisonSociale;                                            // raison sociale
        public string dateCréation;                                             // date de création
        public string statutJuridique;                                          // statut juridique
        public string typeStructure;                                            // type de structure
        public string siteWeb;                                                  // site web
        public string siret;                                                    // n° siret
        public string localisation;                                             // localisation géographique
        public string normes;                                                   // normes, habilitations et certifications
        public string chiffreAffairesAnnuel;                                    // chiffre d'affaires annuel
        public string effectifTotal;                                            // effectif total (etp)
        public string effectifProduction;                                       // effectif de production (etp)
        public string effectifEncadrement;                                      // effectif d'encadrement (etp)
        public string nombreTravailleursHandicapés;                             // nombre de travailleurs handicapés (etp)
        public string nombreHandicapéAccompagné;                                // nombre de personnes handicapées accompagnées par an

        // RESEAUX ET PARTENAIRES (html)
        //public string[] adhésionGroupements;                                    // adhésion à des groupements et fédérations professionnels
        public string[] groupes;                                                // adhésion à des groupements et fédérations professionnels
        public string appartenanceGroupe;                                       // appartenance à un groupe
        public string présentationGroupe;                                       // présentation du groupe
        public string siteWebGroupe;                                            // site web du groupe
        public string adhésionRéseauxHandicap;                                  // adhésion à des réseaux du handicap
        //public string adhésionGroupement;                                     // adhésion à des groupements et fédérations professionnels
        public string cotraitance;                                              // expérience de co-traitance ou de gme avec

        // NOS COORDONNEES (html)
        public string adressePrincipale;                                        // adresse principale
        public string adresseSiège;                                             // adresse du siège
        public string adresseAntennes;                                          // adresse des antennes
        public string email;                                                    // e-mail
        public string tel;                                                      // tél
        public string fax;                                                      // fax
        public string codeApe;                                                  // code ape
        public string numeroFiness;                                             // n° finess

        public Activity[] activités;
        public Contact[] contacts;
        public List<string> unknowInfos = new List<string>();
    }

    public class Handeco_LoadDetailCompanyFromWeb : LoadDataFromWeb_v1<Handeco_DetailCompany>
    {
        // http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017
        //private Unea_HeaderCompany _header;
        private bool _loadImage = false;
        private static bool __trace = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;
        private static Func<string, string> __trimFunc1 = text => text.Trim();
        private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        private static Regex __lastUpdateRegex = new Regex("[0-9]{2}-[0-9]{2}-[0-9]{4}", RegexOptions.Compiled);  // Dernière mise à jour le 18-01-2013
        private static Regex __email1Regex = new Regex("email1\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email1 = "jeu-ser"
        private static Regex __email2Regex = new Regex("email2\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email2 = "wanadoo.fr"
        //private static Func<string, string> __trimFunc2 = text => text.Trim(' ', '\xA0', '\t', '\r', '\n', ':', '?', '-');
        //private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        //private static Func<string, string> __trimFunc2 = text =>
        //{
        //    text = text.Trim(' ', '\xA0', '\t', '\r', '\n', ':', '?', '-');
        //    text = __badCharacters.Replace(text, " ");
        //    return text;
        //};

        private XXElement _currentElement = null;

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
        }

        public Handeco_LoadDetailCompanyFromWeb(string url, bool reload = false, bool loadImage = false)
            : base(url, reload: reload)
        {
            SetRequestParameters(new HttpRequestParameters_v1() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Handeco_DetailCompany GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            Handeco_DetailCompany data = new Handeco_DetailCompany();
            data.sourceUrl = Url;
            data.loadFromWebDate = DateTime.Now;

            //<div style="text-align: right; font-size: 10px;">
            //<em>Dernière mise à jour le 18-01-2013</em>
            //</div>
            //string lastUpdate = xeSource.XPathValue("//em[starts-with(text(), 'Dernière mise à jour')]/text()", __trimFunc1);
            string lastUpdate = __trimFunc1(xeSource.XPathValue("//em[starts-with(text(), 'Dernière mise à jour')]/text()"));
            if (lastUpdate != null)
            {
                Match match = __lastUpdateRegex.Match(lastUpdate);
                DateTime date;
                if (match.Success && DateTime.TryParseExact(match.Value, "dd-MM-yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
                    data.dernièreMiseàjour = date;
                else
                    data.unknowInfos.Add(lastUpdate);
            }
            else
                Trace.WriteLine("error \"Dernière mise à jour\" not found");

            // NOTRE OFFRE - activities - multiple
            //<select style="width: 200px; display: none;" onchange="change_activite(this.selectedIndex);" id="select_activites">
            //    <option>Sous-traitance industrielle - Autre</option>
            //    <option>Assemblage mécanique</option>
            //    <option>Energie renouvelable - Autre</option>
            //</select>
            //string[] activityTypes = xeSource.XPathValues("//select[@id = 'select_activites']/option/text()", __trimFunc1);
            string[] activityTypes = xeSource.XPathValues("//select[@id = 'select_activites']/option/text()").Select(__trimFunc1).ToArray();

            // CONTACTS - multiple
            //<select style="width: 200px; display: none;" onchange="change_contact(this.selectedIndex);" id="select_contacts">
            //    <option>Jacky STEINLE (Chef d'atelier)</option>
            //</select>
            //string[] contactDescriptions = xeSource.XPathValues("//select[@id = 'select_contacts']/option/text()", __trimFunc1);
            string[] contactDescriptions = xeSource.XPathValues("//select[@id = 'select_contacts']/option/text()").Select(__trimFunc1).ToArray();

            int indexActivityType = 0;
            int indexContactDescription = 0;
            List<Activity> activities = new List<Activity>();
            List<Contact> contacts = new List<Contact>();
            foreach (XXElement xxe in xeSource.XPathElements("//table[@class = 'fiche organisation']"))
            {
                //string id = xxe.XPathValue("@id", s => s.ToLower());
                string id = xxe.XPathValue("@id").ToLower();

                if (__trace)
                    Trace.WriteLine("table id = \"{0}\"", id);

                Activity activity = null;
                Contact contact = null;
                if (id != null && id.StartsWith("fiche_activite_"))
                {
                    activity = new Activity();
                    activities.Add(activity);
                    if (indexActivityType < activityTypes.Length)
                        activity.type = activityTypes[indexActivityType++];
                    else
                        Trace.WriteLine("warning miss an activity type in html (<select id='select_activites'>)");
                }
                else if (id != null && id.StartsWith("fiche_contact_"))
                {
                    contact = new Contact();
                    contacts.Add(contact);
                    if (indexContactDescription < contactDescriptions.Length)
                        contact.description = contactDescriptions[indexContactDescription++];
                    else
                        Trace.WriteLine("warning miss an activity type in html (<select id='select_contacts'>)");
                }

                foreach (XXElement xxe2 in xxe.XPathElements(".//tr"))
                {
                    //string valueName = xxe2.XPathValue(".//th//text()", __trimFunc1);
                    string valueName = __trimFunc1(xxe2.XPathValue(".//th//text()"));
                    //string value = xxe2.XPathConcatText(".//td//text()", separator: " ", itemFunc: s => __trimFunc1(__badCharacters.Replace(s, " ")));
                    _currentElement = xxe2;
                    
                    //if (valueName == null || value == null)
                    if (valueName == null)
                        continue;

                    //if ((activity == null || !SetActivityValue(activity, valueName, value))
                    //    && (contact == null || !SetContactValue(contact, valueName, value)))
                    //    SetValue(data, valueName, value);
                    if (activity != null)
                    {
                        if (__trace)
                            Trace.Write("activité ");
                        if (!SetActivityValue(activity, valueName))
                        {
                            if (__trace)
                                Trace.Write("error ");
                            data.unknowInfos.Add("valeur activité inconnu : " + valueName + " = " + GetTextValue());
                        }
                        else if (__trace)
                            Trace.Write("      ");
                        if (__trace)
                            Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue());
                    }
                    else if (contact != null)
                    {
                        if (__trace)
                            Trace.Write("contact  ");
                        if (!SetContactValue(contact, valueName))
                        {
                            if (__trace)
                                Trace.Write("error ");
                            data.unknowInfos.Add("valeur contact inconnu : " + valueName + " = " + GetTextValue());
                        }
                        else if (__trace)
                            Trace.Write("      ");
                        if (__trace)
                            Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue());
                    }
                    else
                    {
                        if (__trace)
                            Trace.Write("société  ");
                        if (!SetValue(data, valueName))
                        {
                            if (__trace)
                                Trace.Write("error ");
                            data.unknowInfos.Add("valeur inconnu : " + valueName + " = " + GetTextValue());
                        }
                        else if (__trace)
                            Trace.Write("      ");
                        if (__trace)
                            Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue());
                    }
                }
            }
            data.activités = activities.ToArray();
            data.contacts = contacts.ToArray();
            return data;
        }

        public string GetTextValue()
        {
            return _currentElement.XPathConcatText(".//td//text()", separator: " ", itemFunc: s => __trimFunc1(__badCharacters.Replace(s, " "))); ;
        }

        public string[] GetTextValues()
        {
            //return _currentElement.XPathValues(".//td//text()", s => __trimFunc1(__badCharacters.Replace(s, " "))); ;
            return _currentElement.XPathValues(".//td//text()").Select(s => __trimFunc1(__badCharacters.Replace(s, " "))).ToArray(); ;
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
            Match match1 = __email1Regex.Match(html);
            Match match2 = __email2Regex.Match(html);
            if (match1.Success && match2.Success)
                return match1.Groups[1].Value + "@" + match2.Groups[1].Value;
            else
            {
                Trace.WriteLine("warning cant extract email from\r\n{0}", html);
                return null;
            }
        }

        //public static bool SetActivityValue(Activity activity, string valueName, string value)
        public bool SetActivityValue(Activity activity, string valueName)
        {
            bool ret = true;
            switch (valueName.ToLower())
            {
                // NOTRE OFFRE (html)
                case "description":
                    activity.description = GetTextValue();
                    break;
                case "moyens techniques disponibles":
                    activity.moyensTechniquesDisponibles = GetTextValue();
                    break;
                case "effectif total mobilisable (etp)":
                    activity.effectifTotalMobilisable = GetTextValue();
                    break;
                case "modalités pratiques":
                    activity.modalitésPratiques = GetTextValue();
                    break;
                case "couverture géographique":
                    activity.couvertureGéographique = GetTextValue();
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        //public static bool SetContactValue(Contact contact, string valueName, string value)
        public bool SetContactValue(Contact contact, string valueName)
        {
            bool ret = true;
            switch (valueName.ToLower())
            {
                // CONTACTS (html)
                case "prénom et nom":
                    contact.nom = GetTextValue();
                    break;
                case "fonction":
                    contact.fonction = GetTextValue();
                    break;
                case "téléphone":
                    contact.tel = GetTextValue();
                    break;
                case "mobile":
                    contact.mobile = GetTextValue();
                    break;
                case "e-mail":
                    contact.email = GetEmail(GetTextValue());
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        //public static bool SetValue(Handeco_DetailCompany company, string valueName, string value)
        public bool SetValue(Handeco_DetailCompany company, string valueName)
        {
            bool ret = true;
            switch (valueName.ToLower())
            {
                // LES INFOS CLES (html)
                case "logo":
                    company.logo = zurl.GetUrl(Url, _currentElement.XPathValue(".//td//img/@src"));
                    break;
                case "raison sociale":
                    company.raisonSociale = GetTextValue();
                    break;
                case "date de création":
                    company.dateCréation = GetTextValue();
                    break;
                case "statut juridique":
                    company.statutJuridique = GetTextValue();
                    break;
                case "type de structure":
                    company.typeStructure = GetTextValue();
                    break;
                case "site web":
                    company.siteWeb = GetTextValue();
                    break;
                case "n° siret":
                    company.siret = GetTextValue();
                    break;
                case "localisation géographique":
                    company.localisation = GetTextValue();
                    break;
                case "normes, habilitations et certifications":
                    company.normes = GetTextValue();
                    break;
                case "chiffre d'affaires annuel":
                    company.chiffreAffairesAnnuel = GetTextValue();
                    break;
                case "effectif total (etp)":
                    company.effectifTotal = GetTextValue();
                    break;
                case "effectif de production (etp)":
                    company.effectifProduction = GetTextValue();
                    break;
                case "effectif d'encadrement (etp)":
                    company.effectifEncadrement = GetTextValue();
                    break;
                case "nombre de travailleurs handicapés (etp)":
                    company.nombreTravailleursHandicapés = GetTextValue();
                    break;
                case "nombre de personnes handicapées accompagnées par an":
                    company.nombreHandicapéAccompagné = GetTextValue();
                    break;
                // RESEAUX ET PARTENAIRES (html)
                case "appartenance à un groupe":
                    company.appartenanceGroupe = GetTextValue();
                    break;
                case "présentation du groupe":
                    company.présentationGroupe = GetTextValue();
                    break;
                case "site web du groupe":
                    company.siteWebGroupe = GetTextValue();
                    break;
                case "adhésion à des réseaux du handicap":
                    company.adhésionRéseauxHandicap = GetTextValue();
                    break;
                case "adhésion à des groupements et fédérations professionnels":
                    //company.adhésionGroupement = GetTextValue();
                    company.groupes = GetTextValues();
                    break;
                case "expérience de co-traitance ou de gme avec":
                    company.cotraitance = GetTextValue();
                    break;
                // NOS COORDONNEES (html)
                case "adresse principale":
                    company.adressePrincipale = GetTextValue();
                    break;
                case "adresse du siège":
                    company.adresseSiège = GetTextValue();
                    break;
                case "adresse des antennes":
                    company.adresseAntennes = GetTextValue();
                    break;
                case "e-mail":
                    company.email = GetEmail(GetTextValue());
                    break;
                case "tél":
                    company.tel = GetTextValue();
                    break;
                case "fax":
                    company.fax = GetTextValue();
                    break;
                case "code ape":
                    company.codeApe = GetTextValue();
                    break;
                case "n° finess":
                    company.numeroFiness = GetTextValue();
                    break;
                default:
                    //company.unknowInfos.Add(valueName + " : " + value);
                    ret = false;
                    break;
            }
            return ret;
        }

        public static Handeco_DetailCompany LoadCompany(string url, bool reload = false, bool loadImage = false)
        {
            Handeco_LoadDetailCompanyFromWeb load = new Handeco_LoadDetailCompanyFromWeb(url, reload, loadImage);
            load.Load();
            return load.Data;
        }
    }

    public class Handeco_LoadDetailCompany : LoadWebData_v1<Handeco_DetailCompany>
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

        public Handeco_LoadDetailCompany(string url)
            : base(url)
        {
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "Handeco detail company";
        }

        protected override Handeco_DetailCompany LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            return Handeco_LoadDetailCompanyFromWeb.LoadCompany(Url, reload, loadImage);
        }

        public static Handeco_DetailCompany LoadCompany(string url, bool reload = false, bool loadImage = false)
        {
            Handeco_LoadDetailCompany load = new Handeco_LoadDetailCompany(url);
            load.Load(reload, loadImage);
            return load.Data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using pb.Data.old;
using pb.Data.Mongo.old;
using pb.Web.Data.old;

namespace hts.WebData
{
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

    public class Handeco_Detail : IKeyData_v4<int> // IPost, IWebData
    {
        public Handeco_Detail()
        {
            //Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

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

        //public string Title;
        //public PrintType PrintType;
        //public string OriginalTitle;
        //public string PostAuthor;
        //public DateTime? PostCreationDate;
        //public string Category;
        //public string[] Description;
        //public NamedValues<ZValue> Infos;
        //public WebImage[] Images;
        //public string[] DownloadLinks;

        //public string GetServer()
        //{
        //    return "vosbooks.net";
        //}

        public int GetKey()
        {
            return Id;
        }

        //public HttpRequest GetDataHttpRequest()
        //{
        //    return new HttpRequest { Url = SourceUrl };
        //}

        //public DateTime GetLoadFromWebDate()
        //{
        //    return LoadFromWebDate;
        //}

        //public WebImage[] GetImages()
        //{
        //    return Images;
        //}

        //public void SetImages(WebImage[] images)
        //{
        //    Images = images;
        //}
    }

    public static class Handeco_DetailManager
    {
        private static bool __trace = false;
        private static WebDataManager_v1<int, Handeco_Detail> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v1<int, Handeco_HeaderPage, Handeco_Header, int, Handeco_Detail> __webHeaderDetailManager = null;
        private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        private static Regex __lastUpdateRegex = new Regex("[0-9]{2}-[0-9]{2}-[0-9]{4}", RegexOptions.Compiled);  // Dernière mise à jour le 18-01-2013
        private static Regex __email1Regex = new Regex("email1\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email1 = "jeu-ser"
        private static Regex __email2Regex = new Regex("email2\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email2 = "wanadoo.fr"

        static Handeco_DetailManager()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("Handeco/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager_v1<int, Handeco_HeaderPage, Handeco_Header, int, Handeco_Detail>();
            __webHeaderDetailManager.HeaderDataPageManager = Handeco_HeaderManager.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager_v1<int, Handeco_Detail> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v1<int, Handeco_HeaderPage, Handeco_Header, int, Handeco_Detail> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager_v1<int, Handeco_Detail> CreateWebDataManager(XElement xe)
        {
            WebDataManager_v1<int, Handeco_Detail> detailWebDataManager = new WebDataManager_v1<int, Handeco_Detail>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<Handeco_Detail>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetKey(httpRequest) / 1000 * 1000).ToString();
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            //detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = Handeco.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages;

            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Handeco_HeaderPage>(document);
            detailWebDataManager.DocumentStore = MongoDocumentStore_v4<int, Handeco_Detail>.Create(xe);

            return detailWebDataManager;
        }

        private static Handeco_Detail GetData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            Handeco_Detail data = new Handeco_Detail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetKey(webResult.WebRequest.HttpRequest);

            //<div style="text-align: right; font-size: 10px;">
            //<em>Dernière mise à jour le 18-01-2013</em>
            //</div>
            string lastUpdate = Handeco.Trim(xeSource.XPathValue("//em[starts-with(text(), 'Dernière mise à jour')]/text()"));
            if (lastUpdate != null)
            {
                Match match = __lastUpdateRegex.Match(lastUpdate);
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

                if (__trace)
                    pb.Trace.WriteLine("table id = \"{0}\"", id);

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
                        if (__trace)
                            pb.Trace.Write("activité ");
                        if (!SetActivityValue(activity, valueName, currentElement))
                        {
                            if (__trace)
                                pb.Trace.Write("error ");
                            data.UnknowInfos.Add("valeur activité inconnu : " + valueName + " = " + GetTextValue(currentElement));
                        }
                        else if (__trace)
                            pb.Trace.Write("      ");
                        if (__trace)
                            pb.Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue(currentElement));
                    }
                    else if (contact != null)
                    {
                        if (__trace)
                            pb.Trace.Write("contact  ");
                        if (!SetContactValue(contact, valueName, currentElement))
                        {
                            if (__trace)
                                pb.Trace.Write("error ");
                            data.UnknowInfos.Add("valeur contact inconnu : " + valueName + " = " + GetTextValue(currentElement));
                        }
                        else if (__trace)
                            pb.Trace.Write("      ");
                        if (__trace)
                            pb.Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue(currentElement));
                    }
                    else
                    {
                        if (__trace)
                            pb.Trace.Write("société  ");
                        if (!SetValue(data, valueName, currentElement))
                        {
                            if (__trace)
                                pb.Trace.Write("error ");
                            data.UnknowInfos.Add("valeur inconnu : " + valueName + " = " + GetTextValue(currentElement));
                        }
                        else if (__trace)
                            pb.Trace.Write("      ");
                        if (__trace)
                            pb.Trace.WriteLine("\"{0}\" =  \"{1}\"", valueName, GetTextValue(currentElement));
                    }
                }
            }
            data.Activités = activities.ToArray();
            data.Contacts = contacts.ToArray();

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;

            //XXElement xePost = xeSource.XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']");

            //XXElement xe = xePost.XPathElement(".//table[@id='post-head']");
            ////string[] dates = xe.DescendantTextList(".//td[@id='head-date']", func: Vosbooks.TrimFunc1).ToArray();
            //string[] dates = xe.XPathElement(".//td[@id='head-date']").DescendantTexts().Select(DownloadPrint.Trim).ToArray();
            //data.PostCreationDate = GetDate(dates, __lastPostDate);
            //if (data.PostCreationDate != null)
            //    __lastPostDate = new Date(data.PostCreationDate.Value);
            //if (__trace)
            //    pb.Trace.WriteLine("post creation date {0} - {1}", data.PostCreationDate, dates.zToStringValues());

            ////data.Title = xePost.XPathValue(".//div[@class='title']//a//text()", DownloadPrint.TrimFunc1);
            //data.Title = xePost.XPathValue(".//div[@class='title']//a//text()").zFunc(DownloadPrint.ReplaceChars).zFunc(DownloadPrint.Trim);
            //PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            //if (titleInfos.foundInfo)
            //{
            //    data.OriginalTitle = data.Title;
            //    data.Title = titleInfos.title;
            //    data.Infos.SetValues(titleInfos.infos);
            //}

            //// Ebooks en Epub / Livre
            ////data.Category = xePost.DescendantTextList(".//div[@class='postdata']//span[@class='category']//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            //data.Category = xePost.XPathElements(".//div[@class='postdata']//span[@class='category']//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            //data.PrintType = GetPrintType(data.Category);
            ////pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            //xe = xePost.XPathElement(".//div[@class='entry']");
            //data.Images = new WebImage[] { new WebImage(zurl.GetUrl(data.SourceUrl, xe.XPathValue("div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p/img/@src"))) };

            //// force load image to get image width and height
            //if (webResult.WebRequest.LoadImage)
            //    data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            //// get infos, description, language, size, nbPages
            //// xe.DescendantTextList(".//p")
            //PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(
            //    xe.XPathElements(".//p").DescendantTexts(
            //    node =>
            //    {
            //        if (node is XText)
            //        {
            //            string text = ((XText)node).Value.Trim();
            //            //if (text.StartsWith("Lien Direct", StringComparison.InvariantCultureIgnoreCase))
            //            if (text.StartsWith("lien ", StringComparison.InvariantCultureIgnoreCase))
            //                return XNodeFilter.Stop;
            //        }
            //        if (node is XElement)
            //        {
            //            XElement xe2 = (XElement)node;
            //            if (xe2.Name == "p" && xe2.zAttribValue("class") == "submeta")
            //                return XNodeFilter.Stop;
            //        }
            //        return XNodeFilter.SelectNode;
            //    }
            //    ).Select(DownloadPrint.ReplaceChars).Select(DownloadPrint.TrimWithoutColon), data.Title);
            //data.Description = textValues.description;
            //data.Infos.SetValues(textValues.infos);

            //data.DownloadLinks = xe.DescendantNodes(
            //    node =>
            //    {
            //        if (!(node is XElement))
            //            return XNodeFilter.DontSelectNode;
            //        XElement xe2 = (XElement)node;
            //        if (xe2.Name == "a")
            //            return XNodeFilter.SelectNode;
            //        if (xe2.Name != "p")
            //            return XNodeFilter.DontSelectNode;
            //        XAttribute xa = xe2.Attribute("class");
            //        if (xa == null)
            //            return XNodeFilter.DontSelectNode;
            //        if (xa.Value != "submeta")
            //            return XNodeFilter.DontSelectNode;
            //        //return XNodeFilter.SkipNode;
            //        return XNodeFilter.Stop;
            //    })
            //    .Select(node => ((XElement)node).Attribute("href").Value).ToArray();
        }

        private static string GetTextValue(XXElement xe)
        {
            //return currentElement.XPathConcatText(".//td//text()", separator: " ", itemFunc: s => Handeco.Trim(__badCharacters.Replace(s, " "))); ;
            return xe.XPathValues(".//td//text()").Select(s => Handeco.Trim(__badCharacters.Replace(s, " "))).zToStringValues(" ");
        }

        private static string[] GetTextValues(XXElement xe)
        {
            return xe.XPathValues(".//td//text()").Select(s => Handeco.Trim(__badCharacters.Replace(s, " "))).ToArray(); ;
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
                pb.Trace.WriteLine("warning cant extract email from\r\n{0}", html);
                return null;
            }
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

        private static bool SetValue(Handeco_Detail detail, string valueName, XXElement xe)
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

        //private static DateTime? GetDate(string[] dates, Date? refDate)
        //{
        //    if (dates.Length >= 2)
        //    {
        //        int month = zdate.GetMonthNumber(dates[0]);
        //        int day;
        //        if (month != 0 && int.TryParse(dates[1], out day))
        //        {
        //            return zdate.GetNearestYearDate(day, month, refDate).DateTime;
        //        }
        //    }
        //    pb.Trace.WriteLine("unknow post creation date {0}", dates.zToStringValues());
        //    return null;
        //}

        private static int GetKey(HttpRequest httpRequest)
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

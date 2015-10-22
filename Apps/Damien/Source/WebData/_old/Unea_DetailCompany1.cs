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
    public class Unea_DetailCompany1
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;

        //   fiche_bloc no 1 : name, location, activities, sectors, unknowInfos
        public string name = null;
        public string location = null;
        public SortedDictionary<string, string> activities = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> sectors = new SortedDictionary<string, string>();  // Filières Métiers UNEA

        //   fiche_bloc no 2 : presentation, clients, leader, employeNumber, lastYearRevenue, certification, siret, downloadDocuments, unknowInfos
        public string presentation = null;
        public string clients = null;
        public string leader = null; // dirigeant
        public int? employeNumber = null; // nombre de salarié
        public string lastYearRevenue = null;  // chiffre d'affaire de l'année écoulée
        public string certification = null; // certification
        public string siret = null;
        public SortedDictionary<string, string> photos = new SortedDictionary<string, string>();
        public SortedDictionary<string, Unea_Document> downloadDocuments = new SortedDictionary<string, Unea_Document>();

        //   fiche_bloc no 3 : address, phone, fax, email, webSite
        public string address = null;
        public string phone = null;
        public string fax = null;
        public string email = null;
        public string webSite = null;

        public List<string> unknowInfos = new List<string>();
    }

    public class Unea_LoadDetailCompany1FromWeb : LoadDataFromWeb_v1<Unea_DetailCompany1>
    {
        // http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017
        //private Unea_HeaderCompany _header;
        private bool _loadImage = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path | UrlFileNameType.Query;
        private static Func<string, string> __trimFunc1 = text => text.Trim();
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

        public Unea_LoadDetailCompany1FromWeb(string url, bool reload = false, bool loadImage = false)
            : base(url, reload: reload)
        {
            //SetRequestParameters(new HttpRequestParameters() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Unea_DetailCompany1 GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            Unea_DetailCompany1 data = new Unea_DetailCompany1();
            data.sourceUrl = Url;
            data.loadFromWebDate = DateTime.Now;

            // <div class="fiche">
            XXElement xeFiche = xeSource.XPathElement(".//div[@class='fiche']");

            //// <div class="fiche_bloc">
            //IEnumerator<XXElement> xeFicheBlocs = xeFiche.XPathElements(".//div[@class='fiche_bloc']").GetEnumerator();

            //// fiche_bloc no 1
            //if (!xeFicheBlocs.MoveNext())
            //{
            //    Trace.CurrentTrace.WriteLine("error fiche_bloc no 1 not found \"<div class='fiche_bloc'>\"");
            //    return data;
            //}
            //XXElement xe = xeFicheBlocs.Current;
            //GetDataFicheBlocNo1(data, xe);

            //// fiche_bloc no 2
            //if (!xeFicheBlocs.MoveNext())
            //{
            //    Trace.CurrentTrace.WriteLine("error fiche_bloc no 2 not found \"<div class='fiche_bloc'>\"");
            //    return data;
            //}
            //xe = xeFicheBlocs.Current;
            //GetDataFicheBlocNo2(data, xe);

            //// fiche_bloc no 3
            //if (!xeFicheBlocs.MoveNext())
            //{
            //    Trace.CurrentTrace.WriteLine("error fiche_bloc no 3 not found \"<div class='fiche_bloc'>\"");
            //    return data;
            //}
            //xe = xeFicheBlocs.Current;
            //GetDataFicheBlocNo3(data, xe);

            GetNewDataFicheBloc(data, xeFiche);

            return data;
        }

        private static void GetNewDataFicheBloc(Unea_DetailCompany1 data, XXElement xeFiche)
        {
            //IEnumerator<XXElement> xeFicheBlocs = xeFiche.XPathElements(".//div[@class='fiche_bloc']").GetEnumerator();

            // <div class="fiche_entete"><!-- <h3>Logo UNEA</h3> --><h1>ALSACE ENTREPRISE ADAPTEE</h1></div>
            //data.name = xe.XPathValue(".//div[@class='fiche_entete']//text()", __trimFunc1);

            //GetDataFicheBlocNo1
            //foreach (string text in xe.DescendantTextList(".//td[@class='fiche_infos']", func: __trimFunc2))
            //GetDataFicheBlocNo2
            //foreach (XText xtext in xe.DescendantTextNodeList(".//div[@class='fiche_contenu']"))
            //GetDataFicheBlocNo3
            //foreach (string text in xe.DescendantTextList(".//table", func: __trimFunc2))

            bool firstText = true;
            Unea_TextType textType = Unea_TextType.unknow;
            //foreach (XText xtext in xeFiche.DescendantTextNodeList(".//div[@class='fiche_bloc']"))
            foreach (XText xtext in xeFiche.XPathElements(".//div[@class='fiche_bloc']").DescendantTextNodes())
            {
                string text = __trimFunc2(xtext.Value);
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
                    foreach (XXElement xe2 in new XXElement(xtext.Parent).XPathElements("ancestor::p/following-sibling::p//img"))
                    {
                        string url = xe2.XPathValue("@src");
                        if (!data.photos.ContainsKey(url))
                            data.photos.Add(url, null);
                        else
                            Trace.CurrentTrace.WriteLine("warning photo already exists \"{0}\"", url);
                    }
                    textType = Unea_TextType.novalues;
                }
                else if (text.Equals("Documents téléchargeables", StringComparison.InvariantCultureIgnoreCase))
                {
                    //foreach (XXElement xe2 in new XXElement(xtext.Parent).XPathElements("ancestor::p/following-sibling::p//a"))
                    bool stop = false;
                    //foreach (XXElement xe2 in new XXElement(xtext.Parent).XPathElements("ancestor::p/following-sibling::p",
                    //    e => 
                    //    { 
                    //        if (e.Value.StartsWith("Photos", StringComparison.InvariantCultureIgnoreCase))
                    //            stop = true;
                    //        return !stop;
                    //    }))
                    foreach (XXElement xe2 in new XXElement(xtext.Parent).XPathElements("ancestor::p/following-sibling::p").Where(
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
                        //string name = name = xe3.XPathValue(".//text()", __trimFunc2);
                        string name = __trimFunc2(xe3.XPathValue(".//text()"));
                        if (!data.downloadDocuments.ContainsKey(url))
                            data.downloadDocuments.Add(url, new Unea_Document() { name = name, url = url });
                        else
                            Trace.CurrentTrace.WriteLine("warning download document already exists \"{0}\" \"{1}\"", name, url);
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
                            if (!data.activities.ContainsKey(text))
                                data.activities.Add(text, null);
                            else
                                Trace.CurrentTrace.WriteLine("warning activity already exists \"{0}\"", text);
                            break;
                        case Unea_TextType.location:
                            data.location = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.sector:
                            if (!data.sectors.ContainsKey(text))
                                data.sectors.Add(text, null);
                            else
                                Trace.CurrentTrace.WriteLine("warning sector already exists \"{0}\"", text);
                            break;
                        // fiche_bloc no 2
                        case Unea_TextType.presentation:
                            data.presentation = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.client:
                            data.clients = text;
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
                            data.lastYearRevenue = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.certification:
                            data.certification = text;
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.siret:
                            data.siret = text;
                            textType = Unea_TextType.unknow;
                            break;
                        // fiche_bloc no 3
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
                        case Unea_TextType.novalue:
                            textType = Unea_TextType.unknow;
                            break;
                        case Unea_TextType.novalues:
                            break;
                        default:
                            if (firstText)
                            {
                                data.name = text;
                                firstText = false;
                            }
                            else
                                data.unknowInfos.Add(text);
                            break;
                    }
                }
            }
        }

        //private static void GetDataFicheBlocNo1(Unea_DetailCompany1 data, XXElement xe)
        //{
        //    // <div class="fiche_entete"><!-- <h3>Logo UNEA</h3> --><h1>ALSACE ENTREPRISE ADAPTEE</h1></div>
        //    data.name = xe.XPathValue(".//div[@class='fiche_entete']//text()", __trimFunc1);

        //    // <td class="fiche_infos">
        //    // <p>
        //    // <span>Activités : </span><br>
        //    // TRAVAUX PAYSAGERS : Entretien<br>
        //    // PROPRETE : Entretien de locaux<br>
        //    // PRESTATION DE SERVICES : Transports<br>
        //    // PRESTATION DE SERVICES : Prestation sur site<br>
        //    // SOUS TRAITANCE INDUSTRIELLE : Automobile<br>
        //    // MECANIQUE : Montage<br>
        //    // AUTOMOBILE : Divers<br>
        //    // METALLURGIE : Montage mécanique
        //    // </p>
        //    // <p style="padding-top: 10px;">
        //    // <span>Région - Département : </span><br>
        //    // Alsace - HAUT RHIN (68)<br>&nbsp;
        //    // </p>
        //    // <p>
        //    // <span>Filières Métiers UNEA : </span><br>
        //    // METALLURGIE<br>
        //    // TRAVAUX PAYSAGERS<br>
        //    // GESTION ELECTRONIQUE DES DOCUMENTS
        //    // </p>
        //    // </td>
        //    Unea_TextType textType = Unea_TextType.unknow;
        //    foreach (string text in xe.DescendantTextList(".//td[@class='fiche_infos']", func: __trimFunc2))
        //    {
        //        if (text == "")
        //            continue;
        //        if (text.Equals("Activités", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.activity;
        //        else if (text.Equals("Région - Département", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.location;
        //        else if (text.Equals("Filières Métiers UNEA", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.sector;
        //        else
        //        {
        //            switch (textType)
        //            {
        //                case Unea_TextType.activity:
        //                    if (!data.activities.ContainsKey(text))
        //                        data.activities.Add(text, null);
        //                    else
        //                        Trace.CurrentTrace.WriteLine("warning activity already exists \"{0}\"", text);
        //                    break;
        //                case Unea_TextType.location:
        //                    data.location = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.sector:
        //                    //data.sectors.Add(text);
        //                    if (!data.sectors.ContainsKey(text))
        //                        data.sectors.Add(text, null);
        //                    else
        //                        Trace.CurrentTrace.WriteLine("warning sector already exists \"{0}\"", text);
        //                    break;
        //                default:
        //                    data.unknowInfos.Add(text);
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private static void GetDataFicheBlocNo2(Unea_DetailCompany1 data, XXElement xe)
        //{
        //    //<div class="fiche_bloc">
        //    //<div class="fiche_entete"><h1>Présentation de l'Entreprise Adaptée</h1></div>
        //    //<div class="fiche_contenu">
        //    //<table cellpadding="0" cellspacing="0" border="0">
        //    //<tr valign="top">
        //    //<td class="fiche_infos_1col">
        //    //<p><span>Présentation : </span><br>
        //    //ALSACE ENTREPRISE ADAPTEE est implantée sur les sites de Colmar et Mulhouse avec un effectif de 106 salariés, avec les activités sous-traitance : assemblage de pièces, cintrage de tuyaux, montage complexe, ainsi qu'une activité prestation de service en espaces verts, ménage et transport.
        //    //</p>
        //    //<p style="padding-top: 10px;">
        //    //<span>Principaux clients : </span><br>
        //    //PSA Peugeot Citroën,Liebherr, Schaeffler - LUK<br>&nbsp;
        //    //</p>
        //    //<p><span>Dirigeant : </span>Directeur commercial Joseph LATSCHA<br>&nbsp;</p>
        //    //<p><span>Nombre de salariés : </span>122</p>
        //    //<p><span>Chiffre d'affaire de l'année écoulée : </span>2400000 ?</p>
        //    //<p><span>Certification : </span>certifié ISO 9001 version 2008 par l'organisme TUV</p>
        //    //<p><span>Numéro SIRET : </span>77564261400256</p>
        //    //<p style="padding-top: 10px;"><span>Documents téléchargeables : </span></p>
        //    //<p>
        //    //<img src="images/pdf.jpg" border="0" width="15px"/>
        //    //<a href="http://unea.griotte.biz/BaseDocumentaire/Docs/Public/4017/Plaquette_AEA.pdf" target="_blank" class="fiche_photos"> Plaquette_AEA.pdf</a>
        //    //</p>
        //    //<p></p>
        //    //</td>
        //    //</tr>
        //    //</table>
        //    //</div>
        //    //</div>
        //    Unea_TextType textType = Unea_TextType.unknow;
        //    foreach (XText xtext in xe.DescendantTextNodeList(".//div[@class='fiche_contenu']"))
        //    {
        //        string text = __trimFunc2(xtext.Value);
        //        if (text == "")
        //            continue;
        //        if (text.Equals("Présentation", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.presentation;
        //        else if (text.Equals("Principaux clients", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.client;
        //        else if (text.Equals("Dirigeant", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.leader;
        //        else if (text.Equals("Nombre de salariés", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.employeNumber;
        //        else if (text.Equals("Chiffre d'affaire de l'année écoulée", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.lastYearRevenue;
        //        else if (text.Equals("Certification", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.certification;
        //        else if (text.Equals("Numéro SIRET", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.siret;
        //        else if (text.Equals("Documents téléchargeables", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            foreach (XXElement xe2 in new XXElement(xtext.Parent).XPathElements("ancestor::p/following-sibling::p//a"))
        //            {
        //                string url = xe2.XPathValue("@href");
        //                string name = name = xe2.XPathValue(".//text()", __trimFunc2);
        //                if (!data.downloadDocuments.ContainsKey(url))
        //                    data.downloadDocuments.Add(url, new Unea_Document() { name = name, url = url });
        //                else
        //                    Trace.CurrentTrace.WriteLine("warning download document already exists \"{0}\" \"{1}\"", name, url);
        //            }
        //            textType = Unea_TextType.novalues;
        //        }
        //        else
        //        {
        //            switch (textType)
        //            {
        //                case Unea_TextType.presentation:
        //                    data.presentation = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.client:
        //                    data.clients = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.leader:
        //                    data.leader = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.employeNumber:
        //                    int employeNumber;
        //                    if (int.TryParse(text, out employeNumber))
        //                        data.employeNumber = employeNumber;
        //                    else
        //                        Trace.CurrentTrace.WriteLine("error unknow employe number \"{0}\"", text);
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.lastYearRevenue:
        //                    data.lastYearRevenue = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.certification:
        //                    data.certification = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.siret:
        //                    data.siret = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.novalues:
        //                    break;
        //                default:
        //                    data.unknowInfos.Add(text);
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private static void GetDataFicheBlocNo3(Unea_DetailCompany1 data, XXElement xe)
        //{
        //    // <div class="fiche_bloc">
        //    // <div class="fiche_entete">
        //    // <!-- <h3>Logo UNEA</h3> -->
        //    // <h1>Nous contacter</h1>
        //    // </div>
        //    // <div class="fiche_contenu">
        //    // <table cellpadding="0" cellspacing="0" border="0">
        //    // <tr valign="top">
        //    // <td class="fiche_infos_1col">
        //    // <p>
        //    // <span>Adresse : </span><br>
        //    // 14 RUE DU PERIGORD<br>
        //    // 68270 WITTENHEIM<br>
        //    // <!-- <a href="http://maps.google.fr/maps?f=q&source=s_q&hl=fr&geocode=&q=&iwloc=addr" target="_blank"><img src="images/carte.png" border="0">Localiser sur une carte</a><br> -->&nbsp;
        //    // </p>
        //    // <p>
        //    // <p><span>Téléphone : </span>0389570210</p>
        //    // <p><span>Fax : </span>0389571761</p>
        //    // <p style="padding-top: 10px;"><span>Adresse e-mail : </span>
        //    // <a href="mailto:info@alsace-ea.com">info@alsace-ea.com</a>
        //    // </p>
        //    // <p>
        //    // <span>Site internet : </span>
        //    // <a href="http://www.alsace-ea.com" target="_blank">http://www.alsace-ea.com</a>
        //    // </p>
        //    // </td>
        //    // </tr>
        //    // </table>
        //    // </div>
        //    // </div>

        //    //   fiche_bloc no 3 : address, phone, fax, email, webSite
        //    Unea_TextType textType = Unea_TextType.unknow;
        //    foreach (string text in xe.DescendantTextList(".//table", func: __trimFunc2))
        //    {
        //        if (text == "")
        //            continue;
        //        if (text.Equals("Adresse", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.address;
        //        else if (text.Equals("Téléphone", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.phone;
        //        else if (text.Equals("Fax", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.fax;
        //        else if (text.Equals("Adresse e-mail", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.email;
        //        else if (text.Equals("Site internet", StringComparison.InvariantCultureIgnoreCase))
        //            textType = Unea_TextType.webSite;
        //        else
        //        {
        //            switch (textType)
        //            {
        //                case Unea_TextType.address:
        //                    if (data.address == null)
        //                        data.address = text;
        //                    else
        //                        data.address += " " + text;
        //                    break;
        //                case Unea_TextType.phone:
        //                    data.phone = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.fax:
        //                    data.fax = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.email:
        //                    data.email = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                case Unea_TextType.webSite:
        //                    data.webSite = text;
        //                    textType = Unea_TextType.unknow;
        //                    break;
        //                default:
        //                    data.unknowInfos.Add(text);
        //                    break;
        //            }
        //        }
        //    }

        //}

        public static Unea_DetailCompany1 LoadCompany(string url, bool reload = false, bool loadImage = false)
        {
            Unea_LoadDetailCompany1FromWeb load = new Unea_LoadDetailCompany1FromWeb(url, reload, loadImage);
            load.Load();
            return load.Data;
        }
    }

    public class Unea_LoadDetailCompany1 : LoadWebData_v1<Unea_DetailCompany1>
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

        protected Unea_LoadDetailCompany1(string url)
            : base(url)
        {
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "Unea detail company 1";
        }

        protected override Unea_DetailCompany1 LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            return Unea_LoadDetailCompany1FromWeb.LoadCompany(Url, reload, loadImage);
        }

        public static Unea_DetailCompany1 LoadCompany(string url, bool reload = false, bool loadImage = false)
        {
            Unea_LoadDetailCompany1 load = new Unea_LoadDetailCompany1(url);
            load.Load(reload, loadImage);
            return load.Data;
        }
    }
}

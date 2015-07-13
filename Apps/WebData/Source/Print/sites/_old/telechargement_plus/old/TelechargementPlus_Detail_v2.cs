using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

namespace Download.Print.TelechargementPlus
{
    //public class TelechargementPlus_Print : TelechargementPlus_Base
    //{
    //    public string url = null;
    //    public DateTime? loadFromWebDate = null;
    //    //public string title = null;
    //    public string category = null;
    //    //public NamedValues<ZValue> infos = new NamedValues<ZValue>();
    //    public List<ImageHtml> images = new List<ImageHtml>();
    //    public List<string> downloadLinks = new List<string>();
    //}

    public class TelechargementPlus_LoadDetailFromWeb_v2 : LoadDataFromWeb_v1<TelechargementPlus_PostDetail>
    {
        // http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017
        //private Unea_HeaderCompany _header;
        private bool _loadImage = false;
        //private static bool __trace = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;
        private static Func<string, string> __trimFunc1 = text => text.Trim();
        //private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        //private static Regex __lastUpdateRegex = new Regex("[0-9]{2}-[0-9]{2}-[0-9]{4}", RegexOptions.Compiled);  // Dernière mise à jour le 18-01-2013
        //private static Regex __email1Regex = new Regex("email1\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email1 = "jeu-ser"
        //private static Regex __email2Regex = new Regex("email2\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email2 = "wanadoo.fr"
        //private static Func<string, string> __trimFunc2 = text => text.Trim(' ', '\xA0', '\t', '\r', '\n', ':', '?', '-');
        //private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        //private static Func<string, string> __trimFunc2 = text =>
        //{
        //    text = text.Trim(' ', '\xA0', '\t', '\r', '\n', ':', '?', '-');
        //    text = __badCharacters.Replace(text, " ");
        //    return text;
        //};

        //private XXElement _currentElement = null;

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
        }

        public TelechargementPlus_LoadDetailFromWeb_v2(string url, bool reload = false, bool loadImage = false)
            : base(url, reload: reload)
        {
            SetRequestParameters(new HttpRequestParameters_v1() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override TelechargementPlus_PostDetail GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            TelechargementPlus_PostDetail data = new TelechargementPlus_PostDetail();
            data.sourceUrl = Url;
            data.loadFromWebDate = DateTime.Now;

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");
            XXElement xe = xePost.XPathElement(".//div[@class='heading']//div[@class='binner']");
            //data.title = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(data.infos, xe.XPathValue(".//text()")));
            // xe.XPathValue(".//text()", TelechargementPlus.TrimFunc1)
            data.title = TelechargementPlus.ExtractTextValues(data.infos, TelechargementPlus.TrimFunc1(xe.XPathValue(".//text()")));
            data.creationDate = TelechargementPlus.ParseDateTime(xe.XPathValue(".//a//text()"));
            //data.category = xe.DescendantTextList(".//div[@class='storeinfo']").Skip(2).Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
            data.category = xe.XPathElements(".//div[@class='storeinfo']").DescendantTexts().Skip(2).Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");

            //TelechargementPlus_Print print = new TelechargementPlus_Print();
            //print.url = Url;
            //print.loadFromWebDate = DateTime.Now;
            //data.infos.SetValues(data.infos);

            //<div class="base">
            //    <div class="heading">
            //        <div class="binner">
            //            <h1>
            //                Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct] Gratuit</h1>
            //            <div class="storeinfo">
            //                <a href="http://www.telechargement-plus.com/2013/10/14/">Aujourd'hui, 11:59</a>
            //                | Cat&eacute;gorie: 
            //                <a href="http://www.telechargement-plus.com/e-book-magazines/">E-Book / Magazines</a>, 
            //                <a href="http://www.telechargement-plus.com/e-book-magazines/journaux/">Journaux</a>, 
            //                <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">Magazines</a>
            //                <!-- | Views: 16-->
            //            </div>
            //        </div>
            //    </div>
            //    <div class="maincont">
            //        <div class="binner">
            //            <div class="shortstory">
            //                <div class="story-text">
            //                    <center>
            //                        <span id="post-img">
            //                            <img src="/templates/film-gratuit/images/prez/livre.png" alt="E-Book / Magazines, Journaux, Magazines" />
            //                        </span>
            //                    </center>
            //                    <span id="post-img">
            //                        <div style="text-align: center;">
            //                            <br />
            //                            <!--dle_image_begin:http://www.hapshack.com/images/TX72Y.jpg|-->
            //                            <img src="http://www.hapshack.com/images/TX72Y.jpg" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                            <br />
            //                            <br />
            //                            <b>Editeur :</b> Presse Fr<br />
            //                            <b>Date de sortie :</b> 2013
            //                            <br />
            //                            <b>H�bergeur : </b>Multi / 
            //                            <b>
            //                                <!--colorstart:#FF0000-->
            //                                <span style="color: #FF0000">
            //                                    <!--/colorstart-->
            //                                    [Link Direct]<!--colorend-->
            //                                </span><!--/colorend-->
            //                            </b>
            //                            <br />
            //                            <br />
            //                            <!--dle_image_begin:http://prezup.eu/prez/infossurlebook.png|-->
            //                            <img src="http://prezup.eu/prez/infossurlebook.png" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                <br />
            //                            <br />
            //                            <b>Advanced Cr�ation Photoshop HS�rie N�19 - Novembre 2013 [Lien Direct]</b>
            //                            <br />
            //                            PDF | French | 186 pages | 100 MB<br />
            //                            <br />
            //                            <b>Le CD | zipper/22 Fichiers &+ | 520 MB</b><br />
            //                            37 Projets complets<br />
            //                            SAVOIR TOUT FAIRE : Avec Photoshop Volume XIII<br />
            //                            SPECIAL PHOTOMONTAGE & PEINTURE NUMERIQUE<br />
            //                            BONUS : 2 Tutoriels Illustrator<br />
            //                            / / /
            //                            <br />
            //                            <br />
            //                        </div>
            //                    </span>
            //                    <span id="post-img">
            //                        <div id="news-id-86887" style="display: inline;">
            //                            *<br />
            //                            *<br />
            //                            *<br />
            //                            <div style="text-align: center;">
            //                                <b>
            //                                    <!--sizestart:6-->
            //                                    <span style="font-size: 24pt;">
            //                                        <!--/sizestart-->
            //                                        <!--colorstart:#FF6600-->
            //                                        <span style="color: #FF6600">
            //                                            <!--/colorstart-->
            //                                            Cloudzer<!--colorend-->
            //                                        </span><!--/colorend--><!--sizeend-->
            //                                    </span><!--/sizeend-->
            //                                    =
            //                                    <!--colorstart:#FF0000-->
            //                                    <span style="color: #FF0000">
            //                                        <!--/colorstart-->
            //                                        [Link Direct]<!--colorend-->
            //                                    </span><!--/colorend-->
            //                                </b>
            //                                <br />
            //                                <br />
            //                                <a href="http://clz.to/q83zrwga" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/0THnp.gif|-->
            //                                    <img src="http://www.hapshack.com/images/0THnp.gif" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <a href="http://ul.to/ukqruco3" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/9MfYk.gif|-->
            //                                    <img src="http://www.hapshack.com/images/9MfYk.gif" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <br />
            //                                <a href="http://hulkfile.eu/gap3aafrlmaj.html" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/Js84x.jpg|-->
            //                                    <img src="http://www.hapshack.com/images/Js84x.jpg" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <br />
            //                                <a href="http://turbobit.net/blki3znuvzeg.html" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/QYeW0.gif|-->
            //                                    <img src="http://www.hapshack.com/images/QYeW0.gif" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <br />
            //                                *<br />
            //                                *<br />
            //                                <b>Le CD &+ : </b>
            //                                <br />
            //                                http://clz.to/o58urag6<br />
            //                                http://ul.to/rpqjypm4<br />
            //                                http://hulkfile.eu/i2k3bbz835zg.html<br />
            //                                http://turbobit.net/v644k3dd8izl.html<br />
            //                                <br />
            //                                <br />
            //                                Bonne lecture<br />
            //                                *************
            //                            </div>
            //                        </div>
            //                    </span>

            //XXElement xe = _xePost.XPathElement(".//div[@class='heading']//div[@class='binner']");
            //_post.title = _print.title = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(_print.infos, xe.XPathValue(".//text()")));
            //string postDate = xe.XPathValue(".//a//text()");
            ////WriteLine("postDate : \"{0}\"", postDate);
            //// Aujourd'hui, 17:13
            ////if (postDate != null)
            ////    _print.infos.SetValue("postDate", new ZString(postDate));
            ////_print.creationDate = FrboardPrint.GetDateTime(date.Trim(_trimAll), time.Trim(_trimAll));
            //_post.creationDate = TelechargementPlus.ParseDateTime(postDate);
            //_print.category = xe.DescendantTextList(".//div[@class='storeinfo']").Skip(2).Select(s => TelechargementPlus.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");

            //print.title = data.title;
            //print.category = data.category;

            xe = xePost.XPathElement(".//div[@class='maincont']//div[@class='binner']//div[@class='story-text']");
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(Url, TelechargementPlus.ImagesToSkip, node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(Url, imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source), node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(xeImg => new ImageHtml(xeImg, Url), imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? false : true).ToList();
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(xeImg => new ImageHtml(xeImg, Url), imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode).ToList();
            data.images = xe.XPathElements(".//span[@id='post-img']")
                .DescendantNodes(node => XmlDescendant.ImageFilter(node, node2 => node2 is XElement && ((XElement)node2).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode))
                .Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, Url))
                .Where(imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source))
                .ToList();
            if (_loadImage)
                pb.old.Http_v2.LoadImageFromWeb(data.images);

            //data.SetTextValues(xe.DescendantTextList(".//span[@id='post-img']", node => node is XElement && ((XElement)node).Name == "a" ? false : true));
            data.SetTextValues(xe.XPathElements(".//span[@id='post-img']").DescendantTexts(node => node is XElement && ((XElement)node).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode));

            data.downloadLinks.AddRange(xe.XPathValues(".//span[@id='post-img']//a/@href"));

            ////<h1 class="shd">
            ////    <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/86236-multi-ici-paris-n3562-9-au-15-octobre-2013.html">
            ////        [Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013
            ////    </a>
            ////</h1>
            //XXElement xe = xePost.XPathElement(".//*[@class='shd']//a");
            //_print.url = xe.XPathValue("@href");
            //_print.title = TrimString(ExtractTextValues(xe.XPathValue(".//text()")));

            ////<div class="shdinf">
            ////    <div class="shdinf">
            ////      <span class="rcol">Auteur: 
            ////          <a onclick="ShowProfile('bakafa', 'http://www.telechargement-plus.com/user/bakafa/', '0'); return false;" href="http://www.telechargement-plus.com/user/bakafa/">
            ////              bakafa
            ////          </a>
            ////      </span> 
            ////      <span class="date">
            ////          <b><a href="http://www.telechargement-plus.com/2013/10/09/">Aujourd'hui, 17:13</a></b>
            ////      </span>
            ////      <span class="lcol">Cat&eacute;gorie: 
            ////          <a href="http://www.telechargement-plus.com/e-book-magazines/">
            ////              E-Book / Magazines
            ////          </a> &raquo; 
            ////          <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">
            ////              Magazines
            ////          </a>
            ////      </span>
            ////    </div>
            ////</div>
            //xe = xePost.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
            //_print.postAuthor = xe.XPathValue(".//span[@class='rcol']//a//text()");
            //string postDate = xe.XPathValue(".//span[@class='date']//text()");
            //// Aujourd'hui, 17:13
            //if (postDate != null)
            //    _print.infos.SetValue("postDate", new ZString(postDate));
            //_print.category = xe.DescendantTextList(".//span[@class='lcol']").Select(s => TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
            ////.zForEach(s => s.Trim())

            ////<span id="post-img">
            ////    <div id="news-id-86236" style="display: inline;">
            ////        <div style="text-align: center;">
            ////            <!--dle_image_begin:http://zupimages.net/up/3/1515486591.jpeg|-->
            ////            <img src="http://zupimages.net/up/3/1515486591.jpeg" alt="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013"
            ////                title="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013" /><!--dle_image_end-->
            ////            <br />
            ////            <b>
            ////                <br />
            ////                Ici Paris N°3562 - 9 au 15 Octobre 2013<br />
            ////                French | 52 pages | HQ PDF | 101 MB
            ////            </b>
            ////            <br />
            ////            <br />
            ////            Ici Paris vous fait partager la vie publique et privée de celles et ceux qui font
            ////            l'actualité : exclusivités, interviews, enquêtes (la face cachée du showbiz, les
            ////            coulisses de la télé) indiscrétions, potins.<br />
            ////        </div>
            ////    </div>
            ////</span>
            //xe = xePost.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
            //_print.images = xe.XPathImages(".//img", _imagesToSkip);
            //if (_loadImage)
            //    Http2.LoadImageFromWeb(_print.images);

            return data;
        }

        //public string GetTextValue()
        //{
        //    return _currentElement.XPathConcatText(".//td//text()", separator: " ", itemFunc: s => __trimFunc1(__badCharacters.Replace(s, " "))); ;
        //}

        //public string[] GetTextValues()
        //{
        //    return _currentElement.XPathValues(".//td//text()", s => __trimFunc1(__badCharacters.Replace(s, " "))); ;
        //}

        //public static string GetEmail(string html)
        //{
        //    // var lien1 = '<a href="mailto:';
        //    // var email1 = "jeu-ser";
        //    // var lien2 = '">';
        //    // var email2 = "wanadoo.fr";
        //    // var lien3 = '</a>';
        //    // document.write(lien1 + email1 + "@" + email2 + lien2 + email1 + "@" + email2 + lien3);"
        //    if (html == null)
        //        return null;
        //    Match match1 = __email1Regex.Match(html);
        //    Match match2 = __email2Regex.Match(html);
        //    if (match1.Success && match2.Success)
        //        return match1.Groups[1].Value + "@" + match2.Groups[1].Value;
        //    else
        //    {
        //        Trace.WriteLine("warning cant extract email from\r\n{0}", html);
        //        return null;
        //    }
        //}

        //public bool SetActivityValue(Activity activity, string valueName)
        //{
        //    bool ret = true;
        //    switch (valueName.ToLower())
        //    {
        //        // NOTRE OFFRE (html)
        //        case "description":
        //            activity.description = GetTextValue();
        //            break;
        //        case "moyens techniques disponibles":
        //            activity.moyensTechniquesDisponibles = GetTextValue();
        //            break;
        //        case "effectif total mobilisable (etp)":
        //            activity.effectifTotalMobilisable = GetTextValue();
        //            break;
        //        case "modalités pratiques":
        //            activity.modalitésPratiques = GetTextValue();
        //            break;
        //        case "couverture géographique":
        //            activity.couvertureGéographique = GetTextValue();
        //            break;
        //        default:
        //            ret = false;
        //            break;
        //    }
        //    return ret;
        //}

        //public bool SetContactValue(Contact contact, string valueName)
        //{
        //    bool ret = true;
        //    switch (valueName.ToLower())
        //    {
        //        // CONTACTS (html)
        //        case "prénom et nom":
        //            contact.nom = GetTextValue();
        //            break;
        //        case "fonction":
        //            contact.fonction = GetTextValue();
        //            break;
        //        case "téléphone":
        //            contact.tel = GetTextValue();
        //            break;
        //        case "mobile":
        //            contact.mobile = GetTextValue();
        //            break;
        //        case "e-mail":
        //            contact.email = GetEmail(GetTextValue());
        //            break;
        //        default:
        //            ret = false;
        //            break;
        //    }
        //    return ret;
        //}

        //public bool SetValue(Handeco_DetailCompany company, string valueName)
        //{
        //    bool ret = true;
        //    switch (valueName.ToLower())
        //    {
        //        // LES INFOS CLES (html)
        //        case "logo":
        //            company.logo = zurl.GetUrl(Url, _currentElement.XPathValue(".//td//img/@src"));
        //            break;
        //        case "raison sociale":
        //            company.raisonSociale = GetTextValue();
        //            break;
        //        case "date de création":
        //            company.dateCréation = GetTextValue();
        //            break;
        //        case "statut juridique":
        //            company.statutJuridique = GetTextValue();
        //            break;
        //        case "type de structure":
        //            company.typeStructure = GetTextValue();
        //            break;
        //        case "site web":
        //            company.siteWeb = GetTextValue();
        //            break;
        //        case "n° siret":
        //            company.siret = GetTextValue();
        //            break;
        //        case "localisation géographique":
        //            company.localisation = GetTextValue();
        //            break;
        //        case "normes, habilitations et certifications":
        //            company.normes = GetTextValue();
        //            break;
        //        case "chiffre d'affaires annuel":
        //            company.chiffreAffairesAnnuel = GetTextValue();
        //            break;
        //        case "effectif total (etp)":
        //            company.effectifTotal = GetTextValue();
        //            break;
        //        case "effectif de production (etp)":
        //            company.effectifProduction = GetTextValue();
        //            break;
        //        case "effectif d'encadrement (etp)":
        //            company.effectifEncadrement = GetTextValue();
        //            break;
        //        case "nombre de travailleurs handicapés (etp)":
        //            company.nombreTravailleursHandicapés = GetTextValue();
        //            break;
        //        case "nombre de personnes handicapées accompagnées par an":
        //            company.nombreHandicapéAccompagné = GetTextValue();
        //            break;
        //        // RESEAUX ET PARTENAIRES (html)
        //        case "appartenance à un groupe":
        //            company.appartenanceGroupe = GetTextValue();
        //            break;
        //        case "présentation du groupe":
        //            company.présentationGroupe = GetTextValue();
        //            break;
        //        case "site web du groupe":
        //            company.siteWebGroupe = GetTextValue();
        //            break;
        //        case "adhésion à des réseaux du handicap":
        //            company.adhésionRéseauxHandicap = GetTextValue();
        //            break;
        //        case "adhésion à des groupements et fédérations professionnels":
        //            //company.adhésionGroupement = GetTextValue();
        //            company.groupes = GetTextValues();
        //            break;
        //        case "expérience de co-traitance ou de gme avec":
        //            company.cotraitance = GetTextValue();
        //            break;
        //        // NOS COORDONNEES (html)
        //        case "adresse principale":
        //            company.adressePrincipale = GetTextValue();
        //            break;
        //        case "adresse du siège":
        //            company.adresseSiège = GetTextValue();
        //            break;
        //        case "adresse des antennes":
        //            company.adresseAntennes = GetTextValue();
        //            break;
        //        case "e-mail":
        //            company.email = GetEmail(GetTextValue());
        //            break;
        //        case "tél":
        //            company.tel = GetTextValue();
        //            break;
        //        case "fax":
        //            company.fax = GetTextValue();
        //            break;
        //        case "code ape":
        //            company.codeApe = GetTextValue();
        //            break;
        //        case "n° finess":
        //            company.numeroFiness = GetTextValue();
        //            break;
        //        default:
        //            //company.unknowInfos.Add(valueName + " : " + value);
        //            ret = false;
        //            break;
        //    }
        //    return ret;
        //}

        public static TelechargementPlus_PostDetail Load(string url, bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_LoadDetailFromWeb_v2 load = new TelechargementPlus_LoadDetailFromWeb_v2(url, reload, loadImage);
            load.Load();
            return load.Data;
        }
    }

    public class TelechargementPlus_LoadDetail_v2 : LoadWebData_v1<TelechargementPlus_PostDetail>
    {
        protected static bool __useXml = false;
        protected static bool __useMongo = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            //__imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            __useXml = xe.zXPathValue("UseXml").zTryParseAs(__useXml);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        public TelechargementPlus_LoadDetail_v2(string url)
            : base(url)
        {
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "TelechargementPlus detail item";
        }

        protected override TelechargementPlus_PostDetail LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            return TelechargementPlus_LoadDetailFromWeb_v2.Load(Url, reload, loadImage);
        }

        public static TelechargementPlus_PostDetail Load(string url, bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_LoadDetail_v2 load = new TelechargementPlus_LoadDetail_v2(url);
            load.Load(reload, loadImage);
            return load.Data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

namespace Download.Print.TelechargementPlus
{
    public class TelechargementPlus_PostDetail : TelechargementPlus_Base
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;
        public DateTime? creationDate = null;
        public string category = null;
        //[MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
        public List<string> downloadLinks = new List<string>();
    }

    public static class TelechargementPlus_LoadDetail
    {
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useXml = false;
        private static string __xmlNodeName = null;
        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static pb.Web.v1.LoadWebData_v2<TelechargementPlus_PostDetail> _load;

        static TelechargementPlus_LoadDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Detail"));
        }

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            __useXml = xe.zXPathValue("UseXml").zTryParseAs(__useXml);
            __xmlNodeName = xe.zXPathValue("XmlNodeName");
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v1<TelechargementPlus_PostDetail> documentStore = null;
            if (__useMongo)
            {
                //documentStore = new MongoDocumentStoreInSpecificItem<TelechargementPlus_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                documentStore = new MongoDocumentStore_v1<TelechargementPlus_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                TelechargementPlus.InitMongoClassMap();
            }
            _load = new pb.Web.v1.LoadWebData_v2<TelechargementPlus_PostDetail>(new pb.Web.v1.LoadDataFromWeb_v2<TelechargementPlus_PostDetail>(LoadPostDetailFromWeb, GetUrlCache()), documentStore);
            //_load.SetXmlParameters(__useXml, __xmlNodeName);
            //_load.SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        public static TelechargementPlus_PostDetail Load(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
        {
            //Trace.WriteLine("TelechargementPlus_LoadDetail.Load  \"{0}\"", url);
            if (requestParameters == null)
                requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            //RequestFromWeb request = new RequestFromWeb(url, requestParameters, reload, loadImage);
            pb.Web.v1.RequestFromWeb_v2 request = new pb.Web.v1.RequestFromWeb_v2(url, requestParameters, reload, false);
            //WebDataRequest<TelechargementPlus_PostDetail> request = new WebDataRequest<TelechargementPlus_PostDetail>(url, GetPostDetailKey(url), requestParameters, reload, loadImage);
            TelechargementPlus_PostDetail postDetail = _load.Load(request, GetPostDetailKey(url));
            if (loadImage)
            {
                pb.old.Http_v2.LoadImageFromWeb(postDetail.images);
            }
            return postDetail;
        }

        public static TelechargementPlus_PostDetail LoadPostDetailFromWeb(pb.Web.v1.RequestFromWeb_v2 request)
        {
            XXElement xeSource = new XXElement(request.GetXmlDocument().Root);
            TelechargementPlus_PostDetail data = new TelechargementPlus_PostDetail();
            data.sourceUrl = request.Url;
            data.loadFromWebDate = DateTime.Now;

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");
            XXElement xe = xePost.XPathElement(".//div[@class='heading']//div[@class='binner']");
            // xe.XPathValue(".//text()", TelechargementPlus.TrimFunc1)
            data.title = TelechargementPlus.ExtractTextValues(data.infos, TelechargementPlus.TrimFunc1(xe.XPathValue(".//text()")));
            data.creationDate = TelechargementPlus.ParseDateTime(xe.XPathValue(".//a//text()"));
            //data.category = xe.DescendantTextList(".//div[@class='storeinfo']").Skip(2).Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
            data.category = xe.XPathElements(".//div[@class='storeinfo']").DescendantTexts().Skip(2).Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");

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

            xe = xePost.XPathElement(".//div[@class='maincont']//div[@class='binner']//div[@class='story-text']");
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(request.Url, TelechargementPlus.ImagesToSkip, node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(request.Url, imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source), node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(xeImg => new ImageHtml(xeImg, request.Url), imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? false : true).ToList();
            //data.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(xeImg => new ImageHtml(xeImg, request.Url), imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode).ToList();
            data.images = xe.XPathElements(".//span[@id='post-img']")
                .DescendantNodes(node => XmlDescendant.ImageFilter(node, node2 => node2 is XElement && ((XElement)node2).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode))
                .Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, request.Url))
                .Where(imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source))
                .ToList();

            if (request.LoadImage)
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

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static object GetPostDetailKey(string url)
        {
            // http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }
    }
}

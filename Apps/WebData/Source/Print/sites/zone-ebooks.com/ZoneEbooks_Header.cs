using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

namespace Download.Print.ZoneEbooks
{
    public class ZoneEbooks_PostHeader
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;
        public string urlDetail = null;

        public string title;
        public string author = null;
        public DateTime? creationDate = null;
        public string category = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
    }

    public class ZoneEbooks_HeaderPage
    {
        public ZoneEbooks_PostHeader[] postHeaders;
        public string urlNextPage;
    }

    public static class ZoneEbooks_LoadHeader
    {
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;

        //private static bool __useXml = false;
        //private static string __xmlNodeName = null;
        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static Dictionary<string, string> __imagesToSkip = null;
        private static pb.Web.v1.LoadWebData_v2<ZoneEbooks_HeaderPage> _load;

        static ZoneEbooks_LoadHeader()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("ZoneEbooks/Header"));
        }

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            //__useXml = xe.zXPathValueBool("UseXml", __useXml);
            //__xmlNodeName = xe.zXPathValue("XmlNodeName");
            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v1<ZoneEbooks_HeaderPage> documentStore = null;
            if (__useMongo)
            {
                documentStore = new MongoDocumentStore_v1<ZoneEbooks_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                ZoneEbooks.InitMongoClassMap();
            }

            _load = new pb.Web.v1.LoadWebData_v2<ZoneEbooks_HeaderPage>(new pb.Web.v1.LoadDataFromWeb_v2<ZoneEbooks_HeaderPage>(LoadHeaderPageFromWeb, GetUrlCache()), documentStore);

            InitImagesToSkip();
        }

        private static void InitImagesToSkip()
        {
            __imagesToSkip = new Dictionary<string, string>();
            __imagesToSkip.Add("http://i.imgur.com/GTPfRoB.png", "infos sur le livre"); // Florence Bellot, “Tresses et bracelets bresiliens” http://zone-ebooks.com/livres/florence-bellot-tresses-et-bracelets-bresiliens-pdf.html
            __imagesToSkip.Add("http://i.imgur.com/Ruuh4CP.png", "description 1"); // Florence Bellot, “Tresses et bracelets bresiliens” http://zone-ebooks.com/livres/florence-bellot-tresses-et-bracelets-bresiliens-pdf.html
            __imagesToSkip.Add("http://www.telechargement-plus.com/mesimages/mag.png", "pdf-ebook-mag-telechargement-plus.com"); // France Football Mardi N 3521 – 1er Octobre 2013 http://zone-ebooks.com/magazines/france-football-mardi-n-3521-1er-octobre-2013-pdf.html
            __imagesToSkip.Add("http://prezup.eu/prez/description.png", "description 2"); // http://zone-ebooks.com/livres/la-derniere-recolte-john-grisham-pdf.html
            __imagesToSkip.Add("http://prezup.eu/prez/infossurlupload.png", "infos sur l'upload"); // http://zone-ebooks.com/livres/la-derniere-recolte-john-grisham-pdf.html
            __imagesToSkip.Add("http://prezup.eu/prez/infossurlebook.png", "infos sur l'ebook"); // http://zone-ebooks.com/bande-dessinee/al-togo-tome-5-cissie-mnatogo-bd-pdf.html
            __imagesToSkip.Add("http://prezup.eu/prez/liens.png", "liens"); // http://zone-ebooks.com/livres/la-derniere-recolte-john-grisham-pdf.html
            __imagesToSkip.Add("http://www.hapshack.com/images/0THnp.gif", "cloudzer"); // http://zone-ebooks.com/magazines/valeurs-actuelles-n-4010-3-au-9-octobre-2013-2-pdf.html
            __imagesToSkip.Add("http://www.hapshack.com/images/9MfYk.gif", "uploaded"); // http://zone-ebooks.com/magazines/valeurs-actuelles-n-4010-3-au-9-octobre-2013-2-pdf.html
            __imagesToSkip.Add("http://www.hapshack.com/images/QYeW0.gif", "turbobit"); // http://zone-ebooks.com/magazines/valeurs-actuelles-n-4010-3-au-9-octobre-2013-2-pdf.html
            //__imagesToSkip.Add("", ""); // 
            //__imagesToSkip.Add("", ""); // 
            //__imagesToSkip.Add("", ""); // 
        }

        public static ZoneEbooks_HeaderPage Load(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
        {
            pb.Web.v1.RequestFromWeb_v2 request = new pb.Web.v1.RequestFromWeb_v2(url, requestParameters, reload, loadImage);
            return _load.Load(request);
        }

        public static ZoneEbooks_HeaderPage LoadHeaderPageFromWeb(pb.Web.v1.RequestFromWeb_v2 request)
        {
            XXElement xeSource = new XXElement(request.GetXmlDocument().Root);
            //string url = request.Url;
            ZoneEbooks_HeaderPage data = new ZoneEbooks_HeaderPage();





            // post list :
            //   <div id="post-1838" class="post-1838 post type-post status-publish format-standard hentry category-journaux tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook-gratuit tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-gratuit tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-telechargement tag-telecharge-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ddl tag-telecharge-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-uptobox tag-telechargement-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf clear-block count-1 odd author-admin first">
            //   _hxr.ReadSelect("//div[starts-with(@id, 'post-')]:.:EmptyRow");
            // next page :
            //   <a href='http://zone-ebooks.com/page/2' class='nextpostslink'>»</a>
            //   _hxr.ReadSelect("//a[@class='nextpostslink']:.:EmptyRow", "./@href");
            data.urlNextPage = zurl.GetUrl(request.Url, xeSource.XPathValue("//a[@class='nextpostslink']/@href"));
            Trace.WriteLine("urlNextPage \"{0}\"", data.urlNextPage);
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[starts-with(@id, 'post-')]");

            List<ZoneEbooks_PostHeader> headers = new List<ZoneEbooks_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                ZoneEbooks_PostHeader header = new ZoneEbooks_PostHeader();
                header.sourceUrl = request.Url;
                header.loadFromWebDate = DateTime.Now;

                //<h2 class="title">
                //    <a href="http://zone-ebooks.com/journaux/le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf.html"
                //        rel="bookmark" title="Lien permanent: Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre">
                //        Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre</a>
                //</h2>

                XXElement xe = xeHeader.XPathElement(".//*[@class='title']//a");
                header.urlDetail = xe.XPathValue("@href");
                header.title = xe.XPathValue(".//text()");

                //<div class="post-date">
                //    <span class="ext">Il y a 2 heures</span>
                //</div>
                string postDate = xeHeader.XPathValue(".//div[@class='post-date']//text()");
                //WriteLine("post date \"{0}\"", postDate);
                //Il y a 57 secondes
                //Il y a 3 minutes
                //Il y a 1 heure
                //Il y a 1 jour
                //Il y a 2 semaines
                //Il y a 2 mois
                if (postDate != null)
                    header.infos.Add("postDate", new ZString(postDate));

                //<div class="post-info">
                //    <span class="a">par 
                //        <a href="http://zone-ebooks.com/author/admin" title="Articles par admin ">
                //            admin
                //        </a>
                //    </span>
                //    dans
                //    <a href="http://zone-ebooks.com/category/journaux" rel="tag" title="Journaux (158 sujets)">Journaux</a>
                //</div>
                xe = xeHeader.XPathElement(".//div[@class='post-info']");
                header.author = xe.XPathValue(".//a//text()");
                header.category = xe.XPathValue("./a//text()");

                //<div class="post-content clear-block">
                xe = xeHeader.XPathElement(".//div[starts-with(@class, 'post-content')]");

                //<img title="Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre  PDF"
                //    alt="Le Parisien + Journal de Paris & supp Economie du lundi 07 octobre  PDF"
                //    src="http://i.imgur.com/f7aWDHF.jpg" width="362" height="446" />
                //header.images = xe.XPathImages(request.Url, __imagesToSkip);
                //header.images = xe.XPathImages(request.Url, imageHtml => !__imagesToSkip.ContainsKey(imageHtml.Source));
                //header.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, request.Url), imageHtml => !__imagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                //header.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, request.Url), imageHtml => !__imagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                header.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, request.Url)).Where(imageHtml => !__imagesToSkip.ContainsKey(imageHtml.Source)).ToList();

                if (request.LoadImage)
                    pb.old.Http_v2.LoadImageFromWeb(header.images);
                // image "infos sur le livre" http://i.imgur.com/GTPfRoB.png
                // image "description" http://i.imgur.com/Ruuh4CP.png
                //**********************************************************************************************************************************************************************************
                // pb image "infos sur le livre"
                //   zone-ebooks.com_img_info_livre_02_02.html
                //   zone-ebooks.com_img_info_livre_02_02.xml
                //   <div style="text-align: center;">
                // image ok
                //     <img title="Florence Bellot, &quot;Tresses et bracelets bresiliens&quot;" alt="Florence Bellot, Tresses et bracelets bresiliens PDF" src="http://i.imgur.com/RHWAvUQ.jpg" />
                //     <p>
                // image "infos sur le livre"
                //       <img title="Florence Bellot, &quot;Tresses et bracelets bresiliens&quot;" alt="Florence Bellot, Tresses et bracelets bresiliens PDF" src="http://i.imgur.com/GTPfRoB.png" />
                //     </p>
                //   ...
                //     <p>
                // image "description"
                //       <img title="Florence Bellot, &quot;Tresses et bracelets bresiliens&quot;" alt="Florence Bellot, Tresses et bracelets bresiliens PDF" src="http://i.imgur.com/Ruuh4CP.png" />
                //     </p>
                //**********************************************************************************************************************************************************************************

                headers.Add(header);
            }

            data.postHeaders = headers.ToArray();
            return data;
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }
    }

    public class ZoneEbooks_LoadHeaderPages : pb.Web.v1.ILoadWebEnumDataPages_v1<ZoneEbooks_PostHeader>
    {
        private ZoneEbooks_HeaderPage _headerPage = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private static string __url = "http://zone-ebooks.com/";

        public IEnumerator<ZoneEbooks_PostHeader> LoadPage(int page, bool reload = false, bool loadImage = false)
        {
            // http://zone-ebooks.com/page/2
            string url = __url;
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page > 1)
                url += string.Format("page/{0}", page);
            _requestParameters = new HttpRequestParameters_v1();
            _requestParameters.encoding = Encoding.UTF8;
            return Load(url, reload, loadImage);
        }

        public IEnumerator<ZoneEbooks_PostHeader> LoadNextPage(bool reload = false, bool loadImage = false)
        {
            if (_headerPage != null)
                return Load(_headerPage.urlNextPage, reload, loadImage);
            else
                return null;
        }

        private IEnumerator<ZoneEbooks_PostHeader> Load(string url, bool reload, bool loadImage)
        {
            if (url != null)
            {
                _headerPage = ZoneEbooks_LoadHeader.Load(url, _requestParameters, reload, loadImage);
                if (_headerPage != null)
                    return _headerPage.postHeaders.AsEnumerable<ZoneEbooks_PostHeader>().GetEnumerator();
            }
            return null;
        }

        public static IEnumerable<ZoneEbooks_PostHeader> LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return new pb.Web.v1.LoadWebEnumDataPages_v1<ZoneEbooks_PostHeader>(new ZoneEbooks_LoadHeaderPages(), startPage, maxPage, reload, loadImage);
        }
    }
}

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

namespace Download.Print.RapideDdl.v1
{
    public class RapideDdl_PostHeader : RapideDdl_Base
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate;
        public string urlDetail;

        public string postAuthor;
        public DateTime? creationDate;
        public string category { get; set; }
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
    }

    public class RapideDdl_HeaderPage
    {
        public RapideDdl_PostHeader[] postHeaders;
        public string urlNextPage;
    }

    public static class RapideDdl_LoadHeader
    {
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        //private static bool __useXml = false;
        //private static string __xmlNodeName = null;
        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static pb.Web.v1.LoadWebData_v2<RapideDdl_HeaderPage> _load;
        //private static LoadWebData_test<TelechargementPlus_HeaderPage> _loadHeaderPage_old;

        static RapideDdl_LoadHeader()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("RapideDdl/Header"));
        }

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            //__useXml = xe.zXPathValueBool("UseXml", __useXml);
            //__xmlNodeName = xe.zXPathValue("XmlNodeName");
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v1<RapideDdl_HeaderPage> documentStore = null;
            if (__useMongo)
            {
                documentStore = new MongoDocumentStore_v1<RapideDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                RapideDdl.InitMongoClassMap();
            }

            _load = new pb.Web.v1.LoadWebData_v2<RapideDdl_HeaderPage>(new pb.Web.v1.LoadDataFromWeb_v2<RapideDdl_HeaderPage>(LoadHeaderPageFromWeb, GetUrlCache()), documentStore);
        }

        public static RapideDdl_HeaderPage Load(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
        {
            pb.Web.v1.RequestFromWeb_v2 request = new pb.Web.v1.RequestFromWeb_v2(url, requestParameters, reload, loadImage);
            //return _load.Load(request);
            RapideDdl_HeaderPage headerPage = _load.Load(request);
            if (loadImage)
            {
                foreach (var postHeader in headerPage.postHeaders)
                    pb.old.Http_v2.LoadImageFromWeb(postHeader.images);
            }
            return headerPage;
        }

        public static RapideDdl_HeaderPage LoadHeaderPageFromWeb(pb.Web.v1.RequestFromWeb_v2 request)
        {
            // loadDataFromWeb
            XXElement xeSource = new XXElement(request.GetXmlDocument().Root);
            string url = request.Url;
            RapideDdl_HeaderPage data = new RapideDdl_HeaderPage();

            //data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation']//a[text()='Next']/@href"));
            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class='base shortstory']");
            List<RapideDdl_PostHeader> headers = new List<RapideDdl_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                RapideDdl_PostHeader header = new RapideDdl_PostHeader();
                header.sourceUrl = url;
                header.loadFromWebDate = DateTime.Now;

                XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                // xe.XPathValue(".//text()", Download.Print.RapideDdl.RapideDdl.TrimFunc1)
                /////////////////////////////////header.title = Download.Print.RapideDdl.RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()").Trim(DownloadPrint.TrimChars));

                //xe = xeHeader.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
                xe = xeHeader.XPathElement(".//div[@class='shdinf']");
                header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                // Aujourd'hui, 17:13
                ////////////////////////////////header.creationDate = Download.Print.RapideDdl.RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), (DateTime)header.loadFromWebDate);

                //xe = xeHeader.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
                xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(url, TelechargementPlus.ImagesToSkip);
                //header.images = xe.XPathImages(url);
                //header.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, url)).ToList();
                header.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, url)).ToList();

                //if (request.LoadImage)
                //    Http2.LoadImageFromWeb(header.images);

                //header.SetTextValues(xe.DescendantTextList());
                header.SetTextValues(xe.DescendantTexts());

                xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//span[@class='lcol']").Select(RapideDdl.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                //header.category = xe.DescendantTextList(".//a").Select(Download.Print.RapideDdl.RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");
                header.category = xe.XPathElements(".//a").DescendantTexts().Select(Download.Print.RapideDdl.RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

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

    public class RapideDdl_LoadHeaderPages : pb.Web.v1.ILoadWebEnumDataPages_v1<RapideDdl_PostHeader>
    {
        private RapideDdl_HeaderPage _headerPage = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private static string __url = "http://www.rapide-ddl.com/ebooks/";

        public IEnumerator<RapideDdl_PostHeader> LoadPage(int page, bool reload = false, bool loadImage = false)
        {
            // http://www.rapide-ddl.com/ebooks/page/2/
            string url = __url;
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page > 1)
                url += string.Format("page/{0}/", page);
            _requestParameters = new HttpRequestParameters_v1();
            _requestParameters.encoding = Encoding.UTF8;
            return Load(url, reload, loadImage);
        }

        public IEnumerator<RapideDdl_PostHeader> LoadNextPage(bool reload = false, bool loadImage = false)
        {
            if (_headerPage != null)
                return Load(_headerPage.urlNextPage, reload, loadImage);
            else
                return null;
        }

        private IEnumerator<RapideDdl_PostHeader> Load(string url, bool reload, bool loadImage)
        {
            if (url != null)
            {
                _headerPage = RapideDdl_LoadHeader.Load(url, _requestParameters, reload, loadImage);
                if (_headerPage != null)
                    return _headerPage.postHeaders.AsEnumerable<RapideDdl_PostHeader>().GetEnumerator();
            }
            return null;
        }

        public static IEnumerable<RapideDdl_PostHeader> LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return new pb.Web.v1.LoadWebEnumDataPages_v1<RapideDdl_PostHeader>(new RapideDdl_LoadHeaderPages(), startPage, maxPage, reload, loadImage);
        }

        public static void LoadNewPages()
        {
        }
    }
}

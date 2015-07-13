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

namespace Download.Print.FreeTelechargement
{
    public class FreeTelechargement_PostHeader
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate;
        public string urlDetail;

        public List<WebImage> images;
    }

    public class FreeTelechargement_HeaderPage
    {
        public FreeTelechargement_PostHeader[] postHeaders;
        public string urlNextPage;
    }

    public class FreeTelechargement_LoadHeaderPageFromWebManager : LoadDataFromWebManager_v3<FreeTelechargement_HeaderPage>
    {
        public FreeTelechargement_LoadHeaderPageFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        protected override FreeTelechargement_HeaderPage GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            throw new PBException("attention mismatch between free-telechargement.org and golden-ddl.net");
#pragma warning disable 162
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            string url = loadDataFromWeb.request.Url;
            FreeTelechargement_HeaderPage data = new FreeTelechargement_HeaderPage();

            // <div class="pagination">
            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='pagination']//a[starts-with(text(), 'suiv ')]/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='content']//table//a");
            List<FreeTelechargement_PostHeader> headers = new List<FreeTelechargement_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                FreeTelechargement_PostHeader header = new FreeTelechargement_PostHeader();
                header.sourceUrl = url;
                header.loadFromWebDate = loadDataFromWeb.loadFromWebDate;

                header.urlDetail = xeHeader.XPathValue("@href");

                //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();
                header.images = xeHeader.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                //header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));

                //xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
                //header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                //// Aujourd'hui, 17:13
                //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);

                //xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //RapideDdl.SetTextValues(header, xe.DescendantTextList());

                //xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//a").Select(RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

                headers.Add(header);
            }
            data.postHeaders = headers.ToArray();
            return data;
#pragma warning restore 162
        }
    }

    public static class FreeTelechargement_LoadHeaderPage
    {
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static LoadWebDataManager_v3<FreeTelechargement_HeaderPage> _load;

        static FreeTelechargement_LoadHeaderPage()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("FreeTelechargement/Header"));
        }

        public static LoadWebDataManager_v3<FreeTelechargement_HeaderPage> Load { get { return _load; } }

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v2<FreeTelechargement_HeaderPage> documentStore = null;
            if (__useMongo)
                documentStore = new MongoDocumentStore_v2<FreeTelechargement_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);

            _load = new LoadWebDataManager_v3<FreeTelechargement_HeaderPage>(new FreeTelechargement_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }
    }

    public class FreeTelechargement_LoadHeaderPages : LoadWebEnumDataPages_v2<FreeTelechargement_PostHeader>
    {
        private static string __url = "http://www.free-telechargement.org/{0}/categorie-Magazines/";
        private FreeTelechargement_HeaderPage _headerPage = null;
        private HttpRequestParameters_v1 _requestParameters = null;

        public FreeTelechargement_LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
            : base(startPage, maxPage, reload, loadImage)
        {
        }

        protected override IEnumerator<FreeTelechargement_PostHeader> LoadPage(int page, bool reload, bool loadImage)
        {
            // http://www.free-telechargement.org/2/categorie-Magazines/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = string.Format(__url, page);
            _requestParameters = new HttpRequestParameters_v1();
            _requestParameters.encoding = Encoding.Default;
            return _Load(url, reload, loadImage);
        }

        protected override IEnumerator<FreeTelechargement_PostHeader> LoadNextPage(bool reload, bool loadImage)
        {
            if (_headerPage != null)
                return _Load(_headerPage.urlNextPage, reload, loadImage);
            else
                return null;
        }

        private IEnumerator<FreeTelechargement_PostHeader> _Load(string url, bool reload, bool loadImage)
        {
            if (url != null)
            {
                // dont use mongo to store header page so key is null and refreshDocumentStore is false
                RequestWebData_v3 request = new RequestWebData_v3(new RequestFromWeb_v3(url, _requestParameters, reload, loadImage));
                _headerPage = FreeTelechargement_LoadHeaderPage.Load.Load(request).Document;
                if (loadImage)
                {
                    foreach (var postHeader in _headerPage.postHeaders)
                        FreeTelechargement.LoadImages(postHeader.images);
                }
                if (_headerPage != null)
                    return _headerPage.postHeaders.AsEnumerable<FreeTelechargement_PostHeader>().GetEnumerator();
            }
            return null;
        }

        public static IEnumerable<FreeTelechargement_PostHeader> Load(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return new FreeTelechargement_LoadHeaderPages(startPage, maxPage, reload, loadImage);
        }
    }
}

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

namespace Download.Print.TelechargementPlus
{
    public class TelechargementPlus_Post
    {
        public TelechargementPlus_PostHeader header = null;
        public TelechargementPlus_PostDetail detail = null;
    }

    public class TelechargementPlus_PostHeader : TelechargementPlus_Base
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;
        public string urlDetail = null;

        public string author = null;
        public string postAuthor;
        public DateTime? creationDate = null;
        public string category = null;
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
    }

    public class TelechargementPlus_HeaderPage
    {
        public TelechargementPlus_PostHeader[] postHeaders;
        public string urlNextPage;
    }

    public static class TelechargementPlus_LoadHeader
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

        private static pb.Web.v1.LoadWebData_v2<TelechargementPlus_HeaderPage> _load;
        //private static LoadWebData_test<TelechargementPlus_HeaderPage> _loadHeaderPage_old;

        static TelechargementPlus_LoadHeader()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Header"));
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

            //UrlCache urlCache = null;
            //if (__useUrlCache)
            //    urlCache = new UrlCache(__cacheDirectory, __urlFileNameType);
            IDocumentStore_v1<TelechargementPlus_HeaderPage> documentStore = null;
            if (__useMongo)
            {
                //documentStore = new MongoDocumentStoreInSpecificItem<TelechargementPlus_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                documentStore = new MongoDocumentStore_v1<TelechargementPlus_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                TelechargementPlus.InitMongoClassMap();
            }

            _load = new pb.Web.v1.LoadWebData_v2<TelechargementPlus_HeaderPage>(new pb.Web.v1.LoadDataFromWeb_v2<TelechargementPlus_HeaderPage>(LoadHeaderPageFromWeb, GetUrlCache()), documentStore);
            //_loadHeaderPage.SetXmlParameters(__useXml, __xmlNodeName);
            //_loadHeaderPage.SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);

            //_loadHeaderPage_old = new LoadWebData_test<TelechargementPlus_HeaderPage>(urlCache);
            //_loadHeaderPage_old.SetGetDataFromWeb(LoadHeaderPageFromWeb);
        }

        public static TelechargementPlus_HeaderPage Load(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
        {
            pb.Web.v1.RequestFromWeb_v2 request = new pb.Web.v1.RequestFromWeb_v2(url, requestParameters, reload, loadImage);
            return _load.Load(request);
        }

        //public static TelechargementPlus_HeaderPage Load_old(string url, HttpRequestParameters requestParameters = null, bool reload = false, bool loadImage = false)
        //{
        //    RequestFromWeb request = new RequestFromWeb(url, requestParameters, reload, loadImage);
        //    return _loadHeaderPage_old.Load(request);
        //}

        public static TelechargementPlus_HeaderPage LoadHeaderPageFromWeb(pb.Web.v1.RequestFromWeb_v2 request)
        {
            // loadDataFromWeb
            XXElement xeSource = new XXElement(request.GetXmlDocument().Root);
            string url = request.Url;
            TelechargementPlus_HeaderPage data = new TelechargementPlus_HeaderPage();

            // post list :
            //   <div class="base shortstory">
            //   _hxr.ReadSelect("//div[@class='base shortstory']:.:EmptyRow", ".//text()");
            // next page :
            //   <div class="navigation">
            //     <div align="center">
            //       <span>Prev.</span> 
            //       <span>1</span> 
            //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">2</a> 
            //       ...
            //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">Next</a>
            //     </div>
            //   </div>
            //   _hxr.ReadSelect("//div[@class='navigation']//a[text()='Next']:.:EmptyRow", "text()", "@href");
            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation']//a[text()='Next']/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class='base shortstory']");
            List<TelechargementPlus_PostHeader> headers = new List<TelechargementPlus_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                TelechargementPlus_PostHeader header = new TelechargementPlus_PostHeader();
                //_postHeader.sourceUrl = _sourceUrl;
                header.sourceUrl = url;
                header.loadFromWebDate = DateTime.Now;

                //<h1 class="shd">
                //    <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/86236-multi-ici-paris-n3562-9-au-15-octobre-2013.html">
                //        [Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013
                //    </a>
                //</h1>
                XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(header.infos, xe.XPathValue(".//text()")));
                // xe.XPathValue(".//text()", TelechargementPlus.TrimFunc1)
                header.title = TelechargementPlus.ExtractTextValues(header.infos, TelechargementPlus.TrimFunc1(xe.XPathValue(".//text()")));

                //<div class="shdinf">
                //    <div class="shdinf">
                //      <span class="rcol">Auteur: 
                //          <a onclick="ShowProfile('bakafa', 'http://www.telechargement-plus.com/user/bakafa/', '0'); return false;" href="http://www.telechargement-plus.com/user/bakafa/">
                //              bakafa
                //          </a>
                //      </span> 
                //      <span class="date">
                //          <b><a href="http://www.telechargement-plus.com/2013/10/09/">Aujourd'hui, 17:13</a></b>
                //      </span>
                //      <span class="lcol">Cat&eacute;gorie: 
                //          <a href="http://www.telechargement-plus.com/e-book-magazines/">
                //              E-Book / Magazines
                //          </a> &raquo; 
                //          <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">
                //              Magazines
                //          </a>
                //      </span>
                //    </div>
                //</div>
                xe = xeHeader.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
                header.postAuthor = xe.XPathValue(".//span[@class='rcol']//a//text()");
                //string postDate = xe.XPathValue(".//span[@class='date']//text()");
                // Aujourd'hui, 17:13
                //if (postDate != null)
                //    _postHeader.infos.SetValue("postDate", new ZString(postDate));
                header.creationDate = TelechargementPlus.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"));
                //header.category = xe.DescendantTextList(".//span[@class='lcol']").Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                header.category = xe.XPathElements(".//span[@class='lcol']").DescendantTexts().Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                //Trace.CurrentTrace.WriteLine("post header category \"{0}\"", _postHeader.category);
                //.zForEach(s => s.Trim())

                //<span id="post-img">
                //    <div id="news-id-86236" style="display: inline;">
                //        <div style="text-align: center;">
                //            <!--dle_image_begin:http://zupimages.net/up/3/1515486591.jpeg|-->
                //            <img src="http://zupimages.net/up/3/1515486591.jpeg" alt="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013"
                //                title="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013" /><!--dle_image_end-->
                //            <br />
                //            <b>
                //                <br />
                //                Ici Paris N°3562 - 9 au 15 Octobre 2013<br />
                //                French | 52 pages | HQ PDF | 101 MB
                //            </b>
                //            <br />
                //            <br />
                //            Ici Paris vous fait partager la vie publique et privée de celles et ceux qui font
                //            l'actualité : exclusivités, interviews, enquêtes (la face cachée du showbiz, les
                //            coulisses de la télé) indiscrétions, potins.<br />
                //        </div>
                //    </div>
                //</span>
                xe = xeHeader.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
                //_postHeader.images = xe.XPathImages(".//img", _url, TelechargementPlus.ImagesToSkip);
                //header.images = xe.XPathImages(url, TelechargementPlus.ImagesToSkip);
                //header.images = xe.XPathImages(url, imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source));
                //header.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, url), imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                header.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, url)).Where(imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();

                if (request.LoadImage)
                    pb.old.Http_v2.LoadImageFromWeb(header.images);

                //header.SetTextValues(xe.DescendantTextList());
                header.SetTextValues(xe.DescendantTexts());

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

    public class TelechargementPlus_LoadHeaderPages : pb.Web.v1.ILoadWebEnumDataPages_v1<TelechargementPlus_PostHeader>
    {
        private TelechargementPlus_HeaderPage _headerPage = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private static string __url = "http://www.telechargement-plus.com/e-book-magazines/";

        public IEnumerator<TelechargementPlus_PostHeader> LoadPage(int page, bool reload = false, bool loadImage = false)
        {
            // http://www.telechargement-plus.com/e-book-magazines/page/2/
            string url = __url;
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page > 1)
                url += string.Format("page/{0}/", page);
            _requestParameters = new HttpRequestParameters_v1();
            _requestParameters.encoding = Encoding.UTF8;
            return Load(url, reload, loadImage);
        }

        public IEnumerator<TelechargementPlus_PostHeader> LoadNextPage(bool reload = false, bool loadImage = false)
        {
            if (_headerPage != null)
                return Load(_headerPage.urlNextPage, reload, loadImage);
            else
                return null;
        }

        private IEnumerator<TelechargementPlus_PostHeader> Load(string url, bool reload, bool loadImage)
        {
            if (url != null)
            {
                //_headerPage = TelechargementPlus2_Header.Load(url, _requestParameters, reload, loadImage);
                _headerPage = TelechargementPlus_LoadHeader.Load(url, _requestParameters, reload, loadImage);
                if (_headerPage != null)
                    return _headerPage.postHeaders.AsEnumerable<TelechargementPlus_PostHeader>().GetEnumerator();
            }
            return null;
        }

        public static IEnumerable<TelechargementPlus_PostHeader> LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return new pb.Web.v1.LoadWebEnumDataPages_v1<TelechargementPlus_PostHeader>(new TelechargementPlus_LoadHeaderPages(), startPage, maxPage, reload, loadImage);
        }
    }

    //public class TelechargementPlus_LoadHeaderPages2_old : IWebDataPages_old<TelechargementPlus_PostHeader>
    //{
    //    private bool _reload = false;
    //    private bool _loadImage = false;
    //    private IEnumerator<TelechargementPlus_PostHeader> _enumerator = null;
    //    private int _page;
    //    private TelechargementPlus_HeaderPage _headerPage = null;
    //    private HttpRequestParameters _requestParameters = null;
    //    private static string __url = "http://www.telechargement-plus.com/e-book-magazines/";

    //    public void LoadPage(int page, bool reload = false, bool loadImage = false)
    //    {
    //        // http://www.telechargement-plus.com/e-book-magazines/page/2/
    //        _page = page;
    //        _reload = reload;
    //        _loadImage = loadImage;
    //        string url = __url;
    //        if (page < 1)
    //            throw new PBException("error wrong page number {0}", page);
    //        if (page > 1)
    //            url += string.Format("page/{0}/", page);
    //        _requestParameters = new HttpRequestParameters();
    //        _requestParameters.encoding = Encoding.UTF8;
    //        //TelechargementPlus_LoadHeader load = new TelechargementPlus_LoadHeader(Url, RequestParameters);
    //        //load.Load(Reload, LoadImage);
    //        //_headerPage = load.Data;
    //        Load(url);
    //    }

    //    public bool LoadNextPage()
    //    {
    //        string url = null;
    //        if (_headerPage != null)
    //            url = _headerPage.urlNextPage;
    //        //requestParameters = _requestParameters;
    //        //requestParameters = RequestParameters;
    //        if (url == null)
    //            return false;
    //        Load(url);
    //        return true;
    //    }

    //    private void Load(string url)
    //    {
    //        _headerPage = TelechargementPlus_LoadHeader.Load_old(url, _requestParameters, _reload, _loadImage);
    //        if (_headerPage != null)
    //            _enumerator = _headerPage.postHeaders.AsEnumerable<TelechargementPlus_PostHeader>().GetEnumerator();
    //    }

    //    public bool GetNextItem()
    //    {
    //        if (_enumerator == null)
    //            return false;
    //        return _enumerator.MoveNext();
    //    }

    //    public TelechargementPlus_PostHeader GetCurrentItem()
    //    {
    //        return _enumerator.Current;
    //    }

    //    public static IEnumerable<TelechargementPlus_PostHeader> LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
    //    {
    //        return new LoadWebDataPages2_old<TelechargementPlus_PostHeader>(new TelechargementPlus_LoadHeaderPages2_old(), startPage, maxPage, reload, loadImage);
    //    }
    //}
}

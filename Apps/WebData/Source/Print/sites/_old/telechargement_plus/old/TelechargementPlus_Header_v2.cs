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

namespace Download.Print.TelechargementPlus
{
    public class TelechargementPlus_LoadHeaderFromWeb_v2 : LoadDataFromWeb_v1<TelechargementPlus_HeaderPage>
    {
        private bool _loadImage = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
        }

        public TelechargementPlus_LoadHeaderFromWeb_v2(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
            : base(url, requestParameters, reload)
        {
            //SetRequestParameters(new HttpRequestParameters() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override TelechargementPlus_HeaderPage GetData()
        {
            return TelechargementPlus_v2.LoadHeaderFromWeb_GetData(this, _loadImage);
        }

        //protected override TelechargementPlus_HeaderPage GetData()
        //{
        //    XXElement xeSource = new XXElement(GetXmlDocument().Root);
        //    string url = Url;
        //    TelechargementPlus_HeaderPage data = new TelechargementPlus_HeaderPage();

        //    // post list :
        //    //   <div class="base shortstory">
        //    //   _hxr.ReadSelect("//div[@class='base shortstory']:.:EmptyRow", ".//text()");
        //    // next page :
        //    //   <div class="navigation">
        //    //     <div align="center">
        //    //       <span>Prev.</span> 
        //    //       <span>1</span> 
        //    //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">2</a> 
        //    //       ...
        //    //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">Next</a>
        //    //     </div>
        //    //   </div>
        //    //   _hxr.ReadSelect("//div[@class='navigation']//a[text()='Next']:.:EmptyRow", "text()", "@href");
        //    data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation']//a[text()='Next']/@href"));
        //    IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class='base shortstory']");
        //    List<TelechargementPlus_PostHeader> headers = new List<TelechargementPlus_PostHeader>();
        //    foreach (XXElement xeHeader in xeHeaders)
        //    {
        //        TelechargementPlus_PostHeader header = new TelechargementPlus_PostHeader();
        //        //_postHeader.sourceUrl = _sourceUrl;
        //        header.sourceUrl = url;
        //        header.loadFromWebDate = DateTime.Now;

        //        //<h1 class="shd">
        //        //    <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/86236-multi-ici-paris-n3562-9-au-15-octobre-2013.html">
        //        //        [Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013
        //        //    </a>
        //        //</h1>
        //        XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
        //        header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
        //        //header.title = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(header.infos, xe.XPathValue(".//text()")));
        //        header.title = TelechargementPlus.ExtractTextValues(header.infos, xe.XPathValue(".//text()", TelechargementPlus.TrimFunc1));

        //        //<div class="shdinf">
        //        //    <div class="shdinf">
        //        //      <span class="rcol">Auteur: 
        //        //          <a onclick="ShowProfile('bakafa', 'http://www.telechargement-plus.com/user/bakafa/', '0'); return false;" href="http://www.telechargement-plus.com/user/bakafa/">
        //        //              bakafa
        //        //          </a>
        //        //      </span> 
        //        //      <span class="date">
        //        //          <b><a href="http://www.telechargement-plus.com/2013/10/09/">Aujourd'hui, 17:13</a></b>
        //        //      </span>
        //        //      <span class="lcol">Cat&eacute;gorie: 
        //        //          <a href="http://www.telechargement-plus.com/e-book-magazines/">
        //        //              E-Book / Magazines
        //        //          </a> &raquo; 
        //        //          <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">
        //        //              Magazines
        //        //          </a>
        //        //      </span>
        //        //    </div>
        //        //</div>
        //        xe = xeHeader.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
        //        header.postAuthor = xe.XPathValue(".//span[@class='rcol']//a//text()");
        //        //string postDate = xe.XPathValue(".//span[@class='date']//text()");
        //        // Aujourd'hui, 17:13
        //        //if (postDate != null)
        //        //    _postHeader.infos.SetValue("postDate", new ZString(postDate));
        //        header.creationDate = TelechargementPlus.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"));
        //        //header.category = xe.DescendantTextList(".//span[@class='lcol']").Select(s => TelechargementPlus.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
        //        header.category = xe.DescendantTextList(".//span[@class='lcol']").Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
        //        //Trace.CurrentTrace.WriteLine("post header category \"{0}\"", _postHeader.category);
        //        //.zForEach(s => s.Trim())

        //        //<span id="post-img">
        //        //    <div id="news-id-86236" style="display: inline;">
        //        //        <div style="text-align: center;">
        //        //            <!--dle_image_begin:http://zupimages.net/up/3/1515486591.jpeg|-->
        //        //            <img src="http://zupimages.net/up/3/1515486591.jpeg" alt="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013"
        //        //                title="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013" /><!--dle_image_end-->
        //        //            <br />
        //        //            <b>
        //        //                <br />
        //        //                Ici Paris N°3562 - 9 au 15 Octobre 2013<br />
        //        //                French | 52 pages | HQ PDF | 101 MB
        //        //            </b>
        //        //            <br />
        //        //            <br />
        //        //            Ici Paris vous fait partager la vie publique et privée de celles et ceux qui font
        //        //            l'actualité : exclusivités, interviews, enquêtes (la face cachée du showbiz, les
        //        //            coulisses de la télé) indiscrétions, potins.<br />
        //        //        </div>
        //        //    </div>
        //        //</span>
        //        xe = xeHeader.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
        //        //_postHeader.images = xe.XPathImages(".//img", _url, TelechargementPlus.ImagesToSkip);
        //        header.images = xe.XPathImages(url, TelechargementPlus.ImagesToSkip);
        //        if (_loadImage)
        //            Http2.LoadImageFromWeb(header.images);

        //        header.SetTextValues(xe.DescendantTextList());

        //        headers.Add(header);
        //    }
        //    data.postHeaders = headers.ToArray();
        //    return data;
        //}
    }

    public class TelechargementPlus_LoadHeader_v2 : LoadWebData_v1<TelechargementPlus_HeaderPage>
    {
        // http://www.telechargement-plus.com/e-book-magazines/
        // http://www.telechargement-plus.com/e-book-magazines/page/2/
        protected static Regex __KeyRegex = new Regex("http://www.telechargement-plus.com/e-book-magazines(?:/page/([0-9]+))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //protected static string __imageCacheDirectory = "image";
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

        public TelechargementPlus_LoadHeader_v2(string url, HttpRequestParameters_v1 requestParameters = null)
            : base(url, requestParameters)
        {
            //_imageCacheDirectory = __imageCacheDirectory;
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "TelechargementPlus header";
        }

        protected override TelechargementPlus_HeaderPage LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_LoadHeaderFromWeb_v2 loadFromWeb = new TelechargementPlus_LoadHeaderFromWeb_v2(Url, RequestParameters, reload, loadImage);
            loadFromWeb.Load();
            return loadFromWeb.Data;
        }

        protected override object GetDocumentKey()
        {
            Match match = __KeyRegex.Match(Url);
            if (!match.Success)
                throw new PBException("key not found in url \"{0}\"", Url);
            string key = match.Groups[1].Value;
            if (key == "")
                key = "1";
            key = "telechargement_plus_page_" + key;
            //Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
            return key;
        }
    }

    public class TelechargementPlus_LoadHeaderPages_v2 : LoadWebDataPages_v1<TelechargementPlus_PostHeader>
    {
        private IEnumerator<TelechargementPlus_PostHeader> _enumerator = null;
        private int _page;
        protected TelechargementPlus_HeaderPage _headerPage = null;
        //private HttpRequestParameters _requestParameters = null;
        private static string __url = "http://www.telechargement-plus.com/e-book-magazines/";

        public TelechargementPlus_LoadHeaderPages_v2(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
            : base(startPage, maxPage, reload, loadImage)
        {
        }

        protected override void GetUrlPage(int page, out string url, out HttpRequestParameters_v1 requestParameters)
        {
            // http://www.telechargement-plus.com/e-book-magazines/page/2/
            _page = page;
            url = __url;
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page > 1)
                url += string.Format("page/{0}/", page);
            requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
        }

        protected override void Load()
        {
            TelechargementPlus_LoadHeader_v2 load = new TelechargementPlus_LoadHeader_v2(Url, RequestParameters);
            load.Load(Reload, LoadImage);
            _headerPage = load.Data;
            if (_headerPage != null)
                _enumerator = _headerPage.postHeaders.AsEnumerable<TelechargementPlus_PostHeader>().GetEnumerator();
        }

        protected override bool GetNextItem()
        {
            if (_enumerator == null)
                return false;
            return _enumerator.MoveNext();
        }

        protected override TelechargementPlus_PostHeader GetCurrentItem()
        {
            return _enumerator.Current;
        }

        protected override bool GetUrlNextPage(out string url, out HttpRequestParameters_v1 requestParameters)
        {
            if (_headerPage != null)
                url = _headerPage.urlNextPage;
            else
                url = null;
            //requestParameters = _requestParameters;
            requestParameters = RequestParameters;
            if (url != null)
                return true;
            else
                return false;
        }
    }
}

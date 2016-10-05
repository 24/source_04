using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
using pb.Web.Data;
using pb.Web.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebData.BlogDemoor
{
    public class BlogDemoorHeaderData : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string Date;
        public string UrlDetail;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class BlogDemoorHeaderDataPages : DataPages<BlogDemoorHeaderData>
    {
        public int Id;
    }

    public class BlogDemoorDetailData : IKeyData, IGetWebImages //, ILoadImages
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public int Id;
        public string Title;
        public Date? Date;
        public string Content;
        public WebImage[] Images;

        public BsonValue GetKey()
        {
            return Id;
        }

        //public void LoadImages(WebImageRequest imageRequest)
        //{
        //    WebImageMongoManager.Current.LoadImages(Images, imageRequest);
        //}

        public IEnumerable<WebImage> GetWebImages()
        {
            return Images;
        }
    }

    public class BlogDemoor : WebHeaderDetailMongoManagerBase<BlogDemoorHeaderData, BlogDemoorDetailData>
    {
        private static string __configName = "BlogDemoor";
        private static BlogDemoor __current = null;
        private static string __urlMainPage = "http://dccjta6europe.canalblog.com/";

        public static BlogDemoor Create(bool test)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }
            __current = new BlogDemoor();
            __current.HeaderPageNominalType = typeof(BlogDemoorHeaderDataPages);
            __current.CreateDataManager(xe);
            //__current.DetailDataManager.Version = 2;     // use WebData<TData>.Load()
            __current.DetailDataManager.ImageLoadVersion = 2;
            return __current;
        }

        protected override IEnumDataPages<BlogDemoorHeaderData> GetHeaderPageData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            BlogDemoorHeaderDataPages data = new BlogDemoorHeaderDataPages();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//a[@class='nextpage']/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class='item_div']");
            List<BlogDemoorHeaderData> headers = new List<BlogDemoorHeaderData>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                BlogDemoorHeaderData header = new BlogDemoorHeaderData();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                XXElement xe = xeHeader.XPathElement(".//h2/a");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");
                // <div class="dateheader">23 juillet 2016</div>
                header.Date = xeHeader.XPathValue(".//div[@class='dateheader']/text()");

                headers.Add(header);
            }
            data.Data = headers.ToArray();
            return data;
        }

        protected override BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        private static Regex __pageKeyRegex = new Regex(@"^p([0-9])+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://dccjta6europe.canalblog.com/
            // page 2 : http://dccjta6europe.canalblog.com/archives/p10-10.html
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            Match match = __pageKeyRegex.Match(uri.Segments[uri.Segments.Length - 1]);
            if (match.Success)
                return int.Parse(match.Groups[1].Value) / 10 + 1;
            throw new PBException("page key not found in url \"{0}\"", httpRequest.Url);
        }

        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : http://dccjta6europe.canalblog.com/
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            return new HttpRequest { Url = url };
        }

        protected override string GetDetailImageCacheUrlSubDirectory(WebData<BlogDemoorDetailData> data)
        {
            //return "Image";
            //return zPath.Combine("Image", zpath.PathSetExtension(data.Result.UrlCachePathResult.SubPath, null));
            //return zpath.PathSetExtension(data.Result.UrlCachePathResult.SubPath, null);
            //UrlCachePathResult urlCachePath = null;
            string subPath = null;
            if (data.Result != null)
                subPath = data.Result.UrlCachePathResult.SubPath;
            else
                subPath = data.Result_v2.Http.HttpRequest.UrlCachePath.SubPath;
            return "img\\" + zpath.PathSetExtension(subPath, null);
        }

        private static CultureInfo __cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
        protected override BlogDemoorDetailData GetDetailData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();

            BlogDemoorDetailData data = new BlogDemoorDetailData();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetDetailKey(webResult.WebRequest.HttpRequest);

            _GetDetailData(xeSource, data);

            return data;
        }

        protected override BlogDemoorDetailData GetDetailData_v2(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();

            BlogDemoorDetailData data = new BlogDemoorDetailData();
            data.SourceUrl = httpResult.Http.HttpRequest.Url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = _GetDetailKey(httpResult.Http.HttpRequest);

            _GetDetailData(xeSource, data);

            return data;
        }

        protected void _GetDetailData(XXElement xeSource, BlogDemoorDetailData data)
        {

            // <div id="content">

            XXElement xe = xeSource.XPathElement("//div[@id='content']//div[@class='item_div']");
            data.Title = xe.XPathValue(".//h2//text()");
            string date = xe.XPathValue(".//div[@class='dateheader']/text()");
            Date d;
            if (Date.TryParseExact(date, "d MMMM yyyy", __cultureInfo, DateTimeStyles.None, out d))
                data.Date = d;
            else
                Trace.WriteLine($"date not found \"{date}\"");

            //<div class="articlebody" itemprop="articleBody">
            XXElement xeBody = xe.XPathElement(".//div[@class='articlebody']");

            if (xeBody.XElement != null)
                data.Content = xeBody.XElement.ToString();

            data.Images = xeBody.XPathValues(".//a/@href").Where(url => new Uri(url).Host.EndsWith(".canalblog.com")).Select(url => new WebImage(zurl.GetUrl(data.SourceUrl, url))).ToArray();

            // force load image to get image width and height
            //if (webResult.WebRequest.LoadImage)
            //    data.LoadImages();

            //if (__trace)
            //    pb.Trace.WriteLine(data.zToJson());
        }

        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            // page 1 : http://dccjta6europe.canalblog.com/
            // page 2 : http://dccjta6europe.canalblog.com/archives/2016/07/20/34103996.html
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = zPath.GetFileNameWithoutExtension(uri.Segments[uri.Segments.Length - 1]);
            int key;
            if (!int.TryParse(lastSegment, out key))
                throw new PBException("detail key not found in url \"{0}\"", url);
            return key;
        }

        public void LoadNewDocuments()
        {
            _headerDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
        }

        public IEnumerable<BlogDemoorDetailData> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            if (sort == null)
                sort = "{ 'download.PostCreationDate': -1 }";
            return _detailDataManager.Find(query, sort: sort, limit: limit, loadImage: loadImage);
        }

        public BlogDemoorDetailData Load(BsonValue id)
        {
            return _detailDataManager.DocumentStore.LoadFromId(id);
        }
    }
}

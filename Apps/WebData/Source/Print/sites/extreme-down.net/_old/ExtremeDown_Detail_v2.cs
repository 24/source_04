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

namespace Download.Print.ExtremeDown.old
{
    public class ExtremeDown_PostDetail_v2 : IPost, IKeyData_v4<int>, IHttpRequestData
    {
        public ExtremeDown_PostDetail_v2()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
            DownloadLinks_new = new PostDownloadLinks();
        }

        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Title;
        public PrintType PrintType;
        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        public string Language;
        public string Size;
        public int? NbPages;
        public NamedValues<ZValue> Infos;
        public WebImage[] Images;
        //public string[] DownloadLinks;
        public PostDownloadLinks DownloadLinks_new;

        public string GetServer()
        {
            return "extreme-down.net";
        }

        public int GetKey()
        {
            return Id;
        }

        public HttpRequest GetDataHttpRequest()
        {
            return new HttpRequest { Url = SourceUrl };
        }

        public DateTime GetLoadFromWebDate()
        {
            return LoadFromWebDate;
        }

        public string GetTitle()
        {
            return Title;
        }

        public PrintType GetPrintType()
        {
            return PrintType;
        }

        public string GetOriginalTitle()
        {
            return OriginalTitle;
        }

        public string GetPostAuthor()
        {
            return PostAuthor;
        }

        public DateTime? GetPostCreationDate()
        {
            return PostCreationDate;
        }

        public WebImage[] GetImages()
        {
            return Images;
        }

        public void SetImages(WebImage[] images)
        {
            Images = images;
        }

        public string[] GetDownloadLinks()
        {
            //return DownloadLinks;
            return null;
        }

        public PostDownloadLinks GetDownloadLinks_new()
        {
            return DownloadLinks_new;
        }
    }

    public static class ExtremeDown_DetailManager_v2
    {
        private static bool __trace = false;
        private static WebDataManager_v1<int, ExtremeDown_PostDetail_v2> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v1<int, ExtremeDown_HeaderPage_v2, ExtremeDown_PostHeader_v2, int, ExtremeDown_PostDetail_v2> __webHeaderDetailManager = null;
        private static bool __getLinksExtremeProtect = true;
        private static ExtremeProtect __extremeProtect = new ExtremeProtect();

        static ExtremeDown_DetailManager_v2()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("ExtremeDown/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager_v1<int, ExtremeDown_HeaderPage_v2, ExtremeDown_PostHeader_v2, int, ExtremeDown_PostDetail_v2>();
            __webHeaderDetailManager.HeaderDataPageManager = ExtremeDown_HeaderManager_v2.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager_v1<int, ExtremeDown_PostDetail_v2> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v1<int, ExtremeDown_HeaderPage_v2, ExtremeDown_PostHeader_v2, int, ExtremeDown_PostDetail_v2> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager_v1<int, ExtremeDown_PostDetail_v2> CreateWebDataManager(XElement xe)
        {
            WebDataManager_v1<int, ExtremeDown_PostDetail_v2> detailWebDataManager = new WebDataManager_v1<int, ExtremeDown_PostDetail_v2>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<ExtremeDown_PostDetail_v2>();

            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
            //    detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            //detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = ExtremeDown_v2.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            detailWebDataManager.LoadImages = DownloadPrint_v1.LoadImages;

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Vosbooks_PostDetail> documentStore = new MongoDocumentStore<int, Vosbooks_PostDetail>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    detailWebDataManager.DocumentStore = documentStore;
            //}

            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Vosbooks_HeaderPage>(document);
            detailWebDataManager.DocumentStore = MongoDocumentStore_v4<int, ExtremeDown_PostDetail_v2>.Create(xe);

            return detailWebDataManager;
        }

        private static ExtremeDown_PostDetail_v2 GetData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            ExtremeDown_PostDetail_v2 data = new ExtremeDown_PostDetail_v2();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPostDetailKey(webResult.WebRequest.HttpRequest);

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");

            //data.Title = xePost.XPathValue(".//h2[@class='blocktitle']//text()", DownloadPrint.Trim);
            data.Title = xePost.XPathValue(".//h2[@class='blocktitle']//text()").Trim(DownloadPrint.TrimChars);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            XXElement xeDiv = xePost.XPathElement(".//div[@class='blockheader']");

            data.Category = xeDiv.XPathValues(".//i[@class='icon-cats']/ancestor::span//a//text()").Select(DownloadPrint.Trim).zToStringValues("/");
            //string category = data.Category.ToLowerInvariant();
            data.PrintType = GetPrintType(data.Category);

            data.PostAuthor = xeDiv.XPathValue(".//span/i[@class='icon-user']/ancestor::span//a//text()");

            string date = xeDiv.XPathValue(".//span/i[@class='icon-date']/ancestor::span//a//text()");
            data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, webResult.LoadFromWebDate, "d-M-yyyy, HH:mm", "d M yyyy", "d MMMM yyyy");
            if (data.PostCreationDate == null)
                pb.Trace.WriteLine("unknow date time \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("creationDate {0} - \"{1}\"", data.PostCreationDate, date);

            xeDiv = xePost.XPathElement(".//div[@class='blockcontent']");

            List<string> description = new List<string>();
            description.AddRange(xeDiv.XPathValues(".//p[@class='release-name']//text()"));

            //data.Images = xeDiv.XPathElement(".//table//td[@class='image-block']").XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();
            data.Images = xeDiv.XPathElement(".//table//td[@class='image-block']").DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            description.AddRange(xeDiv.XPathValues(".//table//td//blockquote//text()"));

            //xeDiv = xePost.XPathElement(".//div[@class='clearfix']");
            xeDiv = xePost.XPathElement(".//div[@class='upload-infos clearfix']");
            description.AddRange(xeDiv.XPathValues(".//table//text()"));

            data.Description = description.ToArray();

            string title = null;
            // xePost.XPathElements(".//script/parent::div//following-sibling::h2")
            Func<XXElement, XNodeFilter> filter =
                xe =>
                {
                    if (xe.XElement.Name == "h2")
                        title = xe.XPathValue(".//text()");
                    else if (xe.XElement.Name == "script")
                        return XNodeFilter.Stop;
                    else if (xe.XElement.Name == "div")
                        return XNodeFilter.SelectNode;
                    return XNodeFilter.DontSelectNode;
                };
            foreach (XXElement xe in xePost.XPathElements(".//div[@class='prez_2']//following-sibling::*").zFilterElements(filter))
            {
                //string s = xe.XPathValue(".//text()");
                //// Liens de téléchargement - Pack 1
                //if (s.StartsWith("Liens de téléchargement"))
                //{
                //    s = s.Substring(23).Trim(' ', '-');
                //    if (s == "")
                //        s = title;
                //    else if (title != null)
                //        s = title + " - " + s;
                //    title = null;
                data.DownloadLinks_new.AddItem(title);
                title = null;
                //foreach (XXElement xe2 in xe.XPathElements("following-sibling::div[1]//a"))
                foreach (XXElement xe2 in xe.XPathElements(".//a"))
                {
                    //s = xe2.DescendantTextList().FirstOrDefault();
                    // <strong class="hebergeur">
                    string server = xe2.XPathValue(".//strong[@class='hebergeur']//text()");
                    string link = xe2.XPathValue("@href");
                    if (__getLinksExtremeProtect && __extremeProtect.IsLinkProtected(link))
                    {
                        data.DownloadLinks_new.AddServer(server, link);
                        data.DownloadLinks_new.AddLinks(__extremeProtect.UnprotectLink(link));
                    }
                    else
                    {
                        data.DownloadLinks_new.AddServer(server);
                        data.DownloadLinks_new.AddLink(link);
                    }
                }
                //}
                //else if (s != null)
                //    title = s;
            }

            //xeDiv = xePost.XPathElement(".//div[@class='blockfooter links']");
            ////data.category = xeDiv.DescendantTextList(".//i[@class='icon-cats']/parent::span//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            //data.Category = xeDiv.XPathElements(".//i[@class='icon-cats']/parent::span//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            //string category = data.Category.ToLowerInvariant();
            //data.PrintType = GetPrintType(category);
            ////pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static PrintType GetPrintType(string category)
        {
            switch (category.ToLowerInvariant())
            {
                case "ebooks/journaux":
                case "ebooks/magazines":
                    return PrintType.Print;
                //case "ebooks/livres":
                //    return PrintType.Book;
                //case "ebooks/bandes dessinée":
                //    return PrintType.Comics;
                //case "ebooks":
                //    return PrintType.UnknowEBook;
                default:
                    return PrintType.Unknow;
            }
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static int GetPostDetailKey(HttpRequest httpRequest)
        {
            // http://www.extreme-down.net/ebooks/journaux/23375-les-journaux-mardi-09-dcembre-2014.html
            // new
            // http://www.ex-down.com/ebooks/magazines/29244-science-vie-hors-serie-no273-decembre-2015.html
            Uri uri = new Uri(httpRequest.Url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
            return int.Parse(match.Value);
        }
    }
}

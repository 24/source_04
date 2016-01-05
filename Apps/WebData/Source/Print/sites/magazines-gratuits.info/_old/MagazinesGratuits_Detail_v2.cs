using System;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using Print;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using pb.Web.Data.old;

namespace Download.Print.MagazinesGratuits.old
{
    // IPostToDownload => IHttpRequestData, IKeyData, IIdData => WebDataManager_v2.SetDataId(), WebDataManager_v2.Refresh()
    public class MagazinesGratuits_PostDetail_v2 : IPostToDownload, IIdData // : IPost, IKeyData<int>, IWebData
    {
        public MagazinesGratuits_PostDetail_v2()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

        public int Id;
        public string Key;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Title;
        public PrintType PrintType;
        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        //public string Language;
        //public string Size;
        //public int? NbPages;
        public NamedValues<ZValue> Infos;
        public WebImage[] Images;
        public string[] DownloadLinks;

        // IPostToDownload
        public string GetServer()
        {
            //return "magazines-gratuits.org";
            return MagazinesGratuits_v2.Name;
        }

        public BsonValue GetId()
        {
            return Id;
        }

        public void SetId(BsonValue id)
        {
            Id = (int)id;
        }

        // IPostToDownload:IKeyData used in WebDataManager_v2.Save() and WebDataManager_v2.Refresh()
        public BsonValue GetKey()
        {
            //return Id;
            return Key;
        }

        // IPostToDownload:IHttpRequestData
        public HttpRequest GetDataHttpRequest()
        {
            return new HttpRequest { Url = SourceUrl };
        }

        //public DateTime GetLoadFromWebDate()
        //{
        //    return LoadFromWebDate;
        //}

        // IPostToDownload
        public string GetTitle()
        {
            return Title;
        }

        // IPostToDownload
        public PrintType GetPrintType()
        {
            return PrintType;
        }

        //public string GetOriginalTitle()
        //{
        //    return OriginalTitle;
        //}

        //public string GetPostAuthor()
        //{
        //    return PostAuthor;
        //}

        //public DateTime? GetPostCreationDate()
        //{
        //    return PostCreationDate;
        //}

        //public WebImage[] GetImages()
        //{
        //    return Images;
        //}

        //public void SetImages(WebImage[] images)
        //{
        //    Images = images;
        //}

        public void LoadImages()
        {
            Images = DownloadPrint.LoadImages(Images).ToArray();
        }

        // IPostToDownload
        //public string[] GetDownloadLinks()
        //{
        //    return DownloadLinks;
        //}

        // IPostToDownload
        public PostDownloadLinks GetDownloadLinks()
        {
            return PostDownloadLinks.Create(DownloadLinks);
        }
    }

    public static class MagazinesGratuits_DetailManager_v2
    {
        private static bool __trace = false;
        private static WebDataManager<MagazinesGratuits_PostDetail_v2> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v2<MagazinesGratuits_PostDetail_v2> __webHeaderDetailManager = null;
        private static Date? __lastPostDate = null;

        static MagazinesGratuits_DetailManager_v2()
        {
            //Init();
            MagazinesGratuits_v2.FakeInit();
        }

        public static void Init(XElement xe)
        {
            __detailWebDataManager = CreateWebDataManager(xe.zXPathElement("Detail"));
            __webHeaderDetailManager = CreateWebHeaderDetailManager();
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager<MagazinesGratuits_PostDetail_v2> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v2<MagazinesGratuits_PostDetail_v2> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager<MagazinesGratuits_PostDetail_v2> CreateWebDataManager(XElement xe)
        {
            WebDataManager<MagazinesGratuits_PostDetail_v2> detailWebDataManager = new WebDataManager<MagazinesGratuits_PostDetail_v2>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<MagazinesGratuits_PostDetail_v2>();

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
                urlCache.GetUrlSubDirectoryFunction = GetSubDirectoryKey;
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            //detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = MagazinesGratuits_v2.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages;
            detailWebDataManager.LoadImages = data => { data.LoadImages(); };

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Vosbooks_PostDetail> documentStore = new MongoDocumentStore<int, Vosbooks_PostDetail>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    detailWebDataManager.DocumentStore = documentStore;
            //}

            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Vosbooks_HeaderPage>(document);
            //detailWebDataManager.DocumentStore = MongoDocumentStore<int, Vosbooks_PostDetail>.Create(xe);
            detailWebDataManager.DocumentStore = MongoDocumentStore<MagazinesGratuits_PostDetail_v2>.Create(xe);

            return detailWebDataManager;
        }

        private static WebHeaderDetailManager_v2<MagazinesGratuits_PostDetail_v2> CreateWebHeaderDetailManager()
        {
            WebHeaderDetailManager_v2<MagazinesGratuits_PostDetail_v2> webHeaderDetailManager = new WebHeaderDetailManager_v2<MagazinesGratuits_PostDetail_v2>();
            webHeaderDetailManager.HeaderDataPageManager = MagazinesGratuits_HeaderManager_v2.HeaderWebDataPageManager;
            webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
            return webHeaderDetailManager;
        }

        private static MagazinesGratuits_PostDetail_v2 GetData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            MagazinesGratuits_PostDetail_v2 data = new MagazinesGratuits_PostDetail_v2();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            //data.Id = _GetPostDetailKey(webResult.WebRequest.HttpRequest);
            data.Key = _GetPostDetailKey(webResult.WebRequest.HttpRequest);

            XXElement xePost = xeSource.XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']");

            XXElement xe = xePost.XPathElement(".//table[@id='post-head']");
            string[] dates = xe.XPathElement(".//td[@id='head-date']").DescendantTexts().Select(DownloadPrint.Trim).ToArray();
            data.PostCreationDate = GetDate(dates, __lastPostDate);
            if (data.PostCreationDate != null)
                __lastPostDate = new Date(data.PostCreationDate.Value);
            if (__trace)
                pb.Trace.WriteLine("post creation date {0} - {1}", data.PostCreationDate, dates.zToStringValues());

            //data.Title = xePost.XPathValue(".//div[@class='title']//a//text()", DownloadPrint.TrimFunc1);
            data.Title = xePost.XPathValue(".//div[@class='title']//a//text()").zFunc(DownloadPrint.ReplaceChars).zFunc(DownloadPrint.Trim);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            // Ebooks en Epub / Livre
            //data.Category = xePost.DescendantTextList(".//div[@class='postdata']//span[@class='category']//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            data.Category = xePost.XPathElements(".//div[@class='postdata']//span[@class='category']//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            data.PrintType = GetPrintType(data.Category);
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            xe = xePost.XPathElement(".//div[@class='entry']");
            data.Images = new WebImage[] { new WebImage(zurl.GetUrl(data.SourceUrl, xe.XPathValue("div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p/img/@src"))) };

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
            // xe.DescendantTextList(".//p")
            PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(
                xe.XPathElements(".//p").DescendantTexts(
                node =>
                {
                    if (node is XText)
                    {
                        string text = ((XText)node).Value.Trim();
                        //if (text.StartsWith("Lien Direct", StringComparison.InvariantCultureIgnoreCase))
                        if (text.StartsWith("lien ", StringComparison.InvariantCultureIgnoreCase))
                            return XNodeFilter.Stop;
                    }
                    if (node is XElement)
                    {
                        XElement xe2 = (XElement)node;
                        if (xe2.Name == "p" && xe2.zAttribValue("class") == "submeta")
                            return XNodeFilter.Stop;
                    }
                    return XNodeFilter.SelectNode;
                }
                ).Select(DownloadPrint.ReplaceChars).Select(DownloadPrint.TrimWithoutColon), data.Title);
            data.Description = textValues.description;
            //data.Language = textValues.language;
            //data.Size = textValues.size;
            //data.NbPages = textValues.nbPages;
            data.Infos.SetValues(textValues.infos);

            //data.DownloadLinks = xe.DescendantNodes(
            //    node => 
            //        {
            //            if (!(node is XElement))
            //                return true;
            //            XElement xe2 = (XElement)node;
            //            if (xe2.Name != "p")
            //                return true;
            //            XAttribute xa = xe2.Attribute("class");
            //            if (xa == null)
            //                return true;
            //            if (xa.Value != "submeta")
            //                return true;
            //            return false;
            //        },
            //    node => node is XElement && ((XElement)node).Name == "a")
            //    .Select(node => ((XElement)node).Attribute("href").Value).ToArray();
            data.DownloadLinks = xe.DescendantNodes(
                node =>
                {
                    if (!(node is XElement))
                        return XNodeFilter.DontSelectNode;
                    XElement xe2 = (XElement)node;
                    if (xe2.Name == "a")
                        return XNodeFilter.SelectNode;
                    if (xe2.Name != "p")
                        return XNodeFilter.DontSelectNode;
                    XAttribute xa = xe2.Attribute("class");
                    if (xa == null)
                        return XNodeFilter.DontSelectNode;
                    if (xa.Value != "submeta")
                        return XNodeFilter.DontSelectNode;
                    //return XNodeFilter.SkipNode;
                    return XNodeFilter.Stop;
                })
                .Select(node => ((XElement)node).Attribute("href").Value).ToArray();


            //// <div id="postlist" class="postlist restrain">
            //xe = xePost.XPathElement(".//div[@id='postlist']");

            //// Aujourd'hui, 07h32 - Aujourd'hui, 10h51 - Hier, 12h55 - 22/02/2014, 21h09
            ////string date = xe.DescendantTextList(".//div[@class='posthead']//text()", nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            //XXElement xe2 = xe.XPathElement(".//div[@class='posthead']");
            //string date = xe2.DescendantTextList(nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            //date = date.Replace('\xA0', ' ');
            //data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, webResult.LoadFromWebDate, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm");
            //if (data.PostCreationDate == null)
            //    pb.Trace.WriteLine("unknow post creation date \"{0}\"", date);

            //data.PostAuthor = xe.XPathValue(".//div[@class='userinfo']//a//text()", DownloadPrint.TrimFunc1);

            //// <div class="postbody">
            //xe = xePost.XPathElement(".//div[@class='postbody']//div[@class='content']//blockquote/div");

            //data.Images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();


            //// get infos, description, language, size, nbPages
            //PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a"), data.Title);
            //data.Description = textValues.description;
            //data.Language = textValues.language;
            //data.Size = textValues.size;
            //data.NbPages = textValues.nbPages;
            //data.Infos.SetValues(textValues.infos);

            //data.DownloadLinks = xe.XPathValues(".//a/@href");

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static DateTime? GetDate(string[] dates, Date? refDate)
        {
            if (dates.Length >= 2)
            {
                int month = zdate.GetMonthNumber(dates[0]);
                int day;
                if (month != 0 && int.TryParse(dates[1], out day))
                {
                    return zdate.GetNearestYearDate(day, month, refDate).DateTime;
                }
            }
            pb.Trace.WriteLine("unknow post creation date {0}", dates.zToStringValues());
            return null;
        }

        private static PrintType GetPrintType(string category)
        {
            return PrintType.Print;
            // return PrintType.Book;
            // return PrintType.Comics;
            // return PrintType.UnknowEBook;
            // return PrintType.Unknow;
        }

        private static string GetSubDirectoryKey(HttpRequest httpRequest)
        {
            string key = _GetPostDetailKey(httpRequest);
            int i = key.LastIndexOf('-');
            if (i != -1)
            {
                i = key.LastIndexOf('-', i + 1);
                if (i != -1)
                {
                    return key.Substring(0, i);
                }
            }
            throw new PBException("unable to get sub-directory key from url \"{0}\"", httpRequest.Url);
        }

        private static BsonValue GetPostDetailKey(HttpRequest httpRequest)
        {
            return _GetPostDetailKey(httpRequest);
        }

        private static string _GetPostDetailKey(HttpRequest httpRequest)
        {
            // http://www.magazines-gratuits.info/le-monde-et-supplement-du-dimanche-13-et-lundi-14-decembre-2015.html
            // http://www.magazines-gratuits.info/les-journaux-du-samedi-12-decembre-2015.html
            Uri uri = new Uri(httpRequest.Url);
            if (uri.Segments.Length == 2)
                return uri.Segments[1];
            else
                throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
        }
    }
}

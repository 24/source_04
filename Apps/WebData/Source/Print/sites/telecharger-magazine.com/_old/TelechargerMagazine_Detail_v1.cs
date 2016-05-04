using System;
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

namespace Download.Print.TelechargerMagazine.old
{
    public class TelechargerMagazine_PostDetail_v1 : IPost, IKeyData_v4<int>, IHttpRequestData
    {
        public TelechargerMagazine_PostDetail_v1()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
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
        public NamedValues<ZValue> Infos;
        public WebImage[] Images;
        public string[] DownloadLinks;

        public string GetServer()
        {
            return TelechargerMagazine_v1.GetServer();
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
            return DownloadLinks;
        }

        public PostDownloadLinks GetDownloadLinks_new()
        {
            return PostDownloadLinks.Create(DownloadLinks);
        }
    }

    public static class TelechargerMagazine_DetailManager_v1
    {
        private static bool __trace = false;
        private static WebDataManager_v1<int, TelechargerMagazine_PostDetail_v1> __detailWebDataManager = null;
        private static MongoDocumentStore_v4<int, TelechargerMagazine_PostDetail_v1> __detailMongoDocumentStore = null;
        private static WebHeaderDetailManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1, int, TelechargerMagazine_PostDetail_v1> __webHeaderDetailManager = null;
        //private static Date? __lastPostDate = null;

        static TelechargerMagazine_DetailManager_v1()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("TelechargerMagazine/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1, int, TelechargerMagazine_PostDetail_v1>();
            __webHeaderDetailManager.HeaderDataPageManager = TelechargerMagazine_HeaderManager_v1.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager_v1<int, TelechargerMagazine_PostDetail_v1> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static MongoDocumentStore_v4<int, TelechargerMagazine_PostDetail_v1> DetailMongoDocumentStore { get { return __detailMongoDocumentStore; } }
        public static WebHeaderDetailManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1, int, TelechargerMagazine_PostDetail_v1> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager_v1<int, TelechargerMagazine_PostDetail_v1> CreateWebDataManager(XElement xe)
        {
            WebDataManager_v1<int, TelechargerMagazine_PostDetail_v1> detailWebDataManager = new WebDataManager_v1<int, TelechargerMagazine_PostDetail_v1>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<TelechargerMagazine_PostDetail_v1>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            //detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = TelechargerMagazine_v1.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            detailWebDataManager.LoadImages = DownloadPrint_v1.LoadImages;

            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Vosbooks_HeaderPage>(document);
            __detailMongoDocumentStore = MongoDocumentStore_v4<int, TelechargerMagazine_PostDetail_v1>.Create(xe);
            detailWebDataManager.DocumentStore = __detailMongoDocumentStore;

            return detailWebDataManager;
        }

        public static TelechargerMagazine_PostDetail_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            TelechargerMagazine_PostDetail_v1 data = new TelechargerMagazine_PostDetail_v1();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPostDetailKey(webResult.WebRequest.HttpRequest);

            // la date est juste la date du jour
            // <div id="calendar-layer">
            // <table id="calendar" cellpadding="3" class="calendar">
            // ...
            // <tr>
            // ...
            // <td  class="day-active-v day-current" ><a class="day-active-v" href="http://www.telecharger-magazine.com/2015/07/17/" title="Article posté dans 17 Juillet 2015">17</a></td>
            // ...
            // </tr>
            // ...
            // </table>
            // </div>

            // <div id='dle-content'>
            // ...
            // <div class="right-full">
            //
            // <div class="cat_name">
            // Posted in:
            // <a href="http://www.telecharger-magazine.com/journaux/">Journaux</a>
            // </div>
            //
            // <h2 class="title">
            // <img src="/templates/MStarter/images/title.png" alt="" class="img" />
            // Journaux Français Du 17 Juillet 2015
            // </h2>
            //
            // <div class="contenttext">

            // la date est juste la date du jour
            // http://www.telecharger-magazine.com/2015/07/17/
            //xeSource.XPathValue("//div[@id='calendar-layer']//table[@id='calendar']//td[@class='day-active-v day-current']//a/@href");


            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']//div[@class='right-full']");

            // Journaux
            data.Category = xePost.XPathValues(".//div[@class='cat_name']//a/text()").Select(DownloadPrint.Trim).zToStringValues("/");
            data.PrintType = GetPrintType(data.Category);
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            data.Title = xePost.XPathValue(".//h2[@class='title']//text()").zFunc(DownloadPrint.ReplaceChars).zFunc(DownloadPrint.Trim);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            XXElement xeContent = xePost.XPathElement(".//div[@class='contenttext']");

            data.Images = new WebImage[] { new WebImage(zurl.GetUrl(data.SourceUrl, xeContent.XPathValue(".//img/@src"))) };

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
            PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(
                xeContent.DescendantTexts(
                node =>
                {
                    if (node is XText)
                    {
                        string text = ((XText)node).Value.Trim();
                        if (text.ToLowerInvariant() == "description")
                            return XNodeFilter.DontSelectNode;
                    }
                    if (node is XElement)
                    {
                        XElement xe = (XElement)node;
                        if (xe.Name == "a")
                            return XNodeFilter.Stop;
                    }
                    return XNodeFilter.SelectNode;
                }
                ).Select(DownloadPrint.ReplaceChars).Select(DownloadPrint.TrimWithoutColon), data.Title, extractValuesFromText: false);
            data.Description = textValues.description;
            data.Infos.SetValues(textValues.infos);

            data.DownloadLinks = xeContent.DescendantNodes(
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
            data.DownloadLinks = xeContent.XPathValues(".//a/@href").ToArray();

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        //private static DateTime? GetDate(string url)
        //{
        //    // url : http://www.telecharger-magazine.com/2015/07/17/
        //    Uri uri = new Uri(url);
        //    int i = uri.Segments.Length - 1;

        //    if (i < 0)
        //        goto err;
        //    string s = uri.Segments[i--];
        //    if (s.EndsWith("/"))
        //        s = s.Substring(0, s.Length - 1);
        //    int day;
        //    if (!int.TryParse(s, out day))
        //        goto err;

        //    if (i < 0)
        //        goto err;
        //    s = uri.Segments[i--];
        //    if (s.EndsWith("/"))
        //        s = s.Substring(0, s.Length - 1);
        //    int month;
        //    if (!int.TryParse(s, out month))
        //        goto err;

        //    if (i < 0)
        //        goto err;
        //    s = uri.Segments[i--];
        //    if (s.EndsWith("/"))
        //        s = s.Substring(0, s.Length - 1);
        //    int year;
        //    if (!int.TryParse(s, out year))
        //        goto err;

        //err:
        //    pb.Trace.WriteLine("unknow post creation date, url \"{0}\"", url);
        //    return null;
        //}

        private static PrintType GetPrintType(string category)
        {
            // Journaux
            // Actualité/Journaux
            // sport/Journaux

            // livres
            // livres/Cuisine
            // livres/science
            // livres/sport
            // Informatique/livres
            // Santé/livres

            // Homme/sport

            // Actualité
            // Art et Culture
            // Auto-Moto
            // Cuisine
            // Femme
            // Informatique
            // Maison et Jardin
            // sport

            category = category.ToLowerInvariant();
            if (category == "livres" || category.StartsWith("livres/") || category.EndsWith("/livres"))
                return PrintType.Book;
            else if (category != null && category != "")
                return PrintType.Print;
            else
                return PrintType.UnknowEBook;
            //switch (category)
            //{
            //    case "journaux":
            //        return PrintType.Print;
            //    default:
            //        return PrintType.UnknowEBook;
            //}
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        private static int GetPostDetailKey(HttpRequest httpRequest)
        {
            // http://www.telecharger-magazine.com/journaux/3841-le-monde-eco-et-entreprise-sport-et-forme-du-samedi-18-juillet-2015.html
            // http://www.telecharger-magazine.com/livres/502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html
            Uri uri = new Uri(httpRequest.Url);
            Match match = __postKeyRegex.Match(uri.Segments[uri.Segments.Length - 1]);
            if (match.Success)
                return int.Parse(match.Value);
            throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
        }
    }
}

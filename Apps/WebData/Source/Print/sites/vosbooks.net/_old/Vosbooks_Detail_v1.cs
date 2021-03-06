﻿using System;
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

namespace Download.Print.Vosbooks.old
{
    public class Vosbooks_PostDetail_v1 : IPost, IKeyData_v4<int>, IHttpRequestData
    {
        public Vosbooks_PostDetail_v1()
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
        //public string Language;
        //public string Size;
        //public int? NbPages;
        public NamedValues<ZValue> Infos;
        public WebImage[] Images;
        public string[] DownloadLinks;

        public string GetServer()
        {
            return "vosbooks.net";
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

    public static class Vosbooks_DetailManager_v1
    {
        private static bool __trace = false;
        private static WebDataManager_v1<int, Vosbooks_PostDetail_v1> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1, int, Vosbooks_PostDetail_v1> __webHeaderDetailManager = null;
        private static Date? __lastPostDate = null;

        static Vosbooks_DetailManager_v1()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("Vosbooks/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1, int, Vosbooks_PostDetail_v1>();
            __webHeaderDetailManager.HeaderDataPageManager = Vosbooks_HeaderManager_v1.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager_v1<int, Vosbooks_PostDetail_v1> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1, int, Vosbooks_PostDetail_v1> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager_v1<int, Vosbooks_PostDetail_v1> CreateWebDataManager(XElement xe)
        {
            WebDataManager_v1<int, Vosbooks_PostDetail_v1> detailWebDataManager = new WebDataManager_v1<int, Vosbooks_PostDetail_v1>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<Vosbooks_PostDetail_v1>();

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
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = Vosbooks_v1.GetHttpRequestParameters;
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
            detailWebDataManager.DocumentStore = MongoDocumentStore_v4<int, Vosbooks_PostDetail_v1>.Create(xe);

            return detailWebDataManager;
        }

        private static Vosbooks_PostDetail_v1 GetData(WebResult webResult)
        {
            //XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            Vosbooks_PostDetail_v1 data = new Vosbooks_PostDetail_v1();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPostDetailKey(webResult.WebRequest.HttpRequest);

            // <div id="page">
            // <div id="wrapper">
            // <table id="layout">
            // <tr>...</tr>
            // <tr>
            // <td class="sidebars">...</td>
            // <td>
            // <div id="left-col">
            // <div id="content-padding">
            // <div id="content">
            // ...
            // <div class="post" id="post-74299" style="margin-top: 0;">
            //
            // <table id="post-head">
            // <tr>
            // <td id="head-date">
            // <div class="date"><span>jan</span> 29</div>
            // </td>
            // <td>
            // <div class="title">
            // <h2><a href="http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html" rel="bookmark" title="Les imposteurs – François Cavanna" >Les imposteurs – François Cavanna </a></h2>
            // <div class="postdata">
            // <span class="category">
            // <a href="http://www.vosbooks.net/category/livre/ebooks-epub" rel="category tag">Ebooks en Epub</a>, 
            // <a href="http://www.vosbooks.net/category/livre" rel="category tag">Livre</a>
            // </span>
            // </div>
            // </div>
            // </td>
            // </tr>
            // </table>
            //
            // <div class="entry">
            // ...
            // <p style="text-align: center;"> 
            // <img class="alignnone"  src="http://imageshack.com/a/img538/3859/6JXSxu.jpg" alt="Les imposteurs – François Cavanna" title="Les imposteurs – François Cavanna" height="540" width="420" />
            // </p>

            // </tr>


            XXElement xePost = xeSource.XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']");

            XXElement xe = xePost.XPathElement(".//table[@id='post-head']");
            //string[] dates = xe.DescendantTextList(".//td[@id='head-date']", func: Vosbooks.TrimFunc1).ToArray();
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
            // Bande Dessinée/Livre
            // Ebooks en Epub/Livre
            // Autres ouvrages/Livre
            // Cuisine/Livre
            // Divers/Français/Langues/Livre/Mathématiques/Physique
            // Divers/Livre
            // Livre/Médecine & Santé
            // Journaux/L'Equipe
            // Autres journaux/Journaux
            // L'Equipe/Revues & Magazines
            // Magazines Francais/Médecine & Santé/Revues & Magazines
            // Cours informatique/Développement Web
            category = category.ToLowerInvariant();
            if (category.Contains("journaux") || category.Contains("magazines"))
                return PrintType.Print;
            else if (category.StartsWith("bande dessinée/"))
                return PrintType.Comics;
            else if (category.Contains("livre"))      // category.EndsWith("/livre")  category.StartsWith("ebooks en epub/")
                return PrintType.Book;
            else
                return PrintType.UnknowEBook;
            // return PrintType.Book;
            // return PrintType.Comics;
            // return PrintType.UnknowEBook;
            // return PrintType.Unknow;
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        private static int GetPostDetailKey(HttpRequest httpRequest)
        {
            // http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html
            // http://www.vosbooks.net/74650-livre/medecine-sante/flash-sante-n-2-2015.html
            Uri uri = new Uri(httpRequest.Url);
            if (uri.Segments.Length >= 2)
            {
                //Match match = __postKeyRegex.Match(uri.Segments[uri.Segments.Length - 2]);
                Match match = __postKeyRegex.Match(uri.Segments[1]);
                if (match.Success)
                    return int.Parse(match.Value);
            }
            throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
        }
    }
}

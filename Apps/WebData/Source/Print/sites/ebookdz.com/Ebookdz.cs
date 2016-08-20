using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Ebookdz
{
    public class Ebookdz_PostDetail : PostDetailSimpleLinks
    {
        public Ebookdz_PostDetail()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        public NamedValues<ZValue> Infos;

        // IPostToDownload
        public override string GetServer()
        {
            return Ebookdz.ServerName;
        }
    }

    public class Ebookdz : PostHeaderDetailMongoManagerBase<PostHeader, Ebookdz_PostDetail>
    {
        private static bool __trace = false;
        private static string __serverName = "ebookdz.com";
        private static string __configName = "Ebookdz";
        private static Ebookdz __current = null;
        //private static string __urlMainPage = "http://www.ebookdz.com/";
        // modif le 07/04/2016
        private static string __urlMainPage = "http://www.ebookdz.com/index.php";

        public override string Name { get { return __serverName; } }

        //public static void Init(bool test = false)
        //{
        //    XElement xe;
        //    if (!test)
        //        xe = XmlConfig.CurrentConfig.GetElement(__configName);
        //    else
        //    {
        //        pb.Trace.WriteLine("{0} init for test", __configName);
        //        xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
        //    }
        //    EbookdzLogin.Init(xe);
        //    __current = new Ebookdz();
        //    __current.HeaderPageNominalType = typeof(PostHeaderDataPage<PostHeader>);
        //    __current.Create(xe);
        //    ServerManagers.Add(__serverName, __current.CreateServerManager(__serverName));
        //}

        public static IServerManager CreateServerManager(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                pb.Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }
            EbookdzLogin.Init(xe);
            __current = new Ebookdz();
            __current.HeaderPageNominalType = typeof(PostHeaderDataPage<PostHeader>);
            __current.CreateDataManager(xe);
            return __current;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static string ServerName { get { return __serverName; } }
        public static Ebookdz Current { get { return __current; } }

        // used by header and detail
        protected override void InitLoadFromWeb()
        {
            EbookdzLogin.InitLoadFromWeb();
        }

        // used by header and detail
        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return EbookdzLogin.GetHttpRequestParameters();
        }

        // header get data
        protected override IEnumDataPages<PostHeader> GetHeaderPageData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<PostHeader> data = new PostHeaderDataPage<PostHeader>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = null;

            // <div id="vba_news4">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='vba_news4']//div[@class='collapse']");
            List<PostHeader> headers = new List<PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                PostHeader header = new PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                //XXElement xe = xeHeader.XPathElement(".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']");
                XXElement xe = xeHeader.XPathElement(".//h2[@class='blockhead']//a[2]");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");

                //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

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

                //pb.Trace.WriteLine(header.Title);

                headers.Add(header);
            }
            data.Headers = headers.ToArray();
            return data;
        }

        // header get key
        protected override BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.ebookdz.com/
            // page 2 : no pagination
            if (httpRequest.Url == __urlMainPage)
                return 1;
            //Uri uri = new Uri(url);
            //string lastSegment = uri.Segments[uri.Segments.Length - 1];
            //lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            //int page;
            //if (!int.TryParse(lastSegment, out page))
            //    throw new PBException("header page key not found in url \"{0}\"", url);
            //return page;
            throw new PBException("header page key not found in url \"{0}\"", httpRequest.Url);
        }

        // header get url page
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = __urlMainPage };
        }

        // used by detail cache
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            // httpRequest => (_GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
            return (_GetDetailKey(httpRequest) / 1000 * 1000).ToString(); ;
        }

        // detail get data
        protected override Ebookdz_PostDetail GetDetailData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            Ebookdz_PostDetail data = new Ebookdz_PostDetail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetDetailKey(webResult.WebRequest.HttpRequest);

            // <div class="body_bd">
            XXElement xePost = xeSource.XPathElement("//div[@class='body_bd']");

            // Le Monde + Magazine + 2 suppléments du samedi 03 janvier 2015
            //data.Title = xePost.XPathValue(".//div[@id='pagetitle']//a//text()").Trim(DownloadPrint.TrimChars);
            data.Title = xePost.XPathValue(".//div[@id='pagetitle']//a//text()").zNotNullFunc(s => s.Trim(DownloadPrint.TrimChars));
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            // Forum / Journaux / Presse quotidienne / Le Monde / Journal Le Monde + Magazine + 2 suppléments du samedi 03 janvier 2015
            string lowerTitle = null;
            if (data.Title != null)
                lowerTitle = data.Title.ToLowerInvariant();
            //data.Category = xePost.DescendantTextList(".//div[@id='breadcrumb']//a").Where(text => { text = text.ToLowerInvariant(); return text != "forum" && !text.EndsWith(lowerTitle); }).Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            data.Category = xePost.XPathElements(".//div[@id='breadcrumb']//a").DescendantTexts().Where(text => { text = text.ToLowerInvariant(); return text != "forum" && !text.EndsWith(lowerTitle); }).Select(DownloadPrint.Trim).zToStringValues("/");
            string category = data.Category.ToLowerInvariant();
            data.PrintType = GetPrintType(category);
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            // <div id="postlist" class="postlist restrain">
            XXElement xe = xePost.XPathElement(".//div[@id='postlist']");

            // Aujourd'hui, 07h32 - Aujourd'hui, 10h51 - Hier, 12h55 - 22/02/2014, 21h09
            //string date = xe.DescendantTextList(".//div[@class='posthead']//text()", nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            XXElement xe2 = xe.XPathElement(".//div[@class='posthead']");
            //string date = xe2.DescendantTextList(nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            string date = xe2.DescendantTexts(node => node.zGetName() != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode).zToStringValues("");
            date = date.Replace('\xA0', ' ');
            data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, webResult.LoadFromWebDate, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm");
            if (data.PostCreationDate == null)
                pb.Trace.WriteLine("unknow post creation date \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("post creation date {0} - \"{1}\"", data.PostCreationDate, date);

            //data.PostAuthor = xe.XPathValue(".//div[@class='userinfo']//a//text()").Trim(DownloadPrint.TrimChars);
            data.PostAuthor = xe.XPathValue(".//div[@class='userinfo']//a//text()").zNotNullFunc(s => s.Trim(DownloadPrint.TrimChars));

            // <div class="postbody">
            xe = xePost.XPathElement(".//div[@class='postbody']//div[@class='content']//blockquote/div");

            //data.Images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();
            data.Images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImageFromWeb)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
            // xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a")
            PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), data.Title);
            data.Description = textValues.description;
            //data.Language = textValues.language;
            //data.Size = textValues.size;
            //data.NbPages = textValues.nbPages;
            data.Infos.SetValues(textValues.infos);

            // modif pour avoir les liens de http://www.ebookdz.com/forum/showthread.php?t=113291
            //data.DownloadLinks = xe.XPathValues(".//a/@href");
            data.DownloadLinks = xePost.XPathElement(".//div[@class='postbody']//div[@class='content']//blockquote").XPathValues(".//a/@href").ToArray();

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static PrintType GetPrintType(string category)
        {
            // Journaux/Presse quotidienne/Le Monde
            // Les Livres/Littérature & Romans/Nouveautés en Librairies/Nouveauté 2014
            // Les Livres/BD, Comics et Mangas/
            category = category.ToLower();
            if (category.StartsWith("journaux/") || category.StartsWith("magazines/"))
                return PrintType.Print;
            else if (category.StartsWith("les livres/bd, comics et mangas/"))
                return PrintType.Comics;
            else if (category.StartsWith("les livres/"))
                return PrintType.Book;
            else
                return PrintType.UnknowEBook;
            // return PrintType.Book;
            // return PrintType.Comics;
            // return PrintType.UnknowEBook;
            // return PrintType.Unknow;
        }

        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static Regex __postKeyRegex = new Regex(@"\?t=([0-9]+)$", RegexOptions.Compiled);
        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            // http://www.ebookdz.com/forum/showthread.php?t=109595
            //Uri uri = new Uri(url);
            //string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(httpRequest.Url);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
            return int.Parse(match.Groups[1].Value);
        }

        // à revoir
        [Obsolete]
        protected override void LoadDetailImages(Ebookdz_PostDetail data)
        {
            data.LoadImages();
        }

        public override void LoadNewDocuments()
        {
            _headerDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 7, startPage: 1, maxPage: 1);
        }

        public override IEnumerable<IPostToDownload> FindFromDateTime(DateTime date)
        {
            string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", date.ToUniversalTime().ToString("o"));
            string sort = "{ 'download.PostCreationDate': -1 }";
            // useCursorCache: true
            return _detailDataManager.Find(query, sort: sort, loadImage: false);
        }

        public override IEnumerable<IPostToDownload> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            if (sort == null)
                sort = "{ 'download.PostCreationDate': -1 }";
            return _detailDataManager.Find(query, sort: sort, limit: limit, loadImage: loadImage);
        }

        public override IPostToDownload Load(BsonValue id)
        {
            return _detailDataManager.DocumentStore.LoadFromId(id);
        }
    }
}

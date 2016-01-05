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
using Print;

namespace Download.Print.ExtremeDown
{
    // PostDetail => IPostToDownload => IHttpRequestData, IKeyData
    public class ExtremeDown_PostDetail : PostDetailMultiLinks
    {
        public ExtremeDown_PostDetail()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        public string Language;
        public string Size;
        public int? NbPages;
        public NamedValues<ZValue> Infos;

        // IPostToDownload
        public override string GetServer()
        {
            return ExtremeDown.ServerName;
        }
    }

    public class ExtremeDown : PostHeaderDetailMongoManagerBase<PostHeader, ExtremeDown_PostDetail>
    {
        private static bool __trace = false;
        private static string __serverName = "extreme-down.net";
        private static string __configName = "ExtremeDown";
        private static ExtremeDown __current = null;
        private static string __urlMainPage = "http://www.ex-down.net/ebooks/";
        private bool _getLinksExtremeProtect = false;
        private ExtremeProtect _extremeProtect = null;

        static ExtremeDown()
        {
            Init(test: DownloadPrint.Test);
        }

        public static void FakeInit()
        {
        }

        public static void Init(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                pb.Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }
            __current = new ExtremeDown();
            __current.HeaderPageNominalType = typeof(PostHeaderDataPage<PostHeader>);
            __current.Create(xe);
            __current._getLinksExtremeProtect = true;
            __current._extremeProtect = new ExtremeProtect();
            ServerManagers.Add(__serverName, __current.CreateServerManager(__serverName));
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static string ServerName { get { return __serverName; } }
        public static ExtremeDown Current { get { return __current; } }

        // header get data
        protected override IEnumDataPages<PostHeader> GetHeaderPageData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<PostHeader> data = new PostHeaderDataPage<PostHeader>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation ignore-select']//a[starts-with(text(), 'Suivant')]/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='dle-content']//div[@class='blockbox']");
            List<PostHeader> headers = new List<PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                PostHeader header = new PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                header.UrlDetail = xeHeader.XPathValue(".//h2[@class='blocktitle']//a/@href");

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
            // page 1 : http://www.extreme-down.net/ebooks/
            // page 2 : http://www.extreme-down.net/ebooks/page/2/
            // new
            // page 1 : http://www.ex-down.net/ebooks/
            // page 2 : http://www.ex-down.net/ebooks/page/2/
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = uri.Segments[uri.Segments.Length - 1];
            if (lastSegment.EndsWith("/"))
                lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            int page;
            if (!int.TryParse(lastSegment, out page))
                throw new PBException("header page key not found in url \"{0}\"", url);
            return page;
        }

        // header get url page
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : http://www.extreme-down.net/ebooks/
            // page 2 : http://www.extreme-down.net/ebooks/page/2/
            // new
            // page 1 : http://www.ex-down.net/ebooks/
            // page 2 : http://www.ex-down.net/ebooks/page/2/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url += string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }

        // used by detail cache
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return (_GetDetailKey(httpRequest) / 1000 * 1000).ToString();
        }

        // detail get data
        protected override ExtremeDown_PostDetail GetDetailData(WebResult webResult)
        {


            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            ExtremeDown_PostDetail data = new ExtremeDown_PostDetail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetDetailKey(webResult.WebRequest.HttpRequest);

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");

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

            data.Images = xeDiv.XPathElement(".//table//td[@class='image-block']").DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            description.AddRange(xeDiv.XPathValues(".//table//td//blockquote//text()"));

            xeDiv = xePost.XPathElement(".//div[@class='upload-infos clearfix']");
            description.AddRange(xeDiv.XPathValues(".//table//text()"));

            data.Description = description.ToArray();

            string title = null;
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
                data.DownloadLinks.AddItem(title);
                title = null;
                //foreach (XXElement xe2 in xe.XPathElements("following-sibling::div[1]//a"))
                foreach (XXElement xe2 in xe.XPathElements(".//a"))
                {
                    //s = xe2.DescendantTextList().FirstOrDefault();
                    // <strong class="hebergeur">
                    string server = xe2.XPathValue(".//strong[@class='hebergeur']//text()");
                    string link = xe2.XPathValue("@href");
                    if (_getLinksExtremeProtect && _extremeProtect.IsLinkProtected(link))
                    {
                        data.DownloadLinks.AddServer(server, link);
                        data.DownloadLinks.AddLinks(_extremeProtect.UnprotectLink(link));
                    }
                    else
                    {
                        data.DownloadLinks.AddServer(server);
                        data.DownloadLinks.AddLink(link);
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

        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            // http://www.extreme-down.net/ebooks/journaux/23375-les-journaux-mardi-09-dcembre-2014.html
            // new
            // http://www.ex-down.net/ebooks/magazines/29244-science-vie-hors-serie-no273-decembre-2015.html
            Uri uri = new Uri(httpRequest.Url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
            return int.Parse(match.Value);
        }

        protected override void LoadDetailImages(ExtremeDown_PostDetail data)
        {
            data.LoadImages();
        }

        protected override void LoadNewDocuments()
        {
            _webHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 10);
        }

        protected override IEnumerable<IPostToDownload> Find(DateTime date)
        {
            string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", date.ToUniversalTime().ToString("o"));
            string sort = "{ 'download.PostCreationDate': -1 }";
            // useCursorCache: true
            return _detailWebDataManager.Find(query, sort: sort, loadImage: false);
        }

        protected override IPostToDownload LoadDocument(BsonValue id)
        {
            return _detailWebDataManager.DocumentStore.LoadFromId(id);
        }
    }
}

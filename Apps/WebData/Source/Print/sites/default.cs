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

namespace Download.Print.Test
{
    // PostDetail => IPostToDownload => IHttpRequestData, IKeyData
    public class Test_PostDetail : PostDetailSimpleLinks
    {
        public Test_PostDetail()
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
            return Test.ServerName;
        }
    }

    public class Test : PostHeaderDetailMongoManagerBase<PostHeader, Test_PostDetail>
    {
        private static bool __trace = false;
        private static string __serverName = "vosbooks.net";
        private static string __configName = "Vosbooks";
        private static Test __current = null;
        private static string __urlMainPage = "http://www.vosbooks.net/";
        private Date? _lastPostDate = null;

        static Test()
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
            __current = new Test();
            __current.HeaderPageNominalType = typeof(PostHeaderDataPage<PostHeader>);
            __current.Create(xe);
            ServerManagers.Add(__serverName, __current.CreateServerManager(__serverName));
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static string ServerName { get { return __serverName; } }
        public static Test Current { get { return __current; } }

        // header get data
        protected override IEnumDataPages<PostHeader> GetHeaderPageData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<PostHeader> data = new PostHeaderDataPage<PostHeader>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='page-nav']//li[last()]//a[text()='>']/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@id='layout']//div[@id='content']/div");
            List<PostHeader> headers = new List<PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                PostHeader header = new PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                XXElement xe = xeHeader.XPathElement(".//div/div/div//a");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");

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
            // page 1 : http://www.vosbooks.net/
            // page 2 : http://www.vosbooks.net/page/2
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
            // page 1 : http://www.vosbooks.net/
            // page 2 : http://www.vosbooks.net/page/2
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
        protected override Test_PostDetail GetDetailData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            Test_PostDetail data = new Test_PostDetail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetDetailKey(webResult.WebRequest.HttpRequest);

            XXElement xePost = xeSource.XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']");

            XXElement xe = xePost.XPathElement(".//table[@id='post-head']");
            //string[] dates = xe.DescendantTextList(".//td[@id='head-date']", func: Vosbooks.TrimFunc1).ToArray();
            string[] dates = xe.XPathElement(".//td[@id='head-date']").DescendantTexts().Select(DownloadPrint.Trim).ToArray();
            data.PostCreationDate = GetDate(dates, _lastPostDate);
            if (data.PostCreationDate != null)
                _lastPostDate = new Date(data.PostCreationDate.Value);
            if (__trace)
                pb.Trace.WriteLine("post creation date {0} - {1}", data.PostCreationDate, dates.zToStringValues());

            data.Title = xePost.XPathValue(".//div[@class='title']//a//text()").zFunc(DownloadPrint.ReplaceChars).zFunc(DownloadPrint.Trim);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            data.Category = xePost.XPathElements(".//div[@class='postdata']//span[@class='category']//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            data.PrintType = GetPrintType(data.Category);

            xe = xePost.XPathElement(".//div[@class='entry']");
            data.Images = new WebImage[] { new WebImage(zurl.GetUrl(data.SourceUrl, xe.XPathValue("div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p/img/@src"))) };

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
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
            data.Infos.SetValues(textValues.infos);

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

        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            // http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html
            // http://www.vosbooks.net/74650-livre/medecine-sante/flash-sante-n-2-2015.html
            Uri uri = new Uri(httpRequest.Url);
            if (uri.Segments.Length >= 2)
            {
                Match match = __postKeyRegex.Match(uri.Segments[1]);
                if (match.Success)
                    return int.Parse(match.Value);
            }
            throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
        }

        protected override void LoadDetailImages(Test_PostDetail data)
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

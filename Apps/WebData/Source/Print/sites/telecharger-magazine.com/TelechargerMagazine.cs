using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using System.Text.RegularExpressions;

namespace Download.Print.TelechargerMagazine
{
    public class TelechargerMagazine_PostDetail : PostDetailSimpleLinks
    {
        public TelechargerMagazine_PostDetail()
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
            return TelechargerMagazine.ServerName;
        }
    }

    public class TelechargerMagazine : PostHeaderDetailMongoManagerBase<PostHeader, TelechargerMagazine_PostDetail>
    {
        private static bool __trace = false;
        // http://telecharger-magazine.org/
        private static string __serverName = "telecharger-magazine.com";
        private static string __configName = "TelechargerMagazine";
        private static TelechargerMagazine __current = null;
        private static string __urlMainPage = "http://www.telecharger-magazine.com/index.php";
        private static string __urlPage = "http://www.telecharger-magazine.com/";

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
        //    __current = new TelechargerMagazine();
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
            __current = new TelechargerMagazine();
            __current.HeaderPageNominalType = typeof(PostHeaderDataPage<PostHeader>);
            __current.CreateDataManager(xe);
            return __current;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static string ServerName { get { return __serverName; } }
        public static TelechargerMagazine Current { get { return __current; } }

        // header get data
        protected override IEnumDataPages<PostHeader> GetHeaderPageData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<PostHeader> data = new PostHeaderDataPage<PostHeader>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            // <div id='dle-content'>
            // <div class="leftpane"> 
            // <div class="movieposter" title="Watch Movie Pachamama : Cuisine des premières nations">
            // <a href="http://www.telecharger-magazine.com/livres/3833-pachamama-cuisine-des-premires-nations.html">
            // <a href="http://www.telecharger-magazine.com/livres/3833-pachamama-cuisine-des-premires-nations.html" title="Pachamama : Cuisine des premières nations">
            // <img src="http://pxhst.co/avaxhome/cd/2a/00152acd.jpeg" width="110" height="150" alt="télécharger Pachamama : Cuisine des premières nations" title="télécharger Pachamama : Cuisine des premières nations" />
            // </a>
            // </div>
            // </div>
            // ...
            // <div class="navigation" align="center">
            // <div class="clear"></div>
            // <span>&#8592; Previous</span> <span>1</span>
            // <a href="http://www.telecharger-magazine.com/page/2/">2</a>
            // ...
            // <a href="http://www.telecharger-magazine.com/page/2/">Next &#8594;</a>
            // <div class="clear"></div>
            // </div>

            XXElement xe = xeSource.XPathElement("//div[@id='dle-content']");

            data.UrlNextPage = zurl.GetUrl(url, xe.XPathValue(".//a[starts-with(text(), 'Next')]/@href"));

            IEnumerable<XXElement> xeHeaders = xe.XPathElements(".//div[@class='leftpane']");
            List<PostHeader> headers = new List<PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                PostHeader header = new PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                if (xeHeader.XPathValue("@class") == "page-nav")
                    continue;

                XXElement xe2 = xeHeader.XPathElement(".//a/a");
                header.Title = xe2.AttribValue("title");
                header.UrlDetail = xe2.AttribValue("href");

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
            // page 1 : http://www.telecharger-magazine.com/index.php
            // page 2 : http://www.telecharger-magazine.com/page/2/
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
            // page 1 : http://www.telecharger-magazine.com/index.php
            // page 2 : http://www.telecharger-magazine.com/page/2/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url = __urlPage + string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }

        // used by detail cache
        protected override string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return (_GetDetailKey(httpRequest) / 1000 * 1000).ToString();
        }

        // detail get data
        protected override TelechargerMagazine_PostDetail GetDetailData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            TelechargerMagazine_PostDetail data = new TelechargerMagazine_PostDetail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetDetailKey(webResult.WebRequest.HttpRequest);

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
            if (webResult.WebRequest.LoadImageFromWeb)
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

        protected override BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            return _GetDetailKey(httpRequest);
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        private static int _GetDetailKey(HttpRequest httpRequest)
        {
            // http://www.telecharger-magazine.com/journaux/3841-le-monde-eco-et-entreprise-sport-et-forme-du-samedi-18-juillet-2015.html
            // http://www.telecharger-magazine.com/livres/502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html
            Uri uri = new Uri(httpRequest.Url);
            Match match = __postKeyRegex.Match(uri.Segments[uri.Segments.Length - 1]);
            if (match.Success)
                return int.Parse(match.Value);
            throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
        }

        // à revoir
        [Obsolete]
        protected override void LoadDetailImages(TelechargerMagazine_PostDetail data)
        {
            data.LoadImages();
        }

        public override void LoadNewDocuments()
        {
            _headerDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 25, startPage: 1, maxPage: 10);
        }

        public override IEnumerable<IPostToDownload> FindFromDateTime(DateTime date)
        {
            string query = string.Format("{{ 'download.LoadFromWebDate': {{ $gt: ISODate('{0}') }} }}", date.ToUniversalTime().ToString("o"));
            string sort = "{ _id: -1 }";
            // useCursorCache: true
            return _detailDataManager.Find(query, sort: sort, loadImage: false);
        }

        public override IEnumerable<IPostToDownload> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            if (sort == null)
                sort = "{ _id: -1 }";
            return _detailDataManager.Find(query, sort: sort, limit: limit, loadImage: loadImage);
        }

        public override IPostToDownload Load(BsonValue id)
        {
            return _detailDataManager.DocumentStore.LoadFromId(id);
        }
    }
}

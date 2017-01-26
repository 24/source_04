using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
using pb.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Download.Print.TelechargerMagazine
{
    public class TelechargerMagazine_PostDetail_v3 : PostDetailSimpleLinks_v2
    {
        public TelechargerMagazine_PostDetail_v3()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

        public string OriginalTitle;
        public string PostAuthor;
        //public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        public NamedValues<ZValue> Infos;

        // IPostToDownload
        public override string GetServer()
        {
            return TelechargerMagazine_v3.Current.Name;
        }
    }

    public class TelechargerMagazine_v3 : PostHeaderDetailMongoManagerBase_v3<PostHeader, TelechargerMagazine_PostDetail_v3>
    {
        private static string _serverName = "telecharger-magazine.com";
        private static string _configName = "TelechargerMagazine";
        private static TelechargerMagazine_v3 _current = null;
        //private static string _urlMainPage = "http://www.telecharger-magazine.com/index.php";
        private static string _urlMainPage = "http://telechargermagazines.com/";
        //private static string _urlPage = "http://www.telecharger-magazine.com/";
        //private XElement _configElement = null;

        public static TelechargerMagazine_v3 Current { get { return _current; } }
        public override string Name { get { return _serverName; } }
        //public XElement ConfigElement { get { return _configElement; } }

        public static TelechargerMagazine_v3 Create(bool test)
        {
            if (test)
                Trace.WriteLine("{0} init for test", _configName);
            XElement xe = GetConfigElement(test);

            _current = new TelechargerMagazine_v3();
            //_current._configElement = xe;
            _current.HeaderPageNominalType = typeof(PostHeaderHeaderDataPages_v2);
            _current.Create(xe);
            return _current;
        }

        public static XElement GetConfigElement(bool test = false)
        {
            if (!test)
                return XmlConfig.CurrentConfig.GetElement(_configName);
            else
                return XmlConfig.CurrentConfig.GetElement(_configName + "_Test");
        }

        // header get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override IEnumDataPages<PostHeader> GetHeaderPageData(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();
            string url = httpResult.Http.HttpRequest.Url;
            PostHeaderHeaderDataPages_v2 data = new PostHeaderHeaderDataPages_v2();
            data.SourceUrl = url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = GetPageKey(httpResult.Http.HttpRequest);

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
                header.LoadFromWebDate = httpResult.Http.RequestTime;

                if (xeHeader.XPathValue("@class") == "page-nav")
                    continue;

                XXElement xe2 = xeHeader.XPathElement(".//a/a");
                header.Title = xe2.AttribValue("title");
                header.UrlDetail = xe2.AttribValue("href");

                headers.Add(header);
            }
            data.Data = headers.ToArray();

            return data;
        }

        // header get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.telecharger-magazine.com/index.php
            // page 2 : http://www.telecharger-magazine.com/page/2/
            string url = httpRequest.Url;
            if (url == _urlMainPage)
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

        // header get url page, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override HttpRequest GetHttpRequestPage(int page)
        {
            ////////////////// page 1 : http://www.telecharger-magazine.com/index.php
            ////////////////// page 2 : http://www.telecharger-magazine.com/page/2/
            // page 1 : http://telechargermagazines.com/
            // page 2 : http://www.telechargermagazines.com/page/2/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = _urlMainPage;
            if (page > 1)
                //url = _urlPage + string.Format("page/{0}/", page);
                url = _urlMainPage + string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }

        // detail image cache get sub-directory, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override string GetDetailImageCacheUrlSubDirectory(WebData<TelechargerMagazine_PostDetail_v3> data)
        {
            string subPath = null;
            subPath = data.Result_v2.Http.HttpRequest.UrlCachePath.SubPath;
            return zpath.PathSetExtension(subPath, null);
        }

        // detail get data, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
        protected override TelechargerMagazine_PostDetail_v3 GetDetailData(HttpResult<string> httpResult)
        {
            XXElement xeSource = httpResult.zGetXDocument().zXXElement();

            TelechargerMagazine_PostDetail_v3 data = new TelechargerMagazine_PostDetail_v3();
            data.SourceUrl = httpResult.Http.HttpRequest.Url;
            data.LoadFromWebDate = httpResult.Http.RequestTime;
            data.Id = _GetDetailKey(httpResult.Http.HttpRequest);

            _GetDetailData(xeSource, data);

            return data;
        }

        protected void _GetDetailData(XXElement xeSource, TelechargerMagazine_PostDetail_v3 data)
        {
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
            //if (webResult.WebRequest.LoadImageFromWeb)
            //    data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

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

        // detail get key, from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>
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

        // IServerManager method, from PostHeaderDetailMongoManagerBase_v3<THeaderData, TDetailData>
        //public override void LoadNewDocuments()
        //{
        //    _headerDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 25, startPage: 1, maxPage: 10);
        //}

        // IServerManager method, from PostHeaderDetailMongoManagerBase_v3<THeaderData, TDetailData>
        //public override IEnumerable<IPostToDownload> FindFromDateTime(DateTime date)
        //{
        //    string query = string.Format("{{ 'data.LoadFromWebDate': {{ $gt: ISODate('{0}') }} }}", date.ToUniversalTime().ToString("o"));
        //    string sort = "{ _id: -1 }";
        //    // useCursorCache: true
        //    return _detailDataManager.Find(query, sort: sort, loadImage: false);
        //}

        // IServerManager method, from PostHeaderDetailMongoManagerBase_v3<THeaderData, TDetailData>
        //public override IEnumerable<IPostToDownload> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        //{
        //    if (sort == null)
        //        sort = "{ _id: -1 }";
        //    return _detailDataManager.Find(query, sort: sort, limit: limit, loadImage: loadImage);
        //}

        // IServerManager method, from PostHeaderDetailMongoManagerBase_v3<THeaderData, TDetailData>
        //public override IPostToDownload Load(BsonValue id)
        //{
        //    return _detailDataManager.LoadFromId(id);
        //}
    }
}

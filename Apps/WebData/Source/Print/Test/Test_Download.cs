using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using System.Collections.Generic;

namespace Download.Print.Test
{
    public static class Test_Download
    {
        public static void Test_01()
        {
            Ebookdz.Ebookdz.Current.WebHeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImage: false, refreshDocumentStore: false).zView_v3();
        }

        public static void Test_08()
        {
            Ebookdz.Ebookdz.Current.WebHeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImage: false, refreshDocumentStore: false).zView();
        }

        public static void Test_02()
        {
            Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: false, loadImage: false, refreshDocumentStore: false).zView_v3();
        }

        public static void Test_03()
        {
            //foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, false))
            //{
            //    if (!(header is IHeaderData))
            //        throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
            //    yield return _detailDataManager.Load(
            //        new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
            //}
            bool reloadDetail = false;
            bool loadImage = false;
            bool refreshDocumentStore = false;
            foreach (var header in Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: false, loadImage: false, refreshDocumentStore: false))
            {
                var details = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.DetailDataManager.Load(
                    new pb.Web.Data.WebRequest { HttpRequest = ((pb.Web.Data.IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore });
                Trace.WriteLine(details.Document.Title);
            }
        }

        public static void Test_04()
        {
            foreach (var header in Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: false, loadImage: false, refreshDocumentStore: false))
            {
                Trace.WriteLine(header.Title);
            }
        }

        public static void Test_05()
        {
            int startPage = 1;
            bool reload = false;
            bool loadImage = false;
            bool refreshDocumentStore = false;
            pb.Web.HttpRequest httpRequest = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.GetHttpRequestPageFunction(startPage);
            var dataPage = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.Load(new pb.Web.Data.WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore });
            foreach (var header in dataPage.Document.GetDataList())
                Trace.WriteLine(header.Title);
        }

        public static void Test_06()
        {
            int startPage = 1;
            pb.Web.HttpRequest httpRequest = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.GetHttpRequestPageFunction(startPage);
            pb.Web.Data.WebRequest webRequest = new pb.Web.Data.WebRequest { HttpRequest = httpRequest };
            var dataPage = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.WebLoadDataManager.LoadData(webRequest);
            foreach (var header in ((PostHeaderDataPage<PostHeader>)dataPage).Headers)
                Trace.WriteLine(header.Title);
        }

        public static void Test_07()
        {
            int startPage = 1;
            pb.Web.HttpRequest httpRequest = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.GetHttpRequestPageFunction(startPage);
            pb.Web.Data.WebRequest webRequest = new pb.Web.Data.WebRequest { HttpRequest = httpRequest };
            //var dataPage = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.WebLoadDataManager.LoadData(webRequest);
            var webResult = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.WebLoadDataManager.Load(webRequest);
            var dataPage = Ebookdz.Ebookdz.Current.WebHeaderDetailManager.HeaderDataPageManager.WebLoadDataManager.GetData(webResult);
            foreach (var header in ((PostHeaderDataPage<PostHeader>)dataPage).Headers)
                Trace.WriteLine(header.Title);
        }

        public static IEnumDataPages<PostHeader> GetHeaderPageData(WebResult webResult)
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

        private static string __urlMainPage = "http://www.ebookdz.com/index.php";
        public static int GetPageKey(HttpRequest httpRequest)
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
    }
}

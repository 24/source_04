using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace hts.WebData
{
    public class Handeco_Header : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        //public string Title;
        public string UrlDetail;

        //public WebImage[] Images;

        public string Name = null;
        //public string Siret = null;
        public string Type = null;
        public string[] Groupes = null;
        public string[] Activités = null;
        public string PostalCode = null;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class Handeco_HeaderPage : IEnumDataPages<Handeco_Header>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Handeco_Header[] Headers;
        public string UrlNextPage;

        public IEnumerable<Handeco_Header> GetDataList()
        {
            return Headers;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage, Referer = "https://www.handeco.org/fournisseurs/rechercher" };
            else
                return null;
        }
    }

    public static class Handeco_HeaderManager
    {
        private static string __urlMainPage = "https://www.handeco.org/fournisseurs/rechercher";
        private static string __content = "raisonSociale=&SIRET=&departements%5B%5D=67&departements%5B%5D=68&departements%5B%5D=24&departements%5B%5D=33&departements%5B%5D=40&departements%5B%5D=47&departements%5B%5D=64&departements%5B%5D=03&departements%5B%5D=15&departements%5B%5D=43&departements%5B%5D=63&departements%5B%5D=14&departements%5B%5D=50&departements%5B%5D=61&departements%5B%5D=21&departements%5B%5D=58&departements%5B%5D=71&departements%5B%5D=89&departements%5B%5D=22&departements%5B%5D=29&departements%5B%5D=35&departements%5B%5D=56&departements%5B%5D=18&departements%5B%5D=28&departements%5B%5D=36&departements%5B%5D=37&departements%5B%5D=41&departements%5B%5D=45&departements%5B%5D=08&departements%5B%5D=10&departements%5B%5D=51&departements%5B%5D=52&departements%5B%5D=2A&departements%5B%5D=2B&departements%5B%5D=25&departements%5B%5D=39&departements%5B%5D=70&departements%5B%5D=90&departements%5B%5D=27&departements%5B%5D=76&departements%5B%5D=75&departements%5B%5D=77&departements%5B%5D=78&departements%5B%5D=91&departements%5B%5D=92&departements%5B%5D=93&departements%5B%5D=94&departements%5B%5D=95&departements%5B%5D=11&departements%5B%5D=30&departements%5B%5D=34&departements%5B%5D=48&departements%5B%5D=66&departements%5B%5D=19&departements%5B%5D=23&departements%5B%5D=87&departements%5B%5D=54&departements%5B%5D=55&departements%5B%5D=57&departements%5B%5D=88&departements%5B%5D=09&departements%5B%5D=12&departements%5B%5D=31&departements%5B%5D=32&departements%5B%5D=46&departements%5B%5D=65&departements%5B%5D=81&departements%5B%5D=82&departements%5B%5D=59&departements%5B%5D=62&departements%5B%5D=44&departements%5B%5D=49&departements%5B%5D=53&departements%5B%5D=72&departements%5B%5D=85&departements%5B%5D=02&departements%5B%5D=60&departements%5B%5D=80&departements%5B%5D=16&departements%5B%5D=17&departements%5B%5D=79&departements%5B%5D=86&departements%5B%5D=04&departements%5B%5D=05&departements%5B%5D=06&departements%5B%5D=13&departements%5B%5D=83&departements%5B%5D=84&departements%5B%5D=01&departements%5B%5D=07&departements%5B%5D=26&departements%5B%5D=38&departements%5B%5D=42&departements%5B%5D=69&departements%5B%5D=73&departements%5B%5D=74&departements%5B%5D=971&departements%5B%5D=973&departements%5B%5D=972&departements%5B%5D=974&departements%5B%5D=988&departements%5B%5D=987&departements%5B%5D=975&departements%5B%5D=976&departements%5B%5D=986&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher";
        //                                 raisonSociale=&SIRET=&departements%5B%5D=67&departements%5B%5D=68&departements%5B%5D=24&departements%5B%5D=33&departements%5B%5D=40&departements%5B%5D=47&departements%5B%5D=64&departements%5B%5D=03&departements%5B%5D=15&departements%5B%5D=43&departements%5B%5D=63&departements%5B%5D=14&departements%5B%5D=50&departements%5B%5D=61&departements%5B%5D=21&departements%5B%5D=58&departements%5B%5D=71&departements%5B%5D=89&departements%5B%5D=22&departements%5B%5D=29&departements%5B%5D=35&departements%5B%5D=56&departements%5B%5D=18&departements%5B%5D=28&departements%5B%5D=36&departements%5B%5D=37&departements%5B%5D=41&departements%5B%5D=45&departements%5B%5D=08&departements%5B%5D=10&departements%5B%5D=51&departements%5B%5D=52&departements%5B%5D=2A&departements%5B%5D=2B&departements%5B%5D=25&departements%5B%5D=39&departements%5B%5D=70&departements%5B%5D=90&departements%5B%5D=27&departements%5B%5D=76&departements%5B%5D=75&departements%5B%5D=77&departements%5B%5D=78&departements%5B%5D=91&departements%5B%5D=92&departements%5B%5D=93&departements%5B%5D=94&departements%5B%5D=95&departements%5B%5D=11&departements%5B%5D=30&departements%5B%5D=34&departements%5B%5D=48&departements%5B%5D=66&departements%5B%5D=19&departements%5B%5D=23&departements%5B%5D=87&departements%5B%5D=54&departements%5B%5D=55&departements%5B%5D=57&departements%5B%5D=88&departements%5B%5D=09&departements%5B%5D=12&departements%5B%5D=31&departements%5B%5D=32&departements%5B%5D=46&departements%5B%5D=65&departements%5B%5D=81&departements%5B%5D=82&departements%5B%5D=59&departements%5B%5D=62&departements%5B%5D=44&departements%5B%5D=49&departements%5B%5D=53&departements%5B%5D=72&departements%5B%5D=85&departements%5B%5D=02&departements%5B%5D=60&departements%5B%5D=80&departements%5B%5D=16&departements%5B%5D=17&departements%5B%5D=79&departements%5B%5D=86&departements%5B%5D=04&departements%5B%5D=05&departements%5B%5D=06&departements%5B%5D=13&departements%5B%5D=83&departements%5B%5D=84&departements%5B%5D=01&departements%5B%5D=07&departements%5B%5D=26&departements%5B%5D=38&departements%5B%5D=42&departements%5B%5D=69&departements%5B%5D=73&departements%5B%5D=74&departements%5B%5D=971&departements%5B%5D=973&departements%5B%5D=972&departements%5B%5D=974&departements%5B%5D=988&departements%5B%5D=987&departements%5B%5D=975&departements%5B%5D=976&departements%5B%5D=986&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher
        private static WebDataPageManager<int, Handeco_HeaderPage, Handeco_Header> __headerWebDataPageManager = null;

        static Handeco_HeaderManager()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Handeco/Header"));
        }

        public static WebDataPageManager<int, Handeco_HeaderPage, Handeco_Header> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<int, Handeco_HeaderPage, Handeco_Header> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager<int, Handeco_HeaderPage, Handeco_Header> headerWebDataPageManager = new WebDataPageManager<int, Handeco_HeaderPage, Handeco_Header>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<Handeco_HeaderPage>();

            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = Handeco.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;

            headerWebDataPageManager.DocumentStore = MongoDocumentStore<int, Handeco_HeaderPage>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static Handeco_HeaderPage GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Handeco_HeaderPage data = new Handeco_HeaderPage();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='page-nav']//li[last()]//a[text()='>']/@href"));

            //IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@id='layout']//div[@id='content']/div");
            //List<Handeco_PostHeader> headers = new List<Handeco_PostHeader>();
            //foreach (XXElement xeHeader in xeHeaders)
            //{
            //    Handeco_PostHeader header = new Handeco_PostHeader();
            //    header.SourceUrl = url;
            //    header.LoadFromWebDate = webResult.LoadFromWebDate;

            //    if (xeHeader.XPathValue("@class") == "page-nav")
            //        continue;

            //    XXElement xe = xeHeader.XPathElement(".//div/div/div//a");
            //    //header.Title = xe.XPathValue(".//text()");
            //    header.UrlDetail = xe.XPathValue("./@href");

            //    headers.Add(header);
            //}
            //data.PostHeaders = headers.ToArray();
            //return data;


            // <div class="paginationControl">
            // page n    : <a href="/fournisseurs/rechercher/page/2#resultats">&gt;</a> |
            // last page : <span class="disabled">&gt;</span> |
            data.UrlNextPage = zurl.RemoveFragment(zurl.GetUrl(url, xeSource.XPathValue("//div[@class='paginationControl']//*[position()=last()-1]/@href")));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table//tr[position() > 1]");
            List<Handeco_Header> headers = new List<Handeco_Header>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Handeco_Header header = new Handeco_Header();
                header.SourceUrl = url;
                header.LoadFromWebDate = DateTime.Now;
                header.Name = Handeco.Trim(xeHeader.XPathValue(".//td[1]//text()"));
                header.UrlDetail = zurl.RemoveFragment(zurl.GetUrl(url, xeHeader.XPathValue(".//td[1]//a/@href")));
                //header.Siret = Handeco.Trim(xeHeader.XPathValue(".//td[2]//text()"));
                header.Type = Handeco.Trim(xeHeader.XPathValue(".//td[2]//text()"));
                header.Groupes = xeHeader.XPathValues(".//td[3]//text()").Select(Handeco.Trim).ToArray();
                header.Activités = xeHeader.XPathValues(".//td[4]//text()").Select(Handeco.Trim).ToArray();
                header.PostalCode = Handeco.Trim(xeHeader.XPathValue(".//td[5]//text()"));
                headers.Add(header);
            }
            data.Headers = headers.ToArray();
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : https://www.handeco.org/fournisseurs/rechercher
            // page 2 : https://www.handeco.org/fournisseurs/rechercher/page/2
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

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : https://www.handeco.org/fournisseurs/rechercher
            // page 2 : https://www.handeco.org/fournisseurs/rechercher/page/2
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page != 1)
                throw new PBException("error impossible to load directly page {0}, only page 1 must be loaded first", page);
            //string url = __urlMainPage;
            //if (page > 1)
            //    url += string.Format("page/{0}/", page);
            //return new HttpRequest { Url = url };

            //_page = page;
            //string url = __urlMainPage;
            //requestParameters = new HttpRequestParameters_v1();
            //requestParameters.method = HttpRequestMethod.Post;
            //requestParameters.content = __content;
            //requestParameters.encoding = Encoding.UTF8;
            // private static string __urlMainPage = "https://www.handeco.org/fournisseurs/rechercher";
            // referer                               "https://www.handeco.org/fournisseurs/rechercher"
            return new HttpRequest { Url = __urlMainPage, Method = HttpRequestMethod.Post, Content = __content };
        }
    }

    public static class Test_Handeco_Header
    {
        public static void Test_Handeco_HeaderPage_01()
        {

        }
    }
}

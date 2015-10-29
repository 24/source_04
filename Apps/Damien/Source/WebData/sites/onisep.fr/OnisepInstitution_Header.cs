using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace hts.WebData
{
    public class OnisepInstitution_Header : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string UrlDetail;

        public string Institution = null;
        public string City = null;
        public string PostalCode = null;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class OnisepInstitution_HeaderPage : IEnumDataPages<OnisepInstitution_Header>, IKeyData<int>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public OnisepInstitution_Header[] Headers;
        public string UrlNextPage;

        public IEnumerable<OnisepInstitution_Header> GetDataList()
        {
            return Headers;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }

        public int GetKey()
        {
            return Id;
        }
    }

    public static class OnisepInstitution_HeaderManager
    {
        private static string __urlMainPage = "http://www.onisep.fr/content/search?searchForm=etab&etabRecherche=1&SearchText=&SubTreeArray=243418&zone_geo=&filters%5Battr_categorie_type_etablissement_t%5D%5B%5D=&etab_autocomplete=&submit=Lancer+la+recherche";
        private static WebDataPageManager<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header> __headerWebDataPageManager = null;

        static OnisepInstitution_HeaderManager()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("OnisepInstitution/Header"));
        }

        public static WebDataPageManager<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header> headerWebDataPageManager = new WebDataPageManager<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<OnisepInstitution_HeaderPage>();

            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = Handeco.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            headerWebDataPageManager.GetKeyFromHttpRequest = GetPageKey;

            headerWebDataPageManager.DocumentStore = MongoDocumentStore<int, OnisepInstitution_HeaderPage>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static OnisepInstitution_HeaderPage GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            OnisepInstitution_HeaderPage data = new OnisepInstitution_HeaderPage();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            // <table class="oni_tableSearchResults" style="width: 463px; margin-left: 5px" summary="">
            // ...
            //   <tbody>
            //     <tr class=oni_odd>
            //       <td class="oni_first"><a href="/Ressources/Univers-Postbac/Postbac/Aquitaine/Pyrenees-Atlantiques/Academie-Basque-du-Sport"> Académie Basque du Sport                        </a></td>
            //       <td>Biarritz</td>
            //       <td>64200</td>
            //     </tr>
            //     ...
            //   </tbody>
            // </table>

            // <div class="pagenavigator">
            //   <p>
            //     <span class="pages">
            //       <span class="current">1</span>
            //       <span class="other"><a href="/content/search/(offset)/10?&amp;SubTreeArray=243418&amp;etabRecherche=1&amp;idFormation=&amp;limit=10">2</a></span>
            //       ...
            //     </span>
            //     <span class="next"><a href="/content/search/(offset)/10?&amp;SubTreeArray=243418&amp;etabRecherche=1&amp;idFormation=&amp;limit=10"><span class="text">Suivant&nbsp;&raquo;</span></a></span>
            //   </p>
            //   <div class="break"></div>
            // </div>

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='pagenavigator']//span[@class='next']//a/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@class='oni_tableSearchResults']//tbody//tr");
            List<OnisepInstitution_Header> headers = new List<OnisepInstitution_Header>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                OnisepInstitution_Header header = new OnisepInstitution_Header();
                header.SourceUrl = url;
                header.LoadFromWebDate = DateTime.Now;
                XXElement xe = xeHeader.XPathElement(".//td[1]");
                header.Institution = OnisepInstitution.Trim(xe.XPathValue(".//text()"));
                header.UrlDetail = zurl.GetUrl(url, xe.XPathValue(".//a/@href"));
                header.City = OnisepInstitution.Trim(xeHeader.XPathValue(".//td[2]//text()"));
                header.PostalCode = OnisepInstitution.Trim(xeHeader.XPathValue(".//td[3]//text()"));
                headers.Add(header);
            }
            data.Headers = headers.ToArray();
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.onisep.fr/content/search?searchForm=etab&etabRecherche=1&SearchText=&SubTreeArray=243418&zone_geo=&filters%5Battr_categorie_type_etablissement_t%5D%5B%5D=&etab_autocomplete=&submit=Lancer+la+recherche
            // page 2 : http://www.onisep.fr/content/search/(offset)/10?&amp;SubTreeArray=243418&amp;etabRecherche=1&amp;idFormation=&amp;limit=10
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = uri.Segments[uri.Segments.Length - 1];
            if (lastSegment.EndsWith("/"))
                lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            int nb;
            if (!int.TryParse(lastSegment, out nb))
                throw new PBException("header page key not found in url \"{0}\"", url);
            return nb / 10 + 1;
        }

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : http://www.onisep.fr/content/search?searchForm=etab&etabRecherche=1&SearchText=&SubTreeArray=243418&zone_geo=&filters%5Battr_categorie_type_etablissement_t%5D%5B%5D=&etab_autocomplete=&submit=Lancer+la+recherche
            // page 2 : http://www.onisep.fr/content/search/(offset)/10?&amp;SubTreeArray=243418&amp;etabRecherche=1&amp;idFormation=&amp;limit=10
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
            return new HttpRequest { Url = __urlMainPage };
        }
    }
}

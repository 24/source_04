using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using pb.Data.old;
using pb.Data.Mongo.old;
using pb.Web.Data.old;

namespace hts.WebData
{
    // Cdefi_Header
    public class Cdefi_Header : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string UrlDetail;

        public int Id;
        public string Name = null;
        public string Acronym = null;     // Sigle

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class Cdefi_HeaderPage : IEnumDataPages<Cdefi_Header>, IKeyData_v4<int>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Cdefi_Header[] Headers;
        public string UrlNextPage;

        public IEnumerable<Cdefi_Header> GetDataList()
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

    public static class Cdefi_HeaderManager
    {
        private static string __urlMainPage = "http://www.cdefi.fr/actions.php";
        private static string __urlReferer = "http://www.cdefi.fr/fr/ecoles-ingenieurs";
        private static string __contentRequest = "action=searchSchool&departement=&domaine=&reseau=&academie=";
        private static WebDataPageManager_v1<int, Cdefi_HeaderPage, Cdefi_Header> __headerWebDataPageManager = null;

        static Cdefi_HeaderManager()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Cdefi/Header"));
        }

        public static WebDataPageManager_v1<int, Cdefi_HeaderPage, Cdefi_Header> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager_v1<int, Cdefi_HeaderPage, Cdefi_Header> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager_v1<int, Cdefi_HeaderPage, Cdefi_Header> headerWebDataPageManager = new WebDataPageManager_v1<int, Cdefi_HeaderPage, Cdefi_Header>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<Cdefi_HeaderPage>();

            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = Cdefi.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            headerWebDataPageManager.GetKeyFromHttpRequest = GetPageKey;

            headerWebDataPageManager.DocumentStore = MongoDocumentStore_v4<int, Cdefi_HeaderPage>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static Cdefi_HeaderPage GetData(WebResult webResult)
        {

            //XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Cdefi_HeaderPage data = new Cdefi_HeaderPage();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(webResult.Http.ResultText);

            // result document :
            //   {
            //     "results": {
            //       "complete"  : "1",
            //       "title"     : "227 écoles correspondent à votre recherche",
            //       "nbResults" : "227",
            //       "schools"   : [
            //         {
            //           "id": "78",
            //           "link":"http://www.cdefi.fr/fr/ecoles-ingenieurs/78/institut-dingenierie-informatique-de-limoges",
            //           "sigle":"3iL",
            //           "name":"Institut d'ingénierie informatique de Limoges"
            //         },
            //         ...
            //       ]
            //     }
            //   }

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='pagenavigator']//span[@class='next']//a/@href"));

            BsonArray schools = document.zGet("results.schools").zAsBsonArray();
            List<Cdefi_Header> headers = new List<Cdefi_Header>();
            foreach (BsonValue school in schools)
            {
                BsonDocument schoolDocument = school.zAsBsonDocument();
                Cdefi_Header header = new Cdefi_Header();
                header.SourceUrl = url;
                header.LoadFromWebDate = DateTime.Now;

                header.Id = int.Parse(schoolDocument.zGet("id").zAsString());
                header.UrlDetail = schoolDocument.zGet("link").zAsString();
                header.Acronym = schoolDocument.zGet("sigle").zAsString();
                header.Name = schoolDocument.zGet("name").zAsString();

                headers.Add(header);
            }
            data.Headers = headers.ToArray();
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // only one page
            // page 1 : http://www.cdefi.fr/actions.php
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            else
                throw new PBException("unknow page url \"{0}\"", url);
        }

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // only one page
            // page 1 : http://www.cdefi.fr/actions.php
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page != 1)
                throw new PBException("error only one page, page {0} dont exists", page);
            return new HttpRequest { Url = __urlMainPage, Method = HttpRequestMethod.Post, Referer = __urlReferer, Content = __contentRequest };
        }
    }
}

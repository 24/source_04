using System;
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
    //Cdefi_Detail
    public class Cdefi_Detail : IKeyData<int> // IWebData
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Institution = null;
        public string Address = null;
        public string Department = null;
        public string Director = null;
        public string Tel = null;
        public string Mail = null;
        public string WebSite = null;
        public string InstitutionType = null;

        public int GetKey()
        {
            return Id;
        }
    }

    public static class Cdefi_DetailManager
    {
        private static bool __trace = false;
        private static WebDataManager<int, Cdefi_Detail> __detailWebDataManager = null;
        private static WebHeaderDetailManager<int, Cdefi_HeaderPage, Cdefi_Header, int, Cdefi_Detail> __webHeaderDetailManager = null;

        static Cdefi_DetailManager()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("Cdefi/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager<int, Cdefi_HeaderPage, Cdefi_Header, int, Cdefi_Detail>();
            __webHeaderDetailManager.HeaderDataPageManager = Cdefi_HeaderManager.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager<int, Cdefi_Detail> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager<int, Cdefi_HeaderPage, Cdefi_Header, int, Cdefi_Detail> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager<int, Cdefi_Detail> CreateWebDataManager(XElement xe)
        {
            WebDataManager<int, Cdefi_Detail> detailWebDataManager = new WebDataManager<int, Cdefi_Detail>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<Cdefi_Detail>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetKey(httpRequest) / 1000 * 1000).ToString();
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => GetGroup(httpRequest);
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            //detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = Cdefi.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages;

            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Handeco_HeaderPage>(document);
            detailWebDataManager.DocumentStore = MongoDocumentStore<int, Cdefi_Detail>.Create(xe);

            return detailWebDataManager;
        }

        private static Cdefi_Detail GetData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            Cdefi_Detail data = new Cdefi_Detail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetKey(webResult.WebRequest.HttpRequest);

            XXElement xeData = xeSource.XPathElement("//div[@id='body']//div[@class='wBloc']");

            data.Institution = Cdefi.Trim(xeData.XPathValue(".//div[@class='schoolContentInfo']//div[@class='schoolContentInfo_infos verticalCenterToContainer']//h2/text()"));
            data.Address = Cdefi.Trim(xeData.XPathValue(".//div[@class='wPage'][1]//span[@class='editable']//text()"));

            foreach (string s in xeData.XPathElement(".//div[@class='wPage'][2]").DescendantTexts().Select(s => Cdefi.Trim(s)))
            {
                //string s2 = Cdefi.Trim(s);
                if (s.StartsWith("Nom du directeur", StringComparison.InvariantCultureIgnoreCase))
                {
                    int i = s.IndexOf(':');
                    if (i != -1)
                        data.Director = Cdefi.Trim(s.Substring(i + 1));
                }
                else if (s.StartsWith("Département", StringComparison.InvariantCultureIgnoreCase))
                {
                    int i = s.IndexOf(':');
                    if (i != -1)
                        data.Department = Cdefi.Trim(s.Substring(i + 1));
                }
                else if (s.StartsWith("Numéro de téléphone", StringComparison.InvariantCultureIgnoreCase))
                {
                    int i = s.IndexOf(':');
                    if (i != -1)
                        data.Tel = Cdefi.Trim(s.Substring(i + 1));
                }
                else if (s.StartsWith("Adresse email de contact", StringComparison.InvariantCultureIgnoreCase))
                {
                    int i = s.IndexOf(':');
                    if (i != -1)
                        data.Mail = Cdefi.Trim(s.Substring(i + 1));
                }
                else if (s.StartsWith("Adresse du site internet", StringComparison.InvariantCultureIgnoreCase))
                {
                    int i = s.IndexOf(':');
                    if (i != -1)
                        data.WebSite = Cdefi.Trim(s.Substring(i + 1));
                }
                else if (s.StartsWith("Nature de l'établissement", StringComparison.InvariantCultureIgnoreCase))
                {
                    int i = s.IndexOf(':');
                    if (i != -1)
                        data.InstitutionType = Cdefi.Trim(s.Substring(i + 1));
                }
                //else
                //    pb.Trace.WriteLine("text : \"{0}\"", s);
            }

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static int GetKey(HttpRequest httpRequest)
        {
            // http://www.onisep.fr/Ressources/Univers-Postbac/Postbac/Aquitaine/Pyrenees-Atlantiques/Academie-Basque-du-Sport
            // http://www.cdefi.fr/fr/ecoles-ingenieurs/78/institut-dingenierie-informatique-de-limoges
            //   key = Aquitaine_Pyrenees-Atlantiques_Academie-Basque-du-Sport
            string url = httpRequest.Url;
            if (!url.StartsWith("http://www.cdefi.fr/fr/ecoles-ingenieurs/"))
                throw new PBException("key not found in url \"{0}\"", url);
            url = url.Substring(41);
            int i = url.IndexOf('/');
            if (i == -1 || i == 0)
                throw new PBException("key not found in url \"{0}\"", httpRequest.Url);
            int id;
            if (!int.TryParse(url.Substring(0, i), out id))
                throw new PBException("key not found in url \"{0}\"", httpRequest.Url);
            return id;
        }
    }
}

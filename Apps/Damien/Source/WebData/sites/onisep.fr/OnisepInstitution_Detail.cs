using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using pb.Web.Data;
using pb.Data.old;
using pb.Data.Mongo.old;
using pb.Web.Data.old;

namespace hts.WebData
{
    public class OnisepInstitution_Detail : IKeyData_v4<string> // IWebData
    {
        public string Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Institution = null;
        public string UAICode = null;
        public string Address = null;
        public string PostalCode = null;
        public string City = null;
        public string Tel = null;
        public string Fax = null;
        public string Mail = null;
        public string WebSite = null;
        public string InstitutionStatus = null;
        public string Lodging = null;                // hébergement
        public string Ulis = null;
        public string[] StudyLevels = null;
        public int? BacLevel = null;

        public string GetKey()
        {
            return Id;
        }
    }

    public static class OnisepInstitution_DetailManager
    {
        private static bool __trace = false;
        private static WebDataManager_v1<string, OnisepInstitution_Detail> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v1<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header, string, OnisepInstitution_Detail> __webHeaderDetailManager = null;
        //private static Regex __badCharacters = new Regex("(\xA0|\t|\r|\n)+", RegexOptions.Compiled);
        //private static Regex __lastUpdateRegex = new Regex("[0-9]{2}-[0-9]{2}-[0-9]{4}", RegexOptions.Compiled);  // Dernière mise à jour le 18-01-2013
        //private static Regex __email1Regex = new Regex("email1\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email1 = "jeu-ser"
        //private static Regex __email2Regex = new Regex("email2\\s*=\\s*\"([^\"]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);  // email2 = "wanadoo.fr"

        static OnisepInstitution_DetailManager()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("OnisepInstitution/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager_v1<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header, string, OnisepInstitution_Detail>();
            __webHeaderDetailManager.HeaderDataPageManager = OnisepInstitution_HeaderManager.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager_v1<string, OnisepInstitution_Detail> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v1<int, OnisepInstitution_HeaderPage, OnisepInstitution_Header, string, OnisepInstitution_Detail> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        private static WebDataManager_v1<string, OnisepInstitution_Detail> CreateWebDataManager(XElement xe)
        {
            WebDataManager_v1<string, OnisepInstitution_Detail> detailWebDataManager = new WebDataManager_v1<string, OnisepInstitution_Detail>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<OnisepInstitution_Detail>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetKey(httpRequest) / 1000 * 1000).ToString();
                urlCache.GetUrlSubDirectory = httpRequest => GetGroup(httpRequest);
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            //detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = OnisepInstitution.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages;

            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Handeco_HeaderPage>(document);
            detailWebDataManager.DocumentStore = MongoDocumentStore_v4<string, OnisepInstitution_Detail>.Create(xe);

            return detailWebDataManager;
        }

        private static OnisepInstitution_Detail GetData(WebResult webResult)
        {
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            OnisepInstitution_Detail data = new OnisepInstitution_Detail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetKey(webResult.WebRequest.HttpRequest);

            XXElement xeData = xeSource.XPathElement("//div[@id='oni_content-page']//div[@class='oni_innerContent']//div[@id='oni_zoom-block']");

            data.Institution = OnisepInstitution.Trim(xeData.XPathValue(".//h1/text()"));
            // <span class="oni_span-title">Code UAI : 0062080D</span>
            string s = OnisepInstitution.Trim(xeData.XPathValue(".//span[@class='oni_span-title']/text()"));
            if (s != null && s.StartsWith("Code UAI :", StringComparison.InvariantCultureIgnoreCase))
                data.UAICode = OnisepInstitution.Trim(s.Substring(10));

            XXElement xe = xeData.XPathElement(".//div[@class='oni_fiche-info-1']");

            data.Address = OnisepInstitution.Trim(xe.XPathValue(".//span[@class='street-address']/text()"));
            data.PostalCode = OnisepInstitution.Trim(xe.XPathValue(".//span[@class='postal-code']/text()"));
            data.City = OnisepInstitution.Trim(xe.XPathValue(".//span[@class='locality']/text()"));
            data.Tel = OnisepInstitution.Trim(xe.XPathValue(".//span[@class='tel']/text()"));
            s = xe.XPathValues(".//p[@class='vcard']//text()").Select(OnisepInstitution.Trim).Where(t => t.StartsWith("Fax :", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (s != null)
                data.Fax = OnisepInstitution.Trim(s.Substring(5));
            s = xe.XPathValue(".//a[@class='email']/@href");
            if (s != null && s.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                s = s.Substring(7);
            data.Mail = s;
            data.WebSite = xe.DescendantTextNodes().Where(xt => string.Equals(OnisepInstitution.Trim(xt.Value), "site :", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()
                //.zXPathValue(".//following-sibling::a/@href");
                .zXPathValue(".//following::a/@href");

            foreach (XXElement xe2 in xeData.XPathElements(".//div[@class='oni_fiche-info-2']//li"))
            {
                string[] values = xe2.DescendantTexts().Take(2).ToArray();
                if (values.Length != 2)
                    continue;
                switch (OnisepInstitution.Trim(values[0]).ToLower())
                {
                    case "statut de l'établissement :":
                        data.InstitutionStatus = OnisepInstitution.Trim(values[1]);
                        break;
                    case "hébergement :":
                        data.Lodging = OnisepInstitution.Trim(values[1]);
                        break;
                    case "présence d'une ulis":
                        data.Ulis = OnisepInstitution.Trim(values[1]);
                        break;
                }
            }

            data.StudyLevels = xeData.XPathElements(".//div[@class='oni_nav-in']//ul[@class='oni_nav-in-ul']//li").Select(li => li.DescendantTexts().zConcatStrings()).Where(txt => txt != null).ToArray();

            data.BacLevel = GetBacLevel(data.StudyLevels);

            // Address    = text in <span class="street-address">
            // PostalCode = text in <span class="postal-code">
            // City       = text in <span class="locality">
            // Tel        = text in <span class="tel">
            // Fax        = text start with "Fax :"
            // Mail       = @href start with mailto: in <a class="email">

            //bool address = false;
            //foreach (XNode node in xeData.XPathElement(".//div[@class='oni_fiche-info-1']//p[@class='vcard']").DescendantNodes())
            //{
            //    if (node is XElement)
            //    {
            //        XElement xe = (XElement)node;
            //        if (xe.Name == "span")
            //        {
            //            XAttribute attribute = xe.Attribute("class");
            //            if (attribute != null && attribute.Value == "street-address")
            //                address = true;
            //        }
            //        else
            //            address = false;
            //    }
            //    if (node is XText)
            //    {
            //        if (address)
            //        {
            //            data.Address = OnisepInstitution.Trim(((XText)node).Value);
            //            address = false;
            //        }
            //    }
            //}

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static Regex __bacLevelRegex = new Regex(@"bac\s*\+\s*([0-9]+)(\s*[aà]\s*([0-9]+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static int? GetBacLevel(string[] studyLevels)
        {
            int bacLevel = 0;
            foreach (string studyLevel in studyLevels)
            {
                foreach (Match match in __bacLevelRegex.Matches(studyLevel))
                {
                    string value = match.Groups[3].Value;
                    if (value == "")
                        value = match.Groups[1].Value;
                    int l = int.Parse(value);
                    if (l > bacLevel)
                        bacLevel = l;
                }
            }
            if (bacLevel > 0)
                return bacLevel;
            else
                return null;
        }

        private static string GetKey(HttpRequest httpRequest)
        {
            // http://www.onisep.fr/Ressources/Univers-Postbac/Postbac/Aquitaine/Pyrenees-Atlantiques/Academie-Basque-du-Sport
            //   key = Aquitaine_Pyrenees-Atlantiques_Academie-Basque-du-Sport
            string url = httpRequest.Url;
            if (!url.StartsWith("http://www.onisep.fr/Ressources/Univers-Postbac/Postbac/"))
                throw new PBException("key not found in url \"{0}\"", url);
            return url.Substring(56).Replace('/', '_');
        }

        private static string GetGroup(HttpRequest httpRequest)
        {
            // http://www.onisep.fr/Ressources/Univers-Postbac/Postbac/Aquitaine/Pyrenees-Atlantiques/Academie-Basque-du-Sport => "Aquitaine"

            string url = httpRequest.Url;
            if (!url.StartsWith("http://www.onisep.fr/Ressources/Univers-Postbac/Postbac/"))
                throw new PBException("group not found in url \"{0}\"", url);
            url = url.Substring(56);
            int i = url.IndexOf('/');
            if (i == -1 || i == 0)
                //throw new PBException("group not found in url \"{0}\"", url);
                return "_unknow";
            else
                return url.Substring(0, i);
        }
    }
}

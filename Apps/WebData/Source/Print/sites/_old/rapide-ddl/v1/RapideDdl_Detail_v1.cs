using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

// pb 3 files to download :
//   http://www.rapide-ddl.com/ebooks/livres/35163-collection-de-775-essais-romans-histoire-format-epub.html
// pb les liens ne sont pas récupérés
//   http://www.rapide-ddl.com/ebooks/magazine/35250-psychologies-no342-juillet-aot-2014.html
//   http://www.rapide-ddl.com/ebooks/magazine/35331-les-cahiers-du-monde-de-lintelligence-n-5-juillet-aout-septembre-2014.html
//   http://www.rapide-ddl.com/ebooks/magazine/36143-secrets-dhistoire-de-dgtours-en-france-no1-2014.html
//   http://www.rapide-ddl.com/ebooks/magazine/36142-le-point-no2184-du-24-au-30-juillet-2014-lien-direct.html
// pas d'image
//   http://www.rapide-ddl.com/ebooks/magazine/35852-ici-paris-no3602-16-au-22-juillet-2014.html

namespace Download.Print.RapideDdl.v1
{
    public class RapideDdl_PostDetail : RapideDdl_Base
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;

        public string postAuthor;
        public DateTime? creationDate = null;
        public string category = null;
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
        public List<string> downloadLinks = new List<string>();
    }

    public static class RapideDdl_LoadDetail
    {
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useXml = false;
        private static string __xmlNodeName = null;
        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static pb.Web.v1.LoadWebData_v2<RapideDdl_PostDetail> _load;

        static RapideDdl_LoadDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("RapideDdl/Detail"));
        }

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            __useXml = xe.zXPathValue("UseXml").zTryParseAs(__useXml);
            __xmlNodeName = xe.zXPathValue("XmlNodeName");
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v1<RapideDdl_PostDetail> documentStore = null;
            if (__useMongo)
            {
                documentStore = new MongoDocumentStore_v1<RapideDdl_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                RapideDdl.InitMongoClassMap();
            }
            _load = new pb.Web.v1.LoadWebData_v2<RapideDdl_PostDetail>(new pb.Web.v1.LoadDataFromWeb_v2<RapideDdl_PostDetail>(LoadPostDetailFromWeb, GetUrlCache()), documentStore);
            //_load.SetXmlParameters(__useXml, __xmlNodeName);
        }

        // bool desactivateDocumentStore = false
        public static RapideDdl_PostDetail Load(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            //Trace.WriteLine("RapideDdl_LoadDetail.Load  \"{0}\"", url);
            if (requestParameters == null)
                requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            pb.Web.v1.RequestFromWeb_v2 request = new pb.Web.v1.RequestFromWeb_v2(url, requestParameters, reload, false);
            //_load.DesactivateDocumentStore = desactivateDocumentStore;
            RapideDdl_PostDetail postDetail = _load.Load(request, GetPostDetailKey(url), refreshDocumentStore);
            //_load.DesactivateDocumentStore = false;
            if (loadImage)
                pb.old.Http_v2.LoadImageFromWeb(postDetail.images);
            return postDetail;
        }

        public static RapideDdl_PostDetail LoadPostDetailFromWeb(pb.Web.v1.RequestFromWeb_v2 request)
        {
            XXElement xeSource = new XXElement(request.GetXmlDocument().Root);
            RapideDdl_PostDetail data = new RapideDdl_PostDetail();
            data.sourceUrl = request.Url;
            data.loadFromWebDate = DateTime.Now;

            XXElement xePost = xeSource.XPathElement("//div[@class='lcolomn mainside']");

            //data.category = xePost.DescendantTextList(".//div[@class='spbar']//a").Select(Download.Print.RapideDdl.RapideDdl.TrimFunc1).Where(s => s != "Accueil" && s != "").zToStringValues("/");
            data.category = xePost.XPathElements(".//div[@class='spbar']//a").DescendantTexts().Select(Download.Print.RapideDdl.RapideDdl.TrimFunc1).Where(s => s != "Accueil" && s != "").zToStringValues("/");

            //data.title = RapideDdl.ExtractTextValues(data.infos, xePost.XPathValue(".//div[@class='base fullstory']//text()", RapideDdl.TrimFunc1));
            //data.title = xePost.DescendantTextList(".//div[@class='spbar']", node => !(node is XElement) || ((XElement)node).Name != "a", RapideDdl.TrimFunc1).FirstOrDefault();
            //data.title = xePost.XPathValue(".//div[@class='spbar']/text()", RapideDdl.TrimFunc1);
            //data.title = xePost.DescendantTextList(".//div[@class='spbar']", func: Download.Print.RapideDdl.RapideDdl.TrimFunc1).LastOrDefault();
            data.title = xePost.XPathElements(".//div[@class='spbar']").DescendantTexts().Select(Download.Print.RapideDdl.RapideDdl.TrimFunc1).LastOrDefault();

            XXElement xe = xePost.XPathElement(".//div[@class='shdinfo']");
            //////////////data.creationDate = Download.Print.RapideDdl.RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), (DateTime)data.loadFromWebDate);
            data.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");

            xe = xePost.XPathElement(".//div[@class='maincont']");
            //data.images = xe.XPathImages(request.Url, nodeFilter: node => node is XElement && ((XElement)node).Name == "a");
            //data.images = xe.XPathImages(request.Url);
            //data.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, request.Url)).ToList();
            data.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, request.Url)).ToList();

            //if (request.LoadImage)
            // force load image to get image width and height
            pb.old.Http_v2.LoadImageFromWeb(data.images);

            //data.SetTextValues(xe.DescendantTextList(".//span[@id='post-img']", node => node is XElement && ((XElement)node).Name == "a" ? false : true));
            //data.SetTextValues(xe.DescendantTextList(".//div"));
            data.SetTextValues(xe.XPathElements(".//div").DescendantTexts());

            //data.downloadLinks.AddRange(xe.XPathValues(".//div[2]//a/@href"));
            //foreach (XXElement xe2 in xe.XPathElements("div/div").Skip(1))
            foreach (XXElement xe2 in xe.XPathElements("div/div"))
            {
                // http://prezup.eu http://pixhst.com/avaxhome/27/36/002e3627.jpeg http://www.zupmage.eu/i/R1UgqdXn4F.jpg
                // http://i.imgur.com/Gu7hagN.jpg http://img11.hostingpics.net/pics/591623liens.png http://www.hapshack.com/images/jUfTZ.gif
                // http://pixhst.com/pictures/3029467
                //data.downloadLinks.AddRange(xe2.XPathValues(".//a/@href").Where(url => !url.StartsWith("http://prezup.eu") && !url.StartsWith("http://pixhst.com") && !url.StartsWith("http://www.zupmage.eu")));
                data.downloadLinks.AddRange(xe2.XPathValues(".//a/@href").Where(url => !url.StartsWith("http://prezup.eu") && !url.StartsWith("http://pixhst.com")
                    && !url.EndsWith(".jpg") && !url.EndsWith("jpeg") && !url.EndsWith("png") && !url.EndsWith("gif")));
            }

            return data;
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static object GetPostDetailKey(string url)
        {
            // http://www.rapide-ddl.com/ebooks/magazine/35030-le-nouvel-observateur-hors-sgrie-week-end-nv-5-juillet-aogt-2014.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }
    }
}

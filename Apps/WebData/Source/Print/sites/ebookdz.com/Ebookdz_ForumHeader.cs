using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using pb;

namespace Download.Print.Ebookdz
{
    public class Ebookdz_ForumHeader : WebDataPageMongoManagerBase<PostHeader>
    {
        private static string __configName = "Ebookdz";
        private static Ebookdz_ForumHeader __current = null;
        //private static string __urlMainForum = "http://www.ebookdz.com/forum/forum.php";
        //private static WebDataPageManager_v2<IHeaderData> __mainForumDataPageManager = null;

        public static Ebookdz_ForumHeader Current { get { return __current; } }

        static Ebookdz_ForumHeader()
        {
            Ebookdz.FakeInit();
            Init(test: DownloadPrint.Test);
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
            EbookdzLogin.Init(xe);
            __current = new Ebookdz_ForumHeader();
            __current.DataPageNominalType = typeof(PostHeaderDataPage<PostHeader>);
            __current.Create(xe.zXPathElement("ForumHeader"));
        }

        protected override void InitLoadFromWeb()
        {
            EbookdzLogin.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return EbookdzLogin.GetHttpRequestParameters();
        }

        protected override IEnumDataPages<PostHeader> GetDataPage(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<PostHeader> data = new PostHeaderDataPage<PostHeader>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetKey(webResult.WebRequest.HttpRequest);

            // <div class="threadpagenav">
            // <span class="prev_next">
            // <a rel="next" href="forumdisplay.php?f=157&amp;page=2&amp;s=fec27f3bac2b58debbb727ab8725c8a4" title="Page suivante - Résultats de 21 à 40 sur 61">
            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='threadpagenav']//span[@class='prev_next']//a[@rel='next']/@href"));

            List<PostHeader> headers = new List<PostHeader>();

            // <div class="body_bd">
            // <div id="threadlist" class="threadlist">
            // <ol id="threads" class="threads">
            // <li class="threadbit " id="thread_111977">
            //   <h3 class="threadtitle">
            //   <a title="" class="title" href="showthread.php?t=111977&amp;s=fec27f3bac2b58debbb727ab8725c8a4" id="thread_title_111977">La  Provence Marseille du lundi 26 janvier 2015</a>

            foreach (XXElement xeHeader in xeSource.XPathElements("//div[@id='threadlist']//ol[@id='threads']/li"))
            {
                PostHeader header = new PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                XXElement xe = xeHeader.XPathElement(".//h3[@class='threadtitle']//a[@class='title']");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = Ebookdz_MainForum.GetUrl(zurl.GetUrl(url, xe.XPathValue("@href")));

                headers.Add(header);
            }

            data.Headers = headers.ToArray();

            //Trace.WriteLine(data.zToJson());

            return data;
        }

        protected override BsonValue GetKey(HttpRequest httpRequest)
        {
            return _GetKey(httpRequest);
        }

        private static Regex __keyRegex = new Regex(@"\?f=([0-9]+)(?:&page=([0-9]+))?$", RegexOptions.Compiled);
        public static int _GetKey(HttpRequest httpRequest)
        {
            // http://www.ebookdz.com/forum/forumdisplay.php?f=156
            // http://www.ebookdz.com/forum/forumdisplay.php?f=156&page=2
            Match match = __keyRegex.Match(httpRequest.Url);
            if (!match.Success)
                throw new PBException("key not found in url \"{0}\"", httpRequest.Url);
            int key = int.Parse(match.Groups[1].Value) * 1000;
            if (match.Groups[2].Value != "")
                key += int.Parse(match.Groups[2].Value);
            else // page 1
                key++;
            //Trace.WriteLine("Ebookdz_SubForum._GetKey() : url \"{0}\" key {1}", httpRequest.Url, key);
            return key;
        }
    }
}

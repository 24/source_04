using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using pb.Web.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Download.Print.Ebookdz
{
    public class Ebookdz_SubForum : WebDataPageMongoManagerBase<EbookdzForumData>
    {
        private static string __configName = "Ebookdz";
        private static Ebookdz_SubForum __current = null;
        //private static string __urlMainForum = "http://www.ebookdz.com/forum/forum.php";
        //private static WebDataPageManager_v2<IHeaderData> __mainForumDataPageManager = null;

        public static Ebookdz_SubForum Current { get { return __current; } }

        //static Ebookdz_SubForum()
        //{
        //    Ebookdz.FakeInit();
        //    Init(test: DownloadPrint.Test);
        //}

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
            __current = new Ebookdz_SubForum();
            __current.DataPageNominalType = typeof(PostHeaderDataPage<EbookdzForumData>);
            __current.Create(xe.zXPathElement("Forum"));
        }

        protected override void InitLoadFromWeb()
        {
            EbookdzLogin.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return EbookdzLogin.GetHttpRequestParameters();
        }

        protected override IEnumDataPages<EbookdzForumData> GetDataPage(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<EbookdzForumData> data = new PostHeaderDataPage<EbookdzForumData>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = null;

            List<EbookdzForumData> forums = new List<EbookdzForumData>();

            Predicate<string> filter = null;

            // <div class="body_bd">
            // <div id="forumbits" class="forumbits">
            // <ol>
            // <li id="forum10" class="forumbit_post new L1">
            //   <div class="forumrow">
            //   <ol id="childforum_for_161" class="childsubforum">
            //     <div class="titleline">
            foreach (XXElement xe in xeSource.XPathElements("//div[@id='forumbits']/ol/li"))
            {
                XXElement xe2 = xe.XPathElement(".//div[@class='forumrow']//a");
                string category = xe2.XPathValue(".//text()");
                string urlCategory = Ebookdz_MainForum.GetUrl(zurl.GetUrl(url, xe2.XPathValue("@href")));
                // Forum = forum
                //forums.Add(new Ebookdz_Forum { Category = category, Url = urlCategory });

                foreach (XXElement xe3 in xe.XPathElements(".//ol[@class='childsubforum']/li//div[@class='titleline']//a"))
                {
                    string name = xe3.XPathValue(".//text()");

                    if (filter != null && !filter(name))
                        continue;
                    string urlSubForum = Ebookdz_MainForum.GetUrl(zurl.GetUrl(url, xe3.XPathValue("@href")));
                    // Forum = forum
                    forums.Add(new EbookdzForumData { SourceUrl = url, LoadFromWebDate = webResult.LoadFromWebDate, Category = category, Name = name, UrlDetail = urlSubForum });
                }
            }

            data.Headers = forums.ToArray();
            return data;

        }

        protected override BsonValue GetKey(HttpRequest httpRequest)
        {
            return _GetKey(httpRequest);
        }

        private static Regex __keyRegex = new Regex(@"\?f=([0-9]+)$", RegexOptions.Compiled);
        public static int _GetKey(HttpRequest httpRequest)
        {
            // http://www.ebookdz.com/forum/forumdisplay.php?f=11
            Match match = __keyRegex.Match(httpRequest.Url);
            if (!match.Success)
                throw new PBException("key not found in url \"{0}\"", httpRequest.Url);
            int key = int.Parse(match.Groups[1].Value);
            //Trace.WriteLine("Ebookdz_SubForum._GetKey() : url \"{0}\" key {1}", httpRequest.Url, key);
            return key;
        }
    }
}

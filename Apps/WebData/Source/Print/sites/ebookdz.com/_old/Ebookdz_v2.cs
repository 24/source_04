using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Web;

namespace Download.Print.Ebookdz.old
{
    public static class Ebookdz_v2
    {
        static Ebookdz_v2()
        {
            Init(test: DownloadPrint.Test);
        }

        public static void FakeInit()
        {
        }

        public static void Init(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement("Ebookdz");
            else
            {
                Trace.WriteLine("Ebookdz init for test");
                xe = XmlConfig.CurrentConfig.GetElement("Ebookdz_Test");
            }
            EbookdzLogin_v2.Init(xe);
            Ebookdz_MainForumManager_v2.Init(xe);
            Ebookdz_SubForumManager_v2.Init(xe);
            Ebookdz_ForumHeaderManager_v2.Init(xe);
            Ebookdz_HeaderManager_v2.Init(xe);
            Ebookdz_DetailManager_v2.Init(xe);
            ServerManagers.Add("Ebookdz", CreateServerManager());
        }

        //public static HttpRequestParameters GetHttpRequestParameters()
        //{
        //    return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        //}

        public static ServerManager CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => Ebookdz_DetailManager_v2.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 7, startPage: 1, maxPage: 1);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        // useCursorCache: true
                        return Ebookdz_DetailManager_v2.DetailWebDataManager.Find(query, sort: sort, loadImage: false);
                    };
            }
            Func<BsonValue, IPostToDownload> loadPost = id => Ebookdz_DetailManager_v2.DetailWebDataManager.Find(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();
            return new ServerManager
            {
                Name = "ebookdz.com",
                EnableLoadNewPost = false,
                EnableSearchPostToDownload = false,
                DownloadDirectory = null,
                LoadNewPost = loadNewPost,
                GetPostList = getPostList,
                LoadPost = loadPost
            };
        }
    }

    public static class EbookdzLogin_v2
    {
        private static bool __isLoggedIn = false;
        private static string __cookiesFile = null;
        private static CookieContainer __cookies = null;

        //static EbookdzLogin_v2()
        //{
        //    InitCookies();
        //}

        public static void Init(XElement xe)
        {
            InitCookies(xe);
        }

        private static void InitCookies(XElement xe)
        {
            if (__cookies == null)
            {
                //__cookiesFile = XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesFile");
                __cookiesFile = xe.zXPathExplicitValue("CookiesFile");
                if (zFile.Exists(__cookiesFile))
                    __cookies = zcookies.LoadCookies(__cookiesFile);
                else
                    __cookies = new CookieContainer();
            }
        }

        public static void InitLoadFromWeb()
        {
            if (__isLoggedIn)
                return;
            Http http = LoadMainPage();
            XXElement xeSource = new XXElement(http.zGetXDocument().Root);
            if (!IsLoggedIn(xeSource))
            {
                http = Login(xeSource);
                SaveCookies(http.RequestParameters.Cookies);
                if (!IsLoggedIn())
                    throw new PBException("unable login to http://www.ebookdz.com/");
            }
            __isLoggedIn = true;
        }

        public static Http LoadMainPage()
        {
            return HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = Ebookdz_v1.UrlMainPage }, new HttpRequestParameters { Cookies = __cookies });
        }

        public static bool IsLoggedIn()
        {
            Http http = LoadMainPage();
            return IsLoggedIn(new XXElement(http.zGetXDocument().Root));
        }

        public static bool IsLoggedIn(XXElement xeSource)
        {
            return GetLogin(xeSource) != null;
        }

        public static string GetLogin(XXElement xeSource)
        {
            // ebookdz.com_forum_showthread.php_t_109595_01_02.html :
            //   <div id="toplinks" class="toplinks">
            //   <li class="welcomelink">Bienvenue, <a href="member.php?u=49369"><b>la_beuze</b></a></li>
            return xeSource.XPathValue("//div[@id='toplinks']//li[@class='welcomelink']//a//text()");
        }

        public static void Login()
        {
            //Http_new http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = __urlWebSite });
            Http http = LoadMainPage();
            Login(new XXElement(http.zGetXDocument().Root));
        }

        public static Http Login(XXElement xeSource)
        {
            XmlConfig localConfig = new XmlConfig(XmlConfig.CurrentConfig.GetExplicit("LocalConfig"));
            string login = localConfig.GetExplicit("DownloadAutomateManager/Ebookdz/Login");
            //string hashPassword = Crypt.ComputeMD5Hash(localConfig.GetExplicit("DownloadAutomateManager/Ebookdz/Password"));
            string hashPassword = Crypt.ComputeMD5Hash(localConfig.GetExplicit("DownloadAutomateManager/Ebookdz/Password")).zToHex(lowercase: true);

            // <base href="http://www.ebookdz.com/forum/" />
            string urlBase = xeSource.XPathValue("//head//base/@href");
            //string urlBase = xeSource.XPathValue("//body//base/@href");
            //Trace.WriteLine("urlBase : \"{0}\"", urlBase);
            XXElement xeForm = xeSource.XPathElement("//form[@id='navbar_loginform']");
            if (xeForm.XElement == null)
            {
                //Trace.WriteLine("element not found \"//form[@id='navbar_loginform']\"");
                throw new PBException("element form not found \"//form[@id='navbar_loginform']\"");
            }
            //Trace.WriteLine("form action : \"{0}\"", xeForm.XPathValue("@action"));
            string urlForm = zurl.GetUrl(urlBase, xeForm.XPathValue("@action"));
            string method = xeForm.XPathValue("@method");
            //Trace.WriteLine("urlForm : \"{0}\" method {1}", urlForm, method);
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (XXElement xeInput in xeForm.XPathElements(".//input"))
            {
                string name = xeInput.XPathValue("@name");
                if (name == null)
                    continue;
                string value = null;
                if (name == "vb_login_username")
                    value = login;
                else if (name == "vb_login_password")
                    value = null;
                else if (name == "vb_login_md5password" || name == "vb_login_md5password_utf")
                    value = hashPassword;
                else
                    value = xeInput.XPathValue("@value");
                if (!first)
                    sb.Append("&");
                sb.AppendFormat("{0}={1}", name, value);
                first = false;
            }
            string content = sb.ToString();
            //Trace.WriteLine("content : \"{0}\"", content);

            HttpRequest httpRequest = new HttpRequest { Url = urlForm, Content = content, Method = Http.GetHttpRequestMethod(method) };
            HttpRequestParameters httpRequestParameters = new HttpRequestParameters();
            Http http = HttpManager.CurrentHttpManager.Load(httpRequest, httpRequestParameters);
            //xeSource = new XXElement(http.zGetXmlDocument().Root);
            //if (!IsLoggedIn(xeSource))
            //    throw new PBException("unable login to http://www.ebookdz.com/");
            return http;
        }

        public static void SaveCookies(CookieContainer cookies)
        {
            __cookies = cookies;
            zcookies.SaveCookies(cookies, Ebookdz_v1.UrlMainPage, __cookiesFile);
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Cookies = __cookies };
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Web;
using Print;
using Download.Print.old;

namespace Download.Print.Ebookdz.old
{
    public static class Ebookdz_v1
    {
        private static string __urlMainPage = "http://www.ebookdz.com/";
        //private static WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader> __headerWebDataPageManager = null;
        //private static WebHeaderDetailManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader, int, Ebookdz_PostDetail> __webHeaderDetailManager = null;
        //private static WebDataManager<int, Ebookdz_PostDetail> __detailWebDataManager = null;

        static Ebookdz_v1()
        {
            //__headerWebDataPageManager = EbookdzHeaderManager.CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Header"));
            //__detailWebDataManager = EbookdzDetailManager.CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Detail"));
            //__webHeaderDetailManager = new WebHeaderDetailManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader, int, Ebookdz_PostDetail>();
            //__webHeaderDetailManager.HeaderDataPageManager = EbookdzHeaderManager.HeaderWebDataPageManager;
            //__webHeaderDetailManager.DetailDataManager = EbookdzDetailManager.DetailWebDataManager;
            //Trace.WriteLine("Ebookdz.Ebookdz()");
            ServerManagers_v1.Add("Ebookdz", CreateServerManager());
        }

        public static void FakeInit()
        {
            //string url = UrlMainPage;
        }

        public static string UrlMainPage { get { return __urlMainPage; } }
        //public static WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }
        //public static WebHeaderDetailManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader, int, Ebookdz_PostDetail> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }
        //public static WebDataManager<int, Ebookdz_PostDetail> DetailWebDataManager { get { return __detailWebDataManager; } }

        public static string GetUrl(string url)
        {
            if (url != null)
            {
                // remove "s" value in query
                // http://www.ebookdz.com/forum/forumdisplay.php?f=1&s=1fdf76d35a57d09aa11e75ff6f0d9985
                PBUriBuilder uriBuilder = new PBUriBuilder(url);
                uriBuilder.RemoveQueryValue("s");
                url = uriBuilder.ToString();
            }
            return url;
        }

        //public static ServerManager CreateServerManager(bool enableLoadNewPost = true, bool enableSearchPostToDownload = true, string downloadDirectory = null)
        public static ServerManager_v1 CreateServerManager(Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload_v1>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = () => Ebookdz_DetailManager_v1.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 7, startPage: 1, maxPage: 1);
            if (getPostList == null)
            {
                getPostList =
                    lastRunDateTime =>
                    {
                        string query = string.Format("{{ 'download.PostCreationDate': {{ $gt: ISODate('{0}') }} }}", lastRunDateTime.ToUniversalTime().ToString("o"));
                        string sort = "{ 'download.PostCreationDate': -1 }";
                        return Ebookdz_DetailManager_v1.DetailWebDataManager.FindDocuments(query, sort: sort, loadImage: false);
                    };
            }
            Func<int, IPostToDownload_v1> loadPost = id => Ebookdz_DetailManager_v1.DetailWebDataManager.FindDocuments(string.Format("{{ _id: {0} }}", id)).FirstOrDefault();
            return new ServerManager_v1
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

    public static class EbookdzLogin_v1
    {
        private static bool __isLoggedIn = false;
        private static string __cookiesFile = null;
        private static CookieContainer __cookies = null;

        static EbookdzLogin_v1()
        {
            InitCookies();
        }

        private static void InitCookies()
        {
            if (__cookies == null)
            {
                __cookiesFile = XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesFile");
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

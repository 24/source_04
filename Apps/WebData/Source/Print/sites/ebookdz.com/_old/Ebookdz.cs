using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Ebookdz.old
{
    public static class Ebookdz
    {
        private static string __urlWebSite = "http://www.ebookdz.com/";
        private static bool __isLoggedIn = false;
        private static string __cookiesFile = null;
        private static CookieContainer __cookies = null;

        static Ebookdz()
        {
            __cookiesFile = XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesFile");
            if (File.Exists(__cookiesFile))
                __cookies = zcookies.LoadCookies(__cookiesFile);
            else
                __cookies = new CookieContainer();
        }

        public static string UrlWebSite { get { return __urlWebSite; } }
        public static CookieContainer Cookies { get { return __cookies; } }

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
            return HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = __urlWebSite }, new HttpRequestParameters { Cookies = __cookies });
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
            zcookies.SaveCookies(cookies, __urlWebSite, __cookiesFile);
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Cookies = __cookies };
        }

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

        public static IKeyData<int> GetForumHeaderPageData(LoadDataFromWeb_v4 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.Http.zGetXDocument().Root);
            string url = loadDataFromWeb.WebRequest.HttpRequest.Url;
            Ebookdz_HeaderPage data = new Ebookdz_HeaderPage();
            data.SourceUrl = url;
            data.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;
            //data.Id = Ebookdz_LoadHeaderPagesManager.GetHeaderPageKey(loadDataFromWeb.WebRequest.HttpRequest);

            // <div id="above_threadlist" class="above_threadlist">
            // <div class="threadpagenav">
            // <span class="prev_next">
            // <a rel="next" href="forumdisplay.php?f=74&amp;page=2&amp;s=4807e931448c05da34dd54fbd0308479" title="Page suivante - Résultats de 21 à 40 sur 66">
            data.UrlNextPage = GetUrl(zurl.GetUrl(url, xeSource.XPathValue("//div[@id='above_threadlist']//span[@class='prev_next']//a[@rel='next']/@href")));

            // <div class="body_bd">
            XXElement xePost = xeSource.XPathElement("//div[@class='body_bd']");

            // <div id="breadcrumb" class="breadcrumb">
            // <ul class="floatcontainer">
            // <li class="navbit">
            // Forum / Journaux / Presse quotidienne / Autres Journaux

            // <div id="threadlist" class="threadlist">
            // <ol id="threads" class="threads">

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='threadlist']//ol[@id='threads']/li");
            List<Ebookdz_PostHeader> headers = new List<Ebookdz_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Ebookdz_PostHeader header = new Ebookdz_PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;

                // <div class="threadinfo" title="">
                // <div class="inner">
                // <a title="" class="title" href="showthread.php?t=111210&amp;s=4807e931448c05da34dd54fbd0308479" id="thread_title_111210">L'OPINION du mardi  20 janvier 2015</a>

                XXElement xe = xeHeader.XPathElement(".//div[@class='threadinfo']//a[@class='title']");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = GetUrl(zurl.GetUrl(loadDataFromWeb.WebRequest.HttpRequest.Url, xe.XPathValue("@href")));

                //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                //header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));

                //xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
                //header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                //// Aujourd'hui, 17:13
                //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);

                //xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //RapideDdl.SetTextValues(header, xe.DescendantTextList());

                //xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//a").Select(RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

                headers.Add(header);
            }
            data.PostHeaders = headers.ToArray();
            //return (IEnumDataPages_new2<int, IHeaderData_new>)data;
            return (IKeyData<int>)data;
        }
    }
}

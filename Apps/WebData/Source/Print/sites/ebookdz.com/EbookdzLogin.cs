using System.Net;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
using pb;
using pb.Text;
using System.Text;

namespace Download.Print.Ebookdz
{
    public static class EbookdzLogin
    {
        //private static string __urlMainPage = "http://www.ebookdz.com/";
        // modif le 07/04/2016
        private static string __urlMainPage = "http://www.ebookdz.com/index.php";
        private static bool __isLoggedIn = false;
        private static string __cookiesFile = null;
        private static CookieContainer __cookies = null;

        public static void Init(XElement xe)
        {
            InitCookies(xe);
        }

        private static void InitCookies(XElement xe)
        {
            if (__cookies == null)
            {
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
                    throw new PBException("unable login to http://www.ebookdz.com/index.php");
            }
            __isLoggedIn = true;
        }

        public static Http LoadMainPage()
        {
            return HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = __urlMainPage }, new HttpRequestParameters { Cookies = __cookies });
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
            XXElement xeForm = xeSource.XPathElement("//form[@id='navbar_loginform']");
            if (xeForm.XElement == null)
            {
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
            return http;
        }

        public static void SaveCookies(CookieContainer cookies)
        {
            __cookies = cookies;
            zcookies.SaveCookies(cookies, __urlMainPage, __cookiesFile);
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Cookies = __cookies };
        }
    }
}

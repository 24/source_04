using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using pb.Web.old;

namespace Download.Print.Ebookdz.old
{
    public static class Ebookdz_Exe
    {
        public static void Test_Md5_01(string text)
        {
            //string hash = GetMd5Hash(text);
            string hash = Crypt.ComputeMD5Hash(text).zToHex(lowercase: true);
            Trace.WriteLine("text     : \"{0}\"", text);
            Trace.WriteLine("MD5 hash : \"{0}\"", hash);
        }

        public static void Test_Md5_02()
        {
            XmlConfig localConfig = new XmlConfig(XmlConfig.CurrentConfig.GetExplicit("LocalConfig"));
            Test_Md5_01(localConfig.Get("DownloadAutomateManager/Ebookdz/Password"));
        }

        public static void Test_Login_02()
        {
            Http http = Ebookdz.LoadMainPage();
            XXElement xeSource = new XXElement(http.zGetXDocument().Root);
            Trace.WriteLine("Login        : \"{0}\"", Ebookdz.GetLogin(xeSource));
            bool isLoggedIn = Ebookdz.IsLoggedIn(xeSource);
            Trace.WriteLine("Is logged in : \"{0}\"", isLoggedIn);
            if (!isLoggedIn)
            {
                http = Ebookdz.Login(xeSource);
                Ebookdz.SaveCookies(http.RequestParameters.Cookies);

                http = Ebookdz.LoadMainPage();
                xeSource = new XXElement(http.zGetXDocument().Root);
                Trace.WriteLine("Login        : \"{0}\"", Ebookdz.GetLogin(xeSource));
                Trace.WriteLine("Is logged in : \"{0}\"", Ebookdz.IsLoggedIn(xeSource));
            }
        }

        public static void Test_Login_01(string url)
        {
            XmlConfig localConfig = new XmlConfig(XmlConfig.CurrentConfig.GetExplicit("LocalConfig"));
            string login = localConfig.GetExplicit("DownloadAutomateManager/Ebookdz/Login");
            string hashPassword = Crypt.ComputeMD5Hash(localConfig.GetExplicit("DownloadAutomateManager/Ebookdz/Password")).zToHex(lowercase: true);

            string urlSite = "http://www.ebookdz.com/";
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            pb.old.Http_v2.LoadUrl(urlSite, requestParameters);
            XXElement xeSource = new XXElement(pb.old.Http_v2.HtmlReader.XDocument.Root);
            Trace.WriteLine("Login        : \"{0}\"", Test_GetLogin_01(xeSource));
            Trace.WriteLine("Is logged in : \"{0}\"", Test_IsLoggedIn_01(xeSource));
            // <base href="http://www.ebookdz.com/forum/" />
            string urlBase = xeSource.XPathValue("//head//base/@href");
            //string urlBase = xeSource.XPathValue("//body//base/@href");
            Trace.WriteLine("urlBase : \"{0}\"", urlBase);
            XXElement xeForm = xeSource.XPathElement("//form[@id='navbar_loginform']");
            if (xeForm.XElement == null)
            {
                Trace.WriteLine("element not found \"//form[@id='navbar_loginform']\"");
                return;
            }
            Trace.WriteLine("form action : \"{0}\"", xeForm.XPathValue("@action"));
            string urlForm = zurl.GetUrl(urlBase, xeForm.XPathValue("@action"));
            string method = xeForm.XPathValue("@method");
            Trace.WriteLine("urlForm : \"{0}\" method {1}", urlForm, method);
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach(XXElement xeInput in xeForm.XPathElements(".//input"))
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
            Trace.WriteLine("content : \"{0}\"", content);

            requestParameters.content = content;
            requestParameters.method = Http.GetHttpRequestMethod(method);
            pb.old.Http_v2.LoadUrl(urlForm, requestParameters);

            //CookieCollection cookies = requestParameters.cookies.GetCookies(new Uri(urlSite));
            //Trace.WriteLine("cookies :");
            //Trace.WriteLine(cookies.zToJson());

            requestParameters.method = HttpRequestMethod.Get;
            requestParameters.content = null;
            pb.old.Http_v2.LoadUrl(url, requestParameters);
            xeSource = new XXElement(pb.old.Http_v2.HtmlReader.XDocument.Root);
            Trace.WriteLine("Login        : \"{0}\"", Test_GetLogin_01(xeSource));
            Trace.WriteLine("Is logged in : \"{0}\"", Test_IsLoggedIn_01(xeSource));

            string cookiesFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesDir"), "cookies.txt");
            Trace.WriteLine("save cookies to \"{0}\"", cookiesFile);
            //zfile.CreateFileDirectory(cookiesFile);
            //CookieCollection cookies = requestParameters.cookies.GetCookies(new Uri(urlSite));
            //cookies.zSave(cookiesFile);
            zcookies.SaveCookies(requestParameters.cookies, urlSite, cookiesFile);

            //cookies = requestParameters.cookies.GetCookies(new Uri(urlSite));
            //Trace.WriteLine("cookies :");
            //Trace.WriteLine(cookies.zToJson());
        }

        public static void Test_LoadWithCookies_01(string url)
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            string cookiesFile = XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesFile");
            requestParameters.cookies = zcookies.LoadCookies(cookiesFile);
            Trace.WriteLine("load cookies from \"{0}\"", cookiesFile);
            pb.old.Http_v2.LoadUrl(url, requestParameters);
            XXElement xeSource = new XXElement(pb.old.Http_v2.HtmlReader.XDocument.Root);
            Trace.WriteLine("Login        : \"{0}\"", Test_GetLogin_01(xeSource));
            Trace.WriteLine("Is logged in : \"{0}\"", Test_IsLoggedIn_01(xeSource));
        }

        public static string Test_GetLogin_01(XXElement xeSource)
        {
            // ebookdz.com_forum_showthread.php_t_109595_01_02.html :
            //   <div id="toplinks" class="toplinks">
            //   <li class="welcomelink">Bienvenue, <a href="member.php?u=49369"><b>la_beuze</b></a></li>
            return xeSource.XPathValue("//div[@id='toplinks']//li[@class='welcomelink']//a//text()");
        }

        public static bool Test_IsLoggedIn_01(XXElement xeSource)
        {
            return Test_GetLogin_01(xeSource) != null;
        }

        public static void Test_SaveCookie_01()
        {
            string urlSite = "http://www.ebookdz.com/";
            string cookiesFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesDir"), "cookies.txt");
            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("PHPSESSID", "o4penpsfd5kn5aiki6h7qins77", "/", "www.ebookdz.com"));
            cookies.Add(new Cookie("bb_sessionhash", "87d9411c00d27b16b835c4a402797fe3", "/", "www.ebookdz.com"));
            Trace.WriteLine("save cookies to \"{0}\"", cookiesFile);
            //zfile.CreateFileDirectory(cookiesFile);
            //CookieCollection cookies2 = cookies.GetCookies(new Uri(urlSite));
            //cookies2.zSave(cookiesFile);
            //GetCookies(cookies.GetCookies(new Uri(urlSite))).zSave(cookiesFile);
            zcookies.SaveCookies(cookies, urlSite, cookiesFile);
        }

        public static void Test_LoadCookie_01()
        {
            string urlSite = "http://www.ebookdz.com/";
            string cookiesFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("Ebookdz/CookiesDir"), "cookies.txt");
            Trace.WriteLine("load cookies from \"{0}\"", cookiesFile);
            //CookieContainer cookies = new CookieContainer();
            //foreach (Cookie cookie in zmongo.BsonReader<Cookie>(cookiesFile))
            //    cookies.Add(cookie);
            CookieContainer cookies = zcookies.LoadCookies(cookiesFile);
            Trace.WriteLine("cookies :");
            Trace.WriteLine(cookies.GetCookies(new Uri(urlSite)).zToJson());
        }
    }
}

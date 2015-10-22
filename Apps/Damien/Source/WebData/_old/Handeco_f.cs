using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PB_Util;
using Download.Handeco;
using pb.Web;
using pb.Web.old;

// doc
//   attention remplir obligatoirement Content-Type sinon aucun résultat
//     Content-Type: application/x-www-form-urlencoded


namespace Download.Damien
{
    static partial class w
    {
        public static void Test_Handeco_LoadHeaderPages_01(int nbPage = 1, bool loadLastPage = false)
        {
            Trace.WriteLine("Test_Handeco_Load_01");
            // www.handeco.org => 87.98.177.160
            // impossible d'aller directement à la page 2
            // les paramètres sont envoyés par un post qui renvoie la page 1
            // puis on fait un get pour les pages suivantes
            string url = "http://www.handeco.org/fournisseurs/rechercher";

            //Http2.LoadUrl(url);

            // tous les départements
            string content = "raisonSociale=&SIRET=&departements%5B%5D=67&departements%5B%5D=68&departements%5B%5D=24&departements%5B%5D=33&departements%5B%5D=40&departements%5B%5D=47&departements%5B%5D=64&departements%5B%5D=03&departements%5B%5D=15&departements%5B%5D=43&departements%5B%5D=63&departements%5B%5D=14&departements%5B%5D=50&departements%5B%5D=61&departements%5B%5D=21&departements%5B%5D=58&departements%5B%5D=71&departements%5B%5D=89&departements%5B%5D=22&departements%5B%5D=29&departements%5B%5D=35&departements%5B%5D=56&departements%5B%5D=18&departements%5B%5D=28&departements%5B%5D=36&departements%5B%5D=37&departements%5B%5D=41&departements%5B%5D=45&departements%5B%5D=08&departements%5B%5D=10&departements%5B%5D=51&departements%5B%5D=52&departements%5B%5D=2A&departements%5B%5D=2B&departements%5B%5D=25&departements%5B%5D=39&departements%5B%5D=70&departements%5B%5D=90&departements%5B%5D=27&departements%5B%5D=76&departements%5B%5D=75&departements%5B%5D=77&departements%5B%5D=78&departements%5B%5D=91&departements%5B%5D=92&departements%5B%5D=93&departements%5B%5D=94&departements%5B%5D=95&departements%5B%5D=11&departements%5B%5D=30&departements%5B%5D=34&departements%5B%5D=48&departements%5B%5D=66&departements%5B%5D=19&departements%5B%5D=23&departements%5B%5D=87&departements%5B%5D=54&departements%5B%5D=55&departements%5B%5D=57&departements%5B%5D=88&departements%5B%5D=09&departements%5B%5D=12&departements%5B%5D=31&departements%5B%5D=32&departements%5B%5D=46&departements%5B%5D=65&departements%5B%5D=81&departements%5B%5D=82&departements%5B%5D=59&departements%5B%5D=62&departements%5B%5D=44&departements%5B%5D=49&departements%5B%5D=53&departements%5B%5D=72&departements%5B%5D=85&departements%5B%5D=02&departements%5B%5D=60&departements%5B%5D=80&departements%5B%5D=16&departements%5B%5D=17&departements%5B%5D=79&departements%5B%5D=86&departements%5B%5D=04&departements%5B%5D=05&departements%5B%5D=06&departements%5B%5D=13&departements%5B%5D=83&departements%5B%5D=84&departements%5B%5D=01&departements%5B%5D=07&departements%5B%5D=26&departements%5B%5D=38&departements%5B%5D=42&departements%5B%5D=69&departements%5B%5D=73&departements%5B%5D=74&departements%5B%5D=971&departements%5B%5D=973&departements%5B%5D=972&departements%5B%5D=974&departements%5B%5D=988&departements%5B%5D=987&departements%5B%5D=975&departements%5B%5D=976&departements%5B%5D=986&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher";
            // Auvergne - 03 Allier
            //string content = "raisonSociale=&SIRET=&departements%5B%5D=03&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher";

            CookieContainer cookies = new CookieContainer();

            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.cookies = cookies;
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = content;
            //requestParameters.Expect100Continue = true;
            //HtmlXmlReader hxr = HtmlXmlReader.CurrentHtmlXmlReader;
            //hxr.LoadRepeatIfError = 1;
            //hxr.Load(url, requestParameters);
            Http_v3.LoadUrl(url, requestParameters);

            requestParameters = new HttpRequestParameters_v1();
            requestParameters.cookies = cookies;
            int page = 1;
            for (int i = 1; i < nbPage; i++)
            {
                // http://www.handeco.org/fournisseurs/rechercher/page/2
                url = "http://www.handeco.org/fournisseurs/rechercher/page/" + (++page).ToString();
                Http_v3.LoadUrl(url, requestParameters);
            }

            if (loadLastPage)
            {
                url = "http://www.handeco.org/fournisseurs/rechercher/page/200";
                Http_v3.LoadUrl(url, requestParameters);
            }
        }

        public static void Test_Handeco_Load_WebRequest_01()
        {
            string outputFile = @"test\handeco.html";

            string url = "http://www.handeco.org/fournisseurs/rechercher";

            string content = "raisonSociale=&SIRET=&departements%5B%5D=03&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher";
            //string cookies = "PHPSESSID=572851556c8b3b2ef74692a0b1de6675; __utma=140104362.2125206869.1396505609.1396505609.1396505609.1; __utmc=140104362; __utmz=140104362.1396505609.1.1.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)";
            string cookies = "PHPSESSID=572851556c8b3b2ef74692a0b1de6675; __utma=140104362.2125206869.1396505609.1396520128.1396528821.5; __utmb=140104362.1.10.1396528821; __utmc=140104362; __utmz=140104362.1396505609.1.1.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)";
            string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36 CoolNovo/2.0.9.20";

            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            System.Net.HttpWebRequest httpWebRequest = webRequest as System.Net.HttpWebRequest;
            httpWebRequest.ServicePoint.Expect100Continue = false;

            httpWebRequest.UserAgent = userAgent;
            //httpWebRequest.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            httpWebRequest.Referer = "http://www.handeco.org/fournisseurs/rechercher";
            //httpWebRequest.Headers.Add(_requestParameters.headers);
            System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
            cookieContainer.zAdd(url, cookies);
            httpWebRequest.CookieContainer = cookieContainer;
            // Content-Type: application/x-www-form-urlencoded
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            Encoding encoding = Encoding.Default;
            byte[] bytes = encoding.GetBytes(content);
            httpWebRequest.ContentLength = bytes.LongLength;
            System.IO.Stream stream = httpWebRequest.GetRequestStream();
            using (System.IO.BinaryWriter w = new System.IO.BinaryWriter(stream))
            {
                w.Write(bytes);
            }
            System.Net.WebResponse webResponse = webRequest.GetResponse();
            stream = webResponse.GetResponseStream();

            System.IO.StreamReader webStream = new System.IO.StreamReader(stream, encoding);
            string textResult = webStream.ReadToEnd();
            outputFile = GetPath(outputFile);
            zfile.WriteFile(outputFile, textResult);

            // Connection: keep-alive
            httpWebRequest.KeepAlive = true;

            // Cache-Control: max-age=0
            // Origin: http://www.handeco.org
            // Accept-Encoding: gzip,deflate,sdch
            // Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4

            //httpWebRequest.CachePolicy
            //httpWebRequest.Headers
            //httpWebRequest.Host
        }

        public static void Test_Handeco_Load_CompanyDetail_01()
        {
            string url = "http://www.handeco.org/fournisseur/consulter/id/1906";
            Http_v3.LoadUrl(url, new HttpRequestParameters_v1 { encoding = Encoding.UTF8 });
            //HtmlXmlReader.CurrentHtmlXmlReader.http.Close();
            //HtmlXmlReader.CurrentHtmlXmlReader.http.Dispose();
        }

        public static void Test_Handeco_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            Handeco.Handeco.Init();
            Handeco_LoadHeaderPages load = new Handeco_LoadHeaderPages(startPage, maxPage, reload, loadImage);
            //_rs.View(load);
            load.zView();
        }

        public static void Test_Handeco_LoadDetailCompany_01(string url, bool reload = false, bool loadImage = false)
        {
            Handeco.Handeco.Init();
            //_rs.View(Handeco_LoadDetailCompany.LoadCompany(url, reload, loadImage));
            Handeco_LoadDetailCompany.LoadCompany(url, reload, loadImage).zView();
        }
    }
}

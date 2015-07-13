using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

namespace Download.Print.ExtremeDown
{
    public class ExtremeProtect : ProtectLink
    {
        private static string __urlFormulaire = "http://extreme-protect.net/requis/captcha_formulaire.php";
        private static string __host = "http://extreme-protect.net/";

        public override string[] UnprotectLink(string protectLink)
        {
            string key = GetKey();

            //HttpRequestParameters requestParameters = new HttpRequestParameters();
            //requestParameters.accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //requestParameters.headers.Add("Accept-Language", "de, en-gb;q=0.9, en;q=0.8");
            //Http2.LoadUrl(protectLink, requestParameters);

            HttpRequestParameters requestParameters = new HttpRequestParameters();
            requestParameters.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            requestParameters.Headers.Add("Accept-Language", "de, en-gb;q=0.9, en;q=0.8");
            HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = protectLink }, requestParameters);

            //string content = "action=qaptcha&qaptcha_key=Wb6aEMQQ_xxQRMgkYX-XuWsdUyGHrQpZ";
            string content = "action=qaptcha&qaptcha_key=" + key;
            requestParameters.Accept = "application/json, text/javascript, */*; q=0.01";
            requestParameters.Headers.Add("X-Requested-With", "XMLHttpRequest");
            requestParameters.ContentType = "application/x-www-form-urlencoded";
            HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = __urlFormulaire, Method = HttpRequestMethod.Post, Referer = protectLink, Content = content }, requestParameters);

            //content = "Wb6aEMQQ_xxQRMgkYX-XuWsdUyGHrQpZ=&submit_captcha=VALIDER";
            content = key + "=&submit_captcha=VALIDER";
            requestParameters.Accept = "text/html, application/xhtml+xml, */*";
            requestParameters.Headers.Add("X-Requested-With", "XMLHttpRequest");
            requestParameters.ContentType = "application/x-www-form-urlencoded";
            Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = protectLink, Method = HttpRequestMethod.Post, Referer = __urlFormulaire, Content = content }, requestParameters);

            //XXElement xeSource = new XXElement(Http2.HtmlReader.XDocument.Root);
            //XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            XXElement xeSource = http.zGetXDocument().zXXElement();
            return xeSource.XPathValues(".//div[@class='all_liens']//a/@href").ToArray();
        }

        private static string __extremeProtectCharsKey = "azertyupqsdfghjkmwxcvbn23456789AZERTYUPQSDFGHJKMWXCVBN_-#@";
        public static string GetKey(int nb = 32)
        {
            // javascript function generatePass() :
            //   function generatePass(nb) {
            //     var chars = 'azertyupqsdfghjkmwxcvbn23456789AZERTYUPQSDFGHJKMWXCVBN_-#@';
            //     var pass = '';
            //     for(i=0;i<nb;i++){
            //       var wpos = Math.round(Math.random()*chars.length);
            //       pass += chars.substring(wpos,wpos+1);
            //     }
            //     return pass;
            //   }
            //string key = "";
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            int l = __extremeProtectCharsKey.Length;
            for (int i = 0; i < nb; i++)
            {
                sb.Append(__extremeProtectCharsKey[random.Next(l)]);
            }
            return sb.ToString();
        }

        //public static bool IsLinkExtremeProtect(string link)
        public override bool IsLinkProtected(string link)
        {
            // http://extreme-protect.net/AVYynltQQ
            return link.StartsWith(__host);
        }
    }

    public class ExtremeProtectTest : ProtectLink
    {
        private static string __host = "http://extreme-protect.net/";
        private Dictionary<string, string[]> _protectedLinks = new Dictionary<string, string[]>();

        public ExtremeProtectTest()
        {
            _protectedLinks.Add("http://extreme-protect.net/fWrwTk3G", new string[] { "http://ul.to/al6orfjh", "http://ul.to/dtssjju4" });
            _protectedLinks.Add("http://extreme-protect.net/VY5J0LF5", new string[] { "http://ul.to/7qtrjvic", "http://ul.to/3sk4i9tp" });
        }

        public override string[] UnprotectLink(string protectLink)
        {
            if (_protectedLinks.ContainsKey(protectLink))
                return _protectedLinks[protectLink];
            else
                return new string[0];
        }

        public override bool IsLinkProtected(string link)
        {
            // http://extreme-protect.net/AVYynltQQ
            return link.StartsWith(__host);
        }
    }
}

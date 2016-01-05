using System;
using System.Text;
using pb;
using pb.Web;
using pb.Web.old;

namespace Download.Print.ExtremeDown
{
    public static class ExtremeDown_Exe
    {
        //public static void Test_ExtremeProtect_01(string url)
        //{
        //    //string url = "http://extreme-protect.net/bihYv";   // Uploaded - Les Journaux (Lundi 08 Décembre 2014) - Pack1     http://ul.to/itupjmhz
        //    //string url = "http://extreme-protect.net/coNLua";  // Turbobit - Les Journaux (Lundi 08 Décembre 2014) - Pack1     http://turbobit.net/3r0d73yd9jdo.html
        //    // http://extreme-protect.net/hercules-2014-extended-2in1-multi-1080p-bluray-avc-dts-hdma-7-1-pch  Hercules.2014.EXTENDED.COMPLETE.BLURAY-PCH

        //    //string key = "Wb6aEMQQ_xxQRMgkYX-XuWsdUyGHrQpZ";
        //    string key = ExtremeProtect.GetKey();

        //    //HttpRequestParameters requestParameters2 = new HttpRequestParameters { encoding = Encoding.UTF8 };

        //    HttpRequestParameters requestParameters = new HttpRequestParameters();
        //    requestParameters.accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    requestParameters.headers.Add("Accept-Language", "de, en-gb;q=0.9, en;q=0.8");
        //    Http2.LoadUrl(url, requestParameters);

        //    string urlFormulaire = "http://extreme-protect.net/requis/captcha_formulaire.php";
        //    //string content = "action=qaptcha&qaptcha_key=Wb6aEMQQ_xxQRMgkYX-XuWsdUyGHrQpZ";
        //    string content = "action=qaptcha&qaptcha_key=" + key;
        //    requestParameters.method = HttpRequestMethod.Post;
        //    requestParameters.referer = url;
        //    requestParameters.accept = "application/json, text/javascript, */*; q=0.01";
        //    requestParameters.headers.Add("X-Requested-With", "XMLHttpRequest");
        //    requestParameters.contentType = "application/x-www-form-urlencoded";
        //    requestParameters.content = content;
        //    Http2.LoadUrl(urlFormulaire, requestParameters);

        //    //content = "Wb6aEMQQ_xxQRMgkYX-XuWsdUyGHrQpZ=&submit_captcha=VALIDER";
        //    content = key + "=&submit_captcha=VALIDER";
        //    requestParameters.method = HttpRequestMethod.Post;
        //    requestParameters.referer = urlFormulaire;
        //    requestParameters.accept = "text/html, application/xhtml+xml, */*";
        //    requestParameters.headers.Add("X-Requested-With", "XMLHttpRequest");
        //    requestParameters.contentType = "application/x-www-form-urlencoded";
        //    requestParameters.content = content;
        //    Http2.LoadUrl(url, requestParameters);
        //}

        public static void Test_ExtremeProtect_02(string url)
        {
            ExtremeProtect extremeProtect = new ExtremeProtect();
            Trace.WriteLine("extreme-protect.net get link from \"{0}\"", url);
            string[] links = extremeProtect.UnprotectLink(url);
            if (links.Length != 0)
            {
                Trace.WriteLine("links :");
                Trace.WriteLine(links.zToStringValues("\r\n"));
            }
            else
                Trace.WriteLine("link not found");
        }
    }
}

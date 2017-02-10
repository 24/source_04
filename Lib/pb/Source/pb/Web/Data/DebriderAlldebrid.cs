using pb.Web.Http;
using System.Collections.Generic;

namespace pb.Web.Data
{
    public class DebriderAlldebrid : Debrider
    {
        //private static bool __trace = false;
        private static string __url = "http://www.alldebrid.com/service.php?pseudo={0}&password={1}&link={2}&view=1";
        private HttpManager_v2 _httpManager = new HttpManager_v2();
        private string _login;
        private string _password;

        public DebriderAlldebrid(string login, string password)
        {
            if (login == null || login == "" || password == null || password == "")
                throw new PBException("error alldebrid missing login or password");
            _login = login;
            _password = password;
        }

        //public static bool Trace { get { return __trace; } set { __trace = value; } }

        //public override string DebridLink(string[] links)
        //{
        //    var q = (from l in links select new { link = l, rate = GetLinkRate(l) }).OrderBy(link => link.rate).Select(link => link.link);
        //    foreach (string link in q)
        //    {
        //        try
        //        {
        //            string downloadLink = DebridLink(link);
        //            if (downloadLink != null)
        //                return downloadLink;
        //        }
        //        catch(Exception ex)
        //        {
        //            Trace.WriteLine("error : {0}", ex.Message);
        //            Trace.WriteLine(ex.StackTrace);
        //        }
        //    }
        //    return null;
        //}

        public override string DebridLink(string link)
        {
            //request           : http://www.alldebrid.com/service.php?pseudo=&password=&link=http%3A%2F%2Fuptobox.com%2F0485eztzwh1y
            //response          : 0,;,<a class='link_dl' href='http://s10.alldebrid.com/dl/dg47yr48c6/Echos.12-08.pdf' style='a:visited { color = black }'>Echos.12-08.pdf</a>,;,0
            //                    1,;,http://www.oboom.com/36U5SZ6H : <span style='color:#a00;'>Invalid link</span>,;,0
            //request view=1    : http://www.alldebrid.com/service.php?pseudo=&password=&link=http%3A%2F%2Fuptobox.com%2F0485eztzwh1y&view=1
            //response          : http://s14.alldebrid.com/dl/dg46ejab1c/Echos.12-08.pdf
            //                    1,;,http://www.oboom.com/9XLPJDT2 : <span style='color:#a00;'>Invalid link</span>,;,0
            //request json=true : http://www.alldebrid.com/service.php?pseudo=&password=&link=http%3A%2F%2Fuptobox.com%2F0485eztzwh1y&json=true
            //response          : {"link":"http:\/\/s19.alldebrid.com\/dl\/dg4a1xda7b\/Echos.12-08.pdf","host":"uptobox","filename":"Echos.12-08.pdf","icon":"\/lib\/images\/hosts\/uptobox.png","streaming":[],"nb":0,"error":"","filesize":"15414067"}
            //                    {"link":"http:\/\/www.oboom.com\/36U5SZ6H","host":false,"filename":"","icon":"\/lib\/images\/hosts\/.png","streaming":[],"nb":0,"error":"This link isn't valid or not supported."}
            if (__trace)
                pb.Trace.Write("DebriderAlldebrid.DebridLink() : \"{0}\"", link);
            string url = string.Format(__url, _login, _password, link);
            //Http.Http http = HttpManager.CurrentHttpManager.Load(new HttpRequest { Url = url });
            HttpResult<string> httpResult = _httpManager.LoadText(new HttpRequest { Url = url });
            //string debridLink = http.ResultText;
            string debridLink = httpResult.Data;
            if (__trace)
                pb.Trace.WriteLine(" result \"{0}\"", debridLink);
            if (!debridLink.StartsWith("http://"))
                debridLink = null;
            return debridLink;
        }

        public override int GetLinkRate(string link)
        {
            return DefaultGetLinkRate(link);
        }

        public static int DefaultGetLinkRate(string link)
        {
            // http://ul.to/0cqaq9ou
            // http://uploaded.net/file/t40jl73t
            // http://uptobox.com/oiyprfxyfn1v
            // http://www.uploadable.ch/file/BgeEV6KxnCbB/VPFN118.rar
            // http://rapidgator.net/file/096cca55aba4d9e9dac902b9508a23b1/MiHN65.rar.html
            // http://turbobit.net/15cejdxrzleh.html
            // http://6i5mqc65bc.1fichier.com/
            //if (link.StartsWith("http://ul.to/"))
            //    return 10;
            //else if (link.StartsWith("http://uploaded.net/"))
            //    return 20;
            //else if (link.StartsWith("http://uptobox.com/"))
            //    return 30;
            //else if (link.StartsWith("http://www.uploadable.ch/"))
            //    return 40;
            //else if (link.StartsWith("http://rapidgator.net/"))
            //    return 50;
            //else if (link.StartsWith("http://turbobit.net/"))
            //    return 60;
            //else if (link.EndsWith(".1fichier.com/"))
            //    return 70;
            //else
            //    return 999;
            return DownloadFileServerInfo.GetLinkRate(DownloadFileServerInfo.GetServerNameFromLink(link));
        }
    }

    public class DebriderAlldebridTest : Debrider
    {
        private Dictionary<string, string> _debridedLinks = new Dictionary<string, string>();

        public DebriderAlldebridTest()
        {
            // Eric Clapton - The Platinum Collection
            _debridedLinks.Add("http://ul.to/al6orfjh", "http://s06.alldebrid.com/dl/f7lxd5539d/Ericclplflc14-ED.part2.rar");
            _debridedLinks.Add("http://ul.to/dtssjju4", "http://s10.alldebrid.com/dl/f7lxdca2d0/Ericclplflc14-ED.part1.rar");
            // Jeff Beck - Albums Collection: 9 Albums (10CD) (2013 - 2014)
            _debridedLinks.Add("http://ul.to/7qtrjvic", "http://s06.alldebrid.com/dl/f7lxdybc82/Jeffb9alb-ED.part2.rar");
            _debridedLinks.Add("http://ul.to/3sk4i9tp", "http://s05.alldebrid.com/dl/f7lxg3c08b/Jeffb9alb-ED.part1.rar");
        }

        public override string DebridLink(string link)
        {
            if (_debridedLinks.ContainsKey(link))
                return _debridedLinks[link];
            else
                //return null;
                return link;
        }

        public override int GetLinkRate(string link)
        {
            return DebriderAlldebrid.DefaultGetLinkRate(link);
        }
    }
}

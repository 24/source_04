using System;
using System.Text.RegularExpressions;

namespace pb.Web
{
    public class DownloadFileServerInfo
    {
        private static Regex __rgHost = new Regex(@"[^\.]+\.[^\.]+$", RegexOptions.Compiled);
        public static string GetServerNameFromLink(string link)
        {
            //if (link.StartsWith("http://ul.to/"))
            //    return "ul.to";
            //else if (link.StartsWith("http://uploaded.net/"))
            //    return "uploaded.net";
            //else if (link.StartsWith("http://uptobox.com/"))
            //    return "uptobox.com";
            //else if (link.StartsWith("http://www.uploadable.ch/"))
            //    return "uploadable.ch";
            //else if (link.StartsWith("http://rapidgator.net/"))
            //    return "rapidgator.net";
            //else if (link.StartsWith("http://turbobit.net/"))
            //    return "turbobit.net";
            //else if (link.EndsWith(".1fichier.com/"))
            //    return "1fichier.com";
            //else
            //    return "unknow";
            Uri uri = new Uri(link);
            Match match = __rgHost.Match(uri.Host);
            if (match.Success)
                return match.Value;
            else
                return "unknow";
        }

        public static int GetLinkRate(string server)
        {
            // http://ul.to/0cqaq9ou
            // http://uploaded.net/file/t40jl73t
            // http://uptobox.com/oiyprfxyfn1v
            // http://www.uploadable.ch/file/BgeEV6KxnCbB/VPFN118.rar
            // http://rapidgator.net/file/096cca55aba4d9e9dac902b9508a23b1/MiHN65.rar.html
            // http://turbobit.net/15cejdxrzleh.html
            // http://6i5mqc65bc.1fichier.com/
            // Uploaded, Turbobit, Uptobox, 1fichier, Letitbit
            if (server == null)
                return 999;
            switch (server.ToLower())
            {
                case "ul":
                case "ul.to":
                    return 10;
                case "uploaded":
                case "uploaded.net":
                    return 20;
                case "uptobox":
                case "uptobox.com":
                    return 30;
                case "uploadable":
                case "uploadable.ch":
                    return 40;
                case "rapidgator":
                case "rapidgator.net":
                    return 50;
                case "turbobit":
                case "turbobit.net":
                    return 60;
                case "1fichier":
                case "1fichier.com":
                    return 70;
                case "letitbit":
                case "letitbit.net":
                    return 80;
                default:
                    return 999;
            }
        }
    }
}

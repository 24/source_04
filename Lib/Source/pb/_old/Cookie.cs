using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace pb.old
{
    public static partial class GlobalExtension
    {
        private static Regex _cookieRegex = new Regex(@"([^\s=]+)=([^\s;]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void zAdd(this CookieContainer container, string url, params string[] cookies)
        {
            // cookies : "skimlinks_enabled=1; bb_lastvisit=1367138318; bb_lastactivity=0"
            if (container == null)
                return;
            Uri uri = new Uri(url);
            foreach (string cookie in cookies)
            {
                Match match = _cookieRegex.Match(cookie);
                while (match.Success)
                {
                    container.Add(uri, new Cookie(match.Groups[1].Value, match.Groups[2].Value));
                    match = match.NextMatch();
                }
            }
        }
    }
}

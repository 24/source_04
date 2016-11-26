using System;
using System.Collections.Generic;
using System.Net;
using pb.Data.Mongo;
using pb.IO;

namespace pb.Web
{
    public static class zcookies
    {
        public static CookieContainer LoadCookies(string file)
        {
            CookieContainer cookies = new CookieContainer();
            foreach (Cookie cookie in zmongo.BsonRead<Cookie>(file))
                cookies.Add(cookie);
            return cookies;
        }

        public static void SaveCookies(CookieContainer cookies, string url, string file)
        {
            zfile.CreateFileDirectory(file);
            GetCookies(cookies.GetCookies(new Uri(url))).zSave(file);
        }

        public static IEnumerable<Cookie> GetCookies(CookieCollection cookies)
        {
            foreach (Cookie cookie in cookies)
            {
                yield return cookie;
            }
        }
    }
}

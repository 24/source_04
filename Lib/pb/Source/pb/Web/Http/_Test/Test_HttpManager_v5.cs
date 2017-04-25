using System;
using System.Net;
using System.Threading.Tasks;

namespace pb.Web.Http.Test
{
    public static class Test_HttpManager_v5
    {
        public static async Task Test_LoadText_01(string url, string cacheDirectory = null)
        {
            await Test_LoadText_01(new HttpRequest_v5 { Url = url }, cacheDirectory);
        }

        public static async Task Test_LoadText_01(HttpRequest_v5 request, string cacheDirectory = null)
        {
            try
            {
                using (HttpManager_v5 httpManager = CreateHttpManager(cacheDirectory))
                    await LoadText(httpManager, request);
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        private static async Task LoadText(HttpManager_v5 httpManager, HttpRequest_v5 request)
        {
            httpManager.ActiveCookies();
            httpManager.SetDefaultHeaders();
            HttpResult_v5<string> httpResult = await httpManager.LoadText(request);
            Trace.WriteLine($"success {httpResult.Success} loadFromWeb {httpResult.LoadFromWeb} loadFromCache {httpResult.LoadFromCache}");

            Trace.WriteLine();
            TraceCookies(httpManager, request.Url);
        }

        public static void TraceCookies(HttpManager_v5 httpManager, string url)
        {
            if (httpManager.HttpClientHandler.CookieContainer == null)
            {
                Trace.WriteLine("cookies is not activated");
                return;
            }
            Trace.WriteLine("cookies :");
            Trace.WriteLine($"  count {httpManager.HttpClientHandler.CookieContainer.Count}");
            CookieCollection cookies = httpManager.HttpClientHandler.CookieContainer.GetCookies(new Uri(url));
            int i = 1;
            foreach (Cookie cookie in cookies)
                Trace.WriteLine($"  {i++,3} name {cookie.Name} path {cookie.Path} value {cookie.Value}");
        }

        public static HttpManager_v5 CreateHttpManager(string cacheDirectory = null)
        {
            HttpManager_v5 httpManager = new HttpManager_v5();
            //httpManager.TraceException = true;
            if (cacheDirectory != null)
            {
                UrlCache urlCache = new UrlCache(cacheDirectory);
                urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
                urlCache.IndexedFile = true;
                httpManager.SetCacheManager(urlCache);
            }
            return httpManager;
        }
    }
}

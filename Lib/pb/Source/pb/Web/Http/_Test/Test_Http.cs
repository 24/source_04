using System;

namespace pb.Web.Http.Test
{
    public static class Test_Http
    {
        public static void Test_LoadText(string url,  string cacheDirectory = null)
        {
            HttpManager_v2 httpManager = CreateHttpManager(cacheDirectory);
            HttpResult<string> result = httpManager.LoadText(new HttpRequest { Url = url });
            Trace.WriteLine($"Success {result.Success} LoadFromWeb {result.LoadFromWeb} LoadFromCache {result.LoadFromCache}");
            string resultText = result.Data;
            resultText = resultText.Substring(0, Math.Min(resultText.Length, 100));
            Trace.WriteLine(resultText + " ...");
        }

        public static HttpManager_v2 CreateHttpManager(string cacheDirectory = null)
        {
            //string directory = @"";
            HttpManager_v2 httpManager = new HttpManager_v2();
            if (cacheDirectory != null)
            {
                UrlCache urlCache = new UrlCache(cacheDirectory);
                urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
                urlCache.IndexedFile = false;
                urlCache.SaveRequest = true;
                httpManager.UrlCache = urlCache;
            }
            return httpManager;
        }
    }
}

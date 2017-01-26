using System.Drawing;

namespace pb.Web.Http
{
    partial class HttpManager_v2
    {
        public HttpResult<Image> LoadImage(HttpRequest httpRequest, string subDirectory = null)
        {
            return Load(httpRequest, http => http.LoadImage(), subDirectory);
        }

        //public HttpResult<Image> LoadImage_v1(HttpRequest httpRequest, string subDirectory = null)
        //{
        //    bool success = false;
        //    Http_v2 http = null;
        //    Image image = null;
        //    try
        //    {
        //        if (_urlCache != null)
        //        {
        //            httpRequest.UrlCachePath = LoadHttpToCache(httpRequest, subDirectory);
        //        }

        //        http = CreateHttp(httpRequest);

        //        Trace.WriteLine(1, "Load image \"{0}\" ({1})", httpRequest.UrlCachePath != null ? httpRequest.UrlCachePath.Path : httpRequest.Url, httpRequest.Method);
        //        image = http.LoadImage();
        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_traceException)
        //            Trace.CurrentTrace.WriteError(ex);
        //        else
        //            throw;
        //    }
        //    return new HttpResult<Image> { Success = success, Http = http, Data = image };
        //}
    }
}

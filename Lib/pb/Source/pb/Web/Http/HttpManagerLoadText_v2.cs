//Load("http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html");
//15/08/2013 12:00:32 Error : A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 5.199.168.178:80 (System.Net.Sockets.SocketException)
//Unable to connect to the remote server (System.Net.WebException)
//----------------------
//   at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
//   at System.Net.ServicePoint.ConnectSocketInternal(Boolean connectFailure, Socket s4, Socket s6, Socket& socket, IPAddress& address, ConnectSocketState state, IAsyncResult asyncResult, Int32 timeout, Exception& exception)
//----------------------
//   at System.Net.HttpWebRequest.GetResponse()
//   at pb.old.Http.OpenWebRequest() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 911
//   at pb.old.Http.Open() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 780
//   at pb.old.Http.Load() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 503
//   at pb.old.HtmlXmlReader.Load(String sUrl) in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\HtmlXmlReader.cs:line 426
//   at Print.download.w.Test_frboard_02()
//   at Print.download.w.Run()

namespace pb.Web
{
    partial class HttpManager_v2
    {
        public HttpResult<string> LoadText(HttpRequest httpRequest)
        {
            return Load(httpRequest, http => http.LoadText());
        }

        //public HttpResult<string> LoadText_v1(HttpRequest httpRequest)
        //{
        //    bool success = false;
        //    bool loadFromWeb = false;
        //    bool loadFromCache = false;
        //    bool trace = false;
        //    Http_v2 http = null;
        //    string text = null;
        //    try
        //    {
        //        if (_urlCache != null)
        //        {
        //            HttpResult<UrlCachePathResult> cachePathResult = LoadHttpToCache(httpRequest);
        //            httpRequest.UrlCachePath = cachePathResult.Data;
        //            if (cachePathResult.Http != null)
        //            {
        //                loadFromWeb = true;
        //                trace = true;
        //            }
        //            else
        //                loadFromCache = true;
        //        }
        //        else
        //            loadFromWeb = true;

        //        if (!trace)
        //            Trace.WriteLine(1, "Load \"{0}\" ({1})", httpRequest.UrlCachePath != null ? httpRequest.UrlCachePath.Path : httpRequest.Url, httpRequest.Method);

        //        http = CreateHttp(httpRequest);

        //        text = http.LoadText();
        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_traceException)
        //            Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} Error : loading \"{1}\" ({2}) {3}", DateTime.Now, httpRequest.UrlCachePath != null ? httpRequest.UrlCachePath.Path : httpRequest.Url, httpRequest.Method, ex.Message);
        //        else
        //            throw;
        //    }
        //    return new HttpResult<string> { Success = success, LoadFromWeb = loadFromWeb, LoadFromCache = loadFromCache, Http = http, Data = text };
        //}
    }
}

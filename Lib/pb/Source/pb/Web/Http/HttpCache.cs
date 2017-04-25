using pb.Data.Mongo;
using pb.IO;
using System.Text;
using System.Threading.Tasks;

namespace pb.Web.Http
{
    public partial class HttpCacheManager
    {
        // _httpManager use : DefaultEncoding in GetEncoding() and in Http_v3
        private HttpManager_v5 _httpManager = null;
        private UrlCache _urlCache = null;

        //public HttpManager_v3 HttpManager { get { return _httpManager; } set { _httpManager = value; } }
        //public UrlCache UrlCache { get { return _urlCache; } set { _urlCache = value; } }

        public HttpCacheManager(HttpManager_v5 httpManager, UrlCache urlCache)
        {
            _httpManager = httpManager;
            _urlCache = urlCache;
        }

        public async Task<HttpResult_v5<string>> LoadText(HttpRequest_v5 httpRequest)
        {
            return await new HttpCache(this, httpRequest).LoadText();
        }
    }

    public partial class HttpCacheManager
    {
        public class HttpCache
        {
            private HttpCacheManager _httpCacheManager = null;
            private HttpRequest_v5 _request = null;
            private bool _success = false;
            private bool _loadFromWeb = false;
            private bool _loadFromCache = false;
            private UrlCachePathResult _urlCachePath = null;
            private HttpMessageResult _httpMessageResult = null;

            //public HttpCacheManager HttpCacheManager { get { return _httpCacheManager; } set { _httpCacheManager = value; } }
            public HttpCache(HttpCacheManager httpCacheManager, HttpRequest_v5 request)
            {
                _httpCacheManager = httpCacheManager;
                _request = request;
            }

            public async Task<HttpResult_v5<string>> LoadText()
            {
                //bool success = false;
                string text = null;
                //if (await Load())
                //{
                    await Load();
                    text = await zfile.ReadAllTextAsync(_urlCachePath.Path, GetTextEncoding());
                    //success = true;
                //}
                
                return new HttpResult_v5<string>(_success, _httpMessageResult.Response.StatusCode, _httpMessageResult, _loadFromWeb, _loadFromCache) { Data = text };
            }

            //private async Task<bool> Load()
            private async Task Load()
            {
                _urlCachePath = _httpCacheManager._urlCache.GetUrlPathResult(_request, _request.CacheSubDirectory);
                string cacheFile = _urlCachePath.Path;
                if (_request.ReloadFromWeb || !zFile.Exists(cacheFile))
                {
                    // cache file dont exists, create 2 cache files : file with content response, file with request response
                    //TraceLevel.WriteLine(1, $"Load from web \"{_httpRequest.Url}\" ({_httpRequest.Method})");
                    //Http_v3 http2 = CreateHttp(httpRequest);
                    //http2.LoadToFile(urlPath);
                    //http2.SaveRequest(zpath.PathSetExtension(urlPath, ".request.json"));
                    using (Http_v5 http = new Http_v5(_request, _httpCacheManager._httpManager))
                    {
                        zfile.CreateFileDirectory(cacheFile);
                        HttpResult_v5 result = await http.LoadToFile(cacheFile);
                        //if (result.Success)
                        //{
                            _httpMessageResult = http.GetHttpMessageResult();
                            _httpMessageResult.CacheFile = _urlCachePath.SubPath;
                            _httpMessageResult.zSave(zpath.PathSetExtension(cacheFile, ".request.json"), jsonIndent: true);
                            _success = result.Success;
                            _loadFromWeb = true;
                            //return true;
                        //}
                        //else
                        //    return false;
                    }
                    //trace = true;
                }
                else
                {
                    // cache file exists, load request from cache file
                    _loadFromCache = true;
                    TraceLevel.WriteLine(1, "Load from cache \"{0}\"", _urlCachePath.Path);
                    string requestFile = zpath.PathSetExtension(cacheFile, ".request.json");
                    if (!zFile.Exists(requestFile))
                        throw new PBException($"request file not found \"{requestFile}\"");
                    _httpMessageResult = zMongo.ReadFileAs<HttpMessageResult>(requestFile);
                    _success = _httpMessageResult.Response.StatusCode == 200;
                    //return true;
                }
            }

            private Encoding GetTextEncoding()
            {
                if (_httpMessageResult.Response.CharSet != null)
                    return Encoding.GetEncoding(_httpMessageResult.Response.CharSet);
                else
                    return _httpCacheManager._httpManager.DefaultEncoding;
            }
        }
    }
}

// from HttpContent.cs https://github.com/dotnet/corefx (c:\pib\dev\OpenProject\microsoft\corefx\src\System.Net.Http\src\System\Net\Http)
//internal static string ReadBufferAsString(ArraySegment<byte> buffer, HttpContentHeaders headers)
//{
//    // We don't validate the Content-Encoding header: If the content was encoded, it's the caller's 
//    // responsibility to make sure to only call ReadAsString() on already decoded content. E.g. if the 
//    // Content-Encoding is 'gzip' the user should set HttpClientHandler.AutomaticDecompression to get a 
//    // decoded response stream.

//    Encoding encoding = null;
//    int bomLength = -1;

//    // If we do have encoding information in the 'Content-Type' header, use that information to convert
//    // the content to a string.
//    if ((headers.ContentType != null) && (headers.ContentType.CharSet != null))
//    {
//        try
//        {
//            encoding = Encoding.GetEncoding(headers.ContentType.CharSet);

//            // Byte-order-mark (BOM) characters may be present even if a charset was specified.
//            bomLength = GetPreambleLength(buffer, encoding);
//        }
//        catch (ArgumentException e)
//        {
//            throw new InvalidOperationException(SR.net_http_content_invalid_charset, e);
//        }
//    }

//    // If no content encoding is listed in the ContentType HTTP header, or no Content-Type header present, 
//    // then check for a BOM in the data to figure out the encoding.
//    if (encoding == null)
//    {
//        if (!TryDetectEncoding(buffer, out encoding, out bomLength))
//        {
//            // Use the default encoding (UTF8) if we couldn't detect one.
//            encoding = DefaultStringEncoding;

//            // We already checked to see if the data had a UTF8 BOM in TryDetectEncoding
//            // and DefaultStringEncoding is UTF8, so the bomLength is 0.
//            bomLength = 0;
//        }
//    }

//    // Drop the BOM when decoding the data.
//    return encoding.GetString(buffer.Array, buffer.Offset + bomLength, buffer.Count - bomLength);
//}


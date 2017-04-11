using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

// List of HTTP header fields https://en.wikipedia.org/wiki/List_of_HTTP_header_fields

namespace pb.Web.Http
{
    public static class HttpTools
    {
        public static string GetFileExtensionFromContentType(string contentType)
        {
            contentType = contentType.ToLower();
            // text/html
            if (contentType.EndsWith("/html"))
                return ".html";
            // text/xml application/xml
            else if (contentType.EndsWith("/xml"))
                return ".xml";
            else
                return ".txt";
        }

        public static string GetContentTypeFromFileExtension(string ext)
        {
            //string contentType
            switch (ext.ToLower())
            {
                case ".xml":
                    return "text/xml";
                case ".htm":
                case ".html":
                case ".asp":
                case ".php":
                    return "text/html";
                case ".txt":
                    return "text/txt";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".tiff":
                    return "image/tiff";
                case ".bmp":
                    return "image/bmp";
                default:
                    return null;
                    //default:
                    //    if (ext.Length > 1)
                    //        _resultContentType = "/" + ext.Substring(1);
                    //    break;
            }
        }

        public static HttpRequestMethod GetHttpRequestMethod(string method)
        {
            switch (method.ToLower())
            {
                case "get":
                    return HttpRequestMethod.Get;
                case "post":
                    return HttpRequestMethod.Post;
                default:
                    throw new PBException($"unknow HttpRequestMethod \"{method}\"");
            }
        }

        public static HttpMethod GetHttpMethod(HttpRequestMethod method)
        {
            switch (method)
            {
                case HttpRequestMethod.Get:
                    return HttpMethod.Get;
                case HttpRequestMethod.Post:
                    return HttpMethod.Post;
                default:
                    throw new PBException($"unknow HttpRequestMethod {method}");
            }
        }

        public static Dictionary<string, string> GetRequestHeaders(System.Net.Http.HttpRequestMessage request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            // request headers
            HttpRequestHeaders requestHeaders = request.Headers;
            if (requestHeaders.Accept.Count > 0)
                headers.Add("accept", requestHeaders.Accept.ToString());
            if (requestHeaders.AcceptCharset.Count > 0)
                headers.Add("accept-charset", requestHeaders.AcceptCharset.ToString());
            if (requestHeaders.AcceptEncoding.Count > 0)
                headers.Add("accept-encoding", requestHeaders.AcceptEncoding.ToString());
            if (requestHeaders.AcceptLanguage.Count > 0)
                headers.Add("accept-language", requestHeaders.AcceptLanguage.ToString());
            if (requestHeaders.Authorization != null)
                headers.Add("authorization", requestHeaders.Authorization.ToString());
            if (requestHeaders.CacheControl != null)
                headers.Add("cache-control", requestHeaders.CacheControl.ToString());
            if (requestHeaders.Connection.Count > 0)
                headers.Add("connection", requestHeaders.Connection.ToString());
            if (requestHeaders.Date != null)
                headers.Add("date", requestHeaders.Date.ToString());
            // ExpectContinue ??
            if (requestHeaders.Expect.Count > 0)
                headers.Add("expect", requestHeaders.Expect.ToString());
            if (requestHeaders.From != null)
                headers.Add("from", requestHeaders.From);
            if (requestHeaders.Host != null)
                headers.Add("host", requestHeaders.Host);
            if (requestHeaders.IfMatch.Count > 0)
                headers.Add("if-match", requestHeaders.IfMatch.ToString());
            if (requestHeaders.IfModifiedSince != null)
                headers.Add("if-modified-since", requestHeaders.IfModifiedSince.ToString());
            if (requestHeaders.IfNoneMatch.Count > 0)
                headers.Add("if-none-match", requestHeaders.IfNoneMatch.ToString());
            if (requestHeaders.IfRange != null)
                headers.Add("if-range", requestHeaders.IfRange.ToString());
            if (requestHeaders.IfUnmodifiedSince != null)
                headers.Add("if-unmodified-since", requestHeaders.IfUnmodifiedSince.ToString());
            if (requestHeaders.MaxForwards != null)
                headers.Add("max-forwards", requestHeaders.MaxForwards.ToString());
            if (requestHeaders.Pragma.Count > 0)
                headers.Add("pragma", requestHeaders.Pragma.ToString());
            if (requestHeaders.ProxyAuthorization != null)
                headers.Add("proxy-authorization", requestHeaders.ProxyAuthorization.ToString());
            if (requestHeaders.Range != null)
                headers.Add("range", requestHeaders.Range.ToString());
            if (requestHeaders.Referrer != null)
                headers.Add("referrer", requestHeaders.Referrer.ToString());
            if (requestHeaders.TE.Count > 0)
                headers.Add("te", requestHeaders.TE.ToString());
            // Trailer ??
            if (requestHeaders.Trailer.Count > 0)
                headers.Add("trailer", requestHeaders.Trailer.ToString());
            // TransferEncoding ??
            if (requestHeaders.TransferEncoding.Count > 0)
                headers.Add("transfer-encoding", requestHeaders.TransferEncoding.ToString());
            // TransferEncodingChunked ??
            if (requestHeaders.TransferEncodingChunked != null)
                headers.Add("transfer-encoding-chunked", requestHeaders.TransferEncodingChunked.ToString());
            if (requestHeaders.Upgrade.Count > 0)
                headers.Add("upgrade", requestHeaders.Upgrade.ToString());
            if (requestHeaders.UserAgent.Count > 0)
                headers.Add("user-agent", requestHeaders.UserAgent.ToString());
            if (requestHeaders.Via.Count > 0)
                headers.Add("via", requestHeaders.Via.ToString());
            if (requestHeaders.Warning.Count > 0)
                headers.Add("warning", requestHeaders.Warning.ToString());

            // request content headers
            if (request.Content != null)
                AddContentHeaders(headers, request.Content.Headers);

            return headers;
        }

        public static Dictionary<string, string> GetResponseHeaders(System.Net.Http.HttpResponseMessage response)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            // request headers
            HttpResponseHeaders responseHeaders = response.Headers;
            if (responseHeaders.AcceptRanges.Count > 0)
                headers.Add("accept-ranges", responseHeaders.AcceptRanges.ToString());
            if (responseHeaders.Age != null)
                headers.Add("age", responseHeaders.Age.ToString());
            if (responseHeaders.CacheControl != null)
                headers.Add("cache-control", responseHeaders.CacheControl.ToString());
            if (responseHeaders.Connection.Count > 0)
                headers.Add("connection", responseHeaders.Connection.ToString());
            if (responseHeaders.Date != null)
                headers.Add("date", responseHeaders.Date.ToString());
            if (responseHeaders.ETag != null)
                headers.Add("etag", responseHeaders.ETag.ToString());
            if (responseHeaders.Location != null)
                headers.Add("location", responseHeaders.Location.ToString());
            if (responseHeaders.Pragma.Count > 0)
                headers.Add("pragma", responseHeaders.Pragma.ToString());
            if (responseHeaders.ProxyAuthenticate.Count > 0)
                headers.Add("proxy-authenticate", responseHeaders.ProxyAuthenticate.ToString());
            if (responseHeaders.RetryAfter != null)
                headers.Add("retry-after", responseHeaders.RetryAfter.ToString());
            if (responseHeaders.Server.Count > 0)
                headers.Add("server", responseHeaders.Server.ToString());
            if (responseHeaders.Trailer.Count > 0)
                headers.Add("trailer", responseHeaders.Trailer.ToString());
            //string s = null;
            //if (responseHeaders.TransferEncoding.Count > 0)
            //    s = responseHeaders.TransferEncoding.ToString();
            //if (responseHeaders.TransferEncodingChunked == true)
            //{
            //    if (s == null)
            //        s = "chunked";
            //    else
            //        s += ";chunked";
            //}
            //if (s != null)
            //    headers.Add("transfer-encoding", s);
            if (responseHeaders.TransferEncoding.Count > 0)
                headers.Add("transfer-encoding", responseHeaders.TransferEncoding.ToString());
            if (responseHeaders.Upgrade.Count > 0)
                headers.Add("upgrade", responseHeaders.Upgrade.ToString());
            if (responseHeaders.Vary.Count > 0)
                headers.Add("vary", responseHeaders.Vary.ToString());
            if (responseHeaders.Via.Count > 0)
                headers.Add("via", responseHeaders.Via.ToString());
            if (responseHeaders.Warning.Count > 0)
                headers.Add("warning", responseHeaders.Warning.ToString());
            if (responseHeaders.WwwAuthenticate.Count > 0)
                headers.Add("www-authenticate", responseHeaders.WwwAuthenticate.ToString());

            // response content headers
            AddContentHeaders(headers, response.Content.Headers);

            return headers;
        }

        public static void AddContentHeaders(Dictionary<string, string> headers, HttpContentHeaders contentHeaders)
        {
            // Allow ??
            if (contentHeaders.Allow.Count > 0)
                headers.Add("allow", contentHeaders.Allow.zToStringValues());
            // ContentDisposition ??
            if (contentHeaders.ContentDisposition != null)
                headers.Add("content-disposition", contentHeaders.ContentDisposition.ToString());
            // ContentEncoding ??
            if (contentHeaders.ContentEncoding.Count > 0)
                headers.Add("content-encoding", contentHeaders.ContentEncoding.zToStringValues());
            // ContentLanguage ??
            if (contentHeaders.ContentLanguage.Count > 0)
                headers.Add("content-language", contentHeaders.ContentLanguage.zToStringValues());
            if (contentHeaders.ContentLength != null)
                headers.Add("content-length", contentHeaders.ContentLength.ToString());
            // ContentLocation ??
            if (contentHeaders.ContentLocation != null)
                headers.Add("content-location", contentHeaders.ContentLocation.ToString());
            //if (contentHeaders.ContentMD5 != null)
            //    headers.Add("content-md5", contentHeaders.ContentMD5.ToString());
            // ContentRange ??
            if (contentHeaders.ContentRange != null)
                headers.Add("content-range", contentHeaders.ContentRange.ToString());
            if (contentHeaders.ContentType != null)
                headers.Add("content-type", contentHeaders.ContentType.ToString());
            // Expires ??
            if (contentHeaders.Expires != null)
                headers.Add("expires", contentHeaders.Expires.ToString());
            // LastModified ??
            if (contentHeaders.LastModified != null)
                headers.Add("lastmodified", contentHeaders.LastModified.ToString());
        }
    }

    public static partial class HttpExtension
    {
        public static void zSet<T>(this HttpHeaderValueCollection<T> header, string value) where T : class
        {
            header.Clear();
            header.ParseAdd(value);
        }
    }
}

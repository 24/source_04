using System;

namespace pb.Web
{
    public enum HttpRequestMethod
    {
        Get = 0,
        Post
    }

    public class HttpRequest
    {
        public string Url;
        public HttpRequestMethod Method = HttpRequestMethod.Get;
        public string Referer = null;
        public string Content = null;
    }
}

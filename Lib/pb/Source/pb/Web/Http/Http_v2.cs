namespace pb.Web.Http
{
    public partial class Http_v2 : HttpBase
    {
        // bool cacheFile = false
        public Http_v2(HttpRequest httpRequest, HttpRequestParameters requestParameters = null) : base(httpRequest, requestParameters)
        {
        }

        public Http_v2(HttpLog httpLog) : base(httpLog)
        {
        }
    }
}

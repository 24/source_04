using System;
using System.Text;
using pb.Web;

namespace hts.WebData
{
    public static class Handeco
    {
        private static Func<string, string> __trim = text => text != null ? text.Trim() : null;
        private static HttpRequestParameters __httpRequestParameters = new HttpRequestParameters { Encoding = Encoding.UTF8 };

        public static Func<string, string> Trim { get { return __trim; } }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return __httpRequestParameters;
        }
    }
}

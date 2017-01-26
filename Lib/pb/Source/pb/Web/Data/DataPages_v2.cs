using pb.Web.Http;
using System;
using System.Collections.Generic;

namespace pb.Web.Data
{
    public class DataPages_v2<TData> : IEnumDataPages<TData>
    {
        public HttpRequest SourceHttpRequest;
        public DateTime LoadFromWebDate;

        public TData[] Data;
        public HttpRequest HttpRequestNextPage;

        // IEnumDataPages<TData>
        public IEnumerable<TData> GetDataList()
        {
            return Data;
        }

        // IEnumDataPages<TData>
        public HttpRequest GetHttpRequestNextPage()
        {
            return HttpRequestNextPage;
        }
    }
}

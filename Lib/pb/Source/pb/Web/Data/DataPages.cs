using System;
using System.Collections.Generic;

namespace pb.Web.Data
{
    public class DataPages<TData> : IEnumDataPages<TData>
    {
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public TData[] Data;
        public string UrlNextPage;

        public IEnumerable<TData> GetDataList()
        {
            return Data;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }
}

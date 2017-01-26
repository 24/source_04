using MongoDB.Bson;
using pb.Data;
using pb.Web;
using pb.Web.Data;
using System;
using System.Collections.Generic;

namespace Download.Print
{
    public class PostHeader : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string UrlDetail;

        public WebImage[] Images;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class PostHeaderDataPage<TDataPage> : IEnumDataPages<TDataPage>, IKeyData where TDataPage : IHeaderData
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public TDataPage[] Headers;
        public string UrlNextPage;

        // IKeyData
        public BsonValue GetKey()
        {
            return Id;
        }

        //public IEnumerable<IHeaderData> GetDataList()
        public IEnumerable<TDataPage> GetDataList()
        {
            return Headers;
        }

        // IEnumDataPages<TData>
        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }
}

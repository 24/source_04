using pb.Web.Data.Mongo;
using System;

namespace TM
{
    public class TMServerHeader //: IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        //public string UrlDetail;

        //public WebImage[] Images;

        //public HttpRequest GetHttpRequestDetail()
        //{
        //    return new HttpRequest { Url = UrlDetail };
        //}
    }

    public class TMServers : WebHeaderMongoManagerBase<TMServerHeader>
    {
    }
}

using System;
using pb.Web.Http;

namespace pb.Web.Data
{
    // used by :
    //   - WebLoadManager
    //   - WebLoadDataManager<TData>
    //   - WebData<TData>
    public class WebResult
    {
        public WebRequest WebRequest;
        public Http.Http Http;
        public bool LoadResult = false;
        public DateTime LoadFromWebDate;
        public UrlCachePathResult UrlCachePathResult;
    }
}

using MongoDB.Bson;
using pb.Web.Http;
using System.Collections.Generic;

namespace pb.Web.Data
{
    // TSource used in request (WebRequest_v2<TSource>)
    // TData used for data
    //public class WebData_v2<THeader, TDetail>
    public class WebData_v2<TSource, TData>
    {
        //private static string _defaultHttpResultName = "default";
        private WebRequest_v2<TSource> _request;
        private BsonValue _id = null;
        private BsonValue _key = null;
        private bool _dataLoaded;
        private bool _dataLoadedFromWeb;
        private bool _dataLoadedFromStore;
        private TData _data;
        private Dictionary<string, HttpResult<string>> _httpResults = new Dictionary<string, HttpResult<string>>();

        public WebData_v2(WebRequest_v2<TSource> request)
        {
            _request = request;
        }

        public WebRequest_v2<TSource> Request { get { return _request; } }
        public BsonValue Id { get { return _id; } set { _id = value; } }
        public BsonValue Key { get { return _key; } set { _key = value; } }
        public bool DataLoaded { get { return _dataLoaded; } set { _dataLoaded = value; } }
        public bool DataLoadedFromWeb { get { return _dataLoadedFromWeb; } set { _dataLoadedFromWeb = value; } }
        public bool DataLoadedFromStore { get { return _dataLoadedFromStore; } set { _dataLoadedFromStore = value; } }
        public TData Data { get { return _data; } set { _data = value; } }
        public Dictionary<string, HttpResult<string>> HttpResults { get { return _httpResults; } }
    }
}

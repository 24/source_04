using MongoDB.Bson;

namespace pb.Web.Data
{
    public interface ILoadDocument<TData>
    {
        TData Document { get; set; }
        bool DocumentLoaded { get; set; }
    }

    public partial class WebData<TData> : ILoadDocument<TData>
    {
        private WebRequest _request;
        private WebResult _result;
        private HttpResult<string> _result_v2;
        //private bool _error;
        private TData _document;
        private bool _documentLoaded;
        private bool _documentLoadedFromWeb;
        private bool _documentLoadedFromStore;
        private BsonValue _id = null;
        private BsonValue _key = null;

        public WebData(WebRequest request)
        {
            _request = request;
        }

        public WebRequest Request { get { return _request; } }
        public WebResult Result { get { return _result; } set { _result = value; } }
        public HttpResult<string> Result_v2 { get { return _result_v2; } }
        //public bool Error { get { return _error; } }
        public TData Document { get { return _document; } set { _document = value; } }
        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
        public bool DocumentLoadedFromWeb { get { return _documentLoadedFromWeb; } set { _documentLoadedFromWeb = value; } }
        public bool DocumentLoadedFromStore { get { return _documentLoadedFromStore; } set { _documentLoadedFromStore = value; } }
    }
}

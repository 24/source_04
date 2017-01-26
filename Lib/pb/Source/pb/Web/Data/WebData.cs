using MongoDB.Bson;

// todo :
//   - mettre WebData<TData>._result dans un autre source il est utilisé par WebDataManager<TData>

namespace pb.Web.Data
{
    public interface ILoadData<TData>
    {
        TData Data { get; set; }
        bool DataLoaded { get; set; }
    }

    public partial class WebData<TData> : ILoadData<TData>
    {
        private WebRequest _request;
        //private bool _error;
        private TData _data;
        private bool _dataLoaded;
        private bool _dataLoadedFromWeb;
        private bool _dataLoadedFromStore;
        private BsonValue _id = null;
        private BsonValue _key = null;

        public WebData(WebRequest request)
        {
            _request = request;
        }

        public WebRequest Request { get { return _request; } }
        //public bool Error { get { return _error; } }
        public TData Data { get { return _data; } set { _data = value; } }
        public bool DataLoaded { get { return _dataLoaded; } set { _dataLoaded = value; } }
        public bool DataLoadedFromWeb { get { return _dataLoadedFromWeb; } set { _dataLoadedFromWeb = value; } }
        public bool DataLoadedFromStore { get { return _dataLoadedFromStore; } set { _dataLoadedFromStore = value; } }
    }
}

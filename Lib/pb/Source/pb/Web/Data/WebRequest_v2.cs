namespace pb.Web.Data
{
    // from WebRequest : able to load detail from 2 or more url
    // used in WebDataManager_v5<THeader, TDetail>
    public class WebRequest_v2<TData>
    {
        private TData _data;
        private bool _reloadFromWeb = false;
        private bool _refreshDocumentStore = false;
        private WebImageRequest _imageRequest = null;

        public TData Data { get { return _data; } set { _data = value; } }
        public bool ReloadFromWeb { get { return _reloadFromWeb; } set { _reloadFromWeb = value; } }
        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } set { _refreshDocumentStore = value; } }
        public WebImageRequest ImageRequest { get { return _imageRequest; } set { _imageRequest = value; } }
    }
}

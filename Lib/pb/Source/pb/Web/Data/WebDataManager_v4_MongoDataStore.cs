using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Web.Http;
using System.Collections.Generic;
using System.Linq;

namespace pb.Web.Data
{
    // web request info :
    //   System.Net.WebRequest  -> HttpRequestLog
    //   System.Net.WebResponse -> HttpResponseLog



    //public class HttpRequest
    //{
    //    public string Url;
    //    public HttpRequestMethod Method = HttpRequestMethod.Get;
    //    public string Referer = null;
    //    public string Content = null;
    //}

    //public class WebRequest
    //{
    //    private HttpRequest _httpRequest = null;
    //    private bool _reloadFromWeb = false;
    //    private bool _refreshDocumentStore = false;
    //    private WebImageRequest _imageRequest = null;

    //    public HttpRequest HttpRequest { get { return _httpRequest; } set { _httpRequest = value; } }
    //    public bool ReloadFromWeb { get { return _reloadFromWeb; } set { _reloadFromWeb = value; } }
    //    public WebImageRequest ImageRequest { get { return _imageRequest; } set { _imageRequest = value; } }
    //    public bool RefreshDocumentStore { get { return _refreshDocumentStore; } set { _refreshDocumentStore = value; } }
    //}

    //public class WebResult
    //{
    //    public WebRequest WebRequest;
    //    public Http Http;
    //    public bool LoadResult = false;
    // *  public DateTime LoadFromWebDate;
    // *  public UrlCachePathResult UrlCachePathResult;
    //}

    //public class UrlCachePathResult
    //{
    //    public string Path;
    //    public string SubPath;
    //}

    //public class Http : IDisposable
    //{
    //    private static bool __trace = false;
    //    // http request
    //    private HttpRequest _httpRequest = null;
    //    private HttpRequestParameters _requestParameters = null;
    //    // parameters
    //    private int _loadRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout
    //    // work variables
    //    private Progress _progress = null;
    // *  private WebRequest _webRequest = null;
    // *  private WebResponse _webResponse = null;
    //    private Stream _stream = null;
    //    private StreamReader _streamReader = null;
    //    private string _resultCharset = null;
    //    private string _resultContentType = null;
    //    private long _resultContentLength = -1;
    //    private string _resultText = null;
    //    private string _exportFile = null;
    //    private bool _exportRequest = true;
    //    private bool _setExportFileExtension = false;

    partial class WebDataManager_v4<TData>
    {
        protected MongoDataStore _dataStore = null;
        protected MongoDataSerializer<TData> _dataSerializer = null;
        protected MongoDataSerializer<HttpLog> _webRequestSerializer = null;

        public MongoDataStore DataStore { get { return _dataStore; } set { _dataStore = value; } }
        public MongoDataSerializer<TData> DataSerializer { get { return _dataSerializer; } set { _dataSerializer = value; } }
        public MongoDataSerializer<HttpLog> WebRequestSerializer { get { return _webRequestSerializer; } }

        protected void InitSerializer()
        {
            _webRequestSerializer = new MongoDataSerializer<HttpLog>();
            _webRequestSerializer.ItemName = "WebRequest";
        }

        public TData LoadFromId(BsonValue id)
        {
            return _dataSerializer.Deserialize(_dataStore.LoadFromId(id));
        }

        public IEnumerable<TData> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            if (loadImage && _webLoadImageManager == null)
                throw new PBException("web load image manager is not defined");
            //return _dataStore.Find(query, sort: sort, limit: limit).zAction(
            //    data =>
            //    {
            //        if (loadImage)
            //        {
            //            _webLoadImageManager.LoadImagesToData(data);
            //        }
            //    });
            IEnumerable<TData> result = _dataStore.Find(query, sort: sort, limit: limit).Select(document => _dataSerializer.Deserialize(document));
            if (loadImage)
                result = result.zAction(data => _webLoadImageManager.LoadImagesToData(data));
            return result;
        }
    }
}

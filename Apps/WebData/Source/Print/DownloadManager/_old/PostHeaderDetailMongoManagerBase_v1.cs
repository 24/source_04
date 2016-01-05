using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using Print;
using pb.Web.Data.old;

namespace Download.Print.old
{
    public class PostHeaderDataPage_v1 : IEnumDataPages<IHeaderData>, IKeyData
    //public class PostHeaderDataPage<TDataPage> : IEnumDataPages<TDataPage>, IKeyData where TDataPage : IHeaderData
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public IHeaderData[] Headers;
        //public TDataPage[] Headers;
        public string UrlNextPage;

        // IKeyData
        public BsonValue GetKey()
        {
            return Id;
        }

        // IEnumDataPages<TData>
        public IEnumerable<IHeaderData> GetDataList()
        //public IEnumerable<TDataPage> GetDataList()
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

    public class PostHeaderDetailMongoManagerBase_v1<TDetailData> : WebHeaderDetailMongoManagerBase_v1<TDetailData>
    {
        protected virtual void CreateManagers(XElement xe)
        {
            CreateHeaderWebDataPageManager(xe.zXPathElement("Header"));
            CreateDetailWebDataManager(xe.zXPathElement("Detail"));
            CreateWebHeaderDetailManager();
        }

        protected ServerManager CreateServerManager(string name, Action loadNewPost = null, Func<DateTime, IEnumerable<IPostToDownload>> getPostList = null)
        {
            if (loadNewPost == null)
                loadNewPost = LoadNewDocuments;
            if (getPostList == null)
                getPostList = Find;
            return new ServerManager
            {
                Name = name,
                EnableLoadNewPost = false,
                EnableSearchPostToDownload = false,
                DownloadDirectory = null,
                LoadNewPost = loadNewPost,
                GetPostList = getPostList,
                LoadPost = LoadDocument
            };
        }

        protected virtual void LoadNewDocuments()
        {
            throw new PBException("LoadNewDocuments() not implemented");
        }

        protected virtual IEnumerable<IPostToDownload> Find(DateTime date)
        {
            throw new PBException("Find() not implemented");
        }

        protected virtual IPostToDownload LoadDocument(BsonValue id)
        {
            throw new PBException("LoadDocument() not implemented");
        }
    }

    public class WebManagerCreator<TDetailData>
    {
        private Action _initLoadFromWeb = null;
        private Func<HttpRequestParameters> _getHttpRequestParameters = null;
        // header
        private Func<WebResult, IEnumDataPages<IHeaderData>> _getHeaderPageData = null;
        private Type _headerPageNominalType = null;
        private Func<int, HttpRequest> _getHttpRequestPage = null;
        private WebDataPageManager<IHeaderData> _headerWebDataPageManager = null;
        // detail
        private Func<HttpRequest, string> _detailCacheGetUrlSubDirectory = null;
        private Func<WebResult, TDetailData> _getDetailData = null;
        private Func<HttpRequest, BsonValue> _getDetailKeyFromHttpRequest = null;
        private Action<TDetailData> _loadDetailImages = null;
        private WebDataManager<TDetailData> _detailWebDataManager = null;

        public Action InitLoadFromWeb { get { return _initLoadFromWeb; } set { _initLoadFromWeb = value; } }
        public Func<HttpRequestParameters> GetHttpRequestParameters { get { return _getHttpRequestParameters; } set { _getHttpRequestParameters = value; } }
        // header
        public Func<WebResult, IEnumDataPages<IHeaderData>> GetHeaderPageData { get { return _getHeaderPageData; } set { _getHeaderPageData = value; } }
        public Type HeaderPageNominalType { get { return _headerPageNominalType; } set { _headerPageNominalType = value; } }
        public Func<int, HttpRequest> GetHttpRequestPage { get { return _getHttpRequestPage; } set { _getHttpRequestPage = value; } }
        // detail
        public Func<HttpRequest, string> DetailCacheGetUrlSubDirectory { get { return _detailCacheGetUrlSubDirectory; } set { _detailCacheGetUrlSubDirectory = value; } }
        public Func<WebResult, TDetailData> GetDetailData { get { return _getDetailData; } set { _getDetailData = value; } }
        public Func<HttpRequest, BsonValue> GetDetailKeyFromHttpRequest { get { return _getDetailKeyFromHttpRequest; } set { _getDetailKeyFromHttpRequest = value; } }
        public Action<TDetailData> LoadDetailImages { get { return _loadDetailImages; } set { _loadDetailImages = value; } }


        public WebDataPageManager<IHeaderData> CreateHeaderWebDataPageManager(XElement xe)
        {
            WebDataPageManager<IHeaderData> headerWebDataPageManager = new WebDataPageManager<IHeaderData>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = _initLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = _getHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = _getHeaderPageData;

            headerWebDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (headerWebDataPageManager.DocumentStore != null)
                headerWebDataPageManager.DocumentStore.NominalType = _headerPageNominalType;

            headerWebDataPageManager.GetHttpRequestPageFunction = _getHttpRequestPage;

            _headerWebDataPageManager = headerWebDataPageManager;

            return headerWebDataPageManager;
        }

        //TDetailData Vosbooks_PostDetail_v2
        public WebDataManager<TDetailData> CreateDetailWebDataManager(XElement xe)
        {
            WebDataManager<TDetailData> detailWebDataManager = new WebDataManager<TDetailData>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<TDetailData>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = _detailCacheGetUrlSubDirectory;
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = _initLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = _getHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = _getDetailData;
            detailWebDataManager.GetKeyFromHttpRequest = _getDetailKeyFromHttpRequest;
            detailWebDataManager.LoadImages = _loadDetailImages;

            detailWebDataManager.DocumentStore = MongoDocumentStore<TDetailData>.Create(xe);

            _detailWebDataManager = detailWebDataManager;

            return detailWebDataManager;
        }

        public WebHeaderDetailManager_v2<TDetailData> CreateWebHeaderDetailManager()
        {
            WebHeaderDetailManager_v2<TDetailData>  webHeaderDetailManager = new WebHeaderDetailManager_v2<TDetailData>();
            webHeaderDetailManager.HeaderDataPageManager = _headerWebDataPageManager;
            webHeaderDetailManager.DetailDataManager = _detailWebDataManager;
            return webHeaderDetailManager;
        }
    }
}

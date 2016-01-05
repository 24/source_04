using MongoDB.Bson;
using pb.Data.Mongo;
using System;
using System.Text;
using System.Xml.Linq;

namespace pb.Web.Data
{
    public class WebHeaderDetailMongoManagerBase<THeaderData, TDetailData>
    {
        protected Type _headerPageNominalType = null;
        //protected WebDataPageManager_v2<IHeaderData> _headerWebDataPageManager = null;
        protected WebDataPageManager<THeaderData> _headerWebDataPageManager = null;
        protected WebDataManager<TDetailData> _detailWebDataManager = null;
        //protected WebHeaderDetailManager_v2<TDetailData> _webHeaderDetailManager = null;
        protected WebHeaderDetailManager<THeaderData, TDetailData> _webHeaderDetailManager = null;

        public Type HeaderPageNominalType { get { return _headerPageNominalType; } set { _headerPageNominalType = value; } }
        public WebDataPageManager<THeaderData> HeaderWebDataPageManager { get { return _headerWebDataPageManager; } }
        public WebDataManager<TDetailData> DetailWebDataManager { get { return _detailWebDataManager; } }
        public WebHeaderDetailManager<THeaderData, TDetailData> WebHeaderDetailManager { get { return _webHeaderDetailManager; } }

        // used by header and detail
        protected virtual void InitLoadFromWeb()
        {
        }

        // used by header and detail
        protected virtual HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        // used by header cache
        protected virtual string GetHeaderPageCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        // header get data
        //protected virtual IEnumDataPages<IHeaderData> GetHeaderPageData(WebResult webResult)
        protected virtual IEnumDataPages<THeaderData> GetHeaderPageData(WebResult webResult)
        {
            throw new PBException("GetHeaderPageData() not implemented");
        }

        // header get key
        protected virtual BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            throw new PBException("GetHeaderKey() not implemented");
        }

        // header get url page
        protected virtual HttpRequest GetHttpRequestPage(int page)
        {
            throw new PBException("GetHttpRequestPage() not implemented");
        }

        // used by detail cache
        protected virtual string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        // detail get data
        protected virtual TDetailData GetDetailData(WebResult webResult)
        {
            throw new PBException("GetDetailData() not implemented");
        }

        protected virtual BsonValue GetDetailKey(HttpRequest httpRequest)
        {
            throw new PBException("GetDetailKey() not implemented");
        }

        protected virtual void LoadDetailImages(TDetailData data)
        {
            throw new PBException("LoadDetailImages() not implemented");
        }

        protected virtual void CreateHeaderWebDataPageManager(XElement xe)
        {
            _headerWebDataPageManager = new WebDataPageManager<THeaderData>();

            //_headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            _headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<THeaderData>>();
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = GetHeaderPageCacheUrlSubDirectory;
                _headerWebDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _headerWebDataPageManager.WebLoadDataManager.GetData = GetHeaderPageData;
            _headerWebDataPageManager.GetKeyFromHttpRequest = GetHeaderKey;

            //_headerWebDataPageManager.DocumentStore = MongoDocumentStore_v5<IEnumDataPages<IHeaderData>>.Create(xe);
            _headerWebDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<THeaderData>>.Create(xe);
            if (_headerWebDataPageManager.DocumentStore != null)
                _headerWebDataPageManager.DocumentStore.NominalType = _headerPageNominalType;

            _headerWebDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
        }

        protected virtual void CreateDetailWebDataManager(XElement xe)
        {
            _detailWebDataManager = new WebDataManager<TDetailData>();

            _detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<TDetailData>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = GetDetailCacheUrlSubDirectory;
                _detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _detailWebDataManager.WebLoadDataManager.GetData = GetDetailData;
            _detailWebDataManager.GetKeyFromHttpRequest = GetDetailKey;
            _detailWebDataManager.LoadImages = LoadDetailImages;

            _detailWebDataManager.DocumentStore = MongoDocumentStore<TDetailData>.Create(xe);
        }

        protected virtual void CreateWebHeaderDetailManager()
        {
            //_webHeaderDetailManager = new WebHeaderDetailManager_v2<TDetailData>();
            _webHeaderDetailManager = new WebHeaderDetailManager<THeaderData, TDetailData>();
            _webHeaderDetailManager.HeaderDataPageManager = _headerWebDataPageManager;
            _webHeaderDetailManager.DetailDataManager = _detailWebDataManager;
        }
    }
}

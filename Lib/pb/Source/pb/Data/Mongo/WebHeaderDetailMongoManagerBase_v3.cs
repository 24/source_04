using MongoDB.Bson;
using pb.Data.Xml;
using pb.Web.Data;
using pb.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace pb.Data.Mongo
{
    // from WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData> using WebDataManager_v5<THeaderData, TDetailData> for detail
    public class WebHeaderDetailMongoManagerBase_v3<THeaderData, TDetailData> where THeaderData : IKeyDetail, INamedHttpRequest
    {
        protected Type _headerPageNominalType = null;
        protected WebDataPageManager_v4<THeaderData> _headerDataPageManager = null;
        protected WebDataManager_v5<THeaderData, TDetailData> _detailDataManager = null;
        protected WebHeaderDetailManager_v5<THeaderData, TDetailData> _headerDetailManager = null;

        public Type HeaderPageNominalType { get { return _headerPageNominalType; } set { _headerPageNominalType = value; } }
        public WebDataPageManager_v4<THeaderData> HeaderDataPageManager { get { return _headerDataPageManager; } }
        public WebDataManager_v5<THeaderData, TDetailData> DetailDataManager { get { return _detailDataManager; } }
        public WebHeaderDetailManager_v5<THeaderData, TDetailData> HeaderDetailManager { get { return _headerDetailManager; } }

        // used by header and detail
        protected virtual void InitLoadFromWeb()
        {
        }

        // used by header and detail
        protected virtual void SetHttpRequestParameters(HttpRequestParameters requestParameters)
        {
        }

        // used by header cache
        protected virtual string GetHeaderPageCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        // header image cache get sub-directory
        protected virtual string GetHeaderImageCacheUrlSubDirectory(WebData<IEnumDataPages<THeaderData>> data)
        {
            return null;
        }

        // header get data
        protected virtual IEnumDataPages<THeaderData> GetHeaderPageData(HttpResult<string> httpResult)
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

        // detail cache get sub-directory
        // attention GetDetailCacheUrlSubDirectory() est appelé pour tous les types de detail
        protected virtual string GetDetailCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        // detail image cache get sub-directory
        //protected virtual string GetDetailImageCacheUrlSubDirectory(WebData<TDetailData> data)
        protected virtual string GetDetailImageCacheUrlSubDirectory(WebData_v2<THeaderData, TDetailData> webData)
        {
            return null;
        }

        // detail get data
        //protected virtual TDetailData GetDetailData(HttpResult<string> httpResult)
        //{
        //    throw new PBException("GetDetailData() not implemented");
        //}

        // detail create data
        protected virtual TDetailData CreateDetailData()
        {
            throw new PBException("CreateDetailData() not implemented");
        }

        // detail get key
        //protected virtual BsonValue GetDetailKey(HttpRequest httpRequest)
        //{
        //    throw new PBException("GetDetailKey() not implemented");
        //}

        protected virtual void Create(XElement xe, IEnumerable<NamedGetData<TDetailData>> namedGetDatas)
        {
            CreateHeaderWebDataPageManager(xe.zXPathElement("Header"));
            CreateDetailWebDataManager(xe.zXPathElement("Detail"), namedGetDatas);
            CreateWebHeaderDetailManager();
        }

        protected virtual void CreateHeaderWebDataPageManager(XElement xe)
        {
            _headerDataPageManager = new WebDataPageManager_v4<THeaderData>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
                urlCache.GetUrlSubDirectory = GetHeaderPageCacheUrlSubDirectory;
            //Trace.WriteLine($"WebHeaderDetailMongoManagerBase_v2<THeaderData, TDetailData>.CreateHeaderWebDataPageManager() : {(urlCache != null ? "urlCache created" : "no urlCache")}");

            _headerDataPageManager.WebLoadDataManager = new WebLoadDataManager_v2<IEnumDataPages<THeaderData>>();
            _headerDataPageManager.WebLoadDataManager.TraceException = true;
            _headerDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            _headerDataPageManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _headerDataPageManager.WebLoadDataManager.GetData = GetHeaderPageData;
            SetHttpRequestParameters(_headerDataPageManager.WebLoadDataManager.RequestParameters);
            _headerDataPageManager.GetKeyFromHttpRequest = GetHeaderKey;

            _headerDataPageManager.DataStore = MongoDataStore.Create(xe);
            MongoDataSerializer<IEnumDataPages<THeaderData>> dataSerializer = new MongoDataSerializer<IEnumDataPages<THeaderData>>();
            dataSerializer.ItemName = xe.zXPathValue("MongoDocumentItemName");
            dataSerializer.NominalType = _headerPageNominalType;
            _headerDataPageManager.DataSerializer = dataSerializer;

            UrlCache imageUrlCache = UrlCache.Create(xe.zXPathElement("Image"));
            _headerDataPageManager.WebLoadImageManager = new WebLoadImageManager_v2<IEnumDataPages<THeaderData>>();
            if (imageUrlCache != null)
            {
                _headerDataPageManager.WebLoadImageManager.WebImageCacheManager = new WebImageCacheManager_v3(imageUrlCache);
                _headerDataPageManager.WebLoadImageManager.WebImageCacheManager.TraceException = true;
                _headerDataPageManager.GetImageSubDirectory = GetHeaderImageCacheUrlSubDirectory;
            }

            _headerDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
        }

        protected virtual void CreateDetailWebDataManager(XElement xe, IEnumerable<NamedGetData<TDetailData>> namedGetDatas)
        {
            _detailDataManager = new WebDataManager_v5<THeaderData, TDetailData>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
                urlCache.GetUrlSubDirectory = GetDetailCacheUrlSubDirectory;

            _detailDataManager.HttpManager = new HttpManager_v2();
            _detailDataManager.HttpManager.TraceException = true;
            _detailDataManager.HttpManager.UrlCache = urlCache;
            _detailDataManager.HttpManager.InitLoadFromWeb = InitLoadFromWeb;
            //_detailDataManager.WebLoadDataManager.GetData = GetDetailData;
            _detailDataManager.CreateData = CreateDetailData;
            _detailDataManager.AddNamedGetDatas(namedGetDatas);
            SetHttpRequestParameters(_detailDataManager.HttpManager.RequestParameters);

            //_detailDataManager.GetKeyFromHttpRequest = GetDetailKey;

            _detailDataManager.Store = MongoDataStore.Create(xe);
            MongoDataSerializer<TDetailData> dataSerializer = new MongoDataSerializer<TDetailData>();
            dataSerializer.ItemName = xe.zXPathValue("MongoDocumentItemName");
            _detailDataManager.DataSerializer = dataSerializer;

            UrlCache imageUrlCache = UrlCache.Create(xe.zXPathElement("Image"));
            _detailDataManager.WebLoadImageManager = new WebLoadImageManager_v2<TDetailData>();
            if (imageUrlCache != null)
            {
                _detailDataManager.WebLoadImageManager.WebImageCacheManager = new WebImageCacheManager_v3(imageUrlCache);
                _detailDataManager.WebLoadImageManager.WebImageCacheManager.TraceException = true;
                _detailDataManager.GetImageSubDirectory = GetDetailImageCacheUrlSubDirectory;
            }
        }

        protected virtual void CreateWebHeaderDetailManager()
        {
            _headerDetailManager = new WebHeaderDetailManager_v5<THeaderData, TDetailData>();
            _headerDetailManager.HeaderDataPageManager = _headerDataPageManager;
            _headerDetailManager.DetailDataManager = _detailDataManager;
        }

        public virtual IEnumerable<string> Backup(string directory)
        {
            return _headerDataPageManager.Backup(directory).Union(_detailDataManager.Backup(directory));
        }
    }
}

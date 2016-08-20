using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Data.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace pb.Web.Data.Mongo
{
    public class WebHeaderDetailMongoManagerBase<THeaderData, TDetailData>
    {
        protected Type _headerPageNominalType = null;
        protected WebDataPageManager<THeaderData> _headerDataPageManager = null;
        protected WebDataManager<TDetailData> _detailDataManager = null;
        protected WebHeaderDetailManager<THeaderData, TDetailData> _headerDetailManager = null;

        public Type HeaderPageNominalType { get { return _headerPageNominalType; } set { _headerPageNominalType = value; } }
        public WebDataPageManager<THeaderData> HeaderDataPageManager { get { return _headerDataPageManager; } }
        public WebDataManager<TDetailData> DetailDataManager { get { return _detailDataManager; } }
        public WebHeaderDetailManager<THeaderData, TDetailData> HeaderDetailManager { get { return _headerDetailManager; } }

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

        protected virtual string GetDetailImageCacheUrlSubDirectory(WebData<TDetailData> data)
        {
            return "Image";
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

        //[Obsolete]
        //protected virtual void LoadDetailImages(TDetailData data)
        //{
        //    throw new PBException("LoadDetailImages() not implemented");
        //}

        protected virtual void CreateDataManager(XElement xe)
        {
            CreateHeaderWebDataPageManager(xe.zXPathElement("Header"));
            CreateDetailWebDataManager(xe.zXPathElement("Detail"));
            CreateWebHeaderDetailManager();
        }

        protected virtual void CreateHeaderWebDataPageManager(XElement xe)
        {
            _headerDataPageManager = new WebDataPageManager<THeaderData>();

            _headerDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<THeaderData>>();
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = GetHeaderPageCacheUrlSubDirectory;
                _headerDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _headerDataPageManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _headerDataPageManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _headerDataPageManager.WebLoadDataManager.GetData = GetHeaderPageData;
            _headerDataPageManager.GetKeyFromHttpRequest = GetHeaderKey;

            _headerDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<THeaderData>>.Create(xe);
            if (_headerDataPageManager.DocumentStore != null)
                _headerDataPageManager.DocumentStore.NominalType = _headerPageNominalType;

            _headerDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
        }

        protected virtual void CreateDetailWebDataManager(XElement xe)
        {
            _detailDataManager = new WebDataManager<TDetailData>();

            _detailDataManager.WebLoadDataManager = new WebLoadDataManager<TDetailData>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = GetDetailCacheUrlSubDirectory;
                _detailDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _detailDataManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _detailDataManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _detailDataManager.WebLoadDataManager.GetData = GetDetailData;
            _detailDataManager.GetKeyFromHttpRequest = GetDetailKey;
            //_detailDataManager.LoadImages = LoadDetailImages;

            _detailDataManager.DocumentStore = MongoDocumentStore<TDetailData>.Create(xe);

            UrlCache imageUrlCache = UrlCache.Create(xe.zXPathElement("Image"));
            if (imageUrlCache != null)
            {
                //imageUrlCache.GetUrlSubDirectory = GetDetailImageCacheUrlSubDirectory;
                _detailDataManager.WebImageCacheManager = new WebImageCacheManager_v2(imageUrlCache);
                _detailDataManager.GetImageSubDirectory = GetDetailImageCacheUrlSubDirectory;
            }

        }

        protected virtual void CreateWebHeaderDetailManager()
        {
            _headerDetailManager = new WebHeaderDetailManager<THeaderData, TDetailData>();
            _headerDetailManager.HeaderDataPageManager = _headerDataPageManager;
            _headerDetailManager.DetailDataManager = _detailDataManager;
        }

        public virtual IEnumerable<string> Backup(string directory)
        {
            return _headerDataPageManager.Backup(directory).Union(_detailDataManager.Backup(directory));
        }
    }
}

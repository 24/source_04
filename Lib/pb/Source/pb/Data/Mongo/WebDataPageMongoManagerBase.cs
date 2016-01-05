using System;
using System.Text;
using System.Xml.Linq;
using pb.Web;
using pb.Web.Data;
using MongoDB.Bson;

namespace pb.Data.Mongo
{
    public class WebDataPageMongoManagerBase<THeaderData> : WebDataPageManager<THeaderData>
    {
        protected Type _dataPageNominalType = null;
        //protected WebDataPageManager_v2<THeaderData> _dataPageManager = null;

        public Type DataPageNominalType { get { return _dataPageNominalType; } set { _dataPageNominalType = value; } }
        //public WebDataPageManager_v2<THeaderData> DataPageManager { get { return _dataPageManager; } }

        protected virtual void InitLoadFromWeb()
        {
        }

        protected virtual HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        protected virtual string GetCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        protected virtual IEnumDataPages<THeaderData> GetDataPage(WebResult webResult)
        {
            throw new PBException("GetHeaderPageData() not implemented");
        }

        protected virtual BsonValue GetKey(HttpRequest httpRequest)
        {
            throw new PBException("GetKey() not implemented");
        }

        protected virtual HttpRequest GetHttpRequestPage(int page)
        {
            throw new PBException("GetHttpRequestPage() not implemented");
        }

        protected virtual void Create(XElement xe)
        {
            //_dataPageManager = new WebDataPageManager_v2<THeaderData>();

            //_dataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<THeaderData>>();
            _webLoadDataManager = new WebLoadDataManager<IEnumDataPages<THeaderData>>();
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = GetCacheUrlSubDirectory;
                //_dataPageManager.WebLoadDataManager.UrlCache = urlCache;
                _webLoadDataManager.UrlCache = urlCache;
            }

            //_dataPageManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            //_dataPageManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            //_dataPageManager.WebLoadDataManager.GetData = GetDataPage;
            //_dataPageManager.GetKeyFromHttpRequest = GetKey;
            _webLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _webLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _webLoadDataManager.GetData = GetDataPage;
            _getKeyFromHttpRequest = GetKey;

            //_dataPageManager.DocumentStore = MongoDocumentStore_v5<IEnumDataPages<THeaderData>>.Create(xe);
            //if (_dataPageManager.DocumentStore != null)
            //    _dataPageManager.DocumentStore.NominalType = _dataPageNominalType;
            _documentStore = MongoDocumentStore<IEnumDataPages<THeaderData>>.Create(xe);
            if (_documentStore != null)
                _documentStore.NominalType = _dataPageNominalType;

            //_dataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            _getHttpRequestPageFunction = GetHttpRequestPage;
        }
    }
}

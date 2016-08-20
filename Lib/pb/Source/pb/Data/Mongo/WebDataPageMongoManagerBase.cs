using System;
using System.Text;
using System.Xml.Linq;
using MongoDB.Bson;
using pb.Data.Mongo;

namespace pb.Web.Data.Mongo
{
    public class WebDataPageMongoManagerBase<TData> : WebDataPageManager<TData>
    {
        protected Type _dataPageNominalType = null;

        public Type DataPageNominalType { get { return _dataPageNominalType; } set { _dataPageNominalType = value; } }

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

        protected virtual IEnumDataPages<TData> GetDataPage(WebResult webResult)
        {
            throw new PBException("GetDataPage() not implemented");
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
            _webLoadDataManager = new WebLoadDataManager<IEnumDataPages<TData>>();
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = GetCacheUrlSubDirectory;
                _webLoadDataManager.UrlCache = urlCache;
            }

            _webLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _webLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _webLoadDataManager.GetData = GetDataPage;
            _getKeyFromHttpRequest = GetKey;

            _documentStore = MongoDocumentStore<IEnumDataPages<TData>>.Create(xe);
            if (_documentStore != null)
                _documentStore.NominalType = _dataPageNominalType;

            _getHttpRequestPageFunction = GetHttpRequestPage;
        }
    }
}

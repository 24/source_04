using System;
using System.Text;
using System.Xml.Linq;
using pb.Web;
using pb.Web.Data;

namespace pb.Data.Mongo.old
{
    public class WebDataPageMongoManagerBase_v1
    {
        protected Type _dataPageNominalType = null;
        protected WebDataPageManager<IHeaderData> _dataPageManager = null;

        public Type DataPageNominalType { get { return _dataPageNominalType; } set { _dataPageNominalType = value; } }
        public WebDataPageManager<IHeaderData> DataPageManager { get { return _dataPageManager; } }

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

        protected virtual IEnumDataPages<IHeaderData> GetDataPage(WebResult webResult)
        {
            throw new PBException("GetHeaderPageData() not implemented");
        }

        protected virtual HttpRequest GetHttpRequestPage(int page)
        {
            throw new PBException("GetHttpRequestPage() not implemented");
        }

        protected virtual void CreateDataPageManager(XElement xe)
        {
            _dataPageManager = new WebDataPageManager<IHeaderData>();

            _dataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = GetCacheUrlSubDirectory;
                _dataPageManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _dataPageManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _dataPageManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _dataPageManager.WebLoadDataManager.GetData = GetDataPage;

            _dataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (_dataPageManager.DocumentStore != null)
                _dataPageManager.DocumentStore.NominalType = _dataPageNominalType;

            _dataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Web;
using pb.Web.Data;
using System.Text;
using pb.Web.Http;

namespace pb.Web.Data.Mongo
{
    public class WebHeaderMongoManagerBase<THeaderData>
    {
        protected Type _headerPageNominalType = null;
        protected WebDataPageManager<THeaderData> _headerWebDataPageManager = null;

        public Type HeaderPageNominalType { get { return _headerPageNominalType; } set { _headerPageNominalType = value; } }
        public WebDataPageManager<THeaderData> HeaderWebDataPageManager { get { return _headerWebDataPageManager; } }

        protected virtual void InitLoadFromWeb()
        {
        }

        protected virtual HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        protected virtual string GetHeaderPageCacheUrlSubDirectory(HttpRequest httpRequest)
        {
            return null;
        }

        protected virtual IEnumDataPages<THeaderData> GetHeaderPageData(WebResult webResult)
        {
            throw new PBException("GetHeaderPageData() not implemented");
        }

        protected virtual BsonValue GetHeaderKey(HttpRequest httpRequest)
        {
            throw new PBException("GetHeaderKey() not implemented");
        }

        protected virtual HttpRequest GetHttpRequestPage(int page)
        {
            throw new PBException("GetHttpRequestPage() not implemented");
        }

        protected virtual void CreateHeaderWebDataPageManager(XElement xe)
        {
            _headerWebDataPageManager = new WebDataPageManager<THeaderData>();

            _headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<THeaderData>>();
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = GetHeaderPageCacheUrlSubDirectory;
                _headerWebDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _headerWebDataPageManager.WebLoadDataManager.GetData = GetHeaderPageData;
            _headerWebDataPageManager.GetKeyFromHttpRequest = GetHeaderKey;

            _headerWebDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<THeaderData>>.Create(xe);
            if (_headerWebDataPageManager.DocumentStore != null)
                _headerWebDataPageManager.DocumentStore.NominalType = _headerPageNominalType;

            _headerWebDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
        }

        public virtual IEnumerable<string> Backup(string directory)
        {
            return _headerWebDataPageManager.Backup(directory);
        }
    }
}

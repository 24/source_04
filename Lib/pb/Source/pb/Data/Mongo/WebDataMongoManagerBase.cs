using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Web.Http;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace pb.Web.Data.Mongo
{
    public class WebDataMongoManagerBase<TData>
    {
        protected WebDataManager<TData> _webDataManager = null;

        public WebDataManager<TData> WebDataManager { get { return _webDataManager; } }

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

        protected virtual TData GetData(WebResult webResult)
        {
            throw new PBException("GetData() not implemented");
        }

        protected virtual BsonValue GetKey(HttpRequest httpRequest)
        {
            throw new PBException("GetKey() not implemented");
        }

        //protected virtual void LoadImages(TData data)
        //{
        //    throw new PBException("LoadImages() not implemented");
        //}

        protected virtual void CreateWebDataManager(XElement xe)
        {
            _webDataManager = new WebDataManager<TData>();

            _webDataManager.WebLoadDataManager = new WebLoadDataManager<TData>();

            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectory = GetCacheUrlSubDirectory;
                _webDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            _webDataManager.WebLoadDataManager.InitLoadFromWeb = InitLoadFromWeb;
            _webDataManager.WebLoadDataManager.GetHttpRequestParameters = GetHttpRequestParameters;
            _webDataManager.WebLoadDataManager.GetData = GetData;
            _webDataManager.GetKeyFromHttpRequest = GetKey;
            //_webDataManager.LoadImages = LoadImages;

            _webDataManager.DocumentStore = MongoDocumentStore<TData>.Create(xe);
        }

        public virtual IEnumerable<string> Backup(string directory)
        {
            return _webDataManager.Backup(directory);
        }
    }
}

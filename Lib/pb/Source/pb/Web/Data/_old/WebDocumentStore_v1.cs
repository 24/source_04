using System;
using System.Collections.Generic;
using pb.Web.old;
using pb.Data.old;

namespace pb.Web.Data.old
{
    public abstract class WebDocumentStore_v1<TKey, TData>
    {
        protected IDocumentStore_v3<TKey, TData> _documentStore = null;
        protected LoadWebDataManager_v4<TKey, TData> _loadWebDataManager = null;

        public IDocumentStore_v3<TKey, TData> DocumentStore { get { return _documentStore; } }

        protected abstract TKey GetKeyFromUrl(string url);
        protected abstract string GetDataSourceUrl(TData data);
        protected abstract void LoadImages(TData data);

        protected virtual HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            return new HttpRequestParameters_v1();
        }

        public LoadWebData_v4<TKey, TData> LoadDocument(string url, HttpRequestParameters_v1 requestParameters = null, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            if (requestParameters == null)
                requestParameters = GetHttpRequestParameters();
            LoadWebData_v4<TKey, TData> loadWebData = _loadWebDataManager.Load(new RequestWebData_v4<TKey>(new RequestFromWeb_v3(url, requestParameters, reloadFromWeb, loadImage), GetKeyFromUrl(url), refreshDocumentStore));
            return loadWebData;
        }

        public TData LoadDocument(TKey key, bool loadImage = false)
        {
            TData data = _documentStore.LoadDocument(key);
            if (loadImage)
                LoadImages(data);
            return data;
        }

        // bool useCursorCache = false
        public IEnumerable<TData> FindDocuments(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            // useCursorCache: useCursorCache
            return _documentStore.FindDocuments(query, sort: sort, limit: limit).zAction(data => { if (loadImage) LoadImages(data); });
        }

        public void UpdateDocuments(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = _documentStore.UpdateDocuments(updateDocument, query, sort, limit);
            Trace.WriteLine("{0} document(s) updated", nb);
        }

        public void RefreshDocumentsStore(Action<TData, TData> action = null, int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        {
            ////int traceLevel = Trace.CurrentTrace.TraceLevel;
            //Trace.CurrentTrace.TraceLevel = 1;
            foreach (TData data in FindDocuments(query, sort: sort, limit: limit, loadImage: false))
            {
                HttpRequestParameters_v1 requestParameters = GetHttpRequestParameters();
                string url = GetDataSourceUrl(data);
                LoadWebData_v4<TKey, TData> loadWebData = _loadWebDataManager.LoadDocumentFromWeb(new RequestWebData_v4<TKey>(new RequestFromWeb_v3(url, requestParameters, reloadFromWeb, loadImage),
                    _documentStore.GetDataKey(data)));

                TData data2 = loadWebData.Document;

                if (action != null)
                    action(data, data2);

                _loadWebDataManager.SaveDocument(loadWebData);
            }
            ////Trace.CurrentTrace.TraceLevel = traceLevel;
        }
    }

    public abstract class WebDocumentStore_v2<TKey, TData>
    {
        protected IDocumentStore_v3<TKey, TData> _documentStore = null;
        protected LoadWebDataManager_v5<TKey, TData> _loadWebDataManager = null;

        public IDocumentStore_v3<TKey, TData> DocumentStore { get { return _documentStore; } }

        //protected abstract TKey GetKeyFromUrl(string url);
        protected abstract TKey GetKeyFromHttpRequest(HttpRequest httpRequest);
        //protected abstract string GetDataSourceUrl(TData data);
        protected abstract HttpRequest GetDataSourceHttpRequest(TData data);
        protected abstract void LoadImages(TData data);

        //protected virtual HttpRequestParameters GetHttpRequestParameters()
        //{
        //    return new HttpRequestParameters();
        //}

        public LoadWebData_v5<TKey, TData> LoadDocument(HttpRequest httpRequest, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            return _loadWebDataManager.Load(new RequestWebData_v5<TKey>(new RequestFromWeb_v4(httpRequest, reloadFromWeb, loadImage), GetKeyFromHttpRequest(httpRequest), refreshDocumentStore));
        }

        public TData LoadDocument(TKey key, bool loadImage = false)
        {
            TData data = _documentStore.LoadDocument(key);
            if (loadImage)
                LoadImages(data);
            return data;
        }

        // bool useCursorCache = false
        public IEnumerable<TData> FindDocuments(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            // useCursorCache: useCursorCache
            return _documentStore.FindDocuments(query, sort: sort, limit: limit).zAction(data => { if (loadImage) LoadImages(data); });
        }

        public void UpdateDocuments(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = _documentStore.UpdateDocuments(updateDocument, query, sort, limit);
            Trace.WriteLine("{0} document(s) updated", nb);
        }

        public void RefreshDocumentsStore(Action<TData, TData> action = null, int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        {
            ////int traceLevel = Trace.CurrentTrace.TraceLevel;
            ////Trace.CurrentTrace.TraceLevel = 1;
            foreach (TData data in FindDocuments(query, sort: sort, limit: limit, loadImage: false))
            {
                //HttpRequestParameters requestParameters = GetHttpRequestParameters();
                //string url = GetDataSourceUrl(data);
                //HttpRequest httpRequest = new HttpRequest { Url = GetDataSourceUrl(data) };
                LoadWebData_v5<TKey, TData> loadWebData = _loadWebDataManager.LoadDocumentFromWeb(new RequestWebData_v5<TKey>(new RequestFromWeb_v4(GetDataSourceHttpRequest(data), reloadFromWeb, loadImage),
                    _documentStore.GetDataKey(data)));

                TData data2 = loadWebData.Document;

                if (action != null)
                    action(data, data2);

                _loadWebDataManager.SaveDocument(loadWebData);
            }
            ////Trace.CurrentTrace.TraceLevel = traceLevel;
        }
    }
}

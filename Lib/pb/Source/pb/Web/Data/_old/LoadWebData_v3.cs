using pb.Web.Data;
using pb.Data.old;

namespace pb.Web.old
{
    public class RequestWebData_v3 : RequestFromWeb_v3
    {
        private object _key;
        private bool _refreshDocumentStore = false;

        public RequestWebData_v3(RequestFromWeb_v3 webRequest, object key = null, bool refreshDocumentStore = false)
            : base(webRequest.Url, webRequest.RequestParameters, webRequest.ReloadFromWeb, webRequest.LoadImage)
        {
            _key = key;
            _refreshDocumentStore = refreshDocumentStore;
        }

        public object Key { get { return _key; } }
        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } }
    }

    public class LoadWebData_v3<T> : ILoadDocument<T>
    {
        private RequestWebData_v3 _request;
        private object _key;
        private T _document;
        private bool _documentLoaded;
        private bool _documentLoadedFromWeb;
        private bool _documentLoadedFromStore;

        public LoadWebData_v3(RequestWebData_v3 request)
        {
            _request = request;
            _key = request.Key;
        }

        public RequestWebData_v3 Request { get { return _request; } }
        public object Key { get { return _key; } set { _key = value; } }
        public T Document { get { return _document; } set { _document = value; } }
        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
        public bool DocumentLoadedFromWeb { get { return _documentLoadedFromWeb; } set { _documentLoadedFromWeb = value; } }
        public bool DocumentLoadedFromStore { get { return _documentLoadedFromStore; } set { _documentLoadedFromStore = value; } }
    }

    public class LoadWebDataManager_v3<T>
    {
        private LoadDataFromWebManager_v3<T> _loadDataFromWeb = null;
        private IDocumentStore_v2<T> _documentStore = null;
        private bool _desactivateDocumentStore = false;

        public LoadWebDataManager_v3(LoadDataFromWebManager_v3<T> loadDataFromWeb, IDocumentStore_v2<T> documentStore = null)
        {
            _loadDataFromWeb = loadDataFromWeb;
            _documentStore = documentStore;
        }

        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }

        public LoadWebData_v3<T> Load(RequestWebData_v3 request)
        {
            LoadWebData_v3<T> loadWebData = new LoadWebData_v3<T>(request);
            if (request.ReloadFromWeb || request.RefreshDocumentStore || !DocumentExists(loadWebData))
            {
                _LoadDocumentFromWeb(loadWebData);
                SaveDocument(loadWebData);
            }
            LoadDocument(loadWebData);
            return loadWebData;
        }

        public LoadWebData_v3<T> LoadDocumentFromWeb(RequestWebData_v3 request)
        {
            LoadWebData_v3<T> loadWebData = new LoadWebData_v3<T>(request);
            _LoadDocumentFromWeb(loadWebData);
            return loadWebData;
        }

        private bool DocumentExists(LoadWebData_v3<T> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                return _documentStore.DocumentExists(loadWebData.Key);
            else
                return false;
        }

        private void LoadDocument(LoadWebData_v3<T> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!loadWebData.DocumentLoaded)
                {
                    loadWebData.Document = _documentStore.LoadDocument(loadWebData.Key);
                    loadWebData.DocumentLoaded = true;
                    loadWebData.DocumentLoadedFromStore = true;
                }
            }
            else
                _LoadDocumentFromWeb(loadWebData);
        }

        public void SaveDocument(LoadWebData_v3<T> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                _documentStore.SaveDocument(loadWebData.Key, loadWebData.Document);
        }

        private void _LoadDocumentFromWeb(LoadWebData_v3<T> loadWebData)
        {
            if (!loadWebData.DocumentLoaded)
            {
                loadWebData.Document = _loadDataFromWeb.Load(loadWebData.Request);
                loadWebData.DocumentLoaded = true;
                loadWebData.DocumentLoadedFromWeb = true;
            }
        }
    }

    public class RequestWebData_v4<TKey> : RequestFromWeb_v3
    {
        private TKey _key;
        private bool _refreshDocumentStore = false;

        public RequestWebData_v4(RequestFromWeb_v3 webRequest, TKey key = default(TKey), bool refreshDocumentStore = false)
            : base(webRequest.Url, webRequest.RequestParameters, webRequest.ReloadFromWeb, webRequest.LoadImage)
        {
            _key = key;
            _refreshDocumentStore = refreshDocumentStore;
        }

        public TKey Key { get { return _key; } }
        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } }
    }

    public class LoadWebData_v4<TKey, TData> : ILoadDocument_v4<TKey, TData>
    {
        private RequestWebData_v4<TKey> _request;
        private TKey _key;
        private TData _document;
        private bool _documentLoaded;
        private bool _documentLoadedFromWeb;
        private bool _documentLoadedFromStore;

        public LoadWebData_v4(RequestWebData_v4<TKey> request)
        {
            _request = request;
            _key = request.Key;
        }

        public RequestWebData_v4<TKey> Request { get { return _request; } }
        public TKey Key { get { return _key; } set { _key = value; } }
        public TData Document { get { return _document; } set { _document = value; } }
        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
        public bool DocumentLoadedFromWeb { get { return _documentLoadedFromWeb; } set { _documentLoadedFromWeb = value; } }
        public bool DocumentLoadedFromStore { get { return _documentLoadedFromStore; } set { _documentLoadedFromStore = value; } }
    }

    public class LoadWebDataManager_v4<TKey, TData>
    {
        private LoadDataFromWebManager_v3<TData> _loadDataFromWeb = null;
        private IDocumentStore_v3<TKey, TData> _documentStore = null;
        private bool _desactivateDocumentStore = false;

        public LoadWebDataManager_v4(LoadDataFromWebManager_v3<TData> loadDataFromWeb, IDocumentStore_v3<TKey, TData> documentStore = null)
        {
            _loadDataFromWeb = loadDataFromWeb;
            _documentStore = documentStore;
        }

        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }

        public LoadWebData_v4<TKey, TData> Load(RequestWebData_v4<TKey> request)
        {
            LoadWebData_v4<TKey, TData> loadWebData = new LoadWebData_v4<TKey, TData>(request);
            if (request.ReloadFromWeb || request.RefreshDocumentStore || !DocumentExists(loadWebData))
            {
                _LoadDocumentFromWeb(loadWebData);
                SaveDocument(loadWebData);
            }
            LoadDocument(loadWebData);
            return loadWebData;
        }

        public LoadWebData_v4<TKey, TData> LoadDocumentFromWeb(RequestWebData_v4<TKey> request)
        {
            LoadWebData_v4<TKey, TData> loadWebData = new LoadWebData_v4<TKey, TData>(request);
            _LoadDocumentFromWeb(loadWebData);
            return loadWebData;
        }

        private bool DocumentExists(LoadWebData_v4<TKey, TData> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                return _documentStore.DocumentExists(loadWebData.Key);
            else
                return false;
        }

        private void LoadDocument(LoadWebData_v4<TKey, TData> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!loadWebData.DocumentLoaded)
                {
                    loadWebData.Document = _documentStore.LoadDocument(loadWebData.Key);
                    loadWebData.DocumentLoaded = true;
                    loadWebData.DocumentLoadedFromStore = true;
                }
            }
            else
                _LoadDocumentFromWeb(loadWebData);
        }

        public void SaveDocument(LoadWebData_v4<TKey, TData> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                _documentStore.SaveDocument(loadWebData.Key, loadWebData.Document);
        }

        private void _LoadDocumentFromWeb(LoadWebData_v4<TKey, TData> loadWebData)
        {
            if (!loadWebData.DocumentLoaded)
            {
                loadWebData.Document = _loadDataFromWeb.Load(loadWebData.Request);
                loadWebData.DocumentLoaded = true;
                loadWebData.DocumentLoadedFromWeb = true;
            }
        }
    }

    public class RequestWebData_v5<TKey> : RequestFromWeb_v4
    {
        private TKey _key;
        private bool _refreshDocumentStore = false;

        public RequestWebData_v5(RequestFromWeb_v4 request, TKey key = default(TKey), bool refreshDocumentStore = false)
            : base(request.HttpRequest, request.ReloadFromWeb, request.LoadImage)
        {
            _key = key;
            _refreshDocumentStore = refreshDocumentStore;
        }

        public TKey Key { get { return _key; } }
        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } }
    }

    public class LoadWebData_v5<TKey, TData> : ILoadDocument_v4<TKey, TData>
    {
        private RequestWebData_v5<TKey> _request;
        private TKey _key;
        private TData _document;
        private bool _documentLoaded;
        private bool _documentLoadedFromWeb;
        private bool _documentLoadedFromStore;

        public LoadWebData_v5(RequestWebData_v5<TKey> request)
        {
            _request = request;
            _key = request.Key;
        }

        public RequestWebData_v5<TKey> Request { get { return _request; } }
        public TKey Key { get { return _key; } set { _key = value; } }
        public TData Document { get { return _document; } set { _document = value; } }
        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
        public bool DocumentLoadedFromWeb { get { return _documentLoadedFromWeb; } set { _documentLoadedFromWeb = value; } }
        public bool DocumentLoadedFromStore { get { return _documentLoadedFromStore; } set { _documentLoadedFromStore = value; } }
    }

    public class LoadWebDataManager_v5<TKey, TData>
    {
        private LoadDataFromWebManager_v4<TData> _loadDataFromWeb = null;
        private IDocumentStore_v3<TKey, TData> _documentStore = null;
        private bool _desactivateDocumentStore = false;

        public LoadWebDataManager_v5(LoadDataFromWebManager_v4<TData> loadDataFromWeb, IDocumentStore_v3<TKey, TData> documentStore = null)
        {
            _loadDataFromWeb = loadDataFromWeb;
            _documentStore = documentStore;
        }

        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }

        public LoadWebData_v5<TKey, TData> Load(RequestWebData_v5<TKey> request)
        {
            LoadWebData_v5<TKey, TData> loadWebData = new LoadWebData_v5<TKey, TData>(request);
            if (request.ReloadFromWeb || request.RefreshDocumentStore || !DocumentExists(loadWebData))
            {
                _LoadDocumentFromWeb(loadWebData);
                SaveDocument(loadWebData);
            }
            LoadDocument(loadWebData);
            return loadWebData;
        }

        public LoadWebData_v5<TKey, TData> LoadDocumentFromWeb(RequestWebData_v5<TKey> request)
        {
            LoadWebData_v5<TKey, TData> loadWebData = new LoadWebData_v5<TKey, TData>(request);
            _LoadDocumentFromWeb(loadWebData);
            return loadWebData;
        }

        private bool DocumentExists(LoadWebData_v5<TKey, TData> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                return _documentStore.DocumentExists(loadWebData.Key);
            else
                return false;
        }

        private void LoadDocument(LoadWebData_v5<TKey, TData> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!loadWebData.DocumentLoaded)
                {
                    loadWebData.Document = _documentStore.LoadDocument(loadWebData.Key);
                    loadWebData.DocumentLoaded = true;
                    loadWebData.DocumentLoadedFromStore = true;
                }
            }
            else
                _LoadDocumentFromWeb(loadWebData);
        }

        public void SaveDocument(LoadWebData_v5<TKey, TData> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                _documentStore.SaveDocument(loadWebData.Key, loadWebData.Document);
        }

        private void _LoadDocumentFromWeb(LoadWebData_v5<TKey, TData> loadWebData)
        {
            if (!loadWebData.DocumentLoaded)
            {
                loadWebData.Document = _loadDataFromWeb.LoadData(loadWebData.Request);
                loadWebData.DocumentLoaded = true;
                loadWebData.DocumentLoadedFromWeb = true;
            }
        }
    }

    public class RequestWebData_v6<TKey> : RequestFromWeb_v4
    {
        private bool _refreshDocumentStore = false;

        public RequestWebData_v6(RequestFromWeb_v4 request, bool refreshDocumentStore = false)
            : base(request.HttpRequest, request.ReloadFromWeb, request.LoadImage)
        {
            _refreshDocumentStore = refreshDocumentStore;
        }

        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } }
    }

    public class LoadWebData_v6<TKey> : ILoadDocument_v6<TKey>
    {
        private RequestWebData_v6<TKey> _request;
        private IKeyData_v4<TKey> _document;
        private bool _documentLoaded;
        private bool _documentLoadedFromWeb;
        private bool _documentLoadedFromStore;

        public LoadWebData_v6(RequestWebData_v6<TKey> request)
        {
            _request = request;
        }

        public RequestWebData_v6<TKey> Request { get { return _request; } }
        public IKeyData_v4<TKey> Document { get { return _document; } set { _document = value; } }
        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
        public bool DocumentLoadedFromWeb { get { return _documentLoadedFromWeb; } set { _documentLoadedFromWeb = value; } }
        public bool DocumentLoadedFromStore { get { return _documentLoadedFromStore; } set { _documentLoadedFromStore = value; } }
    }

    public class LoadWebDataManager_v6<TKey>
    {
        //private LoadDataFromWebManager_new2<TData> _loadDataFromWebManager = null;
        private LoadDataFromWebManager_v5<IKeyData_v4<TKey>> _loadDataFromWebManager = null;
        //private IDocumentStore_new<TKey, TData> _documentStore = null;
        private IDocumentStore_v3<TKey, IKeyData_v4<TKey>> _documentStore = null;
        private bool _desactivateDocumentStore = false;

        public LoadDataFromWebManager_v5<IKeyData_v4<TKey>> LoadDataFromWebManager { get { return _loadDataFromWebManager; } set { _loadDataFromWebManager = value; } }
        public IDocumentStore_v3<TKey, IKeyData_v4<TKey>> DocumentStore { get { return _documentStore; } set { _documentStore = value; } }
        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }

        //public LoadWebData_new2<TKey, TData> Load(RequestWebData_new2<TKey> request)
        public LoadWebData_v6<TKey> Load(RequestWebData_v6<TKey> request)
        {
            //LoadWebData_new2<TKey, TData> loadWebData = new LoadWebData_new2<TKey, TData>(request);
            LoadWebData_v6<TKey> loadWebData = new LoadWebData_v6<TKey>(request);
            if (request.ReloadFromWeb || request.RefreshDocumentStore || !DocumentExists(loadWebData))
            {
                _LoadDocumentFromWeb(loadWebData);
                SaveDocument(loadWebData);
            }
            LoadDocument(loadWebData);
            return loadWebData;
        }

        //public LoadWebData_new2<TKey, TData> LoadDocumentFromWeb(RequestWebData_new2<TKey> request)
        public LoadWebData_v6<TKey> LoadDocumentFromWeb(RequestWebData_v6<TKey> request)
        {
            //LoadWebData_new2<TKey, TData> loadWebData = new LoadWebData_new2<TKey, TData>(request);
            LoadWebData_v6<TKey> loadWebData = new LoadWebData_v6<TKey>(request);
            _LoadDocumentFromWeb(loadWebData);
            return loadWebData;
        }

        //private bool DocumentExists(LoadWebData_new2<TKey, TData> loadWebData)
        private bool DocumentExists(LoadWebData_v6<TKey> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                //return _documentStore.DocumentExists(loadWebData.Key);
                return _documentStore.DocumentExists(loadWebData.Document.GetKey());
            else
                return false;
        }

        //private void LoadDocument(LoadWebData_new2<TKey, TData> loadWebData)
        private void LoadDocument(LoadWebData_v6<TKey> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!loadWebData.DocumentLoaded)
                {
                    //loadWebData.Document = _documentStore.LoadDocument(loadWebData.Key);
                    loadWebData.Document = _documentStore.LoadDocument(loadWebData.Document.GetKey());
                    loadWebData.DocumentLoaded = true;
                    loadWebData.DocumentLoadedFromStore = true;
                }
            }
            else
                _LoadDocumentFromWeb(loadWebData);
        }

        //public void SaveDocument(LoadWebData_new2<TKey, TData> loadWebData)
        public void SaveDocument(LoadWebData_v6<TKey> loadWebData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                //_documentStore.SaveDocument(loadWebData.Key, loadWebData.Document);
                _documentStore.SaveDocument(loadWebData.Document.GetKey(), loadWebData.Document);
        }

        //private void _LoadDocumentFromWeb(LoadWebData_new2<TKey, TData> loadWebData)
        private void _LoadDocumentFromWeb(LoadWebData_v6<TKey> loadWebData)
        {
            if (!loadWebData.DocumentLoaded)
            {
                loadWebData.Document = _loadDataFromWebManager.LoadData(loadWebData.Request);
                loadWebData.DocumentLoaded = true;
                loadWebData.DocumentLoadedFromWeb = true;
            }
        }
    }
}

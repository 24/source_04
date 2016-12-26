using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace pb.Web.Data
{
    // class WebDataManager_v4 copy from WebDataManager (v2)
    //   - remplacement de WebLoadDataManager par WebLoadDataManager_v2 (HttpManager_v2)
    //   - remplacement de IDocumentStore<TData> par MongoDataStore et MongoDataSerializer<TData>, MongoDataSerializer<HttpLog>
    //   - use WebLoadImageManager_v2<TData>
    public partial class WebDataManager_v4<TData>
    {
        //protected int _version = 1; // WebDataManager version : 1 use Load_v1(), 2 use WebData<TData>.Load(), 3 use MongoDataStore, 4 use WebLoadDataManager_v2
        protected WebLoadDataManager_v2<TData> _webLoadDataManager = null;
        //protected IDocumentStore<TData> _documentStore = null;
        protected bool _desactivateDocumentStore = false;
        protected bool _generateId = false;
        protected Func<HttpRequest, BsonValue> _getKeyFromHttpRequest = null;
        protected WebLoadImageManager_v2<TData> _webLoadImageManager = null;

        //public int Version { get { return _version; } set { _version = value; } }
        public WebLoadDataManager_v2<TData> WebLoadDataManager { get { return _webLoadDataManager; } set { _webLoadDataManager = value; } }
        //public IDocumentStore<TData> DocumentStore { get { return _documentStore; } set { _documentStore = value; } }
        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }
        public Func<HttpRequest, BsonValue> GetKeyFromHttpRequest { get { return _getKeyFromHttpRequest; } set { _getKeyFromHttpRequest = value; } }
        public WebLoadImageManager_v2<TData> WebLoadImageManager { get { return _webLoadImageManager; } set { _webLoadImageManager = value; } }

        public WebDataManager_v4()
        {
            InitSerializer();
        }

        public WebData<TData> Load(WebRequest request)
        {
            //if (_version == 1)
            //    return Load_v1(request);
            //else if (_version == 2)
            //    return WebData<TData>.Load(this, request);
            //else // if (_version == 3)
            //    return WebData<TData>.Load_v2(this, request);
            return WebData<TData>.Load_v2(this, request);
        }

        //public WebData<TData> Load_v1(WebRequest request)
        //{
        //    WebData<TData> webData = new WebData<TData>(request);
        //    if (_documentStore != null && _documentStore.GenerateId)
        //    {
        //        BsonValue id = GetId(webData);
        //        if (request.ReloadFromWeb || request.RefreshDocumentStore || id == null)
        //        {
        //            _LoadFromWeb(webData);
        //            if (id == null)
        //                id = _documentStore.GetNewId();
        //            SetDataId(webData, id);
        //            SaveWithId(id, webData);
        //            if (_imageLoadVersion == 1)
        //                LoadImages_v1(webData.Document, request.ImageRequest);
        //            else
        //            {
        //                if (LoadImagesFromWeb(webData))
        //                    SaveWithId(id, webData);
        //            }
        //        }
        //    }
        //    //else
        //    //{
        //    //    // todo : remplacer Exists() par GetKey() puis Exists(key)
        //    //    if (request.ReloadFromWeb || request.RefreshDocumentStore || !Exists(webData))
        //    //    {
        //    //        _LoadFromWeb(webData);
        //    //        SaveWithKey(webData);
        //    //        if (_imageLoadVersion == 1)
        //    //            LoadImages_v1(webData.Document, webData.Request.ImageRequest);
        //    //        else
        //    //        {
        //    //            if (LoadImagesFromWeb(webData))
        //    //                SaveWithKey(webData);
        //    //        }
        //    //    }
        //    //}
        //    else if (request.ReloadFromWeb || request.RefreshDocumentStore || !Exists(webData))
        //    {
        //        // todo : remplacer Exists() par GetKey() puis Exists(key)
        //        _LoadFromWeb(webData);
        //        SaveWithKey(webData);
        //        if (_imageLoadVersion == 1)
        //            LoadImages_v1(webData.Document, webData.Request.ImageRequest);
        //        else
        //        {
        //            if (LoadImagesFromWeb(webData))
        //                SaveWithKey(webData);
        //        }
        //    }

        //    _Load(webData);
        //    return webData;
        //}

        //public WebData<TData> LoadFromWeb(WebRequest request)
        //{
        //    WebData<TData> webData = new WebData<TData>(request);
        //    _LoadFromWeb(webData);
        //    if (_imageLoadVersion == 1)
        //        LoadImages_v1(webData.Document, webData.Request.ImageRequest);
        //    else
        //    {
        //        LoadImagesFromWeb(webData);
        //        LoadImagesToData(webData.Document);
        //    }
        //    return webData;
        //}

        //protected bool Exists(WebData<TData> webData)
        //{
        //    if (_documentStore != null && !_desactivateDocumentStore)
        //    {
        //        BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
        //        return _documentStore.Exists(key);
        //    }
        //    else
        //        return false;
        //}

        //protected BsonValue GetId(WebData<TData> webData)
        //{
        //    if (_documentStore != null && !_desactivateDocumentStore)
        //    {
        //        BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
        //        return _documentStore.GetId(key);
        //    }
        //    else
        //        return null;
        //}

        //protected void SetDataId(WebData<TData> webData, BsonValue id)
        //{
        //    webData.Document.zSetId(id);
        //}

        //protected void _Load(WebData<TData> webData)
        //{
        //    if (_documentStore != null && !_desactivateDocumentStore)
        //    {
        //        if (!webData.DocumentLoaded)
        //        {
        //            BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
        //            webData.Document = _documentStore.LoadFromKey(key);

        //            //LoadImages(webData);
        //            if (webData.Request.ImageRequest.LoadImageToData)
        //            {
        //                if (_imageLoadVersion == 1)
        //                    LoadImages_v1(webData.Document, webData.Request.ImageRequest);
        //                else
        //                    LoadImagesToData(webData.Document);
        //            }

        //            //if (webData.Request.LoadImageFromWeb || webData.Request.LoadImageToData || webData.Request.RefreshImage)
        //            //    LoadImages(webData.Document, WebImageRequest.FromWebRequest(webData.Request));

        //            //    if (!(webData.Document is ILoadImages))
        //            //        throw new PBException($"{typeof(TData).zGetTypeName()} is not ILoadImages");
        //            //    ((ILoadImages)webData.Document).LoadImages(ImageRequest.FromWebRequest(webData.Request));
        //            webData.DocumentLoaded = true;
        //            webData.DocumentLoadedFromStore = true;
        //        }
        //    }
        //    else
        //        _LoadFromWeb(webData);
        //}

        //public void SaveWithId(BsonValue id, WebData<TData> webData)
        //{
        //    if (_documentStore != null && !_desactivateDocumentStore)
        //    {
        //        _documentStore.SaveWithId(id, webData.Document);
        //    }
        //}

        //public void SaveWithKey(WebData<TData> webData)
        //{
        //    if (_documentStore != null && !_desactivateDocumentStore)
        //    {
        //        _documentStore.SaveWithKey(webData.Document.zGetKey_v2(), webData.Document);
        //    }
        //}

        //protected void _LoadFromWeb(WebData<TData> webData)
        //{
        //    if (!webData.DocumentLoaded)
        //    {
        //        WebDataResult<TData> webDataResult = _webLoadDataManager.LoadData(webData.Request);
        //        webData.Result = webDataResult.Result;
        //        webData.Document = webDataResult.Data;
        //        webData.DocumentLoaded = true;
        //        webData.DocumentLoadedFromWeb = true;
        //    }
        //}

        //public IEnumerable<TData> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        //{
        //    //return _documentStore.Find(query, sort: sort, limit: limit).zAction(
        //    //    data =>
        //    //    {
        //    //        if (loadImage)
        //    //        {
        //    //            if (_imageLoadVersion == 1)
        //    //                LoadImages_v1(data, new WebImageRequest { LoadImageToData = true });
        //    //            else
        //    //                LoadImagesToData(data);
        //    //        }
        //    //    });
        //    throw new PBException("not implemented");
        //}

        public int Update(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            //int nb = _documentStore.Update(updateDocument, query, sort, limit);
            //pb.Trace.WriteLine("{0} document(s) updated", nb);
            //return nb;
            throw new PBException("not implemented");
        }

        public int Refresh(Action<TData, TData> action = null, int limit = 0, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false)
        {
            //int nb = 0;
            //foreach (TData data in Find(query, sort: sort, limit: limit, loadImage: false))
            //{
            //    if (!(data is IHttpRequestData))
            //        throw new PBException("type {0} is not IWebData", data.GetType().zGetTypeName());
            //    //TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage });
            //    //TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage } });
            //    TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage } }).Data;

            //    if (action != null)
            //        action(data, data2);
            //    if (_documentStore.GenerateId)
            //        _documentStore.SaveWithId(data2.zGetId(), data2);
            //    else
            //        _documentStore.SaveWithKey(data2.zGetKey_v2(), data2);
            //    nb++;
            //}
            //return nb;
            throw new PBException("not implemented");
        }

        public IEnumerable<string> Backup(string directory)
        {
            //if (_documentStore != null)
            //    return _documentStore.Backup(directory);
            //else
            //    return new string[0];
            throw new PBException("not implemented");
        }

        public BsonValue _GetKeyFromHttpRequest(HttpRequest httpRequest)
        {
            if (_getKeyFromHttpRequest != null)
                return _getKeyFromHttpRequest(httpRequest);
            else
                throw new PBException("WebDataManager.GetKeyFromHttpRequest function is not defined");
        }

        //private void _LoadImages(TData data)
        //{
        //    if (_loadImages != null)
        //        _loadImages(data);
        //    else
        //        throw new PBException("WebDataManager.LoadImages function is not defined");
        //}

        //private void LoadImages(TData data)
        //{
        //    LoadImages(data, new WebImageRequest { LoadImageToData = true });
        //}

        //public void LoadImages_v1(TData data, WebImageRequest request)
        //{
        //    if (request.LoadImageFromWeb || request.LoadImageToData || request.RefreshImage)
        //    {
        //        if (!(data is ILoadImages))
        //            throw new PBException($"{typeof(TData).zGetTypeName()} is not ILoadImages");
        //        ((ILoadImages)data).LoadImages(request);
        //    }
        //}
    }
}

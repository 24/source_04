using System;
using System.Collections.Generic;
using MongoDB.Bson;
using pb.Data;
using pb.Reflection;
using pb.Web.Http;

namespace pb.Web.Data
{
    // WebDataManager_v2<TData> :
    //   load data from web and save data to store
    //   use BsonValue key to identify data
    //   function _getKeyFromHttpRequest retrieve key from HttpRequest
    //   operation : Load(), LoadFromWeb(), Exists(), Save(), Find(), Update(), Refresh()

    // modif :
    //   - WebData<TData>.Load() replace Load_v1()
    //   - [suppression] WebData<TData>.Load_v2() using MongoDataStore instead of IDocumentStore<TData>
    //   - [suppression] use WebLoadDataManager_v2 instead of WebLoadDataManager
    //   - WebDataManager _version : 1 use Load_v1(), 2 use WebData<TData>.Load(), [suppression] 3 use MongoDataStore, [suppression] 4 use WebLoadDataManager_v2
    public partial class WebDataManager<TData>
    {
        protected int _version = 1;
        protected int _imageLoadVersion = 1;
        protected WebLoadDataManager<TData> _webLoadDataManager = null;
        //protected WebLoadDataManager_v2<TData> _webLoadDataManager_v2 = null;
        protected IDocumentStore<TData> _documentStore = null;
        protected bool _desactivateDocumentStore = false;
        protected bool _generateId = false;
        protected Func<HttpRequest, BsonValue> _getKeyFromHttpRequest = null;
        //protected Action<TData> _loadImages = null;
        protected WebLoadImageManager_v2<TData> _webLoadImageManager = null;
        protected Func<WebData<TData>, string> _getImageSubDirectory = null;

        public int Version { get { return _version; } set { _version = value; } }
        public int ImageLoadVersion { get { return _imageLoadVersion; } set { _imageLoadVersion = value; } }
        public WebLoadDataManager<TData> WebLoadDataManager { get { return _webLoadDataManager; } set { _webLoadDataManager = value; } }
        //public WebLoadDataManager_v2<TData> WebLoadDataManager_v2 { get { return _webLoadDataManager_v2; } set { _webLoadDataManager_v2 = value; } }
        public IDocumentStore<TData> DocumentStore { get { return _documentStore; } set { _documentStore = value; } }
        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }
        public Func<HttpRequest, BsonValue> GetKeyFromHttpRequest { get { return _getKeyFromHttpRequest; } set { _getKeyFromHttpRequest = value; } }
        public WebLoadImageManager_v2<TData> WebLoadImageManager { get { return _webLoadImageManager; } set { _webLoadImageManager = value; } }
        public Func<WebData<TData>, string> GetImageSubDirectory { get { return _getImageSubDirectory; } set { _getImageSubDirectory = value; } }

        //[Obsolete]
        //public Action<TData> LoadImages { get { return _loadImages; } set { _loadImages = value; } }

        //public WebDataManager()
        //{
        //    InitSerializer();
        //}

        public WebData<TData> Load(WebRequest request)
        {
            if (_version == 1)
                return Load_v1(request);
            else if (_version == 2)
                return WebData<TData>.Load_v1(this, request);
            //else // if (_version == 3)
            //    return WebData<TData>.Load_v2(this, request);
            throw new PBException($"bad version {_version}");
        }

        public WebData<TData> Load_v1(WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(request);
            if (_documentStore != null && _documentStore.GenerateId)
            {
                BsonValue id = GetId(webData);
                if (request.ReloadFromWeb || request.RefreshDocumentStore || id == null)
                {
                    _LoadFromWeb(webData);
                    if (id == null)
                        id = _documentStore.GetNewId();
                    SetDataId(webData, id);
                    SaveWithId(id, webData);
                    //if (_imageLoadVersion == 1)
                    if (_webLoadImageManager == null)
                        //LoadImages_v1(webData.Document, request.ImageRequest);
                        WebLoadImageManager_v1.LoadImages(webData.Data, request.ImageRequest);
                    else
                    {
                        //if (_webLoadImageManager.LoadImagesFromWeb(webData))
                        if (webData.Data is IGetWebImages && _webLoadImageManager.LoadImagesFromWeb(request.ImageRequest, ((IGetWebImages)webData.Data).GetWebImages(), _getImageSubDirectory?.Invoke(webData)))
                            SaveWithId(id, webData);
                    }
                }
            }
            //else
            //{
            //    // todo : remplacer Exists() par GetKey() puis Exists(key)
            //    if (request.ReloadFromWeb || request.RefreshDocumentStore || !Exists(webData))
            //    {
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
            //}
            else if (request.ReloadFromWeb || request.RefreshDocumentStore || !Exists(webData))
            {
                // todo : remplacer Exists() par GetKey() puis Exists(key)
                _LoadFromWeb(webData);
                SaveWithKey(webData);
                //if (_imageLoadVersion == 1)
                if (_webLoadImageManager == null)
                    //LoadImages_v1(webData.Document, webData.Request.ImageRequest);
                    WebLoadImageManager_v1.LoadImages(webData.Data, webData.Request.ImageRequest);
                else
                {
                    //if (_webLoadImageManager.LoadImagesFromWeb(webData))
                    if (webData.Data is IGetWebImages && _webLoadImageManager.LoadImagesFromWeb(request.ImageRequest, ((IGetWebImages)webData.Data).GetWebImages(), _getImageSubDirectory?.Invoke(webData)))
                        SaveWithKey(webData);
                }
            }

            _Load(webData);
            return webData;
        }

        public WebData<TData> LoadFromWeb(WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(request);
            _LoadFromWeb(webData);
            //if (_imageLoadVersion == 1)
            if (_webLoadImageManager == null)
                //LoadImages_v1(webData.Document, webData.Request.ImageRequest);
                WebLoadImageManager_v1.LoadImages(webData.Data, webData.Request.ImageRequest);
            else
            {
                //_webLoadImageManager.LoadImagesFromWeb(webData);
                if (webData.Data is IGetWebImages)
                    _webLoadImageManager.LoadImagesFromWeb(request.ImageRequest, ((IGetWebImages)webData.Data).GetWebImages(), _getImageSubDirectory?.Invoke(webData));
                _webLoadImageManager.LoadImagesToData(webData.Data);
            }
            return webData;
        }

        protected bool Exists(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
                return _documentStore.Exists(key);
            }
            else
                return false;
        }

        protected BsonValue GetId(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
                return _documentStore.GetId(key);
            }
            else
                return null;
        }

        protected void SetDataId(WebData<TData> webData, BsonValue id)
        {
            webData.Data.zSetId(id);
        }

        protected void _Load(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!webData.DataLoaded)
                {
                    BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
                    webData.Data = _documentStore.LoadFromKey(key);

                    //LoadImages(webData);
                    if (webData.Request.ImageRequest.LoadImageToData)
                    {
                        //if (_imageLoadVersion == 1)
                        if (_webLoadImageManager == null)
                            //LoadImages_v1(webData.Document, webData.Request.ImageRequest);
                            WebLoadImageManager_v1.LoadImages(webData.Data, webData.Request.ImageRequest);
                        else
                            //LoadImagesToData(webData.Document);
                            _webLoadImageManager.LoadImagesToData(webData.Data);
                    }

                    //if (webData.Request.LoadImageFromWeb || webData.Request.LoadImageToData || webData.Request.RefreshImage)
                    //    LoadImages(webData.Document, WebImageRequest.FromWebRequest(webData.Request));

                    //    if (!(webData.Document is ILoadImages))
                    //        throw new PBException($"{typeof(TData).zGetTypeName()} is not ILoadImages");
                    //    ((ILoadImages)webData.Document).LoadImages(ImageRequest.FromWebRequest(webData.Request));
                    webData.DataLoaded = true;
                    webData.DataLoadedFromStore = true;
                }
            }
            else
                _LoadFromWeb(webData);
        }

        public void SaveWithId(BsonValue id, WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                _documentStore.SaveWithId(id, webData.Data);
            }
        }

        public void SaveWithKey(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                _documentStore.SaveWithKey(webData.Data.zGetKey_v2(), webData.Data);
            }
        }

        protected void _LoadFromWeb(WebData<TData> webData)
        {
            if (!webData.DataLoaded)
            {
                //loadWebData.Document = _webLoadDataManager.LoadData(loadWebData.Request);
                WebDataResult<TData> webDataResult = _webLoadDataManager.LoadData(webData.Request);
                webData.Result = webDataResult.Result;
                webData.Data = webDataResult.Data;
                webData.DataLoaded = true;
                webData.DataLoadedFromWeb = true;
            }
        }

        public IEnumerable<TData> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            return _documentStore.Find(query, sort: sort, limit: limit).zAction(
                data =>
                {
                    if (loadImage)
                    {
                        //if (_imageLoadVersion == 1)
                        if (_webLoadImageManager == null)
                            //LoadImages_v1(data, new WebImageRequest { LoadImageToData = true });
                            WebLoadImageManager_v1.LoadImages(data, new WebImageRequest { LoadImageToData = true });
                        else
                            //LoadImagesToData(data);
                            _webLoadImageManager.LoadImagesToData(data);
                    }
                });
        }

        public int Update(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = _documentStore.Update(updateDocument, query, sort, limit);
            pb.Trace.WriteLine("{0} document(s) updated", nb);
            return nb;
        }

        public int Refresh(Action<TData, TData> action = null, int limit = 0, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false)
        {
            int nb = 0;
            foreach (TData data in Find(query, sort: sort, limit: limit, loadImage: false))
            {
                if (!(data is IHttpRequestData))
                    throw new PBException("type {0} is not IWebData", data.GetType().zGetTypeName());
                //TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage });
                //TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage } });
                TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage } }).Data;

                if (action != null)
                    action(data, data2);
                if (_documentStore.GenerateId)
                    _documentStore.SaveWithId(data2.zGetId(), data2);
                else
                    _documentStore.SaveWithKey(data2.zGetKey_v2(), data2);
                nb++;
            }
            return nb;
        }

        public IEnumerable<string> Backup(string directory)
        {
            if (_documentStore != null)
                return _documentStore.Backup(directory);
            else
                return new string[0];
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

        // moved to WebLoadImageManager_v1
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

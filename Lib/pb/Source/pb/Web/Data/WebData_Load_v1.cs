using MongoDB.Bson;
using pb.Data;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private WebDataManager<TData> _webDataManager = null;
        //private WebLoadDataManager<TData> _webLoadDataManager = null;
        private IDocumentStore<TData> _documentStore = null;
        //private Func<HttpRequest, BsonValue> _getKeyFromHttpRequest = null;
        private WebLoadImageManager_v2<TData> _webLoadImageManager = null;

        private WebData(WebDataManager<TData> webDataManager, WebRequest request)
        {
            _webDataManager = webDataManager;
            _request = request;
        }

        private void Load_v1()
        {
            BsonValue id = null;
            BsonValue key = null;
            bool dataExists = false;
            _documentStore = _webDataManager.DocumentStore;
            _webLoadImageManager = _webDataManager.WebLoadImageManager;
            bool useDocumentStore = _documentStore != null && !_webDataManager.DesactivateDocumentStore;
            if (useDocumentStore)
            {
                key = _webDataManager._GetKeyFromHttpRequest(_request.HttpRequest);
                //key = _getKeyFromHttpRequest(_request.HttpRequest);
                if (_documentStore.GenerateId)
                {
                    //id = GetId(webData);
                    id = _documentStore.GetId(key);
                    if (id != null)
                        dataExists = true;
                }
                else
                    //dataExists = Exists(webData);
                    dataExists = _documentStore.Exists(key);
            }

            if (_documentStore == null || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            {
                _LoadFromWeb_v1(_webDataManager.WebLoadDataManager);
            }
            else
            {
                _LoadFromDocumentStore(key);
            }

            if (_documentStore != null && _documentLoadedFromWeb)
            {
                if (_documentStore.GenerateId)
                {
                    if (id == null)
                        id = _documentStore.GetNewId();
                    //SetDataId(webData, id);
                    _document.zSetId(id);
                    //SaveWithId(id, webData);
                    _documentStore.SaveWithId(id, _document);
                }
                else
                {
                    //SaveWithKey(webData);
                    _documentStore.SaveWithKey(key, _document);
                }
            }

            WebImageRequest imageRequest = _request.ImageRequest;
            //if (_webDataManager.ImageLoadVersion == 1)
            if (_webLoadImageManager == null)
            {
                if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                    || (!_documentLoadedFromWeb && (imageRequest.LoadImageToData || imageRequest.LoadMissingImageFromWeb)))
                {
                    //_webDataManager.LoadImages_v1(_document, _request.ImageRequest);
                    WebLoadImageManager_v1.LoadImages(_document, _request.ImageRequest);
                }
            }
            else // _webDataManager.ImageLoadVersion == 2
            {
                //if (_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.LoadMissingImageFromWeb || imageRequest.RefreshImage))
                if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                    || (!_documentLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
                {
                    //if (_webDataManager.LoadImagesFromWeb(this))
                    if (_webLoadImageManager.LoadImagesFromWeb(this))
                    {
                        if (id != null)
                            _documentStore.SaveWithId(id, _document);
                        else
                            _documentStore.SaveWithKey(key, _document);
                    }
                }

                if (imageRequest.LoadImageToData)
                {
                    //_webDataManager.LoadImagesToData(_document);
                    _webLoadImageManager.LoadImagesToData(_document);
                }
            }
        }

        private void _LoadFromWeb_v1(WebLoadDataManager<TData> webLoadDataManager)
        {
            //if (_webDataManager.WebLoadDataManager != null)
            //{
            //WebDataResult<TData> webDataResult = _webDataManager.WebLoadDataManager.LoadData(_request);
            //WebDataResult<TData> webDataResult = _webLoadDataManager.LoadData(_request);
            WebDataResult<TData> webDataResult = webLoadDataManager.LoadData(_request);
            _result = webDataResult.Result;
            _document = webDataResult.Data;
            _documentLoaded = true;
            _documentLoadedFromWeb = true;
            //}
            //else // if (_webDataManager.WebLoadDataManager_v2 != null)
            //{
            //    // add error _LoadFromWeb
            //    WebDataResult_v2<TData> webDataResult = _webDataManager.WebLoadDataManager_v2.LoadData(_request);
            //    _result_v2 = webDataResult.Result;
            //    _error = !webDataResult.Success;
            //    _document = webDataResult.Data;
            //    _documentLoaded = true;
            //    _documentLoadedFromWeb = true;
            //}
        }

        private void _LoadFromDocumentStore(BsonValue key)
        {
            //_document = _webDataManager.DocumentStore.LoadFromKey(key);
            _document = _documentStore.LoadFromKey(key);
            _documentLoaded = true;
            _documentLoadedFromStore = true;
        }

        public static WebData<TData> Load_v1(WebDataManager<TData> webDataManager, WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(webDataManager, request);
            webData.Load_v1();
            return webData;
        }
    }
}

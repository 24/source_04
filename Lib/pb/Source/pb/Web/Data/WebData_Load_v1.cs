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
        //private WebLoadImageManager_v2<TData> _webLoadImageManager = null;

        private WebData(WebDataManager<TData> webDataManager, WebRequest request)
        {
            _webDataManager = webDataManager;
            _request = request;
        }

        private void Load_v1()
        {
            //BsonValue id = null;
            //BsonValue key = null;
            bool dataExists = false;
            _documentStore = _webDataManager.DocumentStore;
            //_webLoadImageManager = _webDataManager.WebLoadImageManager;
            bool useDocumentStore = _documentStore != null && !_webDataManager.DesactivateDocumentStore;
            if (useDocumentStore)
            {
                _key = _webDataManager._GetKeyFromHttpRequest(_request.HttpRequest);
                if (_documentStore.GenerateId)
                {
                    _id = _documentStore.GetId(_key);
                    if (_id != null)
                        dataExists = true;
                }
                else
                    dataExists = _documentStore.Exists(_key);
            }

            if (_documentStore == null || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            {
                _LoadFromWeb_v1(_webDataManager.WebLoadDataManager);
            }
            else
            {
                _LoadFromDocumentStore(_key);
            }

            if (_documentStore != null && _dataLoadedFromWeb)
            {
                if (_documentStore.GenerateId)
                {
                    if (_id == null)
                        _id = _documentStore.GetNewId();
                    _data.zSetId(_id);
                    _documentStore.SaveWithId(_id, _data);
                }
                else
                {
                    _documentStore.SaveWithKey(_key, _data);
                }
            }

            //WebImageRequest imageRequest = _request.ImageRequest;
            //if (_webLoadImageManager == null)
            //{
            //    if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
            //        || (!_documentLoadedFromWeb && (imageRequest.LoadImageToData || imageRequest.LoadMissingImageFromWeb)))
            //    {
            //        WebLoadImageManager_v1.LoadImages(_document, _request.ImageRequest);
            //    }
            //}
            //else
            //{
            //    if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
            //        || (!_documentLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
            //    {
            //        if (_webLoadImageManager.LoadImagesFromWeb(this))
            //        {
            //            if (id != null)
            //                _documentStore.SaveWithId(id, _document);
            //            else
            //                _documentStore.SaveWithKey(key, _document);
            //        }
            //    }

            //    if (imageRequest.LoadImageToData)
            //    {
            //        _webLoadImageManager.LoadImagesToData(_document);
            //    }
            //}
            //LoadImages_v1(_request.ImageRequest);
            LoadImages_v2(_request.ImageRequest);
        }

        private void _LoadFromWeb_v1(WebLoadDataManager<TData> webLoadDataManager)
        {
            //if (_webDataManager.WebLoadDataManager != null)
            //{
            //WebDataResult<TData> webDataResult = _webDataManager.WebLoadDataManager.LoadData(_request);
            //WebDataResult<TData> webDataResult = _webLoadDataManager.LoadData(_request);
            WebDataResult<TData> webDataResult = webLoadDataManager.LoadData(_request);
            _result = webDataResult.Result;
            _data = webDataResult.Data;
            _dataLoaded = true;
            _dataLoadedFromWeb = true;
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
            _data = _documentStore.LoadFromKey(key);
            _dataLoaded = true;
            _dataLoadedFromStore = true;
        }

        public static WebData<TData> Load_v1(WebDataManager<TData> webDataManager, WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(webDataManager, request);
            webData.Load_v1();
            return webData;
        }
    }
}

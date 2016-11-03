using MongoDB.Bson;
using pb.Data;
using pb.Data.Mongo;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private MongoDataStore _dataStore = null;
        private WebDataManager_v4<TData> _webDataManager_v4 = null;

        private WebData(WebDataManager_v4<TData> webDataManager, WebRequest request)
        {
            _webDataManager_v4 = webDataManager;
            _request = request;
        }

        private void Load_v2()
        {
            BsonValue id = null;
            BsonValue key = null;
            bool dataExists = false;
            _dataStore = _webDataManager_v4.DataStore;
            _webLoadImageManager = _webDataManager_v4.WebLoadImageManager;
            bool useDocumentStore = _dataStore != null && !_webDataManager_v4.DesactivateDocumentStore;
            if (useDocumentStore)
            //if (_dataStore != null)
            {
                key = _webDataManager_v4._GetKeyFromHttpRequest(_request.HttpRequest);
                //key = _getKeyFromHttpRequest(_request.HttpRequest);
                if (_dataStore.GenerateId)
                {
                    //id = GetId(webData);
                    id = _dataStore.GetId(key);
                    if (id != null)
                        dataExists = true;
                }
                else
                    //dataExists = Exists(webData);
                    dataExists = _dataStore.Exists(key);
            }

            //if (!useDocumentStore || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            if (_dataStore == null || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            {
                _LoadFromWeb_v2(_webDataManager_v4.WebLoadDataManager);
            }
            else
            {
                _LoadFromDocumentStore_v2(key);
            }

            // _error is not used actually
            //if (_error)
            //    return;

            //if (useDocumentStore && _documentLoadedFromWeb)
            if (_dataStore != null && _documentLoadedFromWeb)
            {
                BsonDocument data = Serialize();
                if (_dataStore.GenerateId)
                {
                    if (id == null)
                        id = _dataStore.GetNewId();
                    _document.zSetId(id);
                    _dataStore.SaveWithId(id, data);
                }
                else
                {
                    _dataStore.SaveWithKey(key, data);
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
                        BsonDocument data = Serialize();
                        if (id != null)
                            //SaveWithId(id, webData);
                            _dataStore.SaveWithId(id, data);
                        else
                            //SaveWithKey(webData);
                            _dataStore.SaveWithKey(key, data);
                    }
                }

                if (imageRequest.LoadImageToData)
                {
                    //_webDataManager.LoadImagesToData(_document);
                    _webLoadImageManager.LoadImagesToData(_document);
                }
            }
        }

        private void _LoadFromWeb_v2(WebLoadDataManager_v2<TData> webLoadDataManager)
        {
            //if (_webDataManager.WebLoadDataManager != null)
            //{
            //WebDataResult<TData> webDataResult = _webDataManager.WebLoadDataManager.LoadData(_request);
            //WebDataResult<TData> webDataResult = _webLoadDataManager.LoadData(_request);
            WebDataResult_v2<TData> webDataResult = webLoadDataManager.LoadData(_request);
            _result_v2 = webDataResult.Result;
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

        private void _LoadFromDocumentStore_v2(BsonValue key)
        {
            BsonDocument document = _dataStore.LoadFromKey(key);
            Deserialize(document);
            _documentLoaded = true;
            _documentLoadedFromStore = true;
        }

        private BsonDocument Serialize()
        {
            BsonDocument document = new BsonDocument { _webDataManager_v4.DataSerializer.Serialize(_document) };
            if (_result_v2 != null)
            {
                document.Add(_webDataManager_v4.WebRequestSerializer.Serialize(_result_v2.Http.GetHttpLog()));
            }
            return document;
        }

        private void Deserialize(BsonDocument document)
        {
            _document = _webDataManager_v4.DataSerializer.Deserialize(document);
            HttpLog httpLog = _webDataManager_v4.WebRequestSerializer.Deserialize(document);
            _result_v2 = new HttpResult<string> { Http = new Http_v2(httpLog) };
        }

        //WebDataManager_v4<TData> _webDataManager_v4
        public static WebData<TData> Load_v2(WebDataManager_v4<TData> webDataManager, WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(webDataManager, request);
            webData.Load_v2();
            return webData;
        }
    }
}

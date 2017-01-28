using MongoDB.Bson;
using pb.Data;
using pb.Data.Mongo;
using pb.Web.Http;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private MongoDataStore _dataStore = null;
        private WebDataManager_v4<TData> _webDataManager_v4 = null;
        //private BsonValue _id = null;
        //private BsonValue _key = null;

        private WebData(WebDataManager_v4<TData> webDataManager, WebRequest request)
        {
            _webDataManager_v4 = webDataManager;
            _request = request;
        }

        private void Load_v2()
        {
            //BsonValue id = null;
            //BsonValue key = null;
            bool dataExists = false;
            _dataStore = _webDataManager_v4.DataStore;
            //_webLoadImageManager = _webDataManager_v4.WebLoadImageManager;
            bool useDocumentStore = _dataStore != null && !_webDataManager_v4.DesactivateDocumentStore;
            if (useDocumentStore)
            {
                _key = _webDataManager_v4._GetKeyFromHttpRequest(_request.HttpRequest);
                if (_dataStore.GenerateId)
                {
                    _id = _dataStore.GetId(_key);
                    if (_id != null)
                        dataExists = true;
                }
                else
                    dataExists = _dataStore.Exists(_key);
            }

            if (_dataStore == null || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            {
                _LoadFromWeb_v2(_webDataManager_v4.WebLoadDataManager);
                if (!_result_v2.Success)
                    return;
            }
            else
            {
                _LoadFromDocumentStore_v2(_key);
            }

            // _error is not used actually
            //if (_error)
            //    return;

            if (_dataStore != null && _dataLoadedFromWeb)
            {
                BsonDocument document = Serialize();
                if (_dataStore.GenerateId)
                {
                    if (_id == null)
                        _id = _dataStore.GetNewId();
                    _data.zSetId(_id);
                    _dataStore.SaveWithId(_id, document);
                }
                else
                {
                    _dataStore.SaveWithKey(_key, document);
                }
            }

            //if (_webLoadImageManager == null)
            //    LoadImages_v1(_request.ImageRequest);
            //else
            //    LoadImages_v3(_request.ImageRequest);
            //LoadImages_v1(_request.ImageRequest);
            LoadImages_v3(_request.ImageRequest);
        }

        private void _LoadFromWeb_v2(WebLoadDataManager_v2<TData> webLoadDataManager)
        {
            //if (_webDataManager.WebLoadDataManager != null)
            //{
            //WebDataResult<TData> webDataResult = _webDataManager.WebLoadDataManager.LoadData(_request);
            //WebDataResult<TData> webDataResult = _webLoadDataManager.LoadData(_request);
            WebDataResult_v2<TData> webDataResult = webLoadDataManager.LoadData(_request);
            _result_v2 = webDataResult.Result;
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

        private void _LoadFromDocumentStore_v2(BsonValue key)
        {
            BsonDocument document = _dataStore.LoadFromKey(key);
            Deserialize(document);
            _dataLoaded = true;
            _dataLoadedFromStore = true;
        }

        private BsonDocument Serialize()
        {
            BsonDocument document = new BsonDocument { _webDataManager_v4.DataSerializer.Serialize(_data) };
            if (_result_v2 != null)
            {
                document.Add(_webDataManager_v4.WebRequestSerializer.Serialize(_result_v2.Http.GetHttpLog()));
            }
            return document;
        }

        private void Deserialize(BsonDocument document)
        {
            _data = _webDataManager_v4.DataSerializer.Deserialize(document);
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

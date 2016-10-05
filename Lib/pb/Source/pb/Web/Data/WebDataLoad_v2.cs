using MongoDB.Bson;
using pb.Data;
using pb.Data.Mongo;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        MongoDataStore _dataStore = null;

        private void Load_v2()
        {
            BsonValue id = null;
            BsonValue key = null;
            bool dataExists = false;
            //IDocumentStore<TData> documentStore = _webDataManager.DocumentStore;
            _dataStore = _webDataManager.DataStore;
            bool useDocumentStore = _dataStore != null && !_webDataManager.DesactivateDocumentStore;
            if (useDocumentStore)
            {
                key = _webDataManager._GetKeyFromHttpRequest(_request.HttpRequest);
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

            if (!useDocumentStore || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            {
                _LoadFromWeb();
            }
            else
            {
                _LoadFromDocumentStore_v2(key);
            }

            if (_error)
                return;

            if (useDocumentStore && _documentLoadedFromWeb)
            {
                BsonDocument data = Serialize();
                if (_dataStore.GenerateId)
                {
                    if (id == null)
                        id = _dataStore.GetNewId();
                    //SetDataId(webData, id);
                    _document.zSetId(id);
                    //SaveWithId(id, webData);
                    _dataStore.SaveWithId(id, data);
                }
                else
                {
                    //SaveWithKey(webData);
                    _dataStore.SaveWithKey(key, data);
                }
            }


            WebImageRequest imageRequest = _request.ImageRequest;
            if (_webDataManager.ImageLoadVersion == 1)
            {
                if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                    || (!_documentLoadedFromWeb && (imageRequest.LoadImageToData || imageRequest.LoadMissingImageFromWeb)))
                {
                    _webDataManager.LoadImages_v1(_document, _request.ImageRequest);
                }
            }
            else // _webDataManager.ImageLoadVersion == 2
            {
                //if (_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.LoadMissingImageFromWeb || imageRequest.RefreshImage))
                if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                    || (!_documentLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
                {
                    if (_webDataManager.LoadImagesFromWeb(this))
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
                    _webDataManager.LoadImagesToData(_document);
                }
            }
        }

        private void _LoadFromDocumentStore_v2(BsonValue key)
        {
            //_document = _webDataManager.DocumentStore.LoadFromKey(key);
            BsonDocument document = _dataStore.LoadFromKey(key);
            Deserialize(document);
            _documentLoaded = true;
            _documentLoadedFromStore = true;
        }

        private BsonDocument Serialize()
        {
            BsonDocument document = new BsonDocument { _webDataManager.DataSerializer.Serialize(_document) };
            //_webDataManager.WebRequestSerializer.Serialize(_result)
            if (_result_v2 != null)
            {
                document.Add(_webDataManager.WebRequestSerializer.Serialize(_result_v2.Http.GetHttpLog()));
            }
            return document;
        }

        private void Deserialize(BsonDocument document)
        {
            _document = _webDataManager.DataSerializer.Deserialize(document);
            HttpLog httpLog = _webDataManager.WebRequestSerializer.Deserialize(document);
            _result_v2 = new HttpResult<string> { Http = new Http_v2(httpLog) };
        }

        public static WebData<TData> Load_v2(WebDataManager<TData> webDataManager, WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(webDataManager, request);
            webData.Load_v2();
            return webData;
        }
    }
}

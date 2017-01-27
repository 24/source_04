using MongoDB.Bson;
using pb.Data;
using pb.Data.Mongo;
using pb.Web.Http;
using System;
using System.Collections.Generic;

namespace pb.Web.Data
{
    public interface INamedHttpRequest
    {
        HttpRequest GetHttpRequest(string name);
    }

    public class NamedGetData<TData>
    {
        public string Name;
        public Action<TData, HttpResult<string>> GetData;
    }

    // from WebDataManager_v4 : able to load detail from 2 or more url
    // TSource must implement IKeyDetail and INamedHttpRequest
    // TData should implement IGetWebImages to load images
    public class WebDataManager_v5<TSource, TData> where TSource: IKeyDetail, INamedHttpRequest
    {
        protected HttpManager_v2 _httpManager = null;
        protected Func<TData> _createData = null;
        protected Dictionary<string, Action<TData, HttpResult<string>>> _getDatas = new Dictionary<string, Action<TData, HttpResult<string>>>();
        protected MongoDataStore _store = null;
        protected MongoDataSerializer<TData> _dataSerializer = null;
        protected MongoDataSerializer<HttpLog> _webRequestSerializer = null;
        protected bool _desactivateDocumentStore = false;
        protected WebLoadImageManager_v2<TData> _webLoadImageManager = null;
        protected Func<WebData_v2<TSource, TData>, string> _getImageSubDirectory = null;

        public HttpManager_v2 HttpManager { get { return _httpManager; } set { _httpManager = value; } }
        public Func<TData> CreateData { get { return _createData; } set { _createData = value; } }
        //public Dictionary<string, Action<TData, HttpResult<string>>> GetDatas { get { return _getDatas; } }
        public MongoDataStore Store { get { return _store; } set { _store = value; } }
        public MongoDataSerializer<TData> DataSerializer { get { return _dataSerializer; } set { _dataSerializer = value; } }
        public MongoDataSerializer<HttpLog> WebRequestSerializer { get { return _webRequestSerializer; } }
        public WebLoadImageManager_v2<TData> WebLoadImageManager { get { return _webLoadImageManager; } set { _webLoadImageManager = value; } }
        public Func<WebData_v2<TSource, TData>, string> GetImageSubDirectory { get { return _getImageSubDirectory; } set { _getImageSubDirectory = value; } }

        public WebDataManager_v5()
        {
            InitSerializer();
        }

        protected void InitSerializer()
        {
            _webRequestSerializer = new MongoDataSerializer<HttpLog>();
            _webRequestSerializer.ItemName = "WebRequest";
        }

        public void AddNamedGetDatas(IEnumerable<NamedGetData<TData>> namedGetDatas)
        {
            foreach (NamedGetData<TData> namedGetData in namedGetDatas)
                _getDatas.Add(namedGetData.Name, namedGetData.GetData);
        }

        public WebData_v2<TSource, TData> Load(WebRequest_v2<TSource> request)
        {
            //Trace.WriteLine();
            //Trace.WriteLine($"WebDataManager_v5.Load()");

            WebData_v2<TSource, TData> webData = new WebData_v2<TSource, TData>(request);
            bool dataExist = DataExist(webData);


            if (_store == null || !dataExist || request.ReloadFromWeb || request.RefreshDocumentStore)
                LoadFromWeb(webData);
            else
                LoadFromStore(webData);

            if (_store != null && webData.DataLoadedFromWeb)
                SaveToStore(webData);

            LoadImages(webData);

            return webData;
        }

        protected bool DataExist(WebData_v2<TSource, TData> webData)
        {
            bool dataExists = false;
            bool useDocumentStore = _store != null && !_desactivateDocumentStore;
            if (useDocumentStore)
            {
                webData.Key = GetDataKey(webData.Request.Data);
                if (_store.GenerateId)
                {
                    webData.Id = _store.GetId(webData.Key);
                    if (webData.Id != null)
                        dataExists = true;
                }
                else
                    dataExists = _store.Exists(webData.Key);
            }
            return dataExists;
        }

        protected BsonValue GetDataKey(TSource data)
        {
            return data.GetDetailKey();
        }

        protected void LoadFromWeb(WebData_v2<TSource, TData> webData)
        {
            //Trace.WriteLine($"WebDataManager_v5.LoadFromWeb()");

            TData data = webData.Data = _createData();
            TSource source = webData.Request.Data;
            foreach (KeyValuePair<string, Action<TData, HttpResult<string>>> value in _getDatas)
            {
                HttpRequest httpRequest = GetHttpRequest(source, value.Key);
                httpRequest.ReloadFromWeb = webData.Request.ReloadFromWeb;
                HttpResult<string> httpResult = _httpManager.LoadText(httpRequest);
                if (httpResult.Success)
                    value.Value(data, httpResult);
                webData.HttpResults.Add(value.Key, httpResult);
            }
            webData.DataLoaded = true;
            webData.DataLoadedFromWeb = true;

            if (webData.Id != null)
                webData.Data.zSetId(webData.Id);
        }

        protected HttpRequest GetHttpRequest(TSource source, string name)
        {
            return source.GetHttpRequest(name);
        }

        protected void LoadFromStore(WebData_v2<TSource, TData> webData)
        {
            //Trace.WriteLine($"WebDataManager_v5.LoadFromStore()");

            BsonDocument document = _store.LoadFromKey(webData.Key);
            Deserialize(webData, document);
            webData.DataLoaded = true;
            webData.DataLoadedFromStore = true;
        }

        protected void SaveToStore(WebData_v2<TSource, TData> webData)
        {
            //Trace.WriteLine($"WebDataManager_v5.SaveToStore()");

            BsonDocument document = Serialize(webData);
            if (_store.GenerateId)
            {
                BsonValue id = webData.Id;
                if (id == null)
                {
                    webData.Id = id = _store.GetNewId();
                    webData.Data.zSetId(id);
                }
                _store.SaveWithId(id, document);
            }
            else
                _store.SaveWithKey(webData.Key, document);
        }

        protected BsonDocument Serialize(WebData_v2<TSource, TData> webData)
        {
            BsonDocument document = new BsonDocument { _dataSerializer.Serialize(webData.Data) };
            //if (webData.HttpResults.Count == 1)
            //{
            //    document.Add(_webRequestSerializer.Serialize(webData.HttpResults.First().Value.Http.GetHttpLog()));
            //}
            //else
            //{
                BsonDocument httpDocument = new BsonDocument();
                foreach (KeyValuePair<string, HttpResult<string>> value in webData.HttpResults)
                {
                    if (value.Value.Http != null)
                        httpDocument.Add(new BsonElement(value.Key, _webRequestSerializer.SerializeAsDocument(value.Value.Http.GetHttpLog())));
                }
                string itemName = _webRequestSerializer.ItemName;
                if (itemName == null)
                    throw new PBException("web request serializer item name is not defined");
                document.Add(new BsonElement(itemName, httpDocument));
            //}
            return document;
        }

        protected void Deserialize(WebData_v2<TSource, TData> webData, BsonDocument document)
        {
            webData.Data = _dataSerializer.Deserialize(document);

            string itemName = _webRequestSerializer.ItemName;
            if (itemName == null)
                throw new PBException("web request serializer item name is not defined");
            if (!document.Contains(itemName))
                throw new PBException($"deserialize web request : serializer document does'nt contain element \"{itemName}\"");
            var element = document[itemName];
            if (element == null)
                throw new PBException($"deserialize web request : element \"{itemName}\" is null");
            if (!(element is BsonDocument))
                throw new PBException($"deserialize web request : element \"{itemName}\" is not a document");
            BsonDocument httpDocument = element as BsonDocument;
            foreach (BsonElement httpElement in httpDocument)
            {
                if (!(httpElement.Value is BsonDocument))
                    throw new PBException($"deserialize web request : element \"{httpElement.Name}\" is not a document");
                HttpLog httpLog = _webRequestSerializer.DeserializeData((BsonDocument)httpElement.Value);
                webData.HttpResults.Add(httpElement.Name, new HttpResult<string> { Http = new Http_v2(httpLog) });
            }
        }

        protected void LoadImages(WebData_v2<TSource, TData> webData)
        {
            WebImageRequest imageRequest = webData.Request.ImageRequest;
            if (imageRequest == null)
                return;
            if ((webData.DataLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                || (!webData.DataLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
            {
                if (webData.Data is IGetWebImages)
                {
                    if (_webLoadImageManager.LoadImagesFromWeb(imageRequest, ((IGetWebImages)webData.Data).GetWebImages(), _getImageSubDirectory?.Invoke(webData)))
                    {
                        SaveToStore(webData);
                    }
                }
            }

            if (imageRequest.LoadImageToData)
            {
                _webLoadImageManager.LoadImagesToData(webData.Data);
            }
        }

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
    }
}

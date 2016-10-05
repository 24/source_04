using MongoDB.Bson;
using pb.Data;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private WebDataManager<TData> _webDataManager = null;

        private WebData(WebDataManager<TData> webDataManager, WebRequest request)
        {
            _webDataManager = webDataManager;
            _request = request;
        }

        private void Load()
        {
            BsonValue id = null;
            BsonValue key = null;
            bool dataExists = false;
            IDocumentStore<TData> documentStore = _webDataManager.DocumentStore;
            bool useDocumentStore = documentStore != null && !_webDataManager.DesactivateDocumentStore;
            if (useDocumentStore)
            {
                key = _webDataManager._GetKeyFromHttpRequest(_request.HttpRequest);
                if (documentStore.GenerateId)
                {
                    //id = GetId(webData);
                    id = documentStore.GetId(key);
                    if (id != null)
                        dataExists = true;
                }
                else
                    //dataExists = Exists(webData);
                    dataExists = documentStore.Exists(key);
            }

            if (!useDocumentStore || !dataExists || _request.ReloadFromWeb || _request.RefreshDocumentStore)
            {
                _LoadFromWeb();
            }
            else
            {
                _LoadFromDocumentStore(key);
            }

            if (useDocumentStore && _documentLoadedFromWeb)
            {
                if (documentStore.GenerateId)
                {
                    if (id == null)
                        id = documentStore.GetNewId();
                    //SetDataId(webData, id);
                    _document.zSetId(id);
                    //SaveWithId(id, webData);
                    documentStore.SaveWithId(id, _document);
                }
                else
                {
                    //SaveWithKey(webData);
                    documentStore.SaveWithKey(key, _document);
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
                        if (id != null)
                            //SaveWithId(id, webData);
                            documentStore.SaveWithId(id, _document);
                        else
                            //SaveWithKey(webData);
                            documentStore.SaveWithKey(key, _document);
                    }
                }

                if (imageRequest.LoadImageToData)
                {
                    _webDataManager.LoadImagesToData(_document);
                }
            }
        }

        private void _LoadFromWeb()
        {
            //if (!_documentLoaded)
            //{
            if (_webDataManager.WebLoadDataManager != null)
            {
                WebDataResult<TData> webDataResult = _webDataManager.WebLoadDataManager.LoadData(_request);
                _result = webDataResult.Result;
                _document = webDataResult.Data;
                _documentLoaded = true;
                _documentLoadedFromWeb = true;
            }
            else // if (_webDataManager.WebLoadDataManager_v2 != null)
            {
                // add error _LoadFromWeb
                WebDataResult_v2<TData> webDataResult = _webDataManager.WebLoadDataManager_v2.LoadData(_request);
                _result_v2 = webDataResult.Result;
                _error = !webDataResult.Success;
                _document = webDataResult.Data;
                _documentLoaded = true;
                _documentLoadedFromWeb = true;
            }
            //}
        }

        private void _LoadFromDocumentStore(BsonValue key)
        {
            _document = _webDataManager.DocumentStore.LoadFromKey(key);
            _documentLoaded = true;
            _documentLoadedFromStore = true;
        }

        public static WebData<TData> Load(WebDataManager<TData> webDataManager, WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(webDataManager, request);
            webData.Load();
            return webData;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using pb.Data;
using pb.Reflection;

namespace pb.Web.Data
{
    public interface ILoadDocument<TData>
    {
        TData Document { get; set; }
        bool DocumentLoaded { get; set; }
    }

    public interface IHttpRequestData
    {
        // get HttpRequest from data to refresh data and to get url
        // used in WebDataManager_v2<TData>.Refresh() and WebDataManager<TKey, TData>.RefreshDocumentsStore()
        // used in DownloadAutomateManager_v2.GetPostMessage()
        HttpRequest GetDataHttpRequest();  // used in WebDataManager<TKey, TData>.RefreshDocumentsStore() and IPostToDownload
    }

    public class WebData<TData> : ILoadDocument<TData>
    {
        private WebRequest _request;
        private TData _document;
        private bool _documentLoaded;
        private bool _documentLoadedFromWeb;
        private bool _documentLoadedFromStore;

        public WebData(WebRequest request)
        {
            _request = request;
        }

        public WebRequest Request { get { return _request; } }
        public TData Document { get { return _document; } set { _document = value; } }
        public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
        public bool DocumentLoadedFromWeb { get { return _documentLoadedFromWeb; } set { _documentLoadedFromWeb = value; } }
        public bool DocumentLoadedFromStore { get { return _documentLoadedFromStore; } set { _documentLoadedFromStore = value; } }
    }

    // WebDataManager_v2<TData> :
    //   load data from web and save data to store
    //   use BsonValue key to identify data
    //   function _getKeyFromHttpRequest retrieve key from HttpRequest
    //   operation : Load(), LoadFromWeb(), Exists(), Save(), Find(), Update(), Refresh()
    public class WebDataManager<TData>
    {
        protected WebLoadDataManager<TData> _webLoadDataManager = null;
        protected IDocumentStore<TData> _documentStore = null;
        protected bool _desactivateDocumentStore = false;
        protected bool _generateId = false;
        protected Func<HttpRequest, BsonValue> _getKeyFromHttpRequest = null;
        protected Action<TData> _loadImages = null;

        public WebLoadDataManager<TData> WebLoadDataManager { get { return _webLoadDataManager; } set { _webLoadDataManager = value; } }
        public IDocumentStore<TData> DocumentStore { get { return _documentStore; } set { _documentStore = value; } }
        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }
        public Func<HttpRequest, BsonValue> GetKeyFromHttpRequest { get { return _getKeyFromHttpRequest; } set { _getKeyFromHttpRequest = value; } }
        public Action<TData> LoadImages { get { return _loadImages; } set { _loadImages = value; } }

        public WebData<TData> Load(WebRequest request)
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
                }
            }
            else
            {
                // todo : remplacer Exists() par GetKey() puis Exists(key)
                if (request.ReloadFromWeb || request.RefreshDocumentStore || !Exists(webData))
                {
                    _LoadFromWeb(webData);
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
            webData.Document.zSetId(id);
        }

        protected void _Load(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!webData.DocumentLoaded)
                {
                    BsonValue key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
                    webData.Document = _documentStore.LoadFromKey(key);
                    webData.DocumentLoaded = true;
                    webData.DocumentLoadedFromStore = true;
                }
            }
            else
                _LoadFromWeb(webData);
        }

        public void SaveWithId(BsonValue id, WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                _documentStore.SaveWithId(id, webData.Document);
            }
        }

        public void SaveWithKey(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                _documentStore.SaveWithKey(webData.Document.zGetKey_v2(), webData.Document);
            }
        }

        protected void _LoadFromWeb(WebData<TData> loadWebData)
        {
            if (!loadWebData.DocumentLoaded)
            {
                loadWebData.Document = _webLoadDataManager.LoadData(loadWebData.Request);
                loadWebData.DocumentLoaded = true;
                loadWebData.DocumentLoadedFromWeb = true;
            }
        }

        public IEnumerable<TData> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            return _documentStore.Find(query, sort: sort, limit: limit).zAction(data => { if (loadImage) _LoadImages(data); });
        }

        public int Update(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = _documentStore.Update(updateDocument, query, sort, limit);
            pb.Trace.WriteLine("{0} document(s) updated", nb);
            return nb;
        }

        public int Refresh(Action<TData, TData> action = null, int limit = 0, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        {
            int nb = 0;
            foreach (TData data in Find(query, sort: sort, limit: limit, loadImage: false))
            {
                if (!(data is IHttpRequestData))
                    throw new PBException("type {0} is not IWebData", data.GetType().zGetTypeName());
                TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, LoadImage = loadImage });

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

        private BsonValue _GetKeyFromHttpRequest(HttpRequest httpRequest)
        {
            if (_getKeyFromHttpRequest != null)
                return _getKeyFromHttpRequest(httpRequest);
            else
                throw new PBException("WebDataManager.GetKeyFromHttpRequest function is not defined");
        }

        private void _LoadImages(TData data)
        {
            if (_loadImages != null)
                _loadImages(data);
            else
                throw new PBException("WebDataManager.LoadImages function is not defined");
        }
    }
}

﻿using System;
using System.Collections.Generic;
using pb.Reflection;
using pb.Data.old;

namespace pb.Web.Data.old
{
    public class WebDataManager_v1<TKey, TData>
    {
        protected WebLoadDataManager<TData> _webLoadDataManager = null;
        protected IDocumentStore_v4<TKey, TData> _documentStore = null;
        protected bool _desactivateDocumentStore = false;
        protected Func<HttpRequest, TKey> _getKeyFromHttpRequest = null;
        protected Action<TData> _loadImages = null;

        public WebLoadDataManager<TData> WebLoadDataManager { get { return _webLoadDataManager; } set { _webLoadDataManager = value; } }
        public IDocumentStore_v4<TKey, TData> DocumentStore { get { return _documentStore; } set { _documentStore = value; } }
        public bool DesactivateDocumentStore { get { return _desactivateDocumentStore; } set { _desactivateDocumentStore = value; } }
        public Func<HttpRequest, TKey> GetKeyFromHttpRequest { get { return _getKeyFromHttpRequest; } set { _getKeyFromHttpRequest = value; } }
        public Action<TData> LoadImages { get { return _loadImages; } set { _loadImages = value; } }

        public WebData<TData> Load(WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(request);
            if (request.ReloadFromWeb || request.RefreshDocumentStore || !DocumentExists(webData))
            {
                _LoadDocumentFromWeb(webData);
                SaveDocument(webData);
            }
            LoadDocument(webData);
            return webData;
        }

        public WebData<TData> LoadDocumentFromWeb(WebRequest request)
        {
            WebData<TData> webData = new WebData<TData>(request);
            _LoadDocumentFromWeb(webData);
            return webData;
        }

        protected bool DocumentExists(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                TKey key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
                return _documentStore.DocumentExists(key);
            }
            else
                return false;
        }

        protected void LoadDocument(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
            {
                if (!webData.DocumentLoaded)
                {
                    TKey key = _GetKeyFromHttpRequest(webData.Request.HttpRequest);
                    webData.Document = _documentStore.LoadDocument(key);
                    webData.DocumentLoaded = true;
                    webData.DocumentLoadedFromStore = true;
                }
            }
            else
                _LoadDocumentFromWeb(webData);
        }

        public void SaveDocument(WebData<TData> webData)
        {
            if (_documentStore != null && !_desactivateDocumentStore)
                _documentStore.SaveDocument(webData.Document.zGetKey<TKey, TData>(), webData.Document);
        }

        protected void _LoadDocumentFromWeb(WebData<TData> loadWebData)
        {
            if (!loadWebData.DocumentLoaded)
            {
                loadWebData.Document = _webLoadDataManager.LoadData(loadWebData.Request);
                loadWebData.DocumentLoaded = true;
                loadWebData.DocumentLoadedFromWeb = true;
            }
        }

        public IEnumerable<TData> FindDocuments(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        {
            return _documentStore.FindDocuments(query, sort: sort, limit: limit).zAction(data => { if (loadImage) _LoadImages(data); });
        }

        public int UpdateDocuments(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = _documentStore.UpdateDocuments(updateDocument, query, sort, limit);
            pb.Trace.WriteLine("{0} document(s) updated", nb);
            return nb;
        }

        public int RefreshDocumentsStore(Action<TData, TData> action = null, int limit = 0, bool reloadFromWeb = false, string query = null, string sort = null, bool LoadImageFromWeb = false)
        {
            int nb = 0;
            foreach (TData data in FindDocuments(query, sort: sort, limit: limit, loadImage: false))
            {
                if (!(data is IHttpRequestData))
                    throw new PBException("type {0} is not IWebData", data.GetType().zGetTypeName());
                TData data2 = _webLoadDataManager.LoadData(new WebRequest { HttpRequest = ((IHttpRequestData)data).GetDataHttpRequest(), ReloadFromWeb = reloadFromWeb, LoadImageFromWeb = LoadImageFromWeb });

                if (action != null)
                    action(data, data2);

                _documentStore.SaveDocument(data2.zGetKey<TKey, TData>(), data2);
                nb++;
            }
            return nb;
        }

        private TKey _GetKeyFromHttpRequest(HttpRequest httpRequest)
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

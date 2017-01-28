using System;
using System.Collections.Generic;
using pb.Reflection;
using pb.Web.Http;

namespace pb.Web.Data
{
    // copy from WebHeaderDetailManager
    // - change WebDataManager<TDetailData> _detailDataManager to WebDataManager_v4<TDetailData> _detailDataManager
    // - change WebDataPageManager<THeaderData> _headerDataPageManager to WebDataPageManager_v4<THeaderData> _headerDataPageManager
    public class WebHeaderDetailManager_v4<THeaderData, TDetailData>
    {
        //private WebDataPageManager<THeaderData> _headerDataPageManager = null;
        private WebDataPageManager_v4<THeaderData> _headerDataPageManager = null;
        private WebDataManager_v4<TDetailData> _detailDataManager = null;
        private Action<WebData<TDetailData>> _onDocumentLoaded = null;

        public WebDataPageManager_v4<THeaderData> HeaderDataPageManager { get { return _headerDataPageManager; } set { _headerDataPageManager = value; } }
        public WebDataManager_v4<TDetailData> DetailDataManager { get { return _detailDataManager; } set { _detailDataManager = value; } }
        public Action<WebData<TDetailData>> OnDocumentLoaded { get { return _onDocumentLoaded; } set { _onDocumentLoaded = value; } }

        // bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false
        public IEnumerable<HeaderDetail<THeaderData, TDetailData>> LoadHeaderDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool refreshDocumentStore = false,
            WebImageRequest imageRequest = null)
        {
            foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, false))
            {
                if (!(header is IHeaderData))
                    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                // ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage } }
                TDetailData detail = _detailDataManager.Load(new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, RefreshDocumentStore = refreshDocumentStore, ImageRequest = imageRequest }).Data;
                yield return new HeaderDetail<THeaderData, TDetailData> { Header = header, Detail = detail };
            }
        }

        // bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false
        public IEnumerable<TDetailData> LoadDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool refreshDocumentStore = false, WebImageRequest imageRequest = null)
        {
            Trace.WriteLine("WebHeaderDetailManager_v4<>.LoadDetails()");

            foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, false))
            {
                if (!(header is IHeaderData))
                    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                // ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage }
                yield return _detailDataManager.Load(
                    new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, ImageRequest = imageRequest, RefreshDocumentStore = refreshDocumentStore }).Data;
            }
        }

        // maxPage = 0 : all pages
        public LoadNewDocumentsResult LoadNewDocuments(int maxDocumentsLoadedFromStore = 5, int maxDocumentsLoaded = 0, int startPage = 1, int maxPage = 20, WebImageRequest webImageRequest = null)
        {
            return _LoadNewDocuments(_headerDataPageManager.LoadPages(startPage, maxPage, reload: true), maxDocumentsLoadedFromStore, maxDocumentsLoaded, webImageRequest);
        }

        // maxPage = 0 : all pages
        public LoadNewDocumentsResult LoadNewDocuments(HttpRequest httpRequest, int maxDocumentsLoadedFromStore = 5, int maxDocumentsLoaded = 0, int maxPage = 20, WebImageRequest webImageRequest = null)
        {
            return _LoadNewDocuments(_headerDataPageManager.LoadPages(httpRequest, maxPage, reload: true), maxDocumentsLoadedFromStore, maxDocumentsLoaded, webImageRequest);
        }

        private LoadNewDocumentsResult _LoadNewDocuments(IEnumerable<THeaderData> headers, int maxDocumentsLoadedFromStore = 5, int maxDocumentsLoaded = 0, WebImageRequest webImageRequest = null)
        {
            bool refreshDocumentStore = false;    // obligatoire sinon nbDocumentLoadedFromStore reste à 0
            if (webImageRequest == null)
                webImageRequest = new WebImageRequest { LoadImageFromWeb = true };
            int nbDocumentsLoaded = 0;
            int nbDocumentsLoadedFromStore = 0;
            int nbDocumentsLoadedFromWeb = 0;
            foreach (THeaderData header in headers)
            {
                if (!(header is IHeaderData))
                    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                WebData<TDetailData> webData = _detailDataManager.Load(new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = false, ImageRequest = webImageRequest, RefreshDocumentStore = refreshDocumentStore });
                nbDocumentsLoaded++;
                if (webData.DataLoadedFromStore)
                    nbDocumentsLoadedFromStore++;
                if (webData.DataLoadedFromWeb)
                    nbDocumentsLoadedFromWeb++;

                if (_onDocumentLoaded != null)
                    _onDocumentLoaded(webData);
                if (maxDocumentsLoadedFromStore != 0 && nbDocumentsLoadedFromStore == maxDocumentsLoadedFromStore)
                    break;
                if (maxDocumentsLoaded != 0 && nbDocumentsLoaded == maxDocumentsLoaded)
                    break;
            }
            return new LoadNewDocumentsResult { NbDocumentsLoadedFromWeb = nbDocumentsLoadedFromWeb, NbDocumentsLoadedFromStore = nbDocumentsLoadedFromStore };
        }
    }
}

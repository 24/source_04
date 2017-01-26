using pb.Data;
using pb.Web.Http;
using System;
using System.Collections.Generic;

namespace pb.Web.Data
{
    // from WebHeaderDetailManager_v4<THeaderData, TDetailData>, replace WebDataPageManager_v4<THeaderData> with WebDataPageManager_v5<THeaderData, TDetailData> to manage multiple detail url
    public class WebHeaderDetailManager_v5<THeaderData, TDetailData> where THeaderData : IKeyDetail, INamedHttpRequest
    {
        private WebDataPageManager_v4<THeaderData> _headerDataPageManager = null;
        private WebDataManager_v5<THeaderData, TDetailData> _detailDataManager = null;
        private Action<WebData_v2<THeaderData, TDetailData>> _onDocumentLoaded = null;

        public WebDataPageManager_v4<THeaderData> HeaderDataPageManager { get { return _headerDataPageManager; } set { _headerDataPageManager = value; } }
        public WebDataManager_v5<THeaderData, TDetailData> DetailDataManager { get { return _detailDataManager; } set { _detailDataManager = value; } }
        public Action<WebData_v2<THeaderData, TDetailData>> OnDocumentLoaded { get { return _onDocumentLoaded; } set { _onDocumentLoaded = value; } }

        //public IEnumerable<HeaderDetail<THeaderData, TDetailData>> LoadHeaderDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false,
        //    bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false,
        //    bool refreshDocumentStore = false)
        public IEnumerable<HeaderDetail<THeaderData, TDetailData>> LoadHeaderDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool refreshDocumentStore = false,
            WebImageRequest imageRequest = null)
        {
            foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, loadImageFromWeb: false, refreshDocumentStore: refreshDocumentStore))
            {
                //if (!(header is IHeaderData))
                //    throw new PBException($"type {header.GetType().zGetTypeName()} is not IHeaderData");
                //TDetailData detail = _detailDataManager.Load(new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage }, RefreshDocumentStore = refreshDocumentStore }).Data;
                TDetailData detail = _detailDataManager.Load(new WebRequest_v2<THeaderData> { Data = header, ReloadFromWeb = reloadDetail, RefreshDocumentStore = refreshDocumentStore, ImageRequest = imageRequest }).Data;
                yield return new HeaderDetail<THeaderData, TDetailData> { Header = header, Detail = detail };
            }
        }

        //public IEnumerable<TDetailData> LoadDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false,
        //    bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false,
        //    bool refreshDocumentStore = false)
        public IEnumerable<TDetailData> LoadDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool refreshDocumentStore = false,
            WebImageRequest imageRequest = null)
        {
            Trace.WriteLine("WebHeaderDetailManager_v4<>.LoadDetails()");

            foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, false))
            {
                //if (!(header is IHeaderData))
                //    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                //yield return _detailDataManager.Load(
                //    new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, ImageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage }, RefreshDocumentStore = refreshDocumentStore }).Data;
                yield return _detailDataManager.Load(new WebRequest_v2<THeaderData> { Data = header, ReloadFromWeb = reloadDetail, RefreshDocumentStore = refreshDocumentStore, ImageRequest = imageRequest }).Data;
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

        private LoadNewDocumentsResult _LoadNewDocuments(IEnumerable<THeaderData> headers, int maxDocumentsLoadedFromStore = 5, int maxDocumentsLoaded = 0, WebImageRequest imageRequest = null)
        {
            bool refreshDocumentStore = false;    // obligatoire sinon nbDocumentLoadedFromStore reste à 0
            if (imageRequest == null)
                imageRequest = new WebImageRequest { LoadImageFromWeb = true };
            int nbDocumentsLoaded = 0;
            int nbDocumentsLoadedFromStore = 0;
            int nbDocumentsLoadedFromWeb = 0;
            foreach (THeaderData header in headers)
            {
                //if (!(header is IHeaderData))
                //    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                //WebData<TDetailData> webData = _detailDataManager.Load(new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = false, ImageRequest = imageRequest, RefreshDocumentStore = refreshDocumentStore });
                WebData_v2<THeaderData, TDetailData> webData = _detailDataManager.Load(new WebRequest_v2<THeaderData> { Data = header, RefreshDocumentStore = refreshDocumentStore, ImageRequest = imageRequest });
                nbDocumentsLoaded++;
                if (webData.DataLoadedFromStore)
                    nbDocumentsLoadedFromStore++;
                if (webData.DataLoadedFromWeb)
                    nbDocumentsLoadedFromWeb++;

                _onDocumentLoaded?.Invoke(webData);
                if (maxDocumentsLoadedFromStore != 0 && nbDocumentsLoadedFromStore == maxDocumentsLoadedFromStore)
                    break;
                if (maxDocumentsLoaded != 0 && nbDocumentsLoaded == maxDocumentsLoaded)
                    break;
            }
            return new LoadNewDocumentsResult { NbDocumentsLoadedFromWeb = nbDocumentsLoadedFromWeb, NbDocumentsLoadedFromStore = nbDocumentsLoadedFromStore };
        }
    }
}

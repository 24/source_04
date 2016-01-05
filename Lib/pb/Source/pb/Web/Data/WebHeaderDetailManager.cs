using System;
using System.Collections.Generic;
using pb.Reflection;

namespace pb.Web.Data
{
    public interface IHeaderData
    {
        HttpRequest GetHttpRequestDetail();
    }

    public class LoadNewDocumentsResult
    {
        public int NbDocumentsLoadedFromWeb;
        public int NbDocumentsLoadedFromStore;
    }

    public class HeaderDetail<THeaderData, TDetailData>
    {
        public THeaderData Header;
        public TDetailData Detail;
    }

    public class WebHeaderDetailManager<THeaderData, TDetailData>
    {
        //private WebDataPageManager_v2<IHeaderData> _headerDataPageManager = null;
        private WebDataPageManager<THeaderData> _headerDataPageManager = null;
        private WebDataManager<TDetailData> _detailDataManager = null;
        private Action<WebData<TDetailData>> _onDocumentLoaded = null;

        public WebDataPageManager<THeaderData> HeaderDataPageManager { get { return _headerDataPageManager; } set { _headerDataPageManager = value; } }
        public WebDataManager<TDetailData> DetailDataManager { get { return _detailDataManager; } set { _detailDataManager = value; } }
        public Action<WebData<TDetailData>> OnDocumentLoaded { get { return _onDocumentLoaded; } set { _onDocumentLoaded = value; } }

        //public IEnumerable<HeaderDetail_v2<TDetailData>> LoadHeaderDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
        //    bool refreshDocumentStore = false)
        public IEnumerable<HeaderDetail<THeaderData, TDetailData>> LoadHeaderDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, false))
            {
                if (!(header is IHeaderData))
                    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                //TDetailData detail = _detailDataManager.Load(new WebRequest { HttpRequest = header.GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
                TDetailData detail = _detailDataManager.Load(new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
                yield return new HeaderDetail<THeaderData, TDetailData> { Header = header, Detail = detail };
            }
        }

        public IEnumerable<TDetailData> LoadDetails(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            foreach (THeaderData header in _headerDataPageManager.LoadPages(startPage, maxPage, reloadHeaderPage, false))
            {
                if (!(header is IHeaderData))
                    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                //yield return _detailDataManager.Load(
                //    new WebRequest { HttpRequest = header.GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
                yield return _detailDataManager.Load(
                    new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = reloadDetail, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
            }
        }

        // maxPage = 0 : all pages
        public LoadNewDocumentsResult LoadNewDocuments(int maxNbDocumentsLoadedFromStore = 5, int startPage = 1, int maxPage = 20, bool loadImage = true)
        {
            return _LoadNewDocuments(_headerDataPageManager.LoadPages(startPage, maxPage, reload: true, loadImage: false), maxNbDocumentsLoadedFromStore, loadImage);
        }

        // maxPage = 0 : all pages
        public LoadNewDocumentsResult LoadNewDocuments(HttpRequest httpRequest, int maxNbDocumentsLoadedFromStore = 5, int maxPage = 20, bool loadImage = true)
        {
            return _LoadNewDocuments(_headerDataPageManager.LoadPages(httpRequest, maxPage, reload: true, loadImage: false), maxNbDocumentsLoadedFromStore, loadImage);
        }

        //private LoadNewDocumentsResult _LoadNewDocuments(IEnumerable<IHeaderData> headers, int maxNbDocumentsLoadedFromStore = 5, bool loadImage = true)
        private LoadNewDocumentsResult _LoadNewDocuments(IEnumerable<THeaderData> headers, int maxNbDocumentsLoadedFromStore = 5, bool loadImage = true)
        {
            bool refreshDocumentStore = false;    // obligatoire sinon nbDocumentLoadedFromStore reste à 0
            int nbDocumentsLoadedFromStore = 0;
            int nbDocumentsLoadedFromWeb = 0;
            foreach (THeaderData header in headers)
            {
                if (!(header is IHeaderData))
                    throw new PBException("type {0} is not IHeaderData", header.GetType().zGetTypeName());
                //WebData<TDetailData> webData = _detailDataManager.Load(new WebRequest { HttpRequest = header.GetHttpRequestDetail(), ReloadFromWeb = false, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore });
                WebData<TDetailData> webData = _detailDataManager.Load(new WebRequest { HttpRequest = ((IHeaderData)header).GetHttpRequestDetail(), ReloadFromWeb = false, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore });
                if (webData.DocumentLoadedFromStore)
                    nbDocumentsLoadedFromStore++;
                if (webData.DocumentLoadedFromWeb)
                    nbDocumentsLoadedFromWeb++;

                if (_onDocumentLoaded != null)
                    _onDocumentLoaded(webData);
                if (maxNbDocumentsLoadedFromStore != 0 && nbDocumentsLoadedFromStore == maxNbDocumentsLoadedFromStore)
                    break;
            }
            return new LoadNewDocumentsResult { NbDocumentsLoadedFromWeb = nbDocumentsLoadedFromWeb, NbDocumentsLoadedFromStore = nbDocumentsLoadedFromStore };
        }
    }
}

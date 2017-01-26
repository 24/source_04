using System;
using System.Collections.Generic;
using System.Linq;

namespace pb.Web.Data.old
{
    public interface IHeaderData_v1
    {
        string GetUrlDetail();
    }

    public interface IHeaderData_v2
    {
        HttpRequest GetHttpRequestDetail();
    }

    public abstract class HeaderDetailWebDocumentStore_v3<THeaderKey, TDetailKey, TDetailData> : WebDocumentStore_v2<TDetailKey, TDetailData>
    {
        protected LoadWebEnumDataPagesManager_v4<THeaderKey, IHeaderData_v2> _loadEnumDataPagesFromWeb = null;

        public IEnumerable<TDetailData> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            return from header in _loadEnumDataPagesFromWeb.LoadPages(startPage, maxPage, reloadHeaderPage, false)
                   select LoadDocument(header.GetHttpRequestDetail(), reloadFromWeb: reloadDetail, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
        }

        // maxPage = 0 : all pages
        public virtual void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true,
            Action<LoadWebData_v5<TDetailKey, TDetailData>> onDocumentLoaded = null)
        {
            //bool loadImage = true;              // obligatoire pour charger les images
            bool refreshDocumentStore = false;    // obligatoire sinon nbDocumentLoadedFromStore reste à 0
            int nbDocumentLoadedFromStore = 0;
            foreach (IHeaderData_v2 header in _loadEnumDataPagesFromWeb.LoadPages(startPage, maxPage, reload: true, loadImage: false))
            {
                LoadWebData_v5<TDetailKey, TDetailData> loadWebData = LoadDocument(header.GetHttpRequestDetail(), reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
                if (loadWebData.DocumentLoadedFromStore)
                    nbDocumentLoadedFromStore++;

                if (onDocumentLoaded != null)
                    onDocumentLoaded(loadWebData);
                if (maxNbDocumentLoadedFromStore != 0 && nbDocumentLoadedFromStore == maxNbDocumentLoadedFromStore)
                    break;
            }
        }
    }

    public abstract class HeaderDetailWebDocumentStore_v2<THeaderKey, TDetailKey, TDetailData> : WebDocumentStore_v1<TDetailKey, TDetailData>
    {
        protected LoadWebEnumDataPagesManager_v3<THeaderKey, IEnumDataPages_v1<THeaderKey, IHeaderData_v1>, IHeaderData_v1> _loadEnumDataPagesFromWeb = null;

        public IEnumerable<TDetailData> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            return from header in _loadEnumDataPagesFromWeb.LoadPages(startPage, maxPage, reloadHeaderPage, false)
                   select LoadDocument(header.GetUrlDetail(), reloadFromWeb: reloadDetail, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
        }

        // maxPage = 0 : all pages
        public virtual void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true,
            Action<LoadWebData_v4<TDetailKey, TDetailData>> onDocumentLoaded = null)
        {
            //bool loadImage = true;              // obligatoire pour charger les images
            bool refreshDocumentStore = false;    // obligatoire sinon nbDocumentLoadedFromStore reste à 0
            int nbDocumentLoadedFromStore = 0;
            foreach (IHeaderData_v1 header in _loadEnumDataPagesFromWeb.LoadPages(startPage, maxPage, reload: true, loadImage: false))
            {
                LoadWebData_v4<TDetailKey, TDetailData> loadWebData = LoadDocument(header.GetUrlDetail(), reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
                if (loadWebData.DocumentLoadedFromStore)
                    nbDocumentLoadedFromStore++;

                if (onDocumentLoaded != null)
                    onDocumentLoaded(loadWebData);
                if (maxNbDocumentLoadedFromStore != 0 && nbDocumentLoadedFromStore == maxNbDocumentLoadedFromStore)
                    break;
            }
        }
    }

    public abstract class HeaderDetailWebDocumentStore_v1<THeaderKey, THeaderData1, THeaderData2, TDetailKey, TDetailData> : WebDocumentStore_v1<TDetailKey, TDetailData>
        where THeaderData1 : IEnumDataPages_v1<THeaderKey, THeaderData2>
        where THeaderData2 : IHeaderData_v1
    {
        protected LoadWebEnumDataPagesManager_v3<THeaderKey, THeaderData1, THeaderData2> _loadEnumDataPagesFromWeb = null;

        public IEnumerable<TDetailData> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            return from header in _loadEnumDataPagesFromWeb.LoadPages(startPage, maxPage, reloadHeaderPage, false)
                   select LoadDocument(header.GetUrlDetail(), reloadFromWeb: reloadDetail, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
        }

        // maxPage = 0 : all pages
        public virtual void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true,
            Action<LoadWebData_v4<TDetailKey, TDetailData>> onDocumentLoaded = null)
        {
            //bool loadImage = true;              // obligatoire pour charger les images
            bool refreshDocumentStore = false;  // obligatoire sinon nbDocumentLoadedFromStore reste à 0
            int nbDocumentLoadedFromStore = 0;
            foreach (THeaderData2 header in _loadEnumDataPagesFromWeb.LoadPages(startPage, maxPage, reload: true, loadImage: false))
            {
                LoadWebData_v4<TDetailKey, TDetailData> loadWebData = LoadDocument(header.GetUrlDetail(), reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
                if (loadWebData.DocumentLoadedFromStore)
                    nbDocumentLoadedFromStore++;

                if (onDocumentLoaded != null)
                    onDocumentLoaded(loadWebData);
                if (maxNbDocumentLoadedFromStore != 0 && nbDocumentLoadedFromStore == maxNbDocumentLoadedFromStore)
                    break;
            }
        }
    }
}

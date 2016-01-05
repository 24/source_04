using System;
using System.Collections.Generic;
using pb.Reflection;
using pb.Web.Data.old;

namespace pb.Web.Data.old
{
    // modif le 25/10/2015 change TKey to TPageKey
    public class WebDataPageManager_v1<TPageKey, TDataPage, TData> : WebDataManager_v1<TPageKey, TDataPage>
    {
        protected Func<int, HttpRequest> _getHttpRequestPage = null;

        public Func<int, HttpRequest> GetHttpRequestPage { get { return _getHttpRequestPage; } set { _getHttpRequestPage = value; } }

        public IEnumerable<TData> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            if (_getHttpRequestPage == null)
                throw new PBException("GetHttpRequestPage is not defined");
            HttpRequest httpRequest = _getHttpRequestPage(startPage);
            return LoadPages(httpRequest, maxPage, reload, loadImage, refreshDocumentStore);
        }

        public IEnumerable<TData> LoadPages(HttpRequest httpRequest, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            TDataPage dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
            if (!(dataPage is IEnumDataPages<TData>))
                throw new PBException("{0} is not IEnumDataPages<{1}>", dataPage.GetType().zGetTypeName(), typeof(TData).zGetTypeName());
            IEnumDataPages<TData> enumDataPages = (IEnumDataPages<TData>)dataPage;

            if (enumDataPages == null)
                yield break;

            // return data from page 1
            foreach (TData data in enumDataPages.GetDataList())
                yield return data;

            // return data from page 2, ...
            for (int nbPage = 1; maxPage == 0 || nbPage < maxPage; nbPage++)
            {
                httpRequest = enumDataPages.GetHttpRequestNextPage();
                if (httpRequest == null)
                    break;
                dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
                if (!(dataPage is IEnumDataPages<TData>))
                    throw new PBException("{0} is not IEnumDataPages<{1}>", dataPage.GetType().zGetTypeName(), typeof(TData).zGetTypeName());
                enumDataPages = (IEnumDataPages<TData>)dataPage;
                foreach (TData data in enumDataPages.GetDataList())
                    yield return data;
            }
        }
    }
}

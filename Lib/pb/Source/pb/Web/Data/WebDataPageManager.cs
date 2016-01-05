using System;
using System.Collections.Generic;

namespace pb.Web.Data
{
    public interface IEnumDataPages<TData>
    {
        IEnumerable<TData> GetDataList();
        HttpRequest GetHttpRequestNextPage();
    }

    //public class WebDataPageManager_v2<TDataPage, TData> : WebDataManager_v2<TDataPage>
    public class WebDataPageManager<TData> : WebDataManager<IEnumDataPages<TData>>
    {
        protected Func<int, HttpRequest> _getHttpRequestPageFunction = null;

        public Func<int, HttpRequest> GetHttpRequestPageFunction { get { return _getHttpRequestPageFunction; } set { _getHttpRequestPageFunction = value; } }

        public IEnumerable<TData> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            if (_getHttpRequestPageFunction == null)
                throw new PBException("GetHttpRequestPage is not defined");
            HttpRequest httpRequest = _getHttpRequestPageFunction(startPage);
            return LoadPages(httpRequest, maxPage, reload, loadImage, refreshDocumentStore);
        }

        public IEnumerable<TData> LoadPages(HttpRequest httpRequest, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            //TDataPage dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
            IEnumDataPages<TData> dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
            //if (!(dataPage is IEnumDataPages<TData>))
            //    throw new PBException("{0} is not IEnumDataPages<{1}>", dataPage.GetType().zGetTypeName(), typeof(TData).zGetTypeName());
            //IEnumDataPages<TData> enumDataPages = (IEnumDataPages<TData>)dataPage;

            if (dataPage == null)
                yield break;

            // return data from page 1
            foreach (TData data in dataPage.GetDataList())
                yield return data;

            // return data from page 2, ...
            for (int nbPage = 1; maxPage == 0 || nbPage < maxPage; nbPage++)
            {
                httpRequest = dataPage.GetHttpRequestNextPage();
                if (httpRequest == null)
                    break;
                dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, LoadImage = loadImage, RefreshDocumentStore = refreshDocumentStore }).Document;
                //if (!(dataPage is IEnumDataPages<TData>))
                //    throw new PBException("{0} is not IEnumDataPages<{1}>", dataPage.GetType().zGetTypeName(), typeof(TData).zGetTypeName());
                //enumDataPages = (IEnumDataPages<TData>)dataPage;
                foreach (TData data in dataPage.GetDataList())
                    yield return data;
            }
        }
    }
}

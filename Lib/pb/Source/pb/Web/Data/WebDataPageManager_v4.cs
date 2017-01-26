using pb.Web.Http;
using System;
using System.Collections.Generic;

namespace pb.Web.Data
{
    public class WebDataPageManager_v4<TData> : WebDataManager_v4<IEnumDataPages<TData>>
    {
        protected Func<int, HttpRequest> _getHttpRequestPageFunction = null;

        public Func<int, HttpRequest> GetHttpRequestPageFunction { get { return _getHttpRequestPageFunction; } set { _getHttpRequestPageFunction = value; } }

        public IEnumerable<TData> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false, bool refreshDocumentStore = false)
        {
            if (_getHttpRequestPageFunction == null)
                throw new PBException("GetHttpRequestPage is not defined");
            HttpRequest httpRequest = _getHttpRequestPageFunction(startPage);
            return LoadPages(httpRequest, maxPage, reload, loadImageFromWeb, loadImageToData, refreshImage, refreshDocumentStore);
        }

        public IEnumerable<TData> LoadPages(HttpRequest httpRequest, int maxPage = 1, bool reload = false, bool loadImageFromWeb = false, bool loadImageToData = false, bool refreshImage = false, bool refreshDocumentStore = false)
        {
            WebImageRequest imageRequest = new WebImageRequest { LoadImageFromWeb = loadImageFromWeb, LoadImageToData = loadImageToData, RefreshImage = refreshImage };
            IEnumDataPages<TData> dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, ImageRequest = imageRequest, RefreshDocumentStore = refreshDocumentStore }).Data;

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
                dataPage = Load(new WebRequest { HttpRequest = httpRequest, ReloadFromWeb = reload, ImageRequest = imageRequest, RefreshDocumentStore = refreshDocumentStore }).Data;
                foreach (TData data in dataPage.GetDataList())
                    yield return data;
            }
        }
    }
}

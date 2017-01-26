using System.Collections.Generic;
using pb.Web.old;
using pb.Data.old;

namespace pb.Web.Data.old
{
    public interface IEnumDataPages_v1<TKey, TData>
    {
        TKey GetKey();
        IEnumerable<TData> GetDataList();
        string GetUrlNextPage();
    }

    public interface IEnumDataPages_v2<TKey, TData>
    {
        TKey GetKey();
        IEnumerable<TData> GetDataList();
        HttpRequest GetHttpRequestNextPage();
    }

    public interface IEnumDataPages_v3<TKey, TData> : IKeyData_v4<TKey>
    {
        //TKey GetKey();
        IEnumerable<TData> GetDataList();
        HttpRequest GetHttpRequestNextPage();
    }

    public abstract class LoadWebEnumDataPagesManager_v3<TKey, TData1, TData2> : LoadWebDataManager_v4<TKey, TData1> where TData1 : IEnumDataPages_v1<TKey, TData2>
    {
        protected abstract string GetUrlPage(int page);
        protected abstract HttpRequestParameters_v1 GetHttpRequestParameters();
        protected abstract TKey GetKeyFromUrl(string url);

        public LoadWebEnumDataPagesManager_v3(LoadDataFromWebManager_v3<TData1> loadDataFromWeb, IDocumentStore_v3<TKey, TData1> documentStore = null)
            : base(loadDataFromWeb, documentStore)
        {
        }

        public IEnumerable<TData2> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            HttpRequestParameters_v1 httpRequestParameters = GetHttpRequestParameters();
            string url = GetUrlPage(startPage);
            IEnumDataPages_v1<TKey, TData2> dataPage = Load(new RequestWebData_v4<TKey>(new RequestFromWeb_v3(url, httpRequestParameters, reload, loadImage), GetKeyFromUrl(url), refreshDocumentStore)).Document;

            if (dataPage == null)
                yield break;

            foreach (TData2 data in dataPage.GetDataList())
            {
                yield return data;
            }
            for (int nbPage = 1; nbPage < maxPage; nbPage++)
            {
                url = dataPage.GetUrlNextPage();
                if (url == null)
                    break;
                dataPage = Load(new RequestWebData_v4<TKey>(new RequestFromWeb_v3(url, httpRequestParameters, reload, loadImage), GetKeyFromUrl(url), refreshDocumentStore)).Document;
                foreach (TData2 data in dataPage.GetDataList())
                {
                    yield return data;
                }
            }
        }
    }

    //public abstract class LoadWebEnumDataPagesManager_new<TKey, TData1, TData2> : LoadWebDataManager_new2<TKey, TData1> where TData1 : IEnumDataPages_new<TKey, TData2>
    // TData1 => IEnumDataPages_new<TKey, TData>       TData2 => TData
    public abstract class LoadWebEnumDataPagesManager_v4<TKey, TData> : LoadWebDataManager_v5<TKey, IEnumDataPages_v2<TKey, TData>>
    {
        protected abstract HttpRequest GetHttpRequestPage(int page);
        protected abstract TKey GetKeyFromHttpRequest(HttpRequest httpRequest);

        public LoadWebEnumDataPagesManager_v4(LoadDataFromWebManager_v4<IEnumDataPages_v2<TKey, TData>> loadDataFromWeb, IDocumentStore_v3<TKey, IEnumDataPages_v2<TKey, TData>> documentStore = null)
            : base(loadDataFromWeb, documentStore)
        {
        }

        public IEnumerable<TData> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            HttpRequest httpRequest = GetHttpRequestPage(startPage);
            IEnumDataPages_v2<TKey, TData> dataPage = Load(new RequestWebData_v5<TKey>(new RequestFromWeb_v4(httpRequest, reload, loadImage), GetKeyFromHttpRequest(httpRequest), refreshDocumentStore)).Document;

            if (dataPage == null)
                yield break;

            foreach (TData data in dataPage.GetDataList())
            {
                yield return data;
            }
            for (int nbPage = 1; nbPage < maxPage; nbPage++)
            {
                httpRequest = dataPage.GetHttpRequestNextPage();
                if (httpRequest == null)
                    break;
                dataPage = Load(new RequestWebData_v5<TKey>(new RequestFromWeb_v4(httpRequest, reload, loadImage), GetKeyFromHttpRequest(httpRequest), refreshDocumentStore)).Document;
                foreach (TData data in dataPage.GetDataList())
                {
                    yield return data;
                }
            }
        }
    }

    //public abstract class LoadWebEnumDataPagesManager_new2<TKey, TData> : LoadWebDataManager_new3<TKey, IEnumDataPages_new<TKey, TData>>
    public abstract class LoadWebEnumDataPagesManager_v5<TKey, TData> : LoadWebDataManager_v6<TKey>
    {
        protected abstract HttpRequest GetHttpRequestPage(int page);
        //protected abstract TKey GetKeyFromHttpRequest(HttpRequest httpRequest);

        //public LoadWebEnumDataPagesManager_new2(LoadDataFromWebManager_new<IEnumDataPages_new<TKey, TData>> loadDataFromWeb, IDocumentStore_new<TKey, IEnumDataPages_new<TKey, TData>> documentStore = null)
        //    : base(loadDataFromWeb, documentStore)
        //{
        //}

        public IEnumerable<TData> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            HttpRequest httpRequest = GetHttpRequestPage(startPage);
            return LoadPages(httpRequest, maxPage, reload, loadImage, refreshDocumentStore);
        }

        public IEnumerable<TData> LoadPages(HttpRequest httpRequest, int maxPage = 1, bool reload = false, bool loadImage = false, bool refreshDocumentStore = false)
        {
            //IEnumDataPages_new<TKey, TData> dataPage = Load(new RequestWebData_new2<TKey>(new RequestFromWeb_new(httpRequest, reload, loadImage), GetKeyFromHttpRequest(httpRequest), refreshDocumentStore)).Document;
            IEnumDataPages_v3<TKey, IKeyData_v4<TKey>> dataPage = (IEnumDataPages_v3<TKey, IKeyData_v4<TKey>>)Load(new RequestWebData_v6<TKey>(new RequestFromWeb_v4(httpRequest, reload, loadImage), refreshDocumentStore)).Document;

            if (dataPage == null)
                yield break;

            foreach (TData data in dataPage.GetDataList())
            {
                yield return data;
            }
            for (int nbPage = 1; nbPage < maxPage; nbPage++)
            {
                httpRequest = dataPage.GetHttpRequestNextPage();
                if (httpRequest == null)
                    break;
                //dataPage = Load(new RequestWebData_new2<TKey>(new RequestFromWeb_new(httpRequest, reload, loadImage), GetKeyFromHttpRequest(httpRequest), refreshDocumentStore)).Document;
                dataPage = (IEnumDataPages_v3<TKey, IKeyData_v4<TKey>>)Load(new RequestWebData_v6<TKey>(new RequestFromWeb_v4(httpRequest, reload, loadImage), refreshDocumentStore)).Document;
                foreach (TData data in dataPage.GetDataList())
                {
                    yield return data;
                }
            }
        }
    }
}

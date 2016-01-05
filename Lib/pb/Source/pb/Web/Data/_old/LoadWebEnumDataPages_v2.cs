using System;
using System.Collections.Generic;

namespace pb.Web.old
{
    //public abstract class LoadEnumDataPagesFromWeb_new<T> : LoadDataFromWebManager<IEnumDataPages<T>>
    //{
    //    //protected abstract IEnumerable<T> LoadPage(int startPage, bool reload, bool loadImage);
    //    //protected abstract IEnumerable<T> LoadNextPage(bool reload, bool loadImage);
    //    protected abstract string GetUrlPage(int page);
    //    protected abstract HttpRequestParameters GetHttpRequestParameters();

    //    public IEnumerable<T> LoadPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
    //    {
    //        HttpRequestParameters httpRequestParameters = GetHttpRequestParameters();
    //        IEnumDataPages<T> dataPage = Load(new RequestFromWeb(GetUrlPage(startPage), httpRequestParameters, reload, loadImage));

    //        //foreach (T data in LoadPage(startPage, reload, loadImage))
    //        foreach (T data in dataPage.GetDataList())
    //        {
    //            yield return data;
    //        }
    //        for (int nbPage = 1; nbPage < maxPage; nbPage++)
    //        {
    //            string urlNextPage = dataPage.GetUrlNextPage();
    //            if (urlNextPage == null)
    //                break;
    //            dataPage = Load(new RequestFromWeb(urlNextPage, httpRequestParameters, reload, loadImage));
    //            //foreach (T data in LoadNextPage(reload, loadImage))
    //            foreach (T data in dataPage.GetDataList())
    //            {
    //                yield return data;
    //            }
    //        }
    //    }
    //}

    public abstract class LoadWebEnumDataPages_v2<T> : IEnumerable<T>, IEnumerator<T>
    {
        //private ILoadWebEnumDataPages<T> _loadWebEnumDataPages = null;
        private int _startPage = 1;
        private int _maxPage = 1;
        private int _nbPage = 0;
        private bool _reload = false;
        private bool _loadImage = false;
        private IEnumerator<T> _enumerator = null;

        public LoadWebEnumDataPages_v2(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            //_loadWebEnumDataPages = loadWebEnumDataPages;
            _startPage = startPage;
            _maxPage = maxPage;
            _reload = reload;
            _loadImage = loadImage;
        }

        public void Dispose()
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get { return _enumerator.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _enumerator.Current; }
        }

        public bool MoveNext()
        {
            if (_nbPage == 0)
            {
                _enumerator = LoadPage(_startPage, _reload, _loadImage);
                _nbPage = 1;
            }
            while (true)
            {
                if (_enumerator == null)
                    return false;
                if (_enumerator.MoveNext())
                    return true;
                if (_nbPage == _maxPage && _maxPage != 0)
                    return false;
                _enumerator = LoadNextPage(_reload, _loadImage);
                _nbPage++;
            }
        }

        protected abstract IEnumerator<T> LoadPage(int startPage, bool reload, bool loadImage);
        protected abstract IEnumerator<T> LoadNextPage(bool reload, bool loadImage);

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}

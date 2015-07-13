using System;
using System.Collections.Generic;
using pb.Web.old;

namespace pb.Web
{
    public abstract class LoadWebDataPages_v1<T> : IEnumerable<T>, IEnumerator<T>
    {
        private string _url = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private int _maxPage = 1;
        private int _nbPage = 0;
        private bool _reload = false;
        private bool _loadImage = false;

        public LoadWebDataPages_v1(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            //_url = GetUrlPage(startPage);
            GetUrlPage(startPage, out _url, out _requestParameters);
            _maxPage = maxPage;
            _reload = reload;
            _loadImage = loadImage;
        }

        public void Dispose()
        {
        }

        public string Url { get { return _url; } }
        public HttpRequestParameters_v1 RequestParameters { get { return _requestParameters; } }
        public int MaxPage { get { return _maxPage; } }
        public int NbPage { get { return _nbPage; } }
        public bool Reload { get { return _reload; } }
        public bool LoadImage { get { return _loadImage; } }

        //protected abstract string GetUrlPage(int page);
        protected abstract void GetUrlPage(int page, out string url, out HttpRequestParameters_v1 requestParameters);

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
            get { return GetCurrentItem(); }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return GetCurrentItem(); }
        }

        public bool MoveNext()
        {
            if (_nbPage == 0)
            {
                Load();
                _nbPage = 1;
            }
            while (true)
            {
                if (GetNextItem())
                    return true;
                if (_nbPage == _maxPage && _maxPage != 0)
                    return false;
                //_url = GetUrlNextPage();
                //if (_url == null)
                //    return false;
                if (!GetUrlNextPage(out _url, out _requestParameters))
                    return false;
                Load();
                _nbPage++;
            }
        }

        protected abstract void Load();
        protected abstract bool GetNextItem();
        protected abstract T GetCurrentItem();
        //protected abstract string GetUrlNextPage();
        protected abstract bool GetUrlNextPage(out string url, out HttpRequestParameters_v1 requestParameters);

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}

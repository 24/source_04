using System;
using System.Collections.Generic;

namespace pb.Web.Data.old
{
    public interface IWebDataPages_v2<T>
    {
        void LoadPage(int page, bool reload = false, bool loadImage = false);
        bool LoadNextPage();
        bool GetNextItem();
        T GetCurrentItem();
    }

    public class LoadWebDataPages_v2<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IWebDataPages_v2<T> _webDataPages = null;
        private int _startPage = 1;
        private int _maxPage = 1;
        private int _nbPage = 0;
        private bool _reload = false;
        private bool _loadImage = false;

        public LoadWebDataPages_v2(IWebDataPages_v2<T> webDataPages, int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            _webDataPages = webDataPages;
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
            get { return _webDataPages.GetCurrentItem(); }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _webDataPages.GetCurrentItem(); }
        }

        public bool MoveNext()
        {
            if (_nbPage == 0)
            {
                //RequestFromWeb request = _webDataPages.GetRequestPage(_startPage);
                _webDataPages.LoadPage(_startPage, _reload, _loadImage);
                _nbPage = 1;
            }
            while (true)
            {
                if (_webDataPages.GetNextItem())
                    return true;
                if (_nbPage == _maxPage && _maxPage != 0)
                    return false;
                //if (!GetUrlNextPage(out _url, out _requestParameters))
                //    return false;
                if (!_webDataPages.LoadNextPage())
                    return false;
                _nbPage++;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}

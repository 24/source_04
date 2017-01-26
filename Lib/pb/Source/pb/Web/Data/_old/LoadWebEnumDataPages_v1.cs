using System;
using System.Collections.Generic;

namespace pb.Web.Data.v1
{
    public interface ILoadWebEnumDataPages_v1<T>
    {
        IEnumerator<T> LoadPage(int page, bool reload = false, bool loadImage = false);
        IEnumerator<T> LoadNextPage(bool reload = false, bool loadImage = false);
    }

    public class LoadWebEnumDataPages_v1<T> : IEnumerable<T>, IEnumerator<T>
    {
        private ILoadWebEnumDataPages_v1<T> _loadWebEnumDataPages = null;
        private int _startPage = 1;
        private int _maxPage = 1;
        private int _nbPage = 0;
        private bool _reload = false;
        private bool _loadImage = false;
        private IEnumerator<T> _enumerator = null;

        public LoadWebEnumDataPages_v1(ILoadWebEnumDataPages_v1<T> loadWebEnumDataPages, int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            _loadWebEnumDataPages = loadWebEnumDataPages;
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
                _enumerator = _loadWebEnumDataPages.LoadPage(_startPage, _reload, _loadImage);
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
                _enumerator = _loadWebEnumDataPages.LoadNextPage(_reload, _loadImage);
                _nbPage++;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}

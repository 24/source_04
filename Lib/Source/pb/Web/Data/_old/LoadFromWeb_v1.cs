using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.Web.old;

// *********************************************************** deprecated use LoadDataFromWeb ***********************************************************

namespace pb.Web
{
    public abstract class LoadListFromWebBasePages_v1<T> : IEnumerable<T>, IEnumerator<T> where T : class
    {
        protected string _url = null;
        protected string _urlFile = null;
        protected bool _reload = false;
        protected int _maxPage = 1;
        protected int _nbPage = 1;
        protected bool _init = false;
        protected bool _loadUrlResult = false;

        // new from LoadFromWebBase1
        protected bool _loadImage = false;
        protected XXElement _xelement = null;
        protected string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected T _item = null;

        //protected LoadFromWeb(string url, string urlFile = null, bool reload = false, int maxPage = 1)
        //{
        //    _url = url;
        //    _urlFile = urlFile;
        //    _reload = reload;
        //    _maxPage = maxPage;
        //    Init();
        //}

        // new from LoadFromWebBase1
        public LoadListFromWebBasePages_v1(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            _url = GetLoadUrl(startPage);
            _maxPage = maxPage;
            _loadImage = loadImage;
        }

        public void Dispose()
        {
        }

        protected void Init()
        {
            if (!_init)
            {
                LoadUrl();
                _init = true;
            }
        }

        protected void LoadUrl()
        {
            _loadUrlResult = false;
            string url = _url;
            if (_urlFile != null)
            {
                if (_reload || !File.Exists(_urlFile))
                {
                    //if (!Http2.LoadToFile(_url, _urlFile))
                    if (!Http_v3.LoadToFile(_url, _urlFile))
                        return;
                }
                url = _urlFile;
            }
            //if (Http2.LoadUrl(url))
            if (Http_v3.LoadUrl(url))
            {
                //**************************************************************************
                // new from LoadFromWebBase1
                //_xelement = new XXElement(Http2.HtmlReader.XDocument.Root);
                _xelement = new XXElement(Http_v3.Http.zGetXDocument().Root);
                _xmlEnum = GetXmlEnumerator();
                //**************************************************************************
                _loadUrlResult = true;
                return;
            }
        }

        //protected abstract void SetXml(XElement xe);
        // new from LoadFromWebBase1
        //protected void SetXml(XElement xelement)
        //{
        //    _xelement = new XXElement(xelement);
        //    InitXml();
        //}

        protected string GetUrl(string url)
        {
            return zurl.GetUrl(_url, url);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        //public abstract T Current { get; }
        // new from LoadFromWebBase1
        public T Current { get { return _item; } }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            Init();
            if (!_loadUrlResult)
                return false;
            while (true)
            {
                //if (_MoveNext())
                //    return true;
                _item = GetNext();
                if (_item != null)
                    return true;
                if (_nbPage == _maxPage)
                    return false;
                _url = GetUrlNextPage();
                if (_url == null)
                    return false;
                LoadUrl();
                if (!_loadUrlResult)
                    return false;
                _nbPage++;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        // abstract and virtual functions

        // new from LoadFromWebBase1
        protected abstract T GetNext(); // remplace _MoveNext()
        protected abstract string GetLoadUrl(int startPage = 1);
        protected abstract IEnumerator<XXElement> GetXmlEnumerator();

        protected virtual string GetUrlNextPage()
        {
            return null;
        }
    }

    public abstract class LoadListFromWebBase_v1<T> : IEnumerable<T>, IEnumerator<T>
    {
        protected string _url = null;
        protected string _urlFile = null;
        protected bool _reload = false;
        protected int _maxPage = 1;
        protected int _nbPage = 1;
        protected bool _init = false;
        protected bool _loadUrlResult = false;

        //protected LoadFromWeb(string url, string urlFile = null, bool reload = false, int maxPage = 1)
        //{
        //    _url = url;
        //    _urlFile = urlFile;
        //    _reload = reload;
        //    _maxPage = maxPage;
        //    Init();
        //}

        public void Dispose()
        {
        }

        protected void Init()
        {
            if (!_init)
            {
                LoadUrl();
                _init = true;
            }
        }

        protected void LoadUrl()
        {
            _loadUrlResult = false;
            string url = _url;
            if (_urlFile != null)
            {
                if (_reload || !File.Exists(_urlFile))
                {
                    //if (!Http2.LoadToFile(_url, _urlFile))
                    if (!Http_v3.LoadToFile(_url, _urlFile))
                        return;
                }
                url = _urlFile;
            }
            //if (Http2.LoadUrl(url))
            if (Http_v3.LoadUrl(url))
            {
                //SetXml(Http2.HtmlReader.XDocument.Root);
                SetXml(Http_v3.Http.zGetXDocument().Root);
                _loadUrlResult = true;
                return;
            }
        }

        protected abstract void SetXml(XElement xe);

        protected string GetUrl(string url)
        {
            return zurl.GetUrl(_url, url);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public abstract T Current { get; }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            Init();
            if (!_loadUrlResult)
                return false;
            while (true)
            {
                //if (_xmlLoader.MoveNext())
                if (_MoveNext())
                    return true;
                if (_nbPage == _maxPage && _maxPage != 0)
                    return false;
                //string url = _xmlLoader.GetUrlNextPage();
                //string url = GetUrlNextPage();
                _url = GetUrlNextPage();
                if (_url == null)
                    return false;
                LoadUrl();
                if (!_loadUrlResult)
                    return false;
                _nbPage++;
            }
        }

        protected abstract bool _MoveNext();

        protected virtual string GetUrlNextPage()
        {
            return null;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class LoadFromWebBase_v1
    {
        protected string _url = null;
        protected string _urlFile = null;
        protected bool _reload = false;
        protected bool _loadUrlResult = false;

        public void Dispose()
        {
        }

        public void Load()
        {
            _loadUrlResult = false;
            string url = _url;
            if (_urlFile != null)
            {
                if (_reload || !File.Exists(_urlFile))
                {
                    //if (!Http2.LoadToFile(_url, _urlFile))
                    if (!Http_v3.LoadToFile(_url, _urlFile))
                        return;
                }
                url = _urlFile;
            }
            //if (Http2.LoadUrl(url))
            if (Http_v3.LoadUrl(url))
            {
                //SetXml(Http2.HtmlReader.XDocument.Root);
                SetXml(Http_v3.Http.zGetXDocument().Root);
                _loadUrlResult = true;
                return;
            }
        }

        protected abstract void SetXml(XElement xe);

        protected string GetUrl(string url)
        {
            return zurl.GetUrl(_url, url);
        }
    }
}

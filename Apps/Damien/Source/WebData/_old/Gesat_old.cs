using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using PB_Util;
using pb.Data.Xml;
using pb.Web;

namespace Download.Gesat
{
    public class ReseauGesat_LoadHeaderFromWebOld2 : LoadFromWebBase_v1, IEnumerable<Gesat_HeaderCompany>, IEnumerator<Gesat_HeaderCompany>
    {
        private bool _loadImage = false;
        private XXElement _xeSource = null;
        private string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected Gesat_HeaderCompany _header = null;
        private static Func<string, string> _trimFunc1 = text => text.Trim();

        public ReseauGesat_LoadHeaderFromWebOld2(string url, string urlFile = null, bool reload = false, bool loadImage = false)
        {
            _url = url;
            _urlFile = urlFile;
            _reload = reload;
            _loadImage = loadImage;
        }

        protected override void SetXml(XElement xelement)
        {
            _xeSource = new XXElement(xelement);
            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            _urlNextPage = GetUrl(_xeSource.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));
            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            _xmlEnum = _xeSource.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]").GetEnumerator();
        }

        public string GetUrlNextPage()
        {
            return _urlNextPage;
        }

        public IEnumerator<Gesat_HeaderCompany> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Gesat_HeaderCompany Current
        {
            get { return _header; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _header; }
        }

        public bool MoveNext()
        {
            while (_xmlEnum.MoveNext())
            {
                XXElement xeHeader = _xmlEnum.Current;
                _header = new Gesat_HeaderCompany();
                _header.sourceUrl = _url;
                _header.loadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                //_header.companyName = xeHeader.ExplicitXPathValue(".//span[@class='NOM']//a//text()");
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    _header.url = GetUrl(xe.ExplicitXPathValue("@href"));
                    //_header.name = xe.ExplicitXPathValue(".//text()", _trimFunc1);
                    _header.name = _trimFunc1(xe.ExplicitXPathValue(".//text()"));
                }
                //<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
                xe = xeHeader.XPathElement(".//span[@class='VILLE']");
                if (xe != null)
                {
                    //IEnumerator<string> texts = xe.DescendantTextList().GetEnumerator();
                    IEnumerator<string> texts = xe.DescendantTexts().GetEnumerator();
                    if (texts.MoveNext())
                        _header.type = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyType not found");
                    if (texts.MoveNext())
                        _header.location = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyLocation not found");

                }
                // <span class="TELEPHONE">01 47 86 11 48</span>
                //_header.phone = xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()", _trimFunc1);
                _header.phone = _trimFunc1(xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()"));
                //<img info_bulle="Signataire de la charte Ethique et Valeurs" border="0" alt="/images/bullesGesat/pictoCharte.png" src="/images/bullesGesat/pictoCharte.png" style=" border: 0;" />
                //<img info_bulle="Lauréat des trophées HandiResponsables 2013" border="0" alt="/images/bullesGesat/LAURIERS-OR-2013.png" src="/images/bullesGesat/LAURIERS-OR-2013.png" style=" border: 0;" />
                //_header.infos = xeHeader.XPathValues(".//img/@info_bulle", _trimFunc1);
                _header.infos = xeHeader.XPathValues(".//img/@info_bulle").Select(_trimFunc1).ToArray();
                //_header.SetInfo(xeHeader.XPathValues(".//img/@info_bulle"));
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class ReseauGesat_LoadHeaderFromWebOld1 : LoadFromWebBase_v1, IEnumerable<Gesat_HeaderCompany>, IEnumerator<Gesat_HeaderCompany>
    {
        private bool _loadImage = false;
        private XXElement _xeSource = null;
        private string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected Gesat_HeaderCompany _header = null;
        private static Func<string, string> _trimFunc1 = text => text.Trim();

        public ReseauGesat_LoadHeaderFromWebOld1(string url, string urlFile = null, bool reload = false, bool loadImage = false)
        {
            _url = url;
            _urlFile = urlFile;
            _reload = reload;
            _loadImage = loadImage;
        }

        protected override void SetXml(XElement xelement)
        {
            _xeSource = new XXElement(xelement);
            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            _urlNextPage = GetUrl(_xeSource.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));
            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            _xmlEnum = _xeSource.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]").GetEnumerator();
        }

        public string GetUrlNextPage()
        {
            return _urlNextPage;
        }

        public IEnumerator<Gesat_HeaderCompany> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Gesat_HeaderCompany Current
        {
            get { return _header; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _header; }
        }

        public bool MoveNext()
        {
            while (_xmlEnum.MoveNext())
            {
                XXElement xeHeader = _xmlEnum.Current;
                _header = new Gesat_HeaderCompany();
                _header.sourceUrl = _url;
                _header.loadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                //_header.companyName = xeHeader.ExplicitXPathValue(".//span[@class='NOM']//a//text()");
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    _header.url = GetUrl(xe.ExplicitXPathValue("@href"));
                    //_header.name = xe.ExplicitXPathValue(".//text()", _trimFunc1);
                    _header.name = _trimFunc1(xe.ExplicitXPathValue(".//text()"));
                }
                //<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
                xe = xeHeader.XPathElement(".//span[@class='VILLE']");
                if (xe != null)
                {
                    //IEnumerator<string> texts = xe.DescendantTextList().GetEnumerator();
                    IEnumerator<string> texts = xe.DescendantTexts().GetEnumerator();
                    if (texts.MoveNext())
                        _header.type = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyType not found");
                    if (texts.MoveNext())
                        _header.location = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyLocation not found");

                }
                // <span class="TELEPHONE">01 47 86 11 48</span>
                //_header.phone = xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()", _trimFunc1);
                _header.phone = _trimFunc1(xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()"));
                //<img info_bulle="Signataire de la charte Ethique et Valeurs" border="0" alt="/images/bullesGesat/pictoCharte.png" src="/images/bullesGesat/pictoCharte.png" style=" border: 0;" />
                //<img info_bulle="Lauréat des trophées HandiResponsables 2013" border="0" alt="/images/bullesGesat/LAURIERS-OR-2013.png" src="/images/bullesGesat/LAURIERS-OR-2013.png" style=" border: 0;" />
                //_header.infos = xeHeader.XPathValues(".//img/@info_bulle", _trimFunc1);
                _header.infos = xeHeader.XPathValues(".//img/@info_bulle").Select(_trimFunc1).ToArray();
                //_header.SetInfo(xeHeader.XPathValues(".//img/@info_bulle"));
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    //******************************************************************************************
    //***************************** PAGES with LoadFromWebBase *********************************
    //******************************************************************************************
    public class ReseauGesat_LoadHeaderFromWebPagesOld1 : LoadListFromWebBase_v1<Gesat_HeaderCompany>
    {
        protected bool _loadImage = false;
        protected XXElement _xelement = null;
        protected string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected Gesat_HeaderCompany _header = null;
        protected static string _urlHeader = "http://www.reseau-gesat.com/Gesat/";
        protected static int _headersNumberByPage = 10;
        private static Func<string, string> _trimFunc1 = text => text.Trim();

        public ReseauGesat_LoadHeaderFromWebPagesOld1(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            string url = _urlHeader;
            if (startPage != 1)
                url += string.Format("EtablissementList-{0}-10.html", (startPage - 1) * _headersNumberByPage);
            _url = url;
            _maxPage = maxPage;
            _loadImage = loadImage;
        }

        public override Gesat_HeaderCompany Current { get { return _header; } }

        protected override void SetXml(XElement xelement)
        {
            _xelement = new XXElement(xelement);
            InitXml();
        }

        protected void InitXml()
        {
            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            _urlNextPage = GetUrl(_xelement.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));
            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            _xmlEnum = _xelement.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]").GetEnumerator();
        }

        protected override string GetUrlNextPage()
        {
            return _urlNextPage;
        }

        protected override bool _MoveNext()
        {
            while (_xmlEnum.MoveNext())
            {
                XXElement xeHeader = _xmlEnum.Current;
                _header = new Gesat_HeaderCompany();
                _header.sourceUrl = _url;
                _header.loadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                //_header.companyName = xeHeader.ExplicitXPathValue(".//span[@class='NOM']//a//text()");
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    _header.url = GetUrl(xe.ExplicitXPathValue("@href"));
                    //_header.name = xe.ExplicitXPathValue(".//text()", _trimFunc1);
                    _header.name = _trimFunc1(xe.ExplicitXPathValue(".//text()"));
                }
                //<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
                xe = xeHeader.XPathElement(".//span[@class='VILLE']");
                if (xe != null)
                {
                    //IEnumerator<string> texts = xe.DescendantTextList().GetEnumerator();
                    IEnumerator<string> texts = xe.DescendantTexts().GetEnumerator();
                    if (texts.MoveNext())
                        _header.type = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyType not found");
                    if (texts.MoveNext())
                        _header.location = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyLocation not found");

                }
                // <span class="TELEPHONE">01 47 86 11 48</span>
                //_header.phone = xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()", _trimFunc1);
                _header.phone = _trimFunc1(xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()"));
                //<img info_bulle="Signataire de la charte Ethique et Valeurs" border="0" alt="/images/bullesGesat/pictoCharte.png" src="/images/bullesGesat/pictoCharte.png" style=" border: 0;" />
                //<img info_bulle="Lauréat des trophées HandiResponsables 2013" border="0" alt="/images/bullesGesat/LAURIERS-OR-2013.png" src="/images/bullesGesat/LAURIERS-OR-2013.png" style=" border: 0;" />
                //_header.infos = xeHeader.XPathValues(".//img/@info_bulle", _trimFunc1);
                _header.infos = xeHeader.XPathValues(".//img/@info_bulle").Select(_trimFunc1).ToArray();
                //_header.SetInfo(xeHeader.XPathValues(".//img/@info_bulle"));
                return true;
            }
            return false;
        }
    }

    //******************************************************************************************
    //***************************** PAGES with LoadFromWebBasePages ****************************
    //******************************************************************************************
    //***************************** OBSOLETE DEPRECATED ****************************************
    public class ReseauGesat_LoadHeaderFromWebPagesOld2 : LoadListFromWebBasePages_v1<Gesat_HeaderCompany>
    {
        protected static string _urlHeader = "http://www.reseau-gesat.com/Gesat/";
        protected static int _headersNumberByPage = 10;

        public ReseauGesat_LoadHeaderFromWebPagesOld2(int startPage = 1, int maxPage = 1, bool loadImage = false)
            : base(startPage, maxPage, loadImage)
        {
            throw new Exception("deprecated");
        }

        protected override string GetLoadUrl(int startPage = 1)
        {
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            string url = _urlHeader;
            if (startPage != 1)
                url += string.Format("EtablissementList-{0}-10.html", (startPage - 1) * _headersNumberByPage);
            return url;
        }

        protected override IEnumerator<XXElement> GetXmlEnumerator()
        {
            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            return _xelement.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]").GetEnumerator();
        }

        protected override string GetUrlNextPage()
        {
            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            return GetUrl(_xelement.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));
        }

        protected override Gesat_HeaderCompany GetNext()
        {
            while (_xmlEnum.MoveNext())
            {
                XXElement xeHeader = _xmlEnum.Current;
                Gesat_HeaderCompany _header = new Gesat_HeaderCompany();
                _header.sourceUrl = _url;
                _header.loadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                //_header.companyName = xeHeader.ExplicitXPathValue(".//span[@class='NOM']//a//text()");
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    _header.url = GetUrl(xe.ExplicitXPathValue("@href"));
                    _header.name = xe.ExplicitXPathValue(".//text()");
                }
                //<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
                xe = xeHeader.XPathElement(".//span[@class='VILLE']");
                if (xe != null)
                {
                    //IEnumerator<string> texts = xe.DescendantTextList().GetEnumerator();
                    IEnumerator<string> texts = xe.DescendantTexts().GetEnumerator();
                    if (texts.MoveNext())
                        _header.type = texts.Current;
                    else
                        Trace.CurrentTrace.WriteLine("error companyType not found");
                    if (texts.MoveNext())
                        _header.location = texts.Current;
                    else
                        Trace.CurrentTrace.WriteLine("error companyLocation not found");

                }
                // <span class="TELEPHONE">01 47 86 11 48</span>
                _header.phone = xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()");
                //<img info_bulle="Signataire de la charte Ethique et Valeurs" border="0" alt="/images/bullesGesat/pictoCharte.png" src="/images/bullesGesat/pictoCharte.png" style=" border: 0;" />
                //<img info_bulle="Lauréat des trophées HandiResponsables 2013" border="0" alt="/images/bullesGesat/LAURIERS-OR-2013.png" src="/images/bullesGesat/LAURIERS-OR-2013.png" style=" border: 0;" />
                //_header.infos = xeHeader.XPathValues(".//img/@info_bulle");
                _header.infos = xeHeader.XPathValues(".//img/@info_bulle").ToArray();
                return _header;
            }
            return null;
        }
    }
}

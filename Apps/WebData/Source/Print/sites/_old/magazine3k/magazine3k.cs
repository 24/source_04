using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.Web;
//using WebData.Source.Print.sites._old.magazine3k;

//namespace Print.download
namespace Download.Print
{
    public class Magazine3kPostHeader
    {
        public string sourceUrl;
        public string url;
        public string title;
        public DateTime? postDate = null;
        public ImageHtml image = null;
        public string postAuthor;
        //public string editor;
        //public string author;
        //public string language;
        //public string category;
        public NamedValue<string> tags = new NamedValue<string>();
        public NamedValue<string> infos = new NamedValue<string>();
    }

    public class LoadMagazine3kPostHeaderFromWeb : LoadListFromWebBase_v1<Magazine3kPostHeader>
    {
        protected bool _loadImage = false;
        protected XXElement _xelement;
        protected string _sourceUrl = null;
        protected string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected Magazine3kPostHeader _postHeader;

        public LoadMagazine3kPostHeaderFromWeb(string url, int maxPage = 1, bool loadImage = false)
        {
            _url = url;
            _maxPage = maxPage;
            _loadImage = loadImage;
        }

        public override Magazine3kPostHeader Current { get { return _postHeader; } }

        protected override void SetXml(XElement xelement)
        {
            _xelement = new XXElement(xelement);
            _sourceUrl = _url;
            InitXml();
        }

        protected void InitXml()
        {
            _urlNextPage = _xelement.XPathValue("//a[@class='pBtnSelected']/following-sibling::a/@href");
            _xmlEnum = _xelement.XPathElements("//div[@class='res']").GetEnumerator();
        }

        protected override string GetUrlNextPage()
        {
            return _urlNextPage;
        }

        protected override bool _MoveNext()
        {
            while (_xmlEnum.MoveNext())
            {
                // xe = xeArticle.XPathElement("./header//a");
                // url = xe.XPathValue("@href");
                // title = xe.XPathValue(".//text()");
                // xe = xeArticle.XPathElement(".//div[@class='entry_top']");
                // xe2 = xe.XPathElement(".//img");

                XXElement xeArticle = _xmlEnum.Current;
                _postHeader = new Magazine3kPostHeader();
                _postHeader.sourceUrl = _sourceUrl;

                //<h2 class="title">
                //    <a href="http://zone-ebooks.com/journaux/le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf.html"
                //        rel="bookmark" title="Lien permanent: Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre">
                //        Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre</a>
                //</h2>

                XXElement xe = xeArticle.XPathElement(".//a");
                _postHeader.url = xe.XPathValue("@href");
                _postHeader.title = xe.XPathValue(".//text()");
                //xe = xeArticle.XPathElement(".//div[@class='entry_top']");
                //_postHeader.image = xe.XPathImage(".//img");
                //foreach (string s in xe.XElement.zDescendantTexts())
                //    _postHeader.SetInfo(s);

                //if (_loadImage && _postHeader.image.Source != null)
                //    Http2.LoadImageFromWeb(_postHeader.image.Source);
                //xe = xeArticle.XPathElement(".//footer");
                return true;
            }
            return false;
        }

        protected static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }
}

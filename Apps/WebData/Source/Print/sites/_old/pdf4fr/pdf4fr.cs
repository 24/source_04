using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Data.Xml;
using pb.Web;

//namespace Print.download
namespace Download.Print
{
    public static class Pdf4fr
    {
    }

    public class Pdf4frPostHeader
    {
        public string sourceUrl;
        public string url;
        public string title;
        public DateTime? postDate = null;
        public ImageHtml image = null;
        public string postAuthor;
        public string editor;
        public string author;
        public string language;
        //public bool paperback;
        public string category;
        public NamedValue<string> tags = new NamedValue<string>();
        public NamedValue<string> infos = new NamedValue<string>();

        private static Regex _rgInfos = new Regex("^(Date de publication:|Editeur:|Auteur:|Langue:|Broché)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public void SetInfo(string info)
        {
            //Date de publication: 03/10/2013<br>
            //Editeur: Pocket<br>
            //Auteur: Erin Hunter<br>
            //Langue: Français<br>
            //Broché<br>
            Match match = _rgInfos.Match(info);
            if (match.Success)
            {
                string s = match.zReplace(info, "").Trim();
                switch (match.Value.ToLower())
                {
                    case "Date de publication:":
                        if (postDate == null)
                        {
                            DateTime dt;
                            if (DateTime.TryParseExact(s, "", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                                postDate = dt;
                        }
                        break;
                    case "Editeur:":
                        editor = s;
                        break;
                    case "Auteur:":
                        author = s;
                        break;
                    case "Langue:":
                        language = s;
                        break;
                    case "Broché":
                        break;
                }
            }
        }
    }

    // IXmlLoader<Pdf4frPostHeader>
    public class LoadPdf4frPostHeaderFromWeb : LoadListFromWebBase_v1<Pdf4frPostHeader>
    {
        protected bool _loadImage = false;
        protected XXElement _xelement;
        protected string _sourceUrl = null;
        protected string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected Pdf4frPostHeader _postHeader;

        public LoadPdf4frPostHeaderFromWeb(string url, int maxPage = 1, bool loadImage = false)
        {
            _url = url;
            _maxPage = maxPage;
            _loadImage = loadImage;
        }

        public override Pdf4frPostHeader Current { get { return _postHeader; } }

        protected override void SetXml(XElement xelement)
        {
            _xelement = new XXElement(xelement);
            _sourceUrl = _url;
            InitXml();
        }

        protected void InitXml()
        {
            _urlNextPage = _xelement.XPathValue("//li[@class='next']//a/@href");
            _xmlEnum = _xelement.XPathElements("//article").GetEnumerator();
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
                _postHeader = new Pdf4frPostHeader();
                _postHeader.sourceUrl = _sourceUrl;
                XXElement xe = xeArticle.XPathElement("./header//a");
                _postHeader.url = xe.XPathValue("@href");
                _postHeader.title = xe.XPathValue(".//text()");
                xe = xeArticle.XPathElement(".//div[@class='entry_top']");
                ////_postHeader.image = xe.XPathImage(".//img", _url);
                //foreach (string s in xe.XElement.zDescendantTextList())
                foreach (string s in xe.XElement.zDescendantTexts())
                    _postHeader.SetInfo(s);

                if (_loadImage && _postHeader.image.Source != null)
                    Http_v2.LoadImageFromWeb(_postHeader.image.Source);
                xe = xeArticle.XPathElement(".//footer");
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

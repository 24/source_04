using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Web;

//namespace Print.download
namespace Download.Print
{
    public class ZoneEbooksPostHeader
    {
        public string sourceUrl;
        public string url;
        public string title;
        public DateTime? postDate = null;
        //public ImageHtml image = null;
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
        public string postAuthor;
        //public string editor;
        //public string author;
        //public string language;
        public string category;
        //public NamedValue<string> tags = new NamedValue<string>();
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
    }

    public class LoadZoneEbooksPostHeaderFromWeb : LoadListFromWebBase_v1<ZoneEbooksPostHeader>
    {
        protected bool _loadImage = false;
        protected XXElement _xelement = null;
        protected string _sourceUrl = null;
        protected string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected ZoneEbooksPostHeader _postHeader = null;

        protected static Dictionary<string, string> _imagesToSkip = null;

        static LoadZoneEbooksPostHeaderFromWeb()
        {
            staticInit();
        }

        public LoadZoneEbooksPostHeaderFromWeb(string url, int maxPage = 1, bool loadImage = false)
        {
            _url = url;
            _maxPage = maxPage;
            _loadImage = loadImage;
        }

        public override ZoneEbooksPostHeader Current { get { return _postHeader; } }

        protected override void SetXml(XElement xelement)
        {
            _xelement = new XXElement(xelement);
            _sourceUrl = _url;
            InitXml();
        }

        protected void InitXml()
        {
            // post list :
            //   <div id="post-1838" class="post-1838 post type-post status-publish format-standard hentry category-journaux tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook-gratuit tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-gratuit tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-telechargement tag-telecharge-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ddl tag-telecharge-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-uptobox tag-telechargement-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf clear-block count-1 odd author-admin first">
            //   _hxr.ReadSelect("//div[starts-with(@id, 'post-')]:.:EmptyRow");
            // next page :
            //   <a href='http://zone-ebooks.com/page/2' class='nextpostslink'>»</a>
            //   _hxr.ReadSelect("//a[@class='nextpostslink']:.:EmptyRow", "./@href");
            _urlNextPage = _xelement.XPathValue("//a[@class='nextpostslink']/@href");
            _xmlEnum = _xelement.XPathElements("//div[starts-with(@id, 'post-')]").GetEnumerator();
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
                _postHeader = new ZoneEbooksPostHeader();
                _postHeader.sourceUrl = _sourceUrl;

                //<h2 class="title">
                //    <a href="http://zone-ebooks.com/journaux/le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf.html"
                //        rel="bookmark" title="Lien permanent: Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre">
                //        Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre</a>
                //</h2>

                XXElement xe = xeArticle.XPathElement(".//*[@class='title']//a");
                _postHeader.url = xe.XPathValue("@href");
                _postHeader.title = xe.XPathValue(".//text()");

                //<div class="post-date">
                //    <span class="ext">Il y a 2 heures</span>
                //</div>
                string postDate = xeArticle.XPathValue(".//div[@class='post-date']//text()");
                //WriteLine("post date \"{0}\"", postDate);
                //Il y a 57 secondes
                //Il y a 3 minutes
                //Il y a 1 heure
                //Il y a 1 jour
                //Il y a 2 semaines
                //Il y a 2 mois
                if (postDate != null)
                    _postHeader.infos.Add("postDate", new ZString(postDate));

                //<div class="post-info">
                //    <span class="a">par 
                //        <a href="http://zone-ebooks.com/author/admin" title="Articles par admin ">
                //            admin
                //        </a>
                //    </span>
                //    dans
                //    <a href="http://zone-ebooks.com/category/journaux" rel="tag" title="Journaux (158 sujets)">Journaux</a>
                //</div>
                xe = xeArticle.XPathElement(".//div[@class='post-info']");
                _postHeader.postAuthor = xe.XPathValue(".//a//text()");
                _postHeader.category = xe.XPathValue("./a//text()");

                //<div class="post-content clear-block">
                xe = xeArticle.XPathElement(".//div[starts-with(@class, 'post-content')]");

                //<img title="Le Parisien + Journal de Paris &amp; supp Economie du lundi 07 octobre  PDF"
                //    alt="Le Parisien + Journal de Paris & supp Economie du lundi 07 octobre  PDF"
                //    src="http://i.imgur.com/f7aWDHF.jpg" width="362" height="446" />
                //_postHeader.images = xe.XPathImages(".//img", _url, _imagesToSkip);
                //_postHeader.images = xe.XPathImages(_url, _imagesToSkip);
                //_postHeader.images = xe.XPathImages(_url, imageHtml => !_imagesToSkip.ContainsKey(imageHtml.Source));
                //_postHeader.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, _url), imageHtml => !_imagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                //_postHeader.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, _url), imageHtml => !_imagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                _postHeader.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, _url)).Where(imageHtml => !_imagesToSkip.ContainsKey(imageHtml.Source)).ToList();

                if (_loadImage)
                    pb.old.Http_v2.LoadImageFromWeb(_postHeader.images);
                // image "infos sur le livre" http://i.imgur.com/GTPfRoB.png
                // image "description" http://i.imgur.com/Ruuh4CP.png
                //**********************************************************************************************************************************************************************************
                // pb image "infos sur le livre"
                //   zone-ebooks.com_img_info_livre_02_02.html
                //   zone-ebooks.com_img_info_livre_02_02.xml
                //   <div style="text-align: center;">
                // image ok
                //     <img title="Florence Bellot, &quot;Tresses et bracelets bresiliens&quot;" alt="Florence Bellot, Tresses et bracelets bresiliens PDF" src="http://i.imgur.com/RHWAvUQ.jpg" />
                //     <p>
                // image "infos sur le livre"
                //       <img title="Florence Bellot, &quot;Tresses et bracelets bresiliens&quot;" alt="Florence Bellot, Tresses et bracelets bresiliens PDF" src="http://i.imgur.com/GTPfRoB.png" />
                //     </p>
                //   ...
                //     <p>
                // image "description"
                //       <img title="Florence Bellot, &quot;Tresses et bracelets bresiliens&quot;" alt="Florence Bellot, Tresses et bracelets bresiliens PDF" src="http://i.imgur.com/Ruuh4CP.png" />
                //     </p>
                //**********************************************************************************************************************************************************************************


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

        protected static void staticInit()
        {
            _imagesToSkip = new Dictionary<string, string>();
            _imagesToSkip.Add("http://i.imgur.com/GTPfRoB.png", "infos sur le livre"); // Florence Bellot, “Tresses et bracelets bresiliens” http://zone-ebooks.com/livres/florence-bellot-tresses-et-bracelets-bresiliens-pdf.html
            _imagesToSkip.Add("http://i.imgur.com/Ruuh4CP.png", "description 1"); // Florence Bellot, “Tresses et bracelets bresiliens” http://zone-ebooks.com/livres/florence-bellot-tresses-et-bracelets-bresiliens-pdf.html
            _imagesToSkip.Add("http://www.telechargement-plus.com/mesimages/mag.png", "pdf-ebook-mag-telechargement-plus.com"); // France Football Mardi N 3521 – 1er Octobre 2013 http://zone-ebooks.com/magazines/france-football-mardi-n-3521-1er-octobre-2013-pdf.html
            _imagesToSkip.Add("http://prezup.eu/prez/description.png", "description 2"); // http://zone-ebooks.com/livres/la-derniere-recolte-john-grisham-pdf.html
            _imagesToSkip.Add("http://prezup.eu/prez/infossurlupload.png", "infos sur l'upload"); // http://zone-ebooks.com/livres/la-derniere-recolte-john-grisham-pdf.html
            _imagesToSkip.Add("http://prezup.eu/prez/infossurlebook.png", "infos sur l'ebook"); // http://zone-ebooks.com/bande-dessinee/al-togo-tome-5-cissie-mnatogo-bd-pdf.html
            _imagesToSkip.Add("http://prezup.eu/prez/liens.png", "liens"); // http://zone-ebooks.com/livres/la-derniere-recolte-john-grisham-pdf.html
            _imagesToSkip.Add("http://www.hapshack.com/images/0THnp.gif", "cloudzer"); // http://zone-ebooks.com/magazines/valeurs-actuelles-n-4010-3-au-9-octobre-2013-2-pdf.html
            _imagesToSkip.Add("http://www.hapshack.com/images/9MfYk.gif", "uploaded"); // http://zone-ebooks.com/magazines/valeurs-actuelles-n-4010-3-au-9-octobre-2013-2-pdf.html
            _imagesToSkip.Add("http://www.hapshack.com/images/QYeW0.gif", "turbobit"); // http://zone-ebooks.com/magazines/valeurs-actuelles-n-4010-3-au-9-octobre-2013-2-pdf.html
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
        }
    }
}

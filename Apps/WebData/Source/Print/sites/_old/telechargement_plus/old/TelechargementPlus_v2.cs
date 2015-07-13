using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;

namespace Download.Print.TelechargementPlus
{
    public static class TelechargementPlus_v2
    {

        public static void Init()
        {
            TelechargementPlus_LoadHeaderFromWeb_v2.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Header"));
            TelechargementPlus_LoadHeader_v2.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Header"));
            TelechargementPlus_LoadDetailFromWeb_v2.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Detail"));
            TelechargementPlus_LoadDetail_v2.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus/Detail"));
            //__xmlCompanyListFile = XmlConfig.CurrentConfig.GetExplicit("Handeco/Xml/XmlCompanyListFile");
            //__xmlDetailCompanyListFile = XmlConfig.CurrentConfig.GetExplicit("Handeco/Xml/XmlDetailCompanyListFile");
        }

        public static IEnumerable<TelechargementPlus_Post> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        {
            return from header in new TelechargementPlus_LoadHeaderPages_v2(startPage, maxPage, reloadHeaderPage, loadImage) select LoadDetailItem(header, reloadDetail, loadImage);
        }

        public static TelechargementPlus_Post LoadDetailItem(TelechargementPlus_PostHeader header, bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_PostDetail detail = TelechargementPlus_LoadDetail_v2.Load(header.urlDetail, reload, loadImage);
            return new TelechargementPlus_Post { header = header, detail = detail };
        }

        public static TelechargementPlus_HeaderPage LoadHeaderFromWeb_GetData(LoadDataFromWeb_v1<TelechargementPlus_HeaderPage> loadDataFromWeb, bool loadImage = false)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            string url = loadDataFromWeb.Url;
            TelechargementPlus_HeaderPage data = new TelechargementPlus_HeaderPage();

            // post list :
            //   <div class="base shortstory">
            //   _hxr.ReadSelect("//div[@class='base shortstory']:.:EmptyRow", ".//text()");
            // next page :
            //   <div class="navigation">
            //     <div align="center">
            //       <span>Prev.</span> 
            //       <span>1</span> 
            //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">2</a> 
            //       ...
            //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">Next</a>
            //     </div>
            //   </div>
            //   _hxr.ReadSelect("//div[@class='navigation']//a[text()='Next']:.:EmptyRow", "text()", "@href");
            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation']//a[text()='Next']/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class='base shortstory']");
            List<TelechargementPlus_PostHeader> headers = new List<TelechargementPlus_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                TelechargementPlus_PostHeader header = new TelechargementPlus_PostHeader();
                //_postHeader.sourceUrl = _sourceUrl;
                header.sourceUrl = url;
                header.loadFromWebDate = DateTime.Now;

                //<h1 class="shd">
                //    <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/86236-multi-ici-paris-n3562-9-au-15-octobre-2013.html">
                //        [Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013
                //    </a>
                //</h1>
                XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(header.infos, xe.XPathValue(".//text()")));
                // xe.XPathValue(".//text()", TelechargementPlus.TrimFunc1)
                header.title = TelechargementPlus.ExtractTextValues(header.infos, TelechargementPlus.TrimFunc1(xe.XPathValue(".//text()")));

                //<div class="shdinf">
                //    <div class="shdinf">
                //      <span class="rcol">Auteur: 
                //          <a onclick="ShowProfile('bakafa', 'http://www.telechargement-plus.com/user/bakafa/', '0'); return false;" href="http://www.telechargement-plus.com/user/bakafa/">
                //              bakafa
                //          </a>
                //      </span> 
                //      <span class="date">
                //          <b><a href="http://www.telechargement-plus.com/2013/10/09/">Aujourd'hui, 17:13</a></b>
                //      </span>
                //      <span class="lcol">Cat&eacute;gorie: 
                //          <a href="http://www.telechargement-plus.com/e-book-magazines/">
                //              E-Book / Magazines
                //          </a> &raquo; 
                //          <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">
                //              Magazines
                //          </a>
                //      </span>
                //    </div>
                //</div>
                xe = xeHeader.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
                header.postAuthor = xe.XPathValue(".//span[@class='rcol']//a//text()");
                //string postDate = xe.XPathValue(".//span[@class='date']//text()");
                // Aujourd'hui, 17:13
                //if (postDate != null)
                //    _postHeader.infos.SetValue("postDate", new ZString(postDate));
                header.creationDate = TelechargementPlus.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"));
                //header.category = xe.DescendantTextList(".//span[@class='lcol']").Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                header.category = xe.XPathElements(".//span[@class='lcol']").DescendantTexts().Select(TelechargementPlus.TrimFunc1).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                //Trace.CurrentTrace.WriteLine("post header category \"{0}\"", _postHeader.category);
                //.zForEach(s => s.Trim())

                //<span id="post-img">
                //    <div id="news-id-86236" style="display: inline;">
                //        <div style="text-align: center;">
                //            <!--dle_image_begin:http://zupimages.net/up/3/1515486591.jpeg|-->
                //            <img src="http://zupimages.net/up/3/1515486591.jpeg" alt="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013"
                //                title="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013" /><!--dle_image_end-->
                //            <br />
                //            <b>
                //                <br />
                //                Ici Paris N°3562 - 9 au 15 Octobre 2013<br />
                //                French | 52 pages | HQ PDF | 101 MB
                //            </b>
                //            <br />
                //            <br />
                //            Ici Paris vous fait partager la vie publique et privée de celles et ceux qui font
                //            l'actualité : exclusivités, interviews, enquêtes (la face cachée du showbiz, les
                //            coulisses de la télé) indiscrétions, potins.<br />
                //        </div>
                //    </div>
                //</span>
                xe = xeHeader.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
                //_postHeader.images = xe.XPathImages(".//img", _url, TelechargementPlus.ImagesToSkip);
                //header.images = xe.XPathImages(url, TelechargementPlus.ImagesToSkip);
                //header.images = xe.XPathImages(url, imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source));
                //header.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, url), imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                header.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, url)).Where(imageHtml => !TelechargementPlus.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                if (loadImage)
                    pb.old.Http_v2.LoadImageFromWeb(header.images);

                //header.SetTextValues(xe.DescendantTextList());
                header.SetTextValues(xe.DescendantTexts());

                headers.Add(header);
            }
            data.postHeaders = headers.ToArray();
            return data;
        }
    }
}

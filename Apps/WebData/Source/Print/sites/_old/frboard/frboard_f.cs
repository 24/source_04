using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.Web.old;
using pb.old;  // zAdd(this CookieContainer container, string url, params string[] cookies)  (Cookie.cs)

//namespace Print.download
namespace Download.Print
{
    public static class Test_Frboard_Exe
    {
        //public static void Test_frboard_01()
        //{
        //    string url = "http://www.frboard.com/magazines-et-journaux/";
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load(url);
        //    HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//li[starts-with(@class, 'threadbit')]",
        //        ".//h3[@class='threadtitle']//a/@href:.:n(href)",
        //        ".//h3[@class='threadtitle']//a/text():.:n(label1)",
        //        ".//div[@class='author']//text():.:Concat( ):n(author)");
        //    //HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//span[@class='prev_next']//a[@rel='next']/@href");
        //}

        public static void Test_frboard_02()
        {
            //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //HtmlXmlReader.CurrentHtmlXmlReader.Cookies.zAdd("http://www.frboard.com/", Frboard.GetCookies());
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.cookies.zAdd("http://www.frboard.com/", Frboard.GetCookies());
            pb.old.HtmlXmlReader.CurrentHtmlXmlReader.Load(url, requestParameters);


            // <div class="postbody"> <div class="content"> <blockquote class="postcontent restore ">
            // <div class="postbody"> <div class="postrow has_after_content">
            //HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='postbody']//blockquote");
            //string xpath = "//div[@class='postbody']//blockquote";
            //string xpath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
            string xpath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
            XDocument doc = pb.old.HtmlXmlReader.CurrentHtmlXmlReader.XDocument;
            XElement xe = doc.Root.zXPathElement(xpath);
            if (xe == null)
            {
                Trace.WriteLine("node not found \"{0}\"", xpath);
                return;
            }
            Trace.WriteLine("found \"{0}\"", xpath);
            Trace.WriteLine("date \"{0}\" time \"{1}\"", doc.Root.zXPathValue("//span[@class='date']//text()"), doc.Root.zXPathValue("//span[@class='time']//text()"));

            // <div class="userinfo">  <div class="username_container"> <div class="popupmenu memberaction">
            // <a rel="nofollow" class="username offline popupctrl" href="http://www.frboard.com/members/145457-lc.good.day.html" title="LC.GooD.Day est déconnecté"><strong>
            // <img src="http://www.frboard.com/images/misc/ur/general.png" class="userrank"></img><strong><font color="#696969">LC.GooD.Day</font></strong></strong></a>
            Trace.WriteLine("author \"{0}\"", doc.Root.zXPathValue("//div[@class='userinfo']//a//text()"));

            xpath = ".//div[@class='content']";
            xe = xe.XPathSelectElement(xpath);
            if (xe == null)
            {
                Trace.WriteLine("node not found \"{0}\"", xpath);
                return;
            }

            foreach (XNode child in xe.DescendantNodes())
            {
                if (child is XElement)
                {
                    XElement xeChild = child as XElement;
                    //Trace.WriteLine("child element \"{0}\"", xeChild.Name);
                    if (xeChild.Name == "img")
                        Trace.WriteLine("img src \"{0}\" alt \"{1}\" title \"{2}\" class \"{3}\"",
                            xeChild.zAttribValue("src"), xeChild.zAttribValue("alt"), xeChild.zAttribValue("title"), xeChild.zAttribValue("class"));
                }
                else if (child is XText)
                {
                    XText xtextChild = child as XText;
                    if (child.Parent.Name == "a")
                    {
                        Trace.WriteLine("link \"{0}\" href \"{1}\" rel \"{2}\"", xtextChild.Value, child.Parent.zAttribValue("href"), child.Parent.zAttribValue("rel"));
                    }
                    else
                        Trace.WriteLine("text \"{0}\"", xtextChild.Value.Trim());
                }
                else
                    Trace.WriteLine("child \"{0}\"", child.NodeType);
            }
            Trace.WriteLine();


            //HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//li[starts-with(@class, 'threadbit')]",
            //    ".//h3[@class='threadtitle']//a/@href:.:n(href)",
            //    ".//h3[@class='threadtitle']//a/text():.:n(label1)",
            //    ".//div[@class='author']//text():.:Concat( ):n(author)");
            //HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//span[@class='prev_next']//a[@rel='next']/@href");
        }

        public static void Test_frboard_03()
        {
            //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //HtmlXmlReader.CurrentHtmlXmlReader.Cookies.zAdd("http://www.frboard.com/", Frboard.GetCookies());
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.cookies.zAdd("http://www.frboard.com/", Frboard.GetCookies());
            pb.old.HtmlXmlReader.CurrentHtmlXmlReader.Load(url, requestParameters);

            string xpath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
            XDocument doc = pb.old.HtmlXmlReader.CurrentHtmlXmlReader.XDocument;
            XElement xe = doc.Root.zXPathElement(xpath);
            if (xe == null)
            {
                Trace.WriteLine("node not found \"{0}\"", xpath);
                return;
            }

            xpath = ".//div[@class='content']";
            xe = xe.XPathSelectElement(xpath);
            if (xe == null)
            {
                Trace.WriteLine("node not found \"{0}\"", xpath);
                return;
            }

            //foreach (XXNode child in xe.DescendantNodes().zWhereSelect(FrboardPost.Filter))
            foreach (pb.old.XXNode child in FrboardPostFilter.GetFilteredNodeList(xe))
            {
                Trace.WriteLine(child.ToString());
            }
        }

        public static void Test_frboard_04()
        {
            string xml = "<a><b/></a>";
            XDocument xd = XDocument.Parse(xml);
            IEnumerable<XNode> nodes = xd.Root.DescendantNodes();
            IEnumerator<XNode> enumerator = nodes.GetEnumerator();
            enumerator.Reset();
            // Specified method is not supported. (System.NotSupportedException)
            // System.Xml.Linq.XContainer.<GetDescendantNodes>d__4.System.Collections.IEnumerator.Reset()
            // Specified method is not supported GetDescendantNodes GetEnumerator Reset

            // Why the Reset() method on Enumerator class must throw a NotSupportedException()?
            //   http://stackoverflow.com/questions/1468170/why-the-reset-method-on-enumerator-class-must-throw-a-notsupportedexception
        }

        public static void FrboardInit()
        {
            //FrboardPostRead.Init(XmlConfig.CurrentConfig.GetElement("frboard/frboardPost"));
            LoadFrboardPost.ClassInit(XmlConfig.CurrentConfig.GetElement("frboard/frboardPost"));
        }

        //public static void Test_FrboardPost_01()
        //{
        //    //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
        //    string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    Trace.WriteLine("FrboardPost detail");
        //    FrboardInit();
        //    FrboardPostRead post = new FrboardPostRead(url);
        //    //post.LoadPost();
        //    if (post.GetCreationDateTime())
        //        Trace.WriteLine("creation date {0}, date xpath \"{1}\", time xpath \"{2}\"", post.CreationDate, FrboardPostRead.CreationDateXPath, FrboardPostRead.CreationTimeXPath);
        //    if (post.GetAuthor())
        //        Trace.WriteLine("author \"{0}\", xpath \"{1}\"", post.Author, FrboardPostRead.AuthorXPath);
        //    if (!post.GetPostNode())
        //        return;
        //    Trace.WriteLine("post node found, xpath \"{0}\"", FrboardPostRead.PostElementXPath);
        //    if (post.GetTitle())
        //        Trace.WriteLine("title \"{0}\", xpath \"{1}\"", post.Title, FrboardPostRead.TitleXPath);
        //    if (!post.GetContentNode())
        //        return;
        //    Trace.WriteLine("content node found, xpath \"{0}\"", FrboardPostRead.ContentElementXPath);
        //    if (!post.GetMultiPrint())
        //    {
        //        Trace.WriteLine("Error counting image");
        //        return;
        //    }
        //    Trace.WriteLine("nb image {0}, multi print {1}", post.NbImage, post.MultiPrint);
        //    Trace.WriteLine("content node list count {0}", post.GetContentNodeList().Count());
        //    //nodes.GetEnumerator().Reset();
        //    foreach (XXNode child in post.GetContentNodeList())
        //    {
        //        Trace.WriteLine(child.ToString());
        //    }
        //}

        //public static void Test_frboard_loadPrint_01()
        //{
        //    Trace.WriteLine("Test_frboard_loadPrint_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
        //    string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    RunSource.CurrentRunSource.View(Frboard.LoadPrint1(url));
        //}

        //public static void Test_frboard_loadPrint_02(bool loadImage = false)
        //{
        //    Trace.WriteLine("Test_frboard_loadPrint_02");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
        //    string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    FrboardInit();
        //    RunSource.CurrentRunSource.View(Frboard.LoadPrint(url, loadImage));
        //}

        //public static void Test_frboard_loadPost_01(bool loadImage = false)
        //{
        //    Trace.WriteLine("Test_frboard_loadPost_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
        //    string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    FrboardInit();
        //    FrboardPostRead post = new FrboardPostRead(url, loadImage);
        //    post.LoadFromWeb();
        //    RunSource.CurrentRunSource.View(post);
        //}

        //public static void Test_frboard_savePostToXml_01()
        //{
        //    Trace.WriteLine("Test_frboard_savePostToXml_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
        //    string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string file = "FrboardPost_01.xml";
        //    FrboardInit();
        //    FrboardPostRead post = new FrboardPostRead(url);
        //    Trace.WriteLine("save post to xml : \"{0}\"", post.GetXmlPath());
        //    post.SaveXml();
        //}

        //public static void Test_frboard_loadPostFromXml_01()
        //{
        //    Trace.WriteLine("Test_frboard_loadPostFromXml_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
        //    string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
        //    //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string file = "FrboardPost_01.xml";
        //    FrboardInit();
        //    FrboardPostRead post = new FrboardPostRead(url);
        //    Trace.WriteLine("load post from xml : \"{0}\"", post.GetXmlPath());
        //    post.LoadFromXml();
        //    RunSource.CurrentRunSource.View(post);
        //}

        public static void Test_frboard_loadPostFromWeb_02(bool loadImage = false)
        {
            Trace.WriteLine("Test_frboard_loadPostFromWeb_01");
            //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
            //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            FrboardInit();
            LoadFrboardPost post = new LoadFrboardPost(url);
            post.LoadFromWeb(loadImage);
            //RunSource.CurrentRunSource.View(post.Post);
            post.Post.zView();
            //RunSource.CurrentRunSource.View(post.Prints);
            post.Prints.zView();
        }

        public static void Test_frboard_savePostToXml_02(bool saveImage = false)
        {
            Trace.WriteLine("Test_frboard_savePostToXml_02");
            //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
            //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string file = "FrboardPost_01.xml";
            FrboardInit();
            LoadFrboardPost post = new LoadFrboardPost(url);
            Trace.WriteLine("save post to xml : \"{0}\"", post.GetXmlFile());
            post.SaveXml(saveImage);
        }

        public static void Test_frboard_loadPostFromXml_02(bool loadImage = false)
        {
            Trace.WriteLine("Test_frboard_loadPostFromXml_02");
            //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
            //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            FrboardInit();
            LoadFrboardPost post = new LoadFrboardPost(url);
            post.LoadFromXml(loadImage);
            //RunSource.CurrentRunSource.View(post.Post);
            //RunSource.CurrentRunSource.View(post.Prints);
            post.Prints.zView();
        }

        public static void Test_frboard_loadPost_02(bool loadImage = false)
        {
            Trace.WriteLine("Test_frboard_loadPost_02");
            //string url = "http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442257-multi-les-journaux-mercredi-14-aout-2013-pdf-lien-direct.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/442270-multi-le-monde-du-jeudi-15-aout-2013-pdf.html";
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446678-multi-le-nouvel-observateur-n-2548-5-au-11-septembre-2013-a.html";
            //string url = "http://www.frboard.com/magazines-et-journaux/446690-multi-paris-match-n-3355-du-05-au-11-septembre-2013-25-1-mo.html#post729974"; // PDF / 136 pages / 247.1 Mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html"; // 2 images horizontales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html"; // 2 images verticales pour le journal
            //string url = "http://www.frboard.com/magazines-et-journaux/446672-uploaded-grands-reportages-n-385-a.html";  // manque description : nepal boudist
            //string url = "http://www.frboard.com/magazines-et-journaux/446623-http-sud-ouest-6-editions-jeudi-5-septembre-2013-a.html"; // SUD OUEST 6 EDITIONS jeudi 5 septembre 2013 : SUD OUEST bassin d'Arcachon jeudi 5 septembre 2013 pdf 10.58 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/446620-http-dernieres-nouvelles-d-alsace-strasbourg-du-jeudi-5-septembre-2013-a.html"; // Dernières Nouvelles d'Alsace Strasbourg-du-jeudi 5 septembre-2013 pdf 21.88 mo
            //string url = "http://www.frboard.com/magazines-et-journaux/447493-uploaded-que-choisir-n-516-a.html"; // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            FrboardInit();
            LoadFrboardPost post = new LoadFrboardPost(url);
            post.Load(loadImage);
            //RunSource.CurrentRunSource.View(post.Post);
            //RunSource.CurrentRunSource.View(post.Prints);
            post.Prints.zView();
        }

        public static void Test_frboard_loadPrint_02(int maxPage = 1, bool loadImage = false)
        {
            Trace.WriteLine("Test_frboard_loadPrint_02");
            FrboardInit();
            //RunSource.CurrentRunSource.View(from postHeader in new LoadFrboardPostHeaderFromWeb(maxPage: maxPage) select (from print in LoadFrboardPost.GetPrints(postHeader.url, loadImage) select print).Take(1));
            (from postHeader in new LoadFrboardPostHeaderFromWeb(maxPage: maxPage) select (from print in LoadFrboardPost.GetPrints(postHeader.url, loadImage) select print).Take(1)).zView();
        }

        //public static void Test_frboard_search_01(int maxPage = 1, bool detail = false, bool loadImage = false)
        //{
        //    Trace.WriteLine("Test_frboard_search_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/";
        //    //string url = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Download\Print\download\frboard\html\frboard.com_magazines-et-journaux_01.html";
        //    FrboardInit();
        //    RunSource.CurrentRunSource.View(Frboard.search(maxPage: maxPage, detail: detail, loadImage: loadImage));
        //}

        public static void Test_frboard_loadPostHeaderFromWeb_02(int maxPage = 1)
        {
            Trace.WriteLine("Test_frboard_loadPostHeaderFromWeb_02");
            //string url = "http://www.frboard.com/magazines-et-journaux/";
            //string url = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Download\Print\download\frboard\html\frboard.com_magazines-et-journaux_01.html";
            FrboardInit();
            //RunSource.CurrentRunSource.View(new LoadFrboardPostHeaderFromWeb(maxPage: maxPage));
            new LoadFrboardPostHeaderFromWeb(maxPage: maxPage).zView();

            /*
             * http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html
             *   title : LES MAGAZINES - JEUDI 15 AOUT 2013 - description : [Liens Direct]
             *   pb les images sont décalées
             *   
             * http://www.frboard.com/magazines-et-journaux/442695-multi-le-midi-olympique-du-16-au-18-aout-2013-pdf-new-post.html
             *   description : ou
             * 
             * http://www.frboard.com/magazines-et-journaux/442688-multi-le-parisien-cahier-paris-du-vendredi-16-aout-2013-pdf.html
             *   multi-le-parisien + magazine
             *   title img info lien, title img info lien
             * 
             * http://www.frboard.com/magazines-et-journaux/446622-multi-le-parisien-cahier-paris-du-jeudi-05-septembre-pdf.html
             *   2 images horizontales pour le journal
             * 
             * http://www.frboard.com/magazines-et-journaux/446700-uploaded-le-parisien-cahier-paris-du-jeudi-05-septembre.html
             *   2 images verticales pour le journal
            */
        }

        public static void Test_frboard_updatePost_01(int maxPage = 1)
        {
            Trace.WriteLine("Test_frboard_updatePost_01");
            FrboardInit();
            LoadFrboardPostHeaderFromWeb loadPostHeader = new LoadFrboardPostHeaderFromWeb(maxPage: maxPage);
            foreach (FrboardPostHeader postHeader in loadPostHeader)
            {
                LoadFrboardPost loadPost = new LoadFrboardPost(postHeader.url);
                //loadPost.SaveXml();
                loadPost.Load();
                FrboardPost post = loadPost.Post;
            }
        }

        //public static void Test_frboard_search_print_01(int maxPage = 1, bool loadImage = false)
        //{
        //    Trace.WriteLine("Test_frboard_search_print_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/";
        //    //string url = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Download\Print\download\frboard\html\frboard.com_magazines-et-journaux_01.html";
        //    FrboardInit();
        //    RunSource.CurrentRunSource.View(new FrboardSearchPrint(maxPage: maxPage, loadImage: loadImage));
        //}
    }
}

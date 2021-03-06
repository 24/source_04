﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using Print;
using Download.Print.old;
using pb.Web.Data.old;

namespace Download.Print.Vosbooks.old
{
    public static class Vosbooks_v3
    {
        private static bool __trace = false;
        private static string __urlMainPage = "http://www.vosbooks.net/";
        private static WebDataPageManager<IHeaderData> __headerWebDataPageManager = null;
        private static WebDataManager<Vosbooks_PostDetail> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v2<Vosbooks_PostDetail> __webHeaderDetailManager = null;
        private static Date? __lastPostDate = null;

        static Vosbooks_v3()
        {
            Init(test: DownloadPrint.Test);
        }

        public static void FakeInit()
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataPageManager<IHeaderData> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }
        public static WebDataManager<Vosbooks_PostDetail> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v2<Vosbooks_PostDetail> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }

        public static void Init(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement("Vosbooks");
            else
            {
                pb.Trace.WriteLine("Vosbooks init for test");
                xe = XmlConfig.CurrentConfig.GetElement("Vosbooks_Test");
            }

            WebManagerCreator<Vosbooks_PostDetail> webManagerCreator = new WebManagerCreator<Vosbooks_PostDetail>();
            //webManagerCreator.InitLoadFromWeb
            webManagerCreator.GetHttpRequestParameters = Vosbooks_v2.GetHttpRequestParameters;
            webManagerCreator.GetHeaderPageData = GetHeaderPageData;
            webManagerCreator.HeaderPageNominalType = typeof(PostHeaderDataPage_v1);
            webManagerCreator.GetHttpRequestPage = GetHttpRequestPage;
            __headerWebDataPageManager = webManagerCreator.CreateHeaderWebDataPageManager(xe.zXPathElement("Header"));

            webManagerCreator.DetailCacheGetUrlSubDirectory = httpRequest => (_GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
            webManagerCreator.GetDetailData = GetDetailData;
            webManagerCreator.GetDetailKeyFromHttpRequest = GetPostDetailKey;
            webManagerCreator.LoadDetailImages = data => { data.LoadImages(); };
            __detailWebDataManager = webManagerCreator.CreateDetailWebDataManager(xe.zXPathElement("Detail"));

            //__webHeaderDetailManager = new WebHeaderDetailManager_v2<Vosbooks_PostDetail_v3>();
            //__webHeaderDetailManager.HeaderDataPageManager = __headerWebDataPageManager;
            //__webHeaderDetailManager.DetailDataManager = __detailWebDataManager;
            __webHeaderDetailManager = webManagerCreator.CreateWebHeaderDetailManager();

            //ServerManagers_v2.Add("Vosbooks", CreateServerManager());
        }

        public static HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters { Encoding = Encoding.UTF8 };
        }

        private static IEnumDataPages<IHeaderData> GetHeaderPageData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Vosbooks_HeaderPage_v2 data = new Vosbooks_HeaderPage_v2();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            // <div id="page">
            // <div id="wrapper">
            // <table id="layout">
            // <tr>
            // <td></td>
            // <td>
            // <div id="left-col">
            // <div id="content-padding">
            // <div id="content">
            //   <div style="height:264px;" class="cover_global" data-zt="divbyzt">...</div>
            //   ...
            // </div>

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='page-nav']//li[last()]//a[text()='>']/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@id='layout']//div[@id='content']/div");
            List<Vosbooks_PostHeader_v1> headers = new List<Vosbooks_PostHeader_v1>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Vosbooks_PostHeader_v1 header = new Vosbooks_PostHeader_v1();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                if (xeHeader.XPathValue("@class") == "page-nav")
                    continue;

                // <div style="" data-zt="divbyzt">
                // <div data-zt="divbyzt">
                // <div data-zt="divbyzt">
                // <center>
                // <strong>
                // <a href="http://www.vosbooks.net/74231-journaux/pack-journaux-francais-du-28-janvier-2015.html" title="">
                // Pack Journaux Français Du 28 Janvier 2015
                // <span class="detail_release" data-zt="spanbyzt"></span>
                // </a>
                // </strong>
                // </center>
                // </div>
                // </div>
                // </div>

                XXElement xe = xeHeader.XPathElement(".//div/div/div//a");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");

                //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                //header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));

                //xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
                //header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                //// Aujourd'hui, 17:13
                //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);

                //xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //RapideDdl.SetTextValues(header, xe.DescendantTextList());

                //xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//a").Select(RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

                headers.Add(header);
            }
            data.PostHeaders = headers.ToArray();
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.vosbooks.net/
            // page 2 : http://www.vosbooks.net/page/2
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = uri.Segments[uri.Segments.Length - 1];
            if (lastSegment.EndsWith("/"))
                lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            int page;
            if (!int.TryParse(lastSegment, out page))
                throw new PBException("header page key not found in url \"{0}\"", url);
            return page;
        }

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : http://www.vosbooks.net/
            // page 2 : http://www.vosbooks.net/page/2
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url += string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }

        private static Vosbooks_PostDetail GetDetailData(WebResult webResult)
        {
            //XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            XXElement xeSource = webResult.Http.zGetXDocument().zXXElement();
            Vosbooks_PostDetail data = new Vosbooks_PostDetail();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetPostDetailKey(webResult.WebRequest.HttpRequest);

            // <div id="page">
            // <div id="wrapper">
            // <table id="layout">
            // <tr>...</tr>
            // <tr>
            // <td class="sidebars">...</td>
            // <td>
            // <div id="left-col">
            // <div id="content-padding">
            // <div id="content">
            // ...
            // <div class="post" id="post-74299" style="margin-top: 0;">
            //
            // <table id="post-head">
            // <tr>
            // <td id="head-date">
            // <div class="date"><span>jan</span> 29</div>
            // </td>
            // <td>
            // <div class="title">
            // <h2><a href="http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html" rel="bookmark" title="Les imposteurs – François Cavanna" >Les imposteurs – François Cavanna </a></h2>
            // <div class="postdata">
            // <span class="category">
            // <a href="http://www.vosbooks.net/category/livre/ebooks-epub" rel="category tag">Ebooks en Epub</a>, 
            // <a href="http://www.vosbooks.net/category/livre" rel="category tag">Livre</a>
            // </span>
            // </div>
            // </div>
            // </td>
            // </tr>
            // </table>
            //
            // <div class="entry">
            // ...
            // <p style="text-align: center;"> 
            // <img class="alignnone"  src="http://imageshack.com/a/img538/3859/6JXSxu.jpg" alt="Les imposteurs – François Cavanna" title="Les imposteurs – François Cavanna" height="540" width="420" />
            // </p>

            // </tr>


            XXElement xePost = xeSource.XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']");

            XXElement xe = xePost.XPathElement(".//table[@id='post-head']");
            //string[] dates = xe.DescendantTextList(".//td[@id='head-date']", func: Vosbooks.TrimFunc1).ToArray();
            string[] dates = xe.XPathElement(".//td[@id='head-date']").DescendantTexts().Select(DownloadPrint.Trim).ToArray();
            data.PostCreationDate = GetDate(dates, __lastPostDate);
            if (data.PostCreationDate != null)
                __lastPostDate = new Date(data.PostCreationDate.Value);
            if (__trace)
                pb.Trace.WriteLine("post creation date {0} - {1}", data.PostCreationDate, dates.zToStringValues());

            //data.Title = xePost.XPathValue(".//div[@class='title']//a//text()", DownloadPrint.TrimFunc1);
            data.Title = xePost.XPathValue(".//div[@class='title']//a//text()").zFunc(DownloadPrint.ReplaceChars).zFunc(DownloadPrint.Trim);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            // Ebooks en Epub / Livre
            //data.Category = xePost.DescendantTextList(".//div[@class='postdata']//span[@class='category']//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            data.Category = xePost.XPathElements(".//div[@class='postdata']//span[@class='category']//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            data.PrintType = GetPrintType(data.Category);
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            xe = xePost.XPathElement(".//div[@class='entry']");
            data.Images = new WebImage[] { new WebImage(zurl.GetUrl(data.SourceUrl, xe.XPathValue("div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p/img/@src"))) };

            // force load image to get image width and height
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
            // xe.DescendantTextList(".//p")
            PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(
                xe.XPathElements(".//p").DescendantTexts(
                node =>
                {
                    if (node is XText)
                    {
                        string text = ((XText)node).Value.Trim();
                        //if (text.StartsWith("Lien Direct", StringComparison.InvariantCultureIgnoreCase))
                        if (text.StartsWith("lien ", StringComparison.InvariantCultureIgnoreCase))
                            return XNodeFilter.Stop;
                    }
                    if (node is XElement)
                    {
                        XElement xe2 = (XElement)node;
                        if (xe2.Name == "p" && xe2.zAttribValue("class") == "submeta")
                            return XNodeFilter.Stop;
                    }
                    return XNodeFilter.SelectNode;
                }
                ).Select(DownloadPrint.ReplaceChars).Select(DownloadPrint.TrimWithoutColon), data.Title);
            data.Description = textValues.description;
            //data.Language = textValues.language;
            //data.Size = textValues.size;
            //data.NbPages = textValues.nbPages;
            data.Infos.SetValues(textValues.infos);

            //data.DownloadLinks = xe.DescendantNodes(
            //    node => 
            //        {
            //            if (!(node is XElement))
            //                return true;
            //            XElement xe2 = (XElement)node;
            //            if (xe2.Name != "p")
            //                return true;
            //            XAttribute xa = xe2.Attribute("class");
            //            if (xa == null)
            //                return true;
            //            if (xa.Value != "submeta")
            //                return true;
            //            return false;
            //        },
            //    node => node is XElement && ((XElement)node).Name == "a")
            //    .Select(node => ((XElement)node).Attribute("href").Value).ToArray();
            data.DownloadLinks = xe.DescendantNodes(
                node =>
                {
                    if (!(node is XElement))
                        return XNodeFilter.DontSelectNode;
                    XElement xe2 = (XElement)node;
                    if (xe2.Name == "a")
                        return XNodeFilter.SelectNode;
                    if (xe2.Name != "p")
                        return XNodeFilter.DontSelectNode;
                    XAttribute xa = xe2.Attribute("class");
                    if (xa == null)
                        return XNodeFilter.DontSelectNode;
                    if (xa.Value != "submeta")
                        return XNodeFilter.DontSelectNode;
                    //return XNodeFilter.SkipNode;
                    return XNodeFilter.Stop;
                })
                .Select(node => ((XElement)node).Attribute("href").Value).ToArray();


            //// <div id="postlist" class="postlist restrain">
            //xe = xePost.XPathElement(".//div[@id='postlist']");

            //// Aujourd'hui, 07h32 - Aujourd'hui, 10h51 - Hier, 12h55 - 22/02/2014, 21h09
            ////string date = xe.DescendantTextList(".//div[@class='posthead']//text()", nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            //XXElement xe2 = xe.XPathElement(".//div[@class='posthead']");
            //string date = xe2.DescendantTextList(nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            //date = date.Replace('\xA0', ' ');
            //data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, webResult.LoadFromWebDate, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm");
            //if (data.PostCreationDate == null)
            //    pb.Trace.WriteLine("unknow post creation date \"{0}\"", date);

            //data.PostAuthor = xe.XPathValue(".//div[@class='userinfo']//a//text()", DownloadPrint.TrimFunc1);

            //// <div class="postbody">
            //xe = xePost.XPathElement(".//div[@class='postbody']//div[@class='content']//blockquote/div");

            //data.Images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();


            //// get infos, description, language, size, nbPages
            //PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a"), data.Title);
            //data.Description = textValues.description;
            //data.Language = textValues.language;
            //data.Size = textValues.size;
            //data.NbPages = textValues.nbPages;
            //data.Infos.SetValues(textValues.infos);

            //data.DownloadLinks = xe.XPathValues(".//a/@href");

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static DateTime? GetDate(string[] dates, Date? refDate)
        {
            if (dates.Length >= 2)
            {
                int month = zdate.GetMonthNumber(dates[0]);
                int day;
                if (month != 0 && int.TryParse(dates[1], out day))
                {
                    return zdate.GetNearestYearDate(day, month, refDate).DateTime;
                }
            }
            pb.Trace.WriteLine("unknow post creation date {0}", dates.zToStringValues());
            return null;
        }

        private static PrintType GetPrintType(string category)
        {
            // Bande Dessinée/Livre
            // Ebooks en Epub/Livre
            // Autres ouvrages/Livre
            // Cuisine/Livre
            // Divers/Français/Langues/Livre/Mathématiques/Physique
            // Divers/Livre
            // Livre/Médecine & Santé
            // Journaux/L'Equipe
            // Autres journaux/Journaux
            // L'Equipe/Revues & Magazines
            // Magazines Francais/Médecine & Santé/Revues & Magazines
            // Cours informatique/Développement Web
            category = category.ToLowerInvariant();
            if (category.Contains("journaux") || category.Contains("magazines"))
                return PrintType.Print;
            else if (category.StartsWith("bande dessinée/"))
                return PrintType.Comics;
            else if (category.Contains("livre"))      // category.EndsWith("/livre")  category.StartsWith("ebooks en epub/")
                return PrintType.Book;
            else
                return PrintType.UnknowEBook;
            // return PrintType.Book;
            // return PrintType.Comics;
            // return PrintType.UnknowEBook;
            // return PrintType.Unknow;
        }

        private static BsonValue GetPostDetailKey(HttpRequest httpRequest)
        {
            return _GetPostDetailKey(httpRequest);
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        private static int _GetPostDetailKey(HttpRequest httpRequest)
        {
            // http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html
            // http://www.vosbooks.net/74650-livre/medecine-sante/flash-sante-n-2-2015.html
            Uri uri = new Uri(httpRequest.Url);
            if (uri.Segments.Length >= 2)
            {
                //Match match = __postKeyRegex.Match(uri.Segments[uri.Segments.Length - 2]);
                Match match = __postKeyRegex.Match(uri.Segments[1]);
                if (match.Success)
                    return int.Parse(match.Value);
            }
            throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
        }
    }
}

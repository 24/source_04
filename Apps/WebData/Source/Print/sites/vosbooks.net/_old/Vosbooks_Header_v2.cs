﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Vosbooks.old
{
    public class Vosbooks_HeaderPage_v2 : IEnumDataPages<IHeaderData>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Vosbooks_PostHeader_v1[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<IHeaderData> GetDataList()
        {
            return PostHeaders;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }

    public static class Vosbooks_HeaderManager_v2
    {
        private static string __urlMainPage = "http://www.vosbooks.net/";
        //private static WebDataPageManager_v2<Vosbooks_PostHeader> __headerWebDataPageManager = null;
        private static WebDataPageManager<IHeaderData> __headerWebDataPageManager = null;

        static Vosbooks_HeaderManager_v2()
        {
            //Init();
            Vosbooks_v2.FakeInit();
        }

        public static void Init(XElement xe)
        {
            //__headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Vosbooks/Header"));
            __headerWebDataPageManager = CreateWebDataPageManager(xe.zXPathElement("Header"));
        }

        public static WebDataPageManager<IHeaderData> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<IHeaderData> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager<IHeaderData> headerWebDataPageManager = new WebDataPageManager<IHeaderData>();

            //headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<Vosbooks_HeaderPage>();
            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = Vosbooks_v2.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;

            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
            // Vosbooks_HeaderPage
            headerWebDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (headerWebDataPageManager.DocumentStore != null)
                headerWebDataPageManager.DocumentStore.NominalType = typeof(Vosbooks_HeaderPage_v2);

            headerWebDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        //private static Vosbooks_HeaderPage GetData(WebResult webResult)
        private static IEnumDataPages<IHeaderData> GetData(WebResult webResult)
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
    }
}

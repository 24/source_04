using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using pb.Web.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebData.BlogDemoor
{
    //public class PostHeaderDataPage<TDataPage> : IEnumDataPages<TDataPage>, IKeyData where TDataPage : IHeaderData
    public class BlogDemoorData //: IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        //public string UrlDetail;

        //public WebImage[] Images;

        //public HttpRequest GetHttpRequestDetail()
        //{
        //    return new HttpRequest { Url = UrlDetail };
        //}
    }

    public class BlogDemoorDataPages<BlogDemoorData> : IEnumDataPages<BlogDemoorData>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public BlogDemoorData[] Data;
        public string UrlNextPage;

        public IEnumerable<BlogDemoorData> GetDataList()
        {
            return Data;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }

    public class BlogDemoor_v2 : WebDataPageMongoManagerBase<BlogDemoorData>
    {
        private static string __configName = "BlogDemoor";
        private static BlogDemoor_v2 __current = null;
        private static string __urlMainPage = "http://www.vosbooks.net/";

        public static BlogDemoor_v2 CreateDataPageManager(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }
            __current = new BlogDemoor_v2();
            __current.DataPageNominalType = typeof(BlogDemoorDataPages<BlogDemoorData>);
            __current.Create(xe.zXPathElement("Data"));
            return __current;
        }

        protected override IEnumDataPages<BlogDemoorData> GetDataPage(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            BlogDemoorDataPages<BlogDemoorData> dataPage = new BlogDemoorDataPages<BlogDemoorData>();
            dataPage.SourceUrl = url;
            dataPage.LoadFromWebDate = webResult.LoadFromWebDate;
            dataPage.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            dataPage.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='page-nav']//li[last()]//a[text()='>']/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@id='layout']//div[@id='content']/div");
            List<BlogDemoorData> dataList = new List<BlogDemoorData>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                BlogDemoorData data = new BlogDemoorData();
                data.SourceUrl = url;
                data.LoadFromWebDate = webResult.LoadFromWebDate;

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
                data.Title = xe.XPathValue(".//text()");


                dataList.Add(data);
            }
            dataPage.Data = dataList.ToArray();
            return dataPage;
        }

        protected override BsonValue GetKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
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

        protected override HttpRequest GetHttpRequestPage(int page)
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

    public class BlogDemoor_v1 : WebDataMongoManagerBase<BlogDemoorData>
    {
        private static string __configName = "BlogDemoor";
        private static BlogDemoor_v1 __current = null;

        public static BlogDemoor_v1 CreateDataManager(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                pb.Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }

            __current = new BlogDemoor_v1();
            __current.CreateWebDataManager(xe.zXPathElement("Data"));
            //__current.WebDataManager.Load();
            return __current;
        }

        protected override BlogDemoorData GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            BlogDemoorData  data = new BlogDemoorData();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            //data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

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


            /**********************************************************************************************************************************

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='page-nav']//li[last()]//a[text()='>']/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@id='layout']//div[@id='content']/div");
            //List<Vosbooks_PostHeader> headers = new List<Vosbooks_PostHeader>();
            List<PostHeader> headers = new List<PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                //Vosbooks_PostHeader header = new Vosbooks_PostHeader();
                PostHeader header = new PostHeader();
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
            data.Headers = headers.ToArray();
            **********************************************************************************************************************************/

            return data;
        }

        protected override BsonValue GetKey(HttpRequest httpRequest)
        {
            throw new PBException("GetKey() not implemented");
        }

        protected override void LoadImages(BlogDemoorData data)
        {
            throw new PBException("LoadImages() not implemented");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using MongoDB.Bson;
using MongoDB.Driver;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Linq;
using pb.Web;
using pb.Web.old;

namespace Download.Print.RapideDdl
{
    public static class RapideDdl_Exe
    {
        public static void Test_RapideDdl_LoadDetailItemList_01(string query = null, string sort = null, int limit = 10, bool loadImage = false)
        {
            (from post in RapideDdl_LoadPostDetail.CurrentLoadPostDetail.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage)
                                            select new
                                            {
                                                id = post.id,
                                                loadFromWebDate = post.loadFromWebDate,
                                                creationDate = post.creationDate,
                                                category = post.category,
                                                title = post.title,
                                                url = post.sourceUrl,
                                                images = (from image in post.images select image.Image).ToArray(),
                                                downloadLinks = post.downloadLinks
                                            }).zView();
        }

        // images
        public static void Test_RapideDdl_LoadDetailItemList_02(string query = null, string sort = null, int limit = 10, bool loadImage = false)
        {
            (from post in RapideDdl_LoadPostDetail.CurrentLoadPostDetail.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage)
                                            where post.images.Length > 1
                                            select new
                                            {
                                                images_nb = post.images.Length,
                                                url = post.sourceUrl,
                                                creationDate = post.creationDate,
                                                title = post.title,
                                                //images = (from image in post.images select new { image = image.Image, url = image.Source }).ToArray()
                                                images = post.images
                                                //downloadLinks = post.downloadLinks
                                            }).zView();
        }

        public static void Test_RapideDdl_LoadUrl_01(string url)
        {
            //string url = "http://rapide-ddl.com/ebooks/";
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            requestParameters.cookies.Add(new Uri(url), new Cookie("hasVisitedSite", "Yes"));
            pb.old.Http_v2.LoadUrl(url, requestParameters);
        }

        //public static void Test_RapideDdl_LoadDetailUrl_01(string url)
        //{
        //    HttpRequestParameters requestParameters = new HttpRequestParameters();
        //    //requestParameters.encoding = Encoding.UTF8;
        //    requestParameters.cookies.Add(new Uri(url), new Cookie("hasVisitedSite", "Yes"));
        //    Http2.LoadUrl(url, requestParameters);
        //}




        public static void Test_RapideDdl_LoadHeaderPagesFromWeb_01()
        {
            string url = "http://www.rapide-ddl.com/ebooks/";
            RapideDdl_LoadHeaderPageFromWebManager load = new RapideDdl_LoadHeaderPageFromWebManager();
            load.Load(new RequestFromWeb_v3(url, new HttpRequestParameters_v1 { encoding = Encoding.UTF8 })).postHeaders.zView();
        }

        public static void Test_RapideDdl_LoadPostDetail_01()
        {
            string url = "http://www.rapide-ddl.com/ebooks/journaux/36784-libgration-du-lundi-04-aogt-2014.html";
            RapideDdl_LoadPostDetail.CurrentLoadPostDetail.LoadDocument(url, refreshDocumentStore: true).zView();
        }

        public static void Test_RapideDdl_LoadPostImages_01(string query = null, string sort = null, int limit = 10, bool loadImage = false)
        {
            //IEnumerable<ImageHtml> images = from image in (from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage) select post.images) select image;
            //IEnumerable<ImageHtml> images = from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage) select (from image in post.images select image);
            //RunSource.CurrentRunSource.View(from image in (from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage) select post.images) select image);
            //RunSource.CurrentRunSource.View(from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage) select (from image in post.images select image));
            //var q = RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage).SelectMany(post => post.images);
            //var q = from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage) select (from image in post.images select image);
            //var q = from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage)
            //        select (from image in post.images
            //                select new { source = image.Source, title = image.Title, width = image.ImageWidth, height = image.ImageHeight, date = post.creationDate, url = post.sourceUrl });
            //var q1 = from post in RapideDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: loadImage) select (from image in post.images select image.Source);
            var q1 = from post in RapideDdl_LoadPostDetail.CurrentLoadPostDetail.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage) select (from image in post.images select image.Url);
            var q2 = q1.SelectMany(image => image).Distinct().OrderBy(image => image);
            UrlCache_v1 urlCache = new UrlCache_v1(@"c:\pib\_cacheImage", UrlFileNameType.Path | UrlFileNameType.Host, (url, requestParameters) => zurl.GetDomain(url));
            var q3 = from image in q2 select new { image = image, file = zurl.UrlToFileName(image, UrlFileNameType.Path | UrlFileNameType.Host), file2 = urlCache.GetUrlSubPath(image) };
            var q = q3;
            //RunSource.CurrentRunSource.View(q);
            q.zView();
        }

        public static void Test_RapideDdl_MongoDocumentStore_01(object key, RapideDdl_PostDetail post)
        {
            MongoDocumentStore_v2<RapideDdl_PostDetail> documentStore = new MongoDocumentStore_v2<RapideDdl_PostDetail>("mongodb://localhost", "test", "Test_RapideDdl_Detail", "download");
            documentStore.SaveDocument(key, post);
        }

        public static void Test_RapideDdl_MongoUpdateDetailItemList_01(string query)
        {
            int imagesIndex = 11;
            int nb = 0;
            MongoCommand.UpdateDocuments("dl", "Test_RapideDdl_Detail", query,
              doc =>
              {
                Trace.WriteLine("update document {0}", doc["_id"]);
                doc["download"].AsBsonDocument.Remove("images2");
                BsonValue images = doc["download"]["images"];
                BsonElement elementImages = doc["download"].AsBsonDocument.GetElement(imagesIndex);
                if (elementImages.Name != "images")
                {
                    Trace.WriteLine("error element index {0} is'nt images but {1}", imagesIndex, elementImages.Name);
                    return;
                }
                if (!images.IsBsonArray)
                {
                    Trace.WriteLine("error element images is'nt an array");
                    return;
                }
                BsonArray images2 = new BsonArray();
                foreach (BsonValue image in images.AsBsonArray)
                {
                    if (image.IsBsonDocument)
                        images2.Add(image["Source"]);
                    else
                        Trace.WriteLine("error value of images is'nt a document");
                }
                //doc["download"].AsBsonDocument.Add("images2", images2);
                //doc["download"].AsBsonDocument.InsertAt(imagesIndex + 1, new BsonElement("images2", images2));
                doc["download"].AsBsonDocument.Remove("images");
                doc["download"].AsBsonDocument.InsertAt(imagesIndex, new BsonElement("images", images2));
                nb++;
              });
            Trace.WriteLine("{0} documents updated", nb);
        }

        public static void Test_RapideDdl_MongoUpdateDetailItemList_02(string query)
        {
            RapideDdl_LoadPostDetail.CurrentLoadPostDetail.UpdateDocuments(
                post =>
                    {
                        Trace.WriteLine("update document \"{0}\"", post.sourceUrl);
                        post.id = RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(post.sourceUrl);
                    }, query);
            //if (query == null)
            //    query = "{}";
            //string sort = "{ 'download.creationDate': -1 }";
            //foreach (RapideDdl_PostDetail post in RapideDdl_LoadPostDetail.Find(query, sort: sort))
            //{
            //    Trace.WriteLine("update post {0}", post.sourceUrl);

            //    BsonPBSerializationProvider.RegisterSerializer(typeof(ZValue), typeof(ZValueSerializer));
            //    BsonPBSerializationProvider.RegisterSerializer(typeof(ZInt), typeof(ZIntSerializer));
            //    BsonPBSerializationProvider.RegisterSerializer(typeof(ZString), typeof(ZStringSerializer));

            //    try
            //    {
            //        RapideDdl_LoadPostDetail.DocumentStore.SaveDocument(RapideDdl_LoadPostDetail.GetPostDetailKey(post.sourceUrl), post);
            //    }
            //    finally
            //    {
            //        BsonPBSerializationProvider.UnregisterSerializer(typeof(ZValue));
            //        BsonPBSerializationProvider.UnregisterSerializer(typeof(ZInt));
            //        BsonPBSerializationProvider.UnregisterSerializer(typeof(ZString));
            //    }
            //}
        }

        public static void Test_RapideDdl_LoadPostLinks_01(string query = null, string sort = null, int limit = 10)
        {
            var q1 = from post in RapideDdl_LoadPostDetail.CurrentLoadPostDetail.FindDocuments(query, sort: sort, limit: limit, loadImage: false) select (from link in post.downloadLinks select link);
            var q2 = q1.SelectMany(link => link).OrderBy(link => link);
            var q = q2;
            //RunSource.CurrentRunSource.View(q);
            q.zView();
        }

        public static void Test_XXElement_DescendantTextList_01()
        {
            string url = @"c:\pib\dev_data\exe\runsource\download\sites\rapide-ddl\cache\detail\39000\ebooks_magazine_39023-multi-lautomobile-no821-octobre-2014.html";
            pb.old.Http_v2.LoadUrl(url);
            XXElement xe = new XXElement(pb.old.Http_v2.HtmlReader.XDocument.Root).XPathElement("//div[@class='lcolomn mainside']").XPathElement(".//div[@class='maincont']");
            //string xpath = ".//div";
            //foreach (string s in xe.DescendantTextList())
            foreach (string s in xe.DescendantTexts())
            {
                Trace.WriteLine(s);
            }
            //foreach (string s in from xe2 in xe.XElement.XPathSelectElements(xpath) from s in xe2.zDescendantTextList() select s)
            //{
            //    Trace.WriteLine(s);
            //}
            //foreach (XElement xe2 in xe.XElement.XPathSelectElements(xpath))
            //{
            //    Trace.WriteLine("XElement {0}", xe2.zGetPath());
            //    foreach (string s in xe2.zDescendantTextList())
            //    {
            //        Trace.WriteLine(s);
            //    }
            //}
        }





        public static void Test_RapideDdl_LoadDetailItemList_01_old(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            //RunSource.CurrentRunSource.View(RapideDdl_LoadPostDetail.CurrentLoadPostDetail.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage, refreshDocumentStore));
            RapideDdl_LoadPostDetail.CurrentLoadPostDetail.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage, refreshDocumentStore).zView();
        }

        public static void Test_RapideDdl_LoadDetailItemList_02_old(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            // RunSource.CurrentRunSource.View
            (from item in RapideDdl_LoadPostDetail.CurrentLoadPostDetail.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage, refreshDocumentStore)
                select new
                {
                    url = item.sourceUrl,
                    creationDate = item.creationDate,
                    title = item.title,
                    images = (from image in item.images select image.Image).ToArray(),
                    downloadLinks = item.downloadLinks
                }).zView();
        }

        //public static void Test_RapideDdl_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        //{
        //    //RunSource.CurrentRunSource.View(new RapideDdl_LoadHeaderPages(startPage, maxPage, reload, loadImage));
        //    RunSource.CurrentRunSource.View(RapideDdl_LoadHeaderPages.Load(startPage, maxPage, reload, loadImage));
        //}
    }
}

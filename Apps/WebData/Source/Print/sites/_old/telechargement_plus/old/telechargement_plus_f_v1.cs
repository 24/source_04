using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using Download.Print.TelechargementPlus;
using pb.Data.Xml;

//namespace Print.download
namespace Download.Print
{
    public static class Telechargement_Plus_Exe_v1
    {
        public static void TelechargementPlusInit()
        {
            //Http2.HtmlReader.WebEncoding = Encoding.UTF8;
            TelechargementPlus_v1.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus"));
            Download.Print.TelechargementPlus.TelechargementPlus_v2.Init();
        }

        public static MongoCollection<BsonDocument> TelechargementPlus_GetMongoCollection()
        {
            string server = XmlConfig.CurrentConfig.Get("TelechargementPlus/MongoServer", "mongodb://localhost");
            string database = XmlConfig.CurrentConfig.Get("TelechargementPlus/MongoDatabase");
            string collection = XmlConfig.CurrentConfig.Get("TelechargementPlus/MongoCollection");
            Trace.WriteLine("connect to mongo server \"{0}\" database \"{1}\" collection \"{2}\"", server, database, collection);
            return new MongoClient(server).GetServer().GetDatabase(database).GetCollection(collection);
        }

        public static void Test_telechargement_plus_loadPostHeader_01(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            //string url = "http://www.telechargement-plus.com/e-book-magazines/";
            //http://www.telechargement-plus.com/page/2/
            //if (startPage != 1)
            //    url += "page/" + (string)startPage.ToString() + "/";
            TelechargementPlusInit();
            //LoadTelechargementPlusPostHeaderFromWeb.ClassInit(XmlConfig.CurrentConfig.GetElement("TelechargementPlus"));
            //LoadFromWeb1<TelechargementPlusPostHeader> load = new LoadFromWeb1<TelechargementPlusPostHeader>(new LoadTelechargementPlusPostHeaderFromWeb(loadImage), url, maxPage);
            //TelechargementPlus_LoadPostHeaderFromWeb load = new TelechargementPlus_LoadPostHeaderFromWeb(url, maxPage, loadImage);
            TelechargementPlus_LoadPostHeaderFromWeb_v1 load = new TelechargementPlus_LoadPostHeaderFromWeb_v1(startPage, maxPage, loadImage);
            //RunSource.CurrentRunSource.View(load);
            load.zView();
        }

        public static void Test_telechargement_plus_loadPostFromWeb_01(bool reload = false, bool loadImage = false)
        {
            Trace.WriteLine("Test_telechargement_plus_loadPostFromWeb_01");
            //string url = "http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html";
            //string url = "http://www.telechargement-plus.com/e-book-magazines/livres/87422-medecine-3e-partie.html";   // 3 liens uploaded
            //string url = "http://www.telechargement-plus.com/e-book-magazines/88218-solutions-pc-n8-septembre-2013-liens-direct.html";
            //string url = "http://www.telechargement-plus.com/e-book-magazines/livres/87000-60-petits-maux-soignys-par-les-huiles-essentielles.html";   // pb liens dans description
            string url = "http://www.telechargement-plus.com/e-book-magazines/journaux/91392-midi-olympique-du-08-novembre-2013-multi.html";  // image uploaded
            //string url = "";
            //string url = "";
            //string url = "";
            //FrboardInit();
            //LoadFrboardPost post = new LoadFrboardPost(url);
            //post.LoadFromWeb(loadImage);
            //RunSource.CurrentRunSource.View(post.Post);
            //RunSource.CurrentRunSource.View(post.Prints);

            //LoadTelechargementPlusPost loadPost = new LoadTelechargementPlusPost(url);
            //HttpLoad load = new HttpLoad(loadPost);
            //load.LoadFromWeb(loadImage);

            TelechargementPlusInit();
            TelechargementPlus_LoadPost_v1 loadPost = new TelechargementPlus_LoadPost_v1(url);

            loadPost.Load(reload: reload, loadImage: loadImage);
            //RunSource.CurrentRunSource.View(loadPost.Post);
            //RunSource.CurrentRunSource.View(loadPost.Prints);
            loadPost.Prints.zView();

            //loadPost.SaveXml(reload: true);

            //loadPost.LoadFromWeb(loadImage);
            //RunSource.CurrentRunSource.View(loadPost.Prints);

            //LoadTelechargementPlusPostFromWeb loadPost = new LoadTelechargementPlusPostFromWeb(url, loadImage: loadImage);
            //RunSource.CurrentRunSource.View(loadPost);
        }

        public static void Test_telechargement_plus_loadPostFromWeb_02(bool reload = false, int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            Trace.WriteLine("Test_telechargement_plus_loadPostFromWeb_02");
            TelechargementPlusInit();
            //RunSource.CurrentRunSource.View(from postHeader in new TelechargementPlus_LoadPostHeaderFromWeb_v1(startPage: startPage, maxPage: maxPage) select from print in TelechargementPlus_LoadPost_v1.GetPrints(postHeader.url, reload, loadImage) select print);
            (from postHeader in new TelechargementPlus_LoadPostHeaderFromWeb_v1(startPage: startPage, maxPage: maxPage) select from print in TelechargementPlus_LoadPost_v1.GetPrints(postHeader.url, reload, loadImage) select print).zView();
        }

        public static void Test_telechargement_plus_loadPostFromWeb_03(bool loadImage = false)
        {
            Trace.WriteLine("Test_telechargement_plus_loadPostFromWeb_03");
            TelechargementPlusInit();
            string dir = XmlConfig.CurrentConfig.Get("TelechargementPlus/CacheDirectory");
            var files = from file in Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories) orderby file descending select file;
            foreach (string file in files)
                Trace.WriteLine(file);
            //TelechargementPlus_LoadPostFromXml load = new TelechargementPlus_LoadPostFromXml();
            var q = from file in Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories) orderby file descending select from print in new TelechargementPlus_LoadPostFromXml_v1(file, loadImage) select print;
            //RunSource.CurrentRunSource.View(q);
            q.zView();
        }

        public static void Test_telechargement_plus_loadPostFromWeb_04(string minTime = null)
        {
            MongoCollection<BsonDocument> collection = TelechargementPlus_GetMongoCollection();
            //IMongoQuery query = new QueryDocument { { "_id", 91230 } };
            IMongoQuery query = null;
            if (minTime != null)
                // minTime "2013-11-07 14:00"
                query = Query.GTE("creationDate", new BsonDateTime(DateTime.ParseExact(minTime, "yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)));
            IMongoSortBy sort = SortBy.Descending("creationDate");
            //QueryDocument query = null;
            MongoCursor<BsonDocument> cursor;
            if (query != null)
                cursor = collection.Find(query);
            else
                cursor = collection.FindAll();
            var q = from doc in cursor.SetSortOrder(sort) select new { _id = doc.zGetInt("_id"), creationDate = doc.zGetDateTime("creationDate") };
            //RunSource.CurrentRunSource.View(q);
            q.zView();
        }

        public static void Test_telechargement_plus_loadPostFromWeb_05(string minTime = null, bool reload = false, bool loadImage = false)
        {
            TelechargementPlusInit();
            MongoCollection<BsonDocument> collection = TelechargementPlus_GetMongoCollection();
            IMongoQuery query = null;
            if (minTime != null)
                // minTime "2013-11-07 14:00"
                query = Query.GTE("creationDate", new BsonDateTime(DateTime.ParseExact(minTime, "yyyy-MM-dd HH:mm", CultureInfo.CurrentCulture)));
            IMongoSortBy sort = SortBy.Descending("creationDate");
            MongoCursor<BsonDocument> cursor;
            if (query != null)
                cursor = collection.Find(query);
            else
                cursor = collection.FindAll();
            var q = from doc in cursor.SetSortOrder(sort) select from print in TelechargementPlus_LoadPost_v1.GetPrints(doc.zGetString("url"), reload: reload, loadImage: loadImage) select print;
            //RunSource.CurrentRunSource.View(q);
            q.zView();
        }
    }
}

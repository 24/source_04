using System;
using System.Data;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using Download.Print.RapideDdl;

namespace Download.Print.RapideDdl.v1
{
    public static class RapideDdl_Exe_v1
    {
        // bool desactivateDocumentStore = false
        public static void Test_RapideDdl_LoadDetailItemList_01(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            //RunSource.CurrentRunSource.View
            RapideDdl.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage, refreshDocumentStore).zView();
        }

        // bool desactivateDocumentStore = false
        public static void Test_RapideDdl_LoadDetailItemList_02(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
            bool refreshDocumentStore = false)
        {
            // RunSource.CurrentRunSource.View
            (from item in RapideDdl.LoadDetailItemList(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImage, refreshDocumentStore)
                     select new
                     {
                         url = item.sourceUrl,
                         creationDate = item.creationDate,
                         title = item.title,
                         images = (from image in item.images select image.Image).ToArray(),
                         downloadLinks = item.downloadLinks
                     }).zView();
        }

        public static void Test_RapideDdl_LoadHeaderPages_01(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            // RunSource.CurrentRunSource.View
            RapideDdl_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reload, loadImage).zView();
        }


        public static void Test_RapideDdl_LoadDetail_01()
        {
            //string url = "http://www.rapide-ddl.com/ebooks/magazine/36143-secrets-dhistoire-de-dgtours-en-france-no1-2014.html";
            //string url = "http://www.rapide-ddl.com/ebooks/magazine/36142-le-point-no2184-du-24-au-30-juillet-2014-lien-direct.html";
            string url = "http://www.rapide-ddl.com/ebooks/bandes-dessinee/36158-x-men-curse-of-the-mutants-sgrie-en-cours-19-tomes-comicmulti.html";
            //string url = "http://www.rapide-ddl.com/ebooks/bandes-dessinee/36153-lgonard-sgrie-en-cours-43-tomes-2hs-bdmulti.html";
            //string url = "http://www.rapide-ddl.com/ebooks/bandes-dessinee/36152-lgon-la-terreur-sgrie-finie-9-tomes-bdmulti.html";
            //string url = "http://www.rapide-ddl.com/ebooks/bandes-dessinee/36151-les-aventures-de-bessy-sgrie-finie-22-tomes-bdmulti.html";
            RapideDdl_PostDetail post = RapideDdl_LoadDetail.Load(url, reload: false, loadImage: true, refreshDocumentStore: true);
            //RunSource.CurrentRunSource.View(post);
            post.zView();
        }

        public static void Test_RapideDdl_Images_01()
        {
            //pb.Data.Mongo.MongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", fields: "{ 'download.images.Source': 1, 'download.images.ImageWidth': 1, 'download.images.ImageHeight': 1 }", limit: 5);
            //string query = "{ 'download.images.ImageHeight': { $gt: 100 } }";
            string query = "{}";
            string fields = "{ 'download.images.Source': 1, 'download.images.ImageWidth': 1, 'download.images.ImageHeight': 1 }";
            int limit = 0;
            MongoCursor<BsonDocument> cursor = MongoCommand.GetDatabase(null, "dl").GetCollection("RapideDdl_Detail").zFind<BsonDocument>(new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query)));
            cursor.SetFields(new FieldsWrapper(BsonSerializer.Deserialize<BsonDocument>(fields)));
            if (limit != 0)
                cursor.SetLimit(limit);
            //DataTable dt = new DataTable();
            //dt.Load()
            //dt.LoadDataRow()
            //dt.ReadXml()
            //foreach (BsonDocument document in cursor)
            //{
            //    BsonArray array = document.GetElement("download").Value.AsBsonDocument.GetElement("images").Value.AsBsonArray;
            //    foreach (BsonValue value in array)
            //    {
            //        //Trace.WriteLine("BsonType : {0}", value.BsonType);
            //        Trace.WriteLine(value.ToJson());
            //    }
            //}
            //DataTable dt = Test_Bson.Test_Bson_f.BsonDocumentsToDataTable(cursor);
            DataTable dt = cursor.zToDataTable2();
            //dt.Select();
            //dt.Rows[0].Delete
            //foreach (DataRow row in dt.Rows)
            //{
            //    object imageHeightValue = row["download.images.ImageHeight"];
            //    //if (imageHeight is int && (int)imageHeight <= 60)
            //    //    row.Delete();
            //    int imageHeight;
            //    if (imageHeightValue is string && int.TryParse((string)imageHeightValue, out imageHeight))
            //    {
            //        if (imageHeight <= 60)
            //            row.Delete();
            //    }
            //}

            for (int i = 0; i < dt.Rows.Count;)
            {
                DataRow row = dt.Rows[i];
                object imageHeightValue = row["download.images.ImageHeight"];
                //if (imageHeight is int && (int)imageHeight <= 60)
                //    row.Delete();
                int imageHeight;
                if (imageHeightValue is string && int.TryParse((string)imageHeightValue, out imageHeight) && imageHeight <= 60)
                {
                    //if (imageHeight <= 60)
                    row.Delete();
                }
                else
                    i++;
            }

            RunSource.CurrentRunSource.SetResult(dt);
        }

        public static void Test_LoadMongoDetail_01()
        {
            Trace.WriteLine("Test_LoadMongoDetail_01");
            Trace.WriteLine();

            RapideDdl.InitMongoClassMap();

            if (BsonClassMap.IsClassMapRegistered(typeof(RapideDdl_Base)))
            {
                BsonClassMap map = BsonClassMap.LookupClassMap(typeof(RapideDdl_Base));
                Trace.WriteLine("change existing class map");
                BsonMemberMap memberMap = map.GetMemberMap("infos");
                memberMap.SetSerializationOptions(DictionarySerializationOptions.ArrayOfDocuments);
            }
            else
            {
                Trace.WriteLine("register class map");
                BsonClassMap.RegisterClassMap<RapideDdl_Base>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.infos).SetSerializationOptions(DictionarySerializationOptions.ArrayOfDocuments);
                });
            }
            Trace.WriteLine();

            string query = "{ _id: 35105 }";
            MongoCursor<BsonDocument> cursor = MongoCommand.GetDatabase(null, "dl").GetCollection("RapideDdl_Detail").zFind<BsonDocument>(new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query)));
            int i = 1;
            foreach (BsonDocument document in cursor)
            {
                BsonDocument document2 = (BsonDocument)document["download"];
                RapideDdl_PostDetail postDetail = BsonSerializer.Deserialize<RapideDdl_PostDetail>(document2);
                //MongoDB.Bson.Serialization.IBsonSerializationOptions options;
                //MongoDB.Bson.Serialization.BsonSerializer.Serialize()
                //Serialization.Options
                //SerializationOptions
                //MongoDB.Bson.Serialization.Options.RepresentationSerializationOptions
                //document2 = postDetail.ToBsonDocument(new DictionarySerializationOptions(DictionaryRepresentation.ArrayOfDocuments));
                //DictionarySerializationOptions.
                //BsonClassMap.RegisterClassMap<RapideDdl_PostDetail>(cm => { cm.MapProperty(c => c.SomeProperty); cm.MapProperty(c => c.AnotherProperty); });
                //document2 = postDetail.ToBsonDocument();
                Trace.WriteLine("document no {0}", i++);
                //DocumentSerializationOptions options = new DocumentSerializationOptions();
                //Trace.WriteLine(postDetail.ToJson(new DictionarySerializationOptions(DictionaryRepresentation.ArrayOfDocuments)));

                //RapideDdl_PostDetail  RapideDdl_Base
                //BsonClassMap<RapideDdl_Base> map = BsonClassMap.LookupClassMap(typeof(RapideDdl_Base));


                //Trace.WriteLine(document.zToJson());
                Trace.WriteLine(postDetail.zToJson());
                Trace.WriteLine();
            }
        }

        public static void Test_ClassMap_01()
        {
            Trace.WriteLine("Test_ClassMap_01");
            Trace.WriteLine();
            //Test_IsClassMapRegistered_01(typeof(RapideDdl_Base));
        }

        public static void Test_IsClassMapRegistered_01(Type type)
        {
            if (BsonClassMap.IsClassMapRegistered(type))
                Trace.WriteLine("class map \"{0}\" is registered", type);
            else
                Trace.WriteLine("class map \"{0}\" is not registered", type);
        }

        public static void Test_RegisterClassMap_RapideDdl_Base_01()
        {
            Trace.WriteLine("register class map RapideDdl_Base");
            BsonClassMap.RegisterClassMap<RapideDdl_Base>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(c => c.infos).SetSerializationOptions(DictionarySerializationOptions.ArrayOfDocuments);
            });
        }
    }
}

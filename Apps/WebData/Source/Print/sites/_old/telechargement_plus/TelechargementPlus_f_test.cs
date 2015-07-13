using System;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.IO;
using pb.Web;
using pb.Web.old;
using Download.Print.TelechargementPlus;

//namespace Print.download
namespace Download.Print
{
    static partial class w
    {
        public static void Test_TelechargementPlus_PostDetail_Serialize_01(string url, bool reload = false, bool loadImage = false)
        {
            Trace.WriteLine("Test_TelechargementPlus_PostDetail_Serialize_01 : url \"{0}\"", url);
            //BsonClassMap.RegisterClassMap<TelechargementPlus_PostDetail>(cm => { cm.AutoMap(); cm.UnmapProperty(c => c.images); });
            BsonClassMap.RegisterClassMap<pb.old.ImageHtml>(cm => { cm.AutoMap(); cm.UnmapProperty(c => c.Image); });
            TelechargementPlus_PostDetail post = TelechargementPlus_LoadDetail.Load(url, reload: reload, loadImage: loadImage);
            Test_TelechargementPlus_PostDetail_Serialize(post);
        }

        public static void Test_TelechargementPlus_PostDetail_Serialize_02(string url, bool reload = false, bool loadImage = false)
        {
            Trace.WriteLine("Test_TelechargementPlus_PostDetail_Serialize_02 : url \"{0}\"", url);
            TelechargementPlus_PostDetail post = LoadPostDetailFromWeb(url, reload, loadImage);
            Test_TelechargementPlus_PostDetail_Serialize(post);
        }

        public static TelechargementPlus_PostDetail LoadPostDetailFromWeb(string url, bool reload = false, bool loadImage = false)
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            pb.Web.v1.RequestFromWeb_v2 request = new pb.Web.v1.RequestFromWeb_v2(url, requestParameters, reload, loadImage);
            return new pb.Web.v1.LoadDataFromWeb_v2<TelechargementPlus_PostDetail>(TelechargementPlus_LoadDetail.LoadPostDetailFromWeb, TelechargementPlus_LoadDetail.GetUrlCache()).Load(request);
        }

        public static void Test_TelechargementPlus_PostDetail_Serialize_03(object key, string database, string collectionName, string itemName = "download", string server = null)
        {
            Trace.WriteLine("Test_TelechargementPlus_PostDetail_Serialize_03");
            BsonDocument document1 = ReadMongoDocument(database, collectionName, key, itemName, server);
            if (document1 == null)
                return;
            //Trace.WriteLine(document1.zToJson());

            Trace.Write("try Deserialize document1 to TelechargementPlus_PostDetail ");
            TryDeserializeDocument<TelechargementPlus_PostDetail>(document1);

            //return;

            //Trace.WriteLine("Deserialize");
            //TelechargementPlus_PostDetail post = BsonSerializer.Deserialize<TelechargementPlus_PostDetail>(document1);
            //Trace.WriteLine("Serialize");
            //var document2 = post.ToBsonDocument();
            //Trace.WriteLine(document2.zToJson());
            //if (document1 == document2)
            //    Trace.WriteLine("documents are identical");
            //else
            //    Trace.WriteLine("documents are different");
        }

        public static void Test_TelechargementPlus_PostDetail_Serialize_05(bool executePostToBsonDocument, string url, object key, string database, string collectionName, string itemName = "download", string server = null, bool reload = false, bool loadImage = false)
        {
            Trace.WriteLine("Test_TelechargementPlus_PostDetail_Serialize_05");
            TelechargementPlus_PostDetail post = LoadPostDetailFromWeb(url, reload, loadImage);
            // très bizarre si post.ToBsonDocument(); pas d'erreur la désérialisation marche
            // si pas de post.ToBsonDocument(); erreur la désérialisation ne marche pas
            if (executePostToBsonDocument)
                post.ToBsonDocument();
            BsonDocument document2 = ReadMongoDocument(database, collectionName, key, itemName, server);

            Trace.Write("try Deserialize document2 to TelechargementPlus_PostDetail ");
            TryDeserializeDocument<TelechargementPlus_PostDetail>(document2);
        }

        public static void Test_TelechargementPlus_PostDetail_Serialize_06()
        {
            Trace.WriteLine("Test_TelechargementPlus_PostDetail_Serialize_05");
            string file = @"c:\pib\dev_data\exe\runsource\test\log\document1.txt";
            Trace.WriteLine("create BsonDocument from \"{0}\"", file);
            string s = zfile.ReadAllText(file);
            BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(s);
            Trace.Write("try Deserialize document to TelechargementPlus_PostDetail ");
            TryDeserializeDocument<TelechargementPlus_PostDetail>(document);
        }

        public static void Test_ZValue_Serialize_01()
        {
            Trace.WriteLine("Test_ZValue_Serialize_01");
            Trace.WriteLine();

            //IDiscriminatorConvention discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(ZValue));
            //Trace.WriteLine("discriminatorConvention(ZValue) : {0}", discriminatorConvention);
            string file = @"c:\pib\dev_data\exe\runsource\test\log\ZValue1.txt";
            Test_DiscriminatorConvention_01(BsonSerializer.Deserialize<BsonDocument>(zfile.ReadAllText(file)));

            Trace.WriteLine("serialize ZString using ToBsonDocument()");
            ZValue value = new ZString("toto");
            BsonDocument document = value.ToBsonDocument();
            //string file = @"c:\pib\dev_data\exe\runsource\test\log\ZValue1.txt";
            //Trace.WriteLine("export ZValue (ZString) to \"{0}\"", file);
            //zfile.WriteFile(file, document.zToJson());
            Trace.WriteLine();

            Test_DiscriminatorConvention_01(document);


            Trace.Write("try Deserialize document to ZValue ");
            TryDeserializeDocument<ZValue>(document);
        }

        public static void Test_DiscriminatorConvention_01(BsonDocument document)
        {
            Trace.WriteLine("Test_DiscriminatorConvention_01");
            BsonReader bsonReader = BsonReader.Create(document);
            IDiscriminatorConvention discriminatorConvention = BsonSerializer.LookupDiscriminatorConvention(typeof(ZValue));
            Trace.WriteLine("discriminatorConvention(ZValue) : {0}", discriminatorConvention);
            Type actualType = discriminatorConvention.GetActualType(bsonReader, typeof(ZValue));
            Trace.WriteLine("actualType : {0}", actualType);
            //discriminatorConvention.GetDiscriminator()
            Trace.WriteLine();
        }

        public static void Test_ZValue_Serialize_02()
        {
            Trace.WriteLine("Test_ZValue_Deserialize_01");

            ////////////////////  c bon il suffit de faire RegisterClassMap<ZString>() pour que la deserialisation fonctionne
            Trace.WriteLine("RegisterClassMap ZString");
            BsonClassMap.RegisterClassMap<ZString>();

            string file = @"c:\pib\dev_data\exe\runsource\test\log\ZValue1.txt";
            Trace.WriteLine("create BsonDocument from \"{0}\"", file);
            string s = zfile.ReadAllText(file);
            BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(s);
            Trace.Write("try Deserialize document to ZValue ");
            TryDeserializeDocument<ZValue>(document);
            //Trace.Write("try Deserialize document to ZString ");
            //TryDeserializeDocument<ZString>(document);
        }

        public static BsonDocument ReadMongoDocument(string database, string collectionName, object key, string itemName, string server = null)
        {
            MongoDatabase mdb = MongoCommand.GetDatabase(server, database);
            Trace.Write("read mongo document key \"{0}\" from collection \"{1}\" ", key, collectionName);
            Trace.WriteLine("server \"mongodb://{0}\" database \"{1}\"", mdb.Server.Instance.Address, mdb.Name);
            MongoCollection collection = mdb.GetCollection(collectionName);
            BsonDocument document = collection.FindOneByIdAs<BsonDocument>(BsonValue.Create(key));
            if (document == null || !document.Contains(itemName))
            {
                Trace.WriteLine("document is null or does'nt contain element \"{0}\"", itemName);
                return null;
            }
            var element = document[itemName];
            if (element == null || !(element is BsonDocument))
            {
                Trace.WriteLine("element \"{0}\" is null or is'nt a BsonDocument", itemName);
                return null;
            }
            document = element as BsonDocument;
            return document;
        }

        public static void Test_TelechargementPlus_PostDetail_Serialize_04(string url, object key, string database, string collectionName, string itemName = "download", string server = null, bool reload = false, bool loadImage = false)
        {
            Trace.WriteLine("Test_TelechargementPlus_PostDetail_Serialize_04");
            TelechargementPlus_PostDetail post = LoadPostDetailFromWeb(url, reload, loadImage);
            BsonDocument document1 = post.ToBsonDocument();
            BsonDocument document2 = ReadMongoDocument(database, collectionName, key, itemName, server);

            Trace.Write("try Deserialize document2 to TelechargementPlus_PostDetail ");
            TryDeserializeDocument<TelechargementPlus_PostDetail>(document2);

            //return;

            ////TelechargementPlus_PostDetail post2 = BsonSerializer.Deserialize<TelechargementPlus_PostDetail>(document2);

            //if (document1.CompareTo(document2) == 0)
            //    Trace.WriteLine("document1 and document2 are identical");
            //else
            //    Trace.WriteLine("document1 and document2 are different");

            ////Trace.WriteLine("document1");
            ////Trace.WriteLine(document1.zToJson());
            ////Trace.WriteLine("document2");
            ////Trace.WriteLine(document2.zToJson());

            //string file1 = @"c:\pib\dev_data\exe\runsource\test\log\document1.txt";
            //Trace.WriteLine("export document1 to \"{0}\"", file1);
            //zfile.WriteFile(file1, document1.zToJson());
            //string file2 = @"c:\pib\dev_data\exe\runsource\test\log\document2.txt";
            //Trace.WriteLine("export document1 to \"{0}\"", file2);
            //zfile.WriteFile(file2, document2.zToJson());

            //Trace.Write("try Deserialize document1 to TelechargementPlus_PostDetail ");
            //TryDeserializeDocument<TelechargementPlus_PostDetail>(document1);
            //Trace.Write("try Deserialize document2 to TelechargementPlus_PostDetail ");
            //TryDeserializeDocument<TelechargementPlus_PostDetail>(document2);

            ////TelechargementPlus_PostDetail post1 = BsonSerializer.Deserialize<TelechargementPlus_PostDetail>(document1);
            ////TelechargementPlus_PostDetail post2 = BsonSerializer.Deserialize<TelechargementPlus_PostDetail>(document2);

        }

        public static void TryDeserializeDocument<T>(BsonDocument document)
        {
            bool error = false;
            try
            {
                T value = BsonSerializer.Deserialize<T>(document);
            }
            catch (Exception ex)
            {
                error = true;
                Trace.WriteLine("error \"{0}\"", ex.Message);
            }
            if (!error)
                Trace.WriteLine("ok");
        }

        public static void Test_BsonDocument_CompareTo_01()
        {
            BsonDocument document1 = new BsonDocument { { "id1", "toto" } };
            BsonDocument document2 = new BsonDocument { { "id1", "toto" } };
            BsonDocument document3 = new BsonDocument { { "id1", "toto2" } };
            if (document1.CompareTo(document2) == 0)
                Trace.WriteLine("document1 and document2 are identical");
            else
                Trace.WriteLine("document1 and document2 are different");
            if (document1.CompareTo(document3) == 0)
                Trace.WriteLine("document1 and document3 are identical");
            else
                Trace.WriteLine("document1 and document3 are different");
        }

        public static void Test_TelechargementPlus_PostDetail_Serialize(TelechargementPlus_PostDetail post)
        {
            Trace.WriteLine("Serialize");
            var document1 = post.ToBsonDocument();
            Trace.WriteLine(document1.zToJson());
            Trace.WriteLine("Deserialize");
            post = BsonSerializer.Deserialize<TelechargementPlus_PostDetail>(document1);
            Trace.WriteLine("Serialize");
            var document2 = post.ToBsonDocument();
            Trace.WriteLine(document2.zToJson());
            if (document1 == document2)
                Trace.WriteLine("documents are identical");
            else
                Trace.WriteLine("documents are different");
        }
    }
}

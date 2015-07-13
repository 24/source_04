using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using pb;
using pb.Compiler;
using pb.Data.Mongo;

//namespace Print.download
namespace Download.Print
{
    // http://docs.mongodb.org/manual/
    // http://docs.mongodb.org/manual/reference/sql-comparison/
    // http://docs.mongodb.org/ecosystem/drivers/csharp/
    // https://github.com/mongodb/mongo-csharp-driver/tree/master
    //  show dbs, show collections
    //  use test
    //  db.test.find()
    //  db.users.find()
    //  db.users.insert( { _id: "abc123", age: 55, status: "A" } )
    //  db.users.insert( { _id: "abc124", age: 55, status: "A", values: [ "toto", "tata", "tutu" ] } )
    //  db.users.insert( { _id: "abc125", age: 55, status: "A", values: [ "toto", "tata", "tutu" ], doc: { text: "zozo", num: 10, values: [ "koko", "kaka", "kuku" ] } } )
    //  db.users.find({ "doc.values": /ko/ })
    //  db.collection.update(<query>, <update>, { upsert: <Boolean>, multi: <Boolean>, } )
    //  db.users.update({ }, { $set: { join_date: new Date() } }, { multi: true } )
    //  db.users.update({ }, { $unset: { join_date: "" } }, { multi: true } )
    //  db.users.drop()
    //  db.users.count()
    //  db.users.find().count()
    //  db.users.distinct( "status" )
    //  db.users.findOne()
    //  db.users.find().limit(1)
    //  db.users.find().limit(5).skip(10)
    //  use dl
    //  db.TelechargementPlus.find().limit(10)
    //  db.TelechargementPlus.find({}, { _id: 1, url: 1, prints: 1 }).limit(10)
    //  db.TelechargementPlus.find({}, { _id: 1, "prints.downloadLinks": 1 }).limit(10)
    //  db.TelechargementPlus.find({ "prints.downloadLinks": /bzh1751.rar/ }, { _id: 1, "prints.downloadLinks": 1 }).limit(10)
    // downloadLinks

    public static class Test_Mongo_Exe
    {
        //public static MongoDatabase GetMongoDatabase(string server = "mongodb://localhost", string database = "test")
        public static MongoDatabase GetMongoDatabase(string server = "mongodb://localhost", string database = "TelechargementPlus")
        {
            //Trace.WriteLine("connect to mongo server \"{0}\" database \"{1}\"", server, database);
            MongoClient mclient = new MongoClient(server);
            MongoServer mserver = mclient.GetServer();
            return mserver.GetDatabase(database);
        }

        public static void Test_Mongo_CollectionName_01(string database, string collectionName)
        {
            MongoDatabase mdb = GetMongoDatabase(database: database);
            MongoCollection collection = mdb.GetCollection(collectionName);
            Trace.WriteLine("server instance address                  : \"{0}\"", mdb.Server.Instance.Address);
            Trace.WriteLine("database name                            : \"{0}\"", mdb.Name);
            Trace.WriteLine("collection name                          : \"{0}\"", collection.Name);
            Trace.WriteLine("collection full name                     : \"{0}\"", collection.FullName);

            // example
            //server instance address                  : "localhost:27017"
            //database name                            : "dl"
            //collection name                          : "TelechargementPlusDetail"
            //collection full name                     : "dl.TelechargementPlusDetail"
        }

        public static void Test_MongoDB_DateTime_01()
        {
            Trace.WriteLine("Test_MongoDB_DateTime_01");
            //1383830499961
            long l = 1383830499961;
            BsonDateTime dt = new BsonDateTime(l);
            Trace.WriteLine("BsonDateTime {0} - {1}", l, dt);
            dt = new BsonDateTime(DateTime.Now);
            Trace.WriteLine("BsonDateTime now {0}", dt);
            Trace.WriteLine("BsonDateTime now.ToLocalTime() {0}", dt.ToLocalTime());
            Trace.WriteLine("BsonDateTime now (DateTime) {0}", (DateTime)dt);
            Trace.WriteLine("DateTime now {0}", DateTime.Now);
        }

        public static void Test_MongoDB_entity_01()
        {
            Trace.WriteLine("Test_MongoDB_entity_01");
            //string server = "mongodb://localhost";
            //string database = "test";
            //Trace.WriteLine("connect to mongo server \"{0}\"", server);
            //MongoClient mclient = new MongoClient(server);
            //MongoServer mserver = mclient.GetServer();
            //Trace.WriteLine("get database \"{0}\"", database);
            //MongoDatabase mdb = mserver.GetDatabase(database);
            MongoDatabase mdb = GetMongoDatabase();
            MongoCollection<Entity> collection = mdb.GetCollection<Entity>("entities");
            Entity entity = new Entity { Name = "Tata" };
            Trace.WriteLine("insert entity");
            collection.Insert(entity);
            ObjectId id = entity.Id;
            Trace.WriteLine("id {0}", id);
        }

        public static void Test_MongoDB_entity_02()
        {
            Trace.WriteLine("Test_MongoDB_entity_02");
            // 526a6df22d8daf1bd42f4f4a Tom
            // 526a72a62d8daf1bd42f4f4b Tata
            List<Entity> entities = new List<Entity>();
            MongoDatabase mdb = GetMongoDatabase();
            MongoCollection<Entity> collection = mdb.GetCollection<Entity>("entities");
            ObjectId id = new ObjectId("526a6df22d8daf1bd42f4f4a");
            Trace.WriteLine("get entity {0}", id);
            IMongoQuery query = Query<Entity>.EQ(e => e.Id, id);
            //Entity entity = collection.FindOne(query);
            entities.Add(collection.FindOne(query));
            entities.Add(collection.FindOne(Query<Entity>.EQ(e => e.Id, new ObjectId("526a72a62d8daf1bd42f4f4b"))));
            //RunSource.CurrentRunSource.View(entities);
            entities.zView();
        }

        public static void Test_MongoDB_data01_01()
        {
            Trace.WriteLine("Test_MongoDB_data01_01");
            MongoDatabase mdb = GetMongoDatabase();
            MongoCollection<Data01> collection = mdb.GetCollection<Data01>("data01");
            //Data01 data = new Data01 { data_id = 1, name = "tata" };
            Data01 data = new Data01 { _id = 1, name = "tata" };
            //Trace.WriteLine("save data01 : {0}", data);
            //collection.Save(data);
            Trace.WriteLine("insert data01 : {0}", data);
            collection.Insert(data);
        }

        public static BsonDocument GetBsonDocument(params object[] prm)
        {
            BsonDocument doc = new BsonDocument();
            bool namePrm = true;
            string name = null;
            foreach (object p in prm)
            {
                if (namePrm)
                    name = (string)p;
                else
                    doc.Add(name, GetBsonValue(p));
                namePrm = !namePrm;
            }
            return doc;
        }

        public static BsonValue GetBsonValue(object value)
        {
            if (value is string)
                return new BsonString((string)value);
            if (value is int)
                return new BsonInt32((int)value);
            else
                return null;
        }

        public static void Test_MongoDB_Insert_01(string collection, params object[] prm)
        {
            Test_MongoDB_Insert_01(collection, GetBsonDocument(prm));
        }

        public static void Test_MongoDB_Insert_01(string collection, BsonDocument doc)
        {
            Trace.WriteLine("Test_MongoDB_Insert_01");
            GetMongoDatabase().GetCollection(collection).Insert(doc);
            //collection.IndexExists
        }

        public static void Test_MongoDB_Save_01(string collection, params object[] prm)
        {
            Test_MongoDB_Save_01(collection, GetBsonDocument(prm));
        }

        public static void Test_MongoDB_Save_01(string collection, BsonDocument doc)
        {
            //BsonDocument d = new BsonDocument("", "");
            Trace.WriteLine("Test_MongoDB_Save_01");
            GetMongoDatabase().GetCollection(collection).Save(doc);
        }

        public static void Test_MongoDB_Update_01(string collection, QueryDocument query, UpdateDocument update)
        {
            Trace.WriteLine("Test_MongoDB_Update_01");
            GetMongoDatabase().GetCollection(collection).Update(query, update);
        }

        public static void Test_MongoDB_Update_Data01_01()
        {
            //Test_MongoDB_Update_01("data01", new QueryDocument { { "_id", 2 } }, new UpdateDocument { { "$set", new BsonDocument { { "age", 20 } } } });
            //BsonArray array = new BsonArray { 
            //    new BsonDocument { { "value", "zaza" } },
            //    new BsonDocument { { "value", "zozo" } },
            //    new BsonDocument { { "value", "zuzu" } },
            //    new BsonDocument { { "value", "zeze" } }
            //};
            //Test_MongoDB_Update_01("data01", new QueryDocument { { "_id", 2 } }, new UpdateDocument { { "$set", new BsonDocument { { "values", array } } } });
            BsonDocument doc = new BsonDocument();
            doc.zAdd("text", "zaaz");
            doc.zAdd("text2", (string)null);
            Test_MongoDB_Update_01("data01", new QueryDocument { { "_id", 3 } }, new UpdateDocument { { "$set", doc } });
        }

        public static void Test_MongoDB_Count_01(string collection, QueryDocument query)
        {
            Trace.WriteLine("Test_MongoDB_Count_01");
            long count = GetMongoDatabase().GetCollection(collection).Count(query);
            Trace.WriteLine("count {0}", count);
        }

        public static void Test_MongoDB_FindAll_01(string collection)
        {
            Trace.WriteLine("Test_MongoDB_FindAll_01");
            MongoDatabase mdb = GetMongoDatabase();
            MongoCursor<BsonDocument> mcursor = mdb.GetCollection(collection).FindAll().SetLimit(10);
            //IEnumerable<BsonDocument> enumDoc = mcursor.AsEnumerable();
            WriteBsonDocuments(mcursor);
            //RunSource.CurrentRunSource.View(mcursor.zGetBsonDocumentsEnumerate());
            mcursor.zGetBsonDocumentsEnumerate().zView();
        }

        public static void Test_MongoDB_Drop_01(string collection)
        {
            MongoDatabase mdb = GetMongoDatabase();
            Trace.WriteLine("drop collection \"{0}\"", collection);
            mdb.GetCollection(collection).Drop();
        }

        public static void WriteBsonDocuments(IEnumerable<IEnumerable<ZBsonElement>> docs)
        {
            foreach (IEnumerable<ZBsonElement> doc in docs)
            {
                Trace.WriteLine("document :");
                foreach (ZBsonElement element in doc)
                {
                    Trace.WriteLine("  {0}", element);
                }
            }
        }

        public static void WriteBsonDocuments(IEnumerable<BsonDocument> docs)
        {
            foreach (BsonDocument doc in docs)
            {
                WriteBsonDocument(doc);
            }
        }

        public static void WriteBsonDocument(BsonDocument doc)
        {
            Trace.WriteLine("BsonType {0}", doc.BsonType);
            Trace.WriteLine("Elements :");
            foreach (BsonElement element in doc.Elements)
            {
                //Trace.WriteLine("  {0} : {1}", element.Name, element.Value);
                Trace.WriteLine("  {0}", (ZBsonElement)element);
            }
        }

        public static void Test_MongoDB_FindAll_02<T>(string collection)
        {
            Trace.WriteLine("Test_MongoDB_FindAll_02");
            MongoDatabase mdb = GetMongoDatabase();
            //MongoCollectionSettings<T> colSettings = new MongoCollectionSettings<T>(mdb, collection);
            //colSettings.ReadPreference.ReadPreferenceMode = ReadPreferenceMode.Nearest;
            //colSettings.WriteConcern = new WriteConcern();
            MongoCollection<T> mcollection = mdb.GetCollection<T>(collection);
            MongoCursor<T> mcursor = mcollection.FindAll();
            mcursor.SetLimit(10);
            //RunSource.CurrentRunSource.View(mcursor);
            mcursor.zView();
        }

        public static void Test_MongoDB_Serialize_01()
        {
            Data01 data = new Data01 { _id = 123, name = "toto" };
            //BsonDocument doc = data.ToBsonDocument();
            BsonDocument doc = data.zToBsonDocument();
            Trace.WriteLine(doc.ToJson());
        }

        public static void Test_MongoDB_Serialize_02()
        {
            BsonSerializer.RegisterSerializationProvider(new MongoDynamicSerializationProvider());
        }

        public class MongoDynamicSerializationProvider : IBsonSerializationProvider
        {
            public IBsonSerializer GetSerializer(Type type)
            {
                //if (typeof(MongoDynamic).IsAssignableFrom(type))
                //    return MongoDynamicBsonSerializer.Instance;
                return null;
            }
        }
    }

    //public static partial class GlobalCast
    //{
    //    public static explicit operator ZBsonElement(BsonElement element)
    //    {
    //        if (element != null)
    //            return new ZBsonElement(element);
    //        else
    //            return null;
    //    }
    //}

    public class Entity
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }

    public class Data01
    {
        //public int data_id { get; set; }
        //public string data_id { get; set; }
        //public string _id { get; set; }
        public int _id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return string.Format("data_id {0} name \"{1}\"", _id, name);
        }
    }
}

namespace Download
{
    public static partial class GlobalExtension
    {
        public static IEnumerable<IEnumerable<ZBsonElement>> zGetBsonDocumentsEnumerate(this IEnumerable<BsonDocument> docs)
        {
            return zmongo.GetBsonDocumentsEnumerate(docs);
        }

        public static void zAdd(this BsonDocument doc, string name, string value)
        {
            if (doc != null)
            {
                BsonValue bsonValue;
                if (value != null)
                    bsonValue = new BsonString(value);
                else
                    bsonValue = BsonNull.Value;
                doc.Add(name, bsonValue);
            }
        }

        public static void zAdd(this BsonDocument doc, string name, IEnumerable<string> values)
        {
            if (doc != null)
            {
                BsonValue bsonValue;
                if (values != null)
                {
                    BsonArray array = new BsonArray();
                    foreach (string value in values)
                        array.Add(new BsonString(value));
                    bsonValue = array;
                }
                else
                    bsonValue = BsonNull.Value;
                doc.Add(name, bsonValue);
            }
        }

        public static void zAdd(this BsonDocument doc, string name, int? value)
        {
            if (doc != null)
            {
                BsonValue bsonValue;
                if (value != null)
                    bsonValue = new BsonInt32((int)value);
                else
                    bsonValue = BsonNull.Value;
                doc.Add(name, bsonValue);
            }
        }

        public static void zAdd(this BsonDocument doc, string name, DateTime? value)
        {
            if (doc != null)
            {
                BsonValue bsonValue;
                if (value != null)
                    bsonValue = new BsonDateTime((DateTime)value);
                else
                    bsonValue = BsonNull.Value;
                doc.Add(name, bsonValue);
            }
        }

        public static BsonArray zAddArray(this BsonDocument doc, string name)
        {
            if (doc != null)
            {
                BsonArray array = new BsonArray();
                doc.Add(name, array);
                return array;
            }
            else
                return null;
        }

        public static BsonDocument zAddDocument(this BsonArray array)
        {
            if (array != null)
            {
                BsonDocument doc = new BsonDocument();
                array.Add(doc);
                return doc;
            }
            else
                return null;
        }

        public static string zGetString(this BsonDocument doc, string name, string defaultValue = null)
        {
            if (doc != null)
            {
                BsonValue value;
                if (doc.TryGetValue(name, out value))
                {
                    if (value != BsonNull.Value)
                        return (string)(BsonString)value;
                }
            }
            return defaultValue;
        }

        public static string[] zGetStringArray(this BsonDocument doc, string name)
        {
            return doc.zGetStringList(name).ToArray();
        }

        public static List<string> zGetStringList(this BsonDocument doc, string name)
        {
            if (doc != null)
            {
                BsonValue value;
                if (doc.TryGetValue(name, out value))
                {
                    if (value != BsonNull.Value)
                    {
                        List<string> texts = new List<string>();
                        foreach (BsonString text in (BsonArray)value)
                        {
                            texts.Add((string)text);
                        }
                        return texts;
                    }
                }
            }
            return new List<string>();
        }

        public static int? zGetInt(this BsonDocument doc, string name, int? defaultValue = null)
        {
            if (doc != null)
            {
                BsonValue value;
                if (doc.TryGetValue(name, out value))
                {
                    if (value != BsonNull.Value)
                        return (int)(BsonInt32)value;
                }
            }
            return defaultValue;
        }

        public static DateTime? zGetDateTime(this BsonDocument doc, string name, DateTime? defaultValue = null)
        {
            if (doc != null)
            {
                BsonValue value;
                if (doc.TryGetValue(name, out value))
                {
                    if (value != BsonNull.Value)
                        //return (DateTime)(BsonDateTime)value;
                        return ((BsonDateTime)value).ToLocalTime();
                }
            }
            return defaultValue;
        }

        public static BsonArray zGetBsonArray(this BsonDocument doc, string name)
        {
            if (doc != null)
            {
                BsonValue value;
                if (doc.TryGetValue(name, out value))
                {
                    if (value != BsonNull.Value)
                        return (BsonArray)value;
                }
            }
            return new BsonArray();
        }
    }
}

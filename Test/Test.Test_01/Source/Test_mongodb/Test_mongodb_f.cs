using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using pb;
using pb.Compiler;

namespace Test.Test_mongodb
{
    static partial class w
    {
        //private static Trace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            Trace.WriteLine("Test_01");
        }

        public static MongoDatabase GetMongoDatabase(string server = "mongodb://localhost", string database = "test")
        {
            Trace.WriteLine("connect to mongo server \"{0}\" database \"{0}\"", server, database);
            MongoClient mclient = new MongoClient(server);
            MongoServer mserver = mclient.GetServer();
            return mserver.GetDatabase(database);
        }

        public static void Test_MongoDB_01()
        {
            Trace.WriteLine("Test_MongoDB_01");
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

        public static void Test_MongoDB_02()
        {
            Trace.WriteLine("Test_MongoDB_02");
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
            entities.zView();
        }

        public static void Test_MongoDB_FindAll_01(string collection)
        {
            MongoDatabase mdb = GetMongoDatabase();
            MongoCollection<Entity> mcollection = mdb.GetCollection<Entity>(collection);
            mcollection.FindAll().zView();
        }
    }

    public class Entity
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}

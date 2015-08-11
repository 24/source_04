using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace pb.Data.Mongo
{
    //public class MongoIdCollection
    //{
    //    public int id;
    //    public int lastId;
    //}

    public static class MongoIdGenerator
    {
        public static MongoIdGenerator<TId> GetIdGenerator<TId>()
        {
            if (typeof(TId) == typeof(int))
            {
                return new MongoIdGeneratorInt() as MongoIdGenerator<TId>;
            }
            else
                throw new PBException("error there is no mongo id generator for type {0}", typeof(TId).zGetTypeName());
        }
    }

    public abstract class MongoIdGenerator<TId>
    {
        public abstract TId GenerateId();
    }

    public class MongoIdGeneratorInt : MongoIdGenerator<int>
    {
        public override int GenerateId()
        {
            /***************************************************************************************************************************************************************
                from Create an Auto-Incrementing Sequence Field  http://docs.mongodb.org/manual/tutorial/create-an-auto-incrementing-field/
                    function getNextSequence(name) {
                       var ret = db.counters.findAndModify(
                              {
                                query: { _id: name },
                                update: { $inc: { seq: 1 } },
                                new: true,
                                upsert: true
                              }
                       );

                       return ret.seq;
                    }
            ***************************************************************************************************************************************************************/
            throw new PBException("not implemented");
        }
    }

    public class MongoIdGenerator_v2
    {
        public int GenerateId(string database, string collection, string server = null)
        {
            //MongoIdCollection idCollection = null;
            //if (!_idCollections.ContainsKey(collection))
            //{
            //    BsonDocument document = GetCollection().zFindOneAs<BsonDocument>(new QueryDocument { { "collection", collection } });
            //    if (document == null)
            //    {
            //        if (collection == "IdGenerator")
            //            idCollection = new MongoIdCollection { id = 1, collection = collection, lastId = 1 };
            //        else
            //            idCollection = new MongoIdCollection { id = GenerateId("IdGenerator"), collection = collection, lastId = 0 };
            //    }
            //    else
            //        idCollection = BsonSerializer.Deserialize<MongoIdCollection>(document);
            //    _idCollections.Add(collection, idCollection);
            //}
            //else
            //    idCollection = _idCollections[collection];
            //idCollection.lastId++;
            //// save idCollection
            //GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(idCollection.id) } }, new UpdateDocument { { "$set", idCollection.zToBsonDocument() } }, UpdateFlags.Upsert);
            //return idCollection.lastId;
            return 0;
        }
    }

    public class MongoIdCollection_v1
    {
        public int id;
        public string collection;
        public int lastId;
    }

    public class MongoIdGenerator_v1
    {
        private string _server = null;
        private string _database = null;
        private string _collectionName = null;
        private MongoCollection _collection = null;
        private Dictionary<string, MongoIdCollection_v1> _idCollections = new Dictionary<string, MongoIdCollection_v1>();

        public MongoIdGenerator_v1(string server, string database, string collectionName = "IdGenerator")
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
        }

        public int GenerateId(string collection)
        {
            MongoIdCollection_v1 idCollection = null;
            if (!_idCollections.ContainsKey(collection))
            {
                BsonDocument document = GetCollection().zFindOne<BsonDocument>(new QueryDocument { { "collection", collection } });
                if (document == null)
                {
                    if (collection == "IdGenerator")
                        idCollection = new MongoIdCollection_v1 { id = 1, collection = collection, lastId = 1 };
                    else
                        idCollection = new MongoIdCollection_v1 { id = GenerateId("IdGenerator"), collection = collection, lastId = 0 };
                }
                else
                    idCollection = BsonSerializer.Deserialize<MongoIdCollection_v1>(document);
                _idCollections.Add(collection, idCollection);
            }
            else
                idCollection = _idCollections[collection];
            idCollection.lastId++;
            // save idCollection
            GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(idCollection.id) } }, new UpdateDocument { { "$set", idCollection.zToBsonDocument() } }, UpdateFlags.Upsert);
            return idCollection.lastId;
        }

        private MongoCollection GetCollection()
        {
            if (_collection == null)
            {
                if (_server == null)
                    throw new PBException("error mongo server is'nt defined");
                if (_database == null)
                    throw new PBException("error mongo database is'nt defined");
                if (_collectionName == null)
                    throw new PBException("error mongo collection is'nt defined");
                _collection = new MongoClient(_server).GetServer().GetDatabase(_database).GetCollection(_collectionName);
            }
            return _collection;
        }
    }
}

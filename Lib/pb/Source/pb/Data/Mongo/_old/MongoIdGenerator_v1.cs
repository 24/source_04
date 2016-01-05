using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace pb.Data.Mongo.old
{
    public class MongoIdCollection_v1
    {
        public int id;
        public string collection;
        public int lastId;
    }

    // version 1 :
    //   LastId est géré dans une collection spécifique ("IdGenerator")
    //   on a un document par collection
    //  {
    //    "_id" : 1,
    //    "collection" : "IdGenerator",
    //    "lastId" : 10
    //  }, {
    //    "_id" : 2,
    //    "collection" : "Download",
    //    "lastId" : 162
    //  },
    //  ...
    //  problème : pour copier la collection sur une autre database il faut copier le LastId séparément
    //  ATTENTION l'utilisation du Dictionary _idCollections n'est pas une bonne idée
    //  il faut utiliser FindAndModify()
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

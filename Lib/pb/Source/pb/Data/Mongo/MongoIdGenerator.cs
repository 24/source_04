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

    //public static class MongoIdGenerator
    //{
    //    public static MongoIdGenerator<TId> GetIdGenerator<TId>()
    //    {
    //        if (typeof(TId) == typeof(int))
    //        {
    //            return new MongoIdGeneratorInt() as MongoIdGenerator<TId>;
    //        }
    //        else
    //            throw new PBException("error there is no mongo id generator for type {0}", typeof(TId).zGetTypeName());
    //    }
    //}

    //public abstract class MongoIdGenerator<TId>
    //{
    //    public abstract TId GenerateId();
    //}

    //public class MongoIdGeneratorInt : MongoIdGenerator<int>
    //{
    //    public override int GenerateId()
    //    {
    //        /***************************************************************************************************************************************************************
    //            from Create an Auto-Incrementing Sequence Field  http://docs.mongodb.org/manual/tutorial/create-an-auto-incrementing-field/
    //                function getNextSequence(name) {
    //                   var ret = db.counters.findAndModify(
    //                          {
    //                            query: { _id: name },
    //                            update: { $inc: { seq: 1 } },
    //                            new: true,
    //                            upsert: true
    //                          }
    //                   );

    //                   return ret.seq;
    //                }
    //        ***************************************************************************************************************************************************************/
    //        throw new PBException("not implemented");
    //    }
    //}

    // version 2 :
    //   LastId est géré par chaque collection
    //   le document _id 0 contient le LastId
    //   { _id: 0, LastId: 123 }
    public class MongoIntIdGenerator
    {
        private string _collectionName = null;
        private string _databaseName = null;
        private string _serverName = null;
        private MongoCollection _collection = null;
        private bool _collectionContainsLastId = false;

        public MongoIntIdGenerator(string collectionName, string databaseName, string serverName = null)
        {
            _collectionName = collectionName;
            _databaseName = databaseName;
            _serverName = serverName;
        }

        public MongoIntIdGenerator(MongoCollection collection)
        {
            _collection = collection;
        }

        public int GetNewId()
        {
            MongoCollection collection = GetCollection();
            // on utilise FindAndModify pour 
            FindAndModifyResult result = collection.zFindAndModify(new QueryDocument { { "_id", 0 } }, new UpdateDocument { { "$inc", new BsonDocument { { "LastId", 1 } } } }, upsert: true);
            // ATTENTION result.ModifiedDocument est le document avant update donc il faut faire + 1
            // si result.ModifiedDocument est null le document _id 0 n'existait pas, il est créé (upsert: true) après update on a { _id: 0, LastId: 1 }
            if (result.ModifiedDocument != null)
                return result.ModifiedDocument["LastId"].AsInt32 + 1;
            else
                return 1;
        }

        public bool GetCollectionContainsIdGeneratorDocument()
        {
            if (!_collectionContainsLastId)
                _collectionContainsLastId = GetCollection().Count(new QueryDocument { { "_id", 0 } }) == 1;
            return _collectionContainsLastId;
        }

        public QueryDocument GetQueryToSkipIdGeneratorDocument(QueryDocument queryDocument)
        {
            // if query parameter is not empty and collection contains id generator document
            //   return query document { $and: [ { _id: { $ne: 0 } }, <query parameter> ] }
            // if query parameter is empty and collection contains id generator document
            //   return query document { _id: { $ne: 0 } }
            // if collection does not contains id generator document
            //   return <query parameter>
            if (GetCollectionContainsIdGeneratorDocument())
            {
                if (queryDocument.ElementCount != 0)
                    queryDocument = new QueryDocument { { "$and", new BsonArray { new BsonDocument { { "_id", new BsonDocument { { "$ne", 0 } } } }, queryDocument } } };
                else
                    queryDocument = new QueryDocument { { "_id", new BsonDocument { { "$ne", 0 } } } };
            }
            return queryDocument;
        }

        private MongoCollection GetCollection()
        {
            if (_collection == null)
                _collection = zmongo.GetCollection(_collectionName, _databaseName, _serverName);
            return _collection;
        }

        public static void TransfertOldLastId(string fromCollection, string fromDatabase, string toCollection = null, string toDatabase = null, string IdGeneratorCollection = "IdGenerator", string fromServer = null, string toServer = null)
        {
            if (toCollection == null)
                toCollection = fromCollection;
            if (toDatabase == null)
                toDatabase = fromDatabase;
            Trace.WriteLine("transfert last id from collection \"{0}\" last id collection \"{1}\" database \"{2}\" server \"{3}\" to collection \"{4}\" database \"{5}\" server \"{6}\"",
                IdGeneratorCollection, fromCollection, fromDatabase, fromServer, toCollection, toDatabase, toServer);
            MongoCollection collection = zmongo.GetCollection(fromDatabase, IdGeneratorCollection, fromServer);
            if (!collection.Exists())
                throw new PBException("collection not found \"{0}\" database \"{1}\" server \"{2}\"", IdGeneratorCollection, fromDatabase, fromServer);
            BsonDocument doc = collection.FindOneAs<BsonDocument>(new QueryDocument { { "collection", fromCollection } });
            if (doc == null)
                throw new PBException("last id not found for collection \"{0}\"", fromCollection);
            int lastId = doc["lastId"].AsInt32;
            collection = zmongo.GetCollection(toDatabase, toCollection, toServer);
            doc = collection.FindOneAs<BsonDocument>(new QueryDocument { { "_id", 0 } });
            if (doc != null)
                throw new PBException("document _id 0 exists already in collection \"{0}\" database \"{1}\" server \"{2}\"", toCollection, toDatabase, toServer);
            doc = new BsonDocument { { "_id", 0 }, { "LastId", lastId } };
            WriteConcernResult result = collection.Insert(doc);
            if (result.Ok)
            {
                Trace.WriteLine("last id transfered to collection \"{0}\" database \"{1}\" server \"{2}\"", toCollection, toDatabase, toServer);
                Trace.WriteLine(doc.zToJson());
            }
            else
                throw new PBException("unknow error inserting last id to collection \"{0}\" database \"{1}\" server \"{2}\"", toCollection, toDatabase, toServer);
        }
    }
}

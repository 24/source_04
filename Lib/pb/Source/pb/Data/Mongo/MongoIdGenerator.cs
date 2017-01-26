using MongoDB.Bson;
using MongoDB.Driver;

namespace pb.Data.Mongo
{
    // version 2 :
    //   LastId est géré par chaque collection
    //   le document _id 0 contient le LastId
    //   { _id: 0, LastId: 123 }
    public abstract class MongoIdGenerator
    {
        protected string _collectionName = null;
        protected string _databaseName = null;
        protected string _serverName = null;
        protected MongoCollection _collection = null;
        protected bool _collectionContainsLastId = false;

        public MongoIdGenerator(string collectionName, string databaseName, string serverName = null)
        {
            _collectionName = collectionName;
            _databaseName = databaseName;
            _serverName = serverName;
        }

        public MongoIdGenerator(MongoCollection collection)
        {
            _collection = collection;
        }

        public abstract BsonValue GetNewId();
        public abstract bool GetCollectionContainsIdGeneratorDocument();
        public abstract QueryDocument GetQueryToSkipIdGeneratorDocument(QueryDocument queryDocument);

        protected MongoCollection GetCollection()
        {
            if (_collection == null)
                _collection = zMongoDB.GetCollection(_collectionName, _databaseName, _serverName);
            return _collection;
        }
    }

    public class MongoIdGeneratorInt : MongoIdGenerator
    {
        public MongoIdGeneratorInt(string collectionName, string databaseName, string serverName = null) : base(collectionName, databaseName, serverName)
        {
        }

        public MongoIdGeneratorInt(MongoCollection collection) : base(collection)
        {
        }

        public override BsonValue GetNewId()
        {
            MongoCollection collection = GetCollection();
            // on utilise FindAndModify pour 
            FindAndModifyResult result = collection.zFindAndModify(new QueryDocument { { "_id", 0 } }, new UpdateDocument { { "$inc", new BsonDocument { { "LastId", 1 } } } }, upsert: true);
            // ATTENTION result.ModifiedDocument est le document avant update donc il faut faire + 1
            // si result.ModifiedDocument est null le document _id 0 n'existait pas, il est créé (upsert: true) après update on a { _id: 0, LastId: 1 }
            int id = 1;
            if (result.ModifiedDocument != null)
                id = result.ModifiedDocument["LastId"].AsInt32 + 1;
            return BsonValue.Create(id);
        }

        public override bool GetCollectionContainsIdGeneratorDocument()
        {
            if (!_collectionContainsLastId)
                _collectionContainsLastId = GetCollection().Count(new QueryDocument { { "_id", 0 } }) == 1;
            return _collectionContainsLastId;
        }

        public override QueryDocument GetQueryToSkipIdGeneratorDocument(QueryDocument queryDocument)
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
    }
}

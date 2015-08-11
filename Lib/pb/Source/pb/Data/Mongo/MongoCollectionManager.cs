using System;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Xml.Linq;
using pb.Data.Xml;

namespace pb.Data.Mongo
{
    public class PBMongoCollection
    {
        protected string _server = null;
        protected string _database = null;
        protected string _collectionName = null;
        protected MongoCollection _collection = null;

        public PBMongoCollection(string server, string database, string collectionName)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
        }

        public MongoCollection GetCollection()
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

        public string GetCollectionFullName()
        {
            return string.Format("server \"{0}\" database \"{1}\" collection \"{2}\"", _server, _database, _collectionName);
        }
    }

    public class MongoCollectionManager<TId, TData> : PBMongoCollection
    {
        protected string _itemName = null;
        protected MongoIdGenerator<TId> _idGenerator = null;

        public MongoCollectionManager(string server, string database, string collectionName, string itemName = null, MongoIdGenerator<TId> idGenerator = null)
            : base (server, database, collectionName)
        {
            _itemName = itemName;
            _idGenerator = idGenerator;
        }

        public TData Load(TId id)
        {
            // new QueryDocument { { "queueDownloadFile.key.server", key.server }, { "queueDownloadFile.key.id", BsonValue.Create(key.id) } }
            BsonDocument document = GetCollection().zFindOne<BsonDocument>(new QueryDocument { { "_id", BsonValue.Create(id) } });
            if (document != null)
                return Deserialize(document);
            else
                return default(TData);
        }

        public TId GetNewId()
        {
            return _idGenerator.GenerateId();
        }


        public void Save(TId id, TData data)
        {
            BsonDocument document = Serialize(data);

            if (_itemName != null)
                document = new BsonDocument(_itemName, document);

            GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(id) } }, new UpdateDocument { { "$set", document } }, UpdateFlags.Upsert);
        }

        public void Remove(TId id)
        {
            GetCollection().zRemove(new QueryDocument { { "_id", BsonValue.Create(id) } });
        }

        protected virtual BsonDocument Serialize(TData data)
        {
            return data.ToBsonDocument(typeof(TData));
        }

        protected virtual TData Deserialize(BsonDocument document)
        {
            if (_itemName != null)
            {
                if (!document.Contains(_itemName))
                    throw new PBException("error load data : document does'nt contain element \"{0}\" {1}", _itemName, GetCollectionFullName());
                var element = document[_itemName];
                if (element == null)
                    throw new PBException("error load data : element \"{0}\" is null {1}", _itemName, GetCollectionFullName());
                if (!(element is BsonDocument))
                    throw new PBException("error load data : element \"{0}\" is not a document {1}", _itemName, GetCollectionFullName());
                document = element as BsonDocument;
            }
            return BsonSerializer.Deserialize<TData>(document);
        }
    }

    public class MongoCollectionManager<TId, TData, TKey> : MongoCollectionManager<TId, TData>
    {
        protected Func<TKey, QueryDocument> _getQueryKey = null;

        public MongoCollectionManager(string server, string database, string collectionName, Func<TKey, QueryDocument> getQueryKey, string itemName = null, MongoIdGenerator<TId> idGenerator = null)
            : base(server, database, collectionName, itemName, idGenerator)
        {
            _getQueryKey = getQueryKey;
        }

        public TData LoadFromKey(TKey key)
        {
            BsonDocument document = GetCollection().zFindOne<BsonDocument>(_getQueryKey(key));
            if (document != null)
                return Deserialize(document);
            else
                return default(TData);
        }
    }

    public class MongoCollectionManager_v1<TKey, TData>
    {
        private string _server = null;
        private string _database = null;
        private string _collectionName = null;
        private string _itemName = null;
        private MongoCollection _collection = null;
        private bool _useCursorCache = true;
        private Func<TKey, QueryDocument> _queryKey = null;
        private MongoIdGenerator_v1 _idGenerator = null;

        // Func<TKey, QueryDocument> queryKey
        public MongoCollectionManager_v1(string server, string database, string collectionName, string itemName = null)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _itemName = itemName;
            _idGenerator = new MongoIdGenerator_v1(server, database);
        }

        public MongoCollectionManager_v1(XElement xe)
        {
            _server = xe.zXPathExplicitValue("MongoServer");
            _database = xe.zXPathExplicitValue("MongoDatabase");
            _collectionName = xe.zXPathExplicitValue("MongoCollection");
            _itemName = xe.zXPathValue("MongoDocumentItemName");
            _idGenerator = new MongoIdGenerator_v1(_server, _database);
        }

        public bool UseCursorCache { get { return _useCursorCache; } set { _useCursorCache = value; } }
        public Func<TKey, QueryDocument> QueryKey { get { return _queryKey; } set { _queryKey = value; } }

        public TData Load(TKey key)
        {
            BsonDocument document = GetCollection().zFindOne<BsonDocument>(_queryKey(key));
            if (document != null)
                return Deserialize(document);
            else
                return default(TData);
        }

        public int GetNewId()
        {
            return _idGenerator.GenerateId(_collectionName);
        }


        public void Save(int id, TData data)
        {
            BsonDocument document = Serialize(data);

            if (_itemName != null)
                document = new BsonDocument(_itemName, document);

            GetCollection().zUpdate(new QueryDocument { { "_id", BsonValue.Create(id) } }, new UpdateDocument { { "$set", document } }, UpdateFlags.Upsert);
        }

        public WriteConcernResult Remove(int id)
        {
            return GetCollection().zRemove(new QueryDocument { { "_id", BsonValue.Create(id) } });
        }

        public WriteConcernResult RemoveFromKey(TKey key)
        {
            return GetCollection().zRemove(_queryKey(key));
        }

        public long Count(string query = null)
        {
            return GetCollection().zCount(query.zToQueryDocument());
        }

        // bool useCursorCache = false
        public IEnumerable<TData> Find(string query, string sort = null, int limit = 0, string options = null)
        {
            if (query == null)
                query = "{}";
            IEnumerable<BsonDocument> cursor = GetCollection().zFind<BsonDocument>(query.zToQueryDocument(), sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument());
            if (_useCursorCache)
                cursor = cursor.zCacheCursor();
            return cursor.Select(Deserialize);
        }

        protected virtual BsonDocument Serialize(TData data)
        {
            return data.ToBsonDocument(typeof(TData));
        }

        protected virtual TData Deserialize(BsonDocument document)
        {
            if (_itemName != null)
            {
                if (!document.Contains(_itemName))
                    throw new PBException("error load data : document does'nt contain element \"{0}\" {1}", _itemName, GetCollectionFullName());
                var element = document[_itemName];
                if (element == null)
                    throw new PBException("error load data : element \"{0}\" is null {1}", _itemName, GetCollectionFullName());
                if (!(element is BsonDocument))
                    throw new PBException("error load data : element \"{0}\" is not a document {1}", _itemName, GetCollectionFullName());
                document = element as BsonDocument;
            }
            return BsonSerializer.Deserialize<TData>(document);
        }

        public MongoCollection GetCollection()
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

        private string GetCollectionFullName()
        {
            return string.Format("server \"{0}\" database \"{1}\" collection \"{2}\"", _server, _database, _collectionName);
        }
    }
}

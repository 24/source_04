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

    // ATTENTION l'utilisation de MongoIntIdGenerator ajoute un document dans la collection pour gérer la last id
    //   donc les fonctions Count() et Find() sont modifiées pour en tenir compte
    public class MongoCollectionManager_v1<TKey, TData>
    {
        private string _serverName = null;
        private string _databaseName = null;
        private string _collectionName = null;
        private string _itemName = null;
        private MongoCollection _collection = null;
        private bool _useCursorCache = true;
        private Func<TKey, QueryDocument> _queryKey = null;
        //private MongoIdGenerator_v1 _idGenerator = null;
        private MongoIntIdGenerator _idGenerator = null;

        // Func<TKey, QueryDocument> queryKey
        public MongoCollectionManager_v1(string serverName, string databaseName, string collectionName, string itemName = null)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _itemName = itemName;
            //_idGenerator = new MongoIdGenerator_v1(server, database);
            _idGenerator = new MongoIntIdGenerator(collectionName, databaseName, serverName);
        }

        public MongoCollectionManager_v1(XElement xe)
        {
            _serverName = xe.zXPathExplicitValue("MongoServer");
            _databaseName = xe.zXPathExplicitValue("MongoDatabase");
            _collectionName = xe.zXPathExplicitValue("MongoCollection");
            _itemName = xe.zXPathValue("MongoDocumentItemName");
            //_idGenerator = new MongoIdGenerator_v1(_server, _database);
            _idGenerator = new MongoIntIdGenerator(_collectionName, _databaseName, _serverName);
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
            //return _idGenerator.GenerateId(_collectionName);
            //return MongoIntIdGenerator.GetNewId(GetCollection());
            return _idGenerator.GetNewId();
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
            return GetCollection().zCount(query.zToQueryDocument()) - (_idGenerator.GetCollectionContainsIdGeneratorDocument() ? 1 : 0);
        }

        public IEnumerable<TData> Find(string query, string sort = null, int limit = 0, string options = null)
        {
            if (query == null)
                query = "{}";
            QueryDocument queryDocument = _idGenerator.GetQueryToSkipIdGeneratorDocument(query.zToQueryDocument());
            IEnumerable<BsonDocument> cursor = GetCollection().zFind<BsonDocument>(queryDocument, sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument());
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
                if (_serverName == null)
                    throw new PBException("error mongo server is'nt defined");
                if (_databaseName == null)
                    throw new PBException("error mongo database is'nt defined");
                if (_collectionName == null)
                    throw new PBException("error mongo collection is'nt defined");
                _collection = new MongoClient(_serverName).GetServer().GetDatabase(_databaseName).GetCollection(_collectionName);
            }
            return _collection;
        }

        private string GetCollectionFullName()
        {
            return string.Format("server \"{0}\" database \"{1}\" collection \"{2}\"", _serverName, _databaseName, _collectionName);
        }
    }
}

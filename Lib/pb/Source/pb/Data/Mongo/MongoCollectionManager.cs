using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using pb.Data.Xml;
using System;
using MongoDB.Driver.Wrappers;

namespace pb.Data.Mongo
{
    //public interface IPartialQuery
    //{
    //    IEnumerable<KeyValuePair<string, object>> GetQuery();
    //}

    public class MongoCollectionManager<TData>
    {
        private string _serverName = null;
        private string _databaseName = null;
        private string _collectionName = null;
        private string _itemName = null;
        private bool _addItemNameToQueryKey = true;
        private string _keyName = null;
        private MongoCollection _collection = null;
        private Type _nominalType = null;                               // type to use for serialize and deserialize
        private bool _useCursorCache = true;
        private bool _generateId = false;
        private MongoIdGenerator _idGenerator = null;
        private string _defaultSort = null;

        public MongoCollectionManager(string serverName, string databaseName, string collectionName, string itemName = null)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _itemName = itemName;
        }

        //public MongoCollectionManager_v2(XElement xe)
        //{
        //    _serverName = xe.zXPathExplicitValue("MongoServer");
        //    _databaseName = xe.zXPathExplicitValue("MongoDatabase");
        //    _collectionName = xe.zXPathExplicitValue("MongoCollection");
        //    _itemName = xe.zXPathValue("MongoDocumentItemName");
        //}

        public bool AddItemNameToQueryKey { get { return _addItemNameToQueryKey; } set { _addItemNameToQueryKey = value; } }
        public string KeyName { get { return _keyName; } set { _keyName = value; } }
        public Type NominalType { get { return _nominalType; } set { _nominalType = value; } }
        public bool GenerateId { get { return _generateId; } set { _generateId = value; } }
        public bool UseCursorCache { get { return _useCursorCache; } set { _useCursorCache = value; } }
        public MongoIdGenerator IdGenerator { get { return _idGenerator; } set { _idGenerator = value; } }
        public string DefaultSort { get { return _defaultSort; } set { _defaultSort = value; } }

        public bool ExistsFromId(BsonValue id)
        {
            return _Exists(GetQuery(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("_id", id) }, addPrefix: false));
        }

        public bool ExistsFromKey(IQuery key, bool addPrefix = true)
        {
            return _Exists(GetQuery(key.GetQueryValues(), addPrefix));
        }

        public bool ExistsFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = true)
        {
            return _Exists(GetQuery(keyValues, addPrefix));
        }

        private bool _Exists(QueryDocument query)
        {
            long n = GetCollection().zCount(query);
            if (n == 1)
                return true;
            else if (n == 0)
                return false;
            else
                throw new PBException("unable evaluate if documant exists, count {0} query {1}", n, query);
        }

        private static FieldsWrapper __IdField = "{ _id: 1 }".zToFieldsWrapper();
        public BsonValue GetIdFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = true)
        {
            QueryDocument query = GetQuery(keyValues, addPrefix);
            BsonDocument document = GetCollection().zFind<BsonDocument>(query, fields: __IdField).FirstOrDefault();
            if (document != null)
                return document["_id"];
            else
                return null;
        }

        public TData LoadFromId(BsonValue id)
        {
            BsonDocument document = GetCollection().zFindOneById<BsonDocument>(id);
            if (document == null)
                throw new PBException("error mongo document not found id \"{0}\" {1}", id, GetCollectionFullName());
            return Deserialize(document);
        }

        //public TData Load<TKey>(TKey key)
        public TData LoadFromKey(IQuery key, bool addPrefix = true, bool throwException = false)
        {
            //if (!(key is IQuery))
            //    throw new PBException("key is not IQuery {0}", key.GetType().zGetTypeName());

            //BsonDocument document = GetCollection().zFindOne<BsonDocument>(_queryKey(key));
            //BsonDocument document = GetCollection().zFindOne<BsonDocument>((key as IQuery).GetQuery());
            //BsonDocument document = GetCollection().zFindOne<BsonDocument>(key.GetQuery().zToQueryDocument());
            //BsonDocument document = GetCollection().zFindOne<BsonDocument>(new QueryDocument(key.GetQuery()));
            //BsonDocument document = GetCollection().zFindOne<BsonDocument>(GetQuery(key.GetQueryValues(), addPrefix));
            //if (document != null)
            //    return Deserialize(document);
            //else
            //    return default(TData);
            return _LoadFromKey(GetQuery(key.GetQueryValues(), addPrefix), throwException);
        }

        public TData LoadFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = true, bool throwException = false)
        {
            return _LoadFromKey(GetQuery(keyValues, addPrefix), throwException);
        }

        private TData _LoadFromKey(QueryDocument query, bool throwException = false)
        {
            BsonDocument document = GetCollection().zFindOne<BsonDocument>(query);
            if (document != null)
                return Deserialize(document);
            else if (throwException)
                throw new PBException("mongo document not found query {0} collection {1}", query, GetCollectionFullName());
            else
                return default(TData);
        }

        public BsonValue GetNewId()
        {
            return _idGenerator.GetNewId();
        }

        public void Save(BsonValue id, TData data)
        {
            _Save(GetQuery(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("_id", id) }, addPrefix: false), data);
        }

        public void Save(IQuery key, TData data, bool addPrefix = true)
        {
            _Save(GetQuery(key.GetQueryValues(), addPrefix), data);
        }

        public void Save(IEnumerable<KeyValuePair<string, object>> keyValues, TData data, bool addPrefix = true)
        {
            _Save(GetQuery(keyValues, addPrefix), data);
        }

        private void _Save(QueryDocument query, TData data)
        {
            BsonDocument document = Serialize(data);

            if (_itemName != null)
                document = new BsonDocument(_itemName, document);

            //GetCollection().zUpdate(new QueryDocument { { "_id", id } }, new UpdateDocument { { "$set", document } }, UpdateFlags.Upsert);
            GetCollection().zUpdate(query, new UpdateDocument { { "$set", document } }, UpdateFlags.Upsert);
        }

        public WriteConcernResult Remove(BsonValue id)
        {
            return GetCollection().zRemove(new QueryDocument { { "_id", id } });
        }

        //public WriteConcernResult RemoveFromKey<TKey>(TKey key)
        public WriteConcernResult RemoveFromKey(IQuery key, bool addPrefix = true)
        {
            //if (!(key is IQuery))
            //    throw new PBException("key is not IQuery {0}", key.GetType().zGetTypeName());
            //return GetCollection().zRemove(_queryKey(key));
            //return GetCollection().zRemove((key as IQuery).GetQuery());
            //return GetCollection().zRemove(key.GetQuery().zToQueryDocument());
            //return GetCollection().zRemove(new QueryDocument(key.GetQuery()));
            return GetCollection().zRemove(GetQuery(key.GetQueryValues(), addPrefix));
        }

        public long Count(string query = null)
        {
            return _Count(query.zToQueryDocument());
        }

        private long _Count(QueryDocument query)
        {
            //return GetCollection().zCount(query) - (query == null && _idGenerator.GetCollectionContainsIdGeneratorDocument() ? 1 : 0);
            return GetCollection().zCount(query) - (query == null && GetCollectionContainsIdGeneratorDocument() ? 1 : 0);
        }

        public IEnumerable<TData> Find(string query, string sort = null, int limit = 0, string options = null)
        {
            //if (query == null)
            //    query = "{}";
            //if (sort == null)
            //    sort = _defaultSort;
            //QueryDocument queryDocument = _idGenerator.GetQueryToSkipIdGeneratorDocument(query.zToQueryDocument());
            //IEnumerable<BsonDocument> cursor = GetCollection().zFind<BsonDocument>(queryDocument, sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument());
            //if (_useCursorCache)
            //    cursor = cursor.zCacheCursor();
            //return cursor.Select(Deserialize);
            return _Find(query, sort, limit, options).Select(Deserialize);
        }

        protected IEnumerable<BsonDocument> _Find(string query, string sort = null, int limit = 0, string options = null)
        {
            if (query == null)
                query = "{}";
            if (sort == null)
                sort = _defaultSort;
            //QueryDocument queryDocument = _idGenerator.GetQueryToSkipIdGeneratorDocument(query.zToQueryDocument());
            QueryDocument queryDocument = GetQueryToSkipIdGeneratorDocument(query.zToQueryDocument());
            IEnumerable<BsonDocument> cursor = GetCollection().zFind<BsonDocument>(queryDocument, sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument());
            if (_useCursorCache)
                cursor = cursor.zCacheCursor();
            return cursor;
        }

        public int Update(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = 0;
            foreach (BsonDocument document in _Find(query, sort: sort, limit: limit))
            {
                TData data = Deserialize(document);
                updateDocument(data);
                BsonValue id = GetId(document);
                if (id == null)
                    throw new PBException("can't update document without id {0}", document);
                Save(id, data);
                nb++;
            }
            return nb;
        }

        public IEnumerable<string> Backup(string directory)
        {
            yield return MongoBackup.Backup(GetCollection(), directory);
        }

        protected virtual BsonDocument Serialize(TData data)
        {
            Type type = _nominalType;
            if (type == null)
                type = typeof(TData);
            return data.ToBsonDocument(type);
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
            if (_nominalType != null)
                return (TData)BsonSerializer.Deserialize(document, _nominalType);
            else
                return BsonSerializer.Deserialize<TData>(document);
        }

        protected virtual BsonValue GetId(BsonDocument document)
        {
            if (document.Contains("_id"))
                return document["_id"];
            else
                return null;
        }

        //protected QueryDocument GetQuery(IQuery key, bool addPrefix)
        protected QueryDocument GetQuery(IEnumerable<KeyValuePair<string, object>> queryValues, bool addPrefix)
        {
            string prefixName = null;
            if (addPrefix)
            {
                if (_addItemNameToQueryKey)
                    prefixName = _itemName;
                if (_keyName != null)
                {
                    if (prefixName == null)
                        prefixName = _keyName;
                    else
                        prefixName += "." + _keyName;
                }
            }
            //IEnumerable<KeyValuePair<string, object>> keyValues = key.GetQueryValues();
            if (prefixName != null)
                queryValues = queryValues.Select(keyValue => new KeyValuePair<string, object>(prefixName + keyValue.Key, keyValue.Value));
            return new QueryDocument(queryValues);
        }

        protected bool GetCollectionContainsIdGeneratorDocument()
        {
            if (_idGenerator != null)
                return _idGenerator.GetCollectionContainsIdGeneratorDocument();
            else
                return false;
        }

        protected QueryDocument GetQueryToSkipIdGeneratorDocument(QueryDocument queryDocument)
        {
            if (_idGenerator != null)
                queryDocument = _idGenerator.GetQueryToSkipIdGeneratorDocument(queryDocument);
            return queryDocument;
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

        public string GetCollectionFullName()
        {
            return string.Format("server \"{0}\" database \"{1}\" collection \"{2}\"", _serverName, _databaseName, _collectionName);
        }

        public static MongoCollectionManager<TData> Create(XElement xe)
        {
            MongoCollectionManager<TData> mongoCollectionManager = new MongoCollectionManager<TData>(xe.zXPathExplicitValue("MongoServer"), xe.zXPathExplicitValue("MongoDatabase"),
                xe.zXPathExplicitValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            mongoCollectionManager.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //MongoGenerateId
            if (xe.zXPathValue("MongoGenerateId").zTryParseAs(false))
            {
                string type = xe.zXPathValue("MongoGenerateId/@type").ToLowerInvariant();
                if (type == "int")
                {
                    mongoCollectionManager._idGenerator = new MongoIdGeneratorInt(mongoCollectionManager.GetCollection());
                    mongoCollectionManager._generateId = true;
                }
                else
                    throw new PBException("unknow id type generator \"{0}\"", type);
            }
            return mongoCollectionManager;
        }
    }
}

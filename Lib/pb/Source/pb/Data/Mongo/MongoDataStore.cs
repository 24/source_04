using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using pb.Data.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace pb.Data.Mongo
{
    // copy of MongoDocumentStore<TData> without serialization
    // and MongoCollectionManager<TData>
    public class MongoDataStore
    {
        private string _serverName = null;
        private string _databaseName = null;
        private string _collectionName = null;
        private MongoCollection _collection = null;

        private string _itemName = null;
        private bool _addItemNameToQueryKey = true;
        private string _keyName = null;

        private static string __idElementName = "_id";
        private string _keyElementName = "_id";

        private bool _useCursorCache = true;
        private bool _generateId = false;
        private MongoIdGenerator _idGenerator = null;
        private string _defaultSort = null;

        public MongoDataStore(string serverName, string databaseName, string collectionName, string itemName = null)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _itemName = itemName;
        }

        public bool AddItemNameToQueryKey { get { return _addItemNameToQueryKey; } set { _addItemNameToQueryKey = value; } }
        public string KeyName { get { return _keyName; } set { _keyName = value; } }
        public string KeyElementName { get { return _keyElementName; } set { _keyElementName = value; } }
        public bool UseCursorCache { get { return _useCursorCache; } set { _useCursorCache = value; } }
        public bool GenerateId { get { return _generateId; } set { _generateId = value; } }
        public MongoIdGenerator IdGenerator { get { return _idGenerator; } set { _idGenerator = value; } }
        public string DefaultSort { get { return _defaultSort; } set { _defaultSort = value; } }

        public bool ExistsFromId(BsonValue id)
        {
            return _Exists(GetQuery(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("_id", id) }, addPrefix: false));
        }

        //public bool ExistsFromKey(IQuery key, bool addPrefix = true)
        public bool ExistsFromKey(IQuery key, bool addPrefix = false)
        {
            return _Exists(GetQuery(key.GetQueryValues(), addPrefix));
        }

        //public bool ExistsFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = true)
        public bool ExistsFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = false)
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
        //public BsonValue GetIdFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = true)
        public BsonValue GetIdFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = false)
        {
            QueryDocument query = GetQuery(keyValues, addPrefix);
            BsonDocument document = GetCollection().zFind<BsonDocument>(query, fields: __IdField).FirstOrDefault();
            if (document != null)
                return document["_id"];
            else
                return null;
        }

        public BsonDocument LoadFromId(BsonValue id)
        {
            BsonDocument document = GetCollection().zFindOneById<BsonDocument>(id);
            if (document == null)
                throw new PBException("error mongo document not found id \"{0}\" {1}", id, GetCollectionFullName());
            return document;
        }

        //public BsonDocument LoadFromKey(IQuery key, bool addPrefix = true, bool throwException = false)
        public BsonDocument LoadFromKey(IQuery key, bool addPrefix = false, bool throwException = false)
        {
            return _LoadFromKey(GetQuery(key.GetQueryValues(), addPrefix), throwException);
        }

        //public BsonDocument LoadFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = true, bool throwException = false)
        public BsonDocument LoadFromKey(IEnumerable<KeyValuePair<string, object>> keyValues, bool addPrefix = false, bool throwException = false)
        {
            return _LoadFromKey(GetQuery(keyValues, addPrefix), throwException);
        }

        private BsonDocument _LoadFromKey(QueryDocument query, bool throwException = false)
        {
            BsonDocument document = GetCollection().zFindOne<BsonDocument>(query);
            if (document == null && throwException)
                throw new PBException("mongo document not found query {0} collection {1}", query, GetCollectionFullName());
            return document;
        }

        public BsonValue GetNewId()
        {
            return _idGenerator.GetNewId();
        }

        public void Save(BsonValue id, BsonDocument data)
        {
            _Save(GetQuery(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("_id", id) }, addPrefix: false), data);
        }

        //public void Save(IQuery key, BsonDocument data, bool addPrefix = true)
        public void Save(IQuery key, BsonDocument data, bool addPrefix = false)
        {
            _Save(GetQuery(key.GetQueryValues(), addPrefix), data);
        }

        //public void Save(IEnumerable<KeyValuePair<string, object>> keyValues, BsonDocument data, bool addPrefix = true)
        public void Save(IEnumerable<KeyValuePair<string, object>> keyValues, BsonDocument data, bool addPrefix = false)
        {
            _Save(GetQuery(keyValues, addPrefix), data);
        }

        private void _Save(QueryDocument query, BsonDocument data)
        {
            //BsonDocument document = Serialize(data);

            //if (_itemName != null)
            //    document = new BsonDocument(_itemName, document);

            GetCollection().zUpdate(query, new UpdateDocument { { "$set", data } }, UpdateFlags.Upsert);
        }

        public WriteConcernResult Remove(BsonValue id)
        {
            return GetCollection().zRemove(new QueryDocument { { "_id", id } });
        }

        //public WriteConcernResult RemoveFromKey(IQuery key, bool addPrefix = true)
        public WriteConcernResult RemoveFromKey(IQuery key, bool addPrefix = false)
        {
            return GetCollection().zRemove(GetQuery(key.GetQueryValues(), addPrefix));
        }

        public long Count(string query = null)
        {
            return _Count(query.zToQueryDocument());
        }

        private long _Count(QueryDocument query)
        {
            return GetCollection().zCount(query) - (query == null && GetCollectionContainsIdGeneratorDocument() ? 1 : 0);
        }

        public IEnumerable<BsonDocument> Find(string query, string sort = null, int limit = 0, string options = null)
        {
            return _Find(query, sort, limit, options);
        }

        protected IEnumerable<BsonDocument> _Find(string query, string sort = null, int limit = 0, string options = null)
        {
            if (query == null)
                query = "{}";
            if (sort == null)
                sort = _defaultSort;
            QueryDocument queryDocument = GetQueryToSkipIdGeneratorDocument(query.zToQueryDocument());
            IEnumerable<BsonDocument> cursor = GetCollection().zFind<BsonDocument>(queryDocument, sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument());
            if (_useCursorCache)
                cursor = cursor.zCacheCursor();
            return cursor;
        }

        public int Update(Action<BsonDocument> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = 0;
            foreach (BsonDocument data in _Find(query, sort: sort, limit: limit))
            {
                //TData data = Deserialize(document);
                updateDocument(data);
                BsonValue id = GetId(data);
                if (id == null)
                    throw new PBException("can't update document without id {0}", data);
                Save(id, data);
                nb++;
            }
            return nb;
        }

        public IEnumerable<string> Backup(string directory)
        {
            yield return MongoBackup.Backup(GetCollection(), directory);
        }

        //protected virtual BsonDocument Serialize(TData data)
        //{
        //    Type type = _nominalType;
        //    if (type == null)
        //        type = typeof(TData);
        //    return data.ToBsonDocument(type);
        //}

        //protected virtual TData Deserialize(BsonDocument document)
        //{
        //    if (_itemName != null)
        //    {
        //        if (!document.Contains(_itemName))
        //            throw new PBException("error load data : document does'nt contain element \"{0}\" {1}", _itemName, GetCollectionFullName());
        //        var element = document[_itemName];
        //        if (element == null)
        //            throw new PBException("error load data : element \"{0}\" is null {1}", _itemName, GetCollectionFullName());
        //        if (!(element is BsonDocument))
        //            throw new PBException("error load data : element \"{0}\" is not a document {1}", _itemName, GetCollectionFullName());
        //        document = element as BsonDocument;
        //    }
        //    if (_nominalType != null)
        //        return (TData)BsonSerializer.Deserialize(document, _nominalType);
        //    else
        //        return BsonSerializer.Deserialize<TData>(document);
        //}

        protected virtual BsonValue GetId(BsonDocument document)
        {
            if (document.Contains("_id"))
                return document["_id"];
            else
                return null;
        }

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
            if (prefixName != null)
                queryValues = queryValues.Select(keyValue => new KeyValuePair<string, object>(prefixName + "." + keyValue.Key, keyValue.Value));
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

        // from MongoDocumentStore<TData>

        public virtual bool Exists(BsonValue key)
        {
            return ExistsFromKey(GetKeyValue(key), addPrefix: false);
        }

        public virtual BsonValue GetId(BsonValue key)
        {
            return GetIdFromKey(GetKeyValue(key), addPrefix: false);
        }

        public BsonDocument LoadFromKey(BsonValue key)
        {
            return LoadFromKey(GetKeyValue(key), addPrefix: false, throwException: true);
        }

        public void SaveWithId(BsonValue id, BsonDocument document)
        {
            Save(GetIdValue(id), document, addPrefix: false);
        }

        public void SaveWithKey(BsonValue key, BsonDocument document)
        {
            Save(GetKeyValue(key), document, addPrefix: false);
        }

        private IEnumerable<KeyValuePair<string, object>> GetIdValue(BsonValue id)
        {
            yield return new KeyValuePair<string, object>(__idElementName, id);
        }

        private IEnumerable<KeyValuePair<string, object>> GetKeyValue(BsonValue key)
        {
            yield return new KeyValuePair<string, object>(_keyElementName, key);
        }

        public static MongoDataStore Create(XElement xe)
        {
            if (!xe.zXPathValue("UseMongo").zTryParseAs(false))
                return null;
            MongoDataStore mongoDataStore = new MongoDataStore(xe.zXPathExplicitValue("MongoServer"), xe.zXPathExplicitValue("MongoDatabase"), xe.zXPathExplicitValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            mongoDataStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            if (xe.zXPathValue("MongoGenerateId").zTryParseAs(false))
            {
                string type = xe.zXPathValue("MongoGenerateId/@type").ToLowerInvariant();
                if (type == "int")
                {
                    mongoDataStore._idGenerator = new MongoIdGeneratorInt(mongoDataStore.GetCollection());
                    mongoDataStore._generateId = true;
                }
                else
                    throw new PBException("unknow id type generator \"{0}\"", type);
            }
            return mongoDataStore;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;  // for zGetKey<TKey, TData>(this TData data)
using MongoDB.Driver;
using pb.Data.Xml;

namespace pb.Data.Mongo
{
    // BsonValue.Create(object value) use key as an object
    public class MongoDocumentStore<TKey, TData> : IDocumentStore<TKey, TData>
    {
        protected string _server = null;
        protected string _database = null;
        protected string _collectionName = null;
        protected string _itemName = null;
        protected MongoCollection _collection = null;
        protected bool _useCursorCache = true;
        //protected Func<TData, TKey> _getDataKey = null;
        protected Func<BsonDocument, TData> _deserialize = null;
        protected string _defaultSort = null;

        public MongoDocumentStore(string server, string database, string collectionName, string itemName = null)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _itemName = itemName;
        }

        //public Func<TData, TKey> GetDataKey { get { return _getDataKey; } set { _getDataKey = value; } }
        public bool UseCursorCache { get { return _useCursorCache; } set { _useCursorCache = value; } }
        public Func<BsonDocument, TData> Deserialize { get { return _deserialize; } set { _deserialize = value; } }
        public string DefaultSort { get { return _defaultSort; } set { _defaultSort = value; } }

        public virtual bool DocumentExists(TKey key)
        {
            return GetCollection().zCount(GetQueryDocument(key)) == 1;
        }

        public virtual TData LoadDocument(TKey key)
        {
            BsonDocument document = GetCollection().zFindOneById<BsonDocument>(GetDocumentBsonKey(key));
            if (document == null)
                throw new PBException("error mongo document not found key \"{0}\" {1}", key, GetCollectionFullName());

            return _Deserialize(document);
        }

        public virtual void SaveDocument(TKey key, TData document)
        {
            BsonDocument bsonDocument = _Serialize(document);

            if (_itemName != null)
                bsonDocument = new BsonDocument(_itemName, bsonDocument);

            GetCollection().zUpdate(GetQueryDocument(key), new UpdateDocument { { "$set", bsonDocument } }, UpdateFlags.Upsert);
        }

        // bool useCursorCache = false
        public virtual IEnumerable<TData> FindDocuments(string query, string sort = null, int limit = 0, string options = null)
        {
            if (query == null)
                query = "{}";
            if (sort == null)
                sort = _defaultSort;
            IEnumerable<BsonDocument> cursor = GetCollection().zFind<BsonDocument>(query.zToQueryDocument(), sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument());
            if (_useCursorCache)
                cursor = cursor.zCacheCursor();
            return cursor.Select(_Deserialize);
        }

        public int UpdateDocuments(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        {
            int nb = 0;
            foreach (TData data in FindDocuments(query, sort: sort, limit: limit))
            {
                updateDocument(data);
                //SaveDocument(_getDataKey(data), data);
                SaveDocument(data.zGetKey<TKey, TData>(), data);
                nb++;
            }
            return nb;
        }

        //protected virtual TKey GetDataKey(TData data)
        //{
        //    //return _getDataKey(data);
        //    if (data is IKeyData<TKey>)
        //        return ((IKeyData<TKey>)data).GetKey();
        //    else
        //        throw new PBException("type {0} is not IKeyData<{1}>", data.GetType().zGetName(), typeof(TKey).zGetName());
        //}

        protected virtual BsonDocument _Serialize(TData value)
        {
            return value.ToBsonDocument(value.GetType());
        }

        protected virtual TData _Deserialize(BsonDocument document)
        {
            if (_itemName != null)
            {
                if (!document.Contains(_itemName))
                    throw new PBException("error LoadDocument : document does'nt contain element \"{0}\" {1}", _itemName, GetCollectionFullName());
                var element = document[_itemName];
                if (element == null)
                    throw new PBException("error LoadDocument : element \"{0}\" is null {1}", _itemName, GetCollectionFullName());
                if (!(element is BsonDocument))
                    throw new PBException("error LoadDocument : element \"{0}\" is not a document {1}", _itemName, GetCollectionFullName());
                document = element as BsonDocument;
            }
            // dont use object BsonSerializer.Deserialize(BsonDocument document, Type nominalType) because it return an object
            if (_deserialize != null)
                return _deserialize(document);
            else
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

        protected QueryDocument GetQueryDocument(TKey key)
        {
            return new QueryDocument { { "_id", GetDocumentBsonKey(key) } };
        }

        protected BsonValue GetDocumentBsonKey(TKey key)
        {
            // BsonValue.Create(object value) use key as an object
            return BsonValue.Create(key);
        }

        protected string GetCollectionFullName()
        {
            return string.Format("server \"{0}\" database \"{1}\" collection \"{2}\"", _server, _database, _collectionName);
        }

        public static MongoDocumentStore<TKey, TData> Create(XElement xe)
        {
            if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            {
                MongoDocumentStore<TKey, TData> documentStore = new MongoDocumentStore<TKey, TData>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
                documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
                return documentStore;
            }
            else
                return null;
        }
    }
}

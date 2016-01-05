using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using pb.Data.old;

namespace pb.Data.Mongo.old
{
    /// <summary>
    /// used to store header page with multiple item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// not used
    public class MongoDocumentStoreMultiple_v2<T> : MongoDocumentStore_v2<T>
    {
        public MongoDocumentStoreMultiple_v2(string server, string database, string collectionName, string itemName = null)
            : base(server, database, collectionName, itemName)
        {
        }

        public override bool DocumentExists(object key)
        {
            // une header page ne contient pas toujours les mêmes item.
            // en général ils sont triés du plus récent au plus ancient
            return false;
        }
    }

    public class MongoDocumentStore_v2<T> : IDocumentStore_v2<T>
    {
        protected string _server = null;
        protected string _database = null;
        protected string _collectionName = null;
        protected string _itemName = null;
        protected MongoCollection _collection = null;

        static MongoDocumentStore_v2()
        {
            //if (!BsonClassMap.IsClassMapRegistered(typeof(ImageHtml)))
            //{
            //    BsonClassMap.RegisterClassMap<ImageHtml>(cm => { cm.AutoMap(); cm.UnmapProperty(c => c.Image); });
            //}
        }

        public MongoDocumentStore_v2(string server, string database, string collectionName, string itemName = null)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _itemName = itemName;
        }

        // ILoadDocument<T> loadDocument
        public virtual bool DocumentExists(object key)
        {
            return GetCollection().zCount(GetQueryDocument(key)) == 1;
        }

        public virtual T LoadDocument(object key)
        {
            //Trace.WriteLine("MongoDocumentStore.LoadDocument key \"{0}\"", documentRequest.Key);
            //if (!loadDocument.DocumentLoaded)
            //{
            BsonDocument document = GetCollection().zFindOneById<BsonDocument>(GetDocumentBsonKey(key));
            if (document == null)
                throw new PBException("error mongo document not found key \"{0}\" {1}", key, GetCollectionFullName());

            //if (_itemName != null)
            //{
            //    if (!document.Contains(_itemName))
            //        throw new PBException("error LoadDocument : document does'nt contain element \"{0}\", key \"{1}\" {2}", _itemName, key, GetCollectionFullName());
            //    var element = document[_itemName];
            //    if (element == null)
            //        throw new PBException("error LoadDocument : element \"{0}\" is null, key \"{1}\" {2}", _itemName, key, GetCollectionFullName());
            //    if (!(element is BsonDocument))
            //        throw new PBException("error LoadDocument : element \"{0}\" is not a document, key \"{1}\" {2}", _itemName, key, GetCollectionFullName());
            //    document = element as BsonDocument;
            //}
            //loadDocument.Document = Deserialize(document);
            //loadDocument.DocumentLoaded = true;
            return Deserialize(document);
            //}
        }

        public virtual void SaveDocument(object key, T document)
        {
            BsonDocument bsonDocument = Serialize(document);

            if (_itemName != null)
                bsonDocument = new BsonDocument(_itemName, bsonDocument);

            GetCollection().zUpdate(GetQueryDocument(key), new UpdateDocument { { "$set", bsonDocument } }, UpdateFlags.Upsert);
        }

        public virtual IEnumerable<T> FindDocuments(string query, string sort = null, int limit = 0, string options = null)
        {
            return from document in GetCollection().zFind<BsonDocument>(query.zToQueryDocument(), sort.zToSortByWrapper(), limit: limit, options: options.zDeserializeToBsonDocument())
                   select Deserialize(document);
        }

        protected virtual BsonDocument Serialize(T value)
        {
            //return value.ToBsonDocument();
            return value.ToBsonDocument(value.GetType());
        }

        protected virtual T Deserialize(BsonDocument document)
        {
            if (_itemName != null)
            {
                if (!document.Contains(_itemName))
                    //throw new PBException("error LoadDocument : document does'nt contain element \"{0}\", key \"{1}\" {2}", _itemName, key, GetCollectionFullName());
                    throw new PBException("error LoadDocument : document does'nt contain element \"{0}\" {1}", _itemName, GetCollectionFullName());
                var element = document[_itemName];
                if (element == null)
                    //throw new PBException("error LoadDocument : element \"{0}\" is null, key \"{1}\" {2}", _itemName, key, GetCollectionFullName());
                    throw new PBException("error LoadDocument : element \"{0}\" is null {1}", _itemName, GetCollectionFullName());
                if (!(element is BsonDocument))
                    //throw new PBException("error LoadDocument : element \"{0}\" is not a document, key \"{1}\" {2}", _itemName, key, GetCollectionFullName());
                    throw new PBException("error LoadDocument : element \"{0}\" is not a document {1}", _itemName, GetCollectionFullName());
                document = element as BsonDocument;
            }
            return BsonSerializer.Deserialize<T>(document);
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

        // ILoadDocument<T> loadDocument
        protected QueryDocument GetQueryDocument(object key)
        {
            return new QueryDocument { { "_id", GetDocumentBsonKey(key) } };
        }

        // ILoadDocument<T> loadDocument
        protected BsonValue GetDocumentBsonKey(object key)
        {
            //object key = loadDocument.Key;
            if (key == null)
                throw new PBException("error key is null {0}", GetCollectionFullName());
            return BsonValue.Create(key);
        }

        protected string GetCollectionFullName()
        {
            return string.Format("server \"{0}\" database \"{1}\" collection \"{2}\"", _server, _database, _collectionName);
        }
    }
}

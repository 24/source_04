using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace pb.Data.Mongo
{
    public class MongoDocumentStore_v1<T> : IDocumentStore_v1<T>
    {
        protected string _server = null;
        protected string _database = null;
        protected string _collectionName = null;
        protected string _itemName = null;
        protected MongoCollection _collection = null;

        static MongoDocumentStore_v1()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(pb.old.ImageHtml)))
            {
                BsonClassMap.RegisterClassMap<pb.old.ImageHtml>(cm => { cm.AutoMap(); cm.UnmapProperty(c => c.Image); });
            }
        }

        public MongoDocumentStore_v1(string server, string database, string collectionName, string itemName = null)
        {
            _server = server;
            _database = database;
            _collectionName = collectionName;
            _itemName = itemName;
        }

        public virtual bool DocumentExists(IDocumentRequest_v1<T> documentRequest)
        {
            return GetCollection().zCount(GetQueryDocument(documentRequest)) == 1;
        }

        public virtual void LoadDocument(IDocumentRequest_v1<T> documentRequest)
        {
            //Trace.WriteLine("MongoDocumentStore.LoadDocument key \"{0}\"", documentRequest.Key);
            if (!documentRequest.DocumentLoaded)
            {
                BsonDocument document = GetCollection().zFindOneById<BsonDocument>(GetDocumentBsonKey(documentRequest));
                if (document == null)
                    throw new PBException("error mongo document not found key \"{0}\" {1}", documentRequest.Key, GetCollectionFullName());

                if (_itemName != null)
                {
                    if (!document.Contains(_itemName))
                        throw new PBException("error LoadDocument : document does'nt contain element \"{0}\", key \"{1}\" {2}", _itemName, documentRequest.Key, GetCollectionFullName());
                    var element = document[_itemName];
                    if (element == null)
                        throw new PBException("error LoadDocument : element \"{0}\" is null, key \"{1}\" {2}", _itemName, documentRequest.Key, GetCollectionFullName());
                    if (!(element is BsonDocument))
                        throw new PBException("error LoadDocument : element \"{0}\" is not a document, key \"{1}\" {2}", _itemName, documentRequest.Key, GetCollectionFullName());
                    document = element as BsonDocument;
                }
                documentRequest.Document = Deserialize(document);
                documentRequest.DocumentLoaded = true;
            }
        }

        protected virtual T Deserialize(BsonDocument document)
        {
            return BsonSerializer.Deserialize<T>(document);
        }

        public virtual void SaveDocument(IDocumentRequest_v1<T> documentRequest)
        {
            BsonDocument document = Serialize(documentRequest.Document);

            if (_itemName != null)
                document = new BsonDocument(_itemName, document);

            GetCollection().zUpdate(GetQueryDocument(documentRequest), new UpdateDocument { { "$set", document } }, UpdateFlags.Upsert);
        }

        public virtual BsonDocument Serialize(T value)
        {
            //return value.ToBsonDocument();
            return value.ToBsonDocument(value.GetType());
        }

        protected MongoCollection GetCollection()
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

        protected QueryDocument GetQueryDocument(IDocumentRequest_v1<T> documentRequest)
        {
            return new QueryDocument { { "_id", GetDocumentBsonKey(documentRequest) } };
        }

        protected BsonValue GetDocumentBsonKey(IDocumentRequest_v1<T> documentRequest)
        {
            object key = documentRequest.Key;
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

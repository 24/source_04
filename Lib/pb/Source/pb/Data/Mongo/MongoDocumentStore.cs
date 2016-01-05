using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data.Xml;

namespace pb.Data.Mongo
{
    public class MongoDocumentStore<TData> : IDocumentStore<TData>
    {
        private static string __idElementName = "_id";
        private MongoCollectionManager<TData> _collectionManager = null;
        private string _keyElementName = "_id";
        //protected string _defaultSort = null;

        public MongoDocumentStore(MongoCollectionManager<TData> collectionManager)
        {
            _collectionManager = collectionManager;
        }

        public string KeyElementName { get { return _keyElementName; } set { _keyElementName = value; } }
        //public string DefaultSort { get { return _collectionManager.DefaultSort; } set { _collectionManager.DefaultSort = value; } }
        public MongoCollectionManager<TData> CollectionManager { get { return _collectionManager; } }
        public Type NominalType { get { return _collectionManager.NominalType; } set { _collectionManager.NominalType = value; } }
        public bool GenerateId { get { return _collectionManager.GenerateId; } }

        public virtual bool Exists(BsonValue key)
        {
            return _collectionManager.ExistsFromKey(GetKeyValue(key), addPrefix: false);
        }

        public virtual BsonValue GetId(BsonValue key)
        {
            return _collectionManager.GetIdFromKey(GetKeyValue(key), addPrefix: false);
        }

        public virtual BsonValue GetNewId()
        {
            return _collectionManager.GetNewId();
        }

        public TData LoadFromId(BsonValue id)
        {
            return _collectionManager.LoadFromId(id);
        }

        public TData LoadFromKey(BsonValue key)
        {
            return _collectionManager.LoadFromKey(GetKeyValue(key), addPrefix: false, throwException: true);
        }

        public void SaveWithId(BsonValue id, TData document)
        {
            _collectionManager.Save(GetIdValue(id), document, addPrefix: false);
        }

        public void SaveWithKey(BsonValue key, TData document)
        {
            _collectionManager.Save(GetKeyValue(key), document, addPrefix: false);
        }

        public IEnumerable<TData> Find(string query, string sort = null, int limit = 0, string options = null)
        {
            return _collectionManager.Find(query, sort, limit, options);
        }

        public int Update(Action<TData> update, string query = null, string sort = null, int limit = 0)
        {
            return _collectionManager.Update(update, query, sort, limit);
        }

        private IEnumerable<KeyValuePair<string, object>> GetIdValue(BsonValue id)
        {
            yield return new KeyValuePair<string, object>(__idElementName, id);
        }

        private IEnumerable<KeyValuePair<string, object>> GetKeyValue(BsonValue key)
        {
            yield return new KeyValuePair<string, object>(_keyElementName, key);
        }

        public static MongoDocumentStore<TData> Create(XElement xe)
        {
            if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            {
                //MongoDocumentStore<TKey, TData> documentStore = new MongoDocumentStore<TKey, TData>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
                //documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
                //return documentStore;
                MongoDocumentStore<TData> mongoDocumentStore = new MongoDocumentStore<TData>(MongoCollectionManager<TData>.Create(xe));
                mongoDocumentStore.KeyElementName = xe.zXPathValue("MongoKeyElementName", mongoDocumentStore.KeyElementName);
                return mongoDocumentStore;
            }
            else
                return null;
        }
    }
}

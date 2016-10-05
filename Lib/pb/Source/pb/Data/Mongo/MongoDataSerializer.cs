using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;

namespace pb.Data.Mongo
{
    public class MongoDataSerializer<TData>
    {
        private Type _nominalType = null;                               // type to use for serialize and deserialize
        private string _itemName = null;

        public Type NominalType { get { return _nominalType; } set { _nominalType = value; } }
        public string ItemName { get { return _itemName; } set { _itemName = value; } }

        public virtual BsonElement Serialize(TData data)
        {
            if (_itemName == null)
                throw new PBException("item name is not defined");

            Type type = _nominalType;
            if (type == null)
                type = typeof(TData);
            return new BsonElement(_itemName, data.ToBsonDocument(type));
        }

        public virtual TData Deserialize(BsonDocument document)
        {
            if (_itemName == null)
                throw new PBException("item name is not defined");

            if (!document.Contains(_itemName))
                throw new PBException("deserialize data : document does'nt contain element \"{0}\"", _itemName);
            var element = document[_itemName];
            if (element == null)
                throw new PBException("deserialize data : element \"{0}\" is null", _itemName);
            if (!(element is BsonDocument))
                throw new PBException("deserialize data : element \"{0}\" is not a document", _itemName);
            document = element as BsonDocument;

            if (_nominalType != null)
                return (TData)BsonSerializer.Deserialize(document, _nominalType);
            else
                return BsonSerializer.Deserialize<TData>(document);
        }
    }
}

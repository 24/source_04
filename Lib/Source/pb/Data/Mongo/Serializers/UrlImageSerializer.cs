using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Web;

namespace pb.Data.Mongo
{
    class UrlImageSerializer : BsonBaseSerializer
    {
        public UrlImageSerializer()
        {
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            VerifyTypes(nominalType, actualType, typeof(WebImage));

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.String:
                    return new WebImage(bsonReader.ReadString());
                default:
                    throw new PBException("error cannot deserialize UrlImage from BsonType {0}.", bsonType);
            }
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
            {
                throw new PBException("error serialize UrlImage value is null");
            }

            bsonWriter.WriteString(((WebImage)value).Url);
        }
    }
}

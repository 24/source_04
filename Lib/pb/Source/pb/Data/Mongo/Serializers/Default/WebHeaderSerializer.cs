using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Net;

namespace pb.Data.Mongo.Serializers
{
    public class WebHeaderSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        //private static WebHeaderSerializer __instance = new WebHeaderSerializer();

        //public WebHeaderSerializer()
        //{
        //}

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        //public static WebHeaderSerializer Instance { get { return __instance; } }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("WebHeaderSerializer.Deserialize()");

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else if (bsonType == BsonType.Document)
            {
                if (nominalType == typeof(object))
                {
                    bsonReader.ReadStartDocument();
                    bsonReader.ReadString("_t"); // skip over discriminator
                    bsonReader.ReadName("_v");
                    var value = Deserialize(bsonReader, actualType, options); // recursive call replacing nominalType with actualType
                    bsonReader.ReadEndDocument();
                    return value;
                }

                var headers = new WebHeaderCollection();

                bsonReader.ReadStartDocument();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    string key = bsonReader.ReadName();

                    bsonType = bsonReader.GetCurrentBsonType();
                    if (bsonType == BsonType.String)
                    {
                        string value = bsonReader.ReadString();
                        headers.Add(key, value);
                    }
                    else if (bsonType == BsonType.Array)
                    {
                        bsonReader.ReadStartArray();
                        bsonType = bsonReader.ReadBsonType();
                        while (bsonType != BsonType.EndOfDocument)
                        {
                            if (bsonType != BsonType.String)
                                throw new PBException("invalid BsonType {0} for header array \"{1}\" value deserialize WebHeaderCollection.", bsonType, key);
                            string value = bsonReader.ReadString();
                            headers.Add(key, value);
                            bsonType = bsonReader.ReadBsonType();
                        }
                        bsonReader.ReadEndArray();
                    }
                    else
                        throw new PBException("error deserialize WebHeaderCollection, invalid BsonType {0} for \"{1}\".", bsonType, key);
                }
                bsonReader.ReadEndDocument();

                return headers;
            }
            else
            {
                throw new PBException("Can't deserialize a {0} from BsonType {1}.", nominalType.FullName, bsonType);
            }
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("WebHeaderSerializer.Serialize()");

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                if (nominalType == typeof(object))
                {
                    var actualType = value.GetType();
                    bsonWriter.WriteStartDocument();
                    bsonWriter.WriteString("_t", TypeNameDiscriminator.GetDiscriminator(actualType));
                    bsonWriter.WriteName("_v");
                    Serialize(bsonWriter, actualType, value, options); // recursive call replacing nominalType with actualType
                    bsonWriter.WriteEndDocument();
                    return;
                }

                var headers = (WebHeaderCollection)value;
                bsonWriter.WriteStartDocument();
                for (int i = 0; i < headers.Count; i++)
                {
                    bsonWriter.WriteName(headers.GetKey(i));
                    string[] headerValues = headers.GetValues(i);
                    if (headerValues.Length == 1)
                        bsonWriter.WriteString(headerValues[0]);
                    else
                    {
                        bsonWriter.WriteStartArray();
                        foreach (string headerValue in headerValues)
                        {
                            bsonWriter.WriteString(headerValue);
                        }
                        bsonWriter.WriteEndArray();
                    }
                }
                bsonWriter.WriteEndDocument();
            }
        }
    }
}

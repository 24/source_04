using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo.Serializers
{
    public class ZStringArraySerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static ZStringArraySerializer _instance = new ZStringArraySerializer();

        public ZStringArraySerializer()
        {
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        // used by ZValueSerializer.Deserialize()
        public static ZStringArraySerializer Instance { get { return _instance; } }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("ZStringArraySerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(ZStringArray));

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Array:
                    bsonReader.ReadStartArray();
                    //return new ZString(bsonReader.ReadString());
                    var array = new List<string>();
                    bsonType = bsonReader.ReadBsonType();
                    while (bsonType != BsonType.EndOfDocument)
                    {
                        if (bsonType != BsonType.String)
                            throw new PBException("error ZStringArray cannot contain value of type {0}", bsonType);
                        var value = bsonReader.ReadString();
                        array.Add(value);
                        bsonType = bsonReader.ReadBsonType();
                    }
                    bsonReader.ReadEndArray();
                    return new ZStringArray(array.ToArray());
                default:
                    throw new PBException("error cannot deserialize ZStringArray from BsonType {0}.", bsonType);
            }
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("ZStringArraySerializer.Serialize()");

            if (value == null)
            {
                throw new PBException("error serialize ZStringArray value is null");
            }

            string[] stringValues = ((ZStringArray)value).Values;

            if (stringValues == null)
                bsonWriter.WriteNull();
            else
            {
                bsonWriter.WriteStartArray();
                foreach (string stringValue in stringValues)
                {
                    bsonWriter.WriteString(stringValue);
                }
                bsonWriter.WriteEndArray();
            }
        }
    }
}

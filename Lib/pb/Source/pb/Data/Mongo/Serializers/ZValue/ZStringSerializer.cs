using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo.Serializers
{
    /// <summary>
    /// Represents a serializer for BsonStrings.
    /// </summary>
    // from BsonStringSerializer
    public class ZStringSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static ZStringSerializer _instance = new ZStringSerializer();

        /// <summary>
        /// Initializes a new instance of the ZStringSerializer class.
        /// </summary>
        public ZStringSerializer()
        {
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        // used by ZValueSerializer.Deserialize()
        public static ZStringSerializer Instance { get { return _instance; } }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("ZStringSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(ZString));

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.String:
                    return new ZString(bsonReader.ReadString());
                default:
                    //var message = string.Format("Cannot deserialize BsonString from BsonType {0}.", bsonType);
                    //throw new FileFormatException(message);
                    throw new PBException("error cannot deserialize ZString from BsonType {0}.", bsonType);
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("ZStringSerializer.Serialize()");

            if (value == null)
            {
                //throw new ArgumentNullException("value");
                throw new PBException("error serialize ZString value is null");
            }

            string stringValue = ((ZString)value).Value;

            if (stringValue == null)
                bsonWriter.WriteNull();
            else
                bsonWriter.WriteString(stringValue);
        }
    }
}

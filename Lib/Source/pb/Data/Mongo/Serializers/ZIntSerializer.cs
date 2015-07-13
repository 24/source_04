﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo
{
    /// <summary>
    /// Represents a serializer for ZInt.
    /// </summary>
    public class ZIntSerializer : BsonBaseSerializer
    {
        // private static fields
        private static ZIntSerializer __instance = new ZIntSerializer();

        // constructors
        /// <summary>
        /// Initializes a new instance of the ZIntSerializer class.
        /// </summary>
        public ZIntSerializer()
        {
        }

        // public static properties
        /// <summary>
        /// Gets an instance of the ZIntSerializer class.
        /// </summary>
        public static ZIntSerializer Instance
        {
            get { return __instance; }
        }

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
            //VerifyTypes(nominalType, actualType, typeof(BsonInt32));
            VerifyTypes(nominalType, actualType, typeof(ZInt));

            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Int32:
                    return new ZInt(bsonReader.ReadInt32());
                default:
                    //var message = string.Format("Cannot deserialize BsonInt32 from BsonType {0}.", bsonType);
                    //throw new FileFormatException(message);
                    throw new PBException("error cannot deserialize ZInt from BsonType {0}.", bsonType);
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
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var zint = (ZInt)value;
            bsonWriter.WriteInt32(zint.value);
        }
    }
}

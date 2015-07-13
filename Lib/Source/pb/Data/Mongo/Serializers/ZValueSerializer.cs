using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo
{
    /// <summary>
    /// Represents a serializer for BsonValues.
    /// </summary>
    public class ZValueSerializer : BsonBaseSerializer, IBsonDocumentSerializer, IBsonArraySerializer
    {
        // from BsonValueSerializer

        // private static fields
        private static ZValueSerializer __instance = new ZValueSerializer();

        // constructors
        /// <summary>
        /// Initializes a new instance of the ZValueSerializer class.
        /// </summary>
        public ZValueSerializer()
        {
        }

        // public static properties
        /// <summary>
        /// Gets an instance of the ZValueSerializer class.
        /// </summary>
        public static ZValueSerializer Instance
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
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, // ignored
            IBsonSerializationOptions options)
        {
            var bsonType = bsonReader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Array: return (ZValue)ZStringArraySerializer.Instance.Deserialize(bsonReader, typeof(ZStringArray), options);
                //case BsonType.Binary: return (BsonValue)BsonBinaryDataSerializer.Instance.Deserialize(bsonReader, typeof(BsonBinaryData), options);
                //case BsonType.Boolean: return (BsonValue)BsonBooleanSerializer.Instance.Deserialize(bsonReader, typeof(BsonBoolean), options);
                //case BsonType.DateTime: return (BsonValue)BsonDateTimeSerializer.Instance.Deserialize(bsonReader, typeof(BsonDateTime), options);
                //case BsonType.Document: return (BsonValue)BsonDocumentSerializer.Instance.Deserialize(bsonReader, typeof(BsonDocument), options);
                //case BsonType.Double: return (BsonValue)BsonDoubleSerializer.Instance.Deserialize(bsonReader, typeof(BsonDouble), options);
                case BsonType.Int32: return (ZValue)ZIntSerializer.Instance.Deserialize(bsonReader, typeof(ZInt), options);
                //case BsonType.Int64: return (BsonValue)BsonInt64Serializer.Instance.Deserialize(bsonReader, typeof(BsonInt64), options);
                //case BsonType.JavaScript: return (BsonValue)BsonJavaScriptSerializer.Instance.Deserialize(bsonReader, typeof(BsonJavaScript), options);
                //case BsonType.JavaScriptWithScope: return (BsonValue)BsonJavaScriptWithScopeSerializer.Instance.Deserialize(bsonReader, typeof(BsonJavaScriptWithScope), options);
                //case BsonType.MaxKey: return (BsonValue)BsonMaxKeySerializer.Instance.Deserialize(bsonReader, typeof(BsonMaxKey), options);
                //case BsonType.MinKey: return (BsonValue)BsonMinKeySerializer.Instance.Deserialize(bsonReader, typeof(BsonMinKey), options);
                //case BsonType.Null: return (BsonValue)BsonNullSerializer.Instance.Deserialize(bsonReader, typeof(BsonNull), options);
                case BsonType.Document:
                    return ReadZValueFromBsonDocument(bsonReader);
                case BsonType.Null:
                    bsonReader.ReadNull();
                    return null;
                //case BsonType.ObjectId: return (BsonValue)BsonObjectIdSerializer.Instance.Deserialize(bsonReader, typeof(BsonObjectId), options);
                //case BsonType.RegularExpression: return (BsonValue)BsonRegularExpressionSerializer.Instance.Deserialize(bsonReader, typeof(BsonRegularExpression), options);
                //case BsonType.String: return (BsonValue)BsonStringSerializer.Instance.Deserialize(bsonReader, typeof(BsonString), options);
                case BsonType.String: return (ZValue)ZStringSerializer.Instance.Deserialize(bsonReader, typeof(ZString), options);
                // ZStringSerializer
                //case BsonType.Symbol: return (BsonValue)BsonSymbolSerializer.Instance.Deserialize(bsonReader, typeof(BsonSymbol), options);
                //case BsonType.Timestamp: return (BsonValue)BsonTimestampSerializer.Instance.Deserialize(bsonReader, typeof(BsonTimestamp), options);
                //case BsonType.Undefined: return (BsonValue)BsonUndefinedSerializer.Instance.Deserialize(bsonReader, typeof(BsonUndefined), options);
                default:
                    //var message = string.Format("Invalid BsonType {0}.", bsonType);
                    //throw new BsonInternalException(message);
                    throw new PBException("error deserialize ZValue, invalid BsonType {0}.", bsonType);
            }
        }

        private ZValue ReadZValueFromBsonDocument(BsonReader bsonReader)
        {
            // { "_t" : "ZString", "value" : "" }
            bsonReader.ReadStartDocument();
            BsonType type = bsonReader.ReadBsonType();
            if (type != BsonType.String)
                throw new PBException("error reading ZValue can't find ZValue type \"_t\"");
            string name = bsonReader.ReadName();
            if (name != "_t")
                throw new PBException("error reading ZValue can't find ZValue type \"_t\"");
            string typeName = bsonReader.ReadString();
            type = bsonReader.ReadBsonType();
            name = bsonReader.ReadName();
            if (name != "value")
                throw new PBException("error reading ZValue can't find ZValue value \"value\"");
            ZValue value = null;
            switch (typeName)
            {
                case "ZString":
                    if (type != BsonType.String)
                        throw new PBException("error reading ZString value is'nt a string ({0})", type);
                    value = new ZString(bsonReader.ReadString());
                    break;
                //case "ZStringArray":
                //    if (type != BsonType.Array)
                //        throw new PBException("error reading ZStringArray value is'nt an array ({0})", type);
                //    value = new ZString(bsonReader.ReadString());
                //    break;
                case "ZInt":
                    if (type != BsonType.Int32)
                        throw new PBException("error reading ZInt value is'nt an int32 ({0})", type);
                    value = new ZInt(bsonReader.ReadInt32());
                    break;
                default:
                    throw new PBException("error reading ZValue type \"{0}\" is'nt a ZValue type", typeName);
            }
            type = bsonReader.ReadBsonType();
            if (type != BsonType.EndOfDocument)
                throw new PBException("error reading ZValue cant find end of document ({0})", type);
            bsonReader.ReadEndDocument();
            return value;
        }

        /// <summary>
        /// Gets the serialization info for a member.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>
        /// The serialization info for the member.
        /// </returns>
        public BsonSerializationInfo GetMemberSerializationInfo(string memberName)
        {
            return new BsonSerializationInfo(
                memberName,
                ZValueSerializer.Instance,
                typeof(BsonValue),
                ZValueSerializer.Instance.GetDefaultSerializationOptions());
        }

        /// <summary>
        /// Gets the serialization info for individual items of the array.
        /// </summary>
        /// <returns>
        /// The serialization info for the items.
        /// </returns>
        public BsonSerializationInfo GetItemSerializationInfo()
        {
            return new BsonSerializationInfo(
                null,
                ZValueSerializer.Instance,
                typeof(BsonValue),
                ZValueSerializer.Instance.GetDefaultSerializationOptions());
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(
            BsonWriter bsonWriter,
            Type nominalType,
            object value,
            IBsonSerializationOptions options)
        {
            if (value == null)
            {
                //throw new ArgumentNullException("value");
                bsonWriter.WriteNull();
            }
            else
            {
                var bsonValue = (BsonValue)value;
                switch (bsonValue.BsonType)
                {
                    case BsonType.Array: ZStringArraySerializer.Instance.Serialize(bsonWriter, typeof(BsonArray), bsonValue, options); break;
                    //case BsonType.Binary: BsonBinaryDataSerializer.Instance.Serialize(bsonWriter, typeof(BsonBinaryData), bsonValue, options); break;
                    //case BsonType.Boolean: BsonBooleanSerializer.Instance.Serialize(bsonWriter, typeof(BsonBoolean), bsonValue, options); break;
                    //case BsonType.DateTime: BsonDateTimeSerializer.Instance.Serialize(bsonWriter, typeof(BsonDateTime), bsonValue, options); break;
                    //case BsonType.Document: BsonDocumentSerializer.Instance.Serialize(bsonWriter, typeof(BsonDocument), bsonValue, options); break;
                    //case BsonType.Double: BsonDoubleSerializer.Instance.Serialize(bsonWriter, typeof(BsonDouble), bsonValue, options); break;
                    case BsonType.Int32: ZIntSerializer.Instance.Serialize(bsonWriter, typeof(BsonInt32), bsonValue, options); break;
                    //case BsonType.Int64: BsonInt64Serializer.Instance.Serialize(bsonWriter, typeof(BsonInt64), bsonValue, options); break;
                    //case BsonType.JavaScript: BsonJavaScriptSerializer.Instance.Serialize(bsonWriter, typeof(BsonJavaScript), bsonValue, options); break;
                    //case BsonType.JavaScriptWithScope: BsonJavaScriptWithScopeSerializer.Instance.Serialize(bsonWriter, typeof(BsonJavaScriptWithScope), bsonValue, options); break;
                    //case BsonType.MaxKey: BsonMaxKeySerializer.Instance.Serialize(bsonWriter, typeof(BsonMaxKey), bsonValue, options); break;
                    //case BsonType.MinKey: BsonMinKeySerializer.Instance.Serialize(bsonWriter, typeof(BsonMinKey), bsonValue, options); break;
                    //case BsonType.Null: BsonNullSerializer.Instance.Serialize(bsonWriter, typeof(BsonNull), bsonValue, options); break;
                    //case BsonType.ObjectId: BsonObjectIdSerializer.Instance.Serialize(bsonWriter, typeof(BsonObjectId), bsonValue, options); break;
                    //case BsonType.RegularExpression: BsonRegularExpressionSerializer.Instance.Serialize(bsonWriter, typeof(BsonRegularExpression), bsonValue, options); break;
                    //case BsonType.String: BsonStringSerializer.Instance.Serialize(bsonWriter, typeof(BsonString), bsonValue, options); break;
                    case BsonType.String: ZStringSerializer.Instance.Serialize(bsonWriter, typeof(ZString), bsonValue, options); break;
                    //case BsonType.Symbol: BsonSymbolSerializer.Instance.Serialize(bsonWriter, typeof(BsonSymbol), bsonValue, options); break;
                    //case BsonType.Timestamp: BsonTimestampSerializer.Instance.Serialize(bsonWriter, typeof(BsonTimestamp), bsonValue, options); break;
                    //case BsonType.Undefined: BsonUndefinedSerializer.Instance.Serialize(bsonWriter, typeof(BsonUndefined), bsonValue, options); break;
                    //default: throw new BsonInternalException("Invalid BsonType.");
                    default: throw new PBException("error serialize ZValue invalid BsonType {0}.", bsonValue.BsonType);
                }
            }
        }
    }
}

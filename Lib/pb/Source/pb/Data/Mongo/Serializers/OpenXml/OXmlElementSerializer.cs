using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    // from BsonValueSerializer
    // IBsonDocumentSerializer -> GetMemberSerializationInfo()
    // IBsonArraySerializer    -> GetItemSerializationInfo()
    public class OXmlElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlElementSerializer _instance = new OXmlElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlElementSerializer Instance { get { return _instance; } }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        // actualType ignored
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlElementSerializer.Deserialize()");

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            string type = bsonReader.FindStringElement("Type");
            OXmlElement value;
            switch (type.ToLower())
            {
                case "line":
                    value = new OXmlElement { Type = OXmlElementType.Line };
                    break;
                case "tabstop":
                    value = new OXmlElement { Type = OXmlElementType.TabStop };
                    break;
                case "paragraph":
                    //value = OXmlParagraphElementSerializer.Instance.Deserialize(bsonReader, typeof(OXmlParagraphElement), options);
                    value = OXmlParagraphElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "text":
                    value = OXmlTextElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "simplefield":
                    value = OXmlSimpleFieldElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "docsection":
                    value = OXmlDocSectionElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "docdefaultsrunproperties":
                    value = OXmlDocDefaultsRunPropertiesElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "docdefaultsparagraphproperties":
                    //value = new OXmlDocDefaultsParagraphPropertiesElement();
                    value = new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
                    break;
                case "openheader":
                    value = OXmlOpenHeaderElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "closeheader":
                    value = new OXmlElement { Type = OXmlElementType.CloseHeader };
                    break;
                case "openfooter":
                    value = OXmlOpenFooterElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "closefooter":
                    value = new OXmlElement { Type = OXmlElementType.CloseFooter };
                    break;
                case "style":
                    value = OXmlStyleElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "picture":
                    //value = OXmlPictureElementSerializer.Instance._Deserialize(bsonReader, options);
                    value = OXmlPictureElementCreator.CreatePicture(zmongo.ReadBsonDocumentWOStartEnd(bsonReader));
                    break;
                default:
                    throw new PBException($"deserialize OXmlElement, invalid Type {type}.");
            }
            bsonReader.ReadEndDocument();
            return value;


            //switch (bsonType)
            //{
            //    case BsonType.Array: return (ZValue)ZStringArraySerializer.Instance.Deserialize(bsonReader, typeof(ZStringArray), options);
            //    //case BsonType.Binary: return (BsonValue)BsonBinaryDataSerializer.Instance.Deserialize(bsonReader, typeof(BsonBinaryData), options);
            //    //case BsonType.Boolean: return (BsonValue)BsonBooleanSerializer.Instance.Deserialize(bsonReader, typeof(BsonBoolean), options);
            //    //case BsonType.DateTime: return (BsonValue)BsonDateTimeSerializer.Instance.Deserialize(bsonReader, typeof(BsonDateTime), options);
            //    //case BsonType.Document: return (BsonValue)BsonDocumentSerializer.Instance.Deserialize(bsonReader, typeof(BsonDocument), options);
            //    //case BsonType.Double: return (BsonValue)BsonDoubleSerializer.Instance.Deserialize(bsonReader, typeof(BsonDouble), options);
            //    case BsonType.Int32: return (ZValue)ZIntSerializer.Instance.Deserialize(bsonReader, typeof(ZInt), options);
            //    //case BsonType.Int64: return (BsonValue)BsonInt64Serializer.Instance.Deserialize(bsonReader, typeof(BsonInt64), options);
            //    //case BsonType.JavaScript: return (BsonValue)BsonJavaScriptSerializer.Instance.Deserialize(bsonReader, typeof(BsonJavaScript), options);
            //    //case BsonType.JavaScriptWithScope: return (BsonValue)BsonJavaScriptWithScopeSerializer.Instance.Deserialize(bsonReader, typeof(BsonJavaScriptWithScope), options);
            //    //case BsonType.MaxKey: return (BsonValue)BsonMaxKeySerializer.Instance.Deserialize(bsonReader, typeof(BsonMaxKey), options);
            //    //case BsonType.MinKey: return (BsonValue)BsonMinKeySerializer.Instance.Deserialize(bsonReader, typeof(BsonMinKey), options);
            //    //case BsonType.Null: return (BsonValue)BsonNullSerializer.Instance.Deserialize(bsonReader, typeof(BsonNull), options);
            //    case BsonType.Document:
            //        return ReadZValueFromBsonDocument(bsonReader);
            //    case BsonType.Null:
            //        bsonReader.ReadNull();
            //        return null;
            //    //case BsonType.ObjectId: return (BsonValue)BsonObjectIdSerializer.Instance.Deserialize(bsonReader, typeof(BsonObjectId), options);
            //    //case BsonType.RegularExpression: return (BsonValue)BsonRegularExpressionSerializer.Instance.Deserialize(bsonReader, typeof(BsonRegularExpression), options);
            //    //case BsonType.String: return (BsonValue)BsonStringSerializer.Instance.Deserialize(bsonReader, typeof(BsonString), options);
            //    case BsonType.String: return (ZValue)ZStringSerializer.Instance.Deserialize(bsonReader, typeof(ZString), options);
            //    // ZStringSerializer
            //    //case BsonType.Symbol: return (BsonValue)BsonSymbolSerializer.Instance.Deserialize(bsonReader, typeof(BsonSymbol), options);
            //    //case BsonType.Timestamp: return (BsonValue)BsonTimestampSerializer.Instance.Deserialize(bsonReader, typeof(BsonTimestamp), options);
            //    //case BsonType.Undefined: return (BsonValue)BsonUndefinedSerializer.Instance.Deserialize(bsonReader, typeof(BsonUndefined), options);
            //    default:
            //        //var message = string.Format("Invalid BsonType {0}.", bsonType);
            //        //throw new BsonInternalException(message);
            //        throw new PBException("error deserialize ZValue, invalid BsonType {0}.", bsonType);
            //}
        }

        /// <summary>
        /// Gets the serialization info for a member.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>
        /// The serialization info for the member.
        /// </returns>
        //public BsonSerializationInfo GetMemberSerializationInfo(string memberName)
        //{
        //    return new BsonSerializationInfo(memberName, Instance, typeof(BsonValue), Instance.GetDefaultSerializationOptions());
        //}

        /// <summary>
        /// Gets the serialization info for individual items of the array.
        /// </summary>
        /// <returns>
        /// The serialization info for the items.
        /// </returns>
        //public BsonSerializationInfo GetItemSerializationInfo()
        //{
        //    return new BsonSerializationInfo(null, Instance, typeof(BsonValue), Instance.GetDefaultSerializationOptions());
        //}

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
                throw new PBException("serialize OXmlElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlElementSerializer.Serialize()");

            bsonWriter.WriteStartDocument();
            switch (((OXmlElement)value).Type)
            {
                case OXmlElementType.Line:
                    bsonWriter.WriteString("Type", "Line");
                    break;
                case OXmlElementType.TabStop:
                    bsonWriter.WriteString("Type", "TabStop");
                    break;
                case OXmlElementType.DocDefaultsParagraphProperties:
                    bsonWriter.WriteString("Type", "DocDefaultsParagraphProperties");
                    break;
                case OXmlElementType.CloseHeader:
                    bsonWriter.WriteString("Type", "CloseHeader");
                    break;
                case OXmlElementType.CloseFooter:
                    bsonWriter.WriteString("Type", "CloseFooter");
                    break;
                default:
                    throw new PBException($"serializer OXmlElement type {((OXmlElement)value).Type} not implemented");
            }
            bsonWriter.WriteEndDocument();
        }
    }
}

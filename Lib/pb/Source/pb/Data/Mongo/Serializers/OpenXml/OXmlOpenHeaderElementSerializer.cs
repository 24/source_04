using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using DocumentFormat.OpenXml.Wordprocessing;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlOpenHeaderElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlOpenHeaderElementSerializer _instance = new OXmlOpenHeaderElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlOpenHeaderElementSerializer Instance { get { return _instance; } }

        public OXmlOpenHeaderElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlOpenHeaderElement element = new OXmlOpenHeaderElement();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "type":
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong type value {bsonType}");
                        string type = bsonReader.ReadString();
                        //"openfooter"
                        if (type.ToLower() != "openheader")
                            throw new PBException($"invalid Type {type} when deserialize OXmlOpenHeader");
                        break;
                    case "headertype":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong HeaderType value {bsonType}");
                        element.HeaderType = bsonReader.ReadString().zParseEnum<HeaderFooterValues>();
                        break;
                    default:
                        //OpenHeaderFooter
                        throw new PBException($"unknow OpenHeader value \"{name}\"");
                }
            }
            return element;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlOpenHeaderElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlOpenHeaderElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlOpenHeader, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlOpenHeader value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlOpenHeaderElementSerializer.Serialize()");

            OXmlOpenHeaderElement element = (OXmlOpenHeaderElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "OpenHeader");
            bsonWriter.WriteString("HeaderType", element.HeaderType.ToString());
            bsonWriter.WriteEndDocument();
        }
    }
}

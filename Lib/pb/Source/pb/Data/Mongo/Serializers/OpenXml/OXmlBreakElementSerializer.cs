using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlBreakElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlBreakElementSerializer _instance = new OXmlBreakElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlBreakElementSerializer Instance { get { return _instance; } }

        public OXmlBreakElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlBreakElement element = new OXmlBreakElement();
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
                        if (type.ToLower() != "break")
                            throw new PBException($"invalid Type {type} when deserialize OXmlBreakElement");
                        break;
                    case "breaktype":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong BreakType value {bsonType}");
                        element.BreakType = bsonReader.ReadString().zParseEnum<BreakValues>(ignoreCase: true);
                        break;
                    default:
                        throw new PBException($"unknow Break value \"{name}\"");
                }
            }
            return element;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlBreakElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlBreakElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlBreakElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlBreakElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlBreakElementSerializer.Serialize()");

            OXmlBreakElement element = (OXmlBreakElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "Break");
            bsonWriter.WriteString("BreakType", element.BreakType.ToString());
            bsonWriter.WriteEndDocument();
        }
    }
}

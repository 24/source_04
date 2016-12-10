using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using DocumentFormat.OpenXml.Wordprocessing;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlOpenFooterElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlOpenFooterElementSerializer _instance = new OXmlOpenFooterElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlOpenFooterElementSerializer Instance { get { return _instance; } }

        public OXmlOpenFooterElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlOpenFooterElement element = new OXmlOpenFooterElement();
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
                        if (type.ToLower() != "openfooter")
                            throw new PBException($"invalid Type {type} when deserialize OXmlOpenFooter");
                        break;
                    case "footertype":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong FooterType value {bsonType}");
                        element.FooterType = bsonReader.ReadString().zParseEnum<HeaderFooterValues>();
                        break;
                    default:
                        throw new PBException($"unknow OpenFooter value \"{name}\"");
                }
            }
            return element;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlOpenFooterElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlOpenFooterElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlOpenFooter, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlOpenFooter value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlOpenFooterElementSerializer.Serialize()");

            OXmlOpenFooterElement element = (OXmlOpenFooterElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "OpenFooter");
            bsonWriter.WriteString("FooterType", element.FooterType.ToString());
            bsonWriter.WriteEndDocument();
        }
    }
}

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlSimpleFieldElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlSimpleFieldElementSerializer _instance = new OXmlSimpleFieldElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlSimpleFieldElementSerializer Instance { get { return _instance; } }

        public OXmlSimpleFieldElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlSimpleFieldElement element = new OXmlSimpleFieldElement();
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
                        if (type.ToLower() != "simplefield")
                            throw new PBException($"invalid Type {type} when deserialize OXmlSimpleFieldElement");
                        break;
                    case "instruction":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong Instruction value {bsonType}");
                        element.Instruction = bsonReader.ReadString();
                        break;
                    default:
                        throw new PBException($"unknow SimpleField value \"{name}\"");
                }
            }
            return element;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlSimpleFieldElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlSimpleFieldElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlSimpleFieldElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlSimpleFieldElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlSimpleFieldElementSerializer.Serialize()");

            OXmlSimpleFieldElement element = (OXmlSimpleFieldElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "SimpleField");
            bsonWriter.WriteString("Instruction", element.Instruction);
            bsonWriter.WriteEndDocument();
        }
    }
}

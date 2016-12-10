using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlTextElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlTextElementSerializer _instance = new OXmlTextElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlTextElementSerializer Instance { get { return _instance; } }

        public OXmlTextElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlTextElement element = new OXmlTextElement();
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
                        if (type.ToLower() != "text")
                            throw new PBException($"invalid Type {type} when deserialize OXmlTextElement");
                        break;
                    case "text":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong text value {bsonType}");
                        element.Text = bsonReader.ReadString();
                        break;
                    case "preservespace":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong PreserveSpace value {bsonType}");
                        element.PreserveSpace = bsonReader.ReadBoolean();
                        break;
                    default:
                        throw new PBException($"unknow Text value \"{name}\"");
                }
            }
            return element;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlTextElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlTextElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlTextElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlTextElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlTextElementSerializer.Serialize()");

            OXmlTextElement element = (OXmlTextElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "Text");
            if (element.Text != null)
                bsonWriter.WriteString("Text", element.Text);
            if (element.PreserveSpace)
                bsonWriter.WriteBoolean("PreserveSpace", element.PreserveSpace);
            bsonWriter.WriteEndDocument();
        }
    }
}

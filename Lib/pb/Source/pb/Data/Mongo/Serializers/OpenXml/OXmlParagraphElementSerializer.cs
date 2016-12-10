using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlParagraphElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlParagraphElementSerializer _instance = new OXmlParagraphElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlParagraphElementSerializer Instance { get { return _instance; } }

        public OXmlParagraphElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlParagraphElement paragraph = new OXmlParagraphElement();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                //if (bsonType != BsonType.String)
                //    throw new PBException("error ZStringArray cannot contain value of type {0}", bsonType);
                //var value = bsonReader.ReadString();
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "type":
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong type value {bsonType}");
                        string type = bsonReader.ReadString();
                        if (type.ToLower() != "paragraph")
                            throw new PBException($"invalid Type {type} when deserialize OXmlParagraphElement");
                        break;
                    case "style":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong style value {bsonType}");
                        paragraph.Style = bsonReader.ReadString();
                        break;
                    default:
                        throw new PBException($"unknow Paragraph value \"{name}\"");
                }
            }
            return paragraph;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlParagraphElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlParagraphElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlParagraphElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            //string type = bsonReader.FindStringElement("Type");
            //if (type.ToLower() != "paragraph")
            //    throw new PBException($"error deserialize OXmlParagraphElement, invalid Type {type}.");
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlParagraphElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlParagraphElementSerializer.Serialize()");

            OXmlParagraphElement paragraph = (OXmlParagraphElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "Paragraph");
            if (paragraph.Style != null)
                bsonWriter.WriteString("Style", paragraph.Style);
            bsonWriter.WriteEndDocument();
        }
    }
}

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlDocDefaultsRunPropertiesElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlDocDefaultsRunPropertiesElementSerializer _instance = new OXmlDocDefaultsRunPropertiesElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlDocDefaultsRunPropertiesElementSerializer Instance { get { return _instance; } }

        public OXmlDocDefaultsRunPropertiesElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlDocDefaultsRunPropertiesElement element = new OXmlDocDefaultsRunPropertiesElement();
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
                        if (type.ToLower() != "docdefaultsrunproperties")
                            throw new PBException($"invalid Type {type} when deserialize OXmlDocDefaultsRunPropertiesElement");
                        break;
                    case "runfonts":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong RunFonts value {bsonType}");
                        element.RunFonts = OXmlCommonSerializer.ReadRunFonts(bsonReader);
                        break;
                    case "fontsize":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong FontSize value {bsonType}");
                        element.FontSize = bsonReader.ReadString();
                        break;
                    default:
                        throw new PBException($"unknow DocDefaultsRunProperties value \"{name}\"");
                }
            }
            return element;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlRunPropertiesDefaultElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlDocDefaultsRunPropertiesElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlDocDefaultsRunPropertiesElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlDocDefaultsRunPropertiesElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlRunPropertiesDefaultElementSerializer.Serialize()");

            OXmlDocDefaultsRunPropertiesElement element = (OXmlDocDefaultsRunPropertiesElement)value;
            bsonWriter.WriteStartDocument();

            bsonWriter.WriteString("Type", "DocDefaultsRunProperties");

            if (element.RunFonts != null)
            {
                bsonWriter.WriteStartDocument("RunFonts");
                OXmlCommonSerializer.WriteRunFonts(bsonWriter, element.RunFonts);
                bsonWriter.WriteEndDocument();
            }

            if (element.FontSize != null)
                bsonWriter.WriteString("FontSize", element.FontSize);

            bsonWriter.WriteEndDocument();
        }
    }
}

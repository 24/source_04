using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlDocSectionElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlDocSectionElementSerializer _instance = new OXmlDocSectionElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlDocSectionElementSerializer Instance { get { return _instance; } }

        public OXmlDocSectionElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlDocSectionElement element = new OXmlDocSectionElement();
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
                        if (type.ToLower() != "docsection")
                            throw new PBException($"invalid Type {type} when deserialize OXmlDocSectionElement");
                        break;
                    case "pagesize":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong PageSize value {bsonType}");
                        element.PageSize = ReadPageSize(bsonReader);
                        break;
                    case "pagemargin":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong PageMargin value {bsonType}");
                        element.PageMargin = ReadPageMargin(bsonReader);
                        break;
                    case "pagenumberstart":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageNumberStart value {bsonType}");
                        element.PageNumberStart = bsonReader.ReadInt32();
                        break;
                    default:
                        throw new PBException($"unknow DocSection value \"{name}\"");
                }
            }
            return element;
        }

        private static OXmlPageSize ReadPageSize(BsonReader bsonReader)
        {
            bsonReader.ReadStartDocument();
            OXmlPageSize value = new OXmlPageSize();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "width":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageSize width value {bsonType}");
                        value.Width = bsonReader.ReadInt32();
                        break;
                    case "height":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageSize height value {bsonType}");
                        value.Height = bsonReader.ReadInt32();
                        break;
                    default:
                        throw new PBException($"unknow PageSize value \"{name}\"");
                }
            }
            bsonReader.ReadEndDocument();
            return value;
        }

        private static OXmlPageMargin ReadPageMargin(BsonReader bsonReader)
        {
            bsonReader.ReadStartDocument();
            OXmlPageMargin value = new OXmlPageMargin();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "top":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageMargin Top value {bsonType}");
                        value.Top = bsonReader.ReadInt32();
                        break;
                    case "bottom":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageMargin Bottom value {bsonType}");
                        value.Bottom = bsonReader.ReadInt32();
                        break;
                    case "left":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageMargin Left value {bsonType}");
                        value.Left = bsonReader.ReadInt32();
                        break;
                    case "right":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageMargin Right value {bsonType}");
                        value.Right = bsonReader.ReadInt32();
                        break;
                    case "header":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageMargin Header value {bsonType}");
                        value.Header = bsonReader.ReadInt32();
                        break;
                    case "footer":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong PageMargin Footer value {bsonType}");
                        value.Footer = bsonReader.ReadInt32();
                        break;
                    default:
                        throw new PBException($"unknow PageMargin value \"{name}\"");
                }
            }
            bsonReader.ReadEndDocument();
            return value;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlDocSectionElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlDocSectionElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlDocSectionElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlDocSectionElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlDocSectionElementSerializer.Serialize()");

            OXmlDocSectionElement element = (OXmlDocSectionElement)value;
            bsonWriter.WriteStartDocument();

            bsonWriter.WriteString("Type", "DocSection");

            if (element.PageSize != null)
            {
                OXmlPageSize pageSize = element.PageSize;
                bsonWriter.WriteStartDocument("PageSize");
                if (pageSize.Width != null)
                    bsonWriter.WriteInt32("Width", (int)pageSize.Width);
                if (pageSize.Height != null)
                    bsonWriter.WriteInt32("Height", (int)pageSize.Height);
                bsonWriter.WriteEndDocument();
            }

            if (element.PageMargin != null)
            {
                OXmlPageMargin pageMargin = element.PageMargin;
                bsonWriter.WriteStartDocument("PageMargin");
                if (pageMargin.Top != null)
                    bsonWriter.WriteInt32("Top", (int)pageMargin.Top);
                if (pageMargin.Bottom != null)
                    bsonWriter.WriteInt32("Bottom", (int)pageMargin.Bottom);
                if (pageMargin.Left != null)
                    bsonWriter.WriteInt32("Left", (int)pageMargin.Left);
                if (pageMargin.Right != null)
                    bsonWriter.WriteInt32("Right", (int)pageMargin.Right);
                if (pageMargin.Header != null)
                    bsonWriter.WriteInt32("Header", (int)pageMargin.Header);
                if (pageMargin.Footer != null)
                    bsonWriter.WriteInt32("Footer", (int)pageMargin.Footer);
                bsonWriter.WriteEndDocument();
            }

            if (element.PageNumberStart != null)
                bsonWriter.WriteInt32("PageNumberStart", (int)element.PageNumberStart);

            bsonWriter.WriteEndDocument();
        }
    }
}

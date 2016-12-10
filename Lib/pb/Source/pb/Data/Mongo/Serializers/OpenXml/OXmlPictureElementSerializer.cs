using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using A = DocumentFormat.OpenXml.Drawing;
using pb.Data.OpenXml;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlPictureElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlPictureElementSerializer _instance = new OXmlPictureElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlPictureElementSerializer Instance { get { return _instance; } }

        //public OXmlPictureElement _Deserialize(BsonDocument document)
        //{
        //}

        //public OXmlPictureElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        //{
        //    OXmlPictureElement element = new OXmlPictureElement();
        //    OXmlPictureDrawing pictureDrawing = new OXmlPictureDrawing();
        //    element.PictureDrawing = pictureDrawing;
        //    OXmlPictureDrawingMode? drawingMode = null;
        //    while (true)
        //    {
        //        BsonType bsonType = bsonReader.ReadBsonType();
        //        if (bsonType == BsonType.EndOfDocument)
        //            break;
        //        string name = bsonReader.ReadName();
        //        switch (name.ToLower())
        //        {
        //            case "type":
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong type value {bsonType}");
        //                string type = bsonReader.ReadString();
        //                if (type.ToLower() != "picture")
        //                    throw new PBException($"invalid Type {type} when deserialize OXmlPictureElement");
        //                break;
        //            case "file":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong File value {bsonType}");
        //                element.File = bsonReader.ReadString();
        //                break;
        //            case "name":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong Name value {bsonType}");
        //                element.Name = bsonReader.ReadString();
        //                break;
        //            case "description":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong Description value {bsonType}");
        //                element.Description = bsonReader.ReadString();
        //                break;
        //            case "width":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.Int32)
        //                    throw new PBException($"wrong Width value {bsonType}");
        //                element.Width = bsonReader.ReadInt32();
        //                break;
        //            case "height":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.Int32)
        //                    throw new PBException($"wrong Height value {bsonType}");
        //                element.Height = bsonReader.ReadInt32();
        //                break;
        //            case "rotation":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.Int32)
        //                    throw new PBException($"wrong Rotation value {bsonType}");
        //                element.Rotation = bsonReader.ReadInt32();
        //                break;
        //            case "horizontalflip":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.Boolean)
        //                    throw new PBException($"wrong HorizontalFlip value {bsonType}");
        //                element.HorizontalFlip = bsonReader.ReadBoolean();
        //                break;
        //            case "verticalflip":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.Boolean)
        //                    throw new PBException($"wrong VerticalFlip value {bsonType}");
        //                element.VerticalFlip = bsonReader.ReadBoolean();
        //                break;
        //            case "compressionstate":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong CompressionState value {bsonType}");
        //                element.CompressionState = bsonReader.ReadString().zParseEnum<A.BlipCompressionValues>(ignoreCase: true);
        //                break;
        //            case "presetshape":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong PresetShape value {bsonType}");
        //                element.PresetShape = bsonReader.ReadString().zParseEnum<A.ShapeTypeValues>(ignoreCase: true);
        //                break;
        //            //case "picturedrawing":
        //            //    if (bsonType == BsonType.Null)
        //            //        break;
        //            //    if (bsonType != BsonType.Document)
        //            //        throw new PBException($"wrong PictureDrawing value {bsonType}");
        //            //    element.PictureDrawing = ReadPictureDrawing(bsonReader);
        //            //    break;
        //            case "drawingmode":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong DrawingMode value {bsonType}");
        //                OXmlPictureDrawingMode drawingMode2 = bsonReader.ReadString().zParseEnum<OXmlPictureDrawingMode>(ignoreCase: true);
        //                if (drawingMode != null && drawingMode != drawingMode2)
        //                    throw new PBException($"wrong DrawingMode {drawingMode2}");
        //                drawingMode = drawingMode2;
        //                pictureDrawing.DrawingMode = drawingMode2;
        //                break;
        //            case "wraptype":
        //                if (bsonType == BsonType.Null)
        //                    break;
        //                if (bsonType != BsonType.String)
        //                    throw new PBException($"wrong WrapType value {bsonType}");
        //                element.WrapType = bsonReader.ReadString().zParseEnum<A.ShapeTypeValues>(ignoreCase: true);
        //                break;
        //            default:
        //                throw new PBException($"unknow Text value \"{name}\"");
        //        }
        //        //OXmlInlinePictureDrawing
        //        //OXmlAnchorPictureDrawing
        //    }
        //    return element;
        //}

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlPictureElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlPictureElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlPictureElement, invalid BsonType {bsonType}.");
            //bsonReader.ReadStartDocument();
            //object value = _Deserialize(bsonReader, options);
            //bsonReader.ReadEndDocument();
            //return value;
            //return _Deserialize(BsonSerializer.Deserialize<BsonDocument>(bsonReader));
            return OXmlPictureElementCreator.CreatePicture(BsonSerializer.Deserialize<BsonDocument>(bsonReader));
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlPictureElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlPictureElementSerializer.Serialize()");

            OXmlPictureElement element = (OXmlPictureElement)value;
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteString("Type", "Picture");
            bsonWriter.WriteString("File", element.File);
            if (element.Name != null)
                bsonWriter.WriteString("Name", element.Name);
            if (element.Description != null)
                bsonWriter.WriteString("Description", element.Description);
            if (element.Width != null)
                bsonWriter.WriteInt32("Width", (int)element.Width);
            if (element.Height != null)
                bsonWriter.WriteInt32("Height", (int)element.Height);
            if (element.Rotation != 0)
                bsonWriter.WriteInt32("Rotation", element.Rotation);
            if (element.HorizontalFlip)
                bsonWriter.WriteBoolean("HorizontalFlip", element.HorizontalFlip);
            if (element.VerticalFlip)
                bsonWriter.WriteBoolean("VerticalFlip", element.VerticalFlip);
            if (element.CompressionState != A.BlipCompressionValues.Print)
                bsonWriter.WriteString("CompressionState", element.CompressionState.ToString());
            if (element.PresetShape != A.ShapeTypeValues.Rectangle)
                bsonWriter.WriteString("PresetShape", element.PresetShape.ToString());
            //bsonWriter.WriteString("DrawingMode", element.PictureDrawing.DrawingMode.ToString());
            bsonWriter.WriteString("DrawingType", element.PictureDrawing.GetDrawingType().ToString());
            if (element.PictureDrawing.DrawingMode == OXmlPictureDrawingMode.Anchor)
            {
                OXmlAnchorPictureDrawing anchor = (OXmlAnchorPictureDrawing)element.PictureDrawing;
                //bsonWriter.WriteString("WrapType", anchor.Wrap.WrapType.ToString());
                bsonWriter.WriteString("HorizontalRelativeFrom", anchor.HorizontalRelativeFrom.ToString());
                if (anchor.HorizontalPositionOffset != null)
                    bsonWriter.WriteInt32("HorizontalPositionOffset", (int)anchor.HorizontalPositionOffset);
                if (anchor.HorizontalAlignment != null)
                    bsonWriter.WriteString("HorizontalAlignment", anchor.HorizontalAlignment);
                bsonWriter.WriteString("VerticalRelativeFrom", anchor.VerticalRelativeFrom.ToString());
                if (anchor.VerticalPositionOffset != null)
                    bsonWriter.WriteInt32("VerticalPositionOffset", (int)anchor.VerticalPositionOffset);
                if (anchor.VerticalAlignment != null)
                    bsonWriter.WriteString("VerticalAlignment", anchor.VerticalAlignment);
                switch (anchor.Wrap.WrapType)
                {
                    case OXmlAnchorWrapType.WrapSquare:
                        SerializeWrapSquare(bsonWriter, (OXmlAnchorWrapSquare)anchor.Wrap);
                        break;
                    case OXmlAnchorWrapType.WrapTight:
                        SerializeWrapTight(bsonWriter, (OXmlAnchorWrapTight)anchor.Wrap);
                        break;
                    case OXmlAnchorWrapType.WrapTopAndBottom:
                        SerializeWrapTopBottom(bsonWriter, (OXmlAnchorWrapTopAndBottom)anchor.Wrap);
                        break;
                    default:
                        throw new PBException($"WrapType {anchor.Wrap.WrapType} serialize not implemented");
                }
            }
            bsonWriter.WriteEndDocument();
        }

        private static void SerializeWrapSquare(BsonWriter bsonWriter, OXmlAnchorWrapSquare wrap)
        {
            //if (wrap.WrapText != DW.WrapTextValues.BothSides)
            bsonWriter.WriteString("WrapText", wrap.WrapText.ToString());
            if (wrap.DistanceFromTop != 0)
                bsonWriter.WriteInt32("DistanceFromTop", (int)wrap.DistanceFromTop);
            if (wrap.DistanceFromBottom != 0)
                bsonWriter.WriteInt32("DistanceFromBottom", (int)wrap.DistanceFromBottom);
            if (wrap.DistanceFromLeft != 0)
                bsonWriter.WriteInt32("DistanceFromLeft", (int)wrap.DistanceFromLeft);
            if (wrap.DistanceFromRight != 0)
                bsonWriter.WriteInt32("DistanceFromRight", (int)wrap.DistanceFromRight);
            SerializeEffectExtent(bsonWriter, wrap.EffectExtent);
        }

        private static void SerializeWrapTight(BsonWriter bsonWriter, OXmlAnchorWrapTight wrap)
        {
            //if (wrap.WrapText != DW.WrapTextValues.BothSides)
            bsonWriter.WriteString("WrapText", wrap.WrapText.ToString());
            if (wrap.DistanceFromLeft != 0)
                bsonWriter.WriteInt32("DistanceFromLeft", (int)wrap.DistanceFromLeft);
            if (wrap.DistanceFromRight != 0)
                bsonWriter.WriteInt32("DistanceFromRight", (int)wrap.DistanceFromRight);
            SerializePolygonBase(bsonWriter, wrap.WrapPolygon);
        }

        private static void SerializeWrapTopBottom(BsonWriter bsonWriter, OXmlAnchorWrapTopAndBottom wrap)
        {
            if (wrap.DistanceFromTop != 0)
                bsonWriter.WriteInt32("DistanceFromTop", (int)wrap.DistanceFromTop);
            if (wrap.DistanceFromBottom != 0)
                bsonWriter.WriteInt32("DistanceFromBottom", (int)wrap.DistanceFromBottom);
            SerializeEffectExtent(bsonWriter, wrap.EffectExtent);
        }

        private static void SerializeEffectExtent(BsonWriter bsonWriter, OXmlEffectExtent effectExtent)
        {
            if (effectExtent != null)
            {
                bsonWriter.WriteStartDocument("EffectExtent");
                if (effectExtent.TopEdge != null)
                    bsonWriter.WriteInt64("TopEdge", (long)effectExtent.TopEdge);
                if (effectExtent.BottomEdge != null)
                    bsonWriter.WriteInt64("BottomEdge", (long)effectExtent.BottomEdge);
                if (effectExtent.LeftEdge != null)
                    bsonWriter.WriteInt64("LeftEdge", (long)effectExtent.LeftEdge);
                if (effectExtent.RightEdge != null)
                    bsonWriter.WriteInt64("RightEdge", (long)effectExtent.RightEdge);
                bsonWriter.WriteEndDocument();
            }
        }

        private static void SerializePolygonBase(BsonWriter bsonWriter, OXmlPolygonBase polygon)
        {
            if (polygon == null)
                return;
            if (polygon is OXmlPolygon)
                SerializePolygon(bsonWriter, (OXmlPolygon)polygon);
            else if (polygon is OXmlSquare)
                SerializeSquare(bsonWriter, (OXmlSquare)polygon);

        }

        private static void SerializePolygon(BsonWriter bsonWriter, OXmlPolygon polygon)
        {
            bsonWriter.WriteStartDocument("Polygon");
            if (polygon.StartPoint != null)
            {
                bsonWriter.WriteInt64("StartPointX", polygon.StartPoint.X);
                bsonWriter.WriteInt64("StartPointY", polygon.StartPoint.Y);
            }
            bsonWriter.WriteStartArray("LinesTo");
            foreach (OXmlPoint2DType lineTo in polygon.LinesTo)
            {
                bsonWriter.WriteStartDocument();
                bsonWriter.WriteInt64("X", lineTo.X);
                bsonWriter.WriteInt64("Y", lineTo.Y);
                bsonWriter.WriteEndDocument();
            }
            bsonWriter.WriteEndArray();
            if (polygon.Edited != null)
                bsonWriter.WriteBoolean("Edited", (bool)polygon.Edited);
            bsonWriter.WriteEndDocument();
        }

        private static void SerializeSquare(BsonWriter bsonWriter, OXmlSquare square)
        {
            bsonWriter.WriteStartDocument("Square");
            if (square.StartPoint != null)
            {
                bsonWriter.WriteInt64("StartPointX", square.StartPoint.X);
                bsonWriter.WriteInt64("StartPointY", square.StartPoint.Y);
            }
            bsonWriter.WriteInt64("HorizontalSize", square.HorizontalSize);
            bsonWriter.WriteInt64("VerticalSize", square.VerticalSize);
            if (square.Edited != null)
                bsonWriter.WriteBoolean("Edited", (bool)square.Edited);
            bsonWriter.WriteEndDocument();
        }
    }
}

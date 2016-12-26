using MongoDB.Bson;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using pb.Data.Mongo;
using System.Collections.Generic;

namespace pb.Data.OpenXml
{
    public class OXmlPictureElementCreator
    {
        public static OXmlElement CreatePicture(BsonDocument document)
        {
            OXmlPictureElement picture = new OXmlPictureElement();
            picture.File = document.zGet("File").zAsString();
            picture.Name = document.zGet("Name").zAsString();
            picture.Description = document.zGet("Description").zAsString();
            picture.Width = document.zGet("Width").zAsNullableInt();
            picture.Height = document.zGet("Height").zAsNullableInt();
            picture.Rotation = document.zGet("Rotation").zAsInt();
            picture.HorizontalFlip = document.zGet("HorizontalFlip").zAsBoolean();
            picture.VerticalFlip = document.zGet("VerticalFlip").zAsBoolean();
            //picture.CompressionState = GetCompressionState(element.zGet("Description").zAsString());
            picture.CompressionState = document.zGet("CompressionState").zAsString().zTryParseEnum(A.BlipCompressionValues.Print);
            picture.PresetShape = document.zGet("PresetShape").zAsString().zTryParseEnum(A.ShapeTypeValues.Rectangle);
            picture.PictureDrawing = CreatePictureDrawing(document);
            return picture;
        }

        private static OXmlPictureDrawing CreatePictureDrawing(BsonDocument element)
        {
            //string drawingMode = element.zGet("DrawingMode").zAsString();
            //switch (drawingMode.ToLower())
            //{
            //    case "inline":
            //        return new OXmlInlinePictureDrawing();
            //    case "anchor":
            //        return CreateAnchorPictureDrawing(element);
            //    default:
            //        throw new PBException($"unknow oxml drawing mode \"{drawingMode}\"");
            //}
            OXmlPictureDrawingType drawingType = element.zGet("DrawingType").zAsString().zParseEnum<OXmlPictureDrawingType>(ignoreCase: true);
            OXmlPictureDrawingMode drawingMode;
            OXmlAnchorWrapType wrapType;
            GetDrawingModeAndWrapType(drawingType, out drawingMode, out wrapType);
            switch (drawingMode)
            {
                case OXmlPictureDrawingMode.Inline:
                    return new OXmlInlinePictureDrawing();
                case OXmlPictureDrawingMode.Anchor:
                    return CreateAnchorPictureDrawing(element, wrapType);
                default:
                    throw new PBException($"unknow oxml drawing mode \"{drawingMode}\"");
            }
        }

        private static void GetDrawingModeAndWrapType(OXmlPictureDrawingType drawingType, out OXmlPictureDrawingMode drawingMode, out OXmlAnchorWrapType wrapType)
        {
            switch (drawingType)
            {
                case OXmlPictureDrawingType.Inline:
                    drawingMode = OXmlPictureDrawingMode.Inline;
                    wrapType = OXmlAnchorWrapType.WrapNone;
                    break;
                case OXmlPictureDrawingType.AnchorWrapNone:
                    drawingMode = OXmlPictureDrawingMode.Anchor;
                    wrapType = OXmlAnchorWrapType.WrapNone;
                    break;
                case OXmlPictureDrawingType.AnchorWrapSquare:
                    drawingMode = OXmlPictureDrawingMode.Anchor;
                    wrapType = OXmlAnchorWrapType.WrapSquare;
                    break;
                case OXmlPictureDrawingType.AnchorWrapTight:
                    drawingMode = OXmlPictureDrawingMode.Anchor;
                    wrapType = OXmlAnchorWrapType.WrapTight;
                    break;
                case OXmlPictureDrawingType.AnchorWrapThrough:
                    drawingMode = OXmlPictureDrawingMode.Anchor;
                    wrapType = OXmlAnchorWrapType.WrapThrough;
                    break;
                case OXmlPictureDrawingType.AnchorWrapTopAndBottom:
                    drawingMode = OXmlPictureDrawingMode.Anchor;
                    wrapType = OXmlAnchorWrapType.WrapTopAndBottom;
                    break;
                default:
                    throw new PBException($"unknow DrawingType {drawingType}");
            }
        }

        private static OXmlAnchorPictureDrawing CreateAnchorPictureDrawing(BsonDocument element, OXmlAnchorWrapType wrapType)
        {
            OXmlAnchorPictureDrawing anchor = new OXmlAnchorPictureDrawing();

            anchor.AnchorId = element.zGet("AnchorId").zAsString();
            anchor.EditId = element.zGet("EditId").zAsString();
            anchor.DistanceFromTop = element.zGet("DistanceFromTop").zAsNullableInt();
            anchor.DistanceFromBottom = element.zGet("DistanceFromBottom").zAsNullableInt();
            anchor.DistanceFromLeft = element.zGet("DistanceFromLeft").zAsNullableInt();
            anchor.DistanceFromRight = element.zGet("DistanceFromRight").zAsNullableInt();

            anchor.HorizontalPosition.RelativeFrom = element.zGet("HorizontalRelativeFrom").zAsString().zTryParseEnum(DW.HorizontalRelativePositionValues.Margin);
            anchor.HorizontalPosition.PositionOffset = element.zGet("HorizontalPositionOffset").zAsNullableInt();
            anchor.HorizontalPosition.HorizontalAlignment = element.zGet("HorizontalAlignment").zAsString();

            anchor.VerticalPosition.RelativeFrom = element.zGet("VerticalRelativeFrom").zAsString().zTryParseEnum(DW.VerticalRelativePositionValues.Paragraph);
            anchor.VerticalPosition.PositionOffset = element.zGet("VerticalPositionOffset").zAsNullableInt();
            anchor.VerticalPosition.VerticalAlignment = element.zGet("VerticalAlignment").zAsString();

            anchor.EffectExtent = CreateEffectExtent(element.zGet("EffectExtent").zAsBsonDocument());

            anchor.AllowOverlap = element.zGet("AllowOverlap").zAsBoolean();
            anchor.BehindDoc = element.zGet("BehindDoc").zAsBoolean();
            anchor.Hidden = element.zGet("Hidden").zAsBoolean();
            anchor.LayoutInCell = element.zGet("LayoutInCell").zAsBoolean();
            anchor.Locked = element.zGet("Locked").zAsBoolean();
            anchor.RelativeHeight = element.zGet("RelativeHeight").zAsInt();

            BsonDocument element2 = element.zGet("SimplePosition").zAsBsonDocument();
            if (element2 != null)
                anchor.SimplePosition = new OXmlPoint2DType { X = element2.zGet("X").zAsLong(), Y = element.zGet("Y").zAsLong() };

            switch (wrapType)
            {
                //case OXmlAnchorWrapType.WrapNone:
                case OXmlAnchorWrapType.WrapSquare:
                    anchor.Wrap = CreateAnchorWrapSquare(element.zGet("WrapSquare").zAsBsonDocument());
                    break;
                case OXmlAnchorWrapType.WrapTight:
                    anchor.Wrap = CreateAnchorWrapTight(element.zGet("WrapTight").zAsBsonDocument());
                    break;
                //case OXmlAnchorWrapType.WrapThrough:
                case OXmlAnchorWrapType.WrapTopAndBottom:
                    anchor.Wrap = CreateAnchorWrapTopAndBottom(element.zGet("WrapTopAndBottom").zAsBsonDocument());
                    break;
                default:
                    throw new PBException($"wrap type \"{wrapType}\" not implemented");
            }

            return anchor;
        }

        private static OXmlAnchorWrapSquare CreateAnchorWrapSquare(BsonDocument element)
        {
            OXmlAnchorWrapSquare wrapSquare = new OXmlAnchorWrapSquare();
            if (element != null)
            {
                wrapSquare.WrapText = element.zGet("WrapText").zAsString().zTryParseEnum(DW.WrapTextValues.BothSides);
                wrapSquare.DistanceFromTop = element.zGet("DistanceFromTop").zAsNullableInt();
                wrapSquare.DistanceFromBottom = element.zGet("DistanceFromBottom").zAsNullableInt();
                wrapSquare.DistanceFromLeft = element.zGet("DistanceFromLeft").zAsNullableInt();
                wrapSquare.DistanceFromRight = element.zGet("DistanceFromRight").zAsNullableInt();
                wrapSquare.EffectExtent = CreateEffectExtent(element.zGet("EffectExtent").zAsBsonDocument());
            }
            return wrapSquare;
        }

        private static OXmlEffectExtent CreateEffectExtent(BsonDocument element)
        {
            if (element == null)
                return null;
            OXmlEffectExtent effectExtent = new OXmlEffectExtent();
            effectExtent.TopEdge = element.zGet("TopEdge").zAsNullableLong();
            effectExtent.BottomEdge = element.zGet("BottomEdge").zAsNullableLong();
            effectExtent.LeftEdge = element.zGet("LeftEdge").zAsNullableLong();
            effectExtent.RightEdge = element.zGet("RightEdge").zAsNullableLong();
            return effectExtent;
        }

        private static OXmlAnchorWrapTight CreateAnchorWrapTight(BsonDocument element)
        {
            OXmlAnchorWrapTight wrapTight = new OXmlAnchorWrapTight();
            if (element != null)
            {
                wrapTight.WrapText = element.zGet("WrapText").zAsString().zTryParseEnum(DW.WrapTextValues.BothSides);
                wrapTight.DistanceFromLeft = element.zGet("DistanceFromLeft").zAsNullableInt();
                wrapTight.DistanceFromRight = element.zGet("DistanceFromRight").zAsNullableInt();
                //wrapTight.WrapPolygon = OXmlDoc.CreateWrapPolygon(element.zGet("WrapPolygonHorizontalSize").zAsLong(), element.zGet("WrapPolygonVerticalSize").zAsLong(), element.zGet("WrapPolygonStartPointX").zAsLong(), element.zGet("WrapPolygonStartPointY").zAsLong());
                //wrapTight.WrapPolygon = new OXmlSquare
                //{
                //    StartPointX = element.zGet("WrapPolygonStartPointX").zAsLong(),
                //    StartPointY = element.zGet("WrapPolygonStartPointY").zAsLong(),
                //    HorizontalSize = element.zGet("WrapPolygonHorizontalSize").zAsLong(),
                //    VerticalSize = element.zGet("WrapPolygonVerticalSize").zAsLong()
                //};
                BsonDocument polygon = element.zGet("Polygon").zAsBsonDocument();
                if (polygon != null)
                    wrapTight.WrapPolygon = CreatePolygon(polygon);
                else
                {
                    polygon = element.zGet("Square").zAsBsonDocument();
                    if (polygon != null)
                        wrapTight.WrapPolygon = CreateSquare(polygon);
                }
            }
            return wrapTight;
        }

        private static OXmlPolygon CreatePolygon(BsonDocument element)
        {
            if (element == null)
                return null;
            OXmlPolygon polygon = new OXmlPolygon
            {
                StartPoint = new OXmlPoint2DType { X = element.zGet("StartPointX").zAsLong(), Y = element.zGet("StartPointY").zAsLong() },
                Edited = element.zGet("Edited").zAsNullableBoolean()
            };
            List<OXmlPoint2DType> linesTo = new List<OXmlPoint2DType>();
            foreach (BsonValue line in element.zGet("LinesTo").zAsBsonArray())
            {
                BsonDocument line2 = line.AsBsonDocument;
                linesTo.Add(new OXmlPoint2DType { X = line2.zGet("X").zAsLong(), Y = line2.zGet("Y").zAsLong() });
            }
            polygon.LinesTo = linesTo.ToArray();
            return polygon;
        }

        private static OXmlSquare CreateSquare(BsonDocument element)
        {
            if (element == null)
                return null;
            return new OXmlSquare
            {
                //StartPointX = element.zGet("StartPointX").zAsLong(),
                //StartPointY = element.zGet("StartPointY").zAsLong(),
                StartPoint = new OXmlPoint2DType { X = element.zGet("StartPointX").zAsLong(), Y = element.zGet("StartPointY").zAsLong() },
                HorizontalSize = element.zGet("HorizontalSize").zAsLong(),
                VerticalSize = element.zGet("VerticalSize").zAsLong(),
                Edited = element.zGet("Edited").zAsNullableBoolean()
            };
        }

        private static OXmlAnchorWrapTopAndBottom CreateAnchorWrapTopAndBottom(BsonDocument element)
        {
            OXmlAnchorWrapTopAndBottom wrapTopAndBottom = new OXmlAnchorWrapTopAndBottom();
            if (element != null)
            {
                wrapTopAndBottom.DistanceFromTop = element.zGet("DistanceFromTop").zAsNullableInt();
                wrapTopAndBottom.DistanceFromBottom = element.zGet("DistanceFromBottom").zAsNullableInt();
                wrapTopAndBottom.EffectExtent = CreateEffectExtent(element.zGet("EffectExtent").zAsBsonDocument());
            }
            return wrapTopAndBottom;
        }
    }
}

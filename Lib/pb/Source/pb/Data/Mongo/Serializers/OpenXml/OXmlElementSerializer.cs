using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;

// doc :
//
//   add paragraph
//     function : OXmlElementReader.CreateParagraph()
//     json :
//     { Type: "Paragraph", Style: (string) }
//
//   add break
//     function : OXmlElementReader.CreateBreak()
//     json :
//     { Type: "Break", BreakType: (BreakValues : Page, Column, TextWrapping) }
//
//   add text
//     function : OXmlElementReader.CreateText()
//     json :
//     { Type: "Text", Text: (string), [PreserveSpace: true] }
//
//
//   add line
//     json :
//     { Type: "Line" }
//
//
//   add tabstop
//     json :
//     { Type: "TabStop" }
//
//
//   add simple field
//     function : OXmlElementReader.CreateSimpleField()
//     json :
//     { Type: "SimpleField", Instruction: (string) }
//     not treated SimpleField :
//       Dirty, FieldData, FieldLock
//
//
//   add document section
//     function : OXmlElementReader.CreateDocSection()
//     json :
//     {
//       Type: "DocSection",
//       [PageSize: { Width: 11907, Height: 16839 }],
//       [PageMargin: { Top: 720, Bottom: 720, Left: 1418, Right: 1418, Header: 284, Footer: 284 }],
//       [PageNumberStart: 1]
//     }
//     PageSize unit 1/20 point, 11907 1/20 point = 21 cm, 16839 1/20 point = 29.7 cm
//     PageMargin unit 1/20 point, 720 1/20 point = 0.6 cm, 1418 1/20 point = 1.27 cm, 284 1/20 point = 0.5 cm
//
//
//   add doc default run properties :
//     docx : styles.xml <w:styles>/<w:docDefaults>/<w:rPrDefault>
//     function : OXmlElementReader.CreateRunPropertiesDefault()
//     json :
//     {
//       Type: "DocDefaultsRunProperties",
//       RunFonts: { Ascii: "Arial", [AsciiTheme: (ThemeFontValues), ComplexScript: (string), ComplexScriptTheme: (ThemeFontValues), EastAsia: (string), EastAsiaTheme: (ThemeFontValues), HighAnsi: (string),
//                   HighAnsiTheme: (ThemeFontValues), Hint: (FontTypeHintValues)]  },
//       FontSize: "22"
//     }
//     ThemeFontValues : MajorEastAsia, MajorBidi, MajorAscii, MajorHighAnsi, MinorEastAsia, MinorBidi, MinorAscii, MinorHighAnsi
//     FontTypeHintValues : Default, EastAsia, ComplexScript
//
//   add doc default paragraph properties  :
//     docx : styles.xml <w:styles>/<w:docDefaults>/<w:pPrDefault />
//     function : OXmlElementReader.CreateParagraphPropertiesDefault()
//     json :
//     {
//       Type: "DocDefaultsParagraphProperties",
//     }
//
//   open header
//     function : OXmlElementReader.CreateOpenHeader()
//     json :
//     {
//       Type: "OpenHeader",
//       [HeaderType: "Default" | "First" | "Even" (HeaderFooterValues)],
//     }
//     WARNING even option dont work for google doc
//
//   open footer
//     function : OXmlElementReader.CreateOpenFooter()
//     json :
//     {
//       Type: "OpenFooter",
//       [FooterType: "Default" | "First" | "Even" (HeaderFooterValues)],
//     }
//     WARNING even option dont work for google doc
//
//   close header
//     function : OXmlElementReader.CreateCloseHeader()
//     json :
//     { Type: "CloseHeader" }
//
//   close header footer
//     function : OXmlElementReader.CreateCloseFooter()
//     json :
//     { Type: "CloseFooter" }
//
//
//   add paragraph style :
//     docx : styles.xml <w:styles>/<w:style>
//     function : OXmlStyleElementCreator.Create()
//     json :
//     {
//       Type: "Style",
//       Id: "TinyParagraph",
//       [Name: "TinyParagraph"],
//       StyleType: "Paragraph",
//       [Aliases: ""],
//       [CustomStyle: true],
//       [DefaultStyle: false],
//       [Locked: true],
//       [SemiHidden: true],
//       [StyleHidden: true],
//       [UnhideWhenUsed: true],
//       [UIPriority: (int)], 
//       [LinkedStyle: ""],
//       [BasedOn: ""],
//       [NextParagraphStyle: ""]
//     - paragraph properties :
//       StyleParagraphProperties: {
//         SpacingBetweenLines: {
//           [After: (string)],                                                               // Spacing Below Paragraph, <w:spacing w:after>, 1 line = 240
//           [AfterAutoSpacing: (bool)],                                                      // Automatically Determine Spacing Below Paragraph, <w:spacing w:afterAutospacing>
//           [AfterLines: (int)],                                                             // Spacing Below Paragraph in Line Units, <w:spacing w:afterLines>
//           [Before: (string)],                                                              // Spacing Above Paragraph, <w:spacing w:before>, 1 line = 240
//           [BeforeAutoSpacing: (bool)],                                                     // Automatically Determine Spacing Above Paragraph, <w:spacing w:beforeAutospacing>
//           [BeforeLines: (int)],                                                            // Spacing Above Paragraph IN Line Units, <w:spacing w:beforeLines>
//           [Line: (string)],                                                                // Spacing Between Lines in Paragraph, <w:spacing w:line>, 1 line = 240
//           [LineRule: (string, LineSpacingRuleValues)]                                      // Type of Spacing Between Lines, <w:spacing w:lineRule>, Auto, Exact, AtLeast
//         }
//         TextAlignment: (string, VerticalTextAlignmentValues)                               // TextAlignment, <w:textAlignment>, Top, Center, Baseline, Bottom, Auto
//         Tabs: [
//           {
//             Position: (int),                                                             // Tab Stop Position, <w:tab w:pos="">
//             Alignment: (string, TabStopValues),                                          // Tab Stop Type, <w:tab w:val="">, Clear, Left, Start, Center, Right, End, Decimal, Bar, Number
//             [LeaderChar: (string, TabStopLeaderCharValues)]                              // Tab Leader Character, <w:tab w:leader="">, None, Dot, Hyphen, Underscore, Heavy, MiddleDot
//           },
//           ...
//         ]
//       }
//     }
//
//     not treated StyleParagraphProperties :
//       AdjustRightIndent, AutoSpaceDE, AutoSpaceDN, BiDi, ContextualSpacing, FrameProperties, Indentation, Justification, KeepLines, KeepNext, Kinsoku, MirrorIndents, NumberingProperties,
//       OutlineLevel, OverflowPunctuation, PageBreakBefore, ParagraphBorders, ParagraphPropertiesChange, Shading, SnapToGrid, SuppressAutoHyphens, SuppressLineNumbers, SuppressOverlap,
//       TextBoxTightWrap, TextDirection, TopLinePunctuation, WidowControl, WordWrap
//     not treated StyleRunProperties :
//       Bold, BoldComplexScript, Border, Caps, CharacterScale, Color, DoubleStrike, EastAsianLayout, Emboss, Emphasis, FitText, FontSize, FontSizeComplexScript, Imprint, Italic,
//       ItalicComplexScript, Kern, Languages, override, NoProof, Outline, Position, RunFonts, RunPropertiesChange, Shading, Shadow, SmallCaps, SnapToGrid, Spacing, SpecVanish,
//       Strike, TextEffect, Underline, Vanish, VerticalTextAlignment, WebHidden
//
//
//   add picture
//     function : OXmlPictureElementCreator.CreatePicture()
//     json :
//     { Type: "Picture", File: "file.jpg", [Name: "image01"], [Description: "image"], [Width: 300], [Height: 300],
//       [Rotation: (int)], [HorizontalFlip: true], [VerticalFlip: true], [CompressionState: (A.BlipCompressionValues)], [PresetShape: (A.ShapeTypeValues)]
//
//       //////////////////DrawingMode: "Inline" | "Anchor",
//       DrawingType: "Inline" | "AnchorWrapNone", "AnchorWrapSquare", "AnchorWrapTight", "AnchorWrapThrough", "AnchorWrapTopAndBottom"
//
//     DrawingType Inline no parameters
//
//     DrawingType AnchorWrapNone, AnchorWrapThrough not implemented
//
//     DrawingType Anchor... parameters :
//       //////////////////WrapType: "WrapSquare" | "WrapTight" | "WrapTopAndBottom"
//       [AnchorId: (string)]
//       [EditId: (string)]
//       [DistanceFromTop: (int)]
//       [DistanceFromBottom: (int)]
//       [DistanceFromLeft: (int)]
//       [DistanceFromRight: (int)]
//       HorizontalRelativeFrom: (DW.HorizontalRelativePositionValues : Margin (default), Page, Column, Character, LeftMargin, RightMargin, InsideMargin, OutsideMargin)
//       [HorizontalPositionOffset: (int)]
//       [HorizontalAlignment (string : left, right, center, inside, outside)]
//       VerticalRelativeFrom (DW.VerticalRelativePositionValues : Paragraph (default), Page, Paragraph, Line, TopMargin, BottomMargin, InsideMargin, OutsideMargin)
//       [VerticalPositionOffset (int)]
//       [VerticalAlignment (string : top, bottom, center, inside, outside)]
//       [EffectExtent: { [TopEdge: (long)], [BottomEdge: (long)], [LeftEdge: (long)], [RightEdge: (long)] }]
//       [AllowOverlap: (bool)]
//       [BehindDoc: (bool)]
//       [Hidden: (bool)]
//       [LayoutInCell: (bool)]
//       [Locked: (bool)]
//       [RelativeHeight: (bool)]
//       [SimplePosition: { X: (long), Y: (long) }]
//
//     DrawingType AnchorWrapSquare parameters :
//       WrapSquare: {
//         WrapText: (DW.WrapTextValues : BothSides (default), Left, Right, Largest)
//         [DistanceFromTop: (int)]
//         [DistanceFromBottom: (int)]
//         [DistanceFromLeft: (int)]
//         [DistanceFromRight: (int)]
//         [EffectExtent: { [TopEdge: (long)], [BottomEdge: (long)], [LeftEdge: (long)], [RightEdge: (long)] }]
//       }
//
//     DrawingType AnchorWrapTight parameters :
//       WrapTight {
//         WrapText: (DW.WrapTextValues.BothSides)
//         [DistanceFromLeft: (int)]
//         [DistanceFromRight: (int)]
//         [Polygon: { [StartPointX: (long)], [StartPointY: (long)], LinesTo: [ { X: (long), Y: (long) }, ...], [Edited: (bool)] }]
//         [Square: { [StartPointX: (long)], [StartPointY: (long)], HorizontalSize: (long), VerticalSize: (long), [Edited: (bool)] }]
//       //////////////////[WrapPolygonHorizontalSize: (int)]
//       //////////////////[WrapPolygonVerticalSize: (int)]
//       //////////////////[WrapPolygonStartPointX: (int)]
//       //////////////////[WrapPolygonStartPointY: (int)]
//       }
//
//     DrawingType AnchorWrapTopAndBottom parameters :
//       WrapTopAndBottom {
//         [DistanceFromTop: (int)]
//         [DistanceFromBottom: (int)]
//         [EffectExtent: { [TopEdge: (long)], [BottomEdge: (long)], [LeftEdge: (long)], [RightEdge: (long)] }]
//       }

namespace pb.Data.Mongo.Serializers
{
    // from BsonValueSerializer
    // IBsonDocumentSerializer -> GetMemberSerializationInfo()
    // IBsonArraySerializer    -> GetItemSerializationInfo()
    public class OXmlElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlElementSerializer _instance = new OXmlElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlElementSerializer Instance { get { return _instance; } }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        // actualType ignored
        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("OXmlElementSerializer.Deserialize()");

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            string type = bsonReader.FindStringElement("Type");
            OXmlElement value;
            switch (type.ToLower())
            {
                case "line":
                    value = new OXmlElement { Type = OXmlElementType.Line };
                    break;
                case "tabstop":
                    value = new OXmlElement { Type = OXmlElementType.TabStop };
                    break;
                case "paragraph":
                    //value = OXmlParagraphElementSerializer.Instance.Deserialize(bsonReader, typeof(OXmlParagraphElement), options);
                    value = OXmlParagraphElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "break":
                    value = OXmlBreakElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "text":
                    value = OXmlTextElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "simplefield":
                    value = OXmlSimpleFieldElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "docsection":
                    value = OXmlDocSectionElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "docdefaultsrunproperties":
                    value = OXmlDocDefaultsRunPropertiesElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "docdefaultsparagraphproperties":
                    //value = new OXmlDocDefaultsParagraphPropertiesElement();
                    value = new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
                    break;
                case "openheader":
                    value = OXmlOpenHeaderElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "closeheader":
                    value = new OXmlElement { Type = OXmlElementType.CloseHeader };
                    break;
                case "openfooter":
                    value = OXmlOpenFooterElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "closefooter":
                    value = new OXmlElement { Type = OXmlElementType.CloseFooter };
                    break;
                case "style":
                    value = OXmlStyleElementSerializer.Instance._Deserialize(bsonReader, options);
                    break;
                case "picture":
                    //value = OXmlPictureElementSerializer.Instance._Deserialize(bsonReader, options);
                    value = OXmlPictureElementCreator.CreatePicture(zMongo.ReadBsonDocumentWOStartEnd(bsonReader));
                    break;
                default:
                    throw new PBException($"deserialize OXmlElement, invalid Type {type}.");
            }
            bsonReader.ReadEndDocument();
            return value;


            //switch (bsonType)
            //{
            //    case BsonType.Array: return (ZValue)ZStringArraySerializer.Instance.Deserialize(bsonReader, typeof(ZStringArray), options);
            //    //case BsonType.Binary: return (BsonValue)BsonBinaryDataSerializer.Instance.Deserialize(bsonReader, typeof(BsonBinaryData), options);
            //    //case BsonType.Boolean: return (BsonValue)BsonBooleanSerializer.Instance.Deserialize(bsonReader, typeof(BsonBoolean), options);
            //    //case BsonType.DateTime: return (BsonValue)BsonDateTimeSerializer.Instance.Deserialize(bsonReader, typeof(BsonDateTime), options);
            //    //case BsonType.Document: return (BsonValue)BsonDocumentSerializer.Instance.Deserialize(bsonReader, typeof(BsonDocument), options);
            //    //case BsonType.Double: return (BsonValue)BsonDoubleSerializer.Instance.Deserialize(bsonReader, typeof(BsonDouble), options);
            //    case BsonType.Int32: return (ZValue)ZIntSerializer.Instance.Deserialize(bsonReader, typeof(ZInt), options);
            //    //case BsonType.Int64: return (BsonValue)BsonInt64Serializer.Instance.Deserialize(bsonReader, typeof(BsonInt64), options);
            //    //case BsonType.JavaScript: return (BsonValue)BsonJavaScriptSerializer.Instance.Deserialize(bsonReader, typeof(BsonJavaScript), options);
            //    //case BsonType.JavaScriptWithScope: return (BsonValue)BsonJavaScriptWithScopeSerializer.Instance.Deserialize(bsonReader, typeof(BsonJavaScriptWithScope), options);
            //    //case BsonType.MaxKey: return (BsonValue)BsonMaxKeySerializer.Instance.Deserialize(bsonReader, typeof(BsonMaxKey), options);
            //    //case BsonType.MinKey: return (BsonValue)BsonMinKeySerializer.Instance.Deserialize(bsonReader, typeof(BsonMinKey), options);
            //    //case BsonType.Null: return (BsonValue)BsonNullSerializer.Instance.Deserialize(bsonReader, typeof(BsonNull), options);
            //    case BsonType.Document:
            //        return ReadZValueFromBsonDocument(bsonReader);
            //    case BsonType.Null:
            //        bsonReader.ReadNull();
            //        return null;
            //    //case BsonType.ObjectId: return (BsonValue)BsonObjectIdSerializer.Instance.Deserialize(bsonReader, typeof(BsonObjectId), options);
            //    //case BsonType.RegularExpression: return (BsonValue)BsonRegularExpressionSerializer.Instance.Deserialize(bsonReader, typeof(BsonRegularExpression), options);
            //    //case BsonType.String: return (BsonValue)BsonStringSerializer.Instance.Deserialize(bsonReader, typeof(BsonString), options);
            //    case BsonType.String: return (ZValue)ZStringSerializer.Instance.Deserialize(bsonReader, typeof(ZString), options);
            //    // ZStringSerializer
            //    //case BsonType.Symbol: return (BsonValue)BsonSymbolSerializer.Instance.Deserialize(bsonReader, typeof(BsonSymbol), options);
            //    //case BsonType.Timestamp: return (BsonValue)BsonTimestampSerializer.Instance.Deserialize(bsonReader, typeof(BsonTimestamp), options);
            //    //case BsonType.Undefined: return (BsonValue)BsonUndefinedSerializer.Instance.Deserialize(bsonReader, typeof(BsonUndefined), options);
            //    default:
            //        //var message = string.Format("Invalid BsonType {0}.", bsonType);
            //        //throw new BsonInternalException(message);
            //        throw new PBException("error deserialize ZValue, invalid BsonType {0}.", bsonType);
            //}
        }

        /// <summary>
        /// Gets the serialization info for a member.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>
        /// The serialization info for the member.
        /// </returns>
        //public BsonSerializationInfo GetMemberSerializationInfo(string memberName)
        //{
        //    return new BsonSerializationInfo(memberName, Instance, typeof(BsonValue), Instance.GetDefaultSerializationOptions());
        //}

        /// <summary>
        /// Gets the serialization info for individual items of the array.
        /// </summary>
        /// <returns>
        /// The serialization info for the items.
        /// </returns>
        //public BsonSerializationInfo GetItemSerializationInfo()
        //{
        //    return new BsonSerializationInfo(null, Instance, typeof(BsonValue), Instance.GetDefaultSerializationOptions());
        //}

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlElementSerializer.Serialize()");

            bsonWriter.WriteStartDocument();
            switch (((OXmlElement)value).Type)
            {
                case OXmlElementType.Line:
                    bsonWriter.WriteString("Type", "Line");
                    break;
                case OXmlElementType.TabStop:
                    bsonWriter.WriteString("Type", "TabStop");
                    break;
                case OXmlElementType.DocDefaultsParagraphProperties:
                    bsonWriter.WriteString("Type", "DocDefaultsParagraphProperties");
                    break;
                case OXmlElementType.CloseHeader:
                    bsonWriter.WriteString("Type", "CloseHeader");
                    break;
                case OXmlElementType.CloseFooter:
                    bsonWriter.WriteString("Type", "CloseFooter");
                    break;
                default:
                    throw new PBException($"serializer OXmlElement type {((OXmlElement)value).Type} not implemented");
            }
            bsonWriter.WriteEndDocument();
        }
    }
}

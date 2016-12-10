using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using pb.Data.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace pb.Data.Mongo.Serializers
{
    public class OXmlStyleElementSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;
        private static OXmlStyleElementSerializer _instance = new OXmlStyleElementSerializer();

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public static OXmlStyleElementSerializer Instance { get { return _instance; } }

        public OXmlStyleElement _Deserialize(BsonReader bsonReader, IBsonSerializationOptions options)
        {
            OXmlStyleElement element = new OXmlStyleElement();
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
                        if (type.ToLower() != "style")
                            throw new PBException($"invalid Type {type} when deserialize OXmlStyleElement");
                        break;
                    case "id":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong Id value {bsonType}");
                        element.Id = bsonReader.ReadString();
                        break;
                    case "name":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong Name value {bsonType}");
                        element.Name = bsonReader.ReadString();
                        break;
                    case "styletype":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleType value {bsonType}");
                        element.StyleType = bsonReader.ReadString().zParseEnum<StyleValues>(ignoreCase: true);
                        break;
                    case "aliases":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong Aliases value {bsonType}");
                        element.Aliases = bsonReader.ReadString();
                        break;
                    case "customstyle":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong CustomStyle value {bsonType}");
                        element.CustomStyle = bsonReader.ReadBoolean();
                        break;
                    case "defaultstyle":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong DefaultStyle value {bsonType}");
                        element.DefaultStyle = bsonReader.ReadBoolean();
                        break;
                    case "locked":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong Locked value {bsonType}");
                        element.Locked = bsonReader.ReadBoolean();
                        break;
                    case "semihidden":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong SemiHidden value {bsonType}");
                        element.SemiHidden = bsonReader.ReadBoolean();
                        break;
                    case "stylehidden":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleHidden value {bsonType}");
                        element.StyleHidden = bsonReader.ReadBoolean();
                        break;
                    case "unhidewhenused":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong UnhideWhenUsed value {bsonType}");
                        element.UnhideWhenUsed = bsonReader.ReadBoolean();
                        break;
                    case "uipriority":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong UIPriority value {bsonType}");
                        element.UIPriority = bsonReader.ReadInt32();
                        break;
                    case "linkedstyle":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong LinkedStyle value {bsonType}");
                        element.LinkedStyle = bsonReader.ReadString();
                        break;
                    case "basedon":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong BasedOn value {bsonType}");
                        element.BasedOn = bsonReader.ReadString();
                        break;
                    case "nextparagraphstyle":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong NextParagraphStyle value {bsonType}");
                        element.NextParagraphStyle = bsonReader.ReadString();
                        break;
                    case "styleparagraphproperties":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong StyleParagraphProperties value {bsonType}");
                        element.StyleParagraphProperties = ReadStyleParagraphProperties(bsonReader);
                        break;
                    case "stylerunproperties":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong StyleRunProperties value {bsonType}");
                        element.StyleRunProperties = ReadStyleRunProperties(bsonReader);
                        break;
                    default:
                        throw new PBException($"unknow Style value \"{name}\"");
                }
            }
            return element;
        }

        private static OXmlStyleParagraphProperties ReadStyleParagraphProperties(BsonReader bsonReader)
        {
            bsonReader.ReadStartDocument();
            OXmlStyleParagraphProperties value = new OXmlStyleParagraphProperties();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "adjustrightindent":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties AdjustRightIndent value {bsonType}");
                        value.AdjustRightIndent = bsonReader.ReadBoolean();
                        break;
                    case "autospacede":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties AutoSpaceDE value {bsonType}");
                        value.AutoSpaceDE = bsonReader.ReadBoolean();
                        break;
                    case "autospacedn":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties AutoSpaceDN value {bsonType}");
                        value.AutoSpaceDN = bsonReader.ReadBoolean();
                        break;
                    case "bidi":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties BiDi value {bsonType}");
                        value.BiDi = bsonReader.ReadBoolean();
                        break;
                    case "contextualspacing":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties ContextualSpacing value {bsonType}");
                        value.ContextualSpacing = bsonReader.ReadBoolean();
                        break;
                    case "justification":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleParagraphProperties Justification value {bsonType}");
                        value.Justification = bsonReader.ReadString().zParseEnum<JustificationValues>(ignoreCase: true);
                        break;
                    case "keeplines":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties KeepLines value {bsonType}");
                        value.KeepLines = bsonReader.ReadBoolean();
                        break;
                    case "keepnext":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties KeepNext value {bsonType}");
                        value.KeepNext = bsonReader.ReadBoolean();
                        break;
                    case "kinsoku":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties Kinsoku value {bsonType}");
                        value.Kinsoku = bsonReader.ReadBoolean();
                        break;
                    case "mirrorindents":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties MirrorIndents value {bsonType}");
                        value.MirrorIndents = bsonReader.ReadBoolean();
                        break;
                    case "outlinelevel":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong StyleParagraphProperties OutlineLevel value {bsonType}");
                        value.OutlineLevel = bsonReader.ReadInt32();
                        break;
                    case "overflowpunctuation":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties OverflowPunctuation value {bsonType}");
                        value.OverflowPunctuation = bsonReader.ReadBoolean();
                        break;
                    case "pagebreakbefore":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties PageBreakBefore value {bsonType}");
                        value.PageBreakBefore = bsonReader.ReadBoolean();
                        break;
                    case "snaptogrid":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties SnapToGrid value {bsonType}");
                        value.SnapToGrid = bsonReader.ReadBoolean();
                        break;
                    case "spacingbetweenlines":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong StyleParagraphProperties SpacingBetweenLines value {bsonType}");
                        value.SpacingBetweenLines = ReadSpacingBetweenLines(bsonReader);
                        break;
                    case "suppressautohyphens":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties SuppressAutoHyphens value {bsonType}");
                        value.SuppressAutoHyphens = bsonReader.ReadBoolean();
                        break;
                    case "suppresslinenumbers":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties SuppressLineNumbers value {bsonType}");
                        value.SuppressLineNumbers = bsonReader.ReadBoolean();
                        break;
                    case "suppressoverlap":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties SuppressOverlap value {bsonType}");
                        value.SuppressOverlap = bsonReader.ReadBoolean();
                        break;
                    case "tabs":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Array)
                            throw new PBException($"wrong StyleParagraphProperties Tabs value {bsonType}");
                        value.Tabs = ReadTabs(bsonReader);
                        break;
                    case "textalignment":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleParagraphProperties TextAlignment value {bsonType}");
                        value.TextAlignment = bsonReader.ReadString().zParseEnum<VerticalTextAlignmentValues>(ignoreCase: true);
                        break;
                    case "textboxtightwrap":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleParagraphProperties TextBoxTightWrap value {bsonType}");
                        value.TextBoxTightWrap = bsonReader.ReadString().zParseEnum<TextBoxTightWrapValues>(ignoreCase: true);
                        break;
                    case "textdirection":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleParagraphProperties TextDirection value {bsonType}");
                        value.TextDirection = bsonReader.ReadString().zParseEnum<TextDirectionValues>(ignoreCase: true);
                        break;
                    case "toplinepunctuation":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties TopLinePunctuation value {bsonType}");
                        value.TopLinePunctuation = bsonReader.ReadBoolean();
                        break;
                    case "widowcontrol":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties WidowControl value {bsonType}");
                        value.WidowControl = bsonReader.ReadBoolean();
                        break;
                    case "wordwrap":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleParagraphProperties WordWrap value {bsonType}");
                        value.WordWrap = bsonReader.ReadBoolean();
                        break;
                    default:
                        throw new PBException($"unknow PageSize value \"{name}\"");
                }
            }
            bsonReader.ReadEndDocument();
            return value;
        }

        private static OXmlSpacingBetweenLines ReadSpacingBetweenLines(BsonReader bsonReader)
        {
            bsonReader.ReadStartDocument();
            OXmlSpacingBetweenLines value = new OXmlSpacingBetweenLines();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "after":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong SpacingBetweenLines After value {bsonType}");
                        value.After = bsonReader.ReadString();
                        break;
                    case "afterautospacing":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong SpacingBetweenLines AfterAutoSpacing value {bsonType}");
                        value.AfterAutoSpacing = bsonReader.ReadBoolean();
                        break;
                    case "afterlines":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong SpacingBetweenLines AfterLines value {bsonType}");
                        value.AfterLines = bsonReader.ReadInt32();
                        break;
                    case "before":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong SpacingBetweenLines Before value {bsonType}");
                        value.Before = bsonReader.ReadString();
                        break;
                    case "beforeautospacing":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong SpacingBetweenLines BeforeAutoSpacing value {bsonType}");
                        value.BeforeAutoSpacing = bsonReader.ReadBoolean();
                        break;
                    case "beforelines":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong SpacingBetweenLines BeforeLines value {bsonType}");
                        value.BeforeLines = bsonReader.ReadInt32();
                        break;
                    case "line":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong SpacingBetweenLines Line value {bsonType}");
                        value.Line = bsonReader.ReadString();
                        break;
                    default:
                        throw new PBException($"unknow PageMargin value \"{name}\"");
                }
            }
            bsonReader.ReadEndDocument();
            return value;
        }

        private static OXmlTabStop[] ReadTabs(BsonReader bsonReader)
        {
            bsonReader.ReadStartArray();
            List<OXmlTabStop> values = new List<OXmlTabStop>();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;

                if (bsonType != BsonType.Document)
                    throw new PBException($"wrong Tabs value {bsonType}");
                bsonReader.ReadStartDocument();

                OXmlTabStop value = new OXmlTabStop();
                while (true)
                {
                    bsonType = bsonReader.ReadBsonType();
                    if (bsonType == BsonType.EndOfDocument)
                        break;
                    string name = bsonReader.ReadName();
                    switch (name.ToLower())
                    {
                        case "position":
                            if (bsonType == BsonType.Null)
                                break;
                            if (bsonType != BsonType.Int32)
                                throw new PBException($"wrong TabStop Position value {bsonType}");
                            value.Position = bsonReader.ReadInt32();
                            break;
                        case "alignment":
                            if (bsonType == BsonType.Null)
                                break;
                            if (bsonType != BsonType.String)
                                throw new PBException($"wrong TabStop Alignment value {bsonType}");
                            value.Alignment = bsonReader.ReadString().zParseEnum<TabStopValues>(ignoreCase: true);
                            break;
                        case "leaderchar":
                            if (bsonType == BsonType.Null)
                                break;
                            if (bsonType != BsonType.String)
                                throw new PBException($"wrong TabStop LeaderChar value {bsonType}");
                            value.LeaderChar = bsonReader.ReadString().zParseEnum<TabStopLeaderCharValues>(ignoreCase: true);
                            break;
                        default:
                            throw new PBException($"unknow TabStop value \"{name}\"");
                    }
                }
                bsonReader.ReadEndDocument();
                values.Add(value);
            }
            bsonReader.ReadEndArray();
            return values.ToArray();
        }

        private static OXmlStyleRunProperties ReadStyleRunProperties(BsonReader bsonReader)
        {
            bsonReader.ReadStartDocument();
            OXmlStyleRunProperties value = new OXmlStyleRunProperties();
            while (true)
            {
                BsonType bsonType = bsonReader.ReadBsonType();
                if (bsonType == BsonType.EndOfDocument)
                    break;
                string name = bsonReader.ReadName();
                switch (name.ToLower())
                {
                    case "bold":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Bold value {bsonType}");
                        value.Bold = bsonReader.ReadBoolean();
                        break;
                    case "boldcomplexscript":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties BoldComplexScript value {bsonType}");
                        value.BoldComplexScript = bsonReader.ReadBoolean();
                        break;
                    case "caps":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Caps value {bsonType}");
                        value.Caps = bsonReader.ReadBoolean();
                        break;
                    case "characterscale":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong StyleRunProperties CharacterScale value {bsonType}");
                        value.CharacterScale = bsonReader.ReadInt32();
                        break;
                    case "doublestrike":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties DoubleStrike value {bsonType}");
                        value.DoubleStrike = bsonReader.ReadBoolean();
                        break;
                    case "emboss":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Emboss value {bsonType}");
                        value.Emboss = bsonReader.ReadBoolean();
                        break;
                    case "emphasis":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleRunProperties Emphasis value {bsonType}");
                        value.Emphasis = bsonReader.ReadString().zParseEnum<EmphasisMarkValues>(ignoreCase: true);
                        break;
                    case "fontsize":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleRunProperties FontSize value {bsonType}");
                        value.FontSize = bsonReader.ReadString();
                        break;
                    case "fontsizecomplexscript":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleRunProperties FontSizeComplexScript value {bsonType}");
                        value.FontSizeComplexScript = bsonReader.ReadString();
                        break;
                    case "imprint":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Imprint value {bsonType}");
                        value.Imprint = bsonReader.ReadBoolean();
                        break;
                    case "italic":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Italic value {bsonType}");
                        value.Italic = bsonReader.ReadBoolean();
                        break;
                    case "italiccomplexscript":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties ItalicComplexScript value {bsonType}");
                        value.ItalicComplexScript = bsonReader.ReadBoolean();
                        break;
                    case "kern":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong StyleRunProperties Kern value {bsonType}");
                        value.Kern = (uint)bsonReader.ReadInt32();
                        break;
                    case "noproof":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties NoProof value {bsonType}");
                        value.NoProof = bsonReader.ReadBoolean();
                        break;
                    case "outline":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Outline value {bsonType}");
                        value.Outline = bsonReader.ReadBoolean();
                        break;
                    case "position":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleRunProperties Position value {bsonType}");
                        value.Position = bsonReader.ReadString();
                        break;
                    case "runfonts":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Document)
                            throw new PBException($"wrong RunFonts value {bsonType}");
                        value.RunFonts = OXmlCommonSerializer.ReadRunFonts(bsonReader);
                        break;
                    case "shadow":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Shadow value {bsonType}");
                        value.Shadow = bsonReader.ReadBoolean();
                        break;
                    case "smallcaps":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties SmallCaps value {bsonType}");
                        value.SmallCaps = bsonReader.ReadBoolean();
                        break;
                    case "snaptogrid":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties SnapToGrid value {bsonType}");
                        value.SnapToGrid = bsonReader.ReadBoolean();
                        break;
                    case "spacing":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Int32)
                            throw new PBException($"wrong StyleRunProperties Spacing value {bsonType}");
                        value.Spacing = bsonReader.ReadInt32();
                        break;
                    case "specvanish":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties SpecVanish value {bsonType}");
                        value.SpecVanish = bsonReader.ReadBoolean();
                        break;
                    case "strike":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Strike value {bsonType}");
                        value.Strike = bsonReader.ReadBoolean();
                        break;
                    case "texteffect":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleRunProperties TextEffect value {bsonType}");
                        value.TextEffect = bsonReader.ReadString().zParseEnum<TextEffectValues>(ignoreCase: true);
                        break;
                    case "vanish":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties Vanish value {bsonType}");
                        value.Vanish = bsonReader.ReadBoolean();
                        break;
                    case "verticaltextalignment":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.String)
                            throw new PBException($"wrong StyleRunProperties VerticalTextAlignment value {bsonType}");
                        value.VerticalTextAlignment = bsonReader.ReadString().zParseEnum<VerticalPositionValues>(ignoreCase: true);
                        break;
                    case "webhidden":
                        if (bsonType == BsonType.Null)
                            break;
                        if (bsonType != BsonType.Boolean)
                            throw new PBException($"wrong StyleRunProperties WebHidden value {bsonType}");
                        value.WebHidden = bsonReader.ReadBoolean();
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
                pb.Trace.WriteLine("OXmlStyleElementSerializer.Deserialize()");

            VerifyTypes(nominalType, actualType, typeof(OXmlStyleElement));

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType != BsonType.Document)
                throw new PBException($"deserialize OXmlStyleElement, invalid BsonType {bsonType}.");
            bsonReader.ReadStartDocument();
            object value = _Deserialize(bsonReader, options);
            bsonReader.ReadEndDocument();
            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                throw new PBException("serialize OXmlStyleElement value is null");
            if (_trace)
                pb.Trace.WriteLine("OXmlStyleElementSerializer.Serialize()");

            OXmlStyleElement element = (OXmlStyleElement)value;
            bsonWriter.WriteStartDocument();

            bsonWriter.WriteString("Type", "Style");

            bsonWriter.WriteString("Id", element.Id);
            if (element.Name != null)
                bsonWriter.WriteString("Name", element.Name);
            bsonWriter.WriteString("StyleType", element.StyleType.ToString());
            if (element.Aliases != null)
                bsonWriter.WriteString("Aliases", element.Aliases);
            bsonWriter.WriteBoolean("CustomStyle", element.CustomStyle);
            bsonWriter.WriteBoolean("DefaultStyle", element.DefaultStyle);
            if (element.Locked != null)
                bsonWriter.WriteBoolean("Locked", (bool)element.Locked);
            if (element.SemiHidden != null)
                bsonWriter.WriteBoolean("SemiHidden", (bool)element.SemiHidden);
            if (element.StyleHidden != null)
                bsonWriter.WriteBoolean("StyleHidden", (bool)element.StyleHidden);
            if (element.UnhideWhenUsed != null)
                bsonWriter.WriteBoolean("UnhideWhenUsed", (bool)element.UnhideWhenUsed);
            if (element.UIPriority != null)
                bsonWriter.WriteInt32("UIPriority", (int)element.UIPriority);
            if (element.LinkedStyle != null)
                bsonWriter.WriteString("LinkedStyle", element.LinkedStyle);
            if (element.BasedOn != null)
                bsonWriter.WriteString("BasedOn", element.BasedOn);
            if (element.NextParagraphStyle != null)
                bsonWriter.WriteString("NextParagraphStyle", element.NextParagraphStyle);
            if (element.StyleParagraphProperties != null)
            {
                bsonWriter.WriteStartDocument("StyleParagraphProperties");
                WriteStyleParagraphProperties(bsonWriter, element.StyleParagraphProperties);
                bsonWriter.WriteEndDocument();
            }
            if (element.StyleRunProperties != null)
            {
                bsonWriter.WriteStartDocument("StyleRunProperties");
                WriteStyleRunProperties(bsonWriter, element.StyleRunProperties);
                bsonWriter.WriteEndDocument();
            }
            bsonWriter.WriteEndDocument();
        }

        private static void WriteStyleParagraphProperties(BsonWriter bsonWriter, OXmlStyleParagraphProperties element)
        {
            if (element.AdjustRightIndent != null)
                bsonWriter.WriteBoolean("AdjustRightIndent", (bool)element.AdjustRightIndent);
            if (element.AutoSpaceDE != null)
                bsonWriter.WriteBoolean("AutoSpaceDE", (bool)element.AutoSpaceDE);
            if (element.AutoSpaceDN != null)
                bsonWriter.WriteBoolean("AutoSpaceDN", (bool)element.AutoSpaceDN);
            if (element.BiDi != null)
                bsonWriter.WriteBoolean("BiDi", (bool)element.BiDi);
            if (element.ContextualSpacing != null)
                bsonWriter.WriteBoolean("ContextualSpacing", (bool)element.ContextualSpacing);
            if (element.Justification != null)
                bsonWriter.WriteString("Justification", element.Justification.ToString());
            if (element.KeepLines != null)
                bsonWriter.WriteBoolean("KeepLines", (bool)element.KeepLines);
            if (element.KeepNext != null)
                bsonWriter.WriteBoolean("KeepNext", (bool)element.KeepNext);
            if (element.Kinsoku != null)
                bsonWriter.WriteBoolean("Kinsoku", (bool)element.Kinsoku);
            if (element.MirrorIndents != null)
                bsonWriter.WriteBoolean("MirrorIndents", (bool)element.MirrorIndents);
            if (element.OutlineLevel != null)
                bsonWriter.WriteInt32("OutlineLevel", (int)element.OutlineLevel);
            if (element.OverflowPunctuation != null)
                bsonWriter.WriteBoolean("OverflowPunctuation", (bool)element.OverflowPunctuation);
            if (element.PageBreakBefore != null)
                bsonWriter.WriteBoolean("PageBreakBefore", (bool)element.PageBreakBefore);
            if (element.SnapToGrid != null)
                bsonWriter.WriteBoolean("SnapToGrid", (bool)element.SnapToGrid);
            if (element.SpacingBetweenLines != null)
            {
                bsonWriter.WriteStartDocument("SpacingBetweenLines");
                WriteSpacingBetweenLines(bsonWriter, element.SpacingBetweenLines);
                bsonWriter.WriteEndDocument();
            }
            if (element.SuppressAutoHyphens != null)
                bsonWriter.WriteBoolean("SuppressAutoHyphens", (bool)element.SuppressAutoHyphens);
            if (element.SuppressLineNumbers != null)
                bsonWriter.WriteBoolean("SuppressLineNumbers", (bool)element.SuppressLineNumbers);
            if (element.SuppressOverlap != null)
                bsonWriter.WriteBoolean("SuppressOverlap", (bool)element.SuppressOverlap);
            if (element.Tabs != null)
            {
                bsonWriter.WriteStartArray("Tabs");
                WriteTabs(bsonWriter, element.Tabs);
                bsonWriter.WriteEndArray();
            }
            if (element.TextAlignment != null)
                bsonWriter.WriteString("TextAlignment", element.TextAlignment.ToString());
            if (element.TextBoxTightWrap != null)
                bsonWriter.WriteString("TextBoxTightWrap", element.TextBoxTightWrap.ToString());
            if (element.TextDirection != null)
                bsonWriter.WriteString("TextDirection", element.TextDirection.ToString());
            if (element.TopLinePunctuation != null)
                bsonWriter.WriteBoolean("TopLinePunctuation", (bool)element.TopLinePunctuation);
            if (element.WidowControl != null)
                bsonWriter.WriteBoolean("WidowControl", (bool)element.WidowControl);
            if (element.WordWrap != null)
                bsonWriter.WriteBoolean("WordWrap", (bool)element.WordWrap);
        }

        private static void WriteSpacingBetweenLines(BsonWriter bsonWriter, OXmlSpacingBetweenLines element)
        {
            if (element.After != null)
                bsonWriter.WriteString("After", element.After);
            if (element.AfterAutoSpacing != null)
                bsonWriter.WriteBoolean("AfterAutoSpacing", (bool)element.AfterAutoSpacing);
            if (element.AfterLines != null)
                bsonWriter.WriteInt32("AfterLines", (int)element.AfterLines);
            if (element.Before != null)
                bsonWriter.WriteString("Before", element.Before);
            if (element.BeforeAutoSpacing != null)
                bsonWriter.WriteBoolean("BeforeAutoSpacing", (bool)element.BeforeAutoSpacing);
            if (element.BeforeLines != null)
                bsonWriter.WriteInt32("BeforeLines", (int)element.BeforeLines);
            if (element.Line != null)
                bsonWriter.WriteString("Line", element.Line);
            if (element.LineRule != null)
                bsonWriter.WriteString("LineRule", element.LineRule.ToString());
        }

        private static void WriteTabs(BsonWriter bsonWriter, OXmlTabStop[] tabs)
        {
            foreach (OXmlTabStop tab in tabs)
            {
                bsonWriter.WriteStartDocument();
                WriteTab(bsonWriter, tab);
                bsonWriter.WriteEndDocument();
            }
        }

        private static void WriteTab(BsonWriter bsonWriter, OXmlTabStop element)
        {
            bsonWriter.WriteInt32("Position", element.Position);
            bsonWriter.WriteString("Alignment", element.Alignment.ToString());
            if (element.LeaderChar != TabStopLeaderCharValues.None)
                bsonWriter.WriteString("LeaderChar", element.LeaderChar.ToString());
        }

        private static void WriteStyleRunProperties(BsonWriter bsonWriter, OXmlStyleRunProperties element)
        {
            if (element.Bold != null)
                bsonWriter.WriteBoolean("Bold", (bool)element.Bold);
            if (element.BoldComplexScript != null)
                bsonWriter.WriteBoolean("BoldComplexScript", (bool)element.BoldComplexScript);
            if (element.Caps != null)
                bsonWriter.WriteBoolean("Caps", (bool)element.Caps);
            if (element.CharacterScale != null)
                bsonWriter.WriteInt32("CharacterScale", (int)element.CharacterScale);
            if (element.DoubleStrike != null)
                bsonWriter.WriteBoolean("DoubleStrike", (bool)element.DoubleStrike);
            if (element.Emboss != null)
                bsonWriter.WriteBoolean("Emboss", (bool)element.Emboss);
            if (element.Emphasis != null)
                bsonWriter.WriteString("Emphasis", element.Emphasis.ToString());
            if (element.FontSize != null)
                bsonWriter.WriteString("FontSize", element.FontSize);
            if (element.FontSizeComplexScript != null)
                bsonWriter.WriteString("FontSizeComplexScript", element.FontSizeComplexScript);
            if (element.Imprint != null)
                bsonWriter.WriteBoolean("Imprint", (bool)element.Imprint);
            if (element.Italic != null)
                bsonWriter.WriteBoolean("Italic", (bool)element.Italic);
            if (element.ItalicComplexScript != null)
                bsonWriter.WriteBoolean("ItalicComplexScript", (bool)element.ItalicComplexScript);
            if (element.Kern != null)
                bsonWriter.WriteInt32("Kern", (int)element.Kern);
            if (element.NoProof != null)
                bsonWriter.WriteBoolean("NoProof", (bool)element.NoProof);
            if (element.Outline != null)
                bsonWriter.WriteBoolean("Outline", (bool)element.Outline);
            if (element.Position != null)
                bsonWriter.WriteString("Position", element.Position);
            if (element.RunFonts != null)
            {
                bsonWriter.WriteStartDocument("RunFonts");
                OXmlCommonSerializer.WriteRunFonts(bsonWriter, element.RunFonts);
                bsonWriter.WriteEndDocument();
            }
            if (element.Shadow != null)
                bsonWriter.WriteBoolean("Shadow", (bool)element.Shadow);
            if (element.SmallCaps != null)
                bsonWriter.WriteBoolean("SmallCaps", (bool)element.SmallCaps);
            if (element.SnapToGrid != null)
                bsonWriter.WriteBoolean("SnapToGrid", (bool)element.SnapToGrid);
            if (element.Spacing != null)
                bsonWriter.WriteInt32("Spacing", (int)element.Spacing);
            if (element.SpecVanish != null)
                bsonWriter.WriteBoolean("SpecVanish", (bool)element.SpecVanish);
            if (element.Strike != null)
                bsonWriter.WriteBoolean("Strike", (bool)element.Strike);
            if (element.TextEffect != null)
                bsonWriter.WriteString("TextEffect", element.TextEffect.ToString());
            if (element.Vanish != null)
                bsonWriter.WriteBoolean("Vanish", (bool)element.Vanish);
            if (element.VerticalTextAlignment != null)
                bsonWriter.WriteString("VerticalTextAlignment", element.VerticalTextAlignment.ToString());
            if (element.WebHidden != null)
                bsonWriter.WriteBoolean("WebHidden", (bool)element.WebHidden);
        }
    }
}

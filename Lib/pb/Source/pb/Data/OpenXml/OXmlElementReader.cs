using System.Collections.Generic;
using MongoDB.Bson;
using pb.Data.Mongo;
using DocumentFormat.OpenXml.Wordprocessing;
using MongoDB.Bson.IO;
using System.Text;

// todo :
//   add include file

namespace pb.Data.OpenXml
{
    // doc :
    //
    //   add paragraph
    //     function : OXmlElementReader.CreateParagraph()
    //     json :
    //     { Type: "Paragraph", Style: (string) }
    //
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
    //       HorizontalRelativeFrom: (DW.HorizontalRelativePositionValues : Margin (default), Page, Column, Character, LeftMargin, RightMargin, InsideMargin, OutsideMargin)
    //       [HorizontalPositionOffset: (int)]
    //       [HorizontalAlignment (string : left, right, center, inside, outside)]
    //       VerticalRelativeFrom (DW.VerticalRelativePositionValues : Paragraph (default), Page, Paragraph, Line, TopMargin, BottomMargin, InsideMargin, OutsideMargin)
    //       [VerticalPositionOffset (int)]
    //       [VerticalAlignment (string : top, bottom, center, inside, outside)]
    //
    //     DrawingType AnchorWrapSquare parameters :
    //       WrapText: (DW.WrapTextValues : BothSides (default), Left, Right, Largest)
    //       [DistanceFromTop: (int)]
    //       [DistanceFromBottom: (int)]
    //       [DistanceFromLeft: (int)]
    //       [DistanceFromRight: (int)]
    //       [EffectExtent: { [TopEdge: (long)], [BottomEdge: (long)], [LeftEdge: (long)], [RightEdge: (long)] }]
    //
    //     DrawingType AnchorWrapTight parameters :
    //       WrapText: (DW.WrapTextValues.BothSides)
    //       [DistanceFromLeft: (int)]
    //       [DistanceFromRight: (int)]
    //       [Polygon: { [StartPointX: (long)], [StartPointY: (long)], LinesTo: [ { X: (long), Y: (long) }, ...], [Edited: (bool)] }]
    //       [Square: { [StartPointX: (long)], [StartPointY: (long)], HorizontalSize: (long), VerticalSize: (long), [Edited: (bool)] }]
    //       //////////////////[WrapPolygonHorizontalSize: (int)]
    //       //////////////////[WrapPolygonVerticalSize: (int)]
    //       //////////////////[WrapPolygonStartPointX: (int)]
    //       //////////////////[WrapPolygonStartPointY: (int)]
    //
    //     DrawingType AnchorWrapTopAndBottom parameters :
    //       [DistanceFromTop: (int)]
    //       [DistanceFromBottom: (int)]
    //       [EffectExtent: { [TopEdge: (long)], [BottomEdge: (long)], [LeftEdge: (long)], [RightEdge: (long)] }]

    public class OXmlElementReader
    {
        private bool _headerFooter = false;

        private IEnumerable<OXmlElement> _Read(BsonReader reader)
        {
            //foreach (BsonDocument sourceElement in zmongo.BsonRead<BsonDocument>(file))
            foreach (BsonDocument sourceElement in zmongo.BsonRead<BsonDocument>(reader))
            {
                string type = sourceElement.zGet("Type").zAsString();
                OXmlElement element = null;
                //IEnumerable<OXmlElement> elements = null;
                switch (type.ToLower())
                {
                    case "paragraph":
                        element = CreateParagraph(sourceElement);
                        break;
                    case "text":
                        element = CreateText(sourceElement);
                        break;
                    case "line":
                        element = new OXmlElement { Type = OXmlElementType.Line };
                        break;
                    case "tabstop":
                        element = new OXmlElement { Type = OXmlElementType.TabStop };
                        break;
                    case "simplefield":
                        element = CreateSimpleField(sourceElement);
                        break;
                    case "docsection":
                        element = CreateDocSection(sourceElement);
                        break;
                    //case "docdefaults":
                    //    element = CreateDocDefaults(sourceElement);
                    //    break;
                    case "docdefaultsrunproperties":
                        element = CreateRunPropertiesDefault(sourceElement);
                        break;
                    case "docdefaultsparagraphproperties":
                        element = CreateParagraphPropertiesDefault();
                        break;
                    case "openheader":
                        //element = CreateOpenHeaderFooter(sourceElement, header: true);
                        element = CreateOpenHeader(sourceElement);
                        break;
                    case "openfooter":
                        //element = CreateOpenHeaderFooter(sourceElement, header: false);
                        element = CreateOpenFooter(sourceElement);
                        break;
                    case "closeheader":
                        element = CreateCloseHeader();
                        break;
                    case "closefooter":
                        element = CreateCloseFooter();
                        break;
                    case "style":
                        element = OXmlStyleElementCreator.Create(sourceElement);
                        break;
                    case "picture":
                        element = OXmlPictureElementCreator.CreatePicture(sourceElement);
                        break;
                    default:
                        throw new PBException($"unknow oxml element type \"{type}\"");
                }
                if (element != null)
                    yield return element;
                //if (elements != null)
                //{
                //    foreach (OXmlElement element2 in elements)
                //        yield return element2;
                //}
            }
        }

        private static OXmlParagraphElement CreateParagraph(BsonDocument element)
        {
            return new OXmlParagraphElement { Style = element.zGet("Style").zAsString() };
        }

        private static OXmlElement CreateText(BsonDocument element)
        {
            return new OXmlTextElement { Text = element.zGet("Text").zAsString(), PreserveSpace = element.zGet("PreserveSpace").zAsBoolean() };
        }

        private OXmlElement CreateSimpleField(BsonDocument element)
        {
            return new OXmlSimpleFieldElement { Instruction = element.zGet("Instruction").zAsString() };
        }

        private OXmlDocSectionElement CreateDocSection(BsonDocument element)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain doc section");

            OXmlDocSectionElement docSection = new OXmlDocSectionElement();
            BsonDocument element2 = element.zGet("PageSize").zAsBsonDocument();
            if (element2 != null)
            {
                docSection.PageSize = new OXmlPageSize { Width = element2.zGet("Width").zAsNullableInt(), Height = element2.zGet("Height").zAsNullableInt() };
            }
            element2 = element.zGet("PageMargin").zAsBsonDocument();
            if (element2 != null)
            {
                docSection.PageMargin = new OXmlPageMargin
                {
                    Top = element2.zGet("Top").zAsNullableInt(),
                    Bottom = element2.zGet("Bottom").zAsNullableInt(),
                    Left = element2.zGet("Left").zAsNullableInt(),
                    Right = element2.zGet("Right").zAsNullableInt(),
                    Header = element2.zGet("Header").zAsNullableInt(),
                    Footer = element2.zGet("Footer").zAsNullableInt()
                };
            }
            docSection.PageNumberStart = element.zGet("PageNumberStart").zAsNullableInt();
            return docSection;
        }

        //private OXmlDocDefaultsElement CreateDocDefaults(BsonDocument element)
        //{
        //    if (_headerFooter)
        //        throw new PBException("header-footer cant contain doc defaults");

        //    string type = element.zGet("DocDefaultsType").zAsString();
        //    if (type.ToLower() == "runpropertiesdefault")
        //        return CreateRunPropertiesDefault(element);
        //    else if (type.ToLower() == "paragraphpropertiesdefault")
        //        return CreateParagraphPropertiesDefault();
        //    else
        //        throw new PBException($"unknow oxml doc defaults type \"{type}\"");
        //}

        private OXmlDocDefaultsRunPropertiesElement CreateRunPropertiesDefault(BsonDocument element)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain run default properties");

            OXmlDocDefaultsRunPropertiesElement runPropertiesDefault = new OXmlDocDefaultsRunPropertiesElement();

            //string runFonts = element.zGet("RunFonts").zAsString();
            //if (runFonts != null)
            //    runPropertiesDefault.RunFonts = new OxmlRunFonts { Ascii = runFonts };
            BsonDocument element2 = element.zGet("RunFonts").zAsBsonDocument();
            if (element2 != null)
            {
                runPropertiesDefault.RunFonts = new OXmlRunFonts
                {
                    Ascii = element2.zGet("Ascii").zAsString(),
                    AsciiTheme = element2.zGet("AsciiTheme").zAsNullableEnum<ThemeFontValues>(ignoreCase: true),
                    ComplexScript = element2.zGet("ComplexScript").zAsString(),
                    ComplexScriptTheme = element2.zGet("ComplexScriptTheme").zAsNullableEnum<ThemeFontValues>(ignoreCase: true),
                    EastAsia = element2.zGet("EastAsia").zAsString(),
                    EastAsiaTheme = element2.zGet("EastAsiaTheme").zAsNullableEnum<ThemeFontValues>(ignoreCase: true),
                    HighAnsi = element2.zGet("HighAnsi").zAsString(),
                    HighAnsiTheme = element2.zGet("HighAnsiTheme").zAsNullableEnum<ThemeFontValues>(ignoreCase: true),
                    Hint = element2.zGet("Hint").zAsNullableEnum<FontTypeHintValues>(ignoreCase: true)
                };
            }

            runPropertiesDefault.FontSize = element.zGet("FontSize").zAsString();
            return runPropertiesDefault;
        }

        //private OXmlDocDefaultsParagraphPropertiesElement CreateParagraphPropertiesDefault()
        private OXmlElement CreateParagraphPropertiesDefault()
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain paragraph default properties");

            //return new OXmlDocDefaultsParagraphPropertiesElement();
            return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
        }

        //private OXmlElement CreateOpenHeaderFooter(BsonDocument element, bool header)
        //{
        //    if (_headerFooter)
        //        throw new PBException("header-footer cant contain nested header-footer");
        //    OXmlOpenHeaderFooter openHeaderFooter = new OXmlOpenHeaderFooter();
        //    openHeaderFooter.Header = header;
        //    openHeaderFooter.HeaderType = element.zGet("HeaderType").zAsString().zTryParseEnum(HeaderFooterValues.Default);
        //    _headerFooter = true;
        //    return openHeaderFooter;
        //}

        private OXmlElement CreateOpenHeader(BsonDocument element)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain nested header-footer");
            OXmlOpenHeaderElement openHeader = new OXmlOpenHeaderElement();
            openHeader.HeaderType = element.zGet("HeaderType").zAsString().zTryParseEnum(HeaderFooterValues.Default);
            _headerFooter = true;
            return openHeader;
        }

        private OXmlElement CreateOpenFooter(BsonDocument element)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain nested header-footer");
            OXmlOpenFooterElement openFooter = new OXmlOpenFooterElement();
            openFooter.FooterType = element.zGet("FooterType").zAsString().zTryParseEnum(HeaderFooterValues.Default);
            _headerFooter = true;
            return openFooter;
        }

        private OXmlElement CreateCloseHeader()
        {
            if (!_headerFooter)
                throw new PBException("close header-footer without an open header-footer");
            _headerFooter = false;
            return new OXmlElement { Type = OXmlElementType.CloseHeader };
        }

        private OXmlElement CreateCloseFooter()
        {
            if (!_headerFooter)
                throw new PBException("close header-footer without an open header-footer");
            _headerFooter = false;
            return new OXmlElement { Type = OXmlElementType.CloseFooter };
        }

        public static IEnumerable<OXmlElement> Read(string file, Encoding encoding = null)
        {
            // ATTENTION il faut boucler et retourner chaque élément sinon le using ferme le flux BsonReader
            //return new OXmlElementReader()._Read(zmongo.OpenBsonReader(file, encoding));
            using (BsonReader reader = zmongo.CreateBsonReaderFromFile(file, encoding))
            {
                foreach (OXmlElement element in new OXmlElementReader()._Read(reader))
                    yield return element;
            }
        }
    }

    public class OXmlStyleElementCreator
    {
        private BsonDocument _sourceElement = null;
        //private StyleParagraphProperties _paragraphProperties = null;
        private OXmlStyleParagraphProperties _paragraphProperties = null;
        //private SpacingBetweenLines _spacingBetweenLines = null;
        //private TextAlignment _textAlignment = null;

        private OXmlStyleElementCreator(BsonDocument sourceElement)
        {
            _sourceElement = sourceElement;
        }

        private OXmlStyleElement _Create()
        {
            OXmlStyleElement style = new OXmlStyleElement();
            style.Id = _sourceElement.zGet("Id").zAsString();
            style.Name = _sourceElement.zGet("Name").zAsString();
            if (style.Name == null)
                style.Name = style.Id;
            style.StyleType = _sourceElement.zGet("StyleType").zAsString().zTryParseEnum(StyleValues.Paragraph);
            style.Aliases = _sourceElement.zGet("Aliases").zAsString();
            style.CustomStyle = _sourceElement.zGet("CustomStyle").zAsBoolean();
            style.DefaultStyle = _sourceElement.zGet("DefaultStyle").zAsBoolean();
            style.Locked = _sourceElement.zGet("Locked").zAsNullableBoolean();
            style.SemiHidden = _sourceElement.zGet("SemiHidden").zAsNullableBoolean();
            style.StyleHidden = _sourceElement.zGet("StyleHidden").zAsNullableBoolean();
            style.UnhideWhenUsed = _sourceElement.zGet("UnhideWhenUsed").zAsNullableBoolean();
            style.UIPriority = _sourceElement.zGet("UIPriority").zAsNullableInt();
            style.LinkedStyle = _sourceElement.zGet("LinkedStyle").zAsString();
            style.BasedOn = _sourceElement.zGet("BasedOn").zAsString();
            style.NextParagraphStyle = _sourceElement.zGet("NextParagraphStyle").zAsString();

            CreateParagraphProperties();
            if (_paragraphProperties != null)
                style.StyleParagraphProperties = _paragraphProperties;

            return style;
        }

        private void CreateParagraphProperties()
        {
            // StyleParagraphProperties :
            // AdjustRightIndent, AutoSpaceDE, AutoSpaceDN, BiDi, ContextualSpacing, FrameProperties, Indentation, Justification, KeepLines, KeepNext, Kinsoku, MirrorIndents, NumberingProperties,
            // OutlineLevel, OverflowPunctuation, PageBreakBefore, ParagraphBorders, ParagraphPropertiesChange, Shading, SnapToGrid, SuppressAutoHyphens, SuppressLineNumbers, SuppressOverlap, TextBoxTightWrap, TextDirection,
            // TopLinePunctuation, WidowControl, WordWrap


            //StyleParagraphProperties
            BsonDocument spp = _sourceElement.zGet("StyleParagraphProperties").zAsBsonDocument();
            if (spp == null)
                return;

            //_paragraphProperties = new StyleParagraphProperties();
            _paragraphProperties = new OXmlStyleParagraphProperties();

            CreateSpacingBetweenLines(spp);

            BsonValue v = spp.zGet("TextAlignment");
            if (v != null)
                // TextAlignment, <w:textAlignment>, Top, Center, Baseline, Bottom, Auto
                //_paragraphProperties.TextAlignment = new TextAlignment { Val = v.zAsString().zParseEnum<VerticalTextAlignmentValues>(ignoreCase: true) };
                _paragraphProperties.TextAlignment = v.zAsString().zParseEnum<VerticalTextAlignmentValues>(ignoreCase: true);
            CreateTabs(spp);

            //if (_spacingBetweenLines != null)
            //{
            //    _paragraphProperties = new StyleParagraphProperties();
            //    //if (_spacingBetweenLines != null)
            //    //    _paragraphProperties.SpacingBetweenLines = _spacingBetweenLines;
            //    //if (_textAlignment != null)
            //    //    _paragraphProperties.TextAlignment = _textAlignment;
            //}
        }

        private void CreateSpacingBetweenLines(BsonDocument spp)
        {
            BsonDocument elementSpacingBetweenLines = spp.zGet("SpacingBetweenLines").zAsBsonDocument();
            if (elementSpacingBetweenLines == null)
                return;

            //SpacingBetweenLines spacingBetweenLines = new SpacingBetweenLines { };
            OXmlSpacingBetweenLines spacingBetweenLines = new OXmlSpacingBetweenLines();

            //BsonValue v;

            //BsonValue v = elementSpacingBetweenLines.zGet("After");
            //if (v != null)
            //    // Spacing Below Paragraph, <w:spacing w:after>, 1 line = 240
            //    spacingBetweenLines.After = v.zAsString();
            // Spacing Below Paragraph, <w:spacing w:after>, 1 line = 240
            spacingBetweenLines.After = elementSpacingBetweenLines.zGet("After").zAsString();

            //v = elementSpacingBetweenLines.zGet("AfterAutoSpacing");
            //if (v != null)
            //    // Automatically Determine Spacing Below Paragraph, <w:spacing w:afterAutospacing>
            //    spacingBetweenLines.AfterAutoSpacing = new OnOffValue(v.zAsBoolean());
            // Automatically Determine Spacing Below Paragraph, <w:spacing w:afterAutospacing>
            spacingBetweenLines.AfterAutoSpacing = elementSpacingBetweenLines.zGet("AfterAutoSpacing").zAsNullableBoolean();

            //v = elementSpacingBetweenLines.zGet("AfterLines");
            //if (v != null)
            //    // Spacing Below Paragraph in Line Units, <w:spacing w:afterLines>
            //    spacingBetweenLines.AfterLines = v.zAsInt();
            // Spacing Below Paragraph in Line Units, <w:spacing w:afterLines>
            spacingBetweenLines.AfterLines = elementSpacingBetweenLines.zGet("AfterLines").zAsNullableInt();

            //v = elementSpacingBetweenLines.zGet("Before");
            //if (v != null)
            //    // Spacing Above Paragraph, <w:spacing w:before>, 1 line = 240
            //    spacingBetweenLines.Before = v.zAsString();
            // Spacing Above Paragraph, <w:spacing w:before>, 1 line = 240
            spacingBetweenLines.Before = elementSpacingBetweenLines.zGet("Before").zAsString();

            //v = elementSpacingBetweenLines.zGet("BeforeAutoSpacing");
            //if (v != null)
            //    // Automatically Determine Spacing Above Paragraph, <w:spacing w:beforeAutospacing>
            //    spacingBetweenLines.BeforeAutoSpacing = new OnOffValue(v.zAsBoolean());
            // Automatically Determine Spacing Above Paragraph, <w:spacing w:beforeAutospacing>
            spacingBetweenLines.BeforeAutoSpacing = elementSpacingBetweenLines.zGet("BeforeAutoSpacing").zAsNullableBoolean();

            //v = elementSpacingBetweenLines.zGet("BeforeLines");
            //if (v != null)
            //    // Spacing Above Paragraph IN Line Units, <w:spacing w:beforeLines>
            //    spacingBetweenLines.BeforeLines = v.zAsInt();
            // Spacing Above Paragraph IN Line Units, <w:spacing w:beforeLines>
            spacingBetweenLines.BeforeLines = elementSpacingBetweenLines.zGet("BeforeLines").zAsNullableInt();

            //v = elementSpacingBetweenLines.zGet("Line");
            //if (v != null)
            //    // Spacing Between Lines in Paragraph, <w:spacing w:line>, 1 line = 240
            //    spacingBetweenLines.Line = v.zAsString();
            // Spacing Between Lines in Paragraph, <w:spacing w:line>, 1 line = 240
            spacingBetweenLines.Line = elementSpacingBetweenLines.zGet("Line").zAsString();

            //v = elementSpacingBetweenLines.zGet("LineRule");
            //if (v != null)
            //    // Type of Spacing Between Lines, <w:spacing w:lineRule>, Auto, Exact, AtLeast
            //    spacingBetweenLines.LineRule = v.zAsString().zParseEnum<LineSpacingRuleValues>(true);
            // Type of Spacing Between Lines, <w:spacing w:lineRule>, Auto, Exact, AtLeast
            spacingBetweenLines.LineRule = elementSpacingBetweenLines.zGet("LineRule").zAsNullableEnum<LineSpacingRuleValues>(true);

            //_paragraphProperties.SpacingBetweenLines = spacingBetweenLines;
            _paragraphProperties.SpacingBetweenLines = spacingBetweenLines;
        }

        private void CreateTabs(BsonDocument spp)
        {
            BsonArray tabs = spp.zGet("Tabs").zAsBsonArray();
            if (tabs == null)
                return;

            //Tabs tabsElement = new Tabs();
            List<OXmlTabStop> tabsElement = new List<OXmlTabStop>();
            foreach (BsonValue tab in tabs)
            {
                BsonDocument dtab = tab.AsBsonDocument;

                // TabStop, <w:tab>
                //TabStop tabStop = new TabStop();
                OXmlTabStop tabStop = new OXmlTabStop();
                // Tab Stop Position, <w:tab w:pos="">
                tabStop.Position = dtab.zGet("Position", mandatory: true).zAsInt();
                // Tab Stop Type, <w:tab w:val="">, Clear, Left, Start, Center, Right, End, Decimal, Bar, Number
                tabStop.Alignment = dtab.zGet("Alignment", mandatory: true).zAsEnum<TabStopValues>(ignoreCase: true);
                // Tab Leader Character, <w:tab w:leader="">, None, Dot, Hyphen, Underscore, Heavy, MiddleDot
                tabStop.LeaderChar = dtab.zGet("LeaderChar").zTryAsEnum(TabStopLeaderCharValues.None, ignoreCase: true);

                //tabsElement.AppendChild(tabStop);
                tabsElement.Add(tabStop);
            }
            _paragraphProperties.Tabs = tabsElement.ToArray();
        }

        //private void CreateStyleParagraphProperties()
        //{
        //    if (_paragraphProperties == null)
        //        _paragraphProperties = new StyleParagraphProperties();
        //}

        public static OXmlStyleElement Create(BsonDocument document)
        {
            return new OXmlStyleElementCreator(document)._Create();
        }
    }
}

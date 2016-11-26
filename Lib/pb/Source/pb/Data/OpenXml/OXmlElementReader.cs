using System.Collections.Generic;
using MongoDB.Bson;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using pb.Data.Mongo;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
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
    //       [PageSize { Width: 11907, Height: 16839 }],
    //       [PageMargin { Top: 720, Bottom: 720, Left: 1418, Right: 1418, Header: 284, Footer: 284 }],
    //       [PageNumberStart: 1]
    //     }
    //     PageSize unit 1/20 point, 11907 1/20 point = 21 cm, 16839 1/20 point = 29.7 cm
    //     PageMargin unit 1/20 point, 720 1/20 point = 0.6 cm, 1418 1/20 point = 1.27 cm, 284 1/20 point = 0.5 cm
    //
    //
    //   add default run properties :
    //     docx : styles.xml <w:styles>/<w:docDefaults>/<w:rPrDefault>
    //     function : OXmlElementReader.CreateRunPropertiesDefault()
    //     json :
    //     {
    //       Type: "DocDefaults",
    //       DocDefaultsType: "RunPropertiesDefault",
    //       RunFonts: "Arial",
    //       FontSize: "22"
    //     }
    //
    //   add default paragraph properties  :
    //     docx : styles.xml <w:styles>/<w:docDefaults>/<w:pPrDefault />
    //     function : OXmlElementReader.CreateParagraphPropertiesDefault()
    //     json :
    //     {
    //       Type: "DocDefaults",
    //       DocDefaultsType: "ParagraphPropertiesDefault"
    //     }
    //
    //   open header footer
    //     function : OXmlElementReader.CreateOpenHeaderFooter()
    //     json :
    //     {
    //       Type: "OpenHeader" | "OpenFooter",
    //       [HeaderType: "Default" | "First" | "Even" (HeaderFooterValues)],
    //       //Content: [ Paragraph | Line | Text | Picture, ... ]
    //     }
    //     WARNING even option dont work for google doc
    //
    //   close header footer
    //     function : OXmlElementReader.CreateCloseHeaderFooter()
    //     json :
    //     { Type: "CloseHeaderFooter" }
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
    //       [Default: false],
    //       [Locked: true],
    //       [SemiHidden: true],
    //       [StyleHidden: true],
    //       [UnhideWhenUsed: true],
    //       [UIPriority: (int)], 
    //       [LinkedStyle: ""],
    //       [BasedOn: ""],
    //       [NextParagraphStyle: ""]
    //     - paragraph properties :
    //       SpacingBetweenLines: {
    //         [After: (string)],                                                               // Spacing Below Paragraph, <w:spacing w:after>, 1 line = 240
    //         [AfterAutoSpacing: (bool)],                                                      // Automatically Determine Spacing Below Paragraph, <w:spacing w:afterAutospacing>
    //         [AfterLines: (int)],                                                             // Spacing Below Paragraph in Line Units, <w:spacing w:afterLines>
    //         [Before: (string)],                                                              // Spacing Above Paragraph, <w:spacing w:before>, 1 line = 240
    //         [BeforeAutoSpacing: (bool)],                                                     // Automatically Determine Spacing Above Paragraph, <w:spacing w:beforeAutospacing>
    //         [BeforeLines: (int)],                                                            // Spacing Above Paragraph IN Line Units, <w:spacing w:beforeLines>
    //         [Line: (string)],                                                                // Spacing Between Lines in Paragraph, <w:spacing w:line>, 1 line = 240
    //         [LineRule: (string, LineSpacingRuleValues)]                                      // Type of Spacing Between Lines, <w:spacing w:lineRule>, Auto, Exact, AtLeast
    //       }
    //       TextAlignment: (string, VerticalTextAlignmentValues)                               // TextAlignment, <w:textAlignment>, Top, Center, Baseline, Bottom, Auto
    //       Tabs: [
    //           {
    //             Position: (int),                                                             // Tab Stop Position, <w:tab w:pos="">
    //             Alignment: (string, TabStopValues),                                          // Tab Stop Type, <w:tab w:val="">, Clear, Left, Start, Center, Right, End, Decimal, Bar, Number
    //             [LeaderChar: (string, TabStopLeaderCharValues)]                              // Tab Leader Character, <w:tab w:leader="">, None, Dot, Hyphen, Underscore, Heavy, MiddleDot
    //           }
    //         ]
    //       }
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
    //       DrawingMode: "Inline" | "Anchor",
    //
    //     DrawingMode Inline no parameters
    //
    //     DrawingMode Anchor parameters :
    //       WrapType: "WrapSquare" | "WrapTight" | "WrapTopAndBottom"
    //       HorizontalRelativeFrom (DW.HorizontalRelativePositionValues.Margin)
    //       HorizontalPositionOffset (int)
    //       HorizontalAlignment (string)
    //       VerticalRelativeFrom (DW.VerticalRelativePositionValues.Paragraph)
    //       VerticalPositionOffset (int)
    //       VerticalAlignment (string)
    //
    //     AnchorWrapSquare :
    //       WrapText(DW.WrapTextValues.BothSides)
    //       DistanceFromTop(int)
    //       DistanceFromBottom(int)
    //       DistanceFromLeft(int)
    //       DistanceFromRight(int)
    //
    //     AnchorWrapTight :
    //       WrapText(DW.WrapTextValues.BothSides)
    //       DistanceFromLeft(int)
    //       DistanceFromRight(int)
    //       WrapPolygonHorizontalSize(int)
    //       WrapPolygonVerticalSize(int)
    //       WrapPolygonStartPointX(int)
    //       WrapPolygonStartPointY(int)
    //
    //     AnchorWrapTopAndBottom:
    //       DistanceFromTop(int)
    //       DistanceFromBottom(int)

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
                    case "docdefaults":
                        element = CreateDocDefaults(sourceElement);
                        break;
                    case "openheader":
                        element = CreateOpenHeaderFooter(sourceElement, header: true);
                        break;
                    case "openfooter":
                        element = CreateOpenHeaderFooter(sourceElement, header: false);
                        break;
                    case "closeheaderfooter":
                        element = CreateCloseHeaderFooter();
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

        private OXmlDocDefaultsElement CreateDocDefaults(BsonDocument element)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain doc defaults");

            string type = element.zGet("DocDefaultsType").zAsString();
            if (type.ToLower() == "runpropertiesdefault")
                return CreateRunPropertiesDefault(element);
            else if (type.ToLower() == "paragraphpropertiesdefault")
                return CreateParagraphPropertiesDefault();
            else
                throw new PBException($"unknow oxml doc defaults type \"{type}\"");
        }

        private OXmlParagraphPropertiesDefaultElement CreateParagraphPropertiesDefault()
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain paragraph default properties");

            return new OXmlParagraphPropertiesDefaultElement();
        }

        private OXmlRunPropertiesDefaultElement CreateRunPropertiesDefault(BsonDocument element)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain run default properties");

            OXmlRunPropertiesDefaultElement runPropertiesDefault = new OXmlRunPropertiesDefaultElement();
            string runFonts = element.zGet("RunFonts").zAsString();
            if (runFonts != null)
                runPropertiesDefault.RunFonts = new RunFonts { Ascii = runFonts };
            string fontSize = element.zGet("FontSize").zAsString();
            if (fontSize != null)
                runPropertiesDefault.FontSize = new FontSize { Val = fontSize };
            return runPropertiesDefault;
        }

        private OXmlElement CreateOpenHeaderFooter(BsonDocument element, bool header)
        {
            if (_headerFooter)
                throw new PBException("header-footer cant contain nested header-footer");
            OXmlOpenHeaderFooter openHeaderFooter = new OXmlOpenHeaderFooter();
            openHeaderFooter.Header = header;
            openHeaderFooter.HeaderType = element.zGet("HeaderType").zAsString().zTryParseEnum(HeaderFooterValues.Default);
            _headerFooter = true;
            return openHeaderFooter;

            //foreach (BsonValue element2 in element.zGet("Content").zAsBsonArray())
            //{
            //    BsonDocument docElement2 = element2.AsBsonDocument;
            //    string type = docElement2.zGet("Type").zAsString();
            //    OXmlElement element3 = null;
            //    switch (type.ToLower())
            //    {
            //        case "paragraph":
            //            element3 = CreateParagraph(docElement2);
            //            break;
            //        case "line":
            //            element3 = new OXmlElement { Type = OXmlElementType.Line };
            //            break;
            //        case "text":
            //            element3 = CreateText(docElement2);
            //            break;
            //        case "picture":
            //            element3 = CreatePicture(docElement2);
            //            break;
            //        default:
            //            throw new PBException($"invalid oxml element type \"{type}\" in header or footer");
            //    }
            //    if (element != null)
            //        yield return element3;
            //}

            //yield return new OXmlElement { Type = OXmlElementType.CloseHeaderFooter };
        }

        private OXmlElement CreateCloseHeaderFooter()
        {
            if (!_headerFooter)
                throw new PBException("close header-footer without an open header-footer");
            _headerFooter = false;
            return new OXmlElement { Type = OXmlElementType.CloseHeaderFooter };
        }

        public static IEnumerable<OXmlElement> Read(string file, Encoding encoding = null)
        {
            return new OXmlElementReader()._Read(zmongo.OpenBsonReader(file, encoding));
        }
    }

    public class OXmlStyleElementCreator
    {
        private BsonDocument _sourceElement = null;
        private StyleParagraphProperties _paragraphProperties = null;
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
            style.DefaultStyle = _sourceElement.zGet("Default").zAsBoolean();
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

            _paragraphProperties = new StyleParagraphProperties();

            CreateSpacingBetweenLines();

            BsonValue v = _sourceElement.zGet("TextAlignment");
            if (v != null)
                // TextAlignment, <w:textAlignment>, Top, Center, Baseline, Bottom, Auto
                _paragraphProperties.TextAlignment = new TextAlignment { Val = v.zAsString().zParseEnum<VerticalTextAlignmentValues>(ignoreCase: true) };
            CreateTabs();

            //if (_spacingBetweenLines != null)
            //{
            //    _paragraphProperties = new StyleParagraphProperties();
            //    //if (_spacingBetweenLines != null)
            //    //    _paragraphProperties.SpacingBetweenLines = _spacingBetweenLines;
            //    //if (_textAlignment != null)
            //    //    _paragraphProperties.TextAlignment = _textAlignment;
            //}
        }

        private void CreateSpacingBetweenLines()
        {
            BsonDocument elementSpacingBetweenLines = _sourceElement.zGet("SpacingBetweenLines").zAsBsonDocument();
            if (elementSpacingBetweenLines == null)
                return;

            SpacingBetweenLines spacingBetweenLines = new SpacingBetweenLines { };
            BsonValue v = elementSpacingBetweenLines.zGet("After");
            if (v != null)
                // Spacing Below Paragraph, <w:spacing w:after>, 1 line = 240
                spacingBetweenLines.After = v.zAsString();
            v = elementSpacingBetweenLines.zGet("AfterAutoSpacing");
            if (v != null)
                // Automatically Determine Spacing Below Paragraph, <w:spacing w:afterAutospacing>
                spacingBetweenLines.AfterAutoSpacing = new OnOffValue(v.zAsBoolean());
            v = elementSpacingBetweenLines.zGet("AfterLines");
            if (v != null)
                // Spacing Below Paragraph in Line Units, <w:spacing w:afterLines>
                spacingBetweenLines.AfterLines = v.zAsInt();
            v = elementSpacingBetweenLines.zGet("Before");
            if (v != null)
                // Spacing Above Paragraph, <w:spacing w:before>, 1 line = 240
                spacingBetweenLines.Before = v.zAsString();
            v = elementSpacingBetweenLines.zGet("BeforeAutoSpacing");
            if (v != null)
                // Automatically Determine Spacing Above Paragraph, <w:spacing w:beforeAutospacing>
                spacingBetweenLines.BeforeAutoSpacing = new OnOffValue(v.zAsBoolean());
            v = elementSpacingBetweenLines.zGet("BeforeLines");
            if (v != null)
                // Spacing Above Paragraph IN Line Units, <w:spacing w:beforeLines>
                spacingBetweenLines.BeforeLines = v.zAsInt();
            v = elementSpacingBetweenLines.zGet("Line");
            if (v != null)
                // Spacing Between Lines in Paragraph, <w:spacing w:line>, 1 line = 240
                spacingBetweenLines.Line = v.zAsString();
            v = elementSpacingBetweenLines.zGet("LineRule");
            if (v != null)
                // Type of Spacing Between Lines, <w:spacing w:lineRule>, Auto, Exact, AtLeast
                spacingBetweenLines.LineRule = v.zAsString().zParseEnum<LineSpacingRuleValues>(true);

            _paragraphProperties.SpacingBetweenLines = spacingBetweenLines;
        }

        private void CreateTabs()
        {
            BsonArray tabs = _sourceElement.zGet("Tabs").zAsBsonArray();
            if (tabs == null)
                return;
            Tabs tabsElement = new Tabs();
            foreach (BsonValue tab in tabs)
            {
                BsonDocument dtab = tab.AsBsonDocument;

                // TabStop, <w:tab>
                TabStop tabStop = new TabStop();
                // Tab Stop Position, <w:tab w:pos="">
                tabStop.Position = dtab.zGet("Position").zAsInt();
                // Tab Stop Type, <w:tab w:val="">, Clear, Left, Start, Center, Right, End, Decimal, Bar, Number
                tabStop.Val = dtab.zGet("Alignment").zAsString().zParseEnum<TabStopValues>(ignoreCase: true);
                // Tab Leader Character, <w:tab w:leader="">, None, Dot, Hyphen, Underscore, Heavy, MiddleDot
                tabStop.Leader = dtab.zGet("LeaderChar").zAsString().zTryParseEnum(TabStopLeaderCharValues.None);

                tabsElement.AppendChild(tabStop);
            }
            _paragraphProperties.Tabs = tabsElement;
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

    public class OXmlPictureElementCreator
    {
        public static OXmlElement CreatePicture(BsonDocument sourceElement)
        {
            OXmlPictureElement picture = new OXmlPictureElement();
            picture.File = sourceElement.zGet("File").zAsString();
            picture.Name = sourceElement.zGet("Name").zAsString();
            picture.Description = sourceElement.zGet("Description").zAsString();
            picture.Width = sourceElement.zGet("Width").zAsNullableInt();
            picture.Height = sourceElement.zGet("Height").zAsNullableInt();
            picture.Rotation = sourceElement.zGet("Rotation").zAsInt();
            picture.HorizontalFlip = sourceElement.zGet("HorizontalFlip").zAsBoolean();
            picture.VerticalFlip = sourceElement.zGet("VerticalFlip").zAsBoolean();
            //picture.CompressionState = GetCompressionState(element.zGet("Description").zAsString());
            picture.CompressionState = sourceElement.zGet("CompressionState").zAsString().zTryParseEnum(A.BlipCompressionValues.Print);
            picture.PresetShape = sourceElement.zGet("PresetShape").zAsString().zTryParseEnum(A.ShapeTypeValues.Rectangle);
            picture.PictureDrawing = CreatePictureDrawing(sourceElement);
            return picture;
        }

        private static OXmlPictureDrawing CreatePictureDrawing(BsonDocument element)
        {
            string drawingMode = element.zGet("DrawingMode").zAsString();
            switch (drawingMode.ToLower())
            {
                case "inline":
                    return new OXmlInlinePictureDrawing();
                case "anchor":
                    return CreateAnchorPictureDrawing(element);
                default:
                    throw new PBException($"unknow oxml drawing mode \"{drawingMode}\"");
            }
        }

        private static OXmlAnchorPictureDrawing CreateAnchorPictureDrawing(BsonDocument element)
        {
            OXmlAnchorPictureDrawing anchor = new OXmlAnchorPictureDrawing();
            string wrapType = element.zGet("WrapType").zAsString();
            switch (wrapType.ToLower())
            {
                //case "wrapnone":
                case "wrapsquare":
                    anchor.Wrap = CreateAnchorWrapSquare(element);
                    break;
                case "wraptight":
                    anchor.Wrap = CreateAnchorWrapTight(element);
                    break;
                //case "wrapthrough":
                case "wraptopandbottom":
                    anchor.Wrap = CreateAnchorWrapTopAndBottom(element);
                    break;
                default:
                    throw new PBException($"unknow oxml wrap type \"{wrapType}\"");
            }

            // Margin, Page, Column, Character, LeftMargin, RightMargin, InsideMargin, OutsideMargin
            anchor.HorizontalRelativeFrom = element.zGet("HorizontalRelativeFrom").zAsString().zTryParseEnum(DW.HorizontalRelativePositionValues.Margin);
            anchor.HorizontalPositionOffset = element.zGet("HorizontalPositionOffset").zAsNullableInt();
            anchor.HorizontalAlignment = element.zGet("HorizontalAlignment").zAsString();

            anchor.VerticalRelativeFrom = element.zGet("VerticalRelativeFrom").zAsString().zTryParseEnum(DW.VerticalRelativePositionValues.Paragraph);
            anchor.VerticalPositionOffset = element.zGet("VerticalPositionOffset").zAsNullableInt();
            anchor.VerticalAlignment = element.zGet("VerticalAlignment").zAsString();

            return anchor;
        }

        private static OXmlAnchorWrapSquare CreateAnchorWrapSquare(BsonDocument element)
        {
            OXmlAnchorWrapSquare wrapSquare = new OXmlAnchorWrapSquare();
            wrapSquare.WrapText = element.zGet("WrapText").zAsString().zTryParseEnum(DW.WrapTextValues.BothSides);
            wrapSquare.DistanceFromTop = (uint)element.zGet("DistanceFromTop").zAsInt();
            wrapSquare.DistanceFromBottom = (uint)element.zGet("DistanceFromBottom").zAsInt();
            wrapSquare.DistanceFromLeft = (uint)element.zGet("DistanceFromLeft").zAsInt();
            wrapSquare.DistanceFromRight = (uint)element.zGet("DistanceFromRight").zAsInt();
            //wrapSquare.EffectExtent
            return wrapSquare;
        }

        private static OXmlAnchorWrapTight CreateAnchorWrapTight(BsonDocument element)
        {
            OXmlAnchorWrapTight wrapTight = new OXmlAnchorWrapTight();
            wrapTight.WrapText = element.zGet("WrapText").zAsString().zTryParseEnum(DW.WrapTextValues.BothSides);
            wrapTight.DistanceFromLeft = (uint)element.zGet("DistanceFromLeft").zAsInt();
            wrapTight.DistanceFromRight = (uint)element.zGet("DistanceFromRight").zAsInt();
            wrapTight.WrapPolygon = OXmlDoc.CreateWrapPolygon(element.zGet("WrapPolygonHorizontalSize").zAsLong(), element.zGet("WrapPolygonVerticalSize").zAsLong(), element.zGet("WrapPolygonStartPointX").zAsLong(), element.zGet("WrapPolygonStartPointY").zAsLong());
            return wrapTight;
        }

        private static OXmlAnchorWrapTopAndBottom CreateAnchorWrapTopAndBottom(BsonDocument element)
        {
            OXmlAnchorWrapTopAndBottom wrapTopAndBottom = new OXmlAnchorWrapTopAndBottom();
            wrapTopAndBottom.DistanceFromTop = (uint)element.zGet("DistanceFromTop").zAsInt();
            wrapTopAndBottom.DistanceFromBottom = (uint)element.zGet("DistanceFromBottom").zAsInt();
            //wrapTopAndBottom.EffectExtent
            return wrapTopAndBottom;
        }
    }
}

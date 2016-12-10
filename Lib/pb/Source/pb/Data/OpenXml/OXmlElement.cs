using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using ODW = DocumentFormat.OpenXml.Drawing.Wordprocessing;

// todo :
//   - ok remove EnumValue<>
//   - ok replace StyleParagraphProperties
//   - ok replace StyleRunProperties
//   - ok replace EffectExtent
//   - ok replace WrapPolygon

namespace pb.Data.OpenXml
{
    public enum OXmlElementType
    {
        Paragraph = 0,
        Text,
        Line,
        TabStop,
        SimpleField,
        DocSection,
        //DocDefaults,
        DocDefaultsRunProperties,
        DocDefaultsParagraphProperties,
        //OpenHeaderFooter,
        OpenHeader,
        CloseHeader,
        OpenFooter,
        CloseFooter,
        //CloseHeaderFooter,
        Style,
        Picture
    }

    public class OXmlElement
    {
        public OXmlElementType Type;

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    // OXmlElementType.Paragraph
    public class OXmlParagraphElement : OXmlElement
    {
        public string Style;

        public OXmlParagraphElement()
        {
            Type = OXmlElementType.Paragraph;
        }

        public override string ToString()
        {
            return $"Paragraph : style \"{Style}\"";
        }
    }

    // OXmlElementType.Text
    public class OXmlTextElement : OXmlElement
    {
        public string Text;
        public bool PreserveSpace;

        public OXmlTextElement()
        {
            Type = OXmlElementType.Text;
        }

        public override string ToString()
        {
            return $"Text : \"{Text}\"";
        }
    }

    // OXmlElementType.SimpleField
    public class OXmlSimpleFieldElement : OXmlElement
    {
        public string Instruction;

        public OXmlSimpleFieldElement()
        {
            Type = OXmlElementType.SimpleField;
        }

        public override string ToString()
        {
            return $"SimpleField : \"{Instruction}\"";
        }
    }

    // OXmlElementType.DocSection
    public class OXmlDocSectionElement : OXmlElement
    {
        public OXmlPageSize PageSize;
        public OXmlPageMargin PageMargin;
        public int? PageNumberStart;

        public OXmlDocSectionElement()
        {
            Type = OXmlElementType.DocSection;
        }

        public override string ToString()
        {
            return "DocSection";
        }
    }

    // OXmlElementType.DocDefaults
    //public enum OXmlDocDefaultsType
    //{
    //    RunPropertiesDefault = 0,
    //    ParagraphPropertiesDefault
    //}

    //public class OXmlDocDefaultsElement : OXmlElement
    //{
    //    public OXmlDocDefaultsType DocDefaultsType;

    //    public OXmlDocDefaultsElement()
    //    {
    //        Type = OXmlElementType.DocDefaults;
    //    }

    //    public override string ToString()
    //    {
    //        return "DocDefaults";
    //    }
    //}

    //public class OXmlRunPropertiesDefaultElement : OXmlDocDefaultsElement
    public class OXmlDocDefaultsRunPropertiesElement : OXmlElement
    {
        private OXmlRunFonts _runFonts = null;
        private string _fontSize = null;

        public OXmlDocDefaultsRunPropertiesElement()
        {
            //DocDefaultsType = OXmlDocDefaultsType.RunPropertiesDefault;
            Type = OXmlElementType.DocDefaultsRunProperties;
        }

        public OXmlRunFonts RunFonts { get { return _runFonts; } set { _runFonts = value; } }
        public string FontSize { get { return _fontSize; } set { _fontSize = value; } }

        public override string ToString()
        {
            return $"DocDefaults.RunProperties : font \"{_runFonts.Ascii}\" size {_fontSize}";
        }
    }

    //public class OXmlParagraphPropertiesDefaultElement : OXmlDocDefaultsElement
    //public class OXmlDocDefaultsParagraphPropertiesElement : OXmlElement
    //{
    //    public OXmlDocDefaultsParagraphPropertiesElement()
    //    {
    //        //DocDefaultsType = OXmlDocDefaultsType.ParagraphPropertiesDefault;
    //        Type = OXmlElementType.DocDefaultsParagraphProperties;
    //    }

    //    public override string ToString()
    //    {
    //        return "DocDefaults.ParagraphProperties";
    //    }
    //}

    // OXmlElementType.OpenHeaderFooter
    //public class OXmlOpenHeaderFooter : OXmlElement
    //{
    //    public bool Header;                        // true Header, false Footer
    //    public HeaderFooterValues HeaderType;      // Default, First, Even

    //    public OXmlOpenHeaderFooter()
    //    {
    //        Type = OXmlElementType.OpenHeaderFooter;
    //    }

    //    public override string ToString()
    //    {
    //        string s = Header ? "header" : "footer";
    //        return $"OpenHeaderFooter : {s} {HeaderType}";
    //    }
    //}

    // OXmlElementType.OpenHeader
    public class OXmlOpenHeaderElement : OXmlElement
    {
        public HeaderFooterValues HeaderType;      // Default, First, Even

        public OXmlOpenHeaderElement()
        {
            Type = OXmlElementType.OpenHeader;
        }

        public override string ToString()
        {
            return $"OpenHeader : {HeaderType}";
        }
    }

    // OXmlElementType.OpenFooter
    public class OXmlOpenFooterElement : OXmlElement
    {
        public HeaderFooterValues FooterType;      // Default, First, Even

        public OXmlOpenFooterElement()
        {
            Type = OXmlElementType.OpenFooter;
        }

        public override string ToString()
        {
            return $"OpenFooter : {FooterType}";
        }
    }

    // OXmlElementType.Style
    public class OXmlStyleElement : OXmlElement
    {
        //public OXmlDocDefaultsType DocDefaultsType;
        public string Id = null;                                        // Style ID
        public string Name = null;                                      // Primary Style Name
        public StyleValues StyleType;                                   // Paragraph, Character, Table, Numbering
        public string Aliases;                                          // Alternate Style Names
        public bool CustomStyle;                                        // User-Defined Style
        public bool DefaultStyle;                                       // Default Style
        public bool? Locked;                                            // Style Cannot Be Applied
        public bool? SemiHidden;                                        // Hide Style From Main User Interface
        public bool? StyleHidden;                                       // Hide Style From User Interface
        public bool? UnhideWhenUsed;                                    // Remove Semi-Hidden Property When Style Is Used
        public int? UIPriority;                                         // Optional User Interface Sorting Order
        public string LinkedStyle;                                      // Linked Style Reference
        public string BasedOn;                                          // Parent Style ID
        public string NextParagraphStyle;                               // Style For Next Paragraph
        //public StyleParagraphProperties StyleParagraphProperties;       // Style Paragraph Properties
        public OXmlStyleParagraphProperties StyleParagraphProperties;   // Style Paragraph Properties
        //public StyleRunProperties StyleRunProperties;                   // Run Properties
        public OXmlStyleRunProperties StyleRunProperties;               // Run Properties

        public OXmlStyleElement()
        {
            Type = OXmlElementType.Style;
        }

        public override string ToString()
        {
            return $"Style : id {Id} name \"{Name}\" type {StyleType}";
        }
    }

    // used in OXmlRunPropertiesDefaultElement, OXmlStyleRunProperties
    public class OXmlRunFonts
    {
        public string Ascii;
        public ThemeFontValues? AsciiTheme;
        public string ComplexScript;
        public ThemeFontValues? ComplexScriptTheme;
        public string EastAsia;
        public ThemeFontValues? EastAsiaTheme;
        public string HighAnsi;
        public ThemeFontValues? HighAnsiTheme;
        public FontTypeHintValues? Hint;

        public RunFonts ToRunFonts()
        {
            RunFonts runFonts = new RunFonts();
            if (Ascii != null)
                runFonts.Ascii = Ascii;
            if (AsciiTheme != null)
                runFonts.AsciiTheme = AsciiTheme;
            if (ComplexScript != null)
                runFonts.ComplexScript = ComplexScript;
            if (ComplexScriptTheme != null)
                runFonts.ComplexScriptTheme = ComplexScriptTheme;
            if (EastAsia != null)
                runFonts.EastAsia = EastAsia;
            if (EastAsiaTheme != null)
                runFonts.EastAsiaTheme = EastAsiaTheme;
            if (HighAnsi != null)
                runFonts.HighAnsi = HighAnsi;
            if (HighAnsiTheme != null)
                runFonts.HighAnsiTheme = HighAnsiTheme;
            if (Hint != null)
                runFonts.Hint = Hint;
            return runFonts;
        }
    }

    // used in OXmlDocSectionElement
    public class OXmlPageSize
    {
        public int? Width;
        public int? Height;
    }

    // used in OXmlDocSectionElement
    public class OXmlPageMargin
    {
        public int? Top;
        public int? Bottom;
        public int? Left;
        public int? Right;
        public int? Header;
        public int? Footer;
    }

    // used in OXmlStyleElement
    // data of StyleParagraphProperties
    public class OXmlStyleParagraphProperties
    {
        public bool? AdjustRightIndent;
        public bool? AutoSpaceDE;
        public bool? AutoSpaceDN;
        public bool? BiDi;
        public bool? ContextualSpacing;
        //public FrameProperties FrameProperties;
        //public Indentation Indentation;
        public JustificationValues? Justification;
        public bool? KeepLines;
        public bool? KeepNext;
        public bool? Kinsoku;
        public bool? MirrorIndents;
        //public NumberingProperties NumberingProperties;
        public int? OutlineLevel;
        public bool? OverflowPunctuation;
        public bool? PageBreakBefore;
        //public ParagraphBorders ParagraphBorders;
        //public ParagraphPropertiesChange ParagraphPropertiesChange;
        //public Shading Shading;
        public bool? SnapToGrid;
        public OXmlSpacingBetweenLines SpacingBetweenLines;
        public bool? SuppressAutoHyphens;
        public bool? SuppressLineNumbers;
        public bool? SuppressOverlap;
        //public IEnumerable<OXmlTabStop> Tabs;
        public OXmlTabStop[] Tabs;
        public VerticalTextAlignmentValues? TextAlignment;
        public TextBoxTightWrapValues? TextBoxTightWrap;
        public TextDirectionValues? TextDirection;
        public bool? TopLinePunctuation;
        public bool? WidowControl;
        public bool? WordWrap;

        public StyleParagraphProperties ToStyleParagraphProperties()
        {
            StyleParagraphProperties spp = new StyleParagraphProperties();

            if (AdjustRightIndent != null)
                spp.AdjustRightIndent = new AdjustRightIndent { Val = AdjustRightIndent };
            if (AutoSpaceDE != null)
                spp.AutoSpaceDE = new AutoSpaceDE { Val = AutoSpaceDE };
            if (AutoSpaceDN != null)
                spp.AutoSpaceDN = new AutoSpaceDN { Val = AutoSpaceDN };
            if (BiDi != null)
                spp.BiDi = new BiDi { Val = BiDi };
            if (ContextualSpacing != null)
                spp.ContextualSpacing = new ContextualSpacing { Val = ContextualSpacing };
            //FrameProperties
            //Indentation
            if (Justification != null)
                spp.Justification = new Justification { Val = Justification };
            if (KeepLines != null)
                spp.KeepLines = new KeepLines { Val = KeepLines };
            if (KeepNext != null)
                spp.KeepNext = new KeepNext { Val = KeepNext };
            if (Kinsoku != null)
                spp.Kinsoku = new Kinsoku { Val = Kinsoku };
            if (MirrorIndents != null)
                spp.MirrorIndents = new MirrorIndents { Val = MirrorIndents };
            //NumberingProperties
            if (OutlineLevel != null)
                spp.OutlineLevel = new OutlineLevel { Val = OutlineLevel };
            if (OverflowPunctuation != null)
                spp.OverflowPunctuation = new OverflowPunctuation { Val = OverflowPunctuation };
            if (PageBreakBefore != null)
                spp.PageBreakBefore = new PageBreakBefore { Val = PageBreakBefore };
            //ParagraphBorders
            //ParagraphPropertiesChange
            //Shading
            if (SnapToGrid != null)
                spp.SnapToGrid = new SnapToGrid { Val = SnapToGrid };
            if (SpacingBetweenLines != null)
                spp.SpacingBetweenLines = SpacingBetweenLines.ToSpacingBetweenLines();
            if (SuppressAutoHyphens != null)
                spp.SuppressAutoHyphens = new SuppressAutoHyphens { Val = SuppressAutoHyphens };
            if (SuppressLineNumbers != null)
                spp.SuppressLineNumbers = new SuppressLineNumbers { Val = SuppressLineNumbers };
            if (SuppressOverlap != null)
                spp.SuppressOverlap = new SuppressOverlap { Val = SuppressOverlap };
            //  Tabs
            if (Tabs != null)
                spp.Tabs = new Tabs(Tabs.Select(tabStop => tabStop.ToTabStop()));
            if (TextAlignment != null)
                spp.TextAlignment = new TextAlignment { Val = TextAlignment };
            if (TextBoxTightWrap != null)
                spp.TextBoxTightWrap = new TextBoxTightWrap { Val = TextBoxTightWrap };
            if (TextDirection != null)
                spp.TextDirection = new TextDirection { Val = TextDirection };
            if (TopLinePunctuation != null)
                spp.TopLinePunctuation = new TopLinePunctuation { Val = TopLinePunctuation };
            if (WidowControl != null)
                spp.WidowControl = new WidowControl { Val = WidowControl };
            if (WordWrap != null)
                spp.WordWrap = new WordWrap { Val = WordWrap };

            return spp;
        }
    }

    // used in OXmlStyleParagraphProperties
    // data of SpacingBetweenLines, <w:spacing>
    public class OXmlSpacingBetweenLines
    {
        public string After;                        // Spacing Below Paragraph, <w:spacing w:after>, 1 line = 240
        public bool? AfterAutoSpacing;              // Automatically Determine Spacing Below Paragraph, <w:spacing w:afterAutospacing>
        public int? AfterLines;                     // Spacing Below Paragraph in Line Units, <w:spacing w:afterLines>
        public string Before;                       // Spacing Above Paragraph, <w:spacing w:before>, 1 line = 240
        public bool? BeforeAutoSpacing;             // Automatically Determine Spacing Above Paragraph, <w:spacing w:beforeAutospacing>
        public int? BeforeLines;                    // Spacing Above Paragraph IN Line Units, <w:spacing w:beforeLines>
        public string Line;                         // Spacing Between Lines in Paragraph, <w:spacing w:line>, 1 line = 240
        public LineSpacingRuleValues? LineRule;     // Type of Spacing Between Lines, <w:spacing w:lineRule>, Auto, Exact, AtLeast

        public SpacingBetweenLines ToSpacingBetweenLines()
        {
            SpacingBetweenLines spacing = new SpacingBetweenLines();
            if (After != null)
                spacing.After = After;
            if (AfterAutoSpacing != null)
                spacing.AfterAutoSpacing = AfterAutoSpacing;
            if (AfterLines != null)
                spacing.AfterLines = AfterLines;
            if (Before != null)
                spacing.Before = Before;
            if (BeforeAutoSpacing != null)
                spacing.BeforeAutoSpacing = BeforeAutoSpacing;
            if (BeforeLines != null)
                spacing.BeforeLines = BeforeLines;
            if (Line != null)
                spacing.Line = Line;
            if (LineRule != null)
                spacing.LineRule = LineRule;
            return spacing;
        }
    }

    // used in OXmlStyleParagraphProperties
    // data of TabStop, <w:tab>
    public class OXmlTabStop
    {
        public int Position;                            // Tab Stop Position, <w:tab w:pos="">
        public TabStopValues Alignment;                 // Tab Stop Type, <w:tab w:val="">, Clear, Left, Start, Center, Right, End, Decimal, Bar, Number
        public TabStopLeaderCharValues LeaderChar;      // Tab Leader Character, <w:tab w:leader="">, None, Dot, Hyphen, Underscore, Heavy, MiddleDot

        public TabStop ToTabStop()
        {
            return new TabStop { Position = Position, Val = Alignment, Leader = LeaderChar };
        }
    }

    // used in OXmlStyleElement
    // data of StyleRunProperties
    public class OXmlStyleRunProperties
    {
        public bool? Bold;
        public bool? BoldComplexScript;
        //BorderType Border
        public bool? Caps;
        public int? CharacterScale;
        //Color Color
        public bool? DoubleStrike;
        //EastAsianLayout EastAsianLayout
        public bool? Emboss;
        public EmphasisMarkValues? Emphasis;
        //FitText FitText
        public string FontSize;
        public string FontSizeComplexScript;
        public bool? Imprint;
        public bool? Italic;
        public bool? ItalicComplexScript;
        public uint? Kern;
        //LanguageType Languages
        public bool? NoProof;
        public bool? Outline;
        public string Position;
        public OXmlRunFonts RunFonts;
        //RunPropertiesChange RunPropertiesChange
        //Shading Shading
        public bool? Shadow;
        public bool? SmallCaps;
        public bool? SnapToGrid;
        public int? Spacing;
        public bool? SpecVanish;
        public bool? Strike;
        public TextEffectValues? TextEffect;
        //Underline Underline
        public bool? Vanish;
        public VerticalPositionValues? VerticalTextAlignment;
        public bool? WebHidden;

        public StyleRunProperties ToStyleRunProperties()
        {
            StyleRunProperties srp = new StyleRunProperties();
            if (Bold != null)
                srp.Bold = new Bold { Val = Bold };
            if (BoldComplexScript != null)
                srp.BoldComplexScript = new BoldComplexScript { Val = BoldComplexScript };
            //BorderType Border
            if (Caps != null)
                srp.Caps = new Caps { Val = Caps };
            if (CharacterScale != null)
                srp.CharacterScale = new CharacterScale { Val = CharacterScale };
            //Color Color
            if (DoubleStrike != null)
                srp.DoubleStrike = new DoubleStrike { Val = DoubleStrike };
            //EastAsianLayout EastAsianLayout
            if (Emboss != null)
                srp.Emboss = new Emboss { Val = Emboss };
            if (Emphasis != null)
                srp.Emphasis = new Emphasis { Val = Emphasis };
            //FitText FitText
            if (FontSize != null)
                srp.FontSize = new FontSize { Val = FontSize };
            if (FontSizeComplexScript != null)
                srp.FontSizeComplexScript = new FontSizeComplexScript { Val = FontSizeComplexScript };
            if (Imprint != null)
                srp.Imprint = new Imprint { Val = Imprint };
            if (Italic != null)
                srp.Italic = new Italic { Val = Italic };
            if (ItalicComplexScript != null)
                srp.ItalicComplexScript = new ItalicComplexScript { Val = ItalicComplexScript };
            if (Kern != null)
                srp.Kern = new Kern { Val = Kern };
            //LanguageType Languages
            if (NoProof != null)
                srp.NoProof = new NoProof { Val = NoProof };
            if (Outline != null)
                srp.Outline = new Outline { Val = Outline };
            if (Position != null)
                srp.Position = new Position { Val = Position };
            if (RunFonts != null)
                srp.RunFonts = RunFonts.ToRunFonts();
            //RunPropertiesChange RunPropertiesChange
            //Shading Shading
            if (Shadow != null)
                srp.Shadow = new Shadow { Val = Shadow };
            if (SmallCaps != null)
                srp.SmallCaps = new SmallCaps { Val = SmallCaps };
            if (SnapToGrid != null)
                srp.SnapToGrid = new SnapToGrid { Val = SnapToGrid };
            if (Spacing != null)
                srp.Spacing = new Spacing { Val = Spacing };
            if (SpecVanish != null)
                srp.SpecVanish = new SpecVanish { Val = SpecVanish };
            if (Strike != null)
                srp.Strike = new Strike { Val = Strike };
            if (TextEffect != null)
                srp.TextEffect = new TextEffect { Val = TextEffect };
            //Underline Underline
            if (Vanish != null)
                srp.Vanish = new Vanish { Val = Vanish };
            if (VerticalTextAlignment != null)
                srp.VerticalTextAlignment = new VerticalTextAlignment { Val = VerticalTextAlignment };
            if (WebHidden != null)
                srp.WebHidden = new WebHidden { Val = WebHidden };

            return srp;
        }
    }

    // OXmlElementType.Picture
    public class OXmlPictureElement : OXmlElement
    {
        public string File;
        public string Name;
        public string Description;
        //public zDocXPictureDrawingMode DrawingMode;
        public int? Width;
        public int? Height;
        public int Rotation;
        public bool HorizontalFlip;
        public bool VerticalFlip;
        //public EnumValue<A.BlipCompressionValues> CompressionState = A.BlipCompressionValues.Print;
        public A.BlipCompressionValues CompressionState = A.BlipCompressionValues.Print;
        //public EnumValue<A.ShapeTypeValues> PresetShape = A.ShapeTypeValues.Rectangle;
        public A.ShapeTypeValues PresetShape = A.ShapeTypeValues.Rectangle;
        // Novacode.Shape : StarAndBannerShapes, CalloutShapes, EquationShapes, BlockArrowShapes, RectangleShapes, BasicShapes, FlowchartShapes
        public OXmlPictureDrawing PictureDrawing;

        public OXmlPictureElement()
        {
            Type = OXmlElementType.Picture;
        }

        public override string ToString()
        {
            return $"Picture : width {Width} height {Height} drawing {PictureDrawing} file \"{File}\"";
        }
    }

    public enum OXmlPictureDrawingType
    {
        Inline = 0,
        //Anchor
        AnchorWrapNone,
        AnchorWrapSquare,
        AnchorWrapTight,
        AnchorWrapThrough,
        AnchorWrapTopAndBottom
    }

    public enum OXmlPictureDrawingMode
    {
        Inline = 0,
        Anchor
        //AnchorWrapNone,
        //AnchorWrapSquare,
        //AnchorWrapTight,
        //AnchorWrapThrough,
        //AnchorWrapTopAndBottom
    }

    public class OXmlPictureDrawing
    {
        public OXmlPictureDrawingMode DrawingMode;

        public OXmlPictureDrawingType GetDrawingType()
        {
            if (DrawingMode == OXmlPictureDrawingMode.Inline)
                return OXmlPictureDrawingType.Inline;
            else
            {
                switch (((OXmlAnchorPictureDrawing)this).Wrap.WrapType)
                {
                    case OXmlAnchorWrapType.WrapNone:
                        return OXmlPictureDrawingType.AnchorWrapNone;
                    case OXmlAnchorWrapType.WrapSquare:
                        return OXmlPictureDrawingType.AnchorWrapSquare;
                    case OXmlAnchorWrapType.WrapTight:
                        return OXmlPictureDrawingType.AnchorWrapTight;
                    case OXmlAnchorWrapType.WrapThrough:
                        return OXmlPictureDrawingType.AnchorWrapThrough;
                    case OXmlAnchorWrapType.WrapTopAndBottom:
                        return OXmlPictureDrawingType.AnchorWrapTopAndBottom;
                    default:
                        throw new PBException($"unknow WrapType {((OXmlAnchorPictureDrawing)this).Wrap.WrapType}");
                }
            }
        }
    }

    public class OXmlInlinePictureDrawing : OXmlPictureDrawing
    {
        public OXmlInlinePictureDrawing()
        {
            DrawingMode = OXmlPictureDrawingMode.Inline;
        }

        public override string ToString()
        {
            return "inline drawing";
        }
    }

    public class OXmlAnchorPictureDrawing : OXmlPictureDrawing
    {
        private OXmlAnchorWrap _wrap = null;
        //private EnumValue<DW.HorizontalRelativePositionValues> _horizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin;
        private DW.HorizontalRelativePositionValues _horizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin;
        private int? _horizontalPositionOffset = null;   // pixel unit
        private string _horizontalAlignment = null;      // left, right, center, inside, outside
        //private EnumValue<DW.VerticalRelativePositionValues> _verticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph;
        private DW.VerticalRelativePositionValues _verticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph;
        private int? _verticalPositionOffset = null;     // pixel unit
        private string _verticalAlignment = null;        // top, bottom, center, inside, outside

        public OXmlAnchorWrap Wrap { get { return _wrap; } set { _wrap = value; } }
        public DW.HorizontalRelativePositionValues HorizontalRelativeFrom { get { return _horizontalRelativeFrom; } set { _horizontalRelativeFrom = value; } }
        public int? HorizontalPositionOffset { get { return _horizontalPositionOffset; } set { _horizontalPositionOffset = value; } }
        public string HorizontalAlignment { get { return _horizontalAlignment; } set { _horizontalAlignment = value; } }
        public DW.VerticalRelativePositionValues VerticalRelativeFrom { get { return _verticalRelativeFrom; } set { _verticalRelativeFrom = value; } }
        public int? VerticalPositionOffset { get { return _verticalPositionOffset; } set { _verticalPositionOffset = value; } }
        public string VerticalAlignment { get { return _verticalAlignment; } set { _verticalAlignment = value; } }

        public OXmlAnchorPictureDrawing()
        {
            DrawingMode = OXmlPictureDrawingMode.Anchor;
        }

        public override string ToString()
        {
            return $"anchor drawing : {_wrap} h.from {_horizontalRelativeFrom} h.pos {_horizontalPositionOffset} v.from {_verticalRelativeFrom} v.pos {_verticalPositionOffset}";
        }
    }

    public enum OXmlAnchorWrapType
    {
        WrapNone = 0,
        WrapSquare,
        WrapTight,
        WrapThrough,
        WrapTopAndBottom
    }

    public abstract class OXmlAnchorWrap
    {
        public OXmlAnchorWrapType WrapType;
    }

    public class OXmlAnchorWrapSquare : OXmlAnchorWrap
    {
        // missing parameters : uint distanceFromTop = 0, uint distanceFromBottom = 0, uint distanceFromLeft = 0, uint distanceFromRight = 0, DW.EffectExtent effectExtent = null
        //private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;
        private DW.WrapTextValues _wrapText = DW.WrapTextValues.BothSides;
        private uint _distanceFromTop = 0;
        private uint _distanceFromBottom = 0;
        private uint _distanceFromLeft = 0;
        private uint _distanceFromRight = 0;
        //private DW.EffectExtent _effectExtent = null;
        private OXmlEffectExtent _effectExtent = null;

        public DW.WrapTextValues WrapText { get { return _wrapText; } set { _wrapText = value; } }
        public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        public uint DistanceFromLeft { get { return _distanceFromLeft; } set { _distanceFromLeft = value; } }
        public uint DistanceFromRight { get { return _distanceFromRight; } set { _distanceFromRight = value; } }
        public OXmlEffectExtent EffectExtent { get { return _effectExtent; } set { _effectExtent = value; } }

        public OXmlAnchorWrapSquare()
        {
            WrapType = OXmlAnchorWrapType.WrapSquare;
        }

        public override string ToString()
        {
            return "wrap square";
        }
    }

    public class OXmlAnchorWrapTight : OXmlAnchorWrap
    {
        //private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;
        private DW.WrapTextValues _wrapText = DW.WrapTextValues.BothSides;
        private uint _distanceFromLeft = 0;
        private uint _distanceFromRight = 0;
        //private int _squareSize = 0;
        //private DW.WrapPolygon _wrapPolygon = null;
        private OXmlPolygonBase _wrapPolygon = null;

        public DW.WrapTextValues WrapText { get { return _wrapText; } set { _wrapText = value; } }
        public uint DistanceFromLeft { get { return _distanceFromLeft; } set { _distanceFromLeft = value; } }
        public uint DistanceFromRight { get { return _distanceFromRight; } set { _distanceFromRight = value; } }
        //public int SquareSize { get { return _squareSize; } set { _squareSize = value; } }
        public OXmlPolygonBase WrapPolygon { get { return _wrapPolygon; } set { _wrapPolygon = value; } }

        public OXmlAnchorWrapTight()
        {
            WrapType = OXmlAnchorWrapType.WrapTight;
        }

        public override string ToString()
        {
            return "wrap tight";
        }
    }

    public class OXmlAnchorWrapTopAndBottom : OXmlAnchorWrap
    {
        private uint _distanceFromTop = 0;
        private uint _distanceFromBottom = 0;
        //private DW.EffectExtent _effectExtent = null;
        private OXmlEffectExtent _effectExtent = null;

        public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        public OXmlEffectExtent EffectExtent { get { return _effectExtent; } set { _effectExtent = value; } }

        public OXmlAnchorWrapTopAndBottom()
        {
            WrapType = OXmlAnchorWrapType.WrapTopAndBottom;
        }

        public override string ToString()
        {
            return "wrap top and bottom";
        }
    }

    // used in OXmlAnchorWrapSquare, OXmlAnchorWrapTopAndBottom
    // data of DW.EffectExtent
    public class OXmlEffectExtent
    {
        public long? TopEdge;
        public long? BottomEdge;
        public long? LeftEdge;
        public long? RightEdge;

        public DW.EffectExtent ToEffectExtent()
        {
            DW.EffectExtent effectExtent = new DW.EffectExtent();
            if (TopEdge != null)
                effectExtent.TopEdge = TopEdge;
            if (BottomEdge != null)
                effectExtent.BottomEdge = BottomEdge;
            if (LeftEdge != null)
                effectExtent.LeftEdge = LeftEdge;
            if (RightEdge != null)
                effectExtent.RightEdge = RightEdge;
            return effectExtent;
        }
    }

    // used in OXmlAnchorWrapTight
    public abstract class OXmlPolygonBase
    {
        public abstract DW.WrapPolygon ToWrapPolygon();
    }

    // data of DW.WrapPolygon
    public class OXmlPolygon : OXmlPolygonBase
    {
        public bool? Edited;
        public OXmlPoint2DType StartPoint;
        public OXmlPoint2DType[] LinesTo;

        public override DW.WrapPolygon ToWrapPolygon()
        {
            DW.WrapPolygon polygon = new DW.WrapPolygon();
            if (Edited != null)
                polygon.Edited = Edited;
            long startPointX = 0;
            long startPointY = 0;
            if (StartPoint != null)
            {
                startPointX = StartPoint.X;
                startPointY = StartPoint.Y;
            }
            polygon.StartPoint = new DW.StartPoint { X = startPointX, Y = startPointY };
            if (LinesTo.Length < 2)
                throw new PBException("polygon must have at least 2 lines");
            polygon.Append(LinesTo.Select(lineTo => new ODW.LineTo() { X = lineTo.X, Y = lineTo.Y }));
            OXmlPoint2DType lastPoint = LinesTo[LinesTo.Length - 1];
            if (lastPoint.X != startPointX || lastPoint.Y != startPointY)
                polygon.AppendChild(new ODW.LineTo() { X = startPointX, Y = startPointY });
            return polygon;
        }
    }

    public class OXmlSquare : OXmlPolygonBase
    {
        public bool? Edited;
        public OXmlPoint2DType StartPoint;
        public long HorizontalSize;
        public long VerticalSize;

        public override DW.WrapPolygon ToWrapPolygon()
        {
            DW.WrapPolygon polygon = new DW.WrapPolygon();
            if (Edited != null)
                polygon.Edited = Edited;
            polygon.StartPoint = new DW.StartPoint { X = StartPoint.X, Y = StartPoint.Y };
            long horizontalSize;
            long verticalSize;
            if (HorizontalSize == 0)
            {
                if (VerticalSize == 0)
                    throw new PBException("horizontal size and vertical size can't both be 0");
                horizontalSize = VerticalSize;
                verticalSize = VerticalSize;
            }
            else if (VerticalSize == 0)
            {
                horizontalSize = HorizontalSize;
                verticalSize = HorizontalSize;
            }
            else
            {
                horizontalSize = HorizontalSize;
                verticalSize = VerticalSize;
            }
            long x = StartPoint.X;
            long y = StartPoint.Y;
            // Wrapping Polygon Line End Position, <wp:lineTo x="0" y="0"/>
            y += verticalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
            x += horizontalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
            y -= verticalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
            x -= horizontalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
            return polygon;
        }
    }

    // used in OXmlPolygon
    public class OXmlPoint2DType
    {
        public long X;
        public long Y;
    }

    //public class zDocXPictureWrapSquareAnchorDrawing : zDocXPictureAnchorDrawing
    //{
    //    private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;

    //    public EnumValue<DW.WrapTextValues> WrapText { get { return _wrapText; } set { _wrapText = value; } }

    //    public zDocXPictureWrapSquareAnchorDrawing()
    //    {
    //        DrawingMode = zDocXPictureDrawingMode.AnchorWrapSquare;
    //    }
    //}

    //public class zDocXPictureWrapTightAnchorDrawing : zDocXPictureAnchorDrawing
    //{
    //    private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;
    //    private int _squareSize = 0;

    //    public EnumValue<DW.WrapTextValues> WrapText { get { return _wrapText; } set { _wrapText = value; } }
    //    public int SquareSize { get { return _squareSize; } set { _squareSize = value; } }

    //    public zDocXPictureWrapTightAnchorDrawing()
    //    {
    //        DrawingMode = zDocXPictureDrawingMode.AnchorWrapTight;
    //    }
    //}

    //public class zDocXPictureWrapTopAndBottomAnchorDrawing : zDocXPictureAnchorDrawing
    //{
    //    private uint _distanceFromTop = 0;
    //    private uint _distanceFromBottom = 0;
    //    private DW.EffectExtent _effectExtent = null;

    //    public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
    //    public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
    //    public DW.EffectExtent EffectExtent { get { return _effectExtent; } set { _effectExtent = value; } }

    //    public zDocXPictureWrapTopAndBottomAnchorDrawing()
    //    {
    //        DrawingMode = zDocXPictureDrawingMode.AnchorWrapTopBottom;
    //    }
    //}
}

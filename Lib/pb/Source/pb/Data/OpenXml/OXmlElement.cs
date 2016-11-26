using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;

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
        DocDefaults,
        OpenHeaderFooter,
        CloseHeaderFooter,
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
    public enum OXmlDocDefaultsType
    {
        RunPropertiesDefault = 0,
        ParagraphPropertiesDefault
    }

    public class OXmlDocDefaultsElement : OXmlElement
    {
        public OXmlDocDefaultsType DocDefaultsType;

        public OXmlDocDefaultsElement()
        {
            Type = OXmlElementType.DocDefaults;
        }

        public override string ToString()
        {
            return "DocDefaults";
        }
    }

    public class OXmlRunPropertiesDefaultElement : OXmlDocDefaultsElement
    {
        private RunFonts _runFonts = null;
        private FontSize _fontSize = null;

        public OXmlRunPropertiesDefaultElement()
        {
            DocDefaultsType = OXmlDocDefaultsType.RunPropertiesDefault;
        }

        public RunFonts RunFonts { get { return _runFonts; } set { _runFonts = value; } }
        public FontSize FontSize { get { return _fontSize; } set { _fontSize = value; } }

        public override string ToString()
        {
            return $"RunPropertiesDefault : font \"{_runFonts.Ascii}\" size {_fontSize.Val}";
        }
    }

    public class OXmlParagraphPropertiesDefaultElement : OXmlDocDefaultsElement
    {
        public OXmlParagraphPropertiesDefaultElement()
        {
            DocDefaultsType = OXmlDocDefaultsType.ParagraphPropertiesDefault;
        }

        public override string ToString()
        {
            return "ParagraphPropertiesDefault";
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

    // OXmlElementType.OpenHeaderFooter
    public class OXmlOpenHeaderFooter : OXmlElement
    {
        public bool Header;                        // true Header, false Footer
        public HeaderFooterValues HeaderType;      // Default, First, Even

        public OXmlOpenHeaderFooter()
        {
            Type = OXmlElementType.OpenHeaderFooter;
        }

        public override string ToString()
        {
            string s = Header ? "header" : "footer";
            return $"OpenHeaderFooter : {s} {HeaderType}";
        }
    }

    // OXmlElementType.Style
    public class OXmlStyleElement : OXmlElement
    {
        //public OXmlDocDefaultsType DocDefaultsType;
        public string Id = null;                                     // Style ID
        public string Name = null;                                   // Primary Style Name
        public StyleValues StyleType;                                // Paragraph, Character, Table, Numbering
        public string Aliases;                                       // Alternate Style Names
        public bool CustomStyle;                                     // User-Defined Style
        public bool DefaultStyle;                                    // Default Style
        public bool? Locked;                                          // Style Cannot Be Applied
        public bool? SemiHidden;                                      // Hide Style From Main User Interface
        public bool? StyleHidden;                                     // Hide Style From User Interface
        public bool? UnhideWhenUsed;                                  // Remove Semi-Hidden Property When Style Is Used
        public int? UIPriority;                                      // Optional User Interface Sorting Order
        public string LinkedStyle;                                   // Linked Style Reference
        public string BasedOn;                                       // Parent Style ID
        public string NextParagraphStyle;                            // Style For Next Paragraph
        public StyleParagraphProperties StyleParagraphProperties;    // Style Paragraph Properties
        public StyleRunProperties StyleRunProperties;                // Run Properties

        public OXmlStyleElement()
        {
            Type = OXmlElementType.Style;
        }

        public override string ToString()
        {
            return $"Style : id {Id} name \"{Name}\" type {StyleType}";
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
        public EnumValue<A.BlipCompressionValues> CompressionState = A.BlipCompressionValues.Print;
        public EnumValue<A.ShapeTypeValues> PresetShape = A.ShapeTypeValues.Rectangle;
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

    public enum OXmlPictureDrawingMode
    {
        Inline = 0,
        Anchor
    }

    public class OXmlPictureDrawing
    {
        public OXmlPictureDrawingMode DrawingMode;
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
        private EnumValue<DW.HorizontalRelativePositionValues> _horizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin;
        private int? _horizontalPositionOffset = null;   // pixel unit
        private string _horizontalAlignment = null;      // left, right, center, inside, outside
        private EnumValue<DW.VerticalRelativePositionValues> _verticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph;
        private int? _verticalPositionOffset = null;     // pixel unit
        private string _verticalAlignment = null;        // top, bottom, center, inside, outside

        public OXmlAnchorWrap Wrap { get { return _wrap; } set { _wrap = value; } }
        public EnumValue<DW.HorizontalRelativePositionValues> HorizontalRelativeFrom { get { return _horizontalRelativeFrom; } set { _horizontalRelativeFrom = value; } }
        public int? HorizontalPositionOffset { get { return _horizontalPositionOffset; } set { _horizontalPositionOffset = value; } }
        public string HorizontalAlignment { get { return _horizontalAlignment; } set { _horizontalAlignment = value; } }
        public EnumValue<DW.VerticalRelativePositionValues> VerticalRelativeFrom { get { return _verticalRelativeFrom; } set { _verticalRelativeFrom = value; } }
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
        WrapTopBottom
    }

    public abstract class OXmlAnchorWrap
    {
        public OXmlAnchorWrapType WrapType;
    }

    public class OXmlAnchorWrapSquare : OXmlAnchorWrap
    {
        // missing parameters : uint distanceFromTop = 0, uint distanceFromBottom = 0, uint distanceFromLeft = 0, uint distanceFromRight = 0, DW.EffectExtent effectExtent = null
        private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;
        private uint _distanceFromTop = 0;
        private uint _distanceFromBottom = 0;
        private uint _distanceFromLeft = 0;
        private uint _distanceFromRight = 0;
        private DW.EffectExtent _effectExtent = null;

        public EnumValue<DW.WrapTextValues> WrapText { get { return _wrapText; } set { _wrapText = value; } }
        public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        public uint DistanceFromLeft { get { return _distanceFromLeft; } set { _distanceFromLeft = value; } }
        public uint DistanceFromRight { get { return _distanceFromRight; } set { _distanceFromRight = value; } }
        public DW.EffectExtent EffectExtent { get { return _effectExtent; } set { _effectExtent = value; } }

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
        private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;
        private uint _distanceFromLeft = 0;
        private uint _distanceFromRight = 0;
        //private int _squareSize = 0;
        private DW.WrapPolygon _wrapPolygon = null;

        public EnumValue<DW.WrapTextValues> WrapText { get { return _wrapText; } set { _wrapText = value; } }
        public uint DistanceFromLeft { get { return _distanceFromLeft; } set { _distanceFromLeft = value; } }
        public uint DistanceFromRight { get { return _distanceFromRight; } set { _distanceFromRight = value; } }
        //public int SquareSize { get { return _squareSize; } set { _squareSize = value; } }
        public DW.WrapPolygon WrapPolygon { get { return _wrapPolygon; } set { _wrapPolygon = value; } }

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
        private DW.EffectExtent _effectExtent = null;

        public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        public DW.EffectExtent EffectExtent { get { return _effectExtent; } set { _effectExtent = value; } }

        public OXmlAnchorWrapTopAndBottom()
        {
            WrapType = OXmlAnchorWrapType.WrapTopBottom;
        }

        public override string ToString()
        {
            return "wrap top and bottom";
        }
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

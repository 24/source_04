using DocumentFormat.OpenXml;
using System.Drawing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;

//namespace Test.Test_OpenXml
namespace pb.Data.OpenXml
{
    public enum zDocXElementType
    {
        //BeginParagraph,
        //EndParagraph,
        Paragraph = 0,
        Text,
        Line,
        Picture
    }

    public class zDocXElement
    {
        public zDocXElementType Type;
    }

    public class zDocXTextElement : zDocXElement
    {
        public string Text;

        public zDocXTextElement()
        {
            Type = zDocXElementType.Text;
        }
    }

    public enum zDocXPictureDrawingMode
    {
        Inline = 0,
        AnchorWrapNone,
        AnchorWrapSquare,
        AnchorWrapTight,
        AnchorWrapThrough,
        AnchorWrapTopBottom
    }

    public class zDocXPictureElement : zDocXElement
    {
        public string File;
        public Image Image;
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
        public zDocXPictureDrawing PictureDrawing;

        public zDocXPictureElement()
        {
            Type = zDocXElementType.Picture;
        }
    }

    public class zDocXPictureDrawing
    {
        public zDocXPictureDrawingMode DrawingMode;
    }

    public class zDocXPictureInlineDrawing : zDocXPictureDrawing
    {
        public zDocXPictureInlineDrawing()
        {
            DrawingMode = zDocXPictureDrawingMode.Inline;
        }
    }

    public class zDocXPictureAnchorDrawing : zDocXPictureDrawing
    {
        private EnumValue<DW.HorizontalRelativePositionValues> _horizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin;
        private int _horizontalPositionOffset = 0;   // pixel unit
        private EnumValue<DW.VerticalRelativePositionValues> _verticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph;
        private int _verticalPositionOffset = 0;    // pixel unit

        public EnumValue<DW.HorizontalRelativePositionValues> HorizontalRelativeFrom { get { return _horizontalRelativeFrom; } set { _horizontalRelativeFrom = value; } }
        public int HorizontalPositionOffset { get { return _horizontalPositionOffset; } set { _horizontalPositionOffset = value; } }
        public EnumValue<DW.VerticalRelativePositionValues> VerticalRelativeFrom { get { return _verticalRelativeFrom; } set { _verticalRelativeFrom = value; } }
        public int VerticalPositionOffset { get { return _verticalPositionOffset; } set { _verticalPositionOffset = value; } }
    }

    public class zDocXPictureWrapSquareAnchorDrawing : zDocXPictureAnchorDrawing
    {
        private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;

        public EnumValue<DW.WrapTextValues> WrapText { get { return _wrapText; } set { _wrapText = value; } }

        public zDocXPictureWrapSquareAnchorDrawing()
        {
            DrawingMode = zDocXPictureDrawingMode.AnchorWrapSquare;
        }
    }

    public class zDocXPictureWrapTightAnchorDrawing : zDocXPictureAnchorDrawing
    {
        private EnumValue<DW.WrapTextValues> _wrapText = DW.WrapTextValues.BothSides;
        private int _squareSize = 0;

        public EnumValue<DW.WrapTextValues> WrapText { get { return _wrapText; } set { _wrapText = value; } }
        public int SquareSize { get { return _squareSize; } set { _squareSize = value; } }

        public zDocXPictureWrapTightAnchorDrawing()
        {
            DrawingMode = zDocXPictureDrawingMode.AnchorWrapTight;
        }
    }

    public class zDocXPictureWrapTopAndBottomAnchorDrawing : zDocXPictureAnchorDrawing
    {
        private uint _distanceFromTop = 0;
        private uint _distanceFromBottom = 0;
        private DW.EffectExtent _effectExtent = null;

        public uint DistanceFromTop { get { return _distanceFromTop; } set { _distanceFromTop = value; } }
        public uint DistanceFromBottom { get { return _distanceFromBottom; } set { _distanceFromBottom = value; } }
        public DW.EffectExtent EffectExtent { get { return _effectExtent; } set { _effectExtent = value; } }

        public zDocXPictureWrapTopAndBottomAnchorDrawing()
        {
            DrawingMode = zDocXPictureDrawingMode.AnchorWrapTopBottom;
        }
    }
}

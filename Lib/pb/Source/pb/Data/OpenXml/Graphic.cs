using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace pb.Data.OpenXml
{
    // Graphic Object, <a:graphic>
    // child : GraphicData <a:graphicData>
    public class Graphic
    {
        private string _embeddedReference = null;
        private long _width;
        private long _height;
        private int _rotation = 0;
        private bool _horizontalFlip = false;
        private bool _verticalFlip = false;
        private EnumValue<A.BlipCompressionValues> _compressionState = A.BlipCompressionValues.Print;
        private EnumValue<A.ShapeTypeValues> _presetShape = A.ShapeTypeValues.Rectangle;

        public int Rotation { get { return _rotation; } set { _rotation = value; } }
        public bool HorizontalFlip { get { return _horizontalFlip; } set { _horizontalFlip = value; } }
        public bool VerticalFlip { get { return _verticalFlip; } set { _verticalFlip = value; } }
        public EnumValue<A.BlipCompressionValues> CompressionState { get { return _compressionState; } set { _compressionState = value; } }
        public EnumValue<A.ShapeTypeValues> PresetShape { get { return _presetShape; } set { _presetShape = value; } }

        public Graphic(string embeddedReference, long width, long height)
        {
            _embeddedReference = embeddedReference;
            _width = width;
            _height = height;
        }

        public A.Graphic Create()
        {
            // Graphic Object, <a:graphic>
            A.Graphic graphic = new A.Graphic();

            // Graphic Object Data, <a:graphicData>
            A.GraphicData graphicData = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };

            // Picture, <pic:pic>
            //   possible child : NonVisualPictureProperties <pic:nvPicPr>, BlipFill <pic:blipFill>, ShapeProperties <pic:spPr>,
            //     DocumentFormat.OpenXml.Office2010.Drawing.Pictures.ShapeStyle <pic14:style>, DocumentFormat.OpenXml.Office2010.Drawing.Pictures.OfficeArtExtensionList <pic14:extLst>
            PIC.Picture picture = new PIC.Picture();

            // Non-Visual Picture Properties, <pic:nvPicPr>
            picture.NonVisualPictureProperties = new PIC.NonVisualPictureProperties();
            // Non-Visual Drawing Properties, <pic:cNvPr>
            picture.NonVisualPictureProperties.NonVisualDrawingProperties = new PIC.NonVisualDrawingProperties() { Id = 0, Name = "Image" };
            // Non-Visual Picture Drawing Properties, <pic:cNvPicPr>
            picture.NonVisualPictureProperties.NonVisualPictureDrawingProperties = new PIC.NonVisualPictureDrawingProperties();

            // Picture Fill, <pic:blipFill>
            picture.BlipFill = new PIC.BlipFill();

            // Blip, <a:blip>
            picture.BlipFill.Blip = new A.Blip() { Embed = _embeddedReference, CompressionState = _compressionState };

            // <a:extLst>
            //A.BlipExtensionList blipExtensions = new A.BlipExtensionList();
            // <a:ext>
            //blipExtensions.AppendChild(new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" });
            //picture.BlipFill.Blip.AppendChild(blipExtensions);

            // Source Rectangle, <a:srcRect>
            picture.BlipFill.SourceRectangle = new A.SourceRectangle() { Bottom = 0, Left = 0, Right = 0, Top = 0 };

            // Stretch, <a:stretch>
            A.Stretch stretch = new A.Stretch();
            // Fill Rectangle, <a:fillRect>
            stretch.AppendChild(new A.FillRectangle());
            picture.BlipFill.AppendChild(stretch);

            // Shape Properties, <pic:spPr>
            picture.ShapeProperties = new PIC.ShapeProperties();

            // Transform2D, <a:xfrm>
            picture.ShapeProperties.Transform2D = new A.Transform2D();
            if (_rotation != 0)
                // Rotation, <a:xfrm rot>
                picture.ShapeProperties.Transform2D.Rotation = _rotation;
            if (_horizontalFlip)
                // Horizontal Flip, <a:xfrm flipH>
                picture.ShapeProperties.Transform2D.HorizontalFlip = _horizontalFlip;
            if (_verticalFlip)
                // Vertical Flip, <a:xfrm flipV>
                picture.ShapeProperties.Transform2D.VerticalFlip = _verticalFlip;
            // Offset, <a:off>
            picture.ShapeProperties.Transform2D.Offset = new A.Offset() { X = 0, Y = 0 };
            // <a:ext>
            picture.ShapeProperties.Transform2D.Extents = new A.Extents() { Cx = _width, Cy = _height };

            // Preset geometry, <a:prstGeom>
            // Preset Shape, <a:prstGeom prst>
            A.PresetGeometry presetGeometry = new A.PresetGeometry() { Preset = _presetShape };
            // Shape Adjust Values, <a:avLst>
            presetGeometry.AppendChild(new A.AdjustValueList());
            picture.ShapeProperties.AppendChild(presetGeometry);

            graphicData.AppendChild(picture);

            graphic.AppendChild(graphicData);

            return graphic;
        }
    }
}

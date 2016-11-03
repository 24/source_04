using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using pb.IO;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

// doc
//   - emus :
//     English Metric Units and Open XML http://polymathprogrammer.com/2009/10/22/english-metric-units-and-open-xml/
//     there are 914400 EMUs per inch, and there are 96 pixels to an inch
//     EMU = pixel * 914400 / 96
//     EMU = pixel * 9525

//namespace Test.Test_OpenXml
namespace pb.Data.OpenXml
{
    public class zOpenXmlDoc
    {
        private WordprocessingDocument _document = null;
        private MainDocumentPart _mainPart = null;
        private Body _body = null;
        private Paragraph _paragraph = null;
        private Run _run = null;
        private uint _pictureId = 0;

        private void _Create(string file, IEnumerable<zDocXElement> elements)
        {
            //DocumentFormat.OpenXml.FileFormatVersions
            using (_document = WordprocessingDocument.Create(file, WordprocessingDocumentType.Document))
            {
                _mainPart = _document.AddMainDocumentPart();
                _mainPart.Document = new Document();
                zOpenXmlPicture.AddNamespaceDeclarations(_mainPart.Document);
                _body = _mainPart.Document.AppendChild(new Body());

                foreach (zDocXElement element in elements)
                {
                    switch (element.Type)
                    {
                        case zDocXElementType.Paragraph:
                            AddParagraph();
                            break;
                        case zDocXElementType.Text:
                            AddText(element);
                            break;
                        case zDocXElementType.Line:
                            AddLine();
                            break;
                        case zDocXElementType.Picture:
                            AddPicture(element);
                            break;
                    }
                }
            }
        }

        private void AddParagraph()
        {
            _paragraph = _body.AppendChild(new Paragraph());
            _run = _paragraph.AppendChild(new Run());
        }

        private void AddText(zDocXElement element)
        {
            if (!(element is zDocXTextElement))
                throw new PBException("text element must be a zDocXElementText");
            if (_paragraph == null)
                AddParagraph();
            _run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text(((zDocXTextElement)element).Text));
        }

        private void AddLine()
        {
            if (_paragraph == null)
                AddParagraph();
            _run.AppendChild(new Break());
        }

        private void AddPicture(zDocXElement element)
        {
            if (!(element is zDocXPictureElement))
                throw new PBException("picture element must be a zDocXElementPicture");
            if (_paragraph == null)
                AddParagraph();
            zDocXPictureElement pictureElement = (zDocXPictureElement)element;


            // Drawing : child Inline, Anchor
            // <w:drawing>
            //Drawing drawing = new Drawing(drawingMode);
            //_run.AppendChild(drawing);

            // Drawing : child Inline, Anchor
            // <w:drawing>
            _run.AppendChild(new Drawing(zOpenXmlPicture.Create(_mainPart, pictureElement, ++_pictureId)));
        }

        public static void Create(string file, IEnumerable<zDocXElement> elements)
        {
            new zOpenXmlDoc()._Create(file, elements);
        }
    }

    public class zOpenXmlPicture
    {
        private const int _emusInPixel = 9525;
        private MainDocumentPart _mainPart;
        private zDocXPictureElement _pictureElement;
        private string _embeddedReference = null;
        private uint _id;
        private string _file;
        private string _name = null;
        //private string _pictureDescription;
        private int? _width;
        private int? _height;
        private long _emuWidth;
        private long _emuHeight;
        private zDocXPictureDrawing _pictureDrawing;

        private zOpenXmlPicture(MainDocumentPart mainPart, zDocXPictureElement pictureElement, uint pictureId)
        {
            _mainPart = mainPart;
            _pictureElement = pictureElement;
            _id = pictureId;
            _file = pictureElement.File;
            //_pictureDescription = pictureElement.Description;
            _width = pictureElement.Width;
            _height = pictureElement.Height;
            _pictureDrawing = pictureElement.PictureDrawing;
        }

        private OpenXmlCompositeElement _Create()
        {
            //if (_mainPart.Document.LookupNamespace())
            //_mainPart.Document.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            //_mainPart.Document.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            //_mainPart.Document.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            //_mainPart.Document.AddNamespaceDeclaration("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");
            //_mainPart.Document.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            SetWidthHeight();
            Trace.WriteLine("add picture \"{0}\" in pixel width {1} height {2} in emu width {3} height {4}", zPath.GetFileName(_file), _width, _height, _emuWidth, _emuHeight);

            ImagePart imagePart = _mainPart.AddImagePart(GetImagePartType(zPath.GetExtension(_file)));
            _embeddedReference = _mainPart.GetIdOfPart(imagePart);

            using (FileStream stream = new FileStream(_file, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            //uint id = ++_pictureId;
            _name = "Picture" + _id.ToString();

            OpenXmlCompositeElement drawingMode = null;
            //if (pictureElement.DrawingMode == zDocXPictureDrawingMode.Inline)
            if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.Inline)
                //drawingMode = CreateInlineDrawing(_mainPart.GetIdOfPart(imagePart), id, name, pictureElement.Description, pictureElement.Width, pictureElement.Height);
                //drawingMode = CreateInlineDrawing_v2(_mainPart.GetIdOfPart(imagePart), _id, name, _pictureElement.Description, width, height);
                drawingMode = CreateInlineDrawing_v2();
            else if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.AnchorWrapSquare)
                //drawingMode = CreateAnchorWrapSquareDrawing(_mainPart.GetIdOfPart(imagePart), _pictureId, name, _pictureElement.Description, width, height, (zDocXPictureDrawingAnchorWrapSquare)_pictureElement.PictureDrawing);
                drawingMode = CreateAnchorWrapSquareDrawing();
            else if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.AnchorWrapTight)
                drawingMode = CreateAnchorWrapTightDrawing();
            else if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.AnchorWrapTopBottom)
                drawingMode = CreateAnchorWrapTopAndBottomDrawing();
            else
                throw new PBException("unknow drawing mode {0}", _pictureDrawing.DrawingMode);
            return drawingMode;
        }

        //drawingMode = CreateAnchorWrapSquareDrawing(_mainPart.GetIdOfPart(imagePart), ++_pictureId, pictureElement.Width, pictureElement.Height, pictureElement.Description, (zDocXPictureDrawingAnchorWrapSquare)pictureElement.PictureDrawing);
        //private static DW.Anchor CreateAnchorWrapSquareDrawing(string embeddedReference, uint id, string name, string description, long width, long height, zDocXPictureDrawingAnchorWrapSquare pictureDrawing)
        private DW.Anchor CreateAnchorWrapSquareDrawing()
        {
            AnchorDrawing drawing = CreateAnchorDrawing();

            drawing.SetWrapSquare(((zDocXPictureWrapSquareAnchorDrawing)_pictureDrawing).WrapText);

            return drawing.Create();
        }

        private DW.Anchor CreateAnchorWrapTightDrawing()
        {
            AnchorDrawing drawing = CreateAnchorDrawing();

            zDocXPictureWrapTightAnchorDrawing pictureWrapTightAnchor = (zDocXPictureWrapTightAnchorDrawing)_pictureDrawing;
            // (long)pictureWrapTightAnchor.SquareSize * _emusInPixel
            drawing.SetWrapTight(pictureWrapTightAnchor.WrapText, AnchorDrawing.CreateSquareWrapPolygon(pictureWrapTightAnchor.SquareSize));

            return drawing.Create();
        }

        private DW.Anchor CreateAnchorWrapTopAndBottomDrawing()
        {
            AnchorDrawing drawing = CreateAnchorDrawing();

            zDocXPictureWrapTopAndBottomAnchorDrawing pictureWrapTopAndBottomAnchor = (zDocXPictureWrapTopAndBottomAnchorDrawing)_pictureDrawing;
            drawing.SetWrapTopAndBottom(pictureWrapTopAndBottomAnchor.DistanceFromTop, pictureWrapTopAndBottomAnchor.DistanceFromBottom, pictureWrapTopAndBottomAnchor.EffectExtent);

            return drawing.Create();
        }

        private AnchorDrawing CreateAnchorDrawing()
        {
            AnchorDrawing drawing = new AnchorDrawing(_embeddedReference, _id, _name, _emuWidth, _emuHeight, _pictureElement.Description);
            drawing.Rotation = _pictureElement.Rotation;
            drawing.HorizontalFlip = _pictureElement.HorizontalFlip;
            drawing.VerticalFlip = _pictureElement.VerticalFlip;
            drawing.CompressionState = _pictureElement.CompressionState;
            drawing.PresetShape = _pictureElement.PresetShape;

            zDocXPictureAnchorDrawing anchorDrawing = (zDocXPictureAnchorDrawing)_pictureDrawing;
            drawing.SetHorizontalPosition(anchorDrawing.HorizontalRelativeFrom, (long)anchorDrawing.HorizontalPositionOffset * _emusInPixel);
            drawing.SetVerticalPosition(anchorDrawing.VerticalRelativeFrom, (long)anchorDrawing.VerticalPositionOffset * _emusInPixel);

            return drawing;
        }

        //private static DW.Inline CreateInlineDrawing_v2(string embeddedReference, uint id, string name, string description, long width, long height)
        private DW.Inline CreateInlineDrawing_v2()
        {
            // DW.Inline.EditId : ??? (<wp:inline wp14:editId="">)

            //if (_name == null)
            //    throw new PBException("missing image name");
            //if (width != null && height == null)
            //    throw new PBException("missing height (width has a value)");
            //if (width == null && height != null)
            //    throw new PBException("missing width (height has a value)");

            // <wp:inline>
            DW.Inline inline = new DW.Inline()
            {
                DistanceFromTop = 0,          // distT
                DistanceFromBottom = 0,       // distB
                DistanceFromLeft = 0,         // distL
                DistanceFromRight = 0         // distR
                //EditId = "50D07946"         // wp14:editId
            };

            // <wp:extent>
            inline.AppendChild(new DW.Extent() { Cx = _emuWidth, Cy = _emuHeight });
            // <wp:effectExtent>
            inline.AppendChild(new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L });
            // <wp:docPr>
            // Id = (UInt32Value)1U, Name = "Picture 1"
            inline.AppendChild(new DW.DocProperties() { Id = _id, Name = _name, Description = _pictureElement.Description });
            // <wp:cNvGraphicFramePr>, <a:graphicFrameLocks>          { NoChangeAspect = true }
            inline.AppendChild(new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks()));

            // <a:graphic>
            A.Graphic graphic = new A.Graphic();
            // <a:graphicData>
            A.GraphicData graphicData = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };
            // <pic:pic>
            PIC.Picture picture = new PIC.Picture();

            // <pic:nvPicPr>
            PIC.NonVisualPictureProperties pictureProperties = new PIC.NonVisualPictureProperties();
            // <pic:cNvPr>
            pictureProperties.AppendChild(new PIC.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "Image" });
            // <pic:cNvPicPr>
            pictureProperties.AppendChild(new PIC.NonVisualPictureDrawingProperties());
            picture.AppendChild(pictureProperties);

            // <pic:blipFill>
            PIC.BlipFill blipFill = new PIC.BlipFill();

            // <a:blip>
            // CompressionState = A.BlipCompressionValues.Print
            A.Blip blip = new A.Blip() { Embed = _embeddedReference, CompressionState = _pictureElement.CompressionState };

            // $$pb todo comment
            // <a:extLst>
            A.BlipExtensionList blipExtensions = new A.BlipExtensionList();
            // <a:ext>
            blipExtensions.AppendChild(new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" });
            blip.AppendChild(blipExtensions);
            // $$pb todo comment end

            blipFill.AppendChild(blip);

            // <a:stretch>
            A.Stretch stretch = new A.Stretch();
            // <a:fillRect>
            stretch.AppendChild(new A.FillRectangle());
            blipFill.AppendChild(stretch);

            picture.AppendChild(blipFill);

            // <pic:spPr>
            PIC.ShapeProperties shapeProperties = new PIC.ShapeProperties();

            // <a:xfrm>
            A.Transform2D transform2D = new A.Transform2D();
            // new A.Offset
            // <a:off>
            transform2D.AppendChild(new A.Offset() { X = 0L, Y = 0L });
            // <a:ext>
            transform2D.AppendChild(new A.Extents() { Cx = _emuWidth, Cy = _emuHeight });
            shapeProperties.AppendChild(transform2D);

            // <a:prstGeom>
            // Preset = A.ShapeTypeValues.Rectangle
            A.PresetGeometry presetGeometry = new A.PresetGeometry() { Preset = _pictureElement.PresetShape };
            // <a:avLst>
            presetGeometry.AppendChild(new A.AdjustValueList());
            shapeProperties.AppendChild(presetGeometry);

            picture.AppendChild(shapeProperties);

            graphicData.AppendChild(picture);
            graphic.AppendChild(graphicData);
            inline.AppendChild(graphic);

            return inline;
        }

        //private static DW.Inline CreateInlineDrawing(string embeddedReference, uint id, string name, string description, long width, long height)
        private DW.Inline CreateInlineDrawing()
        {
            // DW.Inline.EditId : ??? (<wp:inline wp14:editId="">)

            if (_name == null)
                throw new PBException("missing image name");
            //if (width != null && height == null)
            //    throw new PBException("missing image height (width has a value)");
            //if (width == null && height != null)
            //    throw new PBException("missing image width (height has a value)");

            return new DW.Inline(                                                    // <wp:inline>
                new DW.Extent()                                                      // <wp:extent>
                {
                    Cx = _emuWidth,
                    Cy = _emuHeight
                },
                new DW.EffectExtent()                                                // <wp:effectExtent>
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new DW.DocProperties()                                               // <wp:docPr>
                {
                    Id = _id,
                    Name = _name,
                    Description = _pictureElement.Description
                },
                new DW.NonVisualGraphicFrameDrawingProperties(                       // <wp:cNvGraphicFramePr>
                    new A.GraphicFrameLocks()                                        // <a:graphicFrameLocks>          { NoChangeAspect = true }
                ),
                new A.Graphic(                                                       // <a:graphic>
                    new A.GraphicData(                                               // <a:graphicData>
                        new PIC.Picture(                                             // <pic:pic>
                            new PIC.NonVisualPictureProperties(                      // <pic:nvPicPr>
                                new PIC.NonVisualDrawingProperties()                 // <pic:cNvPr>
                                {
                                    Id = (UInt32Value)0U,
                                    Name = "Image"
                                },
                                new PIC.NonVisualPictureDrawingProperties()          // <pic:cNvPicPr>
                            ),
                            new PIC.BlipFill(                                        // <pic:blipFill>
                                new A.Blip(                                          // <a:blip>
                                                                                     // $$pb todo comment
                                    new A.BlipExtensionList(                       // <a:extLst>
                                        new A.BlipExtension()                      // <a:ext>
                                        { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }
                                    )
                                // $$pb todo comment end
                                )
                                {
                                    Embed = _embeddedReference,
                                    CompressionState =
                                    A.BlipCompressionValues.Print
                                },
                                new A.Stretch(                                       // <a:stretch>
                                    new A.FillRectangle()                            // <a:fillRect>
                                )
                            ),
                            new PIC.ShapeProperties(                                 // <pic:spPr>
                                new A.Transform2D(                                   // <a:xfrm>
                                    new A.Offset()                                   // <a:off>
                                    { X = 0L, Y = 0L },
                                    //new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                    new A.Extents()                                  // <a:ext>
                                    { Cx = _emuWidth, Cy = _emuHeight }
                                ),
                                new A.PresetGeometry(                                // <a:prstGeom>
                                    new A.AdjustValueList()                          // <a:avLst>
                                )
                                { Preset = A.ShapeTypeValues.Rectangle }
                            )
                        )
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                )
            )
            {
                DistanceFromTop = (UInt32Value)0U,
                DistanceFromBottom = (UInt32Value)0U,
                DistanceFromLeft = (UInt32Value)0U,
                DistanceFromRight = (UInt32Value)0U,
                EditId = "50D07946"
            };
        }

        private void SetWidthHeight()
        {
            if (_width == null || _height == null)
            {
                Image image = zimg.LoadImageFromFile(_file);
                if (_width != null)
                {
                    _height = (int)(image.Height * ((double)_width / image.Width) + 0.5);
                }
                else if (_height != null)
                {
                    _width = (int)(image.Width * ((double)_height / image.Height) + 0.5);
                }
                else
                {
                    _width = image.Width;
                    _height = image.Height;
                }
            }
            _emuWidth = (long)_width * _emusInPixel;
            _emuHeight = (long)_height * _emusInPixel;
        }

        private static ImagePartType GetImagePartType(string ext)
        {
            switch (ext.ToLower())
            {
                case ".bmp":
                    return ImagePartType.Bmp;
                //case "":
                //    return ImagePartType.Emf;
                case ".gif":
                    return ImagePartType.Gif;
                case ".ico":
                    return ImagePartType.Icon;
                case ".jpg":
                case ".jpeg":
                    return ImagePartType.Jpeg;
                //case "":
                //    return ImagePartType.Pcx;
                case ".png":
                    return ImagePartType.Png;
                //case "":
                //    return ImagePartType.Tiff;
                //case "":
                //    return ImagePartType.Wmf;
                default:
                    throw new PBException("unknow image file type \"{0}\"", ext);
            }
        }

        public static void AddNamespaceDeclarations(Document document)
        {
            document.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            document.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            document.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            document.AddNamespaceDeclaration("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");
            document.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        }

        public static OpenXmlCompositeElement Create(MainDocumentPart mainPart, zDocXPictureElement pictureElement, uint pictureId)
        {
            return new zOpenXmlPicture(mainPart, pictureElement, pictureId)._Create();
        }
    }
}

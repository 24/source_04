using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using pb.IO;

// Structure of a WordprocessingML document (Open XML SDK) https://msdn.microsoft.com/fr-fr/library/office/gg278308.aspx?f=255&MSPPError=-2147217396
// DocumentFormat.OpenXml.Packaging namespace https://msdn.microsoft.com/en-us/library/documentformat.openxml.packaging.aspx
// DocumentFormat.OpenXml.Wordprocessing namespace https://msdn.microsoft.com/en-us/library/documentformat.openxml.wordprocessing.aspx
// Images in Open XML documents https://blogs.msdn.microsoft.com/dmahugh/2006/12/10/images-in-open-xml-documents/
// How to: Insert a picture into a word processing document (Open XML SDK) https://msdn.microsoft.com/fr-fr/library/office/bb497430.aspx
namespace Test.Test_OpenXml
{
    public static class Test_OpenXml_01
    {
        private const string _directoryBase = @"c:\pib\dev_data\exe\runsource\test\Test_OpenXml";
        private static string _directory = null;

        public static void SetDirectory()
        {
            //_directory = zPath.Combine(RunSource.CurrentRunSource.ProjectDirectory, "docs\\Test_OpenXml_01");
            _directory = zPath.Combine(_directoryBase, "docs\\Test_OpenXml_01");
            if (!zDirectory.Exists(_directory))
                zDirectory.CreateDirectory(_directory);
        }

        public static void Test_CreateWordDoc_02(string file, string msg)
        {
            SetDirectory();
            using (WordprocessingDocument doc = WordprocessingDocument.Create(zPath.Combine(_directory, file), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());

                // String msg contains the text, "Hello, Word!"
                run.AppendChild(new Text(msg));

                //DocumentFormat.OpenXml.OpenXmlElement.NamespaceDeclarations
                //mainPart.Document.NamespaceDeclarations
                mainPart.Document.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
                mainPart.Document.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            }
        }

        public static void Test_CreateWordDoc_01(string file, string msg)
        {
            SetDirectory();
            using (WordprocessingDocument doc = WordprocessingDocument.Create(zPath.Combine(_directory, file), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());

                // String msg contains the text, "Hello, Word!"
                run.AppendChild(new Text(msg));
            }
            //DocumentFormat.OpenXml.Wordprocessing.Drawing
            //DocumentFormat.OpenXml.Drawing.Wordprocessing.Anchor
            //DocumentFormat.OpenXml.Drawing.Wordprocessing.HorizontalPosition
            //DocumentFormat.OpenXml.Drawing.Wordprocessing.HorizontalRelativePositionValues : Margin
            //DocumentFormat.OpenXml.Drawing.Wordprocessing.VerticalPosition
            //DocumentFormat.OpenXml.Drawing.Wordprocessing.VerticalRelativePositionValues : Paragraph
        }

        //public static void InsertPicture(string docFile, string imageFile)
        //{
        //    using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(docFile, isEditable: true))
        //    {
        //        MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;
        //        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
        //        using (FileStream stream = new FileStream(imageFile, FileMode.Open))
        //        {
        //            imagePart.FeedData(stream);
        //        }
        //        AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart));


        //        // Define the reference of the image.
        //        var element =
        //             new Drawing(
        //                 new DW.Inline(
        //                     new DW.Extent() { Cx = 990000L, Cy = 792000L },
        //                     new DW.EffectExtent()
        //                     {
        //                         LeftEdge = 0L,
        //                         TopEdge = 0L,
        //                         RightEdge = 0L,
        //                         BottomEdge = 0L
        //                     },
        //                     new DW.DocProperties()
        //                     {
        //                         Id = (UInt32Value)1U,
        //                         Name = "Picture 1"
        //                     },
        //                     new DW.NonVisualGraphicFrameDrawingProperties(
        //                         new A.GraphicFrameLocks() { NoChangeAspect = true }),
        //                     new A.Graphic(
        //                         new A.GraphicData(
        //                             new PIC.Picture(
        //                                 new PIC.NonVisualPictureProperties(
        //                                     new PIC.NonVisualDrawingProperties()
        //                                     {
        //                                         Id = (UInt32Value)0U,
        //                                         Name = "New Bitmap Image.jpg"
        //                                     },
        //                                     new PIC.NonVisualPictureDrawingProperties()),
        //                                 new PIC.BlipFill(
        //                                     new A.Blip(
        //                                         new A.BlipExtensionList(
        //                                             new A.BlipExtension()
        //                                             { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" })
        //                                     )
        //                                     {
        //                                         Embed = relationshipId,
        //                                         CompressionState = A.BlipCompressionValues.Print
        //                                     },
        //                                     new A.Stretch(
        //                                         new A.FillRectangle())),
        //                                 new PIC.ShapeProperties(
        //                                     new A.Transform2D(
        //                                         new A.Offset() { X = 0L, Y = 0L },
        //                                         new A.Extents() { Cx = 990000L, Cy = 792000L }),
        //                                     new A.PresetGeometry(
        //                                         new A.AdjustValueList()
        //                                     )
        //                                     { Preset = A.ShapeTypeValues.Rectangle }))
        //                         )
        //                         { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
        //                 )
        //                 {
        //                     DistanceFromTop = (UInt32Value)0U,
        //                     DistanceFromBottom = (UInt32Value)0U,
        //                     DistanceFromLeft = (UInt32Value)0U,
        //                     DistanceFromRight = (UInt32Value)0U,
        //                     EditId = "50D07946"
        //                 });

        //        // Append the reference to the body. The element should be in 
        //        // a DocumentFormat.OpenXml.Wordprocessing.Run.
        //        wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        //    }
        //}

        public static void InsertAPicture(string documentFile, string imageFile, uint id, string name, Int64Value cx, Int64Value cy)
        {
            SetDirectory();
            using (WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Open(zPath.Combine(_directory, documentFile), true))
            {
                MainDocumentPart mainPart = wordprocessingDocument.MainDocumentPart;

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (FileStream stream = new FileStream(imageFile, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                AddImageToBody(wordprocessingDocument, mainPart.GetIdOfPart(imagePart), id, name, cx, cy);
            }
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId, uint id, string name, Int64Value cx, Int64Value cy)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         //new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.Extent() { Cx = cx, Cy = cy },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             //Id = (UInt32Value)1U,
                             //Name = "Picture 1"
                             Id = id,
                             Name = name
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks()),   // { NoChangeAspect = true }
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             //new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                             new A.Extents() { Cx = cx, Cy = cy }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }
    }
}

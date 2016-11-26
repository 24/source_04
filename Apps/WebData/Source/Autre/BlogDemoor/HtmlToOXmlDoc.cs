using System;
using System.Collections.Generic;
using System.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using pb.Compiler;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.OpenXml;
using pb.Web;

namespace WebData.BlogDemoor
{
    public static class Test_HtmlToOXmlDoc
    {
        public static void Test_Trace_Html(string file)
        {
            string traceFile = file + ".html.trace";
            bool generateCloseTag = true;
            bool disableLineColumn = false;
            bool disableScriptTreatment = false;
            bool useReadAttributeValue_v2 = true;
            bool useTranslateChar = true;
            HtmlReader_v4.ReadFile(file, generateCloseTag: generateCloseTag, disableLineColumn: disableLineColumn, disableScriptTreatment: disableScriptTreatment,
                useReadAttributeValue_v2: useReadAttributeValue_v2, useTranslateChar: useTranslateChar)
                .zSave(traceFile);
        }

        public static void Test_Trace_HtmlDoc(string file)
        {
            string traceFile = file + ".htmldoc.trace";
            HtmlDocReader.ReadFile(file).zSave(traceFile);
        }

        public static void Test_Trace_HtmlDocText(string file)
        {
            string traceFile = file + ".htmldoc.text.trace";
            HtmlDocReader.ReadFile(file).zTraceToFile(traceFile);
        }

        public static void Test_Trace_HtmlToOXml(string file)
        {
            string traceFile = file + ".htmltooxml.trace";
            HtmlToOXmlElements.ToOXmlXElements(HtmlDocReader.ReadFile(file)).zSave(traceFile);
        }

        public static void Test_Trace_HtmlToOXmlText(string file)
        {
            string traceFile = file + ".htmltooxml.text.trace";
            HtmlToOXmlElements.ToOXmlXElements(HtmlDocReader.ReadFile(file)).zTraceToFile(traceFile);
        }

        public static void Test_HtmlToDocx(string file)
        {
            OXmlDoc.Create(file + ".docx", HtmlToOXmlElements.ToOXmlXElements(HtmlDocReader.ReadFile(file)));
        }

        public static void Test_OXmlToDocx(string file)
        {
            OXmlDoc.Create(file + ".docx", OXmlElementReader.Read(file));
        }
    }

    public class HtmlImage
    {
        public string File = null;
        public bool NoneAlign = false;
        public bool LeftAlign = false;
        public int? Width = null;
        public int? Height = null;
    }

    public class HtmlToOXmlElements
    {
        private static string _pictureDir = @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\";
        //private IEnumerable<HtmlDocNode> _nodes = null;
        private int? _forcedImageWidth = 300;
        private int _maxImageHorizontalPosition = 700;
        private int _imageMarge = 5;
        private int _imageHorizontalPosition = 0;
        private int _imageVerticalPosition = 0;
        private int _imageHeight = 0;

        private IEnumerable<OXmlElement> _ToOXmlXElements(IEnumerable<HtmlDocNode> nodes)
        {
            yield return new OXmlParagraphPropertiesDefaultElement();
            foreach (HtmlDocNode node in nodes)
            {
                IEnumerable<OXmlElement> elements = null;
                switch (node.Type)
                {
                    case HtmlDocNodeType.BeginTag:
                        // <p> <a>
                        //element = BeginTag((HtmlDocNodeBeginTag)node);
                        HtmlDocNodeBeginTag beginTag = (HtmlDocNodeBeginTag)node;
                        if (beginTag.Tag == HtmlTagType.P)
                            elements = Paragraph();
                        break;
                    //case HtmlDocNodeType.EndTag:
                    //    //HtmlDocNodeEndTag
                    //    break;
                    case HtmlDocNodeType.Tag:
                        //
                        //HtmlDocNodeTagImg
                        // <br> <img>
                        //element = Tag((HtmlDocNodeTag)node);
                        HtmlDocNodeTag tag = (HtmlDocNodeTag)node;
                        if (tag.Tag == HtmlTagType.BR)
                            elements = NewLine();
                        else if (tag.Tag == HtmlTagType.Img)
                            elements = Image((HtmlDocNodeTagImg)tag);
                            break;
                    case HtmlDocNodeType.Text:
                        //element = new OXmlTextElement { Text = ((HtmlDocNodeText)node).Text };
                        elements = Text((HtmlDocNodeText)node);
                        break;
                }
                if (elements != null)
                {
                    foreach (OXmlElement element in elements)
                        yield return element;
                }
            }
        }

        private IEnumerable<OXmlElement> Paragraph()
        {
            if (_imageHorizontalPosition == 0)
                yield return new OXmlElement { Type = OXmlElementType.Paragraph };
        }

        private IEnumerable<OXmlElement> NewLine()
        {
            if (_imageHorizontalPosition == 0)
                yield return new OXmlElement { Type = OXmlElementType.Line };
        }

        private IEnumerable<OXmlElement> Text(HtmlDocNodeText text)
        {
            if (_imageHorizontalPosition != 0)
            {
                yield return new OXmlElement { Type = OXmlElementType.Paragraph };
                _imageHorizontalPosition = 0;
            }
            yield return new OXmlTextElement { Text = text.Text };
        }

        private IEnumerable<OXmlElement> Image(HtmlDocNodeTagImg imgTag)
        {
            // 300 px ->  7.94 cm
            // 450 px -> 11.91 cm
            // largeur 18.5 cm = 694 px
            // largeur 16 cm   = 600 px
            // example :
            // leftalign width300 - nonealign width300
            // leftalign width300 - nonealign width300
            // example :
            // leftalign width300 - nonealign width300
            // nonealign width450
            // example :
            // nonealign width450
            // nonealign width450

            HtmlImage htmlImage = new HtmlImage();
            Uri uri = new Uri(imgTag.Link);
            htmlImage.File = _pictureDir + uri.Segments[uri.Segments.Length - 1];
            foreach (string className in imgTag.ClassList)
                SetPictureClassParam(htmlImage, className);
            if (_forcedImageWidth != null)
                htmlImage.Width = _forcedImageWidth;
            SetPictureWidthHeight(htmlImage);

            int horizontalPosition;
            int verticalPosition;
            if (_imageHorizontalPosition > 0 && _imageHorizontalPosition + htmlImage.Width > _maxImageHorizontalPosition)
            {
                horizontalPosition = 0;
                _imageVerticalPosition += _imageHeight + _imageMarge;
                verticalPosition = _imageVerticalPosition;
                _imageHorizontalPosition = (int)htmlImage.Width + _imageMarge;
                _imageHeight = (int)htmlImage.Height;
            }
            else
            {
                horizontalPosition = _imageHorizontalPosition;
                _imageHorizontalPosition += (int)htmlImage.Width + _imageMarge;
                verticalPosition = _imageVerticalPosition;
                _imageHeight = Math.Max(_imageHeight, (int)htmlImage.Height);
            }
            yield return new OXmlPictureElement { File = htmlImage.File, Width = htmlImage.Width, Height = htmlImage.Height, PictureDrawing = new OXmlAnchorPictureDrawing
            {
                // SquareSize = 21800
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = horizontalPosition,
                VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = verticalPosition
            } };
        }

        private static void SetPictureClassParam(HtmlImage htmlImage, string className)
        {
            // class : leftalign, nonealign, width300, width450
            switch (className.ToLower())
            {
                case "nonealign":
                    //pictureElement.PictureDrawing = new OXmlPictureInlineDrawing();
                    htmlImage.NoneAlign = true;
                    break;
                case "leftalign":
                    //pictureElement.PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 };
                    //pictureElement.PictureDrawing = new OXmlPictureAnchorDrawing() { Wrap = new OXmlAnchorWrapTight() { SquareSize = 21800 } };
                    htmlImage.LeftAlign = true;
                    break;
                case "width300":
                    //pictureElement.Width = 300;
                    htmlImage.Width = 300;
                    break;
                case "width450":
                    //pictureElement.Width = 450;
                    htmlImage.Width = 450;
                    break;
                default:
                    throw new pb.PBException($"unknow img class \"{className}\"");
            }
        }

        private void SetPictureWidthHeight(HtmlImage htmlImage)
        {
            if (htmlImage.Width == null || htmlImage.Height == null)
            {
                Image image = zimg.LoadImageFromFile(htmlImage.File);
                if (htmlImage.Width != null)
                {
                    htmlImage.Height = (int)(image.Height * ((double)htmlImage.Width / image.Width) + 0.5);
                }
                else if (htmlImage.Height != null)
                {
                    htmlImage.Width = (int)(image.Width * ((double)htmlImage.Height / image.Height) + 0.5);
                }
                else
                {
                    htmlImage.Width = image.Width;
                    htmlImage.Height = image.Height;
                }
            }
        }

        //private OXmlElement BeginTag(HtmlDocNodeBeginTag beginTag)
        //{
        //    // <p> <a>
        //    if (beginTag.Tag == HtmlTagType.P)
        //        return new OXmlElement { Type = OXmlElementType.Paragraph };
        //    return null;
        //}

        //private OXmlElement Tag(HtmlDocNodeTag tag)
        //{
        //    // <br> <img>
        //    if (tag.Tag == HtmlTagType.BR)
        //        return new OXmlElement { Type = OXmlElementType.Line };
        //    else if (tag.Tag == HtmlTagType.Img)
        //    {
        //        HtmlDocNodeTagImg img = (HtmlDocNodeTagImg)tag;
        //        Uri uri = new Uri(img.Link);
        //        string file = uri.Segments[uri.Segments.Length - 1];
        //        // new zDocXPictureElement { File = _pictureDir + file, Width = 300, PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 } }
        //        OXmlPictureElement pictureElement = new OXmlPictureElement { File = _pictureDir + file };
        //        foreach (string className in img.ClassList)
        //            SetPictureClassParam(pictureElement, className);
        //        return pictureElement;
        //    }
        //    else
        //        return null;
        //}

        //private static void SetPictureClassParam(OXmlPictureElement pictureElement, string className)
        //{
        //    // class : leftalign, nonealign, width300, width450
        //    switch (className.ToLower())
        //    {
        //        case "nonealign":
        //            pictureElement.PictureDrawing = new OXmlPictureInlineDrawing();
        //            break;
        //        case "leftalign":
        //            //pictureElement.PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 };
        //            pictureElement.PictureDrawing = new OXmlPictureAnchorDrawing() { Wrap = new OXmlAnchorWrapTight() { SquareSize = 21800 } };
        //            break;
        //        case "width300":
        //            pictureElement.Width = 300;
        //            break;
        //        case "width450":
        //            pictureElement.Width = 450;
        //            break;
        //        default:
        //            throw new pb.PBException($"unknow img class \"{className}\"");
        //    }
        //}

        public static IEnumerable<OXmlElement> ToOXmlXElements(IEnumerable<HtmlDocNode> nodes)
        {
            return new HtmlToOXmlElements()._ToOXmlXElements(nodes);
        }
    }
}

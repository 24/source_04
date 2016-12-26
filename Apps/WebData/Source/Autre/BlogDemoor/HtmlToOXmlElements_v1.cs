using System;
using System.Collections.Generic;
using System.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using pb.Data;
using pb.Data.OpenXml;
using pb.Web;

namespace WebData.BlogDemoor
{
    public class HtmlToOXmlElements_v1
    {
        private static string _pictureDir = @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\";
        //private IEnumerable<HtmlDocNode> _nodes = null;
        private int? _forcedImageWidth = 300;
        private int _maxImageHorizontalPosition = 700;
        private int _imageMarge = 5;

        //private HtmlImage _image = null;

        // Image_v1
        private int _imageHorizontalPosition = 0;
        private int _imageVerticalPosition = 0;
        private int _imageHeight = 0;

        private IEnumerable<OXmlElement> _ToOXmlXElements(IEnumerable<HtmlDocNode> nodes)
        {
            //yield return new OXmlDocDefaultsParagraphPropertiesElement();
            yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
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

        private IEnumerable<OXmlElement> Paragraph(string style = null)
        {
            if (_imageHorizontalPosition == 0)
            //if (_image == null)
                yield return new OXmlParagraphElement { Style = style };
        }

        private IEnumerable<OXmlElement> NewLine()
        {
            if (_imageHorizontalPosition == 0)
            //if (_image == null)
                yield return new OXmlElement { Type = OXmlElementType.Line };
        }

        private IEnumerable<OXmlElement> Text(HtmlDocNodeText text)
        {
            if (_imageHorizontalPosition != 0)
            {
                //yield return new OXmlElement { Type = OXmlElementType.Paragraph };
                yield return new OXmlParagraphElement();
                _imageHorizontalPosition = 0;
            }
            yield return new OXmlTextElement { Text = text.Text };
        }

        private HtmlImage GetHtmlImage(HtmlDocNodeTagImg imgTag)
        {
            HtmlImage htmlImage = new HtmlImage();
            Uri uri = new Uri(imgTag.Link);
            htmlImage.File = _pictureDir + uri.Segments[uri.Segments.Length - 1];
            foreach (string className in imgTag.ClassList)
                SetPictureClassParam(htmlImage, className);
            if (_forcedImageWidth != null)
                htmlImage.Width = _forcedImageWidth;
            SetPictureWidthHeight(htmlImage);
            return htmlImage;
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


            HtmlImage htmlImage = GetHtmlImage(imgTag);

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
                //Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalPosition = new OXmlHorizontalPosition { RelativeFrom = DW.HorizontalRelativePositionValues.Margin, PositionOffset = horizontalPosition },
                VerticalPosition = new OXmlVerticalPosition { RelativeFrom = DW.VerticalRelativePositionValues.Paragraph, PositionOffset = verticalPosition }
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
            return new HtmlToOXmlElements_v1()._ToOXmlXElements(nodes);
        }
    }
}

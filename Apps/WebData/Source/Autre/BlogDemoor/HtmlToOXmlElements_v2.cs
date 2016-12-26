using System;
using System.Collections.Generic;
using System.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using pb.Data;
using pb.Data.OpenXml;
using pb.Web;
using pb.IO;
using pb.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;

// todo :
//   - ok remove unnecessary line
//   - ok remove blank at begin and end text
//   - ok remove \r\n
//   - center image
//   - problème d'afficahge de la police dans word
//   - manage html <strong> (ex: page_001_01-03_tests_de_camping-cars.html)
//   - manage html <span style="color: #0000ff;"> (ex: page_003_01-03_avis_de_Chloé.html)
//   - manage html <ul> <li> (ex: page_005_15-04_BALOO.html)
//   - manage html <span style="text-decoration: underline;"> (ex: page_005_15-04_BALOO.html)
//   - manage space 0xA0 (ex: page_005_15-04_BALOO.html)
//   - patch : insert page break
//   - image trop près du texte du dessus (ex: page_006_19-04_en_vol_pour_Lisbonne.html)

namespace WebData.BlogDemoor
{
    public class HtmlImage
    {
        public string File = null;
        public string Url = null;
        public bool NoneAlign = false;
        public bool LeftAlign = false;
        public bool RightAlign = false;
        public int? Width = null;
        public int? Height = null;
    }

    //public class HtmlImagePatch
    //{
    //    public string Url = null;
    //    public bool? NoneAlign = false;
    //    public bool? LeftAlign = false;
    //    public bool? RightAlign = false;
    //    public int? Width = null;
    //    public int? Height = null;
    //}

    public class HtmlToOXmlElements_v2
    {
        private static bool _trace = false;
        private string _imageDirectory = null;
        private bool _textRemoveBlank = false;
        private bool _textRemoveLine = false;
        private Dictionary<string, NamedValues<ZValue>> _imageClasses = null;
        private int? _forcedMaxImageWidth = null;
        private int _maxImageHorizontalSize = 1500;
        private int _imageHorizontalLeftMarge = 0;
        private int _imageHorizontalMarge = 0;
        private int? _imageHorizontalBorder = null; // 71755 = 0.2 cm
        private string _imageSeparationParagraphStyle = null;
        private bool _useLinkImage = false;                                        // true : take image link from tag a, false : take image link from tag img
        //private string _imagePatchesFile = null;
        private Dictionary<string, NamedValues<ZValue>> _imagePatches = null;

        private WebImage[] _webImages = null;
        private string _sourceUrl = null;
        //private IEnumerable<HtmlDocNode> _nodes = null;
        //private int _maxImageHorizontalPosition = 700;
        private OXmlElement _paragraph = null;
        private OXmlElement _line = null;
        private HtmlDocNodeBeginTagA _currentTagA = null;
        private List<HtmlImage> _htmlImages = null;

        // Image_v1
        //private int _imageHorizontalPosition = 0;
        //private int _imageVerticalPosition = 0;
        //private int _imageHeight = 0;

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public string ImageDirectory { get { return _imageDirectory; } set { _imageDirectory = value; } }
        public bool TextRemoveBlank { get { return _textRemoveBlank; } set { _textRemoveBlank = value; } }
        public bool TextRemoveLine { get { return _textRemoveLine; } set { _textRemoveLine = value; } }
        public Dictionary<string, NamedValues<ZValue>> ImageClasses { get { return _imageClasses; } }
        public int? ForcedMaxImageWidth { get { return _forcedMaxImageWidth; } set { _forcedMaxImageWidth = value; } }
        public int MaxImageHorizontalSize { get { return _maxImageHorizontalSize; } set { _maxImageHorizontalSize = value; } }
        public int ImageHorizontalLeftMarge { get { return _imageHorizontalLeftMarge; } set { _imageHorizontalLeftMarge = value; } }
        public int ImageHorizontalMarge { get { return _imageHorizontalMarge; } set { _imageHorizontalMarge = value; } }
        public int? ImageHorizontalBorder { get { return _imageHorizontalBorder; } set { _imageHorizontalBorder = value; } }
        public string ImageSeparationParagraphStyle { get { return _imageSeparationParagraphStyle; } set { _imageSeparationParagraphStyle = value; } }
        public bool UseLinkImage { get { return _useLinkImage; } set { _useLinkImage = value; } }
        //public string ImagePatchesFile { get { return _imagePatchesFile; } set { _imagePatchesFile = value; } }
        //public Dictionary<string, NamedValues<ZValue>> ImagePatches { get { return _imagePatches; } }

        public void Init(XElement xe)
        {
            if (xe == null)
                return;
            ReadImageClassValues(xe.XPathSelectElement("ImageClasses"));
            //_imagePatches _imagePatchesFile
            //ReadImagePatches();
        }

        public IEnumerable<OXmlElement> ToOXmlXElements(IEnumerable<HtmlDocNode> nodes, WebImage[] webImages = null, string sourceUrl = null, Dictionary<string, NamedValues<ZValue>> imagePatches = null)
        {
            _webImages = webImages;
            _sourceUrl = sourceUrl;
            _imagePatches = imagePatches;

            _paragraph = null;
            _line = null;
            _currentTagA = null;
            _htmlImages = new List<HtmlImage>();

            //yield return new OXmlDocDefaultsParagraphPropertiesElement();
            //yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
            IEnumerable<OXmlElement> elements;
            foreach (HtmlDocNode node in nodes)
            {
                elements = null;
                switch (node.Type)
                {
                    case HtmlDocNodeType.BeginTag:
                        // <p> <a>
                        //element = BeginTag((HtmlDocNodeBeginTag)node);
                        HtmlDocNodeBeginTag beginTag = (HtmlDocNodeBeginTag)node;
                        if (beginTag.Tag == HtmlTagType.P)
                            elements = Paragraph();
                        else if (beginTag.Tag == HtmlTagType.A)
                            Link((HtmlDocNodeBeginTagA)beginTag);
                        break;
                    case HtmlDocNodeType.EndTag:
                        HtmlDocNodeEndTag endTag = (HtmlDocNodeEndTag)node;
                        if (endTag.Tag == HtmlTagType.A)
                            EndLink();
                        break;
                    case HtmlDocNodeType.Tag:
                        //HtmlDocNodeTagImg
                        // <br> <img>
                        //element = Tag((HtmlDocNodeTag)node);
                        HtmlDocNodeTag tag = (HtmlDocNodeTag)node;
                        if (tag.Tag == HtmlTagType.BR)
                            elements = NewLine();
                        else if (tag.Tag == HtmlTagType.Img)
                            //elements = Image((HtmlDocNodeTagImg)tag);
                            Image((HtmlDocNodeTagImg)tag);
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
            elements = GetImages(endOfDocument: true);
            if (elements != null)
            {
                foreach (OXmlElement element in elements)
                    yield return element;
            }
        }

        // string style = null
        private IEnumerable<OXmlElement> Paragraph()
        {
            //if (_imageHorizontalPosition == 0)
            //if (_images.Count == 0)
            //    yield return new OXmlParagraphElement { Style = style };
            if (_line != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("Paragraph()     : remove stored new line");
                _line = null;
            }
            if (_paragraph != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("Paragraph()     : send stored paragraph");
                yield return _paragraph;
            }
            if (_trace)
                pb.Trace.WriteLine("Paragraph()     : store paragraph");
            _paragraph = new OXmlParagraphElement();
        }

        private IEnumerable<OXmlElement> NewLine()
        {
            //if (_imageHorizontalPosition == 0)
            //if (_images.Count == 0)
            if (_paragraph != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("NewLine()       : send stored paragraph");
                yield return _paragraph;
                _paragraph = null;
            }
            else if (_line != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("NewLine()       : send stored new line");
                yield return _line;
                _line = null;
            }
            if (_trace)
                pb.Trace.WriteLine("NewLine()       : store new line");
            _line = new OXmlElement { Type = OXmlElementType.Line };
        }

        private IEnumerable<OXmlElement> Text(HtmlDocNodeText text)
        {
            //if (_imageHorizontalPosition != 0)
            //{
            //    yield return new OXmlParagraphElement();
            //    _imageHorizontalPosition = 0;
            //}

            //if (_trace)
            //    Trace.WriteLine("Text() :");

            string text2 = text.Text;
            if (_textRemoveBlank)
                text2 = text2.Trim();
            if (_textRemoveLine)
            {
                text2 = text2.Replace("\r", "");
                text2 = text2.Replace("\n", "");
            }
            if (text2.Length == 0)
                yield break;

            bool image = false;
            foreach (OXmlElement imageElement in GetImages())
            {
                yield return imageElement;
                image = true;
            }
            if (image)
            {
                if (_line != null)
                {
                    if (_trace)
                        pb.Trace.WriteLine("Text()          : remove stored new line");
                    _line = null;
                }
                if (_paragraph != null)
                {
                    if (_trace)
                        pb.Trace.WriteLine("Text()          : send stored paragraph       after image");
                    yield return _paragraph;
                    _paragraph = null;
                }
                else
                {
                    if (_trace)
                        pb.Trace.WriteLine("Text()          : send paragraph              after image");
                    yield return new OXmlParagraphElement();
                }
            }
            else if (_paragraph != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("Text()          : send stored paragraph");
                yield return _paragraph;
                _paragraph = null;
            }
            else if (_line != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("Text()          : send stored new line");
                yield return _line;
                _line = null;
            }
            if (_trace)
                pb.Trace.WriteLine($"Text()          : send text                   \"{zstr.left(text.Text, 20)}\"");
            //_removeBlank _removeLine
            //string text2 = text.Text;
            //if (_textRemoveBlank)
            //    text2 = text2.Trim();
            //if (_textRemoveLine)
            //{
            //    text2 = text2.Replace("\r", "");
            //    text2 = text2.Replace("\n", "");
            //}
            yield return new OXmlTextElement { Text = text2 };
        }

        private void Link(HtmlDocNodeBeginTagA beginTagA)
        {
            _currentTagA = beginTagA;
        }

        private void EndLink()
        {
            _currentTagA = null;
        }

        private IEnumerable<OXmlElement> GetImages(bool endOfDocument = false)
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

            // ...
            // img       112509336.jpg   width300 leftalign
            // ...
            // img       112509351.jpg   width450 nonealign
            // img       112509354.jpg   width450 nonealign
            // ...
            // img       112509392.jpg   width300 leftalign
            // img       112509399.jpg   width300 nonealign
            // img       112509404.jpg   width450 nonealign
            // ...
            // img       112509359.jpg   width450 nonealign
            // ...
            // img       112509426.jpg   width300 leftalign
            // img       112509432.jpg   width300 nonealign
            // img       112509438.jpg   width300 leftalign
            // img       112509447.jpg   width300 nonealign
            // ...

            // ...
            // Paragraph
            // Picture   112509336.jpg   AnchorWrapSquare          Width 300
            // ...
            // Paragraph
            // Picture   112509351.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset   2
            // Picture   112509354.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset 305
            // ...
            // Paragraph Style: "TinyParagraph"
            // Picture   112509392.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset   2
            // Picture   112509399.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset 305
            // Paragraph
            // Picture   112509404.jpg   Inline                    Width 300
            // ...
            // Paragraph
            // Picture   112509359.jpg   Inline                    Width 300
            // ...
            // Paragraph Style: "TinyParagraph"
            // Picture   112509426.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset   2
            // Picture   112509432.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset 305
            // Paragraph
            // Picture   112509438.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset   2
            // Picture   112509447.jpg   AnchorWrapTopAndBottom    Width 300  HorizontalPositionOffset 305
            // ...

            while (_htmlImages.Count > 0)
            {
                // calculate nb of image horizontaly (_maxImageHorizontalSize)
                int nb = 1;
                HtmlImage htmlImage = _htmlImages[0];
                bool nextImageToBig = false;
                //bool leftAlign = true;
                //if (htmlImage.RightAlign)
                //    leftAlign = false;
                //if (!htmlImage.NoneAlign)
                //{
                int width = (int)htmlImage.Width;
                for (int i = 1; i < _htmlImages.Count; i++)
                {
                    HtmlImage htmlImage2 = _htmlImages[i];
                    if (htmlImage.RightAlign != htmlImage2.RightAlign)
                        break;
                    width += (int)htmlImage2.Width;
                    if (width > _maxImageHorizontalSize)
                    {
                        nextImageToBig = true;
                        break;
                    }
                        nb++;
                }
                //}

                if (_htmlImages.Count > nb)
                {
                    if (_trace)
                        pb.Trace.WriteLine($"GetImages()     : send paragraph              \"{_imageSeparationParagraphStyle}\"");
                    yield return new OXmlParagraphElement { Style = _imageSeparationParagraphStyle };
                }
                else
                {
                    if (_trace)
                        pb.Trace.WriteLine("GetImages()     : send paragraph");
                    yield return new OXmlParagraphElement();
                }

                if (nb == 1)
                {
                    OXmlPictureDrawing pictureDrawing;
                    if (htmlImage.NoneAlign || nextImageToBig || (endOfDocument && _htmlImages.Count == 1))
                    {
                        // Inline
                        if (_trace)
                            pb.Trace.WriteLine($"GetImages()     : send picture                inline \"{zPath.GetFileName(htmlImage.File)}\"");
                        //pictureDrawing = new OXmlInlinePictureDrawing();
                        pictureDrawing = new OXmlAnchorPictureDrawing
                        {
                            Wrap = new OXmlAnchorWrapTopAndBottom(),
                            HorizontalPosition = new OXmlHorizontalPosition { RelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalAlignment = "center" },
                            VerticalPosition = new OXmlVerticalPosition { RelativeFrom = DW.VerticalRelativePositionValues.Line, VerticalAlignment = "top" }
                        };
                    }
                    else
                    {
                        // AnchorWrapSquare
                        if (_trace)
                            pb.Trace.WriteLine($"GetImages()     : send picture                anchor wrap square \"{zPath.GetFileName(htmlImage.File)}\"");
                        pictureDrawing = new OXmlAnchorPictureDrawing
                        {
                            //Wrap = new OXmlAnchorWrapSquare { DistanceFromLeft = htmlImage.RightAlign ? _imageHorizontalBorder : 0, DistanceFromRight = htmlImage.RightAlign ? 0 : _imageHorizontalBorder },
                            Wrap = new OXmlAnchorWrapSquare(),
                            DistanceFromLeft = htmlImage.RightAlign ? _imageHorizontalBorder : null,
                            DistanceFromRight = htmlImage.RightAlign ? null : _imageHorizontalBorder,
                            HorizontalPosition = new OXmlHorizontalPosition { RelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalAlignment = htmlImage.RightAlign ? "right" : "left" },
                            VerticalPosition = new OXmlVerticalPosition { RelativeFrom = DW.VerticalRelativePositionValues.Line, VerticalAlignment = "top" }
                        };
                    }
                    yield return new OXmlPictureElement
                    {
                        File = htmlImage.File,
                        Width = htmlImage.Width,
                        Height = htmlImage.Height,
                        PictureDrawing = pictureDrawing
                    };
                    _htmlImages.RemoveAt(0);
                }
                else
                {
                    // AnchorWrapTopAndBottom
                    int position = _imageHorizontalLeftMarge;
                    while (nb > 0)
                    {
                        if (_trace)
                            pb.Trace.WriteLine($"GetImages()     : send picture                anchor wrap top and bottom \"{zPath.GetFileName(htmlImage.File)}\"");
                        htmlImage = _htmlImages[0];
                        yield return new OXmlPictureElement
                        {
                            File = htmlImage.File,
                            Width = htmlImage.Width,
                            Height = htmlImage.Height,
                            PictureDrawing = new OXmlAnchorPictureDrawing
                            {
                                Wrap = new OXmlAnchorWrapTopAndBottom(),
                                HorizontalPosition = new OXmlHorizontalPosition { RelativeFrom = DW.HorizontalRelativePositionValues.Margin, PositionOffset = position },
                                VerticalPosition = new OXmlVerticalPosition { RelativeFrom = DW.VerticalRelativePositionValues.Line, VerticalAlignment = "top" }
                            }
                        };
                        position += (int)htmlImage.Width + _imageHorizontalMarge;
                        _htmlImages.RemoveAt(0);
                        nb--;
                    }
                }
            }


            //yield return new OXmlPictureElement
            //{
            //    File = htmlImage.File,
            //    Width = htmlImage.Width,
            //    Height = htmlImage.Height,
            //    PictureDrawing = new OXmlAnchorPictureDrawing
            //    {
            //        // SquareSize = 21800
            //        //Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //        Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
            //        HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin,
            //        HorizontalPositionOffset = horizontalPosition,
            //        VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph,
            //        VerticalPositionOffset = verticalPosition
            //    }
            //};
        }

        private HtmlImage GetHtmlImage(HtmlDocNodeTagImg imgTag)
        {
            HtmlImage htmlImage = new HtmlImage();
            //Uri uri;
            string url;
            if (_useLinkImage && _currentTagA != null)
                url = _currentTagA.Link;
            else
                //uri = new Uri(imgTag.Link);
                url = imgTag.Link;
            //string file = uri.Segments[uri.Segments.Length - 1];
            //if (_imageDirectory != null)
            //    file = zPath.Combine(_imageDirectory, file);
            //htmlImage.File = file;
            url = zurl.GetUrl(_sourceUrl, url);
            htmlImage.File = GetImageFile(url);
            htmlImage.Url = url;

            //if (_trace)
            //    Trace.WriteLine($"GetHtmlImage()  : add image \"{htmlImage.File}\"");

            if (imgTag.ClassList != null)
            {
                foreach (string className in imgTag.ClassList)
                    SetHtmlImageValues(htmlImage, className);
            }
            if (imgTag.Width != null)
                htmlImage.Width = imgTag.Width;
            if (_forcedMaxImageWidth != null && (htmlImage.Width > _forcedMaxImageWidth || htmlImage.Width == null))
            {
                pb.Trace.WriteLine($"force image width to {_forcedMaxImageWidth} original width {htmlImage.Width} image \"{htmlImage.File}\"");
                htmlImage.Width = _forcedMaxImageWidth;
            }
            PatchImage(htmlImage);
            SetPictureWidthHeight(htmlImage);
            return htmlImage;
        }

        private string GetImageFile(string url)
        {
            string file = null;
            if (_webImages != null)
            {
                foreach (WebImage webImage in _webImages)
                {
                    if (webImage.Url == url)
                    {
                        file = webImage.File;
                        break;
                    }
                }
                if (file == null)
                    pb.Trace.WriteLine($"image not found in WebImage array \"{url}\"");
            }
            else
            {
                Uri uri = new Uri(url);
                file = uri.Segments[uri.Segments.Length - 1];
            }
            if (_imageDirectory != null && file != null)
                file = zPath.Combine(_imageDirectory, file);
            return file;
        }

        //private IEnumerable<OXmlElement> Image(HtmlDocNodeTagImg imgTag)
        private void Image(HtmlDocNodeTagImg imgTag)
        {
            HtmlImage htmlImage = GetHtmlImage(imgTag);

            if (htmlImage.File == null)
            {
                pb.Trace.WriteLine("can't add picture without file");
                return;
            }

            if (_trace)
                pb.Trace.WriteLine($"Image()         : add picture                 \"{zPath.GetFileName(htmlImage.File)}\" width {htmlImage.Width} height {htmlImage.Height} {(htmlImage.NoneAlign ? "noneAlign " : "" )}{(htmlImage.LeftAlign ? "leftAlign " : "")}{(htmlImage.RightAlign ? "rightAlign" : "")}");
            if (_paragraph != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("Image()         : remove stored paragraph");
                _paragraph = null;
            }
            if (_line != null)
            {
                if (_trace)
                    pb.Trace.WriteLine("Image()         : remove stored new line");
                _line = null;
            }
            _htmlImages.Add(htmlImage);

            //int horizontalPosition;
            //int verticalPosition;
            //if (_imageHorizontalPosition > 0 && _imageHorizontalPosition + htmlImage.Width > _maxImageHorizontalPosition)
            //{
            //    horizontalPosition = 0;
            //    _imageVerticalPosition += _imageHeight + _imageMarge;
            //    verticalPosition = _imageVerticalPosition;
            //    _imageHorizontalPosition = (int)htmlImage.Width + _imageMarge;
            //    _imageHeight = (int)htmlImage.Height;
            //}
            //else
            //{
            //    horizontalPosition = _imageHorizontalPosition;
            //    _imageHorizontalPosition += (int)htmlImage.Width + _imageMarge;
            //    verticalPosition = _imageVerticalPosition;
            //    _imageHeight = Math.Max(_imageHeight, (int)htmlImage.Height);
            //}
            //yield return new OXmlPictureElement
            //{
            //    File = htmlImage.File,
            //    Width = htmlImage.Width,
            //    Height = htmlImage.Height,
            //    PictureDrawing = new OXmlAnchorPictureDrawing
            //    {
            //        // SquareSize = 21800
            //        //Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //        Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
            //        HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin,
            //        HorizontalPositionOffset = horizontalPosition,
            //        VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph,
            //        VerticalPositionOffset = verticalPosition
            //    }
            //};
        }

        private void ReadImageClassValues(XElement xe)
        {
            _imageClasses = new Dictionary<string, NamedValues<ZValue>>();
            if (xe == null)
                return;
            foreach (XElement xe2 in xe.XPathSelectElements("ImageClass"))
            {
                _imageClasses.Add(xe2.Attribute("class").Value, ParseNamedValues.ParseValues(xe2.Attribute("values").Value, useLowercaseKey: true));
            }
        }

        private void SetHtmlImageValues(HtmlImage htmlImage, string className)
        {
            className = className.ToLower();
            if (_imageClasses.ContainsKey(className))
            {
                //if (_trace)
                //    Trace.WriteLine($"GetHtmlImage()  : set image class \"{className}\"");
                foreach (KeyValuePair<string, ZValue> value in _imageClasses[className])
                {
                    SetHtmlImageValue(htmlImage, value.Key, value.Value);
                }
            }
            else
                pb.Trace.WriteLine($"unknow img class \"{className}\" \"{htmlImage.File}\"");
        }

        private static void SetHtmlImageValue(HtmlImage htmlImage, string name, ZValue value)
        {
            //if (_trace)
            //    Trace.WriteLine($"GetHtmlImage()  : set image value \"{name}\" = {value}");
            switch (name)
            {
                case "nonealign":
                    htmlImage.NoneAlign = (bool)(ZBool)value;
                    break;
                case "leftalign":
                    htmlImage.LeftAlign = (bool)(ZBool)value;
                    break;
                case "rightalign":
                    htmlImage.RightAlign = (bool)(ZBool)value;
                    break;
                case "width":
                    htmlImage.Width = (int)(ZInt)value;
                    break;
                default:
                    pb.Trace.WriteLine($"unknow value \"{name}\" = {value}");
                    break;
            }
        }

        //private void ReadImagePatches()
        //{
        //    if (_imagePatchesFile == null)
        //        return;
        //    _imagePatches = new Dictionary<string, NamedValues<ZValue>>();
        //    foreach (BsonDocument doc in zmongo.BsonRead<BsonDocument>(_imagePatchesFile))
        //    {
        //        _imagePatches.Add(doc.zGet("Url").zAsString(), ParseNamedValues.ParseValues(doc.zGet("Values").zAsString(), useLowercaseKey: true));
        //    }
        //}

        private void PatchImage(HtmlImage htmlImage)
        {
            if (_imagePatches == null)
                return;
            if (_imagePatches.ContainsKey(htmlImage.Url))
            {
                foreach (KeyValuePair<string, ZValue> namedValue in _imagePatches[htmlImage.Url])
                {
                    switch (namedValue.Key)
                    {
                        case "nonealign":
                            htmlImage.NoneAlign = (bool)(ZBool)namedValue.Value;
                            break;
                        case "leftalign":
                            htmlImage.LeftAlign = (bool)(ZBool)namedValue.Value;
                            break;
                        case "rightalign":
                            htmlImage.RightAlign = (bool)(ZBool)namedValue.Value;
                            break;
                        case "width":
                            htmlImage.Width = (int)(ZInt)namedValue.Value;
                            break;
                        case "height":
                            htmlImage.Height = (int)(ZInt)namedValue.Value;
                            break;
                        default:
                            throw new PBException($"unknow patch image value {namedValue.Key}");
                    }
                }
            }
        }

        //private static void SetPictureClassParam_v1(HtmlImage htmlImage, string className)
        //{
        //    // class : leftalign, nonealign, width300, width450
        //    switch (className.ToLower())
        //    {
        //        case "nonealign":
        //            //pictureElement.PictureDrawing = new OXmlPictureInlineDrawing();
        //            htmlImage.NoneAlign = true;
        //            break;
        //        case "leftalign":
        //            //pictureElement.PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 };
        //            //pictureElement.PictureDrawing = new OXmlPictureAnchorDrawing() { Wrap = new OXmlAnchorWrapTight() { SquareSize = 21800 } };
        //            htmlImage.LeftAlign = true;
        //            break;
        //        case "width300":
        //            //pictureElement.Width = 300;
        //            htmlImage.Width = 300;
        //            break;
        //        case "width450":
        //            //pictureElement.Width = 450;
        //            htmlImage.Width = 450;
        //            break;
        //        default:
        //            //throw new pb.PBException($"unknow img class \"{className}\"");
        //            Trace.WriteLine($"unknow img class \"{className}\" \"{htmlImage.File}\"");
        //            break;
        //    }
        //}

        private static void SetPictureWidthHeight(HtmlImage htmlImage)
        {
            if ((htmlImage.Width == null || htmlImage.Height == null) && htmlImage.File != null)
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

        //public static IEnumerable<OXmlElement> ToOXmlXElements(IEnumerable<HtmlDocNode> nodes, WebImage[] webImages = null)
        //{
        //    HtmlToOXmlElements_v2 htmlToOXmlElements = new HtmlToOXmlElements_v2();
        //    htmlToOXmlElements._imageDirectory = imageDirectory;
        //    htmlToOXmlElements._webImages = webImages;
        //    return htmlToOXmlElements._ToOXmlXElements(nodes);
        //}

        // string imagePatchesFile = null
        public static HtmlToOXmlElements_v2 Create(XElement xe, string imageDirectory = null)
        {
            HtmlToOXmlElements_v2 htmlToOXmlElements = new HtmlToOXmlElements_v2();
            htmlToOXmlElements._imageDirectory = imageDirectory;
            //htmlToOXmlElements._imagePatchesFile = imagePatchesFile;
            htmlToOXmlElements.Init(xe);
            return htmlToOXmlElements;
        }
    }
}

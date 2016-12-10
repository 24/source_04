using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OA = DocumentFormat.OpenXml.Drawing;
using OW = DocumentFormat.OpenXml.Wordprocessing;
using ODW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using ODP = DocumentFormat.OpenXml.Drawing.Pictures;
using pb.IO;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

// doc
//   - Standard ECMA-376 Office Open XML File Formats http://www.ecma-international.org/publications/standards/Ecma-376.htm
//   - emus :
//     English Metric Units and Open XML http://polymathprogrammer.com/2009/10/22/english-metric-units-and-open-xml/
//     there are 914400 EMUs per inch, and there are 96 pixels to an inch
//     EMU = pixel * 914400 / 96
//     EMU = pixel * 9525
//
// 300 px ->  7.94 cm
// 450 px -> 11.91 cm
//  94 px ->  2.5  cm
// largeur 18.5 cm = 694 px
// largeur 16 cm   = 600 px
// 11907 1/20 point = 595.35 point = 21 cm, 1 cm = 28.35 point
// 16839 1/20 point = 841.95 point = 29.7 cm, 1 cm = 28.35 point
//
// option for even odd headers footers, WARNING this option dont work for google doc

namespace pb.Data.OpenXml
{
    public enum OXmlDocElementType
    {
        Body = 0,
        Header,
        Footer
    }

    public class OXmlDoc
    {
        private WordprocessingDocument _document = null;
        private MainDocumentPart _mainPart = null;
        private Body _body = null;
        private Settings _settings = null;
        private bool _settingsEvenAndOddHeaders = false;
        private OpenXmlCompositeElement _element = null;    // Body, Header or Footer
        //private bool _headerFooter = false;                 // _element is Header or Footer
        private OXmlDocElementType _currentElement = OXmlDocElementType.Body;
        private SectionProperties _sectionProperties = null;
        private bool _sectionPropertiesTitlePage = false;
        private Paragraph _paragraph = null;
        private Run _run = null;
        private uint _pictureId = 0;

        // Body : BodyType : OpenXmlCompositeElement
        // Header : OpenXmlPartRootElement : OpenXmlCompositeElement

        private void _Create(string file, IEnumerable<OXmlElement> elements)
        {
            using (_document = WordprocessingDocument.Create(file, WordprocessingDocumentType.Document))
            {
                _mainPart = _document.AddMainDocumentPart();
                _mainPart.Document = new Document();
                OXmlPicture.AddNamespaceDeclarations(_mainPart.Document);
                _body = _mainPart.Document.AppendChild(new Body());

                foreach (OXmlElement element in elements)
                {
                    switch (element.Type)
                    {
                        case OXmlElementType.Paragraph:
                            AddParagraph((OXmlParagraphElement)element);
                            break;
                        case OXmlElementType.Text:
                            AddText((OXmlTextElement)element);
                            break;
                        case OXmlElementType.Line:
                            AddLine();
                            break;
                        case OXmlElementType.TabStop:
                            AddTabStop();
                            break;
                        case OXmlElementType.SimpleField:
                            AddSimpleField((OXmlSimpleFieldElement)element);
                            break;
                        case OXmlElementType.DocSection:
                            //if (_headerFooter)
                            if (_currentElement != OXmlDocElementType.Body)
                                throw new PBException("DocSection cant be inside Header or Footer");
                            AddDocSection((OXmlDocSectionElement)element);
                            break;
                        //case OXmlElementType.DocDefaults:
                        //    if (_headerFooter)
                        //        throw new PBException("DocDefaults cant be inside Header or Footer");
                        //    AddDocDefaults((OXmlDocDefaultsElement)element);
                        //    break;
                        case OXmlElementType.DocDefaultsRunProperties:
                            //if (_headerFooter)
                            if (_currentElement != OXmlDocElementType.Body)
                                throw new PBException("DocDefaultsRunProperties cant be inside Header or Footer");
                            AddRunPropertiesDefault((OXmlDocDefaultsRunPropertiesElement)element);
                            break;
                        case OXmlElementType.DocDefaultsParagraphProperties:
                            //if (_headerFooter)
                            if (_currentElement != OXmlDocElementType.Body)
                                throw new PBException("DocDefaultsParagraphProperties cant be inside Header or Footer");
                            //AddParagraphPropertiesDefault((OXmlDocDefaultsParagraphPropertiesElement)element);
                            AddParagraphPropertiesDefault();
                            break;
                        //case OXmlElementType.OpenHeaderFooter:
                        //    OpenHeaderFooter((OXmlOpenHeaderFooter)element);
                        //    break;
                        case OXmlElementType.OpenHeader:
                            if (_currentElement != OXmlDocElementType.Body)
                                throw new PBException("Open header cant be inside Header or Footer");
                            OpenHeader((OXmlOpenHeaderElement)element);
                            break;
                        case OXmlElementType.OpenFooter:
                            if (_currentElement != OXmlDocElementType.Body)
                                throw new PBException("Open footer cant be inside Header or Footer");
                            OpenFooter((OXmlOpenFooterElement)element);
                            break;
                        case OXmlElementType.CloseHeader:
                            if (_currentElement != OXmlDocElementType.Header)
                                throw new PBException("Close header without open header");
                            CloseHeaderFooter();
                            break;
                        case OXmlElementType.CloseFooter:
                            if (_currentElement != OXmlDocElementType.Footer)
                                throw new PBException("Close footer without open footer");
                            CloseHeaderFooter();
                            break;
                        case OXmlElementType.Style:
                            //if (_headerFooter)
                            if (_currentElement != OXmlDocElementType.Body)
                                throw new PBException("Style cant be inside Header or Footer");
                            AddStyle((OXmlStyleElement)element);
                            break;
                        case OXmlElementType.Picture:
                            AddPicture((OXmlPictureElement)element);
                            break;
                        default:
                            throw new PBException($"unknow element type {element.Type}");
                    }
                }
            }
        }

        private void AddParagraph()
        {
            if (_paragraph == null)
                AddParagraph(null);
        }

        private void AddParagraph(OXmlParagraphElement element)
        {
            Paragraph paragraph = new Paragraph();
            if (element != null && element.Style != null)
                paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId { Val = element.Style } };
            _paragraph = _element.AppendChild(paragraph);
            //_run = _paragraph.AppendChild(new Run());
            _run = null;
        }

        private void AddRun()
        {
            if (_run == null)
            {
                if (_paragraph == null)
                    AddParagraph();
                _run = _paragraph.AppendChild(new Run());
            }
        }

        private void AddText(OXmlTextElement element)
        {
            //if (_paragraph == null)
            //    AddParagraph();
            AddRun();
            OW.Text text = new OW.Text(element.Text);
            if (element.PreserveSpace)
                text.Space = SpaceProcessingModeValues.Preserve;
            _run.AppendChild(text);
        }

        private void AddLine()
        {
            //if (_paragraph == null)
            //    AddParagraph();
            AddRun();
            _run.AppendChild(new Break());
        }

        private void AddTabStop()
        {
            //if (_paragraph == null)
            //    AddParagraph();
            AddRun();
            _run.AppendChild(new TabStop());
        }

        private void AddSimpleField(OXmlSimpleFieldElement element)
        {
            //if (_paragraph == null)
            //    AddParagraph();
            AddParagraph();
            _paragraph.AppendChild(new SimpleField { Instruction = element.Instruction });
            //_run = _paragraph.AppendChild(new Run());
            _run = null;
        }

        //private void AddPage()
        //{
        //    _paragraph = _body.AppendChild(new Paragraph());
        //    _run = _paragraph.AppendChild(new Run());
        //}

        private void AddDocSection(OXmlDocSectionElement element)
        {
            CreateSectionProperties();

            // PageSize, <w:pgSz w:w="11907" w:h="16839"/>, 11907 1/20 point = 21 cm, 16839 1/20 point = 29.7 cm, unit = 1/20 point, 11907 1/20 point = 595.35 point
            PageSize pageSize = CreatePageSize(element.PageSize);
            if (pageSize != null)
                _sectionProperties.AppendChild(pageSize);

            // <w:pgMar w:top="720" w:right="1418" w:bottom="720" w:left="1418" w:header="284" w:footer="284" w:gutter="0"/>
            PageMargin pageMargin = CreatePageMargin(element.PageMargin);
            if (pageMargin != null)
                _sectionProperties.AppendChild(pageMargin);

            // PageNumberType, <w:pgNumType w:start="0" />
            if (element.PageNumberStart != null)
                _sectionProperties.AppendChild(new PageNumberType { Start = element.PageNumberStart });
        }

        private static PageSize CreatePageSize(OXmlPageSize oXmlPageSize)
        {
            if (oXmlPageSize == null)
                return null;
            PageSize pageSize = new PageSize();
            if (oXmlPageSize.Width != null)
                pageSize.Width = (uint)oXmlPageSize.Width;
            if (oXmlPageSize.Height != null)
                pageSize.Height = (uint)oXmlPageSize.Height;
            return pageSize;
        }

        private static PageMargin CreatePageMargin(OXmlPageMargin oXmlPageMargin)
        {
            if (oXmlPageMargin == null)
                return null;
            PageMargin pageMargin = new PageMargin();
            //new PageMargin { Top = 720, Bottom = 720, Left = 1418, Right = 1418, Header = 284, Footer = 284 }
            if (oXmlPageMargin.Top != null)
                pageMargin.Top = oXmlPageMargin.Top;
            if (oXmlPageMargin.Bottom != null)
                pageMargin.Bottom = oXmlPageMargin.Bottom;
            if (oXmlPageMargin.Left != null)
                pageMargin.Left = (uint)oXmlPageMargin.Left;
            if (oXmlPageMargin.Right != null)
                pageMargin.Right = (uint)oXmlPageMargin.Right;
            if (oXmlPageMargin.Header != null)
                pageMargin.Header = (uint)oXmlPageMargin.Header;
            if (oXmlPageMargin.Footer != null)
                pageMargin.Footer = (uint)oXmlPageMargin.Footer;
            return pageMargin;
        }

        //private void AddDocDefaults(OXmlDocDefaultsElement element)
        //{
        //    switch (element.DocDefaultsType)
        //    {
        //        case OXmlDocDefaultsType.RunPropertiesDefault:
        //            AddRunPropertiesDefault((OXmlRunPropertiesDefaultElement)element);
        //            break;
        //        case OXmlDocDefaultsType.ParagraphPropertiesDefault:
        //            AddParagraphPropertiesDefault((OXmlParagraphPropertiesDefaultElement)element);
        //            break;
        //        default:
        //            throw new PBException($"unknow doc defaults type {element.DocDefaultsType}");
        //    }
        //}

        private void AddRunPropertiesDefault(OXmlDocDefaultsRunPropertiesElement element)
        {
            Styles styles = CreateStyles();
            DocDefaults docDefaults = CreateDocDefaults(styles);
            if (docDefaults.RunPropertiesDefault == null)
                docDefaults.RunPropertiesDefault = new RunPropertiesDefault();
            if (docDefaults.RunPropertiesDefault.RunPropertiesBaseStyle == null)
                docDefaults.RunPropertiesDefault.RunPropertiesBaseStyle = new RunPropertiesBaseStyle();
            if (element.RunFonts != null)
                docDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts = element.RunFonts.ToRunFonts();
            if (element.FontSize != null)
                docDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.FontSize = new FontSize { Val = element.FontSize };
        }

        //private void AddParagraphPropertiesDefault(OXmlDocDefaultsParagraphPropertiesElement element)
        private void AddParagraphPropertiesDefault()
        {
            Styles styles = CreateStyles();
            DocDefaults docDefaults = CreateDocDefaults(styles);
            if (docDefaults.ParagraphPropertiesDefault == null)
                docDefaults.ParagraphPropertiesDefault = new ParagraphPropertiesDefault();
        }

        //private void OpenHeaderFooter(OXmlOpenHeaderFooter element)
        //{
        //    CreateSectionProperties();
        //    OpenXmlCompositeElement headerFooter;
        //    if (element.Header)
        //    {
        //        HeaderPart headerPart = _mainPart.AddNewPart<HeaderPart>();
        //        headerFooter = new Header();
        //        headerPart.Header = (Header)headerFooter;
        //        string headerPartId = _mainPart.GetIdOfPart(headerPart);
        //        _sectionProperties.AppendChild(new HeaderReference { Id = headerPartId, Type = element.HeaderType });
        //    }
        //    else
        //    {
        //        FooterPart footerPart = _mainPart.AddNewPart<FooterPart>();
        //        headerFooter = new Footer();
        //        footerPart.Footer = (Footer)headerFooter;
        //        string footerPartId = _mainPart.GetIdOfPart(footerPart);
        //        _sectionProperties.AppendChild(new FooterReference { Id = footerPartId, Type = element.HeaderType });
        //    }
        //    AddHeaderFooterNamespaceDeclaration((OpenXmlPartRootElement)headerFooter);

        //    SetHeaderFooterProperties(element.HeaderType);

        //    _element = headerFooter;
        //    _headerFooter = true;
        //}

        private void OpenHeader(OXmlOpenHeaderElement element)
        {
            CreateSectionProperties();
            //if (element.Header)
            //{
                HeaderPart headerPart = _mainPart.AddNewPart<HeaderPart>();
            OpenXmlCompositeElement header = new Header();
                headerPart.Header = (Header)header;
                string headerPartId = _mainPart.GetIdOfPart(headerPart);
                _sectionProperties.AppendChild(new HeaderReference { Id = headerPartId, Type = element.HeaderType });
            //}
            //else
            //{
            //    FooterPart footerPart = _mainPart.AddNewPart<FooterPart>();
            //    headerFooter = new Footer();
            //    footerPart.Footer = (Footer)headerFooter;
            //    string footerPartId = _mainPart.GetIdOfPart(footerPart);
            //    _sectionProperties.AppendChild(new FooterReference { Id = footerPartId, Type = element.HeaderType });
            //}
            AddHeaderFooterNamespaceDeclaration((OpenXmlPartRootElement)header);

            SetHeaderFooterProperties(element.HeaderType);

            _element = header;
            //_headerFooter = true;
            _currentElement = OXmlDocElementType.Header;
        }

        private void OpenFooter(OXmlOpenFooterElement element)
        {
            CreateSectionProperties();
            //if (element.Header)
            //{
            //HeaderPart headerPart = _mainPart.AddNewPart<HeaderPart>();
            //OpenXmlCompositeElement header = new Header();
            //headerPart.Header = (Header)header;
            //string headerPartId = _mainPart.GetIdOfPart(headerPart);
            //_sectionProperties.AppendChild(new HeaderReference { Id = headerPartId, Type = element.HeaderType });
            //}
            //else
            //{
            FooterPart footerPart = _mainPart.AddNewPart<FooterPart>();
            OpenXmlCompositeElement footer = new Footer();
            footerPart.Footer = (Footer)footer;
            string footerPartId = _mainPart.GetIdOfPart(footerPart);
            _sectionProperties.AppendChild(new FooterReference { Id = footerPartId, Type = element.FooterType });
            //}
            AddHeaderFooterNamespaceDeclaration((OpenXmlPartRootElement)footer);

            SetHeaderFooterProperties(element.FooterType);

            _element = footer;
            //_headerFooter = true;
            _currentElement = OXmlDocElementType.Footer;
        }

        private void SetHeaderFooterProperties(HeaderFooterValues type)
        {
            if (!_sectionPropertiesTitlePage && type == HeaderFooterValues.First)
            {
                // TitlePage, <w:titlePg />,  mandatory for first page header
                _sectionProperties.AppendChild(new TitlePage { Val = true });
                _sectionPropertiesTitlePage = true;
            }

            if (!_settingsEvenAndOddHeaders && type == HeaderFooterValues.Even)
            {
                CreateSettings();
                // option for even odd headers footers, WARNING this option dont work for google doc
                _settings.AppendChild(new EvenAndOddHeaders());
                _settingsEvenAndOddHeaders = true;
            }
        }

        private static void AddHeaderFooterNamespaceDeclaration(OpenXmlPartRootElement headerFooter)
        {
            headerFooter.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            headerFooter.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            headerFooter.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            headerFooter.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            headerFooter.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            headerFooter.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            headerFooter.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            headerFooter.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            headerFooter.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            headerFooter.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            headerFooter.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            headerFooter.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            headerFooter.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            headerFooter.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            headerFooter.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");
        }

        private void CloseHeaderFooter()
        {
            _element = _body;
            //_headerFooter = false;
            _currentElement = OXmlDocElementType.Body;
        }

        private void AddStyle(OXmlStyleElement element)
        {
            Styles styles = CreateStyles();
            Style style = new Style();

            style.StyleId = element.Id;
            style.StyleName = new StyleName() { Val = element.Name };
            style.Type = element.StyleType;

            if (element.Aliases != null)
                style.Aliases = new Aliases { Val = element.Aliases };

            style.CustomStyle = element.CustomStyle;
            style.Default = element.DefaultStyle;
            if (element.Locked != null)
                style.Locked = new Locked { Val = GetOnOffValue((bool)element.Locked) };
            if (element.SemiHidden != null)
                style.SemiHidden = new SemiHidden { Val = GetOnOffValue((bool)element.SemiHidden) };
            if (element.StyleHidden != null)
                style.StyleHidden = new StyleHidden { Val = GetOnOffValue((bool)element.StyleHidden) };
            if (element.UnhideWhenUsed != null)
                style.UnhideWhenUsed = new UnhideWhenUsed { Val = GetOnOffValue((bool)element.UnhideWhenUsed) };
            if (element.UIPriority != null)
                style.UIPriority = new UIPriority { Val = element.UIPriority };
            if (element.LinkedStyle != null)
                style.LinkedStyle = new LinkedStyle { Val = element.LinkedStyle };
            if (element.BasedOn != null)
                style.BasedOn = new BasedOn { Val = element.BasedOn };
            if (element.NextParagraphStyle != null)
                style.NextParagraphStyle = new NextParagraphStyle { Val = element.NextParagraphStyle };
            if (element.StyleParagraphProperties != null)
                style.StyleParagraphProperties = element.StyleParagraphProperties.ToStyleParagraphProperties();
            if (element.StyleRunProperties != null)
                style.StyleRunProperties = element.StyleRunProperties.ToStyleRunProperties();

            styles.Append(style);
        }

        private void AddPicture(OXmlPictureElement element)
        {
            //if (_paragraph == null)
            //    AddParagraph();
            AddRun();

            // Drawing : child Inline, Anchor
            // <w:drawing>
            _run.AppendChild(new Drawing(OXmlPicture.Create(_mainPart, element, ++_pictureId)));
        }

        private void CreateSectionProperties()
        {
            if (_sectionProperties == null)
                _sectionProperties = _body.AppendChild(new SectionProperties());
        }

        private DocDefaults CreateDocDefaults(Styles styles)
        {
            DocDefaults docDefaults = styles.DocDefaults;
            if (docDefaults == null)
            {
                docDefaults = new DocDefaults();
                styles.DocDefaults = docDefaults;
            }
            return docDefaults;
        }

        private Styles CreateStyles()
        {
            StyleDefinitionsPart styleDefinitionsPart = _mainPart.StyleDefinitionsPart;
            if (styleDefinitionsPart == null)
                styleDefinitionsPart = _mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styles = new Styles();
                styleDefinitionsPart.Styles = styles;
                styles.Save();
            }
            return styles;
        }

        private void CreateSettings()
        {
            if (_settings == null)
            {
                DocumentSettingsPart documentSettingsPart = _mainPart.AddNewPart<DocumentSettingsPart>();
                _settings = new Settings();
                documentSettingsPart.Settings = _settings;
            }
        }

        private static EnumValue<OnOffOnlyValues> GetOnOffValue(bool value)
        {
            if (value)
                return OnOffOnlyValues.On;
            else
                return OnOffOnlyValues.Off;
        }

        //public static ODW.WrapPolygon CreateWrapPolygon(long horizontalSize, long verticalSize = 0, long startPointX = 0, long startPointY = 0, bool edited = false)
        //{
        //    // Tight Wrapping Extents Polygon, <wp:wrapPolygon>, possible child : StartPoint <wp:start>, LineTo <wp:lineTo>
        //    // Edited : Wrapping Points Modified, <wp:wrapPolygon edited>
        //    if (verticalSize == 0)
        //        verticalSize = horizontalSize;
        //    ODW.WrapPolygon polygon = new ODW.WrapPolygon() { Edited = edited };
        //    // Wrapping Polygon Start, <wp:start x="0" y="0">
        //    polygon.StartPoint = new ODW.StartPoint() { X = startPointX, Y = startPointY };
        //    long x = startPointX;
        //    long y = startPointY;
        //    // Wrapping Polygon Line End Position, <wp:lineTo x="0" y="0"/>
        //    y += verticalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
        //    x += horizontalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
        //    y -= verticalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
        //    x -= horizontalSize; polygon.AppendChild(new ODW.LineTo() { X = x, Y = y });
        //    return polygon;
        //}

        public static void Create(string file, IEnumerable<OXmlElement> elements)
        {
            new OXmlDoc()._Create(file, elements);
        }
    }

    public class OXmlPicture
    {
        private const int _emusInPixel = 9525;
        private MainDocumentPart _mainPart;
        private OXmlPictureElement _pictureElement;
        private string _embeddedReference = null;
        private uint _id;
        private string _file;
        private string _name = null;
        private int? _width;
        private int? _height;
        private long _emuWidth;
        private long _emuHeight;
        private OXmlPictureDrawing _pictureDrawing;

        private OXmlPicture(MainDocumentPart mainPart, OXmlPictureElement pictureElement, uint pictureId)
        {
            _mainPart = mainPart;
            _pictureElement = pictureElement;
            _id = pictureId;
            _file = pictureElement.File;
            _width = pictureElement.Width;
            _height = pictureElement.Height;
            _pictureDrawing = pictureElement.PictureDrawing;
        }

        private OpenXmlCompositeElement _Create()
        {
            SetWidthHeight();
            Trace.WriteLine("add picture \"{0}\" in pixel width {1} height {2} in emu width {3} height {4}", zPath.GetFileName(_file), _width, _height, _emuWidth, _emuHeight);

            ImagePart imagePart = _mainPart.AddImagePart(GetImagePartType(zPath.GetExtension(_file)));
            _embeddedReference = _mainPart.GetIdOfPart(imagePart);

            using (FileStream stream = new FileStream(_file, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            _name = "Picture" + _id.ToString();

            OpenXmlCompositeElement drawingMode = null;
            if (_pictureDrawing.DrawingMode == OXmlPictureDrawingMode.Inline)
                drawingMode = CreateInlineDrawing_v2();
            //else if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.AnchorWrapSquare)
            //    drawingMode = CreateAnchorWrapSquareDrawing();
            //else if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.AnchorWrapTight)
            //    drawingMode = CreateAnchorWrapTightDrawing();
            //else if (_pictureDrawing.DrawingMode == zDocXPictureDrawingMode.AnchorWrapTopBottom)
            //    drawingMode = CreateAnchorWrapTopAndBottomDrawing();
            else if (_pictureDrawing.DrawingMode == OXmlPictureDrawingMode.Anchor)
                drawingMode = CreateAnchorDrawing();
            else
                throw new PBException("unknow drawing mode {0}", _pictureDrawing.DrawingMode);
            return drawingMode;
        }

        //private DW.Anchor CreateAnchorWrapSquareDrawing()
        //{
        //    AnchorDrawing drawing = CreateAnchorDrawing();

        //    drawing.SetWrapSquare(((zDocXPictureWrapSquareAnchorDrawing)_pictureDrawing).WrapText);

        //    return drawing.Create();
        //}

        //private DW.Anchor CreateAnchorWrapTightDrawing()
        //{
        //    AnchorDrawing drawing = CreateAnchorDrawing();

        //    zDocXPictureWrapTightAnchorDrawing pictureWrapTightAnchor = (zDocXPictureWrapTightAnchorDrawing)_pictureDrawing;
        //    // (long)pictureWrapTightAnchor.SquareSize * _emusInPixel
        //    drawing.SetWrapTight(pictureWrapTightAnchor.WrapText, AnchorDrawing.CreateSquareWrapPolygon(pictureWrapTightAnchor.SquareSize));

        //    return drawing.Create();
        //}

        //private DW.Anchor CreateAnchorWrapTopAndBottomDrawing()
        //{
        //    AnchorDrawing drawing = CreateAnchorDrawing();

        //    zDocXPictureWrapTopAndBottomAnchorDrawing pictureWrapTopAndBottomAnchor = (zDocXPictureWrapTopAndBottomAnchorDrawing)_pictureDrawing;
        //    drawing.SetWrapTopAndBottom(pictureWrapTopAndBottomAnchor.DistanceFromTop, pictureWrapTopAndBottomAnchor.DistanceFromBottom, pictureWrapTopAndBottomAnchor.EffectExtent);

        //    return drawing.Create();
        //}

        private ODW.Anchor CreateAnchorDrawing()
        {
            AnchorDrawing drawing = new AnchorDrawing(_embeddedReference, _id, _name, _emuWidth, _emuHeight, _pictureElement.Description);
            drawing.Rotation = _pictureElement.Rotation;
            drawing.HorizontalFlip = _pictureElement.HorizontalFlip;
            drawing.VerticalFlip = _pictureElement.VerticalFlip;
            drawing.CompressionState = _pictureElement.CompressionState;
            drawing.PresetShape = _pictureElement.PresetShape;

            OXmlAnchorPictureDrawing anchorDrawing = (OXmlAnchorPictureDrawing)_pictureDrawing;
            drawing.Wrap = anchorDrawing.Wrap;
            long? horizontalPositionOffset = anchorDrawing.HorizontalPositionOffset;
            if (horizontalPositionOffset != null)
                horizontalPositionOffset = (long)horizontalPositionOffset * _emusInPixel;
            drawing.SetHorizontalPosition(anchorDrawing.HorizontalRelativeFrom, horizontalPositionOffset, anchorDrawing.HorizontalAlignment);
            long? verticalPositionOffset = anchorDrawing.VerticalPositionOffset;
            if (verticalPositionOffset != null)
                verticalPositionOffset = (long)verticalPositionOffset * _emusInPixel;
            drawing.SetVerticalPosition(anchorDrawing.VerticalRelativeFrom, verticalPositionOffset, anchorDrawing.VerticalAlignment);

            return drawing.Create();
        }

        private ODW.Inline CreateInlineDrawing_v2()
        {
            // DW.Inline.EditId : ??? (<wp:inline wp14:editId="">)

            //if (_name == null)
            //    throw new PBException("missing image name");
            //if (width != null && height == null)
            //    throw new PBException("missing height (width has a value)");
            //if (width == null && height != null)
            //    throw new PBException("missing width (height has a value)");

            // <wp:inline>
            ODW.Inline inline = new ODW.Inline()
            {
                DistanceFromTop = 0,          // distT
                DistanceFromBottom = 0,       // distB
                DistanceFromLeft = 0,         // distL
                DistanceFromRight = 0         // distR
                //EditId = "50D07946"         // wp14:editId
            };

            // <wp:extent>
            inline.AppendChild(new ODW.Extent() { Cx = _emuWidth, Cy = _emuHeight });
            // <wp:effectExtent>
            inline.AppendChild(new ODW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L });
            // <wp:docPr>
            // Id = (UInt32Value)1U, Name = "Picture 1"
            inline.AppendChild(new ODW.DocProperties() { Id = _id, Name = _name, Description = _pictureElement.Description });
            // <wp:cNvGraphicFramePr>, <a:graphicFrameLocks>          { NoChangeAspect = true }
            inline.AppendChild(new ODW.NonVisualGraphicFrameDrawingProperties(new OA.GraphicFrameLocks()));

            // <a:graphic>
            OA.Graphic graphic = new OA.Graphic();
            // <a:graphicData>
            OA.GraphicData graphicData = new OA.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };
            // <pic:pic>
            ODP.Picture picture = new ODP.Picture();

            // <pic:nvPicPr>
            ODP.NonVisualPictureProperties pictureProperties = new ODP.NonVisualPictureProperties();
            // <pic:cNvPr>
            pictureProperties.AppendChild(new ODP.NonVisualDrawingProperties() { Id = (UInt32Value)0U, Name = "Image" });
            // <pic:cNvPicPr>
            pictureProperties.AppendChild(new ODP.NonVisualPictureDrawingProperties());
            picture.AppendChild(pictureProperties);

            // <pic:blipFill>
            ODP.BlipFill blipFill = new ODP.BlipFill();

            // <a:blip>
            // CompressionState = A.BlipCompressionValues.Print
            OA.Blip blip = new OA.Blip() { Embed = _embeddedReference, CompressionState = _pictureElement.CompressionState };

            // $$pb todo comment
            // <a:extLst>
            OA.BlipExtensionList blipExtensions = new OA.BlipExtensionList();
            // <a:ext>
            blipExtensions.AppendChild(new OA.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" });
            blip.AppendChild(blipExtensions);
            // $$pb todo comment end

            blipFill.AppendChild(blip);

            // <a:stretch>
            OA.Stretch stretch = new OA.Stretch();
            // <a:fillRect>
            stretch.AppendChild(new OA.FillRectangle());
            blipFill.AppendChild(stretch);

            picture.AppendChild(blipFill);

            // <pic:spPr>
            ODP.ShapeProperties shapeProperties = new ODP.ShapeProperties();

            // <a:xfrm>
            OA.Transform2D transform2D = new OA.Transform2D();
            // new A.Offset
            // <a:off>
            transform2D.AppendChild(new OA.Offset() { X = 0L, Y = 0L });
            // <a:ext>
            transform2D.AppendChild(new OA.Extents() { Cx = _emuWidth, Cy = _emuHeight });
            shapeProperties.AppendChild(transform2D);

            // <a:prstGeom>
            // Preset = A.ShapeTypeValues.Rectangle
            OA.PresetGeometry presetGeometry = new OA.PresetGeometry() { Preset = _pictureElement.PresetShape };
            // <a:avLst>
            presetGeometry.AppendChild(new OA.AdjustValueList());
            shapeProperties.AppendChild(presetGeometry);

            picture.AppendChild(shapeProperties);

            graphicData.AppendChild(picture);
            graphic.AppendChild(graphicData);
            inline.AppendChild(graphic);

            return inline;
        }

        private ODW.Inline CreateInlineDrawing()
        {
            // DW.Inline.EditId : ??? (<wp:inline wp14:editId="">)

            if (_name == null)
                throw new PBException("missing image name");
            //if (width != null && height == null)
            //    throw new PBException("missing image height (width has a value)");
            //if (width == null && height != null)
            //    throw new PBException("missing image width (height has a value)");

            return new ODW.Inline(                                                    // <wp:inline>
                new ODW.Extent()                                                      // <wp:extent>
                {
                    Cx = _emuWidth,
                    Cy = _emuHeight
                },
                new ODW.EffectExtent()                                                // <wp:effectExtent>
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new ODW.DocProperties()                                               // <wp:docPr>
                {
                    Id = _id,
                    Name = _name,
                    Description = _pictureElement.Description
                },
                new ODW.NonVisualGraphicFrameDrawingProperties(                       // <wp:cNvGraphicFramePr>
                    new OA.GraphicFrameLocks()                                        // <a:graphicFrameLocks>          { NoChangeAspect = true }
                ),
                new OA.Graphic(                                                       // <a:graphic>
                    new OA.GraphicData(                                               // <a:graphicData>
                        new ODP.Picture(                                             // <pic:pic>
                            new ODP.NonVisualPictureProperties(                      // <pic:nvPicPr>
                                new ODP.NonVisualDrawingProperties()                 // <pic:cNvPr>
                                {
                                    Id = (UInt32Value)0U,
                                    Name = "Image"
                                },
                                new ODP.NonVisualPictureDrawingProperties()          // <pic:cNvPicPr>
                            ),
                            new ODP.BlipFill(                                        // <pic:blipFill>
                                new OA.Blip(                                          // <a:blip>
                                                                                     // $$pb todo comment
                                    new OA.BlipExtensionList(                       // <a:extLst>
                                        new OA.BlipExtension()                      // <a:ext>
                                        { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }
                                    )
                                // $$pb todo comment end
                                )
                                {
                                    Embed = _embeddedReference,
                                    CompressionState =
                                    OA.BlipCompressionValues.Print
                                },
                                new OA.Stretch(                                       // <a:stretch>
                                    new OA.FillRectangle()                            // <a:fillRect>
                                )
                            ),
                            new ODP.ShapeProperties(                                 // <pic:spPr>
                                new OA.Transform2D(                                   // <a:xfrm>
                                    new OA.Offset()                                   // <a:off>
                                    { X = 0L, Y = 0L },
                                    //new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                    new OA.Extents()                                  // <a:ext>
                                    { Cx = _emuWidth, Cy = _emuHeight }
                                ),
                                new OA.PresetGeometry(                                // <a:prstGeom>
                                    new OA.AdjustValueList()                          // <a:avLst>
                                )
                                { Preset = OA.ShapeTypeValues.Rectangle }
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

        public static OpenXmlCompositeElement Create(MainDocumentPart mainPart, OXmlPictureElement pictureElement, uint pictureId)
        {
            return new OXmlPicture(mainPart, pictureElement, pictureId)._Create();
        }
    }
}

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using pb.IO;

// doc
//   - Style Definition, <w:style>
//     possible child : StyleName <w:name>, Aliases <w:aliases>, BasedOn <w:basedOn>, NextParagraphStyle <w:next>, LinkedStyle <w:link>,
//       AutoRedefine <w:autoRedefine>, StyleHidden <w:hidden>, UIPriority <w:uiPriority>, SemiHidden <w:semiHidden>, UnhideWhenUsed <w:unhideWhenUsed>, PrimaryStyle <w:qFormat>
//       Locked <w:locked>, Personal <w:personal>, PersonalCompose <w:personalCompose>, PersonalReply <w:personalReply>, Rsid <w:rsid>, StyleParagraphProperties <w:pPr>,
//       StyleRunProperties <w:rPr>, StyleTableProperties <w:tblPr>, TableStyleConditionalFormattingTableRowProperties <w:trPr>, StyleTableCellProperties <w:tcPr>, TableStyleProperties <w:tblStylePr>

//<w:sectPr w:rsidR="00154ABF" w:rsidSect="00607768">
//  <w:pgSz w:w="11906" w:h="16838"/>
//  <w:pgMar w:top="720" w:right="1418" w:bottom="720" w:left="1418" w:header="709" w:footer="709" w:gutter="0"/>
//  <w:cols w:space="708"/>
//  <w:docGrid w:linePitch="360"/>
//</w:sectPr>

namespace pb.Data.OpenXml.Test
{
    public static class Test_OpenXml_Style
    {
        private const string _directoryBase = @"c:\pib\dev_data\exe\runsource\test\Test_OpenXml";
        private static string _directory = null;

        public static void SetDirectory()
        {
            _directory = zPath.Combine(_directoryBase, "docs\\Test_OpenXml_Style");
            if (!zDirectory.Exists(_directory))
                zDirectory.CreateDirectory(_directory);
        }

        public static void Test_Style_01()
        {
            SetDirectory();
            string file = "test_Style_01.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(zPath.Combine(_directory, file), WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                //body.AppendChild(Test_OXmlCreator.CreateSectionProperties());
                SectionProperties sectionProperties = body.AppendChild(new SectionProperties());
                Test_OpenXml_Creator.SetSectionPage(sectionProperties);

                //SectionProperties sectionProperties = new SectionProperties();
                //// PageSize, <w:pgSz w:w="11907" w:h="16839"/>, 11907 1/20 point = 21 cm, 16839 1/20 point = 29.7 cm, unit = 1/20 point, 11907 1/20 point = 595.35 point
                //sectionProperties.AppendChild(new PageSize { Width = 11907, Height = 16839 });
                //// top 1.27 cm, bottom 1.27 cm, left 2.5 cm, right 2.5 cm, header 0.5 cm, footer 0.5 cm
                //// <w:pgMar w:top="720" w:right="1418" w:bottom="720" w:left="1418" w:header="284" w:footer="284" w:gutter="0"/>
                //sectionProperties.AppendChild(new PageMargin { Top = 720, Bottom = 720, Left = 1418, Right = 1418, Header = 284, Footer = 284 });
                //body.AppendChild(sectionProperties);

                //Trace.WriteLine("add StyleDefinitionsPart");
                Styles styles = Test_OpenXml_Creator.CreateStyles(mainPart);

                //Trace.WriteLine("create DocDefaults");
                styles.DocDefaults = Test_OpenXml_Creator.CreateDocDefaults();
                //string tinyParagraphStyleId = Test_OXmlCreator.CreateStyleTinyParagraph(styles);
                string tinyParagraphStyleId = "TinyParagraph";
                styles.Append(Test_OpenXml_Creator.CreateTinyParagraphStyle(tinyParagraphStyleId));

                //string styleAliases = "style_01";
                //Trace.WriteLine($"create style {styleId}");


                //Paragraph paragraph = body.AppendChild(new Paragraph());
                //Run run = paragraph.AppendChild(new Run());
                //run.RunProperties = new RunProperties(new RunFonts() { Ascii = "Arial" });

                //RunProperties runProperties = new RunProperties(new RunFonts() { Ascii = "Arial" });
                //Run r = package.MainDocumentPart.Document.Descendants<Run>().First();
                //r.PrependChild<RunProperties>(rPr);

                ParagraphProperties paragraphProperties = new ParagraphProperties();
                // ParagraphStyleId, <w:pStyle>
                paragraphProperties.ParagraphStyleId = new ParagraphStyleId { Val = tinyParagraphStyleId };

                //AddText(body);
                //AddText(body, paragraphProperties);
                body.Append(Test_OpenXml_Creator.CreateText_01(paragraphProperties));
            }
        }

        public static void Test_DocDefaults_01()
        {
            SetDirectory();
            string file = "test_DocDefaults_01.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(zPath.Combine(_directory, file), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());


                // Get the Styles part for this document.
                //StyleDefinitionsPart styleDefinitionsPart = mainPart.StyleDefinitionsPart;

                bool createDocDefaults = true;

                if (createDocDefaults)
                {
                    Styles styles = Test_OpenXml_Creator.CreateStyles(mainPart);
                    styles.DocDefaults = Test_OpenXml_Creator.CreateDocDefaults();

                    //Trace.WriteLine("add StyleDefinitionsPart");
                    //StyleDefinitionsPart styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                    //Styles styles = new Styles();
                    //styleDefinitionsPart.Styles = styles;
                    ////styleDefinitionsPart.Styles.Save();
                    //styles.Save();

                    //Trace.WriteLine("create DocDefaults");
                    //DocDefaults docDefaults = new DocDefaults();
                    //RunPropertiesDefault runPropertiesDefault = new RunPropertiesDefault();
                    //RunPropertiesBaseStyle runPropertiesBaseStyle = new RunPropertiesBaseStyle();
                    //runPropertiesBaseStyle.RunFonts = new RunFonts { Ascii = "Arial" };
                    //runPropertiesBaseStyle.FontSize = new FontSize { Val = "22"  };
                    //runPropertiesDefault.RunPropertiesBaseStyle = runPropertiesBaseStyle;
                    //docDefaults.RunPropertiesDefault = runPropertiesDefault;
                    //styles.DocDefaults = docDefaults;

                    ////RunPropertiesDefault RunPropertiesDefault
                    //docDefaults.ParagraphPropertiesDefault = new ParagraphPropertiesDefault();

                    //string styleId = "style_01";
                    //Trace.WriteLine($"create style {styleId}");

                    //Style style = new Style()
                    //{
                    //    Type = StyleValues.Paragraph,
                    //    StyleId = styleId,
                    //    CustomStyle = true,
                    //    Default = false
                    //};

                    ////styleDefinitionsPart.Styles.Append(style);
                    //styles.Append(style);
                }
                //AddText(body);
                body.Append(Test_OpenXml_Creator.CreateText_01());
            }
        }

        public static void Test_Formule_01(string formule)
        {
            SetDirectory();
            string file = "test_formule_01.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(zPath.Combine(_directory, file), WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                body.Append(Test_OpenXml_Creator.CreateText_02(5));
                body.Append(Test_OpenXml_Creator.CreateParagraph_Formule("test : ", formule));
            }

        }

        public static void Test_Header_01(bool header = false, bool footer = false, bool pageNumber = false)
        {
            SetDirectory();
            string file = "test_header_01.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);

            using (WordprocessingDocument doc = WordprocessingDocument.Create(zPath.Combine(_directory, file), WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                if (header || footer)
                {
                    // activate even and odd headers
                    DocumentSettingsPart documentSettingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
                    Settings settings = new Settings();
                    // <w:evenAndOddHeaders />
                    settings.AppendChild(new EvenAndOddHeaders());
                    documentSettingsPart.Settings = settings;
                }

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());


                Styles styles = Test_OpenXml_Creator.CreateStyles(mainPart);
                //styles.DocDefaults = Test_OXmlCreator.CreateDocDefaults();

                string headerStyleId = null;
                if (header)
                {
                    headerStyleId = "Header";
                    styles.Append(Test_OpenXml_Creator.CreateHeaderFooterStyle(headerStyleId));
                }

                string footerStyleId = null;
                if (footer)
                {
                    footerStyleId = "Footer";
                    styles.Append(Test_OpenXml_Creator.CreateHeaderFooterStyle(footerStyleId));
                }

                string defaultHeaderPartId = null;
                string firstHeaderPartId = null;
                string evenHeaderPartId = null;
                if (header)
                {
                    defaultHeaderPartId = Test_OpenXml_Creator.CreateHeader(mainPart, Test_OpenXml_Creator.CreateParagraph_01(headerStyleId, "Default header", tab: 1));
                    firstHeaderPartId = Test_OpenXml_Creator.CreateHeader(mainPart, Test_OpenXml_Creator.CreateParagraph_01(headerStyleId, "First header", tab: 1));
                    evenHeaderPartId = Test_OpenXml_Creator.CreateHeader(mainPart, Test_OpenXml_Creator.CreateParagraph_01(headerStyleId, "Even header", tab: 1));
                }

                string defaultFooterPartId = null;
                string firstFooterPartId = null;
                string evenFooterPartId = null;
                if (footer)
                {
                    OpenXmlCompositeElement element;
                    if (pageNumber)
                        element = Test_OpenXml_Creator.CreateParagraph_PageNumber(footerStyleId, "Default footer");
                    else
                        element = Test_OpenXml_Creator.CreateParagraph_01(footerStyleId, "Default footer", tab: 1);
                    defaultFooterPartId = Test_OpenXml_Creator.CreateFooter(mainPart, element);

                    if (pageNumber)
                        element = Test_OpenXml_Creator.CreateParagraph_PageNumber(footerStyleId, "First footer");
                    else
                        element = Test_OpenXml_Creator.CreateParagraph_01(footerStyleId, "First footer", tab: 1);
                    firstFooterPartId = Test_OpenXml_Creator.CreateFooter(mainPart, element);

                    if (pageNumber)
                        element = Test_OpenXml_Creator.CreateParagraph_PageNumber(footerStyleId, "Even footer");
                    else
                        element = Test_OpenXml_Creator.CreateParagraph_01(footerStyleId, "Even footer", tab: 1);
                    evenFooterPartId = Test_OpenXml_Creator.CreateFooter(mainPart, element);
                }


                // for SectionProperties
                mainPart.Document.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                //AddSection(body, defaultHeaderPartId, defaultFooterPartId, firstHeaderPartId, firstFooterPartId);
                SectionProperties sectionProperties = body.AppendChild(new SectionProperties());
                Test_OpenXml_Creator.SetSectionPage(sectionProperties);
                Test_OpenXml_Creator.SetSectionHeaders(sectionProperties, defaultHeaderPartId, defaultFooterPartId, firstHeaderPartId, firstFooterPartId, evenHeaderPartId, evenFooterPartId);
                //Test_OXmlCreator.SetSectionPageNumberType(sectionProperties, start: 1);

                //AddText(body, 200);
                body.Append(Test_OpenXml_Creator.CreateText_02(200));
            }
        }
    }
}

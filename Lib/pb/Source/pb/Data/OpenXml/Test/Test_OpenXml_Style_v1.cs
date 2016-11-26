using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;

namespace pb.Data.OpenXml.Test
{
    public static class Test_OpenXml_Style_v1
    {
        private static void Test_Header_02(string documentPath)
        {
            // Replace header in target document with header of source document.
            using (WordprocessingDocument document = WordprocessingDocument.Open(documentPath, true))
            {
                // Get the main document part
                MainDocumentPart mainDocumentPart = document.MainDocumentPart;

                // Delete the existing header and footer parts
                mainDocumentPart.DeleteParts(mainDocumentPart.HeaderParts);
                mainDocumentPart.DeleteParts(mainDocumentPart.FooterParts);

                // Create a new header and footer part
                HeaderPart headerPart = mainDocumentPart.AddNewPart<HeaderPart>();
                FooterPart footerPart = mainDocumentPart.AddNewPart<FooterPart>();

                // Get Id of the headerPart and footer parts
                string headerPartId = mainDocumentPart.GetIdOfPart(headerPart);
                string footerPartId = mainDocumentPart.GetIdOfPart(footerPart);

                GenerateHeaderPartContent(headerPart);

                GenerateFooterPartContent(footerPart);

                // Get SectionProperties and Replace HeaderReference and FooterRefernce with new Id
                IEnumerable<SectionProperties> sections = mainDocumentPart.Document.Body.Elements<SectionProperties>();

                foreach (var section in sections)
                {
                    // Delete existing references to headers and footers
                    section.RemoveAllChildren<HeaderReference>();
                    section.RemoveAllChildren<FooterReference>();

                    // Create the new header and footer reference node
                    section.PrependChild<HeaderReference>(new HeaderReference() { Id = headerPartId });
                    section.PrependChild<FooterReference>(new FooterReference() { Id = footerPartId });
                }
            }
        }

        private static void GenerateHeaderPartContent(HeaderPart part)
        {
            Header header1 = new Header() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            header1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            header1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            header1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            header1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            header1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            header1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            header1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            header1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            header1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            header1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            header1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            header1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            header1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            header1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            header1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Header" };

            paragraphProperties1.Append(paragraphStyleId1);

            Run run1 = new Run();
            DocumentFormat.OpenXml.Wordprocessing.Text text1 = new DocumentFormat.OpenXml.Wordprocessing.Text();
            text1.Text = "Header";

            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            header1.Append(paragraph1);

            part.Header = header1;
        }

        private static void GenerateFooterPartContent(FooterPart part)
        {
            Footer footer1 = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            footer1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Footer" };

            paragraphProperties1.Append(paragraphStyleId1);

            Run run1 = new Run();
            DocumentFormat.OpenXml.Wordprocessing.Text text1 = new DocumentFormat.OpenXml.Wordprocessing.Text();
            text1.Text = "Footer";

            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            footer1.Append(paragraph1);

            part.Footer = footer1;
        }

        public static void Test_03()
        {
            //Test_OpenXml_Style.SetDirectory();
            string file = "test_01.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);

            // from How to: Create and add a paragraph style to a word processing document (Open XML SDK) https://msdn.microsoft.com/fr-fr/library/office/gg188062.aspx

            using (WordprocessingDocument doc = WordprocessingDocument.Open(file, true))
            {
                // Get the Styles part for this document.
                StyleDefinitionsPart part = doc.MainDocumentPart.StyleDefinitionsPart;

                // If the Styles part does not exist, add it and then add the style.
                if (part == null)
                {
                    part = AddStylesPartToPackage(doc);
                }

                // Set up a variable to hold the style ID.
                string parastyleid = "OverdueAmountPara";

                // Create and add a paragraph style to the specified styles part 
                // with the specified style ID, style name and aliases.
                CreateAndAddParagraphStyle(part, parastyleid, "Overdue Amount Para", "Late Due, Late Amount");

                // Add a paragraph with a run and some text.
                //Paragraph p = new Paragraph(new Run(new Text("This is some text in a run in a paragraph.")));
                Paragraph p = new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("This is some text in a run in a paragraph.")));

                // Add the paragraph as a child element of the w:body element.
                doc.MainDocumentPart.Document.Body.AppendChild(p);

                // If the paragraph has no ParagraphProperties object, create one.
                if (p.Elements<ParagraphProperties>().Count() == 0)
                {
                    p.PrependChild<ParagraphProperties>(new ParagraphProperties());
                }

                // Get a reference to the ParagraphProperties object.
                ParagraphProperties pPr = p.ParagraphProperties;

                // If a ParagraphStyleId object doesn't exist, create one.
                if (pPr.ParagraphStyleId == null)
                    pPr.ParagraphStyleId = new ParagraphStyleId();

                // Set the style of the paragraph.
                pPr.ParagraphStyleId.Val = parastyleid;
            }
        }

        // Create a new paragraph style with the specified style ID, primary style name, and aliases and 
        // add it to the specified style definitions part.
        public static void CreateAndAddParagraphStyle(StyleDefinitionsPart styleDefinitionsPart, string styleid, string stylename, string aliases = "")
        {
            // Access the root element of the styles part.
            Styles styles = styleDefinitionsPart.Styles;
            if (styles == null)
            {
                styleDefinitionsPart.Styles = new Styles();
                styleDefinitionsPart.Styles.Save();
            }

            // Create a new paragraph style element and specify some of the attributes.
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true,
                Default = false
            };

            // Create and add the child elements (properties of the style).
            Aliases aliases1 = new Aliases() { Val = aliases };
            AutoRedefine autoredefine1 = new AutoRedefine() { Val = OnOffOnlyValues.Off };
            BasedOn basedon1 = new BasedOn() { Val = "Normal" };
            LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "OverdueAmountChar" };
            Locked locked1 = new Locked() { Val = OnOffOnlyValues.Off };
            PrimaryStyle primarystyle1 = new PrimaryStyle() { Val = OnOffOnlyValues.On };
            StyleHidden stylehidden1 = new StyleHidden() { Val = OnOffOnlyValues.Off };
            SemiHidden semihidden1 = new SemiHidden() { Val = OnOffOnlyValues.Off };
            StyleName styleName1 = new StyleName() { Val = stylename };
            NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
            UIPriority uipriority1 = new UIPriority() { Val = 1 };
            UnhideWhenUsed unhidewhenused1 = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };
            if (aliases != "")
                style.Append(aliases1);
            style.Append(autoredefine1);
            style.Append(basedon1);
            style.Append(linkedStyle1);
            style.Append(locked1);
            style.Append(primarystyle1);
            style.Append(stylehidden1);
            style.Append(semihidden1);
            style.Append(styleName1);
            style.Append(nextParagraphStyle1);
            style.Append(uipriority1);
            style.Append(unhidewhenused1);

            // Create the StyleRunProperties object and specify some of the run properties.
            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            Bold bold1 = new Bold();
            Color color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
            RunFonts font1 = new RunFonts() { Ascii = "Lucida Console" };
            Italic italic1 = new Italic();
            // Specify a 12 point size.
            FontSize fontSize1 = new FontSize() { Val = "24" };
            styleRunProperties1.Append(bold1);
            styleRunProperties1.Append(color1);
            styleRunProperties1.Append(font1);
            styleRunProperties1.Append(fontSize1);
            styleRunProperties1.Append(italic1);

            // Add the run properties to the style.
            style.Append(styleRunProperties1);

            // Add the style to the styles part.
            styles.Append(style);
        }

        // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
        public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc)
        {
            StyleDefinitionsPart part;
            part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            Styles root = new Styles();
            root.Save(part);
            return part;
        }

        // from How to: Set the font for a text run https://msdn.microsoft.com/fr-fr/library/office/cc850848.aspx
        //public static void SetRunFont(string fileName)
        //{
        //    using (WordprocessingDocument package = WordprocessingDocument.Open(fileName, true))
        //    {
        //        // Set the font to Arial to the first Run.
        //        // Use an object initializer for RunProperties and rPr.
        //        RunProperties rPr = new RunProperties(
        //            new RunFonts()
        //            {
        //                Ascii = "Arial"
        //            });

        //        Run r = package.MainDocumentPart.Document.Descendants<Run>().First();
        //        r.PrependChild<RunProperties>(rPr);

        //        // Save changes to the MainDocumentPart part.
        //        package.MainDocumentPart.Document.Save();
        //    }
        //}

        //private static string CreateEmptyHeader(MainDocumentPart mainPart, string styleId)
        //{
        //    HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
        //    //Header header = new Header() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
        //    Header header = new Header();
        //    AddHeaderFooterNamespaceDeclaration(header);
        //    //Paragraph paragraph = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };
        //    Paragraph paragraph = new Paragraph();
        //    paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
        //    Run run = paragraph.AppendChild(new Run());
        //    header.AppendChild(paragraph);
        //    headerPart.Header = header;
        //    return mainPart.GetIdOfPart(headerPart);
        //}

        //private static string CreateHeader(MainDocumentPart mainPart, string styleId)
        //{
        //    HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
        //    //Header header = new Header() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
        //    Header header = new Header();
        //    AddHeaderFooterNamespaceDeclaration(header);
        //    //Paragraph paragraph = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };
        //    Paragraph paragraph = new Paragraph();
        //    paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
        //    Run run = paragraph.AppendChild(new Run());
        //    run.AppendChild(new TabStop());
        //    run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text { Text = "Header" });
        //    header.AppendChild(paragraph);
        //    headerPart.Header = header;
        //    return mainPart.GetIdOfPart(headerPart);
        //}

        //private static string CreateEmptyFooter(MainDocumentPart mainPart, string styleId)
        //{
        //    FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
        //    Footer footer = new Footer();
        //    AddHeaderFooterNamespaceDeclaration(footer);
        //    footerPart.Footer = footer;
        //    Paragraph paragraph = new Paragraph();
        //    paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
        //    Run run = paragraph.AppendChild(new Run());
        //    footer.AppendChild(paragraph);
        //    return mainPart.GetIdOfPart(footerPart);
        //}

        //private static string CreateFooter(MainDocumentPart mainPart, string styleId)
        //{
        //    FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
        //    //Footer footer = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
        //    Footer footer = new Footer();
        //    AddHeaderFooterNamespaceDeclaration(footer);

        //    // SdtBlock, <w:sdt>
        //    SdtBlock sdtBlock = new SdtBlock();

        //    // Structured Document Tag Properties, <w:sdtPr>
        //    SdtProperties sdtProperties = new SdtProperties();
        //    // SdtContentDocPartObject, <w:docPartObj>
        //    SdtContentDocPartObject docPartObject = new SdtContentDocPartObject();
        //    // Document Part Gallery Filter, <w:docPartGallery>
        //    docPartObject.AppendChild(new DocPartGallery { Val = "Page Numbers (Bottom of Page)" });
        //    docPartObject.AppendChild(new DocPartUnique());
        //    sdtProperties.AppendChild(docPartObject);
        //    sdtBlock.SdtProperties = sdtProperties;

        //    // Block-Level Structured Document Tag Content, <w:sdtContent>
        //    SdtContentBlock sdtContentBlock = new SdtContentBlock();
        //    Paragraph paragraph = new Paragraph();
        //    paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
        //    Run run = paragraph.AppendChild(new Run());
        //    run.AppendChild(new TabStop());
        //    run.AppendChild(new TabStop());
        //    run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text { Text = "page ", Space = SpaceProcessingModeValues.Preserve });
        //    // Complex Field Character, <w:fldChar>
        //    run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.Begin });
        //    // Field Code, <w:instrText>
        //    // "PAGE   \\* MERGEFORMAT"  "PAGE"
        //    run.AppendChild(new FieldCode("PAGE   \\* MERGEFORMAT"));
        //    run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.Separate });
        //    run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text { Text = "" });
        //    run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.End });
        //    sdtContentBlock.AppendChild(paragraph);
        //    sdtBlock.SdtContentBlock = sdtContentBlock;

        //    footer.AppendChild(sdtBlock);

        //    //Paragraph paragraph = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };
        //    //Paragraph paragraph = new Paragraph();
        //    //paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
        //    //Run run = paragraph.AppendChild(new Run());
        //    //run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text { Text = "page " });
        //    //// Field Code, <w:instrText>
        //    //// "PAGE   \\* MERGEFORMAT"
        //    //run.AppendChild(new FieldCode("PAGE"));
        //    //footer.AppendChild(paragraph);

        //    footerPart.Footer = footer;
        //    return mainPart.GetIdOfPart(footerPart);
        //}


        //public static void Test_02()
        //{
        //    SetDirectory();
        //    string file = "test_01.docx";
        //    Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);


        //    // from OpenXML Add paragraph style (Heading1,Heading2, Head 3 Etc) to a word processing document http://stackoverflow.com/questions/17922763/openxml-add-paragraph-style-heading1-heading2-head-3-etc-to-a-word-processing
        //    //WordprocessingDocument wordDocument = WordprocessingDocument.Create(file, WordprocessingDocumentType.Document);
        //    //MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
        //    //mainPart.Document = new Document();
        //    //Body body = mainPart.Document.AppendChild(new Body());
        //    //Paragraph para = body.AppendChild(new Paragraph());
        //    //Run run = para.AppendChild(new Run());
        //    //run.AppendChild(new Text("Executive Summary"));
        //    //StyleDefinitionPart styleDefinitionsPart = wordDocument.AddStylesDefinitionPart();
        //    //Styles styles = styleDefinitionsPart.Styles;
        //    //Style style = new Style()
        //    //{
        //    //    Type = StyleValues.Paragraph,
        //    //    StyleId = styleid,
        //    //    CustomStyle = true
        //    //};
        //    //StyleName styleName1 = new StyleName() { Val = "Heading1" };
        //    //style.Append(styleName1);
        //    //StyleRunProperties styleRunProperties1 = new StyleRunProperties();
        //    //styleRunProperties1.Append(new Bold);
        //    //styleRunProperties1.Append(new Italic());
        //    //styleRunProperties1.Append(new RunFonts() { Ascii = "Lucida Console" };);
        //    //styleRunProperties1.Append(new FontSize() { Val = "24" });  // Sizes are in half-points. Oy!
        //    //style.Append(styleRunProperties1);
        //    //styles.Append(style);
        //    //pPr.ParagraphStyleId = new ParagraphStyleId() { Val = "Heading1" };
        //    //para.PrependChild<ParagraphProperties>(new ParagraphProperties());
        //}
    }
}

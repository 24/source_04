using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DW = DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

// 17.16.5 Field definitions page 1188
//   Category : Date and Time, Document Automation, Document Information, Equations and Formulas, Form Fields, Index and Tables, Links and References, Mail Merge, Numbering , User Information
//   Equations and Formulas : = formula (§17.16.3), ADVANCE (§17.16.5.2), SYMBOL (§17.16.5.61)
//   Numbering : PAGE (§17.16.5.44)
//   SimpleField, <w:fldSimple w:instr="PAGE" w:fldLock="0" w:dirty="0">

namespace pb.Data.OpenXml.Test
{
    public static class Test_OpenXml_Creator
    {
        public static Styles CreateStyles(MainDocumentPart mainPart)
        {
            StyleDefinitionsPart styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = new Styles();
            styleDefinitionsPart.Styles = styles;
            //styles.Save();
            return styles;
        }

        public static DocDefaults CreateDocDefaults()
        {
            DocDefaults docDefaults = new DocDefaults();
            //styles.DocDefaults = docDefaults;
            docDefaults.ParagraphPropertiesDefault = new ParagraphPropertiesDefault();
            return docDefaults;
        }

        //public static SectionProperties CreateSectionProperties()
        //{
        //    SectionProperties sectionProperties = new SectionProperties();
        //    return sectionProperties;
        //}

        public static void SetSectionPage(SectionProperties sectionProperties)
        {
            // PageSize, <w:pgSz w:w="11907" w:h="16839"/>, 11907 1/20 point = 21 cm, 16839 1/20 point = 29.7 cm, unit = 1/20 point, 11907 1/20 point = 595.35 point
            sectionProperties.AppendChild(new PageSize { Width = 11907, Height = 16839 });
            // top 1.27 cm, bottom 1.27 cm, left 2.5 cm, right 2.5 cm, header 0.5 cm, footer 0.5 cm
            // <w:pgMar w:top="720" w:right="1418" w:bottom="720" w:left="1418" w:header="284" w:footer="284" w:gutter="0"/>
            sectionProperties.AppendChild(new PageMargin { Top = 720, Bottom = 720, Left = 1418, Right = 1418, Header = 284, Footer = 284 });
        }

        public static void SetSectionPageNumberType(SectionProperties sectionProperties, int start = 0)
        {
            // PageNumberType, <w:pgNumType w:start="0" />
            sectionProperties.AppendChild(new PageNumberType { Start = start });
        }

        //public static void AddSection(Body body, string defaultHeaderPartId = null, string defaultFooterPartId = null, string firstHeaderPartId = null, string firstFooterPartId = null)
        public static void SetSectionHeaders(SectionProperties sectionProperties, string defaultHeaderPartId = null, string defaultFooterPartId = null, string firstHeaderPartId = null, string firstFooterPartId = null,
            string evenHeaderPartId = null, string evenFooterPartId = null)
        {
            // TitlePage, <w:titlePg />,  mandatory for first page header
            sectionProperties.AppendChild(new TitlePage { Val = true });

            // EnumValue<HeaderFooterValues> Type : Even, Default, First
            if (defaultHeaderPartId != null)
                sectionProperties.AppendChild(new HeaderReference { Id = defaultHeaderPartId, Type = HeaderFooterValues.Default });
            if (defaultFooterPartId != null)
                sectionProperties.AppendChild(new FooterReference { Id = defaultFooterPartId, Type = HeaderFooterValues.Default });

            if (firstHeaderPartId != null)
                sectionProperties.AppendChild(new HeaderReference { Id = firstHeaderPartId, Type = HeaderFooterValues.First });
            if (firstFooterPartId != null)
                sectionProperties.AppendChild(new FooterReference { Id = firstFooterPartId, Type = HeaderFooterValues.First });

            if (evenHeaderPartId != null)
                sectionProperties.AppendChild(new HeaderReference { Id = evenHeaderPartId, Type = HeaderFooterValues.Even });
            if (evenFooterPartId != null)
                sectionProperties.AppendChild(new FooterReference { Id = evenFooterPartId, Type = HeaderFooterValues.Even });
        }

        // Styles styles
        public static Style CreateTinyParagraphStyle(string styleId, string styleName = null)
        {
            //string styleId = "TinyParagraph";
            //string styleName = "TinyParagraph";
            if (styleName == null)
                styleName = styleId;

            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleId,
                CustomStyle = true,
                Default = false
            };

            style.StyleName = new StyleName() { Val = styleName };
            //if (styleAliases != "")
            //    style.Aliases = new Aliases() { Val = styleAliases };
            //style.AutoRedefine = new AutoRedefine() { Val = OnOffOnlyValues.Off };
            //style.BasedOn = new BasedOn() { Val = "Normal" };
            //style.LinkedStyle = new LinkedStyle() { Val = "Test" };
            //style.Locked = new Locked() { Val = OnOffOnlyValues.Off };
            //style.PrimaryStyle = new PrimaryStyle() { Val = OnOffOnlyValues.On };
            //style.StyleHidden = new StyleHidden() { Val = OnOffOnlyValues.Off };
            //style.SemiHidden = new SemiHidden() { Val = OnOffOnlyValues.Off };
            //style.NextParagraphStyle = new NextParagraphStyle() { Val = "Normal" };
            //style.UIPriority = new UIPriority() { Val = 1 };
            //style.UnhideWhenUsed = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };

            StyleParagraphProperties styleParagraphProperties = new StyleParagraphProperties();
            // interligne 1 = 240 = 12 pt
            // ouvert  : avant 0 pt, après 10 pt, interligne 1.15, <w:spacing w:after="200" w:line="276" w:lineRule="auto"/>
            // aucun   : avant 0 pt, après  0 pt, interligne 1
            // compact : avant 0 pt, après  4 pt, interligne 1,    <w:spacing w:after="80"/>
            // SpacingBetweenLines, <w:spacing>
            styleParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines { Line = "24" };   // 480 = 2 lignes, 24 = 1/10 ligne
            style.StyleParagraphProperties = styleParagraphProperties;

            //StyleRunProperties styleRunProperties = new StyleRunProperties();
            //styleRunProperties.Bold = new Bold();
            //styleRunProperties.Color = new Color() { ThemeColor = ThemeColorValues.Accent2 };
            //styleRunProperties.RunFonts = new RunFonts() { Ascii = "Lucida Console" };
            //styleRunProperties.Italic = new Italic();
            //styleRunProperties.FontSize = new FontSize() { Val = "24" }; // 12 point size.
            //style.StyleRunProperties = styleRunProperties;

            //styles.Append(style);
            //return styleId;
            return style;
        }

        // Styles styles
        public static Style CreateHeaderFooterStyle(string styleId)
        {
            //string styleId = "Header";
            //string styleName = "Header";

            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleId,
                CustomStyle = true,
                Default = false
            };

            style.StyleName = new StyleName() { Val = styleId };
            //if (styleAliases != "")
            //    style.Aliases = new Aliases() { Val = styleAliases };
            //style.AutoRedefine = new AutoRedefine() { Val = OnOffOnlyValues.Off };
            //style.BasedOn = new BasedOn() { Val = "Normal" };
            //style.LinkedStyle = new LinkedStyle() { Val = "Test" };
            //style.Locked = new Locked() { Val = OnOffOnlyValues.Off };
            //style.PrimaryStyle = new PrimaryStyle() { Val = OnOffOnlyValues.On };
            //style.StyleHidden = new StyleHidden() { Val = OnOffOnlyValues.Off };
            //style.SemiHidden = new SemiHidden() { Val = OnOffOnlyValues.Off };
            //style.NextParagraphStyle = new NextParagraphStyle() { Val = "Normal" };
            //style.UIPriority = new UIPriority() { Val = 1 };
            //style.UnhideWhenUsed = new UnhideWhenUsed() { Val = OnOffOnlyValues.On };

            StyleParagraphProperties styleParagraphProperties = new StyleParagraphProperties();
            // Tabs, <w:tabs>
            Tabs tabs = new Tabs();
            // Custom Tab Stop, <w:tab>
            tabs.AppendChild(new TabStop { Position = 4536, Val = TabStopValues.Center });
            tabs.AppendChild(new TabStop { Position = 9072, Val = TabStopValues.Right });
            styleParagraphProperties.Tabs = tabs;
            style.StyleParagraphProperties = styleParagraphProperties;

            //StyleRunProperties styleRunProperties = new StyleRunProperties();
            //styleRunProperties.Bold = new Bold();
            //styleRunProperties.Color = new Color() { ThemeColor = ThemeColorValues.Accent2 };
            //styleRunProperties.RunFonts = new RunFonts() { Ascii = "Lucida Console" };
            //styleRunProperties.Italic = new Italic();
            //styleRunProperties.FontSize = new FontSize() { Val = "24" }; // 12 point size.
            //style.StyleRunProperties = styleRunProperties;

            //styles.Append(style);
            //return styleId;
            return style;
        }

        public static string CreateHeader(MainDocumentPart mainPart, OpenXmlCompositeElement element)
        {
            HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
            Header header = new Header();
            AddHeaderFooterNamespaceDeclaration(header);
            if (element != null)
                header.AppendChild(element);
            headerPart.Header = header;
            return mainPart.GetIdOfPart(headerPart);
        }

        public static string CreateFooter(MainDocumentPart mainPart, OpenXmlCompositeElement element)
        {
            FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
            Footer footer = new Footer();
            AddHeaderFooterNamespaceDeclaration(footer);
            if (element != null)
                footer.AppendChild(element);
            footerPart.Footer = footer;
            return mainPart.GetIdOfPart(footerPart);
        }

        public static Paragraph CreateParagraph_Formule(string text, string formule, string styleId = null)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new DW.Text { Text = text, Space = SpaceProcessingModeValues.Preserve });
            paragraph.AppendChild(new SimpleField { Instruction = formule });
            return paragraph;
        }

        public static Paragraph CreateParagraph_PageNumber(string styleId, string text)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new TabStop());
            run.AppendChild(new DW.Text { Text = text });
            run.AppendChild(new TabStop());
            run.AppendChild(new DW.Text { Text = "page ", Space = SpaceProcessingModeValues.Preserve });
            // fldChar (Complex Field Character)
            paragraph.AppendChild(new SimpleField { Instruction = "PAGE" });
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new DW.Text { Text = "/", Space = SpaceProcessingModeValues.Preserve });
            paragraph.AppendChild(new SimpleField { Instruction = "NUMPAGES" });
            return paragraph;
        }

        // ATTENTION : dont work with google doc use CreateParagraph_PageNumber()
        public static SdtBlock CreatePageNumberBlock(string styleId, string text)
        {
            // SdtBlock, <w:sdt>
            SdtBlock sdtBlock = new SdtBlock();

            // Structured Document Tag Properties, <w:sdtPr>
            SdtProperties sdtProperties = new SdtProperties();
            // SdtContentDocPartObject, <w:docPartObj>
            SdtContentDocPartObject docPartObject = new SdtContentDocPartObject();
            // Document Part Gallery Filter, <w:docPartGallery>
            docPartObject.AppendChild(new DocPartGallery { Val = "Page Numbers (Bottom of Page)" });
            docPartObject.AppendChild(new DocPartUnique());
            sdtProperties.AppendChild(docPartObject);
            sdtBlock.SdtProperties = sdtProperties;

            // Block-Level Structured Document Tag Content, <w:sdtContent>
            SdtContentBlock sdtContentBlock = new SdtContentBlock();
            Paragraph paragraph = new Paragraph();
            paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new TabStop());
            run.AppendChild(new DW.Text { Text = text });
            run.AppendChild(new TabStop());
            run.AppendChild(new DW.Text { Text = "page ", Space = SpaceProcessingModeValues.Preserve });
            // Complex Field Character, <w:fldChar>
            run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.Begin });
            // Field Code, <w:instrText>
            // "PAGE   \\* MERGEFORMAT"  "PAGE"
            run.AppendChild(new FieldCode("PAGE   \\* MERGEFORMAT"));
            run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.Separate });
            run.AppendChild(new DW.Text { Text = "" });
            run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.End });
            sdtContentBlock.AppendChild(paragraph);
            sdtBlock.SdtContentBlock = sdtContentBlock;

            return sdtBlock;
        }

        public static Paragraph CreateParagraph_01(string styleId, string text, int tab = 0)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.ParagraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = styleId } };
            Run run = paragraph.AppendChild(new Run());
            for (int i = 0; i < tab; i++)
                run.AppendChild(new TabStop());
            run.AppendChild(new DW.Text { Text = text });
            return paragraph;
        }

        //public static void AddText(Body body, ParagraphProperties paragraphProperties = null)
        public static IEnumerable<OpenXmlElement> CreateText_01(ParagraphProperties paragraphProperties = null)
        {
            //Paragraph paragraph = body.AppendChild(new Paragraph());
            Paragraph paragraph = new Paragraph();
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new DW.Text("zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo"));
            run.AppendChild(new Break());
            run.AppendChild(new DW.Text("zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo"));
            run.AppendChild(new Break());
            run.AppendChild(new DW.Text("zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo zozo"));
            yield return paragraph;

            //paragraph = body.AppendChild(new Paragraph());
            paragraph = new Paragraph();
            if (paragraphProperties != null)
                paragraph.ParagraphProperties = (ParagraphProperties)paragraphProperties.Clone();
            run = paragraph.AppendChild(new Run());
            yield return paragraph;

            //paragraph = body.AppendChild(new Paragraph());
            paragraph = new Paragraph();
            //if (paragraphProperties != null)
            //    paragraph.ParagraphProperties = (ParagraphProperties)paragraphProperties.Clone();
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new DW.Text("toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto"));
            run.AppendChild(new Break());
            run.AppendChild(new DW.Text("toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto"));
            run.AppendChild(new Break());
            run.AppendChild(new DW.Text("toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto toto"));
            yield return paragraph;

            //paragraph = body.AppendChild(new Paragraph());
            paragraph = new Paragraph();
            if (paragraphProperties != null)
                paragraph.ParagraphProperties = (ParagraphProperties)paragraphProperties.Clone();
            run = paragraph.AppendChild(new Run());
            yield return paragraph;

            //paragraph = body.AppendChild(new Paragraph());
            paragraph = new Paragraph();
            //if (paragraphProperties != null)
            //    paragraph.ParagraphProperties = (ParagraphProperties)paragraphProperties.Clone();
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new DW.Text("tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata"));
            run.AppendChild(new Break());
            run.AppendChild(new DW.Text("tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata"));
            run.AppendChild(new Break());
            run.AppendChild(new DW.Text("tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata tata"));
            yield return paragraph;

            //paragraph = body.AppendChild(new Paragraph());
            for (int i = 0; i < 11; i++)
            {
                paragraph = new Paragraph();
                run = paragraph.AppendChild(new Run());
                yield return paragraph;
            }
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
            //paragraph = body.AppendChild(new Paragraph());
            //run = paragraph.AppendChild(new Run());
        }

        //public static void AddText(Body body, int line, ParagraphProperties paragraphProperties = null)
        public static IEnumerable<OpenXmlElement> CreateText_02(int line, ParagraphProperties paragraphProperties = null)
        {
            //Paragraph paragraph = body.AppendChild(new Paragraph());
            Paragraph paragraph = new Paragraph();
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new DW.Text("test"));
            yield return paragraph;

            for (int i = 0; i < line; i++)
            {
                //paragraph = body.AppendChild(new Paragraph());
                paragraph = new Paragraph();
                if (paragraphProperties != null)
                    paragraph.ParagraphProperties = (ParagraphProperties)paragraphProperties.Clone();
                run = paragraph.AppendChild(new Run());
                yield return paragraph;
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
    }
}

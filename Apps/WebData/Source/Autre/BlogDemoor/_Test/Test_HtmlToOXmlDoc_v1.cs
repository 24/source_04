using DocumentFormat.OpenXml.Wordprocessing;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.OpenXml;
using pb.Data.Xml;
using pb.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

// todo :
//   - pb 2 tailles d'image
//     <a href="http://p0.storage.canalblog.com/05/94/149189/49815487.jpg" target="_blank"><img src="http://p0.storage.canalblog.com/05/94/149189/49815487_p.jpg" border="0" alt="knodel_2" width="203" height="300" /></a>
//     <a name="IMG_112509351" href="http://p2.storage.canalblog.com/24/99/1436926/112509351_o.jpg" target="_blank"><img src="http://p2.storage.canalblog.com/24/99/1436926/112509351.jpg" border="0" alt="Plovdiv1" class="nonealign width450" /></a>
//   - image venant d'un autre site
//     <a href="http://voyages.ideoz.fr/wp-content/uploads/2012/03/knodels-souabes.jpeg" rel="prettyPhoto"><img src="http://voyages.ideoz.fr/wp-content/uploads/2012/03/knodels-souabes-180x136.jpeg" border="0" alt="knodels cuisine de baviere" title="knodels cuisine de baviere" width="244" height="184" class="alignleft size-full wp-image-66029 no-display appear" /></a>
//   - formattage du texte
//     <span style="text-decoration: underline;">
//     <span style="font-family: tahoma,arial,helvetica,sans-serif;">
//   - récupérer le fichier de l'image à partir de son url
//
//   - problème de la police de caractère l'affichage des accents en gras n'est pas bon
//   - "très bonne adaptation au pays !" url "http://dccjta6europe.canalblog.com/archives/2016/04/19/33690043.html"
//     réduire la taille des 4 images pour avoir tout sur une page
//   - "plovdiv : 2ème ville" url "http://dccjta6europe.canalblog.com/archives/2016/09/15/34324922.html"
//     réduire la 2ème et 3ème image pour les avoir cote à cote
//   - "Lisbonne centre et bord de mer" url "http://dccjta6europe.canalblog.com/archives/2016/04/20/33690064.html"
//     vérifier le formattage des images
//   - "à la recherche d'une poussette - découverte d'un très bel endroit" url "http://dccjta6europe.canalblog.com/archives/2016/04/20/33690067.html"
//     alignement vertical bas de 3 images

namespace WebData.BlogDemoor.Test
{
    public static class Test_HtmlToOXmlDoc_v1
    {
        private static CultureInfo _frCulture = null;
        //private static string _imageDirectory = @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images";

        static Test_HtmlToOXmlDoc_v1()
        {
            SetFrCulture();
        }

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
                .zSave(traceFile, jsonIndent: true);
        }

        public static void Test_Trace_HtmlDoc(string file)
        {
            string traceFile = file + ".htmldoc.trace";
            HtmlDocReader.ReadFile(file).zSave(traceFile, jsonIndent: true);
        }

        public static void Test_Trace_HtmlDocText(string file)
        {
            string traceFile = file + ".htmldoc.text.trace";
            HtmlDocReader.ReadFile(file).zTraceToFile(traceFile);
        }

        public static void Test_Trace_HtmlToOXml_v1(string file)
        {
            string traceFile = file + ".oxml.json";
            HtmlToOXmlElements_v1.ToOXmlXElements(HtmlDocReader.ReadFile(file)).zSave(traceFile, jsonIndent: true);
        }

        public static void Test_Trace_HtmlToOXml_v2(string file, string imageDirectory)
        {
            string traceFile = file + ".oxml.json";
            //HtmlToOXmlElements_v2.ToOXmlXElements(HtmlDocReader.ReadFile(file), imageDirectory).zSave(traceFile);
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(BlogDemoor_v4.GetConfigElement().zXPathElement("HtmlToDocx"), imageDirectory);
            HtmlToOXmlElements_v2 htmlToOXmlElements = CreateHtmlToOXmlElements(BlogDemoor_v4.GetConfigElement(), imageDirectory);
            htmlToOXmlElements.ToOXmlXElements(HtmlDocReader.ReadFile(file)).zSave(traceFile, jsonIndent: true);
        }

        public static void Test_Trace_HtmlToOXmlText(string file)
        {
            string traceFile = file + ".htmltooxml.text.trace";
            HtmlToOXmlElements_v1.ToOXmlXElements(HtmlDocReader.ReadFile(file)).zTraceToFile(traceFile);
        }

        //public static void Test_HtmlToDocx_v1(string file, string imageDirectory, string oxmlHeaderFile = null, string title = null)
        //{
        //    IEnumerable<OXmlElement> elements = new OXmlElement[0];
        //    if (oxmlHeaderFile != null)
        //        elements = zmongo.BsonRead<OXmlElement>(oxmlHeaderFile);
        //    if (title != null)
        //        elements = elements.Union(GetTitle(title));
        //    OXmlDoc.Create(file + ".docx", elements.Union(HtmlToOXmlElements_v2.ToOXmlXElements(HtmlDocReader.ReadFile(file), imageDirectory)));
        //}

        //public static void Test_HtmlPagesToDocx(string file, string imageDirectory, string title, string date, string footerText = null)
        //{
        //    HtmlPage htmlPage = new HtmlPage { Title = title, Date = date, HtmlNodes = HtmlDocReader.ReadFile(file) };
        //    HtmlPagesToDocx(file + ".docx", imageDirectory, new HtmlPage[] { htmlPage, htmlPage, htmlPage }, footerText);
        //}

        public static void Test_BlogDemoorToDocx(string file, string footerText, int limit = 0, int skip = 0, string parameters = null)
        {
            BlogDemoor_v4 dataManager = WebData.CreateDataManager_v4(parameters);
            //IEnumerable<BlogDemoorDetailData> pages = dataManager.HeaderDetailManager.LoadDetails(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImageFromWeb, loadImageToData, refreshImage, refreshDocumentStore);
            //pages = pages.Reverse();
            //if (limit != 0)
            //    pages = pages.Take(limit);
            //pages = pages.zAction(page => Trace.WriteLine($"title \"{page.Title}\" url \"{page.SourceUrl}\""));
            //IEnumerable<HtmlPage> htmlPages = pages.Select(page => DetailDataToHtmlPage(page));
            //string imageDirectory = dataManager.DetailDataManager.WebLoadImageManager.WebImageCacheManager.UrlCache.CacheDirectory;
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(dataManager.ConfigElement.zXPathElement("HtmlToDocx"), imageDirectory);
            OXmlDoc.Create(file, HtmlPagesToOXmlElements(CreateHtmlToOXmlElements(dataManager), GetHtmlPages(dataManager, limit, skip), footerText));
        }

        public static HtmlToOXmlElements_v2 CreateHtmlToOXmlElements(BlogDemoor_v4 dataManager)
        {
            return CreateHtmlToOXmlElements(dataManager.ConfigElement, dataManager.DetailDataManager.WebLoadImageManager.WebImageCacheManager.UrlCache.CacheDirectory);
        }

        public static HtmlToOXmlElements_v2 CreateHtmlToOXmlElements(XElement xe, string imageDirectory)
        {
            HtmlToOXmlElements_v2 htmlToOXmlElements = new HtmlToOXmlElements_v2();
            htmlToOXmlElements.ImageDirectory = imageDirectory;
            htmlToOXmlElements.Init(xe.zXPathElement("HtmlToDocx"));
            htmlToOXmlElements.TextRemoveBlank = true;
            htmlToOXmlElements.TextRemoveLine = true;
            //htmlToOXmlElements.ForcedMaxImageWidth = 300;
            htmlToOXmlElements.MaxImageHorizontalSize = 700;
            htmlToOXmlElements.ImageHorizontalLeftMarge = 2;
            htmlToOXmlElements.ImageHorizontalMarge = 3;
            htmlToOXmlElements.ImageHorizontalBorder = 71755; // 71755 = 0.2 cm
            htmlToOXmlElements.ImageSeparationParagraphStyle = "TinyParagraph";
            htmlToOXmlElements.UseLinkImage = false;  // true : take image link from tag a, false : take image link from tag img
            return htmlToOXmlElements;
        }

        public static void Test_BlogDemoorToDocx(string file, IEnumerable<int> detailIds, string footerText, string parameters = null)
        {
            BlogDemoor_v4 dataManager = WebData.CreateDataManager_v4(parameters);
            //IEnumerable<BlogDemoorDetailData> pages = ReadDetails(dataManager, detailIds);
            //pages = pages.zAction(page => Trace.WriteLine($"title \"{page.Title}\" url \"{page.SourceUrl}\""));
            //IEnumerable<HtmlPage> htmlPages = pages.Select(page => DetailDataToHtmlPage(page));
            //string imageDirectory = dataManager.DetailDataManager.WebLoadImageManager.WebImageCacheManager.UrlCache.CacheDirectory;
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(dataManager.ConfigElement.zXPathElement("HtmlToDocx"), imageDirectory);
            OXmlDoc.Create(file, HtmlPagesToOXmlElements(CreateHtmlToOXmlElements(dataManager), GetHtmlPages(dataManager, detailIds), footerText));
        }

        public static void Test_BlogDemoorToOXml(string file, IEnumerable<int> detailIds, string footerText, string parameters = null)
        {
            BlogDemoor_v4 dataManager = WebData.CreateDataManager_v4(parameters);
            //string imageDirectory = dataManager.DetailDataManager.WebLoadImageManager.WebImageCacheManager.UrlCache.CacheDirectory;
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(dataManager.ConfigElement.zXPathElement("HtmlToDocx"), imageDirectory);
            HtmlToOXmlElements_v2 htmlToOXmlElements = CreateHtmlToOXmlElements(dataManager);
            int i = 0;
            foreach (HtmlPage page in GetHtmlPages(dataManager, detailIds))
            {
                string traceFile = file;
                if (i != 0)
                    traceFile += $".{i}";
                traceFile += ".oxml.json";
                //htmlToOXmlElements.ToOXmlXElements(page.HtmlNodes).zSave(traceFile, jsonIndent: true);
                HtmlPageToOXmlElements(htmlToOXmlElements, page, footerText).zSave(traceFile, jsonIndent: true);
            }
        }

        public static IEnumerable<HtmlPage> GetHtmlPages(BlogDemoor_v4 dataManager, int limit = 0, int skip = 0)
        {
            int startPage = 1;
            int maxPage = 0;
            bool reloadHeaderPage = false;
            bool reloadDetail = false;
            bool loadImageFromWeb = false;
            bool loadImageToData = false;
            bool refreshImage = false;
            bool refreshDocumentStore = false;
            IEnumerable<BlogDemoorDetailData> pages = dataManager.HeaderDetailManager.LoadDetails(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImageFromWeb, loadImageToData, refreshImage, refreshDocumentStore);
            pages = pages.Reverse();
            if (skip != 0)
                pages = pages.Skip(skip);
            if (limit != 0)
                pages = pages.Take(limit);
            pages = pages.zAction(page => Trace.WriteLine($"title \"{page.Title}\" url \"{page.SourceUrl}\""));
            return pages.Select(page => DetailDataToHtmlPage(page));
        }

        public static IEnumerable<HtmlPage> GetHtmlPages(BlogDemoor_v4 dataManager, IEnumerable<int> detailIds)
        {
            IEnumerable<BlogDemoorDetailData> pages = ReadDetails(dataManager, detailIds);
            pages = pages.zAction(page => Trace.WriteLine($"title \"{page.Title}\" url \"{page.SourceUrl}\""));
            return pages.Select(page => DetailDataToHtmlPage(page));
        }

        public static IEnumerable<BlogDemoorDetailData> ReadDetails(BlogDemoor_v4 dataManager, IEnumerable<int> detailIds)
        {
            foreach (int detailId in detailIds)
            {
                yield return dataManager.DetailDataManager.LoadFromId(detailId);
            }
        }

        public static void SetFrCulture()
        {
            _frCulture = CultureInfo.GetCultureInfo("fr-FR");
        }

        public static HtmlPage DetailDataToHtmlPage(BlogDemoorDetailData data)
        {
            //return new HtmlPage { Title = data.Title, Date = string.Format(_frCulture, "{0:dd MMMM}", data.Date), HtmlNodes = HtmlDocReader.ReadString(data.Content), WebImages = data.Images };
            return new HtmlPage { Title = data.Title, Date = data.Date, Html = data.Content, HtmlNodes = HtmlDocReader.ReadString(data.Content), WebImages = data.Images };
        }

        //public static void HtmlPagesToDocx(string file, string imageDirectory, IEnumerable<BlogDemoorDetailData> pages, string footerText = null)
        //{
        //    OXmlDoc.Create(file, HtmlPagesToOXmlElements(pages.Select(page => DetailDataToHtmlPage(page)), imageDirectory, footerText));
        //}

        public static void HtmlPagesToDocx(string file, string imageDirectory, IEnumerable<HtmlPage> pages, string footerText = null)
        {
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(BlogDemoor_v4.GetConfigElement().zXPathElement("HtmlToDocx"), imageDirectory);
            HtmlToOXmlElements_v2 htmlToOXmlElements = CreateHtmlToOXmlElements(BlogDemoor_v4.GetConfigElement(), imageDirectory);
            OXmlDoc.Create(file, HtmlPagesToOXmlElements(htmlToOXmlElements, pages, footerText));
        }

        public static IEnumerable<OXmlElement> HtmlPagesToOXmlElements(HtmlToOXmlElements_v2 htmlToOXmlElements, IEnumerable<HtmlPage> pages, string footerText = null)
        {
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(configElement.zXPathElement("HtmlToDocx"), imageDirectory);

            IEnumerable<OXmlElement> elements =
                GetDocDefaults().Union(
                GetStyles()).Union(
                GetDocSection()).Union(
                GetHeaderFooter(footerText));
            bool first = true;
            foreach (HtmlPage page in pages)
            {
                //Trace.WriteLine($"page \"{page.Title}\" date \"{page.Date}\"");
                if (!first)
                    //elements = elements.Union(new OXmlElement[] { new OXmlBreakElement { BreakType = BreakValues.Page } });
                    elements = elements.Union(GetPageBreak());
                first = false;
                elements = elements.Union(GetTitle(page.Date + " : " + page.Title));
                //elements = elements.Union(HtmlToOXmlElements_v2.ToOXmlXElements(page.HtmlNodes, imageDirectory, page.WebImages));
                elements = elements.Union(htmlToOXmlElements.ToOXmlXElements(page.HtmlNodes, page.WebImages));
            }
            return elements;
        }

        public static IEnumerable<OXmlElement> HtmlPageToOXmlElements(HtmlToOXmlElements_v2 htmlToOXmlElements, HtmlPage page, string footerText = null)
        {
            IEnumerable<OXmlElement> elements =
                GetDocDefaults().Union(
                GetStyles()).Union(
                GetDocSection()).Union(
                GetHeaderFooter(footerText));
            //Trace.WriteLine($"page \"{page.Title}\" date \"{page.Date}\"");
            elements = elements.Union(GetTitle(page.Date + " : " + page.Title));
            //elements = elements.Union(HtmlToOXmlElements_v2.ToOXmlXElements(page.HtmlNodes, imageDirectory, page.WebImages));
            elements = elements.Union(htmlToOXmlElements.ToOXmlXElements(page.HtmlNodes, page.WebImages));
            return elements;
        }

        public static void Test_HtmlToDocx(string file, string imageDirectory, string title = null, string footerText = null)
        {
            IEnumerable<OXmlElement> elements =
                GetDocDefaults().Union(
                GetStyles()).Union(
                GetDocSection()).Union(
                GetHeaderFooter(footerText));
            if (title != null)
                elements = elements.Union(GetTitle(title));
            //HtmlToOXmlElements_v2 htmlToOXmlElements = HtmlToOXmlElements_v2.Create(BlogDemoor_v4.GetConfigElement().zXPathElement("HtmlToDocx"), imageDirectory);
            HtmlToOXmlElements_v2 htmlToOXmlElements = CreateHtmlToOXmlElements(BlogDemoor_v4.GetConfigElement(), imageDirectory);
            //elements = elements.Union(HtmlToOXmlElements_v2.ToOXmlXElements(HtmlDocReader.ReadFile(file), imageDirectory));
            elements = elements.Union(htmlToOXmlElements.ToOXmlXElements(HtmlDocReader.ReadFile(file)));
            OXmlDoc.Create(file + ".docx", elements);
        }

        public static IEnumerable<OXmlElement> GetDocDefaults()
        {
            yield return new OXmlDocDefaultsRunPropertiesElement { RunFonts = new OXmlRunFonts { Ascii = "Arial" }, FontSize = "22" };
            yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
        }

        public static IEnumerable<OXmlElement> GetStyles()
        {
            yield return new OXmlStyleElement { Id = "HeaderFooter", StyleType = StyleValues.Paragraph, CustomStyle = true, DefaultStyle = false,
                StyleParagraphProperties = new OXmlStyleParagraphProperties { Tabs = new OXmlTabStop[] {
                new OXmlTabStop { Position = 4536, Alignment = TabStopValues.Center }, new OXmlTabStop { Position = 9072, Alignment = TabStopValues.Right } } } };
            yield return new OXmlStyleElement { Id = "TinyParagraph", StyleType = StyleValues.Paragraph, CustomStyle = true, DefaultStyle = false,
                StyleParagraphProperties = new OXmlStyleParagraphProperties { SpacingBetweenLines = new OXmlSpacingBetweenLines { Line = "48" } } };
            yield return new OXmlStyleElement { Id = "Title", StyleType = StyleValues.Paragraph, CustomStyle = true, DefaultStyle = false,
                StyleRunProperties = new OXmlStyleRunProperties { FontSize = "24", Bold = true } };
        }

        public static IEnumerable<OXmlElement> GetDocSection()
        {
            yield return new OXmlDocSectionElement
            {
                // 11907 1/20 point = 21 cm, 16839 1/20 point = 29.7 cm
                PageSize = new OXmlPageSize { Width = 11907, Height = 16839 },
                // 720 1/20 point = 0.6 cm, 1418 1/20 point = 1.27 cm, 284 1/20 point = 0.5 cm
                //PageMargin = new OXmlPageMargin { Top = 720, Bottom = 720, Left = 1418, Right = 1418, Header = 284, Footer = 284 }
                PageMargin = new OXmlPageMargin { Top = 720, Bottom = 720, Left = 720, Right = 720, Header = 284, Footer = 284 }
            };
        }

        public static IEnumerable<OXmlElement> GetHeaderFooter(string footerText)
        {
            // footer first page
            yield return new OXmlOpenFooterElement { FooterType = HeaderFooterValues.First };
            yield return new OXmlParagraphElement { Style = "HeaderFooter" };
            yield return new OXmlElement { Type = OXmlElementType.TabStop };
            yield return new OXmlTextElement { Text = footerText };
            yield return new OXmlElement { Type = OXmlElementType.CloseFooter };

            // footer default page
            yield return new OXmlOpenFooterElement { FooterType = HeaderFooterValues.Default };
            yield return new OXmlParagraphElement { Style = "HeaderFooter" };
            yield return new OXmlElement { Type = OXmlElementType.TabStop };
            if (footerText != null)
                yield return new OXmlTextElement { Text = footerText };
            yield return new OXmlElement { Type = OXmlElementType.TabStop };
            yield return new OXmlTextElement { Text = "page ", PreserveSpace = true };
            yield return new OXmlSimpleFieldElement { Instruction = "PAGE" };
            yield return new OXmlElement { Type = OXmlElementType.CloseFooter };
        }

        public static IEnumerable<OXmlElement> GetTitle(string title)
        {
            yield return new OXmlParagraphElement { Style = "Title" };
            yield return new OXmlTextElement { Text = title };
            yield return new OXmlParagraphElement();
        }

        public static IEnumerable<OXmlElement> GetPageBreak()
        {
            //yield return new OXmlBreakElement { BreakType = BreakValues.Page };
            yield return new OXmlParagraphElement();
            yield return new OXmlParagraphElement();
            yield return new OXmlParagraphElement();
        }

        //public static void Test_SaveOXml(string inputFile, string outputFile)
        //{
        //    OXmlElementReader.Read(inputFile).zSave(outputFile);
        //}
    }
}

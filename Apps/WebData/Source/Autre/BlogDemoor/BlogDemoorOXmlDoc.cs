using DocumentFormat.OpenXml.Wordprocessing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.OpenXml;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebData.BlogDemoor
{
    public class HtmlPage
    {
        public int Index;
        public string SourceUrl;
        public int Id;
        public string Title;
        //public string Date;
        public Date? Date;
        public string Html;
        public IEnumerable<HtmlDocNode> HtmlNodes;
        public WebImage[] WebImages;
    }

    public class OXmlPage
    {
        public HtmlPage HtmlPage;
        public IEnumerable<OXmlElement> OXmlElements;
    }

    [BsonIgnoreExtraElements]
    public class PagePatch
    {
        public int PageId;
        public bool PageBreak;
        public BsonArray Patches;
    }

    [Flags]
    public enum OXmlDocOptions
    {
        None                 =    0,
        ExportHtml           = 0x01,
        ExportOXmlPage       = 0x02,
        ExportOXmlDoc        = 0x04
    }

    public class BlogDemoorOXmlDoc
    {
        private BlogDemoor_v4 _dataManager = null;
        private HtmlToOXmlElements_v2 _htmlToOXmlElements = null;
        private CultureInfo _frCulture = null;
        private string _exportDirectory = null;
        //private string _patchesFile = null;
        private Dictionary<int, PagePatch> _patches = null;
        private OXmlDocOptions _options = OXmlDocOptions.None;

        public BlogDemoorOXmlDoc(BlogDemoor_v4 dataManager, string patchesFile = null)
        {
            _dataManager = dataManager;
            //_patchesFile = patchesFile;
            CreateHtmlToOXmlElements();
            SetFrCulture();
            ReadPatchesFile(patchesFile);
        }

        private void ReadPatchesFile(string patchesFile)
        {
            if (patchesFile == null)
                return;
            _patches = new Dictionary<int, PagePatch>();
            foreach (PagePatch pagePatch in zmongo.BsonRead<PagePatch>(patchesFile))
                _patches.Add(pagePatch.PageId, pagePatch);
        }

        public void CreateDocx(string file, string footerText, int limit = 0, int skip = 0, OXmlDocOptions options = OXmlDocOptions.None)
        {
            _CreateDocx(file, GetDetailPages(limit, skip), footerText, skip, options);
        }

        public void CreateDocx(string file, string footerText, IEnumerable<int> detailIds, OXmlDocOptions options = OXmlDocOptions.None)
        {
            _CreateDocx(file, GetDetailPages(detailIds), footerText, options: options);
        }

        private void _CreateDocx(string file, IEnumerable<BlogDemoorDetailData> details, string footerText, int skip = 0, OXmlDocOptions options = OXmlDocOptions.None)
        {
            //details = details.zAction(detail => Trace.WriteLine($"title \"{detail.Title}\" url \"{detail.SourceUrl}\""));
            _options = options;

            IEnumerable<HtmlPage> htmlPages = ToHtmlPages(details, skip);

            htmlPages = htmlPages.zAction(htmlPage => Trace.WriteLine($"page {htmlPage.Index} date {htmlPage.Date:dd-MM} title \"{htmlPage.Title}\" url \"{htmlPage.SourceUrl}\""));

            _exportDirectory = zPath.Combine(zPath.GetDirectoryName(file), "files");
            if ((options & OXmlDocOptions.ExportHtml) == OXmlDocOptions.ExportHtml)
            {
                htmlPages = htmlPages.zAction(htmlPage => ExportHtml(htmlPage));
            }

            //IEnumerable<OXmlElement> oxmlElements = ToOXmlElements(htmlPages, footerText);
            //IEnumerable<IEnumerable<OXmlElement>> pages = htmlPages.Select(page => PageToOXmlElements(page, (options & OXmlDocOptions.ExportOXmlPage) == OXmlDocOptions.ExportOXmlPage));
            //IEnumerable<OXmlPage> pages = htmlPages.Select(htmlPage => new OXmlPage { HtmlPage = htmlPage, OXmlElements = PageToOXmlElements(htmlPage, (options & OXmlDocOptions.ExportOXmlPage) == OXmlDocOptions.ExportOXmlPage) });
            //IEnumerable<OXmlElement> oxmlElements = ToOXmlElements(pages, footerText);
            IEnumerable<OXmlElement> oxmlElements = ToOXmlElements(htmlPages, footerText);

            if ((options & OXmlDocOptions.ExportOXmlDoc) == OXmlDocOptions.ExportOXmlDoc)
                ExportOXml(zPath.GetFileNameWithoutExtension(file), oxmlElements);

            OXmlDoc.Create(file, oxmlElements);
        }

        public void TracePages(int limit = 0, int skip = 0)
        {
            int index = 1;
            foreach (BlogDemoorDetailData detail in GetDetailPages(limit, skip))
            {
                Trace.WriteLine($"page {index++:000} - {string.Format(_frCulture, "{0,-17:dd MMMM yyyy}", detail.Date)} - {detail.Title} - {detail.SourceUrl}");
            }
        }

        private IEnumerable<BlogDemoorDetailData> GetDetailPages(int limit = 0, int skip = 0)
        {
            int startPage = 1;
            int maxPage = 0;
            bool reloadHeaderPage = false;
            bool reloadDetail = false;
            bool loadImageFromWeb = false;
            bool loadImageToData = false;
            bool refreshImage = false;
            bool refreshDocumentStore = false;
            IEnumerable<BlogDemoorDetailData> pages = _dataManager.HeaderDetailManager.LoadDetails(startPage, maxPage, reloadHeaderPage, reloadDetail, loadImageFromWeb, loadImageToData, refreshImage, refreshDocumentStore);
            pages = pages.Reverse();
            if (skip != 0)
                pages = pages.Skip(skip);
            if (limit != 0)
                pages = pages.Take(limit);
            return pages;
        }

        private IEnumerable<BlogDemoorDetailData> GetDetailPages(IEnumerable<int> detailIds)
        {
            foreach (int detailId in detailIds)
            {
                yield return _dataManager.DetailDataManager.LoadFromId(detailId);
            }
        }

        private void SetFrCulture()
        {
            _frCulture = CultureInfo.GetCultureInfo("fr-FR");
        }

        private IEnumerable<HtmlPage> ToHtmlPages(IEnumerable<BlogDemoorDetailData> details, int skip = 0)
        {
            int index = 1 + skip;
            foreach (BlogDemoorDetailData detail in details)
            {
                HtmlPage htmlPage = ToHtmlPage(detail);
                htmlPage.Index = index++;
                yield return htmlPage;
            }
        }

        private HtmlPage ToHtmlPage(BlogDemoorDetailData data)
        {
            // Date = string.Format(_frCulture, "{0:dd MMMM}", data.Date)
            return new HtmlPage { SourceUrl = data.SourceUrl, Id = data.Id, Title = data.Title.Trim(), Date = data.Date, Html = data.Content, HtmlNodes = HtmlDocReader.ReadString(data.Content), WebImages = data.Images };
        }

        // bool export = false
        private IEnumerable<OXmlElement> PageToOXmlElements(HtmlPage htmlPage, Dictionary<string, NamedValues<ZValue>> imagePatches = null)
        {
            //elements = elements.Union(GetTitle(page.Date + " : " + page.Title));
            IEnumerable<OXmlElement> elements = GetTitle(string.Format(_frCulture, "{0:dd MMMM}", htmlPage.Date) + " : " + htmlPage.Title);
            elements = elements.Union(_htmlToOXmlElements.ToOXmlXElements(htmlPage.HtmlNodes, htmlPage.WebImages, htmlPage.SourceUrl, imagePatches));
            //if (export)
            if ((_options & OXmlDocOptions.ExportOXmlPage) == OXmlDocOptions.ExportOXmlPage)
                ExportOXml(htmlPage, elements);
            return elements;
        }

        //private IEnumerable<OXmlElement> ToOXmlElements(IEnumerable<HtmlPage> pages, string footerText = null)
        //private IEnumerable<OXmlElement> ToOXmlElements(IEnumerable<IEnumerable<OXmlElement>> pages, string footerText = null)
        //private IEnumerable<OXmlElement> ToOXmlElements(IEnumerable<OXmlPage> pages, string footerText = null)
        private IEnumerable<OXmlElement> ToOXmlElements(IEnumerable<HtmlPage> htmlPages, string footerText = null)
        {
            IEnumerable<OXmlElement> elements =
                GetDocDefaults().Union(
                GetStyles()).Union(
                GetDocSection()).Union(
                GetHeaderFooter(footerText));
            bool first = true;
            //foreach (IEnumerable<OXmlElement> page in pages)
            //foreach (OXmlPage page in pages)
            foreach (HtmlPage htmlPage in htmlPages)
            {
                PagePatch patch = null;
                if (_patches != null && _patches.ContainsKey(htmlPage.Id))
                    patch = _patches[htmlPage.Id];

                bool pageBreak = false;
                if (patch != null && patch.PageBreak)
                    pageBreak = true;

                if (!first)
                    elements = elements.Union(GetPageBreak(pageBreak));
                first = false;
                //elements = elements.Union(GetTitle(string.Format(_frCulture, "{0:dd MMMM}", page.Date) + " : " + page.Title));
                //elements = elements.Union(_htmlToOXmlElements.ToOXmlXElements(page.HtmlNodes, page.WebImages));
                //elements = elements.Union(page.OXmlElements);
                //PageToOXmlElements(htmlPage, (options & OXmlDocOptions.ExportOXmlPage) == OXmlDocOptions.ExportOXmlPage)
                elements = elements.Union(PageToOXmlElements(htmlPage, CreateImagePatches(patch)));
            }
            return elements;
        }

        private static Dictionary<string, NamedValues<ZValue>> CreateImagePatches(PagePatch pagePatch)
        {
            Dictionary<string, NamedValues<ZValue>> imagePatches = null;
            if (pagePatch != null && pagePatch.Patches != null)
            {
                imagePatches = new Dictionary<string, NamedValues<ZValue>>();
                foreach (BsonValue patch in pagePatch.Patches)
                {
                    BsonDocument patchDoc = patch.AsBsonDocument;
                    if (patchDoc.zGet("Type").zAsString().ToLower() == "image")
                        imagePatches.Add(patchDoc.zGet("Url").zAsString(), ParseNamedValues.ParseValues(patchDoc.zGet("Values").zAsString(), useLowercaseKey: true));
                }
            }
            return imagePatches;
        }

        //private IEnumerable<OXmlElement> ToOXmlElements(IEnumerable<HtmlPage> pages, string footerText = null)
        //{
        //    IEnumerable<OXmlElement> elements =
        //        GetDocDefaults().Union(
        //        GetStyles()).Union(
        //        GetDocSection()).Union(
        //        GetHeaderFooter(footerText));
        //    bool first = true;
        //    foreach (HtmlPage page in pages)
        //    {
        //        //Trace.WriteLine($"page \"{page.Title}\" date \"{page.Date}\"");
        //        if (!first)
        //            //elements = elements.Union(new OXmlElement[] { new OXmlBreakElement { BreakType = BreakValues.Page } });
        //            elements = elements.Union(GetPageBreak());
        //        first = false;
        //        //elements = elements.Union(GetTitle(page.Date + " : " + page.Title));
        //        elements = elements.Union(GetTitle(string.Format(_frCulture, "{0:dd MMMM}", page.Date) + " : " + page.Title));
        //        elements = elements.Union(_htmlToOXmlElements.ToOXmlXElements(page.HtmlNodes, page.WebImages));
        //    }
        //    return elements;
        //}

        private static IEnumerable<OXmlElement> GetDocDefaults()
        {
            yield return new OXmlDocDefaultsRunPropertiesElement { RunFonts = new OXmlRunFonts { Ascii = "Arial", ComplexScript = "Arial", EastAsia = "Arial", HighAnsi = "Arial" }, FontSize = "22" };
            yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
        }

        private static IEnumerable<OXmlElement> GetStyles()
        {
            yield return new OXmlStyleElement
            {
                Id = "HeaderFooter",
                StyleType = StyleValues.Paragraph,
                CustomStyle = true,
                DefaultStyle = false,
                StyleParagraphProperties = new OXmlStyleParagraphProperties
                {
                    Tabs = new OXmlTabStop[] {
                new OXmlTabStop { Position = 4536, Alignment = TabStopValues.Center }, new OXmlTabStop { Position = 9072, Alignment = TabStopValues.Right } }
                }
            };
            yield return new OXmlStyleElement
            {
                Id = "TinyParagraph",
                StyleType = StyleValues.Paragraph,
                CustomStyle = true,
                DefaultStyle = false,
                StyleParagraphProperties = new OXmlStyleParagraphProperties { SpacingBetweenLines = new OXmlSpacingBetweenLines { Line = "48" } }
            };
            yield return new OXmlStyleElement
            {
                Id = "Title",
                StyleType = StyleValues.Paragraph,
                CustomStyle = true,
                DefaultStyle = false,
                StyleRunProperties = new OXmlStyleRunProperties { FontSize = "24", Bold = true }
            };
        }

        private static IEnumerable<OXmlElement> GetDocSection()
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

        private static IEnumerable<OXmlElement> GetHeaderFooter(string footerText)
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

        private static IEnumerable<OXmlElement> GetTitle(string title)
        {
            yield return new OXmlParagraphElement { Style = "Title" };
            yield return new OXmlTextElement { Text = title };
            yield return new OXmlParagraphElement();
        }

        private static IEnumerable<OXmlElement> GetPageBreak(bool pageBreak)
        {
            if (pageBreak)
                yield return new OXmlBreakElement { BreakType = BreakValues.Page };
            else
            {
                yield return new OXmlParagraphElement();
                yield return new OXmlParagraphElement();
                yield return new OXmlParagraphElement();
            }
        }

        private void ExportHtml(HtmlPage htmlPage)
        {
            string exportFile = zPath.Combine(_exportDirectory, (htmlPage.Index / 10 * 10).ToString().PadLeft(3, '0'), GetFilename(htmlPage.Index, htmlPage.Title, htmlPage.Date) + ".html");
            //Trace.WriteLine($"export html to \"{exportFile}\"");
            zfile.WriteFile(exportFile, htmlPage.Html);
        }

        private void ExportOXml(HtmlPage htmlPage, IEnumerable<OXmlElement> elements)
        {
            string exportFile = zPath.Combine(_exportDirectory, (htmlPage.Index / 10 * 10).ToString().PadLeft(3, '0'), GetFilename(htmlPage.Index, htmlPage.Title, htmlPage.Date) + ".oxml.json");
            //Trace.WriteLine($"export oxml to \"{exportFile}\"");
            elements.zSave(exportFile, jsonIndent: true);
        }

        private void ExportOXml(string file, IEnumerable<OXmlElement> elements)
        {
            string exportFile = zPath.Combine(_exportDirectory, file + ".oxml.json");
            //Trace.WriteLine($"export oxml to \"{exportFile}\"");
            elements.zSave(exportFile, jsonIndent: true);
        }

        private Regex _multipleUnderscore = new Regex("_{2,}");
        private string GetFilename(int page, string title, Date? date)
        {
            title = zfile.ReplaceBadFilenameChars(title, "_");
            title = title.Replace(" ", "_");
            title = _multipleUnderscore.Replace(title, "_");
            return $"page_{page.ToString().PadLeft(3, '0')}_{date:dd-MM}_{title}";
        }

        private void CreateHtmlToOXmlElements()
        {
            _htmlToOXmlElements = CreateHtmlToOXmlElements(_dataManager.ConfigElement, _dataManager.DetailDataManager.WebLoadImageManager.WebImageCacheManager.UrlCache.CacheDirectory);
        }

        private static HtmlToOXmlElements_v2 CreateHtmlToOXmlElements(XElement xe, string imageDirectory)
        {
            HtmlToOXmlElements_v2 htmlToOXmlElements = new HtmlToOXmlElements_v2();
            htmlToOXmlElements.ImageDirectory = imageDirectory;
            htmlToOXmlElements.TextRemoveBlank = true;
            htmlToOXmlElements.TextRemoveLine = true;
            //htmlToOXmlElements.ForcedMaxImageWidth = 300;
            htmlToOXmlElements.MaxImageHorizontalSize = 700;
            htmlToOXmlElements.ImageHorizontalLeftMarge = 2;
            htmlToOXmlElements.ImageHorizontalMarge = 3;
            htmlToOXmlElements.ImageHorizontalBorder = 71755; // 71755 = 0.2 cm
            htmlToOXmlElements.ImageSeparationParagraphStyle = "TinyParagraph";
            htmlToOXmlElements.UseLinkImage = false;  // true : take image link from tag a, false : take image link from tag img
            //htmlToOXmlElements.ImagePatchesFile = imagePatchesFile;
            htmlToOXmlElements.Init(xe.zXPathElement("HtmlToDocx"));
            return htmlToOXmlElements;
        }

        //public static void CreateDocx(BlogDemoor_v4 dataManager, string file, string footerText, int limit = 0, int skip = 0, OXmlDocOptions options = OXmlDocOptions.None)
        //{
        //    new BlogDemoorOXmlDoc(dataManager)._CreateDocx(file, footerText, limit, skip, options);
        //}

        //public static void CreateDocx(BlogDemoor_v4 dataManager, string file, string footerText, IEnumerable<int> detailIds, OXmlDocOptions options = OXmlDocOptions.None)
        //{
        //    new BlogDemoorOXmlDoc(dataManager)._CreateDocx(file, footerText, detailIds, options);
        //}
    }

    //public static class HtmlToOXmlDocExtension
    //{
    //    //public static IEnumerable<BlogDemoorDetailData> zTrace(this IEnumerable<BlogDemoorDetailData> details)
    //    //{
    //    //    return details.zAction(detail => Trace.WriteLine($"title \"{detail.Title}\" url \"{detail.SourceUrl}\""));
    //    //}

    //    //public static IEnumerable<HtmlPage> zToHtmlPages(this IEnumerable<BlogDemoorDetailData> details)
    //    //{
    //    //    int index = 1;
    //    //    //return details.Select(detail => BlogDemoorOXmlDoc.ToHtmlPage(detail));
    //    //    foreach (BlogDemoorDetailData detail in details)
    //    //    {
    //    //        HtmlPage htmlPage = BlogDemoorOXmlDoc.ToHtmlPage(detail);
    //    //        htmlPage.Index = index++;
    //    //        yield return htmlPage;
    //    //    }
    //    //}
    //}
}

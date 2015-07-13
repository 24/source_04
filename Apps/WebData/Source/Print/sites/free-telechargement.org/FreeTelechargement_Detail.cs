using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using Print;

namespace Download.Print.FreeTelechargement
{
    public class FreeTelechargement_PostDetail
    {
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate;

        public string title;
        public PrintType printType;
        public string originalTitle;
        public string postAuthor;
        public DateTime? creationDate;
        public string category;
        public string[] description;
        public string language;
        public string size;
        public int? nbPages;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        public List<WebImage> images;
        public List<string> downloadLinks = new List<string>();
    }

    public class FreeTelechargement_LoadPostDetailFromWebManager : LoadDataFromWebManager_v3<FreeTelechargement_PostDetail>
    {
        private static bool __trace = false;

        public FreeTelechargement_LoadPostDetailFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        protected override FreeTelechargement_PostDetail GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            throw new PBException("attention mismatch between free-telechargement.org and golden-ddl.net");
#pragma warning disable 162
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            FreeTelechargement_PostDetail data = new FreeTelechargement_PostDetail();
            data.sourceUrl = loadDataFromWeb.request.Url;
            data.loadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.id = GetPostDetailKey(data.sourceUrl);

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");

            //data.category = xePost.DescendantTextList(".//div[@class='hdiin']//a").Select(FreeTelechargement.TrimFunc1).zToStringValues("/");
            data.category = xePost.XPathElements(".//div[@class='hdiin']//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            string category = data.category.ToLowerInvariant();
            data.printType = GetPrintType(category);

            //data.title = xePost.XPathValue(".//div[@class='bheading']//text()", FreeTelechargement.TrimFunc1);
            data.title = xePost.XPathValue(".//div[@class='bheading']//text()").Trim(DownloadPrint.TrimChars);
            PrintTitleInfos titleInfos = FreeTelechargement.PrintTextValuesManager.ExtractTitleInfos(data.title);
            if (titleInfos.foundInfo)
            {
                data.originalTitle = data.title;
                data.title = titleInfos.title;
                data.infos.SetValues(titleInfos.infos);
            }

            string date = xePost.XPathValue(".//div[@class='datenews']//text()");
            data.creationDate = zdate.ParseDateTimeLikeToday(date, loadDataFromWeb.loadFromWebDate, "d-M-yyyy, HH:mm");
            if (data.creationDate == null)
                pb.Trace.WriteLine("unknow date time \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("creationDate {0} - \"{1}\"", data.creationDate, date);

            data.postAuthor = xePost.XPathValue(".//div[@class='argr']//a//text()");

            XXElement xe = xePost.XPathElement(".//div[@class='maincont']");
            //data.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToList();
            data.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToList();

            // force load image to get image width and height
            if (loadDataFromWeb.request.LoadImage)
                FreeTelechargement.LoadImages(data.images);

            // get infos, description, language, size, nbPages
            //PrintTextValues_old textValues = FreeTelechargement.PrintTextValuesManager.GetTextValues_old(xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a"), data.title);
            PrintTextValues_v1 textValues = FreeTelechargement.PrintTextValuesManager.GetTextValues_v1(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), data.title);
            data.description = textValues.description;
            data.language = textValues.language;
            data.size = textValues.size;
            data.nbPages = textValues.nbPages;
            data.infos.SetValues(textValues.infos);

            foreach (XXElement xe2 in xe.XPathElements("div/div"))
            {
                // http://prezup.eu http://pixhst.com/avaxhome/27/36/002e3627.jpeg http://www.zupmage.eu/i/R1UgqdXn4F.jpg
                // http://i.imgur.com/Gu7hagN.jpg http://img11.hostingpics.net/pics/591623liens.png http://www.hapshack.com/images/jUfTZ.gif
                // http://pixhst.com/pictures/3029467
                data.downloadLinks.AddRange(xe2.XPathValues(".//a/@href").Where(url => !url.StartsWith("http://prezup.eu") && !url.StartsWith("http://pixhst.com")
                    && !url.EndsWith(".jpg") && !url.EndsWith("jpeg") && !url.EndsWith("png") && !url.EndsWith("gif")));
            }

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
#pragma warning restore 162
        }

        private static PrintType GetPrintType(string category)
        {
            // category : "Ebooks", "Ebooks/Bandes Dessinée", "Ebooks/Journaux", "Ebooks/Livres", "Ebooks/Magazine"
            switch (category.ToLower())
            {
                case "ebooks/journaux":
                case "ebooks/magazines":
                    return PrintType.Print;
                case "ebooks/livres":
                    return PrintType.Book;
                case "ebooks/bandes dessinée":
                    return PrintType.Comics;
                case "ebooks":
                    return PrintType.UnknowEBook;
                default:
                    return PrintType.Unknow;
            }
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static int GetPostDetailKey(string url)
        {
            // http://www.free-telechargement.org/magazines/43247-pc-update-no10-mars-avril-2004-pdf.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }
    }
}

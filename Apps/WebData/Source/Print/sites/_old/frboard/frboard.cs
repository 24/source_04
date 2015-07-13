using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Data.Xml.old;   // for zReadAttributeString()
using pb.IO;
using pb.Linq;
using pb.Text;
using pb.Web;
using pb.Web.old;
using pb.old;  // zAdd(this CookieContainer container, string url, params string[] cookies)  (Cookie.cs)

//namespace Print.download
namespace Download.Print
{
    public class FrboardPostHeader
    {
        public string url = null;
        public string title = null;
        public string author = null;
        public DateTime? creationDate;
        private static Regex _rgAuthor = new Regex(@"Crée\s+par\s+([^\s]+)\s+,\s*(Aujourd'hui|Hier|[0-9]{2}/[0-9]{2}/[0-9]{4})\s+([0-9]{2}h[0-9]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void SetAuthorAndDate(string authorAndDate)
        {
            // Crée par elmareg , Aujourd'hui 21h49
            // Crée par angelbabe630 , Hier 22h43
            // Crée par bar2dos , 11/08/2013 22h13
            Match match = _rgAuthor.Match(authorAndDate);
            if (match.Success)
            {
                this.author = match.Groups[1].Value;
                creationDate = GetDateTime(match.Groups[2].Value, match.Groups[3].Value);
            }
        }

        public static DateTime? GetDateTime(string date, string time)
        {
            if (string.Equals(date, "Aujourd'hui", StringComparison.CurrentCultureIgnoreCase))
                date = DateTime.Today.ToString("dd/MM/yyyy");
            else if (string.Equals(date, "Hier", StringComparison.CurrentCultureIgnoreCase))
                date = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            //return DateTime.ParseExact(date + " " + time, @"dd/MM/yyyy HH\hmm", CultureInfo.CurrentCulture).AddHours(1);
            DateTime dt;
            if (DateTime.TryParseExact(date + " " + time, @"dd/MM/yyyy HH\hmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                return dt; // AddHours(1)
            Trace.CurrentTrace.WriteLine("unknow date time \"{0}\"", date + " " + time);
            return null;
        }
    }

    public class FrboardPost
    {
        public string url = null;
        public DateTime? loadFromWebDate;
        public string title = null;
        public string author = null;
        public DateTime? creationDate = null;
        public int imageNb = 0;
        //NamedValues1
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        public bool multiPrint = false;

        public void SaveXml(XmlWriter xw)
        {
            xw.zWriteElementWithAttributes("url", "value", url);
            xw.zWriteElementWithAttributes("loadFromWebDate", "value", loadFromWebDate != null ? ((DateTime)loadFromWebDate).ToString("yyyy-MM-dd HH:mm:ss") : "");
            xw.zWriteElementWithAttributes("title", "value", title);
            xw.zWriteElementWithAttributes("creationDate", "value", creationDate != null ? ((DateTime)creationDate).ToString("yyyy-MM-dd HH:mm:ss") : "");
            xw.zWriteElementWithAttributes("author", "value", author);
            xw.zWriteElementWithAttributes("imageNb", "value", imageNb.ToString());
        }
    }

    public class FrboardPrint
    {
        public string url = null;
        public DateTime? loadFromWebDate = null;
        public string postTitle = null;
        public bool multiPrint;
        public string title = null;
        public string author = null;
        public DateTime? creationDate = null;
        //public UrlImage image = null;
        public string description = null;
        public bool info = false;
        public int nbPages;
        public string language = null;
        public string size = null;
        public string otherInfo = null;
        //NamedValues1
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
        public List<string> downloadLinks = new List<string>();
        //private static Trace _tr = Trace.CurrentTrace;

        //public Date updated;

        private static Regex _rgAuthor = new Regex(@"Crée\s+par\s+([^\s]+)\s+,\s*(Aujourd'hui|Hier|[0-9]{2}/[0-9]{2}/[0-9]{4})\s+([0-9]{2}h[0-9]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _rgPages = new Regex(@"([0-9]+)\s+pages", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _rgLanguage = new Regex(@"(French|English)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _rgSize = new Regex(@"[0-9]+\s+mb", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void SetAuthor(string author)
        {
            // Crée par elmareg , Aujourd'hui 21h49
            // Crée par angelbabe630 , Hier 22h43
            // Crée par bar2dos , 11/08/2013 22h13
            Match match = _rgAuthor.Match(author);
            if (match.Success)
            {
                this.author = match.Groups[1].Value;
                //string date = match.Groups[2].Value;
                //if (string.Equals(date, "Aujourd'hui", StringComparison.CurrentCultureIgnoreCase))
                //    date = DateTime.Today.ToString("dd/MM/yyyy");
                //else if (string.Equals(date, "Hier", StringComparison.CurrentCultureIgnoreCase))
                //    date = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
                //string time = match.Groups[3].Value;
                //creationDate = DateTime.ParseExact(date + " " + time, @"dd/MM/yyyy HH\hmm", CultureInfo.CurrentCulture);
                //creationDate = creationDate.AddHours(1);
                creationDate = GetDateTime(match.Groups[2].Value, match.Groups[3].Value);
            }
        }

        public static DateTime? GetDateTime(string date, string time)
        {
            if (string.Equals(date, "Aujourd'hui", StringComparison.CurrentCultureIgnoreCase))
                date = DateTime.Today.ToString("dd/MM/yyyy");
            else if (string.Equals(date, "Hier", StringComparison.CurrentCultureIgnoreCase))
                date = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            //return DateTime.ParseExact(date + " " + time, @"dd/MM/yyyy HH\hmm", CultureInfo.CurrentCulture).AddHours(1);
            DateTime dt;
            if (DateTime.TryParseExact(date + " " + time, @"dd/MM/yyyy HH\hmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                return dt; // AddHours(1)
            WriteLine("unknow date time \"{0}\"", date + " " + time);
            return null;
        }

        public void SetInfo(string text, char sep = '|')
        {
            // French | PDF | 107 MB - French | PDF |  22 Pages | 7 MB
            string[] infos = zsplit.Split(text, sep, true);
            foreach (string info in infos)
            {
                this.info = true;
                if (string.Equals(info, "pdf", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                Match match = _rgPages.Match(info);
                if (match.Success)
                    nbPages = int.Parse(match.Groups[1].Value);
                else
                {
                    match = _rgLanguage.Match(info);
                    if (match.Success)
                        language = match.Groups[1].Value;
                    else
                    {
                        match = _rgSize.Match(info);
                        if (match.Success)
                            size = match.Value;
                        else
                        {
                            if (otherInfo == null)
                                otherInfo = info;
                            else
                                otherInfo += " - " + info;
                        }
                    }
                }
            }
        }

        public void AddDescription(string text)
        {
            if (description == null)
                description = text;
            else
                description += "\r\n" + text;
        }

        public void SaveXml(XmlWriter xw, string xmlDir = null, string imageDir = null)
        {
            xw.zWriteElementWithAttributes("title", "value", title);
            xw.zWriteElementWithAttributes("description", "value", description);
            xw.zWriteElementWithAttributes("info", "value", info.ToString());
            xw.zWriteElementWithAttributes("nbPages", "value", nbPages.ToString());
            xw.zWriteElementWithAttributes("language", "value", language);
            xw.zWriteElementWithAttributes("size", "value", size);
            xw.zWriteElementWithAttributes("otherInfo", "value", otherInfo);

            //foreach (KeyValuePair<string, object> value in infos)
            foreach (KeyValuePair<string, ZValue> value in infos)
            {
                string s = "";
                if (value.Value != null)
                    s = value.Value.ToString();
                xw.zWriteElementWithAttributes("value", "name", value.Key, "value", s);
            }

            foreach (pb.old.ImageHtml image in images)
            {
                string file = null;
                if (xmlDir != null && imageDir != null)
                {
                    file = Path.Combine(imageDir, zurl.UrlToFileName(image.Source, UrlFileNameType.FileName));
                    //Frboard.LoadToFile(image.Source, Path.Combine(xmlDir, file));
                    pb.old.Http_v2.LoadToFile(image.Source, Path.Combine(xmlDir, file));
                }
                xw.zWriteElementWithAttributes("image", "file", file, "source", image.Source, "title", image.Title, "class", image.Class, "alt", image.Alt);
            }

            foreach (string downloadLink in downloadLinks)
            {
                xw.zWriteElementWithAttributes("downloadLink", "value", downloadLink);
            }
        }

        public void LoadFromXml(XmlReader _xmlReader, string xmlDirectory = null)
        {
            while (!_xmlReader.EOF)
            {
                _xmlReader.Read();
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_xmlReader.IsEmptyElement)
                    {
                        string s;
                        switch (_xmlReader.Name)
                        {
                            case "title":
                                title = _xmlReader.zReadAttributeString();
                                break;
                            case "description":
                                description = _xmlReader.zReadAttributeString();
                                break;
                            case "info":
                                s = _xmlReader.zReadAttributeString();
                                if (!bool.TryParse(s, out info))
                                    WriteLine("error wrong info bool value \"{0}\" in xml  (FrboardPrint)", s);
                                break;
                            case "nbPages":
                                s = _xmlReader.zReadAttributeString();
                                if (!int.TryParse(s, out nbPages))
                                    WriteLine("error wrong nbPages int value \"{0}\" in xml  (FrboardPrint)", s);
                                break;
                            case "language":
                                language = _xmlReader.zReadAttributeString();
                                break;
                            case "size":
                                size = _xmlReader.zReadAttributeString();
                                break;
                            case "otherInfo":
                                otherInfo = _xmlReader.zReadAttributeString();
                                break;
                            case "value":
                                string name = _xmlReader.zReadAttributeString("name");
                                s = _xmlReader.zReadAttributeString("value");
                                if (name != "")
                                    //infos.Add(name, s);
                                    infos.Add(name, new ZString(s));
                                else
                                    WriteLine("error wrong infos element value has no name value \"{0}\" in xml  (FrboardPrint)", s);
                                break;
                            case "image":
                                string file = _xmlReader.zReadAttributeString("file");
                                string source = _xmlReader.zReadAttributeString("source");
                                if (source == "")
                                    WriteLine("error wrong image source has no value in xml  (FrboardPrint)");
                                pb.old.ImageHtml image = new pb.old.ImageHtml(source, _xmlReader.zReadAttributeString("alt"), _xmlReader.zReadAttributeString("title"), _xmlReader.zReadAttributeString("class"));
                                if (xmlDirectory != null)
                                {
                                    string path = Path.Combine(xmlDirectory, file);
                                    if (File.Exists(path))
                                        //image.Image = Frboard.LoadImageFromFile(path);
                                        image.Image = pb.old.Http_v2.LoadImageFromFile(path);
                                    else
                                        WriteLine("error wrong image file does'nt exist \"{0}\" in xml  (FrboardPrint)", path);
                                    images.Add(image);
                                }
                                break;
                            case "downloadLink":
                                downloadLinks.Add(_xmlReader.zReadAttributeString());
                                break;
                            default:
                                WriteLine("error unknow element \"{0}\" in xml  (FrboardPrint)", _xmlReader.Name);
                                break;
                        }
                    }
                }
                else if (_xmlReader.NodeType == XmlNodeType.EndElement)
                    break;
            }
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class LoadFrboardPostHeaderFromWeb : IEnumerable<FrboardPostHeader>, IEnumerator<FrboardPostHeader>
    {
        private int _maxPage = 1;
        private int _nbPage = 1;
        private bool _loadUrlResult = false;
        private string _urlNextPage = null;
        private IEnumerator<pb.old.XmlSelect> _xmlEnum = null;
        private FrboardPostHeader _post = null;

        public LoadFrboardPostHeaderFromWeb(string url = null, int maxPage = 1)
        {
            _maxPage = maxPage;
            if (url == null)
                url = Frboard.UrlPrint;
            LoadUrl(url);
        }

        public void Dispose()
        {
        }

        private void LoadUrl(string url)
        {
            //_loadUrlResult = Frboard.LoadUrl(url);
            //_loadUrlResult = Http2.LoadUrl(url);
            _loadUrlResult = pb.old.Http_v2.LoadUrl(url, Frboard.HttpRequestParameters);
            if (_loadUrlResult)
            {
                //_urlNextPage = Frboard.HtmlReader.SelectValue("//span[@class='prev_next']//a[@rel='next']/@href");
                _urlNextPage = pb.old.Http_v2.HtmlReader.SelectValue("//span[@class='prev_next']//a[@rel='next']/@href");
                //XmlSelect select = Frboard.HtmlReader.Select("//li[starts-with(@class, 'threadbit')]",
                pb.old.XmlSelect select = pb.old.Http_v2.HtmlReader.Select("//li[starts-with(@class, 'threadbit')]",
                    ".//h3[@class='threadtitle']//a/@href:.:n(href)",
                    ".//h3[@class='threadtitle']//a/text():.:n(label1)",
                    ".//div[@class='author']//text():.:Concat( ):n(author)");
                _xmlEnum = select.GetEnumerator();
            }
        }

        public IEnumerator<FrboardPostHeader> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public FrboardPostHeader Current
        {
            get { return _post; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _post; }
        }

        public bool MoveNext()
        {
            _post = null;
            if (!_loadUrlResult)
                return false;
            while (true)
            {
                pb.old.XmlSelect xmlSelect;
                while (_xmlEnum.MoveNext())
                {
                    xmlSelect = _xmlEnum.Current;
                    if ((string)xmlSelect["label1"] == "Règlement de la section Ebooks [à Lire avant de poster]")
                        continue;
                    _post = new FrboardPostHeader();
                    _post.url = (string)xmlSelect["href"];
                    _post.title = (string)xmlSelect["label1"];
                    _post.SetAuthorAndDate((string)xmlSelect["author"]);
                    return true;
                }
                if (_urlNextPage == null || _nbPage == _maxPage)
                    return false;
                LoadUrl(_urlNextPage);
                //_loadUrlResult = Frboard.LoadUrl(_urlNextPage);
                if (!_loadUrlResult)
                    return false;
                //_urlNextPage = Frboard.HtmlReader.SelectValue("//span[@class='prev_next']//a[@rel='next']/@href");
                //xmlSelect = Frboard.HtmlReader.Select("//li[starts-with(@class, 'threadbit')]",
                //    ".//h3[@class='threadtitle']//a/@href:.:n(href)",
                //    ".//h3[@class='threadtitle']//a/text():.:n(label1)",
                //    ".//div[@class='author']//text():.:Concat( ):n(author)");
                //_xmlEnum = xmlSelect.GetEnumerator();
                _nbPage++;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class LoadFrboardPostsFromFiles : IEnumerable<FrboardPost>, IEnumerator<FrboardPost>
    {
        private int _maxPost = 0;
        private bool _loadImage = false;
        private string[] _directories = null;
        private int _directoryIndex = -1;
        private string[] _files = null;
        private int _fileIndex = -1;
        private int _postIndex = 0;
        private FrboardPost _currentPost = null;

        public LoadFrboardPostsFromFiles(string dir, int maxPost = 50, bool loadImage = false)
        {
            _directories = Directory.GetDirectories(dir);
            _directoryIndex = -1;
            _maxPost = maxPost;
            _loadImage = loadImage;
        }

        public void Dispose()
        {
        }

        private bool GetNextPost()
        {
            while (true)
            {
                if (_fileIndex == -1 && _files != null)
                {
                    if (++_fileIndex < _files.Length)
                    {
                        if (_postIndex++ >= _maxPost)
                            return false;
                        string file = _files[_fileIndex];
                        LoadFrboardPostFromXml loadPost = new LoadFrboardPostFromXml(file, _loadImage);
                        _currentPost = loadPost.Post;
                        return true;
                    }
                }
                if (_directoryIndex == -1)
                {
                    if (++_directoryIndex >= _directories.Length)
                        return false;
                    string dir = _directories[_directoryIndex];
                    _files = Directory.GetFiles(dir, "*.xml");
                    _fileIndex = -1;
                }
            }
        }

        public IEnumerator<FrboardPost> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public FrboardPost Current
        {
            get { return _currentPost; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _currentPost; }
        }

        public bool MoveNext()
        {
            return GetNextPost();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class LoadFrboardPost : IDisposable
    {
        private string _url = null;

        //private DateTime? _loadFromWebDate;
        //private string _title = null;
        //private string _author = null;
        //private DateTime? _creationDate = null;
        //private int _imageNb = 0;
        //private NamedValues _infos = new NamedValues();
        //private bool _multiPrint = false;

        private int _postId = 0;
        private string _urlFile = null;
        private string _xmlFile = null;
        private LoadFrboardPostFromWeb _loadFromWeb = null;
        private LoadFrboardPostFromXml _loadFromXml = null;

        private FrboardPost _post = null;
        private IEnumerable<FrboardPrint> _prints = null;

        private static Regex _postIdRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);

        private static bool _useUrlCache = false;
        private static string _cacheDirectory = null;
        private static string _imageCacheDirectory = "image";

        public static void ClassInit(XElement xe)
        {
            //_useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            _useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            _cacheDirectory = xe.zXPathValue("CacheDirectory");
            _imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", _imageCacheDirectory);
            LoadFrboardPostFromWeb.ClassInit(xe);
        }

        public LoadFrboardPost(string url)
        {
            _url = url;
        }

        public void Dispose()
        {
            if (_loadFromWeb != null)
            {
                _loadFromWeb.Dispose();
                _loadFromWeb = null;
            }
            if (_loadFromXml != null)
            {
                _loadFromXml.Dispose();
                _loadFromXml = null;
            }
        }

        public FrboardPost Post { get { return _post; } }
        public IEnumerable<FrboardPrint> Prints { get { return _prints; } }

        public static bool UseUrlCache { get { return _useUrlCache; } set { _useUrlCache = value; } }
        public static string CacheDirectory { get { return _cacheDirectory; } set { _cacheDirectory = value; } }
        public static string ImageCacheDirectory { get { return _imageCacheDirectory; } set { _imageCacheDirectory = value; } }

        public void Load(bool loadImage = false)
        {
            string file = GetXmlFile();
            if (!File.Exists(file))
                SaveXml();
            LoadFromXml(loadImage);
        }

        public void LoadFromWeb(bool loadImage = false)
        {
            if (_loadFromWeb == null)
            {
                string file = null;
                if (_useUrlCache)
                    file = GetUrlFile();
                _loadFromWeb = new LoadFrboardPostFromWeb(_url, file, loadImage);
                _post = _loadFromWeb.Post;
                _prints = _loadFromWeb;
            }
        }

        public void LoadFromXml(bool loadImage = false)
        {
            if (_loadFromXml == null)
            {
                string file = GetXmlFile();
                if (!File.Exists(file))
                    throw new PBException("error impossible to load post from xml file does'nt exist \"{0}\"", file);
                _loadFromXml = new LoadFrboardPostFromXml(file, loadImage);
                _post = _loadFromXml.Post;
                _prints = _loadFromXml;
            }
        }

        public void SaveXml(bool reload = false, bool saveImage = true)
        {
            string file = GetXmlFile();
            if (!reload && File.Exists(file))
                return;
            LoadFromWeb();
            zfile.CreateFileDirectory(file);
            string xmlDir = Path.GetDirectoryName(file);
            string imageCacheDirectory = null;
            if (saveImage)
                imageCacheDirectory = _imageCacheDirectory;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            Trace.CurrentTrace.WriteLine("save post to \"{0}\"", file);
            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement("post");
                _post.SaveXml(xw);
                //xw.zWriteElementWithAttributes("url", "value", _url);
                //xw.zWriteElementWithAttributes("loadFromWebDate", "value", _post.loadFromWebDate != null ? ((DateTime)_post.loadFromWebDate).ToString("yyyy-MM-dd HH:mm:ss") : "");
                //xw.zWriteElementWithAttributes("title", "value", _post.title);
                //xw.zWriteElementWithAttributes("creationDate", "value", _post.creationDate != null ? ((DateTime)_post.creationDate).ToString("yyyy-MM-dd HH:mm:ss") : "");
                //xw.zWriteElementWithAttributes("author", "value", _post.author);
                //xw.zWriteElementWithAttributes("imageNb", "value", _post.imageNb.ToString());
                foreach (FrboardPrint print in _prints)
                {
                    xw.WriteStartElement("print");
                    print.SaveXml(xw, xmlDir, imageCacheDirectory);
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
        }

        public string GetUrlFile()
        {
            if (_urlFile == null)
                _urlFile = Path.Combine(_cacheDirectory, string.Format("{0:000000}\\{1}", GetPostId() / 1000, zurl.UrlToFileName(_url, UrlFileNameType.FileName)));
            return _urlFile;
        }

        public string GetXmlFile()
        {
            // _url : "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html"
            if (_xmlFile == null)
                //_xmlFile = Path.Combine(_cacheDirectory, string.Format("{0:000000}\\{1}", GetPostId() / 1000, zurl.UrlToFileName(_url, UrlFileNameType.FileName, ".xml")));
                _xmlFile = zpath.PathSetExtension(GetUrlFile(), ".xml");
            return _xmlFile;
        }

        public int GetPostId()
        {
            // _url : "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html"
            if (_postId == 0)
            {
                Uri uri = new Uri(_url);
                string file = uri.Segments[uri.Segments.Length - 1];
                Match match = _postIdRegex.Match(file);
                if (!match.Success)
                    throw new PBException("frboard post id not found in url \"{0}\"", _url);
                _postId = int.Parse(match.Value);
            }
            return _postId;
        }

        public static IEnumerable<FrboardPrint> GetPrints(string url, bool loadImage = false)
        {
            LoadFrboardPost loaPost = new LoadFrboardPost(url);
            loaPost.Load(loadImage: loadImage);
            return loaPost.Prints;
        }
    }

    public class FrboardPostFilter
    {
        private bool _lastNodeIsImage = false;
        private static char[] _trimAll = { ' ', '\u00A0', '\r', '\n', '\t', ',', '&', '+', '*', '/' };

        public static IEnumerable<pb.old.XXNode> GetFilteredNodeList(XElement node)
        {
            FrboardPostFilter filter = new FrboardPostFilter();
            return node.DescendantNodes().zWhereSelect(filter.Filter);
        }

        private pb.old.XXNode Filter(XNode node)
        {
            if (node is XElement)
            {
                XElement xe = node as XElement;
                if (xe.Name == "img")
                {
                    //return new XXNodeImage(xe);
                    pb.old.XXNodeImage img = new pb.old.XXNodeImage(xe);
                    if (IsPrintImage(img))
                    {
                        if (_lastNodeIsImage)
                            img.followingImage = true;
                        _lastNodeIsImage = true;
                        return img;
                    }
                }
            }
            else if (node is XText)
            {
                XText xtext = node as XText;
                if (node.Parent.Name == "a")
                {
                    pb.old.XXNodeLink link = new pb.old.XXNodeLink(node.Parent);
                    link.text = link.text.Trim(_trimAll);
                    _lastNodeIsImage = false;
                    return link;
                }
                else
                {
                    pb.old.XXNodeText text = new pb.old.XXNodeText(xtext);
                    text.text = text.text.Trim(_trimAll);
                    _lastNodeIsImage = false;
                    return text;
                }
            }
            return null;
        }

        private static bool IsPrintImage(pb.old.XXNodeImage image)
        {
            if (image.alt == "Arrow")
                return false;
            if (image.source.StartsWith("http://www.frboard.com/"))
                return false;
            return true;
        }

    }

    public class LoadFrboardPostFromWeb : IEnumerable<FrboardPrint>, IEnumerator<FrboardPrint>
    {
        private string _url;
        private string _urlFile = null;
        private bool _loadImage = false;

        private IEnumerator<pb.old.XXNode> _enumNodes = null;
        private XElement _root = null;
        private XElement _postNode = null;
        private XElement _postContentNode = null;

        private FrboardPost _post;
        //private DateTime? _loadFromWebDate;
        //private string _title = null;
        //private string _author = null;
        //private DateTime? _creationDate = null;
        //private int _imageNb = 0;
        //private NamedValues _infos = new NamedValues();
        private bool _multiPrint = false;

        private FrboardPrint _currentPrint = null;
        private FrboardPrint _workingPrint = null;
        private pb.old.XXNodeText _lastTextNode = null;
        //NamedValues1
        private NamedValues<ZValue> _lastTextNodeInfos = null;

        //private static Trace _tr = Trace.CurrentTrace;

        private static string _creationDateXPath = "//span[@class='date']//text()";
        private static string _creationTimeXPath = "//span[@class='time']//text()";
        private static string _authorXPath = "//div[@class='userinfo']//a//text()";
        private static string _postElementXPath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
        //private static string _imageListXPath = ".//div[@class='content']//img";
        private static string _titleXPath = ".//h2[@class='title icon']//text()";
        private static string _contentElementXPath = ".//div[@class='content']";
        private static char[] _trimSeparators = { ' ', '\u00A0', '\r', '\n', '\t' }; // \u00A0 = espace insécable
        private static char[] _trimAll = { ' ', '\u00A0', '\r', '\n', '\t', ',', '&', '+', '*', '/' };

        private static RegexValuesList _textInfoRegexList = null;
        private static Regex _pdfInfoRegex = new Regex(@"([\|/])\s*pdf|pdf\s*([\|/])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void ClassInit(XElement xe)
        {
            _textInfoRegexList = new RegexValuesList(xe.XPathSelectElements("TextInfo"));
        }

        public LoadFrboardPostFromWeb(string url, string urlFile = null, bool loadImage = false)
        {
            _url = url;
            _urlFile = urlFile;
            _loadImage = loadImage;
            Load();
        }

        public void Dispose()
        {
        }

        public FrboardPost Post { get { return _post; } }

        private bool Load()
        {
            string url = _url;
            if (_urlFile != null)
            {
                if (!File.Exists(_urlFile))
                {
                    //if (!Frboard.LoadToFile(_url, _urlFile))
                    if (!pb.old.Http_v2.LoadToFile(_url, _urlFile))
                        return false;
                }
                url = _urlFile;
            }
            //if (Frboard.LoadUrl(url))
            //if (Http2.LoadUrl(url))
            if (pb.old.Http_v2.LoadUrl(url, Frboard.HttpRequestParameters))
            {
                _post = new FrboardPost();
                _post.url = _url;
                _post.loadFromWebDate = DateTime.Now;
                //_root = Frboard.HtmlReader.XDocument.Root;
                _root = pb.old.Http_v2.HtmlReader.XDocument.Root;
                InitPost();
                return true;
            }
            return false;
        }

        private void InitPost()
        {
            GetCreationDateTime();
            GetAuthor();
            if (!GetPostNode())
                return;

            GetTitle();

            if (!GetContentNode())
                return;

            GetMultiPrint();

            NewPrint();
            //if (!_multiPrint)
            //    _workingPrint.title = _title;

            _enumNodes = GetContentNodeList().GetEnumerator();
        }

        private bool GetCreationDateTime()
        {
            //<span class="date">Aujourd'hui,&nbsp;<span class="time">10h12</span></span>
            //string xpath = "//span[@class='date']//text()";
            if (_root == null)
                return false;

            string date = _root.zXPathValue(_creationDateXPath);
            if (date == null)
                WriteLine("warning date not found \"{0}\" (FrboardPostRead)", _creationDateXPath);

            //xpath = "//span[@class='time']//text()";
            string time = _root.zXPathValue(_creationTimeXPath);
            if (time == null)
                WriteLine("warning time not found \"{0}\" (FrboardPostRead)", _creationTimeXPath);

            if (date == null || time == null)
                return false;

            //_creationDate = FrboardPrint.GetDateTime(date.Trim(',', ' ', ' '), time.Trim(',', ' ', ' '));
            _post.creationDate = FrboardPrint.GetDateTime(date.Trim(_trimAll), time.Trim(_trimAll));
            return true;
        }

        private bool GetAuthor()
        {
            // <div class="userinfo">  <div class="username_container"> <div class="popupmenu memberaction">
            // <a rel="nofollow" class="username offline popupctrl" href="http://www.frboard.com/members/145457-lc.good.day.html" title="LC.GooD.Day est déconnecté"><strong>
            // <img src="http://www.frboard.com/images/misc/ur/general.png" class="userrank"></img><strong><font color="#696969">LC.GooD.Day</font></strong></strong></a>
            //string xpath = "//div[@class='userinfo']//a//text()";
            if (_root == null)
                return false;

            _post.author = _root.zXPathValue(_authorXPath);
            if (_post.author == null)
            {
                WriteLine("warning author not found \"{0}\" (FrboardPostRead)", _authorXPath);
                return false;
            }
            _post.author = _post.author.Trim(_trimSeparators);
            return true;
        }

        private bool GetPostNode()
        {
            //string xpath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
            if (_root == null)
                return false;

            _postNode = _root.zXPathElement(_postElementXPath);
            if (_postNode == null)
            {
                //_error = true;
                WriteLine("error post node not found \"{0}\" (FrboardPostRead)", _postElementXPath);
                return false;
            }
            return true;
        }

        private bool GetTitle()
        {
            if (_root == null)
                return false;

            //<h2 class="title icon">
            _post.title = _postNode.zXPathValue(_titleXPath);

            if (_post.title == null)
            {
                WriteLine("warning post title not found (\"{0}\")  (FrboardPostRead)", _titleXPath);
                return false;
            }
            _post.infos.SetValues(GetTextInfo(ref _post.title));
            _post.title = _post.title.Trim(_trimAll);
            return true;
        }

        private bool GetContentNode()
        {
            if (_root == null)
                return false;

            //<div class="content">
            _postContentNode = _postNode.XPathSelectElement(_contentElementXPath);
            if (_postContentNode == null)
            {
                //_error = true;
                WriteLine("error content element not found : \"{0}\" (FrboardPostRead)", _contentElementXPath);
                return false;
            }
            return true;
        }

        private bool GetMultiPrint()
        {
            if (_root == null)
                return false;

            _post.imageNb = (from n in GetContentNodeList() where n.type == pb.old.XXNodeType.Image && !((pb.old.XXNodeImage)n).followingImage select n).Count();
            if (_post.imageNb > 1)
                _multiPrint = true;
            _post.multiPrint = _multiPrint;
            if (_post.imageNb == 0)
            {
                WriteLine("warning no image found (FrboardPostRead)");
                return false;
            }
            return true;
        }

        private IEnumerable<pb.old.XXNode> GetContentNodeList()
        {
            if (_root == null)
                return new pb.old.XXNode[0];
            return FrboardPostFilter.GetFilteredNodeList(_postContentNode);
        }

        private void NewPrint()
        {
            _workingPrint = new FrboardPrint();
            _workingPrint.url = _url;
            _workingPrint.loadFromWebDate = _post.loadFromWebDate;
            _workingPrint.postTitle = _post.title;
            _workingPrint.author = _post.author;
            _workingPrint.creationDate = _post.creationDate;
            _workingPrint.multiPrint = _multiPrint;
            if (!_multiPrint)
                _workingPrint.title = _post.title;
        }

        public IEnumerator<FrboardPrint> GetEnumerator()
        {
            return this;
        }

        private bool GetNextPrint()
        {
            _currentPrint = null;
            if (_enumNodes == null)
                return false;
            while (_enumNodes.MoveNext())
            {
                pb.old.XXNode node = _enumNodes.Current;
                if (node.type == pb.old.XXNodeType.Text)
                {
                    AddTextNode(node as pb.old.XXNodeText);
                }
                else if (node.type == pb.old.XXNodeType.Image)
                {
                    pb.old.XXNodeImage img = node as pb.old.XXNodeImage;
                    if (_multiPrint && !img.followingImage)
                    {
                        if (_workingPrint.title != null && _workingPrint.images.Count > 0)
                            //_prints.Add(_print);
                            _currentPrint = _workingPrint;
                        NewPrint();
                    }
                    pb.old.ImageHtml imgUrl = new pb.old.ImageHtml(img.source, img.alt, img.title, img.className);
                    if (_loadImage)
                        //imgUrl.Image = Frboard.LoadImageFromWeb(imgUrl.Source);
                        imgUrl.Image = pb.old.Http_v2.LoadImageFromWeb(imgUrl.Source, Frboard.HttpRequestParameters);
                    _workingPrint.images.Add(imgUrl);
                    if (_currentPrint != null)
                        return true;
                }
                else if (node.type == pb.old.XXNodeType.Link)
                {
                    AddTextNode(null);
                    pb.old.XXNodeLink link = node as pb.old.XXNodeLink;
                    _workingPrint.downloadLinks.Add(link.text);
                }
            }
            _enumNodes = null;
            AddTextNode(null);
            if (_workingPrint.title != null && _workingPrint.images.Count > 0)
            {
                //_prints.Add(_print);
                _currentPrint = _workingPrint;
                return true;
            }
            return false;
        }

        public void AddTextNode(pb.old.XXNodeText node)
        {
            //NamedValues1
            NamedValues<ZValue> values = null;
            if (node != null)
            {
                values = GetTextInfo(ref node.text);
                node.text = node.text.Trim(_trimAll);
                if (string.Equals(node.text, _post.title, StringComparison.CurrentCultureIgnoreCase))
                    return;
            }
            if (_lastTextNode != null)
            {
                _workingPrint.infos.SetValues(_lastTextNodeInfos);
                if (_lastTextNode.text != "")
                {
                    if (_workingPrint.title == null)
                        _workingPrint.title = _lastTextNode.text;
                    else
                    {
                        Match match = _pdfInfoRegex.Match(_lastTextNode.text);
                        if (match.Success)
                        {
                            string sep = match.Groups[1].Value;
                            if (sep == "")
                                sep = match.Groups[2].Value;
                            _workingPrint.SetInfo(_lastTextNode.text, sep[0]);
                        }
                        else if (_workingPrint.downloadLinks.Count == 0)
                        {
                            if (_workingPrint.description == null)
                                _workingPrint.description = _lastTextNode.text;
                            else
                                _workingPrint.description += "\r\n" + _lastTextNode.text;
                        }
                    }
                }
            }
            _lastTextNode = node;
            _lastTextNodeInfos = values;
        }

        private static NamedValues<ZValue> GetTextInfo(ref string text)
        {
            NamedValues<ZValue> values = new NamedValues<ZValue>();
            foreach (RegexValues rv in _textInfoRegexList.Values)
            {
                //Match match = rv.Match_old(text);
                MatchValues matchValues = rv.Match(text);
                //if (match.Success)
                if (matchValues.Success)
                {
                    //values.SetValues(rv.GetValues_old());
                    values.SetValues(matchValues.GetValues());
                    //text = rv.MatchReplace_old("");
                    text = matchValues.Replace("");
                }
            }
            return values;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public FrboardPrint Current
        {
            get { return _currentPrint; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _currentPrint; }
        }

        public bool MoveNext()
        {
            return GetNextPrint();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class LoadFrboardPostFromXml : IEnumerable<FrboardPrint>, IEnumerator<FrboardPrint>
    {
        private string _file;
        private bool _loadImage = false;
        private string _fileDirectory = null;
        private XmlReader _xmlReader = null;
        private FrboardPost _post = null;
        private FrboardPrint _currentPrint = null;

        //private static Trace _tr = Trace.CurrentTrace;

        public LoadFrboardPostFromXml(string file, bool loadImage = false)
        {
            _file = file;
            _loadImage = loadImage;
            Load();
        }

        public void Dispose()
        {
            if (_xmlReader != null)
            {
                _xmlReader.Close();
                _xmlReader = null;
            }
        }

        public FrboardPost Post { get { return _post; } }

        private void Load()
        {
            _fileDirectory = Path.GetDirectoryName(_file);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            _xmlReader = XmlReader.Create(_file, settings);
            _post = new FrboardPost();
            bool root = false;
            while (!_xmlReader.EOF)
            {
                _xmlReader.Read();
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (!root)
                    {
                        root = true;
                        continue;
                    }

                    if (_xmlReader.IsEmptyElement)
                    {
                        string s;
                        DateTime dt;
                        switch (_xmlReader.Name)
                        {
                            case "url":
                                _post.url = _xmlReader.zReadAttributeString();
                                break;
                            case "loadFromWebDate":
                                s = _xmlReader.zReadAttributeString();
                                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                                    _post.loadFromWebDate = dt;
                                else
                                    WriteLine("error wrong loadFromWebDate date time value \"{0}\" in xml  (FrboardPostRead)", s);
                                break;
                            case "title":
                                _post.title = _xmlReader.zReadAttributeString();
                                break;
                            case "creationDate":
                                s = _xmlReader.zReadAttributeString();
                                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                                    _post.creationDate = dt;
                                else
                                    WriteLine("error wrong creationDate date time value \"{0}\" in xml  (FrboardPostRead)", s);
                                break;
                            case "author":
                                _post.author = _xmlReader.zReadAttributeString();
                                break;
                            case "imageNb":
                                s = _xmlReader.zReadAttributeString();
                                if (!int.TryParse(s, out _post.imageNb))
                                    WriteLine("error wrong imageNb int value \"{0}\" in xml  (FrboardPostRead)", s);
                                break;
                            default:
                                WriteLine("error unknow element \"{0}\" in xml  (FrboardPostRead)", _xmlReader.Name);
                                break;
                        }
                    }
                    else if (_xmlReader.Name == "print")
                        break;
                    else
                        WriteLine("error unknow element \"{0}\" in xml  (FrboardPostRead)", _xmlReader.Name);
                }
            }
        }

        private bool GetNextPrint()
        {
            while (!_xmlReader.EOF)
            {
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_xmlReader.Name == "print" && !_xmlReader.IsEmptyElement)
                    {
                        NewPrint();
                        string fileDirectory = null;
                        if (_loadImage)
                            fileDirectory = _fileDirectory;
                        _currentPrint.LoadFromXml(_xmlReader, fileDirectory);
                        return true;
                    }
                    else
                        WriteLine("error unknow xml element \"{0}\"  (FrboardPostRead)", _xmlReader.Name);
                }
                _xmlReader.Read();
            }
            _xmlReader.Close();
            _xmlReader = null;
            return false;
        }

        private void NewPrint()
        {
            _currentPrint = new FrboardPrint();
            _currentPrint.url = _post.url;
            _currentPrint.loadFromWebDate = _post.loadFromWebDate;
            _currentPrint.postTitle = _post.title;
            _currentPrint.author = _post.author;
            _currentPrint.creationDate = _post.creationDate;
            _currentPrint.multiPrint = _post.multiPrint;
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }

        public IEnumerator<FrboardPrint> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public FrboardPrint Current
        {
            get { return _currentPrint; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _currentPrint; }
        }

        public bool MoveNext()
        {
            return GetNextPrint();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public static class Frboard
    {
        //private static HtmlXmlReader _hxr = null;
        public const string Domain = "http://www.frboard.com/";
        public const string UrlPrint = "http://www.frboard.com/magazines-et-journaux/";
        //private static Trace _tr = Trace.CurrentTrace;
        ////private static int _loadXmlRetryTimeout = 180;
        //private static int _loadRepeatIfError = 5;
        private static HttpRequestParameters_v1 _requestParameters = new HttpRequestParameters_v1();

        static Frboard()
        {
            //_hxr = HtmlXmlReader.CurrentHtmlXmlReader;
            ////_hxr.LoadXmlRetryTimeout = _loadXmlRetryTimeout;
            //_hxr.LoadRepeatIfError = _loadRepeatIfError;
            //_hxr.Cookies.zAdd(Domain, GetCookies());
            //Http2.HtmlReader.Cookies.zAdd(Domain, GetCookies());
            _requestParameters.cookies.zAdd(Domain, GetCookies());
        }

        public static HttpRequestParameters_v1 HttpRequestParameters { get { return _requestParameters; } }

        //public static bool LoadUrl(string url)
        //{
        //    try
        //    {
        //        _hxr.Load(url);
        //        //WriteLine("request headers :");
        //        //WriteHeaders(_hxr.http.Request.Headers);
        //        //WriteLine("response headers :");
        //        //WriteHeaders(_hxr.http.Response.Headers);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        //Load("http://www.frboard.com/magazines-et-journaux/441873-multi-les-journaux-mardi-13-aout-2013-pdf-lien-direct.html");
        //        //15/08/2013 12:00:32 Error : A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 5.199.168.178:80 (System.Net.Sockets.SocketException)
        //        //Unable to connect to the remote server (System.Net.WebException)
        //        //----------------------
        //        //   at System.Net.Sockets.Socket.DoConnect(EndPoint endPointSnapshot, SocketAddress socketAddress)
        //        //   at System.Net.ServicePoint.ConnectSocketInternal(Boolean connectFailure, Socket s4, Socket s6, Socket& socket, IPAddress& address, ConnectSocketState state, IAsyncResult asyncResult, Int32 timeout, Exception& exception)
        //        //----------------------
        //        //   at System.Net.HttpWebRequest.GetResponse()
        //        //   at Http.OpenWebRequest() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 911
        //        //   at Http.Open() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 780
        //        //   at Http.Load() in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\Http_Html.cs:line 503
        //        //   at HtmlXmlReader.Load(String sUrl) in c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\PB_Tools\\HtmlXmlReader.cs:line 426
        //        //   at Print.download.w.Test_frboard_02()
        //        //   at Print.download.w.Run()

        //        WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
        //        return false;
        //    }
        //}

        //public static bool LoadToFile(string url, string file)
        //{
        //    try
        //    {
        //        _hxr.LoadToFile(url, file);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
        //        return false;
        //    }
        //}

        //public static void WriteHeaders(WebHeaderCollection headers)
        //{
        //    foreach (string key in headers.AllKeys)
        //        WriteLine("  {0} = \"{1}\"", key, headers[key]);
        //}

        //public static Image LoadImageFromWeb(string url)
        //{
        //    try
        //    {
        //        Image image = _hxr.LoadImage(url);
        //        if (image.Height > 200)
        //            image = image.zResize(height: 200);
        //        return image;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
        //        return null;
        //    }
        //}

        //public static Image LoadImageFromFile(string file)
        //{
        //    try
        //    {
        //        Image image = zimg.LoadFromFile(file);
        //        if (image.Height > 200)
        //            image = image.zResize(height: 200);
        //        return image;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLine("Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());
        //        return null;
        //    }
        //}

        //public static void WriteLine(string msg, params object[] prm)
        //{
        //    _tr.WriteLine(msg, prm);
        //}

        public static string[] GetCookies()
        {
            // Cookie "skimlinks_enabled=1; bb_lastvisit=1367138318; bb_lastactivity=0; bb_userid=170354; bb_password=f2911aa1a0f89fd2a427ec51dc63b92c; bb_forum_view=3c2096874c61a68fb01f0a6e2e41d54fc89c6092a-5-%7Bi-174_i-1376444795_i-58_i-1376444534_i-230_i-1376444542_i-235_i-1376444624_i-216_i-1376444705_%7D; bb_thread_lastview=454b397cfcf2e2a388b263d9bf8df753a3c16469a-64-%7Bi-439694_i-1375442859_i-439634_i-1375393480_i-439610_i-1375391002_i-439581_i-1375368599_i-439879_i-1375553344_i-439973_i-1375555033_i-440080_i-1375618837_i-440074_i-1375616844_i-440073_i-1375616654_i-440068_i-1375615800_i-440059_i-1375610217_i-440057_i-1375610133_i-440056_i-1375610088_i-440055_i-1375610032_i-440047_i-1375605750_i-440009_i-1375576849_i-439996_i-1375568220_i-440257_i-1375699047_i-440273_i-1375693905_i-440272_i-1375693791_i-440270_i-1375693433_i-440269_i-1375693232_i-440215_i-1375657930_i-440150_i-1375653547_i-440182_i-1375642502_i-440181_i-1375641538_i-440169_i-1375639340_i-440168_i-1375639173_i-440478_i-1375794642_i-440449_i-1375787364_i-440448_i-1375783706_i-440443_i-1375783596_i-440440_i-1375783498_i-440433_i-1375783240_i-440415_i-1375767642_i-440367_i-1375736849_i-440357_i-1375730482_i-440676_i-1375881935_i-440668_i-1375877796_i-440649_i-1375870010_i-440579_i-1375868638_i-440970_i-1375972314_i-440926_i-1375971263_i-440893_i-1375967645_i-440921_i-1375964311_i-440906_i-1375962813_i-440922_i-1375957995_i-441237_i-1376061837_i-441200_i-1376055533_i-441090_i-1376031658_i-441032_i-1376000818_i-441332_i-1376146506_i-441515_i-1376204841_i-441717_i-1376307904_i-441664_i-1376262586_i-441661_i-1376262454_i-441626_i-1376247844_i-441625_i-1376247778_i-441835_i-1376379820_i-441834_i-1376379720_i-441815_i-1376348204_i-441745_i-1376315912_i-441873_i-1376461190_i-442236_i-1376461791_%7D; __utma=164418920.193645274.1367138295.1376462653.1376465584.74; __utmb=164418920.1.10.1376465584; __utmc=164418920; __utmz=164418920.1371282698.13.2.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided); vbseo_loggedin=yes; bb_sessionhash=8d2210ff76e837b72a39473bb3e35d19"
            //   skimlinks_enabled=1;
            //   bb_lastvisit=1367138318;
            //   bb_lastactivity=0;
            //   bb_userid=170354;
            //   bb_password=f2911aa1a0f89fd2a427ec51dc63b92c;
            //   bb_forum_view=3c2096874c61a68fb01f0a6e2e41d54fc89c6092a-5-%7Bi-174_i-1376444795_i-58_i-1376444534_i-230_i-1376444542_i-235_i-1376444624_i-216_i-1376444705_%7D;
            //   bb_thread_lastview=454b397cfcf2e2a388b263d9bf8df753a3c16469a-64-%7Bi-439694_i-1375442859_i-439634_i-1375393480_i-439610_i-1375391002_i-439581_i-1375368599_i-439879_i-1375553344_i-439973_i-1375555033_i-440080_i-1375618837_i-440074_i-1375616844_i-440073_i-1375616654_i-440068_i-1375615800_i-440059_i-1375610217_i-440057_i-1375610133_i-440056_i-1375610088_i-440055_i-1375610032_i-440047_i-1375605750_i-440009_i-1375576849_i-439996_i-1375568220_i-440257_i-1375699047_i-440273_i-1375693905_i-440272_i-1375693791_i-440270_i-1375693433_i-440269_i-1375693232_i-440215_i-1375657930_i-440150_i-1375653547_i-440182_i-1375642502_i-440181_i-1375641538_i-440169_i-1375639340_i-440168_i-1375639173_i-440478_i-1375794642_i-440449_i-1375787364_i-440448_i-1375783706_i-440443_i-1375783596_i-440440_i-1375783498_i-440433_i-1375783240_i-440415_i-1375767642_i-440367_i-1375736849_i-440357_i-1375730482_i-440676_i-1375881935_i-440668_i-1375877796_i-440649_i-1375870010_i-440579_i-1375868638_i-440970_i-1375972314_i-440926_i-1375971263_i-440893_i-1375967645_i-440921_i-1375964311_i-440906_i-1375962813_i-440922_i-1375957995_i-441237_i-1376061837_i-441200_i-1376055533_i-441090_i-1376031658_i-441032_i-1376000818_i-441332_i-1376146506_i-441515_i-1376204841_i-441717_i-1376307904_i-441664_i-1376262586_i-441661_i-1376262454_i-441626_i-1376247844_i-441625_i-1376247778_i-441835_i-1376379820_i-441834_i-1376379720_i-441815_i-1376348204_i-441745_i-1376315912_i-441873_i-1376461190_i-442236_i-1376461791_%7D;
            //   __utma=164418920.193645274.1367138295.1376462653.1376465584.74;
            //   __utmb=164418920.1.10.1376465584;
            //   __utmc=164418920;
            //   __utmz=164418920.1371282698.13.2.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided);
            //   vbseo_loggedin=yes;
            //   bb_sessionhash=8d2210ff76e837b72a39473bb3e35d19
            //
            // hapshack.com :
            //   __utma=161206820.1103252835.1376585085.1376585085.1376585085.1
            //   __utmz=161206820.1376585085.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)
            return new string[] {
                "skimlinks_enabled=1",
                "bb_lastvisit=1367138318",
                "bb_lastactivity=0",
                "bb_userid=170354",
                "bb_password=f2911aa1a0f89fd2a427ec51dc63b92c",
                //"bb_forum_view=3c2096874c61a68fb01f0a6e2e41d54fc89c6092a-5-%7Bi-174_i-1376444795_i-58_i-1376444534_i-230_i-1376444542_i-235_i-1376444624_i-216_i-1376444705_%7D",
                //"bb_thread_lastview=454b397cfcf2e2a388b263d9bf8df753a3c16469a-64-%7Bi-439694_i-1375442859_i-439634_i-1375393480_i-439610_i-1375391002_i-439581_i-1375368599_i-439879_i-1375553344_i-439973_i-1375555033_i-440080_i-1375618837_i-440074_i-1375616844_i-440073_i-1375616654_i-440068_i-1375615800_i-440059_i-1375610217_i-440057_i-1375610133_i-440056_i-1375610088_i-440055_i-1375610032_i-440047_i-1375605750_i-440009_i-1375576849_i-439996_i-1375568220_i-440257_i-1375699047_i-440273_i-1375693905_i-440272_i-1375693791_i-440270_i-1375693433_i-440269_i-1375693232_i-440215_i-1375657930_i-440150_i-1375653547_i-440182_i-1375642502_i-440181_i-1375641538_i-440169_i-1375639340_i-440168_i-1375639173_i-440478_i-1375794642_i-440449_i-1375787364_i-440448_i-1375783706_i-440443_i-1375783596_i-440440_i-1375783498_i-440433_i-1375783240_i-440415_i-1375767642_i-440367_i-1375736849_i-440357_i-1375730482_i-440676_i-1375881935_i-440668_i-1375877796_i-440649_i-1375870010_i-440579_i-1375868638_i-440970_i-1375972314_i-440926_i-1375971263_i-440893_i-1375967645_i-440921_i-1375964311_i-440906_i-1375962813_i-440922_i-1375957995_i-441237_i-1376061837_i-441200_i-1376055533_i-441090_i-1376031658_i-441032_i-1376000818_i-441332_i-1376146506_i-441515_i-1376204841_i-441717_i-1376307904_i-441664_i-1376262586_i-441661_i-1376262454_i-441626_i-1376247844_i-441625_i-1376247778_i-441835_i-1376379820_i-441834_i-1376379720_i-441815_i-1376348204_i-441745_i-1376315912_i-441873_i-1376461190_i-442236_i-1376461791_%7D",
                "__utma=164418920.193645274.1367138295.1376462653.1376465584.74",
                "__utmb=164418920.1.10.1376465584",
                "__utmc=164418920",
                "__utmz=164418920.1371282698.13.2.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)",
                "vbseo_loggedin=yes",
                "bb_sessionhash=8d2210ff76e837b72a39473bb3e35d1"
            };
        }
    }
}

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

namespace Print.download
{

    //public class DataProxy<T> : IEnumerable<T>, IEnumerator<T>
    //{
    //    private IDataProxy<T> _dataProxy = null;

    //    public DataProxy(IDataProxy<T> dataProxy)
    //    {
    //        _dataProxy = dataProxy;
    //        if (dataProxy.DataInProxy())
    //            dataProxy.LoadDataFromProxy();
    //        else
    //            dataProxy.LoadDataFromWeb();
    //    }
    //}

    public class FrboardPostRead : IEnumerable<FrboardPrint>, IEnumerator<FrboardPrint>
    {
        private static string _creationDateXPath = "//span[@class='date']//text()";
        private static string _creationTimeXPath = "//span[@class='time']//text()";
        private static string _authorXPath = "//div[@class='userinfo']//a//text()";
        private static string _postElementXPath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
        private static string _imageListXPath = ".//div[@class='content']//img";
        private static string _titleXPath = ".//h2[@class='title icon']//text()";
        private static string _contentElementXPath = ".//div[@class='content']";
        private static char[] _trimSeparators = { ' ', '\u00A0', '\r', '\n', '\t' }; // \u00A0 = espace insécable
        private static char[] _trimAll = { ' ', '\u00A0', '\r', '\n', '\t', ',', '&', '+', '*', '/' };

        //"LES MAGAZINES - JEUDI 15 AOUT 2013 & + [PDF][Liens Direct]"
        private static RegexValuesList _textInfoRegexList = null;

        //private static Regex _pdfRegex = new Regex(@"\[pdf\](?:\s*\[Liens?\s*Direct\])?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _pdfRegex = new Regex(@"\[pdf\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _pdfInfoRegex = new Regex(@"([\|/])\s*pdf|pdf\s*([\|/])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static Regex _noInfoRegex = new Regex(@"^(?:\[pdf\]|\[Liens?\s*Direct\]|[\*\s&+/]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // |[\*\s&+/]
        private static Regex _noInfoRegex = new Regex(@"(?:\[Liens?\s*Direct\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _postIdRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);

        private static string _xmlDirectory = null;
        private static string _xmlImageDirectory = "image";

        private static Trace _tr = Trace.CurrentTrace;

        private string _url = null;
        private bool _loadImage = false;

        // load from web
        private bool _loadFromWeb = false;
        private IEnumerator<XXNode> _enumNodes = null;
        private XElement _root = null;
        private XElement _postNode = null;
        //private List<FrboardPrint> _prints = null;
        private DateTime? _loadFromWebDate;
        private DateTime? _postCreationDate = null;
        private string _postAuthor = null;
        private int _postImageNb = 0;
        private bool _postMultiPrint = false;
        private string _postTitle = null;
        private XElement _postContentNode = null;
        //NamedValues1
        private NamedValues<ZValue> _postInfos = new NamedValues<ZValue>();
        //private bool _error = false;
        private FrboardPrint _currentPrint = null;
        private FrboardPrint _workingPrint = null;
        private XXNodeText _lastTextNode = null;
        //NamedValues1
        private NamedValues<ZValue> _lastTextNodeInfos = null;

        private bool _loadFromXml = false;
        private string _xmlFile = null;
        private string _xmlFileDirectory = null;
        private XmlReader _xmlReader = null;

        public FrboardPostRead(string url, bool loadImage = false)
        {
            _url = url;
            _loadImage = loadImage;
            //if (!Frboard.LoadUrl(url))
            //{
            //    _error = true;
            //    return;
            //}
        }

        public void Dispose()
        {
            if (_xmlReader != null)
            {
                _xmlReader.Close();
                _xmlReader = null;
            }
        }

        public static string XmlDirectory { get { return _xmlDirectory; } set { _xmlDirectory = value; } }
        public static string ImageDirectory { get { return _xmlImageDirectory; } set { _xmlImageDirectory = value; } }

        public DateTime? LoadFromWebDate { get { return _loadFromWebDate; } }
        public DateTime? CreationDate { get { return _postCreationDate; } }
        public string Author { get { return _postAuthor; } }
        public int NbImage { get { return _postImageNb; } }
        public bool MultiPrint { get { return _postMultiPrint; } }
        public string Title { get { return _postTitle; } }

        public void SaveXml()
        {
            LoadFromWeb();
            string file = GetXmlPath();
            zfile.CreateFileDirectory(file);
            string xmlDir = Path.GetDirectoryName(file);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(file, settings))
            {
                xw.WriteStartElement("post");

                //xw.WriteStartElement("url");
                //xw.WriteAttributeString("value", _url);
                //xw.WriteEndElement();
                xw.zWriteElementWithAttributes("url", "value", _url);

                //xw.WriteStartElement("loadFromWebDate");
                //xw.WriteAttributeString("value", ((DateTime)_loadFromWebDate).ToString("yyyy-MM-dd HH:mm:ss"));
                //xw.WriteEndElement();
                xw.zWriteElementWithAttributes("loadFromWebDate", "value", ((DateTime)_loadFromWebDate).ToString("yyyy-MM-dd HH:mm:ss"));

                //writer.WriteStartElement("loadImage");
                //writer.WriteAttributeString("value", _loadImage.ToString());
                //writer.WriteEndElement();

                //xw.WriteStartElement("title");
                //xw.WriteAttributeString("value", _postTitle);
                //xw.WriteEndElement();
                xw.zWriteElementWithAttributes("title", "value", _postTitle);

                //xw.WriteStartElement("creationDate");
                //xw.WriteAttributeString("value", ((DateTime)_postCreationDate).ToString("yyyy-MM-dd HH:mm:ss"));
                //xw.WriteEndElement();
                xw.zWriteElementWithAttributes("creationDate", "value", ((DateTime)_postCreationDate).ToString("yyyy-MM-dd HH:mm:ss"));

                //xw.WriteStartElement("author");
                //xw.WriteAttributeString("value", _postAuthor);
                //xw.WriteEndElement();
                xw.zWriteElementWithAttributes("author", "value", _postAuthor);

                //xw.WriteStartElement("imageNb");
                //xw.WriteAttributeString("value", _postImageNb.ToString());
                //xw.WriteEndElement();
                xw.zWriteElementWithAttributes("imageNb", "value", _postImageNb.ToString());

                foreach (FrboardPrint print in this)
                {
                    xw.WriteStartElement("print");
                    print.SaveXml(xw, xmlDir, _xmlImageDirectory);
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
            }
        }

        public void LoadFromXml()
        {
            _xmlFile = GetXmlPath();
            _xmlFileDirectory = Path.GetDirectoryName(_xmlFile);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            //using (XmlReader xr = XmlReader.Create(file, settings))
            _xmlReader = XmlReader.Create(_xmlFile, settings);
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
                                _url = _xmlReader.zReadAttributeString();
                                break;
                            case "loadFromWebDate":
                                s = _xmlReader.zReadAttributeString();
                                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                                    _loadFromWebDate = dt;
                                else
                                    WriteLine("error wrong loadFromWebDate date time value \"{0}\" in xml  (FrboardPostRead)", s);
                                break;
                            case "title":
                                _postTitle = _xmlReader.zReadAttributeString();
                                break;
                            case "creationDate":
                                s = _xmlReader.zReadAttributeString();
                                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                                    _postCreationDate = dt;
                                else
                                    WriteLine("error wrong creationDate date time value \"{0}\" in xml  (FrboardPostRead)", s);
                                break;
                            case "author":
                                _postAuthor = _xmlReader.zReadAttributeString();
                                break;
                            case "imageNb":
                                s = _xmlReader.zReadAttributeString();
                                if (!int.TryParse(s, out _postImageNb))
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
            _loadFromXml = true;
        }

        private bool GetNextPrintFromXml()
        {
            if (_xmlReader == null)
            {
                WriteLine("error xmlReader is null (FrboardPostRead)");
                return false;
            }
            while (!_xmlReader.EOF)
            {
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_xmlReader.Name == "print" && !_xmlReader.IsEmptyElement)
                    {
                        NewPrint();
                        _workingPrint.LoadFromXml(_xmlReader, _xmlFileDirectory);
                        _currentPrint = _workingPrint;
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

        public int GetPostId()
        {
            // _url : "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html"
            Uri uri = new Uri(_url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = _postIdRegex.Match(file);
            if (!match.Success)
                throw new PBException("frboard post id not found in url \"{0}\"", _url);
            return int.Parse(match.Value);
        }

        public string GetXmlPath()
        {
            // _url : "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html"
            return Path.Combine(_xmlDirectory, string.Format("{0:000000}\\{1}", GetPostId() / 1000, zurl.UrlToFileName(_url, UrlFileNameType.FileName, ".xml")));
        }

        public void LoadFromWeb()
        {
            if (_enumNodes != null)
                return;
            if (Frboard.LoadUrl(_url))
            {
                _root = Frboard.HtmlReader.XDocument.Root;
                _loadFromWebDate = DateTime.Now;
                InitPost();
                _loadFromWeb = true;
            }
        }

        //public FrboardPrint[] LoadPost(bool loadImage = false)
        //{
        //    //_prints = new List<FrboardPrint>();
        //    if (_root != null)
        //        return _LoadPost(loadImage);
        //    else
        //        return new FrboardPrint[0];
        //    //return _prints.ToArray(); ;
        //}

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
            if (!_postMultiPrint)
                _workingPrint.title = _postTitle;

            _enumNodes = GetContentNodeList().GetEnumerator();
        }

        public bool GetCreationDateTime()
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
            _postCreationDate = FrboardPrint.GetDateTime(date.Trim(_trimAll), time.Trim(_trimAll));
            return true;
        }

        public bool GetAuthor()
        {
            // <div class="userinfo">  <div class="username_container"> <div class="popupmenu memberaction">
            // <a rel="nofollow" class="username offline popupctrl" href="http://www.frboard.com/members/145457-lc.good.day.html" title="LC.GooD.Day est déconnecté"><strong>
            // <img src="http://www.frboard.com/images/misc/ur/general.png" class="userrank"></img><strong><font color="#696969">LC.GooD.Day</font></strong></strong></a>
            //string xpath = "//div[@class='userinfo']//a//text()";
            if (_root == null)
                return false;

            _postAuthor = _root.zXPathValue(_authorXPath);
            if (_postAuthor == null)
            {
                WriteLine("warning author not found \"{0}\" (FrboardPostRead)", _authorXPath);
                return false;
            }
            _postAuthor = _postAuthor.Trim(_trimSeparators);
            return true;
        }

        public bool GetPostNode()
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

        public bool GetTitle()
        {
            if (_root == null)
                return false;

            //<h2 class="title icon">
            _postTitle = _postNode.zXPathValue(_titleXPath);

            if (_postTitle == null)
            {
                WriteLine("warning post title not found (\"{0}\")  (FrboardPostRead)", _titleXPath);
                return false;
            }
            _postInfos.SetValues(GetTextInfo(ref _postTitle));
            _postTitle = _postTitle.Trim(_trimAll);
            return true;
        }

        public bool GetContentNode()
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

        public bool GetMultiPrint()
        {
            _postMultiPrint = false;
            if (_root == null)
                return false;

            _postImageNb = (from n in GetContentNodeList() where n.type == XXNodeType.Image && !((XXNodeImage)n).followingImage select n).Count();
            if (_postImageNb > 1)
                _postMultiPrint = true;
            //WriteLine("multi print {0}", _multiPrint);
            if (_postImageNb == 0)
            {
                WriteLine("warning no image found (FrboardPostRead)");
                return false;
            }
            return true;
        }

        public IEnumerable<XXNode> GetContentNodeList()
        {
            if (_root == null)
                return new XXNode[0];
            return FrboardPostFilter.GetFilteredNodeList(_postContentNode);
        }

        private void NewPrint()
        {
            _workingPrint = new FrboardPrint();
            _workingPrint.url = _url;
            _workingPrint.loadFromWebDate = _loadFromWebDate;
            _workingPrint.creationDate = _postCreationDate;
            _workingPrint.author = _postAuthor;
            _workingPrint.postTitle = _postTitle;
            _workingPrint.multiPrint = _postMultiPrint;
        }

        public void AddTextNode(XXNodeText node)
        {
            //NamedValues1
            NamedValues<ZValue> values = null;
            if (node != null)
            {
                values = GetTextInfo(ref node.text);
                node.text = node.text.Trim(_trimAll);
                if (string.Equals(node.text, _postTitle, StringComparison.CurrentCultureIgnoreCase))
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

        private bool GetNextPrintFromWeb()
        {
            _currentPrint = null;
            if (_enumNodes == null)
                return false;
            while (_enumNodes.MoveNext())
            {
                XXNode node = _enumNodes.Current;
                if (node.type == XXNodeType.Text)
                {
                    AddTextNode(node as XXNodeText);
                }
                else if (node.type == XXNodeType.Image)
                {
                    XXNodeImage img = node as XXNodeImage;
                    if (_postMultiPrint && !img.followingImage)
                    {
                        if (_workingPrint.title != null && _workingPrint.images.Count > 0)
                            //_prints.Add(_print);
                            _currentPrint = _workingPrint;
                        NewPrint();
                    }
                    ImageHtml imgUrl = new ImageHtml(img.source, img.alt, img.title, img.className);
                    if (_loadImage)
                        imgUrl.Image = Frboard.LoadImageFromWeb(imgUrl.Source);
                    _workingPrint.images.Add(imgUrl);
                    if (_currentPrint != null)
                        return true;
                }
                else if (node.type == XXNodeType.Link)
                {
                    AddTextNode(null);
                    XXNodeLink link = node as XXNodeLink;
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

        //private FrboardPrint[] _LoadPost(bool loadImage = false)
        //{
        //    //GetCreationDateTime();
        //    //GetAuthor();
        //    //if (!GetPostNode())
        //    //    return;

        //    //GetTitle();

        //    //if (!GetContentNode())
        //    //    return;

        //    //GetMultiPrint();

        //    //NewPrint();
        //    //if (!_multiPrint)
        //    //    _print.title = _title;

        //    InitPost();

        //    List<FrboardPrint> prints = new List<FrboardPrint>();

        //    foreach (XXNode child in GetContentNodeList())
        //    {
        //        if (child.type == XXNodeType.Text)
        //        {
        //            AddTextNode(child as XXNodeText);
        //        }
        //        else if (child.type == XXNodeType.Image)
        //        {
        //            XXNodeImage node = child as XXNodeImage;
        //            if (_postMultiPrint && !node.followingImage)
        //            {
        //                if (_workingPrint.title != null && _workingPrint.images.Count > 0)
        //                    prints.Add(_workingPrint);
        //                NewPrint();
        //            }
        //            UrlImage img = new UrlImage(node.source, node.alt, node.title, node.className);
        //            if (loadImage)
        //                img.Image = Frboard.LoadImage(img.Source);
        //            _workingPrint.images.Add(img);
        //        }
        //        else if (child.type == XXNodeType.Link)
        //        {
        //            AddTextNode(null);
        //            XXNodeLink node = child as XXNodeLink;
        //            _workingPrint.downloadLinks.Add(node.text);
        //        }
        //    }
        //    AddTextNode(null);
        //    if (_workingPrint.title != null && _workingPrint.images.Count > 0)
        //        prints.Add(_workingPrint);
        //    return prints.ToArray();
        //}

        public static string CreationDateXPath { get { return _creationDateXPath; } }
        public static string CreationTimeXPath { get { return _creationTimeXPath; } }
        public static string AuthorXPath { get { return _authorXPath; } }
        public static string PostElementXPath { get { return _postElementXPath; } }
        public static string ImageListXPath { get { return _imageListXPath; } }
        public static string TitleXPath { get { return _titleXPath; } }
        public static string ContentElementXPath { get { return _contentElementXPath; } }
        public static char[] TrimAll { get { return _trimAll; } }

        public static void Init(XElement xe)
        {
            _xmlDirectory = xe.zXPathValue("CacheDirectory");
            _xmlImageDirectory = xe.zXPathValue("ImageCacheDirectory", _xmlImageDirectory);
            _textInfoRegexList = new RegexValuesList(xe.XPathSelectElements("TextInfo"));
        }

        //NamedValues1
        private static NamedValues<ZValue> GetTextInfo(ref string text)
        {
            //NamedValues1
            NamedValues<ZValue> values = new NamedValues<ZValue>();
            foreach (RegexValues rv in _textInfoRegexList.Values)
            {
                Match match = rv.Match(text);
                if (match.Success)
                {
                    values.SetValues(rv.GetValues());
                    text = rv.MatchReplace("");
                }
            }
            return values;
        }

        private static bool IsPrintImage(ImageHtml image)
        {
            if (image.Alt == "Arrow")
                return false;
            if (image.Source.StartsWith("http://www.frboard.com/"))
                return false;
            return true;
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
            //return GetNextPrint();
            bool b = false;
            if (_loadFromXml)
            {
                b = GetNextPrintFromXml();
                if (!b)
                    _loadFromXml = false;
            }
            else if (_loadFromWeb)
            {
                b = GetNextPrintFromWeb();
                if (!b)
                    _loadFromWeb = false;
            }
            else
                WriteLine("error loadFromXml and loadFromWeb are false (FrboardPostRead)");
            return b;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public static void WriteLine(string msg, params object[] prm)
        {
            _tr.WriteLine(msg, prm);
        }
    }

    public class FrboardSearchPrint : IEnumerable<FrboardPrint>, IEnumerator<FrboardPrint>
    {
        private IEnumerator<FrboardPostHeader> _enumPost = null;
        private IEnumerator<FrboardPrint> _enumPrint = null;
        private bool _loadImage = false;

        public FrboardSearchPrint(string url = null, int maxPage = 1, bool loadImage = false)
        {
            _loadImage = loadImage;
            _enumPost = new LoadFrboardPostHeaderFromWeb(url, maxPage).GetEnumerator();
        }

        public void Dispose()
        {
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
            get { return _enumPrint.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _enumPrint.Current; }
        }

        public bool MoveNext()
        {
            while (true)
            {
                if (_enumPrint != null && _enumPrint.MoveNext())
                    return true;
                if (!_enumPost.MoveNext())
                    return false;
                _enumPrint = new FrboardPostRead(_enumPost.Current.url, _loadImage);
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using MongoDB.Bson;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Data.Xml.old;   // for zReadAttributeString()
using pb.IO;
using pb.Web;
using pb.Text;

//namespace Print.download
namespace Download.Print.TelechargementPlus
{
    public class TelechargementPlus_Base_v1
    {
        public string url = null;
        public DateTime? loadFromWebDate = null;
        public string title = null;
        public string author = null;
        public DateTime? creationDate = null;
        public string category = null;
        public string language = null;
        public string size = null;
        public int? nbPages = null;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        public List<string> description = new List<string>();

        public static bool trace = false;

        public void SetTextValues(IEnumerable<string> texts)
        {
            string name = null;
            string text = null;
            foreach (string s in texts)
            {
                // PDF | 116 pages | 53 Mb | French
                //Trace.CurrentTrace.WriteLine("SetTextValues : \"{0}\"", s);
                if (s == "\r\n")
                {
                    if (text != null)
                    {
                        if (name != null)
                            infos.SetValue(name, new ZString(text));
                        else
                            description.Add(text);
                        text = null;
                    }
                    name = null;
                }
                else
                {
                    string s2 = TelechargementPlus_v1.TrimString(TelechargementPlus_v1.ExtractTextValues(infos, s));
                    if (infos.ContainsKey("language"))
                    {
                        language = (string)infos["language"];
                        infos.Remove("language");
                    }
                    else if (infos.ContainsKey("size"))
                    {
                        size = (string)infos["size"];
                        infos.Remove("size");
                    }
                    else if (infos.ContainsKey("page_nb"))
                    {
                        nbPages = int.Parse((string)infos["page_nb"]);
                        infos.Remove("page_nb");
                    }
                    WriteLine("text \"{0}\" => \"{1}\"", s, s2);
                    bool foundName = false;
                    if (s2.EndsWith(":"))
                    {
                        string s3 = s2.Substring(0, s2.Length - 1).Trim();
                        if (s3 != "")
                        {
                            name = s3;
                            foundName = true;
                        }
                    }
                    //else if (s2 != "" && s2 != title)
                    if (!foundName && s2 != "" && s2 != title)
                    {
                        if (text == null)
                            text = s2;
                        else
                            text += " " + s2;
                    }
                }
            }
            if (text != null)
            {
                if (name != null)
                    infos.SetValue(name, new ZString(text));
                else
                    description.Add(text);
            }
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            if (trace)
                Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class TelechargementPlus_PostHeader_v1 : TelechargementPlus_Base_v1
    {
        public string sourceUrl;
        //public string url;
        //public string title;
        //public DateTime? postDate = null;
        //public ImageHtml image = null;
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
        public string postAuthor;
        //public string editor;
        //public string author;
        //public string language;
        //public string category;
        //public NamedValue<string> tags = new NamedValue<string>();
        //public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        //public List<string> description = new List<string>();
    }

    public class TelechargementPlus_Post_v1 : TelechargementPlus_Base_v1
    {
        //public string url;

        public void DocumentXmlLoad(XmlReader xmlReader)
        {
            while (!xmlReader.EOF)
            {
                xmlReader.Read();
                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;
                if (!xmlReader.IsEmptyElement)
                    break;
                string s;
                DateTime dt;
                switch (xmlReader.Name)
                {
                    case "url":
                        url = xmlReader.zReadAttributeString();
                        break;
                    case "loadFromWebDate":
                        s = xmlReader.zReadAttributeString();
                        if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                            loadFromWebDate = dt;
                        else
                            WriteLine("error wrong loadFromWebDate date time value \"{0}\" in xml  (TelechargementPlus_Post)", s);
                        break;
                    case "title":
                        title = xmlReader.zReadAttributeString();
                        break;
                    case "creationDate":
                        s = xmlReader.zReadAttributeString();
                        if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                            creationDate = dt;
                        else
                            WriteLine("error wrong creationDate date time value \"{0}\" in xml  (TelechargementPlus_Post)", s);
                        break;
                    case "author":
                        author = xmlReader.zReadAttributeString();
                        break;
                    case "category":
                        category = xmlReader.zReadAttributeString();
                        break;
                    case "value":
                        string name = xmlReader.zReadAttributeString("name");
                        s = xmlReader.zReadAttributeString("value");
                        if (name != "")
                            infos.Add(name, new ZString(s));
                        else
                            WriteLine("error wrong infos element value has no name value \"{0}\" in xml  (TelechargementPlus_Post)", s);
                        break;
                    default:
                        WriteLine("error unknow element \"{0}\" in xml  (TelechargementPlus_Post)", xmlReader.Name);
                        break;
                }
            }
        }

        public void DocumentXmlSave(XmlWriter xw)
        {
            xw.zWriteElementWithAttributes("url", "value", url);
            xw.zWriteElementWithAttributes("loadFromWebDate", "value", loadFromWebDate != null ? ((DateTime)loadFromWebDate).ToString("yyyy-MM-dd HH:mm:ss") : "");
            xw.zWriteElementWithAttributes("title", "value", title);
            xw.zWriteElementWithAttributes("creationDate", "value", creationDate != null ? ((DateTime)creationDate).ToString("yyyy-MM-dd HH:mm:ss") : "");
            xw.zWriteElementWithAttributes("author", "value", author);
            xw.zWriteElementWithAttributes("category", "value", category);
            foreach (KeyValuePair<string, ZValue> value in infos)
                xw.zWriteElementWithAttributes("value", "name", value.Key, "value", (string)value.Value);
        }

        public void DocumentMongoLoad(BsonDocument doc)
        {
            url = doc.zGetString("url");
            loadFromWebDate = doc.zGetDateTime("loadFromWebDate");
            title = doc.zGetString("title");
            creationDate = doc.zGetDateTime("creationDate");
            author = doc.zGetString("author");
            category = doc.zGetString("category");
            foreach (BsonDocument doc2 in doc.zGetBsonArray("values"))
            {
                string name = doc2.zGetString("name");
                string value = doc2.zGetString("value");
                if (name != null)
                    infos.SetValue(name, new ZString(value));
            }
        }

        public void DocumentMongoSave(BsonDocument doc)
        {
            doc.zAdd("url", url);
            doc.zAdd("loadFromWebDate", loadFromWebDate);
            doc.zAdd("title", title);
            doc.zAdd("creationDate", creationDate);
            doc.zAdd("author", author);
            doc.zAdd("category", category);
            //BsonArray values = new BsonArray();
            BsonArray values = doc.zAddArray("values");
            foreach (KeyValuePair<string, ZValue> value in infos)
            {
                //BsonDocument valueDoc = new BsonDocument();
                BsonDocument valueDoc = values.zAddDocument();
                valueDoc.zAdd("name", value.Key);
                valueDoc.zAdd("value", (string)value.Value);
                //values.Add(valueDoc);
            }
            //doc.Add("values", values);
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class TelechargementPlus_Print_v1 : TelechargementPlus_Base_v1
    {
        public List<pb.old.ImageHtml> images = new List<pb.old.ImageHtml>();
        //public string[] downloadLinks = null;
        public List<string> downloadLinks = new List<string>();

        public void DocumentXmlLoad(XmlReader xmlReader, string fileDirectory = null)
        {
            while (!xmlReader.EOF)
            {
                xmlReader.Read();
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.IsEmptyElement)
                    {
                        string s;
                        switch (xmlReader.Name)
                        {
                            case "title":
                                title = xmlReader.zReadAttributeString();
                                break;
                            case "description":
                                description.Add(xmlReader.zReadAttributeString());
                                break;
                            case "language":
                                language = xmlReader.zReadAttributeString();
                                break;
                            case "size":
                                size = xmlReader.zReadAttributeString();
                                break;
                            case "nbPages":
                                s = xmlReader.zReadAttributeString();
                                if (s != "")
                                {
                                    int n;
                                    if (int.TryParse(s, out n))
                                        nbPages = n;
                                    else
                                        WriteLine("error wrong nbPages int value \"{0}\" in xml  (TelechargementPlus_Print)", s);
                                }
                                break;
                            case "value":
                                string name = xmlReader.zReadAttributeString("name");
                                s = xmlReader.zReadAttributeString("value");
                                if (name != "")
                                    infos.Add(name, new ZString(s));
                                else
                                    WriteLine("error wrong infos element value has no name value \"{0}\" in xml  (TelechargementPlus_Print)", s);
                                break;
                            case "image":
                                pb.old.ImageHtml image = new pb.old.ImageHtml();
                                //string file = xmlReader.zReadAttributeString("file");
                                image.File = xmlReader.zReadAttributeString("file");
                                //string source = xmlReader.zReadAttributeString("source");
                                image.Source = xmlReader.zReadAttributeString("source");
                                if (image.Source == "")
                                    WriteLine("error wrong image source has no value in xml  (TelechargementPlus_Print)");
                                //ImageHtml image = new ImageHtml(source, xmlReader.zReadAttributeString("alt"), xmlReader.zReadAttributeString("title"), xmlReader.zReadAttributeString("class"));
                                image.Alt = xmlReader.zReadAttributeString("alt");
                                image.Title = xmlReader.zReadAttributeString("title");
                                image.Class = xmlReader.zReadAttributeString("class");
                                if (fileDirectory != null)
                                {
                                    string path = Path.Combine(fileDirectory, image.File);
                                    if (File.Exists(path))
                                        image.Image = pb.old.Http_v2.LoadImageFromFile(path);
                                    else
                                        WriteLine("error wrong image file does'nt exist \"{0}\" in xml  (TelechargementPlus_Print)", path);
                                    images.Add(image);
                                }
                                break;
                            case "downloadLink":
                                downloadLinks.Add(xmlReader.zReadAttributeString());
                                break;
                            default:
                                WriteLine("error unknow element \"{0}\" in xml  (TelechargementPlus_Print)", xmlReader.Name);
                                break;
                        }
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement)
                    break;
            }
        }

        //public void DocumentXmlSave(XmlWriter xw, string xmlFile = null, string imageCacheDirectory = null)
        public void DocumentXmlSave(XmlWriter xw, string fileDirectory = null, string imageFile = null)
        {
            xw.zWriteElementWithAttributes("title", "value", title);
            foreach (string s in description)
                xw.zWriteElementWithAttributes("description", "value", s);
            xw.zWriteElementWithAttributes("language", "value", language);
            xw.zWriteElementWithAttributes("size", "value", size);
            xw.zWriteElementWithAttributes("nbPages", "value", nbPages.ToString());

            foreach (KeyValuePair<string, ZValue> value in infos)
                xw.zWriteElementWithAttributes("value", "name", value.Key, "value", (string)value.Value);

            //int imageIndex = 1;
            ////string imageFile = null;
            ////if (xmlFile != null && imageCacheDirectory != null)
            ////    imageFile = Path.Combine(Path.Combine(Path.GetDirectoryName(xmlFile), imageCacheDirectory), Path.GetFileNameWithoutExtension(xmlFile));
            //foreach (ImageHtml image in images)
            //{
            //    string file = null;
            //    if (imageFile != null)
            //    {
            //        //file = Path.Combine(imageCacheDirectory, zurl.UrlToFileName(image.Source, UrlFileNameType.FileName));
            //        // pb image.Source ne contient pas forcement http://
            //        //file = Path.Combine(imageCacheDirectory, title + "_" + imageIndex++.ToString() + zurl.GetUrlFileType(image.Source));
            //        //Http2.LoadToFile(image.Source, Path.Combine(xmlDirectory, file));
            //        file = imageFile + "_" + imageIndex++.ToString() + zurl.GetUrlFileType(image.Source);
            //        Http2.LoadToFile(image.Source, file);
            //    }
            //    xw.zWriteElementWithAttributes("image", "file", file, "source", image.Source, "title", image.Title, "class", image.Class, "alt", image.Alt);
            //}

            SaveImages(fileDirectory, imageFile);

            foreach (pb.old.ImageHtml image in images)
            {
                xw.zWriteElementWithAttributes("image", "file", image.File, "source", image.Source, "title", image.Title, "class", image.Class, "alt", image.Alt);
            }

            foreach (string downloadLink in downloadLinks)
            {
                xw.zWriteElementWithAttributes("downloadLink", "value", downloadLink);
            }
        }

        public void DocumentMongoLoad(BsonDocument doc, string fileDirectory = null)
        {
            title = doc.zGetString("title");
            description = doc.zGetStringList("description");
            language = doc.zGetString("language");
            size = doc.zGetString("size");
            nbPages = doc.zGetInt("nbPages");

            foreach (BsonDocument doc2 in doc.zGetBsonArray("values"))
            {
                string name = doc2.zGetString("name");
                string value = doc2.zGetString("value");
                if (name != null)
                    infos.SetValue(name, new ZString(value));
            }

            foreach (BsonDocument doc2 in doc.zGetBsonArray("images"))
            {
                pb.old.ImageHtml image = new pb.old.ImageHtml();
                images.Add(image);
                image.File = doc2.zGetString("file");
                image.Source = doc2.zGetString("source");
                image.Title = doc2.zGetString("title");
                image.Class = doc2.zGetString("class");
                image.Alt = doc2.zGetString("alt");

                if (image.Source == "")
                    WriteLine("error wrong image source has no value in mongo db  (TelechargementPlus_Print)");
                //ImageHtml image = new ImageHtml(source, xmlReader.zReadAttributeString("alt"), xmlReader.zReadAttributeString("title"), xmlReader.zReadAttributeString("class"));
                if (fileDirectory != null)
                {
                    string path = Path.Combine(fileDirectory, image.File);
                    if (File.Exists(path))
                        image.Image = pb.old.Http_v2.LoadImageFromFile(path);
                    else
                        WriteLine("error wrong image file does'nt exist \"{0}\" in xml  (TelechargementPlus_Print)", path);
                    //images.Add(image);
                }
            }

            downloadLinks = doc.zGetStringList("downloadLinks");
        }

        //public void DocumentMongoSave(BsonDocument doc, string imageFile = null)
        public BsonDocument GetDocumentMongo(string fileDirectory = null, string imageFile = null)
        {
            BsonDocument doc = new BsonDocument();
            doc.zAdd("title", title);
            doc.zAdd("description", description);
            doc.zAdd("language", language);
            doc.zAdd("size", size);
            doc.zAdd("nbPages", nbPages);

            //BsonArray values = new BsonArray();
            BsonArray values = doc.zAddArray("values");
            foreach (KeyValuePair<string, ZValue> value in infos)
            {
                //BsonDocument valueDoc = new BsonDocument();
                BsonDocument valueDoc = values.zAddDocument();
                valueDoc.zAdd("name", value.Key);
                valueDoc.zAdd("value", (string)value.Value);
                //values.Add(valueDoc);
            }
            //doc.Add("values", values);

            SaveImages(fileDirectory, imageFile);

            //BsonArray imageArray = new BsonArray();
            BsonArray imageArray = doc.zAddArray("images");
            foreach (pb.old.ImageHtml image in images)
            {
                //BsonDocument imageDoc = new BsonDocument();
                BsonDocument imageDoc = imageArray.zAddDocument();
                imageDoc.zAdd("file", image.File);
                imageDoc.zAdd("source", image.Source);
                imageDoc.zAdd("title", image.Title);
                imageDoc.zAdd("class", image.Class);
                imageDoc.zAdd("alt", image.Alt);
                //imageArray.Add(imageDoc);
            }
            //doc.Add("images", imageArray);

            doc.zAdd("downloadLinks", downloadLinks);
            return doc;
        }

        private void SaveImages(string fileDirectory, string imageFile)
        {
            if (fileDirectory == null || imageFile == null)
                return;

            zfile.DeleteFiles(Path.Combine(fileDirectory, imageFile + "*.*"));

            int imageIndex = 1;
            foreach (pb.old.ImageHtml image in images)
            {
                image.File = imageFile + "_" + imageIndex++.ToString() + zurl.GetUrlFileType(image.Source);
                pb.old.Http_v2.LoadToFile(image.Source, Path.Combine(fileDirectory, image.File));
            }
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class TelechargementPlus_LoadPostHeaderFromWeb_v1 : LoadListFromWebBase_v1<TelechargementPlus_PostHeader_v1>
    {
        protected bool _loadImage = false;
        protected XXElement _xelement = null;
        protected string _urlNextPage = null;
        protected IEnumerator<XXElement> _xmlEnum = null;
        protected TelechargementPlus_PostHeader_v1 _postHeader = null;
        protected static string _urlPostHeader = "http://www.telechargement-plus.com/e-book-magazines/";

        public TelechargementPlus_LoadPostHeaderFromWeb_v1(int startPage = 1, int maxPage = 1, bool loadImage = false)
        {
            string url = _urlPostHeader;
            if (startPage != 1)
                url += "page/" + (string)startPage.ToString() + "/";
            _url = url;
            _maxPage = maxPage;
            _loadImage = loadImage;
        }

        public override TelechargementPlus_PostHeader_v1 Current { get { return _postHeader; } }

        protected override void SetXml(XElement xelement)
        {
            _xelement = new XXElement(xelement);
            //_sourceUrl = sourceUrl;
            InitXml();
        }

        protected void InitXml()
        {
            // post list :
            //   <div class="base shortstory">
            //   _hxr.ReadSelect("//div[@class='base shortstory']:.:EmptyRow", ".//text()");
            // next page :
            //   <div class="navigation">
            //     <div align="center">
            //       <span>Prev.</span> 
            //       <span>1</span> 
            //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">2</a> 
            //       ...
            //       <a href="http://www.telechargement-plus.com/e-book-magazines/page/2/">Next</a>
            //     </div>
            //   </div>
            //   _hxr.ReadSelect("//div[@class='navigation']//a[text()='Next']:.:EmptyRow", "text()", "@href");
            _urlNextPage = GetUrl(_xelement.XPathValue("//div[@class='navigation']//a[text()='Next']/@href"));
            _xmlEnum = _xelement.XPathElements("//div[@class='base shortstory']").GetEnumerator();
        }

        protected override string GetUrlNextPage()
        {
            return _urlNextPage;
        }

        protected override bool _MoveNext()
        {
            while (_xmlEnum.MoveNext())
            {
                XXElement xePost = _xmlEnum.Current;
                _postHeader = new TelechargementPlus_PostHeader_v1();
                //_postHeader.sourceUrl = _sourceUrl;
                _postHeader.sourceUrl = _url;
                _postHeader.loadFromWebDate = DateTime.Now;

                //<h1 class="shd">
                //    <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/86236-multi-ici-paris-n3562-9-au-15-octobre-2013.html">
                //        [Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013
                //    </a>
                //</h1>
                XXElement xe = xePost.XPathElement(".//*[@class='shd']//a");
                _postHeader.url = GetUrl(xe.XPathValue("@href"));
                //_postHeader.title = TrimString(ExtractTextValues(xe.XPathValue(".//text()")));
                _postHeader.title = TelechargementPlus_v1.TrimString(TelechargementPlus_v1.ExtractTextValues(_postHeader.infos, xe.XPathValue(".//text()")));

                //<div class="shdinf">
                //    <div class="shdinf">
                //      <span class="rcol">Auteur: 
                //          <a onclick="ShowProfile('bakafa', 'http://www.telechargement-plus.com/user/bakafa/', '0'); return false;" href="http://www.telechargement-plus.com/user/bakafa/">
                //              bakafa
                //          </a>
                //      </span> 
                //      <span class="date">
                //          <b><a href="http://www.telechargement-plus.com/2013/10/09/">Aujourd'hui, 17:13</a></b>
                //      </span>
                //      <span class="lcol">Cat&eacute;gorie: 
                //          <a href="http://www.telechargement-plus.com/e-book-magazines/">
                //              E-Book / Magazines
                //          </a> &raquo; 
                //          <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">
                //              Magazines
                //          </a>
                //      </span>
                //    </div>
                //</div>
                xe = xePost.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
                _postHeader.postAuthor = xe.XPathValue(".//span[@class='rcol']//a//text()");
                //string postDate = xe.XPathValue(".//span[@class='date']//text()");
                // Aujourd'hui, 17:13
                //if (postDate != null)
                //    _postHeader.infos.SetValue("postDate", new ZString(postDate));
                _postHeader.creationDate = TelechargementPlus_v1.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"));
                //_postHeader.category = xe.DescendantTextList(".//span[@class='lcol']").Select(s => TelechargementPlus_old1.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                _postHeader.category = xe.XPathElements(".//span[@class='lcol']").DescendantTexts().Select(s => TelechargementPlus_v1.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
                //Trace.CurrentTrace.WriteLine("post header category \"{0}\"", _postHeader.category);
                //.zForEach(s => s.Trim())

                //<span id="post-img">
                //    <div id="news-id-86236" style="display: inline;">
                //        <div style="text-align: center;">
                //            <!--dle_image_begin:http://zupimages.net/up/3/1515486591.jpeg|-->
                //            <img src="http://zupimages.net/up/3/1515486591.jpeg" alt="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013"
                //                title="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013" /><!--dle_image_end-->
                //            <br />
                //            <b>
                //                <br />
                //                Ici Paris N°3562 - 9 au 15 Octobre 2013<br />
                //                French | 52 pages | HQ PDF | 101 MB
                //            </b>
                //            <br />
                //            <br />
                //            Ici Paris vous fait partager la vie publique et privée de celles et ceux qui font
                //            l'actualité : exclusivités, interviews, enquêtes (la face cachée du showbiz, les
                //            coulisses de la télé) indiscrétions, potins.<br />
                //        </div>
                //    </div>
                //</span>
                xe = xePost.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
                //_postHeader.images = xe.XPathImages(".//img", _url, TelechargementPlus.ImagesToSkip);
                //_postHeader.images = xe.XPathImages(_url, TelechargementPlus_old1.ImagesToSkip);
                //_postHeader.images = xe.XPathImages(_url, imageHtml => !TelechargementPlus_old1.ImagesToSkip.ContainsKey(imageHtml.Source));
                //_postHeader.images = xe.XPathImages(xeImg => new ImageHtml(xeImg, _url), imageHtml => !TelechargementPlus_old1.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();
                _postHeader.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, _url)).Where(imageHtml => !TelechargementPlus_v1.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();

                if (_loadImage)
                    pb.old.Http_v2.LoadImageFromWeb(_postHeader.images);

                //_postHeader.SetTextValues(xe.DescendantTextList());
                _postHeader.SetTextValues(xe.DescendantTexts());

                //string name = null;
                //string text = null;
                //foreach (string s in xe.DescendantTextList())
                //{
                //    // PDF | 116 pages | 53 Mb | French
                //    if (s == "\r\n")
                //    {
                //        if (text != null)
                //        {
                //            if (name != null)
                //                _postHeader.infos.SetValue(name, new ZString(text));
                //            else
                //                _postHeader.description.Add(text);
                //            text = null;
                //        }
                //        name = null;
                //    }
                //    else
                //    {
                //        //string s2 = TrimString(ExtractTextValues(s));
                //        string s2 = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(_postHeader.infos, s));
                //        WriteLine("text \"{0}\" => \"{1}\"", s, s2);
                //        if (s2.EndsWith(":"))
                //            name = s2.Substring(0, s2.Length - 1).Trim();
                //        //else if (name != null)
                //        //{
                //        //    _postHeader.infos.SetValue(name, new ZString(s2));
                //        //    name = null;
                //        //}
                //        else if (s2 != "" && s2 != _postHeader.title)
                //        {
                //            //_postHeader.description.Add(s2);
                //            if (text == null)
                //                text = s2;
                //            else
                //                text += " " + s2;
                //        }
                //    }
                //}
                //if (text != null)
                //{
                //    if (name != null)
                //        _postHeader.infos.SetValue(name, new ZString(text));
                //    else
                //        _postHeader.description.Add(text);
                //}

                return true;
            }
            return false;
        }
    }

    public class TelechargementPlus_LoadPostFromWeb_v1 : LoadListFromWebBase_v1<TelechargementPlus_Print_v1>
    {
        private bool _loadImage = false;
        private XXElement _xeSource = null;
        private XXElement _xePost = null;
        private TelechargementPlus_Post_v1 _post = null;
        private TelechargementPlus_Print_v1 _print = null;

        public TelechargementPlus_LoadPostFromWeb_v1(string url, string urlFile = null, bool reload = false, bool loadImage = false)
        {
            _url = url;
            _urlFile = urlFile;
            _reload = reload;
            _loadImage = loadImage;
        }

        public TelechargementPlus_Post_v1 Post { get { Init(); return _post; } }
        public override TelechargementPlus_Print_v1 Current { get { return _print; } }

        protected override void SetXml(XElement xelement)
        {
            _xeSource = new XXElement(xelement);
            InitXml();
        }

        protected void InitXml()
        {
            // <div id='dle-content'>
            _post = new TelechargementPlus_Post_v1();
            _post.url = _url;
            _post.loadFromWebDate = DateTime.Now;
            _xePost = _xeSource.XPathElement("//div[@id='dle-content']");
            XXElement xe = _xePost.XPathElement(".//div[@class='heading']//div[@class='binner']");
            _post.title = TelechargementPlus_v1.TrimString(TelechargementPlus_v1.ExtractTextValues(_post.infos, xe.XPathValue(".//text()")));
            _post.creationDate = TelechargementPlus_v1.ParseDateTime(xe.XPathValue(".//a//text()"));
            //_post.category = xe.DescendantTextList(".//div[@class='storeinfo']").Skip(2).Select(s => TelechargementPlus_old1.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
            _post.category = xe.XPathElements(".//div[@class='storeinfo']").DescendantTexts().Skip(2).Select(s => TelechargementPlus_v1.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
            //Trace.CurrentTrace.WriteLine("post category \"{0}\"", _post.category);
        }

        protected override bool _MoveNext()
        {
            if (_xePost == null)
                return false;
            _print = new TelechargementPlus_Print_v1();
            _print.url = _url;
            _print.loadFromWebDate = DateTime.Now;
            _print.infos.SetValues(_post.infos);


            //<div class="base">
            //    <div class="heading">
            //        <div class="binner">
            //            <h1>
            //                Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct] Gratuit</h1>
            //            <div class="storeinfo">
            //                <a href="http://www.telechargement-plus.com/2013/10/14/">Aujourd'hui, 11:59</a>
            //                | Cat&eacute;gorie: 
            //                <a href="http://www.telechargement-plus.com/e-book-magazines/">E-Book / Magazines</a>, 
            //                <a href="http://www.telechargement-plus.com/e-book-magazines/journaux/">Journaux</a>, 
            //                <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">Magazines</a>
            //                <!-- | Views: 16-->
            //            </div>
            //        </div>
            //    </div>
            //    <div class="maincont">
            //        <div class="binner">
            //            <div class="shortstory">
            //                <div class="story-text">
            //                    <center>
            //                        <span id="post-img">
            //                            <img src="/templates/film-gratuit/images/prez/livre.png" alt="E-Book / Magazines, Journaux, Magazines" />
            //                        </span>
            //                    </center>
            //                    <span id="post-img">
            //                        <div style="text-align: center;">
            //                            <br />
            //                            <!--dle_image_begin:http://www.hapshack.com/images/TX72Y.jpg|-->
            //                            <img src="http://www.hapshack.com/images/TX72Y.jpg" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                            <br />
            //                            <br />
            //                            <b>Editeur :</b> Presse Fr<br />
            //                            <b>Date de sortie :</b> 2013
            //                            <br />
            //                            <b>H�bergeur : </b>Multi / 
            //                            <b>
            //                                <!--colorstart:#FF0000-->
            //                                <span style="color: #FF0000">
            //                                    <!--/colorstart-->
            //                                    [Link Direct]<!--colorend-->
            //                                </span><!--/colorend-->
            //                            </b>
            //                            <br />
            //                            <br />
            //                            <!--dle_image_begin:http://prezup.eu/prez/infossurlebook.png|-->
            //                            <img src="http://prezup.eu/prez/infossurlebook.png" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                <br />
            //                            <br />
            //                            <b>Advanced Cr�ation Photoshop HS�rie N�19 - Novembre 2013 [Lien Direct]</b>
            //                            <br />
            //                            PDF | French | 186 pages | 100 MB<br />
            //                            <br />
            //                            <b>Le CD | zipper/22 Fichiers &+ | 520 MB</b><br />
            //                            37 Projets complets<br />
            //                            SAVOIR TOUT FAIRE : Avec Photoshop Volume XIII<br />
            //                            SPECIAL PHOTOMONTAGE & PEINTURE NUMERIQUE<br />
            //                            BONUS : 2 Tutoriels Illustrator<br />
            //                            / / /
            //                            <br />
            //                            <br />
            //                        </div>
            //                    </span>
            //                    <span id="post-img">
            //                        <div id="news-id-86887" style="display: inline;">
            //                            *<br />
            //                            *<br />
            //                            *<br />
            //                            <div style="text-align: center;">
            //                                <b>
            //                                    <!--sizestart:6-->
            //                                    <span style="font-size: 24pt;">
            //                                        <!--/sizestart-->
            //                                        <!--colorstart:#FF6600-->
            //                                        <span style="color: #FF6600">
            //                                            <!--/colorstart-->
            //                                            Cloudzer<!--colorend-->
            //                                        </span><!--/colorend--><!--sizeend-->
            //                                    </span><!--/sizeend-->
            //                                    =
            //                                    <!--colorstart:#FF0000-->
            //                                    <span style="color: #FF0000">
            //                                        <!--/colorstart-->
            //                                        [Link Direct]<!--colorend-->
            //                                    </span><!--/colorend-->
            //                                </b>
            //                                <br />
            //                                <br />
            //                                <a href="http://clz.to/q83zrwga" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/0THnp.gif|-->
            //                                    <img src="http://www.hapshack.com/images/0THnp.gif" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <a href="http://ul.to/ukqruco3" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/9MfYk.gif|-->
            //                                    <img src="http://www.hapshack.com/images/9MfYk.gif" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <br />
            //                                <a href="http://hulkfile.eu/gap3aafrlmaj.html" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/Js84x.jpg|-->
            //                                    <img src="http://www.hapshack.com/images/Js84x.jpg" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <br />
            //                                <a href="http://turbobit.net/blki3znuvzeg.html" target="_blank">
            //                                    <!--dle_image_begin:http://www.hapshack.com/images/QYeW0.gif|-->
            //                                    <img src="http://www.hapshack.com/images/QYeW0.gif" alt="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]"
            //                                        title="Advanced Cr�ation Photoshop H-S�rie N�19 - Novembre 2013 [Lien Direct]" /><!--dle_image_end-->
            //                                </a>
            //                                <br />
            //                                <br />
            //                                *<br />
            //                                *<br />
            //                                <b>Le CD &+ : </b>
            //                                <br />
            //                                http://clz.to/o58urag6<br />
            //                                http://ul.to/rpqjypm4<br />
            //                                http://hulkfile.eu/i2k3bbz835zg.html<br />
            //                                http://turbobit.net/v644k3dd8izl.html<br />
            //                                <br />
            //                                <br />
            //                                Bonne lecture<br />
            //                                *************
            //                            </div>
            //                        </div>
            //                    </span>

            //XXElement xe = _xePost.XPathElement(".//div[@class='heading']//div[@class='binner']");
            //_post.title = _print.title = TelechargementPlus.TrimString(TelechargementPlus.ExtractTextValues(_print.infos, xe.XPathValue(".//text()")));
            //string postDate = xe.XPathValue(".//a//text()");
            ////WriteLine("postDate : \"{0}\"", postDate);
            //// Aujourd'hui, 17:13
            ////if (postDate != null)
            ////    _print.infos.SetValue("postDate", new ZString(postDate));
            ////_print.creationDate = FrboardPrint.GetDateTime(date.Trim(_trimAll), time.Trim(_trimAll));
            //_post.creationDate = TelechargementPlus.ParseDateTime(postDate);
            //_print.category = xe.DescendantTextList(".//div[@class='storeinfo']").Skip(2).Select(s => TelechargementPlus.TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");

            _print.title = _post.title;
            _print.category = _post.category;

            //xe = xe.XPathElement(".//span[@id='post-img']");
            //xe = _xePost.XPathElement(".//div[@class='maincont']//div[@class='binner']//span[@id='post-img']");
            XXElement xe = _xePost.XPathElement(".//div[@class='maincont']//div[@class='binner']//div[@class='story-text']");
            //_print.images = xe.XPathImages(".//span[@id='post-img']//img", _url, TelechargementPlus.ImagesToSkip);
            //_print.images = xe.XPathImages(".//span[@id='post-img']", _url, TelechargementPlus.ImagesToSkip, node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //_print.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(_url, TelechargementPlus_old1.ImagesToSkip, node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //_print.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(_url, imageHtml => !TelechargementPlus_old1.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? false : true);
            //_print.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(xeImg => new ImageHtml(xeImg, _url), imageHtml => !TelechargementPlus_old1.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? false : true).ToList();
            //_print.images = xe.XPathElements(".//span[@id='post-img']").XPathImages(xeImg => new ImageHtml(xeImg, _url), imageHtml => !TelechargementPlus_old1.ImagesToSkip.ContainsKey(imageHtml.Source),
            //    node => node is XElement && ((XElement)node).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode).ToList();
            _print.images = xe.XPathElements(".//span[@id='post-img']")
                .DescendantNodes(node => XmlDescendant.ImageFilter(node, node2 => node2 is XElement && ((XElement)node2).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode))
                .Select(xeImg => new pb.old.ImageHtml((XElement)xeImg, _url))
                .Where(imageHtml => !TelechargementPlus_v1.ImagesToSkip.ContainsKey(imageHtml.Source)).ToList();

            if (_loadImage)
                pb.old.Http_v2.LoadImageFromWeb(_print.images);

            //_print.SetTextValues(xe.DescendantTextList(".//span[@id='post-img']"));
            //_print.SetTextValues(xe.DescendantTextList(".//span[@id='post-img']", node => node is XElement && ((XElement)node).Name == "a" ? false : true));
            _print.SetTextValues(xe.XPathElements(".//span[@id='post-img']").DescendantTexts(node => node is XElement && ((XElement)node).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode));

            _print.downloadLinks.AddRange(xe.XPathValues(".//span[@id='post-img']//a/@href"));

            ////<h1 class="shd">
            ////    <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/86236-multi-ici-paris-n3562-9-au-15-octobre-2013.html">
            ////        [Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013
            ////    </a>
            ////</h1>
            //XXElement xe = xePost.XPathElement(".//*[@class='shd']//a");
            //_print.url = xe.XPathValue("@href");
            //_print.title = TrimString(ExtractTextValues(xe.XPathValue(".//text()")));

            ////<div class="shdinf">
            ////    <div class="shdinf">
            ////      <span class="rcol">Auteur: 
            ////          <a onclick="ShowProfile('bakafa', 'http://www.telechargement-plus.com/user/bakafa/', '0'); return false;" href="http://www.telechargement-plus.com/user/bakafa/">
            ////              bakafa
            ////          </a>
            ////      </span> 
            ////      <span class="date">
            ////          <b><a href="http://www.telechargement-plus.com/2013/10/09/">Aujourd'hui, 17:13</a></b>
            ////      </span>
            ////      <span class="lcol">Cat&eacute;gorie: 
            ////          <a href="http://www.telechargement-plus.com/e-book-magazines/">
            ////              E-Book / Magazines
            ////          </a> &raquo; 
            ////          <a href="http://www.telechargement-plus.com/e-book-magazines/magazines/">
            ////              Magazines
            ////          </a>
            ////      </span>
            ////    </div>
            ////</div>
            //xe = xePost.XPathElement(".//div[@class='shdinf']/div[@class='shdinf']");
            //_print.postAuthor = xe.XPathValue(".//span[@class='rcol']//a//text()");
            //string postDate = xe.XPathValue(".//span[@class='date']//text()");
            //// Aujourd'hui, 17:13
            //if (postDate != null)
            //    _print.infos.SetValue("postDate", new ZString(postDate));
            //_print.category = xe.DescendantTextList(".//span[@class='lcol']").Select(s => TrimString(s)).Where(s => s != "E-Book / Magazines" && s != "Catégorie:" && s != "").zToStringValues("/");
            ////.zForEach(s => s.Trim())

            ////<span id="post-img">
            ////    <div id="news-id-86236" style="display: inline;">
            ////        <div style="text-align: center;">
            ////            <!--dle_image_begin:http://zupimages.net/up/3/1515486591.jpeg|-->
            ////            <img src="http://zupimages.net/up/3/1515486591.jpeg" alt="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013"
            ////                title="[Multi] Ici Paris N°3562 - 9 au 15 Octobre 2013" /><!--dle_image_end-->
            ////            <br />
            ////            <b>
            ////                <br />
            ////                Ici Paris N°3562 - 9 au 15 Octobre 2013<br />
            ////                French | 52 pages | HQ PDF | 101 MB
            ////            </b>
            ////            <br />
            ////            <br />
            ////            Ici Paris vous fait partager la vie publique et privée de celles et ceux qui font
            ////            l'actualité : exclusivités, interviews, enquêtes (la face cachée du showbiz, les
            ////            coulisses de la télé) indiscrétions, potins.<br />
            ////        </div>
            ////    </div>
            ////</span>
            //xe = xePost.XPathElement(".//span[@id='post-img']//div[starts-with(@id, 'news-id')]");
            //_print.images = xe.XPathImages(".//img", _imagesToSkip);
            //if (_loadImage)
            //    Http2.LoadImageFromWeb(_print.images);

            //string name = null;
            //string text = null;
            //foreach (string s in xe.DescendantTextList())
            //{
            //    // PDF | 116 pages | 53 Mb | French
            //    if (s == "\r\n")
            //    {
            //        if (text != null)
            //        {
            //            if (name != null)
            //                _print.infos.SetValue(name, new ZString(text));
            //            else
            //                _print.description.Add(text);
            //            text = null;
            //        }
            //        name = null;
            //    }
            //    else
            //    {
            //        //string s2 = TrimString(GetStringTag(s));
            //        string s2 = TrimString(ExtractTextValues(s));
            //        WriteLine("text \"{0}\" => \"{1}\"", s, s2);
            //        if (s2.EndsWith(":"))
            //            name = s2.Substring(0, s2.Length - 1).Trim();
            //        //else if (name != null)
            //        //{
            //        //    _print.infos.SetValue(name, new ZString(s2));
            //        //    name = null;
            //        //}
            //        else if (s2 != "" && s2 != _print.title)
            //        {
            //            //_print.description.Add(s2);
            //            if (text == null)
            //                text = s2;
            //            else
            //                text += " " + s2;
            //        }
            //    }
            //}
            //if (text != null)
            //{
            //    if (name != null)
            //        _print.infos.SetValue(name, new ZString(text));
            //    else
            //        _print.description.Add(text);
            //}

            _xePost = null;
            return true;
        }

        protected static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class TelechargementPlus_LoadPostFromXml_v1 : pb.old.LoadFromXml<TelechargementPlus_Print_v1>
    {
        protected TelechargementPlus_Post_v1 _post = null;
        protected TelechargementPlus_Print_v1 _currentPrint = null;

        public TelechargementPlus_LoadPostFromXml_v1(string file, bool loadImage = false)
            : base(file, loadImage)
        {
        }

        public TelechargementPlus_Post_v1 Post { get { return _post; } }
        public override TelechargementPlus_Print_v1 Current { get { return _currentPrint; } }

        protected override void _Load()
        {
            _post = new TelechargementPlus_Post_v1();
            _post.DocumentXmlLoad(_xmlReader);
            if (_xmlReader.NodeType == XmlNodeType.Element)
            {
                if (_xmlReader.IsEmptyElement || _xmlReader.Name != "print")
                    WriteLine("error unknow element \"{0}\" in xml  (TelechargementPlus_LoadPostFromXml)", _xmlReader.Name);
            }
        }

        protected override bool _MoveNext()
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
                        _currentPrint.DocumentXmlLoad(_xmlReader, fileDirectory);
                        return true;
                    }
                    else
                        WriteLine("error unknow xml element \"{0}\"  (TelechargementPlus_LoadPostFromXml)", _xmlReader.Name);
                }
                _xmlReader.Read();
            }
            return false;
        }

        private void NewPrint()
        {
            _currentPrint = new TelechargementPlus_Print_v1();
            _currentPrint.url = _post.url;
            _currentPrint.loadFromWebDate = _post.loadFromWebDate;
            _currentPrint.author = _post.author;
            _currentPrint.creationDate = _post.creationDate;
            _currentPrint.category = _post.category;
        }
    }

    public class TelechargementPlus_LoadPost_v1 : pb.old.HttpLoad, IDisposable
    {
        protected TelechargementPlus_Post_v1 _post = null;
        protected IEnumerable<TelechargementPlus_Print_v1> _prints = null;

        protected static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        protected static bool __useUrlCache = false;
        protected static string __cacheDirectory = null;
        protected static string __imageCacheDirectory = "image";
        protected static bool __documentXml = false;
        protected static bool __documentMongoDb = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
            __imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            __documentXml = xe.zXPathValue("DocumentXml").zTryParseAs(__documentXml);
            __documentMongoDb = xe.zXPathValue("DocumentMongoDb").zTryParseAs(__documentMongoDb);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        public TelechargementPlus_LoadPost_v1(string url)
            : base(url)
        {
            _useUrlCache = __useUrlCache;
            _cacheDirectory = __cacheDirectory;
            _imageCacheDirectory = __imageCacheDirectory;
            _documentXml = __documentXml;
            _documentMongoDb = __documentMongoDb;
            _mongoServer = __mongoServer;
            _mongoDatabase = __mongoDatabase;
            _mongoCollectionName = __mongoCollectionName;
        }

        public void Dispose()
        {
        }

        public TelechargementPlus_Post_v1 Post { get { return _post; } }
        public IEnumerable<TelechargementPlus_Print_v1> Prints { get { return _prints; } }

        public static bool UseUrlCache { get { return __useUrlCache; } set { __useUrlCache = value; } }
        public static string CacheDirectory { get { return __cacheDirectory; } set { __cacheDirectory = value; } }
        public static string ImageCacheDirectory { get { return __imageCacheDirectory; } set { __imageCacheDirectory = value; } }
        public static bool DocumentXml { get { return __documentXml; } set { __documentXml = value; } }
        public static bool DocumentMongoDb { get { return __documentMongoDb; } set { __documentMongoDb = value; } }
        public static string MongoServer { get { return __mongoServer; } set { __mongoServer = value; } }
        public static string MongoDatabase { get { return __mongoDatabase; } set { __mongoDatabase = value; } }
        public static string MongoCollectionName { get { return __mongoCollectionName; } set { __mongoCollectionName = value; } }

        public override string GetName()
        {
            return "post";
        }

        public override object _GetDocumentKey()
        {
            // http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html
            Uri uri = new Uri(_url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", _url);
            return int.Parse(match.Value);
        }

        public override string _GetCacheFileDirectory()
        {
            return Path.Combine(_cacheDirectory, string.Format("{0:000000}", (int)GetDocumentKey() / 1000));
        }

        protected override void _LoadDocumentFromWeb(string file = null, bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_LoadPostFromWeb_v1 loadFromWeb = new TelechargementPlus_LoadPostFromWeb_v1(_url, file, reload, loadImage);
            _post = loadFromWeb.Post;
            _prints = loadFromWeb;
        }

        protected override void _LoadDocumentFromXml(string file, bool loadImage = false)
        {
            TelechargementPlus_LoadPostFromXml_v1 loadFromXml = new TelechargementPlus_LoadPostFromXml_v1(file, loadImage);
            _post = loadFromXml.Post;
            _prints = loadFromXml;
        }

        protected override void _SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        {
            //string imageCacheDirectory = null;
            //if (saveImage)
            //    imageCacheDirectory = _imageCacheDirectory;
            string imageFile = null;
            string fileDirectory = null;
            if (saveImage)
            {
                fileDirectory = GetCacheFileDirectory();
                imageFile = GetImageFile();
            }

            _post.DocumentXmlSave(xw);
            //string imageFile = null;
            //if (_xmlFile != null && _imageCacheDirectory != null)
            //    imageFile = Path.Combine(Path.Combine(Path.GetDirectoryName(_xmlFile), imageCacheDirectory), Path.GetFileNameWithoutExtension(_xmlFile));
            foreach (TelechargementPlus_Print_v1 print in _prints)
            {
                xw.WriteStartElement("print");
                //print.DocumentXmlSave(xw, _xmlFile, imageCacheDirectory);
                print.DocumentXmlSave(xw, fileDirectory, imageFile);
                xw.WriteEndElement();
            }
        }

        protected override void _LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        {
            //Trace.CurrentTrace.WriteLine("_DocumentMongoLoad loadImage {0}", loadImage);
            _post = new TelechargementPlus_Post_v1();
            _post.DocumentMongoLoad(doc);
            _prints = _DocumentMongoLoadPrints(doc, loadImage);
        }

        protected IEnumerable<TelechargementPlus_Print_v1> _DocumentMongoLoadPrints(BsonDocument doc, bool loadImage = false)
        {
            string fileDirectory = null;
            if (loadImage)
                fileDirectory = GetCacheFileDirectory();

            foreach (BsonDocument doc2 in (BsonArray)doc["prints"])
            {
                TelechargementPlus_Print_v1 print = new TelechargementPlus_Print_v1();
                print.url = _post.url;
                print.loadFromWebDate = _post.loadFromWebDate;
                print.author = _post.author;
                print.creationDate = _post.creationDate;
                print.category = _post.category;
                print.DocumentMongoLoad(doc2, fileDirectory);
                yield return print;
            }
        }

        //public void DocumentMongoSave(BsonDocument doc, string imageCacheDirectory = null)
        protected override void _SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        {
            string imageFile = null;
            string fileDirectory = null;
            if (saveImage)
            {
                fileDirectory = GetCacheFileDirectory();
                imageFile = GetImageFile();
            }
            _post.DocumentMongoSave(doc);
            BsonArray prints = doc.zAddArray("prints");
            //BsonArray prints = new BsonArray();
            foreach (TelechargementPlus_Print_v1 print in _prints)
            {
                //BsonDocument doc2 = new BsonDocument();
                //print.DocumentMongoSave(doc2, imageFile);
                //prints.Add(doc2);
                prints.Add(print.GetDocumentMongo(fileDirectory, imageFile));
                
            }
            //doc.Add("prints", prints);
        }

        public static IEnumerable<TelechargementPlus_Print_v1> GetPrints(string url, bool reload = false, bool loadImage = false)
        {
            TelechargementPlus_LoadPost_v1 loadPost = new TelechargementPlus_LoadPost_v1(url);
            loadPost.Load(reload: reload, loadImage: loadImage);
            return loadPost.Prints;
        }
    }

    public static class TelechargementPlus_v1
    {
        private static RegexValuesList _textInfoRegexList = null;
        private static char[] _trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=' };
        private static Dictionary<string, string> _imagesToSkip = null;

        //static TelechargementPlus()
        //{
        //    InitImagesToSkip();
        //}

        public static Dictionary<string, string> ImagesToSkip { get { return _imagesToSkip; } }

        public static void ClassInit(XElement xe)
        {
            TelechargementPlus_LoadPost_v1.ClassInit(xe);
            _textInfoRegexList = new RegexValuesList(xe.XPathSelectElements("TextInfo"));
            InitImagesToSkip(xe);
        }

        public static string ExtractTextValues(NamedValues<ZValue> infos, string s)
        {
            if (s.Contains('|'))
            {
                //Trace.CurrentTrace.WriteLine("info \"{0}\"", s);
                foreach (string s2 in zsplit.Split(s, '|', true))
                {
                    string s3 = s2;
                    NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s3);
                    infos.SetValues(values);
                    s3 = s3.Trim();
                    if (s3 != "")
                        infos.SetValue(s3, null);
                }
                return "";
            }
            else
            {
                NamedValues<ZValue> values = _textInfoRegexList.ExtractTextValues(ref s);
                infos.SetValues(values);
                return s;
            }
        }

        public static string TrimString(string s)
        {
            s = s.Trim(_trimChars);
            return s;
        }

        private static Regex _rgDate = new Regex("(aujourd'hui|hier)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static DateTime? ParseDateTime(string date)
        {
            // Aujourd'hui, 17:13
            if (date == null)
                return null;
            Match match = _rgDate.Match(date);
            if (match.Success)
            {
                string s = null;
                switch (match.Groups[1].Value.ToLower())
                {
                    case "aujourd'hui":
                        s = DateTime.Today.ToString("dd-MM-yyyy");
                        break;
                    case "hier":
                        s = DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy");
                        break;
                }
                date = match.zReplace(date, s);
            }
            //if (string.Equals(date, "Aujourd'hui", StringComparison.CurrentCultureIgnoreCase))
            //    date = DateTime.Today.ToString("dd/MM/yyyy");
            //else if (string.Equals(date, "Hier", StringComparison.CurrentCultureIgnoreCase))
            //    date = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            DateTime dt;
            if (DateTime.TryParseExact(date, @"dd-MM-yyyy, HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                //return true;
                return dt;
            WriteLine("unknow date time \"{0}\"", date);
            //return false;
            return null;
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }

        private static void InitImagesToSkip(XElement xe)
        {
            _imagesToSkip = new Dictionary<string, string>();
            //_imagesToSkip.Add("http://prezup.eu/prez/infossurlebook.png", "infos sur l'ebook"); // http://www.telechargement-plus.com/e-book-magazines/86556-01net-hors-syrie-n-76-octobre-2013-lien-direct.html
            //_imagesToSkip.Add("http://www.telechargement-plus.com/templates/film-gratuit/images/prez/livre.png", "ebook-mag"); // http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html
            //_imagesToSkip.Add("http://www.hapshack.com/images/0THnp.gif", "cloudzer"); // http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html
            //_imagesToSkip.Add("http://www.hapshack.com/images/9MfYk.gif", "uploaded"); // http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html
            //_imagesToSkip.Add("http://www.hapshack.com/images/Js84x.jpg", "hulkfile"); // http://www.telechargement-plus.com/e-book-magazines/87209-les-cahiers-du-monde-de-lintelligence-n-2-novembre-dycembre-2013-janvier-2014-lien-direct.html
            //_imagesToSkip.Add("http://www.hapshack.com/images/QYeW0.gif", "turbobit"); // http://www.telechargement-plus.com/e-book-magazines/88199-skieur-n-102-septembre-2013-liens-direct.html
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            //_imagesToSkip.Add("", ""); // 
            foreach (XElement xe2 in xe.XPathSelectElements("ImageToSkip"))
                _imagesToSkip.Add(xe2.zAttribValue("url"), xe2.zAttribValue("name"));
        }
    }
}

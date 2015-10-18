using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.JScript;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Mongo.Serializers;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
using pb.Web.Data;
using Print;

// Application Domains http://msdn.microsoft.com/en-us/library/cxk374d9(v=vs.85).aspx
// Windows Communication Foundation http://msdn.microsoft.com/en-us/library/dd456779.aspx

//namespace Print.download
namespace Download.Print
{
    public class Test_Class_01
    {
        //public int Value { get; } = 123;
        public int Value { get; set; }
    }

    public class Test_Class_02
    {
        public int Int;
        public DateTime DateTime; 
    }

    public class ExportAttribute : Attribute
    {
    }

    public class Test01
    {
        [Export]
        public int value;
    }

    public static class Download_Exe
    {
        public static void Test_rv_01()
        {
            Trace.WriteLine("Test_rv_01");
        }

        public static void Test_01()
        {
            //string s = null;
            Test_Class_01 test_Class_01 = new Test_Class_01();
            Trace.WriteLine("{0}", test_Class_01.Value);
            //Assembly.ReflectionOnlyLoad()
            //Assembly.ReflectionOnlyLoadFrom();
            //Assembly assembly = null;
            //assembly.FullName
            //Type type = null;
            //type.AssemblyQualifiedName
            //HttpRequestMethod.
        }

        public static void ViewDocuments<TKey, TData>(WebDataManager<TKey, TData> webDataManager, string query = null, string sort = null, int limit = 10, bool loadImage = false)
        {
            //RunSource.CurrentRunSource.View(webDataManager.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage).Select(
            //    data =>
            //        {
            //            IPost post = data as IPost;
            //            return new
            //            {
            //                id = post.GetKey(),
            //                loadFromWebDate = post.GetLoadFromWebDate(),
            //                creationDate = post.GetPostCreationDate(),
            //                printType = post.GetPrintType().ToString(),
            //                //category = post.category,
            //                title = post.GetTitle(),
            //                url = post.GetDataHttpRequest().Url,
            //                images = (from image in post.GetImages() select image.Image).ToArray(),
            //                downloadLinks = post.GetDownloadLinks()
            //            };
            //        }
            //    ));

            //webDataManager.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage).Select(
            //    data =>
            //    {
            //        IPost post = data as IPost;
            //        return new
            //        {
            //            id = post.GetKey(),
            //            loadFromWebDate = post.GetLoadFromWebDate(),
            //            creationDate = post.GetPostCreationDate(),
            //            printType = post.GetPrintType().ToString(),
            //            //category = post.category,
            //            title = post.GetTitle(),
            //            url = post.GetDataHttpRequest().Url,
            //            images = (from image in post.GetImages() select image.Image).ToArray(),
            //            downloadLinks = post.GetDownloadLinks()
            //        };
            //    }
            //    ).zView();

            webDataManager.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage).Select(data => CompactPost.Create(data as IPost)).zView();
        }

        //public static void Test_LoadDetailItemList_v3_01(HeaderDetailWebDocumentStore_v3<int, int, IPost> store, string query = null, string sort = null, int limit = 10, bool loadImage = false)
        //{
        //    RunSource.CurrentRunSource.View(from post in store.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage)
        //                                    select new
        //                                    {
        //                                        id = post.GetKey(),
        //                                        loadFromWebDate = post.GetLoadFromWebDate(),
        //                                        creationDate = post.GetPostCreationDate(),
        //                                        printType = post.GetPrintType().ToString(),
        //                                        //category = post.category,
        //                                        title = post.GetTitle(),
        //                                        url = post.GetDataHttpRequest().Url,
        //                                        images = (from image in post.GetImages() select image.Image).ToArray(),
        //                                        downloadLinks = post.GetDownloadLinks()
        //                                    });
        //}

        //public static void Test_LoadDetailItemList_v2_01(HeaderDetailWebDocumentStore_v2<int, int, IPost> store, string query = null, string sort = null, int limit = 10, bool loadImage = false)
        //{
        //    RunSource.CurrentRunSource.View(from post in store.FindDocuments(query, sort: sort, limit: limit, loadImage: loadImage)
        //                                    select new
        //                                    {
        //                                        id = post.GetKey(),
        //                                        loadFromWebDate = post.GetLoadFromWebDate(),
        //                                        creationDate = post.GetPostCreationDate(),
        //                                        printType = post.GetPrintType().ToString(),
        //                                        //category = post.category,
        //                                        title = post.GetTitle(),
        //                                        url = post.GetDataHttpRequest().Url,
        //                                        images = (from image in post.GetImages() select image.Image).ToArray(),
        //                                        downloadLinks = post.GetDownloadLinks()
        //                                    });
        //}

        public static void Test_Download_01()
        {
            // from http://www.csharp-examples.net/download-files/

            string dir = @"c:\pib\_dl\_jd\_test";
            string url = "http://s100.alldebrid.com/dl/39r7ul146c/40766.6OMC462.zip";
            string file = "40766.6OMC462.zip";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            // 60 Millions de Consommateurs N°462 : 40766.6OMC462.zip      104 998 527 bytes
            //   download avec jdownloader                   04:15 (12:32:45 - 12:37:00)    402 KB/s
            //   download avec bitsadmin                     06:20 (12:40:15 - 12:46:35)    270 KB/s
            //   download avec bitsadmin powershell
            //   download avec WebClient.DownloadFile        04:45 360 KB/s, 04:29 380.9 KB/s
            //   download avec WebClient.DownloadFileAsync   04:02 423.1 KB/s

            Trace.WriteLine("Test_Download_01");
            string path = zPath.Combine(dir, file);
            Trace.WriteLine("download url \"{0}\" in \"{1}\"", url, path);
            WebClient webClient = new WebClient();
            DateTime dt = DateTime.Now;
            webClient.DownloadFile(url, path);
            TimeSpan ts = DateTime.Now.Subtract(dt);
            //FileInfo fi = new FileInfo(path);
            var fi = zFile.CreateFileInfo(path);
            Trace.WriteLine(@"downloaded time {0:hh\:mm\:ss} speed {1:0.0} KB/s", ts, fi.Length / ts.TotalSeconds / 1024);
        }

        public static void Test_Object_01(object obj)
        {
            //The call is ambiguous between the following methods or properties: 'pb.GlobalExtension.zGetName(System.Type)' and 'MongoDB.Bson.Serialization.GlobalExtension.zGetName(System.Type)'
            Trace.WriteLine("type : {0}", obj.GetType().zGetTypeName());
        }

        public static DateTime _downloadStartTime;
        public static string _downloadPath;
        public static int _lastDownloadPercentage;
        public static void Test_Download_02()
        {
            // from http://www.csharp-examples.net/download-files/

            string dir = @"c:\pib\_dl\_jd\_test";
            string url = "http://s100.alldebrid.com/dl/39r7ul146c/40766.6OMC462.zip";
            string file = "40766.6OMC462.zip";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";

            Trace.WriteLine("Test_Download_02");
            _downloadPath = zPath.Combine(dir, file);
            Trace.WriteLine("download async url \"{0}\" in \"{1}\"", url, _downloadPath);
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            _downloadStartTime = DateTime.Now;
            _lastDownloadPercentage = 0;
            webClient.DownloadFileAsync(new Uri(url), _downloadPath);
        }

        private static void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
            if (e.ProgressPercentage >= _lastDownloadPercentage + 5)
            {
                _lastDownloadPercentage = e.ProgressPercentage;
                Trace.WriteLine("receive {0} / {1} bytes {2} %", e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
            }
        }

        private static void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Trace.WriteLine("Download completed {0} %");
            //MessageBox.Show("Download completed!");
            TimeSpan ts = DateTime.Now.Subtract(_downloadStartTime);
            //FileInfo fi = new FileInfo(_downloadPath);
            var fi = zFile.CreateFileInfo(_downloadPath);
            Trace.WriteLine(@"downloaded time {0:hh\:mm\:ss} speed {1:0.0} KB/s", ts, fi.Length / ts.TotalSeconds / 1024);
        }

        //public static void Test_XXElement_01()
        //{
        //    Trace.WriteLine("Test_XXElement_01");
        //    XXElement xxe = new XXElement(HtmlXmlReader.CurrentHtmlXmlReader.XDocument.Root);
        //    string xpath = "//article";
        //    XXElement xxe2 = xxe.XPathElement(xpath);
        //    Trace.WriteLine("xpath \"{0}\" : \"{1}\"", xpath, xxe2.XElement.zGetPath());

        //    xpath = "//article[2]";
        //    xxe2 = xxe.XPathElement(xpath);
        //    Trace.WriteLine("xpath \"{0}\" : \"{1}\"", xpath, xxe2.XElement.zGetPath());

        //    xpath = "//article[3]";
        //    xxe2 = xxe.XPathElement(xpath);
        //    Trace.WriteLine("xpath \"{0}\" : \"{1}\"", xpath, xxe2.XElement.zGetPath());
        //}

        //public static void Test_XText_01()
        //{
        //    Trace.WriteLine("Test_XText_01");
        //    string xpath = "//article";
        //    XElement xe = HtmlXmlReader.CurrentHtmlXmlReader.XDocument.Root.XPathSelectElement(xpath);
        //    Trace.WriteLine("xpath \"{0}\" : \"{1}\"", xpath, xe.zGetPath());
        //    xpath = ".//div[@class='entry_top']";
        //    xe = xe.XPathSelectElement(xpath);
        //    Trace.WriteLine("xpath \"{0}\" : \"{1}\"", xpath, xe.zGetPath());

        //    Trace.WriteLine("select text node :");
        //    var q = from n in xe.DescendantNodes() where n.NodeType == XmlNodeType.Text select (n as XText).Value;
        //    Trace.WriteLine(q.zToStringValues(s => "\"" + s + "\""));

        //}

        //public static void Test_Download_Url_01()
        //{
        //    Trace.WriteLine("Test_Download_Url_01");
        //    //string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    string url = @"c:\pib\dev_data\exe\wrun\test\frboard\post\000442\442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
        //    //Trace.WriteLine("download url \"{0}\" to \"{1}\"", url, file);
        //    //Frboard.LoadToFile(url, file);

        //    Trace.WriteLine("download url \"{0}\"", url);
        //    Http2.LoadUrl(url);

        //    //Frboard.LoadUrl(url);
        //    //Trace.WriteLine("request headers :");
        //    //Frboard.WriteHeaders(HtmlXmlReader.CurrentHtmlXmlReader.http.Request.Headers);
        //    //Trace.WriteLine("response headers :");
        //    //Frboard.WriteHeaders(HtmlXmlReader.CurrentHtmlXmlReader.http.Response.Headers);
        //}

        public static void Test_Uri_01()
        {
            Trace.WriteLine("Test_Uri_01");
            string url = "http://www.frboard.com/magazines-et-journaux/442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.html";
            Uri uri = new Uri(url);
            Trace.WriteLine("url : \"{0}\"", url);
            Trace.WriteLine("last segment  : \"{0}\"", uri.Segments[uri.Segments.Length - 1]);
            Trace.WriteLine("UrlToFileName : \"{0}\"", zurl.UrlToFileName(url, UrlFileNameType.FileName, ".xml"));
            //RunSource.CurrentRunSource.View(uri);
        }

        public static void Test_XmlReader_01()
        {
            // XmlReader - Self-closing element does not fire a EndElement event? http://stackoverflow.com/questions/241336/xmlreader-self-closing-element-does-not-fire-a-endelement-event

            Trace.WriteLine("Test_XmlReader_01");
            string file = @"c:\pib\dev_data\exe\wrun\test\frboard\post\000442\442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.xml";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            using (XmlReader xr = XmlReader.Create(file, settings))
            {
                while (!xr.EOF)
                {
                    xr.Read();
                    Trace.Write("depth {0} nodetype {1} name \"{2}\"", xr.Depth, xr.NodeType, xr.Name);
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        Trace.Write(" isemptyelement {0} hasattributes {1}", xr.IsEmptyElement, xr.HasAttributes);
                        if (xr.HasAttributes)
                        {
                            while (xr.MoveToNextAttribute())
                            {
                                Trace.WriteLine();
                                Trace.Write("  depth {0} nodetype {1} name \"{2}\" value \"{3}\"", xr.Depth, xr.NodeType, xr.Name, xr.Value);
                            }
                            xr.MoveToElement();
                        }
                    }
                    Trace.WriteLine();
                }
            }
        }

        public static void Test_XmlReader_02()
        {
            Trace.WriteLine("Test_XmlReader_01");
            string file = @"c:\pib\dev_data\exe\wrun\test\frboard\post\000442\442649-multi-les-magazines-jeudi-15-aout-2013-pdf-liens-direct-new-post.xml";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            using (XmlReader xr = XmlReader.Create(file, settings))
            {
                while (!xr.EOF)
                {
                    xr.Read();
                    Trace.Write("depth {0} nodetype {1} name \"{2}\"", xr.Depth, xr.NodeType, xr.Name);
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        xr.MoveToAttribute("value");
                        Trace.Write(" attrib value \"{0}\"", xr.Value);
                    }
                    Trace.WriteLine();
                }
            }
        }

        //public static void Test_AllDebrid_01()
        //{
        //    //http://www.alldebrid.com/service.php?pseudo=la_beuze&password=zzzz&link=http://ul.to/k84i4bqn&view=1
        //    //string link = "http://uploaded.net/file/uljqb41x/%F1%F122%F1%F1.zip";
        //    //string link = "http://uploaded.net/file/6bn1mzvu/S_V_1148.zip";
        //    //string link = "http://cloudzer.net/file/ipc3kfc5/S_A_795.zip";
        //    //string link = "http://cloudzer.net/file/tru86c3a/22.zip";
        //    //string link = "http://uploaded.net/file/xsj2r9a3/%F1%F123%F1%F1.zip";
        //    //string link = "http://ul.to/xruyw4ti";
        //    //string link = "http://ul.to/42bhkcpa";
        //    //string link = "http://ul.to/whkfi9dr"; // LES JOURNAUX - SAMEDI 17 AOUT 2013 & + [PDF][Lien Direct] http://clz.to/lnpbnl1w / http://ul.to/whkfi9dr / http://turbobit.net/xjkmsz82qp3y.html
        //    //string link = "http://bayfiles.net/file/Rofb/NxsNgh/zzfigzz.pdf";
        //    //string link = "http://ul.to/hxdv0ho9";
        //    //string link = "http://ul.to/uaf7q3m7"; // 5 livres de Gilbert Sinoué http://bookddl.com/livres/14164-5-livres-de-gilbert-sinoug.html
        //    //string link = "http://ul.to/e2m1luyw";  // L'humanité disparaîtra - Bon débarras ! http://www.telechargement-plus.com/e-book-magazines/livres/92009-lhumanity-disparaotra-bon-dybarras-.html
        //    // http://s18.alldebrid.com/dl/55q89k66c3/LDBD.rar
        //    // Top 500 Sites Internet - Collection 2013 http://www.telechargement-plus.com/e-book-magazines/magazines/92025-top-500-sites-internet-collection-2013.html
        //    // C:\pib\_dl\_pib
        //    //string link = "http://ul.to/aehrpy05"; // http://s11.alldebrid.com/dl/55vptk0b41/armani16.Top_500_Sites_Internet.part1.rar
        //    //string link = "http://ul.to/x96u2gni"; // http://s18.alldebrid.com/dl/55vsf5f46e/armani16.Top_500_Sites_Internet.part2.rar
        //    //string link = "http://ul.to/2n9veejy"; // http://s12.alldebrid.com/dl/55vtk89707/armani16.Top_500_Sites_Internet.part3.rar
        //    //string link = "http://ul.to/a3i0l03b"; // http://s11.alldebrid.com/dl/5sbtv85e03/Windows_Internet_Pratique_12_2013.pdf
        //    string link = "http://clz.to/9d837e3u"; // http://s15.alldebrid.com/dl/5tjohw8b64/@Ge0@N@418.pdf  http://www.telechargement-plus.com/e-book-magazines/94928-gyo-n418-dycembre-2013-liens-direct.html
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    string url = "http://www.alldebrid.com/service.php?pseudo=la_beuze&password=zzzz&view=1&link=" + link;
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load(url);
        //    Trace.WriteLine("result type : {0}", HtmlXmlReader.CurrentHtmlXmlReader.http.ContentType);
        //    //Trace.WriteLine(HtmlXmlReader.CurrentHtmlXmlReader.http.TextResult);
        //    string downloadUrl = HtmlXmlReader.CurrentHtmlXmlReader.http.TextResult;
        //    // "2,;,http://uploaded.net/file/9jly06yf : <span style='color:#a00;'>Link is dead</span> - <a href='javascript:void(0);' onclick="dead_retry(this, 'http%3A%2F%2Fuploaded.net%2Ffile%2F9jly06yf');">retry</a>,;,0"
        //    //string file = Http.UrlToFileName(downloadUrl);
        //    if (downloadUrl.Contains("Link is dead"))
        //        Trace.WriteLine("alldebrid link is dead : \"{0}\"", downloadUrl);
        //    else
        //        Trace.WriteLine("alldebrid download url : \"{0}\"", downloadUrl);
        //    //Trace.WriteLine("file : \"{0}\"", file);
        //    //HtmlXmlReader.CurrentHtmlXmlReader.Save(downloadUrl);
        //}

        //public static void Test_AllDebrid_02()
        //{
        //    //http://www.alldebrid.com/service.php?pseudo=la_beuze&password=zzzz&link=http://ul.to/k84i4bqn&view=1
        //    //string link = "http://uploaded.net/file/uljqb41x/%F1%F122%F1%F1.zip";
        //    //string link = "http://uploaded.net/file/6bn1mzvu/S_V_1148.zip";
        //    //string link = "http://cloudzer.net/file/ipc3kfc5/S_A_795.zip";
        //    //string link = "http://cloudzer.net/file/tru86c3a/22.zip";
        //    //string link = "http://uploaded.net/file/xsj2r9a3/%F1%F123%F1%F1.zip";
        //    //string link = "http://ul.to/xruyw4ti";
        //    //string link = "http://ul.to/42bhkcpa";
        //    string link = "http://ul.to/whkfi9dr"; // LES JOURNAUX - SAMEDI 17 AOUT 2013 & + [PDF][Lien Direct] http://clz.to/lnpbnl1w / http://ul.to/whkfi9dr / http://turbobit.net/xjkmsz82qp3y.html
        //    //string link = "http://bayfiles.net/file/Rofb/NxsNgh/zzfigzz.pdf";
        //    //string link = "totototo";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    //string link = "";
        //    AllDebrid allDebrid = new AllDebrid();
        //    AllDebrid.DownloadDir = @"c:\pib\_dl\_jd\_new\pdf";
        //    allDebrid.Donwload(link);
        //}

        public static void Test_AllDebrid_03()
        {
            string link = "http://s11.alldebrid.com/dl/54ow3t74a2/SinoueGilbert.zip";  // 5 livres de Gilbert Sinoué http://bookddl.com/livres/14164-5-livres-de-gilbert-sinoug.html http://ul.to/uaf7q3m7
            string file = @"c:\pib\_dl\_jd\_bits\Gilbert_Sinoué2\SinoueGilbert.zip";
            //string link = "";
            //string link = "";
            //string link = "";
            //string link = "";
            //string link = "";
            zfile.CreateFileDirectory(file);
            WebClient client = new WebClient();
            client.DownloadFile(link, file);
        }

        //public static void Test_GetNewIndexedDirectory_01()
        //{
        //    Trace.WriteLine("Test_GetNewIndexedDirectory_01");
        //    string dir = @"c:\pib\_dl\_jd\_new\pdf";
        //    Trace.WriteLine("dir \"{0}\"", dir);
        //    string newDir = zfile.GetNewIndexedDirectory(dir);
        //    Trace.WriteLine("new dir \"{0}\"", newDir);
        //}

        public static void Test_JScript_01()
        {
            Trace.WriteLine("Test_JScript_01");

            CodeDomProvider provider = new JScriptCodeProvider();
            CompilerParameters para = new CompilerParameters();
            para.GenerateInMemory = true;
            //            string jscode =
            //            @"class Sample
            //     {
            //         var mytext=’This is JScript’;
            //     }";
            string jscode = zfile.ReadAllText("magazine3k.js");
            //string jscode = "function test_javascript_01() { return \"toto\"; }";
            CompilerResults result = provider.CompileAssemblyFromSource(para, jscode);
            Assembly assembly = result.CompiledAssembly;
            Type jsType = assembly.GetType("magazine3k.test");
            //object jsObject = Activator.CreateInstance(jsType);
            //object retValue = jsType.InvokeMember("text", BindingFlags.GetField, null, jsObject, null);
            //wr.WriteLine("text : {0}", retValue);
            //object retValue = jsType.InvokeMember("test_01", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, null, null);
            // BindingFlags.CreateInstance BindingFlags.GetField | BindingFlags.SetField BindingFlags.GetProperty | BindingFlags.SetProperty
            //object retValue = jsType.InvokeMember("test_01", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
            object retValue = jsType.InvokeMember("test_01", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
            Trace.WriteLine("magazine3k.test.test_01() : {0}", retValue);
            jsType = assembly.GetType("magazine3k.magazine3k");
            object jsObject = Activator.CreateInstance(jsType);
            jsType.InvokeMember("init", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, jsObject, null);
            Trace.WriteLine("magazine3k.magazine3k.init()");
            string s = "wjSWs:VK00ZX.BmUr+K5yxa.hK1sA?UYYqB51IUqPKffxQ";
            retValue = jsType.InvokeMember("encrypt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, jsObject, new object[] { s });
            Trace.WriteLine("magazine3k.magazine3k.encrypt(\"{0}\") : \"{1}\"", s, retValue);
        }

        //public static void Test_JScript_02()
        //{
        //    Trace.WriteLine("Test_JScript_02");
        //    //wr.WriteLine("magazine3k.test.test_01() : {0}", magazine3k.test.test_01());
        //    //magazine3k.magazine3k m3k = new magazine3k.magazine3k();
        //    //wr.WriteLine("Magazine3kEncrypt.initDone : {0}", Magazine3kEncrypt.initDone);
        //    //wr.WriteLine("Magazine3kEncrypt.init()");
        //    //m3k.init();
        //    //Magazine3kEncrypt.init();
        //    //wr.WriteLine("Magazine3kEncrypt.initDone : {0}", Magazine3kEncrypt.initDone);
        //    string s = "wjSWs:VK00ZX.BmUr+K5yxa.hK1sA?UYYqB51IUqPKffxQ";
        //    //wr.WriteLine("magazine3k.magazine3k.encrypt(\"{0}\") : \"{1}\"", s, m3k.encrypt(s));
        //    Trace.WriteLine("magazine3k.magazine3k.encrypt(\"{0}\") : \"{1}\"", s, Magazine3kEncrypt.encrypt(s));
        //}

        public static void Test_Base64_01()
        {
            Trace.WriteLine("Test_Base64_01");
            string s = "TFlwdFg6Sldvb0ZMLjNRaWRJOFBFSHMuUjZGY3c/P01PeDZKOXZrQjdNQmtOVFY=";
            Trace.WriteLine("string : \"{0}\"", s);
            byte[] bytes = System.Convert.FromBase64String(s);
            Trace.WriteLine("FromBase64String : \"{0}\"", bytes.zToStringValues());
            //wr.WriteLine("FromBase64String : \"{0}\"", new string(BytesToChars(bytes)));
            Trace.WriteLine("FromBase64String : \"{0}\"", new string(Encoding.Default.GetChars(bytes)));
        }

        public static char[] BytesToChars(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
                chars[i] = (char)bytes[i];
            return chars;
        }

        //public static void Test_http_01()
        //{
        //    Trace.WriteLine("Test_http_01");
        //    string url = "http://telecharger-pdf.com/";
        //    Http http = new Http(url);
        //    Trace.WriteLine("load \"{0}\"", url);
        //    http.Load();
        //}

        public static void Test_WebRequest_01()
        {
            Trace.WriteLine("Test_WebRequest_01");
            string url = "http://telecharger-pdf.com/";
            Trace.WriteLine("load \"{0}\"", url);
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)webRequest;
                httpRequest.UserAgent = "pb";
                httpRequest.Method = "GET";
                //httpRequest.Headers.Add(gHeaders);
                CookieContainer cookies = new CookieContainer();
                httpRequest.CookieContainer = cookies;
            }
            WebResponse webResponse = webRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            //GetWebRequestHeaderValues();
        }

        //public static void Test_telecharger_pdf_search_01()
        //{
        //    Trace.WriteLine("Test_telecharger_pdf_search_01");
        //    string url = "http://telecharger-pdf.com/";
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load(url);
        //}

        //public static void Test_telecharger_pdf_view_01()
        //{
        //    Trace.WriteLine("Test_telecharger_pdf_view_01");
        //    HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//article:.:EmptyRow", ".//h1/@class:.:EmptyRow", ".//a/text()");
        //}

        //public static void Test_magazine3k_search_01()
        //{
        //    Trace.WriteLine("Test_magazine3k_search_01");
        //    string url = "http://magazine3k.com/search/?q=l%27express&as_occt=title&as_ordb=relevance&sitesearch=magazine3k.com";
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load(url);
        //    //wr.Select("/xml/html/body/div[2]//table//tr");
        //    //wr.Select("//div[@class='res']");
        //    //wr.Select("//div");
        //    //wr.Select("//div[@class]");
        //    //wr.Select("//div[@class = \"res_data\"]");
        //    //wr.Select("//div[@class='res']:.:EmptyRow");
        //    //XmlNode[] nodes = wr.SelectNodes("//div[@class='res']");
        //    //wr.Select(xpath);
        //    DataTable data = HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='res']", "@class", ".//a/@href:.:n(href)", ".//a//text():.:n(label1)",
        //      ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2)", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)",
        //      ".//img/@src:.:n(img)", ".//div[@class='justi']//text():.:n(label2):Concat()", ".//div[@class='cat']/text():.:n(category)");
        //    if (data.Rows.Count == 0) return;
        //    DataRow row;
        //    if (data.Rows.Count >= 4)
        //        row = data.Rows[3];
        //    else
        //        row = data.Rows[0];
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load("http://magazine3k.com/", (string)row["href"]);
        //}

        //public static void Test_magazine3k_search_02()
        //{
        //    Trace.WriteLine("Test_magazine3k_search_01");
        //    string url = "http://magazine3k.com/search/?q=l%27express&as_occt=title&as_ordb=relevance&sitesearch=magazine3k.com";
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load(url);
        //    //DataTable data = hxr.ReadSelect("//div[@class='res']", "@class", ".//a/@href:.:n(href)", ".//a//text():.:n(label1)",
        //    //  ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2)", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)",
        //    //  ".//img/@src:.:n(img)", ".//div[@class='justi']//text():.:n(label2):Concat()", ".//div[@class='cat']/text():.:n(category)");
        //    XmlSelect select = HtmlXmlReader.CurrentHtmlXmlReader.Select("//div[@class='res']", "@class", ".//a/@href:.:n(href)", ".//a//text():.:n(label1)",
        //      ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2):int", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)",
        //      ".//img/@src:.:n(img)", ".//div[@class='justi']//text():.:n(label2):Concat()", ".//div[@class='cat']/text():.:n(category)");

        //    DataTable dt = zdt.Create("node, txt, href, label1, info1, info2 int, info3, info4, img, label2, category");

        //    while (select.Get())
        //    {
        //        DataRow row = dt.NewRow();
        //        row["node"] = select.TranslatedPathCurrentNode;
        //        row["txt"] = select[0];
        //        row["href"] = select["href"];
        //        row["label1"] = select["label1"];
        //        row["info1"] = select["info1"];
        //        row["info2"] = select["info2"];
        //        row["info3"] = select["info3"];
        //        row["info4"] = select["info4"];
        //        row["img"] = select["img"];
        //        row["label2"] = select["label2"];
        //        row["category"] = select["category"];
        //        dt.Rows.Add(row);
        //    }
        //    RunSource.CurrentRunSource.Result = dt;
        //}

        //public static void Test_magazine3k_search_03(bool detail = false)
        //{
        //    Trace.WriteLine("Test_magazine3k_search_03");
        //    string url = "http://magazine3k.com/search/?q=l%27express&as_occt=title&as_ordb=relevance&sitesearch=magazine3k.com";
        //    RunSource.CurrentRunSource.View(magazine3k_01.search(url, detail));
        //}

        //public static void Test_telecharger_pdf_search_02(bool detail = false)
        //{
        //    Trace.WriteLine("Test_telecharger_pdf_search_02");
        //    //string url = "http://telecharger-pdf.com/";
        //    //string url = "http://telecharger-pdf.com/?paged=4";
        //    string dir = @"c:\pib\dev_data\exe\wrun\test\model\telecharger-pdf\";
        //    string url = dir + "0935_telecharger-pdf.com_01_01.html";
        //    telecharger_pdfPrint[] prints = telecharger_pdf.search(url, detail);
        //    Trace.WriteLine("load {0} prints", prints.Length);
        //    //_wr.View(prints);
        //    Trace.WriteLine(prints.ToString());
        //}

        //public static void Test_magazine3k_print_01()
        //{
        //    Trace.WriteLine("Test_magazine3k_print_01");
        //    string url = "http://magazine3k.com/magazine/other/31496/lexpress-lexpress-styles-3180-13-au-19-juin-2012.html";
        //    HtmlXmlReader.CurrentHtmlXmlReader.Load(url);
        //    // Base64.encrypt(Base64.decode('TFlwdFg6Sldvb0ZMLjNRaWRJOFBFSHMuUjZGY3c/P01PeDZKOXZrQjdNQmtOVFY=')) = "http://www.mediafire.com/??k538in7yh0p3kqb"
        //    // Base64.decode('TFlwdFg6Sldvb0ZMLjNRaWRJOFBFSHMuUjZGY3c/P01PeDZKOXZrQjdNQmtOVFY=') = "LYptX:JWooFL.3QidI8PEHs.R6Fcw??MOx6J9vkB7MBkNTV"
        //    // Base64.encrypt(Base64.decode('d2pTV3M6UFF1dUROLjFTZ2ZLK05HRnUudkU3aU8/T09sNFE0LzFOcWZidUJoWA==')) = "http://www.mediafire.com/?k7h85gphvn4vskr"
        //    // Base64.decode('d2pTV3M6UFF1dUROLjFTZ2ZLK05HRnUudkU3aU8/T09sNFE0LzFOcWZidUJoWA==') = "wjSWs:PQuuDN.1SgfK+NGFu.vE7iO?OOl4Q4/1NqfbuBhX"
        //    // Base64.encrypt(Base64.decode('d2pTV3M6VkswMFpYLkJtVXIrSzV5eGEuaEsxc0E/VVlZcUI1MUlVcVBLZmZ4UQ==')) = "http://www.mediafire.com/?5qbpvmkbxgv5a98"
        //    // Base64.decode('d2pTV3M6VkswMFpYLkJtVXIrSzV5eGEuaEsxc0E/VVlZcUI1MUlVcVBLZmZ4UQ==') = "wjSWs:VK00ZX.BmUr+K5yxa.hK1sA?UYYqB51IUqPKffxQ"
        //    //test_javascript_01();
        //    // print title : L'Express + L'Express Styles 3180 - 13 au 19 Juin 2012
        //    //wr.Select("//div[@class='headline']:.:EmptyRow", ".//text()");
        //    string title = HtmlXmlReader.CurrentHtmlXmlReader.SelectValue("//div[@class='headline']", ".//text()");
        //    Trace.WriteLine("title : \"{0}\"", title);
        //    // Downloads:, 604, Updated:, 13-06-2012
        //    //wr.Select("//div[@class='res_data']:.:EmptyRow", ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2)", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)");
        //    //wr.Select("//div[@class='res_data']//text():.:EmptyRow");
        //    string[] infos = HtmlXmlReader.CurrentHtmlXmlReader.SelectValues("//div[@class='res_data']//text()");
        //    Trace.WriteLine("infos : {0}", infos.zToStringValues());
        //    // url image : http://image2.magazine3k.com/data_images/2012/06/13/1339572260_medium.jpeg
        //    //wr.Select("//div[@class='res_image']:.:EmptyRow", ".//img/@src");
        //    string imageUrl = HtmlXmlReader.CurrentHtmlXmlReader.SelectValue("//div[@class='res_image']//img/@src");
        //    Trace.WriteLine("image url : \"{0}\"", imageUrl);
        //    // French | 140+96 pages | HQ PDF | 129+79 MB
        //    // vous offre un autre regard sur l'actualité commentée en mots et en images. vous emmène à la découverte de la mode, de la déco, de l'insolite...
        //    //wr.Select("//div[@class='justi']:.:EmptyRow", ".//div/text()", ".//div/following-sibling::text()");
        //    // ".//div/text()", 
        //    XmlNode node = HtmlXmlReader.CurrentHtmlXmlReader.SelectNode("//div[@class='justi']:.:EmptyRow");
        //    string info = HtmlXmlReader.CurrentHtmlXmlReader.SelectValue(node, ".//div/text()");
        //    Trace.WriteLine("info : \"{0}\"", info);
        //    string comment = HtmlXmlReader.CurrentHtmlXmlReader.SelectValue(node, ".//div/following-sibling::text()");
        //    Trace.WriteLine("comment : \"{0}\"", comment);
        //    // script containing download url
        //    //var url; url = Base64.encrypt(Base64.decode('cy9PS3c6V0pZbFBGOC9OalUuelluK3ovMlBMMzVxOVkzaUJuVmlTJ0FxREJaYVU1XzZuZlh3Xy1fUU5ZX0d0aF91NW1faWFuaVdfMnJiWTkucUJ5Uys='));
        //    //var url; url = Base64.encrypt(Base64.decode('QVRpbWM6dndoYzI4RkcwYXQuVS9BWlVZUm9zUTV1Y2ExRzR0eUYxJzlYKzhrbnBFX3FLa3psbUZfQmNrc0xfLV9RTllfUzUxX01iRV9zVXBzWV9QU2loRS5sTzlkeA=='));
        //    //var url; url = Base64.encrypt(Base64.decode('NXFiZmw6YkU2NlhaLkFuVnEvTDR6d2IuWXpNVjU/UVE3bU9taHJUMEJGd2YvSg=='));
        //    //var url; url = Base64.encrypt(Base64.decode('RVhtaVk6azdGRm9tLkZpUXY2TzkyMWUuOFhveGQ/WlZWbk0wNEZabkNIU1M4ZA=='));
        //    //wr.Select("//div[@class='download_top']/following-sibling::div", "./script/text()");
        //    string[] scriptLinks = HtmlXmlReader.CurrentHtmlXmlReader.SelectValues("//div[@class='download_top']/following-sibling::div", "./script/text()");
        //    Regex rg = new Regex(@"Base64\.encrypt\(Base64\.decode\('([^']*)'\)\)", RegexOptions.Compiled);
        //    foreach (string scriptLink in scriptLinks)
        //    {
        //        Trace.WriteLine("script link : \"{0}\"", scriptLink);
        //        Match match = rg.Match(scriptLink);
        //        if (match.Success)
        //        {
        //            string code = match.Groups[1].Value;
        //            Trace.WriteLine("code : \"{0}\"", code);
        //            string link = magazine3kPrint_01.DecodeDownloadLink(code);
        //            Trace.WriteLine("download link : \"{0}\"", link);
        //        }
        //        else
        //            Trace.WriteLine("code not found");
        //    }
        //}

        //public static void Test_magazine3k_print_02()
        //{
        //    Trace.WriteLine("Test_magazine3k_print_02");
        //    //wr.Load(@"c:\pib\dev_data\exe\wrun\test\magazine3k\magazine3k.com_print_02_02.html");
        //    HtmlXmlReader.CurrentHtmlXmlReader.LoadXml(@"c:\pib\dev_data\exe\wrun\test\magazine3k\magazine3k.com_print_02_02_02.html.xml");
        //    HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='download_top']/following-sibling::div:.:EmptyRow");
        //}

        //public static void Test_magazine3k_print_03()
        //{
        //    Trace.WriteLine("Test_magazine3k_print_03");
        //    string url = "http://magazine3k.com/magazine/other/31496/lexpress-lexpress-styles-3180-13-au-19-juin-2012.html";
        //    magazine3kPrint_01 print = magazine3k_01.loadPrint(url);
        //    Trace.WriteLine("language                : \"{0}\"", print.language);
        //    Trace.WriteLine("title                   : \"{0}\"", print.title);
        //    Trace.WriteLine("downloads               : \"{0}\"", print.downloads);
        //    Trace.WriteLine("updated                 : {0:dd/MM/yyyy}", print.updated);
        //    Trace.WriteLine("imageUrl                : \"{0}\"", print.imageUrl);
        //    Trace.WriteLine("infos                   : {0}", print.infos.zToStringValues());
        //    Trace.WriteLine("comment                 : \"{0}\"", print.comment);
        //    Trace.WriteLine("download links nb       : \"{0}\"", print.downloadLinks.Length);
        //    foreach (string links in print.downloadLinks)
        //    {
        //        Trace.WriteLine("download link           : \"{0}\"", links);
        //    }
        //}

        public static void Test_PrintDirectoryManager_01(string directory)
        {
            PrintDirectoryManager printDirectoryManager = new PrintDirectoryManager(new string[] { ".01_quotidien", ".02_hebdo", ".03_mensuel", ".04_autre", ".05_new_print", ".06_unknow_print", ".07_lettres" });
            Trace.WriteLine("PrintDirectoryManager.GetDirectories() : \"{0}\"", directory);
            Trace.WriteLine(printDirectoryManager.GetDirectories(directory).zToJson());
        }
    }
}

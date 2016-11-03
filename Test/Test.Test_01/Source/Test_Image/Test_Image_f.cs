using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Web;
//using pb.Web.old;

namespace Test.Test_Image
{
    public static class Test_Image_f
    {
        public static void Test_Download_Image_01()
        {
            Trace.WriteLine("Test_Download_Image_01 using WebRequest");
            // hapshack.com :
            //   __utma=161206820.1103252835.1376585085.1376585085.1376585085.1
            //   __utmz=161206820.1376585085.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)
            //string url = "http://www.sudouest.fr/pdf/vignettes2/SO/2013-09-05/unes/21A_141x.jpg";
            // The remote server returned an error: (403) Forbidden. (System.Net.WebException)
            string url = "http://www.hapshack.com/images/M6fo9.jpg";
            //string[] cookies = { "__utma=161206820.1103252835.1376585085.1376585085.1376585085.1", "__utmz=161206820.1376585085.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)" };
            //string cookiesUrl = "http://www.hapshack.com/";
            //string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //string userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36 CoolNovo/2.0.9.11";
            string userAgent = "zozo";
            //Accept-Encoding: gzip,deflate,sdch
            //Accept-Language: en-US,en;q=0.8

            //GET /images/M6fo9.jpg HTTP/1.1
            //Host: www.hapshack.com
            //Connection: keep-alive
            //Cache-Control: max-age=0
            //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
            //X-Purpose: Instant
            //User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36 CoolNovo/2.0.9.11
            //Accept-Encoding: gzip,deflate,sdch
            //Accept-Language: en-US,en;q=0.8
            //If-Modified-Since: Sat, 07 Sep 2013 15:04:17 GMT

            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            Trace.WriteLine("download image \"{0}\"", url);
            System.Net.WebRequest wr = System.Net.WebRequest.Create(url);
            if (wr is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)wr;
                httpRequest.CookieContainer = new CookieContainer();
                //httpRequest.CookieContainer.zAdd(cookiesUrl, cookies);
                //httpRequest.Accept = accept;
                httpRequest.UserAgent = userAgent;
                //httpRequest.CachePolicy
            }
            WebResponse r = wr.GetResponse();
            Stream s = r.GetResponseStream();
            Image image = Image.FromStream(s);
            s.Close();
            r.Close();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("url", typeof(string)));
            dt.Columns.Add(new DataColumn("image", typeof(Image)));
            DataRow row = dt.NewRow();
            row["url"] = url;
            row["image"] = image;
            dt.Rows.Add(row);
            RunSource.CurrentRunSource.SetResult(dt);
        }

        //public static void Test_Download_Image_02()
        //{
        //    Trace.WriteLine("Test_Download_Image_02 using WebRequest");
        //    string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    Trace.WriteLine("download image \"{0}\"", url);
        //    pb.old.Http http = HtmlXmlReader.CurrentHtmlXmlReader.CreateHttp(url);
        //    System.Net.WebRequest wr = http.Request;
        //    WebResponse r = wr.GetResponse();
        //    Stream s = r.GetResponseStream();
        //    Image image = Image.FromStream(s);
        //    s.Close();
        //    r.Close();

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("url", typeof(string)));
        //    dt.Columns.Add(new DataColumn("image", typeof(Image)));
        //    DataRow row = dt.NewRow();
        //    row["url"] = url;
        //    row["image"] = image;
        //    dt.Rows.Add(row);
        //    RunSource.CurrentRunSource.SetResult(dt);
        //}

        //public static void Test_Download_Image_03()
        //{
        //    Trace.WriteLine("Test_Download_Image_03 using HtmlXmlReader.CurrentHtmlXmlReader.LoadImage()");
        //    string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    Trace.WriteLine("download image \"{0}\"", url);

        //    //string userAgent = HtmlXmlReader.CurrentHtmlXmlReader.WebRequestUserAgent;
        //    //HtmlXmlReader.CurrentHtmlXmlReader.WebRequestUserAgent = null;
        //    HttpRequestParameters requestParameters = new HttpRequestParameters();
        //    requestParameters.userAgent = null;
        //    Image image = HtmlXmlReader.CurrentHtmlXmlReader.LoadImage(url, requestParameters);
        //    //HtmlXmlReader.CurrentHtmlXmlReader.WebRequestUserAgent = userAgent;
        //    Trace.WriteLine("image width {0} height {1}", image.Width, image.Height);

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("url", typeof(string)));
        //    dt.Columns.Add(new DataColumn("image", typeof(Image)));
        //    DataRow row = dt.NewRow();
        //    row["url"] = url;
        //    row["image"] = image;
        //    dt.Rows.Add(row);
        //    RunSource.CurrentRunSource.SetResult(dt);
        //}

        //public static void Test_Download_Image_04(bool removeUserAgent = true)
        //{
        //    Trace.WriteLine("Test_Download_Image_04 using Http2.LoadImage()");
        //    //string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    string url = "http://www.hapshack.com/images/Zw0WE.jpg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    Trace.WriteLine("download image \"{0}\"", url);

        //    //string userAgent = HtmlXmlReader.CurrentHtmlXmlReader.WebRequestUserAgent;
        //    //if (removeUserAgent)
        //    //    HtmlXmlReader.CurrentHtmlXmlReader.WebRequestUserAgent = null;
        //    HttpRequestParameters requestParameters = new HttpRequestParameters();
        //    if (removeUserAgent)
        //        requestParameters.userAgent = null;
        //    Image image = Http2.LoadImageFromWeb(url, requestParameters);
        //    //if (removeUserAgent)
        //    //    HtmlXmlReader.CurrentHtmlXmlReader.WebRequestUserAgent = userAgent;
        //    Trace.WriteLine("image width {0} height {1}", image.Width, image.Height);
        //    image = image.zResize(height: 200);
        //    Trace.WriteLine("new image width {0} height {1}", image.Width, image.Height);

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("url", typeof(string)));
        //    dt.Columns.Add(new DataColumn("image", typeof(Image)));
        //    DataRow row = dt.NewRow();
        //    row["url"] = url;
        //    row["image"] = image;
        //    dt.Rows.Add(row);
        //    RunSource.CurrentRunSource.SetResult(dt);
        //}

        //public static void Test_Download_Image_05(string url)
        //{
        //    Trace.WriteLine("Test_Download_Image_05 using Http2.LoadImage()");
        //    //string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    //string url = "http://www.hapshack.com/images/Zw0WE.jpg";
        //    //Error loading html  : "http://www.babelio.com/couv/11657_639334.jpeg"
        //    //string url = "http://www.babelio.com/couv/11657_639334.jpeg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    Trace.WriteLine("download image \"{0}\"", url);

        //    HttpRequestParameters requestParameters = new HttpRequestParameters();
        //    // Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36 CoolNovo/2.0.9.20
        //    // Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        //    //requestParameters.userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36 CoolNovo/2.0.9.20";
        //    //requestParameters.userAgent = "Pib";
        //    //requestParameters.userAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        //    //requestParameters.userAgent = "Mozilla/5.0";
        //    //requestParameters.userAgent = "Mozilla";
        //    //requestParameters.userAgent = "Mozilla/5.0 Pib";
        //    //Trace.WriteLine("set user agent \"{0}\"", requestParameters.userAgent);
        //    //if (removeUserAgent)
        //    //    requestParameters.userAgent = null;
        //    Image image = Http2.LoadImageFromWeb(url, requestParameters);
        //    if (image == null)
        //    {
        //        Trace.WriteLine("image is null");
        //        return;
        //    }
        //    Trace.WriteLine("image width {0} height {1}", image.Width, image.Height);

        //    //return;

        //    //image = image.zResize(height: 200);
        //    //Trace.WriteLine("new image width {0} height {1}", image.Width, image.Height);

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("url", typeof(string)));
        //    dt.Columns.Add(new DataColumn("image", typeof(Image)));
        //    DataRow row = dt.NewRow();
        //    row["url"] = url;
        //    row["image"] = image;
        //    dt.Rows.Add(row);
        //    RunSource.CurrentRunSource.SetResult(dt);
        //}

        public static void Test_Save_Image_01()
        {
            Trace.WriteLine("Test_Save_Image_01 using WebClient");
            string url = "http://www.hapshack.com/images/M6fo9.jpg";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
            Trace.WriteLine("save image \"{0}\" to \"{1}\"", url, file);
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Set(HttpRequestHeader.UserAgent, "toto");
                wc.DownloadFile(url, file);
            }
        }

        public static void Test_Save_Image_02()
        {
            Trace.WriteLine("Test_Save_Image_02 using WebRequest");
            string url = "http://www.hapshack.com/images/M6fo9.jpg";
            string userAgent = "zozo";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            //string url = "";
            string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
            Trace.WriteLine("save image \"{0}\" to \"{1}\"", url, file);
            System.Net.WebRequest wr = System.Net.WebRequest.Create(url);
            if (wr is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)wr;
                httpRequest.UserAgent = userAgent;
            }
            WebResponse r = wr.GetResponse();
            Stream s = r.GetResponseStream();
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                s.CopyTo(fs);
            }
            s.Close();
            r.Close();
        }

        //public static void Test_Save_Image_03()
        //{
        //    Trace.WriteLine("Test_Save_Image_03 using Http.LoadToFile()");
        //    string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
        //    Trace.WriteLine("save image \"{0}\" to \"{1}\"", url, file);
        //    pb.old.Http http = new pb.old.Http(url);
        //    //http.UserAgent = "toto";
        //    http.LoadToFile(file);
        //}

        //public static void Test_Save_Image_04()
        //{
        //    Trace.WriteLine("Test_Save_Image_04 using HtmlXmlReader.LoadToFile()");
        //    string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
        //    Trace.WriteLine("save image \"{0}\" to \"{1}\"", url, file);
        //    HtmlXmlReader.CurrentHtmlXmlReader.LoadToFile(url, file);
        //}

        //public static void Test_Save_Image_05(string url, string file = null)
        //{
        //    Trace.WriteLine("Test_Save_Image_05 using Http2.LoadToFile()");
        //    //string url = "http://www.hapshack.com/images/M6fo9.jpg";
        //    //string url = "http://www.hapshack.com/images/Zw0WE.jpg";
        //    //string url = "http://pixhst.com/avaxhome/05/a4/002da405.jpeg";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string url = "";
        //    //string file = zurl.UrlToFileName(url, UrlFileNameType.FileName);
        //    if (file == null)
        //        file = Path.Combine(RunSource.CurrentRunSource.DataDir, zurl.UrlToFileName(url, UrlFileNameType.FileName));
        //    Trace.WriteLine("save image \"{0}\" to \"{1}\"", url, file);
            
        //    Http2.LoadToFile(url, file);
        //}

        public static void Test_Load_Image_FromFile_01()
        {
            Trace.WriteLine("Test_Load_Image_01");
            //string file = "M6fo9.jpg";
            string file = "Zw0WE.jpg";
            //string file = "";
            //string file = "";
            //string file = "";
            Trace.WriteLine("load image \"{0}\"", file);
            Image image = Image.FromFile(file);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("file", typeof(string)));
            dt.Columns.Add(new DataColumn("image", typeof(Image)));
            DataRow row = dt.NewRow();
            row["file"] = file;
            row["image"] = image;
            dt.Rows.Add(row);
            RunSource.CurrentRunSource.SetResult(dt);
        }

        public static void Test_Load_Image_FromFile_02(string file)
        {
            Trace.WriteLine("Test_Load_Image_02");
            //string file = "M6fo9.jpg";
            //string file = "Zw0WE.jpg";
            //string file = "";
            //string file = "";
            //string file = "";
            //if (!Path.IsPathRooted(file))
            //    file = Path.Combine(RunSource.CurrentRunSource.DataDir, file);
            //file = file.zRootPath(RunSource.CurrentRunSource.DataDir);
            file = file.zRootPath(XmlConfig.CurrentConfig.Get("DataDir"));
            Trace.WriteLine("load image \"{0}\"", file);
            Bitmap bitmap = zimg.LoadBitmapFromFile(file);
            Trace.WriteLine("bitmap width {0} height {1}", bitmap.Width, bitmap.Height);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("file", typeof(string)));
            dt.Columns.Add(new DataColumn("image", typeof(Bitmap)));
            DataRow row = dt.NewRow();
            row["file"] = file;
            row["image"] = bitmap;
            dt.Rows.Add(row);
            RunSource.CurrentRunSource.SetResult(dt);
        }
    }
}

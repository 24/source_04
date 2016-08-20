using MongoDB.Bson;
using pb.Compiler;
using pb.Data;
using pb.Data.Mongo;
using pb.IO;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace pb.Web.Test
{
    public static class Test_HttpLog
    {
        private static string _directory = RunSource.CurrentRunSource.ProjectDirectory;

        public static void Test_HttpRequest(string url, HttpRequestMethod method, string content, string file)
        {
            file = zPath.Combine(_directory, file);
            bool exportResult = HttpManager.CurrentHttpManager.ExportResult;
            HttpManager.CurrentHttpManager.ExportResult = true;
            HttpRun.LoadToFile(new HttpRequest { Url = url, Method = method, Content = content }, file, true, new HttpRequestParameters { Encoding = Encoding.UTF8 });
            HttpManager.CurrentHttpManager.ExportResult = exportResult;
        }

        public static void Test_WebRequest(string url, string method, string content, string file)
        {
            file = zPath.Combine(_directory, file);
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                if (webRequest is HttpWebRequest)
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                    httpWebRequest.UserAgent = "pib/0.1";
                    httpWebRequest.Method = method;
                    if (content != null)
                    {
                        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                        Encoding encoding = Encoding.UTF8;
                        byte[] bytes = encoding.GetBytes(content);
                        httpWebRequest.ContentLength = bytes.LongLength;
                        Stream requestStream = httpWebRequest.GetRequestStream();
                        using (BinaryWriter w = new BinaryWriter(requestStream))
                        {
                            w.Write(bytes);
                        }
                    }
                }
                string resultText;
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            resultText = streamReader.ReadToEnd();
                        }
                    }
                    ExportRequest(webRequest, content, webResponse, file);
                }
                zfile.WriteFile(file, resultText);
            }
            catch (Exception ex)
            {
                Trace.CurrentTrace.WriteError(ex);
            }
        }

        public static void ExportRequest(WebRequest webRequest, string webRequestContent, WebResponse webResponse, string file)
        {
            new HttpResponseLog(webRequest, webRequestContent, webResponse).zSave(zpath.PathSetExtension(file, ".request.json"));
        }

        public static void Test_WebHeader_01()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("xxx", "yyy");
            headers.Add("xxx", "yyy222");
            headers.Add("zzz", "fff");
            headers.Add("xxx", "ttt");
            for (int i = 0; i < headers.Count; ++i)
            {
                string key = headers.GetKey(i);
                foreach (string value in headers.GetValues(i))
                {
                    Trace.WriteLine("{0}: {1}", key, value);
                }
            }
        }

        public static void Test_WebHeader_02()
        {
            string file = @"http\header_01.json";
            string file2 = @"http\header_02.json";

            file = zPath.Combine(_directory, file);
            file2 = zPath.Combine(_directory, file2);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("xxx", "yyy");
            headers.Add("xxx", "yyy222");
            headers.Add("zzz", "fff");
            headers.Add("yyy", "ttt");
            for (int i = 0; i < headers.Count; ++i)
            {
                string key = headers.GetKey(i);
                foreach (string value in headers.GetValues(i))
                {
                    Trace.WriteLine("{0}: {1}", key, value);
                }
            }

            //headers.zSave(file);
            BsonDocument doc = headers.ToBsonDocument();
            doc.zSave(file);
        }

        public static void Test_WebHeader_03()
        {
            string file = @"http\header_01.json";
            string file2 = @"http\header_02.json";

            file = zPath.Combine(_directory, file);
            file2 = zPath.Combine(_directory, file2);

            WebHeaderCollection headers = zmongo.ReadFileAs<WebHeaderCollection>(file);
            headers.zSave(file2);
        }

        public static void Test_WebHeader_04()
        {
            BsonDocument doc = new Test { Name = "toto", Name2 = (ZString)"tata", Number = (ZInt)123 }.ToBsonDocument();
            doc.zToJson().zTrace();
        }
    }

    public class Test
    {
        public string Name;
        public ZString Name2;
        public ZInt Number;
    }

    public class HttpResponseLog
    {
        public int StatusCode;
        public WebHeaderCollection Headers;
        public HttpRequestLog Request;

        public HttpResponseLog(WebRequest webRequest, string webRequestContent, WebResponse webResponse)
        {
            if (webResponse is HttpWebResponse)
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)webResponse;
                StatusCode = (int)httpWebResponse.StatusCode;
                Headers = httpWebResponse.Headers;
                Request = new HttpRequestLog(webRequest, webRequestContent);
            }
        }
    }

    public class HttpRequestLog
    {
        public Uri Uri;
        public string Method;
        public WebHeaderCollection Headers;
        public string Content;

        public HttpRequestLog(WebRequest webRequest, string webRequestContent)
        {
            //if (webRequest is HttpWebRequest)
            //{
            //    HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
            Uri = webRequest.RequestUri;
            Method = webRequest.Method;
            Headers = webRequest.Headers;
            Content = webRequestContent;
            //}
        }
    }
}

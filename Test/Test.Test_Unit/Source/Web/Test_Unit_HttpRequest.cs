using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using pb.Data.Mongo;
using pb.Web;
using pb.Web.old;

namespace Test.Test_Unit.Web
{
    public static class Test_Unit_HttpRequest
    {
        public static void Test_01(string file, bool executeRequest = false)
        {
            Trace.WriteLine("file \"{0}\"", file);
            HttpArchive har = new HttpArchive(file);
            Trace.WriteLine("select first entry");
            var entry = har.Entries.First();
            string url = entry.request.GetHttpRequestUrl();
            Trace.WriteLine("url \"{0}\"", url);
            HttpRequestParameters_v1 requestParameters = entry.request.GetHttpRequestParameters();
            Trace.WriteLine("requestParameters :");
            Trace.WriteLine(requestParameters.zToJson());
            if (executeRequest)
            {
                //Http2.LoadUrl(url, requestParameters);
                Http_v3.LoadUrl(url, requestParameters);
            }
        }

        public static void Test_WebRequest_01()
        {
            string url = "http://rapide-ddl.com/ebooks/";
            string file = @"c:\pib\dev_data\exe\runsource\download\pcap\rapide-ddl.com-ebooks.bin";
            Trace.WriteLine("url \"{0}\"", url);
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            if (webRequest is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)webRequest;

                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";
                httpRequest.AutomaticDecompression = DecompressionMethods.GZip;
                httpRequest.Method = "GET";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                //httpRequest.Expect
                httpRequest.Host = "rapide-ddl.com";
                httpRequest.KeepAlive = true;
                //httpRequest.MediaType
                httpRequest.Referer = "http://rapide-ddl.com/ebooks/";
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                httpRequest.Headers.Add("Accept-Language", "fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4");
                //httpRequest.Headers.Add("Cookie", "hasVisitedSite=Yes");

                Uri uri = new Uri(url);
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.Add(uri, new Cookie("hasVisitedSite", "Yes"));

                //Trace.WriteLine("set HttpWebRequest.ServicePoint.Expect100Continue = false");
                httpRequest.ServicePoint.Expect100Continue = false;
                //httpRequest.ContentType = "";
                //byte[] bytes = encoding.GetBytes(content);
                //httpRequest.ContentLength = bytes.LongLength;
                //Stream stream = httpRequest.GetRequestStream();
                //using (BinaryWriter w = new BinaryWriter(stream))
                //{
                //    w.Write(bytes);
                //}
            }

            Trace.WriteLine("webRequest.GetResponse()");
            WebResponse webResponse = webRequest.GetResponse();
            if (webResponse is HttpWebResponse)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)webResponse;
                // httpResponse.CharacterSet
                // httpResponse.ContentType
                // httpResponse.ContentLength
                Trace.WriteLine("httpResponse.StatusDescription \"{0}\"", httpResponse.StatusDescription);
            }

            Trace.WriteLine("save response to \"{0}\"", file);
            Stream stream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string data = reader.ReadToEnd();
            zfile.WriteFile(file, data);

            reader.Close();
            stream.Close();
            webResponse.Close();

            //BinaryReader reader = new BinaryReader(stream);
            //FileStream fileStream = File.OpenWrite(file);
            //BinaryWriter writer = new BinaryWriter(fileStream);
            //writer.Write()
            //reader.ReadBytes()




        }

        public static void Test_Cookie_01()
        {
            CookieContainer cookies = new CookieContainer();
            Cookie cookie = new Cookie("hasVisitedSite", "Yes");
            cookies.Add(cookie);
        }

        public static void Test_GZipDecompress_01(string gzipFile, string decompressedFile)
        {
            Trace.WriteLine("decompress gzip file \"{0}\" to \"{1}\"", gzipFile, decompressedFile);
            using (FileStream gzipFileStream = File.OpenRead(gzipFile))
            {
                using (FileStream decompressedFileStream = File.Create(decompressedFile))
                {
                    using (GZipStream decompressionStream = new GZipStream(gzipFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
        }

        //public static void Decompress(FileInfo fileToDecompress)
        //{
        //    using (FileStream originalFileStream = fileToDecompress.OpenRead())
        //    {
        //        string currentFileName = fileToDecompress.FullName;
        //        string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

        //        using (FileStream decompressedFileStream = File.Create(newFileName))
        //        {
        //            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
        //            {
        //                decompressionStream.CopyTo(decompressedFileStream);
        //                Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
        //            }
        //        }
        //    }
        //}
    }
}

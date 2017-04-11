using pb.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ENVOYER DES DONNÉES AVEC HTTPCLIENT SELON UN MODÈLE “PUSH” https://www.thomaslevesque.fr/tag/httpcontent/
// https://www.posttestserver.com/
// http://posttestserver.com/post.php?dir=pib

namespace pb.Web.Http.Test
{
    public class HttpResponseText
    {
        public System.Net.Http.HttpResponseMessage Response;
        public string Content;
    }

    public static class Test_HttpClient
    {
        private static List<FileStream> _contentFileStreams = new List<FileStream>();

        public static void AddContentFileStream(FileStream fs)
        {
            _contentFileStreams.Add(fs);
        }

        public static void CloseContentFileStreams()
        {
            foreach (FileStream fs in _contentFileStreams)
                fs.Close();
            _contentFileStreams.Clear();
        }

        public static async Task<string> Test_Upload_01(byte[] image)
        {
            // from C# HttpClient 4.5 multipart/form-data upload http://stackoverflow.com/questions/16416601/c-sharp-httpclient-4-5-multipart-form-data-upload
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StreamContent(new MemoryStream(image)), "bilddatei", "upload.jpg");
                    using (var message = await client.PostAsync("http://www.directupload.net/index.php?mode=upload", content))
                    {
                        var input = await message.Content.ReadAsStringAsync();

                        return !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                    }
                }
            }
        }

        public static async Task Test_Upload_02(string url, string name, string file, string contentName, string contentValue)
        {
            Trace.WriteLine("Test_Upload_02");
            using (var client = new HttpClient())
            {
                Trace.WriteLine("create MultipartFormDataContent");
                using (var content = new MultipartFormDataContent("test upload"))
                {
                    Trace.WriteLine($"open file \"{file}\"");
                    string filename = zPath.GetFileName(file);
                    using (var fs = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Trace.WriteLine($"add content name \"{name}\" file \"{file}\"");
                        content.Add(new StreamContent(fs), name, filename);
                        content.Add(new StringContent(contentValue), contentName);
                        Trace.WriteLine("send request");
                        using (System.Net.Http.HttpResponseMessage response = await client.PostAsync(url, content))
                        {
                            Trace.WriteLine("read response");
                            var responseText = await response.Content.ReadAsStringAsync();

                            Trace.WriteLine($"response : {responseText}");
                        }
                    }
                }
            }
        }

        public static async Task Test_LoadToFile_01(string url, string file, string userAgent = null, string accept = null)
        {
            Trace.WriteLine($"load \"{url}\"");
            using (HttpClient httpClient = new HttpClient())
            {
                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Get, url))
                {
                    // User-Agent:"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                    // Accept : text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
                    // Accept-Language:"en-US,en;q=0.5"
                    //request.Headers.AcceptEncoding.Add("");
                    if (userAgent != null)
                        request.Headers.UserAgent.ParseAdd(userAgent);
                    if (accept != null)
                        request.Headers.Accept.ParseAdd(accept);
                    using (System.Net.Http.HttpResponseMessage response = await httpClient.SendAsync(request))
                    {
                        //string content = await response.Content.ReadAsStringAsync();
                        using (FileStream fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read))
                            await response.Content.CopyToAsync(fs);
                    }
                }
            }
        }

        public static void Test_LoadText_01(string url)
        {
            // error is not catched
            Try(async () => await _Test_LoadText_01(url));
        }

        public static async Task _Test_LoadText_01(string url)
        {
            //string url = "https://www.google.fr";
            Trace.WriteLine($"get http \"{url}\"");
            using (HttpClient httpClient = new HttpClient())
            {
                // Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0
                //HttpCompletionOption completionOption = new HttpCompletionOption();
                using (System.Net.Http.HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    TraceHttpResponse(response);
                    string content = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine();
                    Trace.WriteLine("  content :");
                    Trace.WriteLine("  " + content.Substring(0, 100) + " ...");
                }
            }
        }

        public static FormUrlEncodedContent CreateFormUrlEncodedContent(params string[] values)
        {
            int nb = values.Length / 2;
            KeyValuePair<string, string>[] keyValues = new KeyValuePair<string, string>[nb];
            for (int i = 0; i < nb; i++)
                keyValues[i] = new KeyValuePair<string, string>(values[i * 2], values[i * 2 + 1]);
            return new FormUrlEncodedContent(keyValues);
        }

        //private static FileStream _fsContent = null;
        //public static MultipartFormDataContent CreateMultipartFormDataContentFile(string boundary, string name, string file, params string[] values)
        //{
        //    MultipartFormDataContent content = new MultipartFormDataContent(boundary);
        //    _fsContent = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    content.Add(new StreamContent(_fsContent), name, zPath.GetFileName(file));
        //    name = null;
        //    foreach (string value in values)
        //    {
        //        if (name == null)
        //            name = value;
        //        else
        //        {
        //            content.Add(new StringContent(value), name);
        //            name = null;
        //        }
        //    }
        //    if (name != null)
        //        content.Add(new StringContent(name));
        //    //new MultipartFormDataContent("test upload")
        //    //content.Add(new StringContent(contentValue), contentName);
        //    return content;
        //}

        //public static MultipartFormDataContent CreateMultipartFormDataContent(string boundary, params string[] values)
        //{
        //    MultipartFormDataContent content = new MultipartFormDataContent(boundary);
        //    string name = null;
        //    foreach (string value in values)
        //    {
        //        if (name == null)
        //            name = value;
        //        else
        //        {
        //            content.Add(new StringContent(value), name);
        //            name = null;
        //        }
        //    }
        //    if (name != null)
        //        content.Add(new StringContent(name));
        //    //new MultipartFormDataContent("test upload")
        //    //content.Add(new StringContent(contentValue), contentName);
        //    return content;
        //}

        public static MultipartFormDataContent CreateMultipartFormDataContent(string boundary)
        {
            return new MultipartFormDataContent(boundary);
        }

        public static async void Test_LoadText_02(HttpMethod method, string url, HttpContent content = null)
        {
            // error is catched
            try
            {
                Trace.WriteLine($"load http url : \"{url}\"");
                using (HttpClient httpClient = new HttpClient())
                {
                    using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(method, url))
                    {
                        //if (content != null)
                        //    request.Content = new StringContent(content);
                        request.Content = content;
                        //string contentText = await content.ReadAsStringAsync();
                        //Trace.WriteLine($"content request : \"{contentText}\"");
                        Trace.WriteLine("wait for response");
                        using (System.Net.Http.HttpResponseMessage response = await httpClient.SendAsync(request))
                        {
                            Trace.WriteLine("wait to read content");
                            string responseContent = await response.Content.ReadAsStringAsync();
                            Trace.WriteLine($"response content : \"{responseContent}\"");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
            finally
            {
                //if (_fsContent != null)
                //{
                //    _fsContent.Close();
                //    _fsContent = null;
                //}
                CloseContentFileStreams();
            }
        }

        // Using Func delegate with Async method http://stackoverflow.com/questions/37280405/using-func-delegate-with-async-method
        // How to await on async delegate http://stackoverflow.com/questions/23285753/how-to-await-on-async-delegate
        public static void Try(Func<Task> action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        // exception is not catch -> runsource crash
        public static async void Test_04(string url)
        {
            Trace.WriteLine($"load http url : \"{url}\"");
            using (HttpClient httpClient = new HttpClient())
            {
                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Get, url))
                {
                    Trace.WriteLine("wait for response");
                    using (System.Net.Http.HttpResponseMessage response = await httpClient.SendAsync(request))
                    {
                        Trace.WriteLine("wait to read content");
                        string content = await response.Content.ReadAsStringAsync();
                        Trace.WriteLine($"content : \"{content}\"");
                    }
                }
            }
        }

        public static async Task Test_HttpClient_01(string url, string userAgent = null, string accept = null)
        {
            // http://stackoverflow.com/questions/10679214/how-do-you-set-the-content-type-header-for-an-httpclient-request
            using (HttpClient httpClient = new HttpClient())
            {
                using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Get, url))
                {
                    // User-Agent:"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0"
                    // Accept : text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
                    // Accept-Language:"en-US,en;q=0.5"
                    //request.Headers.AcceptEncoding.Add("");
                    if (userAgent != null)
                        request.Headers.UserAgent.ParseAdd(userAgent);
                    if (accept != null)
                        request.Headers.Accept.ParseAdd(accept);
                    //request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    TraceHttpRequest(request, "request");

                    using (System.Net.Http.HttpResponseMessage response = await httpClient.SendAsync(request))
                    {
                        //Trace.WriteLine();
                        //Trace.WriteLine();
                        //TraceHttpRequest(response.RequestMessage, "response.request");
                        //Trace.WriteLine();
                        Trace.WriteLine();
                        TraceHttpResponse(response);
                        string content = await response.Content.ReadAsStringAsync();
                        //Trace.WriteLine();
                        //Trace.WriteLine();
                        Trace.WriteLine("response content :");
                        TraceValue("length", content.Length);
                        Trace.WriteLine("  " + content.Substring(0, Math.Min(content.Length, 100)) + " ...");
                    }
                }
            }
        }

        //public static async Task Test_HttpLog(string url, string userAgent = null, string accept = null)
        //{
        //    //HttpLog_v3
        //    HttpResponseText responseText = await LoadText(url, userAgent, accept);
        //    try
        //    {
        //        HttpMessageResult httpLog = new HttpMessageResult(responseText.Response);
        //        Trace.WriteLine("httpLog :");
        //        httpLog.zTraceJson();
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteError(ex);
        //    }
        //}

        //public static async Task<HttpResponseText> LoadText(string url, string userAgent = null, string accept = null)
        //{
        //    Trace.WriteLine($"load \"{url}\"");
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Get, url))
        //        {
        //            if (userAgent != null)
        //                request.Headers.UserAgent.ParseAdd(userAgent);
        //            if (accept != null)
        //                request.Headers.Accept.ParseAdd(accept);
        //            using (System.Net.Http.HttpResponseMessage response = await httpClient.SendAsync(request))
        //            {
        //                string content = await response.Content.ReadAsStringAsync();
        //                return new HttpResponseText { Response = response, Content = content };
        //            }
        //        }
        //    }
        //}

        public static void TraceHttpRequest(System.Net.Http.HttpRequestMessage request, string from)
        {
            Trace.WriteLine($"request (from {from}) :");
            Trace.WriteLine($"  method : {request.Method}");
            Trace.WriteLine($"  url    : {request.RequestUri}");
            Trace.WriteLine("  request headers :");
            // UserAgent AcceptCharset AcceptEncoding AcceptLanguage Expect ExpectContinue Pragma Upgrade Via Warning
            TraceValue("user agent", request.Headers.UserAgent);
            TraceValue("accept", request.Headers.Accept);
            TraceValue("accept charset", request.Headers.AcceptCharset);
            TraceValue("accept encoding", request.Headers.AcceptEncoding);
            TraceValue("accept language", request.Headers.AcceptLanguage);
            TraceValue("expect", request.Headers.Expect);
            TraceValue("expect continue", request.Headers.ExpectContinue);
            TraceValue("pragma", request.Headers.Pragma);
            TraceValue("upgrade", request.Headers.Upgrade);
            TraceValue("via", request.Headers.Via);
            TraceValue("warning", request.Headers.Warning);
            TraceValue("authorization", request.Headers.Authorization);
            TraceValue("cache control", request.Headers.CacheControl);
            TraceValue("connection", request.Headers.Connection);
            TraceValue("date", request.Headers.Date);
            TraceValue("max forwards", request.Headers.MaxForwards);
            TraceValue("proxy authorization", request.Headers.ProxyAuthorization);
            TraceValue("range", request.Headers.Range);
            TraceValue("from", request.Headers.From);
            TraceValue("host", request.Headers.Host);
            TraceValue("referrer", request.Headers.Referrer);
            //Trace.WriteLine($"  user agent : {request.Headers.UserAgent}");
            //Trace.WriteLine($"  accept : {request.Headers.Accept}");
            //Trace.WriteLine($"  accept charset : {request.Headers.AcceptCharset}");
            //Trace.WriteLine($"  accept encoding : {request.Headers.AcceptEncoding}");
            //Trace.WriteLine($"  accept language : {request.Headers.AcceptLanguage}");
            //Trace.WriteLine($"  expect : {request.Headers.Expect}");
            //Trace.WriteLine($"  expect continue : {request.Headers.ExpectContinue}");
            //Trace.WriteLine($"  pragma : {request.Headers.Pragma}");
            //Trace.WriteLine($"  upgrade : {request.Headers.Upgrade}");
            //Trace.WriteLine($"  via : {request.Headers.Via}");
            //Trace.WriteLine($"  warning : {request.Headers.Warning}");
            //// Authorization CacheControl Connection Date
            //Trace.WriteLine($"  authorization : {request.Headers.Authorization}");
            //Trace.WriteLine($"  cache control : {request.Headers.CacheControl}");
            //Trace.WriteLine($"  connection : {request.Headers.Connection}");
            //Trace.WriteLine($"  date : {request.Headers.Date}");
            //// MaxForwards ProxyAuthorization Range
            //Trace.WriteLine($"  max forwards : {request.Headers.MaxForwards}");
            //Trace.WriteLine($"  proxy authorization : {request.Headers.ProxyAuthorization}");
            //Trace.WriteLine($"  range : {request.Headers.Range}");
            //// From Host Referrer
            //Trace.WriteLine($"  from : {request.Headers.From}");
            //Trace.WriteLine($"  host : {request.Headers.Host}");
            //Trace.WriteLine($"  referrer : {request.Headers.Referrer}");

            Trace.WriteLine("  request content :");
            if (request.Content == null)
            {
                Trace.WriteLine("  no content");
                return;
            }
            TraceValue("content length", request.Content.Headers.ContentLength);
            TraceValue("content type", request.Content.Headers.ContentType);
            TraceValue("expires", request.Content.Headers.Expires);
            TraceValue("last modified", request.Content.Headers.LastModified);
            TraceValue("allow", request.Content.Headers.Allow);
            TraceValue("content disposition", request.Content.Headers.ContentDisposition);
            TraceValue("content encoding", request.Content.Headers.ContentEncoding);
            TraceValue("content language", request.Content.Headers.ContentLanguage);
            TraceValue("content location", request.Content.Headers.ContentLocation);
            TraceValue("content range", request.Content.Headers.ContentRange);
            //Trace.WriteLine($"  content length : {request.Content.Headers.ContentLength}");
            //Trace.WriteLine($"  content type : {request.Content.Headers.ContentType}");
            //Trace.WriteLine($"  expires : {request.Content.Headers.Expires}");
            //Trace.WriteLine($"  last modified : {request.Content.Headers.LastModified}");
            //Trace.WriteLine($"  allow : {request.Content.Headers.Allow}");
            //Trace.WriteLine($"  content disposition : {request.Content.Headers.ContentDisposition}");
            //Trace.WriteLine($"  content encoding : {request.Content.Headers.ContentEncoding}");
            //Trace.WriteLine($"  content language : {request.Content.Headers.ContentLanguage}");
            //Trace.WriteLine($"  content location : {request.Content.Headers.ContentLocation}");
            //Trace.WriteLine($"  content range : {request.Content.Headers.ContentRange}");
        }

        public static void TraceHttpResponse(System.Net.Http.HttpResponseMessage response)
        {
            Trace.WriteLine("response :");
            TraceValue("status code", response.StatusCode);
            TraceValue("version", response.Version);
            TraceValue("reason phrase", response.ReasonPhrase);
            //Trace.WriteLine($"  status code : {response.StatusCode}");
            //Trace.WriteLine($"  version : {response.Version}");
            //Trace.WriteLine($"  reason phrase : {response.ReasonPhrase}");
            //Trace.WriteLine();
            Trace.WriteLine("response headers :");
            TraceValue("Age", response.Headers.Age);
            TraceValue("AcceptRanges", response.Headers.AcceptRanges);
            TraceValue("CacheControl", response.Headers.CacheControl);
            TraceValue("Connection", response.Headers.Connection);
            TraceValue("ConnectionClose", response.Headers.ConnectionClose);
            TraceValue("Date", response.Headers.Date);
            TraceValue("ETag", response.Headers.ETag);
            TraceValue("Location", response.Headers.Location);
            TraceValue("Pragma", response.Headers.Pragma);
            TraceValue("ProxyAuthenticate", response.Headers.ProxyAuthenticate);
            TraceValue("RetryAfter", response.Headers.RetryAfter);
            TraceValue("Server", response.Headers.Server);
            TraceValue("Trailer", response.Headers.Trailer);
            TraceValue("TransferEncoding", response.Headers.TransferEncoding);
            TraceValue("TransferEncodingChunked", response.Headers.TransferEncodingChunked);
            TraceValue("Upgrade", response.Headers.Upgrade);
            TraceValue("Vary", response.Headers.Vary);
            TraceValue("Via", response.Headers.Via);
            TraceValue("Warning", response.Headers.Warning);
            TraceValue("WwwAuthenticate", response.Headers.WwwAuthenticate);
            //Trace.WriteLine($"  Age   : {response.Headers.Age}");
            //Trace.WriteLine($"  AcceptRanges   : {response.Headers.AcceptRanges}");
            //Trace.WriteLine($"  CacheControl   : {response.Headers.CacheControl}");
            //Trace.WriteLine($"  Connection   : {response.Headers.Connection}");
            //Trace.WriteLine($"  ConnectionClose   : {response.Headers.ConnectionClose}");
            //Trace.WriteLine($"  Date   : {response.Headers.Date}");
            //Trace.WriteLine($"  ETag   : {response.Headers.ETag}");
            //Trace.WriteLine($"  Location   : {response.Headers.Location}");
            //Trace.WriteLine($"  Pragma   : {response.Headers.Pragma}");
            //Trace.WriteLine($"  ProxyAuthenticate   : {response.Headers.ProxyAuthenticate}");
            //Trace.WriteLine($"  RetryAfter   : {response.Headers.RetryAfter}");
            //Trace.WriteLine($"  Server   : {response.Headers.Server}");
            //Trace.WriteLine($"  Trailer   : {response.Headers.Trailer}");
            //Trace.WriteLine($"  TransferEncoding   : {response.Headers.TransferEncoding}");
            //Trace.WriteLine($"  TransferEncodingChunked   : {response.Headers.TransferEncodingChunked}");
            //Trace.WriteLine($"  Upgrade   : {response.Headers.Upgrade}");
            //Trace.WriteLine($"  Vary   : {response.Headers.Vary}");
            //Trace.WriteLine($"  Via   : {response.Headers.Via}");
            //Trace.WriteLine($"  Warning   : {response.Headers.Warning}");
            //Trace.WriteLine($"  WwwAuthenticate   : {response.Headers.WwwAuthenticate}");
            //Trace.WriteLine();
            Trace.WriteLine("response content headers :");
            TraceValue("Allow", response.Content.Headers.Allow);
            TraceValue("ContentDisposition", response.Content.Headers.ContentDisposition);
            TraceValue("ContentEncoding", response.Content.Headers.ContentEncoding);
            TraceValue("ContentLanguage", response.Content.Headers.ContentLanguage);
            TraceValue("ContentLength", response.Content.Headers.ContentLength);
            TraceValue("ContentLocation", response.Content.Headers.ContentLocation);
            TraceValue("ContentRange", response.Content.Headers.ContentRange);
            //TraceValue("ContentType", response.Content.Headers.ContentType);
            TraceValue("ContentType.CharSet", response.Content.Headers.ContentType.CharSet);
            TraceValue("ContentType.MediaType", response.Content.Headers.ContentType.MediaType);
            TraceValue("ContentType.Parameters", response.Content.Headers.ContentType.Parameters);
            TraceValue("Expires", response.Content.Headers.Expires);
            TraceValue("LastModified", response.Content.Headers.LastModified);
            //Trace.WriteLine($"  Allow   : {response.Content.Headers.Allow}");
            //Trace.WriteLine($"  ContentDisposition   : {response.Content.Headers.ContentDisposition}");
            //Trace.WriteLine($"  ContentEncoding   : {response.Content.Headers.ContentEncoding}");
            //Trace.WriteLine($"  ContentLanguage   : {response.Content.Headers.ContentLanguage}");
            //Trace.WriteLine($"  ContentLength   : {response.Content.Headers.ContentLength}");
            //Trace.WriteLine($"  ContentLocation   : {response.Content.Headers.ContentLocation}");
            //Trace.WriteLine($"  ContentRange   : {response.Content.Headers.ContentRange}");
            //Trace.WriteLine($"  ContentType   : {response.Content.Headers.ContentType}");
            //Trace.WriteLine($"  Expires   : {response.Content.Headers.Expires}");
            //Trace.WriteLine($"  LastModified   : {response.Content.Headers.LastModified}");
        }

        private static void TraceValue(string label, object value)
        {
            if (value == null)
                return;
            string s = value.ToString();
            if (s != "")
                Trace.WriteLine($"  {label,-30} : {s}");
        }
    }

    public static class Test_HttpClient_Extension
    {
        public static MultipartFormDataContent zAddStringContent(this MultipartFormDataContent content, params string[] values)
        {
            string name = null;
            foreach (string value in values)
            {
                if (name == null)
                    name = value;
                else
                {
                    content.Add(new StringContent(value), name);
                    name = null;
                }
            }
            if (name != null)
                content.Add(new StringContent(name));
            return content;
        }

        public static MultipartFormDataContent zAddUploadFile(this MultipartFormDataContent content, string name, string file)
        {
            FileStream fs = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            Test_HttpClient.AddContentFileStream(fs);
            content.Add(new StreamContent(fs), name, zPath.GetFileName(file));
            return content;
        }
    }
}

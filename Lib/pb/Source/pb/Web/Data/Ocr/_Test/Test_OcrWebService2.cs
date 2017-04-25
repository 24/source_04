using MongoDB.Bson.Serialization;
using pb.Data.Mongo;
using pb.Data.Xml;
using System;
using System.IO;
using System.Net;
using System.Text;

// 69.64.69.77
// ip.addr == 69.64.69.77
//GET /restservices/getAccountInformation HTTP/1.1
//Authorization: Basic cGJhbWJvdToxQzJDMTQwRS1CMTlFLTQ2QjMtQTM0Mi03NkZGMUM1MDQ1M0U =
//Host: www.ocrwebservice.com

//GET /restservices/getaccountinformation HTTP/1.1
//User-Agent: pib/0.1
//Authorization: Basic Og==
//Host: www.ocrwebservice.com


namespace pb.Web.Data.Ocr.Test
{
    public static class Test_OcrWebService2
    {
        private static string _configFile = @"c:\pib\drive\google\dev_data\exe\runsource\ocr\config\ocrWebService.config.local.xml";
        private static string _urlAccountInformation = "http://www.ocrwebservice.com/restservices/getAccountInformation";
        private static string _urlProcessDocument = "https://www.ocrwebservice.com/restservices/processDocument";

        public static void Test_AccountInformation()
        {
            try
            {
                HttpWebRequest request = CreateHttpWebRequest(_urlAccountInformation, "GET");

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string responseTxt = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    //OCRResponseAccountInfo ocrResponse = JsonConvert.DeserializeObject<OCRResponseAccountInfo>(strJSON);
                    OcrAccountInfoResponse responseData = BsonSerializer.Deserialize<OcrAccountInfoResponse>(responseTxt);

                    Trace.WriteLine($"Available pages      : {responseData.AvailablePages}");
                    Trace.WriteLine($"Max pages            : {responseData.MaxPages}");
                    Trace.WriteLine($"Expiration date      : {responseData.ExpirationDate}");
                    Trace.WriteLine($"Last processing time : {responseData.LastProcessingTime}");
                }
            }
            catch (WebException wex)
            {
                Trace.WriteLine($"Error : HTTPCode {((HttpWebResponse)wex.Response).StatusCode}");
            }
        }

        public static void Test_ProcessDocument(string inputFile)
        {
            try
            {
                string url = _urlProcessDocument + "?language=french,english&outputformat=doc,txt&gettext=true";
                HttpWebRequest request = CreateHttpWebRequest(url, "POST");
                byte[] inputData = ReadFile(inputFile);
                request.ContentLength = inputData.Length;

                //  Send request
                using (Stream post = request.GetRequestStream())
                {
                    post.Write(inputData, 0, inputData.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string responseTxt = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    OcrProcessDocumentResponse responseData = BsonSerializer.Deserialize<OcrProcessDocumentResponse>(responseTxt);
                    responseData.zTraceJson();
                }
            }
            catch (WebException wex)
            {
                Trace.WriteLine($"Error : HTTPCode {((HttpWebResponse)wex.Response).StatusCode}");
            }
        }

        private static byte[] ReadFile(string file)
        {
            FileStream streamContent = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[streamContent.Length];
            streamContent.Read(data, 0, (int)streamContent.Length);
            return data;
        }

        private static HttpWebRequest CreateHttpWebRequest(string url, string method)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);

            XmlConfig config = new XmlConfig(_configFile);
            byte[] authBytes = Encoding.UTF8.GetBytes($"{config.GetExplicit("UserName")}:{config.GetExplicit("LicenseCode")}".ToCharArray());
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(authBytes);
            request.Method = method;
            request.Timeout = 600000;

            // Specify Response format to JSON or XML (application/json or application/xml)
            //request.ContentType = "application/json";
            return request;
        }

        public static void Test_Authorization()
        {
            XmlConfig config = new XmlConfig(_configFile);
            string txt = $"{config.GetExplicit("UserName")}:{config.GetExplicit("LicenseCode")}";
            Trace.WriteLine($"txt : \"{txt}\" {txt.Length}");
            byte[] authBytes = Encoding.UTF8.GetBytes(txt.ToCharArray());
            Trace.WriteLine($"convert to bytes : {authBytes.Length}");
            string base64String = Convert.ToBase64String(authBytes);
            Trace.WriteLine($"convert to base 64 string : \"{base64String}\" {base64String.Length}");
            string authorization = "Basic " + base64String;
            // authorization : "Basic cGJhbWJvdToxQzJDMTQwRS1CMTlFLTQ2QjMtQTM0Mi03NkZGMUM1MDQ1M0U=" 66
            //Authorization:    Basic cGJhbWJvdToxQzJDMTQwRS1CMTlFLTQ2QjMtQTM0Mi03NkZGMUM1MDQ1M0U =
            Trace.WriteLine($"authorization : \"{authorization}\" {authorization.Length}");
        }
    }
}

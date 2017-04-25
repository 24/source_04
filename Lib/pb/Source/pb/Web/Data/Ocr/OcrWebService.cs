using MongoDB.Bson.Serialization;
using pb.Web.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace pb.Web.Data.Ocr
{
    /*

      You should specify OCR settings. See full description http://www.ocrwebservice.com/api/restguide  http://www.ocrwebservice.com/api/keyfeatures

      REST method :
                    processDocument       (POST)   http(s)://www.ocrwebservice.com/restservices/processDocument
                    getAccountInformation (GET)    http(s)://www.ocrwebservice.com/restservices/getAccountInformation


      REST method : processDocument (POST)  http(s)://www.ocrwebservice.com/restservices/processDocument

      Maximum input file size : 100 MB

      Input parameters:

      [language]      - Specifies the recognition language. 
                        This parameter can contain several language names separated with commas. 
                        For example "language=english,german,spanish".
                        Optional parameter. By default:english

      [pagerange]     - Enter page numbers and/or page ranges separated by commas. 
                        For example "pagerange=1,3,5-12" or "pagerange=allpages".
                        Optional parameter. By default:allpages

      [tobw]	  	   - Convert image to black and white (recommend for color image and photo). 
                        For example "tobw=false"
                        Optional parameter. By default:false

      [zone]          - Specifies the region on the image for zonal OCR. 
                        The coordinates in pixels relative to the left top corner in the following format: top:left:height:width. 
                        This parameter can contain several zones separated with commas. 
                        For example "zone=0:0:100:100,50:50:50:50"
                        Optional parameter.

      [outputformat]  - Specifies the output file format.
                        Can be specified up to two output formats, separated with commas.
                        For example "outputformat=pdf,txt"
                        Optional parameter. By default:doc
                        Adobe PDF text document                           pdf
                        Adobe PDF with image and text                     pdfimg
                        Microsoft Word 2003                               doc
                        Microsoft Word 2007                               docx
                        Microsoft Excel 2003                              xls
                        Microsoft Excel 2007                              xlsx
                        RTF document                                      rtf
                        Text plain                                        txt

      [gettext]	      - Specifies that extracted text will be returned.
                        For example "tobw=true"
                        Optional parameter. By default:false

      [getwords]      - If it is TRUE the recognized word`s coordinates will be returned.
                        default:false

      [newline]       - If it is "newline=1" the extracted text will be returned with new line (\n) character
                        default:false

      [description]   - Specifies your task description. Will be returned in response.
                        Optional parameter. 

      HTTP status code :
        200 - Success request
        400 - Bad Request
        401 - Unauthorized Request
        402 - Payment Required
        500 - Internal Server Error

      Response formats :
        XML  - Application/xml in the HTTP Accept header
        JSON - Application/json in the HTTP Accept header


      !!!!  For getting result you must specify "gettext" or "outputformat" !!!!  

      gettext=true
      language=english,german
      zone=0:0:600:400,500:1000:150:400
      pagerange=1-5
      outputformat=doc,txt

      For SSL using :
      ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });


      REST method : getAccountInformation (GET)  http(s)://www.ocrwebservice.com/restservices/getAccountInformation

    */

    public class OcrRequest
    {
        public string DocumentFile = null;
        public string Language = null;      // english,french,german
        public string OutputFormat = null;  // docx,txt
        public bool GetText = false;
        public bool GetWords = false;
        public bool BlackAndWhite = false;
        public bool NewLine = false;
        public string PageRange = null;       // "pagerange=1,3,5-12" or "pagerange=allpages"
        public string Zone = null;           // format: "top:left:height:width,...", example "zone=0:0:100:100,50:50:50:50"
        public string Description = null;
    }

    public class OcrResult<T>
    {
        public bool Success;
        public int StatusCode;
        public T Data;
    }

    public class OcrProcessDocumentResponse
    {
        public int AvailablePages;
        public int ProcessedPages;
        public string OutputFileUrl;
        public string OutputFileUrl2;
        public string OutputFileUrl3;
        public string TaskDescription;
        public string ErrorMessage;
        public string OutputInformation;
        public List<List<string>> OCRText = new List<List<string>>();   // text from each zone and page, OCRText[zone][page]
        public List<List<OcrWord>> OCRWords = new List<List<OcrWord>>();
        public List<List<string>> Reserved = new List<List<string>>();
    }

    public class OcrWord
    {
        public int Top;
        public int Left;
        public int Height;
        public int Width;
        public string OCRWord;
    }

    public class OcrAccountInfoResponse
    {
        public int AvailablePages;
        public int MaxPages;
        public string LastProcessingTime;
        public string SubcriptionPlan;
        public string ExpirationDate;
        public string ErrorMessage;
    }

    public class OcrWebService
    {
        private const int _defaultTimeout = 600; // Timeout 600" = 10 min
        private HttpManager_v5 _httpManager = null;
        private string _userName = null;
        private string _licenseCode = null;

        //private const string _url = "https://www.ocrwebservice.com/restservices/";
        private const string _url = "http://www.ocrwebservice.com/restservices/";
        private const string _processDocumentMethod = "processdocument";
        private const string _accountInfoMethod = "getaccountinformation";
        private const string _contentType = "application/json";

        public HttpManager_v5 HttpManager { get { return _httpManager; } }
        //public string UserName { get { return _userName; } set { _userName = value; } }
        public string UserName { get { return _userName; } }
        //public string LicenseCode { get { return _licenseCode; } set { _licenseCode = value; } }
        public string LicenseCode { get { return _licenseCode; } }

        public OcrWebService(string userName, string licenseCode, int timeout = _defaultTimeout)
        {
            _userName = userName;
            _licenseCode = licenseCode;
            _httpManager = new HttpManager_v5();
            SetHttpClient(timeout);
        }

        private void SetHttpClient(int timeout)
        {
            _httpManager.HttpClient.Timeout = TimeSpan.FromSeconds(timeout);      // Timeout 600" = 10 min
            _httpManager.SetDefaultHeaders();
            //_httpManager.HttpClient.DefaultRequestHeaders.Accept.zSet("application/json");
            _httpManager.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetAuthorization());
        }

        private string GetAuthorization()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_userName}:{_licenseCode}".ToCharArray()));
        }

        public async Task<OcrResult<OcrAccountInfoResponse>> AccountInfo()
        {
            string url = _url + _accountInfoMethod;
            // problem : impossible to set ContentType (mediaType) with GET method
            // Cannot send a content-body with this verb-type. (System.Net.ProtocolViolationException)
            // Content = new TextContent("", mediaType: _contentType)
            HttpResult_v5<string> httpResult = await _httpManager.LoadText(new HttpRequest_v5 { Url = url });
            OcrResult<OcrAccountInfoResponse> result = new OcrResult<OcrAccountInfoResponse> { Success = httpResult.Success, StatusCode = httpResult.StatusCode };
            if (httpResult.Success)
            {
                result.Data = BsonSerializer.Deserialize<OcrAccountInfoResponse>(httpResult.Data);
            }
            return result;
        }

        public async Task<OcrResult<OcrProcessDocumentResponse>> ProcessDocument(OcrRequest request)
        {
            string url = _url + _processDocumentMethod + "?" + zurl.BuildQuery(GetQueryValues(request));
            HttpRequest_v5 httpRequest = new HttpRequest_v5 { Url = url, Method = HttpRequestMethod.Post, Content = new FileContent(request.DocumentFile) { ContentType = _contentType } };
            HttpResult_v5<string> httpResult = await _httpManager.LoadText(httpRequest);
            OcrResult<OcrProcessDocumentResponse> result = new OcrResult<OcrProcessDocumentResponse> { Success = httpResult.Success, StatusCode = httpResult.StatusCode };
            if (httpResult.Success)
            {
                result.Data = BsonSerializer.Deserialize<OcrProcessDocumentResponse>(httpResult.Data);
            }
            return result;
        }

        public async Task DownloadResultDocuments(OcrProcessDocumentResponse response, string file)
        {
            if (response.OutputFileUrl != null && response.OutputFileUrl != "")
                await _httpManager.LoadToFile(new HttpRequest_v5 { Url = response.OutputFileUrl }, file + zurl.GetExtension(response.OutputFileUrl));
            if (response.OutputFileUrl2 != null && response.OutputFileUrl2 != "")
                await _httpManager.LoadToFile(new HttpRequest_v5 { Url = response.OutputFileUrl2 }, file + zurl.GetExtension(response.OutputFileUrl2));
            if (response.OutputFileUrl3 != null && response.OutputFileUrl3 != "")
                await _httpManager.LoadToFile(new HttpRequest_v5 { Url = response.OutputFileUrl3 }, file + zurl.GetExtension(response.OutputFileUrl3));
        }


        private IEnumerable<KeyValuePair<string, string>> GetQueryValues(OcrRequest request)
        {
            if (request.Language != null)
                yield return new KeyValuePair<string, string>("language", request.Language);
            if (request.OutputFormat != null)
                yield return new KeyValuePair<string, string>("outputformat", request.OutputFormat);
            if (request.GetText)
                yield return new KeyValuePair<string, string>("gettext", "true");
            if (request.GetWords)
                yield return new KeyValuePair<string, string>("getwords", "true");
            if (request.BlackAndWhite)
                yield return new KeyValuePair<string, string>("tobw", "true");
            if (request.NewLine)
                yield return new KeyValuePair<string, string>("newline", "true");
            if (request.PageRange != null)
                yield return new KeyValuePair<string, string>("pagerange", request.PageRange);
            if (request.Zone != null)
                yield return new KeyValuePair<string, string>("zone", request.Zone);
            if (request.Description != null)
                yield return new KeyValuePair<string, string>("description", request.Description);
        }
    }
}

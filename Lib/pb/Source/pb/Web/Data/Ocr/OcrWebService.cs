using pb.Web.Http;
using System;
using System.Text;

namespace pb.Web.Data.Ocr
{
    /*

      You should specify OCR settings. See full description http://www.ocrwebservice.com/service/restguide

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

      [gettext]	      - Specifies that extracted text will be returned.
                        For example "tobw=true"
                        Optional parameter. By default:false

      [description]   - Specifies your task description. Will be returned in response.
                        Optional parameter. 

      [getwords]      - If it is TRUE the recognized word`s coordinates will be returned.
                        default:false

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
    */

    public class OcrWebServiceManager
    {
        private string _userName = null;
        private string _licenseCode = null;
        private HttpManager_v2 _httpManager = null;
        //HttpRequestParameters _requestParameters = null;

        private void Init()
        {
            _httpManager = new HttpManager_v2();
            _httpManager.RequestParameters = GetHttpRequestParameters();
            //_httpManager.LoadRetryTimeout
            //request.Timeout = 600000;
        }

        private HttpRequestParameters GetHttpRequestParameters()
        {
            HttpRequestParameters requestParameters = new HttpRequestParameters();
            requestParameters.Accept = "application/json";
            requestParameters.ContentType = "application/json";
            requestParameters.Encoding = Encoding.UTF8;
            requestParameters.Headers["Authorization"] = GetAuthorization();
            return requestParameters;
        }

        private string GetAuthorization()
        {
            byte[] authBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _userName, _licenseCode).ToCharArray());
            return "Basic " + Convert.ToBase64String(authBytes);
        }
    }

    public class OcrWebService
    {
        private const string _processDocumentUrl = "http://www.ocrwebservice.com/restservices/processDocument?";
        private string _language = null;
        private string _pagerange = null;
        private string _outputformat = null;
        private string _documentFile = null;

        private HttpManager_v2 _httpManager = null;

        //request.ContentType = "application/json";
        public void Request()
        {
        }

    }
}

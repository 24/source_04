using pb;
using pb.IO;
using pb.Web.Data.Ocr;
using pb.Web.Http;
using System.Threading.Tasks;

namespace anki
{
    public class ScanManager
    {
        private const int _timeout = 1200; // Timeout 1200" = 20 min
        private OcrWebService _ocrWebService = null;
        private string _language = null;
        private string _outputFormat = null;

        public string Language { get { return _language; } set { _language = value; } }
        public string OutputFormat { get { return _outputFormat; } set { _outputFormat = value; } }

        public ScanManager(string userName, string licenseCode, UrlCache urlCache)
        {
            _ocrWebService = new OcrWebService(userName, licenseCode, _timeout);
            _ocrWebService.HttpManager.SetCacheManager(urlCache);
        }

        public async Task ScanPdf(string pdfFile, string range, string scanFile)
        {
            OcrRequest request = CreateOcrRequest();
            request.DocumentFile = pdfFile;
            request.PageRange = range;

            OcrResult<OcrProcessDocumentResponse> response = await _ocrWebService.ProcessDocument(request);
            if (response.Success)
            {
                //string directory = GetScanDirectory();
                //zdir.CreateDirectory(scanDirectory);
                zfile.CreateFileDirectory(scanFile);
                //string scanFile = zPath.Combine(scanDirectory, "scan");
                Trace.WriteLine($"save scan to \"{scanFile}\"");
                await _ocrWebService.DownloadResultDocuments(response.Data, scanFile);
                Trace.WriteLine($"scan ok {response.Data.ProcessedPages} pages - remainder {response.Data.AvailablePages} pages");
            }
            else
                Trace.WriteLine($"scan error code {response.StatusCode}");
        }

        public async Task ScanImage(string imageFile, string scanFile)
        {
            OcrRequest request = CreateOcrRequest();
            request.DocumentFile = imageFile;

            OcrResult<OcrProcessDocumentResponse> response = await _ocrWebService.ProcessDocument(request);
            if (response.Success)
            {
                //string scanDirectory = GetScanDirectory();
                //zdir.CreateDirectory(scanDirectory);
                zfile.CreateFileDirectory(scanFile);
                //string scanFile = zPath.Combine(scanDirectory, $"scan-page-{page:000}");
                Trace.WriteLine($"save scan to \"{scanFile}\"");
                await _ocrWebService.DownloadResultDocuments(response.Data, scanFile);
                Trace.WriteLine($"scan ok {response.Data.ProcessedPages} pages - remainder {response.Data.AvailablePages} pages");
            }
        }

        private OcrRequest CreateOcrRequest()
        {
            return new OcrRequest { Language = _language, OutputFormat = _outputFormat };
        }
    }
}

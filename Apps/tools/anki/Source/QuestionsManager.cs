using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Pdf;
using pb.Data.Xml;
using pb.IO;
using pb.Linq;
using pb.Text;
using pb.Web.Data.Ocr;
using pb.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace anki
{
    public enum PageRotate
    {
        NoRotate,
        Rotate90,
        Rotate180,
        Rotate270
    }

    public class QuestionsParameters
    {
        public string PageRange;
        public PageRotate PageRotate;
        public int PageColumn = 1;
        public bool ImagesExtracted;
    }

    public class QuestionsManager
    {
        private const int _timeout = 1200; // Timeout 1200" = 20 min
        private static string[] _imageExtensions = new string[] { ".jpg", ".png" };
        private string _baseDirectory = null;
        private string _directory = null;
        private string _filename = null;
        private int _maxLinesPerQuestion = -1;
        private int _maxLinesPerResponse = -1;
        private RegexValuesList _questionRegexValuesList = null;
        private RegexValuesList _responseRegexValuesList = null;

        private int _yearWidth = 12;

        private QuestionResponseFiles _questionResponseFiles = null;

        public string BaseDirectory { get { return _baseDirectory; } set { _baseDirectory = value; } }
        public string Directory { get { return _directory; } set { _directory = value; } }
        public int MaxLinesPerQuestion { get { return _maxLinesPerQuestion; } set { _maxLinesPerQuestion = value; } }
        public int MaxLinesPerResponse { get { return _maxLinesPerResponse; } set { _maxLinesPerResponse = value; } }
        public RegexValuesList QuestionRegexValuesList { get { return _questionRegexValuesList; } set { _questionRegexValuesList = value; } }
        public RegexValuesList ResponseRegexValuesList { get { return _responseRegexValuesList; } set { _responseRegexValuesList = value; } }

        //public QuestionsManager(string baseDirectory, string directory, RegexValuesList questionRegexValuesList, RegexValuesList responseRegexValuesList)
        //{
        //    _baseDirectory = baseDirectory;
        //    _directory = zPath.Combine(baseDirectory, directory);
        //    _filename = zPath.GetFileName(directory);
        //    _questionRegexValuesList = questionRegexValuesList;
        //    _responseRegexValuesList = responseRegexValuesList;
        //}

        public void SetParameters(QuestionsParameters parameters)
        {
            string file = GetParametersFile();
            Trace.WriteLine($"save parameters to \"{file}\"");
            parameters.zSave(file, jsonIndent: true);
        }

        public QuestionsParameters GetParameters()
        {
            string file = GetParametersFile();
            if (!zFile.Exists(file))
                //throw new PBException($"parameters file not found \"{file}\"");
                return null;
            else
                return zMongo.ReadFileAs<QuestionsParameters>(file);
        }

        //string range = null
        public void ExtractImagesFromPdf()
        {
            QuestionsParameters parameters = GetParameters();
            if (parameters == null)
            {
                Trace.WriteLine($"parameters are not defined");
                return;
            }
            if (parameters.PageRange == null)
            {
                Trace.WriteLine($"page range is not defined");
                return;
            }
            ExtractImagesFromPdf(parameters);
        }

        // string range
        private void ExtractImagesFromPdf(QuestionsParameters parameters)
        {
            string pdfFile = GetPdfFile();
            //if (range == null)
            //    range = parameters.PageRange;
            Trace.WriteLine($"extract images from pdf \"{pdfFile}\" range \"{parameters.PageRange}\"");
            string directory = GetImagesDirectory();
            zdir.CreateDirectory(directory);
            //zfile.DeleteFiles(directory, "*.jpeg");
            DeleteImageFiles(directory);
            //iText.ExtractImages(pdfFile,
            //    (image, page, imageIndex) => image.Save(zPath.Combine(directory, $"page-{page:000}{(imageIndex != 1 ? $"-{imageIndex:00}" : "")}.jpeg"), ImageFormat.Jpeg),
            //    range: range);

            foreach (PdfImage image in iText7.EnumImages(pdfFile, parameters.PageRange))
            {
                iText7.SaveImage(image.Image, zPath.Combine(directory, $"page-{image.PageNumber:000}{(image.ImageNumber != 1 ? $"-{image.ImageNumber:00}" : "")}"));
            }
            parameters.ImagesExtracted = true;
            SetParameters(parameters);
        }

        public async Task Scan(bool imageScan = false, string range = null, bool simulate = false)
        {
            QuestionsParameters parameters = GetParameters();
            if (parameters == null)
            {
                Trace.WriteLine($"parameters are not defined");
                return;
            }
            if (parameters.PageRange == null)
            {
                Trace.WriteLine($"page range is not defined");
                return;
            }
            if (parameters.PageColumn == 1 && !imageScan)
                await ScanPdf(parameters, range, simulate);
            else
            {
                if (!parameters.ImagesExtracted)
                    ExtractImagesFromPdf(parameters);
                await ScanImages(parameters, range, simulate);
            }
        }

        private async Task ScanPdf(QuestionsParameters parameters, string range, bool simulate = false)
        {
            string pdfFile = GetPdfFile();
            if (!zFile.Exists(pdfFile))
            {
                Trace.WriteLine($"pdf file not found \"{pdfFile}\"");
                return;
            }
            if (range == null)
                range = parameters.PageRange;
            Trace.WriteLine($"scan pdf \"{pdfFile}\" range \"{range}\"");
            OcrWebService ocrWebService = CreateOcrWebService();
            OcrRequest request = CreateOcrRequest();
            request.DocumentFile = pdfFile;
            request.PageRange = range;
            if (!simulate)
            {
                OcrResult<OcrProcessDocumentResponse> response = await ocrWebService.ProcessDocument(request);
                if (response.Success)
                {
                    Trace.WriteLine($"scan ok {response.Data.ProcessedPages} pages - remainder {response.Data.AvailablePages} pages");
                    string directory = GetScanDirectory();
                    zdir.CreateDirectory(directory);
                    string scanFile = zPath.Combine(directory, "scan");
                    Trace.WriteLine($"save scan to \"{scanFile}\"");
                    await ocrWebService.DownloadResultDocuments(response.Data, scanFile);
                }
                else
                    Trace.WriteLine($"scan error code {response.StatusCode}");
            }
            else
            {
                Trace.WriteLine("OcrRequest :");
                request.zTraceJson();
            }
        }

        private async Task ScanImages(QuestionsParameters parameters, string range, bool simulate = false)
        {
            string directory = GetImagesDirectory();
            if (!zDirectory.Exists(directory))
            {
                Trace.WriteLine($"images directory not found \"{directory}\"");
                return;
            }
            if (range == null)
                range = parameters.PageRange;
            Trace.WriteLine($"scan images \"{directory}\" range \"{range}\"");
            OcrWebService ocrWebService = CreateOcrWebService();
            OcrRequest request = CreateOcrRequest();
            //QuestionsParameters parameters = GetParameters();
            foreach (int page in zstr.EnumRange(range))
            {
                //string imageFile = zPath.Combine(directory, $"page-{page:000}.jpeg");
                string imageBaseFile = zPath.Combine(directory, $"page-{page:000}");
                string imageFile = FindImageFile(imageBaseFile);
                if (!zFile.Exists(imageFile))
                {
                    Trace.WriteLine($"image not found \"{imageBaseFile}\"");
                    return;
                }
                Trace.WriteLine($"scan image \"{imageFile}\"");
                request.DocumentFile = imageFile;
                request.Zone = GetScanZone(parameters, imageFile);
                if (!simulate)
                {
                    OcrResult<OcrProcessDocumentResponse> response = await ocrWebService.ProcessDocument(request);
                    if (response.Success)
                    {
                        string scanDirectory = GetScanDirectory();
                        zdir.CreateDirectory(scanDirectory);
                        string scanFile = zPath.Combine(scanDirectory, $"scan-page-{page:000}");
                        Trace.WriteLine($"save scan to \"{scanFile}\"");
                        await ocrWebService.DownloadResultDocuments(response.Data, scanFile);
                    }
                }
                else
                {
                    Trace.WriteLine("OcrRequest :");
                    request.zTraceJson();
                }
            }
        }

        private static string FindImageFile(string file)
        {
            foreach (string extension in _imageExtensions)
            {
                string file2 = file + extension;
                if (zFile.Exists(file2))
                    return file2;
            }
            return null;
        }

        private static void DeleteImageFiles(string directory)
        {
            foreach (string extension in _imageExtensions)
                zfile.DeleteFiles(directory, "*" + extension);
        }

        private static string GetScanZone(QuestionsParameters parameters, string imageFile)
        {
            // format: "top:left:height:width,...", example "zone=0:0:100:100,50:50:50:50"
            if (parameters.PageColumn == 1)
                return null;
            else if (parameters.PageColumn == 2)
            {
                int width;
                int height;
                zimg.GetImageWidthHeight(imageFile, out width, out height);
                string zone;
                switch (parameters.PageRotate)
                {
                    case PageRotate.NoRotate:
                    case PageRotate.Rotate180:
                        int width2 = width / 2;
                        zone = $"0:0:{width2}:{height},{width2}:0:{width - width2}:{height}";
                        break;
                    case PageRotate.Rotate90:
                    case PageRotate.Rotate270:
                        int height2 = height / 2;
                        zone = $"0:0:{width}:{height2},0:{height2}:{width}:{height - height2}";
                        break;
                    default:
                        throw new PBException($"unknow page rotation {parameters.PageRotate}");
                }
                return zone;
            }
            else
                throw new PBException($"can't create scan zone for {parameters.PageColumn} columns");
        }

        private OcrRequest CreateOcrRequest()
        {
            return new OcrRequest { Language = "french,english", OutputFormat = "txt" };
        }

        private OcrWebService CreateOcrWebService()
        {
            XmlConfig config = XmlConfig.CurrentConfig;
            XmlConfig ocrWebServiceConfig = config.GetConfig("OcrWebServiceConfig");
            OcrWebService ocrWebService = new OcrWebService(ocrWebServiceConfig.GetExplicit("UserName"), ocrWebServiceConfig.GetExplicit("LicenseCode"), _timeout);
            //ocrWebService.UserName = ocrWebServiceConfig.GetExplicit("UserName");
            //ocrWebService.LicenseCode = ocrWebServiceConfig.GetExplicit("LicenseCode");
            string cacheDirectory = config.Get("OcrWebServiceCacheDirectory");
            if (cacheDirectory != null)
            {
                UrlCache urlCache = new UrlCache(cacheDirectory);
                urlCache.UrlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
                if (config.Get("OcrWebServiceCacheDirectory/@option")?.ToLower() == "indexedfile")
                    urlCache.IndexedFile = true;
                ocrWebService.HttpManager.SetCacheManager(urlCache);
            }
            return ocrWebService;
        }

        public void CreateAnkiFileFromScanFiles()
        {
            //string ankiFile = zPath.Combine(_directory, GetFileName() + ".anki.txt");
            string ankiFile = GetAnkiFile();
            Trace.WriteLine($"create anki file \"{ankiFile}\" from scan files");
            // questionResponseFiles
            // Response = questionResponse.Response.GetFormatedResponse()
            AnkiWriter.Write(ankiFile, GetQuestionResponses()
                .Select(questionResponse => new AnkiQuestion { Question = questionResponse.GetHtml(questionNumber: true, replaceSpecialCharacters: true), Response = questionResponse.Response != null ? questionResponse.Response.GetFormatedResponse() : "(unknow response)" }));
        }

        public string CreateAnkiFileFromQuestionFiles()
        {
            //string ankiFile = zPath.Combine(_directory, GetFileName() + ".anki.txt");
            string ankiFile = GetAnkiFile();
            Trace.WriteLine($"create anki file \"{ankiFile}\" from questions files");
            // zPath.Combine(_directory, "data")
            // GetQuestionsDirectory()
            AnkiWriter.Write(ankiFile, QuestionResponses.GetQuestionFiles(_directory).Select(file => QuestionResponses.LoadQuestion(_directory, file))
                .Select(questionResponseHtml => new AnkiQuestion { Question = $"<h1>{questionResponseHtml.Year} - question no {questionResponseHtml.Number}</h1><br>" + questionResponseHtml.QuestionHtml,
                    Response = Response.GetFormatedResponse(questionResponseHtml.Responses) }));
            return ankiFile;
        }

        //private string GetQuestionsDirectory()
        //{
        //    //return zPath.Combine(_directory, @"data\questions");
        //    return QuestionResponses.GetQuestionsDirectory(_directory);
        //}

        //private Regex _numericHeaderScanFile = new Regex("^[0-9]+_", RegexOptions.Compiled);
        private Regex _pageNumberScanFile = new Regex("-page-[0-9]{3}.txt$", RegexOptions.Compiled);
        public void RenameAndCopyScanFiles(bool simulate = false)
        {
            string directory = GetScanDirectory();
            if (!zDirectory.Exists(directory))
                throw new PBException($"scan directory not found (\"{directory}\")");
            //string name = zPath.GetFileName(_directory);
            string name = "scan";
            foreach (string file in zDirectory.EnumerateFiles(directory))
            {
                string filename = zPath.GetFileName(file);
                //Match match1 = _numericHeaderScanFile.Match(filename);
                //if (match1.Success)
                //{
                    Match match2 = _pageNumberScanFile.Match(filename);
                    if (match2.Success)
                    {
                        //string file2 = zPath.Combine(directory, match1.zReplace(filename, ""));
                        string file2 = zPath.Combine(directory, name +  match2.Value);

                        //if (file == file2)
                        //    continue;

                        if (file != file2)
                        {
                            if (zFile.Exists(file2))
                                throw new PBException($"can't rename file \"{zPath.GetFileName(file)}\" file already exists \"{zPath.GetFileName(file2)}\"");
                            Trace.WriteLine($"{(simulate ? "simulate " : "")}rename file \"{zPath.GetFileName(file)}\" to \"{zPath.GetFileName(file2)}\"");
                            if (!simulate)
                                zFile.Move(file, file2);
                        }

                        string file3 = zPath.Combine(directory, zPath.GetFileNameWithoutExtension(file2) + "_02" + zPath.GetExtension(file2));

                        // check if file_02 exists
                        string file4 = zPath.Combine(directory, zPath.GetFileNameWithoutExtension(file) + "_02" + zPath.GetExtension(file));
                        if (zFile.Exists(file4))
                        {
                            if (file4 != file3)
                            {
                                Trace.WriteLine($"{(simulate ? "simulate " : "")}rename file \"{zPath.GetFileName(file4)}\" to \"{zPath.GetFileName(file3)}\"");
                                if (!simulate)
                                    zFile.Move(file4, file3);
                            }
                        }
                        else
                        {
                            // copy file to *_02.txt
                            if (zFile.Exists(file3))
                                throw new PBException($"can't copy file \"{zPath.GetFileName(file2)}\" file already exists \"{zPath.GetFileName(file3)}\"");
                            Trace.WriteLine($"{(simulate ? "simulate " : "")}copy file \"{zPath.GetFileName(file2)}\" to \"{zPath.GetFileName(file3)}\"");
                            if (!simulate)
                                zFile.Copy(file2, file3);
                        }
                    }
                //}
            }
        }

        public void CreateAllQuestionResponseFiles()
        {
            Response[] responses = ReadResponses().ToArray();
            CreateResponsesFile(responses);
            ExportResponses(responses);

            Question[] questions = ReadQuestions().ToArray();
            CreateQuestionsFile(questions);

            QuestionResponses.DeleteUnmodifiedQuestionResponseFiles(_directory);
            CreateQuestionResponseFiles(QuestionResponses.GetQuestionResponses(questions, responses));
        }

        public void CreateQuestionResponseFiles()
        {
            CreateQuestionResponseFiles(GetQuestionResponses());
        }

        //public void DeleteUnmodifiedQuestionResponseFiles()
        //{
        //    string dataDirectory = GetDataDirectory();
        //    if (!zDirectory.Exists(dataDirectory))
        //        return;
        //    foreach (string file in zDirectory.EnumerateFiles(dataDirectory))
        //    {
        //        if (FileNumber.GetNumber(file) == null)
        //        {
        //            zFile.Delete(file);
        //        }
        //    }
        //}

        public void CreateQuestionResponseFiles(IEnumerable<QuestionResponse> questionResponses)
        {
            // create questions files : data/question-01-2015-016.json ...
            //string dataDirectory = GetDataDirectory();
            Trace.WriteLine($"create questions responses files");
            QuestionResponses.CreateQuestionResponseFiles(questionResponses, _directory);
        }

        //private string GetDataDirectory()
        //{
        //    return zPath.Combine(_directory, "data");
        //}

        public void CreateQuestionsFile()
        {
            CreateQuestionsFile(ReadQuestions());
        }

        private void CreateQuestionsFile(IEnumerable<Question> questions)
        {
            //string dataDirectory = zPath.Combine(_directory, "data");
            string file = zPath.Combine(_directory, @"data\questions.json");
            Trace.WriteLine("create questions file \"questions.json\"");
            //QuestionResponseFiles questionResponseFiles = GetQuestionResponseScanFiles();
            //QuestionReader.Read(questionResponseFiles.QuestionFiles, _questionRegexValuesList, questionResponseFiles.BaseDirectory).zSave(file, jsonIndent: true);
            questions.zSave(file, jsonIndent: true);
        }

        public void CreateResponsesFile()
        {
            CreateResponsesFile(ReadResponses());
        }

        private void CreateResponsesFile(IEnumerable<Response> responses)
        {
            string file = zPath.Combine(_directory, @"data\responses.json");
            Trace.WriteLine("create responses file \"responses.json\"");
            responses.zSave(file, jsonIndent: true);
        }

        public void ExportResponses()
        {
            ExportResponses(ReadResponses().ToArray());
        }

        private void ExportResponses(IEnumerable<Response> responses)
        {
            //string file = zPath.Combine(_directory, GetFileName() + ".response.txt");
            string file = zPath.Combine(_directory, @"data\responses.txt");
            Trace.WriteLine("export responses to \"responses.txt\"");
            using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
            {
                bool first = true;
                //bool export = true;
                //string category = null;
                foreach (Response response in responses.zDistinctBy(response2 => response2.Category))
                {
                    //if (!export && response.Category != category)
                    //    export = true;

                    //if (export)
                    //{
                    //    category = response.Category;
                    if (!first)
                    {
                        sw.WriteLine();
                        sw.WriteLine();
                    }
                    string category = response.Category;
                    if (category != null)
                    {
                        sw.WriteLine(category);
                        sw.WriteLine();
                    }
                    ExportResponseCategory(sw, responses, category);
                    //export = false;
                    first = false;
                    //}
                }
            }
        }

        private void ExportResponseCategory(StreamWriter sw, IEnumerable<Response> responses, string category)
        {

            // 2012
            // Q102: ABCD

            //Dictionary<int, int> years = new Dictionary<int, int>();
            //List<IEnumerator<Response>> yearResponses = new List<IEnumerator<Response>>();
            SortedDictionary<int, IEnumerator<Response>> years = new SortedDictionary<int, IEnumerator<Response>>();
            //int index = 0;
            //Trace.WriteLine($"{responses.Length} responses");
            foreach (Response response in responses)
            {
                if (response.Category != category)
                    continue;

                //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
                if (!years.ContainsKey(response.Year))
                {
                    //years.Add(response.Year, index++);
                    //yearResponses.Add(responses.Where(response2 => response2.Year == response.Year).GetEnumerator());
                    years.Add(response.Year, responses.Where(response2 => response2.Year == response.Year && response2.Category == category).GetEnumerator());
                }
            }

            StringBuilder sb = new StringBuilder();

            bool first = true;
            foreach (int year in years.Keys.Reverse())
            {
                if (!first)
                    sb.Append(new string(' ', _yearWidth - 5));
                sb.Append($" {year}");
                first = false;
            }
            sw.WriteLine();
            sw.WriteLine(sb.ToString());
            sw.WriteLine();

            sb.Clear();
            //int lastIndex = -1;
            int l = 0;
            //foreach (Response response in responses)
            while (true)
            {
                bool found = false;
                int lastIndex = -1;
                int index = -1;
                //foreach (IEnumerator<Response> yearResponse in yearResponses)
                foreach (int year in years.Keys.Reverse())
                {
                    index++;
                    IEnumerator<Response> yearResponse = years[year];

                    if (!yearResponse.MoveNext())
                        continue;

                    found = true;
                    Response response = yearResponse.Current;
                    //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
                    //index = years[response.Year];
                    if (index <= lastIndex)
                    {
                        sw.WriteLine(sb.ToString());
                        sw.WriteLine();
                        sb.Clear();
                        lastIndex = -1;
                        l = 0;
                    }
                    if (index != lastIndex - 1 || l > 0)
                        sb.Append(new string(' ', (index - lastIndex - 1) * _yearWidth + l));
                    string text = $" Q{response.QuestionNumber}: {response.Responses}";
                    l = _yearWidth - text.Length;
                    sb.Append(text);
                    lastIndex = index;
                }
                if (!found)
                    break;
                if (sb.Length > 0)
                    sw.WriteLine(sb.ToString());
                sw.WriteLine();
                sb.Clear();
                lastIndex = -1;
                l = 0;
            }
        }

        //public void ExportResponse_v2()
        //{
        //    string file = zPath.Combine(_directory, GetFileName() + ".response.txt");
        //    Response[] responses = ReadResponses().ToArray();

        //    // 2012
        //    // Q102: ABCD
        //    Trace.WriteLine($"export responses to \"{file}\"");

        //    //Dictionary<int, int> years = new Dictionary<int, int>();
        //    //List<IEnumerator<Response>> yearResponses = new List<IEnumerator<Response>>();
        //    SortedDictionary<int, IEnumerator<Response>> years = new SortedDictionary<int, IEnumerator<Response>>();
        //    //int index = 0;
        //    //Trace.WriteLine($"{responses.Length} responses");
        //    foreach (Response response in responses)
        //    {
        //        //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
        //        if (!years.ContainsKey(response.Year))
        //        {
        //            //years.Add(response.Year, index++);
        //            //yearResponses.Add(responses.Where(response2 => response2.Year == response.Year).GetEnumerator());
        //            years.Add(response.Year, responses.Where(response2 => response2.Year == response.Year).GetEnumerator());
        //        }
        //    }

        //    using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        bool first = true;
        //        foreach (int year in years.Keys.Reverse())
        //        {
        //            if (!first)
        //                sb.Append(new string(' ', _yearWidth - 5));
        //            sb.Append($" {year}");
        //            first = false;
        //        }
        //        sw.WriteLine();
        //        sw.WriteLine(sb.ToString());
        //        sw.WriteLine();

        //        sb.Clear();
        //        //int lastIndex = -1;
        //        int l = 0;
        //        //foreach (Response response in responses)
        //        while (true)
        //        {
        //            bool found = false;
        //            int lastIndex = -1;
        //            int index = -1;
        //            //foreach (IEnumerator<Response> yearResponse in yearResponses)
        //            foreach (int year in years.Keys.Reverse())
        //            {
        //                index++;
        //                IEnumerator<Response> yearResponse = years[year];

        //                if (!yearResponse.MoveNext())
        //                    continue;

        //                found = true;
        //                Response response = yearResponse.Current;
        //                //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
        //                //index = years[response.Year];
        //                if (index <= lastIndex)
        //                {
        //                    sw.WriteLine(sb.ToString());
        //                    sw.WriteLine();
        //                    sb.Clear();
        //                    lastIndex = -1;
        //                    l = 0;
        //                }
        //                if (index != lastIndex - 1 || l > 0)
        //                    sb.Append(new string(' ', (index - lastIndex - 1) * _yearWidth + l));
        //                string text = $" Q{response.QuestionNumber}: {response.Responses}";
        //                l = _yearWidth - text.Length;
        //                sb.Append(text);
        //                lastIndex = index;
        //            }
        //            if (!found)
        //                break;
        //            if (sb.Length > 0)
        //                sw.WriteLine(sb.ToString());
        //            sw.WriteLine();
        //            sb.Clear();
        //            lastIndex = -1;
        //            l = 0;
        //        }
        //    }
        //}

        //public void ExportResponse_v1()
        //{
        //    string file = zPath.Combine(_directory, GetFileName() + ".response.txt");
        //    Response[] responses = GetResponses().ToArray();

        //    // 2012
        //    // Q102: ABCD
        //    Trace.WriteLine($"export responses to \"{file}\"");

        //    Dictionary<int, int> years = new Dictionary<int, int>();
        //    List<IEnumerator<Response>> yearResponses = new List<IEnumerator<Response>>();
        //    //SortedDictionary<int, IEnumerator<Response>> years = new SortedDictionary<int, IEnumerator<Response>>();
        //    int index = 0;
        //    //Trace.WriteLine($"{responses.Length} responses");
        //    foreach (Response response in responses)
        //    {
        //        //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
        //        if (!years.ContainsKey(response.Year))
        //        {
        //            years.Add(response.Year, index++);
        //            yearResponses.Add(responses.Where(response2 => response2.Year == response.Year).GetEnumerator());
        //        }
        //    }

        //    using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        bool first = true;
        //        foreach (int year in years.Keys)
        //        {
        //            if (!first)
        //                sb.Append(new string(' ', _yearWidth - 5));
        //            sb.Append($" {year}");
        //            first = false;
        //        }
        //        sw.WriteLine();
        //        sw.WriteLine(sb.ToString());
        //        sw.WriteLine();

        //        sb.Clear();
        //        int lastIndex = -1;
        //        int l = 0;
        //        //foreach (Response response in responses)
        //        while (true)
        //        {
        //            bool found = false;
        //            foreach (IEnumerator<Response> yearResponse in yearResponses)
        //            {
        //                if (!yearResponse.MoveNext())
        //                    continue;

        //                found = true;
        //                Response response = yearResponse.Current;
        //                //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
        //                index = years[response.Year];
        //                if (index <= lastIndex)
        //                {
        //                    sw.WriteLine(sb.ToString());
        //                    sw.WriteLine();
        //                    sb.Clear();
        //                    lastIndex = -1;
        //                    l = 0;
        //                }
        //                if (index != lastIndex - 1 || l > 0)
        //                    sb.Append(new string(' ', (index - lastIndex - 1) * _yearWidth + l));
        //                string text = $" Q{response.QuestionNumber}: {response.Responses}";
        //                l = _yearWidth - text.Length;
        //                sb.Append(text);
        //                lastIndex = index;
        //            }
        //            if (!found)
        //                break;
        //            if (sb.Length > 0)
        //                sw.WriteLine(sb.ToString());
        //            sw.WriteLine();
        //            sb.Clear();
        //            lastIndex = -1;
        //            l = 0;
        //        }
        //    }
        //}

        // QuestionResponseFiles questionResponseFiles
        private IEnumerable<QuestionResponse> GetQuestionResponses()
        {
            //QuestionResponseFiles questionResponseFiles = GetQuestionResponseScanFiles();
            //// , string baseDirectory = null
            //return QuestionResponses.GetQuestionResponses(QuestionReader.Read(questionResponseFiles.QuestionFiles, _questionRegexValuesList, questionResponseFiles.BaseDirectory),
            //    ResponseReader.Read(questionResponseFiles.ResponseFile, _responseRegexValuesList));
            return QuestionResponses.GetQuestionResponses(ReadQuestions(), ReadResponses());
        }

        public IEnumerable<Question> ReadQuestions()
        {
            QuestionResponseFiles questionResponseFiles = GetQuestionResponseScanFiles();
            //return QuestionReader.Read(questionResponseFiles.QuestionFiles, _questionRegexValuesList, questionResponseFiles.BaseDirectory);
            //return new QuestionReader { _regexList = regexList }._Read(files, baseDirectory);
            return new QuestionReader { RegexList = _questionRegexValuesList, MaxLinesPerQuestion = _maxLinesPerQuestion, MaxLinesPerResponse = _maxLinesPerResponse }
                .Read(questionResponseFiles.QuestionFiles, questionResponseFiles.BaseDirectory);
        }

        //private IEnumerable<Response> GetResponses()
        //{
        //    return ResponseReader.Read(GetQuestionResponseScanFiles().ResponseFile, _responseRegexValuesList);
        //}

        private IEnumerable<Response> ReadResponses()
        {
            return ResponseReader.Read(GetQuestionResponseScanFiles().ResponseFile, _responseRegexValuesList);
        }

        // string directory, string filename, string baseDirectory
        public QuestionResponseFiles GetQuestionResponseScanFiles()
        {
            if (_questionResponseFiles == null)
            {
                _questionResponseFiles = new QuestionResponseFiles();
                _questionResponseFiles.BaseDirectory = _baseDirectory;
                // directory, filename
                string[] files = GetScanFiles().ToArray();
                _questionResponseFiles.QuestionFiles = new string[files.Length - 1];
                Array.Copy(files, _questionResponseFiles.QuestionFiles, files.Length - 1);
                _questionResponseFiles.ResponseFile = files[files.Length - 1];
            }
            return _questionResponseFiles;
        }

        private string GetPdfFile()
        {
            return zPath.Combine(_directory, GetFileName() + ".pdf");
        }

        private string GetParametersFile()
        {
            return zPath.Combine(_directory, @"data\parameters.json");
        }

        private IEnumerable<string> GetScanFiles()
        {
            string directory = GetScanDirectory();
            if (!zDirectory.Exists(directory))
                throw new PBException($"scan directory not found (\"{directory}\")");
            int index = 1;
            bool foundOne = false;
            string file = null;
            while (true)
            {
                if (!foundOne && index == 100)
                    throw new PBException($"scan files not found (\"{file}\")");
                //file = zPath.Combine(directory, GetFileName() + $"-page-{index:000}.txt");
                file = zPath.Combine(directory, $"scan-page-{index:000}.txt");

                //Trace.WriteLine(file);
                //string file2 = zPath.Combine(directory, filename + $"-page-{index:000}_02.txt");
                if (zFile.Exists(file))
                {
                    foundOne = true;
                    //if (zFile.Exists(file2))
                    //    yield return file2;
                    //else
                    //    yield return file;
                    yield return GetLastFileNumber(file);
                }
                else if (foundOne)
                    break;
                index++;
            }
        }

        private string GetAnkiFile()
        {
            //return zPath.Combine(_directory, "data", GetFileName() + ".anki.txt");
            //return zPath.Combine(_directory, "data", "anki.txt");
            //return zPath.Combine(_directory, GetFileName() + ".anki.txt");
            return zPath.Combine(_directory, "anki.txt");
        }

        private string GetImagesDirectory()
        {
            return zPath.Combine(_directory, @"data\images");
        }


        private string GetScanDirectory()
        {
            //string directory = zPath.Combine(_directory, "scan");
            return zPath.Combine(_directory, @"data\scan");
        }

        private static string GetLastFileNumber(string file)
        {
            string lastFile = file;
            string directory = zPath.GetDirectoryName(file);
            string filename = zPath.GetFileNameWithoutExtension(file);
            string ext = zPath.GetExtension(file);
            int index = 2;
            while (true)
            {
                string file2 = zPath.Combine(directory, filename + $"_{index:00}" + ext);
                if (!zFile.Exists(file2))
                    break;
                lastFile = file2;
                index++;
            }
            return lastFile;
        }

        private string GetFileName()
        {
            if (_filename == null)
                _filename = zPath.GetFileName(_directory);
            return _filename;
        }
    }
}

using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Linq;
using pb.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

    public class ImageColumn
    {
        public string ImageFile;
        public int Column;
    }

    public partial class QuestionsManager
    {
        //private const int _timeout = 1200; // Timeout 1200" = 20 min
        private static string[] _imageExtensions = new string[] { ".jpg", ".png" };
        private string _baseDirectory = null;
        private string _directory = null;
        private string _filename = null;
        private bool _joinImagesColumn = true;
        private int _maxLinesPerQuestion = -1;
        private int _maxLinesPerChoice = -1;
        private bool _traceUnknowValue = false;
        private RegexValuesList _questionRegexValuesList = null;
        private RegexValuesList _responseRegexValuesList = null;
        private RegexValuesList _questionRegexValuesList_v2 = null;
        private RegexValuesList _responseRegexValuesList_v2 = null;

        private int _yearWidth = 12;

        private QuestionResponseFiles _questionResponseFiles = null;
        private string[] _scanFiles = null;
        private QuestionReader_v2 _questionReader_v2 = null;
        private Question_v2[] _questions = null;
        private Response_v2[] _responses = null;

        public string BaseDirectory { get { return _baseDirectory; } set { _baseDirectory = value; } }
        // set { _directory = value; }
        public string Directory { get { return _directory; } }
        public int MaxLinesPerQuestion { get { return _maxLinesPerQuestion; } set { _maxLinesPerQuestion = value; } }
        public int MaxLinesPerChoice { get { return _maxLinesPerChoice; } set { _maxLinesPerChoice = value; } }
        public bool TraceUnknowValue { get { return _traceUnknowValue; } set { _traceUnknowValue = value; } }
        public RegexValuesList QuestionRegexValuesList { get { return _questionRegexValuesList; } set { _questionRegexValuesList = value; } }
        public RegexValuesList ResponseRegexValuesList { get { return _responseRegexValuesList; } set { _responseRegexValuesList = value; } }
        public RegexValuesList QuestionRegexValuesList_v2 { get { return _questionRegexValuesList_v2; } set { _questionRegexValuesList_v2 = value; } }
        public RegexValuesList ResponseRegexValuesList_v2 { get { return _responseRegexValuesList_v2; } set { _responseRegexValuesList_v2 = value; } }

        //public QuestionsManager(string baseDirectory, string directory, RegexValuesList questionRegexValuesList, RegexValuesList responseRegexValuesList)
        //{
        //    _baseDirectory = baseDirectory;
        //    _directory = zPath.Combine(baseDirectory, directory);
        //    _filename = zPath.GetFileName(directory);
        //    _questionRegexValuesList = questionRegexValuesList;
        //    _responseRegexValuesList = responseRegexValuesList;
        //}

        public void SetDirectory (string directory)
        {
            if (directory != null && !zPath.IsPathRooted(directory))
                directory = zPath.Combine(_baseDirectory, directory);
            _directory = directory;
            _filename = null;
            _questionResponseFiles = null;
            _scanFiles = null;
            _questions = null;
            _responses = null;
        }

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

        //public void CreateAnkiFileFromScanFiles()
        //{
        //    string ankiFile = GetAnkiFile();
        //    Trace.WriteLine($"create anki file \"{ankiFile}\" from scan files");
        //    AnkiWriter.Write(ankiFile, GetQuestionResponses()
        //        .Select(questionResponse =>
        //        new AnkiQuestion { Question = questionResponse.GetHtml(questionNumber: true, replaceSpecialCharacters: true), Response = questionResponse.Response != null ? questionResponse.Response.GetFormatedResponse() : "(unknow response)" }));
        //}

        public string CreateAnkiFileFromQuestionFiles()
        {
            string ankiFile = GetAnkiFile();
            Trace.WriteLine($"create anki file \"{ankiFile}\" from questions files");
            AnkiWriter.Write(ankiFile, QuestionResponses.GetQuestionFiles(_directory).Select(file => QuestionResponses.LoadQuestion(_directory, file))
                .Select(questionResponseHtml => new AnkiQuestion { Question = $"<h1>{questionResponseHtml.Year} - question no {questionResponseHtml.Number}</h1><br>" + questionResponseHtml.QuestionHtml,
                    Response = Response.GetFormatedResponse(questionResponseHtml.Responses) }));
            return ankiFile;
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

        public void CreateAllQuestionResponseFiles_v2()
        {
            // Response_v2[]
            IEnumerable<Response_v2> responses = ReadResponses_v2();
            CreateResponsesFile_v2(responses);
            ExportResponses_v2(responses);

            // Question_v2[]
            IEnumerable<Question_v2> questions = ReadQuestions_v2();
            CreateQuestionsFile_v2(questions);

            QuestionResponses.DeleteUnmodifiedQuestionResponseFiles(_directory);
            CreateQuestionResponseFiles_v2(QuestionResponses.GetQuestionResponses_v2(questions, responses));
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
            Trace.WriteLine("create questions responses files");
            QuestionResponses.CreateQuestionResponseFiles(questionResponses, _directory);
        }

        public void CreateQuestionResponseFiles_v2(IEnumerable<QuestionResponseData_v2> questionResponses)
        {
            // create questions files : data/question-01-2015-016.json ...
            //string dataDirectory = GetDataDirectory();
            Trace.WriteLine("create questions responses files");
            QuestionResponses.CreateQuestionResponseFiles_v2(questionResponses, _directory);
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

        private void CreateQuestionsFile_v2(IEnumerable<Question_v2> questions)
        {
            string file = zPath.Combine(_directory, @"data\questions.json");
            Trace.WriteLine("create questions file \"questions.json\"");
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

        private void CreateResponsesFile_v2(IEnumerable<Response_v2> responses)
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

        private void ExportResponses_v2(IEnumerable<Response_v2> responses)
        {
            string file = zPath.Combine(_directory, @"data\responses.txt");
            Trace.WriteLine("export responses to \"responses.txt\"");
            using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
            {
                bool first = true;
                foreach (Response_v2 response in responses.zDistinctBy(response2 => response2.Category))
                {
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
                    ExportResponseCategory_v2(sw, responses, category);
                    first = false;
                }
            }
        }

        private void ExportResponseCategory_v2(StreamWriter sw, IEnumerable<Response_v2> responses, string category)
        {

            // 2012
            // Q102: ABCD

            SortedDictionary<int, IEnumerator<Response_v2>> years = new SortedDictionary<int, IEnumerator<Response_v2>>();
            foreach (Response_v2 response in responses)
            {
                if (response.Category != category)
                    continue;

                if (!years.ContainsKey(response.Year))
                {
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
            int l = 0;
            while (true)
            {
                bool found = false;
                int lastIndex = -1;
                int index = -1;
                foreach (int year in years.Keys.Reverse())
                {
                    index++;
                    IEnumerator<Response_v2> yearResponse = years[year];

                    if (!yearResponse.MoveNext())
                        continue;

                    found = true;
                    Response_v2 response = yearResponse.Current;
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
            return new QuestionReader { RegexList = _questionRegexValuesList, MaxLinesPerQuestion = _maxLinesPerQuestion, MaxLinesPerChoice = _maxLinesPerChoice,
                TraceUnknowValue = _traceUnknowValue }.Read(questionResponseFiles.QuestionFiles, questionResponseFiles.BaseDirectory);
        }

        //private IEnumerable<Response> GetResponses()
        //{
        //    return ResponseReader.Read(GetQuestionResponseScanFiles().ResponseFile, _responseRegexValuesList);
        //}

        public IEnumerable<Response> ReadResponses()
        {
            QuestionResponseFiles questionResponseFiles = GetQuestionResponseScanFiles();
            return ResponseReader.Read(questionResponseFiles.ResponseFile, _responseRegexValuesList, questionResponseFiles.BaseDirectory);
        }

        //public Question_v2[] ReadQuestions_v2()
        public IEnumerable<Question_v2> ReadQuestions_v2()
        {
            if (_questions == null)
                ReadQuestionsResponses_v2();
            return _questions;
        }

        //public Response_v2[] ReadResponses_v2()
        public IEnumerable<Response_v2> ReadResponses_v2()
        {
            if (_responses == null)
                ReadQuestionsResponses_v2();
            return _responses;
        }

        public void ReadQuestionsResponses_v2()
        {
            if (_questions == null || _responses == null)
            {
                QuestionReader_v2 questionReader = CreateQuestionReader_v2();
                questionReader.ReadAll(GetScanFiles(), _baseDirectory);
                _questions = questionReader.Questions;
                _responses = questionReader.Responses;
            }
        }

        public QuestionReader_v2 CreateQuestionReader_v2()
        {
            if (_questionReader_v2 == null)
            {
                _questionReader_v2 = new QuestionReader_v2();
                _questionReader_v2.QuestionRegexValuesList = _questionRegexValuesList_v2;
                _questionReader_v2.ResponseRegexValuesList = _responseRegexValuesList_v2;
                _questionReader_v2.MaxLinesPerQuestion = XmlConfig.CurrentConfig.GetExplicit("MaxLinesPerQuestion").zParseAs<int>();
                _questionReader_v2.MaxLinesPerChoice = XmlConfig.CurrentConfig.GetExplicit("MaxLinesPerChoice").zParseAs<int>();
                _questionReader_v2.TraceUnknowValue = _traceUnknowValue;
            }
            return _questionReader_v2;
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

        //private IEnumerable<string> GetScanFiles()
        private string[] GetScanFiles()
        {
            if (_scanFiles == null)
            {
                _scanFiles = GetScanFiles(GetScanDirectory()).ToArray();
            }
            return _scanFiles;
        }

        public static IEnumerable<string> GetScanFiles(string directory, int limit = 0)
        {
            // files :
            //   scan_02.txt
            //   scan.txt
            //   scan-page-001_02.txt
            //   scan-page-001.txt
            //   scan-page-001-01_02.txt
            //   scan-page-001-01.txt
            //   scan-page-001-02_02.txt
            //   scan-page-001-02.txt

            if (!zDirectory.Exists(directory))
                throw new PBException($"scan directory not found (\"{directory}\")");
            int index = 1;
            int count = 0;
            bool foundOne = false;
            string file = null;
            file = zPath.Combine(directory, "scan.txt");
            if (zFile.Exists(file))
            {
                yield return GetLastFileNumber(file);
            }
            else
            {
                while (true)
                {
                    if (!foundOne && index == 100)
                        throw new PBException($"scan files not found (\"{file}\")");

                    bool found = false;

                    // first column file : scan-page-001-01.txt
                    file = zPath.Combine(directory, $"scan-page-{index:000}-01.txt");
                    if (zFile.Exists(file))
                    {
                        foundOne = true;
                        found = true;
                        yield return GetLastFileNumber(file);
                        count++;

                        // second column file : scan-page-001-01.txt
                        file = zPath.Combine(directory, $"scan-page-{index:000}-02.txt");
                        if (zFile.Exists(file) && (limit == 0 || count < limit))
                        {
                            yield return GetLastFileNumber(file);
                            count++;
                        }
                    }
                    else
                    {

                        file = zPath.Combine(directory, $"scan-page-{index:000}.txt");

                        //Trace.WriteLine(file);
                        if (zFile.Exists(file))
                        {
                            foundOne = true;
                            found = true;
                            yield return GetLastFileNumber(file);
                            count++;
                        }
                    }

                    //if (!found && foundOne)
                    if ((!found && foundOne) || (limit != 0 && count >= limit))
                        break;
                    index++;
                }
            }
        }

        public static string GetLastFileNumber(string file, int startIndex = 2)
        {
            string lastFile = file;
            string directory = zPath.GetDirectoryName(file);
            string filename = zPath.GetFileNameWithoutExtension(file);
            string ext = zPath.GetExtension(file);
            //int index = 2;
            int index = startIndex;
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

        private string GetAnkiFile()
        {
            //return zPath.Combine(_directory, "data", GetFileName() + ".anki.txt");
            //return zPath.Combine(_directory, "data", "anki.txt");
            //return zPath.Combine(_directory, GetFileName() + ".anki.txt");
            return zPath.Combine(_directory, "anki.txt");
        }

        private string GetScanDirectory()
        {
            //string directory = zPath.Combine(_directory, "scan");
            return zPath.Combine(_directory, @"data\scan");
        }

        private string GetFileName()
        {
            if (_filename == null)
                _filename = zPath.GetFileName(_directory);
            return _filename;
        }
    }
}

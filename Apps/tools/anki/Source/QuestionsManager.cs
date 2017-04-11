using pb;
using pb.Data.Mongo;
using pb.IO;
using pb.Linq;
using pb.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace anki
{
    public class QuestionsManager
    {
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

        private IEnumerable<string> GetScanFiles()
        {
            string directory = GetScanDirectory();
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

        private string GetScanDirectory()
        {
            //string directory = zPath.Combine(_directory, "scan");
            string directory = zPath.Combine(_directory, @"data\scan");
            if (!zDirectory.Exists(directory))
                throw new PBException($"scan directory not found (\"{directory}\")");
            return directory;
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

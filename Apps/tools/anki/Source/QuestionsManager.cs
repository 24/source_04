using pb;
using pb.Data.Mongo;
using pb.IO;
using pb.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace anki
{
    public class QuestionsManager
    {
        private string _baseDirectory = null;
        private string _directory = null;
        private string _filename = null;
        private RegexValuesList _questionRegexValuesList = null;
        private RegexValuesList _responseRegexValuesList = null;

        private int _yearWidth = 12;

        private QuestionResponseFiles _questionResponseFiles = null;

        public QuestionsManager(string baseDirectory, string directory, RegexValuesList questionRegexValuesList, RegexValuesList responseRegexValuesList)
        {
            _baseDirectory = baseDirectory;
            _directory = zPath.Combine(baseDirectory, directory);
            _filename = zPath.GetFileName(directory);
            _questionRegexValuesList = questionRegexValuesList;
            _responseRegexValuesList = responseRegexValuesList;
        }

        public void CreateAnkiFileFromScanFiles()
        {
            string ankiFile = zPath.Combine(_directory, _filename + ".anki.txt");
            Trace.WriteLine($"create anki file \"{ankiFile}\" from scan files");
            // questionResponseFiles
            AnkiWriter.Write(ankiFile, GetQuestionResponses()
                .Select(questionResponse => new AnkiQuestion { Question = questionResponse.GetHtml(questionNumber: true), Response = questionResponse.Response.GetFormatedResponse() }));
        }

        public void CreateAnkiFileFromQuestionFiles()
        {
            string ankiFile = zPath.Combine(_directory, _filename + ".anki.txt");
            Trace.WriteLine($"create anki file \"{ankiFile}\" from questions files");
            AnkiWriter.Write(ankiFile, QuestionResponses.GetQuestionFiles(zPath.Combine(_directory, "data")).Select(file => QuestionResponses.LoadQuestion(_directory, file))
                .Select(questionResponseHtml => new AnkiQuestion { Question = $"<h1>{questionResponseHtml.Year} - question no {questionResponseHtml.Number}</h1><br>" + questionResponseHtml.QuestionHtml,
                    Response = Response.GetFormatedResponse(questionResponseHtml.Responses) }));
        }

        public void CreateQuestionFiles()
        {
            // create questions files : data/question-01-2015-016.json ...
            string dataDirectory = zPath.Combine(_directory, "data");
            Trace.WriteLine($"create questions files in \"{dataDirectory}\"");
            // questionResponseFiles
            QuestionResponses.CreateQuestionFiles(GetQuestionResponses(), dataDirectory);
        }

        public void CreateResponseFile()
        {
            GetResponses().zSave(zPath.Combine(_directory, "response.json"), jsonIndent: true);
        }

        //Response[] responses = ResponseReader.Read(questionResponseFiles.ResponseFile, GetResponseRegexValuesList()).ToArray();
        public void ExportResponse()
        {
            string file = zPath.Combine(_directory, _filename + ".response.txt");
            Response[] responses = GetResponses().ToArray();

            // 2012
            // Q102: ABCD
            Trace.WriteLine($"export responses to \"{file}\"");

            Dictionary<int, int> years = new Dictionary<int, int>();
            List<IEnumerator<Response>> yearResponses = new List<IEnumerator<Response>>();
            int index = 0;
            //Trace.WriteLine($"{responses.Length} responses");
            foreach (Response response in responses)
            {
                //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
                if (!years.ContainsKey(response.Year))
                {
                    years.Add(response.Year, index++);
                    yearResponses.Add(responses.Where(response2 => response2.Year == response.Year).GetEnumerator());
                }
            }

            using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
            {
                StringBuilder sb = new StringBuilder();

                bool first = true;
                foreach (int year in years.Keys)
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
                int lastIndex = -1;
                int l = 0;
                //foreach (Response response in responses)
                while (true)
                {
                    bool found = false;
                    foreach (IEnumerator<Response> yearResponse in yearResponses)
                    {
                        if (!yearResponse.MoveNext())
                            continue;

                        found = true;
                        Response response = yearResponse.Current;
                        //Trace.WriteLine($"{response.Year} - Q{response.QuestionNumber}: {response.Responses}");
                        index = years[response.Year];
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
        }

        // QuestionResponseFiles questionResponseFiles
        private IEnumerable<QuestionResponse> GetQuestionResponses()
        {
            QuestionResponseFiles questionResponseFiles = GetQuestionResponseFiles();
            // , string baseDirectory = null
            return QuestionResponses.GetQuestionResponses(QuestionReader.Read(questionResponseFiles.QuestionFiles, _questionRegexValuesList, questionResponseFiles.BaseDirectory),
                ResponseReader.Read(questionResponseFiles.ResponseFile, _responseRegexValuesList));
        }

        private IEnumerable<Response> GetResponses()
        {
            return ResponseReader.Read(GetQuestionResponseFiles().ResponseFile, _responseRegexValuesList);
        }

        // string directory, string filename, string baseDirectory
        public QuestionResponseFiles GetQuestionResponseFiles()
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

        // string directory, string filename
        private IEnumerable<string> GetScanFiles()
        {
            string directory = zPath.Combine(_directory, "scan");
            int index = 1;
            bool foundOne = false;
            while (true)
            {
                if (!foundOne && index == 100)
                    throw new PBException("scan files not found");
                string file = zPath.Combine(directory, _filename + $"-page-{index:000}.txt");
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
    }
}

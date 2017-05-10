using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Text;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace anki.Test
{
    public static class QuestionTest
    {
        public static NamedValues<ZValue> Test_Find_01(string text, RegexValuesList regexList)
        {
            Trace.WriteLine($"text : \"{text}\"");
            //RegexValuesList regexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
            //FindText findText = QuestionRun.GetQuestionRegexValuesList().Find(text);
            FindText_v2 findText = regexList.Find(text);
            Trace.WriteLine($"found : {findText.Success}");
            return findText.GetAllValues();
        }

        //public static void Test_FindInFile_01(string file)
        //{
        //    //RegexValuesList regexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
        //    RegexValuesList regexList = QuestionRun.GetQuestionRegexValuesList();
        //    foreach (string line in zFile.ReadLines(file))
        //    {
        //        string line2 = line.Trim();
        //        if (line2 == "")
        //            continue;
        //        Trace.WriteLine($"read : \"{line2}\"");
        //        FindText findText = regexList.Find(line2);
        //        if (findText.Found)
        //        {
        //            foreach (KeyValuePair<string, ZValue> namedValue in findText.matchValues.GetValues())
        //                Trace.WriteLine($"  found : {namedValue.Key} = \"{namedValue.Value}\"");
        //        }
        //        else
        //        {
        //            Trace.WriteLine($"  unknow text");
        //        }
        //    }
        //}

        public static void Test_Find_02(string text, RegexValuesList regexList)
        {
            Trace.WriteLine($"text : \"{text}\"");
            FindText_v2 findText = regexList.Find(text);
            if (findText.Success)
            {
                //MatchValues matchValues = findText.matchValues;
                while (findText.Success)
                {
                    Trace.WriteLine();
                    Trace.WriteLine($"  found : index {findText.Match.Index} length {findText.Match.Length}");
                    foreach (KeyValuePair<string, ZValue> namedValue in findText.GetValues())
                        Trace.WriteLine($"        : {namedValue.Key} = \"{namedValue.Value}\"");
                    //matchValues = matchValues.Next();
                    findText.Next();
                }
            }
            else
            {
                Trace.WriteLine($"  unknow text");
            }
        }

        public static void Test_FindInFile_01(string file, RegexValuesList regexList)
        {
            //RegexValuesList regexList = QuestionRun.GetResponseRegexValuesList();
            int lineNumber = 0;
            foreach (string line in zFile.ReadLines(file))
            {
                //string line2 = line.Trim();
                //if (line2 == "")
                //    continue;

                lineNumber++;
                if (line == "")
                    continue;

                Trace.WriteLine();
                Trace.WriteLine($"read : line {lineNumber} \"{line}\"");
                FindText_v2 findText = regexList.Find(line);
                if (findText.Success)
                {
                    //MatchValues matchValues = findText.matchValues;
                    while (findText.Success)
                    {
                        Trace.WriteLine();
                        Trace.WriteLine($"  found : index {findText.Match.Index} length {findText.Match.Length}");
                        foreach (KeyValuePair<string, ZValue> namedValue in findText.GetValues())
                            Trace.WriteLine($"        : {namedValue.Key} = \"{namedValue.Value}\"");
                        //matchValues = matchValues.Next();
                        findText.Next();
                    }
                }
                else
                {
                    Trace.WriteLine($"  unknow text");
                }
            }
        }

        public static IEnumerable<Question> Test_QuestionReader_v1(string directory)
        {
            return QuestionRun.CreateQuestionsManager(directory).ReadQuestions();
        }


        public static IEnumerable<TextData> Test_ReadQuestionFiles(string directory, int limit = 0)
        {
            directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
            //TextDataReader textDataReader = new TextDataReader(QuestionsManager.GetScanFiles(directory, limit));
            TextDataReader textDataReader = new TextDataReader();
            textDataReader.SetRegexList(CreateQuestionRegexValuesList());
            //return textDataReader.Read(QuestionsManager.GetScanFiles(zPath.Combine(directory, @"data\scan"), limit));
            int pageNumber = 1;
            foreach (TextData textData in textDataReader.Read(QuestionsManager.GetScanFiles(zPath.Combine(directory, @"data\scan"), limit)))
            {
                yield return textData;
                if (textData.Values.ContainsKey("pageNumber"))
                    pageNumber++;
                if (textData.Values.ContainsKey("responses") && pageNumber > 1)
                {
                    textDataReader.SetRegexList(CreateResponseRegexValuesList());
                    textDataReader.ContiguousSearch = true;
                }
            }
        }

        public static IEnumerable<QuestionData> Test_ReadQuestionFilesAsQuestionData(string directory, int limit = 0)
        {
            return Test_ReadQuestionFiles(directory, limit).Select(textData => QuestionData.CreateQuestionData(textData));
        }

        public static IEnumerable<QuestionData> Test_ReadQuestionFilesAsQuestionData_v2(string directory, int limit = 0)
        {
            directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
            //TextDataReader textDataReader = new TextDataReader(QuestionsManager.GetScanFiles(directory, limit));
            TextDataReader textDataReader = new TextDataReader();
            textDataReader.SetRegexList(CreateQuestionRegexValuesList());
            foreach (QuestionData value in textDataReader.Read(QuestionsManager.GetScanFiles(zPath.Combine(directory, @"data\scan"), limit)).Select(textData => QuestionData.CreateQuestionData(textData)))
            {
                if (value.Type == QuestionDataType.Responses)
                {
                    //Trace.WriteLine("read responses");
                    textDataReader.SetRegexList(CreateResponseRegexValuesList());
                    textDataReader.ContiguousSearch = true;
                }
                yield return value;
            }
        }

        // string baseDirectory
        public static void Test_QuestionReader_v2_ToFile(string directory, int limit = 0)
        {
            //.Select(question => (Question_v2)question)
            Test_QuestionReader_v2(directory, limit).Select(question => (Question_v2)question).zSave(zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory, @"data\questions_v2.json"), jsonIndent: true);
        }

        // string baseDirectory
        public static IEnumerable<QuestionResponse_v2> Test_QuestionReader_v2(string directory, int limit = 0)
        {
            string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
            return CreateQuestionReader_v2().Read(QuestionsManager.GetScanFiles(zPath.Combine(baseDirectory, directory, @"data\scan"), limit), baseDirectory);
        }

        public static QuestionReader_v2 CreateQuestionReader_v2()
        {
            QuestionReader_v2 questionReader = new QuestionReader_v2();
            questionReader.QuestionRegexValuesList = CreateQuestionRegexValuesList();
            questionReader.ResponseRegexValuesList = CreateResponseRegexValuesList();
            questionReader.MaxLinesPerQuestion = XmlConfig.CurrentConfig.GetExplicit("MaxLinesPerQuestion").zParseAs<int>();
            questionReader.MaxLinesPerChoice = XmlConfig.CurrentConfig.GetExplicit("MaxLinesPerChoice").zParseAs<int>();
            return questionReader;
        }

        public static void Test_ValidateFiles(string testDirectory, string pattern)
        {
            foreach (string file in GetReaderFilesTest(testDirectory, pattern))
            {
                string okDirectory = zPath.Combine(zPath.GetDirectoryName(file), "ok");
                zfile.MoveFile(file, okDirectory, overwrite: true);
            }
        }

        public static IEnumerable<string> GetReaderFilesTest(string directory, string pattern)
        {
            //EnumerateFilesInfo
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter =
                dirInfo => dirInfo.Name == "ok" ? new EnumDirectoryFilter { Select = false, RecurseSubDirectory = false } : new EnumDirectoryFilter { Select = true, RecurseSubDirectory = true };
            //return zdir.EnumerateDirectoriesInfo(directory, directoryFilters: new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter }).Select(dir => dir.Directory);
            return zdir.EnumerateFilesInfo(directory, pattern, directoryFilters: new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter }).Select(file => file.File);
        }

        //public static void CompareQuestionsFiles(string directory)
        public static void CompareJsonFiles(string directory, string jsonFile, IEnumerable<string> elementsToCompare = null, IEnumerable<string> documentReference = null)
        {
            string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
            int okCount = 0;
            int notOkCount = 0;
            int notFoundCount = 0;
            foreach (string questionDirectory in GetQuestionDirectories())
            {
                if (!questionDirectory.StartsWith(baseDirectory))
                    throw new PBException($"wrong directory \"{directory}\"");
                string questionDirectory2 = questionDirectory.Substring(baseDirectory.Length + 1);
                // questions_01.json questions.json
                string file1 = zPath.Combine(questionDirectory, @"data\" + jsonFile);
                string lastFile1 = QuestionsManager.GetLastFileNumber(file1, startIndex: 1);
                if (lastFile1 == file1)
                {
                    Trace.WriteLine($"question file not found \"{file1}\"");
                    notFoundCount++;
                }
                else
                {
                    // ".questions.json"
                    string file2 = zPath.Combine(directory, questionDirectory2 + "." + jsonFile);
                    Trace.WriteLine($"compare question files \"{lastFile1}\" \"{file2}\"");
                    CompareDocumentsResult result = DocumentComparator.CompareBsonDocumentFiles(lastFile1, file2,
                      comparatorOptions: DocumentComparatorOptions.ReturnNotEqualDocuments | DocumentComparatorOptions.ResultNotEqualElements,
                      //elementsToCompare: new string[] { "Year", "Type", "Number", "QuestionText", "Choices", "SourceLine" },
                      elementsToCompare: elementsToCompare,
                      //documentReference: new string[] { "document1.Year", "document1.Number", "document1.SourceFile", "document1.SourceLine", "document2.SourceFile", "document2.SourceLine" });
                      documentReference: documentReference);
                      //.zGetResults().zSave(@"c:\pib\dev_data\exe\runsource\valentin\test\S2\compare.json", jsonIndent: true);
                    if (result.Equal)
                    {
                        Trace.WriteLine("  ok");
                        okCount++;
                    }
                    else
                    {
                        Trace.WriteLine($"  not ok");
                        string resultFile = zPath.Combine(directory, zPath.GetFileNameWithoutExtension(file2) + ".comp.json");
                        //Trace.WriteLine($"  not ok, (\"{resultFile}\")");
                        result.GetResultDocuments().zSave(resultFile, jsonIndent: true);
                        notOkCount++;
                    }
                }
            }
            Trace.WriteLine($"{okCount} ok {notOkCount} not ok {notFoundCount} not found");
        }

        public static IEnumerable<string> GetQuestionDirectories()
        {
            return XmlConfig.CurrentConfig.GetValues("QuestionDirectories/QuestionDirectory");
        }

        public static void Test_QuestionReader_Comparev1v2(IEnumerable<string> directories, string outputDirectory)
        {
            string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
            QuestionReader_v2 questionReader_v2 = CreateQuestionReader_v2();
            foreach (string directory in directories)
            {
                Trace.WriteLine($"compare QuestionReader for \"{directory}\"");
                string directory2 = zPath.Combine(baseDirectory, directory);
                string file1 = zPath.Combine(outputDirectory, "questions.json");
                string file2 = zPath.Combine(outputDirectory, "questions_v2.json");
                QuestionRun.CreateQuestionsManager(directory2).ReadQuestions().zSave(file1, jsonIndent: true);
                questionReader_v2.Read(QuestionsManager.GetScanFiles(zPath.Combine(directory2, @"data\scan")), baseDirectory)
                    .Select(question => (Question_v2)question).zSave(file2, jsonIndent: true);
                if (!zfile.AreFileEqual(file1, file2))
                {
                    Trace.WriteLine("  files are differents");
                    break;
                }
            }
        }

        public static RegexValuesList CreateQuestionRegexValuesList()
        {
            RegexValuesList regexValuesList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos_v2/QuestionInfo"), compileRegex: true);
            //regexValuesList.Add(XmlConfig.CurrentConfig.GetElements("ResponseInfos/ResponseInfo"), compileRegex: true);
            return regexValuesList;
        }

        public static RegexValuesList CreateResponseRegexValuesList()
        {
            RegexValuesList regexValuesList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("ResponseInfos_v2/ResponseInfo"), compileRegex: true);
            return regexValuesList;
        }

        // TestUnit_QuestionReader
        //public static void TestUnit_Reader(IEnumerable<string> directories, string outputDirectory, Func<string, string, bool> testUnitReader, string extension)
        //{
        //    string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
        //    int okCount = 0;
        //    int notOkCount = 0;
        //    int noOkCount = 0;
        //    int errorCount = 0;
        //    foreach (string directory in directories)
        //    {
        //        if (!directory.StartsWith(baseDirectory))
        //            throw new PBException($"wrong directory \"{directory}\"");
        //        //string directory2 = zPath.Combine(baseDirectory, directory);
        //        string directory2 = directory.Substring(baseDirectory.Length + 1);
        //        string file = zPath.Combine(outputDirectory, directory2 + extension);
        //        Trace.WriteLine($"create file \"{directory2 + extension}\"");
        //        if (testUnitReader(directory, file))
        //        {
        //            string okFile = zPath.Combine(zPath.GetDirectoryName(file), "ok", zPath.GetFileName(file));
        //            if (zFile.Exists(okFile))
        //            {
        //                if (zfile.AreFileEqual(file, okFile))
        //                {
        //                    Trace.WriteLine("  ok");
        //                    okCount++;
        //                }
        //                else
        //                {
        //                    Trace.WriteLine("  not ok");
        //                    notOkCount++;
        //                }
        //            }
        //            else
        //            {
        //                Trace.WriteLine("  ok file not found");
        //                noOkCount++;
        //            }
        //        }
        //        else
        //            errorCount++;
        //    }
        //    Trace.WriteLine($"{okCount} ok {notOkCount} not ok {errorCount} error {noOkCount} ok not found");
        //}

        //public static void Test_QuestionReader_v1(IEnumerable<string> directories, string outputDirectory)
        //public static void TestUnit_QuestionReader_v1(string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _traceUnknowValue_v1 = traceUnknowValue;
        //    TestUnit_Reader(GetQuestionDirectories(), outputDirectory, _TestUnit_QuestionReader_v1, ".questions.json");
        //}

        //public static void TestUnit_QuestionReader_v1(string directory, string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _traceUnknowValue_v1 = traceUnknowValue;
        //    directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
        //    TestUnit_Reader(new string[] { directory }, outputDirectory, _TestUnit_QuestionReader_v1, ".questions.json");
        //}

        //private static bool _traceUnknowValue_v1 = false;
        //public static bool _TestUnit_QuestionReader_v1(string directory, string file)
        //{
        //    try
        //    {
        //        // traceUnknowValue: _traceUnknowValue_v1
        //        QuestionRun.CreateQuestionsManager(directory).ReadQuestions().zSave(file, jsonIndent: true);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine($"  error : {ex.Message}");
        //        zFile.Delete(file);
        //        return false;
        //    }
        //}

        //public static void TestUnit_ResponseReader_v1(string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _traceUnknowValue_v1 = traceUnknowValue;
        //    TestUnit_Reader(GetQuestionDirectories(), outputDirectory, _TestUnit_ResponseReader_v1, ".responses.json");
        //}

        //public static void TestUnit_ResponseReader_v1(string directory, string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _traceUnknowValue_v1 = traceUnknowValue;
        //    directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
        //    TestUnit_Reader(new string[] { directory }, outputDirectory, _TestUnit_ResponseReader_v1, ".responses.json");
        //}

        //public static bool _TestUnit_ResponseReader_v1(string directory, string file)
        //{
        //    try
        //    {
        //        // traceUnknowValue: _traceUnknowValue_v1
        //        QuestionRun.CreateQuestionsManager(directory).ReadResponses().zSave(file, jsonIndent: true);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine($"  error : {ex.Message}");
        //        zFile.Delete(file);
        //        return false;
        //    }
        //}

        //public static void TestUnit_QuestionReader_v2(string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _questionReader_v2 = CreateQuestionReader_v2();
        //    _questionReader_v2.TraceUnknowValue = traceUnknowValue;
        //    TestUnit_Reader(GetQuestionDirectories(), outputDirectory, _TestUnit_QuestionReader_v2, ".questions.json");
        //    _questionReader_v2 = null;
        //}

        //public static void TestUnit_QuestionReader_v2(string directory, string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _questionReader_v2 = CreateQuestionReader_v2();
        //    _questionReader_v2.TraceUnknowValue = traceUnknowValue;
        //    directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
        //    TestUnit_Reader(new string[] { directory }, outputDirectory, _TestUnit_QuestionReader_v2, ".questions.json");
        //    _questionReader_v2 = null;
        //}

        //private static QuestionReader_v2 _questionReader_v2 = null;
        //public static bool _TestUnit_QuestionReader_v2(string directory, string file)
        //{
        //    try
        //    {
        //        _questionReader_v2.Read(QuestionsManager.GetScanFiles(zPath.Combine(directory, @"data\scan")), XmlConfig.CurrentConfig.GetExplicit("CardDirectory"))
        //            .Where(question => question is Question_v2).Select(question => (Question_v2)question).zSave(file, jsonIndent: true);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine($"  error : {ex.Message}");
        //        zFile.Delete(file);
        //        return false;
        //    }
        //}

        //public static void TestUnit_ResponseReader_v2(string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _questionReader_v2 = CreateQuestionReader_v2();
        //    _questionReader_v2.TraceUnknowValue = traceUnknowValue;
        //    TestUnit_Reader(GetQuestionDirectories(), outputDirectory, _TestUnit_ResponseReader_v2, ".responses.json");
        //    _questionReader_v2 = null;
        //}

        //public static void TestUnit_ResponseReader_v2(string directory, string outputDirectory, bool traceUnknowValue = false)
        //{
        //    _questionReader_v2 = CreateQuestionReader_v2();
        //    _questionReader_v2.TraceUnknowValue = traceUnknowValue;
        //    directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
        //    TestUnit_Reader(new string[] { directory }, outputDirectory, _TestUnit_ResponseReader_v2, ".responses.json");
        //    _questionReader_v2 = null;
        //}

        //public static bool _TestUnit_ResponseReader_v2(string directory, string file)
        //{
        //    try
        //    {
        //        _questionReader_v2.Read(QuestionsManager.GetScanFiles(zPath.Combine(directory, @"data\scan")), XmlConfig.CurrentConfig.GetExplicit("CardDirectory"))
        //            .Where(question => question is Response_v2).Select(question => (Response_v2)question).zSave(file, jsonIndent: true);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine($"  error : {ex.Message}");
        //        zFile.Delete(file);
        //        return false;
        //    }
        //}
    }
}

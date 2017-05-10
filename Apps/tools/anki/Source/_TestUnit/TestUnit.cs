using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using System;
using System.Collections.Generic;

namespace anki.TestUnit
{
    public enum TestUnitOpe
    {
        ReadQuestions,
        ReadResponses
    }

    public static class TestUnit
    {
        //private static QuestionsManager _questionsManager = null;

        public static void TestUnit_Reader(TestUnitOpe ope, string outputDirectory, bool traceUnknowValue = false, bool v2 = false)
        {
            //_traceUnknowValue_v1 = traceUnknowValue;
            //CreateQuestionsManager(v2: v2, traceUnknowValue: traceUnknowValue);
            // _TestUnit_QuestionReader_v1, ".questions.json"
            TestUnit_Reader(ope, XmlConfig.CurrentConfig.GetValues("QuestionDirectories/QuestionDirectory"), outputDirectory, traceUnknowValue, v2);
        }

        public static void TestUnit_Reader(TestUnitOpe ope, string directory, string outputDirectory, bool traceUnknowValue = false, bool v2 = false)
        {
            //_traceUnknowValue_v1 = traceUnknowValue;
            //CreateQuestionsManager(v2: v2, traceUnknowValue: traceUnknowValue);
            directory = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("CardDirectory"), directory);
            TestUnit_Reader(ope, new string[] { directory }, outputDirectory, traceUnknowValue, v2);
        }

        // Func<string, string, bool> testUnitReader, string extension
        private static void TestUnit_Reader(TestUnitOpe ope, IEnumerable<string> directories, string outputDirectory, bool traceUnknowValue, bool v2)
        {
            QuestionsManager questionsManager = QuestionRun.CreateQuestionsManager(v2: v2);
            questionsManager.TraceUnknowValue = traceUnknowValue;

            string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
            string extension = ope == TestUnitOpe.ReadQuestions ? ".questions.json" : ".responses.json";
            int okCount = 0;
            int notOkCount = 0;
            int noOkCount = 0;
            int errorCount = 0;
            foreach (string directory in directories)
            {
                if (!directory.StartsWith(baseDirectory))
                    throw new PBException($"wrong directory \"{directory}\"");
                string directory2 = directory.Substring(baseDirectory.Length + 1);
                string file = zPath.Combine(outputDirectory, directory2 + extension);
                Trace.WriteLine($"create file \"{directory2 + extension}\"");
                if (ExecOpe(questionsManager, ope, directory, file, v2))
                {
                    string okFile = zPath.Combine(zPath.GetDirectoryName(file), "ok", zPath.GetFileName(file));
                    if (zFile.Exists(okFile))
                    {
                        if (zfile.AreFileEqual(file, okFile))
                        {
                            Trace.WriteLine("  ok");
                            okCount++;
                        }
                        else
                        {
                            Trace.WriteLine("  not ok");
                            notOkCount++;
                        }
                    }
                    else
                    {
                        Trace.WriteLine("  ok file not found");
                        noOkCount++;
                    }
                }
                else
                    errorCount++;
            }
            Trace.WriteLine($"{okCount} ok {notOkCount} not ok {errorCount} error {noOkCount} ok not found");
        }

        private static bool ExecOpe(QuestionsManager questionsManager, TestUnitOpe ope, string directory, string file, bool v2)
        {
            try
            {
                questionsManager.SetDirectory(directory);
                if (ope == TestUnitOpe.ReadQuestions)
                {
                    if (v2)
                        questionsManager.ReadQuestions_v2().zSave(file, jsonIndent: true);
                    else
                        questionsManager.ReadQuestions().zSave(file, jsonIndent: true);
                }
                else //if (ope == TestUnitOpe.ReadResponses)
                {
                    if (v2)
                        questionsManager.ReadResponses_v2().zSave(file, jsonIndent: true);
                    else
                        questionsManager.ReadResponses().zSave(file, jsonIndent: true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"  error : {ex.Message}");
                zFile.Delete(file);
                return false;
            }
        }

        //private static void CreateQuestionsManager(bool v2, bool traceUnknowValue)
        //{
        //    _questionsManager = QuestionRun.CreateQuestionsManager(v2: v2);
        //    _questionsManager.TraceUnknowValue = traceUnknowValue;
        //}
    }
}

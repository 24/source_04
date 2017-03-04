using pb;
using pb.Data.Mongo;
using pb.IO;
using System.Collections.Generic;

namespace anki
{
    public static partial class QuestionResponses
    {
        public static IEnumerable<string> GetQuestionFiles(string directory)
        {
            SortedDictionary<string, string> files = new SortedDictionary<string, string>();
            // question-01-2015-016.json
            foreach (string file in zDirectory.EnumerateFiles(directory, "question-*.json"))
            {
                FileNumber fileNumber = FileNumber.GetFileNumber(file);
                if (!files.ContainsKey(fileNumber.BaseFilename))
                    files.Add(fileNumber.BaseFilename, null);
            }
            return files.Keys;
        }

        public static QuestionResponseHtml LoadQuestion(string directory, string file, bool trace = false)
        {
            file = GetLastFileNumber(zPath.Combine(directory, "data", file));
            if (trace)
                Trace.WriteLine($"LoadQuestion() : file \"{file}\"");
            return zMongo.ReadFileAs<QuestionResponseHtml>(file);
        }

        public static void SaveQuestion(string directory, string file, string questionHtml, bool trace = false)
        {
            QuestionResponseHtml question = LoadQuestion(directory, file);

            question.QuestionHtml = questionHtml;
            file = GetLastFileNumber(zPath.Combine(directory, "data", file), saveFile: true);
            if (trace)
                Trace.WriteLine($"SaveQuestion() : file \"{file}\"");
            question.zSave(file, jsonIndent: true);
        }

        private static string GetLastFileNumber(string file, bool saveFile = false)
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
                {
                    if (saveFile && index == 2)
                        lastFile = file2;
                    break;
                }
                lastFile = file2;
                index++;
            }
            return lastFile;
        }
    }
}

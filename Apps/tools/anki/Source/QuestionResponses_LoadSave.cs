using pb;
using pb.Data.Mongo;
using pb.IO;
using System.Collections.Generic;

namespace anki
{
    public static partial class QuestionResponses
    {
        // directory is pdf directory
        public static IEnumerable<string> GetQuestionFiles(string directory)
        {
            SortedDictionary<string, string> files = new SortedDictionary<string, string>();
            // question-01-2015-016.json
            directory = GetQuestionsDirectory(directory);
            if (!zDirectory.Exists(directory))
                throw new PBException($"directory not found \"{directory}\"");
            foreach (string file in zDirectory.EnumerateFiles(directory, "question-*.json"))
            {
                FileNumber fileNumber = FileNumber.GetFileNumber(file);
                if (!files.ContainsKey(fileNumber.BaseFilename))
                    files.Add(fileNumber.BaseFilename, null);
            }
            return files.Keys;
        }

        // directory is pdf directory
        public static QuestionResponseHtml LoadQuestion(string directory, string filename, bool trace = false)
        {
            //string file = GetLastFileNumber(zPath.Combine(directory, "data", filename));
            directory = GetQuestionsDirectory(directory);
            if (!zDirectory.Exists(directory))
                throw new PBException($"directory not found \"{directory}\"");
            string file = GetLastFileNumber(zPath.Combine(directory, filename));
            if (trace)
                Trace.WriteLine($"LoadQuestion() : file \"{file}\"");
            return zMongo.ReadFileAs<QuestionResponseHtml>(file);
        }

        // directory is pdf directory
        public static void SaveQuestion(string directory, string file, string questionHtml, bool trace = false)
        {
            QuestionResponseHtml question = LoadQuestion(directory, file);

            question.QuestionHtml = questionHtml;
            // zPath.Combine(directory, "data", file)
            file = GetLastFileNumber(zPath.Combine(GetQuestionsDirectory(directory), file), saveFile: true);
            if (trace)
                Trace.WriteLine($"SaveQuestion() : file \"{file}\"");
            question.zSave(file, jsonIndent: true);
        }

        public static string GetQuestionsDirectory(string directory)
        {
            return zPath.Combine(directory, @"data\questions");
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

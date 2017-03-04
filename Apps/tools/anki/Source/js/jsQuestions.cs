using pb;
using pb.Data.Mongo.Serializers;
using pb.IO;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
//#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

namespace anki.js
{
    public class jsQuestions
    {
        static jsQuestions()
        {
            TraceManager.Current.AddTrace(Trace.Current);
            TraceManager.Current.SetWriter(WriteToFile.Create(@"log\_log.txt".zRootPath(zapp.GetExecutingAssemblyDirectory()), FileOption.IndexedFile), "_default");
            MongoSerializationManager.SetDefaultMongoSerializationOptions();
        }

        public async Task<object> GetQuestionFiles(object directory)
        {
            //return _LoadQuestions((string)directory);
            Trace.WriteLine($"GetQuestionFiles() : directory \"{directory}\"");
            return QuestionResponses.GetQuestionFiles(zPath.Combine((string)directory, "data"));
        }

        //private static IEnumerable<string> _LoadQuestions(string directory)
        //{
        //    Trace.WriteLine($"LoadQuestions() : directory \"{directory}\"");
        //    directory = zPath.Combine(directory, "data");
        //    SortedDictionary<string, string> files = new SortedDictionary<string, string>();
        //    // question-01-2015-016.json
        //    foreach (string file in zDirectory.EnumerateFiles(directory, "question-*.json"))
        //    {
        //        FileNumber fileNumber = FileNumber.GetFileNumber(file);
        //        if (!files.ContainsKey(fileNumber.BaseFilename))
        //            files.Add(fileNumber.BaseFilename, null);
        //    }
        //    return files.Keys;
        //}

        public async Task<object> LoadQuestion(dynamic param)
        {
            // LoadQuestion : string param.Directory, string param.File
            //return _LoadQuestion((string)param.Directory, (string)param.File);
            return QuestionResponses.LoadQuestion((string)param.Directory, (string)param.File, trace: true);
        }

        //private static QuestionResponseHtml _LoadQuestion(string directory, string file)
        //{
        //    file = GetLastFileNumber(zPath.Combine(directory, "data", file));
        //    Trace.WriteLine($"LoadQuestion() : file \"{file}\"");
        //    return zMongo.ReadFileAs<QuestionResponseHtml>(file);
        //}

        public async Task<object> SaveQuestion(dynamic param)
        {
            // SaveQuestion : string param.Directory, string param.File, string param.QuestionHtml
            //_SaveQuestion((string)param.Directory, (string)param.File, (string)param.QuestionHtml);
            QuestionResponses.SaveQuestion((string)param.Directory, (string)param.File, (string)param.QuestionHtml);
            return null;
        }

        //private static void _SaveQuestion(string directory, string file, string questionHtml)
        //{
        //    //Trace.WriteLine($"LoadQuestion() : directory \"{directory}\" file \"{file}\"");
        //    //file = zPath.Combine(directory, "data", file);
        //    //string file2 = GetLastFileNumber(file);
        //    //QuestionResponseHtml question = zMongo.ReadFileAs<QuestionResponseHtml>(file2);

        //    QuestionResponseHtml question = _LoadQuestion(directory, file);

        //    question.QuestionHtml = questionHtml;
        //    //FileNumber fileNumber = FileNumber.GetFileNumber(file);
        //    //Trace.WriteLine($"SaveQuestion() : fileNumber.Number {fileNumber.Number} file \"{file}\"");
        //    //if (fileNumber.Number == 0)
        //    //    file = fileNumber.GetPath("_02");
        //    file = GetLastFileNumber(zPath.Combine(directory, "data", file), saveFile: true);
        //    Trace.WriteLine($"SaveQuestion() : file \"{file}\"");
        //    question.zSave(file, jsonIndent: true);
        //}

        //private static string GetLastFileNumber(string file, bool saveFile = false)
        //{
        //    string lastFile = file;
        //    string directory = zPath.GetDirectoryName(file);
        //    string filename = zPath.GetFileNameWithoutExtension(file);
        //    string ext = zPath.GetExtension(file);
        //    int index = 2;
        //    while (true)
        //    {
        //        string file2 = zPath.Combine(directory, filename + $"_{index:00}" + ext);
        //        if (!zFile.Exists(file2))
        //        {
        //            if (saveFile && index == 2)
        //                lastFile = file2;
        //            break;
        //        }
        //        lastFile = file2;
        //        index++;
        //    }
        //    return lastFile;
        //}
    }
}

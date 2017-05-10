using pb;
using pb.Data.Xml;
using pb.Text;

namespace anki
{
    public class QuestionResponseFiles
    {
        public string BaseDirectory;
        public string[] QuestionFiles;
        public string ResponseFile;
    }

    public static class QuestionRun
    {
        public static RegexValuesList GetQuestionRegexValuesList()
        {
            return new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
        }

        public static RegexValuesList GetResponseRegexValuesList()
        {
            return new RegexValuesList(XmlConfig.CurrentConfig.GetElements("ResponseInfos/ResponseInfo"), compileRegex: true);
        }

        public static RegexValuesList GetQuestionRegexValuesList_v2()
        {
            return new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos_v2/QuestionInfo"), compileRegex: true);
        }

        public static RegexValuesList GetResponseRegexValuesList_v2()
        {
            return new RegexValuesList(XmlConfig.CurrentConfig.GetElements("ResponseInfos_v2/ResponseInfo"), compileRegex: true);
        }

        // string directory
        public static QuestionsManager CreateQuestionsManager(string directory = null, bool v2 = true)
        {
            QuestionsManager questionsManager = new QuestionsManager();
            string baseDirectory = XmlConfig.CurrentConfig.GetExplicit("CardDirectory");
            //if (!zPath.IsPathRooted(directory))
            //    directory = zPath.Combine(baseDirectory, directory);
            //else if (!directory.StartsWith(baseDirectory))
            //    baseDirectory = null;
            questionsManager.BaseDirectory = baseDirectory;
            //questionsManager.Directory = directory;
            questionsManager.SetDirectory(directory);
            questionsManager.MaxLinesPerQuestion = XmlConfig.CurrentConfig.GetExplicit("MaxLinesPerQuestion").zParseAs<int>();
            questionsManager.MaxLinesPerChoice = XmlConfig.CurrentConfig.GetExplicit("MaxLinesPerChoice").zParseAs<int>();
            if (!v2)
            {
                questionsManager.QuestionRegexValuesList = GetQuestionRegexValuesList();
                questionsManager.ResponseRegexValuesList = GetResponseRegexValuesList();
            }
            else
            {
                questionsManager.QuestionRegexValuesList_v2 = GetQuestionRegexValuesList_v2();
                questionsManager.ResponseRegexValuesList_v2 = GetResponseRegexValuesList_v2();
            }
            return questionsManager;
        }
    }
}

using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using System.Linq;
using System.Xml.Linq;

namespace anki
{
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

        public static void WriteAnkiFile(string ankiFile, string questionFile, string responseFile)
        {
            AnkiWriter.Write(ankiFile, QuestionReader.Read(questionFile, GetQuestionRegexValuesList()), ResponseReader.Read(responseFile, GetResponseRegexValuesList()));
        }

        public static void WriteAnkiCard(string name)
        {
            XElement xe = XmlConfig.CurrentConfig.GetElement($"Cards/Card[@name = '{name}']");
            if (xe == null)
                throw new PBException($"card not found \"{name}\"");
            string directory = xe.zXPathValue("Directory");
            AnkiWriter.Write(zPath.Combine(directory, xe.zXPathValue("AnkiFile")),
                QuestionReader.Read(xe.zXPathValues("QuestionFile").Select(file => zPath.Combine(directory, file)), GetQuestionRegexValuesList()),
                ResponseReader.Read(zPath.Combine(directory, xe.zXPathValue("ResponseFile")), GetResponseRegexValuesList()));
        }
    }
}

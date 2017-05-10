using System.Text;

namespace anki
{
    public class QuestionResponse_v2
    {
    }

    public class Question_v2 : QuestionResponse_v2
    {
        public int Year;
        public QuestionType Type;
        //public int QuestionNumber;
        public int QuestionNumber;
        public string QuestionText;
        public string[] Choices;
        public string SourceFile;
        public int SourceLine;
    }

    public class Response_v2 : QuestionResponse_v2
    {
        public string Category;
        public int Year;
        public int QuestionNumber;
        public string Responses;
        public string SourceFile;
        public int SourceLine;

        public string GetFormatedResponse()
        {
            return GetFormatedResponse(Responses);
        }

        public static string GetFormatedResponse(string responses)
        {
            if (responses == null)
                return "(unknow response)";
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (char responseCode in responses)
            {
                if (!first)
                    sb.Append(" - ");
                sb.Append(responseCode);
                first = false;
            }
            return sb.ToString();
        }
    }
}

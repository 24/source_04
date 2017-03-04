namespace anki
{
    // used to save data
    public class QuestionResponseHtml
    {
        public int Year;
        public QuestionType Type;
        public int Number;
        public string QuestionText;
        public string QuestionHtml;
        public string[] Choices;
        public string Responses;
        public string SourceFile;
        public int SourceLine;
    }
}

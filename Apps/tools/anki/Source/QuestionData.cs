using pb;
using pb.Data;
using pb.Data.Text;
using pb.Text;
using System.Text;

namespace anki
{
    public enum QuestionDataType
    {
        EmptyLine,
        Unknow,
        QuestionYear,
        QuestionType,
        QuestionNumber,
        QuestionText,
        QuestionChoice,
        Responses,
        ResponseCategory,
        ResponseYear,
        Response,
        PageNumber
    }

    public class QuestionData
    {
        public QuestionDataType Type;
        public string File;
        public string Filename;
        public int LineNumber;
        public string Line;
        public string TrimedLine;

        public QuestionData(TextData data)
        {
            File = data.File;
            Filename = data.Filename;
            Line = data.Line;
            TrimedLine = data.Line.Trim();
            LineNumber = data.LineNumber;
            Type = TrimedLine == "" ? QuestionDataType.EmptyLine : QuestionDataType.Unknow;
        }

        public static QuestionData CreateQuestionData(TextData data)
        {
            if (data.Values.ContainsKey("questionYear"))
                return new QuestionDataQuestionYear(data);
            else if (data.Values.ContainsKey("questionType"))
                return new QuestionDataQuestionType(data);
            else if (data.Values.ContainsKey("questionNumber"))
                return new QuestionDataQuestionNumber(data);
            else if (data.Values.ContainsKey("questionChoice"))
                return new QuestionDataQuestionChoice(data);
            else if (data.Values.ContainsKey("responses"))
                return new QuestionData(data) { Type = QuestionDataType.Responses };
            else if (data.Values.ContainsKey("responseCategory"))
                return new QuestionDataResponseCategory(data);
            else if (data.Values.ContainsKey("responseYear"))
                return new QuestionDataResponseYear(data);
            else if (data.Values.ContainsKey("responseCharacterResponseCodes") || data.Values.ContainsKey("responseNumericResponseCodes"))
                return new QuestionDataResponse(data);
            else if (data.Values.ContainsKey("pageNumber"))
                return new QuestionDataPageNumber(data);
            else
                return new QuestionData(data);
        }
    }

    public class QuestionDataQuestionYear : QuestionData
    {
        public int Year;

        public QuestionDataQuestionYear(TextData data) : base(data)
        {
            Type = QuestionDataType.QuestionYear;
            Year = int.Parse((string)data.Values["questionYear"].Value);
        }
    }

    public class QuestionDataQuestionType : QuestionData
    {
        public QuestionType QuestionType;

        public QuestionDataQuestionType(TextData data) : base(data)
        {
            Type = QuestionDataType.QuestionType;
            string questionTypeText = (string)data.Values["questionType"].Value;
            QuestionType? questionType = GetQuestionType(questionTypeText);
            if (questionType == null)
                throw new PBFileException($"unknow question type \"{questionTypeText}\" line {data.LineNumber} file \"{data.File}\"", data.File, data.LineNumber);
            QuestionType = (QuestionType)questionType;
        }

        private static QuestionType? GetQuestionType(string questionType)
        {
            switch (questionType.ToLower())
            {
                case "simple":
                case "simples":
                case "qcs":
                    return QuestionType.Simple;
                case "multiple":
                case "multiples":
                case "qcm":
                    return QuestionType.Multiples;
                case "association":
                    return QuestionType.Association;
                case "qta":
                    return QuestionType.QTA;
                default:
                    return null;
            }
        }
    }

    public class QuestionDataQuestionNumber : QuestionData
    {
        public int QuestionNumber;
        public int ColumnIndex;
        public int Length;

        public QuestionDataQuestionNumber(TextData data) : base(data)
        {
            Type = QuestionDataType.QuestionNumber;
            RegexValue<ZValue> value = data.Values["questionNumber"];
            QuestionNumber = int.Parse((string)value.Value);
            ColumnIndex = value.Index;
            Length = value.Length;
        }
    }

    public class QuestionDataQuestionChoice : QuestionData
    {
        public int ChoiceNumber;
        public string ChoiceText;

        public QuestionDataQuestionChoice(TextData data) : base(data)
        {
            Type = QuestionDataType.QuestionChoice;
            if (data.Values.ContainsKey("questionNumericChoiceCode"))
                ChoiceNumber = int.Parse((string)data.Values["questionNumericChoiceCode"].Value);
            else
                ChoiceNumber = char.ToUpper(((string)data.Values["questionCharacterChoiceCode"].Value)[0]) - 'A' + 1;
            //ChoiceText = (string)data.Values["questionChoice"];
            ChoiceText = ((string)data.Values["questionChoice"].Value).Trim();
        }
    }

    public class QuestionDataResponseCategory : QuestionData
    {
        public string Category;

        public QuestionDataResponseCategory(TextData data) : base(data)
        {
            Type = QuestionDataType.ResponseCategory;
            Category = (string)data.Values["responseCategory"].Value;
        }
    }

    public class QuestionDataResponseYear : QuestionData
    {
        public int Year;
        public int ColumnIndex;
        public int Length;

        public QuestionDataResponseYear(TextData data) : base(data)
        {
            Type = QuestionDataType.ResponseYear;
            RegexValue<ZValue>  value = data.Values["responseYear"];
            Year = int.Parse((string)value.Value);
            ColumnIndex = value.Index;
            Length = value.Length;
        }
    }

    public class QuestionDataResponse : QuestionData
    {
        //public int ChoiceNumber;
        public string Responses;
        public int ColumnIndex;
        public int Length;

        public QuestionDataResponse(TextData data) : base(data)
        {
            Type = QuestionDataType.Response;
            RegexValue<ZValue> value;
            if (data.Values.ContainsKey("responseNumericResponseCodes"))
            {
                value = data.Values["responseNumericResponseCodes"];
                //ChoiceNumber = int.Parse((string)value.Value);
                StringBuilder sb = new StringBuilder();
                foreach (char c in (string)value.Value)
                    sb.Append((char)(c + 'A' - '1'));
                Responses = sb.ToString();
            }
            else
            {
                value = data.Values["responseCharacterResponseCodes"];
                //ChoiceNumber = char.ToUpper(((string)value.Value)[0]) - 'A' + 1;
                Responses = (string)data.Values["responseCharacterResponseCodes"].Value;
            }
            ColumnIndex = value.Index;
            Length = value.Length;
        }
    }

    public class QuestionDataPageNumber : QuestionData
    {
        public int PageNumber;
        public int? TotalPage;

        public QuestionDataPageNumber(TextData data) : base(data)
        {
            Type = QuestionDataType.PageNumber;
            PageNumber = int.Parse(((string)data.Values["pageNumber"].Value).Replace(" ", ""));
            if (data.Values.ContainsKey("totalPage"))
                TotalPage = int.Parse(((string)data.Values["totalPage"].Value).Replace(" ", ""));
        }
    }
}

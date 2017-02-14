using pb;
using pb.Data;
using pb.IO;
using pb.Text;
using System.Collections.Generic;

namespace anki
{
    public enum QuestionType
    {
        None,
        Simple,
        Multiples
    }

    public class Question
    {
        //public int? Year;
        public int Year;
        public QuestionType Type;
        public int Number;
        public string QuestionText;
        public string[] Responses;
    }

    public class QuestionReader
    {
        private enum QuestionValueType
        {
            None,
            Year,
            QuestionType,
            QuestionNumber,
            Question,
            Response
        }

        private class QuestionValue
        {
            public QuestionValueType Type;
        }

        private class QuestionValueYear : QuestionValue
        {
            public int Year;

            public QuestionValueYear()
            {
                Type = QuestionValueType.Year;
            }
        }

        private class QuestionValueQuestionType : QuestionValue
        {
            public QuestionType QuestionType;

            public QuestionValueQuestionType()
            {
                Type = QuestionValueType.QuestionType;
            }
        }

        private class QuestionValueQuestionNumber : QuestionValue
        {
            public int QuestionNumber;

            public QuestionValueQuestionNumber()
            {
                Type = QuestionValueType.QuestionNumber;
            }
        }

        //private class QuestionValueQuestion : QuestionValue
        //{
        //    public string Question;

        //    public QuestionValueQuestion()
        //    {
        //        Type = QuestionValueType.Question;
        //    }
        //}

        private class QuestionValueResponse : QuestionValue
        {
            public char ResponseCode;
            public string Response;

            public QuestionValueResponse()
            {
                Type = QuestionValueType.Response;
            }
        }

        private class QuestionTmp
        {
            //public int? Year;
            public int Year;
            public QuestionType Type;
            public int Number;
            public string QuestionText;
            public int QuestionLineCount;
            public List<string> Responses = new List<string>();
            public int ResponseLineCount;
        }

        private string _file = null;
        private RegexValuesList _regexList = null;

        private int _lineNumber = 0;
        private int? _year = null;
        private QuestionType _questionType = QuestionType.None;
        //private int? _questionNumber = null;
        //private char? _responseCode = null;
        //private List<string> _responses = null;
        //private QuestionValue _lastValue = null;
        private QuestionValueType _lastValueType = QuestionValueType.None;
        private QuestionTmp _question = null;

        private IEnumerable<Question> _Read(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                _file = file;
                //_year = null;
                //_questionType = QuestionType.None;
                _lastValueType = QuestionValueType.None;
                _question = null;
                _lineNumber = 0;
                foreach (string line in zFile.ReadLines(file))
                {
                    _lineNumber++;
                    string line2 = line.Trim();
                    if (line2 == "")
                    {
                        if (_lastValueType == QuestionValueType.Response)
                        {
                            if (_question != null)
                                yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
                            _question = null;
                            _lastValueType = QuestionValueType.None;
                        }
                        continue;
                    }

                    FindText findText = _regexList.Find(line2);
                    if (findText.Found)
                    {
                        bool newQuestion = false;
                        QuestionTmp question = null;
                        QuestionValue value = GetQuestionValue(findText.matchValues.GetValues());
                        switch (value.Type)
                        {
                            case QuestionValueType.Year:
                                newQuestion = true;
                                _year = ((QuestionValueYear)value).Year;
                                break;
                            case QuestionValueType.QuestionType:
                                newQuestion = true;
                                _questionType = ((QuestionValueQuestionType)value).QuestionType;
                                break;
                            case QuestionValueType.QuestionNumber:
                                newQuestion = true;
                                if (_year == null)
                                    throw new PBException($"unknow year, line {_lineNumber} file \"{_file}\"");
                                question = new QuestionTmp { Year = (int)_year, Type = _questionType, Number = ((QuestionValueQuestionNumber)value).QuestionNumber, QuestionLineCount = 0 };
                                break;
                            case QuestionValueType.Response:
                                if (_question == null)
                                    throw new PBException($"unknow question, line {_lineNumber} file \"{_file}\"");
                                AddResponse(_question, (QuestionValueResponse)value);
                                break;
                            case QuestionValueType.None:
                                break;
                            default:
                                throw new PBException($"wrong value \"{line2}\" line {_lineNumber} file \"{_file}\"");
                        }
                        if (newQuestion)
                        {
                            if (_question != null)
                                yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
                            _question = question;
                        }
                        _lastValueType = value.Type;
                    }
                    else
                    {
                        switch (_lastValueType)
                        {
                            case QuestionValueType.QuestionNumber:
                                _question.QuestionText = line2;
                                _question.QuestionLineCount++;
                                _lastValueType = QuestionValueType.Question;
                                break;
                            case QuestionValueType.Question:
                                if (_question.QuestionLineCount > 1)
                                    throw new PBException($"to many lines for question \"{line2}\" line {_lineNumber} file \"{_file}\"");
                                _question.QuestionText += " " + line2;
                                _question.QuestionLineCount++;
                                break;
                            case QuestionValueType.Response:
                                if (_question.ResponseLineCount > 1)
                                    throw new PBException($"to many lines for response \"{line2}\" line {_lineNumber} file \"{_file}\"");
                                int i = _question.Responses.Count - 1;
                                _question.Responses[i] += " " + line2;
                                _question.ResponseLineCount++;
                                break;
                            case QuestionValueType.None:
                                break;
                            default:
                                if (_year != null)
                                    throw new PBException($"wrong value \"{line2}\" line {_lineNumber} file \"{_file}\"");
                                break;
                        }
                    }
                }
                if (_question != null)
                    yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
            }
        }

        private void AddResponse(QuestionTmp question, QuestionValueResponse response)
        {
            if (question.QuestionText == null)
                throw new PBException($"missing question line {_lineNumber} file \"{_file}\"");
            if (response.ResponseCode != (char)('A' + question.Responses.Count))
                throw new PBException($"wrong response code \"{response.ResponseCode}\" line {_lineNumber} file \"{_file}\"");
            question.Responses.Add(response.Response);
            question.ResponseLineCount = 1;
        }

        private QuestionValue GetQuestionValue(NamedValues<ZValue> namedValues)
        {
            if (namedValues.Count == 0)
                throw new PBException($"wrong question value line {_lineNumber} file \"{_file}\"");

            //KeyValuePair<string, ZValue> firstValue = namedValues.First();
            //QuestionValue questionValue = GetValue(firstValue.Key, firstValue.Value);
            QuestionValue questionValue = null;
            foreach (KeyValuePair<string, ZValue> namedValue in namedValues)
            {
                if (questionValue == null)
                    questionValue = GetValue(namedValue.Key, namedValue.Value);
                else
                    SetValue(questionValue, namedValue.Key, namedValue.Value);
            }
            return questionValue;
        }

        private QuestionValue GetValue(string name, ZValue value)
        {
            switch (name.ToLower())
            {
                case "year":
                    return new QuestionValueYear { Year = int.Parse((string)value) };
                case "questiontype":
                    return new QuestionValueQuestionType { QuestionType = ((string)value).zParseEnum<QuestionType>(ignoreCase: true) };
                case "questionnumber":
                    return new QuestionValueQuestionNumber { QuestionNumber = int.Parse((string)value) };
                case "responsecode":
                    return new QuestionValueResponse { ResponseCode = ((string)value)[0] };
                //case "pagenumber":
                //    return new QuestionValue { Type = QuestionValueType.None };
                default:
                    throw new PBException($"unknow value \"{name}\" = {value} line {_lineNumber} file \"{_file}\"");
            }
        }

        private void SetValue(QuestionValue questionValue, string name, ZValue value)
        {
            switch (name.ToLower())
            {
                case "response":
                    ((QuestionValueResponse)questionValue).Response = (string)value;
                    break;
                default:
                    throw new PBException($"unknow value \"{name}\" = {value} line {_lineNumber} file \"{_file}\"");
            }
        }

        public static IEnumerable<Question> Read(string file, RegexValuesList regexList)
        {
            return new QuestionReader { _regexList = regexList }._Read(new string[] { file });
        }

        public static IEnumerable<Question> Read(IEnumerable<string> files, RegexValuesList regexList)
        {
            return new QuestionReader { _regexList = regexList }._Read(files);
        }
    }
}

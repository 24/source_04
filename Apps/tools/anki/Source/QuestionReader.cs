using pb;
using pb.Data;
using pb.IO;
using pb.Text;
using System.Collections.Generic;

namespace anki
{
    public class Question
    {
        //public int? Year;
        public int Year;
        public QuestionType Type;
        public int Number;
        public string QuestionText;
        public string[] Choices;
        public string SourceFile;
        public int SourceLine;
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

        //private class AssociationQuestionTmp
        private class QuestionBaseTmp
        {
            public QuestionType Type;
            public string QuestionText;
            public int QuestionLineCount;
            public List<string> Choices = new List<string>();
            public int ResponseLineCount;
        }

        private class QuestionTmp : QuestionBaseTmp
        {
            public int Year;
            //public QuestionType Type;
            public int Number;
            //public string QuestionText;
            //public int QuestionLineCount;
            //public List<string> Responses = new List<string>();
            //public int ResponseLineCount;
            public string SourceFile;
            public int SourceLine;
        }

        private class QuestionAssociationTmp : QuestionBaseTmp
        {
            public bool Complete;
        }

        private string _file = null;
        private RegexValuesList _regexList = null;

        private bool _manageAssociationQuestion = true;
        private int _maxLinesPerQuestion = 7;

        private int _lineNumber = 0;
        private int? _year = null;
        private QuestionType _questionType = QuestionType.None;
        //private int? _questionNumber = null;
        //private char? _responseCode = null;
        //private List<string> _responses = null;
        //private QuestionValue _lastValue = null;
        private QuestionValueType _lastValueType = QuestionValueType.None;
        private QuestionTmp _question = null;
        private QuestionAssociationTmp _associationQuestion = null;

        private IEnumerable<Question> _Read(IEnumerable<string> files, string baseDirectory = null)
        {
            foreach (string file in files)
            {
                _file = file;
                //_year = null;
                //_questionType = QuestionType.None;
                _lastValueType = QuestionValueType.None;
                _question = null;
                _lineNumber = 0;
                //Trace.WriteLine($"read question file \"{file}\"");
                foreach (string line in zFile.ReadLines(file))
                {
                    _lineNumber++;
                    string line2 = line.Trim();
                    if (line2 == "")
                    {
                        if (_lastValueType == QuestionValueType.Response)
                        {
                            if (_question != null)
                            {
                                yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Choices = _question.Choices.ToArray(),
                                    SourceFile = _question.SourceFile, SourceLine = _question.SourceLine };
                                _question = null;
                            }
                            else if (_associationQuestion != null)
                            {
                                _associationQuestion.Complete = true;
                            }
                            _lastValueType = QuestionValueType.None;
                        }
                        continue;
                    }

                    FindText_v2 findText = _regexList.Find(line2);
                    if (findText.Success)
                    {
                        bool newQuestion = false;
                        QuestionTmp question = null;
                        QuestionValue value = GetQuestionValue(findText.GetValues());
                        switch (value.Type)
                        {
                            case QuestionValueType.Year:
                                newQuestion = true;
                                _year = ((QuestionValueYear)value).Year;
                                _associationQuestion = null;
                                break;
                            case QuestionValueType.QuestionType:
                                newQuestion = true;
                                _questionType = ((QuestionValueQuestionType)value).QuestionType;
                                _associationQuestion = null;
                                break;
                            case QuestionValueType.QuestionNumber:
                                newQuestion = true;
                                if (_year == null)
                                    throw new PBFileException($"unknow year, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                                string sourceFile = _file;
                                if (sourceFile.StartsWith(baseDirectory))
                                {
                                    int l = baseDirectory.Length;
                                    if (sourceFile[l] == '\\')
                                        l++;
                                    sourceFile = sourceFile.Substring(l);
                                }
                                question = new QuestionTmp { Year = (int)_year, Type = _questionType, Number = ((QuestionValueQuestionNumber)value).QuestionNumber, QuestionLineCount = 0, SourceFile = sourceFile, SourceLine = _lineNumber };
                                if (_associationQuestion != null)
                                    _associationQuestion.Complete = true;
                                break;
                            case QuestionValueType.Response:
                                //if (_question == null)
                                //    throw new PBFileException($"unknow question, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                                //AddResponse(_question, (QuestionValueResponse)value);
                                AddResponse((QuestionValueResponse)value);
                                break;
                            case QuestionValueType.None:
                                break;
                            default:
                                throw new PBFileException($"wrong value \"{line2}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                        }
                        if (newQuestion)
                        {
                            if (_question != null)
                                //yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
                                yield return GetQuestion();
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
                                //if (_question.QuestionLineCount > _maxLinesPerQuestion - 1)
                                //    throw new PBFileException($"to many lines for question \"{line2}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                                //_question.QuestionText += " " + line2;
                                //_question.QuestionLineCount++;
                                AddQuestion(line2);
                                break;
                            case QuestionValueType.Response:
                                //if (_question.ResponseLineCount > 1)
                                //    throw new PBFileException($"to many lines for response \"{line2}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                                //int i = _question.Responses.Count - 1;
                                //_question.Responses[i] += " " + line2;
                                //_question.ResponseLineCount++;
                                AddResponse(line2);
                                break;
                            case QuestionValueType.None:
                                break;
                            default:
                                if (_lastValueType == QuestionValueType.QuestionType && _questionType == QuestionType.Association && _manageAssociationQuestion)
                                {
                                    _associationQuestion = new QuestionAssociationTmp { Type = QuestionType.Association, QuestionText = line2, QuestionLineCount = 1, Complete = false };
                                    _lastValueType = QuestionValueType.Question;
                                }
                                else if (_year != null)
                                    throw new PBFileException($"wrong value \"{line2}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                                break;
                        }
                    }
                }
                if (_question != null)
                    //yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
                    yield return GetQuestion();
            }
        }

        private Question GetQuestion()
        {
            //Question question = new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
            Question question = new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, SourceFile = _question.SourceFile, SourceLine = _question.SourceLine };
            if (_associationQuestion != null)
            {
                question.QuestionText = _associationQuestion.QuestionText + "<br>" + question.QuestionText;
                question.Choices = _associationQuestion.Choices.ToArray();
            }
            else
            {
                question.QuestionText = _question.QuestionText;
                question.Choices = _question.Choices.ToArray();
            }
            _question = null;
            return question;
        }

        private void AddQuestion(string questionText)
        {
            QuestionBaseTmp question;
            if (_question != null)
                question = _question;
            else if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow question, association question is complete, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                question = _associationQuestion;
            }
            else
                throw new PBFileException($"unknow question, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);

            if (question.QuestionLineCount > _maxLinesPerQuestion - 1)
                throw new PBFileException($"to many lines for question \"{questionText}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
            question.QuestionText += " " + questionText;
            question.QuestionLineCount++;
        }

        // QuestionTmp question
        private void AddResponse(QuestionValueResponse response)
        {
            QuestionBaseTmp question;
            if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow response, association question is complete, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                question = _associationQuestion;
            }
            else if (_question != null)
                question = _question;
            else
                throw new PBFileException($"unknow question, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);

            if (question.QuestionText == null)
                throw new PBFileException($"missing question line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
            if (response.ResponseCode != (char)('A' + question.Choices.Count))
                throw new PBFileException($"wrong response code \"{response.ResponseCode}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
            question.Choices.Add(response.Response);
            question.ResponseLineCount = 1;
        }

        private void AddResponse(string response)
        {
            QuestionBaseTmp question;
            if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow response, association question is complete, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
                question = _associationQuestion;
            }
            else if (_question != null)
                question = _question;
            else
                throw new PBFileException($"unknow question, line {_lineNumber} file \"{_file}\"", _file, _lineNumber);

            if (question.ResponseLineCount > 1)
                throw new PBFileException($"to many lines for response \"{response}\" line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
            int i = question.Choices.Count - 1;
            question.Choices[i] += " " + response;
            question.ResponseLineCount++;
        }

        private QuestionValue GetQuestionValue(NamedValues<ZValue> namedValues)
        {
            if (namedValues.Count == 0)
                throw new PBFileException($"wrong question value line {_lineNumber} file \"{_file}\"", _file, _lineNumber);

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
                    //return new QuestionValueQuestionType { QuestionType = ((string)value).zParseEnum<QuestionType>(ignoreCase: true) };
                    return new QuestionValueQuestionType { QuestionType = GetQuestionType((string)value) };
                case "questionnumber":
                    return new QuestionValueQuestionNumber { QuestionNumber = int.Parse((string)value) };
                case "responsecode":
                    return new QuestionValueResponse { ResponseCode = ((string)value)[0] };
                //case "pagenumber":
                //    return new QuestionValue { Type = QuestionValueType.None };
                default:
                    throw new PBFileException($"unknow value \"{name}\" = {value} line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
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
                    throw new PBFileException($"unknow value \"{name}\" = {value} line {_lineNumber} file \"{_file}\"", _file, _lineNumber);
            }
        }

        private QuestionType GetQuestionType(string questionType)
        {
            switch (questionType.ToLower())
            {
                case "simple":
                case "simples":
                    return QuestionType.Simple;
                case "multiples":
                    return QuestionType.Multiples;
                case "association":
                    return QuestionType.Association;
                default:
                    throw new PBFileException($"unknow question type \"{questionType}\"", _file, _lineNumber);
            }
        }

        public static IEnumerable<Question> Read(string file, RegexValuesList regexList, string baseDirectory = null)
        {
            return new QuestionReader { _regexList = regexList }._Read(new string[] { file }, baseDirectory);
        }

        public static IEnumerable<Question> Read(IEnumerable<string> files, RegexValuesList regexList, string baseDirectory = null)
        {
            return new QuestionReader { _regexList = regexList }._Read(files, baseDirectory);
        }
    }
}

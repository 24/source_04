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

            public override string ToString()
            {
                return Type.ToString();
            }
        }

        private class QuestionValueYear : QuestionValue
        {
            public int Year;

            public QuestionValueYear()
            {
                Type = QuestionValueType.Year;
            }

            public override string ToString()
            {
                return $"year {Year}";
            }
        }

        private class QuestionValueQuestionType : QuestionValue
        {
            public QuestionType QuestionType;

            public QuestionValueQuestionType()
            {
                Type = QuestionValueType.QuestionType;
            }

            public override string ToString()
            {
                return $"question type {QuestionType}";
            }
        }

        private class QuestionValueQuestionNumber : QuestionValue
        {
            public int QuestionNumber;

            public QuestionValueQuestionNumber()
            {
                Type = QuestionValueType.QuestionNumber;
            }

            public override string ToString()
            {
                return $"question {QuestionNumber}";
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

            public override string ToString()
            {
                return $"response {ResponseCode} - {Response}";
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

        private static bool _trace = false;
        private static string _newLine = "\r\n";

        private string _file = null;
        private string _filename = null;

        private bool _manageAssociationQuestion = true;
        //private int _maxLinesPerQuestion = 7;
        private int _maxLinesPerQuestion = -1;
        private int _maxLinesPerResponse = -1;
        private int _maxEmptyLine = 5;
        private bool _traceUnknowHeaderValue = false;
        private bool _traceUnknowEndOfPageValue = false;
        private bool _traceUnknowValue = true;
        private RegexValuesList _regexList = null;

        private int _lineNumber = 0;
        private bool _endOfPage = false;
        private int? _year = null;
        private QuestionType _questionType = QuestionType.None;
        //private int? _questionNumber = null;
        //private char? _responseCode = null;
        //private List<string> _responses = null;
        //private QuestionValue _lastValue = null;
        private QuestionValueType _lastValueType = QuestionValueType.None;
        private QuestionTmp _question = null;
        private QuestionAssociationTmp _associationQuestion = null;
        //private static bool _newVersion = true;    // correction bug pour les questions de type association, ex : UE3 - pH et equilibre acido-basique
        //private static bool _newVersion2 = true;   // autorise une question de type association a ne pas avoir de question que des réponse, ex UE3 - Regulation du bilan hydro-sodé page 16 questions 44, 45, 46 - 2013

        public int MaxLinesPerQuestion { get { return _maxLinesPerQuestion; } set { _maxLinesPerQuestion = value; } }
        public int MaxLinesPerResponse { get { return _maxLinesPerResponse; } set { _maxLinesPerResponse = value; } }
        public RegexValuesList RegexList { get { return _regexList; } set { _regexList = value; } }

        public IEnumerable<Question> Read(IEnumerable<string> files, string baseDirectory = null)
        {
            Trace.WriteLine("read questions files");

            if (_maxLinesPerQuestion == -1)
                throw new PBException("undefined MaxLinesPerQuestion");
            if (_maxLinesPerResponse == -1)
                throw new PBException("undefined MaxLinesPerResponse");

            foreach (string file in files)
            {
                _file = file;
                _filename = zPath.GetFileName(_file);
                //_year = null;
                //_questionType = QuestionType.None;
                _lastValueType = QuestionValueType.None;
                _question = null;
                _lineNumber = 0;
                int emptyLineCount = 0;
                _endOfPage = false;

                if (_trace)
                    Trace.WriteLine($"read question file \"{file}\"");

                foreach (string line in zFile.ReadLines(file))
                {
                    _lineNumber++;
                    string line2 = line.Trim();
                    if (line2 == "")
                    {
                        //if (_lastValueType == QuestionValueType.Response)
                        if (_lastValueType == QuestionValueType.Response || (_associationQuestion != null && _associationQuestion.Complete && _lastValueType == QuestionValueType.Question))
                        {
                            if (_question != null)
                            {
                                //if (!_newVersion)
                                //    yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Choices = _question.Choices.ToArray(),
                                //        SourceFile = _question.SourceFile, SourceLine = _question.SourceLine };
                                //else
                                    yield return GetQuestion();
                                _question = null;
                            }
                            else if (_associationQuestion != null)
                            {
                                _associationQuestion.Complete = true;
                            }
                            _lastValueType = QuestionValueType.None;
                        }

                        if (++emptyLineCount == _maxEmptyLine)
                            _endOfPage = true;

                        continue;
                    }

                    emptyLineCount = 0;

                    //FindText_v2 findText = _regexList.Find(line2);
                    FindText_v2 findText = _regexList.Find(line);
                    if (findText.Success)
                    {
                        bool newQuestion = false;
                        QuestionTmp question = null;
                        QuestionValue value = GetQuestionValue(findText.GetValues());
                        if (_trace)
                            Trace.WriteLine($"  line {_lineNumber} - {value}");
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
                                    throw new PBFileException($"unknow year, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                                string sourceFile = _file;
                                if (baseDirectory != null && sourceFile.StartsWith(baseDirectory))
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
                                //    throw new PBFileException($"unknow question, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                                //AddResponse(_question, (QuestionValueResponse)value);
                                AddResponse((QuestionValueResponse)value);
                                break;
                            case QuestionValueType.None:
                                TraceUnknowValue(line2);
                                break;
                            default:
                                throw new PBFileException($"wrong value \"{line2}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
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
                        if (_trace)
                            Trace.WriteLine($"  line {_lineNumber} - text - {line2}");
                        switch (_lastValueType)
                        {
                            case QuestionValueType.QuestionNumber:
                                _question.QuestionText = line2;
                                _question.QuestionLineCount++;
                                _lastValueType = QuestionValueType.Question;
                                break;
                            case QuestionValueType.Question:
                                //if (_question.QuestionLineCount > _maxLinesPerQuestion - 1)
                                //    throw new PBFileException($"to many lines for question \"{line2}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                                //_question.QuestionText += " " + line2;
                                //_question.QuestionLineCount++;
                                AddQuestion(line2);
                                break;
                            case QuestionValueType.Response:
                                //if (_question.ResponseLineCount > 1)
                                //    throw new PBFileException($"to many lines for response \"{line2}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                                //int i = _question.Responses.Count - 1;
                                //_question.Responses[i] += " " + line2;
                                //_question.ResponseLineCount++;
                                AddResponse(line2);
                                break;
                            case QuestionValueType.None:
                                TraceUnknowValue(line2);
                                break;
                            default:
                                if (_lastValueType == QuestionValueType.QuestionType && _questionType == QuestionType.Association && _manageAssociationQuestion)
                                {
                                    _associationQuestion = new QuestionAssociationTmp { Type = QuestionType.Association, QuestionText = line2, QuestionLineCount = 1, Complete = false };
                                    _lastValueType = QuestionValueType.Question;
                                }
                                //else if (_year != null)
                                //    throw new PBFileException($"wrong value \"{line2}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                                else
                                    TraceUnknowValue(line2);
                                break;
                        }
                    }
                }
                if (_question != null)
                    //yield return new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
                    yield return GetQuestion();
            }
        }

        private void TraceUnknowValue(string line)
        {
            string label = null;
            if (_year == null)
            {
                //Trace.WriteLine($"  unknow header value \"{line}\" line {_lineNumber} file \"{_filename}\"");
                if (_traceUnknowHeaderValue)
                    label = "unknow header";
            }
            else if (_endOfPage)
            {
                //Trace.WriteLine($"  end of page value \"{line}\" line {_lineNumber} file \"{_filename}\"");
                if (_traceUnknowEndOfPageValue)
                    label = "unknow end of page";
            }
            else
            {
                //Trace.WriteLine($"  unknow value \"{line}\" line {_lineNumber} file \"{_filename}\"");
                if (_traceUnknowValue)
                    label = "unknow";
            }
            if (label != null)
                Trace.WriteLine($"  {label} value \"{line}\" line {_lineNumber} file \"{_filename}\"");
        }

        private Question GetQuestion()
        {
            //Question question = new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, QuestionText = _question.QuestionText, Responses = _question.Responses.ToArray() };
            Question question = new Question { Year = _question.Year, Type = _question.Type, Number = _question.Number, SourceFile = _question.SourceFile, SourceLine = _question.SourceLine };
            if (_associationQuestion != null)
            {
                //if (!_newVersion)
                //    question.QuestionText = _associationQuestion.QuestionText + "<br>" + question.QuestionText;
                //else
                //
                //question.QuestionText = _associationQuestion.QuestionText + "<br>" + _question.QuestionText;
                question.QuestionText = _associationQuestion.QuestionText + _newLine + _question.QuestionText;
                question.Choices = _associationQuestion.Choices.ToArray();
            }
            else
            {
                question.QuestionText = _question.QuestionText;
                question.Choices = _question.Choices.ToArray();
            }
            //Trace.WriteLine($"{question.Year} - question {question.Number} - choices count {question.Choices.Length}");
            if (question.Choices.Length == 0)
                throw new PBFileException($"question without choice - {question.Year} - question {question.Number} - line {question.SourceLine} file \"{_filename}\"", _file, question.SourceLine);
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
                    throw new PBFileException($"unknow question, association question is complete, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                question = _associationQuestion;
            }
            else
                throw new PBFileException($"unknow question, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);

            if (question.QuestionLineCount > _maxLinesPerQuestion - 1)
                throw new PBFileException($"to many lines ({question.QuestionLineCount}) for question \"{questionText}\" line {_lineNumber} file \"{zPath.GetFileName(_filename)}\"", _file, _lineNumber);
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
                    throw new PBFileException($"unknow response, association question is complete, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                question = _associationQuestion;
            }
            else if (_question != null)
                question = _question;
            //else
            //    throw new PBFileException($"unknow question, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            else
            {
                // _newVersion2
                if (_lastValueType == QuestionValueType.QuestionType && _questionType == QuestionType.Association && _manageAssociationQuestion)
                {
                    _associationQuestion = new QuestionAssociationTmp { Type = QuestionType.Association, QuestionLineCount = 0, Complete = false };
                    question = _associationQuestion;
                }
                else 
                    throw new PBFileException($"unknow question, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            }

            //if (question.QuestionText == null)
            //    throw new PBFileException($"missing question line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            //if (question.QuestionText == null && (!_newVersion2 || question.Type != QuestionType.Association))
            if (question.QuestionText == null && question.Type != QuestionType.Association)
                throw new PBFileException($"missing question line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            if (response.ResponseCode != (char)('A' + question.Choices.Count))
                throw new PBFileException($"wrong response code \"{response.ResponseCode}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            question.Choices.Add(response.Response);
            question.ResponseLineCount = 1;
        }

        private void AddResponse(string response)
        {
            QuestionBaseTmp question;
            if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow response, association question is complete, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
                question = _associationQuestion;
            }
            else if (_question != null)
                question = _question;
            else
                throw new PBFileException($"unknow question, line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);

            //if (question.ResponseLineCount > 1)
            if (question.ResponseLineCount > _maxLinesPerResponse - 1)
                throw new PBFileException($"to many lines ({question.ResponseLineCount}) for response \"{response}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            int i = question.Choices.Count - 1;
            question.Choices[i] += " " + response;
            question.ResponseLineCount++;
        }

        private QuestionValue GetQuestionValue(NamedValues<ZValue> namedValues)
        {
            if (namedValues.Count == 0)
                throw new PBFileException($"wrong question value line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);

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
                //case "responsecode":
                //case "characterresponsecodes":
                case "characterchoicecode":
                    return new QuestionValueResponse { ResponseCode = ((string)value)[0] };
                //case "numericresponsecodes":
                case "numericchoicecode":
                    return new QuestionValueResponse { ResponseCode = ConvertNumericResponseCode(((string)value)[0]) };
                //case "pagenumber":
                //    return new QuestionValue { Type = QuestionValueType.None };
                default:
                    throw new PBFileException($"unknow value \"{name}\" = {value} line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            }
        }

        private char ConvertNumericResponseCode(char numericCode)
        {
            return (char)(numericCode + 'A' - '1');
        }

        private void SetValue(QuestionValue questionValue, string name, ZValue value)
        {
            switch (name.ToLower())
            {
                //case "response":
                case "choice":
                    ((QuestionValueResponse)questionValue).Response = (string)value;
                    break;
                default:
                    throw new PBFileException($"unknow value \"{name}\" = {value} line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            }
        }

        private QuestionType GetQuestionType(string questionType)
        {
            switch (questionType.ToLower())
            {
                case "simple":
                case "simples":
                case "qcs":
                    return QuestionType.Simple;
                case "multiples":
                case "qcm":
                    return QuestionType.Multiples;
                case "association":
                    return QuestionType.Association;
                case "qta":
                    return QuestionType.QTA;
                default:
                    throw new PBFileException($"unknow question type \"{questionType}\" line {_lineNumber} file \"{_filename}\"", _file, _lineNumber);
            }
        }

        //public static IEnumerable<Question> Read(string file, RegexValuesList regexList, string baseDirectory = null)
        //{
        //    return new QuestionReader { _regexList = regexList }._Read(new string[] { file }, baseDirectory);
        //}

        //public static IEnumerable<Question> Read(IEnumerable<string> files, RegexValuesList regexList, string baseDirectory = null)
        //{
        //    return new QuestionReader { _regexList = regexList }._Read(files, baseDirectory);
        //}
    }
}

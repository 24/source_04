using pb;
using pb.Data.Text;
using pb.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace anki
{
    public class QuestionReader_v2
    {
        private bool _trace = false;
        private RegexValuesList _questionRegexValuesList = null;
        private RegexValuesList _responseRegexValuesList = null;
        //private TextDataReader _textDataReader = null;
        private string _baseDirectory = null;

        // question parameters
        private int _minPageForResponses = 2;
        private int _maxLinesPerQuestion = -1;
        private int _maxLinesPerChoice = -1;
        private bool _traceUnknowHeaderValue = false;
        private bool _traceUnknowValue = true;
        private static string _newLine = "\r\n";

        // response parameters
        private int _responseMaxEmptyLine = 5;

        // variables
        private QuestionDataType? _lastValueType = null;
        private bool _dontSetLastValueType = false;
        private int _emptyLineCount = 0;

        // question variables
        private int? _questionYear = null;
        private QuestionType _questionType = QuestionType.None;
        private QuestionReader.QuestionTmp _question = null;
        private QuestionReader.QuestionTmp _questionTmp = null;
        private QuestionReader.QuestionAssociationTmp _associationQuestion = null;
        private bool _sendCurrentQuestion = false;

        // response variables
        private List<ResponseReader.ResponseYear> _responseYears = null;
        private List<ResponseReader.ResponseQuestion> _responseQuestions = null;
        private string _responseCategory = null;
        private bool _responseQuestionBlock = false;
        private ResponseReader.ResponseQuestion _responseQuestion = null;
        private bool _responseEnd = false;

        // result
        private Question_v2[] _questions;
        private Response_v2[] _responses;

        public RegexValuesList QuestionRegexValuesList { get { return _questionRegexValuesList; } set { _questionRegexValuesList = value; } }
        public RegexValuesList ResponseRegexValuesList { get { return _responseRegexValuesList; } set { _responseRegexValuesList = value; } }
        public int MaxLinesPerQuestion { get { return _maxLinesPerQuestion; } set { _maxLinesPerQuestion = value; } }
        public int MaxLinesPerChoice { get { return _maxLinesPerChoice; } set { _maxLinesPerChoice = value; } }
        public bool TraceUnknowValue { get { return _traceUnknowValue; } set { _traceUnknowValue = value; } }
        public Question_v2[] Questions { get { return _questions; } }
        public Response_v2[] Responses { get { return _responses; } }

        public void ReadAll(IEnumerable<string> files, string baseDirectory = null)
        {
            List<Question_v2> questions = new List<Question_v2>();
            List<Response_v2> responses = new List<Response_v2>();
            foreach (QuestionResponse_v2 questionResponse in Read(files, baseDirectory))
            {
                if (questionResponse is Question_v2)
                    questions.Add((Question_v2)questionResponse);
                if (questionResponse is Response_v2)
                    responses.Add((Response_v2)questionResponse);
            }
            _questions = questions.ToArray();
            _responses = responses.ToArray();
        }

        public IEnumerable<QuestionResponse_v2> Read(IEnumerable<string> files, string baseDirectory = null)
        {
            _lastValueType = null;
            _dontSetLastValueType = false;
            _emptyLineCount = 0;

            _questionYear = null;
            _questionType = QuestionType.None;
            _question = null;
            _questionTmp = null;
            _associationQuestion = null;
            _sendCurrentQuestion = false;

            _responseYears = null;
            _responseQuestions = null;
            _responseCategory = null;
            _responseQuestionBlock = false;
            _responseQuestion = null;
            _responseEnd = false;

            _baseDirectory = baseDirectory;
            TextDataReader textDataReader = new TextDataReader();
            textDataReader.SetRegexList(_questionRegexValuesList);
            bool readResponse = false;
            //int emptyLineCount = 0;
            int pageNumber = 1;

            foreach (QuestionData value in textDataReader.Read(files).Select(textData => QuestionData.CreateQuestionData(textData)))
            {
                if (value.Type != QuestionDataType.EmptyLine)
                    _emptyLineCount = 0;
                else
                    _emptyLineCount++;
                _sendCurrentQuestion = false;
                _dontSetLastValueType = false;
                switch (value.Type)
                {
                    case QuestionDataType.Responses:
                        if (pageNumber >= _minPageForResponses)
                        {
                            if (_trace)
                                Trace.WriteLine($"start read response (line {value.LineNumber} \"{value.Filename}\")");
                            textDataReader.SetRegexList(_responseRegexValuesList);
                            textDataReader.ContiguousSearch = true;
                            _sendCurrentQuestion = true;
                            readResponse = true;
                            _responseYears = new List<ResponseReader.ResponseYear>();
                            _responseQuestions = new List<ResponseReader.ResponseQuestion>();
                            _responseQuestionBlock = false;
                        }
                        break;
                    case QuestionDataType.PageNumber:
                        pageNumber++;
                        _sendCurrentQuestion = true;
                        break;
                    default:
                        //throw new PBFileException($"unknow value type {value.Type} line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
                        if (!readResponse)
                            ReadQuestion(value);
                        else
                        {
                            QuestionResponse_v2 response = ReadResponse(value);
                            if (response != null)
                                yield return response;
                        }
                        break;
                }
                if (_sendCurrentQuestion)
                {
                    if (_question != null)
                        yield return GetQuestion();
                    _question = _questionTmp;
                    _questionTmp = null;
                }
                //if (readResponse)
                //    break;
                if (!_dontSetLastValueType)
                    _lastValueType = value.Type;
                if (_responseEnd)
                    break;
            }
            if (_question != null)
                yield return GetQuestion();
            ControlRemindResponseQuestions();
        }

        private void ReadQuestion(QuestionData value)
        {
            //bool lastValueType = false;
            //bool sendCurrentQuestion = false;
            //QuestionReader.QuestionTmp question = null;
            switch (value.Type)
            {
                case QuestionDataType.EmptyLine:
                    if (_lastValueType == QuestionDataType.QuestionChoice
                        || (_associationQuestion != null && _associationQuestion.Complete && _lastValueType == QuestionDataType.QuestionText))
                    {
                        if (_question != null)
                            _sendCurrentQuestion = true;
                        else if (_associationQuestion != null)
                            _associationQuestion.Complete = true;
                    }
                    else
                        // dont change _lastValueType
                        _dontSetLastValueType = true;
                    break;
                case QuestionDataType.QuestionYear:
                    _sendCurrentQuestion = true;
                    _questionYear = ((QuestionDataQuestionYear)value).Year;
                    _associationQuestion = null;
                    break;
                case QuestionDataType.QuestionType:
                    _sendCurrentQuestion = true;
                    _questionType = ((QuestionDataQuestionType)value).QuestionType;
                    _associationQuestion = null;
                    break;
                case QuestionDataType.QuestionNumber:
                    _sendCurrentQuestion = true;
                    if (_questionYear == null)
                        throw new PBFileException($"unknow year, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
                    //string sourceFile = GetSubPath(value.File);
                    _questionTmp = new QuestionReader.QuestionTmp { Year = (int)_questionYear, Type = _questionType, Number = ((QuestionDataQuestionNumber)value).QuestionNumber,
                        QuestionLineCount = 0, SourceFile = GetSubPath(value.File), SourceLine = value.LineNumber };
                    if (_associationQuestion != null)
                        _associationQuestion.Complete = true;
                    break;
                case QuestionDataType.QuestionChoice:
                    QuestionAddChoice((QuestionDataQuestionChoice)value);
                    break;
                case QuestionDataType.Unknow:
                    switch (_lastValueType)
                    {
                        case QuestionDataType.QuestionNumber:
                            _question.QuestionText = value.TrimedLine;
                            _question.QuestionLineCount++;
                            _lastValueType = QuestionDataType.QuestionText;
                            _dontSetLastValueType = true;
                            break;
                        case QuestionDataType.QuestionText:
                            QuestionAddLine(value);
                            // keep _lastValueType to QuestionDataType.QuestionText
                            _dontSetLastValueType = true;
                            break;
                        case QuestionDataType.QuestionChoice:
                            QuestionAddChoiceLine(value);
                            // keep _lastValueType to QuestionDataType.QuestionChoice
                            _dontSetLastValueType = true;
                            break;
                        default:
                            // _manageAssociationQuestion
                            if (_lastValueType == QuestionDataType.QuestionType && _questionType == QuestionType.Association)
                            {
                                _associationQuestion = new QuestionReader.QuestionAssociationTmp { Type = QuestionType.Association, QuestionText = value.TrimedLine, QuestionLineCount = 1, Complete = false };
                                _lastValueType = QuestionDataType.QuestionText;
                                _dontSetLastValueType = true;
                            }
                            else
                                _TraceUnknowValue(value);
                            break;
                    }
                    break;
            }
        }

        private QuestionResponse_v2 ReadResponse(QuestionData value)
        {
            Response_v2 response = null;
            if (_responseQuestion != null && value.Type != QuestionDataType.Response)
            {
                if (_trace)
                    //Trace.WriteLine($"save response question (line {value.LineNumber} \"{value.Filename}\")");
                    Trace.WriteLine($"save response question : {_responseQuestion.Year} - {_responseQuestion.QuestionNumber}");
                SaveResponseQuestion(_responseQuestion);
                _responseQuestion = null;
            }
            switch (value.Type)
            {
                case QuestionDataType.EmptyLine:
                    if (_emptyLineCount == _responseMaxEmptyLine && _responseYears.Count > 0)
                        _responseEnd = true;
                    break;
                case QuestionDataType.ResponseCategory:
                    _responseCategory = ((QuestionDataResponseCategory)value).Category;
                    if (_trace)
                        Trace.WriteLine($"response category : {_responseCategory} (line {value.LineNumber} \"{value.Filename}\")");
                    ControlRemindResponseQuestions();
                    _responseYears = new List<ResponseReader.ResponseYear>();
                    _responseQuestionBlock = false;
                    break;
                case QuestionDataType.ResponseYear:
                    QuestionDataResponseYear responseYear = (QuestionDataResponseYear)value;
                    if (_responseQuestionBlock)
                        throw new PBException($"wrong response year position, line {responseYear.LineNumber} column {responseYear.ColumnIndex + 1} file \"{responseYear.Filename}\"");
                    if (_trace)
                        Trace.WriteLine($"response year : {responseYear.Year} (line {value.LineNumber} \"{value.Filename}\")");
                    ResponseAddYear(responseYear);
                    break;
                case QuestionDataType.QuestionNumber:
                    //responseQuestion = NewQuestion(namedValue.Value.Value, namedValue.Value.Index, namedValue.Value.Length, responseQuestion);
                    QuestionDataQuestionNumber questionNumber = (QuestionDataQuestionNumber)value;
                    if (_trace)
                        Trace.WriteLine($"response question : {questionNumber.QuestionNumber} (line {value.LineNumber} \"{value.Filename}\")");
                    _responseQuestion = ResponseCreateResponseQuestion(questionNumber);
                    _responseQuestionBlock = true;
                    break;
                case QuestionDataType.Response:
                    QuestionDataResponse responseValue = (QuestionDataResponse)value;
                    if (_responseQuestion != null && _responseQuestion.SourceLineNumber != responseValue.LineNumber)
                    {
                        SaveResponseQuestion(_responseQuestion);
                        _responseQuestion = null;
                    }
                    response = SetResponse(responseValue, _responseQuestion);
                    if (_trace)
                        Trace.WriteLine($"response : {response.Responses} question {response.Year} - {response.QuestionNumber} (line {value.LineNumber} \"{value.Filename}\")");
                    _responseQuestion = null;
                    break;
            }
            ControlResponseQuestions();
            return response;
        }

        private Question_v2 GetQuestion()
        {
            Question_v2 question = new Question_v2 { Year = _question.Year, Type = _question.Type, QuestionNumber = _question.Number, SourceFile = _question.SourceFile, SourceLine = _question.SourceLine };
            if (_associationQuestion != null)
            {
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
                throw new PBFileException($"question without choice - {question.Year} - question {question.QuestionNumber} - line {question.SourceLine} file \"{question.SourceFile}\"", question.SourceFile, question.SourceLine);
            _question = null;
            return question;
        }

        private void QuestionAddLine(QuestionData value)
        {
            QuestionReader.QuestionBaseTmp question;
            if (_question != null)
                question = _question;
            else if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow question, association question is complete, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
                question = _associationQuestion;
            }
            else
                throw new PBFileException($"unknow question, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);

            if (question.QuestionLineCount > _maxLinesPerQuestion - 1)
                throw new PBFileException($"to many lines ({question.QuestionLineCount}) for question \"{value.TrimedLine}\" line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
            question.QuestionText += " " + value.TrimedLine;
            question.QuestionLineCount++;
        }

        private void QuestionAddChoice(QuestionDataQuestionChoice value)
        {
            QuestionReader.QuestionBaseTmp question;
            if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow response, association question is complete, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
                question = _associationQuestion;
            }
            else if (_question != null)
                question = _question;
            else
            {
                if (_lastValueType == QuestionDataType.QuestionType && _questionType == QuestionType.Association)
                {
                    _associationQuestion = new QuestionReader.QuestionAssociationTmp { Type = QuestionType.Association, QuestionLineCount = 0, Complete = false };
                    question = _associationQuestion;
                }
                else
                    throw new PBFileException($"unknow question, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
            }

            if (question.QuestionText == null && question.Type != QuestionType.Association)
                throw new PBFileException($"missing question line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
            if (value.ChoiceNumber != question.Choices.Count + 1)
                throw new PBFileException($"wrong response code \"{(char)('A' + value.ChoiceNumber - 1)}\" line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
            question.Choices.Add(value.ChoiceText);
            question.ChoiceLineCount = 1;
        }

        private void QuestionAddChoiceLine(QuestionData value)
        {
            QuestionReader.QuestionBaseTmp question;
            if (_associationQuestion != null)
            {
                if (_associationQuestion.Complete)
                    throw new PBFileException($"unknow response, association question is complete, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
                question = _associationQuestion;
            }
            else if (_question != null)
                question = _question;
            else
                throw new PBFileException($"unknow question, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);

            if (question.ChoiceLineCount > _maxLinesPerChoice - 1)
                throw new PBFileException($"to many lines ({question.ChoiceLineCount}) for response \"{value.TrimedLine}\" line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
            question.Choices[question.Choices.Count - 1] += " " + value.TrimedLine;
            question.ChoiceLineCount++;
        }

        private void ResponseAddYear(QuestionDataResponseYear value)
        {
            _responseYears.Add(new ResponseReader.ResponseYear(value.Year, value.ColumnIndex, value.Length));
        }

        private ResponseReader.ResponseQuestion ResponseCreateResponseQuestion(QuestionDataQuestionNumber value)
        {
            //if (_responseQuestion != null)
            //{
            //    SaveResponseQuestion(_responseQuestion);
            //}
            ResponseReader.ResponseQuestion responseQuestion = new ResponseReader.ResponseQuestion(value.QuestionNumber, value.ColumnIndex, value.Length) { SourceFile = GetSubPath(value.File), SourceFilename = value.Filename, SourceLineNumber = value.LineNumber };
            responseQuestion.Year = GetYear(responseQuestion.Position);
            if (responseQuestion.Year == 0)
                throw new PBFileException($"year not found, line {value.LineNumber} column {value.ColumnIndex + 1} file \"{value.Filename}\"", value.File, value.LineNumber);
            return responseQuestion;
        }

        //private Response SetResponse(ZValue value, int index, int length, ResponseQuestion responseQuestion)
        private Response_v2 SetResponse(QuestionDataResponse value, ResponseReader.ResponseQuestion responseQuestion)
        {
            //ResponseReader.ResponseQuestion responseQuestion;
            //if (_responseQuestion != null)
            //    responseQuestion = _responseQuestion;
            //else
            if (responseQuestion == null)
                //responseQuestion = GetResponseQuestion(GetMiddlePosition(value.ColumnIndex, value.Length));
                responseQuestion = GetResponseQuestion(value);
            //if (_trace)
            //    Trace.WriteLine($"set response \"{value}\" question {responseQuestion.QuestionNumber} index {index} length {length}");
            responseQuestion.FoundResponse = true;
            // $$pb remove responseQuestion from list
            return new Response_v2 { Category = _responseCategory, Year = responseQuestion.Year, QuestionNumber = responseQuestion.QuestionNumber, Responses = value.Responses,
                SourceFile = responseQuestion.SourceFile, SourceLine = responseQuestion.SourceLineNumber };
        }

        //private ResponseReader.ResponseQuestion GetResponseQuestion(int position)
        private ResponseReader.ResponseQuestion GetResponseQuestion(QuestionDataResponse value)
        {
            int position = GetMiddlePosition(value.ColumnIndex, value.Length);
            foreach (ResponseReader.ResponseQuestion responseQuestion in _responseQuestions)
            {
                if (!responseQuestion.FoundResponse && Math.Abs(position - responseQuestion.Position) <= 2)
                    return responseQuestion;
            }
            Trace.WriteLine($"search question at position {position + 1}, line {value.LineNumber}");
            Trace.WriteLine($"  question count {_responseQuestions.Count}");
            foreach (ResponseReader.ResponseQuestion responseQuestion in _responseQuestions)
            {
                Trace.WriteLine($"  question no {responseQuestion.QuestionNumber} at position {responseQuestion.Position + 1} distance {Math.Abs(position - responseQuestion.Position)}");
            }
            throw new PBFileException($"question not found, line {value.LineNumber} column {value.ColumnIndex + 1} file \"{value.Filename}\"", value.File, value.LineNumber);
        }

        public static int GetMiddlePosition(int startPosition, int length)
        {
            // 1 : 0   3 : 1
            return startPosition + (length - 1) / 2;
        }

        private void SaveResponseQuestion(ResponseReader.ResponseQuestion responseQuestion)
        {
            //if (_trace)
            //    Trace.WriteLine($"save question \"{responseQuestion.QuestionNumber}\"");
            foreach (ResponseReader.ResponseQuestion responseQuestion2 in _responseQuestions)
            {
                if (Math.Abs(responseQuestion.Position - responseQuestion2.Position) <= 2)
                    throw new PBFileException($"missing response for \"{responseQuestion.Year} - question no {responseQuestion.QuestionNumber}\", line {responseQuestion.SourceLineNumber} file \"{responseQuestion.SourceFilename}\"", responseQuestion.SourceFile, responseQuestion.SourceLineNumber);
            }
            _responseQuestions.Add(responseQuestion);
        }

        private int GetYear(int position)
        {
            foreach (ResponseReader.ResponseYear year in _responseYears)
            {
                if (Math.Abs(position - year.Position) <= 2)
                    return year.Year;
            }
            //throw new PBException($"year not found, line {_lineNumber} column {position + 1} file \"{_filename}\"");
            return 0;
        }

        private void ControlResponseQuestions()
        {
            if (_responseQuestions.Count > 0)
            {
                bool questionWithoutResponse = false;
                foreach (ResponseReader.ResponseQuestion responseQuestion in _responseQuestions)
                {
                    if (!responseQuestion.FoundResponse)
                    {
                        questionWithoutResponse = true;
                        break;
                    }
                }
                if (!questionWithoutResponse)
                {
                    if (_trace)
                        Trace.WriteLine("create new response questions");
                    _responseQuestions = new List<ResponseReader.ResponseQuestion>();
                }
            }
        }

        private void ControlRemindResponseQuestions()
        {
            if (_responseQuestion != null)
                SaveResponseQuestion(_responseQuestion);
            foreach (ResponseReader.ResponseQuestion responseQuestion in _responseQuestions)
            {
                if (!responseQuestion.FoundResponse)
                {
                    //Trace.WriteLine($"no response for {responseQuestion.Year} - question {responseQuestion.QuestionNumber}");
                    throw new PBFileException($"no response for {responseQuestion.Year} - question {responseQuestion.QuestionNumber} - line {responseQuestion.SourceLineNumber} file \"{responseQuestion.SourceFilename}\"", responseQuestion.SourceFile, responseQuestion.SourceLineNumber);
                }
            }
        }

        private void _TraceUnknowValue(QuestionData value)
        {
            string label = null;
            if (_questionYear == null)
            {
                //Trace.WriteLine($"  unknow header value \"{line}\" line {_lineNumber} file \"{_filename}\"");
                if (_traceUnknowHeaderValue)
                    label = "unknow header";
            }
            else if (_traceUnknowValue)
                label = "unknow";
            if (label != null)
                Trace.WriteLine($"  {label} value \"{value.TrimedLine}\" line {value.LineNumber} file \"{value.Filename}\"");
        }

        private string GetSubPath(string file)
        {
            if (_baseDirectory != null && file.StartsWith(_baseDirectory))
            {
                int l = _baseDirectory.Length;
                if (file[l] == '\\')
                    l++;
                file = file.Substring(l);
            }
            return file;
        }

        //public IEnumerable<QuestionResponse_v2> Read(IEnumerable<string> files, string baseDirectory = null)
        //{
        //    _lastValueType = null;
        //    _questionYear = null;
        //    _questionType = QuestionType.None;
        //    _question = null;
        //    _associationQuestion = null;

        //    _baseDirectory = baseDirectory;
        //    TextDataReader textDataReader = new TextDataReader();
        //    textDataReader.SetRegexList(_questionRegexValuesList);
        //    bool readResponse = false;
        //    int emptyLineCount = 0;
        //    int pageNumber = 1;

        //    bool responseQuestionBlock = false;

        //    foreach (QuestionData value in textDataReader.Read(files).Select(textData => QuestionData.CreateQuestionData(textData)))
        //    {
        //        bool sendCurrentQuestion = false;
        //        bool lastValueType = false;
        //        QuestionReader.QuestionTmp question = null;
        //        ResponseReader.ResponseQuestion responseQuestion = null;
        //        if (value.Type != QuestionDataType.EmptyLine)
        //            emptyLineCount = 0;
        //        switch (value.Type)
        //        {
        //            case QuestionDataType.EmptyLine:
        //                emptyLineCount++;
        //                if (_lastValueType == QuestionDataType.QuestionChoice
        //                    || (_associationQuestion != null && _associationQuestion.Complete && _lastValueType == QuestionDataType.QuestionText))
        //                {
        //                    if (_question != null)
        //                        sendCurrentQuestion = true;
        //                    else if (_associationQuestion != null)
        //                        _associationQuestion.Complete = true;
        //                }
        //                else
        //                    // dont change _lastValueType
        //                    lastValueType = true;
        //                break;
        //            case QuestionDataType.QuestionYear:
        //                sendCurrentQuestion = true;
        //                _questionYear = ((QuestionDataQuestionYear)value).Year;
        //                _associationQuestion = null;
        //                break;
        //            case QuestionDataType.QuestionType:
        //                sendCurrentQuestion = true;
        //                _questionType = ((QuestionDataQuestionType)value).QuestionType;
        //                _associationQuestion = null;
        //                break;
        //            case QuestionDataType.QuestionNumber:
        //                sendCurrentQuestion = true;
        //                if (_questionYear == null)
        //                    throw new PBFileException($"unknow year, line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
        //                if (!readResponse)
        //                {
        //                    string sourceFile = GetSubPath(value.File);
        //                    question = new QuestionReader.QuestionTmp { Year = (int)_questionYear, Type = _questionType, Number = ((QuestionDataQuestionNumber)value).QuestionNumber, QuestionLineCount = 0, SourceFile = sourceFile, SourceLine = value.LineNumber };
        //                    if (_associationQuestion != null)
        //                        _associationQuestion.Complete = true;
        //                }
        //                else
        //                {
        //                    //responseQuestion = NewQuestion(namedValue.Value.Value, namedValue.Value.Index, namedValue.Value.Length, responseQuestion);
        //                    responseQuestion = CreateResponseQuestion((QuestionDataQuestionNumber)value);
        //                    responseQuestionBlock = true;
        //                }
        //                break;
        //            case QuestionDataType.QuestionChoice:
        //                AddChoice((QuestionDataQuestionChoice)value);
        //                break;
        //            case QuestionDataType.Unknow:
        //                switch (_lastValueType)
        //                {
        //                    case QuestionDataType.QuestionNumber:
        //                        _question.QuestionText = value.TrimedLine;
        //                        _question.QuestionLineCount++;
        //                        _lastValueType = QuestionDataType.QuestionText;
        //                        lastValueType = true;
        //                        break;
        //                    case QuestionDataType.QuestionText:
        //                        AddQuestionLine(value);
        //                        // keep _lastValueType to QuestionDataType.QuestionText
        //                        lastValueType = true;
        //                        break;
        //                    case QuestionDataType.QuestionChoice:
        //                        AddChoiceLine(value);
        //                        // keep _lastValueType to QuestionDataType.QuestionChoice
        //                        lastValueType = true;
        //                        break;
        //                    default:
        //                        // _manageAssociationQuestion
        //                        if (_lastValueType == QuestionDataType.QuestionType && _questionType == QuestionType.Association)
        //                        {
        //                            _associationQuestion = new QuestionReader.QuestionAssociationTmp { Type = QuestionType.Association, QuestionText = value.TrimedLine, QuestionLineCount = 1, Complete = false };
        //                            _lastValueType = QuestionDataType.QuestionText;
        //                            lastValueType = true;
        //                        }
        //                        else
        //                            _TraceUnknowValue(value);
        //                        break;
        //                }
        //                break;
        //            case QuestionDataType.Responses:
        //                if (pageNumber >= _minPageForResponses)
        //                {
        //                    textDataReader.SetRegexList(_responseRegexValuesList);
        //                    textDataReader.ContiguousSearch = true;
        //                    sendCurrentQuestion = true;
        //                    readResponse = true;
        //                    _responseYears = new List<ResponseReader.ResponseYear>();
        //                    _responseQuestions = new List<ResponseReader.ResponseQuestion>();
        //                    responseQuestionBlock = false;
        //                }
        //                break;
        //            case QuestionDataType.ResponseCategory:
        //                _responseCategory = ((QuestionDataResponseCategory)value).Category;
        //                _responseYears = new List<ResponseReader.ResponseYear>();
        //                responseQuestionBlock = false;
        //                break;
        //            case QuestionDataType.ResponseYear:
        //                QuestionDataResponseYear responseYear = (QuestionDataResponseYear)value;
        //                if (responseQuestionBlock)
        //                    throw new PBException($"wrong response year position, line {responseYear.LineNumber} column {responseYear.ColumnIndex + 1} file \"{responseYear.Filename}\"");
        //                AddYear(responseYear);
        //                break;
        //            case QuestionDataType.Response:
        //                yield return SetResponse((QuestionDataResponse)value);
        //                responseQuestion = null;
        //                break;
        //            case QuestionDataType.PageNumber:
        //                pageNumber++;
        //                sendCurrentQuestion = true;
        //                break;
        //            default:
        //                throw new PBFileException($"unknow value type {value.Type} line {value.LineNumber} file \"{value.Filename}\"", value.File, value.LineNumber);
        //        }
        //        if (sendCurrentQuestion)
        //        {
        //            if (_question != null)
        //                yield return GetQuestion();
        //            _question = question;
        //        }
        //        if (readResponse)
        //            break;
        //        if (!lastValueType)
        //            _lastValueType = value.Type;
        //    }
        //    if (_question != null)
        //        yield return GetQuestion();
        //}
    }
}

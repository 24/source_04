using pb;
using pb.Data;
using pb.IO;
using pb.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace anki
{
    public class Response
    {
        public string Category;
        public int Year;
        public int QuestionNumber;
        public string Responses;

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
            // Response.Responses.ToCharArray()
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

    // _manageCategory, _category sont utilisés pour lire les réponses en plusieurs parties, ex UE8 - Communication cellulaire
    public class ResponseReader
    {
        private bool _trace = false;
        private bool _manageCategory = true;
        private bool _acceptUnknowText = true;
        private int _maxEmptyLine = 5;

        private string _file = null;
        private string _filename = null;
        private RegexValuesList _regexList = null;

        private int _lineNumber = 0;
        private List<ResponseYear> _years = null;
        private List<ResponseQuestion> _responseQuestions = null;
        private string _category = null;

        private class ResponseYear
        {
            public int Year;
            public int StartPosition;
            public int Length;
            public int Position;

            public ResponseYear(int year, int startPosition, int length)
            {
                Year = year;
                StartPosition = startPosition;
                Length = length;
                Position = GetMiddlePosition(startPosition, length);
            }
        }

        private class ResponseQuestion
        {
            public int Year;
            public int QuestionNumber;
            public bool FoundResponse;
            public int StartPosition;
            public int Length;
            public int Position;

            public ResponseQuestion(int questionNumber, int startPosition, int length)
            {
                QuestionNumber = questionNumber;
                FoundResponse = false;
                StartPosition = startPosition;
                Length = length;
                Position = GetMiddlePosition(startPosition, length);
            }
        }

        private IEnumerable<Response> _Read()
        {
            _filename = zPath.GetFileName(_file);
            _lineNumber = 0;
            _years = new List<ResponseYear>();
            _responseQuestions = new List<ResponseQuestion>();
            bool questionBlock = false;
            int emptyLineCount = 0;
            Trace.WriteLine($"read response file \"{_filename}\"");
            foreach (string line in zFile.ReadLines(_file))
            {
                _lineNumber++;
                //string line2 = line.Trim();
                if (line == "")
                {
                    //if (++emptyLineCount == _maxEmptyLine)
                    if (++emptyLineCount == _maxEmptyLine && _years.Count > 0)
                        break;
                    //int _maxEmptyLine
                    //if (_responseQuestions.Count > 0)
                    //    throw new PBException($"missing response for {GetQuestionWithoutResponse().zToStringValues(", ")}, line {_lineNumber}");
                    continue;
                }

                emptyLineCount = 0;
                bool verifyRemainText = true;
                FindText_v2 findText = _regexList.Find(line, contiguous: true);
                if (findText.Success)
                {
                    //MatchValues matchValues = findText.matchValues;
                    //while (matchValues.Match.Success)
                    ResponseQuestion responseQuestion = null;
                    while (findText.Success)
                    {
                        //ResponseQuestion responseQuestion = null;
                        //foreach (KeyValuePair<string, ZValue> namedValue in findText.GetValues())
                        foreach (KeyValuePair<string, RegexValue<ZValue>> namedValue in findText.GetRegexValues())
                        {
                            switch (namedValue.Key.ToLower())
                            {
                                case "category":
                                    if (!_manageCategory)
                                        throw new PBException($"");
                                    if (_trace)
                                        Trace.WriteLine($"new category \"{namedValue.Value.Value}\"");
                                    _category = (string)namedValue.Value.Value;
                                    _years = new List<ResponseYear>();
                                    questionBlock = false;
                                    break;
                                case "year":
                                    //if (_responseQuestions.Count != 0)
                                    if (questionBlock)
                                        throw new PBException($"wrong year position, line {_lineNumber} column {findText.MatchIndex + 1} file \"{_filename}\"");
                                    //AddYear(namedValue.Value, findText.Match.Index, findText.Match.Length);
                                    AddYear(namedValue.Value.Value, namedValue.Value.Index, namedValue.Value.Length);
                                    //_years.Add(new ResponseYear(int.Parse((string)namedValue.Value), matchValues.Match.Index, matchValues.Match.Length));
                                    break;
                                case "questionnumber":
                                    //if (responseQuestion != null)
                                    //    //_responseQuestions.Add(responseQuestion);
                                    //    SaveQuestion(responseQuestion);
                                    //responseQuestion = new ResponseQuestion(int.Parse((string)namedValue.Value), matchValues.Match.Index, matchValues.Match.Length);
                                    //responseQuestion.Year = GetYear(responseQuestion.Position);
                                    //responseQuestion = NewQuestion(namedValue.Value, findText.Match.Index, findText.Match.Length, responseQuestion);
                                    responseQuestion = NewQuestion(namedValue.Value.Value, namedValue.Value.Index, namedValue.Value.Length, responseQuestion);
                                    questionBlock = true;
                                    break;
                                case "characterresponsecodes":
                                    //if (responseQuestion == null)
                                    //    throw new PBException($"unknow question, line {_lineNumber} column {matchValues.Match.Index + 1}");
                                    //if (namedValue.Value != null)
                                    //{
                                    //if (responseQuestion == null)
                                    //    responseQuestion = GetResponseQuestion(GetMiddlePosition(matchValues.Match.Index, matchValues.Match.Length));
                                    ////GetYear(GetMiddlePosition(matchValues.Match.Index, matchValues.Match.Length))
                                    ////Responses = ((string)namedValue.Value).ToCharArray()
                                    //yield return new Response { Year = responseQuestion.Year, QuestionNumber = responseQuestion.QuestionNumber, Responses = (string)namedValue.Value };
                                    //responseQuestion.FoundResponse = true;
                                    //yield return SetResponse(namedValue.Value, findText.Match.Index, findText.Match.Length, responseQuestion);
                                    yield return SetResponse(namedValue.Value.Value, namedValue.Value.Index, namedValue.Value.Length, responseQuestion);
                                    responseQuestion = null;
                                    //}
                                    break;
                                case "numericresponsecodes":
                                    yield return SetResponse(ConvertNumericResponseCodes(namedValue.Value.Value), namedValue.Value.Index, namedValue.Value.Length, responseQuestion);
                                    responseQuestion = null;
                                    break;
                            }
                        }
                        //if (responseQuestion != null)
                        //    //_responseQuestions.Add(responseQuestion);
                        //    SaveQuestion(responseQuestion);
                        //matchValues = matchValues.Next();
                        //findText.Next();
                        findText.FindNext(contiguous: true);
                    }

                    // control text not found
                    //if (findText.MatchIndex + findText.MatchLength < line.Length)
                    //{
                    //    string textNotFound = line.Substring(findText.MatchIndex + findText.MatchLength).Trim();
                    //    if (textNotFound.Length > 0)
                    //        Trace.WriteLine($"warning remain text \"{textNotFound}\" line {_lineNumber} column {findText.MatchIndex + findText .MatchLength + 1}");
                    //}

                    if (responseQuestion != null)
                        //_responseQuestions.Add(responseQuestion);
                        SaveQuestion(responseQuestion);


                    bool questionWithoutResponse = false;
                    foreach (ResponseQuestion responseQuestion2 in _responseQuestions)
                    {
                        if (!responseQuestion2.FoundResponse)
                        {
                            questionWithoutResponse = true;
                            break;
                        }
                    }
                    if (!questionWithoutResponse)
                        _responseQuestions = new List<ResponseQuestion>();
                }
                else if (_acceptUnknowText)
                {
                    Trace.WriteLine($"  unknow text \"{line}\" line {_lineNumber} file \"{_filename}\"");
                    verifyRemainText = false;
                }

                // control text not found
                if (verifyRemainText && findText.MatchIndex + findText.MatchLength < line.Length)
                {
                    string textNotFound = line.Substring(findText.MatchIndex + findText.MatchLength).Trim();
                    if (textNotFound.Length > 0)
                    {
                        //Trace.WriteLine($"unknow text \"{textNotFound}\" line {_lineNumber} column {findText.MatchIndex + findText.MatchLength + 1} file \"{_filename}\"");
                        throw new PBException($"  unknow text \"{textNotFound}\" line {_lineNumber} column {findText.MatchIndex + findText.MatchLength + 1} file \"{_filename}\"");
                    }
                }
            }
            foreach (ResponseQuestion responseQuestion2 in _responseQuestions)
            {
                if (!responseQuestion2.FoundResponse)
                {
                    Trace.WriteLine($"no response for {responseQuestion2.Year} - question {responseQuestion2.QuestionNumber}");
                }
            }
        }

        private void AddYear(ZValue value, int index, int length)
        {
            if (_trace)
                Trace.WriteLine($"add year \"{value}\" index {index} length {length}");
            _years.Add(new ResponseYear(int.Parse((string)value), index, length));
        }

        private ResponseQuestion NewQuestion(ZValue value, int index, int length, ResponseQuestion responseQuestion)
        {
            if (_trace)
                Trace.WriteLine($"new question \"{value}\" index {index} length {length}");
            if (responseQuestion != null)
            {
                SaveQuestion(responseQuestion);
            }
            responseQuestion = new ResponseQuestion(int.Parse((string)value), index, length);
            responseQuestion.Year = GetYear(responseQuestion.Position);
            return responseQuestion;
        }

        private void SaveQuestion(ResponseQuestion responseQuestion)
        {
            if (_trace)
                Trace.WriteLine($"save question \"{responseQuestion.QuestionNumber}\"");
            foreach (ResponseQuestion responseQuestion2 in _responseQuestions)
            {
                if (Math.Abs(responseQuestion.Position - responseQuestion2.Position) <= 2)
                    throw new PBException($"missing response for \"{responseQuestion2.Year} - question no {responseQuestion2.QuestionNumber}\", line {_lineNumber} file \"{_filename}\"");
            }
            _responseQuestions.Add(responseQuestion);
        }

        private ZString ConvertNumericResponseCodes(ZValue value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in (string)value)
                sb.Append((char)(c + 'A' - '1'));
            return (ZString)sb.ToString();
        }

        private Response SetResponse(ZValue value, int index, int length, ResponseQuestion responseQuestion)
        {
            if (responseQuestion == null)
                responseQuestion = GetResponseQuestion(GetMiddlePosition(index, length));
            if (_trace)
                Trace.WriteLine($"set response \"{value}\" question {responseQuestion.QuestionNumber} index {index} length {length}");
            responseQuestion.FoundResponse = true;
            return new Response { Category = _category, Year = responseQuestion.Year, QuestionNumber = responseQuestion.QuestionNumber, Responses = (string)value };
        }

    private IEnumerable<string> GetQuestionWithoutResponse()
        {
            foreach (ResponseQuestion responseQuestion in _responseQuestions)
            {
                if (!responseQuestion.FoundResponse)
                    yield return $"{responseQuestion.Year} - question no {responseQuestion.QuestionNumber}";
            }
        }

        private int GetYear(int position)
        {
            //Trace.WriteLine($"search year for position {position} year count {_years.Count}");
            foreach (ResponseYear year in _years)
            {
                //Trace.WriteLine($"  year {year.Year} position {year.Position}");
                if (Math.Abs(position - year.Position) <= 2)
                    return year.Year;
            }
            throw new PBException($"year not found, line {_lineNumber} column {position + 1} file \"{_filename}\"");
        }

        private ResponseQuestion GetResponseQuestion(int position)
        {
            foreach (ResponseQuestion responseQuestion in _responseQuestions)
            {
                if (!responseQuestion.FoundResponse && Math.Abs(position - responseQuestion.Position) <= 2)
                    return responseQuestion;
            }
            Trace.WriteLine($"search question at position {position + 1}, line {_lineNumber}");
            Trace.WriteLine($"  question count {_responseQuestions.Count}");
            foreach (ResponseQuestion responseQuestion in _responseQuestions)
            {
                Trace.WriteLine($"  question no {responseQuestion.QuestionNumber} at position {responseQuestion.Position + 1} distance {Math.Abs(position - responseQuestion.Position)}");
            }
            throw new PBException($"question not found, line {_lineNumber} column {position + 1} file \"{_filename}\"");
        }

        public static int GetMiddlePosition(int startPosition, int length)
        {
            // 1 : 0   3 : 1
            return startPosition + (length - 1) / 2;
        }

        public static IEnumerable<Response> Read(string file, RegexValuesList regexList)
        {
            return new ResponseReader { _file = file, _regexList = regexList }._Read();
        }
    }
}

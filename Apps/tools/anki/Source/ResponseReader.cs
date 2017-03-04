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
        public int Year;
        public int QuestionNumber;
        public string Responses;

        public string GetFormatedResponse()
        {
            return GetFormatedResponse(Responses);
        }

        public static string GetFormatedResponse(string responses)
        {
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

    public class ResponseReader
    {
        private bool _trace = false;
        private string _file = null;
        private RegexValuesList _regexList = null;

        private int _lineNumber = 0;
        private List<ResponseYear> _years = null;
        private List<ResponseQuestion> _responseQuestions = null;

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
            _lineNumber = 0;
            _years = new List<ResponseYear>();
            _responseQuestions = new List<ResponseQuestion>();
            //Trace.WriteLine($"read response file \"{_file}\"");
            foreach (string line in zFile.ReadLines(_file))
            {
                _lineNumber++;
                //string line2 = line.Trim();
                if (line == "")
                {
                    //if (_responseQuestions.Count > 0)
                    //    throw new PBException($"missing response for {GetQuestionWithoutResponse().zToStringValues(", ")}, line {_lineNumber}");
                    continue;
                }

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
                                case "year":
                                    if (_responseQuestions.Count != 0)
                                        throw new PBException($"wrong year position, line {_lineNumber} column {findText.MatchIndex + 1}");
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
                                    break;
                                case "responsecodes":
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

                // control text not found
                if (findText.MatchIndex + findText.MatchLength < line.Length)
                {
                    string textNotFound = line.Substring(findText.MatchIndex + findText.MatchLength).Trim();
                    if (textNotFound.Length > 0)
                        //Trace.WriteLine($"warning remain text \"{textNotFound}\" line {_lineNumber} column {findText.MatchIndex + findText.MatchLength + 1}");
                        throw new PBException($"unknow text \"{textNotFound}\" line {_lineNumber} column {findText.MatchIndex + findText.MatchLength + 1}");
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
                    throw new PBException($"missing response for \"{responseQuestion2.Year} - question no {responseQuestion2.QuestionNumber}\", line {_lineNumber}");
            }
            _responseQuestions.Add(responseQuestion);
        }

        private Response SetResponse(ZValue value, int index, int length, ResponseQuestion responseQuestion)
        {
            if (responseQuestion == null)
                responseQuestion = GetResponseQuestion(GetMiddlePosition(index, length));
            if (_trace)
                Trace.WriteLine($"set response \"{value}\" question {responseQuestion.QuestionNumber} index {index} length {length}");
            responseQuestion.FoundResponse = true;
            return new Response { Year = responseQuestion.Year, QuestionNumber = responseQuestion.QuestionNumber, Responses = (string)value };
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
            foreach (ResponseYear year in _years)
            {
                if (Math.Abs(position - year.Position) <= 2)
                    return year.Year;
            }
            throw new PBException($"year not found, line {_lineNumber} column {position + 1}");
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
            throw new PBException($"question not found, line {_lineNumber} column {position + 1}");
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

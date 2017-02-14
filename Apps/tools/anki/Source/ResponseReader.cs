using pb;
using pb.Data;
using pb.IO;
using pb.Text;
using System;
using System.Collections.Generic;

namespace anki
{
    public class Response
    {
        public int Year;
        public int QuestionNumber;
        //public char[] Responses;
        public string Responses;
    }

    public class ResponseReader
    {
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

                FindText findText = _regexList.Find(line);
                if (findText.Found)
                {
                    MatchValues matchValues = findText.matchValues;
                    while (matchValues.Match.Success)
                    {
                        ResponseQuestion responseQuestion = null;
                        foreach (KeyValuePair<string, ZValue> namedValue in matchValues.GetValues())
                        {
                            switch (namedValue.Key.ToLower())
                            {
                                case "year":
                                    if (_responseQuestions.Count != 0)
                                        throw new PBException($"wrong year position, line {_lineNumber} column {matchValues.Match.Index + 1}");
                                    _years.Add(new ResponseYear(int.Parse((string)namedValue.Value), matchValues.Match.Index, matchValues.Match.Length));
                                    break;
                                case "questionnumber":
                                    if (responseQuestion != null)
                                        //_responseQuestions.Add(responseQuestion);
                                        SaveQuestion(responseQuestion);
                                    responseQuestion = new ResponseQuestion(int.Parse((string)namedValue.Value), matchValues.Match.Index, matchValues.Match.Length);
                                    responseQuestion.Year = GetYear(responseQuestion.Position);
                                    break;
                                case "responsecodes":
                                    //if (responseQuestion == null)
                                    //    throw new PBException($"unknow question, line {_lineNumber} column {matchValues.Match.Index + 1}");
                                    if (namedValue.Value != null)
                                    {
                                        if (responseQuestion == null)
                                            responseQuestion = GetResponseQuestion(GetMiddlePosition(matchValues.Match.Index, matchValues.Match.Length));
                                        //GetYear(GetMiddlePosition(matchValues.Match.Index, matchValues.Match.Length))
                                        //Responses = ((string)namedValue.Value).ToCharArray()
                                        yield return new Response { Year = responseQuestion.Year, QuestionNumber = responseQuestion.QuestionNumber, Responses = (string)namedValue.Value };
                                        responseQuestion.FoundResponse = true;
                                        responseQuestion = null;
                                    }
                                    break;
                            }
                        }
                        if (responseQuestion != null)
                            _responseQuestions.Add(responseQuestion);
                        matchValues = matchValues.Next();
                    }

                    bool questionWithoutResponse = false;
                    foreach (ResponseQuestion responseQuestion in _responseQuestions)
                    {
                        if (!responseQuestion.FoundResponse)
                        {
                            questionWithoutResponse = true;
                            break;
                        }
                    }
                    if (!questionWithoutResponse)
                        _responseQuestions = new List<ResponseQuestion>();
                }
            }
        }

        private void SaveQuestion(ResponseQuestion responseQuestion)
        {
            foreach (ResponseQuestion responseQuestion2 in _responseQuestions)
            {
                if (Math.Abs(responseQuestion.Position - responseQuestion2.Position) <= 2)
                    throw new PBException($"missing response for \"{responseQuestion2.Year} - question no {responseQuestion2.QuestionNumber}\", line {_lineNumber}");
            }
            _responseQuestions.Add(responseQuestion);
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
            throw new PBException($"question number not found, line {_lineNumber} column {position + 1}");
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

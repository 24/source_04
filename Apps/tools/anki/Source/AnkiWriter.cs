using pb.IO;
using System.Collections.Generic;
using System.IO;

namespace anki
{
    public class AnkiQuestion
    {
        public string Question;
        public string Response;
    }

    public class AnkiWriter
    {
        private string _file = null;
        //private IEnumerable<Question> _questions = null;
        //private IEnumerable<Response> _responses = null;
        //private Dictionary<int, Response> _responses = null;
        //private Question _question = null;

        //
        //private void _Write(IEnumerable<Question> questions)
        //private void _Write(IEnumerable<QuestionResponse> questionResponses)
        private void _Write(IEnumerable<AnkiQuestion> questions)
        {
            zfile.CreateFileDirectory(_file);
            //using (FileStream fs = zFile.Create(_file))
            using (StreamWriter sw = new StreamWriter(zFile.Create(_file)))
            {
                //foreach (Question question in questions)
                //{
                //    _question = question;
                //    Response response = GetResponse(GetQuestionId(question.Year, question.Number));
                //    WriteQuestion(sw, question, response);
                //}
                //foreach (QuestionResponse questionResponse in questionResponses)
                //{
                //    WriteQuestion(sw, questionResponse);
                //}
                foreach (AnkiQuestion question in questions)
                {
                    sw.Write(question.Question);
                    sw.Write("\t");
                    sw.Write(question.Response);
                    sw.Write("\t");
                    sw.WriteLine();
                }
            }
        }

        //private static void WriteQuestion(StreamWriter sw, Question question, Response response)
        //private static void WriteQuestion(StreamWriter sw, QuestionResponse questionResponse)
        //{
        //    // <h1>2015 - QUESTION N 37</h1>
        //    // <br />
        //    // <div>Parmi les éléments suivants du tissu cardio-necteur, lequel possède la fréquence propre la plus basse ?</div>
        //    // <ol>
        //    // <li>noeud auriculo-ventriculaire</li>
        //    // ...
        //    // </ol>
        //    // <br />
        //    // \t
        //    // A - B - D
        //    // \t

        //    //sw.Write($"<h1>{question.Year} - question no {question.Number}</h1>");
        //    //sw.Write($"<br />");
        //    //sw.Write($"<div>{question.QuestionText}</div>");
        //    //sw.Write($"<ol>");
        //    //foreach (string responseText in question.Responses)
        //    //    sw.Write($"<li>{responseText}</li>");
        //    //sw.Write($"</ol>");
        //    //sw.Write($"<br />");
        //    sw.Write(questionResponse.GetHtml(withoutNewLine: true));

        //    sw.Write("\t");

        //    //bool first = true;
        //    //// response.Responses.ToCharArray()
        //    //foreach (char responseCode in questionResponse.Response.Responses)
        //    //{
        //    //    if (!first)
        //    //        sw.Write(" - ");
        //    //    sw.Write(responseCode);
        //    //    first = false;
        //    //}

        //    sw.Write(questionResponse.Response.GetFormatedResponses());
        //    sw.Write("\t");
        //    sw.WriteLine();
        //}

        //private void CreateResponseDictionary(IEnumerable<Response> responses)
        //{
        //    _responses = new Dictionary<int, Response>();
        //    foreach (Response response in responses)
        //    {
        //        _responses.Add(GetQuestionId(response.Year, response.QuestionNumber), response);
        //    }
        //}

        //private Response GetResponse(int id)
        //{
        //    if (_responses.ContainsKey(id))
        //        return _responses[id];
        //    else
        //        throw new PBFileException($"response not found {_question.Year} - question no {_question.Number}", _question.SourceFile, _question.SourceLine);
        //}

        //private int GetQuestionId(int year, int questionNumber)
        //{
        //    if (questionNumber >= 1000)
        //        throw new PBFileException($"bad question number {_question.Year} - question no {_question.Number}", _question.SourceFile, _question.SourceLine);
        //    return year * 1000 + questionNumber;
        //}

        //public static void Write(string file, IEnumerable<Question> questions, IEnumerable<Response> responses)
        //{
        //    AnkiWriter ankiWriter = new AnkiWriter { _file = file };
        //    ankiWriter.CreateResponseDictionary(responses);
        //    ankiWriter._Write(questions);
        //}

        //public static void Write(string file, IEnumerable<QuestionResponse> questionResponses)
        //{
        //    AnkiWriter ankiWriter = new AnkiWriter { _file = file };
        //    ankiWriter._Write(questionResponses);
        //}

        public static void Write(string file, IEnumerable<AnkiQuestion> questions)
        {
            AnkiWriter ankiWriter = new AnkiWriter { _file = file };
            ankiWriter._Write(questions);
        }
    }
}

using pb;
using pb.IO;
using System.Collections.Generic;
using System.IO;

namespace anki
{
    public class AnkiWriter
    {
        private string _file = null;
        //private IEnumerable<Question> _questions = null;
        //private IEnumerable<Response> _responses = null;
        private Dictionary<int, Response> _responses = null;

        private void _Write(IEnumerable<Question> questions)
        {
            zfile.CreateFileDirectory(_file);
            using (FileStream fs = zFile.Create(_file))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach (Question question in questions)
                {
                    Response response = GetResponse(GetQuestionId(question.Year, question.Number));
                    WriteQuestion(sw, question, response);
                }
            }
        }

        private static void WriteQuestion(StreamWriter sw, Question question, Response response)
        {
            // <h1>2015 - QUESTION N 37</h1>
            // <br />
            // <div>Parmi les éléments suivants du tissu cardio-necteur, lequel possède la fréquence propre la plus basse ?</div>
            // <ol>
            // <li>noeud auriculo-ventriculaire</li>
            // ...
            // </ol>
            // <br />
            // \t
            // A - B - D
            // \t
            sw.Write($"<h1>{question.Year} - question no {question.Number}</h1>");
            sw.Write($"<br />");
            sw.Write($"<div>{question.QuestionText}</div>");
            sw.Write($"<ol>");
            foreach (string responseText in question.Responses)
                sw.Write($"<li>{responseText}</li>");
            sw.Write($"</ol>");
            sw.Write($"<br />");
            sw.Write($"\t");
            bool first = true;
            foreach (char responseCode in response.Responses.ToCharArray())
            {
                if (!first)
                    sw.Write(" - ");
                sw.Write(responseCode);
                first = false;
            }
            sw.Write($"\t");
            sw.WriteLine();
        }

        private void CreateResponseDictionary(IEnumerable<Response> responses)
        {
            _responses = new Dictionary<int, Response>();
            foreach (Response response in responses)
            {
                _responses.Add(GetQuestionId(response.Year, response.QuestionNumber), response);
            }
        }

        private Response GetResponse(int id)
        {
            if (_responses.ContainsKey(id))
                return _responses[id];
            else
                throw new PBException($"response not found id {id}");
        }

        private static int GetQuestionId(int year, int questionNumber)
        {
            if (questionNumber >= 1000)
                throw new PBException($"bad question number {questionNumber}");
            return year * 1000 + questionNumber;
        }

        public static void Write(string file, IEnumerable<Question> questions, IEnumerable<Response> responses)
        {
            AnkiWriter ankiWriter = new AnkiWriter { _file = file };
            ankiWriter.CreateResponseDictionary(responses);
            ankiWriter._Write(questions);
        }
    }
}

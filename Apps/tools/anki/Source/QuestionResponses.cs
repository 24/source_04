using pb;
using pb.Data.Mongo;
using pb.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace anki
{
    public class QuestionResponse
    {
        public Question Question;
        public Response Response;

        public string GetHtml(bool questionNumber = false, bool questionDiv = false, bool response = false, bool newLine = false)
        {
            // <div>              (addQuestionDiv)
            // <h1>2015 - QUESTION N 37</h1>
            // <br>
            // <div>Parmi les éléments suivants du tissu cardio-necteur, lequel possède la fréquence propre la plus basse ?</div>
            // <ol>
            // <li>noeud auriculo-ventriculaire</li>
            // ...
            // </ol>
            // </div>             (addQuestionDiv)
            // <hr align=left>    (withResponse)
            // A - B - D          (withResponse)

            StringBuilder sb = new StringBuilder();


            if (questionDiv)
            {
                sb.Append("<div>");
                if (newLine)
                    sb.AppendLine();
            }

            if (questionNumber)
            {
                sb.Append($"<h1>{Question.Year} - question no {Question.Number}</h1>");
                if (newLine)
                    sb.AppendLine();
                sb.Append("<br>");
                if (newLine)
                    sb.AppendLine();
            }
            sb.Append($"<div>{Question.QuestionText}</div>");
            if (newLine)
                sb.AppendLine();
            sb.Append("<ol>");
            if (newLine)
                sb.AppendLine();
            foreach (string responseText in Question.Choices)
            {
                sb.Append($"<li>{responseText}</li>");
                if (newLine)
                    sb.AppendLine();
            }
            sb.Append("</ol>");
            if (newLine)
                sb.AppendLine();
            //sb.Append($"<br>");
            //if (!withoutNewLine)
            //    sb.AppendLine();

            if (questionDiv)
            {
                sb.Append("</div>");
                if (newLine)
                    sb.AppendLine();
            }

            if (response)
            {
                // <HR align=left>
                //sb.Append($"<br>");
                sb.Append("<hr align=left>");
                if (newLine)
                    sb.AppendLine();
                //bool first = true;
                //// Response.Responses.ToCharArray()
                //foreach (char responseCode in Response.Responses)
                //{
                //    if (!first)
                //        sb.Append(" - ");
                //    sb.Append(responseCode);
                //    first = false;
                //}
                sb.Append(Response.GetFormatedResponse());
                if (newLine)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public static partial class QuestionResponses
    {
        public static IEnumerable<QuestionResponse> GetQuestionResponses(IEnumerable<Question> questions, IEnumerable<Response> responses)
        {
            Dictionary<int, Response> responseDictionary = CreateResponseDictionary(responses);
            foreach (Question question in questions)
            {
                int id = GetQuestionId(question.Year, question.Number);
                if (!responseDictionary.ContainsKey(id))
                    throw new PBFileException($"response not found {question.Year} - question no {question.Number}", question.SourceFile, question.SourceLine);
                yield return new QuestionResponse { Question = question, Response = responseDictionary[id] };
            }
        }

        public static void CreateHtmlFiles(IEnumerable<QuestionResponse> questionResponses, string directory, string htmlHeader, string htmlFooter)
        {
            zdir.CreateDirectory(directory);
            int index = 1;
            foreach (QuestionResponse questionResponse in questionResponses)
            {
                string html = questionResponse.GetHtml(questionNumber: true, questionDiv: true, response: true, newLine: true);
                string file = zPath.Combine(directory, $"question_{index:00}.html");
                using (StreamWriter sw = new StreamWriter(zFile.Create(file)))
                {
                    sw.WriteLine(htmlHeader);
                    sw.Write(html);
                    sw.WriteLine(htmlFooter);
                }
                index++;
            }
        }

        public static void CreateQuestionFiles(IEnumerable<QuestionResponse> questionResponses, string directory)
        {
            zdir.CreateDirectory(directory);
            int index = 1;
            foreach (QuestionResponse questionResponse in questionResponses)
            {
                QuestionResponseHtml questionResponseHtml = new QuestionResponseHtml();
                questionResponseHtml.Year = questionResponse.Question.Year;
                questionResponseHtml.Type = questionResponse.Question.Type;
                questionResponseHtml.Number = questionResponse.Question.Number;
                questionResponseHtml.QuestionText = questionResponse.Question.QuestionText;
                // newLine: true
                questionResponseHtml.QuestionHtml = questionResponse.GetHtml(questionNumber: false, questionDiv: false, response: false, newLine: false);
                questionResponseHtml.Choices = questionResponse.Question.Choices;
                questionResponseHtml.Responses = questionResponse.Response.Responses;
                questionResponseHtml.SourceFile = questionResponse.Question.SourceFile;
                questionResponseHtml.SourceLine = questionResponse.Question.SourceLine;

                string file = zPath.Combine(directory, $"question-{index:00}-{questionResponseHtml.Year:0000}-{questionResponseHtml.Number:000}.json");
                questionResponseHtml.zSave(file, jsonIndent: true);
                index++;
            }
        }

        private static Dictionary<int, Response> CreateResponseDictionary(IEnumerable<Response> responses)
        {
            Dictionary<int, Response> responseDictionary = new Dictionary<int, Response>();
            foreach (Response response in responses)
            {
                responseDictionary.Add(GetQuestionId(response.Year, response.QuestionNumber), response);
            }
            return responseDictionary;
        }

        //private static int GetQuestionId(Response response)
        //{
        //    if (response.QuestionNumber >= 1000)
        //        throw new PBException($"bad question number {response.Year} - question no {response.QuestionNumber}");
        //    return response.Year * 1000 + response.QuestionNumber;
        //}

        private static int GetQuestionId(int year, int questionNumber)
        {
            if (questionNumber >= 1000)
                throw new PBException($"bad question number {year} - question no {questionNumber}");
            return year * 1000 + questionNumber;
        }
    }
}

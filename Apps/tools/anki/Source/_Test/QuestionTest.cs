using pb;
using pb.Data;
using pb.IO;
using pb.Text;
using System.Collections.Generic;

namespace anki.Test
{
    public static class QuestionTest
    {
        public static NamedValues<ZValue> Test_Find_01(string text, RegexValuesList regexList)
        {
            Trace.WriteLine($"text : \"{text}\"");
            //RegexValuesList regexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
            //FindText findText = QuestionRun.GetQuestionRegexValuesList().Find(text);
            FindText_v2 findText = regexList.Find(text);
            Trace.WriteLine($"found : {findText.Success}");
            return findText.GetAllValues();
        }

        //public static void Test_FindInFile_01(string file)
        //{
        //    //RegexValuesList regexList = new RegexValuesList(XmlConfig.CurrentConfig.GetElements("QuestionInfos/QuestionInfo"), compileRegex: true);
        //    RegexValuesList regexList = QuestionRun.GetQuestionRegexValuesList();
        //    foreach (string line in zFile.ReadLines(file))
        //    {
        //        string line2 = line.Trim();
        //        if (line2 == "")
        //            continue;
        //        Trace.WriteLine($"read : \"{line2}\"");
        //        FindText findText = regexList.Find(line2);
        //        if (findText.Found)
        //        {
        //            foreach (KeyValuePair<string, ZValue> namedValue in findText.matchValues.GetValues())
        //                Trace.WriteLine($"  found : {namedValue.Key} = \"{namedValue.Value}\"");
        //        }
        //        else
        //        {
        //            Trace.WriteLine($"  unknow text");
        //        }
        //    }
        //}

        public static void Test_Find_02(string text, RegexValuesList regexList)
        {
            Trace.WriteLine($"text : \"{text}\"");
            FindText_v2 findText = regexList.Find(text);
            if (findText.Success)
            {
                //MatchValues matchValues = findText.matchValues;
                while (findText.Success)
                {
                    Trace.WriteLine();
                    Trace.WriteLine($"  found : index {findText.Match.Index} length {findText.Match.Length}");
                    foreach (KeyValuePair<string, ZValue> namedValue in findText.GetValues())
                        Trace.WriteLine($"        : {namedValue.Key} = \"{namedValue.Value}\"");
                    //matchValues = matchValues.Next();
                    findText.Next();
                }
            }
            else
            {
                Trace.WriteLine($"  unknow text");
            }
        }

        public static void Test_FindInFile_01(string file, RegexValuesList regexList)
        {
            //RegexValuesList regexList = QuestionRun.GetResponseRegexValuesList();
            int lineNumber = 0;
            foreach (string line in zFile.ReadLines(file))
            {
                //string line2 = line.Trim();
                //if (line2 == "")
                //    continue;

                lineNumber++;
                if (line == "")
                    continue;

                Trace.WriteLine();
                Trace.WriteLine($"read : line {lineNumber} \"{line}\"");
                FindText_v2 findText = regexList.Find(line);
                if (findText.Success)
                {
                    //MatchValues matchValues = findText.matchValues;
                    while (findText.Success)
                    {
                        Trace.WriteLine();
                        Trace.WriteLine($"  found : index {findText.Match.Index} length {findText.Match.Length}");
                        foreach (KeyValuePair<string, ZValue> namedValue in findText.GetValues())
                            Trace.WriteLine($"        : {namedValue.Key} = \"{namedValue.Value}\"");
                        //matchValues = matchValues.Next();
                        findText.Next();
                    }
                }
                else
                {
                    Trace.WriteLine($"  unknow text");
                }
            }
        }
    }
}

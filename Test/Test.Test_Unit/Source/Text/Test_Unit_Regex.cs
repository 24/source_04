using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using pb.Data.Mongo;

namespace Test.Test_Unit.Text
{
    public class CompactRegexMatches
    {
        public CompactRegexMatches(string input, Match match)
        {
            this.input = input;
            this.found = false;
            List<CompactRegexMatch> matchList = new List<CompactRegexMatch>();
            while (match.Success)
            {
                this.found = true;
                CompactRegexMatch regexMatch = new CompactRegexMatch();
                regexMatch.value = match.Value;
                bool firstGroup = true;
                bool secondGroup = true;
                StringBuilder sb = new StringBuilder();
                foreach (Group group in match.Groups)
                {
                    if (firstGroup)
                    {
                        firstGroup = false;
                        continue;
                    }

                    if (!secondGroup)
                        sb.Append(' ');
                    secondGroup = false;
                    sb.Append('(');

                    bool firstCapture = true;
                    foreach (Capture capture in group.Captures)
                    {
                        if (!firstCapture)
                            sb.Append(", ");
                        sb.AppendFormat("\"{0}\"", capture.Value);
                        firstCapture = false;
                    }
                    sb.Append(')');
                }
                regexMatch.groups = sb.ToString();
                matchList.Add(regexMatch);
                match = match.NextMatch();
            }
            matches = matchList.ToArray();
        }

        public string input;
        public bool found;
        public CompactRegexMatch[] matches;
    }

    public class CompactRegexMatch
    {
        public string value;
        public string groups;
    }

    public class DetailRegexMatches
    {
        public DetailRegexMatches(string input, Match match)
        {
            this.input = input;
            this.found = false;
            List<DetailRegexMatch> matchList = new List<DetailRegexMatch>();
            while (match.Success)
            {
                this.found = true;
                DetailRegexMatch regexMatch = new DetailRegexMatch();
                regexMatch.indexMatch = match.Index;
                regexMatch.lengthMatch = match.Length;
                regexMatch.value = match.Value;
                List<DetailRegexGroup> groupList = new List<DetailRegexGroup>();
                foreach (Group group in match.Groups)
                {
                    DetailRegexGroup regexGroup = new DetailRegexGroup();
                    regexGroup.indexMatch = group.Index;
                    regexGroup.lengthMatch = group.Length;
                    regexGroup.value = group.Value;
                    List<DetailRegexCapture> captureList = new List<DetailRegexCapture>();
                    foreach (Capture capture in group.Captures)
                    {
                        DetailRegexCapture regexCapture = new DetailRegexCapture();
                        regexCapture.indexMatch = capture.Index;
                        regexCapture.lengthMatch = capture.Length;
                        regexCapture.value = capture.Value;
                        captureList.Add(regexCapture);
                    }
                    regexGroup.captures = captureList.ToArray();
                    groupList.Add(regexGroup);
                }
                regexMatch.groups = groupList.ToArray();
                matchList.Add(regexMatch);
                match = match.NextMatch();
            }
            matches = matchList.ToArray();
        }

        public string input;
        public bool found;
        public DetailRegexMatch[] matches;
    }

    public class DetailRegexMatch
    {
        public int indexMatch = 0;
        public int lengthMatch = 0;
        public string value;
        public DetailRegexGroup[] groups;
    }

    public class DetailRegexGroup
    {
        public int indexMatch = 0;
        public int lengthMatch = 0;
        public string value;
        public DetailRegexCapture[] captures;
    }

    public class DetailRegexCapture
    {
        public int indexMatch = 0;
        public int lengthMatch = 0;
        public string value;
    }

    public static class Test_Unit_Regex
    {
        public static void Test_Regex_01(string pattern, string file, RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase)
        {
            //global::Test.Test_Text.Test_Regex.Test();
            Regex regex = new Regex(pattern, options);
            file = Path.Combine(GetDirectory(), file);
            zmongo.BsonReader<BsonDocument>(file).zDetailRegexMatches(regex).zSave(zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_matches_detail_bson"));
            zmongo.BsonReader<BsonDocument>(file).zCompactRegexMatches(regex).zSave(zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_matches_compact_bson"));
        }

        public static void TraceDetailRegexMatches(string file, string outputFile)
        {
            zmongo.BsonReader<DetailRegexMatches>(Path.Combine(GetDirectory(), file)).zTraceDetailRegexMatches(Path.Combine(GetDirectory(), outputFile));
        }

        public static void TraceCompactRegexMatches(string file, string outputFile)
        {
            zmongo.BsonReader<CompactRegexMatches>(Path.Combine(GetDirectory(), file)).zTraceCompactRegexMatches(Path.Combine(GetDirectory(), outputFile));
            zmongo.BsonReader<CompactRegexMatches>(Path.Combine(GetDirectory(), file)).zView();
        }

        public static IEnumerable<DetailRegexMatches> zDetailRegexMatches(this IEnumerable<BsonDocument> documents, Regex regex)
        {
            foreach (BsonDocument document in documents)
            {
                string text = document["text"].zAsString();
                yield return new DetailRegexMatches(text, regex.Match(text));
            }
        }

        public static IEnumerable<CompactRegexMatches> zCompactRegexMatches(this IEnumerable<BsonDocument> documents, Regex regex)
        {
            foreach (BsonDocument document in documents)
            {
                string text = document["text"].zAsString();
                yield return new CompactRegexMatches(text, regex.Match(text));
            }
        }

        public static void zTraceDetailRegexMatches(this IEnumerable<DetailRegexMatches> regexMatchesList, string file)
        {
            using (StreamWriter sw = File.CreateText(file))
            {
                foreach (DetailRegexMatches regexMatches in regexMatchesList)
                {
                    sw.WriteLine("input                           : \"{0}\"", regexMatches.input);
                    sw.WriteLine("  result                        : {0}", regexMatches.found ? "found" : "not found");
                    int i = 1;
                    foreach (DetailRegexMatch match in regexMatches.matches)
                    {
                        sw.WriteLine("  result  no {0}                  : \"{1}\"", i++, match.value);
                        sw.Write("  groups                        :");
                        bool firstGroup = true;
                        foreach (DetailRegexGroup group in match.groups)
                        {
                            if (firstGroup)
                            {
                                firstGroup = false;
                                continue;
                            }
                            sw.Write(" (");
                            bool firstCapture = true;
                            foreach (DetailRegexCapture capture in group.captures)
                            {
                                if (!firstCapture)
                                    sw.Write(", ");
                                sw.Write("\"{0}\"", capture.value);
                                firstCapture = false;
                            }
                            sw.Write(")");
                        }
                        sw.WriteLine();
                    }
                    sw.WriteLine();
                }
            }
        }

        public static void zTraceCompactRegexMatches(this IEnumerable<CompactRegexMatches> regexMatchesList, string file)
        {
            using (StreamWriter sw = File.CreateText(file))
            {
                foreach (CompactRegexMatches regexMatches in regexMatchesList)
                {
                    sw.WriteLine("input                           : \"{0}\"", regexMatches.input);
                    sw.WriteLine("  result                        : {0}", regexMatches.found ? "found" : "not found");
                    int i = 1;
                    foreach (CompactRegexMatch match in regexMatches.matches)
                    {
                        sw.WriteLine("  result  no {0}                  : \"{1}\"", i++, match.value);
                        sw.WriteLine("  groups                        : {0}", match.groups);
                    }
                    sw.WriteLine();
                }
            }
        }

        private static string GetDirectory()
        {
            return Path.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Text\Regex");
        }
    }
}

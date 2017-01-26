using System.Text.RegularExpressions;

namespace pb.Text.Test
{
    public static class Test_Regex
    {
        //public static void Test_Regex(string sRegex, string sInput)
        //{
        //    Test_Regex(sRegex, sInput, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //}

        public static void Test(string regexPattern, string text, RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase)
        {
            Regex regex = new Regex(regexPattern, options);
            Trace.WriteLine("Regex     : {0}", regexPattern);
            Trace.WriteLine("Input     : {0}", text);
            Match match = regex.Match(text);
            //string s;
            //if (match.Success) s = "found"; else s = "not found";
            //Trace.WriteLine("Result    : {0}", s);
            if (!match.Success)
                Trace.WriteLine("Result    : not found");
            int n = 1;
            while (match.Success)
            {
                Trace.WriteLine("Result    : found no {0}", n++);
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    Trace.WriteLine("Groups[{0}] : \"{1}\"", i, match.Groups[i].Value);
                    if (match.Groups[i].Captures.Count > 1)
                    {
                        for (int j = 0; j < match.Groups[i].Captures.Count; j++)
                        {
                            Trace.WriteLine(" Capture[{0}] : \"{1}\"", j, match.Groups[i].Captures[j]);
                        }
                    }
                }
                match = match.NextMatch();
            }
        }
    }
}

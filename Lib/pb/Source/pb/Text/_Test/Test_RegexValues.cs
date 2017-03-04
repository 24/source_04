using System.Collections.Generic;
using pb.Data;

namespace pb.Text.Test
{
    public static class Test_RegexValues
    {
        public static void Test(RegexValuesList regexValuesList, string text)
        {
            Trace.WriteLine("Input     : {0}", text);
            //FindText findText = regexValuesList.Find(text);
            FindText_v2 findText = regexValuesList.Find(text);
            if (!findText.Success)
                Trace.WriteLine("Result           : not found");
            else
            {
                Trace.WriteLine("Result           : found");
                Trace.WriteLine("  Text           : \"{0}\"", findText.Text);
                Trace.WriteLine("  Key            : \"{0}\"", findText.Key);
                Trace.WriteLine("  Name           : \"{0}\"", findText.Name);
                Trace.WriteLine("  Pattern        : \"{0}\"", findText.Pattern);

                Trace.WriteLine("  Attributes     : {0}", findText.Attributes.Count);
                foreach (KeyValuePair<string, string> attribute in findText.Attributes)
                    Trace.WriteLine("    Attribute  : \"{0}\" = \"{1}\"", attribute.Key, attribute.Value);

                NamedValues<ZValue> values = findText.GetValues();
                Trace.WriteLine("  GetValues()    : {0}", values.Count);
                foreach (KeyValuePair<string, ZValue> value in values)
                    Trace.WriteLine("    Value      : \"{0}\" = \"{1}\"", value.Key, value.Value);

                values = findText.GetAllValues();
                Trace.WriteLine("  GetAllValues() : {0}", values.Count);
                foreach (KeyValuePair<string, ZValue> value in values)
                    Trace.WriteLine("    AllValue   : \"{0}\" = {1}", value.Key, value.Value);
            }
        }

        public static void Test2(RegexValuesList regexValuesList, string text, bool contiguous = false)
        {
            Trace.WriteLine("Input     : {0}", text);
            //FindText findText = regexValuesList.Find(text);
            FindText_v2 findText = regexValuesList.Find(text);
            if (!findText.Success)
            {
                Trace.WriteLine("Result           : not found");
                return;
            }
            while (findText.Success)
            {
                Trace.Write($"found \"{findText.Text}\" position {findText.Match.Index + 1} length {findText.Match.Length}");
                NamedValues<ZValue> values = findText.GetValues();
                if (values.Count > 0)
                {
                    Trace.Write(" values");
                    foreach (KeyValuePair<string, ZValue> value in findText.GetValues())
                        Trace.Write($" \"{value.Key}\" = \"{value.Value}\"");
                }
                if (findText.Attributes.Count > 0)
                {
                    Trace.Write(" attributes");
                    foreach (KeyValuePair<string, string> attribute in findText.Attributes)
                        Trace.Write($" \"{attribute.Key}\" = \"{attribute.Value}\"");
                }
                Trace.WriteLine();
                findText.FindNext(contiguous);
            }
        }
    }
}

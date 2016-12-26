using System.Collections.Generic;
using pb;
using pb.Data;
using pb.Text;

namespace Test.Test_Text
{
    public static class Test_RegexValues
    {
        public static void Test(RegexValuesList regexValuesList, string text)
        {
            Trace.WriteLine("Input     : {0}", text);
            FindText findText = regexValuesList.Find(text);
            if (!findText.Found)
                Trace.WriteLine("Result           : not found");
            else
            {
                Trace.WriteLine("Result           : found");
                Trace.WriteLine("  Text           : \"{0}\"", findText.Text);
                Trace.WriteLine("  Key            : \"{0}\"", findText.matchValues.Key);
                Trace.WriteLine("  Name           : \"{0}\"", findText.matchValues.Name);
                Trace.WriteLine("  Pattern        : \"{0}\"", findText.matchValues.Pattern);

                Trace.WriteLine("  Attributes     : {0}", findText.matchValues.Attributes.Count);
                foreach (KeyValuePair<string, string> attribute in findText.matchValues.Attributes)
                    Trace.WriteLine("    Attribute  : \"{0}\" = \"{1}\"", attribute.Key, attribute.Value);

                NamedValues<ZValue> values = findText.matchValues.GetValues();
                Trace.WriteLine("  GetValues()    : {0}", findText.matchValues.Attributes.Count);
                foreach (KeyValuePair<string, ZValue> value in values)
                    Trace.WriteLine("    Value      : \"{0}\" = \"{1}\"", value.Key, value.Value);

                values = findText.matchValues.GetAllValues();
                Trace.WriteLine("  GetAllValues() : {0}", findText.matchValues.Attributes.Count);
                foreach (KeyValuePair<string, ZValue> value in values)
                    Trace.WriteLine("    AllValue   : \"{0}\" = {1}", value.Key, value.Value);
            }
        }
    }
}

using System;
using System.Text;

namespace anki
{
    public static partial class HtmlEncoder
    {
        // encode only '<', '>', '&'
        public static string HtmlEncodeLevel1(string s)
        {
            if (s == null)
                return null;

            if (s.Length == 0)
                return String.Empty;

            bool needEncode = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '&' || c == '<' || c == '>')
                {
                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
                return s;

            StringBuilder output = new StringBuilder();
            int len = s.Length;

            for (int i = 0; i < len; i++)
            {
                char ch = s[i];
                switch (ch)
                {
                    case '&':
                        output.Append("&amp;");
                        break;
                    case '>':
                        output.Append("&gt;");
                        break;
                    case '<':
                        output.Append("&lt;");
                        break;
                    default:
                        output.Append(ch);
                        break;
                }
            }
            return output.ToString();
        }
    }
}

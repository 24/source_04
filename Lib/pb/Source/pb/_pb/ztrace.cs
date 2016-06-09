using System.Text;

namespace pb
{
    public static class ztrace
    {
        public static void TraceUnicode(string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            Trace.WriteLine("char - code point - {0}", encoding.EncodingName);
            foreach (char c in text)
            {
                if (!char.IsControl(c))
                    Trace.Write("'{0}'  ", c);
                else
                    Trace.Write("'.'  ");
                string codePointHex;
                int codePoint = (int)c;
                if (codePoint < 0x100)
                    codePointHex = ((byte)codePoint).zToHex();
                else if (codePoint < 0x10000)
                    codePointHex = ((short)codePoint).zToHex();
                else
                    codePointHex = codePoint.zToHex();
                Trace.Write("- 0x{0,-8} -", codePointHex);
                foreach (byte b in encoding.GetBytes(new char[] { c }))
                    Trace.Write(" 0x{0}", b.zToHex());
                Trace.WriteLine();
            }
        }

        public static void TraceBytes(byte[] bytes)
        {
            //System.Text.Encoding.GetEncoding("ibm850")
            int nbBytesLine = 16;
            int nbBytesSeparator = 8;
            int col = 0;
            int index = 0;
            StringBuilder sb = new StringBuilder();
            Encoding encoding = Encoding.Default;
            foreach (byte b in bytes)
            {
                if (col == 0)
                    Trace.Write("{0}:", index.zToHex());
                else if (col % nbBytesSeparator == 0)
                    Trace.Write(" |");
                Trace.Write(" {0}", b.zToHex());
                foreach (char c in encoding.GetChars(new byte[] { b }))
                {
                    if (char.IsControl(c))
                        sb.Append('.');
                    else
                        sb.Append(c);
                }
                if (++col == nbBytesLine)
                {
                    Trace.Write(" | ");
                    Trace.WriteLine(sb.ToString());
                    sb.Clear();
                    col = 0;
                }
                index++;
            }
            if (col != 0)
            {
                for (; col < nbBytesLine; col++)
                {
                    if (col % nbBytesSeparator == 0)
                        Trace.Write(" |");
                    Trace.Write("   ");
                }
                Trace.Write(" | ");
                Trace.WriteLine(sb.ToString());
            }
        }
    }
}

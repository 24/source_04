using System;
using System.Text;

namespace Test_wcf_service
{
    public static class TraceToMemory
    {
        private static StringBuilder _sb = new StringBuilder();

        public static string GetTrace()
        {
            return _sb.ToString();
        }

        public static void Write(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            _sb.Append(msg);
        }

        public static void WriteLine()
        {
            Write("\r\n");
        }

        public static void WriteLine(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Write(msg);
            WriteLine();
        }
    }
}
namespace pb
{
    public static class TraceLevel
    {
        private static int _level = 1;

        public static int Level { get { return _level; } set { _level = value; } }

        public static void Write(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _level)
            {
                if (prm.Length > 0)
                    msg = string.Format(msg, prm);
                Trace.Write(msg);
            }
        }

        public static void WriteLine(int traceLevel)
        {
            if (traceLevel <= _level)
                Trace.WriteLine();
        }

        public static void WriteLine(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _level)
            {
                if (prm.Length > 0)
                    msg = string.Format(msg, prm);
                Trace.WriteLine(msg);
            }
        }
    }
}

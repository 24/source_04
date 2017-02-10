namespace pb.old
{
    public static partial class Trace
    {
        public static void Write(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _currentTrace.TraceLevel)
            {
                if (prm.Length > 0)
                    msg = string.Format(msg, prm);
                _currentTrace.Write(msg);
            }
        }

        public static void WriteLine(int traceLevel)
        {
            if (traceLevel <= _currentTrace.TraceLevel)
                _currentTrace.WriteLine();
        }

        public static void WriteLine(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _currentTrace.TraceLevel)
            {
                if (prm.Length > 0)
                    msg = string.Format(msg, prm);
                _currentTrace.WriteLine(msg);
            }
        }
    }

    public partial class TTrace : ITrace
    {
        private int _traceLevel = 1;

        public int TraceLevel { get { return _traceLevel; } set { _traceLevel = value; } }

        public void Write(int traceLevel, string msg)
        {
            if (traceLevel <= _traceLevel)
                Write(msg);
        }

        public void Write(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _traceLevel)
            {
                if (prm.Length > 0)
                    msg = string.Format(msg, prm);
                Write(msg);
            }
        }

        public void WriteLine(int traceLevel)
        {
            if (traceLevel <= _traceLevel)
                Write("\r\n");
        }

        public void WriteLine(int traceLevel, string msg)
        {
            if (traceLevel <= _traceLevel)
            {
                Write(msg);
                WriteLine();
            }
        }

        public void WriteLine(int traceLevel, string msg, params object[] prm)
        {
            if (traceLevel <= _traceLevel)
            {
                if (prm.Length > 0)
                    msg = string.Format(msg, prm);
                Write(msg);
                WriteLine();
            }
        }
    }
}

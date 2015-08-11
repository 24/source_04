using System;

namespace pb
{
    public partial interface ITrace
    {
        void Write(string msg);
        void Write(string msg, params object[] prm);
        void WriteLine();
        void WriteLine(string msg);
        void WriteLine(string msg, params object[] prm);
        void SetAsCurrentTrace();
        void SetViewer(Action<string> viewer);
    }
}

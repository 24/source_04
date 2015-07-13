using System;

namespace pb
{
    public delegate void WritedEvent(string msg);
    //public delegate void WriteToTraceEvent(string msg);

    [Flags]
    public enum LogOptions_v1
    {
        None           = 0x0000,
        IndexedFile    = 0x0001,
        RazLogFile     = 0x0002,
        LogToConsole   = 0x0004
    }

    public interface ITrace_v1
    {
        int TraceLevel { get; set; }
        //string TraceDir { get; set; }
        //bool TraceStackError { get; set; }
        event WritedEvent Writed;
        //event WriteToTraceEvent WriteToTrace;
        void SetAsCurrentTrace();
        void SetLogFile(string file, LogOptions_v1 options);
        void SetTraceDirectory(string dir);
        void AddTraceFile(string file, LogOptions_v1 options);
        void RemoveTraceFile(string file);
        void Write(string msg, params object[] prm);
        void WriteLine();
        void WriteLine(string msg, params object[] prm);
        void WriteError(Exception ex);
    }
}

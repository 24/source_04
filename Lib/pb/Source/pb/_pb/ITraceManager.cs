using pb.IO;
using System;

namespace pb
{
    public interface ITraceManager
    {
        void AddTrace(ITrace trace);
        void RemoveTrace(ITrace trace);
        void SetWriter(string file, FileOption option);
        void RemoveWriter();
        void SetViewer(Action<string> viewer);
        void RemoveViewer();
        void SetAsCurrent();
    }
}

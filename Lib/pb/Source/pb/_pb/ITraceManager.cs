using pb.IO;
using System;

namespace pb
{
    public interface ITraceManager
    {
        void AddTrace(ITrace trace);
        void RemoveTrace(ITrace trace);
        //void SetWriter(string file, FileOption option);
        void SetWriter(IWriteToFile writer, string name = "_default");
        IWriteToFile GetWriter(string name = "_default");
        //void RemoveWriter();
        void RemoveWriter(string name = "_default");
        void SetViewer(Action<string> viewer);
        void RemoveViewer();
        void EnableViewer();
        void DisableViewer();
        bool IsViewerDisabled();
        void SetAsCurrent();
    }
}

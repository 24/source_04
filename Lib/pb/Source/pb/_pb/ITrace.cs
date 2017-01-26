using System;

namespace pb
{
    public interface ITrace
    {
        void SetAsCurrent();
        void AddOnWrite(string name, Action<string> onWrite);
        void RemoveOnWrite(string name);
        void AddOnWriteError(string name, Action<Exception> onWriteError);
        void RemoveOnWriteError(string name);
    }
}

using pb.IO;
using System;

namespace pb
{
    public class TraceManager : ITraceManager
    {
        private static TraceManager _current = new TraceManager();
        private WriteToFileBase _writer = null;
        private bool _disableViewer = false;
        private Action<string> _viewer = null;
        private bool _traceStackErrorToFile = true;
        private bool _viewStackError = true;
        private bool _traceStackError = true;

        public static TraceManager Current { get { return _current; } }
        public bool DisableViewer { get { return _disableViewer; } set { _disableViewer = value; } }
        public bool TraceStackError { get { return _traceStackError; } set { _traceStackError = value; } }
        public bool TraceStackErrorToFile { get { return _traceStackErrorToFile; } set { _traceStackErrorToFile = value; } }
        public bool ViewStackError { get { return _viewStackError; } set { _viewStackError = value; } }

        public void SetAsCurrent()
        {
            _current = this;
        }

        public void AddTrace(ITrace trace)
        {
            trace.AddOnWrite("TraceManager", TraceWrite);
            trace.AddOnWriteError("TraceManager", TraceWriteError);
        }

        public void RemoveTrace(ITrace trace)
        {
            trace.RemoveOnWrite("TraceManager");
            trace.RemoveOnWriteError("TraceManager");
        }

        private void TraceWrite(string msg)
        {
            _writer?.Write(msg);
            if (!_disableViewer)
                _viewer?.Invoke(msg);
        }

        private void TraceWriteError(Exception ex)
        {
            string err = string.Format("{0:dd/MM/yyyy HH:mm:ss} ", DateTime.Now) + Error.GetErrorMessage(ex, false, true) + Environment.NewLine;
            string stack = "----------------------" + Environment.NewLine + Error.GetErrorStackTrace(ex) + Environment.NewLine;

            _writer?.Write(err);
            if (_traceStackError && _traceStackErrorToFile)
                _writer?.Write(stack);
            if (!_disableViewer)
            {
                _viewer?.Invoke(err);
                if (_traceStackError && _viewStackError)
                    _viewer?.Invoke(stack);
            }
        }

        public void SetWriter(string file, FileOption option)
        {
            if (_writer != null)
                _writer.Close();
            _writer = WriteToFile.Create(file, option);

        }

        public void RemoveWriter()
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
        }

        public void SetViewer(Action<string> viewer)
        {
            _viewer = viewer;
        }

        public void RemoveViewer()
        {
            _viewer = null;
        }
    }
}

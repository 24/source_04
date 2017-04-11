using pb.IO;
using System;
using System.Collections.Generic;

namespace pb
{
    public class TraceManager : ITraceManager
    {
        private static TraceManager _current = new TraceManager();
        //private WriteToFileBase _writer = null;
        private Dictionary<string, IWriteToFile> _writers = new Dictionary<string, IWriteToFile>();
        private bool _disableViewer = false;
        private Action<string> _viewer = null;
        private bool _traceStackErrorToFile = true;
        private bool _viewStackError = true;
        private bool _traceStackError = true;

        public static TraceManager Current { get { return _current; } }
        //public bool DisableViewer { get { return _disableViewer; } set { _disableViewer = value; } }
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
            //_writer?.Write(msg);
            Write(msg);
            if (!_disableViewer)
                _viewer?.Invoke(msg);
        }

        private void TraceWriteError(Exception ex)
        {
            string err = string.Format("{0:dd/MM/yyyy HH:mm:ss} ", DateTime.Now) + Error.GetErrorMessage(ex, false, true) + Environment.NewLine;
            string stack = "----------------------" + Environment.NewLine + Error.GetErrorStackTrace(ex) + Environment.NewLine;

            //_writer?.Write(err);
            Write(err);
            if (_traceStackError && _traceStackErrorToFile)
                //_writer?.Write(stack);
                Write(stack);
            if (!_disableViewer)
            {
                _viewer?.Invoke(err);
                if (_traceStackError && _viewStackError)
                    _viewer?.Invoke(stack);
            }
        }

        private void Write(string msg)
        {
            foreach(IWriteToFile writer in _writers.Values)
                writer.Write(msg);
        }

        //public void SetWriter(string file, FileOption option = FileOption.None)
        //{
        //    if (_writer != null)
        //        _writer.Close();
        //    _writer = WriteToFile.Create(file, option);
        //}

        public void SetWriter(IWriteToFile writer, string name = "_default")
        {
            RemoveWriter(name);
            _writers.Add(name, writer);
        }

        public IWriteToFile GetWriter(string name = "_default")
        {
            if (_writers.ContainsKey(name))
                return _writers[name];
            else
                return null;
        }

        //public void RemoveWriter()
        //{
        //    if (_writer != null)
        //    {
        //        _writer.Close();
        //        _writer = null;
        //    }
        //}

        public void RemoveWriter(string name = "_default")
        {
            if (_writers.ContainsKey(name))
            {
                _writers[name].Close();
                _writers.Remove(name);
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

        public void EnableViewer()
        {
            _disableViewer = false;
        }

        public void DisableViewer()
        {
            _disableViewer = true;
        }

        public bool IsViewerDisabled()
        {
            return _disableViewer;
        }
    }
}

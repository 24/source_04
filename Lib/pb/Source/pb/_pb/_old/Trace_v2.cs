using System;
using System.Collections.Generic;
using System.Text;
using pb.IO;

namespace pb.old
{
    public static partial class Trace
    {
        private static TTrace _currentTrace = new TTrace();

        public static TTrace CurrentTrace
        {
            get
            {
                return _currentTrace;
            }
            set
            {
                //WriteLine("Trace : set new CurrentTrace  to \"{0}\"", value != null ? value.GetLogFile() : "(null)");
                _currentTrace = value;
            }
        }

        public static void Write(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            _currentTrace.Write(msg);
        }

        public static void WriteLine()
        {
            _currentTrace.WriteLine();
        }

        public static void WriteLine(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            _currentTrace.WriteLine(msg);
        }
    }

    public partial class TTrace : ITrace
    {
        private bool _disableViewer = false;
        private Action<string> _viewer = null;
        private WriteToFileBase _writer = null;
        private Dictionary<string, Action<string>> _onWrites = new Dictionary<string, Action<string>>();
        private Action<string> _onWrite = null;

        public bool DisableViewer { get { return _disableViewer; } set { _disableViewer = value; } }

        public void Close()
        {
            if (_writer != null)
                _writer.Close();
        }

        public void Write(string msg)
        {
            if (_writer != null)
                _writer.Write(msg);
            if (!_disableViewer && _viewer != null)
                _viewer(msg);
            if (_onWrite != null)
                _onWrite(msg);
        }

        public void Write(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Write(msg);
        }

        public void WriteLine()
        {
            Write("\r\n");
        }

        public void WriteLine(string msg)
        {
            Write(msg);
            WriteLine();
        }

        public void WriteLine(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Write(msg);
            WriteLine();
        }

        public void SetAsCurrentTrace()
        {
            Trace.CurrentTrace = this;
        }

        public void SetViewer(Action<string> viewer)
        {
            _viewer = viewer;
        }

        public void SetWriter(WriteToFileBase writer)
        {
            if (_writer != null)
                _writer.Close();
            _writer = writer;
        }

        public void SetWriter(string file, Encoding encoding = null, bool razFile = false)
        {
            SetWriter(new WriteToFile(file, encoding, appendToFile: !razFile));
        }

        public string GetWriterFile()
        {
            return _writer != null ? _writer.File : null;
        }

        public void AddOnWrite(string name, Action<string> onWrite)
        {
            if (_onWrites.ContainsKey(name))
                throw new PBException("OnWrite already exist : \"{0}\"", name);
            _onWrites.Add(name, onWrite);
            _onWrite += onWrite;
        }

        public void RemoveOnWrite(string name)
        {
            if (!_onWrites.ContainsKey(name))
                throw new PBException("unknow OnWrite, can't remove it: \"{0}\"", name);
            //_onWrite += _onWrites[name];
            _onWrite -= _onWrites[name];
            _onWrites.Remove(name);
        }
    }
}

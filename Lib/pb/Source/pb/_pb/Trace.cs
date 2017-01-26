using System;
using System.Collections.Generic;

namespace pb
{
    public class Trace : ITrace
    {
        private static Trace _current = new Trace();
        private Dictionary<string, Action<string>> _onWrites = new Dictionary<string, Action<string>>();
        private Action<string> _onWrite = null;
        private Dictionary<string, Action<Exception>> _onWriteErrors = new Dictionary<string, Action<Exception>>();
        private Action<Exception> _onWriteError = null;

        public static Trace Current { get { return _current; } }

        public void SetAsCurrent()
        {
            _current = this;
        }

        public static void Write(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            _current._onWrite?.Invoke(msg);
        }

        public static void WriteLine()
        {
            _current._onWrite?.Invoke(Environment.NewLine);
        }

        public static void WriteLine(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            _current._onWrite?.Invoke(msg + Environment.NewLine);
        }

        public void AddOnWrite(string name, Action<string> onWrite)
        {
            if (_onWrites.ContainsKey(name))
                //throw new PBException("OnWrite already exist : \"{0}\"", name);
                RemoveOnWrite(name);
            _onWrites.Add(name, onWrite);
            _onWrite += onWrite;
        }

        public void RemoveOnWrite(string name)
        {
            //if (!_onWrites.ContainsKey(name))
            //    throw new PBException("unknow OnWrite, can't remove it: \"{0}\"", name);
            if (_onWrites.ContainsKey(name))
            {
                _onWrite -= _onWrites[name];
                _onWrites.Remove(name);
            }
        }

        public static void WriteError(Exception ex)
        {
            _current._onWriteError?.Invoke(ex);
        }

        public void AddOnWriteError(string name, Action<Exception> onWriteError)
        {
            if (_onWriteErrors.ContainsKey(name))
                //throw new PBException($"OnWriteError already exist : \"{name}\"");
                RemoveOnWriteError(name);
            _onWriteErrors.Add(name, onWriteError);
            _onWriteError += onWriteError;
        }

        public void RemoveOnWriteError(string name)
        {
            //if (!_onWriteErrors.ContainsKey(name))
            //    throw new PBException($"unknow OnWriteError, can't remove it: \"{name}\"");
            if (_onWriteErrors.ContainsKey(name))
            {
                _onWriteError -= _onWriteErrors[name];
                _onWriteErrors.Remove(name);
            }
        }
    }
}


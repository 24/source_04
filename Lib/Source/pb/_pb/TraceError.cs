using System;
using pb;

namespace pb
{
    public partial class TTrace : ITrace
    {
        private bool _traceStackErrorToFile = true;
        private bool _viewStackError = true;
        private bool _traceStackError = true;

        public bool TraceStackError { get { return _traceStackError; } set { _traceStackError = value; } }
        public bool TraceStackErrorToFile { get { return _traceStackErrorToFile; } set { _traceStackErrorToFile = value; } }
        public bool ViewStackError { get { return _viewStackError; } set { _viewStackError = value; } }

        public void WriteError(Exception ex)
        {
            string err = string.Format("{0:dd/MM/yyyy HH:mm:ss} ", DateTime.Now) + Error.GetErrorMessage(ex, false, true);
            string stack = "----------------------\r\n" + Error.GetErrorStackTrace(ex);

            if (_writer != null)
            {
                _writer.Write(err);
                if (_traceStackError && _traceStackErrorToFile)
                    _writer.Write(stack);
            }
            if (!_disableViewer && _viewer != null)
            {
                _viewer(err);
                if (_traceStackError && _viewStackError)
                    _viewer(stack);
            }
            if (_onWrite != null)
            {
                _onWrite(err);
                if (_traceStackError)
                    _onWrite(stack);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using pb.IO;

namespace pb
{
    public static class Trace_v1
    {
        private static TTrace_v1 _currentTrace = new TTrace_v1();

        public static TTrace_v1 CurrentTrace
        {
            get
            {
                //if (_currentTrace == null)
                //    _currentTrace = new TTrace();
                return _currentTrace;
            }
            set
            {
                WriteLine("Trace : set new CurrentTrace  to \"{0}\"", value != null ? value.GetLogFile() : "(null)");
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

    public class TTrace_v1 : MarshalByRefObject, ITrace_v1
    {
        //private static Trace _currentTrace = null;

        private int _traceLevel = 1;
        private string _traceDir = null;             // used by HtmlXmlReader class
        public event WritedEvent Writed;
        private Log_v1 _log = null;
        private bool _disableBaseLog = false;
        private bool _traceStackError = true;
        private Dictionary<string, Log_v1> _traceFiles = new Dictionary<string, Log_v1>();
        private WritedEvent _writeToTraceFiles;

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

        public int TraceLevel
        {
            get { return _traceLevel; }
            set { _traceLevel = value; }
        }

        //public string TraceDir
        //{
        //    get
        //    {
        //        if (_traceDir == null && _log != null)
        //        {
        //            string pathDef = _log.FileDefinition;
        //            if (pathDef != null)
        //                _traceDir = Path.GetDirectoryName(pathDef) + "\\Trace";
        //        }
        //        return _traceDir;
        //    }
        //    set { _traceDir = value; }
        //}

        //public bool TraceStackError { get { return _traceStackError; } set { _traceStackError = value; } }

        public WritedEvent WriteToTraceFiles { get { return _writeToTraceFiles; } set { _writeToTraceFiles = value; } }

        public void SetAsCurrentTrace()
        {
            Trace_v1.CurrentTrace = this;
        }

        public void SetLogFile(string file, LogOptions_v1 options)
        {
            if (file != null)
                _log = new Log_v1(file, options);
            else
                _log = null;
            //WriteLine("set log file to \"{0}\"", _log.File);
        }

        public string GetLogFile()
        {
            if (_log != null)
                return _log.File;
            else
                return null;
        }

        public void SetTraceDirectory(string dir)
        {
            //if (sDir != null)
            //    Message("Trace(\"{0}\");", sDir);
            //else
            //    Message("Trace(null);");
            _traceDir = dir;
            if (dir != null)
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                _traceLevel = 2;
            }
            else
                _traceLevel = 1;
        }

        public void DisableBaseLog()
        {
            _disableBaseLog = true;
        }

        public void EnableBaseLog()
        {
            _disableBaseLog = false;
        }

        public void AddTraceFile(string file, LogOptions_v1 options = LogOptions_v1.None)
        {
            if (_traceFiles.ContainsKey(file))
                throw new PBException("error trace file already exist : \"{0}\"", file);
            Log_v1 log = new Log_v1(file, options);
            _traceFiles.Add(file, log);
            //Writed += new WritedEvent(log.Write);
            _writeToTraceFiles += log.Write;
        }

        public void RemoveTraceFile(string file)
        {
            if (!_traceFiles.ContainsKey(file))
                throw new PBException("error unknow trace file : \"{0}\"", file);
            Log_v1 log = _traceFiles[file];
            _traceFiles.Remove(file);
            //Writed -= new WritedEvent(log.Write);
            _writeToTraceFiles -= log.Write;
        }

        public void Write(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Write(msg);
        }

        public void Write(string msg)
        {
            if (!_disableBaseLog)
            {
                if (_log != null)
                    _log.Write(msg);
                EventWrited(msg);
            }
            if (_writeToTraceFiles != null)
                _writeToTraceFiles(msg);
        }

        private void EventWrited(string msg)
        {
            if (Writed != null)
                Writed(msg);
        }

        public void WriteLine()
        {
            //WriteLine("");
            Write("\r\n");
        }

        public void WriteLine(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            //if (_log != null && !_disableBaseLog)
            //    _log.WriteLine(msg);
            //EventWritedLine(msg);
            Write(msg + "\r\n");
        }

        //private void EventWritedLine(string msg)
        //{
        //    if (Writed != null)
        //        Writed(msg + "\r\n");
        //}

        public void WriteError(Exception ex)
        {
            string err = string.Format("{0:dd/MM/yyyy HH:mm:ss} ", DateTime.Now) + Error.GetErrorMessage(ex, false, true);
            string stack = "----------------------\r\n" + Error.GetErrorStackTrace(ex);
            if (_log != null && !_disableBaseLog)
            {
                _log.WriteLine(err);
                _log.WriteLine(stack);
            }
            //EventWritedLine(err);
            err = err + "\r\n";
            EventWrited(err);
            if (_writeToTraceFiles != null)
                _writeToTraceFiles(err);

            if (_traceStackError)
            {
                //EventWritedLine(stack);
                stack = stack + "\r\n";
                EventWrited(stack);
                if (_writeToTraceFiles != null)
                    _writeToTraceFiles(stack);
            }
        }
    }

    public class Log_v1
    {
        private string _file = null;
        private string _fileDefinition = null;     // c:\pib\dev_data\exe\runsource\download\log\_log.txt
        private bool _logToConsole = false;
        public event WritedEvent Writed;

        public Log_v1(string file, LogOptions_v1 options = LogOptions_v1.None)
        {
            if ((options & LogOptions_v1.IndexedFile) == LogOptions_v1.IndexedFile)
            {
                _fileDefinition = file;
            }
            else if ((options & LogOptions_v1.LogToConsole) == LogOptions_v1.LogToConsole)
            {
                _logToConsole = true;
            }
            else
            {
                //if (!Path.IsPathRooted(file))
                //    //file = Path.Combine(Directory.GetCurrentDirectory(), file);
                //    file = Path.Combine(zapp.GetAppDirectory(), file);
                //_file = file;
                _file = file.zRootPath(zapp.GetAppDirectory());
                if ((options & LogOptions_v1.RazLogFile) == LogOptions_v1.RazLogFile && System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }
        }

        public string File { get { return _file; } }
        public string FileDefinition { get { return _fileDefinition; } }

        //public void SetPath(string path)
        //{
        //    _path = path;
        //    if (File.Exists(path))
        //        File.Delete(path);
        //}

        //public void SetIndexedPath(string path)
        //{
        //    if (string.Compare(path, _pathDefinition, true) == 0) return;
        //    _pathDefinition = path;
        //    _path = null;
        //}

        private bool _writeInProgress = false;
        //private StreamWriter Open()
        private TextWriter Open()
        {
            if (_logToConsole)
            {
                return Console.Out;
            }

            if (_file == null)
            {
                if (_fileDefinition == null)
                    return null;
                //_file = zfile.GetNewIndexedFileName(_fileDefinition);
                //string sDir = System.IO.Path.GetDirectoryName(_file);
                //if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
                string dir = Path.GetDirectoryName(_fileDefinition);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                _file = zfile.GetNewIndexedFileName(dir, Path.GetFileName(_fileDefinition));
            }

            FileStream fs = null;
            while (_writeInProgress)
            {
                Thread.Sleep(10);
            }
            _writeInProgress = true;
            DateTime dt = DateTime.Now;
            while (true)
            {
                try
                {
                    fs = new FileStream(_file, FileMode.Append, FileAccess.Write, FileShare.Read);
                    break;
                }
                catch // (Exception ex)
                {
                    if (DateTime.Now.Subtract(dt).TotalSeconds > 1)
                        throw;
                    Thread.Sleep(20);
                }
            }

            return new StreamWriter(fs, Encoding.Default);
        }

        private void Close(TextWriter tw)
        {
            if (!_logToConsole)
            {
                if (tw != null) tw.Close();
                _writeInProgress = false;
            }
        }

        public void Write(string sMsg)
        {
            Write(sMsg, new object[0]);
        }

        public void Write(string sMsg, params object[] prm)
        {
            //bool bCloseLog = false; if (gswLog == null) bCloseLog = true;
            _Write(sMsg, prm);
            //if (bCloseLog) CloseLog();
        }

        public void WriteLine()
        {
            Write("\r\n");
        }

        public void WriteLine(string sMsg, params object[] prm)
        {
            Write(sMsg + "\r\n", prm);
        }

        public void _Write(string sMsg, params object[] prm)
        {
            if (prm.Length != 0) sMsg = string.Format(sMsg, prm);
            //while (_writeInProgress)
            //{
            //    Thread.Sleep(10);
            //}
            //_writeInProgress = true;
            //StreamWriter sw = null;
            TextWriter tw = null;
            try
            {
                //sw = Open();
                tw = Open();
                if (tw != null)
                {
                    tw.Write(sMsg);
                    tw.Flush();
                }
            }
            finally
            {
                //_writeInProgress = false;
                //if (tw != null) tw.Close();
                Close(tw);
            }
            if (Writed != null)
                Writed(sMsg);
        }
    }
}

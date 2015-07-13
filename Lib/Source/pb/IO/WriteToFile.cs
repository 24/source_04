using System;
using System.IO;
using System.Text;
using System.Threading;

namespace pb.IO
{
    public abstract class WriteToFileBase : IDisposable
    {
        protected Func<string> _generateFile = null;
        protected string _file = null;
        protected bool _appendToFile = false;
        protected Encoding _encoding = null;
        protected bool _fileGenerated = false;

        public WriteToFileBase(string file, Encoding encoding = null, bool appendToFile = false)
        {
            _file = file.zRootPath(zapp.GetAppDirectory());
            _encoding = encoding;
            if (_encoding == null)
                _encoding = Encoding.UTF8;
            _appendToFile = appendToFile;
            //if (!append)
            //    File.Delete(_file);
        }

        public WriteToFileBase(Func<string> generateFile, Encoding encoding = null, bool appendToFile = false)
        {
            _generateFile = generateFile;
            _encoding = encoding;
            if (_encoding == null)
                _encoding = Encoding.UTF8;
            _appendToFile = appendToFile;
        }

        public void Dispose()
        {
            Close();
        }

        public string File { get { return _file; } }

        public virtual void Close()
        {
        }

        public abstract void Write(string msg);

        public virtual void Write(string msg, params object[] prm)
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

        protected void GenerateFile()
        {
            if (!_fileGenerated)
            {
                if (_generateFile != null)
                    _file = _generateFile();
                else if (!_appendToFile)
                    System.IO.File.Delete(_file);
                string directory = Path.GetDirectoryName(_file);
                if (directory != "" && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                _fileGenerated = true;
            }
        }
    }

    public partial class WriteToFile : WriteToFileBase
    {
        protected bool _writeInProgress = false;

        public WriteToFile(string file, Encoding encoding = null, bool appendToFile = false) : base(file, encoding, appendToFile)
        {
        }

        public WriteToFile(Func<string> generateFile, Encoding encoding = null, bool appendToFile = false) : base(generateFile, encoding, appendToFile)
        {
        }

        public override void Write(string msg)
        {
            GenerateFile();
            WaitNoWriteInProgress();

            FileStream fs = null;
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

            StreamWriter sw = new StreamWriter(fs, _encoding);
            try
            {
                sw.Write(msg);
                sw.Flush();
            }
            finally
            {
                sw.Close();
                _writeInProgress = false;
            }
        }

        protected void WaitNoWriteInProgress()
        {
            while (_writeInProgress)
            {
                Thread.Sleep(10);
            }
            _writeInProgress = true;
        }
    }

    public class WriteToFile2 : WriteToFileBase
    {
        private StreamWriter _sw = null;
        private bool _autoFlush = false;

        public WriteToFile2(string file, Encoding encoding = null, bool appendToFile = false, bool autoFlush = false)
            : base(file, encoding, appendToFile)
        {
            _autoFlush = autoFlush;
        }

        public WriteToFile2(Func<string> generateFile, Encoding encoding = null, bool appendToFile = false, bool autoFlush = false)
            : base(generateFile, encoding, appendToFile)
        {
            _autoFlush = autoFlush;
        }

        public void Open()
        {
            if (_sw == null)
            {
                GenerateFile();
                FileStream fs = new FileStream(_file, FileMode.Append, FileAccess.Write, FileShare.Read);
                _sw = new StreamWriter(fs, _encoding);
            }
        }

        public override void Close()
        {
            if (_sw != null)
            {
                _sw.Close();
                _sw = null;
            }
        }

        public override void Write(string msg)
        {
            Open();
            _sw.Write(msg);
            if (_autoFlush)
                _sw.Flush();
        }
    }
}

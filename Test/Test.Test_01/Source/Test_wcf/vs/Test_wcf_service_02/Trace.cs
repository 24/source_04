using System;
using System.IO;
using System.Text;

namespace Test_wcf_service
{
    public static class Trace
    {
        public static Action<string> OnWrite = null;

        public static void Write(string msg)
        {
            OnWrite(msg);
        }

        public static void Write(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Write(msg);
        }

        public static void WriteLine()
        {
            Write("\r\n");
        }

        public static void WriteLine(string msg)
        {
            Write(msg);
            WriteLine();
        }

        public static void WriteLine(string msg, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Write(msg);
            WriteLine();
        }
    }

    public class WriteToFile
    {
        private string _file = null;
        private Encoding _encoding = null;
        private bool _directoryCreated = false;

        public WriteToFile(string file, Encoding encoding = null)
        {
            _file = file;
            _encoding = encoding;
            if (_encoding == null)
                _encoding = Encoding.UTF8;
        }

        private void CreateDirectory()
        {
            if (!_directoryCreated)
            {
                string directory = Path.GetDirectoryName(_file);
                if (directory != "" && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                _directoryCreated = true;
            }
        }

        public void Write(string text)
        {
            CreateDirectory();
            FileStream fs = new FileStream(_file, FileMode.Append, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, _encoding);
            try
            {
                sw.Write(text);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}

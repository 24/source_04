using System;
using System.Runtime.Serialization;

namespace pb
{
    [Serializable]
    public class PBException : Exception
    {
        // constructor to deserialize PBException
        protected PBException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public PBException(string message) : base(message) { }
        public PBException(string message, params object[] prm) : base(string.Format(message, prm)) { }
        public PBException(Exception innerException, string message) : base(message, innerException) { }
        public PBException(Exception innerException, string message, params object[] prm) : base(string.Format(message, prm), innerException) { }
    }

    public class PBFileException : Exception
    {
        private string _file = null;
        private int? _line = null;
        private int? _column = null;

        public PBFileException(string message, string file, int? line = null, int? column = null) : base(message)
        {
            _file = file;
            _line = line;
            _column = column;
        }

        public PBFileException(Exception innerException, string message, string file, int? line = null, int? column = null) : base(message, innerException)
        {
            _file = file;
            _line = line;
            _column = column;
        }

        public string File { get { return _file; } }
        public int? Line { get { return _line; } }
        public int? Column { get { return _column; } }
    }
}

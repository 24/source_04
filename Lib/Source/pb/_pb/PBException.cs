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
}

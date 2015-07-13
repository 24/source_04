using System;

namespace pb.old
{
    public class PB_Util_Exception : Exception
    {
        public PB_Util_Exception(string sMessage) : base(sMessage) { }
        public PB_Util_Exception(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public PB_Util_Exception(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public PB_Util_Exception(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
}

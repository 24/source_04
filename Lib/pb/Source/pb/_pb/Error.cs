//#define OleDb
//#define SqlClient

using System;
#if OleDb
using System.Data.OleDb;
#endif
#if SqlClient
using System.Data.SqlClient;
#endif

namespace pb
{
    //public class PB_Util_InnerException : Exception
    //{
    //    public PB_Util_InnerException(string sMessage) : base(sMessage) { }
    //    public PB_Util_InnerException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
    //    public PB_Util_InnerException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
    //    public PB_Util_InnerException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    //}

	public class Error
	{
        //private static string[] gsException =
        //        {
        //            "BA_Bdd.cBABdd+BaBddException",
        //            "BA_IN.cCheckUpLiv+CheckUpLivException",
        //            "BA_IN.cFtp+FtpException",
        //            "BA_IN.cHoro+HoroException",
        //            "BA_IN.cLockFuturSchedule+LockFuturScheduleException",
        //            "BA_IN.cMain+MainException",
        //            "BA_IN.cProcess+ProcessException",
        //            "BA_IN.cScheduler+SchedulerException",
        //            "Pkt_AddZip.cAddZip+AddZipException",
        //            "Pkt_AddZip.cAddUnzip+AddUnzipException",
        //            "Pkt_Ado.cAdo+AdoException",
        //            "Pkt_Blat.cBlat+BlatException",
        //            "Pkt_Ftp.FtpClient.FtpException",
        //            "Pkt_Util.cu+IniException",
        //            "System.Web.HttpException",
        //            "Xceed.Ftp"
        //        };

        //private static string[] gsWarning =
        //        {
        //            "BA_IN.cLockFuturSchedule+LockFuturScheduleException",
        //            "Xceed.Ftp"
        //        };

		public static void ErrorWrite(Exception ex)
		{
			Console.WriteLine(GetErrorMessage(ex));
		}

        public static Exception FilterException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                //if (ex is PB_Util_InnerException || ex is System.Reflection.TargetInvocationException)
                if (ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;
                else
                    break;
            }
            return ex;
        }

        public static string GetErrorMessage(Exception ex)
		{
			return GetErrorMessage(ex, true, true);
        }

        public static string GetErrorMessage(Exception ex, bool bStack, bool bAddClassName)
		{
            ex = FilterException(ex);

            //string sErr = _GetErrorMessage(ex, "Error", bAddClassName);
            string sErr = _GetErrorMessage(ex, null, bAddClassName);
            Exception exInner = ex.InnerException;
            //while (exInner != null) { sErr += "\r\n" + _GetErrorMessage(exInner, "Inner", bAddClassName); exInner = exInner.InnerException; }
            while (exInner != null)
            {
                //sErr = _GetErrorMessage(exInner, "Inner", bAddClassName) + "\r\n" + sErr;
                sErr = _GetErrorMessage(exInner, null, bAddClassName) + "\r\n" + sErr;
                exInner = exInner.InnerException;
            }
            sErr = "Error : " + sErr;
            //if (bStack && !InArrayStringBeginWith(ref gsException, ex.GetType().FullName))
            if (bStack)
                sErr += "\r\n----------------------\r\n" + GetErrorStackTrace(ex);
			return sErr;
        }

        private static string _GetErrorMessage(Exception ex, string sErrHeader, bool bAddClassName)
		{
			//string sErr;
			//OleDbException exOleDb;
			//SqlException exSql;
			//SqlError errSql;
            string sErr = "Error";

#if OleDb
            //if (exOleDb != null)
            if (ex is OleDbException)
			{
                OleDbException exOleDb = ex as OleDbException;
                for (int i = exOleDb.Errors.Count - 1; i >= 0; i--)
				{
                    //sErr += string.Format("{0} : {1} (Msg {2})\r\n", sErrHeader, exOleDb.Errors[i].Message, exOleDb.Errors[i].NativeError);
                    sErr += string.Format("{0}{1} (Msg {2})\r\n", sErrHeader, exOleDb.Errors[i].Message, exOleDb.Errors[i].NativeError);
                }
			    return sErr;
			}
#endif
#if SqlClient
            //else if (exSql != null)
            if (ex is SqlException)
			{
                SqlException exSql = ex as SqlException;
                for (int i = 0; i < exSql.Errors.Count; i++)
				{
					SqlError errSql = exSql.Errors[i];
                    //sErr += string.Format("{0} : Msg {1}, State {2}, Procedure {3}, Line {4} : {5}\r\n", sErrHeader, errSql.Number, errSql.State, errSql.Procedure,
                    //    errSql.LineNumber, errSql.Message);
                    sErr += string.Format("{0}Msg {1}, State {2}, Procedure {3}, Line {4} : {5}\r\n", sErrHeader, errSql.Number, errSql.State, errSql.Procedure,
                        errSql.LineNumber, errSql.Message);
                }
			    return sErr;
			}
#endif
            //else
			//{
                //sErr = sErrHeader;
                //if (bAddClassName ) sErr += " " + ex.GetType().FullName;
                //sErr += " : " + ex.Message;
                sErr = "";
                if (sErrHeader != null) sErr = sErrHeader;
                sErr += ex.Message;
                if (bAddClassName) sErr += " (" + ex.GetType().FullName + ")";
            //}
			return sErr;
        }

        public static string GetErrorStackTrace(Exception ex)
        {
            ex = FilterException(ex);
            string sStackTrace = ex.StackTrace;
            Exception exInner = ex.InnerException;
            while (exInner != null)
            {
                //sStackTrace += "\r\n----- Inner ----------\r\n" + exInner.StackTrace;
                sStackTrace = exInner.StackTrace + "\r\n----------------------\r\n" + sStackTrace;
                exInner = exInner.InnerException;
            }
            return sStackTrace;
        }

        public static Exception GetFirstException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }

        //public static bool IsWarning(Exception ex)
        //{
        //    return InArrayStringBeginWith(ref gsWarning, ex.GetType().FullName);
        //}

        //private static bool InArrayStringBeginWith(ref string[] sArray, string sSearch)
        //{
        //    bool ret;
        //    int i;

        //    ret = false;
        //    if (sArray != null)
        //    {
        //        for (i = 0; i < sArray.Length; i++)
        //            if (sSearch.StartsWith(sArray[i])) { ret = true; break; }
        //    }
        //    return ret;
        //}
	}
}

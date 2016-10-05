using System;
using System.IO;

namespace pb.Web
{
    partial class Http_v2
    {
        public string LoadText()
        {
            if (__trace)
                pb.Trace.WriteLine("Http.LoadText()");
            string text = null;

            Action action = () =>
            {
                if (_resultContentType != null && (_resultContentType.StartsWith("text") || _resultContentType == "application/json"))
                {
                    using (StreamReader streamReader = new StreamReader(GetResponseStream(), GetResponseStreamEncoding()))
                    {
                        text = streamReader.ReadToEnd();
                    }
                    SetRequestDuration(DateTime.Now - RequestTime);
                    Export(text);
                }
            };

            Try(action);

            return text;
        }

        //private StreamReader _streamReader = null;

        //public override void Dispose()
        //{
        //    Close();
        //}

        //public override void Close()
        //{
        //    if (_streamReader != null)
        //    {
        //        _streamReader.Close();
        //        _streamReader = null;
        //    }
        //    base.Close();
        //}

        //public string LoadText_v1()
        //{
        //    try
        //    {
        //        if (__trace)
        //            pb.Trace.WriteLine("Http.LoadText()");
        //        Open();
        //        string text = null;
        //        if (_resultContentType != null && (_resultContentType.StartsWith("text") || _resultContentType == "application/json"))
        //        {
        //            text = _LoadText();
        //            Export(text);
        //        }
        //        return text;
        //    }
        //    finally
        //    {
        //        Close();
        //    }
        //}

        //private string _LoadText()
        //{
        //    DateTime dtFirstCatch = DateTime.Now;
        //    while (true)
        //    {
        //        try
        //        {
        //            //CreateStreamReader();
        //            //string _text = _streamReader.ReadToEnd();

        //            string text = null;
        //            using (StreamReader streamReader = new StreamReader(GetResponseStream(), GetResponseStreamEncoding()))
        //            {
        //                text = streamReader.ReadToEnd();
        //            }
        //            SetRequestDuration(DateTime.Now - RequestTime);
        //            return text;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (_loadRetryTimeout == 0)
        //                throw;
        //            if (ex is IOException)
        //                throw;
        //            if (ex is ThreadAbortException)
        //                throw;
        //            // from HttpManager.Load()
        //            if (ex is WebException)
        //            {
        //                WebException wex = (WebException)ex;
        //                // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
        //                // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
        //                if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
        //                    throw;
        //            }
        //            if (ex is ProtocolViolationException)
        //                throw;

        //            if (dtFirstCatch.Ticks == 0)
        //            {
        //                dtFirstCatch = DateTime.Now;
        //            }
        //            else if (_loadRetryTimeout != -1)
        //            {
        //                dtFirstCatch = DateTime.Now;
        //                TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
        //                if (ts.Seconds > _loadRetryTimeout)
        //                    throw;
        //            }

        //            Trace.WriteLine(1, "Error : \"{0}\" ({1})", ex.Message, ex.GetType().ToString());

        //            //if (HttpRetry != null && !HttpRetry(ex))
        //            //    throw;

        //            Close();
        //            Open();
        //        }
        //    }
        //}

        //private void CreateStreamReader()
        //{
        //    //Trace.CurrentTrace.WriteLine("Http : encoding \"{0}\"", encoding.zToStringOrNull());
        //    _streamReader = new StreamReader(GetResponseStream(), GetEncoding());
        //}
    }
}

using pb.IO;
using System;
using System.IO;

namespace pb.Web.Http
{
    partial class Http_v2
    {
        public bool LoadToFile(string file, bool exportRequest = false)
        {
            bool ret = false;
            //FileStream fs = null;

            if (__trace)
                pb.Trace.WriteLine("Http.LoadToFile()");

            zfile.CreateFileDirectory(file);
            //fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);

            Action action = () =>
            {
                StreamTransfer streamTransfer = new StreamTransfer();
                streamTransfer.SourceLength = _resultContentLength;
                ///////////////////streamTransfer.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(StreamTransferProgressChange);
                //using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (FileStream fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read))
                    ret = streamTransfer.Transfer(GetResponseStream(), fs);
                SetRequestDuration(DateTime.Now - RequestTime);
                //if (exportRequest)
                //    ExportRequest(file);
            };

            Action retry = () =>
            {
                //FileStream fs2 = fs;
                //fs = null;
                //fs2.Close();
                //fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            };

            Action final = () =>
            {
                //if (fs != null)
                //    fs.Close();
                if (exportRequest)
                    _ExportRequest(file);
            };

            Try(action, retry, final);

            return ret;
        }

        //public bool LoadToFile_v1(string file, bool exportRequest = false)
        //{
        //    bool ret = false;
        //    FileStream fs = null;
        //    try
        //    {
        //        if (__trace)
        //            pb.Trace.WriteLine("Http.LoadToFile()");
        //        Open();
        //        zfile.CreateFileDirectory(file);
        //        fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);

        //        DateTime dtFirstCatch = DateTime.Now;
        //        while (true)
        //        {
        //            try
        //            {
        //                //if (_abortTransfer)
        //                //{
        //                //    ret = false;
        //                //    break;
        //                //}
        //                StreamTransfer streamTransfer = new StreamTransfer();
        //                streamTransfer.SourceLength = _resultContentLength;
        //                ///////////////////streamTransfer.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(StreamTransferProgressChange);
        //                ret = streamTransfer.Transfer(GetResponseStream(), fs);
        //                SetRequestDuration(DateTime.Now - RequestTime);
        //                if (exportRequest)
        //                    ExportRequest(file);
        //                break;
        //            }
        //            catch (Exception ex)
        //            {
        //                if (_loadRetryTimeout == 0)
        //                    throw;
        //                if (ex is IOException)
        //                    throw;
        //                if (ex is ThreadAbortException)
        //                    throw;
        //                // from HttpManager.Load()
        //                if (ex is WebException)
        //                {
        //                    WebException wex = (WebException)ex;
        //                    // WebExceptionStatus : ConnectFailure, PipelineFailure, ProtocolError, ReceiveFailure, SendFailure, ServerProtocolViolation, Timeout, UnknownError
        //                    // $$pb modif le 27/01/2015 WebExceptionStatus.NameResolutionFailure  ex : "The remote name could not be resolved: 'pixhost.me'"
        //                    if (wex.Status == WebExceptionStatus.ProtocolError || wex.Status == WebExceptionStatus.NameResolutionFailure)
        //                        throw;
        //                }
        //                if (ex is ProtocolViolationException)
        //                    throw;
        //                if (dtFirstCatch.Ticks == 0)
        //                {
        //                    dtFirstCatch = DateTime.Now;
        //                }
        //                else if (_loadRetryTimeout != -1)
        //                {
        //                    dtFirstCatch = DateTime.Now;
        //                    TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
        //                    if (ts.Seconds > _loadRetryTimeout)
        //                        throw;
        //                }
        //                //if (HttpRetry != null && !HttpRetry(ex))
        //                //    throw;

        //                // if not throw trace exception

        //                Close();
        //                Open();
        //                FileStream fs2 = fs;
        //                fs = null;
        //                fs2.Close();
        //                fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        //_abortTransfer = false;
        //        if (fs != null)
        //            fs.Close();
        //        Close();
        //    }
        //    return ret;
        //}
    }
}

using System;
using System.Drawing;

namespace pb.Web.Http
{
    partial class Http_v2
    {
        public Image LoadImage()
        {
            if (__trace)
                pb.Trace.WriteLine("Http.LoadImage()");
            Image image = null;

            Action action = () =>
            {
                image = Image.FromStream(GetResponseStream());
                SetRequestDuration(DateTime.Now - RequestTime);
            };

            Try(action);

            return image;
        }

        //public Image LoadImage_v1()
        //{
        //    try
        //    {
        //        if (__trace)
        //            pb.Trace.WriteLine("Http.LoadImage()");
        //        Image image = null;
        //        Open();
        //        DateTime dtFirstCatch = DateTime.Now;
        //        while (true)
        //        {
        //            try
        //            {
        //                //if (_abortTransfer)
        //                //{
        //                //    break;
        //                //}
        //                image = Image.FromStream(GetResponseStream());
        //                SetRequestDuration(DateTime.Now - RequestTime);
        //                break;
        //            }
        //            catch (Exception ex)
        //            {
        //                //if (ex is IOException)
        //                //    throw;
        //                if (!ex.GetType().FullName.StartsWith("System.Net."))
        //                    throw;
        //                if (ex is ThreadAbortException)
        //                    throw;
        //                if (_loadRetryTimeout == 0)
        //                    throw;

        //                if (dtFirstCatch.Ticks == 0)
        //                {
        //                    dtFirstCatch = DateTime.Now;
        //                }
        //                else if (_loadRetryTimeout != -1)
        //                {
        //                    TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
        //                    if (ts.Seconds > _loadRetryTimeout)
        //                        throw;
        //                }
        //                //if (HttpRetry != null && !HttpRetry(ex))
        //                //    throw;

        //                Close();
        //                Open();
        //            }
        //        }
        //        return image;
        //    }
        //    finally
        //    {
        //        //_abortTransfer = false;
        //        Close();
        //    }
        //}
    }
}

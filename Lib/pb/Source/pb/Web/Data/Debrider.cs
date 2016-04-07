using System;
using System.Linq;

namespace pb.Web
{
    //public abstract class DebridLinkInfo
    //{
    //    public string Link;
    //    public string DebridedLink;
    //    public abstract string GetErrorMessage();
    //}

    public abstract class Debrider
    {
        protected static bool __trace = false;

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        public virtual string DebridLink(string[] links)
        {
            var q = (from l in links select new { link = l, rate = _GetLinkRate(l) }).OrderBy(link => link.rate).Select(link => link.link);
            foreach (string link in q)
            {
                try
                {
                    string downloadLink = DebridLink(link);
                    if (downloadLink != null)
                        return downloadLink;
                }
                catch (Exception exception)
                {
                    pb.Trace.WriteLine("error in Debrider.DebridLink(string[] links) : link \"{0}\"", link);
                    pb.Trace.WriteLine(exception.Message);
                    //pb.Trace.WriteLine(exception.StackTrace);
                }
            }
            return null;
        }

        private int _GetLinkRate(string link)
        {
            try
            {
                return GetLinkRate(link);
            }
            catch (Exception exception)
            {
                pb.Trace.WriteLine("error in Debrider._GetLinkRate() : link \"{0}\"", link);
                pb.Trace.WriteLine(exception.Message);
                return 999;
            }
        }

        public abstract string DebridLink(string link);
        public abstract int GetLinkRate(string link);
    }
}

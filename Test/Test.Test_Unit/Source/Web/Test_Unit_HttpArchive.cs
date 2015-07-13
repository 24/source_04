using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using pb.Data.Mongo;
using pb.Web;

namespace Test.Test_Unit.Web
{
    public static class Test_Unit_HttpArchive
    {
        public static void Test_01(string file)
        {
            Trace.WriteLine("file \"{0}\"", file);
            HttpArchive har = new HttpArchive(file);
            Trace.WriteLine("Version : {0}", har.Version);
            Trace.WriteLine("Creator : {0}", har.Creator);
            Trace.WriteLine("Browser : {0}", har.Browser);
            Trace.WriteLine("Pages : ");
            foreach (var page in har.Pages)
            {
                Trace.WriteLine(page.zToJson());
            }
            Trace.WriteLine("Entries : ");
            foreach (var entry in har.Entries)
            {
                Trace.WriteLine(entry.zToJson());
            }
        }
    }
}

using System;
using pb;
using pb.Data.Mongo;

namespace Download.Print.Telechargementz
{
    public static class Telechargementz_f
    {
        public static void Test_01()
        {
            string url = "";
            Trace.WriteLine(Telechargementz_LoadPostDetail.CurrentLoadPostDetail.LoadDocument(url).Document.zToJson());
        }
    }
}

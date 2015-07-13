using System;
using pb;
using pb.Compiler;

namespace Test_Web
{
    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }
    }
}

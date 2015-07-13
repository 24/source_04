using System;
using System.Collections.Generic;
using System.Text;
using pb;
using pb.Compiler;

namespace Test_cpp
{
    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _wr = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Test_cpp_01()
        {
            //_tr.WriteLine("Test_cpp_01 : \"{0}\"", Test_cpp.Test01.GetText());
        }

        public static void Test_cs_01()
        {
            //_tr.WriteLine("Test_cs_01 : \"{0}\"", Test_cs.Test01.GetText());
        }
    }
}

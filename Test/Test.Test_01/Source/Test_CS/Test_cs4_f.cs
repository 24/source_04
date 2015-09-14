using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb;
using pb.Compiler;

namespace Test_cs4
{
    static partial class w
    {
        //private static ITrace _tr = Trace.CurrentTrace;
        //private static RunSource _wr = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            Trace.WriteLine("Test_01");
        }

        public static void Test_Lambda_01()
        {
            Trace.WriteLine("Test_Lambda_01");
            Func<int, bool> myFunc = x => x == 5;
            //Expression<Func>
        }

        public static void Test_Lambda_02()
        {
            Trace.WriteLine("Test_Lambda_02 : test des expressions lambda");
            // Predicate<int>  ==> delegate bool Predicate<in T>(T obj)              ==> delegate bool Function(int i)
            // Func<bool>      ==> delegate TResult Func<out TResult>()              ==> delegate bool Function()
            // Func<int, bool> ==> delegate TResult Func<in T, out TResult>(T arg)
            Lambda_01(10, i => i > 100);
        }

        public static bool Lambda_01(int i, Predicate<int> f)
        {
            return f(i);
        }
    }
}

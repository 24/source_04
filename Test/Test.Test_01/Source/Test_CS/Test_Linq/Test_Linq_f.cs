using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Linq;

namespace Test.Test_CS.Test_Linq
{
    public static class Test_Linq_f
    {
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

        public static void Test_linq_01()
        {
            Trace.WriteLine("Test_linq_01");

            string[] list = new string[] { "aaaa", "bbbb", "cccc", "dddd" };
            list.Where(Where_01).zView();
            //Func<int, bool> myFunc = x => x == 5;
            //Expression<Func>
        }

        public static bool Where_01(string s)
        {
            if (s == "cccc")
                return false;
            return true;
        }

        public static void Test_WhereSelect_01()
        {
            Trace.WriteLine("Test_WhereSelect_01");

            string[] list = new string[] { "aaaa", "bbbb", "cccc", "dddd" };
            list.zWhereSelect(Filter_01).zView();
            //Func<int, bool> myFunc = x => x == 5;
            //Expression<Func>
        }

        public static GClass2<string, int> Filter_01(string s)
        {
            if (s == "cccc")
                return null;
            return new GClass2<string,int>(s, 123);
        }


    }
}

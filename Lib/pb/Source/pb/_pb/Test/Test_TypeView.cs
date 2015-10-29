using System;

namespace pb.Test
{
    public static class Test_TypeView
    {
        public static void Test_TypeView_01()
        {
            Trace.WriteLine("Test_TypeView_01");
        }

        public static void Test_TypeView_02()
        {
            Trace.WriteLine("Test_TypeView_02");
            TypeView typeView = new TypeView(new Test_01 { Name = "toto", Number = 123 });
        }
    }

    public class Test_01
    {
        public string Name;
        public int Number;
    }
}

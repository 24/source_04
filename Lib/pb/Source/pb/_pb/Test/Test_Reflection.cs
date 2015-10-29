using System;

namespace pb.Test
{
    public static class Test_Reflection
    {
        public static void Test_Reflection_01()
        {
            Trace.WriteLine("Test_Reflection_01");
        }

        public static void Test_Reflection_02()
        {
            Trace.WriteLine("Test_Reflection_01");
            Type type = typeof(Test_01);
            type.GetMembers();
            //type.InvokeMember("", System.Reflection.BindingFlags.Public, null, )
        }
    }
}

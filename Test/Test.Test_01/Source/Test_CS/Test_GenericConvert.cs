using System;
using pb;

namespace Test.Test_CS
{
    public static class Test_GenericConvert
    {
        public static void Test_01(string text)
        {
            //string text = "123";
            int i = Convert<int>(text);
            Trace.WriteLine("Convert<int>(\"{0}\") : {1}", text, i);
            long l = Convert<long>(text);
            Trace.WriteLine("Convert<long>(\"{0}\") : {1}", text, l);
        }

        public static T Convert<T>(string text)
        {
            if (typeof(T) == typeof(int))
            {
                int value = int.Parse(text);
                //T value = __refvalue(__makeref(a), T);
                return __refvalue(__makeref(value), T);
            }
            else if (typeof(T) == typeof(long))
            {
                long value = long.Parse(text);
                //T value = __refvalue(__makeref(a), T);
                return __refvalue(__makeref(value), T);
            }

            throw new NotImplementedException();
        }
    }
}

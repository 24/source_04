using System;

namespace Test_Program_01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test_Program_01");

            //Console.WriteLine("call Test_cs.Test01.GetText()");
            //string s = Test_cs.Test01.GetText();
            //Console.WriteLine("result : \"{0}\"", s);

            Console.WriteLine("call Test_cpp.Test01.GetText()");
            string s = Test_cpp.Test01.GetText();
            Console.WriteLine("result : \"{0}\"", s);
        }
    }
}

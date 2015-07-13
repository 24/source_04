using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_JScript_01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test_JScript_01");
            string s = Test_JScript_01.test.test_01();
            Console.WriteLine("call jscript Test_JScript_01.test.test_01() : \"{0}\"", s);
        }
    }
}

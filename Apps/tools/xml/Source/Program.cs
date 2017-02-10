using System;

namespace xml
{
    class Program
    {
        static void Main(string[] args)
        {
            Test_01();
        }

        static void Test_01()
        {
            Console.Write("toto");
            Console.ReadKey();
            Console.Write("\rtata");
            Console.ReadKey();
            Console.Write("\rzo");
        }
    }
}

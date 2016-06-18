using System;

namespace test_01
{
    class test_01
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test_01");
			Test_PreprocessorSymbol();
        }

        private static void Test_PreprocessorSymbol()
        {
#if TOTO
            Console.WriteLine("TOTO");
#else
            Console.WriteLine("not TOTO");
#endif
#if TATA
            Console.WriteLine("TATA");
#else
            Console.WriteLine("not TATA");
#endif
        }
    }
}

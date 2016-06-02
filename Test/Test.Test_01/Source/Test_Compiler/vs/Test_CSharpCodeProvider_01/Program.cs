using System;

namespace Test_CSharpCodeProvider_01
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test_01();
            }
            catch(Exception ex)
            {
                Console.WriteLine("error : {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void Test_01()
        {
            Compiler_01.Compile(
                finalOutputAssembly: @"test_01\test_01.exe",
                sources: new string[] { @"test_01\test_01.cs" },
                referencedAssemblies: new string[] { },
                compilerVersion: "v4.0",
                //compilerVersion: "v4.5",   // error : Compiler executable file csc.exe cannot be found.
                compilerOptions: "/target:exe"
                );
        }
    }
}

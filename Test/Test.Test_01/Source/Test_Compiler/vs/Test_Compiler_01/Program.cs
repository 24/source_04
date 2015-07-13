using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_Compiler_01
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("no source file");
                return;
            }

            //string test_cs_dll = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Test\Test_cs\vs\Test_cs\bin\Debug\Test_cs.dll";
            //string test_cpp_dll = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Test\Test_cpp\Debug\Test_cpp.dll";
            //string test_cpp_dll = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Test\Test_cpp2\Debug\Test_cpp.dll";
            //string test_cpp_dll = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Test\Test_cpp\vs\Test_cpp2\Debug\Test_cpp.dll";
            string test_cpp_dll = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Test\Test_cpp\vs\Test_cpp\Debug\Test_cpp.dll";

            //string[] assemblies = { test_cs_dll, test_cpp_dll };
            string[] assemblies = { test_cpp_dll };

            //System.BadImageFormatException Could not load file or assembly An attempt was made to load a program with an incorrect format

            //Console.WriteLine("compile \"{0}\" assembly \"{1}\"", args[0], assemblies);
            Compiler.CompileExecutable(args[0], assemblies);
        }
    }
}

using System;
using System.Linq;

// set target framework : .net framework 4.6
// modify global NuGet.Config to set packages folder
// Install-Package Microsoft.CodeAnalysis.CSharp

// https://github.com/dotnet/roslyn
// C# and Visual Basic - Use Roslyn to Write a Live Code Analyzer for Your API https://msdn.microsoft.com/en-us/magazine/dn879356



// console application, .net framework 4.5.2
//   default referenced assemblies :
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\Microsoft.CSharp.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Core.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Data.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Data.DataSetExtensions.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Net.Http.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Xml.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Xml.Linq.dll
//     
//     
// console application, .net framework 4.6
//   default referenced assemblies :
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\Microsoft.CSharp.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.Core.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.Data.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.Data.DataSetExtensions.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.Net.Http.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.Xml.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\System.Xml.Linq.dll
//     
//     
// console application, .net framework 4.6.1
//   default referenced assemblies :
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\Microsoft.CSharp.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Core.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Data.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Data.DataSetExtensions.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Net.Http.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Xml.dll
//     C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Xml.Linq.dll
//     
//   Install-Package Microsoft.CodeAnalysis.CSharp 1.2.2
//     C:\pib\dev\NuGet\repository\Microsoft.CodeAnalysis.Common.1.2.2\lib\net45\Microsoft.CodeAnalysis.dll
//     C:\pib\dev\NuGet\repository\Microsoft.CodeAnalysis.CSharp.1.2.2\lib\net45\Microsoft.CodeAnalysis.CSharp.dll
//     C:\pib\dev\NuGet\repository\System.Collections.Immutable.1.1.37\lib\dotnet\System.Collections.Immutable.dll
//     C:\pib\dev\NuGet\repository\System.Reflection.Metadata.1.2.0\lib\portable-net45+win8\System.Reflection.Metadata.dll

//typeof(object).Assembly.Location : "C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll"
//typeof(Enumerable).Assembly.Location : "C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Core\v4.0_4.0.0.0__b77a5c561934e089\System.Core.dll"

// References :
//   C:\pib\drive\google\dev\project\.net\Test\Test.Test_01\Source\Test_Compiler\vs\Test_Roslyn_01\bin\Debug\Microsoft.CodeAnalysis.dll
//   C:\pib\drive\google\dev\project\.net\Test\Test.Test_01\Source\Test_Compiler\vs\Test_Roslyn_01\bin\Debug\Microsoft.CodeAnalysis.CSharp.dll
//   C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Collections.Immutable\v4.0_1.1.37.0__b03f5f7f11d50a3a\System.Collections.Immutable.dll
//   C:\pib\drive\google\dev\project\.net\Test\Test.Test_01\Source\Test_Compiler\vs\Test_Roslyn_01\bin\Debug\System.Reflection.Metadata.dll
// Analyzers :
//   C:\pib\dev\NuGet\repository\Microsoft.CodeAnalysis.Analyzers.1.1.0\analyzers\dotnet\cs\Microsoft.CodeAnalysis.Analyzers.dll
//   C:\pib\dev\NuGet\repository\Microsoft.CodeAnalysis.Analyzers.1.1.0\analyzers\dotnet\cs\Microsoft.CodeAnalysis.CSharp.Analyzers.dll


namespace Test_Roslyn_01
{
    class Program
    {
        static void Main(string[] args)
        {
            //Compile_01.Compile();
            //Compile_02.Compile();
            //Compile_03.Compile();
            //AssemblyLocation();
            //Try(Test_CSharpCompiler.Test_01);
            Try(Test_CSharpCompiler.Test_02);
        }

        public static void AssemblyLocation()
        {
            // typeof(object).Assembly.Location : "C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll"
            Console.Write("typeof(object).Assembly.Location : \"");
            Console.Write(typeof(object).Assembly.Location);
            Console.WriteLine("\"");

            // typeof(Enumerable).Assembly.Location : "C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Core\v4.0_4.0.0.0__b77a5c561934e089\System.Core.dll"
            Console.Write("typeof(Enumerable).Assembly.Location : \"");
            Console.Write(typeof(Enumerable).Assembly.Location);
            Console.WriteLine("\"");
        }

        public static void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error : {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Test_Roslyn_01
{
    public static class Test_CSharpCompiler
    {
        public static void Test_01()
        {
            CSharpCompiler compiler = new CSharpCompiler();
            compiler.AssemblyName = "test_01";
            compiler.OutputPath = @"test_01\test_01";
            compiler.LanguageVersion = LanguageVersion.CSharp6;
            compiler.OutputKind = OutputKind.ConsoleApplication;
            compiler.OptimizationLevel = OptimizationLevel.Debug;
            compiler.Platform = Platform.AnyCpu;
            compiler.GeneralDiagnosticOption = ReportDiagnostic.Default;
            compiler.WarningLevel = 4;
            compiler.SourceFiles = new string[] { @"test_01\test_01.cs" };
            compiler.Compile();
        }
    }
}

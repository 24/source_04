using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Test_Roslyn_01
{
    public static class Test_CSharpCompiler
    {
        public static void Test_01()
        {
            string frameworkDir = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\";
            CSharpCompiler compiler = new CSharpCompiler();
            compiler.AssemblyName = "test_01";
            compiler.OutputPath = @"test_01\test_01.exe";
            compiler.PdbPath = @"test_01\test_01.pdb";
            compiler.LanguageVersion = LanguageVersion.CSharp6;
            compiler.OutputKind = OutputKind.ConsoleApplication;
            compiler.OptimizationLevel = OptimizationLevel.Debug;
            compiler.Platform = Platform.AnyCpu;
            compiler.GeneralDiagnosticOption = ReportDiagnostic.Default;
            compiler.WarningLevel = 4;
            compiler.SourceFiles = new string[] { @"test_01\test_01.cs" };
            // Could not find file 'C:\pib\drive\google\dev\project\.net\Test\Test.Test_01\Source\Test_Compiler\vs\Test_Roslyn_01\bin\Debug\mscorlib.dll'
            //compiler.AssembliesFiles = new string[] { "mscorlib.dll", "System.dll" };
            compiler.AssembliesFiles = new string[] { frameworkDir + "mscorlib.dll", frameworkDir + "System.dll" };
            compiler.Compile();
        }

        public static void Test_02()
        {
            string frameworkDir = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\";
            CSharpCompiler compiler = new CSharpCompiler();
            compiler.AssemblyName = "test_form_01";
            compiler.OutputPath = @"test_form_01\test_form_01.exe";
            compiler.PdbPath = @"test_form_01\test_form_01.pdb";
            compiler.LanguageVersion = LanguageVersion.CSharp6;
            compiler.OutputKind = OutputKind.WindowsApplication;
            compiler.OptimizationLevel = OptimizationLevel.Debug;
            compiler.Platform = Platform.AnyCpu;
            compiler.GeneralDiagnosticOption = ReportDiagnostic.Default;
            compiler.WarningLevel = 4;
            compiler.Win32ResourceFile = @"test_form_01\test.res";
            compiler.SourceFiles = new string[] { @"test_form_01\Program.cs", @"test_form_01\Form1.cs", @"test_form_01\Form1.Designer.cs", @"test_form_01\Resources.Designer.cs" };
            compiler.ResourceFiles = new ResourceFile[] { new ResourceFile { File = @"test_form_01\Test_WinForm_01.Properties.Resources.resources", Namespace = "Test_WinForm_01.Properties.Resources.resources" } };
            compiler.AssembliesFiles = new string[] { frameworkDir + "mscorlib.dll", frameworkDir + "System.dll", frameworkDir + "System.Windows.Forms.dll", frameworkDir + "System.Drawing.dll" };
            compiler.Compile();
        }

        public static void CompileProject(string projectFile)
        {
            CSharpProject project = CSharpProject.Create(projectFile);

            CSharpCompiler compiler = new CSharpCompiler();
            compiler.LanguageVersion = project.LanguageVersion;
            compiler.OutputKind = project.OutputKind;
            compiler.OptimizationLevel = project.OptimizationLevel;
            compiler.Platform = project.Platform;
            compiler.GeneralDiagnosticOption = project.GeneralDiagnosticOption;
            compiler.WarningLevel = project.WarningLevel;

            compiler.AssemblyName = project.AssemblyName;
            compiler.OutputPath = project.OutputPath;
            compiler.PdbPath = project.PdbPath;
            compiler.Win32ResourceFile = project.Win32ResourceFile;
            compiler.PreprocessorSymbols = project.PreprocessorSymbols;
            compiler.SourceFiles = project.SourceFiles;
            compiler.ResourceFiles = project.ResourceFiles;
            compiler.AssembliesFiles = project.AssembliesFiles;
            compiler.Compile();
        }

        public static void TraceProject(string projectFile)
        {
            CSharpProject.Create(projectFile).Trace();
        }
    }
}

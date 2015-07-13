using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace pb.Compiler
{
    public interface IGenerateAndExecuteManager
    {
        string GenerateAssemblyDirectory { get; set; }
        IGenerateAndExecute New();
        void DeleteGeneratedAssemblies();
    }

    public interface IGenerateAndExecute
    {
        string NameSpace { get; set; }
        string ClassName { get; set; }
        string RunFunctionName { get; set; }
        string InitFunctionName { get; set; }
        string EndFunctionName { get; set; }
        ICompiler Compiler { get; }
        Thread ExecutionThread { get; }

        void AddUsings(IEnumerable<string> usings);
        void GenerateCSharpCode(string code);
        Type GetAssemblyType(string typeName = null);
        MethodInfo GetAssemblyMethod(string methodName);
        MethodInfo GetAssemblyRunMethod();
        MethodInfo GetAssemblyInitMethod();
        MethodInfo GetAssemblyEndMethod();
    }
}

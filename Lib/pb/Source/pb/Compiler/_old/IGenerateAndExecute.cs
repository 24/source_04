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
        string RunMethodName { get; set; }
        string InitMethodName { get; set; }
        string EndMethodName { get; set; }
        ICompiler Compiler { get; }
        Thread ExecutionThread { get; }

        void AddUsings(IEnumerable<string> usings);
        void GenerateCSharpCode(string code);
        Type GetRunType(string typeName = null);
        //MethodInfo GetAssemblyMethod(string methodName, bool throwError = false, bool traceError = false);
        MethodInfo GetAssemblyRunMethod();
        MethodInfo GetAssemblyInitMethod();
        MethodInfo GetAssemblyEndMethod();
    }
}

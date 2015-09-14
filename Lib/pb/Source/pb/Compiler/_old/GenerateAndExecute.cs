using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using pb.IO;

namespace pb.Compiler
{
    public class GenerateAndExecuteManager : IGenerateAndExecuteManager
    {
        private static string __defaultGenerateAssemblySubdirectory = "run";     // c:\pib\prog\tools\runsource\exe\run
        private static string __defaultGenerateAssemblyName = "RunSource";       // RunSource_00001.cs RunSource_00001.dll RunSource_00001.pdb
        private string _generateAssemblyDirectory = null;                        // c:\pib\prog\tools\runsource\exe\run

        public GenerateAndExecuteManager()
        {
            _generateAssemblyDirectory = zPath.Combine(zapp.GetAppDirectory(), __defaultGenerateAssemblySubdirectory);
        }

        public string GenerateAssemblyDirectory { get { return _generateAssemblyDirectory; } set { _generateAssemblyDirectory = value; } }

        public IGenerateAndExecute New()
        {
            string file = GetNewAssemblyFilename();
            return new GenerateAndExecute(file);
        }

        public void DeleteGeneratedAssemblies()
        {
            zfile.DeleteFiles(_generateAssemblyDirectory, __defaultGenerateAssemblyName + "*.*", throwError: false);
        }

        private string GetNewAssemblyFilename()
        {
            // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"
            if (!zDirectory.Exists(_generateAssemblyDirectory))
                zDirectory.CreateDirectory(_generateAssemblyDirectory);
            int i = zfile.GetLastFileNameIndex(_generateAssemblyDirectory) + 1;
            return zPath.Combine(_generateAssemblyDirectory, __defaultGenerateAssemblyName + string.Format("_{0:00000}", i));
        }
    }

    public class GenerateAndExecute : IGenerateAndExecute
    {
        //private static string __defaultNameSpace = "RunSourceExecute";
        private static string __defaultClassName = "w";
        private static string __defaultRunMethodName = "Run";
        private static string __defaultInitMethodName = "Init";
        private static string __defaultEndMethodName = "End";

        private string _assemblyFilename = null;      // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"
        private Dictionary<string, string> _usings = new Dictionary<string, string>();

        private string _nameSpace = null;             // namespace from project or "RunSourceExecute"
        private string _className = null;             // "w"

        //private string _runTypeName = null;           // "NameSpace.w"
        private Type _runType = null;                 // Type _runTypeName in compiled assembly
        private string _runMethodName = null;       // "Run"

        //private string _initTypeName = null;          // if null use _runTypeName
        private string _initMethodName = null;      // "Init"

        //private string _endTypeName = null;           // if null use _runTypeName
        private string _endMethodName = null;       // "End"

        private Compiler _compiler = null;
        private Thread _executionThread = null;

        public GenerateAndExecute(string assemblyFilename)
        {
            _assemblyFilename = assemblyFilename;
            //_nameSpace = __defaultNameSpace;
            _className = __defaultClassName;
            _runMethodName = __defaultRunMethodName;
            _initMethodName = __defaultInitMethodName;
            _endMethodName = __defaultEndMethodName;
            _compiler = new Compiler();
            _compiler.SetOutputAssembly(assemblyFilename);
        }


        // test if (value != null) pour simplifier l'utilisation à partir d'un fichier de config
        /// <summary>example "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"</summary>
        public string AssemblyFilename { get { return _assemblyFilename; } set { if (value != null) _assemblyFilename = value; } }
        public string NameSpace { get { return _nameSpace; } set { if (value != null) _nameSpace = value; } }
        public string ClassName { get { return _className; } set { if (value != null) _className = value; } }
        public string RunMethodName { get { return _runMethodName; } set { if (value != null) _runMethodName = value; } }
        public string InitMethodName { get { return _initMethodName; } set { _initMethodName = value; } }
        public string EndMethodName { get { return _endMethodName; } set { _endMethodName = value; } }
        public ICompiler Compiler { get { return _compiler; } }
        public Thread ExecutionThread { get { return _executionThread; } }

        public void AddUsings(IEnumerable<string> usings)
        {
            foreach (string name in usings)
            {
                if (!_usings.ContainsKey(name))
                    _usings.Add(name, name);
            }
        }

        public void GenerateCSharpCode(string code)
        {
            string file = zpath.PathSetExtension(_assemblyFilename, ".cs");
            using (StreamWriter sw = zFile.CreateText(file))
            {
                CSharpCodeWriter generateCode = new CSharpCodeWriter(sw);

                // using
                generateCode.WriteUsings(_usings.Keys);

                // open namespace
                generateCode.OpenNameSpace(_nameSpace);

                // open class
                // public static partial class ...
                generateCode.OpenClass(_className, ClassOptions.Public | ClassOptions.Static | ClassOptions.Partial);

                // open function
                // public static void Run()
                generateCode.WriteLine("public static void {0}()", _runMethodName);
                generateCode.WriteLine("{");
                generateCode.WriteLine(code);
                generateCode.WriteLine("}");

                // close class
                generateCode.CloseClass();

                // close namespace
                generateCode.CloseNameSpace();
            }
            _compiler.AddSource(new CompilerFile(file));
        }

        private string GetRunTypeName()
        {
            if (_nameSpace != null)
                return _nameSpace + "." + _className;
            else
                return _className;
        }

        //public Type GetAssemblyType(string typeName = null)
        public Type GetRunType(string typeName = null)
        {
            Assembly assembly = _compiler.Results.CompiledAssembly;
            if (typeName == null)
            {
                if (_runType == null)
                {
                    //if (_runTypeName == null)
                    //{
                    //    if (_nameSpace != null)
                    //        runTypeName = _nameSpace + "." + _className;
                    //    else
                    //        runTypeName = _className;
                    //}
                    string runTypeName = GetRunTypeName();
                    _runType = assembly.GetType(runTypeName);
                    if (_runType == null)
                        throw new PBException("type \"{0}\" not found in assembly \"{1}\"", runTypeName, assembly.Location);
                }
                return _runType;
            }
            else
            {
                Type type = assembly.GetType(typeName);
                if (type == null)
                    throw new PBException("type \"{0}\" not found in assembly \"{1}\"", typeName, assembly.Location);
                return type;
            }
        }

        public MethodInfo GetAssemblyMethod(string methodName, bool throwError = false, bool traceError = false)
        {
            if (methodName == null)
                return null;
            Type runType = GetRunType();
            MethodInfo method = runType.GetMethod(methodName);
            if (method == null)
            {
                //string error = null;
                //if (throwError || traceError)
                //    error = string.Format("method not found \"{0}\" from type \"{1}\" in assembly \"{2}\"", methodName, runType.zGetTypeName(), runType.Assembly.Location);
                //if (throwError)
                //    throw new PBException(error);
                //if (traceError)
                //    Trace.WriteLine(error);
                Error(string.Format("method not found \"{0}\" from type \"{1}\" in assembly \"{2}\"", methodName, runType.zGetTypeName(), runType.Assembly.Location), throwError, traceError);
            }
            return method;
        }

        private MethodInfo GetAssemblyMethod_v2(string typeName, string methodName, bool throwError = false, bool traceError = false)
        {
            if (methodName == null)
                return null;
            //Type type = GetAssemblyType();
            Type type = Type.GetType(typeName);
            if (type == null)
            {
                Error(string.Format("type not found \"{0}\"", typeName), throwError, traceError);
            }
            else
            {
                MethodInfo method = type.GetMethod(methodName);
                if (method == null)
                {
                    //string error = null;
                    //if (throwError || traceError)
                    //    error = string.Format("method not found \"{0}\" from type \"{1}\" in assembly \"{2}\"", methodName, type.zGetTypeName(), type.Assembly.Location);
                    //if (throwError)
                    //    throw new PBException(error);
                    //if (traceError)
                    //    Trace.WriteLine(error);
                    Error(string.Format("method not found \"{0}\" from type \"{1}\" in assembly \"{2}\"", methodName, type.zGetTypeName(), type.Assembly.Location), throwError, traceError);
                }
                return method;
            }
            return null;
        }

        private static void Error(string errorMessage, bool throwError, bool traceError)
        {
            if (throwError)
                throw new PBException(errorMessage);
            if (traceError)
                Trace.WriteLine(errorMessage);
        }

        public MethodInfo GetAssemblyRunMethod()
        {
            return GetAssemblyMethod(_runMethodName, throwError: true);
            //return GetAssemblyMethod_v2(GetRunTypeName(), _runMethodName, throwError: true);
        }

        public MethodInfo GetAssemblyInitMethod()
        {
            return GetAssemblyMethod(_initMethodName, traceError: true);
            //return GetAssemblyMethod_v2(_initTypeName ?? GetRunTypeName(), _initMethodName, traceError: true);
        }

        public MethodInfo GetAssemblyEndMethod()
        {
            return GetAssemblyMethod(_endMethodName, traceError: true);
            //return GetAssemblyMethod_v2(_endTypeName ?? GetRunTypeName(), _endMethodName, traceError: true);
        }
    }
}

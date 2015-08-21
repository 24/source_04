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
            zfile.DeleteFiles(_generateAssemblyDirectory, __defaultGenerateAssemblyName + "*.*", false);
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
        private static string __defaultNameSpace = "RunSourceExecute";
        private static string __defaultClassName = "w";
        private static string __defaultRunFunctionName = "Run";
        private static string __defaultInitFunctionName = "Init";
        private static string __defaultEndFunctionName = "End";

        private string _assemblyFilename = null;      // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"
        private Dictionary<string, string> _usings = new Dictionary<string, string>();
        private string _nameSpace = null;             // namespace from project or "RunSourceExecute"
        private string _className = null;             // "w"
        private string _typeName = null;              // "NameSpace.w"
        private Type _type = null;                    // Type _typeName in compiled assembly
        private string _runFunctionName = null;       // "Run"
        private string _initFunctionName = null;      // "Init"
        private string _endFunctionName = null;       // "End"
        private Compiler _compiler = null;
        private Thread _executionThread = null;

        public GenerateAndExecute(string assemblyFilename)
        {
            _assemblyFilename = assemblyFilename;
            _nameSpace = __defaultNameSpace;
            _className = __defaultClassName;
            _runFunctionName = __defaultRunFunctionName;
            _initFunctionName = __defaultInitFunctionName;
            _endFunctionName = __defaultEndFunctionName;
            _compiler = new Compiler();
            _compiler.OutputAssembly = assemblyFilename;
        }


        public string NameSpace { get { return _nameSpace; } set { if (value != null) _nameSpace = value; } }
        public string ClassName { get { return _className; } set { if (value != null) _className = value; } }
        public string RunFunctionName { get { return _runFunctionName; } set { if (value != null) _runFunctionName = value; } }
        public string InitFunctionName { get { return _initFunctionName; } set { _initFunctionName = value; } }
        public string EndFunctionName { get { return _endFunctionName; } set { _endFunctionName = value; } }
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
                GenerateCSharpCode generateCode = new GenerateCSharpCode(sw);

                // using
                generateCode.WriteUsings(_usings.Keys);

                // open namespace
                generateCode.OpenNameSpace(_nameSpace);

                // open class
                // public static partial class ...
                generateCode.OpenClass(_className, ClassOptions.Public | ClassOptions.Static | ClassOptions.Partial);

                // open function
                // public static void Run()
                generateCode.WriteLine("public static void {0}()", _runFunctionName);
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

        //public void Execute(bool useNewThread = true)
        //{
        //    bool bError = false;
        //    bool bOk = false;
        //    Chrono chrono = new Chrono();
        //    try
        //    {
        //        MethodInfo method = GetAssemblyMethod(_initFunctionName);
        //        if (method != null)
        //        {
        //            chrono.Start();
        //            method.Invoke(null, null);
        //            chrono.Stop();
        //        }

        //        method = GetAssemblyMethod(_runFunctionName);
        //        if (method == null)
        //            throw new PBException("function \"{0}\" not found in type \"{1}\" in assembly \"{2}\"", _runFunctionName, _type.zGetTypeName(), _compiler.Results.CompiledAssembly.Location);

        //        MethodInfo endMethod = GetAssemblyMethod(_endFunctionName);

        //        if (useNewThread)
        //        {
        //            _executionThread = new Thread(new ThreadStart(ThreadStart));
        //            _executionThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
        //            _executionThread.SetApartmentState(ApartmentState.STA);
        //            _executionThread.Start();
        //        }
        //        else
        //        {
        //            Trace.WriteLine("execute on main thread");
        //            this.ThreadStart();
        //        }

        //        bOk = true;
        //    }
        //    catch
        //    {
        //        bError = true;
        //        throw;
        //    }
        //    finally
        //    {
        //        if (!bOk && EndRun != null)
        //            EndRun(bError);
        //    }
        //}

        public Type GetAssemblyType(string typeName = null)
        {
            Assembly assembly = _compiler.Results.CompiledAssembly;
            if (typeName == null)
            {
                if (_type == null)
                {
                    if (_typeName == null)
                    {
                        if (_nameSpace != null)
                            _typeName = _nameSpace + "." + _className;
                        else
                            _typeName = _className;
                    }
                    _type = assembly.GetType(_typeName);
                    if (_type == null)
                        throw new PBException("type \"{0}\" not found in assembly \"{1}\"", _typeName, assembly.Location);
                }
                return _type;
            }
            else
            {
                Type type = assembly.GetType(typeName);
                if (type == null)
                    throw new PBException("type \"{0}\" not found in assembly \"{1}\"", typeName, assembly.Location);
                return type;
            }
        }

        public MethodInfo GetAssemblyMethod(string methodName)
        {
            if (methodName == null)
                return null;
            Type type = GetAssemblyType();
            return type.GetMethod(methodName);
        }

        public MethodInfo GetAssemblyRunMethod()
        {
            return GetAssemblyMethod(_runFunctionName);
        }

        public MethodInfo GetAssemblyInitMethod()
        {
            return GetAssemblyMethod(_initFunctionName);
        }

        public MethodInfo GetAssemblyEndMethod()
        {
            return GetAssemblyMethod(_endFunctionName);
        }
    }
}

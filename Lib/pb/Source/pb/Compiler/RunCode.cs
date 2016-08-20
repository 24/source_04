using System;
using System.Reflection;
using System.Threading;
using pb.Reflection;

namespace pb.Compiler
{
    public class RunCode
    {
        private int _id;
        private Assembly _runAssembly = null;
        //private Dictionary<string, CompilerAssembly> _compilerAssemblies = null;
        private string _runMethodName = null;       // "Test._RunCode.Run"

        private Type _runType = null;

        private Thread _runThread = null;
        private Chrono _runChrono = null;
        private Action<bool> _endRun = null;

        public RunCode(int id)
        {
            _id = id;
        }

        public int Id { get { return _id; } }
        public Assembly RunAssembly { get { return _runAssembly; } set { _runAssembly = value; } }
        //public Dictionary<string, CompilerAssembly> CompilerAssemblies { get { return _compilerAssemblies; } set { _compilerAssemblies = value; } }
        public string RunMethodName { get { return _runMethodName; } set { _runMethodName = value; } }
        public Thread RunThread { get { return _runThread; } }
        public Chrono RunChrono { get { return _runChrono; } }
        public Action<bool> EndRun { get { return _endRun; } set { _endRun = value; } }

        public void Run(bool runOnMainThread)
        {
            _runChrono = new Chrono();
            if (_runAssembly == null)
                throw new PBException("assembly is null");
            MethodInfo runMethod = GetRunMethod();

            //AssemblyResolve.Clear();

            if (!runOnMainThread)
            {
                _runThread = new Thread(new ThreadStart(() => _Run(runMethod)));
                _runThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                _runThread.SetApartmentState(ApartmentState.STA);
                _runThread.Start();
            }
            else
            {
                _Run(runMethod);
            }
        }

        private MethodInfo GetRunMethod()
        {
            if (_runMethodName == null)
                throw new PBException("run method name is null");
            MethodElements runMethodElements = zReflection.GetMethodElements(_runMethodName);
            if (runMethodElements.TypeName == null)
                throw new PBException("bad run method name \"{0}\", run method need type name", _runMethodName);
            if (runMethodElements.QualifiedTypeName != null)
                _runType = zReflection.GetType(runMethodElements.QualifiedTypeName, ErrorOptions.ThrowError);
            else
                _runType = zReflection.GetType(_runAssembly, runMethodElements.TypeName, ErrorOptions.ThrowError);
            return zReflection.GetMethod(_runType, runMethodElements.MethodName, ErrorOptions.ThrowError);
        }

        public MethodInfo GetMethod(string methodName)
        {
            if (methodName == null)
                return null;
            MethodElements runMethodElements = zReflection.GetMethodElements(methodName);
            Type type;
            if (runMethodElements.TypeName != null)
            {
                Assembly assembly = null;
                if (runMethodElements.AssemblyName != null)
                {
                    assembly = zReflection.GetAssembly(runMethodElements.AssemblyName, ErrorOptions.None);
                    //if (assembly == null && _compilerAssemblies != null)
                    //{
                    //    // Compiler.AddAssembly() generate _compilerAssemblies with lower case key (ToLowerInvariant)
                    //    string assemblyName = zReflection.GetAssemblyName(runMethodElements.AssemblyName).ToLowerInvariant();
                    //    if (_compilerAssemblies.ContainsKey(assemblyName))
                    //    {
                    //        string file = _compilerAssemblies[assemblyName].File;
                    //        Trace.WriteLine("load assembly from \"{0}\"", file);
                    //        assembly = Assembly.LoadFrom(file);
                    //    }
                    //    else
                    //        Trace.WriteLine("unknow assembly \"{0}\"", assemblyName);
                    //}
                    if (assembly == null)
                    {
                        assembly = Assembly.Load(runMethodElements.AssemblyName);
                    }
                    if (assembly == null)
                    {
                        Error.WriteMessage(ErrorOptions.TraceWarning, "unable to load assembly \"{0}\"", runMethodElements.AssemblyName);
                        return null;
                    }
                }
                else
                    assembly = _runAssembly;
                type = zReflection.GetType(assembly, runMethodElements.TypeName, ErrorOptions.TraceWarning);
            }
            else
                type = _runType;
            if (type != null)
                return zReflection.GetMethod(type, runMethodElements.MethodName, ErrorOptions.TraceWarning);
            else
                return null;
        }

        private void _Run(MethodInfo runMethod)
        {
            bool error = false;
            try
            {
                try
                {
                    _runChrono.Start();
                    runMethod.Invoke(null, null);
                    _runChrono.Stop();
                }
                catch (Exception ex)
                {
                    _runChrono.Stop();
                    error = true;
                    Trace.CurrentTrace.WriteError(ex);
                }
            }
            finally
            {
                _runThread = null;
                if (_endRun != null)
                    _endRun(error);
            }
        }
    }
}

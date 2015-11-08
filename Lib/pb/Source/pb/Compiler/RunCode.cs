using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using pb.Reflection;

namespace pb.Compiler
{
    public class RunCode
    {
        private Assembly _runAssembly = null;
        private Dictionary<string, CompilerAssembly> _compilerAssemblies = null;
        //private string _runClassName = null;        // "Test.w"
        private string _runMethodName = null;       // "Test._RunCode.Run"
        //private string _initClassName = null;       // if null use _runClassName
        private string _initMethodName = null;      // "Init", "Test._RunCode.Init", "Test._RunCode.Init, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
        //private string _endClassName = null;        // if null use _runClassName
        private string _endMethodName = null;       // "End"

        private Type _runType = null;

        private Thread _runThread = null;
        private Chrono _runChrono = null;
        private Action<bool> _endRun = null;

        public RunCode()
        {
        }

        public Assembly RunAssembly { get { return _runAssembly; } set { _runAssembly = value; } }
        public Dictionary<string, CompilerAssembly> CompilerAssemblies { get { return _compilerAssemblies; } set { _compilerAssemblies = value; } }
        //public string RunClassName { get { return _runClassName; } set { _runClassName = value; } }
        public string RunMethodName { get { return _runMethodName; } set { _runMethodName = value; } }
        //public string InitClassName { get { return _initClassName; } set { _initClassName = value; } }
        public string InitMethodName { get { return _initMethodName; } set { _initMethodName = value; } }
        //public string EndClassName { get { return _endClassName; } set { _endClassName = value; } }
        public string EndMethodName { get { return _endMethodName; } set { _endMethodName = value; } }
        public Thread RunThread { get { return _runThread; } }
        public Chrono RunChrono { get { return _runChrono; } }
        public Action<bool> EndRun { get { return _endRun; } set { _endRun = value; } }

        //public void Run(bool useNewThread)
        //{
        //    _runChrono = new Chrono();
        //    if (_runAssembly != null)
        //    {
        //        _runClass = Reflection.GetType(_runAssembly, _runClassName, ErrorOptions.TraceError);
        //        if (_runClass != null)
        //        {
        //            MethodInfo runMethod = Reflection.GetMethod(_runClass, _runMethodName, ErrorOptions.TraceError);
        //            if (runMethod != null)
        //            {
        //                MethodInfo initMethod = GetMethod(_initMethodName, _initClassName);
        //                MethodInfo endMethod = GetMethod(_endMethodName, _endClassName);

        //                AssemblyResolve.Stop();
        //                AssemblyResolve.Clear();
        //                AssemblyResolve.Start();

        //                if (useNewThread)
        //                {
        //                    _runThread = new Thread(new ThreadStart(() => _Run(runMethod, initMethod, endMethod)));
        //                    _runThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
        //                    _runThread.SetApartmentState(ApartmentState.STA);
        //                    _runThread.Start();
        //                }
        //                else
        //                {
        //                    Trace.WriteLine("execute on main thread");
        //                    _Run(runMethod, initMethod, endMethod);
        //                }
        //            }
        //        }
        //    }
        //    else
        //        Error.WriteMessage(ErrorOptions.TraceError, "run assembly is null");
        //}

        public void Run(bool useNewThread)
        {
            _runChrono = new Chrono();
            if (_runAssembly == null)
                throw new PBException("assembly is null");
            MethodInfo runMethod = GetRunMethod();
            MethodInfo initMethod = GetMethod(_initMethodName);
            //if (initMethod != null)
            //    Trace.WriteLine("found init method \"{0}\"", _initMethodName);
            MethodInfo endMethod = GetMethod(_endMethodName);
            //if (endMethod != null)
            //    Trace.WriteLine("found init method \"{0}\"", _endMethodName);

            AssemblyResolve.Stop();
            AssemblyResolve.Clear();
            AssemblyResolve.Start();

            if (useNewThread)
            {
                _runThread = new Thread(new ThreadStart(() => _Run(runMethod, initMethod, endMethod)));
                _runThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                _runThread.SetApartmentState(ApartmentState.STA);
                _runThread.Start();
            }
            else
            {
                Trace.WriteLine("execute on main thread");
                _Run(runMethod, initMethod, endMethod);
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

        private MethodInfo GetMethod(string methodName)
        {
            if (methodName == null)
                return null;
            MethodElements runMethodElements = zReflection.GetMethodElements(methodName);
            Type type;
            if (runMethodElements.TypeName != null)
            {
                //if (runMethodElements.QualifiedTypeName != null)
                //    type = Reflection.GetType(runMethodElements.QualifiedTypeName, ErrorOptions.TraceWarning);
                //_compilerAssemblies
                Assembly assembly = null;
                if (runMethodElements.AssemblyName != null)
                {
                    assembly = zReflection.GetAssembly(runMethodElements.AssemblyName, ErrorOptions.None);
                    if (assembly == null && _compilerAssemblies != null)
                    {
                        string assemblyName = zReflection.GetAssemblyName(runMethodElements.AssemblyName);
                        if (_compilerAssemblies.ContainsKey(assemblyName))
                        {
                            string file = _compilerAssemblies[assemblyName].File;
                            Trace.WriteLine("load assembly from \"{0}\"", file);
                            assembly = Assembly.LoadFrom(file);
                        }
                    }
                    //if (runMethodElements.AssemblyName == "ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                    //{
                    //    string file = @"c:\pib\drive\google\dev\project\Source\Apps\WebData\Source\Print\Project\bin32\ebook.download.dll";
                    //    Trace.WriteLine("load assembly from \"{0}\"", file);
                    //    assembly = Assembly.LoadFrom(file);
                    //}
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
                return zReflection.GetMethod(type, runMethodElements.MethodName, ErrorOptions.ThrowError);
            else
                return null;
        }

        //private MethodInfo GetMethod(string methodName, string className)
        //{
        //    if (methodName == null)
        //        return null;

        //    MethodInfo method = null;
        //    if (className != null)
        //    {
        //        Type type = null;
        //        if (Reflection.IsQualifiedTypeName(className))
        //            type = Reflection.GetType(className, ErrorOptions.TraceError);
        //        else
        //            type = Reflection.GetType(_runAssembly, className, ErrorOptions.TraceError);
        //        if (type != null)
        //            method = Reflection.GetMethod(type, methodName, ErrorOptions.TraceWarning);
        //    }
        //    else
        //        method = Reflection.GetMethod(_runClass, methodName, ErrorOptions.TraceWarning);

        //    return method;
        //}

        private void _Run(MethodInfo runMethod, MethodInfo initMethod = null, MethodInfo endMethod = null)
        {
            bool error = false;
            try
            {
                //_executionAborted = false;
                try
                {
                    if (initMethod != null)
                    {
                        _runChrono.Start();
                        initMethod.Invoke(null, null);
                        _runChrono.Stop();
                    }

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
                try
                {
                    if (endMethod != null)
                    {
                        _runChrono.Start();
                        endMethod.Invoke(null, null);
                        _runChrono.Stop();
                    }
                }
                catch (Exception ex)
                {
                    _runChrono.Stop();
                    error = true;
                    Trace.CurrentTrace.WriteError(ex);
                }
                finally
                {
                    _runThread = null;
                    //_executionPaused = false;
                    //_executionAborted = false;
                    if (_endRun != null)
                        _endRun(error);
                }
            }
        }
    }
}

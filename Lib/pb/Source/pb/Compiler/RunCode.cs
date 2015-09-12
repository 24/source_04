using System;
using System.Reflection;
using System.Threading;

namespace pb.Compiler
{
    public class RunCode
    {
        private Assembly _runAssembly = null;
        private string _runClassName = null;        // "Test.w"
        private string _runMethodName = null;       // "Test._RunCode.Run"
        private string _initClassName = null;       // if null use _runClassName
        private string _initMethodName = null;      // "Init", "Test._RunCode.Init", "Test._RunCode.Init, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
        private string _endClassName = null;        // if null use _runClassName
        private string _endMethodName = null;       // "End"

        private Type _runClass = null;

        private Thread _runThread = null;
        private Chrono _runChrono = null;
        private Action<bool> _endRun = null;

        public RunCode()
        {
        }

        public Assembly RunAssembly { get { return _runAssembly; } set { _runAssembly = value; } }
        public string RunClassName { get { return _runClassName; } set { _runClassName = value; } }
        public string RunMethodName { get { return _runMethodName; } set { _runMethodName = value; } }
        public string InitClassName { get { return _initClassName; } set { _initClassName = value; } }
        public string InitMethodName { get { return _initMethodName; } set { _initMethodName = value; } }
        public string EndClassName { get { return _endClassName; } set { _endClassName = value; } }
        public string EndMethodName { get { return _endMethodName; } set { _endMethodName = value; } }
        public Thread RunThread { get { return _runThread; } }
        public Chrono RunChrono { get { return _runChrono; } }
        public Action<bool> EndRun { get { return _endRun; } set { _endRun = value; } }

        public void Run(bool useNewThread)
        {
            _runChrono = new Chrono();
            if (_runAssembly != null)
            {
                _runClass = Reflection.GetType(_runAssembly, _runClassName, ErrorOptions.TraceError);
                if (_runClass != null)
                {
                    MethodInfo runMethod = Reflection.GetMethod(_runClass, _runMethodName, ErrorOptions.TraceError);
                    if (runMethod != null)
                    {
                        //if (_initMethodName != null)
                        //{
                        //    Type initClass = null;
                        //    if (_initClassName != null)
                        //    {
                        //        if (Reflection.IsQualifiedTypeName(_initClassName))
                        //            initClass = Reflection.GetType(_initClassName, ErrorOptions.TraceError);
                        //        else
                        //            initClass = Reflection.GetType(_runAssembly, _initClassName, ErrorOptions.TraceError);
                        //        if (initClass != null)
                        //            initMethod = Reflection.GetMethod(initClass, _initMethodName, ErrorOptions.TraceWarning);
                        //    }
                        //    else
                        //        initMethod = Reflection.GetMethod(_runClass, _initMethodName, ErrorOptions.TraceWarning);
                        //}
                        MethodInfo initMethod = GetMethod(_initMethodName, _initClassName);
                        MethodInfo endMethod = GetMethod(_endMethodName, _endClassName);

                        //endMethod = Reflection.GetMethod(_runClass, _endMethodName, ErrorOptions.TraceWarning);

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
                }
            }
            else
                //throw new PBException("run assembly is null");
                Error.WriteMessage(ErrorOptions.TraceError, "run assembly is null");
        }

        private MethodInfo GetMethod(string methodName, string className)
        {
            if (methodName == null)
                return null;

            MethodInfo method = null;
            if (className != null)
            {
                Type type = null;
                if (Reflection.IsQualifiedTypeName(className))
                    type = Reflection.GetType(className, ErrorOptions.TraceError);
                else
                    type = Reflection.GetType(_runAssembly, className, ErrorOptions.TraceError);
                if (type != null)
                    method = Reflection.GetMethod(type, methodName, ErrorOptions.TraceWarning);
            }
            else
                method = Reflection.GetMethod(_runClass, methodName, ErrorOptions.TraceWarning);

            return method;
        }

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

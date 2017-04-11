using System;
using System.Reflection;
using System.Threading;
using pb.Reflection;
using System.Threading.Tasks;

namespace pb.Compiler
{
    public class RunCode
    {
        private int _id;
        private Assembly _runAssembly = null;
        private string _runMethodName = null;       // "Test._RunCode.Run"

        private Thread _runThread = null;
        private Chrono _runChrono = null;
        private Action<bool> _endRun = null;

        public RunCode(int id)
        {
            _id = id;
        }

        public int Id { get { return _id; } }
        public Assembly RunAssembly { get { return _runAssembly; } set { _runAssembly = value; } }
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

        public async Task Run_v2(bool runOnMainThread)
        {
            _runChrono = new Chrono();
            if (_runAssembly == null)
                throw new PBException("assembly is null");
            MethodInfo runMethod = GetRunMethod();

            if (!runOnMainThread)
            {
                _runThread = new Thread(new ThreadStart(async () => await _Run_v2(runMethod)));
                _runThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                _runThread.SetApartmentState(ApartmentState.STA);
                _runThread.Start();
            }
            else
            {
                await _Run_v2(runMethod);
            }
        }

        private MethodInfo GetRunMethod()
        {
            if (_runMethodName == null)
                throw new PBException("run method name is null");
            MethodElements runMethodElements = zReflection.GetMethodElements(_runMethodName);
            if (runMethodElements.TypeName == null)
                throw new PBException("bad run method name \"{0}\", run method need type name", _runMethodName);
            Type runType;
            if (runMethodElements.QualifiedTypeName != null)
                runType = zReflection.GetType(runMethodElements.QualifiedTypeName, ErrorOptions.ThrowError);
            else
                runType = zReflection.GetType(_runAssembly, runMethodElements.TypeName, ErrorOptions.ThrowError);
            return zReflection.GetMethod(runType, runMethodElements.MethodName, ErrorOptions.ThrowError);
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
                    Trace.WriteError(ex);
                }
            }
            finally
            {
                _runThread = null;
                _endRun?.Invoke(error);
            }
        }

        private async Task _Run_v2(MethodInfo runMethod)
        {
            bool error = false;
            try
            {
                try
                {
                    _runChrono.Start();
                    await (Task)runMethod.Invoke(null, null);
                    _runChrono.Stop();
                }
                catch (Exception ex)
                {
                    _runChrono.Stop();
                    error = true;
                    Trace.WriteError(ex);
                }
            }
            finally
            {
                _runThread = null;
                _endRun?.Invoke(error);
            }
        }
    }
}

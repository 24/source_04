using System;
using System.IO;
using System.Windows.Forms;
using pb.IO;

namespace pb.Compiler
{
    public class RemoteRunSource : IDisposable
    {
        private string _runsourceDllFilename = null;  // "runsource.dll.dll"
        private string _runSourceDomainName = "RunSource";
        private bool _createRunSourceDomain = false;    // attention si CreateRunSourceDomain = true runsource.launch.exe ne peut pas mettre à jour runsource.runsource.exe
        private AppDomain _domain = null;
        private string _runsourceClassName = null;    // "pb.Compiler.RunSource"
        private IRunSource _runSource = null;
        private string _traceClassName = null;        // "pb.TTrace"
        private ITrace _trace = null;

        //public RemoteRunSource(string runsourceDllFilename, bool createRunSourceDomain = true)
        //{
        //    _createRunSourceDomain = createRunSourceDomain;
        //    //string runsourceDllFilename = Path.Combine(zapp.GetAppDirectory(), "runsource.dll");
        //    runsourceDllFilename = Path.Combine(zapp.GetAppDirectory(), runsourceDllFilename);
        //    if (createRunSourceDomain)
        //    {
        //        _domain = AppDomain.CreateDomain(_runSourceDomainName);
        //        //System.Diagnostics.Debug.WriteLine(string.Format("FriendlyName {0}", _domain.FriendlyName));
        //        //System.Diagnostics.Debug.WriteLine(string.Format("BaseDirectory {0}", _domain.BaseDirectory));
        //        //System.Diagnostics.Debug.WriteLine(string.Format("RelativeSearchPath {0}", _domain.RelativeSearchPath));
        //        _runSource = (IRunSource)_domain.CreateInstanceFromAndUnwrap(runsourceDllFilename, "pb.old.RunSource");
        //        _runSource.SetAsCurrentRunSource();
        //        //_domain.SetData("RunSource", _runSource);
        //    }
        //    else
        //    {
        //        _domain = AppDomain.CurrentDomain;
        //        //_domain.CreateInstance();
        //        _runSource = (IRunSource)_domain.CreateInstanceFromAndUnwrap(runsourceDllFilename, "pb.old.RunSource");
        //        _runSource.SetAsCurrentRunSource();
        //    }
        //}

        public void Dispose()
        {
            if (_runSource != null)
            {
                _runSource.Dispose();
                _runSource = null;
            }
            if (_domain != null && _createRunSourceDomain)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }

        public string RunsourceDllFilename
        {
            get { return _runsourceDllFilename; }
            set
            {
                //_runsourceDllFilename = value;
                //if (!Path.IsPathRooted(_runsourceDllFilename))
                //    _runsourceDllFilename = Path.Combine(zapp.GetAppDirectory(), _runsourceDllFilename);
                _runsourceDllFilename = value.zRootPath(zapp.GetAppDirectory());
            }
        }
        public string RunSourceDomainName { get { return _runSourceDomainName; } set { _runSourceDomainName = value; } }
        public bool CreateRunSourceDomain { get { return _createRunSourceDomain; } set { _createRunSourceDomain = value; } }
        public string RunsourceClassName { get { return _runsourceClassName; } set { _runsourceClassName = value; } }
        public string TraceClassName { get { return _traceClassName; } set { _traceClassName = value; } }

        private AppDomain GetAppDomain()
        {
            if (_domain == null)
            {
                if (_createRunSourceDomain)
                {
                    _domain = AppDomain.CreateDomain(_runSourceDomainName);
                    //System.Diagnostics.Debug.WriteLine(string.Format("FriendlyName {0}", _domain.FriendlyName));
                    //System.Diagnostics.Debug.WriteLine(string.Format("BaseDirectory {0}", _domain.BaseDirectory));
                    //System.Diagnostics.Debug.WriteLine(string.Format("RelativeSearchPath {0}", _domain.RelativeSearchPath));
                }
                else
                {
                    _domain = AppDomain.CurrentDomain;
                    //_domain.CreateInstance();
                }
            }
            return _domain;
        }

        public IRunSource GetRunSource()
        {
            if (_runSource == null)
            {
                _runSource = (IRunSource)GetAppDomain().CreateInstanceFromAndUnwrap(_runsourceDllFilename, _runsourceClassName);
                _runSource.SetAsCurrentRunSource();
            }
            return _runSource;
        }

        public ITrace GetTrace()
        {
            if (_trace == null)
            {
                _trace = (ITrace)GetAppDomain().CreateInstanceFromAndUnwrap(_runsourceDllFilename, _traceClassName);
                _trace.SetAsCurrentTrace();
            }
            return _trace;
        }

        public Form CreateRunSourceForm(string formClassName)
        {
            //return (Form)_domain.CreateInstanceFromAndUnwrap("runsource.dll", "runsourced.frunsource");
            return (Form)GetAppDomain().CreateInstanceFromAndUnwrap(_runsourceDllFilename, formClassName);
        }
    }
}

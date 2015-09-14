using System;
using System.Reflection;
using System.Threading;
using pb.IO;

namespace pb.Compiler
{
    public delegate void EndRunEvent(bool bError);

    public partial class RunSource
    {
        public event EndRunEvent EndRun;

        private Chrono _runCodeChrono = null;  // new Chrono()
        private Thread _runCodeThread = null;
        private GenerateAndExecuteManager _generateAndExecuteManager = null;

        //public IGenerateAndExecuteManager GenerateAndExecuteManager { get { return _generateAndExecuteManager; } }

        private GenerateAndExecuteManager GetGenerateAndExecuteManager()
        {
            if (_generateAndExecuteManager == null)
            {
                _generateAndExecuteManager = new GenerateAndExecuteManager();
                //runSource.GenerateAndExecuteManager.GenerateAssemblyDirectory = config.Get("GenerateAssemblyDirectory", "run").zRootPath(zapp.GetAppDirectory());
                _generateAndExecuteManager.GenerateAssemblyDirectory = GetRunSourceConfig().Get("GenerateAssemblyDirectory", "run").zRootPath(zapp.GetAppDirectory());
            }
            return _generateAndExecuteManager;
        }

        //private void CreateGenerateAndExecute()
        //{
        //    _generateAndExecuteManager = new GenerateAndExecuteManager();
        //}

        private void DeleteGeneratedAssemblies_v1()
        {
            GetGenerateAndExecuteManager().DeleteGeneratedAssemblies();
        }

        private void CompileCode_v1(string code, bool compileWithoutProject = false)
        {
            _GenerateCodeAndCompile_v2(GetGenerateAndExecuteManager().New(), code, compileWithoutProject);
        }

        private void RunCode_v1(string code, bool useNewThread = true, bool compileWithoutProject = false)
        {
            if (code == "")
                return;

            bool bError = false;
            if (_runCodeThread != null)
                throw new PBException("error program already running");

            bool bOk = false;
            _runCodeChrono = new Chrono();
            try
            {
                AssemblyResolve.Stop();
                AssemblyResolve.Clear();

                _refreshRunSourceConfig = true;
                _refreshProjectConfig = true;

                IGenerateAndExecute generateAndExecute = GetGenerateAndExecuteManager().New();
                _GenerateCodeAndCompile_v2(generateAndExecute, code, compileWithoutProject);
                if (generateAndExecute.Compiler.HasError())
                    return;

                MethodInfo runMethod = generateAndExecute.GetAssemblyRunMethod();
                MethodInfo initMethod = generateAndExecute.GetAssemblyInitMethod();
                MethodInfo endMethod = generateAndExecute.GetAssemblyEndMethod();

                AssemblyResolve.Start();
                if (useNewThread)
                {
                    _runCodeThread = new Thread(new ThreadStart(() => _Run(runMethod, initMethod, endMethod)));
                    _runCodeThread.CurrentCulture = FormatInfo.CurrentFormat.CurrentCulture;
                    _runCodeThread.SetApartmentState(ApartmentState.STA);
                    _runCodeThread.Start();
                }
                else
                {
                    Trace.WriteLine("execute on main thread");
                    _Run(runMethod, initMethod, endMethod);
                }

                bOk = true;
            }
            catch
            {
                bError = true;
                throw;
            }
            finally
            {
                if (!bOk && EndRun != null)
                    EndRun(bError);
            }
        }

        private void _GenerateCodeAndCompile_v2(IGenerateAndExecute generateAndExecute, string code, bool compileWithoutProject = false)
        {
            // use CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml

            if (code == "")
                return;

            CompilerProject compilerProject = null;
            if (!compileWithoutProject)
                compilerProject = GetProjectCompilerProject();
            if (compilerProject == null)
                compilerProject = GetDefaultProject();

            if (compilerProject != null)
            {
                generateAndExecute.NameSpace = compilerProject.GetNameSpace();
            }

            if (compilerProject != null)
                generateAndExecute.AddUsings(compilerProject.GetUsings());

            generateAndExecute.GenerateCSharpCode(code);

            SetCompilerValues(generateAndExecute, compilerProject);

            ICompiler compiler = generateAndExecute.Compiler;
            //Compiler compiler = new Compiler();
            //compiler.SetOutputAssembly(assemblyFilename);
            //compiler.AddSource(new CompilerFile(file));
            compiler.Compile();
            //Assembly assembly = _compiler.Results.CompiledAssembly;

            if (compiler.HasError())
            {
                //DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
                //if (dtMessage != null)
                //{
                //    gdtResult = dtMessage;
                //    gdsResult = null;
                //    gsXmlResultFormat = null;
                //    if (ErrorResultSet != null)
                //        ErrorResultSet(gdtResult, null);
                //}
                SetResultCompilerMessages(compiler);
            }
            else
                // trace warning
                compiler.TraceMessages();
        }

        public void SetCompilerValues(IGenerateAndExecute generateAndExecute, CompilerProject compilerProject)
        {
            ICompiler compiler = generateAndExecute.Compiler;
            if (compilerProject != null)
                compiler.DefaultDir = zPath.GetDirectoryName(compilerProject.ProjectFile);
            // CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), dontSetOutput: true);

            // ProjectDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            // $$ProjectDefaultValues disable
            //compiler.SetParameters(GetRunSourceConfigProjectDefaultValues(), dontSetOutput: true);

            compiler.SetParameters(compilerProject, dontSetOutput: true);
        }

        private void _Run(MethodInfo runMethod, MethodInfo initMethod = null, MethodInfo endMethod = null)
        {
            bool bError = false;
            try
            {
                _executionAborted = false;
                try
                {
                    if (initMethod != null)
                    {
                        _runCodeChrono.Start();
                        initMethod.Invoke(null, null);
                        _runCodeChrono.Stop();
                    }

                    _runCodeChrono.Start();
                    runMethod.Invoke(null, null);
                    _runCodeChrono.Stop();
                }
                catch (Exception ex)
                {
                    _runCodeChrono.Stop();
                    bError = true;
                    Trace.CurrentTrace.WriteError(ex);
                }
            }
            finally
            {
                try
                {
                    if (endMethod != null)
                    {
                        _runCodeChrono.Start();
                        endMethod.Invoke(null, null);
                        _runCodeChrono.Stop();
                    }
                }
                catch (Exception ex)
                {
                    _runCodeChrono.Stop();
                    bError = true;
                    Trace.CurrentTrace.WriteError(ex);
                }
                finally
                {
                    _runCodeThread = null;
                    _executionPaused = false;
                    _executionAborted = false;
                    if (EndRun != null)
                        EndRun(bError);
                    #region AppDomain ...
                    //******************************************************************************************************************************************************************************
                    //                                                                              AppDomain
                    //AppDomain.Unload(gDomain);
                    //gDomain = null;
                    //******************************************************************************************************************************************************************************
                    #endregion
                }
            }
        }
    }
}

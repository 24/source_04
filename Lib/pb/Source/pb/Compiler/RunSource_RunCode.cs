﻿using System;
using System.Reflection;
using pb.IO;
using System.Collections.Concurrent;
using pb.Reflection;
using System.Threading.Tasks;

// todo
//   - nouvelle version (3) pour gérer plusieurs exécutions => RunCode _runCode devient ConcurrentDictionary<int, RunCode> _runCodes
//   - gérer une liste de méthodes pour Init et End (26/03/2016).
//     cela permet de déclarer InitMethod, EndMethod dans un IncludeProject
//     voir RunCode_ExecuteCode()
//   ok gérer gExecutionThread
//   ok DeleteGeneratedAssemblies_v2()
//   ok gérer Chrono
//   ok EndRun retourner le Chrono
//   ok CompileCode_v2()
//   ok paramétrer Init et End dans CompileProject, utilisation des qualified name pour le type
//   ok pb si error ds compile runsource l'onglet message est affiché et pas l'onglet result
//   ok remplacer class par type dans GenerateCSharpCode et ...
//   ?? faire une classe qui remplace RunCode_v2()

namespace pb.Compiler
{
    partial class RunSource
    {
        //private bool _allowMultipleExecution = false;
        //private RunCode _runCode = null;
        //private List<RunCode> _runCodes = new List<RunCode>();
        private RunSourceInitEndMethods_v2 _runSourceInitEndMethods = new RunSourceInitEndMethods_v2();
        private ConcurrentDictionary<int, RunCode> _runCodes = new ConcurrentDictionary<int, RunCode>();
        private int _runCodeId = 0;
        private GenerateAssembly _generateAssembly = null;
        private Predicate<CompilerMessage> _messageFilter = null;

        private Action<EndRunCodeInfo> _endRunCode;
        public event OnPauseEvent OnPauseExecution;

        public OnAbortEvent OnAbortExecution { get; set; }
        public Action<EndRunCodeInfo> EndRunCode { get { return _endRunCode; } set { _endRunCode = value; } }
        //public bool AllowMultipleExecution { get { return _allowMultipleExecution; } set { _allowMultipleExecution = value; } }
        //public bool CallInit { get { return _runSourceInitEndMethods.CallInit; } set { _runSourceInitEndMethods.CallInit = value; } }
        public bool CallInitRunOnce { get { return _runSourceInitEndMethods.CallInitRunOnce; } }

        private void InitRunCode()
        {
            // Hidden CS8019 Unnecessary using directive
            _messageFilter = CompilerManager.GetMessageFilter("CS8019");
        }

        public bool IsRunning()
        {
            //return _runCode != null;
            return _runCodes.Count > 0;
        }

        public void PauseExecution(bool pause)
        {
            _executionPaused = pause;
            //if (OnPauseExecution != null)
            //    OnPauseExecution(pause);
            OnPauseExecution?.Invoke(pause);
        }

        public void AbortExecution(bool abort)
        {
            if (!IsRunning())
                return;
            _executionAborted = abort;
            if (abort && OnAbortExecution != null)
                OnAbortExecution();
        }

        public void ForceAbortExecution()
        {
            //if (_runCode != null && _runCode.RunThread != null)
            //    _runCode.RunThread.Abort();
            foreach (RunCode runCode in _runCodes.Values)
            {
                if (runCode.RunThread != null)
                    runCode.RunThread.Abort();
            }
            _runCodes.Clear();
        }

        public bool IsExecutionAlive()
        {
            //if (_runCode != null && _runCode.RunThread != null)
            //    return _runCode.RunThread.IsAlive;
            //else
            //    return false;
            foreach (RunCode runCode in _runCodes.Values)
            {
                if (runCode.RunThread != null)
                {
                    if (runCode.RunThread.IsAlive)
                        return true;
                }
            }
            return false;
        }

        public int GetRunningCount()
        {
            return _runCodes.Count;
        }

        //public void RunCode(string code, bool runOnMainThread = false, bool compileWithoutProject = false, bool allowMultipleRun = false, bool callInit = false)
        public async Task RunCode(string code, bool runOnMainThread = false, bool compileWithoutProject = false, bool allowMultipleRun = false, bool callInit = false)
        {
            await _RunCode(code, runOnMainThread: runOnMainThread, compileWithoutProject: compileWithoutProject, allowMultipleRun: allowMultipleRun, callInit: callInit);
        }

        //public void CompileCode(string code, bool compileWithoutProject = false)
        public async Task CompileCode(string code, bool compileWithoutProject = false)
        {
            await _RunCode(code, compileWithoutProject: compileWithoutProject, dontRunCode: true);
        }

        public void DeleteGeneratedAssemblies()
        {
            GetGenerateAssembly().DeleteGeneratedAssemblies();
        }

        private GenerateAssembly GetGenerateAssembly()
        {
            if (_generateAssembly == null)
            {
                _generateAssembly = new GenerateAssembly();
                _generateAssembly.GenerateAssemblyDirectory = GetRunSourceConfig().Get("GenerateAssemblyDirectory", "run").zRootPath(zapp.GetAppDirectory());
                _generateAssembly.GenerateAssemblyName = GetRunSourceConfig().Get("GenerateAssemblyName", "RunCode");
            }
            return _generateAssembly;
        }

        //private CompilerProject GetRunSourceConfigCompilerDefaultValues()
        //{
        //    return CompilerProject.Create(GetRunSourceConfig().zGetConfigElement("CompilerDefaultValues"));
        //}

        //private void _RunCode(string code, bool runOnMainThread = false, bool compileWithoutProject = false, bool allowMultipleRun = false, bool dontRunCode = false, bool callInit = false)
        private async Task _RunCode(string code, bool runOnMainThread = false, bool compileWithoutProject = false, bool allowMultipleRun = false, bool dontRunCode = false, bool callInit = false)
        {
            if (code == "")
                return;

            //if (!dontRunCode && _runCode != null)
            //    throw new PBException("error program already running");
            //if (!dontRunCode && _runCodes.Count > 0 && !_allowMultipleExecution)
            if (!dontRunCode && _runCodes.Count > 0 && !allowMultipleRun)
                throw new PBException("error program already running and multiple execution is not allowed");

            bool error = false;
            bool doEndRun = true;

            _refreshRunSourceConfig = true;
            _refreshProjectConfig = true;

            try
            {
                CompilerProjectReader projectReader = null;
                if (!compileWithoutProject)
                    projectReader = GetProjectCompilerProject();
                if (projectReader == null)
                {
                    projectReader = GetDefaultProject();
                    //Trace.WriteLine($"compile with default project \"{projectReader.GetProjectCompilerFile().File}\"");
                    Trace.WriteLine($"compile with default project \"{projectReader.ProjectFile}\"");
                }

                string assemblyFile = GetGenerateAssembly().GetNewAssemblyFile();

                GenerateCSharpCodeResult codeResult = RunCode_GenerateCode(code, projectReader, assemblyFile);

                ProjectCompiler compiler = RunCode_CompileCode(projectReader, assemblyFile, codeResult.SourceFile);

                //if (compiler.HasError())
                if (!compiler.Success)
                {
                    SetResult(compiler.GetCompilerMessagesDataTable());
                }
                else
                {
                    // trace warning
                    //compiler.TraceMessages(_messageFilter);
                    compiler.TraceMessages(message => _messageFilter(message) || message.FileName != codeResult.SourceFile);

                    if (!dontRunCode)
                    {
                        //RunCode_ExecuteCode(compiler.Results.LoadAssembly(), codeResult, projectReader, compiler, runOnMainThread, callInit);
                        await RunCode_ExecuteCode(compiler.Results.LoadAssembly(), codeResult, projectReader, compiler, runOnMainThread, callInit);
                        doEndRun = false;
                    }
                }
            }
            //catch
            //{
            //    error = true;
            //    throw;
            //}
            catch (Exception ex)
            {
                error = true;
                if (ex is ProjectCompilerException)
                {
                    Error.WriteMessage(ErrorOptions.TraceError, ex.Message);
                }
                else
                    throw;
            }
            finally
            {
                if (doEndRun)
                    RunCode_EndRun(null, error);
            }
        }

        private GenerateCSharpCodeResult RunCode_GenerateCode(string code, CompilerProjectReader compilerProject, string assemblyFilename)
        {
            GenerateCSharpCode generateCSharpCode = new GenerateCSharpCode(assemblyFilename);
            generateCSharpCode.RunTypeName = GetRunSourceConfig().Get("GenerateCodeRunTypeName", "_RunCode");  // "w"
            generateCSharpCode.RunMethodName = GetRunSourceConfig().Get("GenerateCodeRunMethodName", "Run");

            if (compilerProject != null)
            {
                generateCSharpCode.NameSpace = compilerProject.GetNameSpace();
                generateCSharpCode.AddUsings(compilerProject.GetUsings());
            }

            return generateCSharpCode.GenerateCode(code);
        }

        private ProjectCompiler RunCode_CompileCode(CompilerProjectReader projectReader, string assemblyFilename, string sourceFile)
        {
            ProjectCompiler compiler = new ProjectCompiler(CompilerManager.Current.Win32ResourceCompiler, CompilerManager.Current.ResourceCompiler);

            //try
            //{
            compiler.SetOutputAssembly(assemblyFilename + ".dll");
            //compiler.AddSource(new CompilerFile(sourceFile));
            compiler.AddSource(new CompilerFile { File = sourceFile });

            //if (projectReader != null)
            //    compiler.SetProjectCompilerFile(projectReader.GetProjectCompilerFile());

            // CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            //compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), runCode: true);
            compiler.SetParameters(projectReader, runCode: true);
            compiler.SetTarget("library");

            CompilerManager.Current.UpdateAssemblies(compiler.Assemblies.Values);
            compiler.Compile();

            return compiler;
        }

        // RunCode_ExecuteCode must throw an exception if he can't execute run method
        // if no error thrown RunCode_ExecuteCode must call RunCode_EndRun()
        //private void RunCode_ExecuteCode(Assembly assembly, GenerateCSharpCodeResult codeResult, CompilerProjectReader compilerProject, ProjectCompiler compiler, bool runOnMainThread, bool callInit)
        private async Task RunCode_ExecuteCode(Assembly assembly, GenerateCSharpCodeResult codeResult, CompilerProjectReader compilerProject, ProjectCompiler compiler, bool runOnMainThread, bool callInit)
        {
            RunCode runCode = new RunCode(++_runCodeId);
            runCode.RunAssembly = assembly;
            //runCode.CompilerAssemblies = compiler.Assemblies;
            runCode.RunMethodName = codeResult.GetFullRunMethodName();
            runCode.EndRun += error => RunCode_EndRun(runCode, error);

            //if (forceCallInit)
            //    _runSourceInitEndMethods.CallInit = true;

            _executionAborted = false;

            foreach (CompilerAssembly compilerAssembly in compiler.Assemblies.Values)
            {
                //WriteLine(2, "  Assembly              \"{0}\" resolve {1}", assembly.File, assembly.Resolve);
                if (compilerAssembly.Resolve)
                    AssemblyResolve.Add(compilerAssembly.File, compilerAssembly.ResolveName);
            }

            //_runSourceInitEndMethods.CallInit = callInit;
            //if (callInit)
            //_runSourceInitEndMethods.CallInitMethods(compilerProject.GetInitMethods(), compilerProject.GetEndMethods(), callInit, methodName => runCode.GetMethod(methodName));
            _runSourceInitEndMethods.CallInitMethods(compilerProject.GetInitMethods(), compilerProject.GetEndMethods(), callInit, methodName => zReflection.GetMethod(methodName, assembly, ErrorOptions.TraceWarning));

            // add runCode to _runCodes after call init, if call init fail runCode is not in _runCodes
            if (!_runCodes.TryAdd(runCode.Id, runCode))
                throw new PBException("unable to add RunCode id {0} to ConcurrentDictionary", runCode.Id);

            //runCode.Run(runOnMainThread);
            await runCode.Run_v2(runOnMainThread);

            // problem with AssemblyResolve.Clear() end method may need to resolve assembly
            //AssemblyResolve.Clear();
        }

        //private void RunCode_EndRun(bool error)
        private void RunCode_EndRun(RunCode runCode, bool error)
        {
            IChrono runCodeChrono;
            if (runCode != null)
            {
                runCodeChrono = runCode.RunChrono;
                //runCode = null;
            }
            else
            {
                runCodeChrono = new Chrono();
            }
            _executionPaused = false;
            _executionAborted = false;

            RunCode runCode2;
            bool errRemoveRunCode = false;
            if (runCode != null)
                errRemoveRunCode = !_runCodes.TryRemove(runCode.Id, out runCode2);
            //Trace.WriteLine("RunCode_EndRun : runCode.Id {0} errRemoveRunCode {1}", runCode.Id, errRemoveRunCode);

            _endRunCode?.Invoke(new EndRunCodeInfo { Error = error, RunCodeChrono = runCodeChrono });
            if (errRemoveRunCode)
                throw new PBException("unable to remove RunCode id {0} from ConcurrentDictionary", runCode.Id);
        }
    }
}

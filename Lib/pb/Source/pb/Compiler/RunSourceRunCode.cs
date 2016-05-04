using System;
using System.Reflection;
using pb.Data.Xml;
using pb.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;

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
    public partial class RunSource
    {
        private bool _executionPaused = false;
        private bool _executionAborted = false;
        private bool _allowMultipleExecution = false;
        //private RunCode _runCode = null;
        //private List<RunCode> _runCodes = new List<RunCode>();
        private RunSourceInitEndMethods _runSourceInitEndMethods = new RunSourceInitEndMethods();
        private ConcurrentDictionary<int, RunCode> _runCodes = new ConcurrentDictionary<int, RunCode>();
        private int _runCodeId = 0;
        private GenerateAssembly _generateAssembly = null;

        private Action<EndRunCodeInfo> _endRunCode;
        public event OnPauseEvent OnPauseExecution;

        public OnAbortEvent OnAbortExecution { get; set; }
        public Action<EndRunCodeInfo> EndRunCode { get { return _endRunCode; } set { _endRunCode = value; } }
        public bool AllowMultipleExecution { get { return _allowMultipleExecution; } set { _allowMultipleExecution = value; } }
        public bool CallInit { get { return _runSourceInitEndMethods.CallInit; } set { _runSourceInitEndMethods.CallInit = value; } }

        public bool IsRunning()
        {
            //return _runCode != null;
            return _runCodes.Count > 0;
        }

        public bool IsExecutionPaused()
        {
            return _executionPaused;
        }

        public void PauseExecution(bool pause)
        {
            _executionPaused = pause;
            if (OnPauseExecution != null)
                OnPauseExecution(pause);
        }

        public bool IsExecutionAborted()
        {
            return _executionAborted;
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
            //_runCodes.Clear();
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

        public void RunCode(string code, bool useNewThread = true, bool compileWithoutProject = false)
        {
            _RunCode(code, useNewThread, compileWithoutProject);
        }

        public void CompileCode(string code, bool compileWithoutProject = false)
        {
            _RunCode(code, compileWithoutProject: compileWithoutProject, dontRunCode: true);
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

        private CompilerProject GetRunSourceConfigCompilerDefaultValues()
        {
            return CompilerProject.Create(GetRunSourceConfig().zGetConfigElement("CompilerDefaultValues"));
        }

        private void _RunCode(string code, bool useNewThread = true, bool compileWithoutProject = false, bool dontRunCode = false)
        {
            if (code == "")
                return;

            //if (!dontRunCode && _runCode != null)
            //    throw new PBException("error program already running");
            //_allowMultipleExecution
            if (!dontRunCode && _runCodes.Count > 0 && !_allowMultipleExecution)
                throw new PBException("error program already running and multiple execution is not allowed");

            bool error = false;
            bool doEndRun = true;

            _refreshRunSourceConfig = true;
            _refreshProjectConfig = true;

            try
            {
                CompilerProject compilerProject = null;
                if (!compileWithoutProject)
                    compilerProject = GetProjectCompilerProject();
                if (compilerProject == null)
                    compilerProject = GetDefaultProject();

                string assemblyFile = GetGenerateAssembly().GetNewAssemblyFile();

                GenerateCSharpCodeResult codeResult = RunCode_GenerateCode(code, compilerProject, assemblyFile);

                Compiler compiler = RunCode_CompileCode(compilerProject, assemblyFile, codeResult.SourceFile);

                if (compiler.HasError())
                {
                    SetResult(compiler.GetCompilerMessagesDataTable());
                }
                else
                {
                    // trace warning
                    compiler.TraceMessages();

                    if (!dontRunCode)
                    {
                        RunCode_ExecuteCode(compiler.Results.CompiledAssembly, codeResult, compilerProject, compiler, useNewThread);
                        doEndRun = false;
                    }
                }
            }
            catch
            {
                error = true;
                throw;
            }
            finally
            {
                if (doEndRun)
                    RunCode_EndRun(null, error);
            }
        }

        private GenerateCSharpCodeResult RunCode_GenerateCode(string code, CompilerProject compilerProject, string assemblyFilename)
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

        private Compiler RunCode_CompileCode(CompilerProject compilerProject, string assemblyFilename, string sourceFile)
        {
            Compiler compiler = new Compiler();
            compiler.SetOutputAssembly(assemblyFilename);
            compiler.AddSource(new CompilerFile(sourceFile));

            if (compilerProject != null)
                compiler.SetProjectCompilerFile(compilerProject.GetProjectCompilerFile());

            // CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), runCode: true);
            compiler.SetParameters(compilerProject, runCode: true);

            compiler.Compile();

            return compiler;
        }

        // RunCode_ExecuteCode must throw an exception if he can't execute run method
        // if no error thrown RunCode_ExecuteCode must call RunCode_EndRun()
        private void RunCode_ExecuteCode(Assembly assembly, GenerateCSharpCodeResult codeResult, CompilerProject compilerProject, Compiler compiler, bool useNewThread)
        {
            //_runCode = new RunCode();
            RunCode runCode = new RunCode(++_runCodeId);
            runCode.RunAssembly = assembly;
            runCode.CompilerAssemblies = compiler.Assemblies;
            runCode.RunMethodName = codeResult.GetFullRunMethodName();
            //runCode.InitMethodName = compilerProject.GetInitMethod();  // "Init"
            //runCode.EndMethodName = compilerProject.GetEndMethod();    // "End"
            runCode.EndRun += error => RunCode_EndRun(runCode, error);
            if (!_runCodes.TryAdd(runCode.Id, runCode))
                throw new PBException("unable to add RunCode id {0} to ConcurrentDictionary", runCode.Id);

            _executionAborted = false;

            _runSourceInitEndMethods.CallInitMethods(compilerProject.GetInitMethods(), compilerProject.GetEndMethods(), methodName => runCode.GetMethod(methodName));

            runCode.Run(useNewThread);
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

            if (_endRunCode != null)
                _endRunCode(new EndRunCodeInfo { Error = error, RunCodeChrono = runCodeChrono });
            if (errRemoveRunCode)
                throw new PBException("unable to remove RunCode id {0} from ConcurrentDictionary", runCode.Id);
        }
    }
}

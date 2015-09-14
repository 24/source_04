using System;
using System.Data;
using System.Reflection;
using System.Threading;
using pb.Data.Xml;
using pb.IO;

// todo
//   ok gérer gExecutionThread
//   ok DeleteGeneratedAssemblies_v2()
//   ok gérer Chrono
//   ok EndRun retourner le Chrono
//   ok CompileCode_v2()
//   ok paramétrer Init et End dans CompileProject, utilisation des qualified name pour le type
//   ok pb si error ds compile runsource l'onglet message est affiché et pas l'onglet result
//   ok remplacer class par type dans GenerateCSharpCode et ...
//   faire une classe qui remplace RunCode_v2()

// $$RunSourceRunCode_v1

namespace pb.Compiler
{
    public partial class RunSource
    {
        // $$RunSourceRunCode_v1
        //public bool UseNewRunCode = true;

        private bool _executionPaused = false;
        private bool _executionAborted = false;
        private RunCode _runCode = null;
        //private Chrono _runCodeChrono = null;  // new Chrono()
        //private static string __defaultGenerateAssemblySubdirectory = "run";     // c:\pib\prog\tools\runsource\exe\run
        //private static string __defaultGenerateAssemblyName = "RunSource";       // RunSource_00001.cs RunSource_00001.dll RunSource_00001.pdb
        private GenerateAssembly _generateAssembly = null;

        //public event EndRunEvent EndRun;
        private Action<EndRunCodeInfo> _endRunCode;
        public event OnPauseEvent OnPauseExecution;

        public OnAbortEvent OnAbortExecution { get; set; }
        //public IChrono RunCodeChrono { get { if (_runCode != null) return _runCode.RunChrono; else return _runCodeChrono; } }
        //public IChrono RunCodeChrono { get { return _runCodeChrono; } }
        public Action<EndRunCodeInfo> EndRunCode { get { return _endRunCode; } set { _endRunCode = value; } }

        public bool IsRunning()
        {
            // $$RunSourceRunCode_v1
            //return _runCode != null || _runCodeThread != null;
            return _runCode != null;
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
            //if (gExecutionThread == null)
            //    return;
            if (!IsRunning())
                return;
            _executionAborted = abort;
            if (abort && OnAbortExecution != null)
                OnAbortExecution();
        }

        public void ForceAbortExecution()
        {
            if (_runCode != null && _runCode.RunThread != null)
                _runCode.RunThread.Abort();
            // $$RunSourceRunCode_v1
            //if (_runCodeThread != null)
            //    _runCodeThread.Abort();
        }

        public bool IsExecutionAlive()
        {
            if (_runCode != null && _runCode.RunThread != null)
                return _runCode.RunThread.IsAlive;
            // $$RunSourceRunCode_v1
            //if (_runCodeThread != null)
            //    return _runCodeThread.IsAlive;
            else
                return false;
        }

        public void RunCode(string code, bool useNewThread = true, bool compileWithoutProject = false)
        {
            //if (!UseNewRunCode)
            //    // $$RunSourceRunCode_v1
            //    RunCode_v1(code, useNewThread, compileWithoutProject);
            //else
                _RunCode(code, useNewThread, compileWithoutProject);
        }

        public void CompileCode(string code, bool compileWithoutProject = false)
        {
            //if (!UseNewRunCode)
            //    // $$RunSourceRunCode_v1
            //    CompileCode_v1(code, compileWithoutProject);
            //else
                _RunCode(code, compileWithoutProject: compileWithoutProject, dontRunCode: true);
        }

        public void DeleteGeneratedAssemblies()
        {
            //if (!UseNewRunCode)
            //    // $$RunSourceRunCode_v1
            //    DeleteGeneratedAssemblies_v1();
            //else
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

            // $$RunSourceRunCode_v1
            //if (!dontRunCode && (_runCode != null || _runCodeThread != null))
            if (!dontRunCode && _runCode != null)
                throw new PBException("error program already running");

            bool error = false;
            bool doEndRun = true;

            _refreshRunSourceConfig = true;
            _refreshProjectConfig = true;

            //_runCodeChrono = new Chrono();
            try
            {
                CompilerProject compilerProject = null;
                if (!compileWithoutProject)
                    compilerProject = GetProjectCompilerProject();
                if (compilerProject == null)
                    compilerProject = GetDefaultProject();

                //string assemblyFile = RunCode_GetNewAssemblyFile();
                string assemblyFile = GetGenerateAssembly().GetNewAssemblyFile();

                GenerateCSharpCodeResult codeResult = RunCode_GenerateCode(code, compilerProject, assemblyFile);

                Compiler compiler = RunCode_CompileCode(compilerProject, assemblyFile, codeResult.SourceFile);

                if (compiler.HasError())
                {
                    //SetResultCompilerMessages(compiler);
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
                //if (error && EndRun != null)
                //    EndRun(error);
                // call RunCode_EndRun() only if exception has bin catched (error = true)
                //if (error || dontRunCode)
                //    RunCode_EndRun(error);
                if (doEndRun)
                    RunCode_EndRun(error);
            }
        }

        //private static string RunCode_GetNewAssemblyFile()
        //{
        //    // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"
        //    if (!zDirectory.Exists(__defaultGenerateAssemblySubdirectory))
        //        zDirectory.CreateDirectory(__defaultGenerateAssemblySubdirectory);
        //    int i = zfile.GetLastFileNameIndex(__defaultGenerateAssemblySubdirectory) + 1;
        //    return zPath.Combine(__defaultGenerateAssemblySubdirectory, __defaultGenerateAssemblyName + string.Format("_{0:00000}", i));
        //}

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
                compiler.DefaultDir = zPath.GetDirectoryName(compilerProject.ProjectFile);

            // CompilerDefaultValues from runsource.runsource.config.xml runsource.runsource.config.local.xml
            compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), dontSetOutput: true);
            compiler.SetParameters(compilerProject, dontSetOutput: true);

            compiler.Compile();

            return compiler;
        }

        // RunCode_ExecuteCode must throw an exception if he can't execute run method
        // if no error thrown RunCode_ExecuteCode must call RunCode_EndRun()
        private void RunCode_ExecuteCode(Assembly assembly, GenerateCSharpCodeResult codeResult, CompilerProject compilerProject, Compiler compiler, bool useNewThread)
        {
            _runCode = new RunCode();
            _runCode.RunAssembly = assembly;
            _runCode.CompilerAssemblies = compiler.Assemblies;
            //_runCode.RunClassName = codeResult.GetFullClassName();
            //_runCode.RunMethodName = codeResult.RunMethodName;
            _runCode.RunMethodName = codeResult.GetFullRunMethodName();
            _runCode.InitMethodName = compilerProject.GetInitMethod();  // "Init"
            _runCode.EndMethodName = compilerProject.GetEndMethod();    // "End"
            _runCode.EndRun += RunCode_EndRun;

            _executionAborted = false;

            _runCode.Run(useNewThread);
            //_runCodeThread = runCode.RunThread;
        }

        private void RunCode_EndRun(bool error)
        {
            IChrono runCodeChrono;
            if (_runCode != null)
            {
                //_runCodeChrono = _runCode.RunChrono;
                runCodeChrono = _runCode.RunChrono;
                _runCode = null;
            }
            else
            {
                //_runCodeChrono = new Chrono();
                runCodeChrono = new Chrono();
            }
            _executionPaused = false;
            _executionAborted = false;
            //if (EndRun != null)
            //    EndRun(error);
            if (_endRunCode != null)
                _endRunCode(new EndRunCodeInfo { Error = error, RunCodeChrono = runCodeChrono });
        }

        //private void SetResultCompilerMessages(ICompiler compiler)
        //{
        //    DataTable messages = compiler.GetCompilerMessagesDataTable();
        //    if (messages != null)
        //    {
        //        gdtResult = messages;
        //        gdsResult = null;
        //        gsXmlResultFormat = null;
        //        if (ErrorResultSet != null)
        //            ErrorResultSet(gdtResult, null);
        //    }
        //}
    }
}

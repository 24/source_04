using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using pb.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace runsourced
{
    partial class RunSourceForm_v3
    {
        public SetRestartRunsourceEvent SetRestartRunsource;
        private IRunSource _runSource = null;

        private void InitRunSource()
        {
            _runSource.SetRunSourceConfig(_config.ConfigFile);
            _runSource.StartAssemblyResolve();
            _runSource.DeleteGeneratedAssemblies();
            _runSource.DisableMessageChanged += EventDisableMessageChanged;
            _runSource.GridResultSetDataTable += EventGridResultSetDataTable;
            _runSource.GridResultSetDataSet += EventGridResultSetDataSet;
            _runSource.ProgressChange += EventProgressChange;
            _runSource.EndRunCode += EventEndRunCode;
            _runSource.Progress_MinimumMillisecondsBetweenMessage = _config.Get("ProgressMinimumMillisecondsBetweenMessage").zTryParseAs(_runSource.Progress_MinimumMillisecondsBetweenMessage);
        }

        private void EndRunSource()
        {
            if (_runSource != null)
            {
                _runSource.DisableMessageChanged -= EventDisableMessageChanged;
                _runSource.GridResultSetDataTable -= EventGridResultSetDataTable;
                _runSource.GridResultSetDataSet -= EventGridResultSetDataSet;
                _runSource.ProgressChange -= EventProgressChange;
                _runSource.EndRunCode -= EventEndRunCode;
                _runSource = null;
            }
        }

        private void SetSourceFile(string file)
        {
            _runSource.SourceFile = file;
            _runSource.SetProject(file);
        }

        private bool GetCallInit()
        {
            return _runSource.CallInit;
        }

        private void SetCallInit(bool callInit)
        {
            _runSource.CallInit = callInit;
        }

        private bool GetAllowMultipleExecution()
        {
            return _runSource.AllowMultipleExecution;
        }

        private void SetAllowMultipleExecution(bool allowMultipleExecution)
        {
            _runSource.AllowMultipleExecution = allowMultipleExecution;
        }


        private void _RunCode(bool useNewThread = true, bool compileWithoutProject = false)
        {
            if (_runSource.IsRunning() && !_runSource.AllowMultipleExecution)
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (GetCode() == "")
            {
                MessageBox.Show("Aucune instruction sélectionnée !", "Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_runSource.SourceFile != null)
            {
                var file = zFile.CreateFileInfo(_runSource.SourceFile);
                if ((file.Attributes & FileAttributes.ReadOnly) == 0)
                    Save();
            }

            RazResult();
            RazProgress();

            _stopButton.Enabled = true;
            _runSource.RunCode(GetCode(), useNewThread, compileWithoutProject);
            _menuRunInit.Checked = _runSource.CallInit;
        }

        private void CompileCode()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (_runSource.SourceFile == null)
                SetSourceFile(SelectSaveFile(_runSource.SourceFile));
            if (_runSource.SourceFile != null)
            {
                var file = zFile.CreateFileInfo(_runSource.SourceFile);
                if ((file.Attributes & FileAttributes.ReadOnly) == 0)
                    Save();
            }

            RazResult();
            RazProgress();
            _executeButton.Enabled = false;
            string s = GetCode();
            _runSource.CompileCode(s);
        }

        private void AbortExecution()
        {
            DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "Run source", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
            if (r == DialogResult.OK)
                Try(AbortThreadExecution);
        }

        private void AbortThreadExecution()
        {
            bool cancel;
            AbortThreadExecution(out cancel);
        }

        private void AbortThreadExecution(out bool cancel)
        {
            cancel = false;
            int timeout = _config.Get("AbortThreadExecutionTimeout").zTryParseAs(30);
            _trace.WriteLine("Abort execution process (Timeout {0} sec)", timeout);
            _runSource.AbortExecution(true);
            DateTime dt = DateTime.Now;
            while (true)
            {
                while (true)
                {
                    Application.DoEvents();
                    if (!_runSource.IsRunning())
                    {
                        _trace.WriteLine("Execution process aborted");
                        return;
                    }
                    TimeSpan ts = DateTime.Now.Subtract(dt);
                    if (ts.Seconds > timeout) break;
                    Thread.Sleep(50);
                }
                DialogResult r = MessageBox.Show("Le programme est toujours en cours d'exécution. Voulez-vous forcer l'interruption ?", "RunSource",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Cancel)
                {
                    _runSource.AbortExecution(false);
                    cancel = true;
                    return;
                }
                if (r == DialogResult.Yes)
                    break;
            }
            if (!_runSource.IsRunning())
            {
                _trace.WriteLine("No thread execution process");
                return;
            }
            timeout = _config.Get("ForceAbortThreadExecutionTimeout").zTryParseAs(30);
            _trace.WriteLine("Force abort of execution process (Timeout {0})", timeout);
            _runSource.ForceAbortExecution();
            dt = DateTime.Now;
            while (true)
            {
                Application.DoEvents();
                if (!_runSource.IsExecutionAlive())
                    break;
                Thread.Sleep(100);
                TimeSpan ts = DateTime.Now.Subtract(dt);
                if (ts.Seconds > timeout)
                {
                    _trace.WriteLine("Impossible to abort execution process");
                    MessageBox.Show("Impossible to abort execution process.", "RunSource", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cancel = true;
                    return;
                }
            }
            _trace.WriteLine("Execution process aborted");
        }

        private void _UpdateRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            CompileRunSource();
            RestartRunSource();
        }

        private void _CompileRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            CompileRunSource();
        }

        private void CompileRunSource()
        {
            Chrono chrono = new Chrono();
            chrono.Start();
            int nbProject = 0;
            try
            {
                string updateDir = _config.GetExplicit("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
                Dictionary<string, List<string>> projectFiles = new Dictionary<string, List<string>>();

                foreach (XElement project in _config.GetElements("UpdateRunSource/Project"))
                {
                    ICompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
                    if (compiler.HasError())
                        return;
                    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
                    if (copyOutput != null)
                    {
                        //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                        compiler.CopyResultFilesToDirectory(copyOutput);
                    }
                    //_trace.WriteLine("  copy result files to directory \"{0}\"", updateDir);
                    compiler.CopyResultFilesToDirectory(updateDir);
                    nbProject++;
                }

                foreach (XElement project in _config.GetElements("UpdateRunSource/ProjectRunSourceLaunch"))
                {
                    ICompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
                    if (compiler.HasError())
                        return;
                    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
                    if (copyOutput != null)
                    {
                        //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                        compiler.CopyResultFilesToDirectory(copyOutput);
                    }
                    nbProject++;
                }
            }
            finally
            {
                chrono.Stop();
                _trace.WriteLine("{0} project(s) compiled", nbProject);
                _trace.WriteLine("Process completed {0}", chrono.TotalTimeString);
            }
        }

        private ICompiler CompileProject(string projectPath)
        {
            ICompiler compiler = _runSource.CompileProject(projectPath);
            if (compiler.HasError())
            {
                DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
                if (dtMessage != null)
                    SetResult(dtMessage, null);
                EventEndRunCode(new EndRunCodeInfo { Error = false });
            }
            return compiler;
        }

        private void EventEndRunCode(EndRunCodeInfo endRunCodeInfo)
        {
            if (InvokeRequired)
            {
                // Error 13 Cannot convert lambda expression to type 'System.Delegate' because it is not a delegate type
                //Invoke(() => { EventEndRunCode(endRunCodeInfo); });
                Invoke((Action)(() => EventEndRunCode(endRunCodeInfo)));
            }
            else
            {
                try
                {
                    _trace.Write("Process completed ");
                    if (endRunCodeInfo.RunCodeChrono != null)
                        _trace.Write(endRunCodeInfo.RunCodeChrono.TotalTimeString);
                    else
                        _trace.Write("----");
                    _trace.WriteLine();
                }
                catch (Exception ex)
                {
                    _trace.WriteError(ex);
                    zerrf.ErrorMessageBox(ex);
                }

                try
                {

                    if (_newDataTableResult)
                        ViewDataTableResult();
                    if (_newTreeViewResult)
                        ViewTreeViewResult();

                    bool error = endRunCodeInfo.Error;

                    // on sélectionne l'onglet sauf si :
                    //   - DontSelectResultTab = true, _errorResult = false et error = false
                    if (!_runSource.DontSelectResultTab || error)
                    {
                        if (_selectTreeViewResult && !error)
                            SelectTreeResultTab();
                        else if (_newDataTableResult && !error)
                        {
                            if (_xmlResultFormat != null || _dataSetResult != null)
                                SelectGrid1ResultTab();
                            else
                                SelectGridResultTab();
                        }
                        else if (_newTreeViewResult && !error)
                            SelectTreeResultTab();
                        else
                            SelectMessageResultTab();
                    }
                }
                catch (Exception ex)
                {
                    _trace.WriteError(ex);
                }

                _executeButton.Enabled = true;
                _pauseButton.Text = "&Pause";
                _stopButton.Enabled = _runSource.IsRunning();

                SetFormTitle();
            }
        }

        private void _RestartRunSource()
        {
            if (_runSource.IsRunning())
            {
                MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RazResult();
            RazProgress();
            RestartRunSource();
        }

        private void RestartRunSource()
        {
            if (SetRestartRunsource != null)
                SetRestartRunsource(new RunSourceRestartParameters { SourceFile = _runSource.SourceFile, SelectionStart = _source.SelectionStart, SelectionEnd = _source.SelectionEnd });
            this.Close();
        }
    }
}

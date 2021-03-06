﻿using pb;
using pb.Compiler;
using pb.IO;
using pb.Windows.Forms;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceFormExe
    {
        private IRunSource _runSource = null;
        private RemoteRunSource _remoteRunSource = null;
        public SetRestartRunsourceEvent SetRestartRunsource;

        private void InitRunSource()
        {
            //_runSource.SetRunSourceConfig(_config.ConfigFile);
            _runSource.Init(_config.ConfigFile);
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
            return _runSource.CallInitRunOnce;
        }

        //private void SetCallInit(bool callInit)
        //{
        //    _runSource.CallInit = callInit;
        //}

        //private bool GetAllowMultipleExecution()
        //{
        //    return _runSource.AllowMultipleExecution;
        //}

        //private void SetAllowMultipleRun(bool allowMultipleRun)
        //{
        //    _runSource.AllowMultipleExecution = allowMultipleRun;
        //}


        //private void _RunCode(bool runOnMainThread = false, bool runWithoutProject = false)
        private async Task _RunCode(bool runOnMainThread = false, bool runWithoutProject = false)
        {
            if (!runOnMainThread)
                runOnMainThread = _menuRunOnMainThread.Checked;
            if (!runWithoutProject)
                runWithoutProject = _menuRunWithoutProject.Checked;
            bool allowMultipleRun = _menuAllowMultipleRun.Checked;
            bool callInit = _menuRunInit.Checked;
            //if (_runSource.IsRunning() && !_runSource.AllowMultipleExecution)
            if (_runSource.IsRunning() && !allowMultipleRun)
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

            //_trace.WriteLine("_RunCode {0} {1} {2} {3}", runOnMainThread, allowMultipleRun, runWithoutProject, forceCallInit);
            Trace.WriteLine(GetRunSourceStatusRunType(runOnMainThread, allowMultipleRun, runWithoutProject, callInit));

            _stopButton.Enabled = true;
            await _runSource.RunCode(GetCode(), runOnMainThread: runOnMainThread, compileWithoutProject: runWithoutProject, allowMultipleRun: allowMultipleRun, callInit: callInit);
            _menuRunInit.Checked = _runSource.CallInitRunOnce;
            UpdateRunSourceStatus();
        }

        private string GetRunSourceStatusRunType(bool runOnMainThread, bool allowMultipleRun, bool runWithoutProject, bool callInit = false)
        {
            //run init, running
            // run, run on main thread, run multiple, [without project]
            string status;
            if (runOnMainThread)
                status = "run on main thread";
            else if (allowMultipleRun)
                status = "run multiple";
            else
                status = "run";
            if (runWithoutProject)
                status += ", without project";
            if (callInit)
                status += ", call init";
            return status;
        }

        //private void CompileCode()
        private async Task CompileCode()
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
            await _runSource.CompileCode(s);
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
            Trace.WriteLine("Abort execution process (Timeout {0} sec)", timeout);
            _runSource.AbortExecution(true);
            DateTime dt = DateTime.Now;
            while (true)
            {
                while (true)
                {
                    Application.DoEvents();
                    if (!_runSource.IsRunning())
                    {
                        Trace.WriteLine("Execution process aborted");
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
                Trace.WriteLine("No thread execution process");
                return;
            }
            timeout = _config.Get("ForceAbortThreadExecutionTimeout").zTryParseAs(30);
            Trace.WriteLine("Force abort of execution process (Timeout {0})", timeout);
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
                    Trace.WriteLine("Impossible to abort execution process");
                    MessageBox.Show("Impossible to abort execution process.", "RunSource", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cancel = true;
                    return;
                }
            }
            Trace.WriteLine("Execution process aborted");
            UpdateRunSourceFormLayout();
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
            //CompileRunSource();
            CompileProjects(_config.GetExplicit("RunSourceProjects"));
            RestartRunSource();
        }

        private void _CompileRunSource()
        {
            //if (_runSource.IsRunning())
            //{
            //    MessageBox.Show("Un programme est déjà en cours d'exécution !", "Compile", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            RazResult();
            RazProgress();
            //CompileRunSource();
            CompileProjects(_config.GetExplicit("RunSourceProjects"));
        }

        //private void CompileRunSource()
        //{
        //    if (!_runSource.CompileProjects(_config.GetExplicit("RunSourceProjects"), _config.GetExplicit("RunsourceSourceDirectory").zRootPath(zapp.GetEntryAssemblyDirectory())))
        //        EventEndRunCode(new EndRunCodeInfo { Error = false });
        //}

        private void _CompileProjects(string projectsFile)
        {
            RazResult();
            RazProgress();
            CompileProjects(projectsFile);
        }

        private void CompileProjects(string projectsFile)
        {
            if (!_runSource.CompileProjects(projectsFile, _config.GetExplicit("RunsourceSourceDirectory").zRootPath(zapp.GetEntryAssemblyDirectory())))
                EventEndRunCode(new EndRunCodeInfo { Error = false });
        }

        //private void CompileRunSource()
        //{
        //    Chrono chrono = new Chrono();
        //    chrono.Start();
        //    int nbProject = 0;
        //    try
        //    {
        //        string updateDir = _config.GetExplicit("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
        //        //Dictionary<string, List<string>> projectFiles = new Dictionary<string, List<string>>();

        //        foreach (XElement project in _config.GetElements("UpdateRunSource/Project"))
        //        {
        //            IProjectCompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
        //            if (!compiler.Success)
        //                return;
        //            string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
        //            if (copyOutput != null)
        //            {
        //                //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
        //                compiler.CopyResultFilesToDirectory(copyOutput);
        //            }
        //            if (project.zAttribValue("copyToUpdateDirectory").zTryParseAs(false))
        //            {
        //                //_trace.WriteLine("  copy result files to directory \"{0}\"", updateDir);
        //                compiler.CopyResultFilesToDirectory(updateDir);
        //            }
        //            nbProject++;
        //        }

        //        //foreach (XElement project in _config.GetElements("UpdateRunSource/ProjectRunSourceLaunch"))
        //        //{
        //        //    IProjectCompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
        //        //    if (!compiler.Success)
        //        //        return;
        //        //    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
        //        //    if (copyOutput != null)
        //        //    {
        //        //        //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
        //        //        compiler.CopyResultFilesToDirectory(copyOutput);
        //        //    }
        //        //    nbProject++;
        //        //}
        //    }
        //    finally
        //    {
        //        chrono.Stop();
        //        _trace.WriteLine("{0} project(s) compiled", nbProject);
        //        _trace.WriteLine("Process completed {0}", chrono.TotalTimeString);
        //    }
        //}

        //private IProjectCompiler CompileProject(string projectPath)
        //{
        //    IProjectCompiler compiler = _runSource.CompileProject(projectPath);
        //    //if (compiler.HasError())
        //    if (!compiler.Success)
        //    {
        //        DataTable dtMessage = compiler.GetCompilerMessagesDataTable();
        //        if (dtMessage != null)
        //            SetResult(dtMessage, null);
        //        EventEndRunCode(new EndRunCodeInfo { Error = false });
        //    }
        //    return compiler;
        //}

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
                    Trace.Write("Process completed ");
                    if (endRunCodeInfo.RunCodeChrono != null)
                        Trace.Write(endRunCodeInfo.RunCodeChrono.TotalTimeString);
                    else
                        Trace.Write("----");
                    Trace.WriteLine();
                }
                catch (Exception ex)
                {
                    Trace.WriteError(ex);
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
                    Trace.WriteError(ex);
                }

                //_executeButton.Enabled = true;
                //_pauseButton.Text = "&Pause";
                //_stopButton.Enabled = _runSource.IsRunning();

                //SetFormTitle();
                //UpdateRunSourceStatus();
                UpdateRunSourceFormLayout();
            }
        }

        private void UpdateRunSourceFormLayout()
        {
            _executeButton.Enabled = true;
            _pauseButton.Text = "&Pause";
            _stopButton.Enabled = _runSource.IsRunning();

            SetFormTitle();
            UpdateRunSourceStatus();
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

        private void UpdateCurrentVSProject(bool simulate = false)
        {
            if (_runSource.ProjectFile == null)
            {
                Trace.WriteLine("no project defined");
                return;
            }
            UpdateVSProject(_runSource.ProjectFile, simulate);
        }

        private void UpdateVSProject(string runSourceProject, bool simulate = false)
        {
            //Trace.WriteLine($"UpdateVSProject() : simulate {simulate}");
            //return;
            DateTime start = DateTime.Now;
            // _vsProjectAddSource _vsProjectRemoveSource _vsProjectAddSourceLink _vsProjectRemoveSourceLink _vsProjectAddAssemblyReference _vsProjectRemoveAssemblyReference
            VSProjectUpdateOptions options = VSProjectUpdateOptions.BackupVSProject;
            if (_vsProjectAddSource.Checked)
                options |= VSProjectUpdateOptions.AddSource;
            if (_vsProjectRemoveSource.Checked)
                options |= VSProjectUpdateOptions.RemoveSource;
            if (_vsProjectAddSourceLink.Checked)
                options |= VSProjectUpdateOptions.AddSourceLink;
            if (_vsProjectRemoveSourceLink.Checked)
                options |= VSProjectUpdateOptions.RemoveSourceLink;
            if (_vsProjectAddAssemblyReference.Checked)
                options |= VSProjectUpdateOptions.AddAssemblyReference;
            if (_vsProjectRemoveAssemblyReference.Checked)
                options |= VSProjectUpdateOptions.RemoveAssemblyReference;
            if (options == VSProjectUpdateOptions.BackupVSProject)
            {
                Trace.WriteLine("no operation selected");
                return;
            }
            if (simulate)
                options |= VSProjectUpdateOptions.Simulate;
            _remoteRunSource.CreateRunSourceVSProjectManager().UpdateVSProject(runSourceProject, options);
            Trace.WriteLine($@"Process completed {DateTime.Now - start:hh\:mm\:ss\.fff}");
        }
    }
}

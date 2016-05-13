using pb.Windows.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using HWND = System.IntPtr;

namespace runsourced
{
    partial class RunSourceForm_v3
    {
        private IntPtr _hwnd = IntPtr.Zero;
        private const int WM_CUSTOM_NEW = WM.WM_USER;
        private const int WM_CUSTOM_OPEN_FILE = WM.WM_USER + 1;
        private const int WM_CUSTOM_SAVE = WM.WM_USER + 2;
        private const int WM_CUSTOM_SAVE_AS = WM.WM_USER + 3;

        private const int WM_CUSTOM_RUN_CODE = WM.WM_USER + 4;
        private const int WM_CUSTOM_RUN_CODE_ON_MAIN_THREAD = WM.WM_USER + 5;
        private const int WM_CUSTOM_RUN_CODE_WITHOUT_PROJECT = WM.WM_USER + 6;
        private const int WM_CUSTOM_COMPILE_CODE = WM.WM_USER + 7;
        private const int WM_CUSTOM_COMPILE_RUNSOURCE = WM.WM_USER + 8;

        private const int WM_CUSTOM_RESTART_RUNSOURCE = WM.WM_USER + 9;
        private const int WM_CUSTOM_UPDATE_RUNSOURCE = WM.WM_USER + 10;
        private const int WM_CUSTOM_ABORT_EXECUTION = WM.WM_USER + 11;

        [DllImport("user32")]
        public static extern int PostMessage(HWND hwnd, int wMsg, int wParam, int lParam);

        private void RunSourceForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(string.Format("fWRun_KeyDown : {0} Ctrl={1} Shift={2} Alt={3}", e.KeyCode.ToString(), e.Control, e.Shift, e.Alt));
            //Trace.WriteLine("fWRun_KeyDown : {0} Ctrl={1} Shift={2} Alt={3}", e.KeyCode.ToString(), e.Control, e.Shift, e.Alt);
            switch (e.KeyCode)
            {
                case Keys.F5:
                    if (!e.Alt && !e.Control && !e.Shift)
                        //Exe(new fExe(RunCode));
                        PostMessage(_hwnd, WM_CUSTOM_RUN_CODE, 0, 0);
                    else if (!e.Alt && !e.Control && e.Shift)
                        //Exe(new fExe(RunCodeOnMainThread));
                        PostMessage(_hwnd, WM_CUSTOM_RUN_CODE_ON_MAIN_THREAD, 0, 0);
                    else if (!e.Alt && e.Control && !e.Shift)
                        //Exe(new fExe(RunCodeWithoutProject));
                        PostMessage(_hwnd, WM_CUSTOM_RUN_CODE_WITHOUT_PROJECT, 0, 0);
                    break;
                //case Keys.Escape:
                //    if (!e.Alt && !e.Control && !e.Shift && gbThreadExecutionRunning)
                //    {
                //        DialogResult r = MessageBox.Show("Voulez-vous interrompre l'exécution du programme ?", "WRun", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                //        if (r == DialogResult.OK) Exe(new fExe(AbortThreadExecution));
                //    }
                //    break;
                case Keys.A:
                    if (!e.Alt && e.Control && !e.Shift)
                        //Exe(new fExe(SaveAs));
                        PostMessage(_hwnd, WM_CUSTOM_SAVE_AS, 0, 0);
                    break;
                case Keys.B:
                    if (!e.Alt && e.Control && e.Shift)
                        //Exe(new fExe(CompileCode));
                        PostMessage(_hwnd, WM_CUSTOM_COMPILE_CODE, 0, 0);
                    break;
                case Keys.C:
                    // Ctrl-C
                    if (!e.Alt && e.Control && !e.Shift && _runSource.IsRunning())
                        //AbortExecution();
                        PostMessage(_hwnd, WM_CUSTOM_ABORT_EXECUTION, 0, 0);
                    // Shift-Ctrl-C
                    else if (!e.Alt && e.Control && e.Shift)
                        //Exe(new fExe(_CompileRunSource));
                        PostMessage(_hwnd, WM_CUSTOM_COMPILE_RUNSOURCE, 0, 0);
                    break;
                case Keys.N:
                    if (!e.Alt && e.Control && !e.Shift)
                        //Exe(new fExe(New));
                        PostMessage(_hwnd, WM_CUSTOM_NEW, 0, 0);
                    break;
                case Keys.O:
                    if (!e.Alt && e.Control && !e.Shift)
                        //Exe(new fExe(Open));
                        PostMessage(_hwnd, WM_CUSTOM_OPEN_FILE, 0, 0);
                    break;
                case Keys.R:  // désactiver Shift-Ctrl-R dans 4t Tray minimizer 02/10/2014
                    // Shift-Ctrl-R
                    if (!e.Alt && e.Control && e.Shift)
                        //Exe(new fExe(_RestartRunSource));
                        PostMessage(_hwnd, WM_CUSTOM_RESTART_RUNSOURCE, 0, 0);
                    break;
                case Keys.S:
                    if (!e.Alt && e.Control && !e.Shift)
                        //Exe(new fExe(Save));
                        PostMessage(_hwnd, WM_CUSTOM_SAVE, 0, 0);
                    break;
                case Keys.U:
                    if (!e.Alt && e.Control && e.Shift)
                        //Exe(new fExe(_UpdateRunSource));
                        PostMessage(_hwnd, WM_CUSTOM_UPDATE_RUNSOURCE, 0, 0);
                    break;
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM.WM_CREATE:
                    _hwnd = m.HWnd;
                    break;

                case WM_CUSTOM_NEW:
                    //Exe(new fExe(New));
                    Try(New);
                    break;
                case WM_CUSTOM_OPEN_FILE:
                    //Exe(new fExe(Open));
                    Try(Open);
                    break;
                case WM_CUSTOM_SAVE:
                    //Exe(new fExe(Save));
                    Try(Save);
                    break;
                case WM_CUSTOM_SAVE_AS:
                    //Exe(new fExe(SaveAs));
                    Try(SaveAs);
                    break;
                case WM_CUSTOM_RUN_CODE:
                    //Exe(new fExe(RunCode));
                    Try(() => _RunCode());
                    break;
                case WM_CUSTOM_RUN_CODE_ON_MAIN_THREAD:
                    //Exe(new fExe(RunCodeOnMainThread));
                    Try(() => _RunCode(useNewThread: false));
                    break;
                case WM_CUSTOM_RUN_CODE_WITHOUT_PROJECT:
                    //Exe(new fExe(RunCodeWithoutProject));
                    Try(() => _RunCode(compileWithoutProject: true));
                    break;
                case WM_CUSTOM_COMPILE_CODE:
                    //Exe(new fExe(CompileCode));
                    Try(CompileCode);
                    break;
                case WM_CUSTOM_COMPILE_RUNSOURCE:
                    //Exe(new fExe(_CompileRunSource));
                    Try(_CompileRunSource);
                    break;
                case WM_CUSTOM_RESTART_RUNSOURCE:
                    //Exe(new fExe(_RestartRunSource));
                    Try(_RestartRunSource);
                    break;
                case WM_CUSTOM_UPDATE_RUNSOURCE:
                    //Exe(new fExe(_UpdateRunSource));
                    Try(_UpdateRunSource);
                    break;
                case WM_CUSTOM_ABORT_EXECUTION:
                    AbortExecution();
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

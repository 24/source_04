using pb.Windows.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HWND = System.IntPtr;

namespace runsourced
{
    public partial class RunSourceForm
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

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM.WM_CREATE:
                    _hwnd = m.HWnd;
                    break;

                case WM_CUSTOM_NEW:
                    Exe(new fExe(New));
                    break;
                case WM_CUSTOM_OPEN_FILE:
                    Exe(new fExe(Open));
                    break;
                case WM_CUSTOM_SAVE:
                    Exe(new fExe(Save));
                    break;
                case WM_CUSTOM_SAVE_AS:
                    Exe(new fExe(SaveAs));
                    break;
                case WM_CUSTOM_RUN_CODE:
                    Exe(new fExe(RunCode));
                    break;
                case WM_CUSTOM_RUN_CODE_ON_MAIN_THREAD:
                    Exe(new fExe(RunCodeOnMainThread));
                    break;
                case WM_CUSTOM_RUN_CODE_WITHOUT_PROJECT:
                    Exe(new fExe(RunCodeWithoutProject));
                    break;
                case WM_CUSTOM_COMPILE_CODE:
                    Exe(new fExe(CompileCode));
                    break;
                case WM_CUSTOM_COMPILE_RUNSOURCE:
                    Exe(new fExe(_CompileRunSource));
                    break;
                case WM_CUSTOM_RESTART_RUNSOURCE:
                    Exe(new fExe(_RestartRunSource));
                    break;
                case WM_CUSTOM_UPDATE_RUNSOURCE:
                    Exe(new fExe(_UpdateRunSource));
                    break;
                case WM_CUSTOM_ABORT_EXECUTION:
                    AbortExecution();
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

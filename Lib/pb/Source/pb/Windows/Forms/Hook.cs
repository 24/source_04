using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using pb.Windows.Win32;

namespace pb.Windows.Forms
{
    #region class HookException
    public class HookException : Exception
    {
        public HookException(string sMessage) : base(sMessage) { }
        public HookException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public HookException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public HookException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region hook doc
    // hook example in                             http://support.microsoft.com/kb/318804/fr
    // doc msdn SetWindowsHookEx()                 http://msdn.microsoft.com/en-us/library/ms644990.aspx
    // doc msdn KeyboardProc                       http://msdn.microsoft.com/en-us/library/ms644984(VS.85).aspx
    // doc msdn Using Hooks                        http://msdn.microsoft.com/en-us/library/ms644960(VS.85).aspx
    // doc Le hook du clavier et de la souris      http://humann.developpez.com/hook/

    // keep the HookProc delegate alive manually
    // call CallNextHookEx in HookProc callback delegate
    // call UnhookWindowsHookEx using the handle returned by SetWindowsHookEx
    // AppDomain.GetCurrentThreadId() is deprecated use Kernel.GetCurrentThreadId() but dont use Thread.CurrentThread.ManagedThreadId
    #endregion

    #region class KeyboardHookMessage
    public class KeyboardHookMessage
    {
        public int nCode;
        public IntPtr wParam;
        public IntPtr lParam;
        public bool RemoveMessage = false;
        public User.VK vk;
        public KeyboardState KeyState;
        public KeystrokeMessageFlags KeyFlags;
        public User.Struct.GUITHREADINFO GUIThreadInfo;
    }
    #endregion

    #region class KeyboardHook
    /// <summary>Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure (WH_KEYBOARD).</summary>
    public class KeyboardHook : IDisposable
    {
        #region doc
        /*************************************************************************************************************************************************************************************************
            WH_KEYBOARD : Installs a hook procedure that monitors keystroke messages.

            KeyboardProc : int _HookProc(int nCode, IntPtr wParam, IntPtr lParam) (http://msdn.microsoft.com/en-us/library/ms644984(VS.85).aspx)
            nCode :  Specifies a code the hook procedure uses to determine how to process the message.
                     If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.
                     This parameter can be one of the following values.
                     HC_ACTION   : The wParam and lParam parameters contain information about a keystroke message.
                     HC_NOREMOVE : The wParam and lParam parameters contain information about a keystroke message, and the keystroke message has not been removed from the message queue.
                                  (An application called the PeekMessage function, specifying the PM_NOREMOVE flag.)
            wParam : Specifies the virtual-key code of the key that generated the keystroke message.
            lParam : Specifies the repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
                     For more information about the lParam parameter, see Keystroke Message Flags. This parameter can be one or more of the following values.
                     0-15  : Specifies the repeat count. The value is the number of times the keystroke is repeated as a result of the user's holding down the key.
                     16-23 : Specifies the scan code. The value depends on the OEM.
                     24    : Specifies whether the key is an extended key, such as a function key or a key on the numeric keypad. The value is 1 if the key is an extended key; otherwise, it is 0.
                     25-28 : Reserved.
                     29    : Specifies the context code. The value is 1 if the ALT key is down; otherwise, it is 0.
                     30    : Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
                     31    : Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.
           return  : If code is less than zero, the hook procedure must return the value returned by CallNextHookEx.
                     If code is greater than or equal to zero, and the hook procedure did not process the message,
                     it is highly recommended that you call CallNextHookEx and return the value it returns;
                     otherwise, other applications that have installed WH_KEYBOARD hooks will not receive hook notifications and may behave incorrectly as a result.
                     If the hook procedure processed the message, it may return a nonzero value to prevent the system from passing the message to the rest of the hook chain
                     or the target window procedure.
        *************************************************************************************************************************************************************************************************/
        #endregion

        #region variable
        private IntPtr gHookHandle = IntPtr.Zero;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private User.HookProc gHookProc = null;
        private bool gbTrace = false;

        public delegate void HookProc(KeyboardHookMessage msg);
        public HookProc HookEvent = null;
        #endregion

        #region constructor
        public KeyboardHook()
        {
            InitHook();
        }
        #endregion

        #region InitHook
        private void InitHook()
        {
            gHookProc = new User.HookProc(_HookProc);
            gHookHandle = User.SetWindowsHookEx(User.HookType.WH_KEYBOARD, gHookProc, IntPtr.Zero, Kernel.GetCurrentThreadId());

            if (gHookHandle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new HookException("error {0} calling SetWindowsHookEx for hook type WH_KEYBOARD : {1}", err, new System.ComponentModel.Win32Exception(err).Message);
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            gHookProc = null;
            if (gHookHandle == IntPtr.Zero) return;
            User.UnhookWindowsHookEx(gHookHandle);
            gHookHandle = IntPtr.Zero;
        }
        #endregion

        #region Trace
        public bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region _HookProc
        private int _HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (HookEvent != null || gbTrace))
            {
                User.VK vk = Messages.GetVirtualKey((int)wParam);
                KeyboardState ks = Messages.GetKeyboardState();
                // attention (uint)lParam peut générer une exception si la valeur dépasse la capacité d'un int, utiliser (uint)p.ToInt64()
                //KeystrokeMessageFlags kf = Messages.GetKeystrokeMessageFlags((uint)lParam);
                KeystrokeMessageFlags kf = Messages.GetKeystrokeMessageFlags((uint)lParam.ToInt64());
                User.Struct.GUITHREADINFO GUIThreadInfo = Win32.Windows.GetGUIThreadInfo();

                if (gbTrace)
                {
                    string sCode;
                    if (nCode == User.Const.HC_ACTION)
                        sCode = "HC_ACTION";
                    else if (nCode == User.Const.HC_NOREMOVE)
                        sCode = "HC_NOREMOVE";
                    else
                        sCode = nCode.ToString();
                    string sKey = vk.ToString();
                    if (ks.Alt) sKey = "ALT-" + sKey;
                    if (ks.Ctrl) sKey = "CTRL-" + sKey;
                    if (ks.Shift) sKey = "SHIFT-" + sKey;
                    string sTransition;
                    if (kf.TransitionState == 0) sTransition = "WM_KEYDOWN"; else sTransition = "WM_KEYUP";
                    string sPreviousKeyState;
                    if (kf.PreviousKeyState == 0) sPreviousKeyState = "KEYUP"; else sPreviousKeyState = "KEYDOWN";
                    pb.Trace.WriteLine("KeyboardHook                                   : {0,-20} {1,-20} {2:x,4} {3,-11} prev key state {4,-7} lParam {5:x}", sTransition, sKey, wParam, sCode, sPreviousKeyState, lParam);
                }

                if (HookEvent != null)
                {
                    KeyboardHookMessage msg = new KeyboardHookMessage() { nCode = nCode, wParam = wParam, lParam = lParam, vk = vk, KeyState = ks, KeyFlags = kf, GUIThreadInfo = GUIThreadInfo };
                    HookEvent(msg);
                    if (msg.RemoveMessage) return 1;
                }
            }
            return User.CallNextHookEx(gHookHandle, nCode, wParam, lParam);
        }
        #endregion
    }
    #endregion
        
    #region class KeyboardLowLevelHookMessage
    public class KeyboardLowLevelHookMessage
    {
        public int nCode;
        public User.Struct.KBDLLHOOKSTRUCT kbParam;
        public bool RemoveMessage = false;
        public User.WM wm;
        public User.VK vk;
        //public KeyboardState KeyState;
        public KeystrokeMessageFlags KeyFlags;
        public User.Struct.GUITHREADINFO GUIThreadInfo;
        public uint ProcessId;
        public uint ThreadId;
    }
    #endregion

    #region class KeyboardLowLevelHook
    // pb GetKeyboardState() ne renseigne que pour le thread courant
    // il faudrait gérer le GetKeyboardState() manuellement
    /// <summary>Install a hook procedure that monitors low-level keyboard input events. (WH_KEYBOARD_LL) (Windows NT/2000/XP)</summary>
    public class KeyboardLowLevelHook : IDisposable
    {

        #region doc
        /*************************************************************************************************************************************************************************************************
            WH_KEYBOARD_LL : Install a hook procedure that monitors low-level keyboard input events. (Windows NT/2000/XP)

            LowLevelKeyboardProc : int _HookProc(int nCode, IntPtr wParam, IntPtr lParam) (http://msdn.microsoft.com/en-us/library/ms644985(VS.85).aspx)

            nCode  : Specifies a code the hook procedure uses to determine how to process the message.
                     If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.
                     This parameter can be HC_ACTION
            wParam : Specifies the identifier of the keyboard message. This parameter can be one of the following messages: WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, or WM_SYSKEYUP.
            lParam : Pointer to a KBDLLHOOKSTRUCT structure.
            return : If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
                     If nCode is greater than or equal to zero, and the hook procedure did not process the message,
                     it is highly recommended that you call CallNextHookEx and return the value it returns;
                     otherwise, other applications that have installed WH_KEYBOARD_LL hooks will not receive hook notifications and may behave incorrectly as a result.
                     If the hook procedure processed the message, it may return a nonzero value to prevent the system from passing the message
                     to the rest of the hook chain or the target window procedure.
        *************************************************************************************************************************************************************************************************/
        #endregion

        #region variable
        private IntPtr gHookHandle = IntPtr.Zero;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private User.LowLevelKeyboardHookProc gHookProc = null;
        private bool gbTrace = false;

        public delegate void HookProc(KeyboardLowLevelHookMessage msg);
        public HookProc HookEvent = null;
        #endregion

        #region constructor
        public KeyboardLowLevelHook()
        {
            InitHook();
        }
        #endregion

        #region InitHook
        private void InitHook()
        {
            gHookProc = new User.LowLevelKeyboardHookProc(_HookProc);

            //Kernel.GetModuleHandle(this.GetType().Module.Name) idem Marshal.GetHINSTANCE(this.GetType().Module)
            gHookHandle = User.SetWindowsHookEx(User.HookType.WH_KEYBOARD_LL, gHookProc, Marshal.GetHINSTANCE(this.GetType().Module), 0);

            if (gHookHandle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new HookException("error {0} calling SetWindowsHookEx for hook type WH_KEYBOARD_LL : {1}", err, new System.ComponentModel.Win32Exception(err).Message);
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            gHookProc = null;
            if (gHookHandle == IntPtr.Zero) return;
            User.UnhookWindowsHookEx(gHookHandle);
            gHookHandle = IntPtr.Zero;
        }
        #endregion

        #region Trace
        public bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region _HookProc
        // [In] Indique que les données doivent être marshalées de l'appelant vers l'appelé, mais pas à nouveau vers l'appelant. InAttribute class dans System.Runtime.InteropServices (http://msdn.microsoft.com/fr-fr/library/system.runtime.interopservices.inattribute.aspx)
        private int _HookProc(int nCode, User.WM wm, [In] User.Struct.KBDLLHOOKSTRUCT kbParam)
        {
            if (nCode >= 0 && (HookEvent != null || gbTrace))
            {
                User.VK vk = Messages.GetVirtualKey((int)kbParam.vkCode);
                KeystrokeMessageFlags kf = Messages.GetKeystrokeMessageFlags(kbParam.Flags);
                User.Struct.GUITHREADINFO GUIThreadInfo = Win32.Windows.GetGUIThreadInfo();
                uint ProcessId;
                uint ThreadId = User.GetWindowThreadProcessId(GUIThreadInfo.hwndFocus, out ProcessId);

                if (gbTrace)
                {
                    string sCode;
                    if (nCode == User.Const.HC_ACTION)
                        sCode = "HC_ACTION";
                    else
                        sCode = nCode.ToString();
                    string sKey = vk.ToString();
                    pb.Trace.WriteLine("KeyboardLowLevelHook                           : {0,-20} {1,-20} {2:x,4} {3,-11} process {4:x} thread {5:x}", wm, sKey, kbParam.vkCode, sCode, ProcessId, ThreadId);
                }

                if (HookEvent != null)
                {
                    KeyboardLowLevelHookMessage msg = new KeyboardLowLevelHookMessage()
                        { nCode = nCode, kbParam = kbParam, wm = wm, vk = vk, KeyFlags = kf, GUIThreadInfo = GUIThreadInfo, ProcessId = ProcessId, ThreadId = ThreadId };
                    HookEvent(msg);
                    if (msg.RemoveMessage) return 1;
                }
            }
            return User.CallNextHookEx(gHookHandle, nCode, wm, kbParam);
        }
        #endregion
    }
    #endregion

    #region class PreWindowHookMessage
    public class PreWindowHookMessage
    {
        public int nCode;
        public IntPtr wParam;
        public IntPtr lParam;
        //public bool RemoveMessage = false;
        public User.Struct.CWPSTRUCT msg;
        public User.WM wm;
    }
    #endregion

    #region class PreWindowHook
    /// <summary>Install a hook procedure that monitors messages before the system sends them to the destination window procedure. (WH_CALLWNDPROC).</summary>
    public class PreWindowHook : IDisposable
    {
        #region doc
        /*************************************************************************************************************************************************************************************************
            Il n'est pas possible de bloquer le message (supprimer le message)

            WH_CALLWNDPROC : Install a hook procedure that monitors messages before the system sends them to the destination window procedure.

            CallWndProc : int _HookProc(int nCode, IntPtr wParam, IntPtr lParam) (http://msdn.microsoft.com/en-us/library/ms644975(VS.85).aspx)
            nCode  : Specifies whether the hook procedure must process the message. If nCode is HC_ACTION, the hook procedure must process the message.
                     If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing
                     and must return the value returned by CallNextHookEx.
            wParam : Specifies whether the message was sent by the current thread. If the message was sent by the current thread, it is nonzero; otherwise, it is zero.
            lParam : Pointer to a CWPSTRUCT structure that contains details about the message.
            return : If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
                     If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns;
                     otherwise, other applications that have installed WH_CALLWNDPROC hooks will not receive hook notifications and may behave incorrectly as a result.
                     If the hook procedure does not call CallNextHookEx, the return value should be zero.
        *************************************************************************************************************************************************************************************************/
        #endregion

        #region variable
        private IntPtr gHookHandle = IntPtr.Zero;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private User.HookProc gHookProc = null;
        private bool gbTrace = false;

        public delegate void HookProc(PreWindowHookMessage msg);
        public HookProc HookEvent = null;
        #endregion

        #region constructor
        public PreWindowHook()
        {
            InitHook();
        }
        #endregion

        #region InitHook
        private void InitHook()
        {
            gHookProc = new User.HookProc(_HookProc);
            gHookHandle = User.SetWindowsHookEx(User.HookType.WH_CALLWNDPROC, gHookProc, IntPtr.Zero, Kernel.GetCurrentThreadId());

            if (gHookHandle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new HookException("error {0} calling SetWindowsHookEx for hook type WH_CALLWNDPROC : {1}", err, new System.ComponentModel.Win32Exception(err).Message);
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            gHookProc = null;
            if (gHookHandle == IntPtr.Zero) return;
            User.UnhookWindowsHookEx(gHookHandle);
            gHookHandle = IntPtr.Zero;
        }
        #endregion

        #region Trace
        public bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region _HookProc
        private int _HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (HookEvent != null || gbTrace))
            {
                User.Struct.CWPSTRUCT cwp = (User.Struct.CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(User.Struct.CWPSTRUCT));
                User.WM message = Messages.GetWindowMessage(cwp.message);

                if (gbTrace)
                {
                    string sMessage = message.ToString();
                    if (message == User.WM.WM_UNKNOW) sMessage += " (" + cwp.message.ToString("x") + ")";
                    string sCode; if (nCode == User.Const.HC_ACTION) sCode = "HC_ACTION"; else sCode = nCode.ToString();
                    pb.Trace.WriteLine("PreWindowHook                                  : {0,-20} {1,-11} wParam {2,-8:x} wParam {3,-8:x} lParam {4,-8:x} hwnd {5:x}", sMessage, sCode, (int)wParam, (int)cwp.wParam, (int)cwp.lParam, (int)cwp.hwnd);
                }

                if (HookEvent != null)
                {
                    PreWindowHookMessage msg = new PreWindowHookMessage() { nCode = nCode, wParam = wParam, lParam = lParam, msg = cwp, wm = message };
                    HookEvent(msg);
                    //if (msg.RemoveMessage) return 0;
                }
            }
            return User.CallNextHookEx(gHookHandle, nCode, wParam, lParam);
        }
        #endregion
    }
    #endregion

    #region class PostWindowHookMessage
    public class PostWindowHookMessage
    {
        public int nCode;
        public IntPtr wParam;
        public IntPtr lParam;
        //public bool RemoveMessage = false;
        public User.Struct.CWPRETSTRUCT msg;
        public User.WM wm;
    }
    #endregion

    #region class PostWindowHook
    /// <summary>Install a hook procedure that monitors messages after they have been processed by the destination window procedure. (WH_CALLWNDPROCRET).</summary>
    public class PostWindowHook : IDisposable
    {
        #region doc
        /*************************************************************************************************************************************************************************************************
            WH_CALLWNDPROCRET : Install a hook procedure that monitors messages after they have been processed by the destination window procedure.

            CallWndRetProc : int _HookProc(int nCode, IntPtr wParam, IntPtr lParam) (http://msdn.microsoft.com/en-us/library/ms644976(VS.85).aspx)
                             The CallWndRetProc hook procedure is an application-defined or library-defined callback function used with the SetWindowsHookEx function.
                             The system calls this function after the SendMessage function is called. The hook procedure can examine the message; it cannot modify it.
            nCode  : Specifies whether the hook procedure must process the message. If nCode is HC_ACTION, the hook procedure must process the message.
                     If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.
            wParam : Specifies whether the message is sent by the current process. If the message is sent by the current process, it is nonzero; otherwise, it is NULL.
            lParam : Pointer to a CWPRETSTRUCT structure that contains details about the message.
            return : If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
                     If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise,
                     other applications that have installed WH_CALLWNDPROCRET hooks will not receive hook notifications and may behave incorrectly as a result.
                     If the hook procedure does not call CallNextHookEx, the return value should be zero.
        *************************************************************************************************************************************************************************************************/
        #endregion

        #region variable
        private IntPtr gHookHandle = IntPtr.Zero;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private User.HookProc gHookProc = null;
        private bool gbTrace = false;

        public delegate void HookProc(PostWindowHookMessage msg);
        public HookProc HookEvent = null;
        #endregion

        #region constructor
        public PostWindowHook()
        {
            InitHook();
        }
        #endregion

        #region InitHook
        private void InitHook()
        {
            gHookProc = new User.HookProc(_HookProc);
            gHookHandle = User.SetWindowsHookEx(User.HookType.WH_CALLWNDPROCRET, gHookProc, IntPtr.Zero, Kernel.GetCurrentThreadId());

            if (gHookHandle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new HookException("error {0} calling SetWindowsHookEx for hook type WH_CALLWNDPROCRET : {1}", err, new System.ComponentModel.Win32Exception(err).Message);
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            gHookProc = null;
            if (gHookHandle == IntPtr.Zero) return;
            User.UnhookWindowsHookEx(gHookHandle);
            gHookHandle = IntPtr.Zero;
        }
        #endregion

        #region Trace
        public bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region _HookProc
        private int _HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (HookEvent != null || gbTrace))
            {
                User.Struct.CWPRETSTRUCT cwp = (User.Struct.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(User.Struct.CWPRETSTRUCT));
                User.WM message = Messages.GetWindowMessage(cwp.message);

                if (gbTrace)
                {
                    string sMessage = message.ToString();
                    if (message == User.WM.WM_UNKNOW) sMessage += " (" + cwp.message.ToString("x") + ")";
                    string sCode; if (nCode == User.Const.HC_ACTION) sCode = "HC_ACTION"; else sCode = nCode.ToString();
                    pb.Trace.WriteLine("PostWindowHook                                 : {0,-20} {1,-11} wParam {2,-8:x} wParam {3,-8:x} lParam {4,-8:x} lResult {5,-8:x} hwnd {6:x}", sMessage, sCode, (int)wParam, (int)cwp.wParam, (int)cwp.lParam, (int)cwp.lResult, (int)cwp.hwnd);
                }

                if (HookEvent != null)
                {
                    PostWindowHookMessage msg = new PostWindowHookMessage() { nCode = nCode, wParam = wParam, lParam = lParam, msg = cwp, wm = message };
                    HookEvent(msg);
                    //if (msg.RemoveMessage) return 1;
                }
            }
            return User.CallNextHookEx(gHookHandle, nCode, wParam, lParam);
        }
        #endregion
    }
    #endregion

    #region class GetMessageWindowHookMessage
    public class GetMessageWindowHookMessage
    {
        public int nCode;
        public IntPtr wParam;
        public IntPtr lParam;
        public bool RemoveMessage = false;
        public User.Struct.MSG msg;
        public User.WM wm;
    }
    #endregion

    #region class GetMessageWindowHook
    /// <summary>Installs a hook procedure that monitors messages posted to a message queue. (WH_GETMESSAGE).</summary>
    public class GetMessageWindowHook : IDisposable
    {
        #region doc
        /*************************************************************************************************************************************************************************************************
            ????? Il n'est pas possible de bloquer le message (supprimer le message)

            WH_GETMESSAGE : Installs a hook procedure that monitors messages posted to a message queue.

            GetMsgProc : int _HookProc(int nCode, IntPtr wParam, IntPtr lParam) (http://msdn.microsoft.com/en-us/library/ms644981(VS.85).aspx)
            nCode  : Specifies whether the hook procedure must process the message.
                     If code is HC_ACTION, the hook procedure must process the message.
                     If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing
                     and should return the value returned by CallNextHookEx.
            wParam : Specifies whether the message has been removed from the queue. This parameter can be one of the following values.
                     PM_NOREMOVE : Specifies that the message has not been removed from the queue. (An application called the PeekMessage function, specifying the PM_NOREMOVE flag.)
                     PM_REMOVE   : Specifies that the message has been removed from the queue. (An application called GetMessage, or it called the PeekMessage function, specifying the PM_REMOVE flag.)
            lParam : Pointer to an MSG structure that contains details about the message.
            return : If code is less than zero, the hook procedure must return the value returned by CallNextHookEx.
                     If code is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns;
                     otherwise, other applications that have installed WH_GETMESSAGE hooks will not receive hook notifications and may behave incorrectly as a result.
                     If the hook procedure does not call CallNextHookEx, the return value should be zero.
        *************************************************************************************************************************************************************************************************/
        #endregion

        #region variable
        private IntPtr gHookHandle = IntPtr.Zero;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        private User.HookProc gHookProc = null;
        private bool gbTrace = false;

        public delegate void HookProc(GetMessageWindowHookMessage msg);
        public HookProc HookEvent = null;
        #endregion

        #region constructor
        public GetMessageWindowHook()
        {
            InitHook();
        }
        #endregion

        #region InitHook
        private void InitHook()
        {
            gHookProc = new User.HookProc(_HookProc);
            gHookHandle = User.SetWindowsHookEx(User.HookType.WH_GETMESSAGE, gHookProc, IntPtr.Zero, Kernel.GetCurrentThreadId());

            if (gHookHandle == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                throw new HookException("error {0} calling SetWindowsHookEx for hook type WH_GETMESSAGE : {1}", err, new System.ComponentModel.Win32Exception(err).Message);
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            gHookProc = null;
            if (gHookHandle == IntPtr.Zero) return;
            User.UnhookWindowsHookEx(gHookHandle);
            gHookHandle = IntPtr.Zero;
        }
        #endregion

        #region Trace
        public bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region _HookProc
        private int _HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (HookEvent != null || gbTrace))
            {
                User.Struct.MSG msg = (User.Struct.MSG)Marshal.PtrToStructure(lParam, typeof(User.Struct.MSG));
                User.WM wm = Messages.GetWindowMessage((int)msg.message);

                if (gbTrace)
                {
                    string sMessage = wm.ToString();
                    if (wm == User.WM.WM_UNKNOW) sMessage += " (" + msg.message.ToString("x") + ")";
                    string sCode; if (nCode == User.Const.HC_ACTION) sCode = "HC_ACTION"; else sCode = nCode.ToString();
                    pb.Trace.WriteLine("GetMessageWindowHook                           : {0,-20} {1,-11} wParam {2,-8:x} wParam {3,-8:x} lParam {4,-8:x} hwnd {5:x}", sMessage, sCode, (int)wParam, (int)msg.wParam, (int)msg.lParam, (int)msg.hwnd);
                }

                if (HookEvent != null)
                {
                    GetMessageWindowHookMessage msg2 = new GetMessageWindowHookMessage() { nCode = nCode, wParam = wParam, lParam = lParam, msg = msg, wm = wm };
                    HookEvent(msg2);
                    if (msg2.RemoveMessage) return 0;
                }
            }
            return User.CallNextHookEx(gHookHandle, nCode, wParam, lParam);
        }
        #endregion
    }
    #endregion
}

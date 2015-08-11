using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

//namespace Win32
namespace pb.Windows.Win32
{
    #region class Win32Exception
    public class Win32Exception : Exception
    {
        public Win32Exception(string sMessage) : base(sMessage) { }
        public Win32Exception(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public Win32Exception(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public Win32Exception(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region class Windows
    public class Windows
    {
        #region GetWindowsHandle
        public static int GetWindowsHandle(string sMainClass, string sMainWindow, string sClass)
        {
            int hWndMain = Win32.User.FindWindow(sMainClass, sMainWindow);
            if (hWndMain == 0) return 0;
            return Win32.User.FindWindowEx(new IntPtr(hWndMain), new IntPtr(0), sClass, "");
        }
        #endregion

        #region SetForegroundWindow(string sMainClass, string sMainWindow, string sClass)
        public static bool SetForegroundWindow(string sMainClass, string sMainWindow, string sClass)
        {
            int hWnd = GetWindowsHandle(sMainClass, sMainWindow, sClass);
            if (hWnd == 0) return false;
            Win32.User.SetForegroundWindow(new IntPtr(hWnd));
            return true;
        }
        #endregion

        #region SetForegroundWindow(int hWnd)
        public static void SetForegroundWindow(int hWnd)
        {
            Win32.User.SetForegroundWindow(new IntPtr(hWnd));
        }
        #endregion

        #region GetWindowsBitmap(IntPtr hWnd)
        public static Bitmap GetWindowsBitmap(IntPtr hWnd)
        {
            User.Struct.RECT rect = new User.Struct.RECT();
            User.GetClientRect(hWnd, ref rect);
            return GetWindowsBitmap(hWnd, rect.Left, rect.Top, rect.Right - rect.Left + 1, rect.Bottom - rect.Top + 1);
        }
        #endregion

        #region GetWindowsBitmap(IntPtr hWnd)
        public static Bitmap GetWindowsBitmap(IntPtr hWnd, int x, int y, int width, int height)
        {
            //Win32_struct.RECT rect = new Win32_struct.RECT();
            //Win32.User.GetClientRect(hWnd, ref rect);
            //int x1 = rect.Left;
            //int y1 = rect.Top;
            //int x2 = rect.Right;
            //int y2 = rect.Bottom;
            int x2 = x + width - 1;
            int y2 = y + height - 1;
            GetScreenCoordinates(hWnd, ref x, ref y);
            GetScreenCoordinates(hWnd, ref x2, ref y2);
            Rectangle rect2 = new Rectangle(x, y, x2 - x + 1, y2 - y + 1);
            Bitmap bmp = new Bitmap(rect2.Width, rect2.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect2.Left, rect2.Top, 0, 0, rect2.Size, CopyPixelOperation.SourceCopy);
            g.Dispose();
            return bmp;
        }
        #endregion

        #region GetScreenCoordinates
        public static void GetScreenCoordinates(IntPtr HwndPtr, ref int x, ref int y)
        {
            User.Struct.POINT point = new User.Struct.POINT();
            point.x = x;
            point.y = y;
            Win32.User.ClientToScreen(HwndPtr, ref point);
            x = point.x;
            y = point.y;
        }
        #endregion

        #region GetAbsoluteCoordinates
        private static void GetAbsoluteCoordinates(ref int x, ref int y)
        {
            Size screenSize = SystemInformation.PrimaryMonitorSize;
            x = x * 65535 / screenSize.Width;
            y = y * 65535 / screenSize.Height;
        }
        #endregion

        #region SendInput(User_struct.INPUT input)
        public static void SendInput(User.Struct.INPUT input)
        {
            User.Struct.INPUT[] inputs = new User.Struct.INPUT[1];
            inputs[0] = input;
            User.SendInput(1, inputs, Marshal.SizeOf(input));
        }
        #endregion

        #region SendInput(User_struct.INPUT[] inputs)
        public static void SendInput(User.Struct.INPUT[] inputs)
        {
            User.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
        }
        #endregion

        #region MouseEvent
        public static void MouseEvent(int x, int y, params uint[] mouseEvents)
        {
            GetAbsoluteCoordinates(ref x, ref y);

            User.Struct.INPUT input = new User.Struct.INPUT(User.Const.INPUT_MOUSE);
            //User_struct.INPUT[] input = new User_struct.INPUT[1];
            //input[0] = new User_struct.INPUT();
            input.type = User.Const.INPUT_MOUSE;
            input.mi.dx = x;
            input.mi.dy = y;
            input.mi.mouseData = 0;
            input.mi.time = 0;
            input.mi.dwExtraInfo = User.GetMessageExtraInfo();

            foreach (uint mouseEvent in mouseEvents)
            {
                input.mi.dwFlags = mouseEvent;
                //User.SendInput(1, ref input, Marshal.SizeOf(input));
                //User.SendInput(1, input, Marshal.SizeOf(input));
                SendInput(input);
            }
        }
        #endregion

        #region MouseMove
        public static void MouseMove(int x, int y)
        {
            MouseEvent(x, y, Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_MOVE);
        }
        #endregion

        #region MouseRightClick
        public static void MouseRightClick(int x, int y)
        {
            MouseEvent(x, y,
                Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_MOVE,
                Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_RIGHTDOWN,
                Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_RIGHTUP);
        }
        #endregion

        #region MouseLeftClick
        public static void MouseLeftClick(int x, int y)
        {
            MouseEvent(x, y,
                Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_MOVE,
                Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_LEFTDOWN,
                Win32.User.Const.MOUSEEVENTF_ABSOLUTE | Win32.User.Const.MOUSEEVENTF_LEFTUP);
        }
        #endregion

        #region enum KB
        [Flags]
        public enum KB : uint
        {
            KB_SHIFT   = 0x01,
            KB_CONTROL = 0x02,
            KB_ALT     = 0x04,
            KB_WIN     = 0x08
        }
        #endregion

        #region KeyboardEvent(User.VK vk)
        public static void KeyboardEvent(User.VK vk)
        {
            KeyboardEvent(vk, 0);
        }
        #endregion

        #region KeyboardEvent(User.VK vk, KB kb)
        public static void KeyboardEvent(User.VK vk, KB kb)
        {
            User.Struct.INPUT input = new User.Struct.INPUT(User.Const.INPUT_KEYBOARD);
            input.ki.dwExtraInfo = User.GetMessageExtraInfo();

            KeyDown(kb);

            // Key down the actual key-code
            input.ki.wVk = (ushort)vk;
            SendInput(input);

            // Key up the actual key-code
            input.ki.dwFlags = User.Const.KEYEVENTF_KEYUP;
            SendInput(input);

            KeyUp(kb);
        }
        #endregion

        #region KeyboardEvent(char c)
        public static void KeyboardEvent(char c)
        {
            KeyboardEvent(c, 0);
        }
        #endregion

        #region KeyboardEvent(char c, KB kb)
        public static void KeyboardEvent(char c, KB kb)
        {
            User.Struct.INPUT input = new User.Struct.INPUT(User.Const.INPUT_KEYBOARD);
            input.ki.dwExtraInfo = User.GetMessageExtraInfo();

            KeyDown(kb);

            input.ki.wVk = 0;
            input.ki.wScan = c;
            input.ki.dwFlags = User.Const.KEYEVENTF_UNICODE;
            SendInput(input);

            input.ki.dwFlags = User.Const.KEYEVENTF_UNICODE | User.Const.KEYEVENTF_KEYUP;
            SendInput(input);

            KeyUp(kb);
        }
        #endregion

        #region KeyboardEvent(string s)
        public static void KeyboardEvent(string s)
        {
            KeyboardEvent(s, 0);
        }
        #endregion

        #region KeyboardEvent(string s, KB kb)
        public static void KeyboardEvent(string s, KB kb)
        {
            User.Struct.INPUT input = new User.Struct.INPUT(User.Const.INPUT_KEYBOARD);
            input.ki.dwExtraInfo = User.GetMessageExtraInfo();

            KeyDown(kb);

            foreach (char c in s)
            {
                input.ki.wVk = 0;
                input.ki.wScan = c;
                input.ki.dwFlags = User.Const.KEYEVENTF_UNICODE;
                SendInput(input);

                input.ki.dwFlags = User.Const.KEYEVENTF_UNICODE | User.Const.KEYEVENTF_KEYUP;
                SendInput(input);
            }

            KeyUp(kb);
        }
        #endregion

        #region KeyDown
        public static void KeyDown(KB kb)
        {
            User.Struct.INPUT input = new User.Struct.INPUT(User.Const.INPUT_KEYBOARD);
            input.ki.dwExtraInfo = User.GetMessageExtraInfo();

            // Key down shift, ctrl, and/or alt
            if ((kb & KB.KB_SHIFT) == KB.KB_SHIFT)
            {
                input.ki.wVk = (ushort)User.VK.VK_SHIFT;
                SendInput(input);
            }
            if ((kb & KB.KB_CONTROL) == KB.KB_CONTROL)
            {
                input.ki.wVk = (ushort)User.VK.VK_CONTROL;
                SendInput(input);
            }
            if ((kb & KB.KB_ALT) == KB.KB_ALT)
            {
                input.ki.wVk = (ushort)User.VK.VK_MENU;
                SendInput(input);
            }
            if ((kb & KB.KB_WIN) == KB.KB_WIN)
            {
                input.ki.wVk = (ushort)User.VK.VK_LWIN;
                SendInput(input);
            }
        }
        #endregion

        #region KeyUp
        public static void KeyUp(KB kb)
        {
            User.Struct.INPUT input = new User.Struct.INPUT(User.Const.INPUT_KEYBOARD);
            input.ki.dwExtraInfo = User.GetMessageExtraInfo();

            // Key down shift, ctrl, and/or alt
            input.ki.dwFlags = User.Const.KEYEVENTF_KEYUP;

            if ((kb & KB.KB_SHIFT) == KB.KB_SHIFT)
            {
                input.ki.wVk = (ushort)User.VK.VK_SHIFT;
                SendInput(input);
            }
            if ((kb & KB.KB_CONTROL) == KB.KB_CONTROL)
            {
                input.ki.wVk = (ushort)User.VK.VK_CONTROL;
                SendInput(input);
            }
            if ((kb & KB.KB_ALT) == KB.KB_ALT)
            {
                input.ki.wVk = (ushort)User.VK.VK_MENU;
                SendInput(input);
            }
            if ((kb & KB.KB_WIN) == KB.KB_WIN)
            {
                input.ki.wVk = (ushort)User.VK.VK_LWIN;
                SendInput(input);
            }
        }
        #endregion

        #region GetWindowsList
        public static IntPtr[] GetWindowsList()
        {
            EnumWindows enumWindows = new EnumWindows();
            return enumWindows.GetWindowsList().ToArray();
        }
        #endregion

        public static WindowInfo[] GetWindowsInfoList()
        {
            EnumWindows enumWindows = new EnumWindows();
            return (from hWnd in enumWindows.GetWindowsList() select GetWindowInfo(hWnd)).ToArray();
        }

        public static WindowInfo[] GetWindowsInfoList(string title, string className)
        {
            EnumWindows enumWindows = new EnumWindows();
            enumWindows.TitleFilter = title;
            enumWindows.ClassNameFilter = className;
            return (from hWnd in enumWindows.GetWindowsList() select GetWindowInfo(hWnd)).ToArray();
        }

        public static WindowInfo[] GetWindowsInfoListByClassName(string className)
        {
            EnumWindows enumWindows = new EnumWindows();
            enumWindows.ClassNameFilter = className;
            //return (from hWnd in enumWindows.GetWindowsList() where GetWindowClassName(hWnd) == className select GetWindowInfo(hWnd)).ToArray();
            return (from hWnd in enumWindows.GetWindowsList() select GetWindowInfo(hWnd)).ToArray();
        }

        #region GetChildWindowsList
        public static IntPtr[] GetChildWindowsList(IntPtr hwndParent)
        {
            EnumWindows enumWindows = new EnumWindows();
            return enumWindows.GetChildWindowsList(hwndParent).ToArray();
        }
        #endregion

        #region GetChildWindowsInfoList
        public static WindowInfo[] GetChildWindowsInfoList(IntPtr hwndParent)
        {
            EnumWindows enumWindows = new EnumWindows();
            return (from hWnd in enumWindows.GetChildWindowsList(hwndParent) select GetWindowInfo(hWnd)).ToArray();
        }
        #endregion

        #region GetDesktopWindowsList()
        public static IntPtr[] GetDesktopWindowsList()
        {
            return GetDesktopWindowsList(IntPtr.Zero);
        }
        #endregion

        #region GetDesktopWindowsList(IntPtr hDesktop)
        public static IntPtr[] GetDesktopWindowsList(IntPtr hDesktop)
        {
            EnumWindows enumWindows = new EnumWindows();
            return enumWindows.GetDesktopWindowsList(hDesktop).ToArray();
        }
        #endregion

        #region GetDesktopWindowsInfoList()
        public static WindowInfo[] GetDesktopWindowsInfoList()
        {
            return GetDesktopWindowsInfoList(IntPtr.Zero);
        }
        #endregion

        #region GetDesktopWindowsInfoList(IntPtr hDesktop)
        public static WindowInfo[] GetDesktopWindowsInfoList(IntPtr hDesktop)
        {
            EnumWindows enumWindows = new EnumWindows();
            return (from hWnd in enumWindows.GetDesktopWindowsList(hDesktop) select GetWindowInfo(hWnd)).ToArray();
        }
        #endregion

        #region GetProcessWindowsList ...
        #region GetProcessWindowsList(string processName)
        public static IntPtr[] GetProcessWindowsList(string processName)
        {
            return GetProcessWindowsList(processName, true);
        }
        #endregion

        #region GetProcessWindowsList(string processName, bool mainWindow)
        public static IntPtr[] GetProcessWindowsList(string processName, bool mainWindow)
        {
            List<IntPtr> windowsList = new List<IntPtr>();
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
                windowsList.AddRange(GetProcessWindowsList(process, mainWindow));
            return windowsList.ToArray();
        }
        #endregion

        #region GetProcessWindowsList(int processId)
        public static IntPtr[] GetProcessWindowsList(int processId)
        {
            return GetProcessWindowsList(processId, true);
        }
        #endregion

        #region GetProcessWindowsList(int processId, bool mainWindow)
        public static IntPtr[] GetProcessWindowsList(int processId, bool mainWindow)
        {
            return GetProcessWindowsList(Process.GetProcessById(processId), mainWindow);
        }
        #endregion

        #region GetProcessWindowsList(Process process)
        public static IntPtr[] GetProcessWindowsList(Process process)
        {
            return GetProcessWindowsList(process, true);
        }
        #endregion

        #region GetProcessWindowsList(Process process, bool mainWindow)
        public static IntPtr[] GetProcessWindowsList(Process process, bool mainWindow)
        {
            List<IntPtr> windowsList = new List<IntPtr>();
            EnumWindows enumWindows = new EnumWindows();
            foreach (ProcessThread thread in process.Threads)
            {
                if (mainWindow)
                    windowsList.AddRange(from hWnd in enumWindows.GetThreadWindowsList(thread.Id) where User.GetParent(hWnd) == IntPtr.Zero select hWnd);
                else
                    windowsList.AddRange(enumWindows.GetThreadWindowsList(thread.Id));
            }
            return windowsList.ToArray();
        }
        #endregion
        #endregion

        #region GetProcessWindowsInfoList ...
        #region GetProcessWindowsInfoList(string processName)
        public static WindowInfo[] GetProcessWindowsInfoList(string processName)
        {
            return GetProcessWindowsInfoList(processName, true);
        }
        #endregion

        #region GetProcessWindowsInfoList(string processName, bool mainWindow)
        public static WindowInfo[] GetProcessWindowsInfoList(string processName, bool mainWindow)
        {
            List<WindowInfo> windowsList = new List<WindowInfo>();
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
                windowsList.AddRange(GetProcessWindowsInfoList(process, mainWindow));
            return windowsList.ToArray();
        }
        #endregion

        #region GetProcessWindowsInfoList(int processId)
        public static WindowInfo[] GetProcessWindowsInfoList(int processId)
        {
            return GetProcessWindowsInfoList(processId, true);
        }
        #endregion

        #region GetProcessWindowsInfoList(int processId, bool mainWindow)
        public static WindowInfo[] GetProcessWindowsInfoList(int processId, bool mainWindow)
        {
            return GetProcessWindowsInfoList(Process.GetProcessById(processId), mainWindow);
        }
        #endregion

        #region GetProcessWindowsInfoList(Process process)
        public static WindowInfo[] GetProcessWindowsInfoList(Process process)
        {
            return GetProcessWindowsInfoList(process, true);
        }
        #endregion

        #region GetProcessWindowsInfoList(Process process, bool mainWindow)
        public static WindowInfo[] GetProcessWindowsInfoList(Process process, bool mainWindow)
        {
            List<IntPtr> windowsList = new List<IntPtr>();
            EnumWindows enumWindows = new EnumWindows();
            foreach (ProcessThread thread in process.Threads)
            {
                if (mainWindow)
                    windowsList.AddRange(from hWnd in enumWindows.GetThreadWindowsList(thread.Id) where User.GetParent(hWnd) == IntPtr.Zero select hWnd);
                else
                    windowsList.AddRange(enumWindows.GetThreadWindowsList(thread.Id));
            }
            return (from hWnd in windowsList select GetWindowInfo(hWnd)).ToArray();
        }
        #endregion
        #endregion

        #region GetProcessWindowsInfoListByClassName
        public static WindowInfo[] GetProcessWindowsInfoListByClassName(Process process, string className)
        {
            List<IntPtr> windowsList = new List<IntPtr>();
            EnumWindows enumWindows = new EnumWindows();
            foreach (ProcessThread thread in process.Threads)
                windowsList.AddRange(from hWnd in enumWindows.GetThreadWindowsList(thread.Id) where GetWindowClassName(hWnd) == className select hWnd);
            return (from hWnd in windowsList select GetWindowInfo(hWnd)).ToArray();
        }
        #endregion

        #region GetThreadWindowsList
        public static IntPtr[] GetThreadWindowsList(int threadId)
        {
            EnumWindows enumWindows = new EnumWindows();
            return enumWindows.GetThreadWindowsList(threadId).ToArray();
        }
        #endregion

        #region GetThreadWindowsInfoList
        public static WindowInfo[] GetThreadWindowsInfoList(int threadId)
        {
            EnumWindows enumWindows = new EnumWindows();
            return (from hWnd in enumWindows.GetThreadWindowsList(threadId) select GetWindowInfo(hWnd)).ToArray();
        }
        #endregion

        #region GetWindowText
        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            User.GetWindowText(hWnd, sb, 256);
            return sb.ToString();
        }
        #endregion

        #region GetWindowClassName
        public static string GetWindowClassName(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            User.GetClassName(hWnd, sb, 256);
            return sb.ToString();
        }
        #endregion

        #region GetWindowInfoStruct
        public static User.Struct.WINDOWINFO GetWindowInfoStruct(IntPtr hWnd)
        {
            User.Struct.WINDOWINFO wi = new User.Struct.WINDOWINFO();
            wi.cbSize = (uint)Marshal.SizeOf(wi);
            User.GetWindowInfo(hWnd, ref wi);
            return wi;
        }
        #endregion

        #region GetWindowInfo
        public static WindowInfo GetWindowInfo(IntPtr hWnd)
        {
            WindowInfo window = new WindowInfo();

            window.hWnd = hWnd;
            window.Title = GetWindowText(hWnd);
            window.ClassName = GetWindowClassName(hWnd);
            window.wi = GetWindowInfoStruct(hWnd);


            window.ControlId = User.GetDlgCtrlID(hWnd);
            window.ParenthWnd = User.GetParent(hWnd);
            window.ControlText = GetDlgItemText(hWnd);

            uint processId;
            window.ThreadId = User.GetWindowThreadProcessId(hWnd, out processId);
            window.ProcessId = processId;
            //User.WM.wm_nu
            //User.WM.WM_UN
            //User.HookType.WH_DEBUG

            Process process = Process.GetProcessById((int)processId);
            if (process != null)
            {
                window.ProcessName = process.ProcessName;
                try
                {
                    if (process.MainModule != null)
                        window.ProcessFileName = process.MainModule.FileName;
                }
                catch
                {
                }
            }

            return window;
        }
        #endregion

        #region GetWindowStatusString
        public static string GetWindowStatusString(uint dwWindowStatus)
        {
            string sStatus = "";
            if (dwWindowStatus == User.Const.WS_ACTIVECAPTION) sStatus = "WS_ACTIVECAPTION"; else if (dwWindowStatus != 0) sStatus = dwWindowStatus.ToString();
            return sStatus;
        }
        #endregion

        #region GetWindowStyleString
        public static string GetWindowStyleString(uint dwStyle)
        {
            string sStyle = "";
            if ((dwStyle & User.Const.WS_OVERLAPPED) == User.Const.WS_OVERLAPPED) sStyle += ", WS_OVERLAPPED";
            if ((dwStyle & User.Const.WS_POPUP) == User.Const.WS_POPUP) sStyle += ", WS_POPUP";
            if ((dwStyle & User.Const.WS_CHILD) == User.Const.WS_CHILD) sStyle += ", WS_CHILD";
            if ((dwStyle & User.Const.WS_MINIMIZE) == User.Const.WS_MINIMIZE) sStyle += ", WS_MINIMIZE";
            if ((dwStyle & User.Const.WS_VISIBLE) == User.Const.WS_VISIBLE) sStyle += ", WS_VISIBLE";
            if ((dwStyle & User.Const.WS_DISABLED) == User.Const.WS_DISABLED) sStyle += ", WS_DISABLED";
            if ((dwStyle & User.Const.WS_CLIPSIBLINGS) == User.Const.WS_CLIPSIBLINGS) sStyle += ", WS_CLIPSIBLINGS";
            if ((dwStyle & User.Const.WS_CLIPCHILDREN) == User.Const.WS_CLIPCHILDREN) sStyle += ", WS_CLIPCHILDREN";
            if ((dwStyle & User.Const.WS_MAXIMIZE) == User.Const.WS_MAXIMIZE) sStyle += ", WS_MAXIMIZE";
            if ((dwStyle & User.Const.WS_BORDER) == User.Const.WS_BORDER) sStyle += ", WS_BORDER";
            if ((dwStyle & User.Const.WS_DLGFRAME) == User.Const.WS_DLGFRAME) sStyle += ", WS_DLGFRAME";
            if ((dwStyle & User.Const.WS_VSCROLL) == User.Const.WS_VSCROLL) sStyle += ", WS_VSCROLL";
            if ((dwStyle & User.Const.WS_HSCROLL) == User.Const.WS_HSCROLL) sStyle += ", WS_HSCROLL";
            if ((dwStyle & User.Const.WS_SYSMENU) == User.Const.WS_SYSMENU) sStyle += ", WS_SYSMENU";
            if ((dwStyle & User.Const.WS_THICKFRAME) == User.Const.WS_THICKFRAME) sStyle += ", WS_THICKFRAME";
            if ((dwStyle & User.Const.WS_GROUP) == User.Const.WS_GROUP) sStyle += ", WS_GROUP";
            if ((dwStyle & User.Const.WS_TABSTOP) == User.Const.WS_TABSTOP) sStyle += ", WS_TABSTOP";
            if ((dwStyle & User.Const.WS_MINIMIZEBOX) == User.Const.WS_MINIMIZEBOX) sStyle += ", WS_MINIMIZEBOX";
            if ((dwStyle & User.Const.WS_MAXIMIZEBOX) == User.Const.WS_MAXIMIZEBOX) sStyle += ", WS_MAXIMIZEBOX";
            if (sStyle != "") sStyle = sStyle.Substring(2);
            return sStyle;
        }
        #endregion

        #region GetExtendedWindowStyleString
        public static string GetExtendedWindowStyleString(uint dwExStyle)
        {
            string sStyle = "";
            if ((dwExStyle & User.Const.WS_EX_DLGMODALFRAME) == User.Const.WS_EX_DLGMODALFRAME) sStyle += ", WS_EX_DLGMODALFRAME";
            if ((dwExStyle & User.Const.WS_EX_NOPARENTNOTIFY) == User.Const.WS_EX_NOPARENTNOTIFY) sStyle += ", WS_EX_NOPARENTNOTIFY";
            if ((dwExStyle & User.Const.WS_EX_TOPMOST) == User.Const.WS_EX_TOPMOST) sStyle += ", WS_EX_TOPMOST";
            if ((dwExStyle & User.Const.WS_EX_ACCEPTFILES) == User.Const.WS_EX_ACCEPTFILES) sStyle += ", WS_EX_ACCEPTFILES";
            if ((dwExStyle & User.Const.WS_EX_TRANSPARENT) == User.Const.WS_EX_TRANSPARENT) sStyle += ", WS_EX_TRANSPARENT";
            if ((dwExStyle & User.Const.WS_EX_MDICHILD) == User.Const.WS_EX_MDICHILD) sStyle += ", WS_EX_MDICHILD";
            if ((dwExStyle & User.Const.WS_EX_TOOLWINDOW) == User.Const.WS_EX_TOOLWINDOW) sStyle += ", WS_EX_TOOLWINDOW";
            if ((dwExStyle & User.Const.WS_EX_WINDOWEDGE) == User.Const.WS_EX_WINDOWEDGE) sStyle += ", WS_EX_WINDOWEDGE";
            if ((dwExStyle & User.Const.WS_EX_CLIENTEDGE) == User.Const.WS_EX_CLIENTEDGE) sStyle += ", WS_EX_CLIENTEDGE";
            if ((dwExStyle & User.Const.WS_EX_CONTEXTHELP) == User.Const.WS_EX_CONTEXTHELP) sStyle += ", WS_EX_CONTEXTHELP";
            if ((dwExStyle & User.Const.WS_EX_RIGHT) == User.Const.WS_EX_RIGHT) sStyle += ", WS_EX_RIGHT";
            if ((dwExStyle & User.Const.WS_EX_LEFT) == User.Const.WS_EX_LEFT) sStyle += ", WS_EX_LEFT";
            if ((dwExStyle & User.Const.WS_EX_RTLREADING) == User.Const.WS_EX_RTLREADING) sStyle += ", WS_EX_RTLREADING";
            if ((dwExStyle & User.Const.WS_EX_LTRREADING) == User.Const.WS_EX_LTRREADING) sStyle += ", WS_EX_LTRREADING";
            if ((dwExStyle & User.Const.WS_EX_LEFTSCROLLBAR) == User.Const.WS_EX_LEFTSCROLLBAR) sStyle += ", WS_EX_LEFTSCROLLBAR";
            if ((dwExStyle & User.Const.WS_EX_RIGHTSCROLLBAR) == User.Const.WS_EX_RIGHTSCROLLBAR) sStyle += ", WS_EX_RIGHTSCROLLBAR";
            if ((dwExStyle & User.Const.WS_EX_CONTROLPARENT) == User.Const.WS_EX_CONTROLPARENT) sStyle += ", WS_EX_CONTROLPARENT";
            if ((dwExStyle & User.Const.WS_EX_STATICEDGE) == User.Const.WS_EX_STATICEDGE) sStyle += ", WS_EX_STATICEDGE";
            if ((dwExStyle & User.Const.WS_EX_APPWINDOW) == User.Const.WS_EX_APPWINDOW) sStyle += ", WS_EX_APPWINDOW";
            if ((dwExStyle & User.Const.WS_EX_LAYERED) == User.Const.WS_EX_LAYERED) sStyle += ", WS_EX_LAYERED";
            if ((dwExStyle & User.Const.WS_EX_NOINHERITLAYOUT) == User.Const.WS_EX_NOINHERITLAYOUT) sStyle += ", WS_EX_NOINHERITLAYOUT";
            if ((dwExStyle & User.Const.WS_EX_LAYOUTRTL) == User.Const.WS_EX_LAYOUTRTL) sStyle += ", WS_EX_LAYOUTRTL";
            if ((dwExStyle & User.Const.WS_EX_COMPOSITED) == User.Const.WS_EX_COMPOSITED) sStyle += ", WS_EX_COMPOSITED";
            if ((dwExStyle & User.Const.WS_EX_NOACTIVATE) == User.Const.WS_EX_NOACTIVATE) sStyle += ", WS_EX_NOACTIVATE";
            if (sStyle != "") sStyle = sStyle.Substring(2);
            return sStyle;
        }
        #endregion

        #region GetEndSessionLParamString
        public static string GetEndSessionLParamString(uint lParam)
        {
            switch (lParam)
            {
                case User.Const.ENDSESSION_CLOSEAPP:
                    return "ENDSESSION_CLOSEAPP";
                case User.Const.ENDSESSION_CRITICAL:
                    return "ENDSESSION_CRITICAL";
                case User.Const.ENDSESSION_LOGOFF:
                    return "ENDSESSION_LOGOFF";
            }
            return lParam.ToString();
        }
        #endregion

        #region GetDlgItemText(IntPtr hDlg, int nIDDlgItem)
        public static string GetDlgItemText(IntPtr hDlg, int nIDDlgItem)
        {
            StringBuilder sb = new StringBuilder(256);
            User.GetDlgItemText(hDlg, nIDDlgItem, sb, 256);
            return sb.ToString();
        }
        #endregion

        #region GetDlgItemText(IntPtr hCtrl)
        public static string GetDlgItemText(IntPtr hCtrl)
        {
            StringBuilder sb = new StringBuilder(256);
            User.SendMessage(hCtrl, (int)User.WM.WM_GETTEXT, (IntPtr)256, sb);
            return sb.ToString();
        }
        #endregion

        #region GetDlgItemInt(IntPtr hDlg, int nIDDlgItem)
        public static int GetDlgItemInt(IntPtr hDlg, int nIDDlgItem)
        {
            IntPtr result = IntPtr.Zero;
            return (int)User.GetDlgItemInt(hDlg, nIDDlgItem, result, true);
        }
        #endregion

        #region GetDlgItemInt(IntPtr hCtrl)
        public static int GetDlgItemInt(IntPtr hCtrl)
        {
            StringBuilder sb = new StringBuilder(256);
            User.SendMessage(hCtrl, (int)User.WM.WM_GETTEXT, (IntPtr)256, sb);
            return int.Parse(sb.ToString());
        }
        #endregion

        #region SetDlgItemText(IntPtr hDlg, int nIDDlgItem, string s)
        public static void SetDlgItemText(IntPtr hDlg, int nIDDlgItem, string s)
        {
            User.SetDlgItemText(hDlg, nIDDlgItem, s);;
        }
        #endregion

        #region SetDlgItemText(IntPtr hCtrl, string s)
        public static void SetDlgItemText(IntPtr hCtrl, string s)
        {
            User.SendMessage(hCtrl, (int)User.WM.WM_SETTEXT, (IntPtr)0, s);
        }
        #endregion

        #region class EnumWindows
        public class EnumWindows
        {
            private List<IntPtr> gWindows;
            public string TitleFilter = null;
            public string ClassNameFilter = null;

            #region GetWindowsList
            public List<IntPtr> GetWindowsList()
            {
                gWindows = new List<IntPtr>();
                User.EnumWindowsCallBackType callBackPtr = new User.EnumWindowsCallBackType(EnumWindowsCallBack);
                User.EnumWindows(callBackPtr, IntPtr.Zero);
                return gWindows;
            }
            #endregion

            #region GetThreadWindowsList
            public List<IntPtr> GetThreadWindowsList(int threadId)
            {
                gWindows = new List<IntPtr>();
                User.EnumWindowsCallBackType callBackPtr = new User.EnumWindowsCallBackType(EnumWindowsCallBack);
                User.EnumThreadWindows((uint)threadId, callBackPtr, IntPtr.Zero);
                return gWindows;
            }
            #endregion

            #region GetChildWindowsList
            public List<IntPtr> GetChildWindowsList(IntPtr hwndParent)
            {
                gWindows = new List<IntPtr>();
                User.EnumWindowsCallBackType callBackPtr = new User.EnumWindowsCallBackType(EnumWindowsCallBack);
                User.EnumChildWindows(hwndParent, callBackPtr, IntPtr.Zero);
                return gWindows;
            }
            #endregion

            #region GetDesktopWindowsList
            public List<IntPtr> GetDesktopWindowsList(IntPtr hDesktop)
            {
                gWindows = new List<IntPtr>();
                User.EnumWindowsCallBackType callBackPtr = new User.EnumWindowsCallBackType(EnumWindowsCallBack);
                User.EnumDesktopWindows(hDesktop, callBackPtr, IntPtr.Zero);
                return gWindows;
            }
            #endregion

            #region EnumWindowsCallBack
            public bool EnumWindowsCallBack(IntPtr hWnd, IntPtr lParam)
            {
                if (TitleFilter != null && !Windows.GetWindowText(hWnd).Contains(TitleFilter))
                    return true;
                if (ClassNameFilter != null && ClassNameFilter != Windows.GetWindowClassName(hWnd))
                    return true;
                gWindows.Add(hWnd);
                return true;
            }
            #endregion
        }
        #endregion

        #region GetGUIThreadInfo()
        public static User.Struct.GUITHREADINFO GetGUIThreadInfo()
        {
            return GetGUIThreadInfo(0);
        }
        #endregion

        #region GetGUIThreadInfo(uint idThread)
        public static User.Struct.GUITHREADINFO GetGUIThreadInfo(uint idThread)
        {
            User.Struct.GUITHREADINFO threadInfo = new User.Struct.GUITHREADINFO();
            threadInfo.cbSize = (uint)Marshal.SizeOf(threadInfo);

            User.GetGUIThreadInfo(idThread, out threadInfo);

            return threadInfo;
        }
        #endregion
    }
    #endregion

    #region class WindowInfo
    public class WindowInfo
    {
        public IntPtr hWnd;
        public IntPtr ParenthWnd;
        public string ClassName;
        public string Title;
        public uint ProcessId;
        public uint ThreadId;
        public string ProcessName;
        public string ProcessFileName;
        public int ControlId;
        public string ControlText;
        //public int ControlInt;
        public User.Struct.WINDOWINFO wi;
    }
    #endregion

    #region class Extension
    public static class Extension
    {
        #region zGetLowValue(this IntPtr)
        public static short zGetLowValue(this IntPtr value)
        {
            return (short)value.ToInt32();
        }
        #endregion

        #region zGetHightValue
        public static short zGetHightValue(this IntPtr value)
        {
            return (short)(value.ToInt32() >> 16);
        }
        #endregion
    }
    #endregion
}

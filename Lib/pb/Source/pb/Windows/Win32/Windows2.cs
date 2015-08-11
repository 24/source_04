using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace Win32
namespace pb.Windows.Win32
{
    public enum ComparisonType
    {
        Equals = 1,
        EqualsIgnoreCase,
        StartsWith,
        StartsWithIgnoreCase,
        Contains
    }

    public static class Windows2
    {
        // doc et todo

        // doc message :
        //
        // WM_MOUSEMOVE, WM_LBUTTONDOWN, WM_LBUTTONUP, WM_RBUTTONDOWN, WM_RBUTTONUP : http://msdn.microsoft.com/en-us/library/windows/desktop/ms645616(v=vs.85).aspx
        //   wParam : Indicates whether various virtual keys are down
        //     MK_CONTROL  0x0008 The CTRL key is down.
        //     MK_LBUTTON  0x0001 The left mouse button is down.
        //     MK_MBUTTON  0x0010 The middle mouse button is down.
        //     MK_RBUTTON  0x0002 The right mouse button is down.
        //     MK_SHIFT    0x0004 The SHIFT key is down.
        //     MK_XBUTTON1 0x0020 The first X button is down.
        //     MK_XBUTTON2 0x0040 The second X button is down.
        //   lParam :
        //     The low-order word specifies the x-coordinate of the cursor. The coordinate is relative to the upper-left corner of the client area.
        //     The high-order word specifies the y-coordinate of the cursor. The coordinate is relative to the upper-left corner of the client area.
        //
        // WM_KEYDOWN : http://msdn.microsoft.com/en-us/library/windows/desktop/ms646280(v=vs.85).aspx
        //   wParam : The virtual-key code of the nonsystem key.
        //     VK_... VK_LBUTTON, VK_RBUTTON, VK_SHIFT, VK_CONTROL, ...
        //   lParam : The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
        //     Bits   nb Meaning
        //      0-15  16 The repeat count for the current message. The value is the number of times the keystroke is autorepeated as a result of the user holding down the key. If the keystroke is held long enough, multiple messages are sent. However, the repeat count is not cumulative.
        //     16-23   8 The scan code. The value depends on the OEM.
        //     24      1 Indicates whether the key is an extended key, such as the right-hand ALT and CTRL keys that appear on an enhanced 101- or 102-key keyboard. The value is 1 if it is an extended key; otherwise, it is 0.
        //     25-28   4 Reserved; do not use.
        //     29      1 The context code. The value is always 0 for a WM_KEYDOWN message.
        //     30      1 The previous key state. The value is 1 if the key is down before the message is sent, or it is zero if the key is up.
        //     31      1 The transition state. The value is always 0 for a WM_KEYDOWN message.
        //   <00250> 002E1C56 P WM_KEYDOWN nVirtKey:VK_OEM_MINUS cRepeat:1 ScanCode:45 fExtended:1 fAltDown:0 fRepeat:0 fUp:0 [wParam:000000BD lParam:01450001 time:49:23:18.879 point:(1208, 620)]
        //     wParam:000000BD : virtual_key_code = VK_OEM_MINUS 0xBD For any country/region, the '-' key
        //     lParam:01450001 : repeat=0001, scan_code=45, extended_key=1, reserved=0, context_code=0, previous_key_state=0, transition_state=0
        //
        // WM_KEYUP : http://msdn.microsoft.com/en-us/library/windows/desktop/ms646281(v=vs.85).aspx
        //   wParam : The virtual-key code of the nonsystem key.
        //     VK_... VK_LBUTTON, VK_RBUTTON, VK_SHIFT, VK_CONTROL, ...
        //   lParam : The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
        //     Bits   nb Meaning
        //      0-15  16 The repeat count for the current message. The value is the number of times the keystroke is autorepeated as a result of the user holding down the key.
        //               The repeat count is always 1 for a WM_KEYUP message.
        //     16-23   8 The scan code. The value depends on the OEM.
        //     24      1 Indicates whether the key is an extended key, such as the right-hand ALT and CTRL keys that appear on an enhanced 101- or 102-key keyboard. The value is 1 if it is an extended key; otherwise, it is 0.
        //     25-28   4 Reserved; do not use.
        //     29      1 The context code. The value is always 0 for a WM_KEYUP message.
        //     30      1 The previous key state. The value is always 1 for a WM_KEYUP message.
        //     31      1 The transition state. The value is always 1 for a WM_KEYUP message.
        //   <00251> 002E1C56 P WM_KEYUP nVirtKey:VK_OEM_MINUS cRepeat:1 ScanCode:45 fExtended:1 fAltDown:0 fRepeat:1 fUp:1 [wParam:000000BD lParam:C1450001 time:49:23:18.879 point:(1208, 620)]
        //     wParam:000000BD : virtual_key_code = VK_OEM_MINUS 0xBD For any country/region, the '-' key
        //     lParam:C1450001 : repeat=0001, scan_code=45, extended_key=1, reserved=0, context_code=0, previous_key_state=1, transition_state=1


        // WM_MOUSEMOVE
        //<00231> 002E1C56 S WM_SETCURSOR hwnd:002E1C56 nHittest:HTCLIENT wMouseMsg:WM_MOUSEMOVE [wParam:002E1C56 lParam:02000001]
        //<00232> 002E1C56 R WM_SETCURSOR fHaltProcessing:True [lResult:00000001]
        //<00233> 002E1C56 P WM_MOUSEMOVE fwKeys:0000 xPos:341 yPos:452 [wParam:00000000 lParam:01C40155 time:49:23:17.225 point:(1208, 620)]
        //
        // WM_LBUTTONDOWN, WM_LBUTTONUP
        //<00234> 002E1C56 S WM_MOUSEACTIVATE hwndTopLevel:00101856 nHittest:HTCLIENT uMsg:WM_LBUTTONDOWN [wParam:00101856 lParam:02010001]
        //<00235> 002E1C56 R WM_MOUSEACTIVATE fuActivate:MA_ACTIVATE [lResult:00000001]
        //<00236> 002E1C56 S WM_SETCURSOR hwnd:002E1C56 nHittest:HTCLIENT wMouseMsg:WM_LBUTTONDOWN [wParam:002E1C56 lParam:02010001]
        //<00237> 002E1C56 R WM_SETCURSOR fHaltProcessing:True [lResult:00000001]
        //<00238> 002E1C56 P WM_LBUTTONDOWN fwKeys:MK_LBUTTON xPos:341 yPos:452 [wParam:00000001 lParam:01C40155 time:49:23:18.021 point:(1208, 620)]
        //<00239> 002E1C56 P WM_LBUTTONUP fwKeys:0000 xPos:341 yPos:452 [wParam:00000000 lParam:01C40155 time:49:23:18.145 point:(1208, 620)]
        //<00240> 002E1C56 S WM_CAPTURECHANGED hwndNewCapture:00000000 [wParam:00000000 lParam:00000000]
        //<00241> 002E1C56 R WM_CAPTURECHANGED lResult:00000000

        // WM_RBUTTONDOWN, WM_RBUTTONUP
        //<00245> 002E1C56 S WM_MOUSEACTIVATE hwndTopLevel:00101856 nHittest:HTCLIENT uMsg:WM_RBUTTONDOWN [wParam:00101856 lParam:02040001]
        //<00246> 002E1C56 R WM_MOUSEACTIVATE fuActivate:MA_ACTIVATE [lResult:00000001]
        //<00247> 002E1C56 S WM_SETCURSOR hwnd:002E1C56 nHittest:HTCLIENT wMouseMsg:WM_RBUTTONDOWN [wParam:002E1C56 lParam:02040001]
        //<00248> 002E1C56 R WM_SETCURSOR fHaltProcessing:True [lResult:00000001]
        //<00249> 002E1C56 P WM_RBUTTONDOWN fwKeys:MK_RBUTTON xPos:341 yPos:452 [wParam:00000002 lParam:01C40155 time:49:23:18.863 point:(1208, 620)]
        //<00250> 002E1C56 P WM_KEYDOWN nVirtKey:VK_OEM_MINUS cRepeat:1 ScanCode:45 fExtended:1 fAltDown:0 fRepeat:0 fUp:0 [wParam:000000BD lParam:01450001 time:49:23:18.879 point:(1208, 620)]
        //<00251> 002E1C56 P WM_KEYUP nVirtKey:VK_OEM_MINUS cRepeat:1 ScanCode:45 fExtended:1 fAltDown:0 fRepeat:1 fUp:1 [wParam:000000BD lParam:C1450001 time:49:23:18.879 point:(1208, 620)]
        //<00252> 002E1C56 S WM_SETCURSOR hwnd:002E1C56 nHittest:HTCLIENT wMouseMsg:WM_MOUSEMOVE [wParam:002E1C56 lParam:02000001]
        //<00253> 002E1C56 R WM_SETCURSOR fHaltProcessing:True [lResult:00000001]
        //<00254> 002E1C56 P WM_MOUSEMOVE fwKeys:MK_RBUTTON xPos:341 yPos:452 [wParam:00000002 lParam:01C40155 time:49:23:18.348 point:(1208, 620)]
        //<00255> 002E1C56 S WM_SETCURSOR hwnd:002E1C56 nHittest:HTCLIENT wMouseMsg:WM_RBUTTONUP [wParam:002E1C56 lParam:02050001]
        //<00256> 002E1C56 R WM_SETCURSOR fHaltProcessing:True [lResult:00000001]
        //<00257> 002E1C56 P WM_RBUTTONUP fwKeys:0000 xPos:341 yPos:452 [wParam:00000000 lParam:01C40155 time:49:23:19.019 point:(1208, 620)]
        //<00258> 002E1C56 S WM_SETCURSOR hwnd:002E1C56 nHittest:HTCLIENT wMouseMsg:WM_MOUSEMOVE [wParam:002E1C56 lParam:02000001]
        //<00259> 002E1C56 R WM_SETCURSOR fHaltProcessing:True [lResult:00000001]
        //<00260> 002E1C56 P WM_MOUSEMOVE fwKeys:0000 xPos:341 yPos:452 [wParam:00000000 lParam:01C40155 time:49:23:19.035 point:(1208, 620)]

        public static IEnumerable<IntPtr> GetWindowsList(string title, ComparisonType comparisonType)
        {
            if (comparisonType == ComparisonType.Equals)
                return from w in Windows.GetWindowsList() where Windows.GetWindowText(w) == title select w;
            else if (comparisonType == ComparisonType.EqualsIgnoreCase)
                return from w in Windows.GetWindowsList() where Windows.GetWindowText(w).Equals(title, StringComparison.InvariantCultureIgnoreCase) select w;
            else if (comparisonType == ComparisonType.Contains)
                return from w in Windows.GetWindowsList() where Windows.GetWindowText(w).Contains(title) select w;
            else if (comparisonType == ComparisonType.StartsWith)
                return from w in Windows.GetWindowsList() where Windows.GetWindowText(w).StartsWith(title) select w;
            else if (comparisonType == ComparisonType.StartsWithIgnoreCase)
                return from w in Windows.GetWindowsList() where Windows.GetWindowText(w).StartsWith(title, StringComparison.CurrentCultureIgnoreCase) select w;
            else
                throw new Exception(string.Format("error wrong ComparisonType {0}", comparisonType));
        }

        public static IntPtr GetWindowHandle(string title)
        {
            return GetWindowHandle(title, ComparisonType.Equals);
        }

        public static IntPtr GetWindowHandle(string title, ComparisonType comparisonType)
        {
            IEnumerable<IntPtr> windows = GetWindowsList(title, comparisonType);
            if (windows.Count() == 0)
                throw new Exception(string.Format("error window not found : title \"{0}\"", title));
            if (windows.Count() > 1)
                throw new Exception(string.Format("error {0} windows with title \"{1}\"", windows.Count(), title));
            IntPtr hwnd = windows.First();
            return hwnd;
        }

        public static void GetWindowSize(IntPtr hwnd, out int width, out int height)
        {
            User.Struct.WINDOWINFO wi = Windows.GetWindowInfoStruct(hwnd);
            width = wi.rcWindow.Right - wi.rcWindow.Left;
            height = wi.rcWindow.Bottom - wi.rcWindow.Top;
        }

        public static void ClientToScreen(IntPtr hwnd, ref int x, ref int y)
        {
            User.Struct.POINT point = new User.Struct.POINT();
            point.x = x;
            point.y = y;
            Win32.User.ClientToScreen(hwnd, ref point);
            x = point.x;
            y = point.y;
        }

        public static void ScreenToClient(IntPtr hwnd, ref int x, ref int y)
        {
            User.Struct.POINT point = new User.Struct.POINT();
            point.x = x;
            point.y = y;
            Win32.User.ScreenToClient(hwnd, ref point);
            x = point.x;
            y = point.y;
        }

        public static void GetAbsoluteCoordinates(ref int x, ref int y)
        {
            int xScreen = User.GetSystemMetrics(User.Const.SM_CXSCREEN);
            int yScreen = User.GetSystemMetrics(User.Const.SM_CYSCREEN);
            x = (x * 65535) / xScreen;
            y = (y * 65535) / yScreen;
        }

        public static void MouseMove(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseMove(x, y);
        }

        public static void MouseMove(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
        }

        public static void MouseLeftDown(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseLeftDown(x, y);
        }

        public static void MouseLeftDown(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTDOWN, x, y);
        }

        public static void MouseLeftUp(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseLeftUp(x, y);
        }

        public static void MouseLeftUp(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTUP, x, y);
        }

        public static void MouseLeftClick(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseLeftClick(x, y);
        }

        public static void MouseLeftClick(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTDOWN, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTUP, x, y);
        }

        public static void MouseLeftDoubleClick(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseLeftDoubleClick(x, y);
        }

        public static void MouseLeftDoubleClick(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTDOWN, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTUP, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTDOWN, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_LEFTUP, x, y);
        }

        public static void MouseRightDown(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseRightDown(x, y);
        }

        public static void MouseRightDown(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_RIGHTDOWN, x, y);
        }

        public static void MouseRightUp(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseRightUp(x, y);
        }

        public static void MouseRightUp(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_RIGHTUP, x, y);
        }

        public static void MouseRightClick(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseRightClick(x, y);
        }

        public static void MouseRightClick(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_RIGHTDOWN, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_RIGHTUP, x, y);
        }

        public static void MouseRightDoubleClick(IntPtr hwnd, int x, int y)
        {
            ClientToScreen(hwnd, ref x, ref y);
            MouseRightDoubleClick(x, y);
        }

        public static void MouseRightDoubleClick(int x, int y)
        {
            GetAbsoluteCoordinates(ref x, ref y);
            SendInputMouse(User.Const.MOUSEEVENTF_ABSOLUTE | User.Const.MOUSEEVENTF_MOVE, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_RIGHTDOWN, x, y);
            SendInputMouse(User.Const.MOUSEEVENTF_RIGHTUP, x, y);
        }

        public static void SendInputMouse(uint flags, int x, int y)
        {
            // SendInput doesn't perform click mouse button unless I move cursor  http://stackoverflow.com/questions/8021954/sendinput-doesnt-perform-click-mouse-button-unless-i-move-cursor
            User.Struct.INPUT input = new User.Struct.INPUT();
            input.type = User.Const.INPUT_MOUSE;
            input.mi.dx = x;
            input.mi.dy = y;
            input.mi.dwFlags = flags; //MOUSEEVENTF_ABSOLUTE, MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP, MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP
            input.mi.mouseData = 0;
            input.mi.dwExtraInfo = IntPtr.Zero;
            input.mi.time = 0;
            Windows.SendInput(input);
        }

        public static void MessageMouseLeftClick(IntPtr hwnd, int x, int y)
        {
            int lparam = (y << 16) + x;
            User.SendMessage(hwnd, (uint)User.WM.WM_LBUTTONDOWN, new IntPtr(User.Const.MK_LBUTTON), new IntPtr(lparam));
            User.SendMessage(hwnd, (uint)User.WM.WM_LBUTTONUP, IntPtr.Zero, new IntPtr(lparam));
        }

        public static void MessageMouseRightClick(IntPtr hwnd, int x, int y)
        {
            int lparamxy = (y << 16) + x;
            User.SendMessage(hwnd, (uint)User.WM.WM_RBUTTONDOWN, new IntPtr(User.Const.MK_RBUTTON), new IntPtr(lparamxy));

            //   <00250> 002E1C56 P WM_KEYDOWN nVirtKey:VK_OEM_MINUS cRepeat:1 ScanCode:45 fExtended:1 fAltDown:0 fRepeat:0 fUp:0 [wParam:000000BD lParam:01450001 time:49:23:18.879 point:(1208, 620)]
            //     wParam:000000BD : virtual_key_code = VK_OEM_MINUS 0xBD For any country/region, the '-' key
            //     lParam:01450001 : repeat=0001, scan_code=45, extended_key=1, reserved=0, context_code=0, previous_key_state=0, transition_state=0
            int lparam = 0x01450001;
            User.SendMessage(hwnd, (uint)User.WM.WM_KEYDOWN, new IntPtr((uint)User.VK.VK_OEM_MINUS), new IntPtr(lparam));

            //   <00251> 002E1C56 P WM_KEYUP nVirtKey:VK_OEM_MINUS cRepeat:1 ScanCode:45 fExtended:1 fAltDown:0 fRepeat:1 fUp:1 [wParam:000000BD lParam:C1450001 time:49:23:18.879 point:(1208, 620)]
            //     wParam:000000BD : virtual_key_code = VK_OEM_MINUS 0xBD For any country/region, the '-' key
            //     lParam:C1450001 : repeat=0001, scan_code=45, extended_key=1, reserved=0, context_code=0, previous_key_state=1, transition_state=1
            lparam = unchecked((int)0xC1450001);
            User.SendMessage(hwnd, (uint)User.WM.WM_KEYUP, new IntPtr((uint)User.VK.VK_OEM_MINUS), new IntPtr(lparam));

            User.SendMessage(hwnd, (uint)User.WM.WM_RBUTTONUP, IntPtr.Zero, new IntPtr(lparamxy));
        }
    }
}

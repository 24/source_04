using System.Runtime.InteropServices;
using HWND = System.IntPtr;

namespace Test_ScintillaNET_Bug_01
{
    public static class WM
    {
        public const int WM_CREATE = 0x0001;
        public const int WM_USER = 0x0400;
        public const int WM_ACTIVATEAPP = 0x001C;
    }

    public static class User32
    {
        [DllImport("user32")]
        public static extern int PostMessage(HWND hwnd, int wMsg, int wParam, int lParam);
    }
}

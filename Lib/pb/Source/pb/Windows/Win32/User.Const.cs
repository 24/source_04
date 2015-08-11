using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace Win32
namespace pb.Windows.Win32
{
    public abstract partial class User
    {
        #region WINVER, _WIN32_WINDOWS, _WIN32_WINNT
        /***********************************************************************
         *  Windows version        WINVER   _WIN32_WINNT    _WIN32_WINDOWS    
         *  Windows 95             0x400                                      
         *  Windows 98             0x400                    0x0410            
         *  Windows Me             0x400                    0x0490            
         *  Windows NT 4.0         0x400    0x0400                            
         *  Windows NT 4.0 SP3     0x400    0x0403                            
         *  Windows 2000           0x500    0x0500
         *  Windows XP             0x501    0x0501
         *  Windows Server 2003    0x502    0x0502
         * 
         *  http://osdir.com/ml/gnu.mingw.devel/2003-09/msg00040.html
        ***********************************************************************/
        #endregion

        #region doc msdn
        /*****************************************************************************************************************
        // Keyboard input messages (http://msdn.microsoft.com/en-us/library/ms674715(VS.85).aspx)
        // Keyboard input notifications (http://msdn.microsoft.com/en-us/library/ms674716(VS.85).aspx)
        // Keyboard accelerator messages (http://msdn.microsoft.com/en-us/library/ms674707(VS.85).aspx)
        // Keyboard accelerator notifications (http://msdn.microsoft.com/en-us/library/ms674709(VS.85).aspx)
        // Mouse input notifications (http://msdn.microsoft.com/en-us/library/ms674824(VS.85).aspx)
        // Common dialog box messages (http://msdn.microsoft.com/en-us/library/ms674695(VS.85).aspx)
        // Common dialog box notifications (http://msdn.microsoft.com/en-us/library/ms674697(VS.85).aspx)
        // Raw input notifications (http://msdn.microsoft.com/en-us/library/ms674831(VS.85).aspx)
        // Dialog box messages (http://msdn.microsoft.com/en-us/library/ms674842(VS.85).aspx)
        // Dialog box notifications (http://msdn.microsoft.com/en-us/library/ms674843(VS.85).aspx)
        // Hooks notifications (http://msdn.microsoft.com/en-us/library/ms674849(VS.85).aspx)
        // Queues notifications (http://msdn.microsoft.com/en-us/library/ms674855(VS.85).aspx)
        // MDI messages (http://msdn.microsoft.com/en-us/library/ms674861(VS.85).aspx)
        // Timer notifications (http://msdn.microsoft.com/en-us/library/ms674867(VS.85).aspx)
        // Windows messages (http://msdn.microsoft.com/en-us/library/ms674886(VS.85).aspx)
        // Windows notifications (http://msdn.microsoft.com/en-us/library/ms674887(VS.85).aspx)
        // Clipboard messages (http://msdn.microsoft.com/en-us/library/ms674558(VS.85).aspx)
        // Clipboard notifications (http://msdn.microsoft.com/en-us/library/ms674561(VS.85).aspx)
        // Data copy messages (http://msdn.microsoft.com/en-us/library/ms674575(VS.85).aspx)
        // DDE messages (http://msdn.microsoft.com/en-us/library/ms674594(VS.85).aspx)
        // DDE notifications (http://msdn.microsoft.com/en-us/library/ms674595(VS.85).aspx)
        // Cursor notifications (http://msdn.microsoft.com/en-us/library/ms674646(VS.85).aspx)
        // Icon notifications (http://msdn.microsoft.com/en-us/library/ms674655(VS.85).aspx)
        // Menu notifications (http://msdn.microsoft.com/en-us/library/ms674668(VS.85).aspx)
        *****************************************************************************************************************/
        #endregion

        #region class Const (WinUser.h)
        public abstract partial class Const
        {
            // WinUser.h (c:\Program Files\Microsoft SDKs\Windows\v6.1\Include\)


            //#include <macwin32.h> // _MAC

            //#include <stdarg.h>

            /*
             * Predefined Resource Types
             */
            //#define RT_CURSOR           MAKEINTRESOURCE(1)
            //#define RT_BITMAP           MAKEINTRESOURCE(2)
            //#define RT_ICON             MAKEINTRESOURCE(3)
            //#define RT_MENU             MAKEINTRESOURCE(4)
            //#define RT_DIALOG           MAKEINTRESOURCE(5)
            //#define RT_STRING           MAKEINTRESOURCE(6)
            //#define RT_FONTDIR          MAKEINTRESOURCE(7)
            //#define RT_FONT             MAKEINTRESOURCE(8)
            //#define RT_ACCELERATOR      MAKEINTRESOURCE(9)
            //#define RT_RCDATA           MAKEINTRESOURCE(10)
            //#define RT_MESSAGETABLE     MAKEINTRESOURCE(11)

            //#define DIFFERENCE     11
            //#define RT_GROUP_CURSOR MAKEINTRESOURCE((ULONG_PTR)(RT_CURSOR) + DIFFERENCE)
            //#define RT_GROUP_ICON   MAKEINTRESOURCE((ULONG_PTR)(RT_ICON) + DIFFERENCE)
            //#define RT_VERSION      MAKEINTRESOURCE(16)
            //#define RT_DLGINCLUDE   MAKEINTRESOURCE(17)
            //#define RT_PLUGPLAY     MAKEINTRESOURCE(19)
            //#define RT_VXD          MAKEINTRESOURCE(20)
            //#define RT_ANICURSOR    MAKEINTRESOURCE(21)
            //#define RT_ANIICON      MAKEINTRESOURCE(22)
            //#define RT_HTML         MAKEINTRESOURCE(23)


            /*
             * SPI_SETDESKWALLPAPER defined constants
             */
            //#define SETWALLPAPER_DEFAULT    ((LPWSTR)-1)

            /*
             * Scroll Bar Constants
             */
            public const int SB_HORZ             = 0;
            public const int SB_VERT             = 1;
            public const int SB_CTL              = 2;
            public const int SB_BOTH             = 3;

            /*
             * Scroll Bar Commands
             */
            public const int SB_LINEUP           = 0;
            public const int SB_LINELEFT         = 0;
            public const int SB_LINEDOWN         = 1;
            public const int SB_LINERIGHT        = 1;
            public const int SB_PAGEUP           = 2;
            public const int SB_PAGELEFT         = 2;
            public const int SB_PAGEDOWN         = 3;
            public const int SB_PAGERIGHT        = 3;
            public const int SB_THUMBPOSITION    = 4;
            public const int SB_THUMBTRACK       = 5;
            public const int SB_TOP              = 6;
            public const int SB_LEFT             = 6;
            public const int SB_BOTTOM           = 7;
            public const int SB_RIGHT            = 7;
            public const int SB_ENDSCROLL        = 8;

            /*
             * ShowWindow() Commands
             */
            public const int SW_HIDE             = 0;
            public const int SW_SHOWNORMAL       = 1;
            public const int SW_NORMAL           = 1;
            public const int SW_SHOWMINIMIZED    = 2;
            public const int SW_SHOWMAXIMIZED    = 3;
            public const int SW_MAXIMIZE         = 3;
            public const int SW_SHOWNOACTIVATE   = 4;
            public const int SW_SHOW             = 5;
            public const int SW_MINIMIZE         = 6;
            public const int SW_SHOWMINNOACTIVE  = 7;
            public const int SW_SHOWNA           = 8;
            public const int SW_RESTORE          = 9;
            public const int SW_SHOWDEFAULT      = 10;
            public const int SW_FORCEMINIMIZE    = 11;
            public const int SW_MAX              = 11;


            /*
             * Old ShowWindow() Commands
             */
            public const int HIDE_WINDOW         = 0;
            public const int SHOW_OPENWINDOW     = 1;
            public const int SHOW_ICONWINDOW     = 2;
            public const int SHOW_FULLSCREEN     = 3;
            public const int SHOW_OPENNOACTIVATE = 4;

            /*
             * Identifiers for the WM_SHOWWINDOW message
             */
            public const int SW_PARENTCLOSING    = 1;
            public const int SW_OTHERZOOM        = 2;
            public const int SW_PARENTOPENING    = 3;
            public const int SW_OTHERUNZOOM      = 4;

            /*
             * AnimateWindow() Commands
             */
            public const int AW_HOR_POSITIVE             = 0x00000001;
            public const int AW_HOR_NEGATIVE             = 0x00000002;
            public const int AW_VER_POSITIVE             = 0x00000004;
            public const int AW_VER_NEGATIVE             = 0x00000008;
            public const int AW_CENTER                   = 0x00000010;
            public const int AW_HIDE                     = 0x00010000;
            public const int AW_ACTIVATE                 = 0x00020000;
            public const int AW_SLIDE                    = 0x00040000;
            public const int AW_BLEND                    = 0x00080000;

            /*
             * WM_KEYUP/DOWN/CHAR HIWORD(lParam) flags
             */
            public const int KF_EXTENDED       = 0x0100;
            public const int KF_DLGMODE        = 0x0800;
            public const int KF_MENUMODE       = 0x1000;
            public const int KF_ALTDOWN        = 0x2000;
            public const int KF_REPEAT         = 0x4000;
            public const int KF_UP             = 0x8000;

            /*
             * SetWindowsHook() codes
             */
            //public const int WH_MIN              = (-1);
            //public const int WH_MSGFILTER        = (-1);
            //public const int WH_JOURNALRECORD    = 0;
            //public const int WH_JOURNALPLAYBACK  = 1;
            //public const int WH_KEYBOARD         = 2;
            //public const int WH_GETMESSAGE       = 3;
            //public const int WH_CALLWNDPROC      = 4;
            //public const int WH_CBT              = 5;
            //public const int WH_SYSMSGFILTER     = 6;
            //public const int WH_MOUSE            = 7;
            //public const int WH_HARDWARE         = 8;
            //public const int WH_DEBUG            = 9;
            //public const int WH_SHELL           = 10;
            //public const int WH_FOREGROUNDIDLE  = 11;
            //public const int WH_CALLWNDPROCRET  = 12;
            //public const int WH_KEYBOARD_LL     = 13;
            //public const int WH_MOUSE_LL        = 14;

          //public const int WH_MAX             = 14; // WINVER >= 0x0400 && _WIN32_WINNT >= 0x0400
          //public const int WH_MAX             = 12; // WINVER >= 0x0400 && _WIN32_WINNT < 0x0400
          //public const int WH_MAX             = 11; // else

            //public const int WH_MINHOOK         = WH_MIN;
            //public const int WH_MAXHOOK         = WH_MAX;

            // Hook Codes
            public const int HC_ACTION           = 0;
            public const int HC_GETNEXT          = 1;
            public const int HC_SKIP             = 2;
            public const int HC_NOREMOVE         = 3;
            public const int HC_NOREM            = HC_NOREMOVE;
            public const int HC_SYSMODALON       = 4;
            public const int HC_SYSMODALOFF      = 5;

            // CBT Hook Codes
            public const int HCBT_MOVESIZE       = 0;
            public const int HCBT_MINMAX         = 1;
            public const int HCBT_QS             = 2;
            public const int HCBT_CREATEWND      = 3;
            public const int HCBT_DESTROYWND     = 4;
            public const int HCBT_ACTIVATE       = 5;
            public const int HCBT_CLICKSKIPPED   = 6;
            public const int HCBT_KEYSKIPPED     = 7;
            public const int HCBT_SYSCOMMAND     = 8;
            public const int HCBT_SETFOCUS       = 9;


            // codes passed in WPARAM for WM_WTSSESSION_CHANGE
            public const int WTS_CONSOLE_CONNECT                = 0x1;
            public const int WTS_CONSOLE_DISCONNECT             = 0x2;
            public const int WTS_REMOTE_CONNECT                 = 0x3;
            public const int WTS_REMOTE_DISCONNECT              = 0x4;
            public const int WTS_SESSION_LOGON                  = 0x5;
            public const int WTS_SESSION_LOGOFF                 = 0x6;
            public const int WTS_SESSION_LOCK                   = 0x7;
            public const int WTS_SESSION_UNLOCK                 = 0x8;
            public const int WTS_SESSION_REMOTE_CONTROL         = 0x9;

            // WH_MSGFILTER Filter Proc Codes
            public const int MSGF_DIALOGBOX      = 0;
            public const int MSGF_MESSAGEBOX     = 1;
            public const int MSGF_MENU           = 2;
            public const int MSGF_SCROLLBAR      = 5;
            public const int MSGF_NEXTWINDOW     = 6;
            public const int MSGF_MAX            = 8;                       // unused
            public const int MSGF_USER           = 4096;

            // Shell support
            public const int HSHELL_WINDOWCREATED        = 1;
            public const int HSHELL_WINDOWDESTROYED      = 2;
            public const int HSHELL_ACTIVATESHELLWINDOW  = 3;

            public const int HSHELL_WINDOWACTIVATED      = 4;
            public const int HSHELL_GETMINRECT           = 5;
            public const int HSHELL_REDRAW               = 6;
            public const int HSHELL_TASKMAN              = 7;
            public const int HSHELL_LANGUAGE             = 8;
            public const int HSHELL_SYSMENU              = 9;
            public const int HSHELL_ENDTASK              = 10;
            public const int HSHELL_ACCESSIBILITYSTATE   = 11;
            public const int HSHELL_APPCOMMAND           = 12;
            public const int HSHELL_WINDOWREPLACED       = 13;
            public const int HSHELL_WINDOWREPLACING      = 14;

            public const int HSHELL_HIGHBIT            = 0x8000;
            public const int HSHELL_FLASH              = (HSHELL_REDRAW|HSHELL_HIGHBIT);
            public const int HSHELL_RUDEAPPACTIVATED   = (HSHELL_WINDOWACTIVATED|HSHELL_HIGHBIT);

            /* cmd for HSHELL_APPCOMMAND and WM_APPCOMMAND */
            public const int APPCOMMAND_BROWSER_BACKWARD       = 1;
            public const int APPCOMMAND_BROWSER_FORWARD        = 2;
            public const int APPCOMMAND_BROWSER_REFRESH        = 3;
            public const int APPCOMMAND_BROWSER_STOP           = 4;
            public const int APPCOMMAND_BROWSER_SEARCH         = 5;
            public const int APPCOMMAND_BROWSER_FAVORITES      = 6;
            public const int APPCOMMAND_BROWSER_HOME           = 7;
            public const int APPCOMMAND_VOLUME_MUTE            = 8;
            public const int APPCOMMAND_VOLUME_DOWN            = 9;
            public const int APPCOMMAND_VOLUME_UP              = 10;
            public const int APPCOMMAND_MEDIA_NEXTTRACK        = 11;
            public const int APPCOMMAND_MEDIA_PREVIOUSTRACK    = 12;
            public const int APPCOMMAND_MEDIA_STOP             = 13;
            public const int APPCOMMAND_MEDIA_PLAY_PAUSE       = 14;
            public const int APPCOMMAND_LAUNCH_MAIL            = 15;
            public const int APPCOMMAND_LAUNCH_MEDIA_SELECT    = 16;
            public const int APPCOMMAND_LAUNCH_APP1            = 17;
            public const int APPCOMMAND_LAUNCH_APP2            = 18;
            public const int APPCOMMAND_BASS_DOWN              = 19;
            public const int APPCOMMAND_BASS_BOOST             = 20;
            public const int APPCOMMAND_BASS_UP                = 21;
            public const int APPCOMMAND_TREBLE_DOWN            = 22;
            public const int APPCOMMAND_TREBLE_UP              = 23;
            public const int APPCOMMAND_MICROPHONE_VOLUME_MUTE = 24;
            public const int APPCOMMAND_MICROPHONE_VOLUME_DOWN = 25;
            public const int APPCOMMAND_MICROPHONE_VOLUME_UP   = 26;
            public const int APPCOMMAND_HELP                   = 27;
            public const int APPCOMMAND_FIND                   = 28;
            public const int APPCOMMAND_NEW                    = 29;
            public const int APPCOMMAND_OPEN                   = 30;
            public const int APPCOMMAND_CLOSE                  = 31;
            public const int APPCOMMAND_SAVE                   = 32;
            public const int APPCOMMAND_PRINT                  = 33;
            public const int APPCOMMAND_UNDO                   = 34;
            public const int APPCOMMAND_REDO                   = 35;
            public const int APPCOMMAND_COPY                   = 36;
            public const int APPCOMMAND_CUT                    = 37;
            public const int APPCOMMAND_PASTE                  = 38;
            public const int APPCOMMAND_REPLY_TO_MAIL          = 39;
            public const int APPCOMMAND_FORWARD_MAIL           = 40;
            public const int APPCOMMAND_SEND_MAIL              = 41;
            public const int APPCOMMAND_SPELL_CHECK            = 42;
            public const int APPCOMMAND_DICTATE_OR_COMMAND_CONTROL_TOGGLE    = 43;
            public const int APPCOMMAND_MIC_ON_OFF_TOGGLE      = 44;
            public const int APPCOMMAND_CORRECTION_LIST        = 45;
            public const int APPCOMMAND_MEDIA_PLAY             = 46;
            public const int APPCOMMAND_MEDIA_PAUSE            = 47;
            public const int APPCOMMAND_MEDIA_RECORD           = 48;
            public const int APPCOMMAND_MEDIA_FAST_FORWARD     = 49;
            public const int APPCOMMAND_MEDIA_REWIND           = 50;
            public const int APPCOMMAND_MEDIA_CHANNEL_UP       = 51;
            public const int APPCOMMAND_MEDIA_CHANNEL_DOWN     = 52;
            public const int APPCOMMAND_DELETE                 = 53;
            public const int APPCOMMAND_DWM_FLIP3D             = 54;

            public const int FAPPCOMMAND_MOUSE = 0x8000;
            public const int FAPPCOMMAND_KEY   = 0;
            public const int FAPPCOMMAND_OEM   = 0x1000;
            public const int FAPPCOMMAND_MASK  = 0xF000;

            //#define GET_APPCOMMAND_LPARAM(lParam) ((short)(HIWORD(lParam) & ~FAPPCOMMAND_MASK))
            //#define GET_DEVICE_LPARAM(lParam)     ((WORD)(HIWORD(lParam) & FAPPCOMMAND_MASK))
            //#define GET_MOUSEORKEY_LPARAM         GET_DEVICE_LPARAM
            //#define GET_FLAGS_LPARAM(lParam)      (LOWORD(lParam))
            //#define GET_KEYSTATE_LPARAM(lParam)   GET_FLAGS_LPARAM(lParam)

            // Low level hook flags
            //#define LLKHF_EXTENDED       (KF_EXTENDED >> 8)
            //#define LLKHF_INJECTED       0x00000010
            //#define LLKHF_ALTDOWN        (KF_ALTDOWN >> 8)
            //#define LLKHF_UP             (KF_UP >> 8)

            public const int LLMHF_INJECTED       = 0x00000001;

            // Keyboard Layout API
            public const int HKL_PREV            = 0;
            public const int HKL_NEXT            = 1;

            public const int KLF_ACTIVATE        = 0x00000001;
            public const int KLF_SUBSTITUTE_OK   = 0x00000002;
            public const int KLF_REORDER         = 0x00000008;
            public const int KLF_REPLACELANG     = 0x00000010;
            public const int KLF_NOTELLSHELL     = 0x00000080;
            public const int KLF_SETFORPROCESS   = 0x00000100;
            public const int KLF_SHIFTLOCK       = 0x00010000;
            public const int KLF_RESET           = 0x40000000;

            // Bits in wParam of WM_INPUTLANGCHANGEREQUEST message
            public const int INPUTLANGCHANGE_SYSCHARSET = 0x0001;
            public const int INPUTLANGCHANGE_FORWARD    = 0x0002;
            public const int INPUTLANGCHANGE_BACKWARD   = 0x0004;

            // Size of KeyboardLayoutName (number of characters), including nul terminator
            public const int KL_NAMELENGTH = 9;

            // Values for resolution parameter of GetMouseMovePointsEx
            public const int GMMP_USE_DISPLAY_POINTS          = 1;
            public const int GMMP_USE_HIGH_RESOLUTION_POINTS  = 2;

            // Desktop-specific access flags
            public const int DESKTOP_READOBJECTS         = 0x0001;
            public const int DESKTOP_CREATEWINDOW        = 0x0002;
            public const int DESKTOP_CREATEMENU          = 0x0004;
            public const int DESKTOP_HOOKCONTROL         = 0x0008;
            public const int DESKTOP_JOURNALRECORD       = 0x0010;
            public const int DESKTOP_JOURNALPLAYBACK     = 0x0020;
            public const int DESKTOP_ENUMERATE           = 0x0040;
            public const int DESKTOP_WRITEOBJECTS        = 0x0080;
            public const int DESKTOP_SWITCHDESKTOP       = 0x0100;

            // Desktop-specific control flags
            public const int DF_ALLOWOTHERACCOUNTHOOK    = 0x0001;

            // Windowstation-specific access flags
            public const int WINSTA_ENUMDESKTOPS         = 0x0001;
            public const int WINSTA_READATTRIBUTES       = 0x0002;
            public const int WINSTA_ACCESSCLIPBOARD      = 0x0004;
            public const int WINSTA_CREATEDESKTOP        = 0x0008;
            public const int WINSTA_WRITEATTRIBUTES      = 0x0010;
            public const int WINSTA_ACCESSGLOBALATOMS    = 0x0020;
            public const int WINSTA_EXITWINDOWS          = 0x0040;
            public const int WINSTA_ENUMERATE            = 0x0100;
            public const int WINSTA_READSCREEN           = 0x0200;

            public const int WINSTA_ALL_ACCESS           = (WINSTA_ENUMDESKTOPS  | WINSTA_READATTRIBUTES  | WINSTA_ACCESSCLIPBOARD | WINSTA_CREATEDESKTOP | WINSTA_WRITEATTRIBUTES 
                                                            | WINSTA_ACCESSGLOBALATOMS | WINSTA_EXITWINDOWS   | WINSTA_ENUMERATE       | WINSTA_READSCREEN);

            // Windowstation creation flags.
            public const int CWF_CREATE_ONLY          = 0x00000001;

            // Windowstation-specific attribute flags
            public const int WSF_VISIBLE                 = 0x0001;

            public const int UOI_FLAGS       = 1;
            public const int UOI_NAME        = 2;
            public const int UOI_TYPE        = 3;
            public const int UOI_USER_SID    = 4;
            public const int UOI_HEAPSIZE    = 5;
            public const int UOI_IO          = 6;


            //#define POINTSTOPOINT(pt, pts)                          \
            //        { (pt).x = (LONG)(SHORT)LOWORD(*(LONG*)&pts);   \
            //          (pt).y = (LONG)(SHORT)HIWORD(*(LONG*)&pts); }

            //#define POINTTOPOINTS(pt)      (MAKELONG((short)((pt).x), (short)((pt).y)))
            //#define MAKEWPARAM(l, h)      ((WPARAM)(DWORD)MAKELONG(l, h))
            //#define MAKELPARAM(l, h)      ((LPARAM)(DWORD)MAKELONG(l, h))
            //#define MAKELRESULT(l, h)     ((LRESULT)(DWORD)MAKELONG(l, h))

            // Window field offsets for GetWindowLong()
            public const int GWL_WNDPROC         = (-4);
            public const int GWL_HINSTANCE       = (-6);
            public const int GWL_HWNDPARENT      = (-8);
            public const int GWL_STYLE           = (-16);
            public const int GWL_EXSTYLE         = (-20);
            public const int GWL_USERDATA        = (-21);
            public const int GWL_ID              = (-12);

            public const int GWLP_WNDPROC        = (-4);
            public const int GWLP_HINSTANCE      = (-6);
            public const int GWLP_HWNDPARENT     = (-8);
            public const int GWLP_USERDATA       = (-21);
            public const int GWLP_ID             = (-12);

            // Class field offsets for GetClassLong()
            public const int GCL_MENUNAME        = (-8);
            public const int GCL_HBRBACKGROUND   = (-10);
            public const int GCL_HCURSOR         = (-12);
            public const int GCL_HICON           = (-14);
            public const int GCL_HMODULE         = (-16);
            public const int GCL_CBWNDEXTRA      = (-18);
            public const int GCL_CBCLSEXTRA      = (-20);
            public const int GCL_WNDPROC         = (-24);
            public const int GCL_STYLE           = (-26);
            public const int GCW_ATOM            = (-32);

            public const int GCL_HICONSM         = (-34);

            public const int GCLP_MENUNAME       = (-8);
            public const int GCLP_HBRBACKGROUND  = (-10);
            public const int GCLP_HCURSOR        = (-12);
            public const int GCLP_HICON          = (-14);
            public const int GCLP_HMODULE        = (-16);
            public const int GCLP_WNDPROC        = (-24);
            public const int GCLP_HICONSM        = (-34);



            //public const int WM_NULL                         = 0x0000;
            //public const int WM_CREATE                       = 0x0001;
            //public const int WM_DESTROY                      = 0x0002;
            //public const int WM_MOVE                         = 0x0003;
            //public const int WM_SIZE                         = 0x0005;

            //public const int WM_ACTIVATE                     = 0x0006;
            // WM_ACTIVATE state values
            //public const int WA_INACTIVE                     = 0;
            //public const int WA_ACTIVE                       = 1;
            //public const int WA_CLICKACTIVE                  = 2;

            //public const int WM_SETFOCUS                     = 0x0007;
            //public const int WM_KILLFOCUS                    = 0x0008;
            //public const int WM_ENABLE                       = 0x000A;
            //public const int WM_SETREDRAW                    = 0x000B;
            //public const int WM_SETTEXT                      = 0x000C;
            //public const int WM_GETTEXT                      = 0x000D;
            //public const int WM_GETTEXTLENGTH                = 0x000E;
            //public const int WM_PAINT                        = 0x000F;
            //public const int WM_CLOSE                        = 0x0010;
            //public const int WM_QUERYENDSESSION              = 0x0011;
            //public const int WM_QUERYOPEN                    = 0x0013;
            //public const int WM_ENDSESSION                   = 0x0016;
            //public const int WM_QUIT                         = 0x0012;
            //public const int WM_ERASEBKGND                   = 0x0014;
            //public const int WM_SYSCOLORCHANGE               = 0x0015;
            //public const int WM_SHOWWINDOW                   = 0x0018;
            //public const int WM_WININICHANGE                 = 0x001A;
            //public const int WM_SETTINGCHANGE                = WM_WININICHANGE;

            //public const int WM_DEVMODECHANGE                = 0x001B;
            //public const int WM_ACTIVATEAPP                  = 0x001C;
            //public const int WM_FONTCHANGE                   = 0x001D;
            //public const int WM_TIMECHANGE                   = 0x001E;
            //public const int WM_CANCELMODE                   = 0x001F;
            //public const int WM_SETCURSOR                    = 0x0020;
            //public const int WM_MOUSEACTIVATE                = 0x0021;
            //public const int WM_CHILDACTIVATE                = 0x0022;
            //public const int WM_QUEUESYNC                    = 0x0023;

            //public const int WM_GETMINMAXINFO                = 0x0024;

            //public const int WM_PAINTICON                    = 0x0026;
            //public const int WM_ICONERASEBKGND               = 0x0027;
            //public const int WM_NEXTDLGCTL                   = 0x0028;
            //public const int WM_SPOOLERSTATUS                = 0x002A;
            //public const int WM_DRAWITEM                     = 0x002B;
            //public const int WM_MEASUREITEM                  = 0x002C;
            //public const int WM_DELETEITEM                   = 0x002D;
            //public const int WM_VKEYTOITEM                   = 0x002E;
            //public const int WM_CHARTOITEM                   = 0x002F;
            //public const int WM_SETFONT                      = 0x0030;
            //public const int WM_GETFONT                      = 0x0031;
            //public const int WM_SETHOTKEY                    = 0x0032;
            //public const int WM_GETHOTKEY                    = 0x0033;
            //public const int WM_QUERYDRAGICON                = 0x0037;
            //public const int WM_COMPAREITEM                  = 0x0039;
            //public const int WM_GETOBJECT                    = 0x003D;
            //public const int WM_COMPACTING                   = 0x0041;
            //public const int WM_COMMNOTIFY                   = 0x0044  /* no longer suported */;
            //public const int WM_WINDOWPOSCHANGING            = 0x0046;
            //public const int WM_WINDOWPOSCHANGED             = 0x0047;

            //public const int WM_POWER                        = 0x0048;
            // wParam for WM_POWER window message and DRV_POWER driver notification
            public const int PWR_OK                          = 1;
            public const int PWR_FAIL                        = (-1);
            public const int PWR_SUSPENDREQUEST              = 1;
            public const int PWR_SUSPENDRESUME               = 2;
            public const int PWR_CRITICALRESUME              = 3;

            //public const int WM_COPYDATA                     = 0x004A;
            //public const int WM_CANCELJOURNAL                = 0x004B;

            //public const int WM_NOTIFY                       = 0x004E;
            //public const int WM_INPUTLANGCHANGEREQUEST       = 0x0050;
            //public const int WM_INPUTLANGCHANGE              = 0x0051;
            //public const int WM_TCARD                        = 0x0052;
            //public const int WM_HELP                         = 0x0053;
            //public const int WM_USERCHANGED                  = 0x0054;
            //public const int WM_NOTIFYFORMAT                 = 0x0055;

            public const int NFR_ANSI                             = 1;
            public const int NFR_UNICODE                          = 2;
            public const int NF_QUERY                             = 3;
            public const int NF_REQUERY                           = 4;

            //public const int WM_CONTEXTMENU                  = 0x007B;
            //public const int WM_STYLECHANGING                = 0x007C;
            //public const int WM_STYLECHANGED                 = 0x007D;
            //public const int WM_DISPLAYCHANGE                = 0x007E;
            //public const int WM_GETICON                      = 0x007F;
            //public const int WM_SETICON                      = 0x0080;

            //public const int WM_NCCREATE                     = 0x0081;
            //public const int WM_NCDESTROY                    = 0x0082;
            //public const int WM_NCCALCSIZE                   = 0x0083;
            //public const int WM_NCHITTEST                    = 0x0084;
            //public const int WM_NCPAINT                      = 0x0085;
            //public const int WM_NCACTIVATE                   = 0x0086;
            //public const int WM_GETDLGCODE                   = 0x0087;
            //public const int WM_SYNCPAINT                    = 0x0088;
            //public const int WM_NCMOUSEMOVE                  = 0x00A0;
            //public const int WM_NCLBUTTONDOWN                = 0x00A1;
            //public const int WM_NCLBUTTONUP                  = 0x00A2;
            //public const int WM_NCLBUTTONDBLCLK              = 0x00A3;
            //public const int WM_NCRBUTTONDOWN                = 0x00A4;
            //public const int WM_NCRBUTTONUP                  = 0x00A5;
            //public const int WM_NCRBUTTONDBLCLK              = 0x00A6;
            //public const int WM_NCMBUTTONDOWN                = 0x00A7;
            //public const int WM_NCMBUTTONUP                  = 0x00A8;
            //public const int WM_NCMBUTTONDBLCLK              = 0x00A9;

            //public const int WM_NCXBUTTONDOWN                = 0x00AB;
            //public const int WM_NCXBUTTONUP                  = 0x00AC;
            //public const int WM_NCXBUTTONDBLCLK              = 0x00AD;

            //public const int WM_INPUT_DEVICE_CHANGE          = 0x00FE;

            //public const int WM_INPUT                        = 0x00FF;

            //public const int WM_KEYFIRST                     = 0x0100;
            //public const int WM_KEYDOWN                      = 0x0100;
            //public const int WM_KEYUP                        = 0x0101;
            //public const int WM_CHAR                         = 0x0102;
            //public const int WM_DEADCHAR                     = 0x0103;
            //public const int WM_SYSKEYDOWN                   = 0x0104;
            //public const int WM_SYSKEYUP                     = 0x0105;
            //public const int WM_SYSCHAR                      = 0x0106;
            //public const int WM_SYSDEADCHAR                  = 0x0107;
            //public const int WM_UNICHAR                      = 0x0109;
            //public const int WM_KEYLAST                      = 0x0109;
            public const int UNICODE_NOCHAR                  = 0xFFFF;
          //public const int WM_KEYLAST                      = 0x0108; //_WIN32_WINNT < 0x0501

            //public const int WM_IME_STARTCOMPOSITION         = 0x010D;
            //public const int WM_IME_ENDCOMPOSITION           = 0x010E;
            //public const int WM_IME_COMPOSITION              = 0x010F;
            //public const int WM_IME_KEYLAST                  = 0x010F;

            //public const int WM_INITDIALOG                   = 0x0110;
            //public const int WM_COMMAND                      = 0x0111;
            //public const int WM_SYSCOMMAND                   = 0x0112;
            //public const int WM_TIMER                        = 0x0113;
            //public const int WM_HSCROLL                      = 0x0114;
            //public const int WM_VSCROLL                      = 0x0115;
            //public const int WM_INITMENU                     = 0x0116;
            //public const int WM_INITMENUPOPUP                = 0x0117;
            //public const int WM_MENUSELECT                   = 0x011F;
            //public const int WM_MENUCHAR                     = 0x0120;
            //public const int WM_ENTERIDLE                    = 0x0121;
            //public const int WM_MENURBUTTONUP                = 0x0122;
            //public const int WM_MENUDRAG                     = 0x0123;
            //public const int WM_MENUGETOBJECT                = 0x0124;
            //public const int WM_UNINITMENUPOPUP              = 0x0125;
            //public const int WM_MENUCOMMAND                  = 0x0126;

            //public const int WM_CHANGEUISTATE                = 0x0127;
            //public const int WM_UPDATEUISTATE                = 0x0128;
            //public const int WM_QUERYUISTATE                 = 0x0129;

            // LOWORD(wParam) values in WM_*UISTATE*
            public const int UIS_SET                         = 1;
            public const int UIS_CLEAR                       = 2;
            public const int UIS_INITIALIZE                  = 3;

            // HIWORD(wParam) values in WM_*UISTATE*
            public const int UISF_HIDEFOCUS                  = 0x1;
            public const int UISF_HIDEACCEL                  = 0x2;
            public const int UISF_ACTIVE                     = 0x4;

            //public const int WM_CTLCOLORMSGBOX               = 0x0132;
            //public const int WM_CTLCOLOREDIT                 = 0x0133;
            //public const int WM_CTLCOLORLISTBOX              = 0x0134;
            //public const int WM_CTLCOLORBTN                  = 0x0135;
            //public const int WM_CTLCOLORDLG                  = 0x0136;
            //public const int WM_CTLCOLORSCROLLBAR            = 0x0137;
            //public const int WM_CTLCOLORSTATIC               = 0x0138;
            public const int MN_GETHMENU                     = 0x01E1;

            //public const int WM_MOUSEFIRST                   = 0x0200;
            //public const int WM_MOUSEMOVE                    = 0x0200;
            //public const int WM_LBUTTONDOWN                  = 0x0201;
            //public const int WM_LBUTTONUP                    = 0x0202;
            //public const int WM_LBUTTONDBLCLK                = 0x0203;
            //public const int WM_RBUTTONDOWN                  = 0x0204;
            //public const int WM_RBUTTONUP                    = 0x0205;
            //public const int WM_RBUTTONDBLCLK                = 0x0206;
            //public const int WM_MBUTTONDOWN                  = 0x0207;
            //public const int WM_MBUTTONUP                    = 0x0208;
            //public const int WM_MBUTTONDBLCLK                = 0x0209;
            //public const int WM_MOUSEWHEEL                   = 0x020A;
            //public const int WM_XBUTTONDOWN                  = 0x020B;
            //public const int WM_XBUTTONUP                    = 0x020C;
            //public const int WM_XBUTTONDBLCLK                = 0x020D;
            //public const int WM_MOUSEHWHEEL                  = 0x020E;

            //public const int WM_MOUSELAST                    = 0x020E; //#if (_WIN32_WINNT >= 0x0600)
          //public const int WM_MOUSELAST                    = 0x020D; //#elif (_WIN32_WINNT >= 0x0500)
          //public const int WM_MOUSELAST                    = 0x020A; //#elif (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
          //public const int WM_MOUSELAST                    = 0x0209; //#else

            // Value for rolling one detent
            public const int WHEEL_DELTA                     = 120;
            //#define GET_WHEEL_DELTA_WPARAM(wParam)  ((short)HIWORD(wParam))

            // Setting to scroll one page for SPI_GET/SETWHEELSCROLLLINES
            //#define WHEEL_PAGESCROLL                (UINT_MAX)

            //#define GET_KEYSTATE_WPARAM(wParam)     (LOWORD(wParam))
            //#define GET_NCHITTEST_WPARAM(wParam)    ((short)LOWORD(wParam))
            //#define GET_XBUTTON_WPARAM(wParam)      (HIWORD(wParam))

            // XButton values are WORD flags
            public const int XBUTTON1      = 0x0001;
            public const int XBUTTON2      = 0x0002;
            // Were there to be an XBUTTON3, its value would be 0x0004

            //public const int WM_PARENTNOTIFY                 = 0x0210;
            //public const int WM_ENTERMENULOOP                = 0x0211;
            //public const int WM_EXITMENULOOP                 = 0x0212;

            //public const int WM_NEXTMENU                     = 0x0213;
            //public const int WM_SIZING                       = 0x0214;
            //public const int WM_CAPTURECHANGED               = 0x0215;
            //public const int WM_MOVING                       = 0x0216;

            //public const int WM_POWERBROADCAST               = 0x0218;

            //#ifndef _WIN32_WCE
            public const int PBT_APMQUERYSUSPEND             = 0x0000;
            public const int PBT_APMQUERYSTANDBY             = 0x0001;
            public const int PBT_APMQUERYSUSPENDFAILED       = 0x0002;
            public const int PBT_APMQUERYSTANDBYFAILED       = 0x0003;
            public const int PBT_APMSUSPEND                  = 0x0004;
            public const int PBT_APMSTANDBY                  = 0x0005;
            public const int PBT_APMRESUMECRITICAL           = 0x0006;
            public const int PBT_APMRESUMESUSPEND            = 0x0007;
            public const int PBT_APMRESUMESTANDBY            = 0x0008;
            public const int PBTF_APMRESUMEFROMFAILURE       = 0x00000001;
            public const int PBT_APMBATTERYLOW               = 0x0009;
            public const int PBT_APMPOWERSTATUSCHANGE        = 0x000A;
            public const int PBT_APMOEMEVENT                 = 0x000B;
            public const int PBT_APMRESUMEAUTOMATIC          = 0x0012;
            public const int PBT_POWERSETTINGCHANGE          = 0x8013;

            //public const int WM_DEVICECHANGE                 = 0x0219;

            //public const int WM_MDICREATE                    = 0x0220;
            //public const int WM_MDIDESTROY                   = 0x0221;
            //public const int WM_MDIACTIVATE                  = 0x0222;
            //public const int WM_MDIRESTORE                   = 0x0223;
            //public const int WM_MDINEXT                      = 0x0224;
            //public const int WM_MDIMAXIMIZE                  = 0x0225;
            //public const int WM_MDITILE                      = 0x0226;
            //public const int WM_MDICASCADE                   = 0x0227;
            //public const int WM_MDIICONARRANGE               = 0x0228;
            //public const int WM_MDIGETACTIVE                 = 0x0229;

            //public const int WM_MDISETMENU                   = 0x0230;
            //public const int WM_ENTERSIZEMOVE                = 0x0231;
            //public const int WM_EXITSIZEMOVE                 = 0x0232;
            //public const int WM_DROPFILES                    = 0x0233;
            //public const int WM_MDIREFRESHMENU               = 0x0234;

            //public const int WM_IME_SETCONTEXT               = 0x0281;
            //public const int WM_IME_NOTIFY                   = 0x0282;
            //public const int WM_IME_CONTROL                  = 0x0283;
            //public const int WM_IME_COMPOSITIONFULL          = 0x0284;
            //public const int WM_IME_SELECT                   = 0x0285;
            //public const int WM_IME_CHAR                     = 0x0286;
            //public const int WM_IME_REQUEST                  = 0x0288;
            //public const int WM_IME_KEYDOWN                  = 0x0290;
            //public const int WM_IME_KEYUP                    = 0x0291;

            //public const int WM_MOUSEHOVER                   = 0x02A1;
            //public const int WM_MOUSELEAVE                   = 0x02A3;
            //public const int WM_NCMOUSEHOVER                 = 0x02A0;
            //public const int WM_NCMOUSELEAVE                 = 0x02A2;

            //public const int WM_WTSSESSION_CHANGE            = 0x02B1;

            //public const int WM_TABLET_FIRST                 = 0x02c0;
            //public const int WM_TABLET_LAST                  = 0x02df;

            //public const int WM_CUT                          = 0x0300;
            //public const int WM_COPY                         = 0x0301;
            //public const int WM_PASTE                        = 0x0302;
            //public const int WM_CLEAR                        = 0x0303;
            //public const int WM_UNDO                         = 0x0304;
            //public const int WM_RENDERFORMAT                 = 0x0305;
            //public const int WM_RENDERALLFORMATS             = 0x0306;
            //public const int WM_DESTROYCLIPBOARD             = 0x0307;
            //public const int WM_DRAWCLIPBOARD                = 0x0308;
            //public const int WM_PAINTCLIPBOARD               = 0x0309;
            //public const int WM_VSCROLLCLIPBOARD             = 0x030A;
            //public const int WM_SIZECLIPBOARD                = 0x030B;
            //public const int WM_ASKCBFORMATNAME              = 0x030C;
            //public const int WM_CHANGECBCHAIN                = 0x030D;
            //public const int WM_HSCROLLCLIPBOARD             = 0x030E;
            //public const int WM_QUERYNEWPALETTE              = 0x030F;
            //public const int WM_PALETTEISCHANGING            = 0x0310;
            //public const int WM_PALETTECHANGED               = 0x0311;
            //public const int WM_HOTKEY                       = 0x0312;

            //public const int WM_PRINT                        = 0x0317;
            //public const int WM_PRINTCLIENT                  = 0x0318;

            //public const int WM_APPCOMMAND                   = 0x0319;

            //public const int WM_THEMECHANGED                 = 0x031A;

            //public const int WM_CLIPBOARDUPDATE              = 0x031D;

            //public const int WM_DWMCOMPOSITIONCHANGED        = 0x031E;
            //public const int WM_DWMNCRENDERINGCHANGED        = 0x031F;
            //public const int WM_DWMCOLORIZATIONCOLORCHANGED  = 0x0320;
            //public const int WM_DWMWINDOWMAXIMIZEDCHANGE     = 0x0321;

            //public const int WM_GETTITLEBARINFOEX            = 0x033F;

            //public const int WM_HANDHELDFIRST                = 0x0358;
            //public const int WM_HANDHELDLAST                 = 0x035F;

            //public const int WM_AFXFIRST                     = 0x0360;
            //public const int WM_AFXLAST                      = 0x037F;

            //public const int WM_PENWINFIRST                  = 0x0380;
            //public const int WM_PENWINLAST                   = 0x038F;

            //public const int WM_APP                          = 0x8000;

            //public const int WM_USER                         = 0x0400;

            // wParam for WM_SIZING message
            public const int WMSZ_LEFT           = 1;
            public const int WMSZ_RIGHT          = 2;
            public const int WMSZ_TOP            = 3;
            public const int WMSZ_TOPLEFT        = 4;
            public const int WMSZ_TOPRIGHT       = 5;
            public const int WMSZ_BOTTOM         = 6;
            public const int WMSZ_BOTTOMLEFT     = 7;
            public const int WMSZ_BOTTOMRIGHT    = 8;

            // WM_NCHITTEST and MOUSEHOOKSTRUCT Mouse Position Codes
            public const int HTERROR             = (-2);
            public const int HTTRANSPARENT       = (-1);
            public const int HTNOWHERE           = 0;
            public const int HTCLIENT            = 1;
            public const int HTCAPTION           = 2;
            public const int HTSYSMENU           = 3;
            public const int HTGROWBOX           = 4;
            public const int HTSIZE              = HTGROWBOX;
            public const int HTMENU              = 5;
            public const int HTHSCROLL           = 6;
            public const int HTVSCROLL           = 7;
            public const int HTMINBUTTON         = 8;
            public const int HTMAXBUTTON         = 9;
            public const int HTLEFT              = 10;
            public const int HTRIGHT             = 11;
            public const int HTTOP               = 12;
            public const int HTTOPLEFT           = 13;
            public const int HTTOPRIGHT          = 14;
            public const int HTBOTTOM            = 15;
            public const int HTBOTTOMLEFT        = 16;
            public const int HTBOTTOMRIGHT       = 17;
            public const int HTBORDER            = 18;
            public const int HTREDUCE            = HTMINBUTTON;
            public const int HTZOOM              = HTMAXBUTTON;
            public const int HTSIZEFIRST         = HTLEFT;
            public const int HTSIZELAST          = HTBOTTOMRIGHT;
            public const int HTOBJECT            = 19;
            public const int HTCLOSE             = 20;
            public const int HTHELP              = 21;

            // SendMessageTimeout values
            public const int SMTO_NORMAL             = 0x0000;
            public const int SMTO_BLOCK              = 0x0001;
            public const int SMTO_ABORTIFHUNG        = 0x0002;
            public const int SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;
            public const int SMTO_ERRORONEXIT        = 0x0020;

            // WM_MOUSEACTIVATE Return Codes
            public const int MA_ACTIVATE         = 1;
            public const int MA_ACTIVATEANDEAT   = 2;
            public const int MA_NOACTIVATE       = 3;
            public const int MA_NOACTIVATEANDEAT = 4;

            // WM_SETICON / WM_GETICON Type Codes
            public const int ICON_SMALL          = 0;
            public const int ICON_BIG            = 1;
            public const int ICON_SMALL2         = 2;


            // WM_SIZE message wParam values
            public const int SIZE_RESTORED       = 0;
            public const int SIZE_MINIMIZED      = 1;
            public const int SIZE_MAXIMIZED      = 2;
            public const int SIZE_MAXSHOW        = 3;
            public const int SIZE_MAXHIDE        = 4;

            // Obsolete constant names
            public const int SIZENORMAL          = SIZE_RESTORED;
            public const int SIZEICONIC          = SIZE_MINIMIZED;
            public const int SIZEFULLSCREEN      = SIZE_MAXIMIZED;
            public const int SIZEZOOMSHOW        = SIZE_MAXSHOW;
            public const int SIZEZOOMHIDE        = SIZE_MAXHIDE;

            // WM_NCCALCSIZE "window valid rect" return values
            public const int WVR_ALIGNTOP        = 0x0010;
            public const int WVR_ALIGNLEFT       = 0x0020;
            public const int WVR_ALIGNBOTTOM     = 0x0040;
            public const int WVR_ALIGNRIGHT      = 0x0080;
            public const int WVR_HREDRAW         = 0x0100;
            public const int WVR_VREDRAW         = 0x0200;
            public const int WVR_REDRAW          = (WVR_HREDRAW | WVR_VREDRAW);
            public const int WVR_VALIDRECTS      = 0x0400;

            // Key State Masks for Mouse Messages
            public const int MK_LBUTTON          = 0x0001;
            public const int MK_RBUTTON          = 0x0002;
            public const int MK_SHIFT            = 0x0004;
            public const int MK_CONTROL          = 0x0008;
            public const int MK_MBUTTON          = 0x0010;
            public const int MK_XBUTTON1         = 0x0020;
            public const int MK_XBUTTON2         = 0x0040;

            public const uint TME_HOVER       = 0x00000001;
            public const uint TME_LEAVE       = 0x00000002;
            public const uint TME_NONCLIENT   = 0x00000010;
            public const uint TME_QUERY       = 0x40000000;
            public const uint TME_CANCEL      = 0x80000000;

            public const uint HOVER_DEFAULT   = 0xFFFFFFFF;

            // Window Styles
            public const uint WS_OVERLAPPED       = 0x00000000;
            public const uint WS_POPUP            = 0x80000000;
            public const uint WS_CHILD            = 0x40000000;
            public const uint WS_MINIMIZE         = 0x20000000;
            public const uint WS_VISIBLE          = 0x10000000;
            public const uint WS_DISABLED         = 0x08000000;
            public const uint WS_CLIPSIBLINGS     = 0x04000000;
            public const uint WS_CLIPCHILDREN     = 0x02000000;
            public const uint WS_MAXIMIZE         = 0x01000000;
            public const uint WS_CAPTION          = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
            public const uint WS_BORDER           = 0x00800000;
            public const uint WS_DLGFRAME         = 0x00400000;
            public const uint WS_VSCROLL          = 0x00200000;
            public const uint WS_HSCROLL          = 0x00100000;
            public const uint WS_SYSMENU          = 0x00080000;
            public const uint WS_THICKFRAME       = 0x00040000;
            public const uint WS_GROUP            = 0x00020000;
            public const uint WS_TABSTOP          = 0x00010000;
            public const uint WS_MINIMIZEBOX      = 0x00020000;
            public const uint WS_MAXIMIZEBOX      = 0x00010000;

            public const uint WS_TILED            = WS_OVERLAPPED;
            public const uint WS_ICONIC           = WS_MINIMIZE;
            public const uint WS_SIZEBOX          = WS_THICKFRAME;
            public const uint WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW;

            // Common Window Styles
            public const uint WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
            public const uint WS_POPUPWINDOW      = (WS_POPUP | WS_BORDER | WS_SYSMENU);
            public const uint WS_CHILDWINDOW      = (WS_CHILD);

            // Extended Window Styles
            public const uint WS_EX_DLGMODALFRAME     = 0x00000001;
            public const uint WS_EX_NOPARENTNOTIFY    = 0x00000004;
            public const uint WS_EX_TOPMOST           = 0x00000008;
            public const uint WS_EX_ACCEPTFILES       = 0x00000010;
            public const uint WS_EX_TRANSPARENT       = 0x00000020;
            public const uint WS_EX_MDICHILD          = 0x00000040;
            public const uint WS_EX_TOOLWINDOW        = 0x00000080;
            public const uint WS_EX_WINDOWEDGE        = 0x00000100;
            public const uint WS_EX_CLIENTEDGE        = 0x00000200;
            public const uint WS_EX_CONTEXTHELP       = 0x00000400;

            public const uint WS_EX_RIGHT             = 0x00001000;
            public const uint WS_EX_LEFT              = 0x00000000;
            public const uint WS_EX_RTLREADING        = 0x00002000;
            public const uint WS_EX_LTRREADING        = 0x00000000;
            public const uint WS_EX_LEFTSCROLLBAR     = 0x00004000;
            public const uint WS_EX_RIGHTSCROLLBAR    = 0x00000000;

            public const uint WS_EX_CONTROLPARENT     = 0x00010000;
            public const uint WS_EX_STATICEDGE        = 0x00020000;
            public const uint WS_EX_APPWINDOW         = 0x00040000;

            public const uint WS_EX_OVERLAPPEDWINDOW  = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
            public const uint WS_EX_PALETTEWINDOW     = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);

            public const uint WS_EX_LAYERED           = 0x00080000;

            public const uint WS_EX_NOINHERITLAYOUT   = 0x00100000; // Disable inheritence of mirroring by children
            public const uint WS_EX_LAYOUTRTL         = 0x00400000; // Right to left mirroring

            public const uint WS_EX_COMPOSITED        = 0x02000000;
            public const uint WS_EX_NOACTIVATE        = 0x08000000;

            // Class styles
            public const int CS_VREDRAW          = 0x0001;
            public const int CS_HREDRAW          = 0x0002;
            public const int CS_DBLCLKS          = 0x0008;
            public const int CS_OWNDC            = 0x0020;
            public const int CS_CLASSDC          = 0x0040;
            public const int CS_PARENTDC         = 0x0080;
            public const int CS_NOCLOSE          = 0x0200;
            public const int CS_SAVEBITS         = 0x0800;
            public const int CS_BYTEALIGNCLIENT  = 0x1000;
            public const int CS_BYTEALIGNWINDOW  = 0x2000;
            public const int CS_GLOBALCLASS      = 0x4000;
            public const int CS_IME              = 0x00010000;
            public const int CS_DROPSHADOW       = 0x00020000;

            // WM_PRINT flags
            public const uint PRF_CHECKVISIBLE    = 0x00000001;
            public const uint PRF_NONCLIENT       = 0x00000002;
            public const uint PRF_CLIENT          = 0x00000004;
            public const uint PRF_ERASEBKGND      = 0x00000008;
            public const uint PRF_CHILDREN        = 0x00000010;
            public const uint PRF_OWNED           = 0x00000020;

            // 3D border styles
            public const int BDR_RAISEDOUTER = 0x0001;
            public const int BDR_SUNKENOUTER = 0x0002;
            public const int BDR_RAISEDINNER = 0x0004;
            public const int BDR_SUNKENINNER = 0x0008;
            public const int BDR_OUTER       = (BDR_RAISEDOUTER | BDR_SUNKENOUTER);
            public const int BDR_INNER       = (BDR_RAISEDINNER | BDR_SUNKENINNER);
            public const int BDR_RAISED      = (BDR_RAISEDOUTER | BDR_RAISEDINNER);
            public const int BDR_SUNKEN      = (BDR_SUNKENOUTER | BDR_SUNKENINNER);
            public const int EDGE_RAISED     = (BDR_RAISEDOUTER | BDR_RAISEDINNER);
            public const int EDGE_SUNKEN     = (BDR_SUNKENOUTER | BDR_SUNKENINNER);
            public const int EDGE_ETCHED     = (BDR_SUNKENOUTER | BDR_RAISEDINNER);
            public const int EDGE_BUMP       = (BDR_RAISEDOUTER | BDR_SUNKENINNER);

            // Border flags
            public const int BF_LEFT         = 0x0001;
            public const int BF_TOP          = 0x0002;
            public const int BF_RIGHT        = 0x0004;
            public const int BF_BOTTOM       = 0x0008;
            public const int BF_TOPLEFT      = (BF_TOP | BF_LEFT);
            public const int BF_TOPRIGHT     = (BF_TOP | BF_RIGHT);
            public const int BF_BOTTOMLEFT   = (BF_BOTTOM | BF_LEFT);
            public const int BF_BOTTOMRIGHT  = (BF_BOTTOM | BF_RIGHT);
            public const int BF_RECT         = (BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM);
            public const int BF_DIAGONAL     = 0x0010;

            // For diagonal lines, the BF_RECT flags specify the end point of the
            // vector bounded by the rectangle parameter.
            public const int BF_DIAGONAL_ENDTOPRIGHT     = (BF_DIAGONAL | BF_TOP | BF_RIGHT);
            public const int BF_DIAGONAL_ENDTOPLEFT      = (BF_DIAGONAL | BF_TOP | BF_LEFT);
            public const int BF_DIAGONAL_ENDBOTTOMLEFT   = (BF_DIAGONAL | BF_BOTTOM | BF_LEFT);
            public const int BF_DIAGONAL_ENDBOTTOMRIGHT  = (BF_DIAGONAL | BF_BOTTOM | BF_RIGHT);

            public const int BF_MIDDLE       = 0x0800;  // Fill in the middle
            public const int BF_SOFT         = 0x1000;  // For softer buttons
            public const int BF_ADJUST       = 0x2000;  // Calculate the space left over
            public const int BF_FLAT         = 0x4000;  // For flat rather than 3D borders
            public const int BF_MONO         = 0x8000;  // For monochrome borders

            // flags for DrawFrameControl

            public const int DFC_CAPTION             = 1;
            public const int DFC_MENU                = 2;
            public const int DFC_SCROLL              = 3;
            public const int DFC_BUTTON              = 4;
            public const int DFC_POPUPMENU           = 5;

            public const int DFCS_CAPTIONCLOSE       = 0x0000;
            public const int DFCS_CAPTIONMIN         = 0x0001;
            public const int DFCS_CAPTIONMAX         = 0x0002;
            public const int DFCS_CAPTIONRESTORE     = 0x0003;
            public const int DFCS_CAPTIONHELP        = 0x0004;

            public const int DFCS_MENUARROW           = 0x0000;
            public const int DFCS_MENUCHECK           = 0x0001;
            public const int DFCS_MENUBULLET          = 0x0002;
            public const int DFCS_MENUARROWRIGHT      = 0x0004;
            public const int DFCS_SCROLLUP            = 0x0000;
            public const int DFCS_SCROLLDOWN          = 0x0001;
            public const int DFCS_SCROLLLEFT          = 0x0002;
            public const int DFCS_SCROLLRIGHT         = 0x0003;
            public const int DFCS_SCROLLCOMBOBOX      = 0x0005;
            public const int DFCS_SCROLLSIZEGRIP      = 0x0008;
            public const int DFCS_SCROLLSIZEGRIPRIGHT = 0x0010;

            public const int DFCS_BUTTONCHECK        = 0x0000;
            public const int DFCS_BUTTONRADIOIMAGE   = 0x0001;
            public const int DFCS_BUTTONRADIOMASK    = 0x0002;
            public const int DFCS_BUTTONRADIO        = 0x0004;
            public const int DFCS_BUTTON3STATE       = 0x0008;
            public const int DFCS_BUTTONPUSH         = 0x0010;

            public const int DFCS_INACTIVE           = 0x0100;
            public const int DFCS_PUSHED             = 0x0200;
            public const int DFCS_CHECKED            = 0x0400;

            public const int DFCS_TRANSPARENT        = 0x0800;
            public const int DFCS_HOT                = 0x1000;

            public const int DFCS_ADJUSTRECT         = 0x2000;
            public const int DFCS_FLAT               = 0x4000;
            public const int DFCS_MONO               = 0x8000;

            // flags for DrawCaption
            public const int DC_ACTIVE           = 0x0001;
            public const int DC_SMALLCAP         = 0x0002;
            public const int DC_ICON             = 0x0004;
            public const int DC_TEXT             = 0x0008;
            public const int DC_INBUTTON         = 0x0010;
            public const int DC_GRADIENT         = 0x0020;
            public const int DC_BUTTONS          = 0x1000;

            public const int IDANI_OPEN          = 1;
            public const int IDANI_CAPTION       = 3;

            // Predefined Clipboard Formats
            public const int CF_TEXT             = 1;
            public const int CF_BITMAP           = 2;
            public const int CF_METAFILEPICT     = 3;
            public const int CF_SYLK             = 4;
            public const int CF_DIF              = 5;
            public const int CF_TIFF             = 6;
            public const int CF_OEMTEXT          = 7;
            public const int CF_DIB              = 8;
            public const int CF_PALETTE          = 9;
            public const int CF_PENDATA          = 10;
            public const int CF_RIFF             = 11;
            public const int CF_WAVE             = 12;
            public const int CF_UNICODETEXT      = 13;
            public const int CF_ENHMETAFILE      = 14;
            public const int CF_HDROP            = 15;
            public const int CF_LOCALE           = 16;
            public const int CF_DIBV5            = 17;
            public const int CF_MAX              = 18; // WINVER >= 0x0500
          //public const int CF_MAX              = 17; // WINVER >= 0x0400 && WINVER < 0x0500
          //public const int CF_MAX              = 15; // WINVER <  0x0400
            public const int CF_OWNERDISPLAY     = 0x0080;
            public const int CF_DSPTEXT          = 0x0081;
            public const int CF_DSPBITMAP        = 0x0082;
            public const int CF_DSPMETAFILEPICT  = 0x0083;
            public const int CF_DSPENHMETAFILE   = 0x008E;

            // "Private" formats don't get GlobalFree()'d
            public const int CF_PRIVATEFIRST     = 0x0200;
            public const int CF_PRIVATELAST      = 0x02FF;

            // "GDIOBJ" formats do get DeleteObject()'d
            public const int CF_GDIOBJFIRST      = 0x0300;
            public const int CF_GDIOBJLAST       = 0x03FF;

            // Defines for the fVirt field of the Accelerator table structure.
            //public const int FVIRTKEY  = TRUE;          /* Assumed to be == TRUE */
            public const int FNOINVERT = 0x02;
            public const int FSHIFT    = 0x04;
            public const int FCONTROL  = 0x08;
            public const int FALT      = 0x10;

            public const int WPF_SETMINPOSITION          = 0x0001;
            public const int WPF_RESTORETOMAXIMIZED      = 0x0002;
            public const int WPF_ASYNCWINDOWPLACEMENT    = 0x0004;


            // Owner draw control types
            public const int ODT_MENU        = 1;
            public const int ODT_LISTBOX     = 2;
            public const int ODT_COMBOBOX    = 3;
            public const int ODT_BUTTON      = 4;
            public const int ODT_STATIC      = 5;

            // Owner draw actions
            public const int ODA_DRAWENTIRE  = 0x0001;
            public const int ODA_SELECT      = 0x0002;
            public const int ODA_FOCUS       = 0x0004;

            // Owner draw state
            public const int ODS_SELECTED        = 0x0001;
            public const int ODS_GRAYED          = 0x0002;
            public const int ODS_DISABLED        = 0x0004;
            public const int ODS_CHECKED         = 0x0008;
            public const int ODS_FOCUS           = 0x0010;
            public const int ODS_DEFAULT         = 0x0020;
            public const int ODS_COMBOBOXEDIT    = 0x1000;
            public const int ODS_HOTLIGHT        = 0x0040;
            public const int ODS_INACTIVE        = 0x0080;
            public const int ODS_NOACCEL         = 0x0100;
            public const int ODS_NOFOCUSRECT     = 0x0200;

            // PeekMessage() Options
            public const int PM_NOREMOVE         = 0x0000;
            public const int PM_REMOVE           = 0x0001;
            public const int PM_NOYIELD          = 0x0002;
            public const int PM_QS_INPUT         = (QS_INPUT << 16);
            public const int PM_QS_POSTMESSAGE   = ((QS_POSTMESSAGE | QS_HOTKEY | QS_TIMER) << 16);
            public const int PM_QS_PAINT         = (QS_PAINT << 16);
            public const int PM_QS_SENDMESSAGE   = (QS_SENDMESSAGE << 16);

            public const int MOD_ALT         = 0x0001;
            public const int MOD_CONTROL     = 0x0002;
            public const int MOD_SHIFT       = 0x0004;
            public const int MOD_WIN         = 0x0008;

            public const int IDHOT_SNAPWINDOW        = (-1);    /* SHIFT-PRINTSCRN  */
            public const int IDHOT_SNAPDESKTOP       = (-2);    /* PRINTSCRN        */

            public const uint ENDSESSION_LOGOFF    = 0x80000000;
            public const uint ENDSESSION_CRITICAL  = 0x40000000;
            public const uint ENDSESSION_CLOSEAPP  = 0x00000001;

            public const int EWX_LOGOFF          = 0;
            public const int EWX_SHUTDOWN        = 0x00000001;
            public const int EWX_REBOOT          = 0x00000002;
            public const int EWX_FORCE           = 0x00000004;
            public const int EWX_POWEROFF        = 0x00000008;
            public const int EWX_FORCEIFHUNG     = 0x00000010;
            public const int EWX_QUICKRESOLVE    = 0x00000020;
            public const int EWX_RESTARTAPPS     = 0x00000040;

            //Broadcast Special Message Recipient list
            public const int BSM_ALLCOMPONENTS       = 0x00000000;
            public const int BSM_VXDS                = 0x00000001;
            public const int BSM_NETDRIVER           = 0x00000002;
            public const int BSM_INSTALLABLEDRIVERS  = 0x00000004;
            public const int BSM_APPLICATIONS        = 0x00000008;
            public const int BSM_ALLDESKTOPS         = 0x00000010;

            //Broadcast Special Message Flags
            public const int BSF_QUERY               = 0x00000001;
            public const int BSF_IGNORECURRENTTASK   = 0x00000002;
            public const int BSF_FLUSHDISK           = 0x00000004;
            public const int BSF_NOHANG              = 0x00000008;
            public const int BSF_POSTMESSAGE         = 0x00000010;
            public const int BSF_FORCEIFHUNG         = 0x00000020;
            public const int BSF_NOTIMEOUTIFNOTHUNG  = 0x00000040;
            public const int BSF_ALLOWSFW            = 0x00000080;
            public const int BSF_SENDNOTIFYMESSAGE   = 0x00000100;
            public const int BSF_RETURNHDESK         = 0x00000200;
            public const int BSF_LUID                = 0x00000400;

            public const int BROADCAST_QUERY_DENY    = 0x424D5144;  // Return this value to deny a query.

            // RegisterDeviceNotification

            public const int DEVICE_NOTIFY_WINDOW_HANDLE          = 0x00000000;
            public const int DEVICE_NOTIFY_SERVICE_HANDLE         = 0x00000001;
            public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES  = 0x00000004;

            // Special HWND value for use with PostMessage() and SendMessage()
            public IntPtr HWND_BROADCAST   = (IntPtr)0xffff;
            public IntPtr HWND_MESSAGE     = (IntPtr)(-3);


            // InSendMessageEx return value
            public const int ISMEX_NOSEND      = 0x00000000;
            public const int ISMEX_SEND        = 0x00000001;
            public const int ISMEX_NOTIFY      = 0x00000002;
            public const int ISMEX_CALLBACK    = 0x00000004;
            public const int ISMEX_REPLIED     = 0x00000008;

            public const uint CW_USEDEFAULT     = 0x80000000;

            // Special value for CreateWindow, et al.
            public IntPtr HWND_DESKTOP         = (IntPtr)0;

            public const int PW_CLIENTONLY           = 0x00000001;
            public const int LWA_COLORKEY            = 0x00000001;
            public const int LWA_ALPHA               = 0x00000002;
            public const int ULW_COLORKEY            = 0x00000001;
            public const int ULW_ALPHA               = 0x00000002;
            public const int ULW_OPAQUE              = 0x00000004;
            public const int ULW_EX_NORESIZE         = 0x00000008;

            public const int FLASHW_STOP         = 0;
            public const int FLASHW_CAPTION      = 0x00000001;
            public const int FLASHW_TRAY         = 0x00000002;
            public const int FLASHW_ALL          = (FLASHW_CAPTION | FLASHW_TRAY);
            public const int FLASHW_TIMER        = 0x00000004;
            public const int FLASHW_TIMERNOFG    = 0x0000000C;

            // SetWindowPos Flags
            public const int SWP_NOSIZE          = 0x0001;
            public const int SWP_NOMOVE          = 0x0002;
            public const int SWP_NOZORDER        = 0x0004;
            public const int SWP_NOREDRAW        = 0x0008;
            public const int SWP_NOACTIVATE      = 0x0010;
            public const int SWP_FRAMECHANGED    = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
            public const int SWP_SHOWWINDOW      = 0x0040;
            public const int SWP_HIDEWINDOW      = 0x0080;
            public const int SWP_NOCOPYBITS      = 0x0100;
            public const int SWP_NOOWNERZORDER   = 0x0200;  /* Don't do owner Z ordering */
            public const int SWP_NOSENDCHANGING  = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */
            public const int SWP_DRAWFRAME       = SWP_FRAMECHANGED;
            public const int SWP_NOREPOSITION    = SWP_NOOWNERZORDER;
            public const int SWP_DEFERERASE      = 0x2000;
            public const int SWP_ASYNCWINDOWPOS  = 0x4000;

            public IntPtr HWND_TOP        = (IntPtr)0;
            public IntPtr HWND_BOTTOM     = (IntPtr)1;
            public IntPtr HWND_TOPMOST    = (IntPtr)(-1);
            public IntPtr HWND_NOTOPMOST  = (IntPtr)(-2);


            //#include <pshpack2.h>


            //#include <poppack.h> /* Resume normal packing */

            // Window extra byted needed for private dialog classes.
            //#ifndef _MAC
            //public const int DLGWINDOWEXTRA = 30;
            //#else
            //public const int DLGWINDOWEXTRA = 48;
            //#endif

            public const int KEYEVENTF_EXTENDEDKEY       = 0x0001;
            public const int KEYEVENTF_KEYUP             = 0x0002;
            public const int KEYEVENTF_UNICODE           = 0x0004;
            public const int KEYEVENTF_SCANCODE          = 0x0008;

            public const int MOUSEEVENTF_MOVE            = 0x0001;  /* mouse move */
            public const int MOUSEEVENTF_LEFTDOWN        = 0x0002;  /* left button down */
            public const int MOUSEEVENTF_LEFTUP          = 0x0004;  /* left button up */
            public const int MOUSEEVENTF_RIGHTDOWN       = 0x0008;  /* right button down */
            public const int MOUSEEVENTF_RIGHTUP         = 0x0010;  /* right button up */
            public const int MOUSEEVENTF_MIDDLEDOWN      = 0x0020;  /* middle button down */
            public const int MOUSEEVENTF_MIDDLEUP        = 0x0040;  /* middle button up */
            public const int MOUSEEVENTF_XDOWN           = 0x0080;  /* x button down */
            public const int MOUSEEVENTF_XUP             = 0x0100;  /* x button down */
            public const int MOUSEEVENTF_WHEEL           = 0x0800;  /* wheel button rolled */
            public const int MOUSEEVENTF_HWHEEL          = 0x01000; /* hwheel button rolled */
            public const int MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;  /* do not coalesce mouse moves */
            public const int MOUSEEVENTF_VIRTUALDESK     = 0x4000;  /* map to entire virtual desktop */
            public const int MOUSEEVENTF_ABSOLUTE        = 0x8000;  /* absolute move */

            public const int INPUT_MOUSE     = 0;
            public const int INPUT_KEYBOARD  = 1;
            public const int INPUT_HARDWARE  = 2;

            public const int MAPVK_VK_TO_VSC     = (0);
            public const int MAPVK_VSC_TO_VK     = (1);
            public const int MAPVK_VK_TO_CHAR    = (2);
            public const int MAPVK_VSC_TO_VK_EX  = (3);
            public const int MAPVK_VK_TO_VSC_EX  = (4);

            public const int MWMO_WAITALL        = 0x0001;
            public const int MWMO_ALERTABLE      = 0x0002;
            public const int MWMO_INPUTAVAILABLE = 0x0004;

            /*
             * Queue status flags for GetQueueStatus() and MsgWaitForMultipleObjects()
             */
            public const int QS_KEY              = 0x0001;
            public const int QS_MOUSEMOVE        = 0x0002;
            public const int QS_MOUSEBUTTON      = 0x0004;
            public const int QS_POSTMESSAGE      = 0x0008;
            public const int QS_TIMER            = 0x0010;
            public const int QS_PAINT            = 0x0020;
            public const int QS_SENDMESSAGE      = 0x0040;
            public const int QS_HOTKEY           = 0x0080;
            public const int QS_ALLPOSTMESSAGE   = 0x0100;
            public const int QS_RAWINPUT         = 0x0400;

            public const int QS_MOUSE           = (QS_MOUSEMOVE     | QS_MOUSEBUTTON);

            public const int QS_INPUT           = (QS_MOUSE         | QS_KEY           | QS_RAWINPUT);
            //public const int QS_INPUT           = (QS_MOUSE         | QS_KEY);  // #if (_WIN32_WINNT < 0x0501)

            public const int QS_ALLEVENTS       = (QS_INPUT         | QS_POSTMESSAGE   | QS_TIMER         | QS_PAINT         | QS_HOTKEY);

            public const int QS_ALLINPUT        = (QS_INPUT         | QS_POSTMESSAGE   | QS_TIMER         | QS_PAINT         | QS_HOTKEY        | QS_SENDMESSAGE);

            public const int USER_TIMER_MAXIMUM  = 0x7FFFFFFF;
            public const int USER_TIMER_MINIMUM  = 0x0000000A;

            /*
             * GetSystemMetrics() codes
             */
            public const int SM_CXSCREEN             = 0;
            public const int SM_CYSCREEN             = 1;
            public const int SM_CXVSCROLL            = 2;
            public const int SM_CYHSCROLL            = 3;
            public const int SM_CYCAPTION            = 4;
            public const int SM_CXBORDER             = 5;
            public const int SM_CYBORDER             = 6;
            public const int SM_CXDLGFRAME           = 7;
            public const int SM_CYDLGFRAME           = 8;
            public const int SM_CYVTHUMB             = 9;
            public const int SM_CXHTHUMB             = 10;
            public const int SM_CXICON               = 11;
            public const int SM_CYICON               = 12;
            public const int SM_CXCURSOR             = 13;
            public const int SM_CYCURSOR             = 14;
            public const int SM_CYMENU               = 15;
            public const int SM_CXFULLSCREEN         = 16;
            public const int SM_CYFULLSCREEN         = 17;
            public const int SM_CYKANJIWINDOW        = 18;
            public const int SM_MOUSEPRESENT         = 19;
            public const int SM_CYVSCROLL            = 20;
            public const int SM_CXHSCROLL            = 21;
            public const int SM_DEBUG                = 22;
            public const int SM_SWAPBUTTON           = 23;
            public const int SM_RESERVED1            = 24;
            public const int SM_RESERVED2            = 25;
            public const int SM_RESERVED3            = 26;
            public const int SM_RESERVED4            = 27;
            public const int SM_CXMIN                = 28;
            public const int SM_CYMIN                = 29;
            public const int SM_CXSIZE               = 30;
            public const int SM_CYSIZE               = 31;
            public const int SM_CXFRAME              = 32;
            public const int SM_CYFRAME              = 33;
            public const int SM_CXMINTRACK           = 34;
            public const int SM_CYMINTRACK           = 35;
            public const int SM_CXDOUBLECLK          = 36;
            public const int SM_CYDOUBLECLK          = 37;
            public const int SM_CXICONSPACING        = 38;
            public const int SM_CYICONSPACING        = 39;
            public const int SM_MENUDROPALIGNMENT    = 40;
            public const int SM_PENWINDOWS           = 41;
            public const int SM_DBCSENABLED          = 42;
            public const int SM_CMOUSEBUTTONS        = 43;

            public const int SM_CXFIXEDFRAME           = SM_CXDLGFRAME;  /* ;win40 name change */
            public const int SM_CYFIXEDFRAME           = SM_CYDLGFRAME;  /* ;win40 name change */
            public const int SM_CXSIZEFRAME            = SM_CXFRAME;     /* ;win40 name change */
            public const int SM_CYSIZEFRAME            = SM_CYFRAME;     /* ;win40 name change */

            public const int SM_SECURE               = 44;
            public const int SM_CXEDGE               = 45;
            public const int SM_CYEDGE               = 46;
            public const int SM_CXMINSPACING         = 47;
            public const int SM_CYMINSPACING         = 48;
            public const int SM_CXSMICON             = 49;
            public const int SM_CYSMICON             = 50;
            public const int SM_CYSMCAPTION          = 51;
            public const int SM_CXSMSIZE             = 52;
            public const int SM_CYSMSIZE             = 53;
            public const int SM_CXMENUSIZE           = 54;
            public const int SM_CYMENUSIZE           = 55;
            public const int SM_ARRANGE              = 56;
            public const int SM_CXMINIMIZED          = 57;
            public const int SM_CYMINIMIZED          = 58;
            public const int SM_CXMAXTRACK           = 59;
            public const int SM_CYMAXTRACK           = 60;
            public const int SM_CXMAXIMIZED          = 61;
            public const int SM_CYMAXIMIZED          = 62;
            public const int SM_NETWORK              = 63;
            public const int SM_CLEANBOOT            = 67;
            public const int SM_CXDRAG               = 68;
            public const int SM_CYDRAG               = 69;
            public const int SM_SHOWSOUNDS           = 70;
            public const int SM_CXMENUCHECK          = 71;   /* Use instead of GetMenuCheckMarkDimensions()! */
            public const int SM_CYMENUCHECK          = 72;
            public const int SM_SLOWMACHINE          = 73;
            public const int SM_MIDEASTENABLED       = 74;

            public const int SM_MOUSEWHEELPRESENT    = 75;
            public const int SM_XVIRTUALSCREEN       = 76;
            public const int SM_YVIRTUALSCREEN       = 77;
            public const int SM_CXVIRTUALSCREEN      = 78;
            public const int SM_CYVIRTUALSCREEN      = 79;
            public const int SM_CMONITORS            = 80;
            public const int SM_SAMEDISPLAYFORMAT    = 81;
            public const int SM_IMMENABLED           = 82;
            public const int SM_CXFOCUSBORDER        = 83;
            public const int SM_CYFOCUSBORDER        = 84;

            public const int SM_TABLETPC             = 86;
            public const int SM_MEDIACENTER          = 87;
            public const int SM_STARTER              = 88;
            public const int SM_SERVERR2             = 89;

            public const int SM_MOUSEHORIZONTALWHEELPRESENT    = 91;
            public const int SM_CXPADDEDBORDER       = 92;

          //public const int SM_CMETRICS             = 76; // WINVER <  0x500 && _WIN32_WINNT < 0x0400
          //public const int SM_CMETRICS             = 83; // WINVER == 0x500
            public const int SM_CMETRICS             = 90; // WINVER == 0x501
          //public const int SM_CMETRICS             = 93; // WINVER >= 0x502

            public const int SM_REMOTESESSION        = 0x1000;
            public const int SM_SHUTTINGDOWN         = 0x2000;
            public const int SM_REMOTECONTROL        = 0x2001;
            public const int SM_CARETBLINKINGENABLED = 0x2002;

            //#if(WINVER >= 0x0400)
            /* return codes for WM_MENUCHAR */
            public const int MNC_IGNORE  = 0;
            public const int MNC_CLOSE   = 1;
            public const int MNC_EXECUTE = 2;
            public const int MNC_SELECT  = 3;

            public const uint MNS_NOCHECK         = 0x80000000;
            public const uint MNS_MODELESS        = 0x40000000;
            public const uint MNS_DRAGDROP        = 0x20000000;
            public const uint MNS_AUTODISMISS     = 0x10000000;
            public const uint MNS_NOTIFYBYPOS     = 0x08000000;
            public const uint MNS_CHECKORBMP      = 0x04000000;

            public const uint MIM_MAXHEIGHT               = 0x00000001;
            public const uint MIM_BACKGROUND              = 0x00000002;
            public const uint MIM_HELPID                  = 0x00000004;
            public const uint MIM_MENUDATA                = 0x00000008;
            public const uint MIM_STYLE                   = 0x00000010;
            public const uint MIM_APPLYTOSUBMENUS         = 0x80000000;

            /*
             * WM_MENUDRAG return values.
             */
            public const int MND_CONTINUE       = 0;
            public const int MND_ENDMENU        = 1;

            /*
             * MENUGETOBJECTINFO dwFlags values
             */
            public const int MNGOF_TOPGAP         = 0x00000001;
            public const int MNGOF_BOTTOMGAP      = 0x00000002;

            /*
             * WM_MENUGETOBJECT return values
             */
            public const int MNGO_NOINTERFACE     = 0x00000000;
            public const int MNGO_NOERROR         = 0x00000001;

            public const int MIIM_STATE       = 0x00000001;
            public const int MIIM_ID          = 0x00000002;
            public const int MIIM_SUBMENU     = 0x00000004;
            public const int MIIM_CHECKMARKS  = 0x00000008;
            public const int MIIM_TYPE        = 0x00000010;
            public const int MIIM_DATA        = 0x00000020;

            public const int MIIM_STRING      = 0x00000040;
            public const int MIIM_BITMAP      = 0x00000080;
            public const int MIIM_FTYPE       = 0x00000100;

            public IntPtr HBMMENU_CALLBACK            = (IntPtr)(-1);
            public IntPtr HBMMENU_SYSTEM              = (IntPtr)  1;
            public IntPtr HBMMENU_MBAR_RESTORE        = (IntPtr)  2;
            public IntPtr HBMMENU_MBAR_MINIMIZE       = (IntPtr)  3;
            public IntPtr HBMMENU_MBAR_CLOSE          = (IntPtr)  5;
            public IntPtr HBMMENU_MBAR_CLOSE_D        = (IntPtr)  6;
            public IntPtr HBMMENU_MBAR_MINIMIZE_D     = (IntPtr)  7;
            public IntPtr HBMMENU_POPUP_CLOSE         = (IntPtr)  8;
            public IntPtr HBMMENU_POPUP_RESTORE       = (IntPtr)  9;
            public IntPtr HBMMENU_POPUP_MAXIMIZE      = (IntPtr) 10;
            public IntPtr HBMMENU_POPUP_MINIMIZE      = (IntPtr) 11;

            public const int GMDI_USEDISABLED    = 0x0001;
            public const int GMDI_GOINTOPOPUPS   = 0x0002;

            /*
             * Flags for TrackPopupMenu
             */
            public const int TPM_LEFTBUTTON      = 0x0000;
            public const int TPM_RIGHTBUTTON     = 0x0002;
            public const int TPM_LEFTALIGN       = 0x0000;
            public const int TPM_CENTERALIGN     = 0x0004;
            public const int TPM_RIGHTALIGN      = 0x0008;
            public const int TPM_TOPALIGN        = 0x0000;
            public const int TPM_VCENTERALIGN    = 0x0010;
            public const int TPM_BOTTOMALIGN     = 0x0020;
            public const int TPM_HORIZONTAL      = 0x0000;     /* Horz alignment matters more */
            public const int TPM_VERTICAL        = 0x0040;     /* Vert alignment matters more */
            public const int TPM_NONOTIFY        = 0x0080;     /* Don't send any notification msgs */
            public const int TPM_RETURNCMD       = 0x0100;
            public const int TPM_RECURSE         = 0x0001;
            public const int TPM_HORPOSANIMATION = 0x0400;
            public const int TPM_HORNEGANIMATION = 0x0800;
            public const int TPM_VERPOSANIMATION = 0x1000;
            public const int TPM_VERNEGANIMATION = 0x2000;
            public const int TPM_NOANIMATION     = 0x4000;
            public const int TPM_LAYOUTRTL       = 0x8000;


            public const int DOF_EXECUTABLE      = 0x8001;      // wFmt flags
            public const int DOF_DOCUMENT        = 0x8002;
            public const int DOF_DIRECTORY       = 0x8003;
            public const int DOF_MULTIPLE        = 0x8004;
            public const int DOF_PROGMAN         = 0x0001;
            public const int DOF_SHELLDATA       = 0x0002;

            public const int DO_DROPFILE         = 0x454C4946;
            public const int DO_PRINTFILE        = 0x544E5250;

            /*
             * DrawText() Format Flags
             */
            public const int DT_TOP                      = 0x00000000;
            public const int DT_LEFT                     = 0x00000000;
            public const int DT_CENTER                   = 0x00000001;
            public const int DT_RIGHT                    = 0x00000002;
            public const int DT_VCENTER                  = 0x00000004;
            public const int DT_BOTTOM                   = 0x00000008;
            public const int DT_WORDBREAK                = 0x00000010;
            public const int DT_SINGLELINE               = 0x00000020;
            public const int DT_EXPANDTABS               = 0x00000040;
            public const int DT_TABSTOP                  = 0x00000080;
            public const int DT_NOCLIP                   = 0x00000100;
            public const int DT_EXTERNALLEADING          = 0x00000200;
            public const int DT_CALCRECT                 = 0x00000400;
            public const int DT_NOPREFIX                 = 0x00000800;
            public const int DT_INTERNAL                 = 0x00001000;

            public const int DT_EDITCONTROL              = 0x00002000;
            public const int DT_PATH_ELLIPSIS            = 0x00004000;
            public const int DT_END_ELLIPSIS             = 0x00008000;
            public const int DT_MODIFYSTRING             = 0x00010000;
            public const int DT_RTLREADING               = 0x00020000;
            public const int DT_WORD_ELLIPSIS            = 0x00040000;
            public const int DT_NOFULLWIDTHCHARBREAK     = 0x00080000;
            public const int DT_HIDEPREFIX               = 0x00100000;
            public const int DT_PREFIXONLY               = 0x00200000;

            /* Monolithic state-drawing routine */
            /* Image type */
            public const int DST_COMPLEX     = 0x0000;
            public const int DST_TEXT        = 0x0001;
            public const int DST_PREFIXTEXT  = 0x0002;
            public const int DST_ICON        = 0x0003;
            public const int DST_BITMAP      = 0x0004;

            /* State type */
            public const int DSS_NORMAL      = 0x0000;
            public const int DSS_UNION       = 0x0010;  /* Gray string appearance */
            public const int DSS_DISABLED    = 0x0020;
            public const int DSS_MONO        = 0x0080;
            public const int DSS_HIDEPREFIX  = 0x0200;
            public const int DSS_PREFIXONLY  = 0x0400;
            public const int DSS_RIGHT       = 0x8000;

            public const int ASFW_ANY    = -1;

            public const int LSFW_LOCK       = 1;
            public const int LSFW_UNLOCK     = 2;

            /*
             * GetDCEx() flags
             */
            public const int DCX_WINDOW           = 0x00000001;
            public const int DCX_CACHE            = 0x00000002;
            public const int DCX_NORESETATTRS     = 0x00000004;
            public const int DCX_CLIPCHILDREN     = 0x00000008;
            public const int DCX_CLIPSIBLINGS     = 0x00000010;
            public const int DCX_PARENTCLIP       = 0x00000020;
            public const int DCX_EXCLUDERGN       = 0x00000040;
            public const int DCX_INTERSECTRGN     = 0x00000080;
            public const int DCX_EXCLUDEUPDATE    = 0x00000100;
            public const int DCX_INTERSECTUPDATE  = 0x00000200;
            public const int DCX_LOCKWINDOWUPDATE = 0x00000400;

            public const int DCX_VALIDATE         = 0x00200000;

            /*
             * RedrawWindow() flags
             */
            public const int RDW_INVALIDATE          = 0x0001;
            public const int RDW_INTERNALPAINT       = 0x0002;
            public const int RDW_ERASE               = 0x0004;

            public const int RDW_VALIDATE            = 0x0008;
            public const int RDW_NOINTERNALPAINT     = 0x0010;
            public const int RDW_NOERASE             = 0x0020;

            public const int RDW_NOCHILDREN          = 0x0040;
            public const int RDW_ALLCHILDREN         = 0x0080;

            public const int RDW_UPDATENOW           = 0x0100;
            public const int RDW_ERASENOW            = 0x0200;

            public const int RDW_FRAME               = 0x0400;
            public const int RDW_NOFRAME             = 0x0800;

            public const int SW_SCROLLCHILDREN   = 0x0001;  /* Scroll children within *lprcScroll. */
            public const int SW_INVALIDATE       = 0x0002;  /* Invalidate after scrolling */
            public const int SW_ERASE            = 0x0004;  /* If SW_INVALIDATE, don't send WM_ERASEBACKGROUND */
            public const int SW_SMOOTHSCROLL     = 0x0010;  /* Use smooth scrolling */

            /*
             * EnableScrollBar() flags
             */
            public const int ESB_ENABLE_BOTH     = 0x0000;
            public const int ESB_DISABLE_BOTH    = 0x0003;

            public const int ESB_DISABLE_LEFT    = 0x0001;
            public const int ESB_DISABLE_RIGHT   = 0x0002;

            public const int ESB_DISABLE_UP      = 0x0001;
            public const int ESB_DISABLE_DOWN    = 0x0002;

            public const int ESB_DISABLE_LTUP    = ESB_DISABLE_LEFT;
            public const int ESB_DISABLE_RTDN    = ESB_DISABLE_RIGHT;


            public const int HELPINFO_WINDOW    = 0x0001;
            public const int HELPINFO_MENUITEM  = 0x0002;

            /*
             * MessageBox() Flags
             */
            public const int MB_OK                       = 0x00000000;
            public const int MB_OKCANCEL                 = 0x00000001;
            public const int MB_ABORTRETRYIGNORE         = 0x00000002;
            public const int MB_YESNOCANCEL              = 0x00000003;
            public const int MB_YESNO                    = 0x00000004;
            public const int MB_RETRYCANCEL              = 0x00000005;
            public const int MB_CANCELTRYCONTINUE        = 0x00000006;
            public const int MB_ICONHAND                 = 0x00000010;
            public const int MB_ICONQUESTION             = 0x00000020;
            public const int MB_ICONEXCLAMATION          = 0x00000030;
            public const int MB_ICONASTERISK             = 0x00000040;
            public const int MB_USERICON                 = 0x00000080;
            public const int MB_ICONWARNING              = MB_ICONEXCLAMATION;
            public const int MB_ICONERROR                = MB_ICONHAND;
            public const int MB_ICONINFORMATION          = MB_ICONASTERISK;
            public const int MB_ICONSTOP                 = MB_ICONHAND;
            public const int MB_DEFBUTTON1               = 0x00000000;
            public const int MB_DEFBUTTON2               = 0x00000100;
            public const int MB_DEFBUTTON3               = 0x00000200;
            public const int MB_DEFBUTTON4               = 0x00000300;
            public const int MB_APPLMODAL                = 0x00000000;
            public const int MB_SYSTEMMODAL              = 0x00001000;
            public const int MB_TASKMODAL                = 0x00002000;
            public const int MB_HELP                     = 0x00004000; // Help Button
            public const int MB_NOFOCUS                  = 0x00008000;
            public const int MB_SETFOREGROUND            = 0x00010000;
            public const int MB_DEFAULT_DESKTOP_ONLY     = 0x00020000;
            public const int MB_TOPMOST                  = 0x00040000;
            public const int MB_RIGHT                    = 0x00080000;
            public const int MB_RTLREADING               = 0x00100000;
            public const int MB_SERVICE_NOTIFICATION          = 0x00200000; //_WIN32_WINNT >= 0x0400
          //public const int MB_SERVICE_NOTIFICATION          = 0x00040000; //_WIN32_WINNT <  0x0400
            public const int MB_SERVICE_NOTIFICATION_NT3X     = 0x00040000;
            public const int MB_TYPEMASK                 = 0x0000000F;
            public const int MB_ICONMASK                 = 0x000000F0;
            public const int MB_DEFMASK                  = 0x00000F00;
            public const int MB_MODEMASK                 = 0x00003000;
            public const int MB_MISCMASK                 = 0x0000C000;

            public const int CWP_ALL             = 0x0000;
            public const int CWP_SKIPINVISIBLE   = 0x0001;
            public const int CWP_SKIPDISABLED    = 0x0002;
            public const int CWP_SKIPTRANSPARENT = 0x0004;


            /*
             * Color Types
             */
            public const int CTLCOLOR_MSGBOX         = 0;
            public const int CTLCOLOR_EDIT           = 1;
            public const int CTLCOLOR_LISTBOX        = 2;
            public const int CTLCOLOR_BTN            = 3;
            public const int CTLCOLOR_DLG            = 4;
            public const int CTLCOLOR_SCROLLBAR      = 5;
            public const int CTLCOLOR_STATIC         = 6;
            public const int CTLCOLOR_MAX            = 7;

            public const int COLOR_SCROLLBAR         = 0;
            public const int COLOR_BACKGROUND        = 1;
            public const int COLOR_ACTIVECAPTION     = 2;
            public const int COLOR_INACTIVECAPTION   = 3;
            public const int COLOR_MENU              = 4;
            public const int COLOR_WINDOW            = 5;
            public const int COLOR_WINDOWFRAME       = 6;
            public const int COLOR_MENUTEXT          = 7;
            public const int COLOR_WINDOWTEXT        = 8;
            public const int COLOR_CAPTIONTEXT       = 9;
            public const int COLOR_ACTIVEBORDER      = 10;
            public const int COLOR_INACTIVEBORDER    = 11;
            public const int COLOR_APPWORKSPACE      = 12;
            public const int COLOR_HIGHLIGHT         = 13;
            public const int COLOR_HIGHLIGHTTEXT     = 14;
            public const int COLOR_BTNFACE           = 15;
            public const int COLOR_BTNSHADOW         = 16;
            public const int COLOR_GRAYTEXT          = 17;
            public const int COLOR_BTNTEXT           = 18;
            public const int COLOR_INACTIVECAPTIONTEXT = 19;
            public const int COLOR_BTNHIGHLIGHT      = 20;
            public const int COLOR_3DDKSHADOW        = 21;
            public const int COLOR_3DLIGHT           = 22;
            public const int COLOR_INFOTEXT          = 23;
            public const int COLOR_INFOBK            = 24;
            public const int COLOR_HOTLIGHT          = 26;
            public const int COLOR_GRADIENTACTIVECAPTION = 27;
            public const int COLOR_GRADIENTINACTIVECAPTION = 28;
            public const int COLOR_MENUHILIGHT       = 29;
            public const int COLOR_MENUBAR           = 30;
            public const int COLOR_DESKTOP           = COLOR_BACKGROUND;
            public const int COLOR_3DFACE            = COLOR_BTNFACE;
            public const int COLOR_3DSHADOW          = COLOR_BTNSHADOW;
            public const int COLOR_3DHIGHLIGHT       = COLOR_BTNHIGHLIGHT;
            public const int COLOR_3DHILIGHT         = COLOR_BTNHIGHLIGHT;
            public const int COLOR_BTNHILIGHT        = COLOR_BTNHIGHLIGHT;

            /*
             * GetWindow() Constants
             */
            public const int GW_HWNDFIRST        = 0;
            public const int GW_HWNDLAST         = 1;
            public const int GW_HWNDNEXT         = 2;
            public const int GW_HWNDPREV         = 3;
            public const int GW_OWNER            = 4;
            public const int GW_CHILD            = 5;
          //public const int GW_MAX              = 5;  // WINVER <= 0x0400
            public const int GW_ENABLEDPOPUP     = 6;
            public const int GW_MAX              = 6;

            /* ;win40  -- A lot of MF_* flags have been renamed as MFT_* and MFS_* flags */
            /*
             * Menu flags for Add/Check/EnableMenuItem()
             */
            public const int MF_INSERT           = 0x00000000;
            public const int MF_CHANGE           = 0x00000080;
            public const int MF_APPEND           = 0x00000100;
            public const int MF_DELETE           = 0x00000200;
            public const int MF_REMOVE           = 0x00001000;
            public const int MF_BYCOMMAND        = 0x00000000;
            public const int MF_BYPOSITION       = 0x00000400;
            public const int MF_SEPARATOR        = 0x00000800;
            public const int MF_ENABLED          = 0x00000000;
            public const int MF_GRAYED           = 0x00000001;
            public const int MF_DISABLED         = 0x00000002;
            public const int MF_UNCHECKED        = 0x00000000;
            public const int MF_CHECKED          = 0x00000008;
            public const int MF_USECHECKBITMAPS  = 0x00000200;
            public const int MF_STRING           = 0x00000000;
            public const int MF_BITMAP           = 0x00000004;
            public const int MF_OWNERDRAW        = 0x00000100;
            public const int MF_POPUP            = 0x00000010;
            public const int MF_MENUBARBREAK     = 0x00000020;
            public const int MF_MENUBREAK        = 0x00000040;
            public const int MF_UNHILITE         = 0x00000000;
            public const int MF_HILITE           = 0x00000080;
            public const int MF_DEFAULT          = 0x00001000;
            public const int MF_SYSMENU          = 0x00002000;
            public const int MF_HELP             = 0x00004000;
            public const int MF_RIGHTJUSTIFY     = 0x00004000;
            public const int MF_MOUSESELECT      = 0x00008000;
          //public const int MF_END              = 0x00000080;  /* Obsolete -- only used by old RES files  WINVER >= 0x0400 */

            public const int MFT_STRING          = MF_STRING;
            public const int MFT_BITMAP          = MF_BITMAP;
            public const int MFT_MENUBARBREAK    = MF_MENUBARBREAK;
            public const int MFT_MENUBREAK       = MF_MENUBREAK;
            public const int MFT_OWNERDRAW       = MF_OWNERDRAW;
            public const int MFT_RADIOCHECK      = 0x00000200;
            public const int MFT_SEPARATOR       = MF_SEPARATOR;
            public const int MFT_RIGHTORDER      = 0x00002000;
            public const int MFT_RIGHTJUSTIFY    = MF_RIGHTJUSTIFY;

            /* Menu flags for Add/Check/EnableMenuItem() */
            public const int MFS_GRAYED          = 0x00000003;
            public const int MFS_DISABLED        = MFS_GRAYED;
            public const int MFS_CHECKED         = MF_CHECKED;
            public const int MFS_HILITE          = MF_HILITE;
            public const int MFS_ENABLED         = MF_ENABLED;
            public const int MFS_UNCHECKED       = MF_UNCHECKED;
            public const int MFS_UNHILITE        = MF_UNHILITE;
            public const int MFS_DEFAULT         = MF_DEFAULT;

            public const int MF_END              = 0x00000080;

            /*
             * System Menu Command Values
             */
            //public const int SC_SIZE         = 0xF000;
            //public const int SC_MOVE         = 0xF010;
            //public const int SC_MINIMIZE     = 0xF020;
            //public const int SC_MAXIMIZE     = 0xF030;
            //public const int SC_NEXTWINDOW   = 0xF040;
            //public const int SC_PREVWINDOW   = 0xF050;
            //public const int SC_CLOSE        = 0xF060;
            //public const int SC_VSCROLL      = 0xF070;
            //public const int SC_HSCROLL      = 0xF080;
            //public const int SC_MOUSEMENU    = 0xF090;
            //public const int SC_KEYMENU      = 0xF100;
            //public const int SC_ARRANGE      = 0xF110;
            //public const int SC_RESTORE      = 0xF120;
            //public const int SC_TASKLIST     = 0xF130;
            //public const int SC_SCREENSAVE   = 0xF140;
            //public const int SC_HOTKEY       = 0xF150;
            //public const int SC_DEFAULT      = 0xF160;
            //public const int SC_MONITORPOWER = 0xF170;
            //public const int SC_CONTEXTHELP  = 0xF180;
            //public const int SC_SEPARATOR    = 0xF00F;

            public const int SCF_ISSECURE    = 0x00000001;

            /*
             * Obsolete names
             */
            //public const int SC_ICON         = SC_MINIMIZE;
            //public const int SC_ZOOM         = SC_MAXIMIZE;

            /*
             * Standard Cursor IDs
             */
            //public const int IDC_ARROW           = MAKEINTRESOURCE(32512);
            //public const int IDC_IBEAM           = MAKEINTRESOURCE(32513);
            //public const int IDC_WAIT            = MAKEINTRESOURCE(32514);
            //public const int IDC_CROSS           = MAKEINTRESOURCE(32515);
            //public const int IDC_UPARROW         = MAKEINTRESOURCE(32516);
            //public const int IDC_SIZE            = MAKEINTRESOURCE(32640);  /* OBSOLETE: use IDC_SIZEALL */
            //public const int IDC_ICON            = MAKEINTRESOURCE(32641);  /* OBSOLETE: use IDC_ARROW */
            //public const int IDC_SIZENWSE        = MAKEINTRESOURCE(32642);
            //public const int IDC_SIZENESW        = MAKEINTRESOURCE(32643);
            //public const int IDC_SIZEWE          = MAKEINTRESOURCE(32644);
            //public const int IDC_SIZENS          = MAKEINTRESOURCE(32645);
            //public const int IDC_SIZEALL         = MAKEINTRESOURCE(32646);
            //public const int IDC_NO              = MAKEINTRESOURCE(32648); /*not in win3.1 */
            //public const int IDC_HAND            = MAKEINTRESOURCE(32649);
            //public const int IDC_APPSTARTING     = MAKEINTRESOURCE(32650); /*not in win3.1 */
            //public const int IDC_HELP            = MAKEINTRESOURCE(32651);

            public const int IMAGE_BITMAP        = 0;
            public const int IMAGE_ICON          = 1;
            public const int IMAGE_CURSOR        = 2;
            public const int IMAGE_ENHMETAFILE   = 3;

            public const int LR_DEFAULTCOLOR     = 0x00000000;
            public const int LR_MONOCHROME       = 0x00000001;
            public const int LR_COLOR            = 0x00000002;
            public const int LR_COPYRETURNORG    = 0x00000004;
            public const int LR_COPYDELETEORG    = 0x00000008;
            public const int LR_LOADFROMFILE     = 0x00000010;
            public const int LR_LOADTRANSPARENT  = 0x00000020;
            public const int LR_DEFAULTSIZE      = 0x00000040;
            public const int LR_VGACOLOR         = 0x00000080;
            public const int LR_LOADMAP3DCOLORS  = 0x00001000;
            public const int LR_CREATEDIBSECTION = 0x00002000;
            public const int LR_COPYFROMRESOURCE = 0x00004000;
            public const int LR_SHARED           = 0x00008000;

            public const int DI_MASK         = 0x0001;
            public const int DI_IMAGE        = 0x0002;
            public const int DI_NORMAL       = 0x0003;
            public const int DI_COMPAT       = 0x0004;
            public const int DI_DEFAULTSIZE  = 0x0008;
            public const int DI_NOMIRROR     = 0x0010;

            public const int RES_ICON    = 1;
            public const int RES_CURSOR  = 2;

            /*
             * OEM Resource Ordinal Numbers
             */
            public const int OBM_CLOSE           = 32754;
            public const int OBM_UPARROW         = 32753;
            public const int OBM_DNARROW         = 32752;
            public const int OBM_RGARROW         = 32751;
            public const int OBM_LFARROW         = 32750;
            public const int OBM_REDUCE          = 32749;
            public const int OBM_ZOOM            = 32748;
            public const int OBM_RESTORE         = 32747;
            public const int OBM_REDUCED         = 32746;
            public const int OBM_ZOOMD           = 32745;
            public const int OBM_RESTORED        = 32744;
            public const int OBM_UPARROWD        = 32743;
            public const int OBM_DNARROWD        = 32742;
            public const int OBM_RGARROWD        = 32741;
            public const int OBM_LFARROWD        = 32740;
            public const int OBM_MNARROW         = 32739;
            public const int OBM_COMBO           = 32738;
            public const int OBM_UPARROWI        = 32737;
            public const int OBM_DNARROWI        = 32736;
            public const int OBM_RGARROWI        = 32735;
            public const int OBM_LFARROWI        = 32734;
            public const int OBM_OLD_CLOSE       = 32767;
            public const int OBM_SIZE            = 32766;
            public const int OBM_OLD_UPARROW     = 32765;
            public const int OBM_OLD_DNARROW     = 32764;
            public const int OBM_OLD_RGARROW     = 32763;
            public const int OBM_OLD_LFARROW     = 32762;
            public const int OBM_BTSIZE          = 32761;
            public const int OBM_CHECK           = 32760;
            public const int OBM_CHECKBOXES      = 32759;
            public const int OBM_BTNCORNERS      = 32758;
            public const int OBM_OLD_REDUCE      = 32757;
            public const int OBM_OLD_ZOOM        = 32756;
            public const int OBM_OLD_RESTORE     = 32755;

            public const int OCR_NORMAL          = 32512;
            public const int OCR_IBEAM           = 32513;
            public const int OCR_WAIT            = 32514;
            public const int OCR_CROSS           = 32515;
            public const int OCR_UP              = 32516;
            public const int OCR_SIZE            = 32640;   /* OBSOLETE: use OCR_SIZEALL */
            public const int OCR_ICON            = 32641;   /* OBSOLETE: use OCR_NORMAL */
            public const int OCR_SIZENWSE        = 32642;
            public const int OCR_SIZENESW        = 32643;
            public const int OCR_SIZEWE          = 32644;
            public const int OCR_SIZENS          = 32645;
            public const int OCR_SIZEALL         = 32646;
            public const int OCR_ICOCUR          = 32647;   /* OBSOLETE: use OIC_WINLOGO */
            public const int OCR_NO              = 32648;
            public const int OCR_HAND            = 32649;
            public const int OCR_APPSTARTING     = 32650;

            public const int OIC_SAMPLE          = 32512;
            public const int OIC_HAND            = 32513;
            public const int OIC_QUES            = 32514;
            public const int OIC_BANG            = 32515;
            public const int OIC_NOTE            = 32516;
            public const int OIC_WINLOGO         = 32517;
            public const int OIC_WARNING         = OIC_BANG;
            public const int OIC_ERROR           = OIC_HAND;
            public const int OIC_INFORMATION     = OIC_NOTE;
            public const int OIC_SHIELD          = 32518;

            public const int ORD_LANGDRIVER    = 1;     /* The ordinal number for the entry point of language drivers. */

            /*
             * Standard Icon IDs
             */
            // RC_INVOKED defined
            public const int IDI_APPLICATION     = 32512;
            public const int IDI_HAND            = 32513;
            public const int IDI_QUESTION        = 32514;
            public const int IDI_EXCLAMATION     = 32515;
            public const int IDI_ASTERISK        = 32516;
            public const int IDI_WINLOGO         = 32517;
            public const int IDI_SHIELD          = 32518;

            // RC_INVOKED not defined
            //public const int IDI_APPLICATION     = MAKEINTRESOURCE(32512);
            //public const int IDI_HAND            = MAKEINTRESOURCE(32513);
            //public const int IDI_QUESTION        = MAKEINTRESOURCE(32514);
            //public const int IDI_EXCLAMATION     = MAKEINTRESOURCE(32515);
            //public const int IDI_ASTERISK        = MAKEINTRESOURCE(32516);
            //public const int IDI_WINLOGO         = MAKEINTRESOURCE(32517);
            //public const int IDI_SHIELD          = MAKEINTRESOURCE(32518);

            public const int IDI_WARNING     = IDI_EXCLAMATION;
            public const int IDI_ERROR       = IDI_HAND;
            public const int IDI_INFORMATION = IDI_ASTERISK;

            /*
             * Dialog Box Command IDs
             */
            public const int IDOK                = 1;
            public const int IDCANCEL            = 2;
            public const int IDABORT             = 3;
            public const int IDRETRY             = 4;
            public const int IDIGNORE            = 5;
            public const int IDYES               = 6;
            public const int IDNO                = 7;
            public const int IDCLOSE             = 8;
            public const int IDHELP              = 9;
            public const int IDTRYAGAIN          = 10;
            public const int IDCONTINUE          = 11;
            public const int IDTIMEOUT           = 32000;


            /*
             * Control Manager Structures and Definitions
             */

            /*
             * Edit Control Styles
             */
            public const int ES_LEFT             = 0x0000;
            public const int ES_CENTER           = 0x0001;
            public const int ES_RIGHT            = 0x0002;
            public const int ES_MULTILINE        = 0x0004;
            public const int ES_UPPERCASE        = 0x0008;
            public const int ES_LOWERCASE        = 0x0010;
            public const int ES_PASSWORD         = 0x0020;
            public const int ES_AUTOVSCROLL      = 0x0040;
            public const int ES_AUTOHSCROLL      = 0x0080;
            public const int ES_NOHIDESEL        = 0x0100;
            public const int ES_OEMCONVERT       = 0x0400;
            public const int ES_READONLY         = 0x0800;
            public const int ES_WANTRETURN       = 0x1000;
            public const int ES_NUMBER           = 0x2000;


            /*
             * Edit Control Notification Codes
             */
            public const int EN_SETFOCUS         = 0x0100;
            public const int EN_KILLFOCUS        = 0x0200;
            public const int EN_CHANGE           = 0x0300;
            public const int EN_UPDATE           = 0x0400;
            public const int EN_ERRSPACE         = 0x0500;
            public const int EN_MAXTEXT          = 0x0501;
            public const int EN_HSCROLL          = 0x0601;
            public const int EN_VSCROLL          = 0x0602;

            public const int EN_ALIGN_LTR_EC     = 0x0700;
            public const int EN_ALIGN_RTL_EC     = 0x0701;

            /* Edit control EM_SETMARGIN parameters */
            public const int EC_LEFTMARGIN       = 0x0001;
            public const int EC_RIGHTMARGIN      = 0x0002;
            public const int EC_USEFONTINFO      = 0xffff;

            /* wParam of EM_GET/SETIMESTATUS  */
            public const int EMSIS_COMPOSITIONSTRING        = 0x0001;

            /* lParam for EMSIS_COMPOSITIONSTRING  */
            public const int EIMES_GETCOMPSTRATONCE         = 0x0001;
            public const int EIMES_CANCELCOMPSTRINFOCUS     = 0x0002;
            public const int EIMES_COMPLETECOMPSTRKILLFOCUS = 0x0004;

            /*
             * Edit Control Messages
             */
            public const int EM_GETSEL               = 0x00B0;
            public const int EM_SETSEL               = 0x00B1;
            public const int EM_GETRECT              = 0x00B2;
            public const int EM_SETRECT              = 0x00B3;
            public const int EM_SETRECTNP            = 0x00B4;
            public const int EM_SCROLL               = 0x00B5;
            public const int EM_LINESCROLL           = 0x00B6;
            public const int EM_SCROLLCARET          = 0x00B7;
            public const int EM_GETMODIFY            = 0x00B8;
            public const int EM_SETMODIFY            = 0x00B9;
            public const int EM_GETLINECOUNT         = 0x00BA;
            public const int EM_LINEINDEX            = 0x00BB;
            public const int EM_SETHANDLE            = 0x00BC;
            public const int EM_GETHANDLE            = 0x00BD;
            public const int EM_GETTHUMB             = 0x00BE;
            public const int EM_LINELENGTH           = 0x00C1;
            public const int EM_REPLACESEL           = 0x00C2;
            public const int EM_GETLINE              = 0x00C4;
            public const int EM_LIMITTEXT            = 0x00C5;
            public const int EM_CANUNDO              = 0x00C6;
            public const int EM_UNDO                 = 0x00C7;
            public const int EM_FMTLINES             = 0x00C8;
            public const int EM_LINEFROMCHAR         = 0x00C9;
            public const int EM_SETTABSTOPS          = 0x00CB;
            public const int EM_SETPASSWORDCHAR      = 0x00CC;
            public const int EM_EMPTYUNDOBUFFER      = 0x00CD;
            public const int EM_GETFIRSTVISIBLELINE  = 0x00CE;
            public const int EM_SETREADONLY          = 0x00CF;
            public const int EM_SETWORDBREAKPROC     = 0x00D0;
            public const int EM_GETWORDBREAKPROC     = 0x00D1;
            public const int EM_GETPASSWORDCHAR      = 0x00D2;
            public const int EM_SETMARGINS           = 0x00D3;
            public const int EM_GETMARGINS           = 0x00D4;
            public const int EM_SETLIMITTEXT         = EM_LIMITTEXT;   /* ;win40 Name change */
            public const int EM_GETLIMITTEXT         = 0x00D5;
            public const int EM_POSFROMCHAR          = 0x00D6;
            public const int EM_CHARFROMPOS          = 0x00D7;
            public const int EM_SETIMESTATUS         = 0x00D8;
            public const int EM_GETIMESTATUS         = 0x00D9;


            /*
             * EDITWORDBREAKPROC code values
             */
            public const int WB_LEFT            = 0;
            public const int WB_RIGHT           = 1;
            public const int WB_ISDELIMITER     = 2;


            /*
             * Button Control Styles
             */
            public const int BS_PUSHBUTTON       = 0x00000000;
            public const int BS_DEFPUSHBUTTON    = 0x00000001;
            public const int BS_CHECKBOX         = 0x00000002;
            public const int BS_AUTOCHECKBOX     = 0x00000003;
            public const int BS_RADIOBUTTON      = 0x00000004;
            public const int BS_3STATE           = 0x00000005;
            public const int BS_AUTO3STATE       = 0x00000006;
            public const int BS_GROUPBOX         = 0x00000007;
            public const int BS_USERBUTTON       = 0x00000008;
            public const int BS_AUTORADIOBUTTON  = 0x00000009;
            public const int BS_PUSHBOX          = 0x0000000A;
            public const int BS_OWNERDRAW        = 0x0000000B;
            public const int BS_TYPEMASK         = 0x0000000F;
            public const int BS_LEFTTEXT         = 0x00000020;
            public const int BS_TEXT             = 0x00000000;
            public const int BS_ICON             = 0x00000040;
            public const int BS_BITMAP           = 0x00000080;
            public const int BS_LEFT             = 0x00000100;
            public const int BS_RIGHT            = 0x00000200;
            public const int BS_CENTER           = 0x00000300;
            public const int BS_TOP              = 0x00000400;
            public const int BS_BOTTOM           = 0x00000800;
            public const int BS_VCENTER          = 0x00000C00;
            public const int BS_PUSHLIKE         = 0x00001000;
            public const int BS_MULTILINE        = 0x00002000;
            public const int BS_NOTIFY           = 0x00004000;
            public const int BS_FLAT             = 0x00008000;
            public const int BS_RIGHTBUTTON      = BS_LEFTTEXT;

            /*
             * User Button Notification Codes
             */
            public const int BN_CLICKED          = 0;
            public const int BN_PAINT            = 1;
            public const int BN_HILITE           = 2;
            public const int BN_UNHILITE         = 3;
            public const int BN_DISABLE          = 4;
            public const int BN_DOUBLECLICKED    = 5;
            public const int BN_PUSHED           = BN_HILITE;
            public const int BN_UNPUSHED         = BN_UNHILITE;
            public const int BN_DBLCLK           = BN_DOUBLECLICKED;
            public const int BN_SETFOCUS         = 6;
            public const int BN_KILLFOCUS        = 7;

            /*
             * Button Control Messages
             */
            public const int BM_GETCHECK        = 0x00F0;
            public const int BM_SETCHECK        = 0x00F1;
            public const int BM_GETSTATE        = 0x00F2;
            public const int BM_SETSTATE        = 0x00F3;
            public const int BM_SETSTYLE        = 0x00F4;
            public const int BM_CLICK           = 0x00F5;
            public const int BM_GETIMAGE        = 0x00F6;
            public const int BM_SETIMAGE        = 0x00F7;
            public const int BM_SETDONTCLICK    = 0x00F8;

            public const int BST_UNCHECKED      = 0x0000;
            public const int BST_CHECKED        = 0x0001;
            public const int BST_INDETERMINATE  = 0x0002;
            public const int BST_PUSHED         = 0x0004;
            public const int BST_FOCUS          = 0x0008;

            /*
             * Static Control Constants
             */
            public const int SS_LEFT             = 0x00000000;
            public const int SS_CENTER           = 0x00000001;
            public const int SS_RIGHT            = 0x00000002;
            public const int SS_ICON             = 0x00000003;
            public const int SS_BLACKRECT        = 0x00000004;
            public const int SS_GRAYRECT         = 0x00000005;
            public const int SS_WHITERECT        = 0x00000006;
            public const int SS_BLACKFRAME       = 0x00000007;
            public const int SS_GRAYFRAME        = 0x00000008;
            public const int SS_WHITEFRAME       = 0x00000009;
            public const int SS_USERITEM         = 0x0000000A;
            public const int SS_SIMPLE           = 0x0000000B;
            public const int SS_LEFTNOWORDWRAP   = 0x0000000C;
            public const int SS_OWNERDRAW        = 0x0000000D;
            public const int SS_BITMAP           = 0x0000000E;
            public const int SS_ENHMETAFILE      = 0x0000000F;
            public const int SS_ETCHEDHORZ       = 0x00000010;
            public const int SS_ETCHEDVERT       = 0x00000011;
            public const int SS_ETCHEDFRAME      = 0x00000012;
            public const int SS_TYPEMASK         = 0x0000001F;
            public const int SS_REALSIZECONTROL  = 0x00000040;
            public const int SS_NOPREFIX         = 0x00000080; /* Don't do "&" character translation */
            public const int SS_NOTIFY           = 0x00000100;
            public const int SS_CENTERIMAGE      = 0x00000200;
            public const int SS_RIGHTJUST        = 0x00000400;
            public const int SS_REALSIZEIMAGE    = 0x00000800;
            public const int SS_SUNKEN           = 0x00001000;
            public const int SS_EDITCONTROL      = 0x00002000;
            public const int SS_ENDELLIPSIS      = 0x00004000;
            public const int SS_PATHELLIPSIS     = 0x00008000;
            public const int SS_WORDELLIPSIS     = 0x0000C000;
            public const int SS_ELLIPSISMASK     = 0x0000C000;

            /*
             * Static Control Mesages
             */
            public const int STM_SETICON         = 0x0170;
            public const int STM_GETICON         = 0x0171;
            public const int STM_SETIMAGE        = 0x0172;
            public const int STM_GETIMAGE        = 0x0173;
            public const int STN_CLICKED         = 0;
            public const int STN_DBLCLK          = 1;
            public const int STN_ENABLE          = 2;
            public const int STN_DISABLE         = 3;
            public const int STM_MSGMAX          = 0x0174;

            /*
             * Dialog window class
             */
            //public const int WC_DIALOG       = (MAKEINTATOM(0x8002));

            /*
             * Get/SetWindowWord/Long offsets for use with WC_DIALOG windows
             */
            public const int DWL_MSGRESULT   = 0;
            public const int DWL_DLGPROC     = 4;
            public const int DWL_USER        = 8;


            public const int DWLP_MSGRESULT  = 0;
            //public const int DWLP_DLGPROC    = DWLP_MSGRESULT + sizeof(LRESULT);
            //public const int DWLP_USER       = DWLP_DLGPROC + sizeof(DLGPROC);


            /*
             * DlgDirList, DlgDirListComboBox flags values
             */
            public const int DDL_READWRITE       = 0x0000;
            public const int DDL_READONLY        = 0x0001;
            public const int DDL_HIDDEN          = 0x0002;
            public const int DDL_SYSTEM          = 0x0004;
            public const int DDL_DIRECTORY       = 0x0010;
            public const int DDL_ARCHIVE         = 0x0020;
            public const int DDL_POSTMSGS        = 0x2000;
            public const int DDL_DRIVES          = 0x4000;
            public const int DDL_EXCLUSIVE       = 0x8000;


            /*
             * Dialog Styles
             */
            public const int DS_ABSALIGN         = 0x001;
            public const int DS_SYSMODAL         = 0x002;
            public const int DS_LOCALEDIT        = 0x020;   /* Edit items get Local storage. */
            public const int DS_SETFONT          = 0x040;   /* User specified font for Dlg controls */
            public const int DS_MODALFRAME       = 0x080;   /* Can be combined with WS_CAPTION  */
            public const int DS_NOIDLEMSG        = 0x100;  /* WM_ENTERIDLE message will not be sent */
            public const int DS_SETFOREGROUND    = 0x200;  /* not in win3.1 */
            public const int DS_3DLOOK           = 0x0004;
            public const int DS_FIXEDSYS         = 0x0008;
            public const int DS_NOFAILCREATE     = 0x0010;
            public const int DS_CONTROL          = 0x0400;
            public const int DS_CENTER           = 0x0800;
            public const int DS_CENTERMOUSE      = 0x1000;
            public const int DS_CONTEXTHELP      = 0x2000;
            public const int DS_SHELLFONT        = (DS_SETFONT | DS_FIXEDSYS);
            public const int DS_USEPIXELS        = 0x8000;
            public const int DM_GETDEFID         = ((int)WM.WM_USER+0);
            public const int DM_SETDEFID         = ((int)WM.WM_USER+1);
            public const int DM_REPOSITION       = ((int)WM.WM_USER+2);

            /*
             * Returned in HIWORD() of DM_GETDEFID result if msg is supported
             */
            public const int DC_HASDEFID         = 0x534B;

            /*
             * Dialog Codes
             */
            public const int DLGC_WANTARROWS      = 0x0001;      /* Control wants arrow keys         */
            public const int DLGC_WANTTAB         = 0x0002;      /* Control wants tab keys           */
            public const int DLGC_WANTALLKEYS     = 0x0004;      /* Control wants all keys           */
            public const int DLGC_WANTMESSAGE     = 0x0004;      /* Pass message to control          */
            public const int DLGC_HASSETSEL       = 0x0008;      /* Understands EM_SETSEL message    */
            public const int DLGC_DEFPUSHBUTTON   = 0x0010;      /* Default pushbutton               */
            public const int DLGC_UNDEFPUSHBUTTON = 0x0020;      /* Non-default pushbutton           */
            public const int DLGC_RADIOBUTTON     = 0x0040;      /* Radio button                     */
            public const int DLGC_WANTCHARS       = 0x0080;      /* Want WM_CHAR messages            */
            public const int DLGC_STATIC          = 0x0100;      /* Static item: don't include       */
            public const int DLGC_BUTTON          = 0x2000;      /* Button item: can be checked      */

            public const int LB_CTLCODE          = 0;

            /*
             * Listbox Return Values
             */
            public const int LB_OKAY             = 0;
            public const int LB_ERR              = (-1);
            public const int LB_ERRSPACE         = (-2);

            /*
            **  The idStaticPath parameter to DlgDirList can have the following values
            **  ORed if the list box should show other details of the files along with
            **  the name of the files;
            */
            /* all other details also will be returned */


            /*
             * Listbox Notification Codes
             */
            public const int LBN_ERRSPACE        = (-2);
            public const int LBN_SELCHANGE       = 1;
            public const int LBN_DBLCLK          = 2;
            public const int LBN_SELCANCEL       = 3;
            public const int LBN_SETFOCUS        = 4;
            public const int LBN_KILLFOCUS       = 5;

            /*
             * Listbox messages
             */
            public const int LB_ADDSTRING            = 0x0180;
            public const int LB_INSERTSTRING         = 0x0181;
            public const int LB_DELETESTRING         = 0x0182;
            public const int LB_SELITEMRANGEEX       = 0x0183;
            public const int LB_RESETCONTENT         = 0x0184;
            public const int LB_SETSEL               = 0x0185;
            public const int LB_SETCURSEL            = 0x0186;
            public const int LB_GETSEL               = 0x0187;
            public const int LB_GETCURSEL            = 0x0188;
            public const int LB_GETTEXT              = 0x0189;
            public const int LB_GETTEXTLEN           = 0x018A;
            public const int LB_GETCOUNT             = 0x018B;
            public const int LB_SELECTSTRING         = 0x018C;
            public const int LB_DIR                  = 0x018D;
            public const int LB_GETTOPINDEX          = 0x018E;
            public const int LB_FINDSTRING           = 0x018F;
            public const int LB_GETSELCOUNT          = 0x0190;
            public const int LB_GETSELITEMS          = 0x0191;
            public const int LB_SETTABSTOPS          = 0x0192;
            public const int LB_GETHORIZONTALEXTENT  = 0x0193;
            public const int LB_SETHORIZONTALEXTENT  = 0x0194;
            public const int LB_SETCOLUMNWIDTH       = 0x0195;
            public const int LB_ADDFILE              = 0x0196;
            public const int LB_SETTOPINDEX          = 0x0197;
            public const int LB_GETITEMRECT          = 0x0198;
            public const int LB_GETITEMDATA          = 0x0199;
            public const int LB_SETITEMDATA          = 0x019A;
            public const int LB_SELITEMRANGE         = 0x019B;
            public const int LB_SETANCHORINDEX       = 0x019C;
            public const int LB_GETANCHORINDEX       = 0x019D;
            public const int LB_SETCARETINDEX        = 0x019E;
            public const int LB_GETCARETINDEX        = 0x019F;
            public const int LB_SETITEMHEIGHT        = 0x01A0;
            public const int LB_GETITEMHEIGHT        = 0x01A1;
            public const int LB_FINDSTRINGEXACT      = 0x01A2;
            public const int LB_SETLOCALE            = 0x01A5;
            public const int LB_GETLOCALE            = 0x01A6;
            public const int LB_SETCOUNT             = 0x01A7;
            public const int LB_INITSTORAGE          = 0x01A8;
            public const int LB_ITEMFROMPOINT        = 0x01A9;
            public const int LB_MULTIPLEADDSTRING    = 0x01B1;


            public const int LB_GETLISTBOXINFO       = 0x01B2;

            public const int LB_MSGMAX               = 0x01B3; // _WIN32_WINNT >= 0x0501
          //public const int LB_MSGMAX               = 0x01B1; // else if _WIN32_WCE >= 0x0400
          //public const int LB_MSGMAX               = 0x01B0; // else if WINVER >= 0x0400
          //public const int LB_MSGMAX               = 0x01A8; // else

            /*
             * Listbox Styles
             */
            public const uint LBS_NOTIFY            = 0x0001;
            public const uint LBS_SORT              = 0x0002;
            public const uint LBS_NOREDRAW          = 0x0004;
            public const uint LBS_MULTIPLESEL       = 0x0008;
            public const uint LBS_OWNERDRAWFIXED    = 0x0010;
            public const uint LBS_OWNERDRAWVARIABLE = 0x0020;
            public const uint LBS_HASSTRINGS        = 0x0040;
            public const uint LBS_USETABSTOPS       = 0x0080;
            public const uint LBS_NOINTEGRALHEIGHT  = 0x0100;
            public const uint LBS_MULTICOLUMN       = 0x0200;
            public const uint LBS_WANTKEYBOARDINPUT = 0x0400;
            public const uint LBS_EXTENDEDSEL       = 0x0800;
            public const uint LBS_DISABLENOSCROLL   = 0x1000;
            public const uint LBS_NODATA            = 0x2000;
            public const uint LBS_NOSEL             = 0x4000;
            public const uint LBS_COMBOBOX          = 0x8000;
            public const uint LBS_STANDARD          = (LBS_NOTIFY | LBS_SORT | WS_VSCROLL | WS_BORDER);

            /*
             * Combo Box return Values
             */
            public const int CB_OKAY             = 0;
            public const int CB_ERR              = (-1);
            public const int CB_ERRSPACE         = (-2);

            /*
             * Combo Box Notification Codes
             */
            public const int CBN_ERRSPACE        = (-1);
            public const int CBN_SELCHANGE       = 1;
            public const int CBN_DBLCLK          = 2;
            public const int CBN_SETFOCUS        = 3;
            public const int CBN_KILLFOCUS       = 4;
            public const int CBN_EDITCHANGE      = 5;
            public const int CBN_EDITUPDATE      = 6;
            public const int CBN_DROPDOWN        = 7;
            public const int CBN_CLOSEUP         = 8;
            public const int CBN_SELENDOK        = 9;
            public const int CBN_SELENDCANCEL    = 10;

            /*
             * Combo Box styles
             */
            public const int CBS_SIMPLE            = 0x0001;
            public const int CBS_DROPDOWN          = 0x0002;
            public const int CBS_DROPDOWNLIST      = 0x0003;
            public const int CBS_OWNERDRAWFIXED    = 0x0010;
            public const int CBS_OWNERDRAWVARIABLE = 0x0020;
            public const int CBS_AUTOHSCROLL       = 0x0040;
            public const int CBS_OEMCONVERT        = 0x0080;
            public const int CBS_SORT              = 0x0100;
            public const int CBS_HASSTRINGS        = 0x0200;
            public const int CBS_NOINTEGRALHEIGHT  = 0x0400;
            public const int CBS_DISABLENOSCROLL   = 0x0800;
            public const int CBS_UPPERCASE         = 0x2000;
            public const int CBS_LOWERCASE         = 0x4000;

            /*
             * Combo Box messages
             */
            public const int CB_GETEDITSEL               = 0x0140;
            public const int CB_LIMITTEXT                = 0x0141;
            public const int CB_SETEDITSEL               = 0x0142;
            public const int CB_ADDSTRING                = 0x0143;
            public const int CB_DELETESTRING             = 0x0144;
            public const int CB_DIR                      = 0x0145;
            public const int CB_GETCOUNT                 = 0x0146;
            public const int CB_GETCURSEL                = 0x0147;
            public const int CB_GETLBTEXT                = 0x0148;
            public const int CB_GETLBTEXTLEN             = 0x0149;
            public const int CB_INSERTSTRING             = 0x014A;
            public const int CB_RESETCONTENT             = 0x014B;
            public const int CB_FINDSTRING               = 0x014C;
            public const int CB_SELECTSTRING             = 0x014D;
            public const int CB_SETCURSEL                = 0x014E;
            public const int CB_SHOWDROPDOWN             = 0x014F;
            public const int CB_GETITEMDATA              = 0x0150;
            public const int CB_SETITEMDATA              = 0x0151;
            public const int CB_GETDROPPEDCONTROLRECT    = 0x0152;
            public const int CB_SETITEMHEIGHT            = 0x0153;
            public const int CB_GETITEMHEIGHT            = 0x0154;
            public const int CB_SETEXTENDEDUI            = 0x0155;
            public const int CB_GETEXTENDEDUI            = 0x0156;
            public const int CB_GETDROPPEDSTATE          = 0x0157;
            public const int CB_FINDSTRINGEXACT          = 0x0158;
            public const int CB_SETLOCALE                = 0x0159;
            public const int CB_GETLOCALE                = 0x015A;
            public const int CB_GETTOPINDEX              = 0x015b;
            public const int CB_SETTOPINDEX              = 0x015c;
            public const int CB_GETHORIZONTALEXTENT      = 0x015d;
            public const int CB_SETHORIZONTALEXTENT      = 0x015e;
            public const int CB_GETDROPPEDWIDTH          = 0x015f;
            public const int CB_SETDROPPEDWIDTH          = 0x0160;
            public const int CB_INITSTORAGE              = 0x0161;
            public const int CB_MULTIPLEADDSTRING        = 0x0163;

            public const int CB_GETCOMBOBOXINFO          = 0x0164;

            public const int CB_MSGMAX                   = 0x0165; // _WIN32_WINNT >= 0x0501
          //public const int CB_MSGMAX                   = 0x0163; // _WIN32_WCE >= 0x0400
          //public const int CB_MSGMAX                   = 0x0162; // WINVER >= 0x0400
          //public const int CB_MSGMAX                   = 0x015B; // else

            /*
             * Scroll Bar Styles
             */
            public const int SBS_HORZ                    = 0x0000;
            public const int SBS_VERT                    = 0x0001;
            public const int SBS_TOPALIGN                = 0x0002;
            public const int SBS_LEFTALIGN               = 0x0002;
            public const int SBS_BOTTOMALIGN             = 0x0004;
            public const int SBS_RIGHTALIGN              = 0x0004;
            public const int SBS_SIZEBOXTOPLEFTALIGN     = 0x0002;
            public const int SBS_SIZEBOXBOTTOMRIGHTALIGN = 0x0004;
            public const int SBS_SIZEBOX                 = 0x0008;
            public const int SBS_SIZEGRIP                = 0x0010;

            /*
             * Scroll bar messages
             */
            public const int SBM_SETPOS                  = 0x00E0; /*not in win3.1 */
            public const int SBM_GETPOS                  = 0x00E1; /*not in win3.1 */
            public const int SBM_SETRANGE                = 0x00E2; /*not in win3.1 */
            public const int SBM_SETRANGEREDRAW          = 0x00E6; /*not in win3.1 */
            public const int SBM_GETRANGE                = 0x00E3; /*not in win3.1 */
            public const int SBM_ENABLE_ARROWS           = 0x00E4; /*not in win3.1 */
            public const int SBM_SETSCROLLINFO           = 0x00E9;
            public const int SBM_GETSCROLLINFO           = 0x00EA;
            public const int SBM_GETSCROLLBARINFO        = 0x00EB;

            public const int SIF_RANGE           = 0x0001;
            public const int SIF_PAGE            = 0x0002;
            public const int SIF_POS             = 0x0004;
            public const int SIF_DISABLENOSCROLL = 0x0008;
            public const int SIF_TRACKPOS        = 0x0010;
            public const int SIF_ALL             = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

            /*
             * MDI client style bits
             */
            public const int MDIS_ALLCHILDSTYLES    = 0x0001;

            /*
             * wParam Flags for WM_MDITILE and WM_MDICASCADE messages.
             */
            public const int MDITILE_VERTICAL       = 0x0000; /*not in win3.1 */
            public const int MDITILE_HORIZONTAL     = 0x0001; /*not in win3.1 */
            public const int MDITILE_SKIPDISABLED   = 0x0002; /*not in win3.1 */
            public const int MDITILE_ZORDER         = 0x0004;

            /*
             * Commands to pass to WinHelp()
             */
            public const int HELP_CONTEXT      = 0x0001;  /* Display topic in ulTopic */
            public const int HELP_QUIT         = 0x0002;  /* Terminate help */
            public const int HELP_INDEX        = 0x0003;  /* Display index */
            public const int HELP_CONTENTS     = 0x0003;
            public const int HELP_HELPONHELP   = 0x0004;  /* Display help on using help */
            public const int HELP_SETINDEX     = 0x0005;  /* Set current Index for multi index help */
            public const int HELP_SETCONTENTS  = 0x0005;
            public const int HELP_CONTEXTPOPUP = 0x0008;
            public const int HELP_FORCEFILE    = 0x0009;
            public const int HELP_KEY          = 0x0101;  /* Display topic for keyword in offabData */
            public const int HELP_COMMAND      = 0x0102;
            public const int HELP_PARTIALKEY   = 0x0105;
            public const int HELP_MULTIKEY     = 0x0201;
            public const int HELP_SETWINPOS    = 0x0203;
            public const int HELP_CONTEXTMENU  = 0x000a;
            public const int HELP_FINDER       = 0x000b;
            public const int HELP_WM_HELP      = 0x000c;
            public const int HELP_SETPOPUP_POS = 0x000d;

            public const int HELP_TCARD              = 0x8000;
            public const int HELP_TCARD_DATA         = 0x0010;
            public const int HELP_TCARD_OTHER_CALLER = 0x0011;

            // These are in winhelp.h in Win95.
            public const int IDH_NO_HELP                     = 28440;
            public const int IDH_MISSING_CONTEXT             = 28441; // Control doesn't have matching help context
            public const int IDH_GENERIC_HELP_BUTTON         = 28442; // Property sheet help button
            public const int IDH_OK                          = 28443;
            public const int IDH_CANCEL                      = 28444;
            public const int IDH_HELP                        = 28445;

            public const int GR_GDIOBJECTS     = 0;       /* Count of GDI objects */
            public const int GR_USEROBJECTS    = 1;       /* Count of USER objects */

            /*
             * Parameter for SystemParametersInfo()
             */
            public const int SPI_GETBEEP                 = 0x0001;
            public const int SPI_SETBEEP                 = 0x0002;
            public const int SPI_GETMOUSE                = 0x0003;
            public const int SPI_SETMOUSE                = 0x0004;
            public const int SPI_GETBORDER               = 0x0005;
            public const int SPI_SETBORDER               = 0x0006;
            public const int SPI_GETKEYBOARDSPEED        = 0x000A;
            public const int SPI_SETKEYBOARDSPEED        = 0x000B;
            public const int SPI_LANGDRIVER              = 0x000C;
            public const int SPI_ICONHORIZONTALSPACING   = 0x000D;
            public const int SPI_GETSCREENSAVETIMEOUT    = 0x000E;
            public const int SPI_SETSCREENSAVETIMEOUT    = 0x000F;
            public const int SPI_GETSCREENSAVEACTIVE     = 0x0010;
            public const int SPI_SETSCREENSAVEACTIVE     = 0x0011;
            public const int SPI_GETGRIDGRANULARITY      = 0x0012;
            public const int SPI_SETGRIDGRANULARITY      = 0x0013;
            public const int SPI_SETDESKWALLPAPER        = 0x0014;
            public const int SPI_SETDESKPATTERN          = 0x0015;
            public const int SPI_GETKEYBOARDDELAY        = 0x0016;
            public const int SPI_SETKEYBOARDDELAY        = 0x0017;
            public const int SPI_ICONVERTICALSPACING     = 0x0018;
            public const int SPI_GETICONTITLEWRAP        = 0x0019;
            public const int SPI_SETICONTITLEWRAP        = 0x001A;
            public const int SPI_GETMENUDROPALIGNMENT    = 0x001B;
            public const int SPI_SETMENUDROPALIGNMENT    = 0x001C;
            public const int SPI_SETDOUBLECLKWIDTH       = 0x001D;
            public const int SPI_SETDOUBLECLKHEIGHT      = 0x001E;
            public const int SPI_GETICONTITLELOGFONT     = 0x001F;
            public const int SPI_SETDOUBLECLICKTIME      = 0x0020;
            public const int SPI_SETMOUSEBUTTONSWAP      = 0x0021;
            public const int SPI_SETICONTITLELOGFONT     = 0x0022;
            public const int SPI_GETFASTTASKSWITCH       = 0x0023;
            public const int SPI_SETFASTTASKSWITCH       = 0x0024;
            public const int SPI_SETDRAGFULLWINDOWS      = 0x0025;
            public const int SPI_GETDRAGFULLWINDOWS      = 0x0026;
            public const int SPI_GETNONCLIENTMETRICS     = 0x0029;
            public const int SPI_SETNONCLIENTMETRICS     = 0x002A;
            public const int SPI_GETMINIMIZEDMETRICS     = 0x002B;
            public const int SPI_SETMINIMIZEDMETRICS     = 0x002C;
            public const int SPI_GETICONMETRICS          = 0x002D;
            public const int SPI_SETICONMETRICS          = 0x002E;
            public const int SPI_SETWORKAREA             = 0x002F;
            public const int SPI_GETWORKAREA             = 0x0030;
            public const int SPI_SETPENWINDOWS           = 0x0031;

            public const int SPI_GETHIGHCONTRAST         = 0x0042;
            public const int SPI_SETHIGHCONTRAST         = 0x0043;
            public const int SPI_GETKEYBOARDPREF         = 0x0044;
            public const int SPI_SETKEYBOARDPREF         = 0x0045;
            public const int SPI_GETSCREENREADER         = 0x0046;
            public const int SPI_SETSCREENREADER         = 0x0047;
            public const int SPI_GETANIMATION            = 0x0048;
            public const int SPI_SETANIMATION            = 0x0049;
            public const int SPI_GETFONTSMOOTHING        = 0x004A;
            public const int SPI_SETFONTSMOOTHING        = 0x004B;
            public const int SPI_SETDRAGWIDTH            = 0x004C;
            public const int SPI_SETDRAGHEIGHT           = 0x004D;
            public const int SPI_SETHANDHELD             = 0x004E;
            public const int SPI_GETLOWPOWERTIMEOUT      = 0x004F;
            public const int SPI_GETPOWEROFFTIMEOUT      = 0x0050;
            public const int SPI_SETLOWPOWERTIMEOUT      = 0x0051;
            public const int SPI_SETPOWEROFFTIMEOUT      = 0x0052;
            public const int SPI_GETLOWPOWERACTIVE       = 0x0053;
            public const int SPI_GETPOWEROFFACTIVE       = 0x0054;
            public const int SPI_SETLOWPOWERACTIVE       = 0x0055;
            public const int SPI_SETPOWEROFFACTIVE       = 0x0056;
            public const int SPI_SETCURSORS              = 0x0057;
            public const int SPI_SETICONS                = 0x0058;
            public const int SPI_GETDEFAULTINPUTLANG     = 0x0059;
            public const int SPI_SETDEFAULTINPUTLANG     = 0x005A;
            public const int SPI_SETLANGTOGGLE           = 0x005B;
            public const int SPI_GETWINDOWSEXTENSION     = 0x005C;
            public const int SPI_SETMOUSETRAILS          = 0x005D;
            public const int SPI_GETMOUSETRAILS          = 0x005E;
            public const int SPI_SETSCREENSAVERRUNNING   = 0x0061;
            public const int SPI_SCREENSAVERRUNNING     = SPI_SETSCREENSAVERRUNNING;
            public const int SPI_GETFILTERKEYS          = 0x0032;
            public const int SPI_SETFILTERKEYS          = 0x0033;
            public const int SPI_GETTOGGLEKEYS          = 0x0034;
            public const int SPI_SETTOGGLEKEYS          = 0x0035;
            public const int SPI_GETMOUSEKEYS           = 0x0036;
            public const int SPI_SETMOUSEKEYS           = 0x0037;
            public const int SPI_GETSHOWSOUNDS          = 0x0038;
            public const int SPI_SETSHOWSOUNDS          = 0x0039;
            public const int SPI_GETSTICKYKEYS          = 0x003A;
            public const int SPI_SETSTICKYKEYS          = 0x003B;
            public const int SPI_GETACCESSTIMEOUT       = 0x003C;
            public const int SPI_SETACCESSTIMEOUT       = 0x003D;
            public const int SPI_GETSERIALKEYS          = 0x003E;
            public const int SPI_SETSERIALKEYS          = 0x003F;
            public const int SPI_GETSOUNDSENTRY         = 0x0040;
            public const int SPI_SETSOUNDSENTRY         = 0x0041;
            public const int SPI_GETSNAPTODEFBUTTON     = 0x005F;
            public const int SPI_SETSNAPTODEFBUTTON     = 0x0060;
            public const int SPI_GETMOUSEHOVERWIDTH     = 0x0062;
            public const int SPI_SETMOUSEHOVERWIDTH     = 0x0063;
            public const int SPI_GETMOUSEHOVERHEIGHT    = 0x0064;
            public const int SPI_SETMOUSEHOVERHEIGHT    = 0x0065;
            public const int SPI_GETMOUSEHOVERTIME      = 0x0066;
            public const int SPI_SETMOUSEHOVERTIME      = 0x0067;
            public const int SPI_GETWHEELSCROLLLINES    = 0x0068;
            public const int SPI_SETWHEELSCROLLLINES    = 0x0069;
            public const int SPI_GETMENUSHOWDELAY       = 0x006A;
            public const int SPI_SETMENUSHOWDELAY       = 0x006B;

            public const int SPI_GETWHEELSCROLLCHARS   = 0x006C;
            public const int SPI_SETWHEELSCROLLCHARS   = 0x006D;

            public const int SPI_GETSHOWIMEUI          = 0x006E;
            public const int SPI_SETSHOWIMEUI          = 0x006F;

            public const int SPI_GETMOUSESPEED         = 0x0070;
            public const int SPI_SETMOUSESPEED         = 0x0071;
            public const int SPI_GETSCREENSAVERRUNNING = 0x0072;
            public const int SPI_GETDESKWALLPAPER      = 0x0073;

            public const int SPI_GETAUDIODESCRIPTION   = 0x0074;
            public const int SPI_SETAUDIODESCRIPTION   = 0x0075;

            public const int SPI_GETSCREENSAVESECURE   = 0x0076;
            public const int SPI_SETSCREENSAVESECURE   = 0x0077;


            public const int SPI_GETACTIVEWINDOWTRACKING         = 0x1000;
            public const int SPI_SETACTIVEWINDOWTRACKING         = 0x1001;
            public const int SPI_GETMENUANIMATION                = 0x1002;
            public const int SPI_SETMENUANIMATION                = 0x1003;
            public const int SPI_GETCOMBOBOXANIMATION            = 0x1004;
            public const int SPI_SETCOMBOBOXANIMATION            = 0x1005;
            public const int SPI_GETLISTBOXSMOOTHSCROLLING       = 0x1006;
            public const int SPI_SETLISTBOXSMOOTHSCROLLING       = 0x1007;
            public const int SPI_GETGRADIENTCAPTIONS             = 0x1008;
            public const int SPI_SETGRADIENTCAPTIONS             = 0x1009;
            public const int SPI_GETKEYBOARDCUES                 = 0x100A;
            public const int SPI_SETKEYBOARDCUES                 = 0x100B;
            public const int SPI_GETMENUUNDERLINES               = SPI_GETKEYBOARDCUES;
            public const int SPI_SETMENUUNDERLINES               = SPI_SETKEYBOARDCUES;
            public const int SPI_GETACTIVEWNDTRKZORDER           = 0x100C;
            public const int SPI_SETACTIVEWNDTRKZORDER           = 0x100D;
            public const int SPI_GETHOTTRACKING                  = 0x100E;
            public const int SPI_SETHOTTRACKING                  = 0x100F;
            public const int SPI_GETMENUFADE                     = 0x1012;
            public const int SPI_SETMENUFADE                     = 0x1013;
            public const int SPI_GETSELECTIONFADE                = 0x1014;
            public const int SPI_SETSELECTIONFADE                = 0x1015;
            public const int SPI_GETTOOLTIPANIMATION             = 0x1016;
            public const int SPI_SETTOOLTIPANIMATION             = 0x1017;
            public const int SPI_GETTOOLTIPFADE                  = 0x1018;
            public const int SPI_SETTOOLTIPFADE                  = 0x1019;
            public const int SPI_GETCURSORSHADOW                 = 0x101A;
            public const int SPI_SETCURSORSHADOW                 = 0x101B;
            public const int SPI_GETMOUSESONAR                   = 0x101C;
            public const int SPI_SETMOUSESONAR                   = 0x101D;
            public const int SPI_GETMOUSECLICKLOCK               = 0x101E;
            public const int SPI_SETMOUSECLICKLOCK               = 0x101F;
            public const int SPI_GETMOUSEVANISH                  = 0x1020;
            public const int SPI_SETMOUSEVANISH                  = 0x1021;
            public const int SPI_GETFLATMENU                     = 0x1022;
            public const int SPI_SETFLATMENU                     = 0x1023;
            public const int SPI_GETDROPSHADOW                   = 0x1024;
            public const int SPI_SETDROPSHADOW                   = 0x1025;
            public const int SPI_GETBLOCKSENDINPUTRESETS         = 0x1026;
            public const int SPI_SETBLOCKSENDINPUTRESETS         = 0x1027;

            public const int SPI_GETUIEFFECTS                    = 0x103E;
            public const int SPI_SETUIEFFECTS                    = 0x103F;

            public const int SPI_GETDISABLEOVERLAPPEDCONTENT     = 0x1040;
            public const int SPI_SETDISABLEOVERLAPPEDCONTENT     = 0x1041;
            public const int SPI_GETCLIENTAREAANIMATION          = 0x1042;
            public const int SPI_SETCLIENTAREAANIMATION          = 0x1043;
            public const int SPI_GETCLEARTYPE                    = 0x1048;
            public const int SPI_SETCLEARTYPE                    = 0x1049;
            public const int SPI_GETSPEECHRECOGNITION            = 0x104A;
            public const int SPI_SETSPEECHRECOGNITION            = 0x104B;

            public const int SPI_GETFOREGROUNDLOCKTIMEOUT        = 0x2000;
            public const int SPI_SETFOREGROUNDLOCKTIMEOUT        = 0x2001;
            public const int SPI_GETACTIVEWNDTRKTIMEOUT          = 0x2002;
            public const int SPI_SETACTIVEWNDTRKTIMEOUT          = 0x2003;
            public const int SPI_GETFOREGROUNDFLASHCOUNT         = 0x2004;
            public const int SPI_SETFOREGROUNDFLASHCOUNT         = 0x2005;
            public const int SPI_GETCARETWIDTH                   = 0x2006;
            public const int SPI_SETCARETWIDTH                   = 0x2007;

            public const int SPI_GETMOUSECLICKLOCKTIME           = 0x2008;
            public const int SPI_SETMOUSECLICKLOCKTIME           = 0x2009;
            public const int SPI_GETFONTSMOOTHINGTYPE            = 0x200A;
            public const int SPI_SETFONTSMOOTHINGTYPE            = 0x200B;

            /* constants for SPI_GETFONTSMOOTHINGTYPE and SPI_SETFONTSMOOTHINGTYPE: */
            public const int FE_FONTSMOOTHINGSTANDARD            = 0x0001;
            public const int FE_FONTSMOOTHINGCLEARTYPE           = 0x0002;
            public const int FE_FONTSMOOTHINGDOCKING             = 0x8000;

            public const int SPI_GETFONTSMOOTHINGCONTRAST           = 0x200C;
            public const int SPI_SETFONTSMOOTHINGCONTRAST           = 0x200D;

            public const int SPI_GETFOCUSBORDERWIDTH             = 0x200E;
            public const int SPI_SETFOCUSBORDERWIDTH             = 0x200F;
            public const int SPI_GETFOCUSBORDERHEIGHT            = 0x2010;
            public const int SPI_SETFOCUSBORDERHEIGHT            = 0x2011;

            public const int SPI_GETFONTSMOOTHINGORIENTATION           = 0x2012;
            public const int SPI_SETFONTSMOOTHINGORIENTATION           = 0x2013;

            /* constants for SPI_GETFONTSMOOTHINGORIENTATION and SPI_SETFONTSMOOTHINGORIENTATION: */
            public const int FE_FONTSMOOTHINGORIENTATIONBGR   = 0x0000;
            public const int FE_FONTSMOOTHINGORIENTATIONRGB   = 0x0001;

            public const int SPI_GETMINIMUMHITRADIUS             = 0x2014;
            public const int SPI_SETMINIMUMHITRADIUS             = 0x2015;
            public const int SPI_GETMESSAGEDURATION              = 0x2016;
            public const int SPI_SETMESSAGEDURATION              = 0x2017;

            /*
             * Flags
             */
            public const int SPIF_UPDATEINIFILE    = 0x0001;
            public const int SPIF_SENDWININICHANGE = 0x0002;
            public const int SPIF_SENDCHANGE       = SPIF_SENDWININICHANGE;


            public const int METRICS_USEDEFAULT = -1;

            public const int ARW_BOTTOMLEFT              = 0x0000;
            public const int ARW_BOTTOMRIGHT             = 0x0001;
            public const int ARW_TOPLEFT                 = 0x0002;
            public const int ARW_TOPRIGHT                = 0x0003;
            public const int ARW_STARTMASK               = 0x0003;
            public const int ARW_STARTRIGHT              = 0x0001;
            public const int ARW_STARTTOP                = 0x0002;

            public const int ARW_LEFT                    = 0x0000;
            public const int ARW_RIGHT                   = 0x0000;
            public const int ARW_UP                      = 0x0004;
            public const int ARW_DOWN                    = 0x0004;
            public const int ARW_HIDE                    = 0x0008;

            /* flags for SERIALKEYS dwFlags field */
            public const int SERKF_SERIALKEYSON  = 0x00000001;
            public const int SERKF_AVAILABLE     = 0x00000002;
            public const int SERKF_INDICATOR     = 0x00000004;

            /* flags for HIGHCONTRAST dwFlags field */
            public const int HCF_HIGHCONTRASTON  = 0x00000001;
            public const int HCF_AVAILABLE       = 0x00000002;
            public const int HCF_HOTKEYACTIVE    = 0x00000004;
            public const int HCF_CONFIRMHOTKEY   = 0x00000008;
            public const int HCF_HOTKEYSOUND     = 0x00000010;
            public const int HCF_INDICATOR       = 0x00000020;
            public const int HCF_HOTKEYAVAILABLE = 0x00000040;
            public const int HCF_LOGONDESKTOP    = 0x00000100;
            public const int HCF_DEFAULTDESKTOP  = 0x00000200;

            /* Flags for ChangeDisplaySettings */
            public const int CDS_UPDATEREGISTRY           = 0x00000001;
            public const int CDS_TEST                     = 0x00000002;
            public const int CDS_FULLSCREEN               = 0x00000004;
            public const int CDS_GLOBAL                   = 0x00000008;
            public const int CDS_SET_PRIMARY              = 0x00000010;
            public const int CDS_VIDEOPARAMETERS          = 0x00000020;
            public const int CDS_ENABLE_UNSAFE_MODES      = 0x00000100;
            public const int CDS_DISABLE_UNSAFE_MODES     = 0x00000200;
            public const int CDS_RESET                    = 0x40000000;
            public const int CDS_NORESET                  = 0x10000000;

            //#include <tvout.h>

            /* Return values for ChangeDisplaySettings */
            public const int DISP_CHANGE_SUCCESSFUL       = 0;
            public const int DISP_CHANGE_RESTART          = 1;
            public const int DISP_CHANGE_FAILED          = -1;
            public const int DISP_CHANGE_BADMODE         = -2;
            public const int DISP_CHANGE_NOTUPDATED      = -3;
            public const int DISP_CHANGE_BADFLAGS        = -4;
            public const int DISP_CHANGE_BADPARAM        = -5;
            public const int DISP_CHANGE_BADDUALVIEW     = -6;

            public const int ENUM_CURRENT_SETTINGS       = -1;
            public const int ENUM_REGISTRY_SETTINGS      = -2;

            /* Flags for EnumDisplaySettingsEx */
            public const int EDS_RAWMODE                   = 0x00000002;
            public const int EDS_ROTATEDMODE               = 0x00000004;

            /* Flags for EnumDisplayDevices */
            public const int EDD_GET_DEVICE_INTERFACE_NAME = 0x00000001;

            /*
             * FILTERKEYS dwFlags field
             */
            public const int FKF_FILTERKEYSON    = 0x00000001;
            public const int FKF_AVAILABLE       = 0x00000002;
            public const int FKF_HOTKEYACTIVE    = 0x00000004;
            public const int FKF_CONFIRMHOTKEY   = 0x00000008;
            public const int FKF_HOTKEYSOUND     = 0x00000010;
            public const int FKF_INDICATOR       = 0x00000020;
            public const int FKF_CLICKON         = 0x00000040;

            /*
             * STICKYKEYS dwFlags field
             */
            public const uint SKF_STICKYKEYSON    = 0x00000001;
            public const uint SKF_AVAILABLE       = 0x00000002;
            public const uint SKF_HOTKEYACTIVE    = 0x00000004;
            public const uint SKF_CONFIRMHOTKEY   = 0x00000008;
            public const uint SKF_HOTKEYSOUND     = 0x00000010;
            public const uint SKF_INDICATOR       = 0x00000020;
            public const uint SKF_AUDIBLEFEEDBACK = 0x00000040;
            public const uint SKF_TRISTATE        = 0x00000080;
            public const uint SKF_TWOKEYSOFF      = 0x00000100;
            public const uint SKF_LALTLATCHED       = 0x10000000;
            public const uint SKF_LCTLLATCHED       = 0x04000000;
            public const uint SKF_LSHIFTLATCHED     = 0x01000000;
            public const uint SKF_RALTLATCHED       = 0x20000000;
            public const uint SKF_RCTLLATCHED       = 0x08000000;
            public const uint SKF_RSHIFTLATCHED     = 0x02000000;
            public const uint SKF_LWINLATCHED       = 0x40000000;
            public const uint SKF_RWINLATCHED       = 0x80000000;
            public const uint SKF_LALTLOCKED        = 0x00100000;
            public const uint SKF_LCTLLOCKED        = 0x00040000;
            public const uint SKF_LSHIFTLOCKED      = 0x00010000;
            public const uint SKF_RALTLOCKED        = 0x00200000;
            public const uint SKF_RCTLLOCKED        = 0x00080000;
            public const uint SKF_RSHIFTLOCKED      = 0x00020000;
            public const uint SKF_LWINLOCKED        = 0x00400000;
            public const uint SKF_RWINLOCKED        = 0x00800000;

            /*
             * MOUSEKEYS dwFlags field
             */
            public const uint MKF_MOUSEKEYSON     = 0x00000001;
            public const uint MKF_AVAILABLE       = 0x00000002;
            public const uint MKF_HOTKEYACTIVE    = 0x00000004;
            public const uint MKF_CONFIRMHOTKEY   = 0x00000008;
            public const uint MKF_HOTKEYSOUND     = 0x00000010;
            public const uint MKF_INDICATOR       = 0x00000020;
            public const uint MKF_MODIFIERS       = 0x00000040;
            public const uint MKF_REPLACENUMBERS  = 0x00000080;
            public const uint MKF_LEFTBUTTONSEL   = 0x10000000;
            public const uint MKF_RIGHTBUTTONSEL  = 0x20000000;
            public const uint MKF_LEFTBUTTONDOWN  = 0x01000000;
            public const uint MKF_RIGHTBUTTONDOWN = 0x02000000;
            public const uint MKF_MOUSEMODE       = 0x80000000;

            /*
             * ACCESSTIMEOUT dwFlags field
             */
            public const int ATF_TIMEOUTON       = 0x00000001;
            public const int ATF_ONOFFFEEDBACK   = 0x00000002;

            /* values for SOUNDSENTRY iFSGrafEffect field */
            public const int SSGF_NONE       = 0;
            public const int SSGF_DISPLAY    = 3;

            /* values for SOUNDSENTRY iFSTextEffect field */
            public const int SSTF_NONE       = 0;
            public const int SSTF_CHARS      = 1;
            public const int SSTF_BORDER     = 2;
            public const int SSTF_DISPLAY    = 3;

            /* values for SOUNDSENTRY iWindowsEffect field */
            public const int SSWF_NONE     = 0;
            public const int SSWF_TITLE    = 1;
            public const int SSWF_WINDOW   = 2;
            public const int SSWF_DISPLAY  = 3;
            public const int SSWF_CUSTOM   = 4;

            /*
             * SOUNDSENTRY dwFlags field
             */
            public const int SSF_SOUNDSENTRYON   = 0x00000001;
            public const int SSF_AVAILABLE       = 0x00000002;
            public const int SSF_INDICATOR       = 0x00000004;

            /*
             * TOGGLEKEYS dwFlags field
             */
            public const int TKF_TOGGLEKEYSON    = 0x00000001;
            public const int TKF_AVAILABLE       = 0x00000002;
            public const int TKF_HOTKEYACTIVE    = 0x00000004;
            public const int TKF_CONFIRMHOTKEY   = 0x00000008;
            public const int TKF_HOTKEYSOUND     = 0x00000010;
            public const int TKF_INDICATOR       = 0x00000020;

            /*
             * SetLastErrorEx() types.
             */
            public const int SLE_ERROR       = 0x00000001;
            public const int SLE_MINORERROR  = 0x00000002;
            public const int SLE_WARNING     = 0x00000003;

            public const int MONITOR_DEFAULTTONULL       = 0x00000000;
            public const int MONITOR_DEFAULTTOPRIMARY    = 0x00000001;
            public const int MONITOR_DEFAULTTONEAREST    = 0x00000002;

            public const int MONITORINFOF_PRIMARY        = 0x00000001;

            public const int CCHDEVICENAME = 32;

            /*
             * dwFlags for SetWinEventHook
             */
            public const int WINEVENT_OUTOFCONTEXT   = 0x0000;  // Events are ASYNC
            public const int WINEVENT_SKIPOWNTHREAD  = 0x0001;  // Don't call back for events on installer's thread
            public const int WINEVENT_SKIPOWNPROCESS = 0x0002;  // Don't call back for events on installer's process
            public const int WINEVENT_INCONTEXT      = 0x0004;  // Events are SYNC, this causes your dll to be injected into every process

            /*
             * idObject values for WinEventProc and NotifyWinEvent
             */

            /*
             * hwnd + idObject can be used with OLEACC.DLL's OleGetObjectFromWindow()
             * to get an interface pointer to the container.  indexChild is the item
             * within the container in question.  Setup a VARIANT with vt VT_I4 and
             * lVal the indexChild and pass that in to all methods.  Then you
             * are raring to go.
             */


            /*
             * Common object IDs (cookies, only for sending WM_GETOBJECT to get at the
             * thing in question).  Positive IDs are reserved for apps (app specific),
             * negative IDs are system things and are global, 0 means "just little old
             * me".
             */
            public const int CHILDID_SELF        = 0;
            public const int INDEXID_OBJECT      = 0;
            public const int INDEXID_CONTAINER   = 0;

            /*
             * Reserved IDs for system objects
             */
            public const uint OBJID_WINDOW            = 0x00000000;
            public const uint OBJID_SYSMENU           = 0xFFFFFFFF;
            public const uint OBJID_TITLEBAR          = 0xFFFFFFFE;
            public const uint OBJID_MENU              = 0xFFFFFFFD;
            public const uint OBJID_CLIENT            = 0xFFFFFFFC;
            public const uint OBJID_VSCROLL           = 0xFFFFFFFB;
            public const uint OBJID_HSCROLL           = 0xFFFFFFFA;
            public const uint OBJID_SIZEGRIP          = 0xFFFFFFF9;
            public const uint OBJID_CARET             = 0xFFFFFFF8;
            public const uint OBJID_CURSOR            = 0xFFFFFFF7;
            public const uint OBJID_ALERT             = 0xFFFFFFF6;
            public const uint OBJID_SOUND             = 0xFFFFFFF5;
            public const uint OBJID_QUERYCLASSNAMEIDX = 0xFFFFFFF4;
            public const uint OBJID_NATIVEOM          = 0xFFFFFFF0;

            /*
             * EVENT DEFINITION
             */
            public const int EVENT_MIN           = 0x00000001;
            public const int EVENT_MAX           = 0x7FFFFFFF;


            /*
             *  EVENT_SYSTEM_SOUND
             *  Sent when a sound is played.  Currently nothing is generating this, we
             *  this event when a system sound (for menus, etc) is played.  Apps
             *  generate this, if accessible, when a private sound is played.  For
             *  example, if Mail plays a "New Mail" sound.
             *
             *  System Sounds:
             *  (Generated by PlaySoundEvent in USER itself)
             *      hwnd            is NULL
             *      idObject        is OBJID_SOUND
             *      idChild         is sound child ID if one
             *  App Sounds:
             *  (PlaySoundEvent won't generate notification; up to app)
             *      hwnd + idObject gets interface pointer to Sound object
             *      idChild identifies the sound in question
             *  are going to be cleaning up the SOUNDSENTRY feature in the control panel
             *  and will use this at that time.  Applications implementing WinEvents
             *  are perfectly welcome to use it.  Clients of IAccessible* will simply
             *  turn around and get back a non-visual object that describes the sound.
             */
            public const int EVENT_SYSTEM_SOUND              = 0x0001;

            /*
             * EVENT_SYSTEM_ALERT
             * System Alerts:
             * (Generated by MessageBox() calls for example)
             *      hwnd            is hwndMessageBox
             *      idObject        is OBJID_ALERT
             * App Alerts:
             * (Generated whenever)
             *      hwnd+idObject gets interface pointer to Alert
             */
            public const int EVENT_SYSTEM_ALERT              = 0x0002;

            /*
             * EVENT_SYSTEM_FOREGROUND
             * Sent when the foreground (active) window changes, even if it is changing
             * to another window in the same thread as the previous one.
             *      hwnd            is hwndNewForeground
             *      idObject        is OBJID_WINDOW
             *      idChild    is INDEXID_OBJECT
             */
            public const int EVENT_SYSTEM_FOREGROUND         = 0x0003;

            /*
             * Menu
             *      hwnd            is window (top level window or popup menu window)
             *      idObject        is ID of control (OBJID_MENU, OBJID_SYSMENU, OBJID_SELF for popup)
             *      idChild         is CHILDID_SELF
             *
             * EVENT_SYSTEM_MENUSTART
             * EVENT_SYSTEM_MENUEND
             * For MENUSTART, hwnd+idObject+idChild refers to the control with the menu bar,
             *  or the control bringing up the context menu.
             *
             * Sent when entering into and leaving from menu mode (system, app bar, and
             * track popups).
             */
            public const int EVENT_SYSTEM_MENUSTART          = 0x0004;
            public const int EVENT_SYSTEM_MENUEND            = 0x0005;

            /*
             * EVENT_SYSTEM_MENUPOPUPSTART
             * EVENT_SYSTEM_MENUPOPUPEND
             * Sent when a menu popup comes up and just before it is taken down.  Note
             * that for a call to TrackPopupMenu(), a client will see EVENT_SYSTEM_MENUSTART
             * followed almost immediately by EVENT_SYSTEM_MENUPOPUPSTART for the popup
             * being shown.
             *
             * For MENUPOPUP, hwnd+idObject+idChild refers to the NEW popup coming up, not the
             * parent item which is hierarchical.  You can get the parent menu/popup by
             * asking for the accParent object.
             */
            public const int EVENT_SYSTEM_MENUPOPUPSTART     = 0x0006;
            public const int EVENT_SYSTEM_MENUPOPUPEND       = 0x0007;


            /*
             * EVENT_SYSTEM_CAPTURESTART
             * EVENT_SYSTEM_CAPTUREEND
             * Sent when a window takes the capture and releases the capture.
             */
            public const int EVENT_SYSTEM_CAPTURESTART       = 0x0008;
            public const int EVENT_SYSTEM_CAPTUREEND         = 0x0009;

            /*
             * Move Size
             * EVENT_SYSTEM_MOVESIZESTART
             * EVENT_SYSTEM_MOVESIZEEND
             * Sent when a window enters and leaves move-size dragging mode.
             */
            public const int EVENT_SYSTEM_MOVESIZESTART      = 0x000A;
            public const int EVENT_SYSTEM_MOVESIZEEND        = 0x000B;

            /*
             * Context Help
             * EVENT_SYSTEM_CONTEXTHELPSTART
             * EVENT_SYSTEM_CONTEXTHELPEND
             * Sent when a window enters and leaves context sensitive help mode.
             */
            public const int EVENT_SYSTEM_CONTEXTHELPSTART   = 0x000C;
            public const int EVENT_SYSTEM_CONTEXTHELPEND     = 0x000D;

            /*
             * Drag & Drop
             * EVENT_SYSTEM_DRAGDROPSTART
             * EVENT_SYSTEM_DRAGDROPEND
             * Send the START notification just before going into drag&drop loop.  Send
             * the END notification just after canceling out.
             * Note that it is up to apps and OLE to generate this, since the system
             * doesn't know.  Like EVENT_SYSTEM_SOUND, it will be a while before this
             * is prevalent.
             */
            public const int EVENT_SYSTEM_DRAGDROPSTART      = 0x000E;
            public const int EVENT_SYSTEM_DRAGDROPEND        = 0x000F;

            /*
             * Dialog
             * Send the START notification right after the dialog is completely
             *  initialized and visible.  Send the END right before the dialog
             *  is hidden and goes away.
             * EVENT_SYSTEM_DIALOGSTART
             * EVENT_SYSTEM_DIALOGEND
             */
            public const int EVENT_SYSTEM_DIALOGSTART        = 0x0010;
            public const int EVENT_SYSTEM_DIALOGEND          = 0x0011;

            /*
             * EVENT_SYSTEM_SCROLLING
             * EVENT_SYSTEM_SCROLLINGSTART
             * EVENT_SYSTEM_SCROLLINGEND
             * Sent when beginning and ending the tracking of a scrollbar in a window,
             * and also for scrollbar controls.
             */
            public const int EVENT_SYSTEM_SCROLLINGSTART     = 0x0012;
            public const int EVENT_SYSTEM_SCROLLINGEND       = 0x0013;

            /*
             * Alt-Tab Window
             * Send the START notification right after the switch window is initialized
             * and visible.  Send the END right before it is hidden and goes away.
             * EVENT_SYSTEM_SWITCHSTART
             * EVENT_SYSTEM_SWITCHEND
             */
            public const int EVENT_SYSTEM_SWITCHSTART        = 0x0014;
            public const int EVENT_SYSTEM_SWITCHEND          = 0x0015;

            /*
             * EVENT_SYSTEM_MINIMIZESTART
             * EVENT_SYSTEM_MINIMIZEEND
             * Sent when a window minimizes and just before it restores.
             */
            public const int EVENT_SYSTEM_MINIMIZESTART      = 0x0016;
            public const int EVENT_SYSTEM_MINIMIZEEND        = 0x0017;


            public const int EVENT_SYSTEM_DESKTOPSWITCH      = 0x0020;

            public const int EVENT_CONSOLE_CARET             = 0x4001;
            public const int EVENT_CONSOLE_UPDATE_REGION     = 0x4002;
            public const int EVENT_CONSOLE_UPDATE_SIMPLE     = 0x4003;
            public const int EVENT_CONSOLE_UPDATE_SCROLL     = 0x4004;
            public const int EVENT_CONSOLE_LAYOUT            = 0x4005;
            public const int EVENT_CONSOLE_START_APPLICATION = 0x4006;
            public const int EVENT_CONSOLE_END_APPLICATION   = 0x4007;

            /*
             * Flags for EVENT_CONSOLE_START/END_APPLICATION.
             */
          //public const int CONSOLE_APPLICATION_16BIT       = 0x0000; // _WIN64
            public const int CONSOLE_APPLICATION_16BIT       = 0x0001; // not _WIN64

            /*
             * Flags for EVENT_CONSOLE_CARET
             */
            public const int CONSOLE_CARET_SELECTION         = 0x0001;
            public const int CONSOLE_CARET_VISIBLE           = 0x0002;

            /*
             * Object events
             *
             * The system AND apps generate these.  The system generates these for
             * real windows.  Apps generate these for objects within their window which
             * act like a separate control, e.g. an item in a list view.
             *
             * When the system generate them, dwParam2 is always WMOBJID_SELF.  When
             * apps generate them, apps put the has-meaning-to-the-app-only ID value
             * in dwParam2.
             * For all events, if you want detailed accessibility information, callers
             * should
             *      * Call AccessibleObjectFromWindow() with the hwnd, idObject parameters
             *          of the event, and IID_IAccessible as the REFIID, to get back an
             *          IAccessible* to talk to
             *      * Initialize and fill in a VARIANT as VT_I4 with lVal the idChild
             *          parameter of the event.
             *      * If idChild isn't zero, call get_accChild() in the container to see
             *          if the child is an object in its own right.  If so, you will get
             *          back an IDispatch* object for the child.  You should release the
             *          parent, and call QueryInterface() on the child object to get its
             *          IAccessible*.  Then you talk directly to the child.  Otherwise,
             *          if get_accChild() returns you nothing, you should continue to
             *          use the child VARIANT.  You will ask the container for the properties
             *          of the child identified by the VARIANT.  In other words, the
             *          child in this case is accessible but not a full-blown object.
             *          Like a button on a titlebar which is 'small' and has no children.
             */

            /*
             * For all EVENT_OBJECT events,
             *      hwnd is the dude to Send the WM_GETOBJECT message to (unless NULL,
             *          see above for system things)
             *      idObject is the ID of the object that can resolve any queries a
             *          client might have.  It's a way to deal with windowless controls,
             *          controls that are just drawn on the screen in some larger parent
             *          window (like SDM), or standard frame elements of a window.
             *      idChild is the piece inside of the object that is affected.  This
             *          allows clients to access things that are too small to have full
             *          blown objects in their own right.  Like the thumb of a scrollbar.
             *          The hwnd/idObject pair gets you to the container, the dude you
             *          probably want to talk to most of the time anyway.  The idChild
             *          can then be passed into the acc properties to get the name/value
             *          of it as needed.
             *
             * Example #1:
             *      System propagating a listbox selection change
             *      EVENT_OBJECT_SELECTION
             *          hwnd == listbox hwnd
             *          idObject == OBJID_WINDOW
             *          idChild == new selected item, or CHILDID_SELF if
             *              nothing now selected within container.
             *      Word '97 propagating a listbox selection change
             *          hwnd == SDM window
             *          idObject == SDM ID to get at listbox 'control'
             *          idChild == new selected item, or CHILDID_SELF if
             *              nothing
             *
             * Example #2:
             *      System propagating a menu item selection on the menu bar
             *      EVENT_OBJECT_SELECTION
             *          hwnd == top level window
             *          idObject == OBJID_MENU
             *          idChild == ID of child menu bar item selected
             *
             * Example #3:
             *      System propagating a dropdown coming off of said menu bar item
             *      EVENT_OBJECT_CREATE
             *          hwnd == popup item
             *          idObject == OBJID_WINDOW
             *          idChild == CHILDID_SELF
             *
             * Example #4:
             *
             * For EVENT_OBJECT_REORDER, the object referred to by hwnd/idObject is the
             * PARENT container in which the zorder is occurring.  This is because if
             * one child is zordering, all of them are changing their relative zorder.
             */
            public const int EVENT_OBJECT_CREATE                 = 0x8000;  // hwnd + ID + idChild is created item
            public const int EVENT_OBJECT_DESTROY                = 0x8001;  // hwnd + ID + idChild is destroyed item
            public const int EVENT_OBJECT_SHOW                   = 0x8002;  // hwnd + ID + idChild is shown item
            public const int EVENT_OBJECT_HIDE                   = 0x8003;  // hwnd + ID + idChild is hidden item
            public const int EVENT_OBJECT_REORDER                = 0x8004;  // hwnd + ID + idChild is parent of zordering children

            /*
             * NOTE:
             * Minimize the number of notifications!
             *
             * When you are hiding a parent object, obviously all child objects are no
             * longer visible on screen.  They still have the same "visible" status,
             * but are not truly visible.  Hence do not send HIDE notifications for the
             * children also.  One implies all.  The same goes for SHOW.
             */

            public const int EVENT_OBJECT_FOCUS                  = 0x8005;  // hwnd + ID + idChild is focused item
            public const int EVENT_OBJECT_SELECTION              = 0x8006;  // hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
            public const int EVENT_OBJECT_SELECTIONADD           = 0x8007;  // hwnd + ID + idChild is item added
            public const int EVENT_OBJECT_SELECTIONREMOVE        = 0x8008;  // hwnd + ID + idChild is item removed
            public const int EVENT_OBJECT_SELECTIONWITHIN        = 0x8009;  // hwnd + ID + idChild is parent of changed selected items

            /*
             * NOTES:
             * There is only one "focused" child item in a parent.  This is the place
             * keystrokes are going at a given moment.  Hence only send a notification
             * about where the NEW focus is going.  A NEW item getting the focus already
             * implies that the OLD item is losing it.
             *
             * SELECTION however can be multiple.  Hence the different SELECTION
             * notifications.  Here's when to use each:
             *
             * (1) Send a SELECTION notification in the simple single selection
             *     case (like the focus) when the item with the selection is
             *     merely moving to a different item within a container.  hwnd + ID
             *     is the container control, idChildItem is the new child with the
             *     selection.
             *
             * (2) Send a SELECTIONADD notification when a new item has simply been added
             *     to the selection within a container.  This is appropriate when the
             *     number of newly selected items is very small.  hwnd + ID is the
             *     container control, idChildItem is the new child added to the selection.
             *
             * (3) Send a SELECTIONREMOVE notification when a new item has simply been
             *     removed from the selection within a container.  This is appropriate
             *     when the number of newly selected items is very small, just like
             *     SELECTIONADD.  hwnd + ID is the container control, idChildItem is the
             *     new child removed from the selection.
             *
             * (4) Send a SELECTIONWITHIN notification when the selected items within a
             *     control have changed substantially.  Rather than propagate a large
             *     number of changes to reflect removal for some items, addition of
             *     others, just tell somebody who cares that a lot happened.  It will
             *     be faster an easier for somebody watching to just turn around and
             *     query the container control what the new bunch of selected items
             *     are.
             */

            public const int EVENT_OBJECT_STATECHANGE            = 0x800A;  // hwnd + ID + idChild is item w/ state change

            /*
             * Examples of when to send an EVENT_OBJECT_STATECHANGE include
             *      * It is being enabled/disabled (USER does for windows)
             *      * It is being pressed/released (USER does for buttons)
             *      * It is being checked/unchecked (USER does for radio/check buttons)
             */
            public const int EVENT_OBJECT_LOCATIONCHANGE         = 0x800B;  // hwnd + ID + idChild is moved/sized item

            /*
             * Note:
             * A LOCATIONCHANGE is not sent for every child object when the parent
             * changes shape/moves.  Send one notification for the topmost object
             * that is changing.  For example, if the user resizes a top level window,
             * USER will generate a LOCATIONCHANGE for it, but not for the menu bar,
             * title bar, scrollbars, etc.  that are also changing shape/moving.
             *
             * In other words, it only generates LOCATIONCHANGE notifications for
             * real windows that are moving/sizing.  It will not generate a LOCATIONCHANGE
             * for every non-floating child window when the parent moves (the children are
             * logically moving also on screen, but not relative to the parent).
             *
             * Now, if the app itself resizes child windows as a result of being
             * sized, USER will generate LOCATIONCHANGEs for those dudes also because
             * it doesn't know better.
             *
             * Note also that USER will generate LOCATIONCHANGE notifications for two
             * non-window sys objects:
             *      (1) System caret
             *      (2) Cursor
             */

            public const int EVENT_OBJECT_NAMECHANGE             = 0x800C;  // hwnd + ID + idChild is item w/ name change
            public const int EVENT_OBJECT_DESCRIPTIONCHANGE      = 0x800D;  // hwnd + ID + idChild is item w/ desc change
            public const int EVENT_OBJECT_VALUECHANGE            = 0x800E;  // hwnd + ID + idChild is item w/ value change
            public const int EVENT_OBJECT_PARENTCHANGE           = 0x800F;  // hwnd + ID + idChild is item w/ new parent
            public const int EVENT_OBJECT_HELPCHANGE             = 0x8010;  // hwnd + ID + idChild is item w/ help change
            public const int EVENT_OBJECT_DEFACTIONCHANGE        = 0x8011;  // hwnd + ID + idChild is item w/ def action change
            public const int EVENT_OBJECT_ACCELERATORCHANGE      = 0x8012;  // hwnd + ID + idChild is item w/ keybd accel change
            public const int EVENT_OBJECT_INVOKED                = 0x8013;  // hwnd + ID + idChild is item invoked
            public const int EVENT_OBJECT_TEXTSELECTIONCHANGED   = 0x8014;  // hwnd + ID + idChild is item w? test selection change

            /*
             * EVENT_OBJECT_CONTENTSCROLLED
             * Sent when ending the scrolling of a window object.
             *
             * Unlike the similar event (EVENT_SYSTEM_SCROLLEND), this event will be
             * associated with the scrolling window itself. There is no difference
             * between horizontal or vertical scrolling.
             *
             * This event should be posted whenever scroll action is completed, including
             * when it is scrolled by scroll bars, mouse wheel, or keyboard navigations.
             *
             *   example:
             *          hwnd == window that is scrolling
             *          idObject == OBJID_CLIENT
             *          idChild == CHILDID_SELF
             */
            public const int EVENT_OBJECT_CONTENTSCROLLED        = 0x8015;

            /*
             * Child IDs
             */

            /*
             * System Sounds (idChild of system SOUND notification)
             */
            public const int SOUND_SYSTEM_STARTUP            = 1;
            public const int SOUND_SYSTEM_SHUTDOWN           = 2;
            public const int SOUND_SYSTEM_BEEP               = 3;
            public const int SOUND_SYSTEM_ERROR              = 4;
            public const int SOUND_SYSTEM_QUESTION           = 5;
            public const int SOUND_SYSTEM_WARNING            = 6;
            public const int SOUND_SYSTEM_INFORMATION        = 7;
            public const int SOUND_SYSTEM_MAXIMIZE           = 8;
            public const int SOUND_SYSTEM_MINIMIZE           = 9;
            public const int SOUND_SYSTEM_RESTOREUP          = 10;
            public const int SOUND_SYSTEM_RESTOREDOWN        = 11;
            public const int SOUND_SYSTEM_APPSTART           = 12;
            public const int SOUND_SYSTEM_FAULT              = 13;
            public const int SOUND_SYSTEM_APPEND             = 14;
            public const int SOUND_SYSTEM_MENUCOMMAND        = 15;
            public const int SOUND_SYSTEM_MENUPOPUP          = 16;
            public const int CSOUND_SYSTEM                   = 16;

            /*
             * System Alerts (indexChild of system ALERT notification)
             */
            public const int ALERT_SYSTEM_INFORMATIONAL      = 1;       // MB_INFORMATION
            public const int ALERT_SYSTEM_WARNING            = 2;       // MB_WARNING
            public const int ALERT_SYSTEM_ERROR              = 3;       // MB_ERROR
            public const int ALERT_SYSTEM_QUERY              = 4;       // MB_QUESTION
            public const int ALERT_SYSTEM_CRITICAL           = 5;       // HardSysErrBox
            public const int CALERT_SYSTEM                   = 6;

            public const int GUI_CARETBLINKING   = 0x00000001;
            public const int GUI_INMOVESIZE      = 0x00000002;
            public const int GUI_INMENUMODE      = 0x00000004;
            public const int GUI_SYSTEMMENUMODE  = 0x00000008;
            public const int GUI_POPUPMENUMODE   = 0x00000010;
          //public const int GUI_16BITTASK       = 0x00000000; // _WIN64
            public const int GUI_16BITTASK       = 0x00000020; // not _WIN64

            public const int USER_DEFAULT_SCREEN_DPI = 96;

            public const int STATE_SYSTEM_UNAVAILABLE        = 0x00000001;  // Disabled
            public const int STATE_SYSTEM_SELECTED           = 0x00000002;
            public const int STATE_SYSTEM_FOCUSED            = 0x00000004;
            public const int STATE_SYSTEM_PRESSED            = 0x00000008;
            public const int STATE_SYSTEM_CHECKED            = 0x00000010;
            public const int STATE_SYSTEM_MIXED              = 0x00000020;  // 3-state checkbox or toolbar button
            public const int STATE_SYSTEM_INDETERMINATE      = STATE_SYSTEM_MIXED;
            public const int STATE_SYSTEM_READONLY           = 0x00000040;
            public const int STATE_SYSTEM_HOTTRACKED         = 0x00000080;
            public const int STATE_SYSTEM_DEFAULT            = 0x00000100;
            public const int STATE_SYSTEM_EXPANDED           = 0x00000200;
            public const int STATE_SYSTEM_COLLAPSED          = 0x00000400;
            public const int STATE_SYSTEM_BUSY               = 0x00000800;
            public const int STATE_SYSTEM_FLOATING           = 0x00001000;  // Children "owned" not "contained" by parent
            public const int STATE_SYSTEM_MARQUEED           = 0x00002000;
            public const int STATE_SYSTEM_ANIMATED           = 0x00004000;
            public const int STATE_SYSTEM_INVISIBLE          = 0x00008000;
            public const int STATE_SYSTEM_OFFSCREEN          = 0x00010000;
            public const int STATE_SYSTEM_SIZEABLE           = 0x00020000;
            public const int STATE_SYSTEM_MOVEABLE           = 0x00040000;
            public const int STATE_SYSTEM_SELFVOICING        = 0x00080000;
            public const int STATE_SYSTEM_FOCUSABLE          = 0x00100000;
            public const int STATE_SYSTEM_SELECTABLE         = 0x00200000;
            public const int STATE_SYSTEM_LINKED             = 0x00400000;
            public const int STATE_SYSTEM_TRAVERSED          = 0x00800000;
            public const int STATE_SYSTEM_MULTISELECTABLE    = 0x01000000;  // Supports multiple selection
            public const int STATE_SYSTEM_EXTSELECTABLE      = 0x02000000;  // Supports extended selection
            public const int STATE_SYSTEM_ALERT_LOW          = 0x04000000;  // This information is of low priority
            public const int STATE_SYSTEM_ALERT_MEDIUM       = 0x08000000;  // This information is of medium priority
            public const int STATE_SYSTEM_ALERT_HIGH         = 0x10000000;  // This information is of high priority
            public const int STATE_SYSTEM_PROTECTED          = 0x20000000;  // access to this is restricted
            public const int STATE_SYSTEM_VALID              = 0x3FFFFFFF;

            public const int CCHILDREN_TITLEBAR              = 5;
            public const int CCHILDREN_SCROLLBAR             = 5;

            public const int CURSOR_SHOWING     = 0x00000001;

            public const int WS_ACTIVECAPTION    = 0x0001;

            /*
             * The "real" ancestor window
             */
            public const int GA_PARENT       = 1;
            public const int GA_ROOT         = 2;
            public const int GA_ROOTOWNER    = 3;

            /*
             * Raw Input Messages.
             */


            /*
             * WM_INPUT wParam
             */


            /*
             * The input is in the regular message flow,
             * the app is required to call DefWindowProc
             * so that the system can perform clean ups.
             */
            public const int RIM_INPUT       = 0;

            /*
             * The input is sink only. The app is expected
             * to behave nicely.
             */
            public const int RIM_INPUTSINK   = 1;

            /*
             * Type of the raw input
             */
            public const int RIM_TYPEMOUSE       = 0;
            public const int RIM_TYPEKEYBOARD    = 1;
            public const int RIM_TYPEHID         = 2;

            /*
             * Define the mouse button state indicators.
             */

            public const int RI_MOUSE_LEFT_BUTTON_DOWN   = 0x0001;  // Left Button changed to down.
            public const int RI_MOUSE_LEFT_BUTTON_UP     = 0x0002;  // Left Button changed to up.
            public const int RI_MOUSE_RIGHT_BUTTON_DOWN  = 0x0004;  // Right Button changed to down.
            public const int RI_MOUSE_RIGHT_BUTTON_UP    = 0x0008;  // Right Button changed to up.
            public const int RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010;  // Middle Button changed to down.
            public const int RI_MOUSE_MIDDLE_BUTTON_UP   = 0x0020;  // Middle Button changed to up.

            public const int RI_MOUSE_BUTTON_1_DOWN      = RI_MOUSE_LEFT_BUTTON_DOWN;
            public const int RI_MOUSE_BUTTON_1_UP        = RI_MOUSE_LEFT_BUTTON_UP;
            public const int RI_MOUSE_BUTTON_2_DOWN      = RI_MOUSE_RIGHT_BUTTON_DOWN;
            public const int RI_MOUSE_BUTTON_2_UP        = RI_MOUSE_RIGHT_BUTTON_UP;
            public const int RI_MOUSE_BUTTON_3_DOWN      = RI_MOUSE_MIDDLE_BUTTON_DOWN;
            public const int RI_MOUSE_BUTTON_3_UP        = RI_MOUSE_MIDDLE_BUTTON_UP;

            public const int RI_MOUSE_BUTTON_4_DOWN      = 0x0040;
            public const int RI_MOUSE_BUTTON_4_UP        = 0x0080;
            public const int RI_MOUSE_BUTTON_5_DOWN      = 0x0100;
            public const int RI_MOUSE_BUTTON_5_UP        = 0x0200;

            /*
             * If usButtonFlags has RI_MOUSE_WHEEL, the wheel delta is stored in usButtonData.
             * Take it as a signed value.
             */
            public const int RI_MOUSE_WHEEL              = 0x0400;

            /*
             * Define the mouse indicator flags.
             */
            public const int MOUSE_MOVE_RELATIVE         = 0;
            public const int MOUSE_MOVE_ABSOLUTE         = 1;
            public const int MOUSE_VIRTUAL_DESKTOP    = 0x02;  // the coordinates are mapped to the virtual desktop
            public const int MOUSE_ATTRIBUTES_CHANGED = 0x04;  // requery for mouse attributes
            public const int MOUSE_MOVE_NOCOALESCE    = 0x08;  // do not coalesce mouse moves

            /*
             * Define the keyboard overrun MakeCode.
             */

            public const int KEYBOARD_OVERRUN_MAKE_CODE    = 0xFF;

            /*
             * Define the keyboard input data Flags.
             */
            public const int RI_KEY_MAKE             = 0;
            public const int RI_KEY_BREAK            = 1;
            public const int RI_KEY_E0               = 2;
            public const int RI_KEY_E1               = 4;
            public const int RI_KEY_TERMSRV_SET_LED  = 8;
            public const int RI_KEY_TERMSRV_SHADOW   = 0x10;

            /*
             * Flags for GetRawInputData
             */

            public const int RID_INPUT               = 0x10000003;
            public const int RID_HEADER              = 0x10000005;

            /*
             * Raw Input Device Information
             */
            public const int RIDI_PREPARSEDDATA      = 0x20000005;
            public const int RIDI_DEVICENAME         = 0x20000007;  // the return valus is the character length, not the byte size
            public const int RIDI_DEVICEINFO         = 0x2000000b;

            public const int RIDEV_REMOVE            = 0x00000001;
            public const int RIDEV_EXCLUDE           = 0x00000010;
            public const int RIDEV_PAGEONLY          = 0x00000020;
            public const int RIDEV_NOLEGACY          = 0x00000030;
            public const int RIDEV_INPUTSINK         = 0x00000100;
            public const int RIDEV_CAPTUREMOUSE      = 0x00000200;  // effective when mouse nolegacy is specified, otherwise it would be an error
            public const int RIDEV_NOHOTKEYS         = 0x00000200;  // effective for keyboard.
            public const int RIDEV_APPKEYS           = 0x00000400;  // effective for keyboard.
            public const int RIDEV_EXINPUTSINK       = 0x00001000;
            public const int RIDEV_DEVNOTIFY         = 0x00002000;
            public const int RIDEV_EXMODEMASK        = 0x000000F0;

            //#define RIDEV_EXMODE(mode)  ((mode) & RIDEV_EXMODEMASK)

            /*
             * Flags for the WM_INPUT_DEVICE_CHANGE message.
             */
            public const int GIDC_ARRIVAL             = 1;
            public const int GIDC_REMOVAL             = 2;
            //#define GET_DEVICE_CHANGE_LPARAM(lParam)  (LOWORD(lParam))

            public const int MSGFLT_ADD = 1;
            public const int MSGFLT_REMOVE = 2;

            public const int MAX_STR_BLOCKREASON = 256;

        }
        #endregion

        #region class Const (Dde.h)
        public abstract partial class Const
        {
            // Dde.h (c:\Program Files\Microsoft SDKs\Windows\v6.1\Include\)

            //#include <windef.h>

            /* DDE window messages */

            public const int WM_DDE_FIRST	    = 0x03E0;
            public const int WM_DDE_INITIATE     = (WM_DDE_FIRST);
            public const int WM_DDE_TERMINATE    = (WM_DDE_FIRST+1);
            public const int WM_DDE_ADVISE	    = (WM_DDE_FIRST+2);
            public const int WM_DDE_UNADVISE     = (WM_DDE_FIRST+3);
            public const int WM_DDE_ACK	        = (WM_DDE_FIRST+4);
            public const int WM_DDE_DATA	        = (WM_DDE_FIRST+5);
            public const int WM_DDE_REQUEST	    = (WM_DDE_FIRST+6);
            public const int WM_DDE_POKE	        = (WM_DDE_FIRST+7);
            public const int WM_DDE_EXECUTE	    = (WM_DDE_FIRST+8);
            public const int WM_DDE_LAST	        = (WM_DDE_FIRST+8);

        }
        #endregion

        #region class Const (CommDlg.h)
        public abstract partial class Const
        {
            // CommDlg.h (c:\Program Files\Microsoft SDKs\Windows\v6.1\Include\)

			//#include <prsht.h>
			//#include <pshpack1.h>         /* Assume byte packing throughout */

			public const int OFN_READONLY                 = 0x00000001;
			public const int OFN_OVERWRITEPROMPT          = 0x00000002;
			public const int OFN_HIDEREADONLY             = 0x00000004;
			public const int OFN_NOCHANGEDIR              = 0x00000008;
			public const int OFN_SHOWHELP                 = 0x00000010;
			public const int OFN_ENABLEHOOK               = 0x00000020;
			public const int OFN_ENABLETEMPLATE           = 0x00000040;
			public const int OFN_ENABLETEMPLATEHANDLE     = 0x00000080;
			public const int OFN_NOVALIDATE               = 0x00000100;
			public const int OFN_ALLOWMULTISELECT         = 0x00000200;
			public const int OFN_EXTENSIONDIFFERENT       = 0x00000400;
			public const int OFN_PATHMUSTEXIST            = 0x00000800;
			public const int OFN_FILEMUSTEXIST            = 0x00001000;
			public const int OFN_CREATEPROMPT             = 0x00002000;
			public const int OFN_SHAREAWARE               = 0x00004000;
			public const int OFN_NOREADONLYRETURN         = 0x00008000;
			public const int OFN_NOTESTFILECREATE         = 0x00010000;
			public const int OFN_NONETWORKBUTTON          = 0x00020000;
			public const int OFN_NOLONGNAMES              = 0x00040000;     // force no long names for 4.x modules
			public const int OFN_EXPLORER                 = 0x00080000;     // new look commdlg
			public const int OFN_NODEREFERENCELINKS       = 0x00100000;
			public const int OFN_LONGNAMES                = 0x00200000;     // force long names for 3.x modules
			// OFN_ENABLEINCLUDENOTIFY and OFN_ENABLESIZING require
			// Windows 2000 or higher to have any effect.
			public const int OFN_ENABLEINCLUDENOTIFY      = 0x00400000;     // send include message to callback
			public const int OFN_ENABLESIZING             = 0x00800000;
			public const int OFN_DONTADDTORECENT          = 0x02000000;
			public const int OFN_FORCESHOWHIDDEN          = 0x10000000;    // Show All files including System and hidden files

			//FlagsEx Values
			public const int OFN_EX_NOPLACESBAR         = 0x00000001;

			// Return values for the registered message sent to the hook function
			// when a sharing violation occurs.  OFN_SHAREFALLTHROUGH allows the
			// filename to be accepted, OFN_SHARENOWARN rejects the name but puts
			// up no warning (returned when the app has already put up a warning
			// message), and OFN_SHAREWARN puts up the default warning message
			// for sharing violations.
			//
			// Note:  Undefined return values map to OFN_SHAREWARN, but are
			//        reserved for future use.

			public const int OFN_SHAREFALLTHROUGH     = 2;
			public const int OFN_SHARENOWARN          = 1;
			public const int OFN_SHAREWARN            = 0;


            //public const uint CDN_FIRST   = (0U-601U);
            //public const uint CDN_LAST    = (0U-699U);

			// Notifications from Open or Save dialog
            //public const uint CDN_INITDONE            = (CDN_FIRST - 0x0000);
            //public const uint CDN_SELCHANGE           = (CDN_FIRST - 0x0001);
            //public const uint CDN_FOLDERCHANGE        = (CDN_FIRST - 0x0002);
            //public const uint CDN_SHAREVIOLATION      = (CDN_FIRST - 0x0003);
            //public const uint CDN_HELP                = (CDN_FIRST - 0x0004);
            //public const uint CDN_FILEOK              = (CDN_FIRST - 0x0005);
            //public const uint CDN_TYPECHANGE          = (CDN_FIRST - 0x0006);
            //public const uint CDN_INCLUDEITEM         = (CDN_FIRST - 0x0007);

			public const int CDM_FIRST       = ((int)WM.WM_USER + 100);
			public const int CDM_LAST        = ((int)WM.WM_USER + 200);

			// Messages to query information from the Open or Save dialogs

			// lParam = pointer to text buffer that gets filled in
			// wParam = max number of characters of the text buffer (including NULL)
			// return = < 0 if error; number of characters needed (including NULL)
			public const int CDM_GETSPEC             = (CDM_FIRST + 0x0000);

			// lParam = pointer to text buffer that gets filled in
			// wParam = max number of characters of the text buffer (including NULL)
			// return = < 0 if error; number of characters needed (including NULL)
			public const int CDM_GETFILEPATH         = (CDM_FIRST + 0x0001);

			// lParam = pointer to text buffer that gets filled in
			// wParam = max number of characters of the text buffer (including NULL)
			// return = < 0 if error; number of characters needed (including NULL)
			public const int CDM_GETFOLDERPATH       = (CDM_FIRST + 0x0002);

			// lParam = pointer to ITEMIDLIST buffer that gets filled in
			// wParam = size of the ITEMIDLIST buffer
			// return = < 0 if error; length of buffer needed
			public const int CDM_GETFOLDERIDLIST     = (CDM_FIRST + 0x0003);

			// lParam = pointer to a string
			// wParam = ID of control to change
			// return = not used
			public const int CDM_SETCONTROLTEXT      = (CDM_FIRST + 0x0004);

			// lParam = not used
			// wParam = ID of control to change
			// return = not used
			public const int CDM_HIDECONTROL         = (CDM_FIRST + 0x0005);

			// lParam = pointer to default extension (no dot)
			// wParam = not used
			// return = not used
			public const int CDM_SETDEFEXT           = (CDM_FIRST + 0x0006);

			public const int CC_RGBINIT               = 0x00000001;
			public const int CC_FULLOPEN              = 0x00000002;
			public const int CC_PREVENTFULLOPEN       = 0x00000004;
			public const int CC_SHOWHELP              = 0x00000008;
			public const int CC_ENABLEHOOK            = 0x00000010;
			public const int CC_ENABLETEMPLATE        = 0x00000020;
			public const int CC_ENABLETEMPLATEHANDLE  = 0x00000040;
			public const int CC_SOLIDCOLOR            = 0x00000080;
			public const int CC_ANYCOLOR              = 0x00000100;

			public const uint FR_DOWN                         = 0x00000001;
			public const uint FR_WHOLEWORD                    = 0x00000002;
			public const uint FR_MATCHCASE                    = 0x00000004;
			public const uint FR_FINDNEXT                     = 0x00000008;
			public const uint FR_REPLACE                      = 0x00000010;
			public const uint FR_REPLACEALL                   = 0x00000020;
			public const uint FR_DIALOGTERM                   = 0x00000040;
			public const uint FR_SHOWHELP                     = 0x00000080;
			public const uint FR_ENABLEHOOK                   = 0x00000100;
			public const uint FR_ENABLETEMPLATE               = 0x00000200;
			public const uint FR_NOUPDOWN                     = 0x00000400;
			public const uint FR_NOMATCHCASE                  = 0x00000800;
			public const uint FR_NOWHOLEWORD                  = 0x00001000;
			public const uint FR_ENABLETEMPLATEHANDLE         = 0x00002000;
			public const uint FR_HIDEUPDOWN                   = 0x00004000;
			public const uint FR_HIDEMATCHCASE                = 0x00008000;
			public const uint FR_HIDEWHOLEWORD                = 0x00010000;
			public const uint FR_RAW                          = 0x00020000;
			public const uint FR_MATCHDIAC                    = 0x20000000;
			public const uint FR_MATCHKASHIDA                 = 0x40000000;
			public const uint FR_MATCHALEFHAMZA               = 0x80000000;

			public const int CF_SCREENFONTS             = 0x00000001;
			public const int CF_PRINTERFONTS            = 0x00000002;
			public const int CF_BOTH                    = (CF_SCREENFONTS | CF_PRINTERFONTS);
			public const int CF_SHOWHELP                = 0x00000004;
			public const int CF_ENABLEHOOK              = 0x00000008;
			public const int CF_ENABLETEMPLATE          = 0x00000010;
			public const int CF_ENABLETEMPLATEHANDLE    = 0x00000020;
			public const int CF_INITTOLOGFONTSTRUCT     = 0x00000040;
			public const int CF_USESTYLE                = 0x00000080;
			public const int CF_EFFECTS                 = 0x00000100;
			public const int CF_APPLY                   = 0x00000200;
			public const int CF_ANSIONLY                = 0x00000400;
			public const int CF_SCRIPTSONLY             = CF_ANSIONLY;
			public const int CF_NOVECTORFONTS           = 0x00000800;
			public const int CF_NOOEMFONTS              = CF_NOVECTORFONTS;
			public const int CF_NOSIMULATIONS           = 0x00001000;
			public const int CF_LIMITSIZE               = 0x00002000;
			public const int CF_FIXEDPITCHONLY          = 0x00004000;
			public const int CF_WYSIWYG                 = 0x00008000; // must also have CF_SCREENFONTS & CF_PRINTERFONTS
			public const int CF_FORCEFONTEXIST          = 0x00010000;
			public const int CF_SCALABLEONLY            = 0x00020000;
			public const int CF_TTONLY                  = 0x00040000;
			public const int CF_NOFACESEL               = 0x00080000;
			public const int CF_NOSTYLESEL              = 0x00100000;
			public const int CF_NOSIZESEL               = 0x00200000;
			public const int CF_SELECTSCRIPT            = 0x00400000;
			public const int CF_NOSCRIPTSEL             = 0x00800000;
			public const int CF_NOVERTFONTS             = 0x01000000;

			// these are extra nFontType bits that are added to what is returned to the
			// EnumFonts callback routine

			public const int SIMULATED_FONTTYPE    = 0x8000;
			public const int PRINTER_FONTTYPE      = 0x4000;
			public const int SCREEN_FONTTYPE       = 0x2000;
			public const int BOLD_FONTTYPE         = 0x0100;
			public const int ITALIC_FONTTYPE       = 0x0200;
			public const int REGULAR_FONTTYPE      = 0x0400;

			// EnumFonts callback routine only uses these bits, so we can use the rest

			// #define RASTER_FONTTYPE     0x001
			// #define DEVICE_FONTTYPE     0x002
			// #define TRUETYPE_FONTTYPE   0x004

			public const int PS_OPENTYPE_FONTTYPE  = 0x10000;
			public const int TT_OPENTYPE_FONTTYPE  = 0x20000;
			public const int TYPE1_FONTTYPE        = 0x40000;

			public const int WM_CHOOSEFONT_GETLOGFONT      = ((int)WM.WM_USER + 1);
			public const int WM_CHOOSEFONT_SETLOGFONT      = ((int)WM.WM_USER + 101);
			public const int WM_CHOOSEFONT_SETFLAGS        = ((int)WM.WM_USER + 102);

			// strings used to obtain unique window message for communication
			// between dialog and caller

            //#define LBSELCHSTRINGA  "commdlg_LBSelChangedNotify"
            //#define SHAREVISTRINGA  "commdlg_ShareViolation"
            //#define FILEOKSTRINGA   "commdlg_FileNameOK"
            //#define COLOROKSTRINGA  "commdlg_ColorOK"
            //#define SETRGBSTRINGA   "commdlg_SetRGBColor"
            //#define HELPMSGSTRINGA  "commdlg_help"
            //#define FINDMSGSTRINGA  "commdlg_FindReplace"

            //#define LBSELCHSTRINGW  L"commdlg_LBSelChangedNotify"
            //#define SHAREVISTRINGW  L"commdlg_ShareViolation"
            //#define FILEOKSTRINGW   L"commdlg_FileNameOK"
            //#define COLOROKSTRINGW  L"commdlg_ColorOK"
            //#define SETRGBSTRINGW   L"commdlg_SetRGBColor"
            //#define HELPMSGSTRINGW  L"commdlg_help"
            //#define FINDMSGSTRINGW  L"commdlg_FindReplace"

            //#ifdef UNICODE
            //#define LBSELCHSTRING  LBSELCHSTRINGW
            //#define SHAREVISTRING  SHAREVISTRINGW
            //#define FILEOKSTRING   FILEOKSTRINGW
            //#define COLOROKSTRING  COLOROKSTRINGW
            //#define SETRGBSTRING   SETRGBSTRINGW
            //#define HELPMSGSTRING  HELPMSGSTRINGW
            //#define FINDMSGSTRING  FINDMSGSTRINGW
            //#else
            //#define LBSELCHSTRING  LBSELCHSTRINGA
            //#define SHAREVISTRING  SHAREVISTRINGA
            //#define FILEOKSTRING   FILEOKSTRINGA
            //#define COLOROKSTRING  COLOROKSTRINGA
            //#define SETRGBSTRING   SETRGBSTRINGA
            //#define HELPMSGSTRING  HELPMSGSTRINGA
            //#define FINDMSGSTRING  FINDMSGSTRINGA
            //#endif

			// HIWORD values for lParam of commdlg_LBSelChangeNotify message
			public const int CD_LBSELNOITEMS = -1;
			public const int CD_LBSELCHANGE   = 0;
			public const int CD_LBSELSUB      = 1;
			public const int CD_LBSELADD      = 2;

			//  Flags for PrintDlg and PrintDlgEx.
			public const int PD_ALLPAGES                    = 0x00000000;
			public const int PD_SELECTION                   = 0x00000001;
			public const int PD_PAGENUMS                    = 0x00000002;
			public const int PD_NOSELECTION                 = 0x00000004;
			public const int PD_NOPAGENUMS                  = 0x00000008;
			public const int PD_COLLATE                     = 0x00000010;
			public const int PD_PRINTTOFILE                 = 0x00000020;
			public const int PD_PRINTSETUP                  = 0x00000040;
			public const int PD_NOWARNING                   = 0x00000080;
			public const int PD_RETURNDC                    = 0x00000100;
			public const int PD_RETURNIC                    = 0x00000200;
			public const int PD_RETURNDEFAULT               = 0x00000400;
			public const int PD_SHOWHELP                    = 0x00000800;
			public const int PD_ENABLEPRINTHOOK             = 0x00001000;
			public const int PD_ENABLESETUPHOOK             = 0x00002000;
			public const int PD_ENABLEPRINTTEMPLATE         = 0x00004000;
			public const int PD_ENABLESETUPTEMPLATE         = 0x00008000;
			public const int PD_ENABLEPRINTTEMPLATEHANDLE   = 0x00010000;
			public const int PD_ENABLESETUPTEMPLATEHANDLE   = 0x00020000;
			public const int PD_USEDEVMODECOPIES            = 0x00040000;
			public const int PD_USEDEVMODECOPIESANDCOLLATE  = 0x00040000;
			public const int PD_DISABLEPRINTTOFILE          = 0x00080000;
			public const int PD_HIDEPRINTTOFILE             = 0x00100000;
			public const int PD_NONETWORKBUTTON             = 0x00200000;
			public const int PD_CURRENTPAGE                 = 0x00400000;
			public const int PD_NOCURRENTPAGE               = 0x00800000;
			public const int PD_EXCLUSIONFLAGS              = 0x01000000;
			public const int PD_USELARGETEMPLATE            = 0x10000000;

			//
			//  Exclusion flags for PrintDlgEx.
			//
			//public const int PD_EXCL_COPIESANDCOLLATE       = (DM_COPIES | DM_COLLATE);


			//
			//  Define the start page for the print dialog when using PrintDlgEx.
			//
			public const uint START_PAGE_GENERAL             = 0xffffffff;


			//
			//  Result action ids for PrintDlgEx.
			//
			public const int PD_RESULT_CANCEL               = 0;
			public const int PD_RESULT_PRINT                = 1;
			public const int PD_RESULT_APPLY                = 2;


			public const int DN_DEFAULTPRN      = 0x0001;


			public const int WM_PSD_PAGESETUPDLG     = ((int)WM.WM_USER  );
			public const int WM_PSD_FULLPAGERECT     = ((int)WM.WM_USER+1);
			public const int WM_PSD_MINMARGINRECT    = ((int)WM.WM_USER+2);
			public const int WM_PSD_MARGINRECT       = ((int)WM.WM_USER+3);
			public const int WM_PSD_GREEKTEXTRECT    = ((int)WM.WM_USER+4);
			public const int WM_PSD_ENVSTAMPRECT     = ((int)WM.WM_USER+5);
			public const int WM_PSD_YAFULLPAGERECT   = ((int)WM.WM_USER+6);

			public const int PSD_DEFAULTMINMARGINS             = 0x00000000; // default (printer's)
			public const int PSD_INWININIINTLMEASURE           = 0x00000000; // 1st of 4 possible

			public const int PSD_MINMARGINS                    = 0x00000001; // use caller's
			public const int PSD_MARGINS                       = 0x00000002; // use caller's
			public const int PSD_INTHOUSANDTHSOFINCHES         = 0x00000004; // 2nd of 4 possible
			public const int PSD_INHUNDREDTHSOFMILLIMETERS     = 0x00000008; // 3rd of 4 possible
			public const int PSD_DISABLEMARGINS                = 0x00000010;
			public const int PSD_DISABLEPRINTER                = 0x00000020;
			public const int PSD_NOWARNING                     = 0x00000080; // must be same as PD_*
			public const int PSD_DISABLEORIENTATION            = 0x00000100;
			public const int PSD_RETURNDEFAULT                 = 0x00000400; // must be same as PD_*
			public const int PSD_DISABLEPAPER                  = 0x00000200;
			public const int PSD_SHOWHELP                      = 0x00000800; // must be same as PD_*
			public const int PSD_ENABLEPAGESETUPHOOK           = 0x00002000; // must be same as PD_*
			public const int PSD_ENABLEPAGESETUPTEMPLATE       = 0x00008000; // must be same as PD_*
			public const int PSD_ENABLEPAGESETUPTEMPLATEHANDLE = 0x00020000; // must be same as PD_*
			public const int PSD_ENABLEPAGEPAINTHOOK           = 0x00040000;
			public const int PSD_DISABLEPAGEPAINTING           = 0x00080000;
			public const int PSD_NONETWORKBUTTON               = 0x00200000; // must be same as PD_*

        }
        #endregion

        #region enum VK
        /// <summary>from msdn (http://msdn.microsoft.com/en-us/library/ms645540.aspx)</summary>
        public enum VK : ushort
        {
            /// <summary></summary>
            VK_UNKNOW              = 0x00,
            // Virtual Keys, Standard Set
            /// <summary>Left mouse button</summary>
            VK_LBUTTON             = 0x01,
            /// <summary>Right mouse button</summary>
            VK_RBUTTON             = 0x02,
            /// <summary>Control-break processing</summary>
            VK_CANCEL              = 0x03,
            /// <summary>Middle mouse button (three-button mouse)</summary>
            VK_MBUTTON             = 0x04,  // NOT contiguous with L & RBUTTON
            /// <summary>Windows 2000/XP: X1 mouse button</summary>
            VK_XBUTTON1            = 0x05,  // NOT contiguous with L & RBUTTON
            /// <summary>Windows 2000/XP: X2 mouse button</summary>
            VK_XBUTTON2            = 0x06,  // NOT contiguous with L & RBUTTON
            // 0x07 : unassigned
            /// <summary>BACKSPACE key</summary>
            VK_BACK                = 0x08,
            /// <summary>TAB key</summary>
            VK_TAB                 = 0x09,
            // 0x0A - 0x0B : reserved
            /// <summary>CLEAR key</summary>
            VK_CLEAR               = 0x0C,
            /// <summary>ENTER key</summary>
            VK_RETURN              = 0x0D,
            /// <summary>SHIFT key</summary>
            VK_SHIFT               = 0x10,
            /// <summary>CTRL key</summary>
            VK_CONTROL             = 0x11,
            /// <summary>ALT key</summary>
            VK_MENU                = 0x12,
            /// <summary>PAUSE key</summary>
            VK_PAUSE               = 0x13,
            /// <summary>CAPS LOCK key</summary>
            VK_CAPITAL             = 0x14,
            /// <summary>Input Method Editor (IME) Kana mode</summary>
            VK_KANA                = 0x15,
            /// <summary>IME Hanguel mode (maintained for compatibility; use VK_HANGUL)</summary>
            VK_HANGEUL             = 0x15,
            /// <summary>IME Hangul mode</summary>
            VK_HANGUL              = 0x15,
            /// <summary>IME Junja mode</summary>
            VK_JUNJA               = 0x17,
            /// <summary>IME final mode</summary>
            VK_FINAL               = 0x18,
            /// <summary>IME Hanja mode</summary>
            VK_HANJA               = 0x19,
            /// <summary>IME Kanji mode</summary>
            VK_KANJI               = 0x19,
            /// <summary>ESC key</summary>
            VK_ESCAPE              = 0x1B,
            /// <summary>IME convert</summary>
            VK_CONVERT             = 0x1C,
            /// <summary>IME nonconvert</summary>
            VK_NONCONVERT          = 0x1D,
            /// <summary>IME accept</summary>
            VK_ACCEPT              = 0x1E,
            /// <summary>IME mode change request</summary>
            VK_MODECHANGE          = 0x1F,
            /// <summary>SPACEBAR</summary>
            VK_SPACE               = 0x20,
            /// <summary>PAGE UP key</summary>
            VK_PRIOR               = 0x21,
            /// <summary>PAGE DOWN key</summary>
            VK_NEXT                = 0x22,
            /// <summary>END key</summary>
            VK_END                 = 0x23,
            /// <summary>HOME key</summary>
            VK_HOME                = 0x24,
            /// <summary>LEFT ARROW key</summary>
            VK_LEFT                = 0x25,
            /// <summary>UP ARROW key</summary>
            VK_UP                  = 0x26,
            /// <summary>RIGHT ARROW key</summary>
            VK_RIGHT               = 0x27,
            /// <summary>DOWN ARROW key</summary>
            VK_DOWN                = 0x28,
            /// <summary>SELECT key</summary>
            VK_SELECT              = 0x29,
            /// <summary>PRINT key</summary>
            VK_PRINT               = 0x2A,
            /// <summary>EXECUTE key</summary>
            VK_EXECUTE             = 0x2B,
            /// <summary>PRINT SCREEN key</summary>
            VK_SNAPSHOT            = 0x2C,
            /// <summary>INS key</summary>
            VK_INSERT              = 0x2D,
            /// <summary>DEL key</summary>
            VK_DELETE              = 0x2E,
            /// <summary>HELP key</summary>
            VK_HELP                = 0x2F,
            /// <summary>0 key</summary>
            VK_0                   = 0x30,
            /// <summary>1 key</summary>
            VK_1                   = 0x31,
            /// <summary>2 key</summary>
            VK_2                   = 0x32,
            /// <summary>3 key</summary>
            VK_3                   = 0x33,
            /// <summary>4 key</summary>
            VK_4                   = 0x34,
            /// <summary>5 key</summary>
            VK_5                   = 0x35,
            /// <summary>6 key</summary>
            VK_6                   = 0x36,
            /// <summary>7 key</summary>
            VK_7                   = 0x37,
            /// <summary>8 key</summary>
            VK_8                   = 0x38,
            /// <summary>9 key</summary>
            VK_9                   = 0x39,
            // 0x40 : unassigned
            /// <summary>A key</summary>
            VK_A                   = 0x41,
            /// <summary>B key</summary>
            VK_B                   = 0x42,
            /// <summary>C key</summary>
            VK_C                   = 0x43,
            /// <summary>D key</summary>
            VK_D                   = 0x44,
            /// <summary>E key</summary>
            VK_E                   = 0x45,
            /// <summary>F key</summary>
            VK_F                   = 0x46,
            /// <summary>G key</summary>
            VK_G                   = 0x47,
            /// <summary>H key</summary>
            VK_H                   = 0x48,
            /// <summary>I key</summary>
            VK_I                   = 0x49,
            /// <summary>J key</summary>
            VK_J                   = 0x4A,
            /// <summary>K key</summary>
            VK_K                   = 0x4B,
            /// <summary>L key</summary>
            VK_L                   = 0x4C,
            /// <summary>M key</summary>
            VK_M                   = 0x4D,
            /// <summary>N key</summary>
            VK_N                   = 0x4E,
            /// <summary>O key</summary>
            VK_O                   = 0x4F,
            /// <summary>P key</summary>
            VK_P                   = 0x50,
            /// <summary>Q key</summary>
            VK_Q                   = 0x51,
            /// <summary>R key</summary>
            VK_R                   = 0x52,
            /// <summary>S key</summary>
            VK_S                   = 0x53,
            /// <summary>T key</summary>
            VK_T                   = 0x54,
            /// <summary>U key</summary>
            VK_U                   = 0x55,
            /// <summary>V key</summary>
            VK_V                   = 0x56,
            /// <summary>W key</summary>
            VK_W                   = 0x57,
            /// <summary>X key</summary>
            VK_X                   = 0x58,
            /// <summary>Y key</summary>
            VK_Y                   = 0x59,
            /// <summary>Z key</summary>
            VK_Z                   = 0x5A,
            /// <summary>Left Windows key (Microsoft Natural keyboard)</summary>
            VK_LWIN                = 0x5B,
            /// <summary>Right Windows key (Natural keyboard)</summary>
            VK_RWIN                = 0x5C,
            /// <summary>Applications key (Natural keyboard)</summary>
            VK_APPS                = 0x5D,
            // 0x5E : reserved
            /// <summary>Computer Sleep key</summary>
            VK_SLEEP               = 0x5F,
            /// <summary>Numeric keypad 0 key</summary>
            VK_NUMPAD0             = 0x60,
            /// <summary>Numeric keypad 1 key</summary>
            VK_NUMPAD1             = 0x61,
            /// <summary>Numeric keypad 2 key</summary>
            VK_NUMPAD2             = 0x62,
            /// <summary>Numeric keypad 3 key</summary>
            VK_NUMPAD3             = 0x63,
            /// <summary>Numeric keypad 4 key</summary>
            VK_NUMPAD4             = 0x64,
            /// <summary>Numeric keypad 5 key</summary>
            VK_NUMPAD5             = 0x65,
            /// <summary>Numeric keypad 6 key</summary>
            VK_NUMPAD6             = 0x66,
            /// <summary>Numeric keypad 7 key</summary>
            VK_NUMPAD7             = 0x67,
            /// <summary>Numeric keypad 8 key</summary>
            VK_NUMPAD8             = 0x68,
            /// <summary>Numeric keypad 9 key</summary>
            VK_NUMPAD9             = 0x69,
            /// <summary>Multiply key</summary>
            VK_MULTIPLY            = 0x6A,
            /// <summary>Add key</summary>
            VK_ADD                 = 0x6B,
            /// <summary>Separator key</summary>
            VK_SEPARATOR           = 0x6C,
            /// <summary>Subtract key</summary>
            VK_SUBTRACT            = 0x6D,
            /// <summary>Decimal key</summary>
            VK_DECIMAL             = 0x6E,
            /// <summary>Divide key</summary>
            VK_DIVIDE              = 0x6F,
            /// <summary>F1 key</summary>
            VK_F1                  = 0x70,
            /// <summary>F2 key</summary>
            VK_F2                  = 0x71,
            /// <summary>F3 key</summary>
            VK_F3                  = 0x72,
            /// <summary>F4 key</summary>
            VK_F4                  = 0x73,
            /// <summary>F5 key</summary>
            VK_F5                  = 0x74,
            /// <summary>F6 key</summary>
            VK_F6                  = 0x75,
            /// <summary>F7 key</summary>
            VK_F7                  = 0x76,
            /// <summary>F8 key</summary>
            VK_F8                  = 0x77,
            /// <summary>F9 key</summary>
            VK_F9                  = 0x78,
            /// <summary>F10 key</summary>
            VK_F10                 = 0x79,
            /// <summary>F11 key</summary>
            VK_F11                 = 0x7A,
            /// <summary>F12 key</summary>
            VK_F12                 = 0x7B,
            /// <summary>F13 key</summary>
            VK_F13                 = 0x7C,
            /// <summary>F14 key</summary>
            VK_F14                 = 0x7D,
            /// <summary>F15 key</summary>
            VK_F15                 = 0x7E,
            /// <summary>F16 key</summary>
            VK_F16                 = 0x7F,
            /// <summary>F17 key</summary>
            VK_F17                 = 0x80,
            /// <summary>F18 key</summary>
            VK_F18                 = 0x81,
            /// <summary>F19 key</summary>
            VK_F19                 = 0x82,
            /// <summary>F20 key</summary>
            VK_F20                 = 0x83,
            /// <summary>F21 key</summary>
            VK_F21                 = 0x84,
            /// <summary>F22 key</summary>
            VK_F22                 = 0x85,
            /// <summary>F23 key</summary>
            VK_F23                 = 0x86,
            /// <summary>F24 key</summary>
            VK_F24                 = 0x87,
            // 0x88 - 0x8F : unassigned
            /// <summary>NUM LOCK key</summary>
            VK_NUMLOCK             = 0x90,
            /// <summary>SCROLL LOCK key</summary>
            VK_SCROLL              = 0x91,
            /// <summary>NEC PC-9800 '=' key on numpad</summary>
            VK_OEM_NEC_EQUAL       = 0x92,
            /// <summary>Fujitsu/OASYS 'Dictionary' key</summary>
            VK_OEM_FJ_JISHO        = 0x92,
            /// <summary>Fujitsu/OASYS 'Unregister word' key</summary>
            VK_OEM_FJ_MASSHOU      = 0x93,
            /// <summary>Fujitsu/OASYS 'Register word' key</summary>
            VK_OEM_FJ_TOUROKU      = 0x94,
            /// <summary>Fujitsu/OASYS 'Left OYAYUBI' key</summary>
            VK_OEM_FJ_LOYA         = 0x95,
            /// <summary>Fujitsu/OASYS 'Right OYAYUBI' key</summary>
            VK_OEM_FJ_ROYA         = 0x96,
            // 0x97 - 0x9F : unassigned
            // VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys. Used only as parameters to GetAsyncKeyState() and GetKeyState().
            /// <summary>Left SHIFT key</summary>
            VK_LSHIFT              = 0xA0,
            /// <summary>Right SHIFT key</summary>
            VK_RSHIFT              = 0xA1,
            /// <summary>Left CONTROL key</summary>
            VK_LCONTROL            = 0xA2,
            /// <summary>Right CONTROL key</summary>
            VK_RCONTROL            = 0xA3,
            /// <summary>Left MENU key</summary>
            VK_LMENU               = 0xA4,
            /// <summary>Right MENU key</summary>
            VK_RMENU               = 0xA5,
            /// <summary>Windows 2000/XP: Browser Back key</summary>
            VK_BROWSER_BACK        = 0xA6,
            /// <summary>Windows 2000/XP: Browser Forward key</summary>
            VK_BROWSER_FORWARD     = 0xA7,
            /// <summary>Windows 2000/XP: Browser Refresh key</summary>
            VK_BROWSER_REFRESH     = 0xA8,
            /// <summary>Windows 2000/XP: Browser Stop key</summary>
            VK_BROWSER_STOP        = 0xA9,
            /// <summary>Windows 2000/XP: Browser Search key</summary>
            VK_BROWSER_SEARCH      = 0xAA,
            /// <summary>Windows 2000/XP: Browser Favorites key</summary>
            VK_BROWSER_FAVORITES   = 0xAB,
            /// <summary>Windows 2000/XP: Browser Start and Home key</summary>
            VK_BROWSER_HOME        = 0xAC,
            /// <summary>Windows 2000/XP: Volume Mute key</summary>
            VK_VOLUME_MUTE         = 0xAD,
            /// <summary>Windows 2000/XP: Volume Down key</summary>
            VK_VOLUME_DOWN         = 0xAE,
            /// <summary>Windows 2000/XP: Volume Up key</summary>
            VK_VOLUME_UP           = 0xAF,
            /// <summary>Windows 2000/XP: Next Track key</summary>
            VK_MEDIA_NEXT_TRACK    = 0xB0,
            /// <summary>Windows 2000/XP: Previous Track key</summary>
            VK_MEDIA_PREV_TRACK    = 0xB1,
            /// <summary>Windows 2000/XP: Stop Media key</summary>
            VK_MEDIA_STOP          = 0xB2,
            /// <summary>Windows 2000/XP: Play/Pause Media key</summary>
            VK_MEDIA_PLAY_PAUSE    = 0xB3,
            /// <summary>Windows 2000/XP: Start Mail key</summary>
            VK_LAUNCH_MAIL         = 0xB4,
            /// <summary>Windows 2000/XP: Select Media key</summary>
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            /// <summary>Windows 2000/XP: Start Application 1 key</summary>
            VK_LAUNCH_APP1         = 0xB6,
            /// <summary>Windows 2000/XP: Start Application 2 key</summary>
            VK_LAUNCH_APP2         = 0xB7,
            // 0xB8 - 0xB9 : reserved
            /// <summary>Windows 2000/XP: For the US standard keyboard, the ';:' key</summary>
            VK_OEM_1               = 0xBA,
            /// <summary>Windows 2000/XP: For any country/region, the '+' key</summary>
            VK_OEM_PLUS            = 0xBB,
            /// <summary>Windows 2000/XP: For any country/region, the ',' key</summary>
            VK_OEM_COMMA           = 0xBC,
            /// <summary>Windows 2000/XP: For any country/region, the '-' key</summary>
            VK_OEM_MINUS           = 0xBD,
            /// <summary>Windows 2000/XP: For any country/region, the '.' key</summary>
            VK_OEM_PERIOD          = 0xBE,
            /// <summary>Windows 2000/XP: For the US standard keyboard, the '/?' key</summary>
            VK_OEM_2               = 0xBF,
            /// <summary>Windows 2000/XP: For the US standard keyboard, the '`~' key</summary>
            VK_OEM_3               = 0xC0,
            // 0xC1 - 0xD7 : reserved
            // 0xD8 - 0xDA : unassigned
            /// <summary>Windows 2000/XP: For the US standard keyboard, the '[{' key</summary>
            VK_OEM_4               = 0xDB,
            /// <summary>Windows 2000/XP: For the US standard keyboard, the '\|' key</summary>
            VK_OEM_5               = 0xDC,
            /// <summary>Windows 2000/XP: For the US standard keyboard, the ']}' key</summary>
            VK_OEM_6               = 0xDD,
            /// <summary>Windows 2000/XP: For the US standard keyboard, the 'single-quote/double-quote' key</summary>
            VK_OEM_7               = 0xDE,
            /// <summary></summary>
            VK_OEM_8               = 0xDF,
            // 0xE0 : reserved
            // Various extended or enhanced keyboards
            /// <summary>'AX' key on Japanese AX kbd</summary>
            VK_OEM_AX              = 0xE1,
            /// <summary>"&lt;&gt;" or "\|" on RT 102-key kbd.</summary>
            VK_OEM_102             = 0xE2,
            /// <summary>Help key on ICO</summary>
            VK_ICO_HELP            = 0xE3,
            /// <summary>00 key on ICO</summary>
            VK_ICO_00              = 0xE4,
            /// <summary>Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key</summary>
            VK_PROCESSKEY          = 0xE5,
            /// <summary>OEM specific</summary>
            VK_ICO_CLEAR           = 0xE6,
            /// <summary>Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP</summary>
            VK_PACKET              = 0xE7,
            // 0xE8 : unassigned

            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_RESET           = 0xE9,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_JUMP            = 0xEA,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_PA1             = 0xEB,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_PA2             = 0xEC,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_PA3             = 0xED,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_WSCTRL          = 0xEE,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_CUSEL           = 0xEF,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_ATTN            = 0xF0,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_FINISH          = 0xF1,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_COPY            = 0xF2,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_AUTO            = 0xF3,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_ENLW            = 0xF4,
            /// <summary>Nokia/Ericsson definitions</summary>
            VK_OEM_BACKTAB         = 0xF5,

            /// <summary>Attn key</summary>
            VK_ATTN                = 0xF6,
            /// <summary>CrSel key</summary>
            VK_CRSEL               = 0xF7,
            /// <summary>ExSel key</summary>
            VK_EXSEL               = 0xF8,
            /// <summary>Erase EOF key</summary>
            VK_EREOF               = 0xF9,
            /// <summary>Play key</summary>
            VK_PLAY                = 0xFA,
            /// <summary>Zoom key</summary>
            VK_ZOOM                = 0xFB,
            /// <summary>Reserved</summary>
            VK_NONAME              = 0xFC,
            /// <summary>PA1 key</summary>
            VK_PA1                 = 0xFD,
            /// <summary>Clear key</summary>
            VK_OEM_CLEAR           = 0xFE 

            // 0xFF : reserved
        }
        #endregion

        #region enum HookType
        /// <summary>from pinvoke.net (http://www.pinvoke.net/default.aspx/Enums/HookType.html)</summary>
        public enum HookType : int
        {
            /// <summary>Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar. For more information, see the MessageProc hook procedure.</summary>
            WH_MSGFILTER             = -1,
            /// <summary></summary>
            WH_MIN                   = -1,
            /// <summary>Installs a hook procedure that records input messages posted to the system message queue. This hook is useful for recording macros. For more information, see the JournalRecordProc hook procedure.</summary>
            WH_JOURNALRECORD         = 0,
            /// <summary>Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook procedure. For more information, see the JournalPlaybackProc hook procedure.</summary>
            WH_JOURNALPLAYBACK       = 1,
            /// <summary>Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure.</summary>
            WH_KEYBOARD              = 2,
            /// <summary>Installs a hook procedure that monitors messages posted to a message queue. For more information, see the GetMsgProc hook procedure.</summary>
            WH_GETMESSAGE            = 3,
            /// <summary>Installs a hook procedure that monitors messages before the system sends them to the destination window procedure. For more information, see the CallWndProc hook procedure.</summary>
            WH_CALLWNDPROC           = 4,
            /// <summary>Installs a hook procedure that receives notifications useful to a computer-based training (CBT) application. For more information, see the CBTProc hook procedure.</summary>
            WH_CBT                   = 5,
            /// <summary>Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar. The hook procedure monitors these messages for all applications in the same desktop as the calling thread. For more information, see the SysMsgProc hook procedure.</summary>
            WH_SYSMSGFILTER          = 6,
            /// <summary>Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure.</summary>
            WH_MOUSE                 = 7,
            /// <summary>???</summary>
            WH_HARDWARE              = 8,
            /// <summary>Installs a hook procedure useful for debugging other hook procedures. For more information, see the DebugProc hook procedure.</summary>
            WH_DEBUG                 = 9,
            /// <summary>Installs a hook procedure that receives notifications useful to shell applications. For more information, see the ShellProc hook procedure.</summary>
            WH_SHELL                 = 10,
            /// <summary>Installs a hook procedure that will be called when the application's foreground thread is about to become idle. This hook is useful for performing low priority tasks during idle time. For more information, see the ForegroundIdleProc hook procedure.</summary>
            WH_FOREGROUNDIDLE        = 11,
            /// <summary>Installs a hook procedure that monitors messages after they have been processed by the destination window procedure. For more information, see the CallWndRetProc hook procedure.</summary>
            WH_CALLWNDPROCRET        = 12,
            /// <summary>Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard input events. For more information, see the LowLevelKeyboardProc hook procedure.</summary>
            WH_KEYBOARD_LL           = 13,
            /// <summary>Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events. For more information, see the LowLevelMouseProc hook procedure.</summary>
            WH_MOUSE_LL              = 14,
            //WH_GESTURE_POSTPROCESS (Windows 7 and later: Installs a hook procedure that receives gesture notifications. For more information, see the GestureProc hook procedure.)
            WH_MAX                   = 14,
            WH_MINHOOK               = WH_MIN,
            WH_MAXHOOK               = WH_MAX
        }
        #endregion

        #region enum WA
        /// <summary>WM_ACTIVATE state values</summary>
        // doc msdn http://msdn.microsoft.com/en-us/library/ms646274(VS.85).aspx
        // defined in WinUser.h
        public enum WA : int
        {
            /// <summary>Unknow WM_ACTIVATE state</summary>
            WA_UNKNOW                       = -1,
            /// <summary>Activated by some method other than a mouse click (for example, by a call to the SetActiveWindow function or by use of the keyboard interface to select the window).</summary>
            WA_INACTIVE                     = 0,
            /// <summary>Activated by a mouse click.</summary>
            WA_ACTIVE                       = 1,
            /// <summary>Deactivated.</summary>
            WA_CLICKACTIVE                  = 2
        }
        #endregion

        #region enum SC
        /// <summary>type of system command (WM_SYSCOMMAND)</summary>
        // doc msdn http://msdn.microsoft.com/en-us/library/ms646360(VS.85).aspx
        // defined in WinUser.h
        public enum SC : int
        {
            /// <summary>Unknow type of system command</summary>
            SC_UNKNOW       = -1,
            /// <summary>Sizes the window.</summary>
            SC_SIZE         = 0xF000,
            /// <summary>Moves the window.</summary>
            SC_MOVE         = 0xF010,
            /// <summary>Minimizes the window.</summary>
            SC_MINIMIZE     = 0xF020,
            /// <summary>Maximizes the window.</summary>
            SC_MAXIMIZE     = 0xF030,
            /// <summary>Moves to the next window.</summary>
            SC_NEXTWINDOW   = 0xF040,
            /// <summary>Moves to the previous window.</summary>
            SC_PREVWINDOW   = 0xF050,
            /// <summary>Closes the window.</summary>
            SC_CLOSE        = 0xF060,
            /// <summary>Scrolls vertically.</summary>
            SC_VSCROLL      = 0xF070,
            /// <summary>Scrolls horizontally.</summary>
            SC_HSCROLL      = 0xF080,
            /// <summary>Retrieves the window menu as a result of a mouse click.</summary>
            SC_MOUSEMENU    = 0xF090,
            /// <summary>Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section.</summary>
            SC_KEYMENU      = 0xF100,
            /// <summary></summary>
            SC_ARRANGE      = 0xF110,
            /// <summary>Restores the window to its normal position and size.</summary>
            SC_RESTORE      = 0xF120,
            /// <summary>Activates the Start menu.</summary>
            SC_TASKLIST     = 0xF130,
            /// <summary>Executes the screen saver application specified in the [boot] section of the System.ini file.</summary>
            SC_SCREENSAVE   = 0xF140,
            /// <summary>Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate.</summary>
            SC_HOTKEY       = 0xF150,
            /// <summary>Selects the default item; the user double-clicked the window menu.</summary>
            SC_DEFAULT      = 0xF160,
            /// <summary>
            /// Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer.
            /// The lParam parameter can have the following values:
            /// -1 - the display is powering on
            /// 1 - the display is going to low power
            /// 2 - the display is being shut off
            /// </summary>
            SC_MONITORPOWER = 0xF170,
            /// <summary>Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message.</summary>
            SC_CONTEXTHELP  = 0xF180,
            /// <summary></summary>
            SC_SEPARATOR    = 0xF00F
            //obsolete : SC_ICON         = SC_MINIMIZE
            //obsolete : SC_ZOOM         = SC_MAXIMIZE
        }
        #endregion

        #region enum WM
        // from pinvoke.net (http://www.pinvoke.net/default.aspx/Enums/WindowsMessages.html)
        /// <summary>
        /// Windows Messages
        /// Defined in winuser.h from Windows SDK v6.1
        /// Documentation pulled from MSDN.
        /// </summary>
        public enum WM : uint
        {
            /// <summary>Ajouter pour GetWindowMessage()</summary>
            /// <value>toto</value>
            WM_UNKNOW                       = 0xFFFFFFFF,
            /// <summary>
            /// The WM_NULL message performs no operation. An application sends the WM_NULL message if it wants to post a message that the recipient window will ignore.
            /// </summary>
            WM_NULL = 0x0000,
            /// <summary>The WM_CREATE message is sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) The window procedure of the new window receives this message after the window is created, but before the window becomes visible.</summary>
            WM_CREATE                       = 0x0001,
            /// <summary>
            /// The WM_DESTROY message is sent when a window is being destroyed. It is sent to the window procedure of the window being destroyed after the window is removed from the screen. 
            /// This message is sent first to the window being destroyed and then to the child windows (if any) as they are destroyed. During the processing of the message, it can be assumed that all child windows still exist.
            /// /// </summary>
            //DESTROY = 0x0002,
            WM_DESTROY                      = 0x0002,
            /// <summary>
            /// The WM_MOVE message is sent after a window has been moved. 
            /// </summary>
            //MOVE = 0x0003,
            WM_MOVE                         = 0x0003,
            /// <summary>
            /// The WM_SIZE message is sent to a window after its size has changed.
            /// </summary>
            //SIZE = 0x0005,
            WM_SIZE                         = 0x0005,
            /// <summary>
            /// The WM_ACTIVATE message is sent to both the window being activated and the window being deactivated. If the windows use the same input queue, the message is sent synchronously, first to the window procedure of the top-level window being deactivated, then to the window procedure of the top-level window being activated. If the windows use different input queues, the message is sent asynchronously, so the window is activated immediately. 
            /// </summary>
            //ACTIVATE = 0x0006,
            WM_ACTIVATE                     = 0x0006,
            /// <summary>
            /// The WM_SETFOCUS message is sent to a window after it has gained the keyboard focus. 
            /// </summary>
            //SETFOCUS = 0x0007,
            WM_SETFOCUS                     = 0x0007,
            /// <summary>
            /// The WM_KILLFOCUS message is sent to a window immediately before it loses the keyboard focus. 
            /// </summary>
            //KILLFOCUS = 0x0008,
            WM_KILLFOCUS                    = 0x0008,
            /// <summary>
            /// The WM_ENABLE message is sent when an application changes the enabled state of a window. It is sent to the window whose enabled state is changing. This message is sent before the EnableWindow function returns, but after the enabled state (WS_DISABLED style bit) of the window has changed. 
            /// </summary>
            //ENABLE = 0x000A,
            WM_ENABLE                       = 0x000A,
            /// <summary>
            /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or to prevent changes in that window from being redrawn. 
            /// </summary>
            //SETREDRAW = 0x000B,
            WM_SETREDRAW                    = 0x000B,
            /// <summary>
            /// An application sends a WM_SETTEXT message to set the text of a window. 
            /// </summary>
            //SETTEXT = 0x000C,
            WM_SETTEXT                      = 0x000C,
            /// <summary>
            /// An application sends a WM_GETTEXT message to copy the text that corresponds to a window into a buffer provided by the caller. 
            /// </summary>
            //GETTEXT = 0x000D,
            WM_GETTEXT                      = 0x000D,
            /// <summary>
            /// An application sends a WM_GETTEXTLENGTH message to determine the length, in characters, of the text associated with a window. 
            /// </summary>
            //GETTEXTLENGTH = 0x000E,
            WM_GETTEXTLENGTH                = 0x000E,
            /// <summary>
            /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window. The message is sent when the UpdateWindow or RedrawWindow function is called, or by the DispatchMessage function when the application obtains a WM_PAINT message by using the GetMessage or PeekMessage function. 
            /// </summary>
            //PAINT = 0x000F,
            WM_PAINT                        = 0x000F,
            /// <summary>
            /// The WM_CLOSE message is sent as a signal that a window or an application should terminate.
            /// </summary>
            //CLOSE = 0x0010,
            WM_CLOSE                        = 0x0010,
            /// <summary>
            /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an application calls one of the system shutdown functions. If any application returns zero, the session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one application returns zero.
            /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter set to the results of the WM_QUERYENDSESSION message.
            /// </summary>
            //QUERYENDSESSION = 0x0011,
            WM_QUERYENDSESSION              = 0x0011,
            /// <summary>
            /// The WM_QUERYOPEN message is sent to an icon when the user requests that the window be restored to its previous size and position.
            /// </summary>
            //QUERYOPEN = 0x0013,
            WM_QUERYOPEN                    = 0x0013,
            /// <summary>
            /// The WM_ENDSESSION message is sent to an application after the system processes the results of the WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session is ending.
            /// </summary>
            //ENDSESSION = 0x0016,
            WM_ENDSESSION                   = 0x0016,
            /// <summary>
            /// The WM_QUIT message indicates a request to terminate an application and is generated when the application calls the PostQuitMessage function. It causes the GetMessage function to return zero.
            /// </summary>
            //QUIT = 0x0012,
            WM_QUIT                         = 0x0012,
            /// <summary>
            /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare an invalidated portion of a window for painting. 
            /// </summary>
            //ERASEBKGND = 0x0014,
            WM_ERASEBKGND                   = 0x0014,
            /// <summary>
            /// This message is sent to all top-level windows when a change is made to a system color setting. 
            /// </summary>
            //SYSCOLORCHANGE = 0x0015,
            WM_SYSCOLORCHANGE               = 0x0015,
            /// <summary>
            /// The WM_SHOWWINDOW message is sent to a window when the window is about to be hidden or shown.
            /// </summary>
            //SHOWWINDOW = 0x0018,
            WM_SHOWWINDOW                   = 0x0018,
            /// <summary>
            /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
            /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
            /// </summary>
            //WININICHANGE = 0x001A,
            WM_WININICHANGE                 = 0x001A,
            /// <summary>
            /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
            /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
            /// </summary>
            //SETTINGCHANGE = WM.WININICHANGE,
            WM_SETTINGCHANGE                = WM_WININICHANGE,
            /// <summary>
            /// The WM_DEVMODECHANGE message is sent to all top-level windows whenever the user changes device-mode settings. 
            /// </summary>
            //DEVMODECHANGE = 0x001B,
            WM_DEVMODECHANGE                = 0x001B,
            /// <summary>
            /// The WM_ACTIVATEAPP message is sent when a window belonging to a different application than the active window is about to be activated. The message is sent to the application whose window is being activated and to the application whose window is being deactivated.
            /// </summary>
            //ACTIVATEAPP = 0x001C,
            WM_ACTIVATEAPP                  = 0x001C,
            /// <summary>
            /// An application sends the WM_FONTCHANGE message to all top-level windows in the system after changing the pool of font resources. 
            /// </summary>
            //FONTCHANGE = 0x001D,
            WM_FONTCHANGE                   = 0x001D,
            /// <summary>
            /// A message that is sent whenever there is a change in the system time.
            /// </summary>
            //TIMECHANGE = 0x001E,
            WM_TIMECHANGE                   = 0x001E,
            /// <summary>
            /// The WM_CANCELMODE message is sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the active window when a dialog box or message box is displayed. Certain functions also send this message explicitly to the specified window regardless of whether it is the active window. For example, the EnableWindow function sends this message when disabling the specified window.
            /// </summary>
            //CANCELMODE = 0x001F,
            WM_CANCELMODE                   = 0x001F,
            /// <summary>
            /// The WM_SETCURSOR message is sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured. 
            /// </summary>
            //SETCURSOR = 0x0020,
            WM_SETCURSOR                    = 0x0020,
            /// <summary>
            /// The WM_MOUSEACTIVATE message is sent when the cursor is in an inactive window and the user presses a mouse button. The parent window receives this message only if the child window passes it to the DefWindowProc function.
            /// </summary>
            //MOUSEACTIVATE = 0x0021,
            WM_MOUSEACTIVATE                = 0x0021,
            /// <summary>
            /// The WM_CHILDACTIVATE message is sent to a child window when the user clicks the window's title bar or when the window is activated, moved, or sized.
            /// </summary>
            //CHILDACTIVATE = 0x0022,
            WM_CHILDACTIVATE                = 0x0022,
            /// <summary>
            /// The WM_QUEUESYNC message is sent by a computer-based training (CBT) application to separate user-input messages from other messages sent through the WH_JOURNALPLAYBACK Hook procedure. 
            /// </summary>
            //QUEUESYNC = 0x0023,
            WM_QUEUESYNC                    = 0x0023,
            /// <summary>
            /// The WM_GETMINMAXINFO message is sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size. 
            /// </summary>
            //GETMINMAXINFO = 0x0024,
            WM_GETMINMAXINFO                = 0x0024,
            /// <summary>
            /// Windows NT 3.51 and earlier: The WM_PAINTICON message is sent to a minimized window when the icon is to be painted. This message is not sent by newer versions of Microsoft Windows, except in unusual circumstances explained in the Remarks.
            /// </summary>
            //PAINTICON = 0x0026,
            WM_PAINTICON                    = 0x0026,
            /// <summary>
            /// Windows NT 3.51 and earlier: The WM_ICONERASEBKGND message is sent to a minimized window when the background of the icon must be filled before painting the icon. A window receives this message only if a class icon is defined for the window; otherwise, WM_ERASEBKGND is sent. This message is not sent by newer versions of Windows.
            /// </summary>
            //ICONERASEBKGND = 0x0027,
            WM_ICONERASEBKGND               = 0x0027,
            /// <summary>
            /// The WM_NEXTDLGCTL message is sent to a dialog box procedure to set the keyboard focus to a different control in the dialog box. 
            /// </summary>
            //NEXTDLGCTL = 0x0028,
            WM_NEXTDLGCTL                   = 0x0028,
            /// <summary>
            /// The WM_SPOOLERSTATUS message is sent from Print Manager whenever a job is added to or removed from the Print Manager queue. 
            /// </summary>
            //SPOOLERSTATUS = 0x002A,
            WM_SPOOLERSTATUS                = 0x002A,
            /// <summary>
            /// The WM_DRAWITEM message is sent to the parent window of an owner-drawn button, combo box, list box, or menu when a visual aspect of the button, combo box, list box, or menu has changed.
            /// </summary>
            //DRAWITEM = 0x002B,
            WM_DRAWITEM                     = 0x002B,
            /// <summary>
            /// The WM_MEASUREITEM message is sent to the owner window of a combo box, list box, list view control, or menu item when the control or menu is created.
            /// </summary>
            //MEASUREITEM = 0x002C,
            WM_MEASUREITEM                  = 0x002C,
            /// <summary>
            /// Sent to the owner of a list box or combo box when the list box or combo box is destroyed or when items are removed by the LB_DELETESTRING, LB_RESETCONTENT, CB_DELETESTRING, or CB_RESETCONTENT message. The system sends a WM_DELETEITEM message for each deleted item. The system sends the WM_DELETEITEM message for any deleted list box or combo box item with nonzero item data.
            /// </summary>
            //DELETEITEM = 0x002D,
            WM_DELETEITEM                   = 0x002D,
            /// <summary>
            /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_KEYDOWN message. 
            /// </summary>
            //VKEYTOITEM = 0x002E,
            WM_VKEYTOITEM                   = 0x002E,
            /// <summary>
            /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_CHAR message. 
            /// </summary>
            //CHARTOITEM = 0x002F,
            WM_CHARTOITEM                   = 0x002F,
            /// <summary>
            /// An application sends a WM_SETFONT message to specify the font that a control is to use when drawing text. 
            /// </summary>
            //SETFONT = 0x0030,
            WM_SETFONT                      = 0x0030,
            /// <summary>
            /// An application sends a WM_GETFONT message to a control to retrieve the font with which the control is currently drawing its text. 
            /// </summary>
            //GETFONT = 0x0031,
            WM_GETFONT                      = 0x0031,
            /// <summary>
            /// An application sends a WM_SETHOTKEY message to a window to associate a hot key with the window. When the user presses the hot key, the system activates the window. 
            /// </summary>
            //SETHOTKEY = 0x0032,
            WM_SETHOTKEY                    = 0x0032,
            /// <summary>
            /// An application sends a WM_GETHOTKEY message to determine the hot key associated with a window. 
            /// </summary>
            //GETHOTKEY = 0x0033,
            WM_GETHOTKEY                    = 0x0033,
            /// <summary>
            /// The WM_QUERYDRAGICON message is sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an icon defined for its class. An application can return a handle to an icon or cursor. The system displays this cursor or icon while the user drags the icon.
            /// </summary>
            //QUERYDRAGICON = 0x0037,
            WM_QUERYDRAGICON                = 0x0037,
            /// <summary>
            /// The system sends the WM_COMPAREITEM message to determine the relative position of a new item in the sorted list of an owner-drawn combo box or list box. Whenever the application adds a new item, the system sends this message to the owner of a combo box or list box created with the CBS_SORT or LBS_SORT style. 
            /// </summary>
            //COMPAREITEM = 0x0039,
            WM_COMPAREITEM                  = 0x0039,
            /// <summary>
            /// Active Accessibility sends the WM_GETOBJECT message to obtain information about an accessible object contained in a server application. 
            /// Applications never send this message directly. It is sent only by Active Accessibility in response to calls to AccessibleObjectFromPoint, AccessibleObjectFromEvent, or AccessibleObjectFromWindow. However, server applications handle this message. 
            /// </summary>
            //GETOBJECT = 0x003D,
            WM_GETOBJECT                    = 0x003D,
            /// <summary>
            /// The WM_COMPACTING message is sent to all top-level windows when the system detects more than 12.5 percent of system time over a 30- to 60-second interval is being spent compacting memory. This indicates that system memory is low.
            /// </summary>
            //COMPACTING = 0x0041,
            WM_COMPACTING                   = 0x0041,
            /// <summary>
            /// WM_COMMNOTIFY is Obsolete for Win32-Based Applications
            /// </summary>
            [Obsolete]
            //COMMNOTIFY = 0x0044,
            WM_COMMNOTIFY                   = 0x0044,
            /// <summary>
            /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.
            /// </summary>
            //WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGING            = 0x0046,
            /// <summary>
            /// The WM_WINDOWPOSCHANGED message is sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.
            /// </summary>
            //WINDOWPOSCHANGED = 0x0047,
            WM_WINDOWPOSCHANGED             = 0x0047,
            /// <summary>
            /// Notifies applications that the system, typically a battery-powered personal computer, is about to enter a suspended mode.
            /// Use: POWERBROADCAST
            /// </summary>
            [Obsolete]
            //POWER = 0x0048,
            WM_POWER                        = 0x0048,
            /// <summary>
            /// An application sends the WM_COPYDATA message to pass data to another application. 
            /// </summary>
            //COPYDATA = 0x004A,
            WM_COPYDATA                     = 0x004A,
            /// <summary>
            /// The WM_CANCELJOURNAL message is posted to an application when a user cancels the application's journaling activities. The message is posted with a NULL window handle. 
            /// </summary>
            //CANCELJOURNAL = 0x004B,
            WM_CANCELJOURNAL                = 0x004B,
            /// <summary>
            /// Sent by a common control to its parent window when an event has occurred or the control requires some information. 
            /// </summary>
            //NOTIFY = 0x004E,
            WM_NOTIFY                       = 0x004E,
            /// <summary>
            /// The WM_INPUTLANGCHANGEREQUEST message is posted to the window with the focus when the user chooses a new input language, either with the hotkey (specified in the Keyboard control panel application) or from the indicator on the system taskbar. An application can accept the change by passing the message to the DefWindowProc function or reject the change (and prevent it from taking place) by returning immediately. 
            /// </summary>
            //INPUTLANGCHANGEREQUEST = 0x0050,
            WM_INPUTLANGCHANGEREQUEST       = 0x0050,
            /// <summary>
            /// The WM_INPUTLANGCHANGE message is sent to the topmost affected window after an application's input language has been changed. You should make any application-specific settings and pass the message to the DefWindowProc function, which passes the message to all first-level child windows. These child windows can pass the message to DefWindowProc to have it pass the message to their child windows, and so on. 
            /// </summary>
            //INPUTLANGCHANGE = 0x0051,
            WM_INPUTLANGCHANGE              = 0x0051,
            /// <summary>
            /// Sent to an application that has initiated a training card with Microsoft Windows Help. The message informs the application when the user clicks an authorable button. An application initiates a training card by specifying the HELP_TCARD command in a call to the WinHelp function.
            /// </summary>
            //TCARD = 0x0052,
            WM_TCARD                        = 0x0052,
            /// <summary>
            /// Indicates that the user pressed the F1 key. If a menu is active when F1 is pressed, WM_HELP is sent to the window associated with the menu; otherwise, WM_HELP is sent to the window that has the keyboard focus. If no window has the keyboard focus, WM_HELP is sent to the currently active window.
            /// </summary>
            //HELP = 0x0053,
            WM_HELP                         = 0x0053,
            /// <summary>
            /// The WM_USERCHANGED message is sent to all windows after the user has logged on or off. When the user logs on or off, the system updates the user-specific settings. The system sends this message immediately after updating the settings.
            /// </summary>
            //USERCHANGED = 0x0054,
            WM_USERCHANGED                  = 0x0054,
            /// <summary>
            /// Determines if a window accepts ANSI or Unicode structures in the WM_NOTIFY notification message. WM_NOTIFYFORMAT messages are sent from a common control to its parent window and from the parent window to the common control.
            /// </summary>
            //NOTIFYFORMAT = 0x0055,
            WM_NOTIFYFORMAT                 = 0x0055,
            /// <summary>
            /// The WM_CONTEXTMENU message notifies a window that the user clicked the right mouse button (right-clicked) in the window.
            /// </summary>
            //CONTEXTMENU = 0x007B,
            WM_CONTEXTMENU                  = 0x007B,
            /// <summary>
            /// The WM_STYLECHANGING message is sent to a window when the SetWindowLong function is about to change one or more of the window's styles.
            /// </summary>
            //STYLECHANGING = 0x007C,
            WM_STYLECHANGING                = 0x007C,
            /// <summary>
            /// The WM_STYLECHANGED message is sent to a window after the SetWindowLong function has changed one or more of the window's styles
            /// </summary>
            //STYLECHANGED = 0x007D,
            WM_STYLECHANGED                 = 0x007D,
            /// <summary>
            /// The WM_DISPLAYCHANGE message is sent to all windows when the display resolution has changed.
            /// </summary>
            //DISPLAYCHANGE = 0x007E,
            WM_DISPLAYCHANGE                = 0x007E,
            /// <summary>
            /// The WM_GETICON message is sent to a window to retrieve a handle to the large or small icon associated with a window. The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption. 
            /// </summary>
            //GETICON = 0x007F,
            WM_GETICON                      = 0x007F,
            /// <summary>
            /// An application sends the WM_SETICON message to associate a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption. 
            /// </summary>
            //SETICON = 0x0080,
            WM_SETICON                      = 0x0080,
            /// <summary>
            /// The WM_NCCREATE message is sent prior to the WM_CREATE message when a window is first created.
            /// </summary>
            //NCCREATE = 0x0081,
            WM_NCCREATE                     = 0x0081,
            /// <summary>
            /// The WM_NCDESTROY message informs a window that its nonclient area is being destroyed. The DestroyWindow function sends the WM_NCDESTROY message to the window following the WM_DESTROY message. WM_DESTROY is used to free the allocated memory object associated with the window. 
            /// The WM_NCDESTROY message is sent after the child windows have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed.
            /// </summary>
            //NCDESTROY = 0x0082,
            WM_NCDESTROY                    = 0x0082,
            /// <summary>
            /// The WM_NCCALCSIZE message is sent when the size and position of a window's client area must be calculated. By processing this message, an application can control the content of the window's client area when the size or position of the window changes.
            /// </summary>
            //NCCALCSIZE = 0x0083,
            WM_NCCALCSIZE                   = 0x0083,
            /// <summary>
            /// The WM_NCHITTEST message is sent to a window when the cursor moves, or when a mouse button is pressed or released. If the mouse is not captured, the message is sent to the window beneath the cursor. Otherwise, the message is sent to the window that has captured the mouse.
            /// </summary>
            //NCHITTEST = 0x0084,
            WM_NCHITTEST                    = 0x0084,
            /// <summary>
            /// The WM_NCPAINT message is sent to a window when its frame must be painted. 
            /// </summary>
            //NCPAINT = 0x0085,
            WM_NCPAINT                      = 0x0085,
            /// <summary>
            /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
            /// </summary>
            //NCACTIVATE = 0x0086,
            WM_NCACTIVATE                   = 0x0086,
            /// <summary>
            /// The WM_GETDLGCODE message is sent to the window procedure associated with a control. By default, the system handles all keyboard input to the control; the system interprets certain types of keyboard input as dialog box navigation keys. To override this default behavior, the control can respond to the WM_GETDLGCODE message to indicate the types of input it wants to process itself.
            /// </summary>
            //GETDLGCODE = 0x0087,
            WM_GETDLGCODE                   = 0x0087,
            /// <summary>
            /// The WM_SYNCPAINT message is used to synchronize painting while avoiding linking independent GUI threads.
            /// </summary>
            //SYNCPAINT = 0x0088,
            WM_SYNCPAINT                    = 0x0088,
            /// <summary>
            /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCMOUSEMOVE = 0x00A0,
            WM_NCMOUSEMOVE                  = 0x00A0,
            /// <summary>
            /// The WM_NCLBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONDOWN                = 0x00A1,
            /// <summary>
            /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONUP                  = 0x00A2,
            /// <summary>
            /// The WM_NCLBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCLBUTTONDBLCLK = 0x00A3,
            WM_NCLBUTTONDBLCLK              = 0x00A3,
            /// <summary>
            /// The WM_NCRBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONDOWN                = 0x00A4,
            /// <summary>
            /// The WM_NCRBUTTONUP message is posted when the user releases the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONUP                  = 0x00A5,
            /// <summary>
            /// The WM_NCRBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCRBUTTONDBLCLK = 0x00A6,
            WM_NCRBUTTONDBLCLK              = 0x00A6,
            /// <summary>
            /// The WM_NCMBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONDOWN                = 0x00A7,
            /// <summary>
            /// The WM_NCMBUTTONUP message is posted when the user releases the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONUP                  = 0x00A8,
            /// <summary>
            /// The WM_NCMBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCMBUTTONDBLCLK = 0x00A9,
            WM_NCMBUTTONDBLCLK              = 0x00A9,
            /// <summary>
            /// The WM_NCXBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCXBUTTONDOWN = 0x00AB,
            WM_NCXBUTTONDOWN                = 0x00AB,
            /// <summary>
            /// The WM_NCXBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCXBUTTONUP = 0x00AC,
            WM_NCXBUTTONUP                  = 0x00AC,
            /// <summary>
            /// The WM_NCXBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
            /// </summary>
            //NCXBUTTONDBLCLK = 0x00AD,
            WM_NCXBUTTONDBLCLK              = 0x00AD,
            /// <summary>
            /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its WindowProc function.
            /// </summary>
            //INPUT_DEVICE_CHANGE = 0x00FE,
            WM_INPUT_DEVICE_CHANGE          = 0x00FE,
            /// <summary>
            /// The WM_INPUT message is sent to the window that is getting raw input. 
            /// </summary>
            //INPUT = 0x00FF,
            WM_INPUT                        = 0x00FF,
            /// <summary>
            /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed. 
            /// </summary>
            //KEYDOWN = 0x0100,
            WM_KEYDOWN                      = 0x0100,
            /// <summary>
            /// This message filters for keyboard messages.
            /// </summary>
            //KEYFIRST = 0x0100,
            WM_KEYFIRST                     = 0x0100,
            /// <summary>
            /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus. 
            /// </summary>
            //KEYUP = 0x0101,
            WM_KEYUP                        = 0x0101,
            /// <summary>
            /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_CHAR message contains the character code of the key that was pressed. 
            /// </summary>
            //CHAR = 0x0102,
            WM_CHAR                         = 0x0102,
            /// <summary>
            /// The WM_DEADCHAR message is posted to the window with the keyboard focus when a WM_KEYUP message is translated by the TranslateMessage function. WM_DEADCHAR specifies a character code generated by a dead key. A dead key is a key that generates a character, such as the umlaut (double-dot), that is combined with another character to form a composite character. For example, the umlaut-O character (Ö) is generated by typing the dead key for the umlaut character, and then typing the O key. 
            /// </summary>
            //DEADCHAR = 0x0103,
            WM_DEADCHAR                     = 0x0103,
            /// <summary>
            /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter. 
            /// </summary>
            //SYSKEYDOWN = 0x0104,
            WM_SYSKEYDOWN                   = 0x0104,
            /// <summary>
            /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter. 
            /// </summary>
            //SYSKEYUP = 0x0105,
            WM_SYSKEYUP                     = 0x0105,
            /// <summary>
            /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. It specifies the character code of a system character key — that is, a character key that is pressed while the ALT key is down. 
            /// </summary>
            //SYSCHAR = 0x0106,
            WM_SYSCHAR                      = 0x0106,
            /// <summary>
            /// The WM_SYSDEADCHAR message is sent to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. WM_SYSDEADCHAR specifies the character code of a system dead key — that is, a dead key that is pressed while holding down the ALT key. 
            /// </summary>
            //SYSDEADCHAR = 0x0107,
            WM_SYSDEADCHAR                  = 0x0107,
            /// <summary>
            /// The WM_UNICHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_UNICHAR message contains the character code of the key that was pressed. 
            /// The WM_UNICHAR message is equivalent to WM_CHAR, but it uses Unicode Transformation Format (UTF)-32, whereas WM_CHAR uses UTF-16. It is designed to send or post Unicode characters to ANSI windows and it can can handle Unicode Supplementary Plane characters.
            /// </summary>
            //UNICHAR = 0x0109,
            WM_UNICHAR                      = 0x0109,
            /// <summary>
            /// This message filters for keyboard messages.
            /// </summary>
            //KEYLAST = 0x0109,
            WM_KEYLAST                      = 0x0109,
            /// <summary>
            /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_STARTCOMPOSITION = 0x010D,
            WM_IME_STARTCOMPOSITION         = 0x010D,
            /// <summary>
            /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_ENDCOMPOSITION = 0x010E,
            WM_IME_ENDCOMPOSITION           = 0x010E,
            /// <summary>
            /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_COMPOSITION = 0x010F,
            WM_IME_COMPOSITION              = 0x010F,
            //IME_KEYLAST = 0x010F,
            WM_IME_KEYLAST                  = 0x010F,
            /// <summary>
            /// The WM_INITDIALOG message is sent to the dialog box procedure immediately before a dialog box is displayed. Dialog box procedures typically use this message to initialize controls and carry out any other initialization tasks that affect the appearance of the dialog box. 
            /// </summary>
            //INITDIALOG = 0x0110,
            WM_INITDIALOG                   = 0x0110,
            /// <summary>
            /// The WM_COMMAND message is sent when the user selects a command item from a menu, when a control sends a notification message to its parent window, or when an accelerator keystroke is translated. 
            /// </summary>
            //COMMAND = 0x0111,
            WM_COMMAND                      = 0x0111,
            /// <summary>
            /// A window receives this message when the user chooses a command from the Window menu (formerly known as the system or control menu) or when the user chooses the maximize button, minimize button, restore button, or close button.
            /// </summary>
            //SYSCOMMAND = 0x0112,
            WM_SYSCOMMAND                   = 0x0112,
            /// <summary>
            /// The WM_TIMER message is posted to the installing thread's message queue when a timer expires. The message is posted by the GetMessage or PeekMessage function. 
            /// </summary>
            //TIMER = 0x0113,
            WM_TIMER                        = 0x0113,
            /// <summary>
            /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard horizontal scroll bar. This message is also sent to the owner of a horizontal scroll bar control when a scroll event occurs in the control. 
            /// </summary>
            //HSCROLL = 0x0114,
            WM_HSCROLL                      = 0x0114,
            /// <summary>
            /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's standard vertical scroll bar. This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs in the control. 
            /// </summary>
            //VSCROLL = 0x0115,
            WM_VSCROLL                      = 0x0115,
            /// <summary>
            /// The WM_INITMENU message is sent when a menu is about to become active. It occurs when the user clicks an item on the menu bar or presses a menu key. This allows the application to modify the menu before it is displayed. 
            /// </summary>
            //INITMENU = 0x0116,
            WM_INITMENU                     = 0x0116,
            /// <summary>
            /// The WM_INITMENUPOPUP message is sent when a drop-down menu or submenu is about to become active. This allows an application to modify the menu before it is displayed, without changing the entire menu. 
            /// </summary>
            //INITMENUPOPUP = 0x0117,
            WM_INITMENUPOPUP                = 0x0117,
            /// <summary>
            /// The WM_MENUSELECT message is sent to a menu's owner window when the user selects a menu item. 
            /// </summary>
            //MENUSELECT = 0x011F,
            WM_MENUSELECT                   = 0x011F,
            /// <summary>
            /// The WM_MENUCHAR message is sent when a menu is active and the user presses a key that does not correspond to any mnemonic or accelerator key. This message is sent to the window that owns the menu. 
            /// </summary>
            //MENUCHAR = 0x0120,
            WM_MENUCHAR                     = 0x0120,
            /// <summary>
            /// The WM_ENTERIDLE message is sent to the owner window of a modal dialog box or menu that is entering an idle state. A modal dialog box or menu enters an idle state when no messages are waiting in its queue after it has processed one or more previous messages. 
            /// </summary>
            //ENTERIDLE = 0x0121,
            WM_ENTERIDLE                    = 0x0121,
            /// <summary>
            /// The WM_MENURBUTTONUP message is sent when the user releases the right mouse button while the cursor is on a menu item. 
            /// </summary>
            //MENURBUTTONUP = 0x0122,
            WM_MENURBUTTONUP                = 0x0122,
            /// <summary>
            /// The WM_MENUDRAG message is sent to the owner of a drag-and-drop menu when the user drags a menu item. 
            /// </summary>
            //MENUDRAG = 0x0123,
            WM_MENUDRAG                     = 0x0123,
            /// <summary>
            /// The WM_MENUGETOBJECT message is sent to the owner of a drag-and-drop menu when the mouse cursor enters a menu item or moves from the center of the item to the top or bottom of the item. 
            /// </summary>
            //MENUGETOBJECT = 0x0124,
            WM_MENUGETOBJECT                = 0x0124,
            /// <summary>
            /// The WM_UNINITMENUPOPUP message is sent when a drop-down menu or submenu has been destroyed. 
            /// </summary>
            //UNINITMENUPOPUP = 0x0125,
            WM_UNINITMENUPOPUP              = 0x0125,
            /// <summary>
            /// The WM_MENUCOMMAND message is sent when the user makes a selection from a menu. 
            /// </summary>
            //MENUCOMMAND = 0x0126,
            WM_MENUCOMMAND                  = 0x0126,
            /// <summary>
            /// An application sends the WM_CHANGEUISTATE message to indicate that the user interface (UI) state should be changed.
            /// </summary>
            //CHANGEUISTATE = 0x0127,
            WM_CHANGEUISTATE                = 0x0127,
            /// <summary>
            /// An application sends the WM_UPDATEUISTATE message to change the user interface (UI) state for the specified window and all its child windows.
            /// </summary>
            //UPDATEUISTATE = 0x0128,
            WM_UPDATEUISTATE                = 0x0128,
            /// <summary>
            /// An application sends the WM_QUERYUISTATE message to retrieve the user interface (UI) state for a window.
            /// </summary>
            //QUERYUISTATE = 0x0129,
            WM_QUERYUISTATE                 = 0x0129,
            /// <summary>
            /// The WM_CTLCOLORMSGBOX message is sent to the owner window of a message box before Windows draws the message box. By responding to this message, the owner window can set the text and background colors of the message box by using the given display device context handle. 
            /// </summary>
            //CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLORMSGBOX               = 0x0132,
            /// <summary>
            /// An edit control that is not read-only or disabled sends the WM_CTLCOLOREDIT message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the edit control. 
            /// </summary>
            //CTLCOLOREDIT = 0x0133,
            WM_CTLCOLOREDIT                 = 0x0133,
            /// <summary>
            /// Sent to the parent window of a list box before the system draws the list box. By responding to this message, the parent window can set the text and background colors of the list box by using the specified display device context handle. 
            /// </summary>
            //CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORLISTBOX              = 0x0134,
            /// <summary>
            /// The WM_CTLCOLORBTN message is sent to the parent window of a button before drawing the button. The parent window can change the button's text and background colors. However, only owner-drawn buttons respond to the parent window processing this message. 
            /// </summary>
            //CTLCOLORBTN = 0x0135,
            WM_CTLCOLORBTN                  = 0x0135,
            /// <summary>
            /// The WM_CTLCOLORDLG message is sent to a dialog box before the system draws the dialog box. By responding to this message, the dialog box can set its text and background colors using the specified display device context handle. 
            /// </summary>
            //CTLCOLORDLG = 0x0136,
            WM_CTLCOLORDLG                  = 0x0136,
            /// <summary>
            /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn. By responding to this message, the parent window can use the display context handle to set the background color of the scroll bar control. 
            /// </summary>
            //CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSCROLLBAR            = 0x0137,
            /// <summary>
            /// A static control, or an edit control that is read-only or disabled, sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the static control. 
            /// </summary>
            //CTLCOLORSTATIC = 0x0138,
            WM_CTLCOLORSTATIC               = 0x0138,
            /// <summary>
            /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. If the mouse is not captured, the message is posted to the window that contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //MOUSEMOVE = 0x0200,
            WM_MOUSEMOVE                    = 0x0200,
            /// <summary>
            /// Use WM_MOUSEFIRST to specify the first mouse message. Use the PeekMessage() Function.
            /// </summary>
            //MOUSEFIRST = 0x0200,
            WM_MOUSEFIRST                   = 0x0200,
            /// <summary>
            /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //LBUTTONDOWN = 0x0201,
            WM_LBUTTONDOWN                  = 0x0201,
            /// <summary>
            /// The WM_LBUTTONUP message is posted when the user releases the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //LBUTTONUP = 0x0202,
            WM_LBUTTONUP                    = 0x0202,
            /// <summary>
            /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //LBUTTONDBLCLK = 0x0203,
            WM_LBUTTONDBLCLK                = 0x0203,
            /// <summary>
            /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //RBUTTONDOWN = 0x0204,
            WM_RBUTTONDOWN                  = 0x0204,
            /// <summary>
            /// The WM_RBUTTONUP message is posted when the user releases the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //RBUTTONUP = 0x0205,
            WM_RBUTTONUP                    = 0x0205,
            /// <summary>
            /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //RBUTTONDBLCLK = 0x0206,
            WM_RBUTTONDBLCLK                = 0x0206,
            /// <summary>
            /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //MBUTTONDOWN = 0x0207,
            WM_MBUTTONDOWN                  = 0x0207,
            /// <summary>
            /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //MBUTTONUP = 0x0208,
            WM_MBUTTONUP                    = 0x0208,
            /// <summary>
            /// The WM_MBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //MBUTTONDBLCLK = 0x0209,
            WM_MBUTTONDBLCLK                = 0x0209,
            /// <summary>
            /// The WM_MOUSEWHEEL message is sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
            /// </summary>
            //MOUSEWHEEL = 0x020A,
            WM_MOUSEWHEEL                   = 0x020A,
            /// <summary>
            /// The WM_XBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //XBUTTONDOWN = 0x020B,
            WM_XBUTTONDOWN                  = 0x020B,
            /// <summary>
            /// The WM_XBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //XBUTTONUP = 0x020C,
            WM_XBUTTONUP                    = 0x020C,
            /// <summary>
            /// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
            /// </summary>
            //XBUTTONDBLCLK = 0x020D,
            WM_XBUTTONDBLCLK                = 0x020D,
            /// <summary>
            /// The WM_MOUSEHWHEEL message is sent to the focus window when the mouse's horizontal scroll wheel is tilted or rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
            /// </summary>
            //MOUSEHWHEEL = 0x020E,
            WM_MOUSEHWHEEL                  = 0x020E,
            /// <summary>
            /// Use WM_MOUSELAST to specify the last mouse message. Used with PeekMessage() Function.
            /// </summary>
            //MOUSELAST = 0x020E,
            WM_MOUSELAST                    = 0x020E,
            /// <summary>
            /// The WM_PARENTNOTIFY message is sent to the parent of a child window when the child window is created or destroyed, or when the user clicks a mouse button while the cursor is over the child window. When the child window is being created, the system sends WM_PARENTNOTIFY just before the CreateWindow or CreateWindowEx function that creates the window returns. When the child window is being destroyed, the system sends the message before any processing to destroy the window takes place.
            /// </summary>
            //PARENTNOTIFY = 0x0210,
            WM_PARENTNOTIFY                 = 0x0210,
            /// <summary>
            /// The WM_ENTERMENULOOP message informs an application's main window procedure that a menu modal loop has been entered. 
            /// </summary>
            //ENTERMENULOOP = 0x0211,
            WM_ENTERMENULOOP                = 0x0211,
            /// <summary>
            /// The WM_EXITMENULOOP message informs an application's main window procedure that a menu modal loop has been exited. 
            /// </summary>
            //EXITMENULOOP = 0x0212,
            WM_EXITMENULOOP                 = 0x0212,
            /// <summary>
            /// The WM_NEXTMENU message is sent to an application when the right or left arrow key is used to switch between the menu bar and the system menu. 
            /// </summary>
            //NEXTMENU = 0x0213,
            WM_NEXTMENU                     = 0x0213,
            /// <summary>
            /// The WM_SIZING message is sent to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position. 
            /// </summary>
            //SIZING = 0x0214,
            WM_SIZING                       = 0x0214,
            /// <summary>
            /// The WM_CAPTURECHANGED message is sent to the window that is losing the mouse capture.
            /// </summary>
            //CAPTURECHANGED = 0x0215,
            WM_CAPTURECHANGED               = 0x0215,
            /// <summary>
            /// The WM_MOVING message is sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag rectangle and, if needed, change its position.
            /// </summary>
            //MOVING = 0x0216,
            WM_MOVING                       = 0x0216,
            /// <summary>
            /// Notifies applications that a power-management event has occurred.
            /// </summary>
            //POWERBROADCAST = 0x0218,
            WM_POWERBROADCAST               = 0x0218,
            /// <summary>
            /// Notifies an application of a change to the hardware configuration of a device or the computer.
            /// </summary>
            //DEVICECHANGE = 0x0219,
            WM_DEVICECHANGE                 = 0x0219,
            /// <summary>
            /// An application sends the WM_MDICREATE message to a multiple-document interface (MDI) client window to create an MDI child window. 
            /// </summary>
            //MDICREATE = 0x0220,
            WM_MDICREATE                    = 0x0220,
            /// <summary>
            /// An application sends the WM_MDIDESTROY message to a multiple-document interface (MDI) client window to close an MDI child window. 
            /// </summary>
            //MDIDESTROY = 0x0221,
            WM_MDIDESTROY                   = 0x0221,
            /// <summary>
            /// An application sends the WM_MDIACTIVATE message to a multiple-document interface (MDI) client window to instruct the client window to activate a different MDI child window. 
            /// </summary>
            //MDIACTIVATE = 0x0222,
            WM_MDIACTIVATE                  = 0x0222,
            /// <summary>
            /// An application sends the WM_MDIRESTORE message to a multiple-document interface (MDI) client window to restore an MDI child window from maximized or minimized size. 
            /// </summary>
            //MDIRESTORE = 0x0223,
            WM_MDIRESTORE                   = 0x0223,
            /// <summary>
            /// An application sends the WM_MDINEXT message to a multiple-document interface (MDI) client window to activate the next or previous child window. 
            /// </summary>
            //MDINEXT = 0x0224,
            WM_MDINEXT                      = 0x0224,
            /// <summary>
            /// An application sends the WM_MDIMAXIMIZE message to a multiple-document interface (MDI) client window to maximize an MDI child window. The system resizes the child window to make its client area fill the client window. The system places the child window's window menu icon in the rightmost position of the frame window's menu bar, and places the child window's restore icon in the leftmost position. The system also appends the title bar text of the child window to that of the frame window. 
            /// </summary>
            //MDIMAXIMIZE = 0x0225,
            WM_MDIMAXIMIZE                  = 0x0225,
            /// <summary>
            /// An application sends the WM_MDITILE message to a multiple-document interface (MDI) client window to arrange all of its MDI child windows in a tile format. 
            /// </summary>
            //MDITILE = 0x0226,
            WM_MDITILE                      = 0x0226,
            /// <summary>
            /// An application sends the WM_MDICASCADE message to a multiple-document interface (MDI) client window to arrange all its child windows in a cascade format. 
            /// </summary>
            //MDICASCADE = 0x0227,
            WM_MDICASCADE                   = 0x0227,
            /// <summary>
            /// An application sends the WM_MDIICONARRANGE message to a multiple-document interface (MDI) client window to arrange all minimized MDI child windows. It does not affect child windows that are not minimized. 
            /// </summary>
            //MDIICONARRANGE = 0x0228,
            WM_MDIICONARRANGE               = 0x0228,
            /// <summary>
            /// An application sends the WM_MDIGETACTIVE message to a multiple-document interface (MDI) client window to retrieve the handle to the active MDI child window. 
            /// </summary>
            //MDIGETACTIVE = 0x0229,
            WM_MDIGETACTIVE                 = 0x0229,
            /// <summary>
            /// An application sends the WM_MDISETMENU message to a multiple-document interface (MDI) client window to replace the entire menu of an MDI frame window, to replace the window menu of the frame window, or both. 
            /// </summary>
            //MDISETMENU = 0x0230,
            WM_MDISETMENU                   = 0x0230,
            /// <summary>
            /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. 
            /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
            /// </summary>
            //ENTERSIZEMOVE = 0x0231,
            WM_ENTERSIZEMOVE                = 0x0231,
            /// <summary>
            /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. 
            /// </summary>
            //EXITSIZEMOVE = 0x0232,
            WM_EXITSIZEMOVE                 = 0x0232,
            /// <summary>
            /// Sent when the user drops a file on the window of an application that has registered itself as a recipient of dropped files.
            /// </summary>
            //DROPFILES = 0x0233,
            WM_DROPFILES                    = 0x0233,
            /// <summary>
            /// An application sends the WM_MDIREFRESHMENU message to a multiple-document interface (MDI) client window to refresh the window menu of the MDI frame window. 
            /// </summary>
            //MDIREFRESHMENU = 0x0234,
            WM_MDIREFRESHMENU               = 0x0234,
            /// <summary>
            /// Sent to an application when a window is activated. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_SETCONTEXT = 0x0281,
            WM_IME_SETCONTEXT               = 0x0281,
            /// <summary>
            /// Sent to an application to notify it of changes to the IME window. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_NOTIFY = 0x0282,
            WM_IME_NOTIFY                   = 0x0282,
            /// <summary>
            /// Sent by an application to direct the IME window to carry out the requested command. The application uses this message to control the IME window that it has created. To send this message, the application calls the SendMessage function with the following parameters.
            /// </summary>
            //IME_CONTROL = 0x0283,
            WM_IME_CONTROL                  = 0x0283,
            /// <summary>
            /// Sent to an application when the IME window finds no space to extend the area for the composition window. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_COMPOSITIONFULL = 0x0284,
            WM_IME_COMPOSITIONFULL          = 0x0284,
            /// <summary>
            /// Sent to an application when the operating system is about to change the current IME. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_SELECT = 0x0285,
            WM_IME_SELECT                   = 0x0285,
            /// <summary>
            /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_CHAR = 0x0286,
            WM_IME_CHAR                     = 0x0286,
            /// <summary>
            /// Sent to an application to provide commands and request information. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_REQUEST = 0x0288,
            WM_IME_REQUEST                  = 0x0288,
            /// <summary>
            /// Sent to an application by the IME to notify the application of a key press and to keep message order. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_KEYDOWN = 0x0290,
            WM_IME_KEYDOWN                  = 0x0290,
            /// <summary>
            /// Sent to an application by the IME to notify the application of a key release and to keep message order. A window receives this message through its WindowProc function. 
            /// </summary>
            //IME_KEYUP = 0x0291,
            WM_IME_KEYUP                    = 0x0291,
            /// <summary>
            /// The WM_MOUSEHOVER message is posted to a window when the cursor hovers over the client area of the window for the period of time specified in a prior call to TrackMouseEvent.
            /// </summary>
            //MOUSEHOVER = 0x02A1,
            WM_MOUSEHOVER                   = 0x02A1,
            /// <summary>
            /// The WM_MOUSELEAVE message is posted to a window when the cursor leaves the client area of the window specified in a prior call to TrackMouseEvent.
            /// </summary>
            //MOUSELEAVE = 0x02A3,
            WM_MOUSELEAVE                   = 0x02A3,
            /// <summary>
            /// The WM_NCMOUSEHOVER message is posted to a window when the cursor hovers over the nonclient area of the window for the period of time specified in a prior call to TrackMouseEvent.
            /// </summary>
            //NCMOUSEHOVER = 0x02A0,
            WM_NCMOUSEHOVER                 = 0x02A0,
            /// <summary>
            /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to TrackMouseEvent.
            /// </summary>
            //NCMOUSELEAVE = 0x02A2,
            WM_NCMOUSELEAVE                 = 0x02A2,
            /// <summary>
            /// The WM_WTSSESSION_CHANGE message notifies applications of changes in session state.
            /// </summary>
            //WTSSESSION_CHANGE = 0x02B1,
            WM_WTSSESSION_CHANGE            = 0x02B1,
            //TABLET_FIRST = 0x02c0,
            WM_TABLET_FIRST                 = 0x02c0,
            //TABLET_LAST = 0x02df,
            WM_TABLET_LAST                  = 0x02df,
            /// <summary>
            /// An application sends a WM_CUT message to an edit control or combo box to delete (cut) the current selection, if any, in the edit control and copy the deleted text to the clipboard in CF_TEXT format. 
            /// </summary>
            //CUT = 0x0300,
            WM_CUT                          = 0x0300,
            /// <summary>
            /// An application sends the WM_COPY message to an edit control or combo box to copy the current selection to the clipboard in CF_TEXT format. 
            /// </summary>
            //COPY = 0x0301,
            WM_COPY                         = 0x0301,
            /// <summary>
            /// An application sends a WM_PASTE message to an edit control or combo box to copy the current content of the clipboard to the edit control at the current caret position. Data is inserted only if the clipboard contains data in CF_TEXT format. 
            /// </summary>
            //PASTE = 0x0302,
            WM_PASTE                        = 0x0302,
            /// <summary>
            /// An application sends a WM_CLEAR message to an edit control or combo box to delete (clear) the current selection, if any, from the edit control. 
            /// </summary>
            //CLEAR = 0x0303,
            WM_CLEAR                        = 0x0303,
            /// <summary>
            /// An application sends a WM_UNDO message to an edit control to undo the last operation. When this message is sent to an edit control, the previously deleted text is restored or the previously added text is deleted.
            /// </summary>
            //UNDO = 0x0304,
            WM_UNDO                         = 0x0304,
            /// <summary>
            /// The WM_RENDERFORMAT message is sent to the clipboard owner if it has delayed rendering a specific clipboard format and if an application has requested data in that format. The clipboard owner must render data in the specified format and place it on the clipboard by calling the SetClipboardData function. 
            /// </summary>
            //RENDERFORMAT = 0x0305,
            WM_RENDERFORMAT                 = 0x0305,
            /// <summary>
            /// The WM_RENDERALLFORMATS message is sent to the clipboard owner before it is destroyed, if the clipboard owner has delayed rendering one or more clipboard formats. For the content of the clipboard to remain available to other applications, the clipboard owner must render data in all the formats it is capable of generating, and place the data on the clipboard by calling the SetClipboardData function. 
            /// </summary>
            //RENDERALLFORMATS = 0x0306,
            WM_RENDERALLFORMATS             = 0x0306,
            /// <summary>
            /// The WM_DESTROYCLIPBOARD message is sent to the clipboard owner when a call to the EmptyClipboard function empties the clipboard. 
            /// </summary>
            //DESTROYCLIPBOARD = 0x0307,
            WM_DESTROYCLIPBOARD             = 0x0307,
            /// <summary>
            /// The WM_DRAWCLIPBOARD message is sent to the first window in the clipboard viewer chain when the content of the clipboard changes. This enables a clipboard viewer window to display the new content of the clipboard. 
            /// </summary>
            //DRAWCLIPBOARD = 0x0308,
            WM_DRAWCLIPBOARD                = 0x0308,
            /// <summary>
            /// The WM_PAINTCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area needs repainting. 
            /// </summary>
            //PAINTCLIPBOARD = 0x0309,
            WM_PAINTCLIPBOARD               = 0x0309,
            /// <summary>
            /// The WM_VSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's vertical scroll bar. The owner should scroll the clipboard image and update the scroll bar values. 
            /// </summary>
            //VSCROLLCLIPBOARD = 0x030A,
            WM_VSCROLLCLIPBOARD             = 0x030A,
            /// <summary>
            /// The WM_SIZECLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area has changed size. 
            /// </summary>
            //SIZECLIPBOARD = 0x030B,
            WM_SIZECLIPBOARD                = 0x030B,
            /// <summary>
            /// The WM_ASKCBFORMATNAME message is sent to the clipboard owner by a clipboard viewer window to request the name of a CF_OWNERDISPLAY clipboard format.
            /// </summary>
            //ASKCBFORMATNAME = 0x030C,
            WM_ASKCBFORMATNAME              = 0x030C,
            /// <summary>
            /// The WM_CHANGECBCHAIN message is sent to the first window in the clipboard viewer chain when a window is being removed from the chain. 
            /// </summary>
            //CHANGECBCHAIN = 0x030D,
            WM_CHANGECBCHAIN                = 0x030D,
            /// <summary>
            /// The WM_HSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window. This occurs when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's horizontal scroll bar. The owner should scroll the clipboard image and update the scroll bar values. 
            /// </summary>
            //HSCROLLCLIPBOARD = 0x030E,
            WM_HSCROLLCLIPBOARD             = 0x030E,
            /// <summary>
            /// This message informs a window that it is about to receive the keyboard focus, giving the window the opportunity to realize its logical palette when it receives the focus. 
            /// </summary>
            //QUERYNEWPALETTE = 0x030F,
            WM_QUERYNEWPALETTE              = 0x030F,
            /// <summary>
            /// The WM_PALETTEISCHANGING message informs applications that an application is going to realize its logical palette. 
            /// </summary>
            //PALETTEISCHANGING = 0x0310,
            WM_PALETTEISCHANGING            = 0x0310,
            /// <summary>
            /// This message is sent by the OS to all top-level and overlapped windows after the window with the keyboard focus realizes its logical palette. 
            /// This message enables windows that do not have the keyboard focus to realize their logical palettes and update their client areas.
            /// </summary>
            //PALETTECHANGED = 0x0311,
            WM_PALETTECHANGED               = 0x0311,
            /// <summary>
            /// The WM_HOTKEY message is posted when the user presses a hot key registered by the RegisterHotKey function. The message is placed at the top of the message queue associated with the thread that registered the hot key. 
            /// </summary>
            //HOTKEY = 0x0312,
            WM_HOTKEY                       = 0x0312,
            /// <summary>
            /// The WM_PRINT message is sent to a window to request that it draw itself in the specified device context, most commonly in a printer device context.
            /// </summary>
            //PRINT = 0x0317,
            WM_PRINT                        = 0x0317,
            /// <summary>
            /// The WM_PRINTCLIENT message is sent to a window to request that it draw its client area in the specified device context, most commonly in a printer device context.
            /// </summary>
            //PRINTCLIENT = 0x0318,
            WM_PRINTCLIENT                  = 0x0318,
            /// <summary>
            /// The WM_APPCOMMAND message notifies a window that the user generated an application command event, for example, by clicking an application command button using the mouse or typing an application command key on the keyboard.
            /// </summary>
            //APPCOMMAND = 0x0319,
            WM_APPCOMMAND                   = 0x0319,
            /// <summary>
            /// The WM_THEMECHANGED message is broadcast to every window following a theme change event. Examples of theme change events are the activation of a theme, the deactivation of a theme, or a transition from one theme to another.
            /// </summary>
            //THEMECHANGED = 0x031A,
            WM_THEMECHANGED                 = 0x031A,
            /// <summary>
            /// Sent when the contents of the clipboard have changed.
            /// </summary>
            //CLIPBOARDUPDATE = 0x031D,
            WM_CLIPBOARDUPDATE              = 0x031D,
            /// <summary>
            /// The system will send a window the WM_DWMCOMPOSITIONCHANGED message to indicate that the availability of desktop composition has changed.
            /// </summary>
            //DWMCOMPOSITIONCHANGED = 0x031E,
            WM_DWMCOMPOSITIONCHANGED        = 0x031E,
            /// <summary>
            /// WM_DWMNCRENDERINGCHANGED is called when the non-client area rendering status of a window has changed. Only windows that have set the flag DWM_BLURBEHIND.fTransitionOnMaximized to true will get this message. 
            /// </summary>
            //DWMNCRENDERINGCHANGED = 0x031F,
            WM_DWMNCRENDERINGCHANGED        = 0x031F,
            /// <summary>
            /// Sent to all top-level windows when the colorization color has changed. 
            /// </summary>
            //DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            WM_DWMCOLORIZATIONCOLORCHANGED  = 0x0320,
            /// <summary>
            /// WM_DWMWINDOWMAXIMIZEDCHANGE will let you know when a DWM composed window is maximized. You also have to register for this message as well. You'd have other windowd go opaque when this message is sent.
            /// </summary>
            //DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
            WM_DWMWINDOWMAXIMIZEDCHANGE     = 0x0321,
            /// <summary>
            /// Sent to request extended title bar information. A window receives this message through its WindowProc function.
            /// </summary>
            //GETTITLEBARINFOEX = 0x033F,
            WM_GETTITLEBARINFOEX            = 0x033F,
            //HANDHELDFIRST = 0x0358,
            WM_HANDHELDFIRST                = 0x0358,
            //HANDHELDLAST = 0x035F,
            WM_HANDHELDLAST                 = 0x035F,
            //AFXFIRST = 0x0360,
            WM_AFXFIRST                     = 0x0360,
            //AFXLAST = 0x037F,
            WM_AFXLAST                      = 0x037F,
            //PENWINFIRST = 0x0380,
            WM_PENWINFIRST                  = 0x0380,
            //PENWINLAST = 0x038F,
            WM_PENWINLAST                   = 0x038F,
            /// <summary>
            /// The WM_APP constant is used by applications to help define private messages, usually of the form WM_APP+X, where X is an integer value. 
            /// </summary>
            //APP = 0x8000,
            WM_APP                          = 0x8000,
            /// <summary>
            /// The WM_USER constant is used by applications to help define private messages for use by private window classes, usually of the form WM_USER+X, where X is an integer value. 
            /// </summary>
            //USER = 0x0400,
            WM_USER                         = 0x0400,

            /// <summary>
            /// An application sends the WM_CPL_LAUNCH message to Windows Control Panel to request that a Control Panel application be started. 
            /// </summary>
            CPL_LAUNCH                      = WM_USER + 0x1000,
            /// <summary>
            /// The WM_CPL_LAUNCHED message is sent when a Control Panel application, started by the WM_CPL_LAUNCH message, has closed. The WM_CPL_LAUNCHED message is sent to the window identified by the wParam parameter of the WM_CPL_LAUNCH message that started the application. 
            /// </summary>
            CPL_LAUNCHED                    = WM_USER + 0x1001,
            /// <summary>
            /// WM_SYSTIMER is a well-known yet still undocumented message. Windows uses WM_SYSTIMER for internal actions like scrolling.
            /// </summary>
            SYSTIMER                        = 0x118
        }
        #endregion
    }
}

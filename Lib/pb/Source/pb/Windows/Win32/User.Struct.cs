using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using HANDLE = System.IntPtr;
using HWND = System.IntPtr;
using HDC = System.IntPtr;

//namespace Win32
namespace pb.Windows.Win32
{
    public abstract partial class User
    {
        public abstract partial class Struct
        {
            // ajout
            #region WinUser.h
            /******************************************************************************************************
     * source : WinUser.h in C:\Program Files\Microsoft SDKs\Windows\v6.1\Include\
     * 

        #define INPUT_MOUSE     0
        #define INPUT_KEYBOARD  1
        #define INPUT_HARDWARE  2

        typedef struct tagINPUT {
            DWORD   type;

            union
            {
                MOUSEINPUT      mi;
                KEYBDINPUT      ki;
                HARDWAREINPUT   hi;
            } DUMMYUNIONNAME;
        } INPUT, *PINPUT, FAR* LPINPUT;

        typedef struct tagMOUSEINPUT {
            LONG    dx;
            LONG    dy;
            DWORD   mouseData;
            DWORD   dwFlags;
            DWORD   time;
            ULONG_PTR dwExtraInfo;
        } MOUSEINPUT, *PMOUSEINPUT, FAR* LPMOUSEINPUT;

        typedef struct tagKEYBDINPUT {
            WORD    wVk;
            WORD    wScan;
            DWORD   dwFlags;
            DWORD   time;
            ULONG_PTR dwExtraInfo;
        } KEYBDINPUT, *PKEYBDINPUT, FAR* LPKEYBDINPUT;

        typedef struct tagHARDWAREINPUT {
            DWORD   uMsg;
            WORD    wParamL;
            WORD    wParamH;
        } HARDWAREINPUT, *PHARDWAREINPUT, FAR* LPHARDWAREINPUT;

     * 
     * 
    ******************************************************************************************************/
            #endregion

            #region //struct INPUT
            //public struct INPUT
            //{
            //    public int type;
            //    public MOUSEINPUT mi;
            //}
            #endregion

            #region //struct MOUSEINPUT
            //public struct MOUSEINPUT
            //{
            //    public int dx;
            //    public int dy;
            //    public int mouseData;
            //    public int dwFlags;
            //    public int time;
            //    public int dwExtraInfo;
            //}
            #endregion

            #region struct MOUSEINPUT
            [StructLayout(LayoutKind.Sequential)]
            public struct MOUSEINPUT
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }
            #endregion

            #region struct KEYBDINPUT
            [StructLayout(LayoutKind.Sequential)]
            public struct KEYBDINPUT
            {
                public ushort wVk;
                public ushort wScan;
                public uint dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }
            #endregion

            #region struct HARDWAREINPUT
            [StructLayout(LayoutKind.Sequential)]
            public struct HARDWAREINPUT
            {
                public uint uMsg;
                public ushort wParamL;
                public ushort wParamH;
            }
            #endregion

            #region struct INPUT
            [StructLayout(LayoutKind.Explicit)]
            public struct INPUT
            {
                [FieldOffset(0)]
                public int type;
                [FieldOffset(4)]
                public MOUSEINPUT mi;
                [FieldOffset(4)]
                public KEYBDINPUT ki;
                [FieldOffset(4)]
                public HARDWAREINPUT hi;

                public INPUT(int type)
                {
                    this.type = type;

                    ki.wVk = 0;
                    ki.wScan = 0;
                    ki.dwFlags = 0;
                    ki.time = 0;
                    ki.dwExtraInfo = IntPtr.Zero;

                    mi.dx = 0;
                    mi.dy = 0;
                    mi.mouseData = 0;
                    mi.dwFlags = 0;
                    mi.time = 0;
                    mi.dwExtraInfo = IntPtr.Zero;

                    hi.uMsg = 0;
                    hi.wParamL = 0;
                    hi.wParamH = 0;

                    switch (type)
                    {
                        case User.Const.INPUT_KEYBOARD:
                            ki.wVk = 0;
                            ki.wScan = 0;
                            ki.dwFlags = 0;
                            ki.time = 0;
                            ki.dwExtraInfo = IntPtr.Zero;
                            break;
                        case User.Const.INPUT_MOUSE:
                            mi.dx = 0;
                            mi.dy = 0;
                            mi.mouseData = 0;
                            mi.dwFlags = 0;
                            mi.time = 0;
                            mi.dwExtraInfo = IntPtr.Zero;
                            break;
                        case User.Const.INPUT_HARDWARE:
                            hi.uMsg = 0;
                            hi.wParamL = 0;
                            hi.wParamH = 0;
                            break;
                    }
                }
            }
            #endregion


            #region struct RECT
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }
            #endregion

            #region //struct POINT
            //public struct POINT
            //{
            //    public int x;
            //    public int y;
            //}
            #endregion

            #region struct POINT
            // from http://www.pinvoke.net/default.aspx/Structures.POINT
            // defined in WinDef.h
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int x;
                public int y;

                public POINT(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }

                public static implicit operator System.Drawing.Point(POINT p)
                {
                    return new System.Drawing.Point(p.x, p.y);
                }

                public static implicit operator POINT(System.Drawing.Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }
            #endregion

            #region struct SIZE
            public struct SIZE
            {
                public int cx;
                public int cy;
            }
            #endregion

            #region struct FILETIME
            public struct FILETIME
            {
                public int dwLowDateTime;
                public int dwHighDateTime;
            }
            #endregion

            #region struct SYSTEMTIME
            public struct SYSTEMTIME
            {
                public short wYear;
                public short wMonth;
                public short wDayOfWeek;
                public short wDay;
                public short wHour;
                public short wMinute;
                public short wSecond;
                public short wMilliseconds;
            }
            #endregion

            #region struct WINDOWINFO
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWINFO
            {
                public uint cbSize;
                public RECT rcWindow;
                public RECT rcClient;
                public uint dwStyle;
                public uint dwExStyle;
                public uint dwWindowStatus;
                public uint cxWindowBorders;
                public uint cyWindowBorders;
                public ushort atomWindowType;
                public ushort wCreatorVersion;
            }
            #endregion


            #region class KBDLLHOOKSTRUCT
            // from pinvoke.net (http://www.pinvoke.net/default.aspx/Structures/KBDLLHOOKSTRUCT.html)
            // WinUser.h
            [StructLayout(LayoutKind.Sequential)]
            public class KBDLLHOOKSTRUCT
            {
                public UInt32 vkCode;
                public UInt32 ScanCode;
                public UInt32 Flags;
                public UInt32 Time;
                public IntPtr dwExtraInfo;
            }
            #endregion

            #region class MSLLHOOKSTRUCT
            // from pinvoke.net (http://www.pinvoke.net/default.aspx/Structures/MSLLHOOKSTRUCT.html)
            // WinUser.h
            [StructLayout(LayoutKind.Sequential)]
            public class MSLLHOOKSTRUCT
            {
                public POINT pt;
                public int mouseData;
                public int flags;
                public int time;
                public IntPtr dwExtraInfo;
            }
            #endregion


            #region struct GUITHREADINFO
            [Serializable, StructLayout(LayoutKind.Sequential)]
            public struct GUITHREADINFO
            {
                /// <summary>Specifies the size of this structure, in bytes</summary>
                public uint cbSize;
                /// <summary>Specifies the thread state.</summary>
                public uint flags;
                /// <summary>Handle to the active window within the thread.</summary>
                public IntPtr hwndActive;
                /// <summary>Handle to the window that has the keyboard focus.</summary>
                public IntPtr hwndFocus;
                /// <summary>Handle to the window that has captured the mouse.</summary>
                public IntPtr hwndCapture;
                /// <summary>Handle to the window that owns any active menus.</summary>
                public IntPtr hwndMenuOwner;
                /// <summary>Handle to the window in a move or size loop.</summary>
                public IntPtr hwndMoveSize;
                /// <summary>Handle to the window that is displaying the caret.</summary>
                public IntPtr hwndCaret;
                /// <summary>A RECT structure that describes the caret's bounding rectangle, in client coordinates, relative to the window specified by the hwndCaret member.</summary>
                public RECT rcCaret;
            }
            #endregion
            // fin ajout

            #region struct CBTACTIVATESTRUCT
            public struct CBTACTIVATESTRUCT
            {
                public int fMouse;
                public HWND hwndActive;
            }
            #endregion

            #region struct EVENTMSG
            public struct EVENTMSG
            {
                public int message;
                public int paramL;
                public int paramH;
                public int time;
                public HWND hwnd;
            }
            #endregion

            #region //struct CWPSTRUCT
            //public struct CWPSTRUCT
            //{
            //    public int lParam;
            //    public int wParam;
            //    public int message;
            //    public HWND hwnd;
            //}
            #endregion

            #region struct CWPSTRUCT
            // windows definition in WinUser.h, definition from http://www.pinvoke.net/default.aspx/Structures/CWPSTRUCT.html
            [StructLayout(LayoutKind.Sequential)]
            public struct CWPSTRUCT
            {
                public IntPtr lParam;
                public IntPtr wParam;
                public int message;
                public IntPtr hwnd;
            }
            #endregion

            #region struct CWPRETSTRUCT
            // windows definition in WinUser.h, definition perso (not found in pinvoke)
            [StructLayout(LayoutKind.Sequential)]
            public struct CWPRETSTRUCT
            {
                public IntPtr lResult;
                public IntPtr lParam;
                public IntPtr wParam;
                public int message;
                public IntPtr hwnd;
            }
            #endregion

            #region struct DEBUGHOOKINFO
            public struct DEBUGHOOKINFO
            {
                public HANDLE hModuleHook;
                public int Reserved;
                public int lParam;
                public int wParam;
                public int code;
            }
            #endregion

            #region struct MOUSEHOOKSTRUCT
            public struct MOUSEHOOKSTRUCT
            {
                public POINT pt;
                public HWND hwnd;
                public int wHitTestCode;
                public int dwExtraInfo;
            }
            #endregion

            #region struct MINMAXINFO
            public struct MINMAXINFO
            {
                public POINT ptReserved;
                public POINT ptMaxSize;
                public POINT ptMaxPosition;
                public POINT ptMinTrackSize;
                public POINT ptMaxTrackSize;
            }
            #endregion

            #region struct COPYDATASTRUCT
            public struct COPYDATASTRUCT
            {
                public int dwData;
                public int cbData;
                public int lpData;
            }
            #endregion

            #region struct WINDOWPOS
            public struct WINDOWPOS
            {
                public HWND hwnd;
                public HWND hwndInsertAfter;
                public int x;
                public int y;
                public int cx;
                public int cy;
                public int flags;
            }
            #endregion

            #region struct ACCEL
            public struct ACCEL
            {
                public byte fVirt;
                public short key;
                public short cmd;
            }
            #endregion

            #region struct PAINTSTRUCT
            public struct PAINTSTRUCT
            {
                public HDC hdc;
                public int fErase;
                public RECT rcPaint;
                public int fRestore;
                public int fIncUpdate;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public byte rgbReserved;
            }
            #endregion

            #region struct CREATESTRUCT
            public struct CREATESTRUCT
            {
                public int lpCreateParams;
                public HANDLE hInstance;
                public HANDLE hMenu;
                public HWND hwndParent;
                public int cy;
                public int cx;
                public int y;
                public int x;
                public int style;
                public string lpszName;
                public string lpszClass;
                public int ExStyle;
            }
            #endregion

            #region struct CBT_CREATEWND
            public struct CBT_CREATEWND
            {
                public CREATESTRUCT lpcs;
                public HWND hwndInsertAfter;
            }
            #endregion

            #region struct WINDOWPLACEMENT
            public struct WINDOWPLACEMENT
            {
                public int Length;
                public int flags;
                public int showCmd;
                public POINT ptMinPosition;
                public POINT ptMaxPosition;
                public RECT rcNormalPosition;
            }
            #endregion

            #region struct MEASUREITEMSTRUCT
            public struct MEASUREITEMSTRUCT
            {
                public int CtlType;
                public int CtlID;
                public int itemID;
                public int itemWidth;
                public int itemHeight;
                public int itemData;
            }
            #endregion

            #region struct DRAWITEMSTRUCT
            public struct DRAWITEMSTRUCT
            {
                public int CtlType;
                public int CtlID;
                public int itemID;
                public int itemAction;
                public int itemState;
                public HWND hwndItem;
                public HDC hdc;
                public RECT rcItem;
                public int itemData;
            }
            #endregion

            #region struct DELETEITEMSTRUCT
            public struct DELETEITEMSTRUCT
            {
                public int CtlType;
                public int CtlID;
                public int itemID;
                public HWND hwndItem;
                public int itemData;
            }
            #endregion

            #region struct COMPAREITEMSTRUCT
            public struct COMPAREITEMSTRUCT
            {
                public int CtlType;
                public int CtlID;
                public HWND hwndItem;
                public int itemID1;
                public int itemData1;
                public int itemID2;
                public int itemData2;
            }
            #endregion

            #region //struct MSG
            //public struct MSG
            //{
            //    public HWND hwnd;
            //    public int message;
            //    public int wParam;
            //    public int lParam;
            //    public int time;
            //    public POINT pt;
            //}
            #endregion

            #region struct MSG
            // from http://www.pinvoke.net/default.aspx/Structures.MSG
            // defined in WinUser.h
            [StructLayout(LayoutKind.Sequential)]
            public struct MSG
            {
                public IntPtr hwnd;
                public UInt32 message;
                public IntPtr wParam;
                public IntPtr lParam;
                public UInt32 time;
                public POINT pt;
            }
            #endregion

            #region struct WNDCLASS
            public struct WNDCLASS
            {
                public int style;
                public int lpfnwndproc;
                public int cbClsextra;
                public int cbWndExtra2;
                public HANDLE hInstance;
                public HANDLE hIcon;
                public HANDLE hCursor;
                public HANDLE hbrBackground;
                public string lpszMenuName;
                public string lpszClassName;
            }
            #endregion

            #region struct DLGTEMPLATE
            public struct DLGTEMPLATE
            {
                public int style;
                public int dwExtendedStyle;
                public short cdit;
                public short x;
                public short y;
                public short cx;
                public short cy;
            }
            #endregion

            #region struct DLGITEMTEMPLATE
            public struct DLGITEMTEMPLATE
            {
                public int style;
                public int dwExtendedStyle;
                public short x;
                public short y;
                public short cx;
                public short cy;
                public short id;
            }
            #endregion

            #region struct MENUITEMTEMPLATEHEADER
            public struct MENUITEMTEMPLATEHEADER
            {
                public short versionNumber;
                public short offset;
            }
            #endregion

            #region struct MENUITEMTEMPLATE
            public struct MENUITEMTEMPLATE
            {
                public short mtOption;
                public short mtID;
                public byte mtString;
            }
            #endregion

            #region struct ICONINFO
            public struct ICONINFO
            {
                public int fIcon;
                public int xHotspot;
                public int yHotspot;
                public HANDLE hbmMask;
                public HANDLE hbmColor;
            }
            #endregion

            #region struct MDICREATESTRUCT
            public struct MDICREATESTRUCT
            {
                public string szClass;
                public string szTitle;
                public HWND hOwner;
                public int x;
                public int y;
                public int cx;
                public int cy;
                public int style;
                public int lParam;
            }
            #endregion

            #region struct CLIENTCREATESTRUCT
            public struct CLIENTCREATESTRUCT
            {
                public HANDLE hWindowMenu;
                public int idFirstChild;
            }
            #endregion

            #region struct MULTIKEYHELP
            public struct MULTIKEYHELP
            {
                public int mkSize;
                public byte mkKeylist;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 253)]
                public string szKeyphrase;
            }
            #endregion

            #region struct HELPWININFO
            public struct HELPWININFO
            {
                public int wStructSize;
                public int x;
                public int y;
                public int dx;
                public int dy;
                public int wMax;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
                public string rgchMember;
            }
            #endregion

            #region struct DDEACK
            public struct DDEACK
            {
                public short bAppReturnCode;
                public short Reserved;
                public short fbusy;
                public short fack;
            }
            #endregion

            #region struct DDEADVISE
            public struct DDEADVISE
            {
                public short Reserved;
                public short fDeferUpd;
                public short fAckReq;
                public short cfFormat;
            }
            #endregion

            #region struct DDEDATA
            public struct DDEDATA
            {
                public short unused;
                public short fresponse;
                public short fRelease;
                public short Reserved;
                public short fAckReq;
                public short cfFormat;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
                public byte Value;
            }
            #endregion

            #region struct DDEPOKE
            public struct DDEPOKE
            {
                public short unused;
                public short fRelease;
                public short fReserved;
                public short cfFormat;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
                public byte Value;
            }
            #endregion

            #region struct DDELN
            public struct DDELN
            {
                public short unused;
                public short fRelease;
                public short fDeferUpd;
                public short fAckReq;
                public short cfFormat;
            }
            #endregion

            #region struct DDEUP
            public struct DDEUP
            {
                public short unused;
                public short fAck;
                public short fRelease;
                public short fReserved;
                public short fAckReq;
                public short cfFormat;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
                public byte rgb;
            }
            #endregion

            #region struct HSZPAIR
            public struct HSZPAIR
            {
                public HANDLE hszSvc;
                public HANDLE hszTopic;
            }
            #endregion

            #region struct SECURITY_QUALITY_OF_SERVICE
            public struct SECURITY_QUALITY_OF_SERVICE
            {
                public int Length;
                public short Impersonationlevel;
                public short ContextTrackingMode;
                public int EffectiveOnly;
            }
            #endregion

            #region struct CONVCONTEXT
            public struct CONVCONTEXT
            {
                public int cb;
                public int wFlags;
                public int wCountryID;
                public int iCodePage;
                public int dwLangID;
                public int dwSecurity;
                public SECURITY_QUALITY_OF_SERVICE qos;
            }
            #endregion

            #region struct CONVINFO
            public struct CONVINFO
            {
                public int cb;
                public HANDLE hUser;
                public HANDLE hConvPartner;
                public HANDLE hszSvcPartner;
                public HANDLE hszServiceReq;
                public HANDLE hszTopic;
                public HANDLE hszItem;
                public int wFmt;
                public int wType;
                public int wStatus;
                public int wConvst;
                public int wLastError;
                public HANDLE hConvList;
                public CONVCONTEXT ConvCtxt;
                public HWND hwnd;
                public HWND hwndPartner;
            }
            #endregion

            #region struct DDEML_MSG_HOOK_DATA
            public struct DDEML_MSG_HOOK_DATA
            {
                public int uiLo;
                public int uiHi;
                public int cbData;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public int Data;
            }
            #endregion

            #region struct MONMSGSTRUCT
            public struct MONMSGSTRUCT
            {
                public int cb;
                public HWND hwndTo;
                public int dwTime;
                public HANDLE htask;
                public int wMsg;
                public int wParam;
                public int lParam;
                public DDEML_MSG_HOOK_DATA dmhd;
            }
            #endregion

            #region struct MONCBSTRUCT
            public struct MONCBSTRUCT
            {
                public int cb;
                public int dwTime;
                public HANDLE htask;
                public int dwRet;
                public int wType;
                public int wFmt;
                public HANDLE hConv;
                public HANDLE hsz1;
                public HANDLE hsz2;
                public HANDLE hData;
                public int dwData1;
                public int dwData2;
                public CONVCONTEXT cc;
                public int cbData;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public int Data;
            }
            #endregion

            #region struct MONHSZSTRUCT
            public struct MONHSZSTRUCT
            {
                public int cb;
                public int fsAction;
                public int dwTime;
                public HANDLE hsz;
                public HANDLE htask;
                public byte str;
            }
            #endregion

            #region struct MONERRSTRUCT
            public struct MONERRSTRUCT
            {
                public int cb;
                public int wLastError;
                public int dwTime;
                public HANDLE htask;
            }
            #endregion

            #region struct MONLINKSTRUCT
            public struct MONLINKSTRUCT
            {
                public int cb;
                public int dwTime;
                public HANDLE htask;
                public int fEstablished;
                public int fNoData;
                public HANDLE hszSvc;
                public HANDLE hszTopic;
                public HANDLE hszItem;
                public int wFmt;
                public int fServer;
                public HANDLE hConvServer;
                public HANDLE hConvClient;
            }
            #endregion

            #region struct MONCONVSTRUCT
            public struct MONCONVSTRUCT
            {
                public int cb;
                public int fConnect;
                public int dwTime;
                public HANDLE htask;
                public HANDLE hszSvc;
                public HANDLE hszTopic;
                public HANDLE hConvClient;
                public HANDLE hConvServer;
            }
            #endregion

            #region struct DRAWTEXTPARAMS
            public struct DRAWTEXTPARAMS
            {
                public int cbSize;
                public int iTabLength;
                public int iLeftMargin;
                public int iRightMargin;
                public int uiLengthDrawn;
            }
            #endregion

            #region struct MENUITEMINFO
            public struct MENUITEMINFO
            {
                public int cbSize;
                public int fMask;
                public int fType;
                public int fState;
                public int wID;
                public HANDLE hSubMenu;
                public HANDLE hbmpChecked;
                public HANDLE hbmpUnchecked;
                public int dwItemData;
                public string dwTypeData;
                public int cch;
            }
            #endregion

            #region struct SCROLLINFO
            public struct SCROLLINFO
            {
                public int cbSize;
                public int fMask;
                public int nMin;
                public int nMax;
                public int nPage;
                public int nPos;
                public int nTrackPos;
            }
            #endregion

            #region struct MSGBOXPARAMS
            public struct MSGBOXPARAMS
            {
                public int cbSize;
                public HWND hwndOwner;
                public HANDLE hInstance;
                public string lpszText;
                public string lpszCaption;
                public int dwStyle;
                public string lpszIcon;
                public int dwContextHelpId;
                public int lpfnMsgBoxCallback;
                public int dwLanguageId;
            }
            #endregion

            #region struct WNDCLASSEX
            public struct WNDCLASSEX
            {
                public int cbSize;
                public int style;
                public int lpfnWndProc;
                public int cbClsExtra;
                public int cbWndExtra;
                public HANDLE hInstance;
                public HANDLE hIcon;
                public HANDLE hCursor;
                public HANDLE hbrBackground;
                public string lpszMenuName;
                public string lpszClassName;
                public HANDLE hIconSm;
            }
            #endregion

            #region struct TPMPARAMS
            public struct TPMPARAMS
            {
                public int cbSize;
                public RECT rcExclude;
            }
            #endregion
        }
    }
}

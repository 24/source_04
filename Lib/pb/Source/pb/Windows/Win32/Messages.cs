using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace Win32
namespace pb.Windows.Win32
{
    #region enum MessageCategory
    [Flags]
    public enum MessageCategory : ulong
    {
        // category from spy++
        Unknown         = 0x000000000001,
        Keyboard        = 0x000000000002,
        Mouse           = 0x000000000004,
        Clipboard       = 0x000000000008,
        Dialog          = 0x000000000010,
        Pen             = 0x000000000020,
        EditField       = 0x000000000040,
        Button          = 0x000000000080,
        ListView        = 0x000000000100,
        TreeView        = 0x000000000200,
        ToolTip         = 0x000000000400,
        Toolbar         = 0x000000000800,
        Header          = 0x000000001000,
        Statusbar       = 0x000000002000,
        TabCntrl        = 0x000000004000,
        NonClient       = 0x000000008000,
        IME             = 0x000000010000,
        MDI             = 0x000000020000,
        DDE             = 0x000000040000,
        AFX_MFC         = 0x000000080000,
        Listbox         = 0x000000100000,
        Combobox        = 0x000000200000,
        Hotkey          = 0x000000400000,
        UpDown          = 0x000000800000,
        Progress        = 0x000001000000,
        Trackbar        = 0x000002000000,
        MonthCal        = 0x000004000000,
        DateTime        = 0x000008000000,
        General         = 0x000010000000,
        Registered      = 0x000020000000,
        //Unknown         = 0x000040000000,
        WM_User         = 0x000040000000,
        Scrollbar       = 0x000080000000,
        Static          = 0x000100000000,
        Animate         = 0x000200000000,
        ComboEx         = 0x000400000000,
        Pager           = 0x000800000000,
        ReBar           = 0x001000000000,
        IPAddress       = 0x002000000000,
        SysLink         = 0x004000000000
    }
    #endregion

    #region class KeystrokeMessageFlags
    public class KeystrokeMessageFlags
    {
        // doc msdn http://msdn.microsoft.com/en-us/library/ms646267(VS.85).aspx
        /// <summary>Specifies the repeat count. The value is the number of times the keystroke is repeated as a result of the user's holding down the key. (lParam bit 0-15)</summary>
        public int RepeatCount;
        /// <summary>Specifies the scan code. The value depends on the OEM. (lParam bit 16-23)</summary>
        public int ScanCode;
        /// <summary>Specifies whether the key is an extended key, such as a function key or a key on the numeric keypad. The value is 1 if the key is an extended key; otherwise, it is 0. (lParam bit 24)</summary>
        public bool ExtendedKey;
        /// <summary>Specifies the context code. The value is 1 if the ALT key is down; otherwise, it is 0. (lParam bit 29)</summary>
        public byte ContextCode;        // always 0 for WM_KEYDOWN
        /// <summary>Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up. (lParam bit 30)</summary>
        public byte PreviousKeyState;   // 1 : key was down before the message is sent, 0 : key was up
        /// <summary>Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released. (lParam bit 31)</summary>
        public byte TransitionState;    // always 0 for WM_KEYDOWN
    }
    #endregion

    #region class KeyboardState
    public class KeyboardState
    {
        public bool Shift;
        public bool LShift;
        public bool RShift;
        public bool Ctrl;
        public bool LCtrl;
        public bool RCtrl;
        public bool Alt;
        public bool LAlt;
        public bool RAlt;
        public bool LWin;
        public bool RWin;
        public bool CapsLock;
        public bool NumLock;
        public bool ScrollLock;
        public byte[] VKState;

        #region ToString
        public override string ToString()
        {
            string s = "";
            if (this.CapsLock) s = "M"; else s = "m";
            if (this.NumLock) s += " N"; else s += " n";
            if (this.ScrollLock) s += " S"; else s += " s";

            if (this.Shift) s += " Shift";
            if (this.LShift) s += " LShift";
            if (this.RShift) s += " RShift";
            if (this.Ctrl) s += " Ctrl";
            if (this.LCtrl) s += " LCtrl";
            if (this.RCtrl) s += " RCtrl";
            if (this.Alt) s += " Alt";
            if (this.LAlt) s += " LAlt";
            if (this.RAlt) s += " RAlt";
            if (this.LWin) s += " LWin";
            if (this.RWin) s += " RWin";

            return s;
        }
        #endregion
    }
    #endregion

    #region class Messages
    public class Messages
    {
        #region GetWindowMessage
        #pragma warning disable 612
        public static User.WM GetWindowMessage(int msg)
        {
            switch (msg)
            {
                case (int)User.WM.WM_NULL:                           return User.WM.WM_NULL;
                case (int)User.WM.WM_CREATE:                         return User.WM.WM_CREATE;
                case (int)User.WM.WM_DESTROY:                        return User.WM.WM_DESTROY;
                case (int)User.WM.WM_MOVE:                           return User.WM.WM_MOVE;
                case (int)User.WM.WM_SIZE:                           return User.WM.WM_SIZE;
                case (int)User.WM.WM_ACTIVATE:                       return User.WM.WM_ACTIVATE;
                case (int)User.WM.WM_SETFOCUS:                       return User.WM.WM_SETFOCUS;
                case (int)User.WM.WM_KILLFOCUS:                      return User.WM.WM_KILLFOCUS;
                case (int)User.WM.WM_ENABLE:                         return User.WM.WM_ENABLE;
                case (int)User.WM.WM_SETREDRAW:                      return User.WM.WM_SETREDRAW;
                case (int)User.WM.WM_SETTEXT:                        return User.WM.WM_SETTEXT;
                case (int)User.WM.WM_GETTEXT:                        return User.WM.WM_GETTEXT;
                case (int)User.WM.WM_GETTEXTLENGTH:                  return User.WM.WM_GETTEXTLENGTH;
                case (int)User.WM.WM_PAINT:                          return User.WM.WM_PAINT;
                case (int)User.WM.WM_CLOSE:                          return User.WM.WM_CLOSE;
                case (int)User.WM.WM_QUERYENDSESSION:                return User.WM.WM_QUERYENDSESSION;
                case (int)User.WM.WM_QUERYOPEN:                      return User.WM.WM_QUERYOPEN;
                case (int)User.WM.WM_ENDSESSION:                     return User.WM.WM_ENDSESSION;
                case (int)User.WM.WM_QUIT:                           return User.WM.WM_QUIT;
                case (int)User.WM.WM_ERASEBKGND:                     return User.WM.WM_ERASEBKGND;
                case (int)User.WM.WM_SYSCOLORCHANGE:                 return User.WM.WM_SYSCOLORCHANGE;
                case (int)User.WM.WM_SHOWWINDOW:                     return User.WM.WM_SHOWWINDOW;
                case (int)User.WM.WM_WININICHANGE:                   return User.WM.WM_WININICHANGE;
                //case (int)User.WM.WM_SETTINGCHANGE:                  return User.WM.WM_SETTINGCHANGE;  // idem WM_WININICHANGE
                case (int)User.WM.WM_DEVMODECHANGE:                  return User.WM.WM_DEVMODECHANGE;
                case (int)User.WM.WM_ACTIVATEAPP:                    return User.WM.WM_ACTIVATEAPP;
                case (int)User.WM.WM_FONTCHANGE:                     return User.WM.WM_FONTCHANGE;
                case (int)User.WM.WM_TIMECHANGE:                     return User.WM.WM_TIMECHANGE;
                case (int)User.WM.WM_CANCELMODE:                     return User.WM.WM_CANCELMODE;
                case (int)User.WM.WM_SETCURSOR:                      return User.WM.WM_SETCURSOR;
                case (int)User.WM.WM_MOUSEACTIVATE:                  return User.WM.WM_MOUSEACTIVATE;
                case (int)User.WM.WM_CHILDACTIVATE:                  return User.WM.WM_CHILDACTIVATE;
                case (int)User.WM.WM_QUEUESYNC:                      return User.WM.WM_QUEUESYNC;
                case (int)User.WM.WM_GETMINMAXINFO:                  return User.WM.WM_GETMINMAXINFO;
                case (int)User.WM.WM_PAINTICON:                      return User.WM.WM_PAINTICON;
                case (int)User.WM.WM_ICONERASEBKGND:                 return User.WM.WM_ICONERASEBKGND;
                case (int)User.WM.WM_NEXTDLGCTL:                     return User.WM.WM_NEXTDLGCTL;
                case (int)User.WM.WM_SPOOLERSTATUS:                  return User.WM.WM_SPOOLERSTATUS;
                case (int)User.WM.WM_DRAWITEM:                       return User.WM.WM_DRAWITEM;
                case (int)User.WM.WM_MEASUREITEM:                    return User.WM.WM_MEASUREITEM;
                case (int)User.WM.WM_DELETEITEM:                     return User.WM.WM_DELETEITEM;
                case (int)User.WM.WM_VKEYTOITEM:                     return User.WM.WM_VKEYTOITEM;
                case (int)User.WM.WM_CHARTOITEM:                     return User.WM.WM_CHARTOITEM;
                case (int)User.WM.WM_SETFONT:                        return User.WM.WM_SETFONT;
                case (int)User.WM.WM_GETFONT:                        return User.WM.WM_GETFONT;
                case (int)User.WM.WM_SETHOTKEY:                      return User.WM.WM_SETHOTKEY;
                case (int)User.WM.WM_GETHOTKEY:                      return User.WM.WM_GETHOTKEY;
                case (int)User.WM.WM_QUERYDRAGICON:                  return User.WM.WM_QUERYDRAGICON;
                case (int)User.WM.WM_COMPAREITEM:                    return User.WM.WM_COMPAREITEM;
                case (int)User.WM.WM_GETOBJECT:                      return User.WM.WM_GETOBJECT;
                case (int)User.WM.WM_COMPACTING:                     return User.WM.WM_COMPACTING;
                case (int)User.WM.WM_COMMNOTIFY:                     return User.WM.WM_COMMNOTIFY;
                case (int)User.WM.WM_WINDOWPOSCHANGING:              return User.WM.WM_WINDOWPOSCHANGING;
                case (int)User.WM.WM_WINDOWPOSCHANGED:               return User.WM.WM_WINDOWPOSCHANGED;
                case (int)User.WM.WM_POWER:                          return User.WM.WM_POWER;
                case (int)User.WM.WM_COPYDATA:                       return User.WM.WM_COPYDATA;
                case (int)User.WM.WM_CANCELJOURNAL:                  return User.WM.WM_CANCELJOURNAL;
                case (int)User.WM.WM_NOTIFY:                         return User.WM.WM_NOTIFY;
                case (int)User.WM.WM_INPUTLANGCHANGEREQUEST:         return User.WM.WM_INPUTLANGCHANGEREQUEST;
                case (int)User.WM.WM_INPUTLANGCHANGE:                return User.WM.WM_INPUTLANGCHANGE;
                case (int)User.WM.WM_TCARD:                          return User.WM.WM_TCARD;
                case (int)User.WM.WM_HELP:                           return User.WM.WM_HELP;
                case (int)User.WM.WM_USERCHANGED:                    return User.WM.WM_USERCHANGED;
                case (int)User.WM.WM_NOTIFYFORMAT:                   return User.WM.WM_NOTIFYFORMAT;
                case (int)User.WM.WM_CONTEXTMENU:                    return User.WM.WM_CONTEXTMENU;
                case (int)User.WM.WM_STYLECHANGING:                  return User.WM.WM_STYLECHANGING;
                case (int)User.WM.WM_STYLECHANGED:                   return User.WM.WM_STYLECHANGED;
                case (int)User.WM.WM_DISPLAYCHANGE:                  return User.WM.WM_DISPLAYCHANGE;
                case (int)User.WM.WM_GETICON:                        return User.WM.WM_GETICON;
                case (int)User.WM.WM_SETICON:                        return User.WM.WM_SETICON;
                case (int)User.WM.WM_NCCREATE:                       return User.WM.WM_NCCREATE;
                case (int)User.WM.WM_NCDESTROY:                      return User.WM.WM_NCDESTROY;
                case (int)User.WM.WM_NCCALCSIZE:                     return User.WM.WM_NCCALCSIZE;
                case (int)User.WM.WM_NCHITTEST:                      return User.WM.WM_NCHITTEST;
                case (int)User.WM.WM_NCPAINT:                        return User.WM.WM_NCPAINT;
                case (int)User.WM.WM_NCACTIVATE:                     return User.WM.WM_NCACTIVATE;
                case (int)User.WM.WM_GETDLGCODE:                     return User.WM.WM_GETDLGCODE;
                case (int)User.WM.WM_SYNCPAINT:                      return User.WM.WM_SYNCPAINT;
                case (int)User.WM.WM_NCMOUSEMOVE:                    return User.WM.WM_NCMOUSEMOVE;
                case (int)User.WM.WM_NCLBUTTONDOWN:                  return User.WM.WM_NCLBUTTONDOWN;
                case (int)User.WM.WM_NCLBUTTONUP:                    return User.WM.WM_NCLBUTTONUP;
                case (int)User.WM.WM_NCLBUTTONDBLCLK:                return User.WM.WM_NCLBUTTONDBLCLK;
                case (int)User.WM.WM_NCRBUTTONDOWN:                  return User.WM.WM_NCRBUTTONDOWN;
                case (int)User.WM.WM_NCRBUTTONUP:                    return User.WM.WM_NCRBUTTONUP;
                case (int)User.WM.WM_NCRBUTTONDBLCLK:                return User.WM.WM_NCRBUTTONDBLCLK;
                case (int)User.WM.WM_NCMBUTTONDOWN:                  return User.WM.WM_NCMBUTTONDOWN;
                case (int)User.WM.WM_NCMBUTTONUP:                    return User.WM.WM_NCMBUTTONUP;
                case (int)User.WM.WM_NCMBUTTONDBLCLK:                return User.WM.WM_NCMBUTTONDBLCLK;
                case (int)User.WM.WM_NCXBUTTONDOWN:                  return User.WM.WM_NCXBUTTONDOWN;
                case (int)User.WM.WM_NCXBUTTONUP:                    return User.WM.WM_NCXBUTTONUP;
                case (int)User.WM.WM_NCXBUTTONDBLCLK:                return User.WM.WM_NCXBUTTONDBLCLK;
                case (int)User.WM.WM_INPUT_DEVICE_CHANGE:            return User.WM.WM_INPUT_DEVICE_CHANGE;
                case (int)User.WM.WM_INPUT:                          return User.WM.WM_INPUT;
                //case (int)User.WM.WM_KEYFIRST:                       return User.WM.WM_KEYFIRST; // idem WM_KEYDOWN
                case (int)User.WM.WM_KEYDOWN:                        return User.WM.WM_KEYDOWN;
                case (int)User.WM.WM_KEYUP:                          return User.WM.WM_KEYUP;
                case (int)User.WM.WM_CHAR:                           return User.WM.WM_CHAR;
                case (int)User.WM.WM_DEADCHAR:                       return User.WM.WM_DEADCHAR;
                case (int)User.WM.WM_SYSKEYDOWN:                     return User.WM.WM_SYSKEYDOWN;
                case (int)User.WM.WM_SYSKEYUP:                       return User.WM.WM_SYSKEYUP;
                case (int)User.WM.WM_SYSCHAR:                        return User.WM.WM_SYSCHAR;
                case (int)User.WM.WM_SYSDEADCHAR:                    return User.WM.WM_SYSDEADCHAR;
                case (int)User.WM.WM_UNICHAR:                        return User.WM.WM_UNICHAR;
                //case (int)User.WM.WM_KEYLAST:                        return User.WM.WM_KEYLAST; // idem WM_UNICHAR
                case (int)User.WM.WM_IME_STARTCOMPOSITION:           return User.WM.WM_IME_STARTCOMPOSITION;
                case (int)User.WM.WM_IME_ENDCOMPOSITION:             return User.WM.WM_IME_ENDCOMPOSITION;
                case (int)User.WM.WM_IME_COMPOSITION:                return User.WM.WM_IME_COMPOSITION;
                //case (int)User.WM.WM_IME_KEYLAST:                    return User.WM.WM_IME_KEYLAST; // WM_IME_COMPOSITION
                case (int)User.WM.WM_INITDIALOG:                     return User.WM.WM_INITDIALOG;
                case (int)User.WM.WM_COMMAND:                        return User.WM.WM_COMMAND;
                case (int)User.WM.WM_SYSCOMMAND:                     return User.WM.WM_SYSCOMMAND;
                case (int)User.WM.WM_TIMER:                          return User.WM.WM_TIMER;
                case (int)User.WM.WM_HSCROLL:                        return User.WM.WM_HSCROLL;
                case (int)User.WM.WM_VSCROLL:                        return User.WM.WM_VSCROLL;
                case (int)User.WM.WM_INITMENU:                       return User.WM.WM_INITMENU;
                case (int)User.WM.WM_INITMENUPOPUP:                  return User.WM.WM_INITMENUPOPUP;
                case (int)User.WM.WM_MENUSELECT:                     return User.WM.WM_MENUSELECT;
                case (int)User.WM.WM_MENUCHAR:                       return User.WM.WM_MENUCHAR;
                case (int)User.WM.WM_ENTERIDLE:                      return User.WM.WM_ENTERIDLE;
                case (int)User.WM.WM_MENURBUTTONUP:                  return User.WM.WM_MENURBUTTONUP;
                case (int)User.WM.WM_MENUDRAG:                       return User.WM.WM_MENUDRAG;
                case (int)User.WM.WM_MENUGETOBJECT:                  return User.WM.WM_MENUGETOBJECT;
                case (int)User.WM.WM_UNINITMENUPOPUP:                return User.WM.WM_UNINITMENUPOPUP;
                case (int)User.WM.WM_MENUCOMMAND:                    return User.WM.WM_MENUCOMMAND;
                case (int)User.WM.WM_CHANGEUISTATE:                  return User.WM.WM_CHANGEUISTATE;
                case (int)User.WM.WM_UPDATEUISTATE:                  return User.WM.WM_UPDATEUISTATE;
                case (int)User.WM.WM_QUERYUISTATE:                   return User.WM.WM_QUERYUISTATE;
                case (int)User.WM.WM_CTLCOLORMSGBOX:                 return User.WM.WM_CTLCOLORMSGBOX;
                case (int)User.WM.WM_CTLCOLOREDIT:                   return User.WM.WM_CTLCOLOREDIT;
                case (int)User.WM.WM_CTLCOLORLISTBOX:                return User.WM.WM_CTLCOLORLISTBOX;
                case (int)User.WM.WM_CTLCOLORBTN:                    return User.WM.WM_CTLCOLORBTN;
                case (int)User.WM.WM_CTLCOLORDLG:                    return User.WM.WM_CTLCOLORDLG;
                case (int)User.WM.WM_CTLCOLORSCROLLBAR:              return User.WM.WM_CTLCOLORSCROLLBAR;
                case (int)User.WM.WM_CTLCOLORSTATIC:                 return User.WM.WM_CTLCOLORSTATIC;
                //case (int)User.WM.WM_MOUSEFIRST:                     return User.WM.WM_MOUSEFIRST; idem WM_MOUSEMOVE
                case (int)User.WM.WM_MOUSEMOVE:                      return User.WM.WM_MOUSEMOVE;
                case (int)User.WM.WM_LBUTTONDOWN:                    return User.WM.WM_LBUTTONDOWN;
                case (int)User.WM.WM_LBUTTONUP:                      return User.WM.WM_LBUTTONUP;
                case (int)User.WM.WM_LBUTTONDBLCLK:                  return User.WM.WM_LBUTTONDBLCLK;
                case (int)User.WM.WM_RBUTTONDOWN:                    return User.WM.WM_RBUTTONDOWN;
                case (int)User.WM.WM_RBUTTONUP:                      return User.WM.WM_RBUTTONUP;
                case (int)User.WM.WM_RBUTTONDBLCLK:                  return User.WM.WM_RBUTTONDBLCLK;
                case (int)User.WM.WM_MBUTTONDOWN:                    return User.WM.WM_MBUTTONDOWN;
                case (int)User.WM.WM_MBUTTONUP:                      return User.WM.WM_MBUTTONUP;
                case (int)User.WM.WM_MBUTTONDBLCLK:                  return User.WM.WM_MBUTTONDBLCLK;
                case (int)User.WM.WM_MOUSEWHEEL:                     return User.WM.WM_MOUSEWHEEL;
                case (int)User.WM.WM_XBUTTONDOWN:                    return User.WM.WM_XBUTTONDOWN;
                case (int)User.WM.WM_XBUTTONUP:                      return User.WM.WM_XBUTTONUP;
                case (int)User.WM.WM_XBUTTONDBLCLK:                  return User.WM.WM_XBUTTONDBLCLK;
                case (int)User.WM.WM_MOUSEHWHEEL:                    return User.WM.WM_MOUSEHWHEEL;
                //case (int)User.WM.WM_MOUSELAST:                      return User.WM.WM_MOUSELAST; // idem WM_MOUSEHWHEEL
                case (int)User.WM.WM_PARENTNOTIFY:                   return User.WM.WM_PARENTNOTIFY;
                case (int)User.WM.WM_ENTERMENULOOP:                  return User.WM.WM_ENTERMENULOOP;
                case (int)User.WM.WM_EXITMENULOOP:                   return User.WM.WM_EXITMENULOOP;
                case (int)User.WM.WM_NEXTMENU:                       return User.WM.WM_NEXTMENU;
                case (int)User.WM.WM_SIZING:                         return User.WM.WM_SIZING;
                case (int)User.WM.WM_CAPTURECHANGED:                 return User.WM.WM_CAPTURECHANGED;
                case (int)User.WM.WM_MOVING:                         return User.WM.WM_MOVING;
                case (int)User.WM.WM_POWERBROADCAST:                 return User.WM.WM_POWERBROADCAST;
                case (int)User.WM.WM_DEVICECHANGE:                   return User.WM.WM_DEVICECHANGE;
                case (int)User.WM.WM_MDICREATE:                      return User.WM.WM_MDICREATE;
                case (int)User.WM.WM_MDIDESTROY:                     return User.WM.WM_MDIDESTROY;
                case (int)User.WM.WM_MDIACTIVATE:                    return User.WM.WM_MDIACTIVATE;
                case (int)User.WM.WM_MDIRESTORE:                     return User.WM.WM_MDIRESTORE;
                case (int)User.WM.WM_MDINEXT:                        return User.WM.WM_MDINEXT;
                case (int)User.WM.WM_MDIMAXIMIZE:                    return User.WM.WM_MDIMAXIMIZE;
                case (int)User.WM.WM_MDITILE:                        return User.WM.WM_MDITILE;
                case (int)User.WM.WM_MDICASCADE:                     return User.WM.WM_MDICASCADE;
                case (int)User.WM.WM_MDIICONARRANGE:                 return User.WM.WM_MDIICONARRANGE;
                case (int)User.WM.WM_MDIGETACTIVE:                   return User.WM.WM_MDIGETACTIVE;
                case (int)User.WM.WM_MDISETMENU:                     return User.WM.WM_MDISETMENU;
                case (int)User.WM.WM_ENTERSIZEMOVE:                  return User.WM.WM_ENTERSIZEMOVE;
                case (int)User.WM.WM_EXITSIZEMOVE:                   return User.WM.WM_EXITSIZEMOVE;
                case (int)User.WM.WM_DROPFILES:                      return User.WM.WM_DROPFILES;
                case (int)User.WM.WM_MDIREFRESHMENU:                 return User.WM.WM_MDIREFRESHMENU;
                case (int)User.WM.WM_IME_SETCONTEXT:                 return User.WM.WM_IME_SETCONTEXT;
                case (int)User.WM.WM_IME_NOTIFY:                     return User.WM.WM_IME_NOTIFY;
                case (int)User.WM.WM_IME_CONTROL:                    return User.WM.WM_IME_CONTROL;
                case (int)User.WM.WM_IME_COMPOSITIONFULL:            return User.WM.WM_IME_COMPOSITIONFULL;
                case (int)User.WM.WM_IME_SELECT:                     return User.WM.WM_IME_SELECT;
                case (int)User.WM.WM_IME_CHAR:                       return User.WM.WM_IME_CHAR;
                case (int)User.WM.WM_IME_REQUEST:                    return User.WM.WM_IME_REQUEST;
                case (int)User.WM.WM_IME_KEYDOWN:                    return User.WM.WM_IME_KEYDOWN;
                case (int)User.WM.WM_IME_KEYUP:                      return User.WM.WM_IME_KEYUP;
                case (int)User.WM.WM_MOUSEHOVER:                     return User.WM.WM_MOUSEHOVER;
                case (int)User.WM.WM_MOUSELEAVE:                     return User.WM.WM_MOUSELEAVE;
                case (int)User.WM.WM_NCMOUSEHOVER:                   return User.WM.WM_NCMOUSEHOVER;
                case (int)User.WM.WM_NCMOUSELEAVE:                   return User.WM.WM_NCMOUSELEAVE;
                case (int)User.WM.WM_WTSSESSION_CHANGE:              return User.WM.WM_WTSSESSION_CHANGE;
                case (int)User.WM.WM_TABLET_FIRST:                   return User.WM.WM_TABLET_FIRST;
                case (int)User.WM.WM_TABLET_LAST:                    return User.WM.WM_TABLET_LAST;
                case (int)User.WM.WM_CUT:                            return User.WM.WM_CUT;
                case (int)User.WM.WM_COPY:                           return User.WM.WM_COPY;
                case (int)User.WM.WM_PASTE:                          return User.WM.WM_PASTE;
                case (int)User.WM.WM_CLEAR:                          return User.WM.WM_CLEAR;
                case (int)User.WM.WM_UNDO:                           return User.WM.WM_UNDO;
                case (int)User.WM.WM_RENDERFORMAT:                   return User.WM.WM_RENDERFORMAT;
                case (int)User.WM.WM_RENDERALLFORMATS:               return User.WM.WM_RENDERALLFORMATS;
                case (int)User.WM.WM_DESTROYCLIPBOARD:               return User.WM.WM_DESTROYCLIPBOARD;
                case (int)User.WM.WM_DRAWCLIPBOARD:                  return User.WM.WM_DRAWCLIPBOARD;
                case (int)User.WM.WM_PAINTCLIPBOARD:                 return User.WM.WM_PAINTCLIPBOARD;
                case (int)User.WM.WM_VSCROLLCLIPBOARD:               return User.WM.WM_VSCROLLCLIPBOARD;
                case (int)User.WM.WM_SIZECLIPBOARD:                  return User.WM.WM_SIZECLIPBOARD;
                case (int)User.WM.WM_ASKCBFORMATNAME:                return User.WM.WM_ASKCBFORMATNAME;
                case (int)User.WM.WM_CHANGECBCHAIN:                  return User.WM.WM_CHANGECBCHAIN;
                case (int)User.WM.WM_HSCROLLCLIPBOARD:               return User.WM.WM_HSCROLLCLIPBOARD;
                case (int)User.WM.WM_QUERYNEWPALETTE:                return User.WM.WM_QUERYNEWPALETTE;
                case (int)User.WM.WM_PALETTEISCHANGING:              return User.WM.WM_PALETTEISCHANGING;
                case (int)User.WM.WM_PALETTECHANGED:                 return User.WM.WM_PALETTECHANGED;
                case (int)User.WM.WM_HOTKEY:                         return User.WM.WM_HOTKEY;
                case (int)User.WM.WM_PRINT:                          return User.WM.WM_PRINT;
                case (int)User.WM.WM_PRINTCLIENT:                    return User.WM.WM_PRINTCLIENT;
                case (int)User.WM.WM_APPCOMMAND:                     return User.WM.WM_APPCOMMAND;
                case (int)User.WM.WM_THEMECHANGED:                   return User.WM.WM_THEMECHANGED;
                case (int)User.WM.WM_CLIPBOARDUPDATE:                return User.WM.WM_CLIPBOARDUPDATE;
                case (int)User.WM.WM_DWMCOMPOSITIONCHANGED:          return User.WM.WM_DWMCOMPOSITIONCHANGED;
                case (int)User.WM.WM_DWMNCRENDERINGCHANGED:          return User.WM.WM_DWMNCRENDERINGCHANGED;
                case (int)User.WM.WM_DWMCOLORIZATIONCOLORCHANGED:    return User.WM.WM_DWMCOLORIZATIONCOLORCHANGED;
                case (int)User.WM.WM_DWMWINDOWMAXIMIZEDCHANGE:       return User.WM.WM_DWMWINDOWMAXIMIZEDCHANGE;
                case (int)User.WM.WM_GETTITLEBARINFOEX:              return User.WM.WM_GETTITLEBARINFOEX;
                case (int)User.WM.WM_HANDHELDFIRST:                  return User.WM.WM_HANDHELDFIRST;
                case (int)User.WM.WM_HANDHELDLAST:                   return User.WM.WM_HANDHELDLAST;
                case (int)User.WM.WM_AFXFIRST:                       return User.WM.WM_AFXFIRST;
                case (int)User.WM.WM_AFXLAST:                        return User.WM.WM_AFXLAST;
                case (int)User.WM.WM_PENWINFIRST:                    return User.WM.WM_PENWINFIRST;
                case (int)User.WM.WM_PENWINLAST:                     return User.WM.WM_PENWINLAST;
                case (int)User.WM.WM_APP:                            return User.WM.WM_APP;
                case (int)User.WM.WM_USER:                           return User.WM.WM_USER;
                case (int)User.WM.CPL_LAUNCH:                        return User.WM.CPL_LAUNCH;
                case (int)User.WM.CPL_LAUNCHED:                      return User.WM.CPL_LAUNCHED;
                case (int)User.WM.SYSTIMER:                          return User.WM.SYSTIMER;
            }
            return User.WM.WM_UNKNOW;
        }
        #pragma warning restore
        #endregion

        #region GetMessageCategory
        public static MessageCategory GetMessageCategory(int msg)
        {
            MessageCategory messageCategory = MessageCategory.Unknown;
            switch (msg)
            {
                // Keyboard input messages (http://msdn.microsoft.com/en-us/library/ms674715(VS.85).aspx)
                case (int)User.WM.WM_GETHOTKEY:
                case (int)User.WM.WM_SETHOTKEY:
                // Keyboard input notifications (http://msdn.microsoft.com/en-us/library/ms674716(VS.85).aspx)
                //case User.WM.WM_ACTIVATE:
                //case User.WM.WM_APPCOMMAND:
                case (int)User.WM.WM_CHAR:
                case (int)User.WM.WM_DEADCHAR:
                case (int)User.WM.WM_HOTKEY:
                case (int)User.WM.WM_KEYDOWN:
                case (int)User.WM.WM_KEYUP:
                case (int)User.WM.WM_KILLFOCUS:
                case (int)User.WM.WM_SETFOCUS:
                case (int)User.WM.WM_SYSDEADCHAR:
                case (int)User.WM.WM_SYSKEYDOWN:
                case (int)User.WM.WM_SYSKEYUP:
                case (int)User.WM.WM_UNICHAR:
                // Keyboard accelerator messages (http://msdn.microsoft.com/en-us/library/ms674707(VS.85).aspx)
                case (int)User.WM.WM_CHANGEUISTATE:
                case (int)User.WM.WM_INITMENU:
                case (int)User.WM.WM_QUERYUISTATE:
                case (int)User.WM.WM_UPDATEUISTATE:
                // Keyboard accelerator notifications (http://msdn.microsoft.com/en-us/library/ms674709(VS.85).aspx)
                case (int)User.WM.WM_INITMENUPOPUP:
                case (int)User.WM.WM_MENUCHAR:
                case (int)User.WM.WM_MENUSELECT:
                case (int)User.WM.WM_SYSCHAR:
                case (int)User.WM.WM_SYSCOMMAND:
                    messageCategory = MessageCategory.Keyboard;
                    break;

                // Mouse input notifications (http://msdn.microsoft.com/en-us/library/ms674824(VS.85).aspx)
                case (int)User.WM.WM_CAPTURECHANGED:
                case (int)User.WM.WM_LBUTTONDBLCLK:
                case (int)User.WM.WM_LBUTTONDOWN:
                case (int)User.WM.WM_LBUTTONUP:
                case (int)User.WM.WM_MBUTTONDBLCLK:
                case (int)User.WM.WM_MBUTTONDOWN:
                case (int)User.WM.WM_MBUTTONUP:
                case (int)User.WM.WM_MOUSEACTIVATE:
                case (int)User.WM.WM_MOUSEHOVER:
                case (int)User.WM.WM_MOUSEHWHEEL:
                case (int)User.WM.WM_MOUSELEAVE:
                case (int)User.WM.WM_MOUSEMOVE:
                case (int)User.WM.WM_MOUSEWHEEL:
                case (int)User.WM.WM_NCHITTEST:
                case (int)User.WM.WM_NCLBUTTONDBLCLK:
                case (int)User.WM.WM_NCLBUTTONDOWN:
                case (int)User.WM.WM_NCLBUTTONUP:
                case (int)User.WM.WM_NCMBUTTONDBLCLK:
                case (int)User.WM.WM_NCMBUTTONDOWN:
                case (int)User.WM.WM_NCMBUTTONUP:
                case (int)User.WM.WM_NCMOUSEHOVER:
                case (int)User.WM.WM_NCMOUSELEAVE:
                case (int)User.WM.WM_NCMOUSEMOVE:
                case (int)User.WM.WM_NCRBUTTONDBLCLK:
                case (int)User.WM.WM_NCRBUTTONDOWN:
                case (int)User.WM.WM_NCRBUTTONUP:
                case (int)User.WM.WM_NCXBUTTONDBLCLK:
                case (int)User.WM.WM_NCXBUTTONDOWN:
                case (int)User.WM.WM_NCXBUTTONUP:
                case (int)User.WM.WM_RBUTTONDBLCLK:
                case (int)User.WM.WM_RBUTTONDOWN:
                case (int)User.WM.WM_RBUTTONUP:
                case (int)User.WM.WM_XBUTTONDBLCLK:
                case (int)User.WM.WM_XBUTTONDOWN:
                case (int)User.WM.WM_XBUTTONUP:
                // Cursor notifications (http://msdn.microsoft.com/en-us/library/ms674646(VS.85).aspx)
                case (int)User.WM.WM_SETCURSOR:
                    messageCategory = MessageCategory.Mouse;
                    break;

                // Common dialog box messages (http://msdn.microsoft.com/en-us/library/ms674695(VS.85).aspx)
                case User.Const.CDM_GETFILEPATH:
                case User.Const.CDM_GETFOLDERIDLIST:
                case User.Const.CDM_GETFOLDERPATH:
                case User.Const.CDM_GETSPEC:
                case User.Const.CDM_HIDECONTROL:
                case User.Const.CDM_SETCONTROLTEXT:
                case User.Const.CDM_SETDEFEXT:
                //case User.WM.SETRGBSTRING:
                case User.Const.WM_CHOOSEFONT_GETLOGFONT:
                // case 1126 existe déjà case User.WM.WM_CHOOSEFONT_SETFLAGS:       idem CDM_GETFOLDERPATH
                // case 1125 existe déjà case User.WM.WM_CHOOSEFONT_SETLOGFONT:     idem CDM_GETFILEPATH
                // Common dialog box notifications (http://msdn.microsoft.com/en-us/library/ms674697(VS.85).aspx)
                //case User.WM.CDN_FILEOK:
                //case User.WM.CDN_FOLDERCHANGE:
                //case User.WM.CDN_HELP:
                //case User.WM.CDN_INCLUDEITEM:
                //case User.WM.CDN_INITDONE:
                //case User.WM.CDN_SELCHANGE:
                //case User.WM.CDN_SHAREVIOLATION:
                //case User.WM.CDN_TYPECHANGE:
                //case User.WM.COLOROKSTRING:
                //case User.WM.FILEOKSTRING:
                //case User.WM.FINDMSGSTRING:
                //case User.WM.HELPMSGSTRING:
                //case User.WM.LBSELCHSTRING:
                //case User.WM.SHAREVISTRING:
                //case User.WM.WM_PSD_ENVSTAMPRECT:
                //case User.WM.WM_PSD_FULLPAGERECT:
                //case User.WM.WM_PSD_GREEKTEXTRECT:
                //case User.WM.WM_PSD_MARGINRECT:
                //case User.WM.WM_PSD_MINMARGINRECT:
                //case User.WM.WM_PSD_PAGESETUPDLG:
                //case User.WM.WM_PSD_YAFULLPAGERECT:
                // Dialog box messages (http://msdn.microsoft.com/en-us/library/ms674842(VS.85).aspx)
                //case User.WM.DM_GETDEFID:
                //case User.WM.DM_REPOSITION:
                // case 1025 existe déjà case User.WM.DM_SETDEFID:
                // Dialog box notifications (http://msdn.microsoft.com/en-us/library/ms674843(VS.85).aspx)
                case (int)User.WM.WM_CTLCOLORDLG:
                case (int)User.WM.WM_ENTERIDLE:
                case (int)User.WM.WM_GETDLGCODE:
                case (int)User.WM.WM_INITDIALOG:
                case (int)User.WM.WM_NEXTDLGCTL:
                    messageCategory = MessageCategory.Dialog;
                    break;

                // MDI messages (http://msdn.microsoft.com/en-us/library/ms674861(VS.85).aspx)
                case (int)User.WM.WM_MDIACTIVATE:
                case (int)User.WM.WM_MDICASCADE:
                case (int)User.WM.WM_MDICREATE:
                case (int)User.WM.WM_MDIDESTROY:
                case (int)User.WM.WM_MDIGETACTIVE:
                case (int)User.WM.WM_MDIICONARRANGE:
                case (int)User.WM.WM_MDIMAXIMIZE:
                case (int)User.WM.WM_MDINEXT:
                case (int)User.WM.WM_MDIREFRESHMENU:
                case (int)User.WM.WM_MDIRESTORE:
                case (int)User.WM.WM_MDISETMENU:
                case (int)User.WM.WM_MDITILE:
                    messageCategory = MessageCategory.MDI;
                    break;

                // Keyboard input notifications (http://msdn.microsoft.com/en-us/library/ms674716(VS.85).aspx)
                case (int)User.WM.WM_ACTIVATE:
                case (int)User.WM.WM_APPCOMMAND:
                // Raw input notifications (http://msdn.microsoft.com/en-us/library/ms674831(VS.85).aspx)
                case (int)User.WM.WM_INPUT:
                case (int)User.WM.WM_INPUT_DEVICE_CHANGE:
                // Queues notifications (http://msdn.microsoft.com/en-us/library/ms674855(VS.85).aspx)
                //case User.WM.OCM__BASE:
                case (int)User.WM.WM_APP:
                case (int)User.WM.WM_QUIT:
                // Timer notifications (http://msdn.microsoft.com/en-us/library/ms674867(VS.85).aspx)
                case (int)User.WM.WM_TIMER:
                // Hooks notifications (http://msdn.microsoft.com/en-us/library/ms674849(VS.85).aspx)
                case (int)User.WM.WM_CANCELJOURNAL:
                case (int)User.WM.WM_QUEUESYNC:
                // Windows messages (http://msdn.microsoft.com/en-us/library/ms674886(VS.85).aspx)
                case User.Const.MN_GETHMENU:
                case (int)User.WM.WM_GETFONT:
                case (int)User.WM.WM_GETTEXT:
                case (int)User.WM.WM_GETTEXTLENGTH:
                case (int)User.WM.WM_SETFONT:
                case (int)User.WM.WM_SETICON:
                case (int)User.WM.WM_SETTEXT:
                // Windows notifications (http://msdn.microsoft.com/en-us/library/ms674887(VS.85).aspx)
                case (int)User.WM.WM_ACTIVATEAPP:
                case (int)User.WM.WM_CANCELMODE:
                case (int)User.WM.WM_CHILDACTIVATE:
                case (int)User.WM.WM_CLOSE:
                case (int)User.WM.WM_COMPACTING:
                case (int)User.WM.WM_CREATE:
                case (int)User.WM.WM_DESTROY:
                case (int)User.WM.WM_ENABLE:
                case (int)User.WM.WM_ENTERSIZEMOVE:
                case (int)User.WM.WM_EXITSIZEMOVE:
                case (int)User.WM.WM_GETICON:
                case (int)User.WM.WM_GETMINMAXINFO:
                case (int)User.WM.WM_INPUTLANGCHANGE:
                case (int)User.WM.WM_INPUTLANGCHANGEREQUEST:
                case (int)User.WM.WM_MOVE:
                case (int)User.WM.WM_MOVING:
                case (int)User.WM.WM_NCACTIVATE:
                case (int)User.WM.WM_NCCALCSIZE:
                case (int)User.WM.WM_NCCREATE:
                case (int)User.WM.WM_NCDESTROY:
                case (int)User.WM.WM_NULL:
                case (int)User.WM.WM_PARENTNOTIFY:
                case (int)User.WM.WM_QUERYDRAGICON:
                case (int)User.WM.WM_QUERYOPEN:
                case (int)User.WM.WM_SHOWWINDOW:
                case (int)User.WM.WM_SIZE:
                case (int)User.WM.WM_SIZING:
                case (int)User.WM.WM_STYLECHANGED:
                case (int)User.WM.WM_STYLECHANGING:
                case (int)User.WM.WM_THEMECHANGED:
                case (int)User.WM.WM_USERCHANGED:
                case (int)User.WM.WM_WINDOWPOSCHANGED:
                case (int)User.WM.WM_WINDOWPOSCHANGING:
                // Menu notifications (http://msdn.microsoft.com/en-us/library/ms674668(VS.85).aspx)
                case (int)User.WM.WM_COMMAND:
                case (int)User.WM.WM_CONTEXTMENU:
                case (int)User.WM.WM_ENTERMENULOOP:
                case (int)User.WM.WM_EXITMENULOOP:
                case (int)User.WM.WM_GETTITLEBARINFOEX:
                case (int)User.WM.WM_MENUCOMMAND:
                case (int)User.WM.WM_MENUDRAG:
                case (int)User.WM.WM_MENUGETOBJECT:
                case (int)User.WM.WM_MENURBUTTONUP:
                case (int)User.WM.WM_NEXTMENU:
                case (int)User.WM.WM_UNINITMENUPOPUP:
                // Icon notifications (http://msdn.microsoft.com/en-us/library/ms674655(VS.85).aspx)
                case (int)User.WM.WM_ERASEBKGND:
                case (int)User.WM.WM_ICONERASEBKGND:
                case (int)User.WM.WM_PAINTICON:
                // Shell messages and notifications
                case (int)User.WM.WM_DROPFILES:
                case (int)User.WM.WM_HELP:
                case (int)User.WM.WM_TCARD:
                    messageCategory = MessageCategory.General;
                    break;

                // Clipboard messages (http://msdn.microsoft.com/en-us/library/ms674558(VS.85).aspx)
                case (int)User.WM.WM_CLEAR:
                case (int)User.WM.WM_COPY:
                case (int)User.WM.WM_CUT:
                case (int)User.WM.WM_PASTE:
                // Clipboard notifications (http://msdn.microsoft.com/en-us/library/ms674561(VS.85).aspx)
                case (int)User.WM.WM_ASKCBFORMATNAME:
                case (int)User.WM.WM_CHANGECBCHAIN:
                case (int)User.WM.WM_CLIPBOARDUPDATE:
                case (int)User.WM.WM_DESTROYCLIPBOARD:
                case (int)User.WM.WM_DRAWCLIPBOARD:
                case (int)User.WM.WM_HSCROLLCLIPBOARD:
                case (int)User.WM.WM_PAINTCLIPBOARD:
                case (int)User.WM.WM_RENDERALLFORMATS:
                case (int)User.WM.WM_RENDERFORMAT:
                case (int)User.WM.WM_SIZECLIPBOARD:
                case (int)User.WM.WM_VSCROLLCLIPBOARD:
                // Data copy messages (http://msdn.microsoft.com/en-us/library/ms674575(VS.85).aspx)
                case (int)User.WM.WM_COPYDATA:
                    messageCategory = MessageCategory.Clipboard;
                    break;

                // DDE messages (http://msdn.microsoft.com/en-us/library/ms674594(VS.85).aspx)
                case User.Const.WM_DDE_INITIATE:
                // DDE notifications (http://msdn.microsoft.com/en-us/library/ms674595(VS.85).aspx)
                case User.Const.WM_DDE_ACK:
                case User.Const.WM_DDE_ADVISE:
                case User.Const.WM_DDE_DATA:
                case User.Const.WM_DDE_EXECUTE:
                case User.Const.WM_DDE_POKE:
                case User.Const.WM_DDE_REQUEST:
                case User.Const.WM_DDE_TERMINATE:
                case User.Const.WM_DDE_UNADVISE:
                    messageCategory = MessageCategory.DDE;
                    break;

                // Shell messages and notifications
                //case User.WM.ABM_ACTIVATE:
                //case User.WM.ABM_GETAUTOHIDEBAR:
                //case User.WM.ABM_GETSTATE:
                //case User.WM.ABM_GETTASKBARPOS:
                //case User.WM.ABM_NEW:
                //case User.WM.ABM_QUERYPOS:
                //case User.WM.ABM_REMOVE:
                //case User.WM.ABM_SETAUTOHIDEBAR:
                //case User.WM.ABM_SETPOS:
                //case User.WM.ABM_SETSTATE:
                //case User.WM.ABM_WINDOWPOSCHANGED:
                //case User.WM.ABN_FULLSCREENAPP:
                //case User.WM.ABN_POSCHANGED:
                //case User.WM.ABN_STATECHANGE:
                //case User.WM.ABN_WINDOWARRANGE:
                //case User.WM.CPL_DBLCLK:
                //case User.WM.CPL_EXIT:
                //case User.WM.CPL_GETCOUNT:
                //case User.WM.CPL_INIT:
                //case User.WM.CPL_INQUIRE:
                //case User.WM.CPL_NEWINQUIRE:
                //case User.WM.CPL_SELECT:
                //case User.WM.CPL_STARTWPARMS:
                //case User.WM.CPL_STOP:
                //case User.WM.DFM_GETDEFSTATICID:
                //case User.WM.DFM_GETHELPTEXT:
                //case User.WM.DFM_GETHELPTEXTW:
                //case User.WM.DFM_GETVERB:
                //case User.WM.DFM_INVOKECOMMAND:
                //case User.WM.DFM_INVOKECOMMANDEX:
                //case User.WM.DFM_MAPCOMMANDNAME:
                //case User.WM.DFM_MERGECONTEXTMENU:
                //case User.WM.DFM_MERGECONTEXTMENU_BOTTOM:
                //case User.WM.DFM_MERGECONTEXTMENU_TOP:
                //case User.WM.DFM_MODIFYQCMFLAGS:
                //case User.WM.DFM_VALIDATECMD:
                //case User.WM.DFM_WM_DRAWITEM:
                //case User.WM.DFM_WM_INITMENUPOPUP:
                //case User.WM.DFM_WM_MEASUREITEM:
                //case User.WM.FM_GETDRIVEINFO:
                //case User.WM.FM_GETFILESEL:
                //case User.WM.FM_GETFILESELLFN:
                //case User.WM.FM_GETFOCUS:
                //case User.WM.FM_GETSELCOUNT:
                //case User.WM.FM_GETSELCOUNTLFN:
                //case User.WM.FM_REFRESH_WINDOWS:
                //case User.WM.FM_RELOAD_EXTENSIONS:
                //case User.WM.FMEVENT_HELPMENUITEM:
                //case User.WM.FMEVENT_HELPSTRING:
                //case User.WM.FMEVENT_INITMENU:
                //case User.WM.FMEVENT_LOAD:
                //case User.WM.FMEVENT_SELCHANGE:
                //case User.WM.FMEVENT_TOOLBARLOAD:
                //case User.WM.FMEVENT_UNLOAD:
                //case User.WM.FMEVENT_USER_REFRESH:
                //case User.WM.SFVM_ADDOBJECT:
                //case User.WM.SFVM_ADDPROPERTYPAGES:
                //case User.WM.SFVM_BACKGROUNDENUM:
                //case User.WM.SFVM_BACKGROUNDENUMDONE:
                //case User.WM.SFVM_COLUMNCLICK:
                //case User.WM.SFVM_DEFITEMCOUNT:
                //case User.WM.SFVM_DEFVIEWMODE:
                //case User.WM.SFVM_DIDDRAGDROP:
                //case User.WM.SFVM_FSNOTIFY:
                //case User.WM.SFVM_GETANIMATION:
                //case User.WM.SFVM_GETBUTTONINFO:
                //case User.WM.SFVM_GETBUTTONS:
                //case User.WM.SFVM_GETDETAILSOF:
                //case User.WM.SFVM_GETHELPTEXT:
                //case User.WM.SFVM_GETHELPTOPIC:
                //case User.WM.SFVM_GETNOTIFY:
                //case User.WM.SFVM_GETPANE:
                //case User.WM.SFVM_GETSELECTEDOBJECTS:
                //case User.WM.SFVM_GETSORTDEFAULTS:
                //case User.WM.SFVM_GETTOOLTIPTEXT:
                //case User.WM.SFVM_GETZONE:
                //case User.WM.SFVM_INITMENUPOPUP:
                //case User.WM.SFVM_INVOKECOMMAND:
                //case User.WM.SFVM_MERGEMENU:
                //case User.WM.SFVM_QUERYFSNOTIFY:
                //case User.WM.SFVM_REARRANGE:
                //case User.WM.SFVM_REMOVEOBJECT:
                //case User.WM.SFVM_SETCLIPBOARD:
                //case User.WM.SFVM_SETISFV:
                //case User.WM.SFVM_SETITEMPOS:
                //case User.WM.SFVM_SETPOINTS:
                //case User.WM.SFVM_SIZE:
                //case User.WM.SFVM_THISIDLIST:
                //case User.WM.SFVM_UNMERGEMENU:
                //case User.WM.SFVM_UPDATEOBJECT:
                //case User.WM.SFVM_UPDATESTATUSBAR:
                //case User.WM.SFVM_WINDOWCREATED:
                //case User.WM.SMC_CHEVRONEXPAND:
                //case User.WM.SMC_CHEVRONGETTIP:
                //case User.WM.SMC_CREATE:
                //case User.WM.SMC_DEFAULTICON:
                //case User.WM.SMC_DEMOTE:
                //case User.WM.SMC_DISPLAYCHEVRONTIP:
                //case User.WM.SMC_EXITMENU:
                //case User.WM.SMC_GETINFO:
                //case User.WM.SMC_GETOBJECT:
                //case User.WM.SMC_GETSFINFO:
                //case User.WM.SMC_GETSFOBJECT:
                //case User.WM.SMC_INITMENU:
                //case User.WM.SMC_NEWITEM:
                //case User.WM.SMC_PROMOTE:
                //case User.WM.SMC_REFRESH:
                //case User.WM.SMC_SETSFOBJECT:
                //case User.WM.SMC_SFDDRESTRICTED:
                //case User.WM.SMC_SFEXEC:
                //case User.WM.SMC_SFSELECTITEM:
                //case User.WM.SMC_SHCHANGENOTIFY:
                //case User.WM.WM_CPL_LAUNCH:
                //case User.WM.WM_CPL_LAUNCHED:
            }
            return messageCategory;
        }
        #endregion

        #region MessageToString0
        public static string MessageToString0(int msg)
        {
            string sMsg = null;
            switch (msg)
            {
                // Keyboard input messages (http://msdn.microsoft.com/en-us/library/ms674715(VS.85).aspx)
                case (int)User.WM.WM_GETHOTKEY:                 sMsg = "WM_GETHOTKEY";                 break;
                case (int)User.WM.WM_SETHOTKEY:                 sMsg = "WM_SETHOTKEY";                 break;

                // Keyboard input notifications (http://msdn.microsoft.com/en-us/library/ms674716(VS.85).aspx)
                case (int)User.WM.WM_ACTIVATE:                  sMsg = "WM_ACTIVATE";                  break;
                case (int)User.WM.WM_APPCOMMAND:                sMsg = "WM_APPCOMMAND";                break;
                case (int)User.WM.WM_CHAR:                      sMsg = "WM_CHAR";                      break;
                case (int)User.WM.WM_DEADCHAR:                  sMsg = "WM_DEADCHAR";                  break;
                case (int)User.WM.WM_HOTKEY:                    sMsg = "WM_HOTKEY";                    break;
                case (int)User.WM.WM_KEYDOWN:                   sMsg = "WM_KEYDOWN";                   break; // idem User.Const.WM_KEYFIRST
                case (int)User.WM.WM_KEYUP:                     sMsg = "WM_KEYUP";                     break;
                case (int)User.WM.WM_KILLFOCUS:                 sMsg = "WM_KILLFOCUS";                 break;
                case (int)User.WM.WM_SETFOCUS:                  sMsg = "WM_SETFOCUS";                  break;
                case (int)User.WM.WM_SYSDEADCHAR:               sMsg = "WM_SYSDEADCHAR";               break;
                case (int)User.WM.WM_SYSKEYDOWN:                sMsg = "WM_SYSKEYDOWN";                break;
                case (int)User.WM.WM_SYSKEYUP:                  sMsg = "WM_SYSKEYUP";                  break;
                case (int)User.WM.WM_UNICHAR:                   sMsg = "WM_UNICHAR";                   break;

                // Keyboard accelerator messages (http://msdn.microsoft.com/en-us/library/ms674707(VS.85).aspx)
                case (int)User.WM.WM_CHANGEUISTATE:             sMsg = "WM_CHANGEUISTATE";             break;
                case (int)User.WM.WM_INITMENU:                  sMsg = "WM_INITMENU";                  break;
                case (int)User.WM.WM_QUERYUISTATE:              sMsg = "WM_QUERYUISTATE";              break;
                case (int)User.WM.WM_UPDATEUISTATE:             sMsg = "WM_UPDATEUISTATE";             break;

                // Keyboard accelerator notifications (http://msdn.microsoft.com/en-us/library/ms674709(VS.85).aspx)
                case (int)User.WM.WM_INITMENUPOPUP:             sMsg = "WM_INITMENUPOPUP";             break;
                case (int)User.WM.WM_MENUCHAR:                  sMsg = "WM_MENUCHAR";                  break;
                case (int)User.WM.WM_MENUSELECT:                sMsg = "WM_MENUSELECT";                break;
                case (int)User.WM.WM_SYSCHAR:                   sMsg = "WM_SYSCHAR";                   break;
                case (int)User.WM.WM_SYSCOMMAND:                sMsg = "WM_SYSCOMMAND";                break;

                // Mouse input notifications (http://msdn.microsoft.com/en-us/library/ms674824(VS.85).aspx)
                case (int)User.WM.WM_CAPTURECHANGED:            sMsg = "WM_CAPTURECHANGED";            break;
                case (int)User.WM.WM_LBUTTONDBLCLK:             sMsg = "WM_LBUTTONDBLCLK";             break;
                case (int)User.WM.WM_LBUTTONDOWN:               sMsg = "WM_LBUTTONDOWN";               break;
                case (int)User.WM.WM_LBUTTONUP:                 sMsg = "WM_LBUTTONUP";                 break;
                case (int)User.WM.WM_MBUTTONDBLCLK:             sMsg = "WM_MBUTTONDBLCLK";             break; // idem User.Const.WM_MOUSELAST
                case (int)User.WM.WM_MBUTTONDOWN:               sMsg = "WM_MBUTTONDOWN";               break;
                case (int)User.WM.WM_MBUTTONUP:                 sMsg = "WM_MBUTTONUP";                 break;
                case (int)User.WM.WM_MOUSEACTIVATE:             sMsg = "WM_MOUSEACTIVATE";             break;
                case (int)User.WM.WM_MOUSEHOVER:                sMsg = "WM_MOUSEHOVER";                break;
                case (int)User.WM.WM_MOUSEHWHEEL:               sMsg = "WM_MOUSEHWHEEL";               break;
                case (int)User.WM.WM_MOUSELEAVE:                sMsg = "WM_MOUSELEAVE";                break;
                case (int)User.WM.WM_MOUSEMOVE:                 sMsg = "WM_MOUSEMOVE";                 break; // idem User.Const.WM_MOUSEFIRST
                case (int)User.WM.WM_MOUSEWHEEL:                sMsg = "WM_MOUSEWHEEL";                break;
                case (int)User.WM.WM_NCHITTEST:                 sMsg = "WM_NCHITTEST";                 break;
                case (int)User.WM.WM_NCLBUTTONDBLCLK:           sMsg = "WM_NCLBUTTONDBLCLK";           break;
                case (int)User.WM.WM_NCLBUTTONDOWN:             sMsg = "WM_NCLBUTTONDOWN";             break;
                case (int)User.WM.WM_NCLBUTTONUP:               sMsg = "WM_NCLBUTTONUP";               break;
                case (int)User.WM.WM_NCMBUTTONDBLCLK:           sMsg = "WM_NCMBUTTONDBLCLK";           break;
                case (int)User.WM.WM_NCMBUTTONDOWN:             sMsg = "WM_NCMBUTTONDOWN";             break;
                case (int)User.WM.WM_NCMBUTTONUP:               sMsg = "WM_NCMBUTTONUP";               break;
                case (int)User.WM.WM_NCMOUSEHOVER:              sMsg = "WM_NCMOUSEHOVER";              break;
                case (int)User.WM.WM_NCMOUSELEAVE:              sMsg = "WM_NCMOUSELEAVE";              break;
                case (int)User.WM.WM_NCMOUSEMOVE:               sMsg = "WM_NCMOUSEMOVE";               break;
                case (int)User.WM.WM_NCRBUTTONDBLCLK:           sMsg = "WM_NCRBUTTONDBLCLK";           break;
                case (int)User.WM.WM_NCRBUTTONDOWN:             sMsg = "WM_NCRBUTTONDOWN";             break;
                case (int)User.WM.WM_NCRBUTTONUP:               sMsg = "WM_NCRBUTTONUP";               break;
                case (int)User.WM.WM_NCXBUTTONDBLCLK:           sMsg = "WM_NCXBUTTONDBLCLK";           break;
                case (int)User.WM.WM_NCXBUTTONDOWN:             sMsg = "WM_NCXBUTTONDOWN";             break;
                case (int)User.WM.WM_NCXBUTTONUP:               sMsg = "WM_NCXBUTTONUP";               break;
                case (int)User.WM.WM_RBUTTONDBLCLK:             sMsg = "WM_RBUTTONDBLCLK";             break;
                case (int)User.WM.WM_RBUTTONDOWN:               sMsg = "WM_RBUTTONDOWN";               break;
                case (int)User.WM.WM_RBUTTONUP:                 sMsg = "WM_RBUTTONUP";                 break;
                case (int)User.WM.WM_XBUTTONDBLCLK:             sMsg = "WM_XBUTTONDBLCLK";             break;
                case (int)User.WM.WM_XBUTTONDOWN:               sMsg = "WM_XBUTTONDOWN";               break;
                case (int)User.WM.WM_XBUTTONUP:                 sMsg = "WM_XBUTTONUP";                 break;

                // Common dialog box messages (http://msdn.microsoft.com/en-us/library/ms674695(VS.85).aspx)
                case User.Const.CDM_GETFILEPATH:                sMsg = "CDM_GETFILEPATH";              break;
                case User.Const.CDM_GETFOLDERIDLIST:            sMsg = "CDM_GETFOLDERIDLIST";          break;
                case User.Const.CDM_GETFOLDERPATH:              sMsg = "CDM_GETFOLDERPATH";            break;
                case User.Const.CDM_GETSPEC:                    sMsg = "CDM_GETSPEC";                  break;
                case User.Const.CDM_HIDECONTROL:                sMsg = "CDM_HIDECONTROL";              break;
                case User.Const.CDM_SETCONTROLTEXT:             sMsg = "CDM_SETCONTROLTEXT";           break;
                case User.Const.CDM_SETDEFEXT:                  sMsg = "CDM_SETDEFEXT";                break;
                //case User.Const.SETRGBSTRING:                 sMsg = "SETRGBSTRING";                 break;
                case User.Const.WM_CHOOSEFONT_GETLOGFONT:       sMsg = "WM_CHOOSEFONT_GETLOGFONT";     break;
                // case 1126 existe déjà case User.Const.WM_CHOOSEFONT_SETFLAGS:       sMsg = "WM_CHOOSEFONT_SETFLAGS";       break; idem CDM_GETFOLDERPATH
                // case 1125 existe déjà case User.Const.WM_CHOOSEFONT_SETLOGFONT:     sMsg = "WM_CHOOSEFONT_SETLOGFONT";     break; idem CDM_GETFILEPATH

                // Common dialog box notifications (http://msdn.microsoft.com/en-us/library/ms674697(VS.85).aspx)
                //case User.Const.CDN_FILEOK:                   sMsg = "CDN_FILEOK";                   break;
                //case User.Const.CDN_FOLDERCHANGE:             sMsg = "CDN_FOLDERCHANGE";             break;
                //case User.Const.CDN_HELP:                     sMsg = "CDN_HELP";                     break;
                //case User.Const.CDN_INCLUDEITEM:              sMsg = "CDN_INCLUDEITEM";              break;
                //case User.Const.CDN_INITDONE:                 sMsg = "CDN_INITDONE";                 break;
                //case User.Const.CDN_SELCHANGE:                sMsg = "CDN_SELCHANGE";                break;
                //case User.Const.CDN_SHAREVIOLATION:           sMsg = "CDN_SHAREVIOLATION";           break;
                //case User.Const.CDN_TYPECHANGE:               sMsg = "CDN_TYPECHANGE";               break;
                //case User.Const.COLOROKSTRING:                sMsg = "COLOROKSTRING";                break;
                //case User.Const.FILEOKSTRING:                 sMsg = "FILEOKSTRING";                 break;
                //case User.Const.FINDMSGSTRING:                sMsg = "FINDMSGSTRING";                break;
                //case User.Const.HELPMSGSTRING:                sMsg = "HELPMSGSTRING";                break;
                //case User.Const.LBSELCHSTRING:                sMsg = "LBSELCHSTRING";                break;
                //case User.Const.SHAREVISTRING:                sMsg = "SHAREVISTRING";                break;
                //case User.Const.WM_PSD_ENVSTAMPRECT:          sMsg = "WM_PSD_ENVSTAMPRECT";          break;
                //case User.Const.WM_PSD_FULLPAGERECT:          sMsg = "WM_PSD_FULLPAGERECT";          break;
                //case User.Const.WM_PSD_GREEKTEXTRECT:         sMsg = "WM_PSD_GREEKTEXTRECT";         break;
                //case User.Const.WM_PSD_MARGINRECT:            sMsg = "WM_PSD_MARGINRECT";            break;
                //case User.Const.WM_PSD_MINMARGINRECT:         sMsg = "WM_PSD_MINMARGINRECT";         break;
                //case User.Const.WM_PSD_PAGESETUPDLG:          sMsg = "WM_PSD_PAGESETUPDLG";          break;
                //case User.Const.WM_PSD_YAFULLPAGERECT:        sMsg = "WM_PSD_YAFULLPAGERECT";        break;

                // Raw input notifications (http://msdn.microsoft.com/en-us/library/ms674831(VS.85).aspx)
                case (int)User.WM.WM_INPUT:                     sMsg = "WM_INPUT";                     break;
                case (int)User.WM.WM_INPUT_DEVICE_CHANGE:       sMsg = "WM_INPUT_DEVICE_CHANGE";       break;

                // Dialog box messages (http://msdn.microsoft.com/en-us/library/ms674842(VS.85).aspx)
                //case User.Const.DM_GETDEFID:                  sMsg = "DM_GETDEFID";                  break;
                //case User.Const.DM_REPOSITION:                sMsg = "DM_REPOSITION";                break;
                // case 1025 existe déjà case User.Const.DM_SETDEFID:                  sMsg = "DM_SETDEFID";                  break;

                // Dialog box notifications (http://msdn.microsoft.com/en-us/library/ms674843(VS.85).aspx)
                case (int)User.WM.WM_CTLCOLORDLG:               sMsg = "WM_CTLCOLORDLG";               break;
                case (int)User.WM.WM_ENTERIDLE:                 sMsg = "WM_ENTERIDLE";                 break;
                case (int)User.WM.WM_GETDLGCODE:                sMsg = "WM_GETDLGCODE";                break;
                case (int)User.WM.WM_INITDIALOG:                sMsg = "WM_INITDIALOG";                break;
                case (int)User.WM.WM_NEXTDLGCTL:                sMsg = "WM_NEXTDLGCTL";                break;

                // Hooks notifications (http://msdn.microsoft.com/en-us/library/ms674849(VS.85).aspx)
                case (int)User.WM.WM_CANCELJOURNAL:             sMsg = "WM_CANCELJOURNAL";             break;
                case (int)User.WM.WM_QUEUESYNC:                 sMsg = "WM_QUEUESYNC";                 break;

                // Queues notifications (http://msdn.microsoft.com/en-us/library/ms674855(VS.85).aspx)
                //case User.Const.OCM__BASE:                    sMsg = "OCM__BASE";                    break;
                case (int)User.WM.WM_APP:                       sMsg = "WM_APP";                       break;
                case (int)User.WM.WM_QUIT:                      sMsg = "WM_QUIT";                      break;

                // MDI messages (http://msdn.microsoft.com/en-us/library/ms674861(VS.85).aspx)
                case (int)User.WM.WM_MDIACTIVATE:               sMsg = "WM_MDIACTIVATE";               break;
                case (int)User.WM.WM_MDICASCADE:                sMsg = "WM_MDICASCADE";                break;
                case (int)User.WM.WM_MDICREATE:                 sMsg = "WM_MDICREATE";                 break;
                case (int)User.WM.WM_MDIDESTROY:                sMsg = "WM_MDIDESTROY";                break;
                case (int)User.WM.WM_MDIGETACTIVE:              sMsg = "WM_MDIGETACTIVE";              break;
                case (int)User.WM.WM_MDIICONARRANGE:            sMsg = "WM_MDIICONARRANGE";            break;
                case (int)User.WM.WM_MDIMAXIMIZE:               sMsg = "WM_MDIMAXIMIZE";               break;
                case (int)User.WM.WM_MDINEXT:                   sMsg = "WM_MDINEXT";                   break;
                case (int)User.WM.WM_MDIREFRESHMENU:            sMsg = "WM_MDIREFRESHMENU";            break;
                case (int)User.WM.WM_MDIRESTORE:                sMsg = "WM_MDIRESTORE";                break;
                case (int)User.WM.WM_MDISETMENU:                sMsg = "WM_MDISETMENU";                break;
                case (int)User.WM.WM_MDITILE:                   sMsg = "WM_MDITILE";                   break;

                // Timer notifications (http://msdn.microsoft.com/en-us/library/ms674867(VS.85).aspx)
                case (int)User.WM.WM_TIMER:                     sMsg = "WM_TIMER";                     break;

                // Windows messages (http://msdn.microsoft.com/en-us/library/ms674886(VS.85).aspx)
                case User.Const.MN_GETHMENU:                    sMsg = "MN_GETHMENU";                  break;
                case (int)User.WM.WM_GETFONT:                   sMsg = "WM_GETFONT";                   break;
                case (int)User.WM.WM_GETTEXT:                   sMsg = "WM_GETTEXT";                   break;
                case (int)User.WM.WM_GETTEXTLENGTH:             sMsg = "WM_GETTEXTLENGTH";             break;
                case (int)User.WM.WM_SETFONT:                   sMsg = "WM_SETFONT";                   break;
                case (int)User.WM.WM_SETICON:                   sMsg = "WM_SETICON";                   break;
                case (int)User.WM.WM_SETTEXT:                   sMsg = "WM_SETTEXT";                   break;

                // Windows notifications (http://msdn.microsoft.com/en-us/library/ms674887(VS.85).aspx)
                case (int)User.WM.WM_ACTIVATEAPP:               sMsg = "WM_ACTIVATEAPP";               break;
                case (int)User.WM.WM_CANCELMODE:                sMsg = "WM_CANCELMODE";                break;
                case (int)User.WM.WM_CHILDACTIVATE:             sMsg = "WM_CHILDACTIVATE";             break;
                case (int)User.WM.WM_CLOSE:                     sMsg = "WM_CLOSE";                     break;
                case (int)User.WM.WM_COMPACTING:                sMsg = "WM_COMPACTING";                break;
                case (int)User.WM.WM_CREATE:                    sMsg = "WM_CREATE";                    break;
                case (int)User.WM.WM_DESTROY:                   sMsg = "WM_DESTROY";                   break;
                case (int)User.WM.WM_ENABLE:                    sMsg = "WM_ENABLE";                    break;
                case (int)User.WM.WM_ENTERSIZEMOVE:             sMsg = "WM_ENTERSIZEMOVE";             break;
                case (int)User.WM.WM_EXITSIZEMOVE:              sMsg = "WM_EXITSIZEMOVE";              break;
                case (int)User.WM.WM_GETICON:                   sMsg = "WM_GETICON";                   break;
                case (int)User.WM.WM_GETMINMAXINFO:             sMsg = "WM_GETMINMAXINFO";             break;
                case (int)User.WM.WM_INPUTLANGCHANGE:           sMsg = "WM_INPUTLANGCHANGE";           break;
                case (int)User.WM.WM_INPUTLANGCHANGEREQUEST:    sMsg = "WM_INPUTLANGCHANGEREQUEST";    break;
                case (int)User.WM.WM_MOVE:                      sMsg = "WM_MOVE";                      break;
                case (int)User.WM.WM_MOVING:                    sMsg = "WM_MOVING";                    break;
                case (int)User.WM.WM_NCACTIVATE:                sMsg = "WM_NCACTIVATE";                break;
                case (int)User.WM.WM_NCCALCSIZE:                sMsg = "WM_NCCALCSIZE";                break;
                case (int)User.WM.WM_NCCREATE:                  sMsg = "WM_NCCREATE";                  break;
                case (int)User.WM.WM_NCDESTROY:                 sMsg = "WM_NCDESTROY";                 break;
                case (int)User.WM.WM_NULL:                      sMsg = "WM_NULL";                      break;
                case (int)User.WM.WM_PARENTNOTIFY:              sMsg = "WM_PARENTNOTIFY";              break;
                case (int)User.WM.WM_QUERYDRAGICON:             sMsg = "WM_QUERYDRAGICON";             break;
                case (int)User.WM.WM_QUERYOPEN:                 sMsg = "WM_QUERYOPEN";                 break;
                case (int)User.WM.WM_SHOWWINDOW:                sMsg = "WM_SHOWWINDOW";                break;
                case (int)User.WM.WM_SIZE:                      sMsg = "WM_SIZE";                      break;
                case (int)User.WM.WM_SIZING:                    sMsg = "WM_SIZING";                    break;
                case (int)User.WM.WM_STYLECHANGED:              sMsg = "WM_STYLECHANGED";              break;
                case (int)User.WM.WM_STYLECHANGING:             sMsg = "WM_STYLECHANGING";             break;
                case (int)User.WM.WM_THEMECHANGED:              sMsg = "WM_THEMECHANGED";              break;
                case (int)User.WM.WM_USERCHANGED:               sMsg = "WM_USERCHANGED";               break;
                case (int)User.WM.WM_WINDOWPOSCHANGED:          sMsg = "WM_WINDOWPOSCHANGED";          break;
                case (int)User.WM.WM_WINDOWPOSCHANGING:         sMsg = "WM_WINDOWPOSCHANGING";         break;

                // Clipboard messages (http://msdn.microsoft.com/en-us/library/ms674558(VS.85).aspx)
                case (int)User.WM.WM_CLEAR:                     sMsg = "WM_CLEAR";                     break;
                case (int)User.WM.WM_COPY:                      sMsg = "WM_COPY";                      break;
                case (int)User.WM.WM_CUT:                       sMsg = "WM_CUT";                       break;
                case (int)User.WM.WM_PASTE:                     sMsg = "WM_PASTE";                     break;

                // Clipboard notifications (http://msdn.microsoft.com/en-us/library/ms674561(VS.85).aspx)
                case (int)User.WM.WM_ASKCBFORMATNAME:           sMsg = "WM_ASKCBFORMATNAME";           break;
                case (int)User.WM.WM_CHANGECBCHAIN:             sMsg = "WM_CHANGECBCHAIN";             break;
                case (int)User.WM.WM_CLIPBOARDUPDATE:           sMsg = "WM_CLIPBOARDUPDATE";           break;
                case (int)User.WM.WM_DESTROYCLIPBOARD:          sMsg = "WM_DESTROYCLIPBOARD";          break;
                case (int)User.WM.WM_DRAWCLIPBOARD:             sMsg = "WM_DRAWCLIPBOARD";             break;
                case (int)User.WM.WM_HSCROLLCLIPBOARD:          sMsg = "WM_HSCROLLCLIPBOARD";          break;
                case (int)User.WM.WM_PAINTCLIPBOARD:            sMsg = "WM_PAINTCLIPBOARD";            break;
                case (int)User.WM.WM_RENDERALLFORMATS:          sMsg = "WM_RENDERALLFORMATS";          break;
                case (int)User.WM.WM_RENDERFORMAT:              sMsg = "WM_RENDERFORMAT";              break;
                case (int)User.WM.WM_SIZECLIPBOARD:             sMsg = "WM_SIZECLIPBOARD";             break;
                case (int)User.WM.WM_VSCROLLCLIPBOARD:          sMsg = "WM_VSCROLLCLIPBOARD";          break;

                // Data copy messages (http://msdn.microsoft.com/en-us/library/ms674575(VS.85).aspx)
                case (int)User.WM.WM_COPYDATA:                  sMsg = "WM_COPYDATA";                  break;

                // DDE messages (http://msdn.microsoft.com/en-us/library/ms674594(VS.85).aspx)
                case User.Const.WM_DDE_INITIATE:                sMsg = "WM_DDE_INITIATE";              break;

                // DDE notifications (http://msdn.microsoft.com/en-us/library/ms674595(VS.85).aspx)
                case User.Const.WM_DDE_ACK:                     sMsg = "WM_DDE_ACK";                   break;
                case User.Const.WM_DDE_ADVISE:                  sMsg = "WM_DDE_ADVISE";                break;
                case User.Const.WM_DDE_DATA:                    sMsg = "WM_DDE_DATA";                  break;
                case User.Const.WM_DDE_EXECUTE:                 sMsg = "WM_DDE_EXECUTE";               break;
                case User.Const.WM_DDE_POKE:                    sMsg = "WM_DDE_POKE";                  break;
                case User.Const.WM_DDE_REQUEST:                 sMsg = "WM_DDE_REQUEST";               break;
                case User.Const.WM_DDE_TERMINATE:               sMsg = "WM_DDE_TERMINATE";             break;
                case User.Const.WM_DDE_UNADVISE:                sMsg = "WM_DDE_UNADVISE";              break;

                // Cursor notifications (http://msdn.microsoft.com/en-us/library/ms674646(VS.85).aspx)
                case (int)User.WM.WM_SETCURSOR:                 sMsg = "WM_SETCURSOR";                 break;

                // Icon notifications (http://msdn.microsoft.com/en-us/library/ms674655(VS.85).aspx)
                case (int)User.WM.WM_ERASEBKGND:                sMsg = "WM_ERASEBKGND";                break;
                case (int)User.WM.WM_ICONERASEBKGND:            sMsg = "WM_ICONERASEBKGND";            break;
                case (int)User.WM.WM_PAINTICON:                 sMsg = "WM_PAINTICON";                 break;

                // Menu notifications (http://msdn.microsoft.com/en-us/library/ms674668(VS.85).aspx)
                case (int)User.WM.WM_COMMAND:                   sMsg = "WM_COMMAND";                   break;
                case (int)User.WM.WM_CONTEXTMENU:               sMsg = "WM_CONTEXTMENU";               break;
                case (int)User.WM.WM_ENTERMENULOOP:             sMsg = "WM_ENTERMENULOOP";             break;
                case (int)User.WM.WM_EXITMENULOOP:              sMsg = "WM_EXITMENULOOP";              break;
                case (int)User.WM.WM_GETTITLEBARINFOEX:         sMsg = "WM_GETTITLEBARINFOEX";         break;
                case (int)User.WM.WM_MENUCOMMAND:               sMsg = "WM_MENUCOMMAND";               break;
                case (int)User.WM.WM_MENUDRAG:                  sMsg = "WM_MENUDRAG";                  break;
                case (int)User.WM.WM_MENUGETOBJECT:             sMsg = "WM_MENUGETOBJECT";             break;
                case (int)User.WM.WM_MENURBUTTONUP:             sMsg = "WM_MENURBUTTONUP";             break;
                case (int)User.WM.WM_NEXTMENU:                  sMsg = "WM_NEXTMENU";                  break;
                case (int)User.WM.WM_UNINITMENUPOPUP:           sMsg = "WM_UNINITMENUPOPUP";           break;

                // Shell messages and notifications
                //case User.Const.ABM_ACTIVATE:                 sMsg = "ABM_ACTIVATE";                 break;
                //case User.Const.ABM_GETAUTOHIDEBAR:           sMsg = "ABM_GETAUTOHIDEBAR";           break;
                //case User.Const.ABM_GETSTATE:                 sMsg = "ABM_GETSTATE";                 break;
                //case User.Const.ABM_GETTASKBARPOS:            sMsg = "ABM_GETTASKBARPOS";            break;
                //case User.Const.ABM_NEW:                      sMsg = "ABM_NEW";                      break;
                //case User.Const.ABM_QUERYPOS:                 sMsg = "ABM_QUERYPOS";                 break;
                //case User.Const.ABM_REMOVE:                   sMsg = "ABM_REMOVE";                   break;
                //case User.Const.ABM_SETAUTOHIDEBAR:           sMsg = "ABM_SETAUTOHIDEBAR";           break;
                //case User.Const.ABM_SETPOS:                   sMsg = "ABM_SETPOS";                   break;
                //case User.Const.ABM_SETSTATE:                 sMsg = "ABM_SETSTATE";                 break;
                //case User.Const.ABM_WINDOWPOSCHANGED:         sMsg = "ABM_WINDOWPOSCHANGED";         break;
                //case User.Const.ABN_FULLSCREENAPP:            sMsg = "ABN_FULLSCREENAPP";            break;
                //case User.Const.ABN_POSCHANGED:               sMsg = "ABN_POSCHANGED";               break;
                //case User.Const.ABN_STATECHANGE:              sMsg = "ABN_STATECHANGE";              break;
                //case User.Const.ABN_WINDOWARRANGE:            sMsg = "ABN_WINDOWARRANGE";            break;
                //case User.Const.CPL_DBLCLK:                   sMsg = "CPL_DBLCLK";                   break;
                //case User.Const.CPL_EXIT:                     sMsg = "CPL_EXIT";                     break;
                //case User.Const.CPL_GETCOUNT:                 sMsg = "CPL_GETCOUNT";                 break;
                //case User.Const.CPL_INIT:                     sMsg = "CPL_INIT";                     break;
                //case User.Const.CPL_INQUIRE:                  sMsg = "CPL_INQUIRE";                  break;
                //case User.Const.CPL_NEWINQUIRE:               sMsg = "CPL_NEWINQUIRE";               break;
                //case User.Const.CPL_SELECT:                   sMsg = "CPL_SELECT";                   break;
                //case User.Const.CPL_STARTWPARMS:              sMsg = "CPL_STARTWPARMS";              break;
                //case User.Const.CPL_STOP:                     sMsg = "CPL_STOP";                     break;
                //case User.Const.DFM_GETDEFSTATICID:           sMsg = "DFM_GETDEFSTATICID";           break;
                //case User.Const.DFM_GETHELPTEXT:              sMsg = "DFM_GETHELPTEXT";              break;
                //case User.Const.DFM_GETHELPTEXTW:             sMsg = "DFM_GETHELPTEXTW";             break;
                //case User.Const.DFM_GETVERB:                  sMsg = "DFM_GETVERB";                  break;
                //case User.Const.DFM_INVOKECOMMAND:            sMsg = "DFM_INVOKECOMMAND";            break;
                //case User.Const.DFM_INVOKECOMMANDEX:          sMsg = "DFM_INVOKECOMMANDEX";          break;
                //case User.Const.DFM_MAPCOMMANDNAME:           sMsg = "DFM_MAPCOMMANDNAME";           break;
                //case User.Const.DFM_MERGECONTEXTMENU:         sMsg = "DFM_MERGECONTEXTMENU";         break;
                //case User.Const.DFM_MERGECONTEXTMENU_BOTTOM:  sMsg = "DFM_MERGECONTEXTMENU_BOTTOM";  break;
                //case User.Const.DFM_MERGECONTEXTMENU_TOP:     sMsg = "DFM_MERGECONTEXTMENU_TOP";     break;
                //case User.Const.DFM_MODIFYQCMFLAGS:           sMsg = "DFM_MODIFYQCMFLAGS";           break;
                //case User.Const.DFM_VALIDATECMD:              sMsg = "DFM_VALIDATECMD";              break;
                //case User.Const.DFM_WM_DRAWITEM:              sMsg = "DFM_WM_DRAWITEM";              break;
                //case User.Const.DFM_WM_INITMENUPOPUP:         sMsg = "DFM_WM_INITMENUPOPUP";         break;
                //case User.Const.DFM_WM_MEASUREITEM:           sMsg = "DFM_WM_MEASUREITEM";           break;
                //case User.Const.FM_GETDRIVEINFO:              sMsg = "FM_GETDRIVEINFO";              break;
                //case User.Const.FM_GETFILESEL:                sMsg = "FM_GETFILESEL";                break;
                //case User.Const.FM_GETFILESELLFN:             sMsg = "FM_GETFILESELLFN";             break;
                //case User.Const.FM_GETFOCUS:                  sMsg = "FM_GETFOCUS";                  break;
                //case User.Const.FM_GETSELCOUNT:               sMsg = "FM_GETSELCOUNT";               break;
                //case User.Const.FM_GETSELCOUNTLFN:            sMsg = "FM_GETSELCOUNTLFN";            break;
                //case User.Const.FM_REFRESH_WINDOWS:           sMsg = "FM_REFRESH_WINDOWS";           break;
                //case User.Const.FM_RELOAD_EXTENSIONS:         sMsg = "FM_RELOAD_EXTENSIONS";         break;
                //case User.Const.FMEVENT_HELPMENUITEM:         sMsg = "FMEVENT_HELPMENUITEM";         break;
                //case User.Const.FMEVENT_HELPSTRING:           sMsg = "FMEVENT_HELPSTRING";           break;
                //case User.Const.FMEVENT_INITMENU:             sMsg = "FMEVENT_INITMENU";             break;
                //case User.Const.FMEVENT_LOAD:                 sMsg = "FMEVENT_LOAD";                 break;
                //case User.Const.FMEVENT_SELCHANGE:            sMsg = "FMEVENT_SELCHANGE";            break;
                //case User.Const.FMEVENT_TOOLBARLOAD:          sMsg = "FMEVENT_TOOLBARLOAD";          break;
                //case User.Const.FMEVENT_UNLOAD:               sMsg = "FMEVENT_UNLOAD";               break;
                //case User.Const.FMEVENT_USER_REFRESH:         sMsg = "FMEVENT_USER_REFRESH";         break;
                //case User.Const.SFVM_ADDOBJECT:               sMsg = "SFVM_ADDOBJECT";               break;
                //case User.Const.SFVM_ADDPROPERTYPAGES:        sMsg = "SFVM_ADDPROPERTYPAGES";        break;
                //case User.Const.SFVM_BACKGROUNDENUM:          sMsg = "SFVM_BACKGROUNDENUM";          break;
                //case User.Const.SFVM_BACKGROUNDENUMDONE:      sMsg = "SFVM_BACKGROUNDENUMDONE";      break;
                //case User.Const.SFVM_COLUMNCLICK:             sMsg = "SFVM_COLUMNCLICK";             break;
                //case User.Const.SFVM_DEFITEMCOUNT:            sMsg = "SFVM_DEFITEMCOUNT";            break;
                //case User.Const.SFVM_DEFVIEWMODE:             sMsg = "SFVM_DEFVIEWMODE";             break;
                //case User.Const.SFVM_DIDDRAGDROP:             sMsg = "SFVM_DIDDRAGDROP";             break;
                //case User.Const.SFVM_FSNOTIFY:                sMsg = "SFVM_FSNOTIFY";                break;
                //case User.Const.SFVM_GETANIMATION:            sMsg = "SFVM_GETANIMATION";            break;
                //case User.Const.SFVM_GETBUTTONINFO:           sMsg = "SFVM_GETBUTTONINFO";           break;
                //case User.Const.SFVM_GETBUTTONS:              sMsg = "SFVM_GETBUTTONS";              break;
                //case User.Const.SFVM_GETDETAILSOF:            sMsg = "SFVM_GETDETAILSOF";            break;
                //case User.Const.SFVM_GETHELPTEXT:             sMsg = "SFVM_GETHELPTEXT";             break;
                //case User.Const.SFVM_GETHELPTOPIC:            sMsg = "SFVM_GETHELPTOPIC";            break;
                //case User.Const.SFVM_GETNOTIFY:               sMsg = "SFVM_GETNOTIFY";               break;
                //case User.Const.SFVM_GETPANE:                 sMsg = "SFVM_GETPANE";                 break;
                //case User.Const.SFVM_GETSELECTEDOBJECTS:      sMsg = "SFVM_GETSELECTEDOBJECTS";      break;
                //case User.Const.SFVM_GETSORTDEFAULTS:         sMsg = "SFVM_GETSORTDEFAULTS";         break;
                //case User.Const.SFVM_GETTOOLTIPTEXT:          sMsg = "SFVM_GETTOOLTIPTEXT";          break;
                //case User.Const.SFVM_GETZONE:                 sMsg = "SFVM_GETZONE";                 break;
                //case User.Const.SFVM_INITMENUPOPUP:           sMsg = "SFVM_INITMENUPOPUP";           break;
                //case User.Const.SFVM_INVOKECOMMAND:           sMsg = "SFVM_INVOKECOMMAND";           break;
                //case User.Const.SFVM_MERGEMENU:               sMsg = "SFVM_MERGEMENU";               break;
                //case User.Const.SFVM_QUERYFSNOTIFY:           sMsg = "SFVM_QUERYFSNOTIFY";           break;
                //case User.Const.SFVM_REARRANGE:               sMsg = "SFVM_REARRANGE";               break;
                //case User.Const.SFVM_REMOVEOBJECT:            sMsg = "SFVM_REMOVEOBJECT";            break;
                //case User.Const.SFVM_SETCLIPBOARD:            sMsg = "SFVM_SETCLIPBOARD";            break;
                //case User.Const.SFVM_SETISFV:                 sMsg = "SFVM_SETISFV";                 break;
                //case User.Const.SFVM_SETITEMPOS:              sMsg = "SFVM_SETITEMPOS";              break;
                //case User.Const.SFVM_SETPOINTS:               sMsg = "SFVM_SETPOINTS";               break;
                //case User.Const.SFVM_SIZE:                    sMsg = "SFVM_SIZE";                    break;
                //case User.Const.SFVM_THISIDLIST:              sMsg = "SFVM_THISIDLIST";              break;
                //case User.Const.SFVM_UNMERGEMENU:             sMsg = "SFVM_UNMERGEMENU";             break;
                //case User.Const.SFVM_UPDATEOBJECT:            sMsg = "SFVM_UPDATEOBJECT";            break;
                //case User.Const.SFVM_UPDATESTATUSBAR:         sMsg = "SFVM_UPDATESTATUSBAR";         break;
                //case User.Const.SFVM_WINDOWCREATED:           sMsg = "SFVM_WINDOWCREATED";           break;
                //case User.Const.SMC_CHEVRONEXPAND:            sMsg = "SMC_CHEVRONEXPAND";            break;
                //case User.Const.SMC_CHEVRONGETTIP:            sMsg = "SMC_CHEVRONGETTIP";            break;
                //case User.Const.SMC_CREATE:                   sMsg = "SMC_CREATE";                   break;
                //case User.Const.SMC_DEFAULTICON:              sMsg = "SMC_DEFAULTICON";              break;
                //case User.Const.SMC_DEMOTE:                   sMsg = "SMC_DEMOTE";                   break;
                //case User.Const.SMC_DISPLAYCHEVRONTIP:        sMsg = "SMC_DISPLAYCHEVRONTIP";        break;
                //case User.Const.SMC_EXITMENU:                 sMsg = "SMC_EXITMENU";                 break;
                //case User.Const.SMC_GETINFO:                  sMsg = "SMC_GETINFO";                  break;
                //case User.Const.SMC_GETOBJECT:                sMsg = "SMC_GETOBJECT";                break;
                //case User.Const.SMC_GETSFINFO:                sMsg = "SMC_GETSFINFO";                break;
                //case User.Const.SMC_GETSFOBJECT:              sMsg = "SMC_GETSFOBJECT";              break;
                //case User.Const.SMC_INITMENU:                 sMsg = "SMC_INITMENU";                 break;
                //case User.Const.SMC_NEWITEM:                  sMsg = "SMC_NEWITEM";                  break;
                //case User.Const.SMC_PROMOTE:                  sMsg = "SMC_PROMOTE";                  break;
                //case User.Const.SMC_REFRESH:                  sMsg = "SMC_REFRESH";                  break;
                //case User.Const.SMC_SETSFOBJECT:              sMsg = "SMC_SETSFOBJECT";              break;
                //case User.Const.SMC_SFDDRESTRICTED:           sMsg = "SMC_SFDDRESTRICTED";           break;
                //case User.Const.SMC_SFEXEC:                   sMsg = "SMC_SFEXEC";                   break;
                //case User.Const.SMC_SFSELECTITEM:             sMsg = "SMC_SFSELECTITEM";             break;
                //case User.Const.SMC_SHCHANGENOTIFY:           sMsg = "SMC_SHCHANGENOTIFY";           break;
                //case User.Const.WM_CPL_LAUNCH:                sMsg = "WM_CPL_LAUNCH";                break;
                //case User.Const.WM_CPL_LAUNCHED:              sMsg = "WM_CPL_LAUNCHED";              break;
                case (int)User.WM.WM_DROPFILES:                 sMsg = "WM_DROPFILES";                 break;
                case (int)User.WM.WM_HELP:                      sMsg = "WM_HELP";                      break;
                case (int)User.WM.WM_TCARD:                     sMsg = "WM_TCARD";                     break;
            }
            return sMsg;
        }
        #endregion

        #region //GetMessageKeyData
        //public static MessageKeyData GetMessageKeyData(int VKCode, int lParam)
        //{
        //    MessageKeyData keyData = new MessageKeyData();
        //    keyData.VirtualKeyCode = GetVirtualKey(VKCode);
        //    // lParam : 0-15 RepeatCount, 16-23 ScanCode, 24 ExtendedKey, 25-28 reserved, 29 ContextCode, 30 PreviousKeyState, 31 TransitionState
        //    keyData.RepeatCount = (ushort)lParam;
        //    keyData.ScanCode = (byte)(lParam >> 16);
        //    if (((lParam >> 24) & 1) == 1) keyData.ExtendedKey = true; else keyData.ExtendedKey = false;
        //    keyData.ContextCode = (byte)((lParam >> 29) & 1);
        //    keyData.PreviousKeyState = (byte)((lParam >> 30) & 1);
        //    keyData.TransitionState = (byte)((lParam >> 31) & 1);
        //    return keyData;
        //}
        #endregion

        #region GetKeystrokeMessageFlags
        public static KeystrokeMessageFlags GetKeystrokeMessageFlags(uint lParam)
        {
            // doc msdn http://msdn.microsoft.com/en-us/library/ms646267(VS.85).aspx
            //lParam : Specifies the repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag.
            //         For more information about the lParam parameter, see Keystroke Message Flags. This parameter can be one or more of the following values.
            //         0-15  : Specifies the repeat count. The value is the number of times the keystroke is repeated as a result of the user's holding down the key.
            //         16-23 : Specifies the scan code. The value depends on the OEM.
            //         24    : Specifies whether the key is an extended key, such as a function key or a key on the numeric keypad. The value is 1 if the key is an extended key; otherwise, it is 0.
            //         25-28 : Reserved.
            //         29    : Specifies the context code. The value is 1 if the ALT key is down; otherwise, it is 0.
            //         30    : Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
            //         31    : Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.

            KeystrokeMessageFlags keyFlags = new KeystrokeMessageFlags();
            keyFlags.RepeatCount = (ushort)lParam; // bit 0-15
            keyFlags.ScanCode = (byte)(lParam >> 16); // bit 16-23
            if (((lParam >> 24) & 1) == 1) keyFlags.ExtendedKey = true; else keyFlags.ExtendedKey = false; // bit 24
            keyFlags.ContextCode = (byte)((lParam >> 29) & 1);       // bit 29
            keyFlags.PreviousKeyState = (byte)((lParam >> 30) & 1);  // bit 30
            keyFlags.TransitionState = (byte)((lParam >> 31) & 1);   // bit 31
            return keyFlags;
        }
        #endregion

        #region GetKeyboardState
        public static KeyboardState GetKeyboardState()
        {
            KeyboardState ks = new KeyboardState();
            byte[] ks2 = new byte[256];
            User.GetKeyboardState(ks2);
            ks.VKState = ks2;
            ks.Ctrl = (ks2[(int)User.VK.VK_CONTROL] & 0x80) == 0x80;
            ks.LCtrl = (ks2[(int)User.VK.VK_LCONTROL] & 0x80) == 0x80;
            ks.RCtrl = (ks2[(int)User.VK.VK_RCONTROL] & 0x80) == 0x80;
            ks.Alt = (ks2[(int)User.VK.VK_MENU] & 0x80) == 0x80;
            ks.LAlt = (ks2[(int)User.VK.VK_LMENU] & 0x80) == 0x80;
            ks.RAlt = (ks2[(int)User.VK.VK_RMENU] & 0x80) == 0x80;
            ks.LWin = (ks2[(int)User.VK.VK_LWIN] & 0x80) == 0x80;
            ks.RWin = (ks2[(int)User.VK.VK_RWIN] & 0x80) == 0x80;
            ks.Shift = (ks2[(int)User.VK.VK_SHIFT] & 0x80) == 0x80;
            ks.LShift = (ks2[(int)User.VK.VK_LSHIFT] & 0x80) == 0x80;
            ks.RShift = (ks2[(int)User.VK.VK_RSHIFT] & 0x80) == 0x80;
            ks.CapsLock = (ks2[(int)User.VK.VK_CAPITAL] & 1) == 1;
            ks.NumLock = (ks2[(int)User.VK.VK_NUMLOCK] & 1) == 1;
            ks.ScrollLock = (ks2[(int)User.VK.VK_SCROLL] & 1) == 1;
            //VK_PAUSE
            return ks;
        }
        #endregion

        #region GetVirtualKey
        public static User.VK GetVirtualKey(int VKCode)
        {
            switch (VKCode)
            {
                case (int)User.VK.VK_LBUTTON: return User.VK.VK_LBUTTON;
                case (int)User.VK.VK_RBUTTON: return User.VK.VK_RBUTTON;
                case (int)User.VK.VK_CANCEL: return User.VK.VK_CANCEL;
                case (int)User.VK.VK_MBUTTON: return User.VK.VK_MBUTTON;
                case (int)User.VK.VK_XBUTTON1: return User.VK.VK_XBUTTON1;
                case (int)User.VK.VK_XBUTTON2: return User.VK.VK_XBUTTON2;
                case (int)User.VK.VK_BACK: return User.VK.VK_BACK;
                case (int)User.VK.VK_TAB: return User.VK.VK_TAB;
                case (int)User.VK.VK_CLEAR: return User.VK.VK_CLEAR;
                case (int)User.VK.VK_RETURN: return User.VK.VK_RETURN;
                case (int)User.VK.VK_SHIFT: return User.VK.VK_SHIFT;
                case (int)User.VK.VK_CONTROL: return User.VK.VK_CONTROL;
                case (int)User.VK.VK_MENU: return User.VK.VK_MENU;
                case (int)User.VK.VK_PAUSE: return User.VK.VK_PAUSE;
                case (int)User.VK.VK_CAPITAL: return User.VK.VK_CAPITAL;
                case (int)User.VK.VK_KANA: return User.VK.VK_KANA;
                //case (int)User.VK.VK_HANGUL: return User.VK.VK_HANGUL;
                case (int)User.VK.VK_JUNJA: return User.VK.VK_JUNJA;
                case (int)User.VK.VK_FINAL: return User.VK.VK_FINAL;
                case (int)User.VK.VK_HANJA: return User.VK.VK_HANJA;
                //case (int)User.VK.VK_KANJI: return User.VK.VK_KANJI;
                case (int)User.VK.VK_ESCAPE: return User.VK.VK_ESCAPE;
                case (int)User.VK.VK_CONVERT: return User.VK.VK_CONVERT;
                case (int)User.VK.VK_NONCONVERT: return User.VK.VK_NONCONVERT;
                case (int)User.VK.VK_ACCEPT: return User.VK.VK_ACCEPT;
                case (int)User.VK.VK_MODECHANGE: return User.VK.VK_MODECHANGE;
                case (int)User.VK.VK_SPACE: return User.VK.VK_SPACE;
                case (int)User.VK.VK_PRIOR: return User.VK.VK_PRIOR;
                case (int)User.VK.VK_NEXT: return User.VK.VK_NEXT;
                case (int)User.VK.VK_END: return User.VK.VK_END;
                case (int)User.VK.VK_HOME: return User.VK.VK_HOME;
                case (int)User.VK.VK_LEFT: return User.VK.VK_LEFT;
                case (int)User.VK.VK_UP: return User.VK.VK_UP;
                case (int)User.VK.VK_RIGHT: return User.VK.VK_RIGHT;
                case (int)User.VK.VK_DOWN: return User.VK.VK_DOWN;
                case (int)User.VK.VK_SELECT: return User.VK.VK_SELECT;
                case (int)User.VK.VK_PRINT: return User.VK.VK_PRINT;
                case (int)User.VK.VK_EXECUTE: return User.VK.VK_EXECUTE;
                case (int)User.VK.VK_SNAPSHOT: return User.VK.VK_SNAPSHOT;
                case (int)User.VK.VK_INSERT: return User.VK.VK_INSERT;
                case (int)User.VK.VK_DELETE: return User.VK.VK_DELETE;
                case (int)User.VK.VK_HELP: return User.VK.VK_HELP;
                case (int)User.VK.VK_0: return User.VK.VK_0;
                case (int)User.VK.VK_1: return User.VK.VK_1;
                case (int)User.VK.VK_2: return User.VK.VK_2;
                case (int)User.VK.VK_3: return User.VK.VK_3;
                case (int)User.VK.VK_4: return User.VK.VK_4;
                case (int)User.VK.VK_5: return User.VK.VK_5;
                case (int)User.VK.VK_6: return User.VK.VK_6;
                case (int)User.VK.VK_7: return User.VK.VK_7;
                case (int)User.VK.VK_8: return User.VK.VK_8;
                case (int)User.VK.VK_9: return User.VK.VK_9;
                case (int)User.VK.VK_A: return User.VK.VK_A;
                case (int)User.VK.VK_B: return User.VK.VK_B;
                case (int)User.VK.VK_C: return User.VK.VK_C;
                case (int)User.VK.VK_D: return User.VK.VK_D;
                case (int)User.VK.VK_E: return User.VK.VK_E;
                case (int)User.VK.VK_F: return User.VK.VK_F;
                case (int)User.VK.VK_G: return User.VK.VK_G;
                case (int)User.VK.VK_H: return User.VK.VK_H;
                case (int)User.VK.VK_I: return User.VK.VK_I;
                case (int)User.VK.VK_J: return User.VK.VK_J;
                case (int)User.VK.VK_K: return User.VK.VK_K;
                case (int)User.VK.VK_L: return User.VK.VK_L;
                case (int)User.VK.VK_M: return User.VK.VK_M;
                case (int)User.VK.VK_N: return User.VK.VK_N;
                case (int)User.VK.VK_O: return User.VK.VK_O;
                case (int)User.VK.VK_P: return User.VK.VK_P;
                case (int)User.VK.VK_Q: return User.VK.VK_Q;
                case (int)User.VK.VK_R: return User.VK.VK_R;
                case (int)User.VK.VK_S: return User.VK.VK_S;
                case (int)User.VK.VK_T: return User.VK.VK_T;
                case (int)User.VK.VK_U: return User.VK.VK_U;
                case (int)User.VK.VK_V: return User.VK.VK_V;
                case (int)User.VK.VK_W: return User.VK.VK_W;
                case (int)User.VK.VK_X: return User.VK.VK_X;
                case (int)User.VK.VK_Y: return User.VK.VK_Y;
                case (int)User.VK.VK_Z: return User.VK.VK_Z;
                case (int)User.VK.VK_LWIN: return User.VK.VK_LWIN;
                case (int)User.VK.VK_RWIN: return User.VK.VK_RWIN;
                case (int)User.VK.VK_APPS: return User.VK.VK_APPS;
                case (int)User.VK.VK_SLEEP: return User.VK.VK_SLEEP;
                case (int)User.VK.VK_NUMPAD0: return User.VK.VK_NUMPAD0;
                case (int)User.VK.VK_NUMPAD1: return User.VK.VK_NUMPAD1;
                case (int)User.VK.VK_NUMPAD2: return User.VK.VK_NUMPAD2;
                case (int)User.VK.VK_NUMPAD3: return User.VK.VK_NUMPAD3;
                case (int)User.VK.VK_NUMPAD4: return User.VK.VK_NUMPAD4;
                case (int)User.VK.VK_NUMPAD5: return User.VK.VK_NUMPAD5;
                case (int)User.VK.VK_NUMPAD6: return User.VK.VK_NUMPAD6;
                case (int)User.VK.VK_NUMPAD7: return User.VK.VK_NUMPAD7;
                case (int)User.VK.VK_NUMPAD8: return User.VK.VK_NUMPAD8;
                case (int)User.VK.VK_NUMPAD9: return User.VK.VK_NUMPAD9;
                case (int)User.VK.VK_MULTIPLY: return User.VK.VK_MULTIPLY;
                case (int)User.VK.VK_ADD: return User.VK.VK_ADD;
                case (int)User.VK.VK_SEPARATOR: return User.VK.VK_SEPARATOR;
                case (int)User.VK.VK_SUBTRACT: return User.VK.VK_SUBTRACT;
                case (int)User.VK.VK_DECIMAL: return User.VK.VK_DECIMAL;
                case (int)User.VK.VK_DIVIDE: return User.VK.VK_DIVIDE;
                case (int)User.VK.VK_F1: return User.VK.VK_F1;
                case (int)User.VK.VK_F2: return User.VK.VK_F2;
                case (int)User.VK.VK_F3: return User.VK.VK_F3;
                case (int)User.VK.VK_F4: return User.VK.VK_F4;
                case (int)User.VK.VK_F5: return User.VK.VK_F5;
                case (int)User.VK.VK_F6: return User.VK.VK_F6;
                case (int)User.VK.VK_F7: return User.VK.VK_F7;
                case (int)User.VK.VK_F8: return User.VK.VK_F8;
                case (int)User.VK.VK_F9: return User.VK.VK_F9;
                case (int)User.VK.VK_F10: return User.VK.VK_F10;
                case (int)User.VK.VK_F11: return User.VK.VK_F11;
                case (int)User.VK.VK_F12: return User.VK.VK_F12;
                case (int)User.VK.VK_F13: return User.VK.VK_F13;
                case (int)User.VK.VK_F14: return User.VK.VK_F14;
                case (int)User.VK.VK_F15: return User.VK.VK_F15;
                case (int)User.VK.VK_F16: return User.VK.VK_F16;
                case (int)User.VK.VK_F17: return User.VK.VK_F17;
                case (int)User.VK.VK_F18: return User.VK.VK_F18;
                case (int)User.VK.VK_F19: return User.VK.VK_F19;
                case (int)User.VK.VK_F20: return User.VK.VK_F20;
                case (int)User.VK.VK_F21: return User.VK.VK_F21;
                case (int)User.VK.VK_F22: return User.VK.VK_F22;
                case (int)User.VK.VK_F23: return User.VK.VK_F23;
                case (int)User.VK.VK_F24: return User.VK.VK_F24;
                case (int)User.VK.VK_NUMLOCK: return User.VK.VK_NUMLOCK;
                case (int)User.VK.VK_SCROLL: return User.VK.VK_SCROLL;
                case (int)User.VK.VK_OEM_NEC_EQUAL: return User.VK.VK_OEM_NEC_EQUAL;
                //case (int)User.VK.VK_OEM_FJ_JISHO: return User.VK.VK_OEM_FJ_JISHO;
                case (int)User.VK.VK_OEM_FJ_MASSHOU: return User.VK.VK_OEM_FJ_MASSHOU;
                case (int)User.VK.VK_OEM_FJ_TOUROKU: return User.VK.VK_OEM_FJ_TOUROKU;
                case (int)User.VK.VK_OEM_FJ_LOYA: return User.VK.VK_OEM_FJ_LOYA;
                case (int)User.VK.VK_OEM_FJ_ROYA: return User.VK.VK_OEM_FJ_ROYA;
                case (int)User.VK.VK_LSHIFT: return User.VK.VK_LSHIFT;
                case (int)User.VK.VK_RSHIFT: return User.VK.VK_RSHIFT;
                case (int)User.VK.VK_LCONTROL: return User.VK.VK_LCONTROL;
                case (int)User.VK.VK_RCONTROL: return User.VK.VK_RCONTROL;
                case (int)User.VK.VK_LMENU: return User.VK.VK_LMENU;
                case (int)User.VK.VK_RMENU: return User.VK.VK_RMENU;
                case (int)User.VK.VK_BROWSER_BACK: return User.VK.VK_BROWSER_BACK;
                case (int)User.VK.VK_BROWSER_FORWARD: return User.VK.VK_BROWSER_FORWARD;
                case (int)User.VK.VK_BROWSER_REFRESH: return User.VK.VK_BROWSER_REFRESH;
                case (int)User.VK.VK_BROWSER_STOP: return User.VK.VK_BROWSER_STOP;
                case (int)User.VK.VK_BROWSER_SEARCH: return User.VK.VK_BROWSER_SEARCH;
                case (int)User.VK.VK_BROWSER_FAVORITES: return User.VK.VK_BROWSER_FAVORITES;
                case (int)User.VK.VK_BROWSER_HOME: return User.VK.VK_BROWSER_HOME;
                case (int)User.VK.VK_VOLUME_MUTE: return User.VK.VK_VOLUME_MUTE;
                case (int)User.VK.VK_VOLUME_DOWN: return User.VK.VK_VOLUME_DOWN;
                case (int)User.VK.VK_VOLUME_UP: return User.VK.VK_VOLUME_UP;
                case (int)User.VK.VK_MEDIA_NEXT_TRACK: return User.VK.VK_MEDIA_NEXT_TRACK;
                case (int)User.VK.VK_MEDIA_PREV_TRACK: return User.VK.VK_MEDIA_PREV_TRACK;
                case (int)User.VK.VK_MEDIA_STOP: return User.VK.VK_MEDIA_STOP;
                case (int)User.VK.VK_MEDIA_PLAY_PAUSE: return User.VK.VK_MEDIA_PLAY_PAUSE;
                case (int)User.VK.VK_LAUNCH_MAIL: return User.VK.VK_LAUNCH_MAIL;
                case (int)User.VK.VK_LAUNCH_MEDIA_SELECT: return User.VK.VK_LAUNCH_MEDIA_SELECT;
                case (int)User.VK.VK_LAUNCH_APP1: return User.VK.VK_LAUNCH_APP1;
                case (int)User.VK.VK_LAUNCH_APP2: return User.VK.VK_LAUNCH_APP2;
                case (int)User.VK.VK_OEM_1: return User.VK.VK_OEM_1;
                case (int)User.VK.VK_OEM_PLUS: return User.VK.VK_OEM_PLUS;
                case (int)User.VK.VK_OEM_COMMA: return User.VK.VK_OEM_COMMA;
                case (int)User.VK.VK_OEM_MINUS: return User.VK.VK_OEM_MINUS;
                case (int)User.VK.VK_OEM_PERIOD: return User.VK.VK_OEM_PERIOD;
                case (int)User.VK.VK_OEM_2: return User.VK.VK_OEM_2;
                case (int)User.VK.VK_OEM_3: return User.VK.VK_OEM_3;
                case (int)User.VK.VK_OEM_4: return User.VK.VK_OEM_4;
                case (int)User.VK.VK_OEM_5: return User.VK.VK_OEM_5;
                case (int)User.VK.VK_OEM_6: return User.VK.VK_OEM_6;
                case (int)User.VK.VK_OEM_7: return User.VK.VK_OEM_7;
                case (int)User.VK.VK_OEM_8: return User.VK.VK_OEM_8;
                case (int)User.VK.VK_OEM_AX: return User.VK.VK_OEM_AX;
                case (int)User.VK.VK_OEM_102: return User.VK.VK_OEM_102;
                case (int)User.VK.VK_ICO_HELP: return User.VK.VK_ICO_HELP;
                case (int)User.VK.VK_ICO_00: return User.VK.VK_ICO_00;
                case (int)User.VK.VK_PROCESSKEY: return User.VK.VK_PROCESSKEY;
                case (int)User.VK.VK_ICO_CLEAR: return User.VK.VK_ICO_CLEAR;
                case (int)User.VK.VK_PACKET: return User.VK.VK_PACKET;
                case (int)User.VK.VK_OEM_RESET: return User.VK.VK_OEM_RESET;
                case (int)User.VK.VK_OEM_JUMP: return User.VK.VK_OEM_JUMP;
                case (int)User.VK.VK_OEM_PA1: return User.VK.VK_OEM_PA1;
                case (int)User.VK.VK_OEM_PA2: return User.VK.VK_OEM_PA2;
                case (int)User.VK.VK_OEM_PA3: return User.VK.VK_OEM_PA3;
                case (int)User.VK.VK_OEM_WSCTRL: return User.VK.VK_OEM_WSCTRL;
                case (int)User.VK.VK_OEM_CUSEL: return User.VK.VK_OEM_CUSEL;
                case (int)User.VK.VK_OEM_ATTN: return User.VK.VK_OEM_ATTN;
                case (int)User.VK.VK_OEM_FINISH: return User.VK.VK_OEM_FINISH;
                case (int)User.VK.VK_OEM_COPY: return User.VK.VK_OEM_COPY;
                case (int)User.VK.VK_OEM_AUTO: return User.VK.VK_OEM_AUTO;
                case (int)User.VK.VK_OEM_ENLW: return User.VK.VK_OEM_ENLW;
                case (int)User.VK.VK_OEM_BACKTAB: return User.VK.VK_OEM_BACKTAB;
                case (int)User.VK.VK_ATTN: return User.VK.VK_ATTN;
                case (int)User.VK.VK_CRSEL: return User.VK.VK_CRSEL;
                case (int)User.VK.VK_EXSEL: return User.VK.VK_EXSEL;
                case (int)User.VK.VK_EREOF: return User.VK.VK_EREOF;
                case (int)User.VK.VK_PLAY: return User.VK.VK_PLAY;
                case (int)User.VK.VK_ZOOM: return User.VK.VK_ZOOM;
                case (int)User.VK.VK_NONAME: return User.VK.VK_NONAME;
                case (int)User.VK.VK_PA1: return User.VK.VK_PA1;
                case (int)User.VK.VK_OEM_CLEAR: return User.VK.VK_OEM_CLEAR;
            }
            //throw new Exception(string.Format("unknow virtual-key code {0}", VKCode));
            return User.VK.VK_UNKNOW;
        }
        #endregion

        #region GetWA
        public static User.WA GetWA(int value)
        {
            switch (value)
            {
                case (int)User.WA.WA_INACTIVE: return User.WA.WA_INACTIVE;
                case (int)User.WA.WA_ACTIVE: return User.WA.WA_ACTIVE;
                case (int)User.WA.WA_CLICKACTIVE: return User.WA.WA_CLICKACTIVE;
                default: return User.WA.WA_UNKNOW;
            }
            //throw new Win32Exception("Unknow WM_ACTIVATE state value {0}", value);
        }
        #endregion

        #region GetSC
        public static User.SC GetSC(int value)
        {
            switch (value)
            {
                case (int)User.SC.SC_SIZE: return User.SC.SC_SIZE;
                case (int)User.SC.SC_MOVE: return User.SC.SC_MOVE;
                case (int)User.SC.SC_MINIMIZE: return User.SC.SC_MINIMIZE;
                case (int)User.SC.SC_MAXIMIZE: return User.SC.SC_MAXIMIZE;
                case (int)User.SC.SC_NEXTWINDOW: return User.SC.SC_NEXTWINDOW;
                case (int)User.SC.SC_PREVWINDOW: return User.SC.SC_PREVWINDOW;
                case (int)User.SC.SC_CLOSE: return User.SC.SC_CLOSE;
                case (int)User.SC.SC_VSCROLL: return User.SC.SC_VSCROLL;
                case (int)User.SC.SC_HSCROLL: return User.SC.SC_HSCROLL;
                case (int)User.SC.SC_MOUSEMENU: return User.SC.SC_MOUSEMENU;
                case (int)User.SC.SC_KEYMENU: return User.SC.SC_KEYMENU;
                case (int)User.SC.SC_ARRANGE: return User.SC.SC_ARRANGE;
                case (int)User.SC.SC_RESTORE: return User.SC.SC_RESTORE;
                case (int)User.SC.SC_TASKLIST: return User.SC.SC_TASKLIST;
                case (int)User.SC.SC_SCREENSAVE: return User.SC.SC_SCREENSAVE;
                case (int)User.SC.SC_HOTKEY: return User.SC.SC_HOTKEY;
                case (int)User.SC.SC_DEFAULT: return User.SC.SC_DEFAULT;
                case (int)User.SC.SC_MONITORPOWER: return User.SC.SC_MONITORPOWER;
                case (int)User.SC.SC_CONTEXTHELP: return User.SC.SC_CONTEXTHELP;
                case (int)User.SC.SC_SEPARATOR: return User.SC.SC_SEPARATOR;
                default: return User.SC.SC_UNKNOW;
            }
            //throw new Win32Exception("Unknow type of system command (WM_SYSCOMMAND) {0}", value);
        }
        #endregion

        #region //WM_SYSCOMMAND_wParam_ToString
        //public static string WM_SYSCOMMAND_wParam_ToString(IntPtr wParam)
        //{
        //    int iwParam = wParam.ToInt32();
        //    switch (iwParam)
        //    {
        //        case User.Const.SC_ARRANGE: return "SC_ARRANGE";
        //        case User.Const.SC_CLOSE: return "SC_CLOSE";
        //        case User.Const.SC_HOTKEY: return "SC_HOTKEY";
        //        case User.Const.SC_HSCROLL: return "SC_HSCROLL";
        //        //case User_const.SC_ICON: return "SC_MINIMIZE";
        //        case User.Const.SC_KEYMENU: return "SC_KEYMENU";
        //        case User.Const.SC_MAXIMIZE: return "SC_MAXIMIZE";
        //        case User.Const.SC_MINIMIZE: return "SC_MINIMIZE";
        //        case User.Const.SC_MOUSEMENU: return "SC_MOUSEMENU";
        //        case User.Const.SC_MOVE: return "SC_MOVE";
        //        case User.Const.SC_NEXTWINDOW: return "SC_NEXTWINDOW";
        //        case User.Const.SC_PREVWINDOW: return "SC_PREVWINDOW";
        //        case User.Const.SC_RESTORE: return "SC_RESTORE";
        //        case User.Const.SC_SCREENSAVE: return "SC_SCREENSAVE";
        //        case User.Const.SC_SIZE: return "SC_SIZE";
        //        case User.Const.SC_TASKLIST: return "SC_TASKLIST";
        //        case User.Const.SC_VSCROLL: return "SC_VSCROLL";
        //        //case User_const.SC_ZOOM: return "SC_MAXIMIZE";
        //        default: return iwParam.ToString();
        //    }
        //}
        #endregion

    }
    #endregion
}

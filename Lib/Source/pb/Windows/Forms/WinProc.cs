using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using pb.Windows.Win32;

namespace pb.Windows.Forms
{
    #region class WindowsAppException
    public class WindowsAppException : Exception
    {
        public WindowsAppException(string sMessage) : base(sMessage) { }
        public WindowsAppException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public WindowsAppException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public WindowsAppException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region class WindowData
    public class WindowData
    {
        public Form Form;
        public IWinProc WinProc;
        //public FormWinProc FormWinProc;
        public bool EnableCtrlF4Close = true;
        public bool DisableAltF4 = true;
        public bool CloseApplicationOnAltF4 = false;
    }
    #endregion

    #region class OrderedWindows
    public class OrderedWindows
    {
        #region variable
        private List<Form> gOrderedWindows = new List<Form>();
        #endregion

        #region Count
        public int Count
        {
            get { return gOrderedWindows.Count; }
        }
        #endregion

        #region this[int i]
        public Form this[int i]
        {
            get { if (i < gOrderedWindows.Count) return gOrderedWindows[i]; else return null; }
            set { gOrderedWindows[i] = value; }
        }
        #endregion

        #region Remove
        public void Remove(Form window)
        {
            int i = gOrderedWindows.IndexOf(window);
            if (i != -1) gOrderedWindows.RemoveAt(i);
        }
        #endregion

        #region SetFirst
        public void SetFirst(Form window)
        {
            int i = gOrderedWindows.IndexOf(window);
            if (i == 0) return;
            if (i != -1) gOrderedWindows.RemoveAt(i);
            gOrderedWindows.Insert(0, window);
        }
        #endregion

        #region PreviousWindow
        public Form PreviousWindow(Form window)
        {
            if (gOrderedWindows.Count < 2) return null;
            int i = gOrderedWindows.IndexOf(window);
            if (i == -1) return null;
            if (--i < 0) i = gOrderedWindows.Count - 1;
            return gOrderedWindows[i];
        }
        #endregion

        #region NextWindow
        public Form NextWindow(Form window)
        {
            if (gOrderedWindows.Count < 2) return null;
            int i = gOrderedWindows.IndexOf(window);
            if (i == -1) return null;
            if (++i >= gOrderedWindows.Count) i = 0;
            return gOrderedWindows[i];
        }
        #endregion
    }
    #endregion

    #region enum WAOptions
    [Flags]
    public enum WAOptions
    {
        /// <summary>option minimize : quand on réduit une fenêtre, c'est la fenêtre parent qui est réduite, donc plus aucune fenêtre n'est visible</summary>
        MinimizeParentWindowWhenMinimize = 0x0001,
        /// <summary>option minimize : quand on réduit une fenêtre elle est cachée, si toutes les fenêtres sont cachées la fenetre parent est cachée donc plus d'icone de la barre des taches</summary>
        HideWindowWhenMinimize           = 0x0002,
        /// <summary>cache la fenetre au lieu de la fermer</summary>
        HideWindowWhenClose              = 0x0004,
        /// <summary>CTRL-F4 ferme la fenetre</summary>
        EnableCtrlF4Close                = 0x0008,
        /// <summary>désactive la fermeture de la fenetre avec ALT-F4</summary>
        DisableAltF4                     = 0x0010,
        /// <summary>ALT-F4 ferme l'application</summary>
        CloseApplicationOnAltF4          = 0x0020,
        /// <summary>Create notify icon</summary>
        CreateNotifyIcon                 = 0x0040,
        /// <summary>Minimize parent window when minimize, enable ctrl-F4 close, disable alt-F4, create notify icon</summary>
        Default                          = MinimizeParentWindowWhenMinimize | EnableCtrlF4Close | DisableAltF4 | CreateNotifyIcon,
        /// <summary>Hide window when minimize, hide window when close, close application on alt-F4, create notify icon</summary>
        Type2                            = HideWindowWhenMinimize | HideWindowWhenClose | CloseApplicationOnAltF4 | CreateNotifyIcon
    }
    #endregion

    #region doc class WindowsApp
    // Principe :
    //   Pouvoir afficher plusieurs fenêtres indépendante (sans fenêtre mère)
    //     - création d'une fenêtre parent cachée qui permet d'afficher une icone dans la barre de tâche
    //       pour Windows la fenêtre est visible mais a une position en dehors de l'écran (x=-32000, y=-32000)
    //     - chaque fenêtre indépendante a pour parent la fenêtre parent cachée.
    //     - gestion CTRL-TAB
    //     - gestion des fonctions : minimize, restore.
    //       - gbMinimizeParentWindowWhenMinimize = true :
    //         quand on réduit une fenêtre, c'est la fenêtre parent qui est réduite, donc plus aucune fenêtre n'est visible,
    //         quand on restore la fenêtre parent, toutes les fenêtres redeviennent visible.
    //       - gbHideWindowWhenMinimize = true :
    //         quand on réduit une fenêtre elle est cachée, si toutes les fenêtres sont cachées la fenetre parent est cachée donc plus d'icone de la barre des taches,
    //         cette option est utilisée avec un icone dans system tray
    //     - si plus aucune fenetre n'est visible, la fenetre parent est cachée
    //     - option gbHideWindowWhenClose : cache la fenetre au lieu de la fermer
    //     - option gbEnableCtrlF4Close : CTRL-F4 ferme la fenetre
    //     - option gbDisableAltF4 : désactive la fermeture de la fenetre avec ALT-F4
    //     - option gbCloseApplicationOnAltF4 : ALT-F4 ferme l'application
    //     - ferme l'application lors de la fermeture de session (WM_ENDSESSION)

    // a faire :
    //   Dispose()
    #endregion

    public class WindowsApp
    {
        //private static ITrace _tr = pb.Trace.CurrentTrace;
        private static WindowsApp gCurrentWindowsApp = null;
        private static bool gControlApplicationExit = false;
        private static bool gbTrace = false;

        private WindowData gParentWindow = null;
        private TrayIcon gTrayIcon = null;
        private NotifyIcon gNotifyIcon = null;
        private ContextMenuStrip gNotifyMenu = null;
        private Icon gIcon = null;

        private bool gbMinimizeParentWindowWhenMinimize = true;  // option minimize : quand on réduit une fenêtre, c'est la fenêtre parent qui est réduite, donc plus aucune fenêtre n'est visible
        private bool gbHideWindowWhenMinimize = false;           // option minimize : quand on réduit une fenêtre elle est cachée, si toutes les fenêtres sont cachées la fenetre parent est cachée donc plus d'icone de la barre des taches
        private bool gbHideWindowWhenClose = false;              // cache la fenetre au lieu de la fermer
        private bool gbEnableCtrlF4Close = true;                 // CTRL-F4 ferme la fenetre
        private bool gbDisableAltF4 = true;                      // désactive la fermeture de la fenetre avec ALT-F4
        private bool gbCloseApplicationOnAltF4 = false;          // ALT-F4 ferme l'application

        private bool gbDisableSetFirstWindow = false;            // Désactive le changement de la 1ère fenêtre pendant SC_MINIMIZE de la fenêtre parent
        private bool gbWindowActivatedByKeyOngoing = false;      // gestion de CTRL-TAB
        private bool gbApplicationExitInProgress = false;

        private bool gbNotifyMenu_SetParentWindowVisible = false;
        private bool gbNotifyMenu_ParentWindowShowInTaskbar = false;

        private WindowData gFirstWindow = null; // 1ère fenetre fille ajouté
        private SortedList<int, WindowData> gWindows = new SortedList<int, WindowData>();
        private List<WindowData> gWindowsTmp = new List<WindowData>(); // liste des fenetres en cours de création
        private OrderedWindows gOrderedWindows = new OrderedWindows();
        private KeyboardHook gKeyboardHook = null;
        //private PreWindowHook gPreWindowHook = null;
        //private PostWindowHook gPostWindowHook = null;
        //private GetMessageWindowHook gGetMessageWindowHook = null;
        //public MessageFilter AppMessageFilter = null;

        public delegate void EventApplicationExiting();
        public EventApplicationExiting ApplicationExiting = null;

        #region constructor
        public WindowsApp()
        {
            Init(null, null, null, WAOptions.Default);
        }

        public WindowsApp(Form parentWindow)
        {
            Init(parentWindow, null, null, WAOptions.Default);
        }

        public WindowsApp(Form parentWindow, IWinProc wp)
        {
            Init(parentWindow, wp, null, WAOptions.Default);
        }

        public WindowsApp(Icon icon, WAOptions options)
        {
            Init(null, null, icon, options);
        }
        #endregion

        #region Init
        public void Init(Form parentWindow, IWinProc wp, Icon icon, WAOptions options)
        {

            if (parentWindow == null)
                parentWindow = new MainBaseForm();

            if (wp == null)
            {
                if (parentWindow is IWinProc)
                    wp = (IWinProc)parentWindow;
                else
                    wp = new WinProc(parentWindow);
            }
            parentWindow.ShowInTaskbar = true;
            parentWindow.Load += new EventHandler(parentWindow_Load);

            wp.MessageEvent += new MessageProc(WndProc_ParentWindow);
            gParentWindow = new WindowData() { Form = parentWindow, WinProc = wp, EnableCtrlF4Close = false, DisableAltF4 = false, CloseApplicationOnAltF4 = false };

            //AppMessageFilter = new MessageFilter();
            //AppMessageFilter.PreFilterMessageEvent += new MessageFilter.PreFilterMessageProc(AppPreFilterMessage);
            //Application.AddMessageFilter(AppMessageFilter);

            // gestion CTRL-TAB, CTRL-F4, ALT-F4
            gKeyboardHook = new KeyboardHook();
            gKeyboardHook.HookEvent += new KeyboardHook.HookProc(KeyboardHookProc);

            //gPreWindowHook = new PreWindowHook();
            //gPreWindowHook.HookEvent += new PreWindowHook.HookProc(PreWindowHookProc);

            //gPostWindowHook = new PostWindowHook();
            //gPostWindowHook.HookEvent += new PostWindowHook.HookProc(PostWindowHookProc);

            //gGetMessageWindowHook = new GetMessageWindowHook();
            //gGetMessageWindowHook.HookEvent += new GetMessageWindowHook.HookProc(GetMessageWindowHookProc);
            //gGetMessageWindowHook.Trace = true;

            gbMinimizeParentWindowWhenMinimize = false;
            if ((options & WAOptions.MinimizeParentWindowWhenMinimize) == WAOptions.MinimizeParentWindowWhenMinimize)
                gbMinimizeParentWindowWhenMinimize = true;
            gbHideWindowWhenMinimize = false;
            if ((options & WAOptions.HideWindowWhenMinimize) == WAOptions.HideWindowWhenMinimize)
                gbHideWindowWhenMinimize = true;
            gbHideWindowWhenClose = false;
            if ((options & WAOptions.HideWindowWhenClose) == WAOptions.HideWindowWhenClose)
                gbHideWindowWhenClose = true;
            gbEnableCtrlF4Close = false;
            if ((options & WAOptions.EnableCtrlF4Close) == WAOptions.EnableCtrlF4Close)
                gbEnableCtrlF4Close = true;
            gbDisableAltF4 = false;
            if ((options & WAOptions.DisableAltF4) == WAOptions.DisableAltF4)
                gbDisableAltF4 = true;
            gbCloseApplicationOnAltF4 = false;
            if ((options & WAOptions.CloseApplicationOnAltF4) == WAOptions.CloseApplicationOnAltF4)
                gbCloseApplicationOnAltF4 = true;
            if ((options & WAOptions.CreateNotifyIcon) == WAOptions.CreateNotifyIcon)
                CreateNotifyIcon(icon);
            Icon = icon;

            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : new WindowsApp                            window {0,-25} hWnd {1}", parentWindow.Text, parentWindow.Handle);
        }
        #endregion

        #region parentWindow_Load
        private void parentWindow_Load(object sender, EventArgs e)
        {
            gParentWindow.Form.Location = new Point(-32000, -32000);
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : parentWindow_Load                         window {0,-25} hWnd {1} Location {2}-{3}", gParentWindow.Form.Text, gParentWindow.Form.Handle, gParentWindow.Form.Location.X, gParentWindow.Form.Location.Y);
        }
        #endregion

        #region //parentWindow_Shown
        //void parentWindow_Shown(object sender, EventArgs e)
        //{
        //    ((Form)sender).Location = new Point(-32000, -32000);
        //}
        #endregion

        #region property ...
        #region CurrentWindowsApp
        public static WindowsApp CurrentWindowsApp
        {
            get { return gCurrentWindowsApp; }
            set { gCurrentWindowsApp = value; }
        }
        #endregion

        #region Trace
        public static bool Trace
        {
            get { return gbTrace; }
            set
            {
                gbTrace = value;
                WinProc.Trace = value;
                FormWinProc.Trace = value;
            }
        }
        #endregion

        #region ParentWindow
        public WindowData ParentWindow
        {
            get { return gParentWindow; }
        }
        #endregion

        #region ParentForm
        public Form ParentForm
        {
            get { return gParentWindow.Form; }
        }
        #endregion

        #region TrayIcon
        public TrayIcon TrayIcon
        {
            get { return gTrayIcon; }
        }
        #endregion

        #region NotifyIcon
        public NotifyIcon NotifyIcon
        {
            get { return gNotifyIcon; }
        }
        #endregion

        #region NotifyMenu
        public ContextMenuStrip NotifyMenu
        {
            get { return gNotifyMenu; }
            set
            {
                if (gNotifyMenu != null)
                {
                    gNotifyMenu.Opening -= new CancelEventHandler(NotifyMenu_Opening);
                    gNotifyMenu.Closing -= new ToolStripDropDownClosingEventHandler(NotifyMenu_Closing);
                }
                gNotifyMenu = value;
                gNotifyMenu.Opening += new CancelEventHandler(NotifyMenu_Opening);
                gNotifyMenu.Closing += new ToolStripDropDownClosingEventHandler(NotifyMenu_Closing);
            }
        }
        #endregion

        #region Icon
        public Icon Icon
        {
            get { return gIcon; }
            set
            {
                gIcon = value;
                if (gParentWindow != null) gParentWindow.Form.Icon = value;
                if (gNotifyIcon != null) gNotifyIcon.Icon = value;
            }
        }
        #endregion

        #region DisableSetFirstWindow
        public bool DisableSetFirstWindow
        {
            get { return gbDisableSetFirstWindow; }
            set { gbDisableSetFirstWindow = value; }
        }
        #endregion

        #region MinimizeParentWindowWhenMinimize
        public bool MinimizeParentWindowWhenMinimize
        {
            get { return gbMinimizeParentWindowWhenMinimize; }
            set { gbMinimizeParentWindowWhenMinimize = value; }
        }
        #endregion

        #region HideWindowWhenMinimize
        public bool HideWindowWhenMinimize
        {
            get { return gbHideWindowWhenMinimize; }
            set { gbHideWindowWhenMinimize = value; }
        }
        #endregion

        #region HideWindowWhenClose
        public bool HideWindowWhenClose
        {
            get { return gbHideWindowWhenClose; }
            set { gbHideWindowWhenClose = value; }
        }
        #endregion

        #region EnableCtrlF4Close
        public bool EnableCtrlF4Close
        {
            get { return gbEnableCtrlF4Close; }
            set { gbEnableCtrlF4Close = value; }
        }
        #endregion

        #region DisableAltF4
        public bool DisableAltF4
        {
            get { return gbDisableAltF4; }
            set { gbDisableAltF4 = value; }
        }
        #endregion

        #region CloseApplicationOnAltF4
        public bool CloseApplicationOnAltF4
        {
            get { return gbCloseApplicationOnAltF4; }
            set { gbCloseApplicationOnAltF4 = value; }
        }
        #endregion

        #region Windows
        public SortedList<int, WindowData> Windows
        {
            get { return gWindows; }
        }
        #endregion
        #endregion

        #region CreateNotifyIcon
        public void CreateNotifyIcon(Icon icon)
        {
            if (icon == null) throw new WindowsAppException("Error creating notify icon, icon is null");
            gTrayIcon = new TrayIcon(icon);
            gNotifyIcon = gTrayIcon.NotifyIcon;
            NotifyMenu = gTrayIcon.NotifyMenu;
        }
        #endregion

        #region ApplicationExit
        public void ApplicationExit()
        {
            // ATTENTION : l'appel à thread.Join() à partir du thread principal va bloquer si le thread appel cTrace.Trace()
            //   et que la trace s'affiche dans une TextBox qui fait appel TextBox.Invoke
            //      gRapidshareTask.TaskThread.Join(1000);
            //      gTaskScheduler.Thread.Join(1000);
            //SaveParameters();
            if (gTrayIcon != null) gTrayIcon.Dispose();
            Thread thread = new Thread(new ThreadStart(_ApplicationExit));
            thread.Start();
            thread.Join(2000);
            Application.Exit();
        }
        #endregion

        #region _ApplicationExit
        private void _ApplicationExit()
        {
            if (gbApplicationExitInProgress) return;
            gbApplicationExitInProgress = true;
            //if (gTrayIcon != null) gTrayIcon.Dispose(); // ne pas faire gTrayIcon.Dispose() ici, le gComponents.Dispose() génère une exception  InvalidOperationException : Cross-thread operation not valid: Control '' accessed from a thread other than the thread it was created on
            //if (gNotifyIcon != null) gNotifyIcon.Dispose();
            if (ApplicationExiting != null)
                ApplicationExiting();
            //cTrace.Dispose();
            //Application.Exit();
        }
        #endregion

        #region AddWindow(Form window)
        public WindowData AddWindow(Form window)
        {
            return AddWindow(window, null);
        }
        #endregion

        #region AddWindow(Form window, IWinProc wp)
        public WindowData AddWindow(Form window, IWinProc wp)
        {
            bool bWinProcCreated = false;

            //FormWinProc windowWndProc = null;
            //if (window is FormWinProc)
            //    windowWndProc = (FormWinProc)window;
            if (wp == null)
            {
                if (window is IWinProc)
                    wp = (IWinProc)window;
                else
                {
                    wp = new WinProc(window);
                    //wp.Trace = gbTrace;
                    bWinProcCreated = true;
                }
            }
            if (window.Created && gWindows.ContainsKey(window.Handle.ToInt32()))
            {
                if (bWinProcCreated) wp.Dispose();
                throw new WindowsAppException("error adding window : window is already in window list : {0} - {1}", window.Text, window.Handle);
            }
            // **************************************************************************************************
            // ***** attention ici (après l'appel de gWindows.ContainsKey()) c'est trop tard : ******************
            // **************************************************************************************************
            // *****              if (wp == null) wp = new WinProc(window);                    ******************
            // **************************************************************************************************
            //WindowData windowData = new WindowData() { Form = window, WinProc = wp, EnableCtrlF4Close = EnableCtrlF4Close, DisableAltF4 = DisableAltF4, CloseApplicationOnAltF4 = CloseApplicationOnAltF4 };

            WindowData windowData = null;
            //if (windowWndProc == null)
            //    windowData = new WindowData() { Form = window, WinProc = wp, EnableCtrlF4Close = gbEnableCtrlF4Close, DisableAltF4 = gbDisableAltF4, CloseApplicationOnAltF4 = gbCloseApplicationOnAltF4 };
            //else
            //    windowData = new WindowData() { Form = window, FormWinProc = windowWndProc, EnableCtrlF4Close = gbEnableCtrlF4Close, DisableAltF4 = gbDisableAltF4, CloseApplicationOnAltF4 = gbCloseApplicationOnAltF4 };
            windowData = new WindowData() { Form = window, WinProc = wp, EnableCtrlF4Close = gbEnableCtrlF4Close, DisableAltF4 = gbDisableAltF4, CloseApplicationOnAltF4 = gbCloseApplicationOnAltF4 };

            if (window.Created)
                gWindows.Add(window.Handle.ToInt32(), windowData);
            else
            {
                // si window.Created est false le handle de la fenetre n'est pas encore créé, donc on ajoute la fonction window_Load dans l'event Load
                gWindowsTmp.Add(windowData);
                window.Load += new EventHandler(LoadWindowEvent);
            }
            if (gFirstWindow == null)
            {
                gFirstWindow = windowData;
                gParentWindow.Form.Text = window.Text;
                window.TextChanged += new EventHandler(FirstWindow_TextChanged);
            }

            //if (windowWndProc == null)
            //    wp.MessageEvent += new WinProc.MessageProc(WndProc_Window);
            //else
            //    windowWndProc.MessageEvent += new FormWinProc.MessageProc(WndProc_Window);
            wp.MessageEvent += new MessageProc(WndProc_Window);

            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : AddWindow                                 window {0,-25} hWnd {1}", window.Text, window.Handle);
            return windowData;
        }
        #endregion

        #region LoadWindowEvent
        private void LoadWindowEvent(object sender, EventArgs e)
        {
            Form window = (Form)sender;
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : LoadWindowEvent                           window {0,-25} hWnd {1}", window.Text, window.Handle);
            int i = 0;
            foreach (WindowData windowData in gWindowsTmp)
            {
                if (windowData.Form == window)
                {
                    if (window.Created && gWindows.ContainsKey(window.Handle.ToInt32()))
                    {
                        //if (bWinProcCreated) wp.Dispose();
                        throw new WindowsAppException("error adding window : window is already in window list : {0} - {1}", window.Text, window.Handle);
                    }
                    gWindows.Add(window.Handle.ToInt32(), windowData);
                    window.Load -= new EventHandler(LoadWindowEvent);
                    gWindowsTmp.RemoveAt(i);
                    return;
                }
                i++;
            }
            throw new WindowsAppException("error adding window : window is not found in window temp list : {0} - {1}", window.Text, window.Handle);
        }
        #endregion

        #region RemoveWindow
        public void RemoveWindow(Form window)
        {
            int i = gWindows.IndexOfKey(window.Handle.ToInt32());
            //if (i == -1) throw new Exception("error removing window : window is'nt in window list");
            if (i == -1)
            {
                pb.Trace.WriteLine("error removing window : window is'nt in window list : hWnd {0}, Title {1}", window.Handle, window.Text);
                return;
            }
            WindowData orderedWindow = gWindows.Values[i];
            //if (orderedWindow.WinProc != null)
            //    orderedWindow.WinProc.MessageEvent -= new WinProc.MessageProc(WndProc_Window);
            //else if (orderedWindow.FormWinProc != null)
            //    orderedWindow.FormWinProc.MessageEvent -= new FormWinProc.MessageProc(WndProc_Window);
            orderedWindow.WinProc.MessageEvent -= new MessageProc(WndProc_Window);
            if (gFirstWindow == orderedWindow)
            {
                window.TextChanged -= new EventHandler(FirstWindow_TextChanged);
                gFirstWindow = null;
            }
            gWindows.RemoveAt(i);
            gOrderedWindows.Remove(window);
            if (gWindows.Count == 0)
            {
                //if (gParentWindow.WinProc != null)
                //    gParentWindow.WinProc.MessageEvent -= new WinProc.MessageProc(WndProc_ParentWindow);
                //else if (gParentWindow.FormWinProc != null)
                //    gParentWindow.FormWinProc.MessageEvent -= new FormWinProc.MessageProc(WndProc_ParentWindow);
                gParentWindow.WinProc.MessageEvent -= new MessageProc(WndProc_ParentWindow);
                gParentWindow.Form.Close();
                //Application.RemoveMessageFilter(AppMessageFilter);
                ApplicationExit();
            }
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : RemoveWindow                              window {0,-25} hWnd {1}", window.Text, window.Handle);
        }
        #endregion

        #region SetFirstWindow()
        public void SetFirstWindow()
        {
            SetFirstWindow(Form.ActiveForm);
        }
        #endregion

        #region SetFirstWindow(Form window)
        public void SetFirstWindow(Form window)
        {
            if (window == null || gbDisableSetFirstWindow) return;
            gOrderedWindows.SetFirst(window);
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : SetFirstWindow                            window {0,-25} hWnd {1}", window.Text, window.Handle);
        }
        #endregion

        #region ActivateFirstWindow
        public void ActivateFirstWindow()
        {
            Form window = null;
            string sWindow = "";
            if (gOrderedWindows.Count > 0)
            {
                window = gOrderedWindows[0];
                sWindow = string.Format("window {0,-25} hWnd {1}", window.Text, window.Handle);
            }
            else
                sWindow = "no window to activate";
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : ActivateFirstWindow                       {0}", sWindow);

            if (window != null) window.Activate();
        }
        #endregion

        #region ActivatePreviousWindow
        public void ActivatePreviousWindow()
        {
            if (Form.ActiveForm == null)
            {
                if (gbTrace)
                    pb.Trace.WriteLine("WindowsApp                        : ActivatePreviousWindow                    no active window");
                return;
            }
            if (gOrderedWindows.Count < 2)
            {
                if (gbTrace)
                    pb.Trace.WriteLine("WindowsApp                        : ActivatePreviousWindow                    only {0} window, no window to activate", gOrderedWindows.Count);
                return;
            }
            Form window = gOrderedWindows.PreviousWindow(Form.ActiveForm);
            string sWindow;
            if (window != null) sWindow = string.Format("window {0,-25} hWnd {1}", window.Text, window.Handle); else sWindow = "no window to activate";
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : ActivatePreviousWindow                    window {0,-25} hWnd {1} -> {2}", Form.ActiveForm.Text, Form.ActiveForm.Handle, sWindow);
            if (window != null) window.Activate();
        }
        #endregion

        #region ActivateNextWindow
        public void ActivateNextWindow()
        {
            if (Form.ActiveForm == null)
            {
                if (gbTrace)
                    pb.Trace.WriteLine("WindowsApp                        : ActivateNextWindow                        no active window");
                return;
            }
            if (gOrderedWindows.Count < 2)
            {
                if (gbTrace)
                    pb.Trace.WriteLine("WindowsApp                        : ActivateNextWindow                        only {0} window, no window to activate", gOrderedWindows.Count);
                return;
            }
            Form window = gOrderedWindows.NextWindow(Form.ActiveForm);
            string sWindow;
            if (window != null) sWindow = string.Format("window {0,-25} hWnd {1}", window.Text, window.Handle); else sWindow = "no window to activate";
            if (gbTrace)
                pb.Trace.WriteLine("WindowsApp                        : ActivateNextWindow                        window {0,-25} hWnd {1} -> {2}", Form.ActiveForm.Text, Form.ActiveForm.Handle, sWindow);
            if (window != null) window.Activate();
        }
        #endregion

        #region //NotifyIcon_MouseClick
        //private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left && gNotifyMenu != null)
        //    {
        //        gNotifyIcon.ContextMenuStrip = gNotifyMenu;
        //        MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
        //        mi.Invoke(gNotifyIcon, null);
        //        gNotifyIcon.ContextMenuStrip = null;
        //    }
        //}
        #endregion

        #region NotifyMenu_Opening
        private void NotifyMenu_Opening(object sender, CancelEventArgs e)
        {
            // pour éviter d'avoir un icone blanc dans le task-switcher (alt-tab) il faut qu'une fenetre de l'application est le focus quand le menu est visible
            gbNotifyMenu_SetParentWindowVisible = false;
            if (gParentWindow.Form.Visible == false)
            {
                gbNotifyMenu_ParentWindowShowInTaskbar = gParentWindow.Form.ShowInTaskbar;
                gParentWindow.Form.ShowInTaskbar = false;
                gbNotifyMenu_SetParentWindowVisible = true;
                gParentWindow.Form.Visible = true;
                gParentWindow.Form.Focus();
            }
            else
                ActivateFirstWindow();
        }
        #endregion

        #region NotifyMenu_Closing
        void NotifyMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (gbNotifyMenu_SetParentWindowVisible)
            {
                gParentWindow.Form.Visible = false;
                gParentWindow.Form.ShowInTaskbar = gbNotifyMenu_ParentWindowShowInTaskbar;
                gbNotifyMenu_SetParentWindowVisible = false;
            }
        }
        #endregion

        #region FirstWindow_TextChanged
        void FirstWindow_TextChanged(object sender, EventArgs e)
        {
            gParentWindow.Form.Text = ((Form)sender).Text;
        }
        #endregion

        #region WndProc_ParentWindow
        // Active la 1ère fenêtre fille si la fenêtre mère est activée
        // Désactive le changement de la 1ère fenêtre pendant SC_RESTORE (gbDisableSetFirstWindow)
        private bool WndProc_ParentWindow(WinProcMessage msg)
        {
            string swParam;
            bool bCallBaseWndProc = true;  // si bCallBaseWndProc = true base.WndProc() est appelé

            switch (msg.wm)
            {
                case User.WM.WM_ENDSESSION:
                    string slParam = Win32.Windows.GetEndSessionLParamString((uint)msg.msg.LParam);
                    if (gbTrace)
                        pb.Trace.WriteLine("WinProc ParentWindow              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam {4}", msg.wm, msg.msg.WParam, msg.form.Text, msg.form.Handle, slParam);
                    if ((int)msg.msg.WParam == 1)
                    {
                        ApplicationExit();
                    }
                    break;
                case User.WM.WM_ACTIVATE:
                    short low = msg.msg.WParam.zGetLowValue();
                    User.WA wa = Messages.GetWA(low);
                    swParam = wa.ToString();

                    if (gbTrace)
                        pb.Trace.WriteLine("WinProc ParentWindow              : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, msg.form.Text, msg.form.Handle);

                    if ((wa == User.WA.WA_ACTIVE || wa == User.WA.WA_CLICKACTIVE) && msg.form.WindowState != FormWindowState.Minimized)
                    {
                        Form firstWindow = gOrderedWindows[0];
                        if (firstWindow != null)
                        {
                            firstWindow.Activate();
                            //Form.Activate();
                        }
                    }
                    break;
                case User.WM.WM_SYSCOMMAND:

                    User.SC sc = Messages.GetSC((int)msg.msg.WParam);
                    if (sc == User.SC.SC_CLOSE)
                    {
                        //if (gbTrace) 
                        pb.Trace.WriteLine("WinProc ParentWindow              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x} Close window", msg.wm, sc, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());
                        ApplicationExit();
                    }
                    else if (sc == User.SC.SC_MINIMIZE)
                    {
                        if (gbTrace)
                            pb.Trace.WriteLine("WinProc ParentWindow              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x} Minimize window", msg.wm, sc, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());
                    }
                    else if (sc == User.SC.SC_RESTORE)
                    {
                        gbDisableSetFirstWindow = false;
                        if (gbTrace)
                            pb.Trace.WriteLine("WinProc ParentWindow              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x}, Restore window", msg.wm, sc, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());
                    }
                    break;
            }

            return bCallBaseWndProc;
        }
        #endregion

        #region WndProc_Window
        private bool WndProc_Window(WinProcMessage msg)
        {
            string swParam = null;
            bool bCallBaseWndProc = true;  // si bCallBaseWndProc = true base.WndProc() est appelé

            //Form parentWindow;
            switch (msg.wm)
            {
                case User.WM.WM_ENDSESSION:
                    string slParam = Win32.Windows.GetEndSessionLParamString((uint)msg.msg.LParam);
                    if (gbTrace)
                        pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam {4}", msg.wm, msg.msg.WParam, msg.form.Text, msg.form.Handle, slParam);
                    break;
                case User.WM.WM_CLOSE:
                    if (gbTrace)
                        pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, msg.form.Text, msg.form.Handle);
                    RemoveWindow(msg.form);
                    ActivateFirstWindow();
                    break;
                case User.WM.WM_ACTIVATE:
                    short low = msg.msg.WParam.zGetLowValue();
                    User.WA wa = Messages.GetWA(low);
                    swParam = wa.ToString();
                    if (gbTrace)
                        pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, msg.form.Text, msg.form.Handle);
                    if (wa == User.WA.WA_CLICKACTIVE || wa == User.WA.WA_ACTIVE)
                        SetFirstWindow(msg.form);
                    break;
                case User.WM.WM_SYSCOMMAND:
                    User.SC sc = Messages.GetSC((int)msg.msg.WParam);
                    //parentWindow = msg.form.Owner;
                    //if (parentWindow == null) break;
                    if (sc == User.SC.SC_CLOSE)
                    {
                        if (gbTrace)
                            pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x} Close window", msg.wm, sc, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());
                        if (gbHideWindowWhenClose)
                        {
                            msg.form.Visible = false;
                            bCallBaseWndProc = false;
                        }
                    }
                    else if (sc == User.SC.SC_MINIMIZE)
                    {
                        if (gbTrace)
                            pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x} Minimize window", msg.wm, sc, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());

                        if (gbMinimizeParentWindowWhenMinimize)
                        {
                            gbDisableSetFirstWindow = true;
                            Point pt = Cursor.Position;
                            //User.SendMessage(parentWindow.Handle, (uint)User.WM.WM_SYSCOMMAND, (IntPtr)User.SC.SC_MINIMIZE, new IntPtr(cu.MakeValue(pt.X, pt.Y)));
                            User.SendMessage(gParentWindow.Form.Handle, (uint)User.WM.WM_SYSCOMMAND, (IntPtr)User.SC.SC_MINIMIZE, new IntPtr(zconvert.MakeValue(pt.X, pt.Y)));
                            bCallBaseWndProc = false;
                        }
                        if (gbHideWindowWhenMinimize)
                        {
                            msg.form.Visible = false;
                            //HideWindow(msg.form);
                            bCallBaseWndProc = false;
                        }
                    }
                    else if (sc == User.SC.SC_RESTORE)
                    {
                        if (gbTrace)
                            pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x} Restore window", msg.wm, sc, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());
                        //ShowWindow(msg.form);
                        //bCallBaseWndProc = false;
                    }
                    break;
                case User.WM.WM_SHOWWINDOW:
                    if (gbTrace)
                        pb.Trace.WriteLine("WinProc Child Window              : {0,-20} {1,-20} window {2,-25} hWnd {3} lParam = {4:x} Show or hide window", msg.wm, msg.msg.WParam, msg.form.Text, msg.form.Handle, msg.msg.LParam.ToInt32());
                    if ((int)msg.msg.WParam == 0) // 0 = false
                    {
                        // normalement pas besoin de tester gbHideWindowWhenMinimize
                        // si plus aucune fenetre n'est visible, la fenetre parent est cachée
                        // if (gbHideWindowWhenMinimize)
                        HideParentWindow(msg.form);
                    }
                    else // 1 = true
                    {
                        //gParentWindow.Form.Visible = true;
                        ShowParentWindow();
                    }
                    break;
            }

            return bCallBaseWndProc;
        }
        #endregion

        #region ShowParentWindow
        private void ShowParentWindow()
        {
            gParentWindow.Form.Visible = true;
            //gParentWindow.Form.ShowInTaskbar = true;
        }
        #endregion

        #region HideParentWindow
        private void HideParentWindow(Form form)
        {
            foreach (WindowData window in gWindows.Values)
            {
                if (window.Form != form && window.Form.Visible) return;
            }
            gParentWindow.Form.Visible = false;
            //gParentWindow.Form.ShowInTaskbar = false;
        }
        #endregion

        #region KeyboardHookProc
        // gestion CTRL-TAB, CTRL-F4, ALT-F4
        private void KeyboardHookProc(KeyboardHookMessage msg)
        {
            if (msg.nCode != User.Const.HC_ACTION) return;

            // gestion CTRL-TAB
            bool bActivateWindow = false;
            if (msg.KeyFlags.TransitionState == 0 && msg.vk == User.VK.VK_TAB && msg.KeyState.Ctrl && !msg.KeyState.Alt)
            {
                WindowsApp.CurrentWindowsApp.DisableSetFirstWindow = true;
                if (msg.KeyState.Shift)
                {
                    if (gbTrace)
                        pb.Trace.WriteLine("KeyboardHook                      : Active previous window");
                    WindowsApp.CurrentWindowsApp.ActivatePreviousWindow();
                }
                else
                {
                    if (gbTrace)
                        pb.Trace.WriteLine("KeyboardHook                      : Active next window");
                    WindowsApp.CurrentWindowsApp.ActivateNextWindow();
                }
                gbWindowActivatedByKeyOngoing = true;
                bActivateWindow = true;
                msg.RemoveMessage = true;
            }
            if (!bActivateWindow && gbWindowActivatedByKeyOngoing && msg.vk != User.VK.VK_TAB)
            {
                gbWindowActivatedByKeyOngoing = false;
                WindowsApp.CurrentWindowsApp.DisableSetFirstWindow = false;
                WindowsApp.CurrentWindowsApp.SetFirstWindow();
            }

            // gestion CTRL-F4
            if (msg.KeyFlags.TransitionState == 0 && msg.vk == User.VK.VK_F4)
            {
                if (msg.KeyState.Ctrl && !msg.KeyState.Alt && !msg.KeyState.Shift)
                {
                    WindowData window = FindWindow(msg.GUIThreadInfo.hwndActive);
                    string sWindow = "not found";
                    if (window != null) sWindow = window.Form.Text;
                    string sAction;
                    if (window != null && window.EnableCtrlF4Close) sAction = "close window"; else sAction = "don't close window";
                    if (gbTrace)
                        pb.Trace.WriteLine("KeyboardHook                      : form-{0,-25} CTRL-F4 hwnd active {1:x} focus {2:x} {3}", sWindow, (int)msg.GUIThreadInfo.hwndActive, (int)msg.GUIThreadInfo.hwndFocus, sAction);
                    if (window != null && window.EnableCtrlF4Close) window.Form.Close();
                    msg.RemoveMessage = true;
                }
            }

            // gestion ALT-F4
            if (msg.KeyState.Alt && !msg.KeyState.Ctrl && msg.vk == User.VK.VK_F4)
            {
                WindowData window = FindWindow(msg.GUIThreadInfo.hwndActive);
                string sWindow = "not found";
                if (window != null) sWindow = window.Form.Text;
                string sAction;
                if (window != null && window.DisableAltF4) sAction = "remove message ALT-F4"; else sAction = "don't remove message ALT-F4";
                if (window != null && window.CloseApplicationOnAltF4) sAction += " and close application"; else sAction = " and don't close application";
                if (gbTrace)
                    pb.Trace.WriteLine("KeyboardHook                      : form-{0,-25} ATL-F4 hwnd active {1:x} focus {2:x} {3}", sWindow, (int)msg.GUIThreadInfo.hwndActive, (int)msg.GUIThreadInfo.hwndFocus, sAction);
                if (window != null)
                {
                    if (window.DisableAltF4) msg.RemoveMessage = true;
                    if (window.CloseApplicationOnAltF4) ApplicationExit();
                }
            }
        }
        #endregion

        #region PreWindowHookProc
        // ParentWindow : WA_ACTIVE WA_CLICKACTIVE, SC_RESTORE
        //      WA_ACTIVE WA_CLICKACTIVE : Active la 1ère fenêtre fille si la fenêtre mère est activée
        //      SC_RESTORE               : Désactive le changement de la 1ère fenêtre (gbDisableSetFirstWindow)
        // Window       : WM_CLOSE, WA_ACTIVE WA_CLICKACTIVE, SC_MINIMIZE
        //      WM_CLOSE                 : Supprime la fenêtre fille et active la 1ère fenêtre fille
        //      WA_ACTIVE WA_CLICKACTIVE : Met la fenêtre fille en 1ère position
        //      SC_MINIMIZE              : Désactive SetFirstWindow et envoie SC_MINIMIZE à la fenêtre parent
        private void PreWindowHookProc(PreWindowHookMessage msg)
        {
            if (msg.nCode != User.Const.HC_ACTION) return;

            User.WA wa = User.WA.WA_UNKNOW;
            User.SC sc = User.SC.SC_UNKNOW;
            string swParam = null;
            if (msg.wm == User.WM.WM_ACTIVATE)
            {
                short low = msg.msg.wParam.zGetLowValue();
                wa = Messages.GetWA(low);
                swParam = wa.ToString();
            }
            else if (msg.wm == User.WM.WM_SYSCOMMAND)
            {
                sc = Messages.GetSC((int)msg.msg.wParam);
                if (sc != User.SC.SC_MINIMIZE && sc != User.SC.SC_RESTORE) return;
                swParam = sc.ToString();
            }
            else if (msg.wm != User.WM.WM_CLOSE)
                return;

            WindowData window = null;
            window = FindWindow(msg.msg.hwnd);
            string sWindow;
            if (window == null)
            {
                sWindow = "error window not found";
                if (gbTrace)
                    pb.Trace.WriteLine("PreWindowHookProc error           : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);
                return;
            }
            sWindow = window.Form.Text;


            if (window == gParentWindow)
            {
                if (gbTrace)
                    pb.Trace.WriteLine("PreWindowHookProc ParentWindow    : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);

                return;

                //switch (msg.wm)
                //{
                //    // Parent Window WA_ACTIVE WA_CLICKACTIVE : Active la 1ère fenêtre fille si la fenêtre mère est activée
                //    case User.WM.WM_ACTIVATE:
                //        if ((wa == User.WA.WA_ACTIVE || wa == User.WA.WA_CLICKACTIVE) && window.Form.WindowState != FormWindowState.Minimized)
                //        {
                //            Form firstWindow = gOrderedWindows[0];
                //            if (firstWindow != null) firstWindow.Activate();
                //        }
                //        break;
                //    // Parent Window SC_RESTORE : Désactive le changement de la 1ère fenêtre (gbDisableSetFirstWindow)
                //    case User.WM.WM_SYSCOMMAND:
                //        if (sc == User.SC.SC_RESTORE)
                //            gbDisableSetFirstWindow = false;
                //        break;
                //}
            }
            else
            {
                if (gbTrace)
                    pb.Trace.WriteLine("PreWindowHookProc Window          : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);

                return;

                //switch (msg.wm)
                //{
                //    // Child Window WM_CLOSE : Supprime la fenêtre fille et active la 1ère fenêtre fille
                //    case User.WM.WM_CLOSE:
                //        RemoveWindow(window.Form);
                //        ActivateFirstWindow();
                //        break;
                //    // Child Window WA_ACTIVE WA_CLICKACTIVE : Met la fenêtre fille en 1ère position
                //    case User.WM.WM_ACTIVATE:
                //        if (wa == User.WA.WA_CLICKACTIVE || wa == User.WA.WA_ACTIVE)
                //            SetFirstWindow(window.Form);
                //        break;
                //    // Child Window SC_MINIMIZE : Désactive SetFirstWindow et envoie SC_MINIMIZE à la fenêtre parent
                //    case User.WM.WM_SYSCOMMAND:
                //        Form parentWindow = window.Form.Owner;
                //        if (parentWindow == null)
                //        {
                //            if (gbTrace) pb.Trace.WriteLine("PreWindowHookProc Window error    : error this window dont have parent");
                //            break;
                //        }
                //        if (sc == User.SC.SC_MINIMIZE)
                //        {
                //            //gbDisableSetFirstWindow = true;
                //            //Point pt = Cursor.Position;
                //            //User.SendMessage(parentWindow.Handle, (uint)User.WM.WM_SYSCOMMAND, (IntPtr)User.SC.SC_MINIMIZE, new IntPtr(cu.MakeValue(pt.X, pt.Y)));
                //            //msg.RemoveMessage = true;
                //        }
                //        break;
                //}
            }
        }
        #endregion

        #region PostWindowHookProc
        private void PostWindowHookProc(PostWindowHookMessage msg)
        {
            if (msg.nCode != User.Const.HC_ACTION) return;

            if (msg.wm == User.WM.WM_ACTIVATE || msg.wm == User.WM.WM_SYSCOMMAND)
            {
                WindowData window = FindWindow(msg.msg.hwnd);
                string sWindow = "not found";
                if (window != null) sWindow = window.Form.Text;

                short low = msg.msg.wParam.zGetLowValue();

                string swParam = null;
                if (msg.wm == User.WM.WM_ACTIVATE)
                {
                    User.WA wa = Messages.GetWA(low);
                    swParam = wa.ToString();
                    if (wa == User.WA.WA_UNKNOW) swParam += " (" + low.ToString("x") + ")";
                }
                else if (msg.wm == User.WM.WM_SYSCOMMAND)
                {
                    User.SC sc = Messages.GetSC((int)msg.msg.wParam);
                    swParam = sc.ToString();
                    if (sc == User.SC.SC_UNKNOW) swParam += " (" + ((int)msg.msg.wParam).ToString("x") + ")";
                }
                if (gbTrace)
                    pb.Trace.WriteLine("PostWindowHookProc                : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);
            }
        }
        #endregion

        #region GetMessageWindowHookProc
        private void GetMessageWindowHookProc(GetMessageWindowHookMessage msg)
        {
            if (msg.nCode != User.Const.HC_ACTION) return;

            User.WA wa = User.WA.WA_UNKNOW;
            User.SC sc = User.SC.SC_UNKNOW;
            string swParam = null;
            if (msg.wm == User.WM.WM_ACTIVATE)
            {
                short low = msg.msg.wParam.zGetLowValue();
                wa = Messages.GetWA(low);
                swParam = wa.ToString();
            }
            else if (msg.wm == User.WM.WM_SYSCOMMAND)
            {
                sc = Messages.GetSC((int)msg.msg.wParam);
                if (sc != User.SC.SC_MINIMIZE && sc != User.SC.SC_RESTORE) return;
                swParam = sc.ToString();
            }
            else if (msg.wm != User.WM.WM_CLOSE)
                return;

            WindowData window = null;
            window = FindWindow(msg.msg.hwnd);
            string sWindow;
            if (window == null)
            {
                sWindow = "error window not found";
                if (gbTrace)
                    pb.Trace.WriteLine("GetMessage Hook error             : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);
                return;
            }
            sWindow = window.Form.Text;


            if (window == gParentWindow)
            {
                if (gbTrace)
                    pb.Trace.WriteLine("GetMessage Hook ParentWindow      : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);

                //switch (msg.msg)
                //{
                //    // Parent Window WA_ACTIVE WA_CLICKACTIVE : Active la 1ère fenêtre fille si la fenêtre mère est activée
                //    case User.WM.WM_ACTIVATE:
                //        if ((wa == User.WA.WA_ACTIVE || wa == User.WA.WA_CLICKACTIVE) && window.Form.WindowState != FormWindowState.Minimized)
                //        {
                //            Form firstWindow = gOrderedWindows[0];
                //            if (firstWindow != null) firstWindow.Activate();
                //        }
                //        break;
                //    // Parent Window SC_RESTORE : Désactive le changement de la 1ère fenêtre (gbDisableSetFirstWindow)
                //    case User.WM.WM_SYSCOMMAND:
                //        if (sc == User.SC.SC_RESTORE)
                //            gbDisableSetFirstWindow = false;
                //        break;
                //}
            }
            else
            {
                if (gbTrace)
                    pb.Trace.WriteLine("GetMessage Hook Window            : {0,-20} {1,-20} window {2,-25} hWnd {3}", msg.wm, swParam, sWindow, (int)msg.msg.hwnd);

                //switch (msg.msg)
                //{
                //    // Child Window WM_CLOSE : Supprime la fenêtre fille et active la 1ère fenêtre fille
                //    case User.WM.WM_CLOSE:
                //        RemoveWindow(window.Form);
                //        ActivateFirstWindow();
                //        break;
                //    // Child Window WA_ACTIVE WA_CLICKACTIVE : Met la fenêtre fille en 1ère position
                //    case User.WM.WM_ACTIVATE:
                //        if (wa == User.WA.WA_CLICKACTIVE || wa == User.WA.WA_ACTIVE)
                //            SetFirstWindow(window.Form);
                //        break;
                //    // Child Window SC_MINIMIZE : Désactive SetFirstWindow et envoie SC_MINIMIZE à la fenêtre parent
                //    case User.WM.WM_SYSCOMMAND:
                //        Form parentWindow = window.Form.Owner;
                //        if (parentWindow == null)
                //        {
                //            if (gbTrace) _tr.WriteLine("PreWindowHookProc Window error    : error this window dont have parent");
                //            break;
                //        }
                //        if (sc == User.SC.SC_MINIMIZE)
                //        {
                //            //gbDisableSetFirstWindow = true;
                //            //Point pt = Cursor.Position;
                //            //User.SendMessage(parentWindow.Handle, (uint)User.WM.WM_SYSCOMMAND, (IntPtr)User.SC.SC_MINIMIZE, new IntPtr(cu.MakeValue(pt.X, pt.Y)));
                //            //msg.RemoveMessage = true;
                //        }
                //        break;
                //}
            }
        }
        #endregion

        #region FindWindow
        private WindowData FindWindow(IntPtr hWnd)
        {
            while (hWnd != IntPtr.Zero)
            {
                if (gParentWindow != null && gParentWindow.Form.Handle == hWnd)
                    return gParentWindow;
                int i = gWindows.IndexOfKey(hWnd.ToInt32());
                if (i != -1)
                {
                    return gWindows.Values[i];
                }
                hWnd = User.GetParent(hWnd);
            }
            return null;
        }
        #endregion

        #region static RunControlApplicationExit
        public static void RunControlApplicationExit()
        {
            if (gControlApplicationExit) return;
            if (gbTrace)
                pb.Trace.WriteLine("Run control application exit");
            Process process = Process.GetCurrentProcess();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = process.MainModule.FileName;
            startInfo.Arguments = "-ControlApplicationExit:" + process.Id;
            //startInfo.ErrorDialog = true;
            //pb.Trace.WriteLine("  FileName : {0}", startInfo.FileName);
            //pb.Trace.WriteLine("  Verb : {0}", startInfo.Verb);
            //pb.Trace.WriteLine("  Verbs : {0}", startInfo.Verbs.zToStringValues());
            //pb.Trace.WriteLine("  Start new process ...");
            Process.Start(startInfo);
        }
        #endregion

        #region static ControlApplicationExit(string[] args)
        public static bool ControlApplicationExit(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("-ControlApplicationExit:", StringComparison.InvariantCultureIgnoreCase))
                {
                    int creationProcessId;
                    if (int.TryParse(arg.Substring(24, arg.Length - 24), out creationProcessId))
                    {
                        ControlApplicationExit(creationProcessId);
                    }
                    //cTrace.Dispose();
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region static ControlApplicationExit(int creationProcessId)
        public static void ControlApplicationExit(int creationProcessId)
        {
            gControlApplicationExit = true;
            if (gbTrace)
                pb.Trace.WriteLine("Control application exit {0}", creationProcessId);
            Process creationProcess = null;
            string creationProcessName = null;
            try
            {
                creationProcess = Process.GetProcessById(creationProcessId);
                creationProcessName = creationProcess.ProcessName;
            }
            catch
            {
                if (gbTrace)
                    pb.Trace.WriteLine("Control application exit : process already exit");
                return;
            }
            if (gbTrace)
                pb.Trace.WriteLine("Control application exit : wait process exit {0}-{1}", creationProcessName, creationProcessId);
            DateTime dt = DateTime.Now;
            while (true)
            {
                Thread.Sleep(10);
                if (creationProcess.HasExited)
                {
                    if (gbTrace)
                        pb.Trace.WriteLine("Control application exit : process exit {0}-{1}", creationProcessName, creationProcessId);
                    break;
                }
                if (DateTime.Now.Subtract(dt).TotalMilliseconds > 2000)
                {
                    string msg = string.Format("l'application {0}-{1} n'est pas encore arrêté, voulez-vous détruire l'application (yes) ou attendre (no) ?", creationProcessName, creationProcessId);
                    DialogResult r = MessageBox.Show(msg, "Quitter", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                    if (r == DialogResult.Yes)
                    {
                        if (!creationProcess.HasExited)
                        {
                            if (gbTrace)
                                pb.Trace.WriteLine("Control application exit : kill process {0}-{1}", creationProcessName, creationProcessId);
                            creationProcess.Kill();
                        }
                    }
                    dt = DateTime.Now;
                }
            }
        }
        #endregion
    }

    public class WinProcMessage
    {
        public Form form;
        public Message msg;
        public User.WM wm;
    }

    public delegate bool MessageProc(WinProcMessage msg);

    #region interface IWinProc
    public interface IWinProc : IDisposable
    {
        #region MessageEvent
        MessageProc MessageEvent
        {
            get;
            set;
        }
        #endregion

        #region Form
        Form Form
        {
            get;
        }
        #endregion
    }
    #endregion

    public class WinProc : NativeWindow, IDisposable, IWinProc
    {
        //private static ITrace _tr = pb.Trace.CurrentTrace;
        private static bool gbTrace = false;
        private static List<WinProc> gWinProcList = new List<WinProc>();

        private Form gForm = null;
        private bool gbHandleAssigned = false;
        private IntPtr gAssignedHandle = IntPtr.Zero;
        //public delegate bool MessageProc(WinProcMessage msg);
        public MessageProc gMessageEvent = null;

        #region constructor
        public WinProc(Form form)
        {
            //form.HandleCreated += new EventHandler(Form_HandleCreated);
            //form.HandleDestroyed += new EventHandler(Form_HandleDestroyed);
            gForm = form;
            //string sAssignHandle;
            //if (form.Created)
            //{
            //    AssignHandle(form.Handle); // NativeWindow.AssignHandle()
            //    sAssignHandle = "hWnd " + form.Handle.ToString();
            //}
            //else
            //{
            //    sAssignHandle = "hWnd " + form.Handle.ToString() + " hWnd not assigned";
            //}

            string sAssignHandle = "handle assigned";
            if (!gbHandleAssigned)
            {
                gbHandleAssigned = true;
                gAssignedHandle = form.Handle;
                AssignHandle(form.Handle); // NativeWindow.AssignHandle()
                sAssignHandle = "assign handle";
            }
            if (gbTrace)
                pb.Trace.WriteLine("WndProc                           : constructor                               window {0,-25} hWnd {1} form Created = {2} {3}", form.Text, form.Handle, form.Created, sAssignHandle);
            form.HandleCreated += new EventHandler(Form_HandleCreated);
            form.HandleDestroyed += new EventHandler(Form_HandleDestroyed);
            gWinProcList.Add(this);
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            gForm.HandleCreated -= new EventHandler(Form_HandleCreated);
            gForm.HandleDestroyed -= new EventHandler(Form_HandleDestroyed);
        }
        #endregion

        #region Trace
        public static bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region Form
        public Form Form
        {
            get { return gForm; }
        }
        #endregion

        #region MessageEvent
        public MessageProc MessageEvent
        {
            get { return gMessageEvent; }
            set { gMessageEvent = value; }
        }
        #endregion

        #region Form_HandleCreated
        /// <summary>
        /// Survient lorsque la fenêtre associée est créée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_HandleCreated(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            string sAssignHandle = "handle assigned";
            if (!gbHandleAssigned)
            {
                gbHandleAssigned = true;
                gAssignedHandle = form.Handle;
                AssignHandle(form.Handle); // NativeWindow.AssignHandle()
                sAssignHandle = "assign handle";
            }
            if (gbTrace)
                pb.Trace.WriteLine("WndProc                           : Form_HandleCreated event                  window {0,-25} hWnd {1} form Created = {2} {3}", form.Text, form.Handle, form.Created, sAssignHandle);
        }
        #endregion

        #region Form_HandleDestroyed
        /// <summary>
        /// Survient lorsque la fenêtre associée est détruite.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_HandleDestroyed(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            if (gbTrace)
                pb.Trace.WriteLine("WndProc                           : Form_HandleDestroyed event                window {0,-25} hWnd {1} form Created = {2} release handle", form.Text, form.Handle, form.Created);
            gbHandleAssigned = false;
            gAssignedHandle = IntPtr.Zero;
            ReleaseHandle(); // NativeWindow.ReleaseHandle()
        }
        #endregion

        #region WndProc
        /// <summary>
        /// Routine de récupération des messages système.
        /// </summary>
        /// <param name="msg">Le message reçu</param>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message msg)
        {
            #region liste des commandes WM_SYSCOMMAND SC_...
            //public const int SC_ARRANGE = 0xF110;
            //public const int SC_CLOSE = 0xF060;
            //public const int SC_HOTKEY = 0xF150;
            //public const int SC_HSCROLL = 0xF080;
            //public const int SC_ICON = SC_MINIMIZE;
            //public const int SC_KEYMENU = 0xF100;
            //public const int SC_MAXIMIZE = 0xF030;
            //public const int SC_MINIMIZE = 0xF020;
            //public const int SC_MOUSEMENU = 0xF090;
            //public const int SC_MOVE = 0xF010;
            //public const int SC_NEXTWINDOW = 0xF040;
            //public const int SC_PREVWINDOW = 0xF050;
            //public const int SC_RESTORE = 0xF120;
            //public const int SC_SCREENSAVE = 0xF140;
            //public const int SC_SIZE = 0xF000;
            //public const int SC_TASKLIST = 0xF130;
            //public const int SC_VSCROLL = 0xF070;
            //public const int SC_ZOOM = SC_MAXIMIZE;
            #endregion

            #region liste des commandes WM_...
            //public const int WM_ACTIVATE = 0x6;
            //public const int WM_ACTIVATEAPP = 0x1C;
            //public const int WM_ASKCBFORMATNAME = 0x30C;
            //public const int WM_CANCELJOURNAL = 0x4B;
            //public const int WM_CANCELMODE = 0x1F;
            //public const int WM_CHANGECBCHAIN = 0x30D;
            //public const int WM_CHAR = 0x102;
            //public const int WM_CHARTOITEM = 0x2F;
            //public const int WM_CHILDACTIVATE = 0x22;
            //public const int WM_CLEAR = 0x303;
            //public const int WM_CLOSE = 0x10;
            //public const int WM_COMMAND = 0x111;
            //public const int WM_COMMNOTIFY = 0x44;
            //public const int WM_COMPACTING = 0x41;
            //public const int WM_COMPAREITEM = 0x39;
            //public const int WM_COPY = 0x301;
            //public const int WM_COPYDATA = 0x4A;
            //public const int WM_CREATE = 0x1;
            //public const int WM_CTLCOLORBTN = 0x135;
            //public const int WM_CTLCOLORDLG = 0x136;
            //public const int WM_CTLCOLOREDIT = 0x133;
            //public const int WM_CTLCOLORLISTBOX = 0x134;
            //public const int WM_CTLCOLORMSGBOX = 0x132;
            //public const int WM_CTLCOLORSCROLLBAR = 0x137;
            //public const int WM_CTLCOLORSTATIC = 0x138;
            //public const int WM_CUT = 0x300;
            //public const int WM_DDE_ACK = (WM_DDE_FIRST + 4);
            //public const int WM_DDE_ADVISE = (WM_DDE_FIRST + 2);
            //public const int WM_DDE_DATA = (WM_DDE_FIRST + 5);
            //public const int WM_DDE_EXECUTE = (WM_DDE_FIRST + 8);
            //public const int WM_DDE_FIRST = 0x3E0;
            //public const int WM_DDE_INITIATE = (WM_DDE_FIRST);
            //public const int WM_DDE_LAST = (WM_DDE_FIRST + 8);
            //public const int WM_DDE_POKE = (WM_DDE_FIRST + 7);
            //public const int WM_DDE_REQUEST = (WM_DDE_FIRST + 6);
            //public const int WM_DDE_TERMINATE = (WM_DDE_FIRST + 1);
            //public const int WM_DDE_UNADVISE = (WM_DDE_FIRST + 3);
            //public const int WM_DEADCHAR = 0x103;
            //public const int WM_DELETEITEM = 0x2D;
            //public const int WM_DESTROY = 0x2;
            //public const int WM_DESTROYCLIPBOARD = 0x307;
            //public const int WM_DEVMODECHANGE = 0x1B;
            //public const int WM_DRAWCLIPBOARD = 0x308;
            //public const int WM_DRAWITEM = 0x2B;
            //public const int WM_DROPFILES = 0x233;
            //public const int WM_ENABLE = 0xA;
            //public const int WM_ENDSESSION = 0x16;
            //public const int WM_ENTERIDLE = 0x121;
            //public const int WM_ENTERMENULOOP = 0x211;
            //public const int WM_ERASEBKGND = 0x14;
            //public const int WM_EXITMENULOOP = 0x212;
            //public const int WM_FONTCHANGE = 0x1D;
            //public const int WM_GETDLGCODE = 0x87;
            //public const int WM_GETFONT = 0x31;
            //public const int WM_GETHOTKEY = 0x33;
            //public const int WM_GETMINMAXINFO = 0x24;
            //public const int WM_GETTEXT = 0xD;
            //public const int WM_GETTEXTLENGTH = 0xE;
            //public const int WM_HOTKEY = 0x312;
            //public const int WM_HSCROLL = 0x114;
            //public const int WM_HSCROLLCLIPBOARD = 0x30E;
            //public const int WM_ICONERASEBKGND = 0x27;
            //public const int WM_INITDIALOG = 0x110;
            //public const int WM_INITMENU = 0x116;
            //public const int WM_INITMENUPOPUP = 0x117;
            //public const int WM_KEYDOWN = 0x100;
            //public const int WM_KEYFIRST = 0x100;
            //public const int WM_KEYLAST = 0x108;
            //public const int WM_KEYUP = 0x101;
            //public const int WM_KILLFOCUS = 0x8;
            //public const int WM_LBUTTONDBLCLK = 0x203;
            //public const int WM_LBUTTONDOWN = 0x201;
            //public const int WM_LBUTTONUP = 0x202;
            //public const int WM_MBUTTONDBLCLK = 0x209;
            //public const int WM_MBUTTONDOWN = 0x207;
            //public const int WM_MBUTTONUP = 0x208;
            //public const int WM_MDIACTIVATE = 0x222;
            //public const int WM_MDICASCADE = 0x227;
            //public const int WM_MDICREATE = 0x220;
            //public const int WM_MDIDESTROY = 0x221;
            //public const int WM_MDIGETACTIVE = 0x229;
            //public const int WM_MDIICONARRANGE = 0x228;
            //public const int WM_MDIMAXIMIZE = 0x225;
            //public const int WM_MDINEXT = 0x224;
            //public const int WM_MDIREFRESHMENU = 0x234;
            //public const int WM_MDIRESTORE = 0x223;
            //public const int WM_MDISETMENU = 0x230;
            //public const int WM_MDITILE = 0x226;
            //public const int WM_MEASUREITEM = 0x2C;
            //public const int WM_MENUCHAR = 0x120;
            //public const int WM_MENUSELECT = 0x11F;
            //public const int WM_MOUSEACTIVATE = 0x21;
            //public const int WM_MOUSEFIRST = 0x200;
            //public const int WM_MOUSELAST = 0x209;
            //public const int WM_MOUSEMOVE = 0x200;
            //public const int WM_MOVE = 0x3;
            //public const int WM_NCACTIVATE = 0x86;
            //public const int WM_NCCALCSIZE = 0x83;
            //public const int WM_NCCREATE = 0x81;
            //public const int WM_NCDESTROY = 0x82;
            //public const int WM_NCHITTEST = 0x84;
            //public const int WM_NCLBUTTONDBLCLK = 0xA3;
            //public const int WM_NCLBUTTONDOWN = 0xA1;
            //public const int WM_NCLBUTTONUP = 0xA2;
            //public const int WM_NCMBUTTONDBLCLK = 0xA9;
            //public const int WM_NCMBUTTONDOWN = 0xA7;
            //public const int WM_NCMBUTTONUP = 0xA8;
            //public const int WM_NCMOUSEMOVE = 0xA0;
            //public const int WM_NCPAINT = 0x85;
            //public const int WM_NCRBUTTONDBLCLK = 0xA6;
            //public const int WM_NCRBUTTONDOWN = 0xA4;
            //public const int WM_NCRBUTTONUP = 0xA5;
            //public const int WM_NEXTDLGCTL = 0x28;
            //public const int WM_NULL = 0x0;
            //public const int WM_OTHERWINDOWCREATED = 0x42;
            //public const int WM_OTHERWINDOWDESTROYED = 0x43;
            //public const int WM_PAINT = 0xF;
            //public const int WM_PAINTCLIPBOARD = 0x309;
            //public const int WM_PAINTICON = 0x26;
            //public const int WM_PALETTECHANGED = 0x311;
            //public const int WM_PALETTEISCHANGING = 0x310;
            //public const int WM_PARENTNOTIFY = 0x210;
            //public const int WM_PASTE = 0x302;
            //public const int WM_PENWINFIRST = 0x380;
            //public const int WM_PENWINLAST = 0x38F;
            //public const int WM_POWER = 0x48;
            //public const int WM_QUERYDRAGICON = 0x37;
            //public const int WM_QUERYENDSESSION = 0x11;
            //public const int WM_QUERYNEWPALETTE = 0x30F;
            //public const int WM_QUERYOPEN = 0x13;
            //public const int WM_QUEUESYNC = 0x23;
            //public const int WM_QUIT = 0x12;
            //public const int WM_RBUTTONDBLCLK = 0x206;
            //public const int WM_RBUTTONDOWN = 0x204;
            //public const int WM_RBUTTONUP = 0x205;
            //public const int WM_RENDERALLFORMATS = 0x306;
            //public const int WM_RENDERFORMAT = 0x305;
            //public const int WM_SETCURSOR = 0x20;
            //public const int WM_SETFOCUS = 0x7;
            //public const int WM_SETFONT = 0x30;
            //public const int WM_SETHOTKEY = 0x32;
            //public const int WM_SETREDRAW = 0xB;
            //public const int WM_SETTEXT = 0xC;
            //public const int WM_SHOWWINDOW = 0x18;
            //public const int WM_SIZE = 0x5;
            //public const int WM_SIZECLIPBOARD = 0x30B;
            //public const int WM_SPOOLERSTATUS = 0x2A;
            //public const int WM_SYSCHAR = 0x106;
            //public const int WM_SYSCOLORCHANGE = 0x15;
            //public const int WM_SYSCOMMAND = 0x112;
            //public const int WM_SYSDEADCHAR = 0x107;
            //public const int WM_SYSKEYDOWN = 0x104;
            //public const int WM_SYSKEYUP = 0x105;
            //public const int WM_TIMECHANGE = 0x1E;
            //public const int WM_TIMER = 0x113;
            //public const int WM_UNDO = 0x304;
            //public const int WM_USER = 0x400;
            //public const int WM_VKEYTOITEM = 0x2E;
            //public const int WM_VSCROLL = 0x115;
            //public const int WM_VSCROLLCLIPBOARD = 0x30A;
            //public const int WM_WINDOWPOSCHANGED = 0x47;
            //public const int WM_WINDOWPOSCHANGING = 0x46;
            //public const int WM_WININICHANGE = 0x1A;
            #endregion

            //if (msg.Msg == (int)User.WM.WM_KEYDOWN || msg.Msg == (int)User.WM.WM_KEYUP)
            //{
            //    if (gbTrace) _tr.WriteLine("WndProc                 : {0,-20} ({1:x,4}) window {2}, ", wm, msg.Msg, gForm.Text);
            //}

            bool bCallBaseWndProc = true;  // si bCallBaseWndProc = true base.WndProc() est appelé
            if (gMessageEvent != null)
            {
                User.WM wm = Messages.GetWindowMessage(msg.Msg);
                bCallBaseWndProc = gMessageEvent(new WinProcMessage() { form = gForm, msg = msg, wm = wm });
            }
            if (bCallBaseWndProc) base.WndProc(ref msg);
        }
        #endregion

        #region TraceWinProcList
        public static void TraceWinProcList()
        {
            foreach (WinProc wp in gWinProcList)
            {
                Form form = wp.Form;
                pb.Trace.WriteLine("WndProc                           : WndProcList                               window {0,-25} hWnd {1} - {2} - {3} form Created = {4}", form.Text, form.Handle, wp.gAssignedHandle, wp.Handle, form.Created);
            }
        }
        #endregion
    }

    #region class FormWinProc
    public class FormWinProc : Form, IWinProc
    {
        #region variable
        private static bool gbTrace = true;

        //public delegate bool MessageProc(WinProcMessage msg);
        public MessageProc gMessageEvent = null;
        #endregion

        #region Trace
        public static bool Trace
        {
            get { return gbTrace; }
            set { gbTrace = value; }
        }
        #endregion

        #region Form
        public Form Form
        {
            get { return this; }
        }
        #endregion

        #region MessageEvent
        public MessageProc MessageEvent
        {
            get { return gMessageEvent; }
            set { gMessageEvent = value; }
        }
        #endregion

        #region WndProc
        protected override void WndProc(ref Message msg)
        {
            bool bCallBaseWndProc = true;  // si bCallBaseWndProc = true base.WndProc() est appelé
            if (gMessageEvent != null)
            {
                User.WM wm = Messages.GetWindowMessage(msg.Msg);
                if (gMessageEvent != null) bCallBaseWndProc = gMessageEvent(new WinProcMessage() { form = this, msg = msg, wm = wm });
            }
            if (bCallBaseWndProc) base.WndProc(ref msg);
        }
        #endregion
    }
    #endregion

    #region class MessageFilterData
    public class MessageFilterData
    {
        public Message msg;
        public User.WM wm;
        public bool DontDistributeMessage = false;
    }
    #endregion

    #region class MessageFilter
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class MessageFilter : IMessageFilter
    {
        public delegate void PreFilterMessageProc(MessageFilterData msg);
        public PreFilterMessageProc PreFilterMessageEvent = null;

        #region PreFilterMessage
        public bool PreFilterMessage(ref Message msg)
        {
            bool bDontDistributeMessage = false;
            if (PreFilterMessageEvent != null)
            {
                User.WM wm = Messages.GetWindowMessage(msg.Msg);
                MessageFilterData msg2 = new MessageFilterData() { msg = msg, wm = wm };
                PreFilterMessageEvent(msg2);
                msg.Result = msg2.msg.Result;
                bDontDistributeMessage = msg2.DontDistributeMessage;
            }
            return bDontDistributeMessage;
        }
        #endregion
    }
    #endregion

    #region example NativeWindow from msdn ...
    // Summary description for Form1.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class Form1 : System.Windows.Forms.Form
    {
        private MyNativeWindowListener nwl;
        private MyNativeWindow nw;

        internal void ApplicationActivated(bool ApplicationActivated)
        {
            // The application has been activated or deactivated
            System.Diagnostics.Debug.WriteLine("Application Active = " + ApplicationActivated.ToString());
        }

        private Form1()
        {
            this.Size = new System.Drawing.Size(300, 300);
            this.Text = "Form1";

            nwl = new MyNativeWindowListener(this);
            nw = new MyNativeWindow(this);

        }

        // duplicate Main() with Pib.main.Main()
        //[STAThread]
        //static void Main()
        //{
        //    Application.Run(new Form1());
        //}
    }

    // NativeWindow class to listen to operating system messages.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    internal class MyNativeWindowListener : NativeWindow
    {

        // Constant value was found in the "windows.h" header file.
        private const int WM_ACTIVATEAPP = 0x001C;

        private Form1 parent;

        public MyNativeWindowListener(Form1 parent)
        {

            parent.HandleCreated += new EventHandler(this.OnHandleCreated);
            parent.HandleDestroyed += new EventHandler(this.OnHandleDestroyed);
            this.parent = parent;
        }

        // Listen for the control's window creation and then hook into it.
        internal void OnHandleCreated(object sender, EventArgs e)
        {
            // Window is now created, assign handle to NativeWindow.
            AssignHandle(((Form1)sender).Handle);
        }
        internal void OnHandleDestroyed(object sender, EventArgs e)
        {
            // Window was destroyed, release hook.
            ReleaseHandle();
        }
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages

            switch (m.Msg)
            {
                case WM_ACTIVATEAPP:

                    // Notify the form that this message was received.
                    // Application is activated or deactivated, 
                    // based upon the WParam parameter.
                    parent.ApplicationActivated(((int)m.WParam != 0));

                    break;
            }
            base.WndProc(ref m);
        }
    }

    // MyNativeWindow class to create a window given a class name.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    internal class MyNativeWindow : NativeWindow
    {

        // Constant values were found in the "windows.h" header file.
        private const int WS_CHILD = 0x40000000,
                          WS_VISIBLE = 0x10000000,
                          WM_ACTIVATEAPP = 0x001C;

        private int windowHandle;

        public MyNativeWindow(Form parent)
        {

            CreateParams cp = new CreateParams();

            // Fill in the CreateParams details.
            cp.Caption = "Click here";
            cp.ClassName = "Button";

            // Set the position on the form
            cp.X = 100;
            cp.Y = 100;
            cp.Height = 100;
            cp.Width = 100;

            // Specify the form as the parent.
            cp.Parent = parent.Handle;

            // Create as a child of the specified parent
            cp.Style = WS_CHILD | WS_VISIBLE;

            // Create the actual window
            this.CreateHandle(cp);
        }

        // Listen to when the handle changes to keep the variable in sync
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnHandleChange()
        {
            windowHandle = (int)this.Handle;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for messages that are sent to the button window. Some messages are sent
            // to the parent window instead of the button's window.

            switch (m.Msg)
            {
                case WM_ACTIVATEAPP:
                    // Do something here in response to messages
                    break;
            }
            base.WndProc(ref m);
        }
    }
    #endregion
}

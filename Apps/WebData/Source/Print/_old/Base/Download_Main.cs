using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Download
{
    #region class main
    class main
    {
        #region variable
        private static MenuItemForm gmMain = null;
        private static XmlParameter gAppParameters = null;
        private static IDbConnection gCon = null;
        private static fBdd1 gDataForm = null;
        #endregion

        #region Main
        [STAThread] // STAThread est nécessaire pour accéder au clipboard (Clipboard.GetText)
        public static void Main(string[] args)
        {
            try
            {
                XmlConfig.CurrentConfig = new XmlConfig();
                cTrace.NewOutputPath(XmlConfig.CurrentConfig.Get("Log"));
                //cTrace.KeepOutputOpen = XmlConfig.CurrentConfig.Get<bool>("LogKeepOutputOpen", false);

                FormatInfo.SetInvariantCulture();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                WindowsApp.Trace = XmlConfig.CurrentConfig.Get<bool>("WinProcTrace", false);

                if (!WindowsApp.ControlApplicationExit(args))
                    return;

                //cTrace.Trace("Download");

                Icon icon = WRunSource.Class.Print.Base.Download_resource.AppIcon;
                WindowsApp.CurrentWindowsApp = new WindowsApp(icon, WAOptions.Type2);
                WindowsApp.CurrentWindowsApp.ApplicationExiting += new WindowsApp.EventApplicationExiting(ApplicationExit);
                WindowsApp.CurrentWindowsApp.TrayIcon.NotifyIcon.Text = "Download";

                Init();

                Application.Run();
            }
            catch (Exception ex)
            {
                cf.ErrorMessageBox(ex);
            }
        }
        #endregion

        #region Init()
        private static void Init()
        {
            //XmlConfig.CurrentConfig.Get("DBConnection");

            // file c:\Users\Pierre\AppData\Local\[AssemblyCompany]\[name]\
            // file example : c:\Users\Pierre\AppData\Local\Pierre Beuzart\Download\
            gAppParameters = new XmlParameter(Application.ProductName); // Application.ProductName = "Download"

            WindowsApp.CurrentWindowsApp.TrayIcon.NotifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_MouseDoubleClick);

            gmMain = new MenuItemForm("Download &main", true, new MenuItemForm.EventCreateForm(CreateMain), WindowsApp.CurrentWindowsApp.ParentForm);
            MenuItemForm mBase = new MenuItemForm("&Base", true, new MenuItemForm.EventCreateForm(CreateBase), WindowsApp.CurrentWindowsApp.ParentForm);
            ToolStripMenuItem mTraceForm = cf.CreateMenuItem("&Trace form", false, new EventHandler(m_trace_form_Click));
            ToolStripMenuItem mQuit = cf.CreateMenuItem("&Quit", false, new EventHandler(m_quit_Click));
            WindowsApp.CurrentWindowsApp.TrayIcon.AddMenuItems(gmMain.MenuItem, new ToolStripSeparator(), mBase.MenuItem, new ToolStripSeparator(), mTraceForm, new ToolStripSeparator(), mQuit);
        }
        #endregion

        #region ApplicationExit
        private static void ApplicationExit()
        {
            //cTrace.Trace("Exit in progress");
            gAppParameters.Save();
            if (gCon != null) gCon.Close();
            WindowsApp.RunControlApplicationExit();
        }
        #endregion

        #region m_trace_form_Click
        private static void m_trace_form_Click(object sender, EventArgs e)
        {
            Form form = WindowsApp.CurrentWindowsApp.ParentWindow.Form;
            // parentWindow.WindowState, parentWindow.Size
            cTrace.Trace("List of forms :");
            cTrace.Trace("  Parent window : Handle {0,-10} Text {1,-20} Visible {2,-5} ShowIcon {3,-5} ShowInTaskbar {4,-5}", form.Handle, form.Text, form.Visible, form.ShowIcon, form.ShowInTaskbar);
            foreach (WindowData window in WindowsApp.CurrentWindowsApp.Windows.Values)
            {
                form = window.Form;
                cTrace.Trace("  Child window  : Handle {0,-10} Text {1,-20} Visible {2,-5} ShowIcon {3,-5} ShowInTaskbar {4,-5}", form.Handle, form.Text, form.Visible, form.ShowIcon, form.ShowInTaskbar);
            }
        }
        #endregion

        #region m_quit_Click
        private static void m_quit_Click(object sender, EventArgs e)
        {
            WindowsApp.CurrentWindowsApp.ApplicationExit();
        }
        #endregion

        #region CreateMain
        private static Form CreateMain()
        {
            Form form = new MainForm(WindowsApp.CurrentWindowsApp.Icon);
            // il faut faire l'appel à AddWindow() ici et non pas dans le constructeur de la Form, sinon il y a un icone qui apparait dans la liste des taches (alt-tab)
            WindowsApp.CurrentWindowsApp.AddWindow(form);
            //form.ShowIcon = false;
            form.ShowInTaskbar = false;
            new PersistentFormParameters(gAppParameters, form, "MainForm");
            return form;
        }
        #endregion

        #region notifyIcon_MouseDoubleClick
        private static void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            gmMain.ShowHideForm();
        }
        #endregion

        #region //CreateBase
        //private static Form CreateBase()
        //{
        //    Form form = new MainForm(WindowsApp.CurrentWindowsApp.Icon);
        //    // il faut faire l'appel à AddWindow() ici et non pas dans le constructeur de la Form, sinon il y a un icone qui apparait dans la liste des taches (alt-tab)
        //    WindowsApp.CurrentWindowsApp.AddWindow(form);
        //    form.ShowInTaskbar = false;
        //    new PersistentFormParameters(gAppParameters, form, "BaseForm");
        //    form.Text = "BaseForm";
        //    return form;
        //}
        #endregion

        #region CreateBase
        private static Form CreateBase()
        {
            XDocument xml = XDocument.Load(cu.GetPathFichier(XmlConfig.CurrentConfig.Get("InputBddXml")));
            //gCon = Ado.OpenCon(XmlConfig.CurrentConfig.Get("DBConnection"), true);
            gCon = Ado.CreateCon(XmlConfig.CurrentConfig.Get("DBConnection"), true);
            gDataForm = new fBdd1(gCon, xml.zXPathElement("data_enter/data_enter_tree"), xml.zXPathElement("data_enter/data_enter_def"), null, "Base", "Base data", true);
            WindowsApp.CurrentWindowsApp.AddWindow(gDataForm);
            gDataForm.ShowInTaskbar = false;
            new PersistentFormParameters(gAppParameters, gDataForm, "BaseForm");
            //form.Text = "BaseForm";
            CreateMenuDataForm(gDataForm);
            return gDataForm;
        }
        #endregion

        #region CreateMenuDataForm
        private static void CreateMenuDataForm(Form form)
        {
            MenuStrip menu = new MenuStrip();
            ToolStripMenuItem m_file = new ToolStripMenuItem();
            ToolStripMenuItem m_quit = new ToolStripMenuItem();
            menu.SuspendLayout();
            menu.Items.AddRange(new ToolStripItem[] { m_file });
            m_file.DropDownItems.AddRange(new ToolStripItem[] { m_quit } );
            m_file.Text = "&File";
            m_quit.Text = "&Update data definition";
            m_quit.Click += new System.EventHandler(m_update_data_def_Click);
            //form.MainMenuStrip = menu;
            form.Controls.Add(menu);
            menu.ResumeLayout();
        }
        #endregion

        #region m_update_data_def_Click
        private static void m_update_data_def_Click(object sender, EventArgs e)
        {
            cf.MenuExe(new cf.fExe(UpdateDataDef));
        }
        #endregion

        #region UpdateDataDef
        private static void UpdateDataDef()
        {
            XDocument xml = XDocument.Load(cu.GetPathFichier(XmlConfig.CurrentConfig.Get("InputBddXml")));
            gDataForm.UpdateDataDefinition(xml.zXPathElement("data_enter/data_enter_tree"), xml.zXPathElement("data_enter/data_enter_def"), null);
        }
        #endregion

    }
    #endregion
}

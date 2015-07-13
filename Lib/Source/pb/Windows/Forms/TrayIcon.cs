using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    #region class TrayIcon
    public class TrayIcon : IDisposable
    {
        #region variable
        private IContainer gComponents = null;
        private NotifyIcon gNotifyIcon = null;
        private ContextMenuStrip gNotifyMenu = null;
        #endregion

        #region constructor
        public TrayIcon(Icon icon)
        {
            gComponents = new Container();
            //gNotifyIcon = WindowsApp.CurrentWindowsApp.NotifyIcon;
            gNotifyIcon = new NotifyIcon();
            gNotifyIcon.Icon = icon;
            gNotifyIcon.Visible = true;
            gNotifyIcon.MouseClick += new MouseEventHandler(NotifyIcon_MouseClick);
            gNotifyMenu = new ContextMenuStrip(gComponents);
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (gNotifyIcon != null) gNotifyIcon.Dispose();
            if (gComponents != null) gComponents.Dispose();  // InvalidOperationException : Cross-thread operation not valid: Control '' accessed from a thread other than the thread it was created on
        }
        #endregion

        #region property ...
        #region Components
        public IContainer Components
        {
            get { return gComponents; }
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
            set { gNotifyMenu = value; }
        }
        #endregion
        #endregion

        #region NotifyIcon_MouseClick
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && gNotifyIcon != null && gNotifyMenu != null)
            {
                gNotifyIcon.ContextMenuStrip = gNotifyMenu;
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(gNotifyIcon, null);
                gNotifyIcon.ContextMenuStrip = null;
            }
        }
        #endregion

        #region //CreateMenu(params ToolStripItem[] menuItems)
        //public ContextMenuStrip CreateMenuStrip(params ToolStripItem[] menuItems)
        //public ContextMenuStrip CreateMenu(params ToolStripItem[] menuItems)
        //{
        //    ContextMenuStrip menu = new ContextMenuStrip(gComponents);
        //    menu.SuspendLayout();
        //    //menu.Items.Insert();
        //    menu.Items.AddRange(menuItems);
        //    menu.ResumeLayout(false);
        //    return menu;
        //}
        #endregion

        public void AddMenuItems(params ToolStripItem[] menuItems)
        {
            gNotifyMenu.SuspendLayout();
            gNotifyMenu.Items.AddRange(menuItems);
            gNotifyMenu.ResumeLayout(false);
        }

        #region InsertMenuItems
        public void InsertMenuItems(int index, params ToolStripItem[] menuItems)
        {
            gNotifyMenu.SuspendLayout();
            foreach (ToolStripItem menuItem in menuItems.Reverse())
                gNotifyMenu.Items.Insert(index, menuItem);
            gNotifyMenu.ResumeLayout(false);
        }
        #endregion

        #region //notifyIcon_MouseClick
        //private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        gNotifyIcon.ContextMenuStrip = gNotifyMenu;
        //        MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
        //        mi.Invoke(gNotifyIcon, null);
        //        gNotifyIcon.ContextMenuStrip = null;
        //    }
        //}
        #endregion

        #region static CreateTrayIcon
        public static TrayIcon CreateTrayIcon(Icon icon)
        {
            return new TrayIcon(icon);
        }
        #endregion
    }
    #endregion
}


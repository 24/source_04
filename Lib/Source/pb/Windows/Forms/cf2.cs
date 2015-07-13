using System;
using System.Drawing;
using System.Windows.Forms;
using pb.Data.Xml;

namespace pb.Windows.Forms
{
    public class cMenu
    {
        public delegate void fExe();

        public static void Exe(fExe f)
        {
            try
            {
                f();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
            }
        }

        public static void Exe(Form form, fExe f)
        {
            try
            {
                form.Cursor = Cursors.WaitCursor;
                f();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
            }
            finally
            {
                form.Cursor = Cursors.Default;
            }
        }
    }

    public class MenuItemForm
    {
        #region variable
        private Form gForm = null;
        private Form gParentForm = null;
        private ToolStripMenuItem gMenuItem = null;
        public delegate Form EventCreateForm();
        private EventCreateForm gCreateForm = null;
        #endregion

        #region constructor
        public MenuItemForm(string text, bool checkOnClick, Form form, Form parentForm)
        {
            CreateMenuItem(text, checkOnClick);
            gForm = form;
            gParentForm = parentForm;
            gForm.VisibleChanged += new EventHandler(Form_VisibleChanged);
        }

        public MenuItemForm(string text, bool checkOnClick, EventCreateForm createForm, Form parentForm)
        {
            CreateMenuItem(text, checkOnClick);
            gCreateForm = createForm;
            gParentForm = parentForm;
        }
        #endregion

        #region MenuItem
        public ToolStripMenuItem MenuItem
        {
            get { return gMenuItem; }
        }
        #endregion

        #region CreateMenuItem
        private void CreateMenuItem(string text, bool checkOnClick)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem();
            menu.Text = text;
            menu.CheckOnClick = checkOnClick;
            menu.Click += new EventHandler(menu_Click); ;
            gMenuItem = menu;
        }
        #endregion

        #region CreateForm
        private bool CreateForm()
        {
            if (gForm != null) return true;
            if (gCreateForm == null)
                throw new Exception("error no creation form delegate");
            try
            {
                gForm = gCreateForm();
            }
            catch (Exception ex)
            {
                zerrf.ErrorMessageBox(ex);
                return false;
            }
            gForm.VisibleChanged += new EventHandler(Form_VisibleChanged);
            return true;
        }
        #endregion

        #region ShowHideForm
        public void ShowHideForm()
        {
            if (!CreateForm()) return;
            if (gForm.Visible)
                gForm.Hide();
            else
                ShowForm();
        }
        #endregion

        #region ShowForm
        private void ShowForm()
        {
            gForm.Show(gParentForm);
            gMenuItem.Checked = true;
        }
        #endregion

        #region menu_Click
        private void menu_Click(object sender, EventArgs e)
        {
            ShowHideForm();
        }
        #endregion

        #region Form_VisibleChanged
        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!gForm.Visible)
                gMenuItem.Checked = false;
        }
        #endregion
    }

    public class PersistentFormParameters
    {
        private Form gForm = null;
        private XmlParameters_v1 gAppParameters = null;
        private string gParameterName = null;
        private Point gFormLocation = new Point(-1, -1);

        #region constructor
        public PersistentFormParameters(XmlParameters_v1 appParameters, Form form, string parameterName)
        {
            gAppParameters = appParameters;
            appParameters.BeforeSave += new XmlParameters_v1.BeforeSaveEvent(Parameters_BeforeSaveEvent);
            gForm = form;
            gParameterName = parameterName;
            LoadParameters();
            form.VisibleChanged += new EventHandler(Form_VisibleChanged);
            form.FormClosing += new FormClosingEventHandler(Form_FormClosing);
        }
        #endregion

        #region LoadParameters
        private void LoadParameters()
        {
            object oX = gAppParameters.Get(gParameterName + "_X");
            object oY = gAppParameters.Get(gParameterName + "_Y");
            if (oX != null && oY != null) { gFormLocation.X = (int)oX; gFormLocation.Y = (int)oY; }

            if (gFormLocation.X > 0 && gFormLocation.Y > 0) gForm.Location = gFormLocation;
        }
        #endregion

        #region SaveParameters
        private void SaveParameters()
        {
            Point location;
            if (gForm.Visible) location = gForm.Location; else location = gFormLocation;
            if (location.X > 0 && location.Y > 0)
            {
                gAppParameters.Set(gParameterName + "_X", location.X);
                gAppParameters.Set(gParameterName + "_Y", location.Y);
            }
        }
        #endregion

        #region Parameters_BeforeSaveEvent
        private void Parameters_BeforeSaveEvent()
        {
            SaveParameters();
        }
        #endregion

        #region Form_VisibleChanged
        private void Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!gForm.Visible)
                gFormLocation = gForm.Location;
        }
        #endregion

        #region Form_FormClosing
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveParameters();
        }
        #endregion
    }
}

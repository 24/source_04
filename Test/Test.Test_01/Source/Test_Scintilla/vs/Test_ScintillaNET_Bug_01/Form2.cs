using System;
using System.Windows.Forms;

namespace Test_ScintillaNET_Bug_01
{
    public partial class Form2 : Form
    {
        private const int WM_CUSTOM_MSG_01 = WM.WM_USER;
        private IntPtr _hwnd = IntPtr.Zero;

        public Form2()
        {
            InitializeComponent();

            scintilla1.ClearCmdKey(Keys.Control | Keys.O);
        }

        private void OpenDialog()
        {
            Dialog dialog = new Dialog();
            dialog.ShowDialog();
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt && e.Control && !e.Shift && e.KeyCode == Keys.O)
            {
                //OpenDialog();
                User32.PostMessage(_hwnd, WM_CUSTOM_MSG_01, 0, 0);
            }
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM.WM_CREATE:
                    _hwnd = m.HWnd;
                    break;

                case WM_CUSTOM_MSG_01:
                    OpenDialog();
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

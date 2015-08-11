using System;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public static class zerrf
    {
        public static void ErrorMessageBox(Exception ex)
        {
            MessageBox.Show(Error.GetErrorMessage(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
}

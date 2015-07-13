using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using pb.IO;

namespace pb.Windows.Forms
{
    public class cf
    {
        public delegate void fExe();
        public delegate void fExe2(Form form);

        public static void MenuExe(fExe f)
        {
            MenuExe(null, f);
        }

        #region MenuExe(Form form, fExe f)
        public static void MenuExe(Form form, fExe f)
        {
            try
            {
                if (form != null) form.Cursor = Cursors.WaitCursor;
                f();
                if (form != null) form.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                if (form != null) form.Cursor = Cursors.Default;
                zerrf.ErrorMessageBox(ex);
            }
        }
        #endregion

        #region MenuExe
        public static void MenuExe(fExe2 f)
        {
            MenuExe(null, f);
        }
        #endregion

        #region MenuExe(Form form, fExe2 f)
        public static void MenuExe(Form form, fExe2 f)
        {
            try
            {
                form.Cursor = Cursors.WaitCursor;
                f(form);
                form.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                form.Cursor = Cursors.Default;
                zerrf.ErrorMessageBox(ex);
            }
        }
        #endregion

        #region DialogOpenFile
        public static string DialogOpenFile(string sPathDefaut, string sFilter)
        {
            string sPath, sFileName, sDir;
            OpenFileDialog ofDlg;
            DialogResult dr;

            sDir = zapp.GetAppDirectory();
            sFileName = null;
            if (File.Exists(sPathDefaut))
            {
                sDir = Path.GetDirectoryName(sPathDefaut);
                sFileName = Path.GetFileName(sPathDefaut);
            }
            else if (Directory.Exists(sPathDefaut))
            {
                sDir = sPathDefaut;
            }
            else
            {
                sPathDefaut = Path.GetDirectoryName(sPathDefaut);
                if (Directory.Exists(sPathDefaut)) sDir = sPathDefaut;
            }

            ofDlg = new OpenFileDialog();
            ofDlg.InitialDirectory = sDir;
            if (sFilter == null) sFilter = "All files (*.*)|*.*"; // "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            ofDlg.Filter = sFilter;
            ofDlg.FilterIndex = 1;
            ofDlg.RestoreDirectory = true;
            ofDlg.FileName = sFileName;
            ofDlg.CheckFileExists = false;
            dr = ofDlg.ShowDialog();

            sPath = null;
            if (dr == DialogResult.OK)
            {
                sPath = ofDlg.FileName;
            }
            return sPath;
        }
        #endregion

        #region DialogOpenFiles
        public static string[] DialogOpenFiles(string sPathDefaut, string sFilter)
        {
            string sDir = zapp.GetAppDirectory();
            string sFileName = null;
            if (File.Exists(sPathDefaut))
            {
                sDir = Path.GetDirectoryName(sPathDefaut);
                sFileName = Path.GetFileName(sPathDefaut);
            }
            else if (Directory.Exists(sPathDefaut))
            {
                sDir = sPathDefaut;
            }
            else
            {
                sPathDefaut = Path.GetDirectoryName(sPathDefaut);
                if (Directory.Exists(sPathDefaut)) sDir = sPathDefaut;
            }

            OpenFileDialog ofDlg = new OpenFileDialog();
            ofDlg.InitialDirectory = sDir;
            if (sFilter == null) sFilter = "All files (*.*)|*.*"; // "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            ofDlg.Multiselect = true;
            ofDlg.Filter = sFilter;
            ofDlg.FilterIndex = 1;
            ofDlg.RestoreDirectory = true;
            ofDlg.FileName = sFileName;
            ofDlg.CheckFileExists = false;
            DialogResult dr = ofDlg.ShowDialog();

            string[] sPath = null;
            if (dr == DialogResult.OK)
            {
                sPath = ofDlg.FileNames;
            }
            return sPath;
        }
        #endregion
    }
}

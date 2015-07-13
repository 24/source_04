using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    #region class MainBaseForm
    public partial class MainBaseForm : Form //FormWinProc
    {
        //WinProc gWinProc = null;

        //#region WinProc
        //public WinProc WinProc
        //{
        //    get { return gWinProc; }
        //}
        //#endregion

        #region constructor
        public MainBaseForm()
        {
            InitializeComponent();
            this.Text = "Form base";
            //WindowsApp.CurrentWindowsApp = new WindowsApp(this);
            //WindowsApp.CurrentWindowsApp.Trace = true;

            //gWinProc = new WinProc(this);
        }
        #endregion
    }
    #endregion
}

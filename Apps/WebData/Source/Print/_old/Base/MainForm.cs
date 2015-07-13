using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Download
{
    #region class MainForm
    public partial class MainForm : Form
    {
        #region constructor
        public MainForm(Icon icon)
        {
            Init(icon);
        }
        #endregion

        #region Init
        public void Init(Icon icon)
        {
            InitializeComponent();
            this.Icon = icon;
        }
        #endregion

        private void m_quit_Click(object sender, EventArgs e)
        {

        }
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //listBox1.Items[0]
            listBox1.SelectedIndex = 5;
        }

        private void btExecute_Click(object sender, EventArgs e)
        {
            tbTextBox1.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

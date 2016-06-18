using System.Windows.Forms;

namespace Test_WinForm_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.app;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            label1.Text = Test_WinForm_01.Properties.Resources.String1;
        }
    }
}

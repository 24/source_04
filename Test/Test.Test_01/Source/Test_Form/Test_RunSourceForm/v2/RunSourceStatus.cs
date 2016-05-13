using System;
using System.Drawing;
using System.Windows.Forms;

namespace Test.Test_Form.Test_RunSourceForm.v2
{
    partial class RunSourceForm
    {
        private Panel pan_status_v2;

        private void CreatePanStatus_v2()
        {
            ToolStrip toolStrip1 = new ToolStrip();
            //ToolStripTextBox toolStripTextBox1 = new ToolStripTextBox();

            //ToolStripButton button = new ToolStripButton();
            //button.Text = "result 1";
            //button.Click += ToolStripButton1_Click;
            //toolStrip1.Items.Add(button);
            //ToolStripButton toolStripButton2 = new ToolStripButton();
            //toolStripButton2.Text = "result 2";
            //toolStripButton2.Click += ToolStripButton2_Click;
            //toolStrip1.Items.Add(toolStripButton2);

            toolStrip1.Items.Add(CreateToolStripButton("result 1", (sender, eventArgs) => ActivePanResult(1)));
            toolStrip1.Items.Add(CreateToolStripButton("result 2", (sender, eventArgs) => ActivePanResult(2)));
            toolStrip1.Items.Add(CreateToolStripButton("result 3", (sender, eventArgs) => ActivePanResult(3)));
            toolStrip1.Items.Add(CreateToolStripButton("result 4", (sender, eventArgs) => ActivePanResult(4)));
            toolStrip1.Items.Add(CreateToolStripButton("messages", (sender, eventArgs) => ActivePanResult(5)));

            pan_status_v2 = new Panel();
            pan_status_v2.Size = new Size(0, 20);
            pan_status_v2.BackColor = Color.DarkGreen;
            pan_status_v2.Dock = DockStyle.Bottom;
            pan_status_v2.Controls.Add(toolStrip1);

        }

        private static ToolStripButton CreateToolStripButton(string text, EventHandler onClick = null)
        {
            ToolStripButton button = new ToolStripButton();
            button.Text = text;
            button.Click += onClick;
            return button;
        }

        //private void ToolStripButton1_Click(object sender, EventArgs e)
        //{
        //    ActivePanResult(1);
        //}

        //private void ToolStripButton2_Click(object sender, EventArgs e)
        //{
        //    ActivePanResult(2);
        //}
    }
}

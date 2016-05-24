using System.Windows.Forms;
using pb.Windows.Forms;
using System.Drawing;

namespace Test.Test_Form.Test_ToolStrip
{
    public class ToolStrip1 : Form
    {
        public ToolStrip1()
        {
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.SuspendLayout();
            toolStrip.Dock = DockStyle.Top;
            var tb = zForm.CreateToolStripTextBox(backColor: SystemColors.Control, width: 100);
            tb.Enabled = false;
            toolStrip.Items.Add(tb);
            toolStrip.Items.Add(zForm.CreateToolStripLabel("tata"));
            toolStrip.Items.Add(zForm.CreateToolStripLabelSeparator(100));
            toolStrip.Items.Add(zForm.CreateToolStripTextBox("toto"));
            toolStrip.ResumeLayout(false);
            this.SuspendLayout();
            this.ClientSize = new Size(1000, 300);
            this.Controls.Add(toolStrip);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

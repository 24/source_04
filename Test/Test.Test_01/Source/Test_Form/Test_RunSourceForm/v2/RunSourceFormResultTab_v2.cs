using System.Drawing;
using System.Windows.Forms;

namespace Test.Test_Form.Test_RunSourceForm.v2
{
    partial class RunSourceForm
    {
        private Panel pan_result_v2;
        private Panel pan_tab_result1;
        private Panel pan_tab_result2;
        private Panel pan_tab_result3;
        private Panel pan_tab_result4;
        private Panel pan_tab_result5;
        private int _activeResult = 0;

        private void CreateResultTab_v2()
        {
            //pan_tab_result1 = new Panel();
            //pan_tab_result1.SuspendLayout();
            //pan_tab_result1.Dock = DockStyle.Fill;
            //pan_tab_result1.BackColor = Color.Aqua;
            ////pan_tab_result1.Visible = false;
            //pan_tab_result1.ResumeLayout(false);

            //pan_tab_result2 = new Panel();
            //pan_tab_result2.SuspendLayout();
            //pan_tab_result2.Dock = DockStyle.Fill;
            //pan_tab_result2.BackColor = Color.Beige;
            ////pan_tab_result2.Visible = false;
            //pan_tab_result2.ResumeLayout(false);

            //SystemColors.Control
            pan_tab_result1 = CreatePanel(DockStyle.Fill);  // Color.Aqua
            pan_tab_result2 = CreatePanel(DockStyle.Fill);  // Color.Beige
            pan_tab_result3 = CreatePanel(DockStyle.Fill);  // Color.AliceBlue
            pan_tab_result4 = CreatePanel(DockStyle.Fill);  // Color.AntiqueWhite
            pan_tab_result5 = CreatePanel(DockStyle.Fill);  // Color.BlueViolet

            pan_result_v2 = new Panel();
            pan_result_v2.SuspendLayout();
            pan_result_v2.Dock = DockStyle.Fill;
            //pan_result_v2.BackColor = SystemColors.ControlDark;
            pan_result_v2.BackColor = Color.Transparent;
            pan_result_v2.Controls.Add(pan_tab_result1);
            pan_result_v2.Controls.Add(pan_tab_result2);
            pan_result_v2.Controls.Add(pan_tab_result3);
            pan_result_v2.Controls.Add(pan_tab_result4);
            pan_result_v2.Controls.Add(pan_tab_result5);
            //this.Controls.Add(pan_result_v2);
            pan_result_v2.ResumeLayout(false);
            pan_result_v2.PerformLayout();

            ActivePanResult(1);
        }

        private Panel CreatePanel(DockStyle dockStyle, Color? backColor = null)
        {
            Panel panel = new Panel();
            panel.SuspendLayout();
            panel.Dock = dockStyle;
            if (backColor != null)
                panel.BackColor = (Color)backColor;
            panel.ResumeLayout(false);
            return panel;
        }

        private void ActivePanResult(int i)
        {
            if (_activeResult == i)
                return;
            //pan_result_v2.SuspendLayout();
            //pan_tab_result1.SuspendLayout();
            //pan_tab_result2.SuspendLayout();
            //if (_gridResult2 != null)
            //    _gridResult2.SuspendLayout();
            pan_tab_result1.Visible = false;
            pan_tab_result2.Visible = false;
            pan_tab_result3.Visible = false;
            pan_tab_result4.Visible = false;
            pan_tab_result5.Visible = false;
            switch (i)
            {
                case 1:
                    pan_tab_result1.Visible = true;
                    break;
                case 2:
                    pan_tab_result2.Visible = true;
                    break;
                case 3:
                    pan_tab_result3.Visible = true;
                    break;
                case 4:
                    pan_tab_result4.Visible = true;
                    break;
                case 5:
                    pan_tab_result5.Visible = true;
                    break;
            }
            //pan_tab_result1.ResumeLayout(false);
            //pan_tab_result2.ResumeLayout(false);
            //pan_result_v2.ResumeLayout(false);
            //if (_gridResult2 != null)
            //    _gridResult2.ResumeLayout(false);
            //if (i == 1)
            //{
            //    //pan_tab_result1.PerformLayout();
            //}
            //else if (i == 2)
            //{
            //    //if (_gridResult2 != null)
            //    //    _gridResult2.PerformLayout();
            //    //pan_tab_result2.PerformLayout();
            //    //pan_result_v2.PerformLayout();
            //}
            _activeResult = i;
        }
    }
}

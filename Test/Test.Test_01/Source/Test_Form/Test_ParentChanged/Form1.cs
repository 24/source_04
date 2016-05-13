using pb;
using pb.Windows.Forms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// problème : l'ordre des control de _tabPanel peut changer
// par exemple si on affiche panel 5 puis panel 4, l'ordre devient : panel 4, panel 1, panel 2, panel 3, panel 5

namespace Test.Test_Form.Test_ParentChanged
{
    public class Form1 : Form
    {
        private IContainer _components = null;
        private Panel _topPanel;
        private TextBox _textBox;

        public Form1()
        {
            Trace.WriteLine("Form1 constructor");
            _topPanel = zForm.CreatePanel(dockStyle: DockStyle.Top, backColor: Color.DodgerBlue, height: 300);
            _topPanel.SuspendLayout();

            Trace.WriteLine("Create TextBox");
            _textBox = zForm.CreateTextBox();
            //_textBox.ParentChanged += _textBox_ParentChanged;
            ControlFindForm.Find(_textBox, FormFound);
            Trace.WriteLine("before add TextBox to topPanel");
            _topPanel.Controls.Add(_textBox);
            Trace.WriteLine("after add TextBox to topPanel");

            this.SuspendLayout();

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 600);

            Trace.WriteLine("before add topPanel to Form");
            this.Controls.Add(_topPanel);
            Trace.WriteLine("after add topPanel to Form");

            _topPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void FormFound(Form form)
        {
            Trace.WriteLine("Form found");
            Trace.WriteLine(form == this ? "Form == this" : "Form != this");
        }

        //private void _textBox_ParentChanged(object sender, EventArgs e)
        //{
        //    Trace.WriteLine("TextBox ParentChanged :");
        //    Trace.WriteLine("  sender {0}", sender.GetType().Name);
        //    Trace.WriteLine("  form {0}", _textBox.FindForm() != null ? "found" : "not found");
        //    Control parent = _textBox.Parent;
        //    int i = 1;
        //    while (parent != null)
        //    {
        //        Trace.WriteLine("  {0} parent {1}", i++, parent.GetType().Name);
        //        parent = parent.Parent;
        //    }
        //    Trace.WriteLine();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

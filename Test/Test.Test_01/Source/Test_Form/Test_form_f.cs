using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pb;
using pb.Compiler;

namespace Test.Test_Form
{
    static partial class w
    {
        private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
            //_rs.InitConfig("test");
            //string log = _rs.Config.GetRootSubPath("Log", null);
            //if (log != null) Trace.CurrentTrace.SetLogFile(log, LogOptions.IndexedFile);
            //string trace = _rs.Config.GetRootSubPath("Trace", null);
            //if (trace != null) _rs.Trace.SetTraceDirectory(trace);
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            Trace.WriteLine("toto");
        }

        public static void Test_Form_ViewOpenForms()
        {
            foreach (Form form in Application.OpenForms)
            {
                Trace.WriteLine("form : \"{0}\" {1}", form.Text, form.GetType());
            }
        }

        public static Form GetForm(string title)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text == title)
                    return form;
            }
            Trace.WriteLine("Form \"{0}\" not found", title);
            return null;
        }

        public static void Form1_CreateForm()
        {
            if (_rs.IsRunning())
            {
                Trace.WriteLine("execute with shift F5 to use main thread for creating form");
                return;
            }
            Form1 form1 = new Form1();
            form1.Show();
        }

        public static void Scintilla01_CreateForm()
        {
            if (_rs.IsRunning())
            {
                Trace.WriteLine("execute with shift F5 to use main thread for creating form");
                return;
            }
            Scintilla01 form = new Scintilla01();
            form.Show();
        }

        public static void Form2_ChangeFormHeight()
        {
            Form form = GetForm("Form2");
            if (form == null)
                return;
            int height;
            if (form.Size.Height == 800)
                height = 600;
            else
                height = 800;
            Trace.WriteLine("Form2 set size to {0} x {1}", form.Size.Width, height);
            form.Size = new System.Drawing.Size(form.Size.Width, height);
        }

        public static void Form2_ChangeTabControlHeight()
        {
            Form form = GetForm("Form2");
            if (form == null)
                return;
            Control control = form.Controls["tc_result"];
            if (control == null)
            {
                Trace.WriteLine("control \"tc_result\" not found");
                return;
            }
            int height;
            if (control.Size.Height == 400)
                height = 200;
            else
                height = 400;
            Trace.WriteLine("Form2 control \"tc_result\" size {0} x {1}", control.Size.Width, control.Size.Height);
            Trace.WriteLine("Form2 control \"tc_result\" set size to {0} x {1}", control.Size.Width, height);
            control.Size = new System.Drawing.Size(control.Size.Width, height);
        }
    }
}

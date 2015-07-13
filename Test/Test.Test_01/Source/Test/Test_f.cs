using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;

namespace Test
{
    class test
    {
        public string t1;
        public string t2;
        public int[] i = new int[] { 1, 2, 3, 4, 5 }; 
    }


    static partial class w
    {
        //private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;
        //private static HtmlXmlReader _hxr = HtmlXmlReader.CurrentHtmlXmlReader;

        public static void Init()
        {
            //_rs.InitConfig("test");
            XmlConfig.CurrentConfig = new XmlConfig("test");
            //string log = XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetAppDirectory());
            //if (log != null)
            //    Trace.CurrentTrace.SetLogFile(log, LogOptions.IndexedFile);
            Trace.CurrentTrace.SetWriter(XmlConfig.CurrentConfig.Get("Log"), XmlConfig.CurrentConfig.Get("Log/@option").zTextDeserialize(FileOption.None));

            //string trace = XmlConfig.CurrentConfig.Get("Trace").zRootPath(zapp.GetAppDirectory());
            //if (trace != null)
            //    Trace.CurrentTrace.SetTraceDirectory(trace);
            //_hxr.SetResult += new SetResultEvent(_wr.SetResult);
        }

        public static void End()
        {
            //_hxr.SetResult -= new SetResultEvent(_wr.SetResult);
        }

        public static void Test_01(string s)
        {
            Trace.WriteLine("toto {0}", s);
            Test_02(j: 30);
        }

        public static void Test_02(int i = 10, int j = 20)
        {
            Trace.WriteLine("Test_02");
            //test v = new test();
            ///v.t1 = "toto";
            //v.t2 = "tata";
            //_wr.View(v);
            List<test> list = new List<test>();
            list.Add(new test() { t1 = "toto1", t2 = "tata1" });
            list.Add(new test() { t1 = "toto2", t2 = "tata2" });
            list.Add(new test() { t1 = "toto3", t2 = "tata3" });
            list.Add(new test() { t1 = "toto4", t2 = "tata4" });
            list.Add(new test() { t1 = "toto5", t2 = "tata5" });
            list.Add(new test() { t1 = "toto6", t2 = "tata6" });
            list.zView();
        }

        public static void Test_Interface_01()
        {
            Trace.WriteLine("Test_Interface_01");

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
            //return (Form2)Application.OpenForms["Form2"];
            foreach (Form form in Application.OpenForms)
            {
                //Trace.WriteLine("form : \"{0}\" {1}", form.Text, form.GetType());
                //if (form is Form2)
                //    return (Form2)form;
                if (form.Text == title)
                    return form;
            }
            Trace.WriteLine("Form2 not found");
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

        public static void Form2_CreateForm()
        {
            if (_rs.IsRunning())
            {
                Trace.WriteLine("execute with shift F5 to use main thread for creating form");
                return;
            }
            Form2 form2 = new Form2();
            form2.Show();
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

        public static void RunSourceForm_CreateForm()
        {
            if (_rs.IsRunning())
            {
                Trace.WriteLine("execute with shift F5 to use main thread for creating form");
                return;
            }
            RunSourceForm form = new RunSourceForm();
            form.Show();
        }

        public static void RunSourceForm_ChangeTabControlHeight()
        {
            string title = "Run source 1";
            Form form = GetForm(title);
            if (form == null)
                return;
            Control control = form.Controls["tc_result"];
            if (control == null)
            {
                Trace.WriteLine("control \"tc_result\" not found");
                return;
            }
            int height;
            if (control.Size.Height == 500)
                height = 300;
            else
                height = 500;
            Trace.WriteLine("{0} control \"tc_result\" size {1} x {2}", title, control.Size.Width, control.Size.Height);
            Trace.WriteLine("{0} control \"tc_result\" set size to {1} x {2}", title, control.Size.Width, height);
            control.Size = new System.Drawing.Size(control.Size.Width, height);
        }

        public static void RunSourceForm2_CreateForm()
        {
            if (_rs.IsRunning())
            {
                Trace.WriteLine("execute with shift F5 to use main thread for creating form");
                return;
            }
            RunSourceForm2 form = new RunSourceForm2();
            form.Show();
        }

        public static void RunSourceForm2_ChangeTabControlHeight()
        {
            string title = "Run source 2";
            //string controlName = "tc_result";
            string controlName = "pan_top";
            Form form = GetForm(title);
            if (form == null)
                return;
            Control control = form.Controls[controlName];
            if (control == null)
            {
                Trace.WriteLine("control \"{0}\" not found", controlName);
                return;
            }
            int height;
            if (control.Size.Height == 200)
                height = 400;
            else
                height = 200;
            Trace.WriteLine("{0} control \"{1}\" size {2} x {3}", title, controlName, control.Size.Width, control.Size.Height);
            Trace.WriteLine("{0} control \"{1}\" set size to {2} x {3}", title, controlName, control.Size.Width, height);
            control.Size = new System.Drawing.Size(control.Size.Width, height);
        }

    }

    interface ITest
    {
        ITest GetCurrent();
    }

    class Test : ITest
    {
        public ITest GetCurrent()
        {
            return null;
        }
    }
}

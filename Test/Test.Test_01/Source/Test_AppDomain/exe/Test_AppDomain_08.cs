using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using pb;
using pb.Compiler;
using pb.Data.Xml;

namespace Test_AppDomain_08
{
    class Program
    {
        private static IRunSource _runSource = null;
        private static bool _disableMessage = false;
        //private static bool _selectTreeViewResult = false;  // true si il faut sélectionner le résultat du TreeView

        static void Main()
        {
            Console.WriteLine("Test_AppDomain_08 : test runsource.dll");
            AppDomain domain = AppDomain.CreateDomain("New Domain");

            //Console.WriteLine("set initial lease time to 10 sec");
            //LifetimeServices.LeaseTime = TimeSpan.FromSeconds(10);  // This is the InitialLeaseTime property
            //Console.WriteLine();

            DateTime dt1 = DateTime.Now;
            //IRunSource rs = (IRunSource)domain.CreateInstanceFromAndUnwrap("runsource.dll", "pb.RunSource");
            //Console.WriteLine("{0:HH:mm:ss} create object pb.RunSource", dt1);

            Console.WriteLine("{0:HH:mm:ss} create object RemoteRunSource", dt1);
            Console.WriteLine();
            RemoteRunSource remoteRunSource = new RemoteRunSource();
            _runSource = remoteRunSource.GetRunSource();
            _runSource.SetRunSourceConfig(@"c:\pib\prog\tools\runsource\runsource32_config.xml");
            //_runSource.Trace.Writed += new WritedEvent(EventWrited);
            Trace.CurrentTrace.SetViewer(EventWrited);
            _runSource.DisableMessageChanged += new DisableMessageChangedEvent(EventDisableMessageChanged);
            _runSource.GridResultSetDataTable += new SetDataTableEvent(EventGridResultSetDataTable);
            _runSource.GridResultSetDataSet += new SetDataSetEvent(EventGridResultSetDataSet);
            //_runSource.TreeViewResultAdd += new TreeViewResultAddEvent(EventTreeViewResultAdd);
            //_runSource.TreeViewResultSelect += new TreeViewResultSelectEvent(EventTreeViewResultSelect);
            _runSource.ErrorResultSet += new SetDataTableEvent(EventErrorResultSet);
            _runSource.ProgressChange += new ProgressChangeEvent(EventProgressChange);
            _runSource.EndRunCode += new EndRunEvent(EventEndRun);


            DateTime dt2;
            string s = "RunSource.CurrentDomainRunSource.Trace.WriteLine(DateTime.Now.ToString());";

            dt2 = DateTime.Now;
            Console.WriteLine("{0:HH:mm:ss}-{1} execute : {2}", dt2, TimeSpanToString(dt2.Subtract(dt1)), s);
            _runSource.RunCode(s);
            while (_runSource.IsRunning())
                Thread.Sleep(100);

            //Console.WriteLine("wait 15 sec");
            //Thread.Sleep(15000);
            //dt2 = DateTime.Now;
            //Console.WriteLine("{0:HH:mm:ss}-{1} test_03.GetMessage() : {2}", dt2, TimeSpanToString(dt2.Subtract(dt1)), test03.GetMessage());

            //Console.WriteLine("wait 2 min");
            //Thread.Sleep(120000);
            //dt2 = DateTime.Now;
            //Console.WriteLine("{0:HH:mm:ss}-{1} test_03.GetMessage() : {2}", dt2, TimeSpanToString(dt2.Subtract(dt1)), test03.GetMessage());

            Console.WriteLine("wait 6 min");
            Thread.Sleep(360000);
            //Console.WriteLine("wait 10 min");
            //Thread.Sleep(600000);
            //Console.WriteLine("wait 15 min");
            //Thread.Sleep(900000);
            //Console.WriteLine("wait 30 min");
            //Thread.Sleep(1800000);
            //Console.WriteLine("wait 2h");
            //Thread.Sleep(7200000);
            dt2 = DateTime.Now;
            Console.WriteLine("{0:HH:mm:ss}-{1} execute : {2}", dt2, TimeSpanToString(dt2.Subtract(dt1)), s);
            _runSource.RunCode(s);
            while (_runSource.IsRunning())
                Thread.Sleep(100);


            //for (int i = 0; i < 500; i++)
            //{
            //    Thread.Sleep(1000);
            //    DateTime dt2 = DateTime.Now;
            //    Console.WriteLine("{0,6} {1:HH:mm:ss}-{2} test_03.GetMessage() : {3}", i + 1, dt2, TimeSpanToString(dt2.Subtract(dt1)), test03.GetMessage());
            //}

            AppDomain.Unload(domain);
        }

        //private static void EventWrited(string msg, params object[] prm)
        private static void EventWrited(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void EventDisableMessageChanged(bool disableMessage)
        {
            _disableMessage = disableMessage;
        }

        private static void EventGridResultSetDataTable(DataTable dt, string xmlFormat)
        {
        }

        private static void EventGridResultSetDataSet(DataSet ds, string xmlFormat)
        {
        }

        private static void EventTreeViewResultAdd(string nodeName, XElement xmlElement, XFormat xFormat)
        {
        }

        //private static void EventTreeViewResultSelect()
        //{
        //    _selectTreeViewResult = true;
        //}

        private static void EventErrorResultSet(DataTable dt, string xmlFormat)
        {
        }

        private static void EventProgressChange(int current, int total, string message, params object[] prm)
        {
        }

        private static void EventEndRun(bool error)
        {
            //try
            //{
                //_runSource.Trace.WriteLine("Process completed {0}", _runSource.ExecutionChrono.TotalTimeString);
                //Console.WriteLine("Process completed {0}", _runSource.ExecutionChrono.TotalTimeString);
            //}
            //catch (Exception ex)
            //{
            //    cf.ErrorMessageBox(ex);
            //}
            //_runSource.Pause = false;
            Console.WriteLine("End run (domain \"{0}\")", AppDomain.CurrentDomain.FriendlyName);
        }

        public static string TimeSpanToString(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }
}

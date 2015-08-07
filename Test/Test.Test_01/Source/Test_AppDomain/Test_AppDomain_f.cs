using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;

namespace Test_AppDomain
{
    static partial class w
    {
        private static RunSource _wr = RunSource.CurrentRunSource;
        private static ITrace _tr = Trace.CurrentTrace;
        private static string _dataDir;

        public static void Init()
        {
            XmlConfig.CurrentConfig = new XmlConfig("Test_AppDomain");
            //string log = XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetAppDirectory());
            //if (log != null)
            //    _tr.SetLogFile(log, LogOptions.IndexedFile);
            Trace.CurrentTrace.SetWriter(XmlConfig.CurrentConfig.Get("Log"), XmlConfig.CurrentConfig.Get("Log/@option").zTextDeserialize(FileOption.None));
            _dataDir = XmlConfig.CurrentConfig.GetExplicit("DataDir");

            //string trace = XmlConfig.CurrentConfig.Get("Trace").zRootPath(zapp.GetAppDirectory());
            //if (trace != null)
            //    Trace.CurrentTrace.SetTraceDirectory(trace);
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Test_AppDomain_01()
        {
            _tr.WriteLine("Test_AppDomain_01");
            _tr.WriteLine("Current directory \"{0}\"", zDirectory.GetCurrentDirectory());
            _tr.WriteLine("Current domain friendly name \"{0}\"", AppDomain.CurrentDomain.FriendlyName);
            _tr.WriteLine("Current domain base directory \"{0}\"", AppDomain.CurrentDomain.BaseDirectory);
            _tr.WriteLine("Current domain relative search path \"{0}\"", AppDomain.CurrentDomain.RelativeSearchPath);
            AppDomainSetup setup = new AppDomainSetup();
            setup.PrivateBinPath = "wrun";
            AppDomain domain = AppDomain.CreateDomain("New Domain", null, setup);
            _tr.WriteLine("New domain friendly name \"{0}\"", domain.FriendlyName);
            _tr.WriteLine("New domain base directory \"{0}\"", domain.BaseDirectory);
            _tr.WriteLine("New domain relative search path \"{0}\"", domain.RelativeSearchPath);
            AppDomain.Unload(domain);
        }

        public static void Test_AppDomain_02()
        {
            _tr.WriteLine("Test_AppDomain_02 : use of AppDomain.DoCallBack");
            AppDomainSetup setup = new AppDomainSetup();
            setup.PrivateBinPath = "wrun";
            AppDomain domain = AppDomain.CreateDomain("New Domain", null, setup);
            //domain.DoCallBack(new CrossAppDomainDelegate(Test_AppDomain_02_function));
            domain.DoCallBack(Test_AppDomain_02_function);
            AppDomain.Unload(domain);
        }

        public static void Test_AppDomain_02_function()
        {
            //string file = @"c:\pib\dev_data\exe\wrun\test\app_domain\test_01.txt";
            Console.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            _tr.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            string file = @"c:\pib\dev_data\exe\wrun\test\app_domain\test_01.txt";
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            sw.Close();
        }

        public static void Test_AppDomain_03()
        {
            _tr.WriteLine("Test_AppDomain_03 : use of AppDomain.SetData and AppDomain.GetData");
            AppDomainSetup setup = new AppDomainSetup();
            setup.PrivateBinPath = "wrun";
            AppDomain domain = AppDomain.CreateDomain("New Domain", null, setup);
            domain.SetData("message", "hello");
            domain.SetData("test_01", new test_01() { text = "test_01 from Test_AppDomain_03" });
            domain.SetData("test_02", new test_02() { text = "test_02 from Test_AppDomain_03", trace = _tr });
            //domain.DoCallBack(new CrossAppDomainDelegate(SayHello));
            domain.DoCallBack(Test_AppDomain_03_function);
            AppDomain.Unload(domain);
        }

        public static void Test_AppDomain_03_function()
        {
            Console.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            _tr.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            _tr.WriteLine("Message : " + AppDomain.CurrentDomain.GetData("message"));
            string file = @"c:\pib\dev_data\exe\wrun\test\app_domain\test_01.txt";
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            sw.WriteLine("Message : " + AppDomain.CurrentDomain.GetData("message"));
            sw.WriteLine("class test_01.text : " + ((test_01)AppDomain.CurrentDomain.GetData("test_01")).text);
            sw.WriteLine("class test_02.text : " + ((test_02)AppDomain.CurrentDomain.GetData("test_02")).text);
            sw.Close();
            test_02 test02 = (test_02)AppDomain.CurrentDomain.GetData("test_02");
            test02.WriteLine("hello from Test_AppDomain_03_function");
        }

        public static void Test_AppDomain_04()
        {
            _tr.WriteLine("Test_AppDomain_04 : use of AppDomain.CreateInstanceAndUnwrap");
            AppDomainSetup setup = new AppDomainSetup();
            setup.PrivateBinPath = "wrun";
            AppDomain domain = AppDomain.CreateDomain("New Domain", null, setup);
            //test_03 test03 = (test_03)domain.CreateInstanceAndUnwrap(typeof(test_03).Assembly.FullName, typeof(test_03).FullName);
            object otest03 = domain.CreateInstanceAndUnwrap(typeof(test_03).Assembly.FullName, typeof(test_03).FullName);
            _tr.WriteLine("typeof(test_03).Assembly.FullName : {0}", typeof(test_03).Assembly.FullName);
            _tr.WriteLine("typeof(test_03).FullName : {0}", typeof(test_03).FullName);
            _tr.WriteLine("otest03.GetType().FullName : {0}", otest03.GetType().Assembly.FullName);
            _tr.WriteLine("otest03.GetType().FullName : {0}", otest03.GetType().FullName);
            itest_03 test03 = (itest_03)otest03;
            _tr.WriteLine(test03.GetMessage());
            AppDomain.Unload(domain);
        }
    }

    [Serializable]
    public class test_01
    {
        public string text;
    }

    public class test_02 : MarshalByRefObject
    {
        public string text;
        public ITrace trace;

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

        public void WriteLine(string message)
        {
            trace.WriteLine(message);
        }
    }

    public interface itest_03
    {
        string GetMessage();
    }

    public class test_03 : MarshalByRefObject, itest_03
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

        public string GetMessage()
        {
            return "message from test_03 in domain " + AppDomain.CurrentDomain.FriendlyName;
        }
    }
}

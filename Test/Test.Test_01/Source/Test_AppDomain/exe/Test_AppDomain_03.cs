using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace Test_AppDomain_03
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Test_AppDomain_03 : use of AppDomain.SetData and AppDomain.GetData");
            AppDomain domain = AppDomain.CreateDomain("New Domain");
            domain.SetData("message", "hello");
            domain.SetData("test_01", new test_01() { text = "test_01 from Test_AppDomain_03" });
            domain.SetData("test_02", new test_02() { text = "test_02 from Test_AppDomain_03" });
            //domain.DoCallBack(new CrossAppDomainDelegate(SayHello));
            domain.DoCallBack(Test_AppDomain_03_function);
            AppDomain.Unload(domain);
        }

        public static void Test_AppDomain_03_function()
        {
            Console.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("Message : " + AppDomain.CurrentDomain.GetData("message"));
            Console.WriteLine("class test_01.text : " + ((test_01)AppDomain.CurrentDomain.GetData("test_01")).text);
            Console.WriteLine("class test_02.text : " + ((test_02)AppDomain.CurrentDomain.GetData("test_02")).text);
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

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }
    }
}

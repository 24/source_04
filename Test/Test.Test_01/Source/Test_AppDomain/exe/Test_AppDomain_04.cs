using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace Test_AppDomain_04
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Test_AppDomain_04 : use of AppDomain.CreateInstanceAndUnwrap");
            AppDomain domain = AppDomain.CreateDomain("New Domain");
            object otest03 = domain.CreateInstanceAndUnwrap(typeof(test_03).Assembly.FullName, typeof(test_03).FullName);
            // Exception: System.IO.FileNotFoundException: Could not load file or assembly 'file:///...\Test_AppDomain_04, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies. The system cannot find the file specified.
            object otest03_2 = domain.CreateInstanceFromAndUnwrap("Test_AppDomain_04.exe", typeof(test_03).FullName);
            Console.WriteLine("typeof(test_03).Assembly.FullName : {0}", typeof(test_03).Assembly.FullName);
            Console.WriteLine("typeof(test_03).FullName : {0}", typeof(test_03).FullName);
            Console.WriteLine("otest03.GetType().FullName : {0}", otest03.GetType().Assembly.FullName);
            Console.WriteLine("otest03.GetType().FullName : {0}", otest03.GetType().FullName);
            test_03 test03 = (test_03)otest03;
            Console.WriteLine("test03.GetMessage() : {0}", test03.GetMessage());
            Console.WriteLine("test03_2.GetMessage() : {0}", ((test_03)otest03_2).GetMessage());
            AppDomain.Unload(domain);
        }
    }

    public class test_03 : MarshalByRefObject
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

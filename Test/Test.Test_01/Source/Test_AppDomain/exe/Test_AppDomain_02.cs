using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Test_AppDomain_02
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Test_AppDomain_02 : use of AppDomain.DoCallBack");
            AppDomain domain = AppDomain.CreateDomain("New Domain");
            domain.DoCallBack(new CrossAppDomainDelegate(Test_AppDomain_02_function));
            //domain.DoCallBack(Test_AppDomain_02_function);
            AppDomain.Unload(domain);
        }

        public static void Test_AppDomain_02_function()
        {
            Console.WriteLine("Hi from " + AppDomain.CurrentDomain.FriendlyName);
        }
    }
}

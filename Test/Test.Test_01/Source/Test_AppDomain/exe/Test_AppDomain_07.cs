using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using Test_AppDomain_05;

namespace Test_AppDomain_07
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Test_AppDomain_07 : test of life time service with Test_AppDomain_05.dll");
            AppDomain domain = AppDomain.CreateDomain("New Domain");

            Console.WriteLine("set initial lease time to 10 sec");
            LifetimeServices.LeaseTime = TimeSpan.FromSeconds(10);  // This is the InitialLeaseTime property
            Console.WriteLine();

            DateTime dt1 = DateTime.Now;
            itest_01 test02 = (itest_01)domain.CreateInstanceFromAndUnwrap("Test_AppDomain_05.dll", "Test_AppDomain_05.test_02");
            itest_01 test03 = (itest_01)domain.CreateInstanceFromAndUnwrap("Test_AppDomain_05.dll", "Test_AppDomain_05.test_03");
            Console.WriteLine("{0:HH:mm:ss} create object Test_AppDomain_05.test_02 and Test_AppDomain_05.test_03", dt1);
            Console.WriteLine();

            DateTime dt2;

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
            //Console.WriteLine("wait 30 min");
            //Thread.Sleep(1800000);
            //Console.WriteLine("wait 2h");
            //Thread.Sleep(7200000);
            dt2 = DateTime.Now;
            Console.WriteLine("{0:HH:mm:ss}-{1} test_02.GetMessage() : {2}", dt2, TimeSpanToString(dt2.Subtract(dt1)), test02.GetMessage());
            Console.WriteLine("{0:HH:mm:ss}-{1} test_03.GetMessage() : {2}", dt2, TimeSpanToString(dt2.Subtract(dt1)), test03.GetMessage());


            //for (int i = 0; i < 500; i++)
            //{
            //    Thread.Sleep(1000);
            //    DateTime dt2 = DateTime.Now;
            //    Console.WriteLine("{0,6} {1:HH:mm:ss}-{2} test_03.GetMessage() : {3}", i + 1, dt2, TimeSpanToString(dt2.Subtract(dt1)), test03.GetMessage());
            //}

            AppDomain.Unload(domain);
        }

        public static string TimeSpanToString(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }
}

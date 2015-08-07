using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using pb.IO;

namespace Test_AppDomain_01
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Test_AppDomain_01");
            Console.WriteLine("Current directory \"{0}\"", zDirectory.GetCurrentDirectory());
            Console.WriteLine("Current domain friendly name \"{0}\"", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("Current domain base directory \"{0}\"", AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Current domain relative search path \"{0}\"", AppDomain.CurrentDomain.RelativeSearchPath);
            AppDomain domain = AppDomain.CreateDomain("New Domain");
            Console.WriteLine("New domain friendly name \"{0}\"", domain.FriendlyName);
            Console.WriteLine("New domain base directory \"{0}\"", domain.BaseDirectory);
            Console.WriteLine("New domain relative search path \"{0}\"", domain.RelativeSearchPath);
            AppDomain.Unload(domain);
        }
    }
}

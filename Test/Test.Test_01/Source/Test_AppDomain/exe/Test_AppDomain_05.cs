using System;
using System.Reflection;

[assembly: AssemblyTitle("runsource")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pierre")]
[assembly: AssemblyProduct("runsource")]
[assembly: AssemblyCopyright("Copyright © Pierre Beuzart 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Test_AppDomain_05
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Test_AppDomain_05 : test loading dll in a domain use of AppDomain.CreateInstanceFromAndUnwrap");
            Console.WriteLine("Create domain \"Test domain\"");
            AppDomain domain = AppDomain.CreateDomain("Test domain");
            Console.WriteLine("Create instance from \"Test_AppDomain_05.dll\", \"Test_AppDomain_05.test_01\"");
            object test01 = domain.CreateInstanceFromAndUnwrap("Test_AppDomain_05.dll", "Test_AppDomain_05.test_01");
            itest_01 itest01 = (itest_01)test01;
            itest_02 itest02 = (itest_02)test01;
            Console.WriteLine("test01.GetMessage() \"{0}\"", itest01.GetMessage());
            Console.WriteLine("test01.GetCallingAssembly() \"{0}\"", itest02.GetCallingAssembly());
            Console.WriteLine("test01.GetCallingAssemblyProduct() \"{0}\"", itest02.GetCallingAssemblyProduct());
            Console.WriteLine("test01.GetEntryAssembly() \"{0}\"", itest02.GetEntryAssembly());
            Console.WriteLine("test01.GetExecutingAssembly() \"{0}\"", itest02.GetExecutingAssembly());
            Console.WriteLine("test01.GetExecutingAssemblyProduct() \"{0}\"", itest02.GetExecutingAssemblyProduct());
            Console.WriteLine("test01.GetApplicationCompany() \"{0}\"", itest02.GetApplicationCompany());
            Console.WriteLine("Unload domain \"Test domain\"");
            AppDomain.Unload(domain);
        }
    }
}

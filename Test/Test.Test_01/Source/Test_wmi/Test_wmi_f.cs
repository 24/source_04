using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using pb;

namespace Test.Test_wmi
{
    static partial class w
    {
        public static void Init()
        {
            //Trace.CurrentTrace.SetLogFile("log.txt", LogOptions.None);
        }

        public static void End()
        {
        }

    }

    public static class Test_wmi_f
    {
        public static void Test_wmi_01()
        {

            SelectQuery selectQuery = new SelectQuery("Win32_LogicalDisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);
            foreach (ManagementObject disk in searcher.Get())
            {
                Trace.WriteLine(disk.ToString());
            }
        }

        public static void TraceWmiNamespaces(string wmiNamespace = "root")
        {
            foreach (string wmiNamespace2 in GetWmiNamespaces(wmiNamespace))
                Trace.WriteLine(wmiNamespace2);
        }

        public static void TraceWmiClassNames(string wmiNamespace = "root")
        {
            foreach (string wmiNamespace2 in GetWmiNamespaces(wmiNamespace))
            {
                Trace.WriteLine(wmiNamespace2);
                foreach (string className in GetWmiClassNames(wmiNamespace2))
                {
                    Trace.WriteLine(className);
                }
            }
        }

        public static IEnumerable<String> GetWmiNamespaces(string wmiNamespace)
        {
            //ManagementScope scope = new ManagementScope("\\\\FullComputerName\\root\\cimv2");
            //ManagementScope scope = new ManagementScope("\\\\localhost\\root\\cimv2");
            //scope.Connect();


            //ManagementClass nsClass = new ManagementClass(new ManagementScope(root), new ManagementPath("__namespace"), null);
            //foreach (ManagementObject ns in nsClass.GetInstances())
            //{
            //    string namespaceName = root + "\\" + ns["Name"].ToString();
            //    namespaces.Add(namespaceName);
            //    namespaces.AddRange(GetWmiNamespaces(namespaceName));
            //}

            // from http://dotnetcodr.com/2014/11/21/finding-all-wmi-class-names-within-a-wmi-namespace-with-net-c/
            //string root = "root";

            //try
            //{
                ManagementClass nsClass = new ManagementClass(new ManagementScope(wmiNamespace), new ManagementPath("__namespace"), null);
                foreach (ManagementObject ns in nsClass.GetInstances())
                {
                    string wmiNamespace2 = wmiNamespace + "\\" + ns["Name"].ToString();
                    //namespaces.Add(namespaceName);
                    //namespaces.AddRange(GetWmiNamespaces(namespaceName));
                    //Trace.WriteLine(wmiNamespace2);
                    yield return wmiNamespace2;
                    GetWmiNamespaces(wmiNamespace2);
                }
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine("error namespace \"{0}\" - {1}", wmiNamespace, ex.Message);
            //}
        }

        public static IEnumerable<String> GetWmiClassNames(string wmiNamespace)
        {
            List<String> classes = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope(wmiNamespace), new WqlObjectQuery("SELECT * FROM meta_class"));
            foreach (ManagementClass wmiClass in searcher.Get())
            {
                //string stringified = wmiClass.ToString();
                //string[] parts = stringified.Split(new char[] { ':' });
                //classes.Add(parts[1]);
                yield return wmiClass.ToString();
            }
        }
    }
}

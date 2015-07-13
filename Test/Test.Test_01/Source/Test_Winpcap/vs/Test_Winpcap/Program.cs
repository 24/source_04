using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wpcap_dn;

namespace Test_Winpcap
{
    class Program
    {
        static void Main(string[] args)
        {
            Test_Winpcap_FindAllDevs_01();
        }

        public static void Test_Winpcap_FindAllDevs_01()
        {
            Device[] devices = wpcap.FindAllDevs();
            Console.WriteLine("{0} device(s)", devices.Length);
            foreach (Device device in devices)
            {
                Console.WriteLine("Device :");
                Console.WriteLine("  Name        : {0}", device.Name);
                Console.WriteLine("  Description : {0}", device.Description);
                Console.WriteLine("  Address     : {0}", device.Address);
                Console.WriteLine("  Netmask     : {0}", device.Netmask);
            }
        }
    }
}

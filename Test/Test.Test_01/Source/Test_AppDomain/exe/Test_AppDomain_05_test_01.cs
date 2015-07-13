using System;
using System.Reflection;
using System.Security.Permissions;

[assembly: AssemblyTitle("runsource_dll")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pierre")]
[assembly: AssemblyProduct("runsource_dll")]
[assembly: AssemblyCopyright("Copyright © Pierre Beuzart 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Test_AppDomain_05
{
    public class test_01 : MarshalByRefObject, itest_01, itest_02
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

        public string GetMessage()
        {
            return string.Format("Message from class test_01 executed in domain \"{0}\"", AppDomain.CurrentDomain.FriendlyName);
        }

        public string GetCallingAssembly()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            if (assembly != null)
            {
                return assembly.Location;
            }
            return null;
        }

        public string GetCallingAssemblyProduct()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            if (assembly != null)
            {
                return GetAssemblyProductAttribute(assembly);
            }
            return null;
        }

        public string GetEntryAssembly()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return assembly.Location;
            }
            return null;
        }

        public string GetExecutingAssembly()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                return assembly.Location;
            }
            return null;
        }

        public string GetExecutingAssemblyProduct()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                return GetAssemblyProductAttribute(assembly);
            }
            return null;
        }

        public string GetApplicationCompany()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                object[] oAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (oAttributes.Length > 0) return ((AssemblyCompanyAttribute)oAttributes[0]).Company;
            }
            return null;
        }

        public string GetAssemblyProductAttribute(Assembly assembly)
        {
            object attribute = GetAssemblyAttribute<AssemblyProductAttribute>(assembly);
            if (attribute != null)
                return ((AssemblyProductAttribute)attribute).Product;
            return null;
        }

        public object GetAssemblyAttribute<T>(Assembly assembly)
        {
            if (assembly != null)
            {
                object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
                if (attributes.Length > 0) return attributes[0];
            }
            return null;
        }
    }

    public class test_02 : MarshalByRefObject, itest_01
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // from http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
            return null;
        }

        public string GetMessage()
        {
            return string.Format("Message from class test_01 executed in domain \"{0}\"", AppDomain.CurrentDomain.FriendlyName);
        }
    }

    public class test_03 : MarshalByRefObject, itest_01
    {
        //public override object InitializeLifetimeService()
        //{
        //    // This ensures the object lasts for as long as the client wants it
        //    return null;
        //}

        public string GetMessage()
        {
            return string.Format("Message from class test_01 executed in domain \"{0}\"", AppDomain.CurrentDomain.FriendlyName);
        }
    }
}

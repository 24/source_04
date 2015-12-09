using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using pb.IO;

// Assembly.GetExecutingAssembly() : gets the assembly that contains the code that is currently executing. ex : RunSource_00004.dll
// Assembly.GetEntryAssembly()     : the assembly that is the process executable in the default application domain,
//                                   or the first executable that was executed by AppDomain.ExecuteAssembly, ex : runsource.runsource.exe

namespace pb
{
    public static class zapp
    {
        private static string __entryAssemblyFile = null;
        private static string __entryAssemblyDirectory = null;
        private static string __entryAssemblyFilename = null;

        //public static Assembly EntryAssembly
        //{
        //    get { return Assembly.GetEntryAssembly(); }
        //}

        //public static Assembly ExecutingAssembly
        //{
        //    get { return Assembly.GetExecutingAssembly(); }
        //}

        //public static string GetApplicationCompany()
        public static string GetEntryAssemblyCompany()
        {
            AssemblyCompanyAttribute attribute = Assembly.GetEntryAssembly().zGetAssemblyAttribute<AssemblyCompanyAttribute>();
            if (attribute != null)
                return attribute.Company;
            return null;
        }

        public static string GetExecutingAssemblyCompany()
        {
            AssemblyCompanyAttribute attribute = Assembly.GetExecutingAssembly().zGetAssemblyAttribute<AssemblyCompanyAttribute>();
            if (attribute != null)
                return attribute.Company;
            return null;
        }

        //public static string GetApplicationProduct()
        public static string GetEntryAssemblyProduct()
        {
            AssemblyProductAttribute attribute = Assembly.GetEntryAssembly().zGetAssemblyAttribute<AssemblyProductAttribute>();
            if (attribute != null)
                return attribute.Product;
            return null;
        }

        public static string GetExecutingAssemblyProduct()
        {
            AssemblyProductAttribute attribute = Assembly.GetExecutingAssembly().zGetAssemblyAttribute<AssemblyProductAttribute>();
            if (attribute != null)
                return attribute.Product;
            return null;
        }

        //public static string GetExecutablePath()
        public static string GetEntryAssemblyFile()
        {
            if (__entryAssemblyFile == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                    __entryAssemblyFile = assembly.Location;
            }
            return __entryAssemblyFile;
        }

        public static string GetEntryAssemblyDirectory()
        {
            if (__entryAssemblyDirectory == null)
            {
                string file = GetEntryAssemblyFile();
                if (file != null)
                    __entryAssemblyDirectory = zPath.GetDirectoryName(file);
            }
            return __entryAssemblyDirectory;
        }

        //public static string GetExecutableName()
        public static string GetEntryAssemblyFilename()
        {
            if (__entryAssemblyFilename == null)
            {
                string file = GetEntryAssemblyFile();
                if (file != null)
                    __entryAssemblyFilename = zPath.GetFileNameWithoutExtension(file);
            }
            return __entryAssemblyFilename;
        }

        public static string GetAppDirectory()
        {
            //return Thread.GetDomain().BaseDirectory;
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetConfigurationFile()
        {
            return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

        public static string GetPathFichier(string sNomFic)
        {
            return Thread.GetDomain().BaseDirectory + sNomFic;
        }

        public static string GetLocalSettingsDirectory(string product = null)
        {
            // "C:\Users\Pierre\AppData\Local"
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // "Pierre Beuzart"
            string company = zapp.GetEntryAssemblyCompany();
            // "runsource.runsource"
            if (product == null)
                product = zapp.GetEntryAssemblyProduct();
            if (company == null || product == null)
            {
                company = zapp.GetExecutingAssemblyCompany();
                product = zapp.GetExecutingAssemblyProduct();
            }
            if (company != null)
                directory += "\\" + company;
            if (product != null)
                directory += "\\" + product;
            // "C:\Users\Pierre\AppData\Local\Pierre Beuzart\runsource.runsource"
            return directory;
        }
    }

    public static class AssemblyExtension
    {
        public static T zGetAssemblyAttribute<T>(this Assembly assembly) where T : class
        {
            if (assembly != null)
            {
                object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
                if (attributes.Length > 0) return (T)attributes[0];
            }
            return null;
        }
    }
}

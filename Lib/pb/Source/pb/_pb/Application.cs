using System;
using System.Reflection;
using System.Threading;
using pb.IO;

// Assembly.GetExecutingAssembly() : gets the assembly that contains the code that is currently executing. ex : RunSource_00004.dll
// Assembly.GetEntryAssembly()     : the assembly that is the process executable in the default application domain,
//                                   or the first executable that was executed by AppDomain.ExecuteAssembly, ex : runsource.runsource.exe

namespace pb
{
    public static class zapp
    {
        private static string _entryAssemblyFile = null;
        private static string _entryAssemblyDirectory = null;
        private static string _entryAssemblyFilename = null;
        private static string _executingAssemblyFile = null;
        private static string _executingAssemblyDirectory = null;
        //

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
            if (_entryAssemblyFile == null)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                    _entryAssemblyFile = assembly.Location;
            }
            return _entryAssemblyFile;
        }

        public static string GetExecutingAssemblyFile()
        {
            if (_executingAssemblyFile == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                if (assembly != null)
                    _executingAssemblyFile = assembly.Location;
            }
            return _executingAssemblyFile;
        }

        public static string GetEntryAssemblyDirectory()
        {
            if (_entryAssemblyDirectory == null)
            {
                string file = GetEntryAssemblyFile();
                if (file != null)
                    _entryAssemblyDirectory = zPath.GetDirectoryName(file);
            }
            return _entryAssemblyDirectory;
        }

        public static string GetExecutingAssemblyDirectory()
        {
            if (_executingAssemblyDirectory == null)
            {
                string file = GetExecutingAssemblyFile();
                if (file != null)
                    _executingAssemblyDirectory = zPath.GetDirectoryName(file);
            }
            return _executingAssemblyDirectory;
        }

        //public static string GetExecutableName()
        public static string GetEntryAssemblyFilename()
        {
            if (_entryAssemblyFilename == null)
            {
                string file = GetEntryAssemblyFile();
                if (file != null)
                    _entryAssemblyFilename = zPath.GetFileNameWithoutExtension(file);
            }
            return _entryAssemblyFilename;
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

    public static class AssemblyExtension1
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

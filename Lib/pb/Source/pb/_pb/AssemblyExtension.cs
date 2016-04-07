using System.Linq;
using System.Reflection;

namespace pb
{
    public static class AssemblyExtension2
    {
        public static AssemblyInfo zGetAssemblyInfo(this Assembly assembly)
        {
            if (assembly == null)
                return null;
            AssemblyInfo assemblyInfo = new AssemblyInfo();
            assemblyInfo.Name = assembly.GetName().Name;
            assemblyInfo.IsDynamic = assembly.IsDynamic;
            assemblyInfo.FullName = assembly.FullName;
            if (!assembly.IsDynamic)
            {
                assemblyInfo.Location = assembly.Location;
                //assemblyInfo.CodeBase = assembly.CodeBase;
                assemblyInfo.ManifestResourceNames = assembly.GetManifestResourceNames().zToStringValues();
            }
            //assemblyInfo.ModulesNames = assembly.GetModules().Select(module => module.Name).zToStringValues();
            return assemblyInfo;
        }
    }

    public class AssemblyInfo
    {
        public string Name;
        public bool IsDynamic;
        public string FullName;
        public string Location;
        //public string CodeBase;
        public string ManifestResourceNames;
        //public string ModulesNames;
    }
}

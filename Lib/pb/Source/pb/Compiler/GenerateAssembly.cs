using System;
using pb.IO;

namespace pb.Compiler
{
    public class GenerateAssembly
    {
        //private static string __defaultGenerateAssemblySubdirectory = "run";
        //private static string __defaultGenerateAssemblyName = "RunSource";

        private string _generateAssemblyDirectory = null;         // c:\pib\prog\tools\runsource\exe\run
        private string _generateAssemblyName = null;              // RunSource_00001.cs RunSource_00001.dll RunSource_00001.pdb

        public string GenerateAssemblyDirectory { get { return _generateAssemblyDirectory; } set { _generateAssemblyDirectory = value; } }
        public string GenerateAssemblyName { get { return _generateAssemblyName; } set { _generateAssemblyName = value; } }

        public string GetNewAssemblyFile()
        {
            // "c:\pib\prog\tools\runsource\exe\run\RunSource_00001"
            if (!zDirectory.Exists(_generateAssemblyDirectory))
                zDirectory.CreateDirectory(_generateAssemblyDirectory);
            int i = zfile.GetLastFileNameIndex(_generateAssemblyDirectory) + 1;
            return zPath.Combine(_generateAssemblyDirectory, _generateAssemblyName + string.Format("_{0:00000}", i));
        }

        public void DeleteGeneratedAssemblies()
        {
            zfile.DeleteFiles(_generateAssemblyDirectory, _generateAssemblyName + "*.*", throwError: false);
        }
    }
}

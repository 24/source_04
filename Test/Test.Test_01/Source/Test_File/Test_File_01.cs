using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Test.Test_File
{
    public static class Test_File_01
    {
        public static void Test_FileAccessControl_01(string file)
        {
            FileAttributes attributes  = File.GetAttributes(file);
            //File.SetCreationTime
        }
    }
}

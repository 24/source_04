using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using pb;
using pb.Compiler;

namespace Test_directory
{
    // What's the best way to calculate the size of a directory in .NET? http://stackoverflow.com/questions/468119/whats-the-best-way-to-calculate-the-size-of-a-directory-in-net
    // Calculate the Size of a Folder/Directory using .NET 4.0 http://www.devcurry.com/2010/07/calculate-size-of-folderdirectory-using.html#.UjrLzxz9vp0
    // Get Size of file on disk http://stackoverflow.com/questions/3750590/get-size-of-file-on-disk

    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

    }
}

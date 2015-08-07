using System;
using System.Linq;
using System.IO;
using pb.IO;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;

namespace DirSize
{
    // Calculate the Size of a Folder/Directory using .NET 4.0 http://www.devcurry.com/2010/07/calculate-size-of-folderdirectory-using.html#.UjrLzxz9vp0

    class Program
    {
        static void Main(string[] args)
        {
            //DirectoryInfo dInfo = new DirectoryInfo(@"C:/Articles");
            var dInfo = zDirectory.CreateDirectoryInfo(@"C:/Articles");
            // set bool parameter to false if you
            // do not want to include subdirectories.
            long sizeOfDir = DirectorySize(dInfo, true);

            Console.WriteLine("Directory size in Bytes : " +
            "{0:N0} Bytes", sizeOfDir);
            Console.WriteLine("Directory size in KB : " +
            "{0:N2} KB", ((double)sizeOfDir) / 1024);
            Console.WriteLine("Directory size in MB : " +
            "{0:N2} MB", ((double)sizeOfDir) / (1024 * 1024));

            Console.ReadLine();
        }

        static long DirectorySize(DirectoryInfo dInfo, bool includeSubDir)
        {
            // Enumerate all the files
            long totalSize = dInfo.EnumerateFiles().Sum(file => file.Length);

            // If Subdirectories are to be included
            if (includeSubDir)
            {
                // Enumerate all sub-directories
                totalSize += dInfo.EnumerateDirectories().Sum(dir => DirectorySize(dir, true));
            }
            return totalSize;
        }
    }
}

using System;
using Pri.LongPath;

namespace Test_02
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Help();
                    return;
                }
                switch (args[0])
                {
                    case "dir":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for dir command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_GetDirectoryName_01(args[1]);
                        break;
                    case "dir2":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for dir2 command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_GetDirectoryName_02(args[1]);
                        break;
                    case "dir3":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for dir3 command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_GetDirectoryName_03(args[1]);
                        break;
                    case "norm":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for norm command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_NormalizeLongPath_01(args[1]);
                        break;
                    case "norm2":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for norm2 command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_NormalizeLongPath_02(args[1]);
                        break;
                    case "rootlen":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for rootlen command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_GetRootLength_01(args[1]);
                        break;
                    case "fullpath":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("missing path for fullpath command : \"{0}\"", args[0]);
                            Help();
                        }
                        else
                            Test_GetFullPath_01(args[1]);
                        break;
                    default:
                        Console.WriteLine("unknow command : \"{0}\"", args[0]);
                        Help();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void Help()
        {
            Console.WriteLine(@"Test_02 dir c:\toto\tata.txt");
            Console.WriteLine(@"Test_02 dir2 c:\toto\tata.txt");
            Console.WriteLine(@"Test_02 dir3 c:\toto\tata.txt");
            Console.WriteLine(@"Test_02 norm c:\toto\tata.txt");
            Console.WriteLine(@"Test_02 norm2 c:\toto\tata.txt");
            Console.WriteLine(@"Test_02 rootlen c:\toto\tata.txt");
            Console.WriteLine(@"Test_02 fullpath c:\toto\tata.txt");
        }

        public static void Test_GetDirectoryName_01(string path)
        {
            //string path = @"..\toto\tata\tutu.txt";
            Console.WriteLine("path : \"{0}\"", path);
            string dir = Path.GetDirectoryName(path);
            Console.WriteLine("Path.GetDirectoryName() : \"{0}\"", dir);
        }

        public static void Test_GetDirectoryName_02(string path)
        {
            //string path = @"..\toto\tata\tutu.txt";
            Console.WriteLine("path                                          : \"{0}\"", path);

            string dir = Path.GetDirectoryName(path);
            Console.WriteLine("Path.GetDirectoryName()                       : \"{0}\"", dir);

            string path2 = Path.NormalizeLongPath(path);
            Console.WriteLine("Path.NormalizeLongPath()                      : \"{0}\"", path2);
            path2 = Path.RemoveLongPathPrefix(path2);
            Console.WriteLine("Path.RemoveLongPathPrefix()                   : \"{0}\"", path2);

            int rootLength = Path.GetRootLength(path2);
            Console.WriteLine("Path.GetRootLength()                          : {0}", rootLength);

            int length = path2.Length;
            do
            {
            } while (length > rootLength && path2[--length] != System.IO.Path.DirectorySeparatorChar && path2[length] != System.IO.Path.AltDirectorySeparatorChar);
            Console.WriteLine("calculated length                             : {0} - \"{1}\"", length, path2.Substring(0, length));

            string basePath = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine("System.IO.Directory.GetCurrentDirectory()     : \"{0}\"", basePath);

            path2 = path2.Substring(basePath.Length + 1);
            Console.WriteLine("Substring(basePath.Length + 1)                : \"{0}\"", path2);

            length = length - basePath.Length - 1;
            Console.WriteLine("length - basePath.Length - 1                  : {0} - \"{1}\"", length, path2.Substring(0, length));
        }

        public static void Test_GetDirectoryName_03(string path)
        {
            Console.WriteLine("path                                          : \"{0}\"", path);

            string path2 = path;
            int rootLength = Path.GetRootLength(path2);
            Console.WriteLine("Path.GetRootLength()                          : {0}", rootLength);

            int length = path2.Length;
            do
            {
            } while (length > rootLength && path2[--length] != System.IO.Path.DirectorySeparatorChar && path2[length] != System.IO.Path.AltDirectorySeparatorChar);
            Console.WriteLine("calculated length                             : {0} - \"{1}\"", length, path2.Substring(0, length));
        }

        public static void Test_NormalizeLongPath_01(string path)
        {
            //string path = @"..\toto\tata\tutu.txt";
            Console.WriteLine("path : \"{0}\"", path);
            string normalizedPath = Path.NormalizeLongPath(path);
            Console.WriteLine("Path.NormalizeLongPath() : \"{0}\"", normalizedPath);
        }

        public static void Test_NormalizeLongPath_02(string path)
        {
            //string path = @"..\toto\tata\tutu.txt";
            Console.WriteLine("path : \"{0}\"", path);
            string normalizedPath = Path.NormalizeLongPath(path);
            normalizedPath = Path.RemoveLongPathPrefix(normalizedPath);
            Console.WriteLine("Path.RemoveLongPathPrefix(Path.NormalizeLongPath()) : \"{0}\"", normalizedPath);
        }

        public static void Test_GetRootLength_01(string path)
        {
            Console.WriteLine("path                                          : \"{0}\"", path);
            int rootLength = Path.GetRootLength(path);
            Console.WriteLine("Path.GetRootLength()                          : {0}", rootLength);
        }

        public static void Test_GetFullPath_01(string path)
        {
            Console.WriteLine("path                                          : \"{0}\"", path);
            string fullPath = Path.GetFullPath(path);
            Console.WriteLine("Path.GetFullPath()                            : \"{0}\"", fullPath);
        }
    }
}

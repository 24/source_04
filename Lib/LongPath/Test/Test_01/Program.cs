using System;
using Pri.LongPath;

namespace Test_01
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
            Console.WriteLine(@"Test_01 dir c:\toto\tata.txt");
        }

        public static void Test_GetDirectoryName_01(string path)
        {
            //string path = @"..\toto\tata\tutu.txt";
            Console.WriteLine("path : \"{0}\"", path);
            string dir = Path.GetDirectoryName(path);
            Console.WriteLine("Path.GetDirectoryName() : \"{0}\"", dir);
        }
    }
}

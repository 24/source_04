using System;

namespace Test.Test_Debug
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                Test_StackTrace_01();
                //Test_Exception_01();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void Test_01()
        {
            Console.WriteLine("toto");
        }

        public static void Test_Exception_01()
        {
            object o = null;
            o.ToString();
        }

        public static void Test_StackTrace_01()
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            //Trace.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);
            int i = 1;
            foreach (System.Diagnostics.StackFrame stackFrame in stackTrace.GetFrames())
            {
                Console.WriteLine("stack frame {0}", i++);
                Console.WriteLine("  Method               : {0}", stackFrame.GetMethod().Name);
                Console.WriteLine("  Method assembly      : {0}", stackFrame.GetMethod().Module.Assembly);
                Console.WriteLine("  Method module        : {0}", stackFrame.GetMethod().Module.Name);
                Console.WriteLine("  FileName             : {0}", stackFrame.GetFileName());
                Console.WriteLine("  LineNumber           : {0}", stackFrame.GetFileLineNumber());
                Console.WriteLine("  ColumnNumber         : {0}", stackFrame.GetFileColumnNumber());
                Console.WriteLine();
            }
        }
    }
}

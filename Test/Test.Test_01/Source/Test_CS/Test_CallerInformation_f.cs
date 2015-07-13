using System;
using System.Runtime.CompilerServices;
using pb;

namespace Test.Test_CS
{
    public static class Test_CallerInformation_f
    {
        // need .Net framework 4.5
        public static void Test_CallerInformation_01(string message,
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
        {
            Trace.WriteLine("message: " + message);
            Trace.WriteLine("member name: " + memberName);
            Trace.WriteLine("source file path: " + sourceFilePath);
            Trace.WriteLine("source line number: " + sourceLineNumber);
        }
    }
}

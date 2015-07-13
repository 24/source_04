using System;
using System.Diagnostics;

namespace Test.Test_Diagnostics
{
    public static class Test_Diagnostics_Trace
    {
        public static void Test_Diagnostics_Trace_01()
        {
            //ConsoleTraceListener myWriter = new ConsoleTraceListener();
            //Trace.Listeners.Add(myWriter);
            Trace.WriteLine("Test_Diagnostics_Trace_01");
            Test_TraceListener traceListener = new Test_TraceListener();
            pb.Trace.WriteLine("Test_TraceListener.Name : \"{0}\"", traceListener);
            //traceListener.Name = "Test_TraceListener";
            pb.Trace.WriteLine("Trace.Listeners.Add(traceListener)");
            Trace.Listeners.Add(traceListener);
            pb.Trace.WriteLine("Test_TraceListener.Name : \"{0}\"", traceListener);

            pb.Trace.WriteLine("begin test trace listener");
            Trace.WriteLine("test trace listener 01");
            Trace.WriteLine("test trace listener 02");
            Trace.WriteLine("test trace listener 03");
            Trace.WriteLine("test trace listener 04");
            pb.Trace.WriteLine("end test trace listener");

            pb.Trace.WriteLine("Trace.Listeners.Remove(traceListener)");
            Trace.Listeners.Remove(traceListener);

            pb.Trace.WriteLine("begin test trace listener");
            Trace.WriteLine("test trace listener 01");
            Trace.WriteLine("test trace listener 02");
            Trace.WriteLine("test trace listener 03");
            Trace.WriteLine("test trace listener 04");
            pb.Trace.WriteLine("end test trace listener");

        }
    }

    public class Test_TraceListener : TraceListener
    {
        public override void Write(string message)
        {
            pb.Trace.Write("Test_TraceListener : ");
            pb.Trace.Write(message);
        }

        public override void WriteLine(string message)
        {
            pb.Trace.Write("Test_TraceListener : ");
            pb.Trace.WriteLine(message);
        }
    }
}

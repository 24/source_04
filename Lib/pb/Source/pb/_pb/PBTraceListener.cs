using System.Diagnostics;
using System.Runtime.InteropServices;

namespace pb
{

    // trace level : 
    //   Off      : 0   Outputs no messages to Trace Listeners
    //   Error    : 1   Outputs only error messages to Trace Listeners
    //   Warning  : 2   Outputs error and warning messages to Trace Listeners
    //   Info     : 3   Outputs informational, warning and error messages to Trace Listeners
    //   Verbose  : 4   Outputs all messages to Trace Listeners

    public class PBTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            // from TraceListener.cs
            if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, message, null, null, null))
                return;

            pb.Trace.Write(message);
        }

        public override void WriteLine(string message)
        {
            // from TraceListener.cs
            if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, message, null, null, null))
                return;

            pb.Trace.WriteLine(message);
        }

        [ComVisible(false)]
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            // from TraceListener.cs
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
                return;

            //pb.Trace.Write("TraceEvent source \"{0}\" type {1} id {2} : ", source, eventType, id);
            pb.Trace.WriteLine(message);
        }

        [ComVisible(false)]
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            // from TraceListener.cs
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                return;

            //pb.Trace.Write("TraceEvent source \"{0}\" type {1} id {2} : ", source, eventType, id);
            pb.Trace.WriteLine(format, args);
        }
    }
}

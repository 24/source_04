using System;

namespace pb.Test
{
    public static class Test_CSharp
    {
        public static void Test_Action_01()
        {
            Trace.WriteLine("call 01");
            Test_Action action = new Test_Action();
            action.Call();
            // no output
            Trace.WriteLine();

            Trace.WriteLine("call 02");
            action = new Test_Action();
            action.Action = () => Trace.WriteLine("action 01");
            action.Call();
            Trace.WriteLine();
            // output :
            // action 01

            Trace.WriteLine("call 03");
            action = new Test_Action();
            action.Action += () => Trace.WriteLine("action 01");
            action.Action += () => Trace.WriteLine("action 02");
            action.Call();
            Trace.WriteLine();
            // output :
            // action 01
            // action 02

            Trace.WriteLine("call 04");
            action = new Test_Action();
            action.Action += () => Trace.WriteLine("action 01");
            action.Action += () => Trace.WriteLine("action 02");
            action.Action = () => Trace.WriteLine("action 03");
            action.Call();
            // output :
            // action 03
        }
    }

    public class Test_Action
    {
        public Action Action = null;

        public void Call()
        {
            Action?.Invoke();
        }
    }
}

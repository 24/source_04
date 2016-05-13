using pb;
using System;
using System.Collections.Generic;

namespace Test.Test_LambdaExpression
{
    public static class Test_LambdaExpression
    {
        public static void Test_01()
        {
            List<Action> actions = new List<Action>();
            for (int i = 0; i < 5; i++)
            {
                actions.Add(() => Trace.WriteLine("{0}", i));
            }
            actions[4]();
            actions[3]();
            actions[2]();
            actions[1]();
            actions[0]();
        }
    }
}

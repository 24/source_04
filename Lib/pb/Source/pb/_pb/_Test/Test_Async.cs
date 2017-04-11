using System;
using System.Threading.Tasks;

namespace pb.Test
{
    public static class Test_Async
    {
        public static async Task Test_CatchException()
        {
            try
            {
                //await Test_Exception("toto");
                await Test_Exception();
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        #pragma warning disable 1998
        public static async Task Test_Warning()
        #pragma warning restore 1998
        {

        }

        public static async Task Test_Try_01()
        {
            //await TryAsync(Test_Exception);                                    // ok
            //await TryAsync(async () => { await Test_Exception("toto"); });     // ok
            //await TryAsync(async () => await Test_Exception("toto"));          // ok
            await TryAsync(async () => await Test_Exception());          // ok
        }

        public static void Test_Try_02()
        {
            //TryAsync2(async () => await Test_Exception("toto"));            // ok
            //TryAsync2(async () => await Test_Exception());            // ok
            TryAsync2(Test_Exception);            // ok
        }

        public static async Task TryAsync(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction();
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        public static async void TryAsync2(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction();
            }
            catch (Exception ex)
            {
                Trace.WriteError(ex);
            }
        }

        //public static async Task Test_Exception(string message)
        public static async Task Test_Exception()
        {
            //Trace.WriteLine($"message : \"{message}\"");
            Trace.WriteLine("wait");
            await Task.Delay(3000);
            Trace.WriteLine("throw exception");
            throw new PBException("Test_Exception");
        }
    }
}

using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
//#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

namespace pb.NodeJS.Test
{
    public class Test_NodeJS_01
    {
        public async Task<object> Test_01(object input)
        {
            return input + " Test_NodeJS_01.Test_01()";
        }
    }
}

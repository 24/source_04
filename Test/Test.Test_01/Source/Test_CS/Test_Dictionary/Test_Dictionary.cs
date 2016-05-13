using System.Collections.Generic;

namespace Test.Test_Dictionary
{
    public class Test
    {
        public string Name;
    }

    public static class Test_Dictionary
    {
        public static void Test_01()
        {
            Dictionary<Test, Test> dictionary = new Dictionary<Test, Test>();
            Test test = new Test { Name = "toto" };
            dictionary.Add(test, test);
            test = new Test { Name = "tata" };
            dictionary.Add(test, test);
            dictionary.Add(test, test);
        }
    }
}

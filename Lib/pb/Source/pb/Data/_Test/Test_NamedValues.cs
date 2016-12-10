using pb.Data.Mongo;

namespace pb.Data.Test
{
    public static class Test_NamedValues
    {
        //Test_NamedValues.Test_ParseValues_01("bool = true, int = 123, double = 1.123, string1 = \"toto tata\", string2 = 'toto tata'", useLowercaseKey: true);
        //Test_NamedValues.Test_ParseValues_01("datetime1 = 01/01/2015 01:35:52.123, datetime2 = 2015-01-01 01:35:52.123, date1 = 01/01/2015, date2 = 2015-01-01, time = 1.01:35:52.1234567", useLowercaseKey: true);
        public static void Test_ParseValues_01(string parameters, bool useLowercaseKey = false)
        {
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters, useLowercaseKey: true);
            ParseNamedValues.ParseValues(parameters, useLowercaseKey: useLowercaseKey).zTraceJson();
        }
    }
}

using pb.Data;

namespace hts.WebData
{
    public static partial class WebData
    {
        public static NamedValues<ZValue> ParseParameters(string parameters)
        {
            return ParseNamedValues.ParseValues(parameters, useLowercaseKey: true);
        }

        public static bool GetTestValue(NamedValues<ZValue> parameters)
        {
            if (parameters.ContainsKey("test"))
                return (bool)parameters["test"];
            return false;
        }
    }
}

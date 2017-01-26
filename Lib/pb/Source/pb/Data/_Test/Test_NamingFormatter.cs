using CenterCLR;
using System;
using System.Collections.Generic;

namespace pb.Data.Test
{
    public static class Test_NamingFormatter
    {
        public static void Test_01()
        {
            // from https://github.com/kekyo/CenterCLR.NamingFormatter
            var keyValues = new Dictionary<string, object> { { "lastName", "Matsui" }, { "firstName", "Kouji" } };
            string format = "FirstName:{firstName}, LastName:{lastName}";
            var formatted = Named.Format(format, keyValues);
            Trace.WriteLine($"format    : \"{format}\"");
            Trace.WriteLine($"formatted : \"{formatted}\"");
        }

        public static void Test_02()
        {
            var keyValues = new Dictionary<string, object> { { "date", DateTime.Now.ToUniversalTime().ToString("o") } };
            string format = "{{ 'data.LoadFromWebDate': {{ $gt: ISODate('{date}') }} }}";
            var formatted = Named.Format(format, keyValues);
            Trace.WriteLine($"format    : \"{format}\"");
            Trace.WriteLine($"formatted : \"{formatted}\"");
        }
    }
}

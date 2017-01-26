using System;
using System.Collections.Generic;
using pb.Data.Mongo;

namespace pb.Reflection.Test
{
    public class Test_01
    {
        public string Test_01_Name;
        public int Test_01_Number;
        public string Test_01_Name2 { get; set; }
        public int Test_01_Number2 { get; set; }
    }

    public class Test_02
    {
        public string Test_02_Name;
        public int Test_02_Number;
        public string Test_02_Name2 { get; set; }
        public int Test_02_Number2 { get; set; }
        public Test_01 Test_02_Test_01;
        public int[] Test_02_Numbers;
    }

    public class Test_Company
    {
        public string Name;
        public int[] Numbers;
        public DateTime Date;
        public Test_Contact[] Contacts;
    }

    public class Test_Contact
    {
        public string Name;
        public int[] Numbers;
    }

    public static class Test_Data
    {
        public static IEnumerable<Test_Company> GetCompanies(string file)
        {
            //return BsonSerializer.Deserialize<Test_Company>("");
            return zMongo.BsonRead<Test_Company>(file);
        }

    }
}

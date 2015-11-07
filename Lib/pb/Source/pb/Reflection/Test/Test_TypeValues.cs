using System;
using System.Linq;

namespace pb.Reflection.Test
{
    public static class Test_TypeValues
    {
        public static void Test_TypeValues_01()
        {
            Trace.WriteLine("Test_TypeValues_01");
        }

        public static void Test_TypeValues_02(string file, params string[] values)
        {
            bool onlyNextValue = true;
            //bool onlyNextValue = false;

            TypeValues<Test_Company> typeValues = new TypeValues<Test_Company>();

            foreach (string value in values)
                typeValues.AddValue(value);

            int index = 1;
            foreach (Test_Company company in Test_Data.GetCompanies(file))
            {
                typeValues.SetData(company);
                Trace.WriteLine("row {0}", index++);

                foreach (string value in values)
                    Trace.Write("  {0,-20}", value);
                Trace.WriteLine();

                foreach (string value in values)
                    Trace.Write("  {0,-20}", typeValues.GetValue(value));
                Trace.WriteLine();

                while (typeValues.NextValues())
                {
                    foreach (string value in values)
                        Trace.Write("  {0,-20}", typeValues.GetValue(value, onlyNextValue: onlyNextValue));
                    Trace.WriteLine();
                }

                Trace.WriteLine();
            }
        }

        public static void Test_TypeValues_03(string file)
        {
            bool onlyNextValue = true;
            //bool onlyNextValue = false;

            TypeValues<Test_Company> typeValues = new TypeValues<Test_Company>();
            typeValues.AddAllValues(MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property);
            Test_TraceCompanies(file, typeValues, onlyNextValue);
        }

        public static void Test_TraceCompanies(string file, TypeValues<Test_Company> typeValues, bool onlyNextValue)
        {
            string[] values = typeValues.MemberValues.Values.Where(memberValue => memberValue.MemberAccess.IsValueType).Select(memberValue => memberValue.MemberAccess.TreeName).ToArray();
            int index = 1;
            foreach (Test_Company company in Test_Data.GetCompanies(file))
            {
                typeValues.SetData(company);
                Trace.WriteLine("row {0}", index++);

                foreach (string value in values)
                    Trace.Write("  {0,-20}", value);
                Trace.WriteLine();

                foreach (string value in values)
                    Trace.Write("  {0,-20}", typeValues.GetValue(value));
                Trace.WriteLine();

                while (typeValues.NextValues())
                {
                    foreach (string value in values)
                        Trace.Write("  {0,-20}", typeValues.GetValue(value, onlyNextValue: onlyNextValue));
                    Trace.WriteLine();
                }

                Trace.WriteLine();
            }
        }
    }
}

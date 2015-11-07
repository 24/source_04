using System;

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

            //string value1 = "Name";
            //string value2 = "Numbers";

            //string value1 = "Name";
            //string value2 = "Numbers";
            //string value3 = "Contacts.Name";
            //string value4 = "Contacts.Numbers";

            //typeValues.AddValue(value1);
            //typeValues.AddValue(value2);
            //typeValues.AddValue(value3);
            //typeValues.AddValue(value4);

            foreach (string value in values)
                typeValues.AddValue(value);

            int index = 1;
            foreach (Test_Company company in Test_Data.GetCompanies(file))
            {
                typeValues.SetData(company);
                Trace.WriteLine("row {0}", index++);

                //Trace.WriteLine("  {0,-20}  {1}", value1, value2);
                foreach (string value in values)
                    Trace.Write("  {0,-20}", value);
                Trace.WriteLine();

                //Trace.WriteLine("  {0,-20}  {1}", typeValues.GetValue(value1), typeValues.GetValue(value2));
                foreach (string value in values)
                    Trace.Write("  {0,-20}", typeValues.GetValue(value));
                Trace.WriteLine();

                //while (typeValues.NextValues())
                //    Trace.WriteLine("  {0,-20}  {1}", typeValues.GetValue(value1, onlyNextValue: onlyNextValue), typeValues.GetValue(value2, onlyNextValue: onlyNextValue));
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
        }
    }
}

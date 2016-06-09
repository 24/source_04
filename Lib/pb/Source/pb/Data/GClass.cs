namespace pb.Data
{
    public class GClass2<T1, T2>
    {
        public T1 Value1 = default(T1);
        public T2 Value2 = default(T2);

        public GClass2()
        {
        }

        public GClass2(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }

    public class GClass3<T1, T2, T3>
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;

        public GClass3()
        {
        }

        public GClass3(T1 value1, T2 value2, T3 value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }
    }

    public class GClass4<T1, T2, T3, T4>
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;

        public GClass4()
        {
        }

        public GClass4(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }
    }
}

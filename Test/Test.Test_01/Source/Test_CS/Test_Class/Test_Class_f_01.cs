using System;
using System.Collections.Generic;

namespace Test.Test_CS.Test_Class
{
    public static class Test_Class_f_01
    {
        public static void Test_01()
        {
            GoldenDdl_HeaderPage page = null;
            IEnumDataPages<GoldenDdl_PostHeader> enumDataPages = null;
            enumDataPages = page;

            GoldenDdl_LoadHeaderPageFromWebManager load = null;
            LoadDataFromWebManager<GoldenDdl_HeaderPage> load2 = null;
            load2 = load;
            //LoadDataFromWebManager<IEnumDataPages<GoldenDdl_PostHeader>> load3 = null;
            //load3 = load;
        }
    }

    public class GoldenDdl_PostHeader
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate;
        public string urlDetail;
    }

    public interface IEnumDataPages<T>
    {
        IEnumerable<T> GetDataList();
        string GetUrlNextPage();
    }

    public class GoldenDdl_HeaderPage : IEnumDataPages<GoldenDdl_PostHeader>
    {
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate;

        public GoldenDdl_PostHeader[] postHeaders;
        public string urlNextPage;

        public IEnumerable<GoldenDdl_PostHeader> GetDataList()
        {
            return postHeaders;
        }

        public string GetUrlNextPage()
        {
            return urlNextPage;
        }
    }

    public abstract class LoadDataFromWebManager<T>
    {
    }

    public abstract class LoadDataFromWebManager2<TData1, TData2> where TData1 : IEnumDataPages<TData2>
    {
    }

    public class GoldenDdl_LoadHeaderPageFromWebManager : LoadDataFromWebManager<GoldenDdl_HeaderPage>
    {
    }

    public abstract class LoadWebEnumDataPagesManager<TKey, TData1, TData2> : LoadDataFromWebManager<TData1> where TData1 : IEnumDataPages<TData2>
    {
    }

    public interface ITest_01
    {
    }

    public class Test_02
    {
    }

    public static class Test_01
    {
        public static void Test01<T>(T value)
        {
            ITest_01 ivalue = (ITest_01)value;
            // compile error : Cannot convert type 'T' to 'Test.Test_CS.Test_Class.Test_02'
            //Test_02 value2 = (Test_02)value;
            Test_02 value2 = value as Test_02;
        }
    }

}

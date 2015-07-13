using System;

namespace Test_AppDomain_05
{
    public interface itest_01
    {
        string GetMessage();
    }

    public interface itest_02
    {
        string GetCallingAssembly();
        string GetCallingAssemblyProduct();
        string GetEntryAssembly();
        string GetExecutingAssembly();
        string GetExecutingAssemblyProduct();
        string GetApplicationCompany();
    }
}

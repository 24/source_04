using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace pb.Reflection.Test
{
    // typeof(Test_01).zGetTypeValuesInfos(BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field | MemberTypes.Property).zToTraceValuesInfos().zTraceJson();
    //[{
    //    "Name" : "Name2",
    //    "ValueType" : "System.String",
    //    "IsValueType" : true,
    //    "DeclaringType" : "pb.Test.Test_01",
    //    "ReflectedType" : "pb.Test.Test_01",
    //    "MemberType" : "Property",
    //    "MetadataToken" : 385876005,
    //    "Module" : "RunCode_00007.dll"
    //  }, {
    //    "Name" : "Number2",
    //    "ValueType" : "System.Int32",
    //    "IsValueType" : true,
    //    "DeclaringType" : "pb.Test.Test_01",
    //    "ReflectedType" : "pb.Test.Test_01",
    //    "MemberType" : "Property",
    //    "MetadataToken" : 385876006,
    //    "Module" : "RunCode_00007.dll"
    //  }, {
    //    "Name" : "Name",
    //    "ValueType" : "System.String",
    //    "IsValueType" : true,
    //    "DeclaringType" : "pb.Test.Test_01",
    //    "ReflectedType" : "pb.Test.Test_01",
    //    "MemberType" : "Field",
    //    "MetadataToken" : 67108918,
    //    "Module" : "RunCode_00007.dll"
    //  }, {
    //    "Name" : "Number",
    //    "ValueType" : "System.Int32",
    //    "IsValueType" : true,
    //    "DeclaringType" : "pb.Test.Test_01",
    //    "ReflectedType" : "pb.Test.Test_01",
    //    "MemberType" : "Field",
    //    "MetadataToken" : 67108919,
    //    "Module" : "RunCode_00007.dll"
    //  }]

    public static class Test_Reflection
    {
        public static void Test_Reflection_01()
        {
            Trace.WriteLine("Test_Reflection_01");
        }

        public static void Test_Reflection_02()
        {
            Trace.WriteLine("Test_Reflection_01");
            Type type = typeof(Test_01);
            type.GetMembers();
            type.GetFields();
            //type.InvokeMember("", System.Reflection.BindingFlags.Public, null, )
            //System.Reflection.RtFieldInfo
        }

        public static void Test_Reflection_03()
        {
           //System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(string className);
            //Assembly.GetCallingAssembly().CreateInstance();
            //return (T)Activator.CreateInstance(typeof(T), new object[] { weight });
            //Activator.CreateInstance();
            //Type.GetType()
            //Type type = typeof(TypeValues<>);
            //type.MakeGenericType(typeof(Test_Company));
            //typeof(TypeValues<Test_Company>).zGetTypeName();
        }

        public static async Task Test_Reflection_04()
        {
            // from How to await an async private method invoked using reflection in WinRT? http://stackoverflow.com/questions/14711585/how-to-await-an-async-private-method-invoked-using-reflection-in-winrt
            Trace.WriteLine("Test_Reflection_04");

            Test_Async testAsync = new Test_Async();
            MethodInfo methodInfo = typeof(Test_Async).GetTypeInfo().GetDeclaredMethod("Test_01");
            Trace.WriteLine("Test_Reflection_04 : before call");
            await (Task)methodInfo.Invoke(testAsync, null);
            Trace.WriteLine("Test_Reflection_04 : after call");
        }
    }

    public class Test_Async
    {
        public async Task Test_01()
        {
            Trace.WriteLine("Test_Async.Test_01() : 01");
            await Task.Delay(5000);
            Trace.WriteLine("Test_Async.Test_01() : 02");
        }
    }

    public class Test_Yield
    {
        public IEnumerable<string> Test()
        {
            Trace.WriteLine("Test begin");
            yield return "toto";
            Trace.WriteLine("Test end");
        }
    }

    public static partial class GlobalExtension
    {
        //IEnumerable<ValueInfo>
        //public static 
    }
}

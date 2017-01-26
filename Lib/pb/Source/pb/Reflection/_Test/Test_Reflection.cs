﻿using System;
using System.Collections.Generic;

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
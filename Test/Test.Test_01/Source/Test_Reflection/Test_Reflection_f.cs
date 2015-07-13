using System;
using System.Collections.Generic;
using pb;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Test_Reflection
{
    public static class Test_Reflection_f
    {
        public static void Test_Reflection_01()
        {
            Trace.WriteLine("Test_Reflection_01");
        }

        public static void Test_GetCurrentMethod_01()
        {
            Trace.WriteLine("MethodBase.GetCurrentMethod().DeclaringType : \"{0}\"", MethodBase.GetCurrentMethod().DeclaringType.zGetTypeName());
        }

        public static void Test_GetCallerInfo_01()
        {
            Test_GetCallerInfo();
        }

        public static void Test_GetCallerInfo([CallerMemberName]string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Trace.WriteLine("[CallerMemberName] : \"{0}\"", callerMemberName);
            Trace.WriteLine("[CallerFilePath]   : \"{0}\"", callerFilePath);
            Trace.WriteLine("[CallerLineNumber] : {0}", callerLineNumber);
        }

        public static void Test_StackFrame_01()
        {
            Test_StackFrame();
        }

        public static void Test_StackFrame()
        {
            System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(skipFrames: 1, fNeedFileInfo: true);

            Trace.WriteLine("create StackFrame(skipFrames: 1, fNeedFileInfo: true) :");
            Trace.WriteLine("  GetMethod().DeclaringType : \"{0}\"", stackFrame.GetMethod().DeclaringType.zGetTypeName());
            Trace.WriteLine("  GetFileName()             : \"{0}\"", stackFrame.GetFileName());
            Trace.WriteLine("  GetFileLineNumber()       : {0}", stackFrame.GetFileLineNumber());
            Trace.WriteLine("  GetFileColumnNumber()     : {0}", stackFrame.GetFileColumnNumber());
        }

        public static void Test_StackTrace_01()
        {
            Test_StackTrace();
        }

        public static void Test_StackTrace()
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(fNeedFileInfo: true);
            Trace.WriteLine("create StackTrace(fNeedFileInfo: true) :");
            Trace.WriteLine("  FrameCount : {0}", stackTrace.FrameCount);
            foreach (System.Diagnostics.StackFrame stackFrame in stackTrace.GetFrames())
            {
                Trace.WriteLine("  Frame GetMethod().DeclaringType : \"{0}\"", stackFrame.GetMethod().DeclaringType.zGetTypeName());
                Trace.WriteLine("  Frame GetFileName()             : \"{0}\"", stackFrame.GetFileName());
                Trace.WriteLine("  Frame GetFileLineNumber()       : {0}", stackFrame.GetFileLineNumber());
                Trace.WriteLine("  FrameGetFileColumnNumber()      : {0}", stackFrame.GetFileColumnNumber());
            }
        }

        public static void Test_Type_01()
        {
            Trace.WriteLine();
            Trace.WriteLine("Test_Type_01");
            Test_01 test_01 = new Test_01();
            Trace.WriteLine("Type test_01.GetType() : {0} {1}", test_01.GetType().Name, test_01.GetType().FullName);
            ITest_01 itest_01 = test_01;
            Trace.WriteLine("Type itest_01.GetType() : {0} {1}", itest_01.GetType().Name, itest_01.GetType().FullName);
            Test_Type_01(test_01);
            Trace.WriteLine("Type typeof(Test_01) : {0} {1}", typeof(Test_01).Name, typeof(Test_01).FullName);
            Trace.WriteLine("Type typeof(ITest_01) : {0} {1}", typeof(ITest_01).Name, typeof(ITest_01).FullName);
            Trace.WriteLine("Type test_01.GetType() == typeof(Test_01) : {0}", test_01.GetType() == typeof(Test_01));
            Trace.WriteLine("Type test_01.GetType() == typeof(ITest_01) : {0}", test_01.GetType() == typeof(ITest_01));
            Trace.WriteLine("Type itest_01.GetType() == typeof(Test_01) : {0}", itest_01.GetType() == typeof(Test_01));
            Trace.WriteLine("Type itest_01.GetType() == typeof(ITest_01) : {0}", itest_01.GetType() == typeof(ITest_01));
            Trace.WriteLine("Type typeof(Test_01) == typeof(ITest_01) : {0}", typeof(Test_01) == typeof(ITest_01));
        }

        public static void Test_Type_01(ITest_01 itest_01)
        {
            Trace.WriteLine("Type itest_01 : {0} {1}", itest_01.GetType().Name, itest_01.GetType().FullName);
        }

        public static void Test_GetType_01(string typeName)
        {
            //Trace.WriteLine();
            //Trace.WriteLine("Test_GetType_01");
            // bug : Type.GetType("Test_Reflection.Test_01, RunSource_00001, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") : null
            Type type = Type.GetType(typeName);
            Trace.WriteLine("Type.GetType(\"{0}\") : {1}", typeName, type != null ? type.AssemblyQualifiedName : "null");
        }

        public static void Test_TraceType_01()
        {
            TraceTypeVerbose(typeof(string));
            TraceTypeVerbose(typeof(List<string>));
            TraceTypeVerbose(typeof(Dictionary<string, string>));
        }

        public static void TraceType(Type type)
        {
            _TraceType(type);
            Trace.WriteLine();
        }

        public static void TraceTypeVerbose(Type type)
        {
            _TraceType(type);
            _TraceTypeVerbose(type);
            Trace.WriteLine();
        }

        public static void _TraceType(Type type)
        {
            Trace.WriteLine("trace type :");
            //Trace.WriteLine("  zName()                             \"{0}\"", type.zName());
            Trace.WriteLine("  zGetName()                          \"{0}\"", type.zGetTypeName());
            Trace.WriteLine("  Name                                \"{0}\"", type.Name);
            Trace.WriteLine("  Namespace                           \"{0}\"", type.Namespace);
            Trace.WriteLine("  FullName                            \"{0}\"", type.FullName);
            Trace.WriteLine("  AssemblyQualifiedName               \"{0}\"", type.AssemblyQualifiedName);
            Trace.WriteLine("  Assembly                            \"{0}\"", type.Assembly.FullName);
            Trace.WriteLine("  Module                              \"{0}\"", type.Module.Name);
        }

        public static void _TraceTypeVerbose(Type type)
        {
            Trace.WriteLine("  DeclaringMethod                     {0}", type.IsGenericParameter ? type.DeclaringMethod.ToString() : "n/a");
            Trace.WriteLine("  DeclaringType                       \"{0}\"", type.DeclaringType != null ? type.DeclaringType.FullName : "n/a");
            Trace.WriteLine("  ReflectedType                       \"{0}\"", type.ReflectedType != null ? type.ReflectedType.FullName : "n/a");
            Trace.WriteLine("  UnderlyingSystemType                \"{0}\"", type.UnderlyingSystemType.FullName);
            Trace.WriteLine("  GUID                                {0}", type.GUID);
            Trace.WriteLine("  MetadataToken                       {0}", type.MetadataToken);
            Trace.WriteLine("  ContainsGenericParameters           {0}", type.ContainsGenericParameters);
            Trace.WriteLine("  IsAbstract                          {0}", type.IsAbstract);
            Trace.WriteLine("  IsAnsiClass                         {0}", type.IsAnsiClass);
            Trace.WriteLine("  IsArray                             {0}", type.IsArray);
            Trace.WriteLine("  IsAutoClass                         {0}", type.IsAutoClass);
            Trace.WriteLine("  IsAutoLayout                        {0}", type.IsAutoLayout);
            Trace.WriteLine("  IsByRef                             {0}", type.IsByRef);
            Trace.WriteLine("  IsClass                             {0}", type.IsClass);
            Trace.WriteLine("  IsCOMObject                         {0}", type.IsCOMObject);
            Trace.WriteLine("  IsContextful                        {0}", type.IsContextful);
            Trace.WriteLine("  IsEnum                              {0}", type.IsEnum);
            Trace.WriteLine("  IsExplicitLayout                    {0}", type.IsExplicitLayout);
            Trace.WriteLine("  IsGenericParameter                  {0}", type.IsGenericParameter);
            Trace.WriteLine("  IsGenericType                       {0}", type.IsGenericType);
            Trace.WriteLine("  IsGenericTypeDefinition             {0}", type.IsGenericTypeDefinition);
            Trace.WriteLine("  IsImport                            {0}", type.IsImport);
            Trace.WriteLine("  IsInterface                         {0}", type.IsInterface);
            Trace.WriteLine("  IsLayoutSequential                  {0}", type.IsLayoutSequential);
            Trace.WriteLine("  IsMarshalByRef                      {0}", type.IsMarshalByRef);
            Trace.WriteLine("  IsNested                            {0}", type.IsNested);
            Trace.WriteLine("  IsNestedAssembly                    {0}", type.IsNestedAssembly);
            Trace.WriteLine("  IsNestedFamANDAssem                 {0}", type.IsNestedFamANDAssem);
            Trace.WriteLine("  IsNestedFamily                      {0}", type.IsNestedFamily);
            Trace.WriteLine("  IsNestedFamORAssem                  {0}", type.IsNestedFamORAssem);
            Trace.WriteLine("  IsNestedPrivate                     {0}", type.IsNestedPrivate);
            Trace.WriteLine("  IsNestedPublic                      {0}", type.IsNestedPublic);
            Trace.WriteLine("  IsNotPublic                         {0}", type.IsNotPublic);
            Trace.WriteLine("  IsPointer                           {0}", type.IsPointer);
            Trace.WriteLine("  IsPrimitive                         {0}", type.IsPrimitive);
            Trace.WriteLine("  IsPublic                            {0}", type.IsPublic);
            Trace.WriteLine("  IsSealed                            {0}", type.IsSealed);
            Trace.WriteLine("  IsSecurityCritical                  {0}", type.IsSecurityCritical);
            Trace.WriteLine("  IsSecuritySafeCritical              {0}", type.IsSecuritySafeCritical);
            Trace.WriteLine("  IsSecurityTransparent               {0}", type.IsSecurityTransparent);
            Trace.WriteLine("  IsSerializable                      {0}", type.IsSerializable);
            Trace.WriteLine("  IsSpecialName                       {0}", type.IsSpecialName);
            Trace.WriteLine("  IsUnicodeClass                      {0}", type.IsUnicodeClass);
            Trace.WriteLine("  IsValueType                         {0}", type.IsValueType);
            Trace.WriteLine("  IsVisible                           {0}", type.IsVisible);
        }

        public static void Test_Assembly_01()
        {
            Trace.WriteLine("Test_Assembly_01");
            Assembly exe = Assembly.GetExecutingAssembly();
            if (exe == null)
                Trace.WriteLine("Assembly.GetExecutingAssembly() : not found");
            else
                Trace.WriteLine("Assembly.GetExecutingAssembly() : FullName \"{0}\" Location \"{1}\"", exe.FullName, exe.Location);
            Assembly entry = Assembly.GetEntryAssembly();
            if (entry == null)
                Trace.WriteLine("Assembly.GetEntryAssembly() : not found");
            else
                Trace.WriteLine("Assembly.GetEntryAssembly() : FullName \"{0}\" Location \"{1}\"", entry.FullName, entry.Location);
        }

        public static void Test_Assembly_02()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                Trace.WriteLine("Assembly.GetEntryAssembly() : not found");
            else
                Trace.WriteLine("Assembly.GetEntryAssembly() : FullName \"{0}\" Location \"{1}\"", assembly.FullName, assembly.Location);
            Trace.WriteLine("assembly.GetManifestResourceNames() : {0}", assembly.GetManifestResourceNames().zToStringValues());
            //assembly.GetTypes()
            //Type type = null;
            //type.zGetTypeName();
        }
    }

        public interface ITest_01
    {
        void Function_01();
    }

    public class Test_01 : ITest_01
    {
        public void Function_01()
        {
        }
    }
}

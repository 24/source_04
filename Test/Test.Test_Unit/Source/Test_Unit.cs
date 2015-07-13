Test_Unit_UrlToFileName.Test(_dataDir);
_rs.View(Test_Unit_UrlToFileName.ReadFile(Path.Combine(_dataDir, @"UrlToFileName\UrlToFileName_01.txt")));
Trace.WriteLine("toto");


Test_01();
Test_ViewUrlToFileNameList_01();
Test_MongoToJson_01();
Test_MongoDeserialize_01();

Test_Http.Test_Uri.Test();

Test_FastJson.Test_FastJson.Test_FastJsonToJson_01();
Test_FastJson.Test_FastJson.Test_FastJsonToObject_01();
Test_FastJson.Test_FastJson.Test_FastJsonToJsonToObject_01();
Test_FastJson.Test_FastJson.Test_FastJson_01();
Test_FastJson.Test_FastJson.Test_FastJson_02();
Test_FastJson.Test_FastJson.Test_FastJson_Test_01_01();
Test_FastJson.Test_FastJson.Test_FastJson_Test_01_02();
Test_FastJson.Test_FastJson.Test_FastJson_GClass_01();
Test_FastJson.Test_FastJson.Test_Type_01();
Test_FastJson.Test_FastJson.Test_FastJson_NameValueCollection_01();

Test_Bson.Test_Bson_f.Test_Serialize_NameValueCollection_01();
Test_Bson.Test_Bson_f.Test_Serialize_NameValueCollection_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Dynamic);
Test_Bson.Test_Bson_f.Test_Serialize_NameValueCollection_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document);
Test_Bson.Test_Bson_f.Test_Serialize_NameValueCollection_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfDocuments);
Test_Bson.Test_Bson_f.Test_Serialize_NameValueCollection_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays);
Test_Bson.Test_Bson_f.Test_Serialize_Dictionary_01();
Test_Bson.Test_Bson_f.Test_Serialize_Dictionary_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Dynamic);
Test_Bson.Test_Bson_f.Test_Serialize_Dictionary_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document);
Test_Bson.Test_Bson_f.Test_Serialize_Dictionary_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfDocuments);
Test_Bson.Test_Bson_f.Test_Serialize_Dictionary_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays);
Test_Bson.Test_Bson_f.Test_NameValueCollection_01();
Test_Bson.Test_Bson_f.Test_LookupSerializer_01(typeof(System.Collections.Specialized.NameValueCollection));
Test_Bson.Test_Bson_f.Test_ViewSerializationProviders_01();
Test_Bson.Test_Bson_f.Test_RegisterBsonPBSerializationProvider_01();

Test_Reflection.Test_Reflection_f.Test_Reflection_01();
Test_Reflection.Test_Reflection_f.Test_Type_01();

Test_Reflection.Test_Reflection_f.Test_GetType_01("Test_01");
Test_Reflection.Test_Reflection_f.Test_GetType_01("ITest_01");
Test_Reflection.Test_Reflection_f.Test_GetType_01("int");
Test_Reflection.Test_Reflection_f.Test_GetType_01(typeof(int).AssemblyQualifiedName);
Test_Reflection.Test_Reflection_f.Test_GetType_01(typeof(Test_Reflection.Test_01).AssemblyQualifiedName);
Test_Reflection.Test_Reflection_f.Test_GetType_01("");
Test_Reflection.Test_Reflection_f.Test_GetType_01("");

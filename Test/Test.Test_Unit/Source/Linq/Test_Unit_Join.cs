using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Linq;

namespace Test.Test_Unit.Linq
{
    public static class Test_Unit_Join
    {
        public static void Test()
        {
            Test_Join_01("Products.txt", "Categories.txt", "CategoryId", "_id", "Products_Categories.txt");
        }

        public static void Test_Join_01(string file1, string file2, string key1, string key2, string resultFile)
        {
            string dir = GetDirectory();
            file1 = Path.Combine(dir, file1);
            file2 = Path.Combine(dir, file2);
            resultFile = Path.Combine(dir, resultFile);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_InnerJoin"), JoinType.InnerJoin);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_LeftOuterJoin"), JoinType.LeftOuterJoin);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_RightOuterJoin"), JoinType.RightOuterJoin);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_FullOuterJoin"), JoinType.FullOuterJoin);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_LeftOuterJoinWithoutInner"), JoinType.LeftOuterJoinWithoutInner);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_RightOuterJoinWithoutInner"), JoinType.RightOuterJoinWithoutInner);
            Test_Join(file1, file2, key1, key2, zpath.PathSetFileName(resultFile, Path.GetFileNameWithoutExtension(resultFile) + "_FullOuterJoinWithoutInner"), JoinType.FullOuterJoinWithoutInner);
        }

        public static void Test_Join(string file1, string file2, string key1, string key2, string resultFile, JoinType joinType)
        {
            IEnumerable<BsonDocument> query =
                zmongo.BsonReader<BsonDocument>(file1).zJoin(
                    zmongo.BsonReader<BsonDocument>(file2),
                    document1 => document1[key1],
                    document2 => document2[key2],
                    GetResultSelector(),
                    joinType);
            //query.zSaveToJsonFile(resultFile);
            query.zSave(resultFile);
        }

        private static Func<BsonDocument, BsonDocument, BsonDocument> GetResultSelector()
        {
            return (document1, document2) => new BsonDocument { { "product", document1 ?? new BsonDocument() }, { "category", document2 ?? new BsonDocument() } };
        }

        public static void ViewFile(string file)
        {
            string dir = GetDirectory();
            RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_v2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(Path.Combine(dir, file))));
        }

        public static void CreateBasicFiles()
        {
            string dir = GetDirectory();
            //GetProducts().zToBsonDocuments().zSaveToJsonFile(Path.Combine(dir, "Products.txt"));
            GetProducts().zToBsonDocuments().zSave(Path.Combine(dir, "Products.txt"));
            //GetCategories().zToBsonDocuments().zSaveToJsonFile(Path.Combine(dir, "Categories.txt"));
            GetCategories().zToBsonDocuments().zSave(Path.Combine(dir, "Categories.txt"));
        }

        public static Product[] GetProducts()
        {
            return new Product[]
                {
                    new Product { Id = 1, Name = "le monde", CategoryId = 2 },
                    new Product { Id = 2, Name = "le figaro", CategoryId = 2 },
                    new Product { Id = 3, Name = "libération", CategoryId = 2 },
                    new Product { Id = 4, Name = "le point", CategoryId = 3 },
                    new Product { Id = 5, Name = "l'espress", CategoryId = 3 },
                    new Product { Id = 6, Name = "marianne", CategoryId = 3 },
                    new Product { Id = 7, Name = "la bible", CategoryId = 1 },
                    new Product { Id = 8, Name = "atlas", CategoryId = 1 },
                    new Product { Id = 9, Name = "histoire", CategoryId = 1 },
                    new Product { Id = 10, Name = "astérix", CategoryId = null },
                    new Product { Id = 11, Name = "alix", CategoryId = null },
                    new Product { Id = 12, Name = "tintin", CategoryId = null }
                };
        }

        public static Category[] GetCategories()
        {
            return new Category[]
                {
                    new Category { Id = 1, Name = "livre" },
                    new Category { Id = 2, Name = "journal" },
                    new Category { Id = 3, Name = "magazine" },
                    new Category { Id = 4, Name = "film" },
                    new Category { Id = 5, Name = "music" },
                    new Category { Id = 6, Name = "image" }
                };
        }

        private static string GetDirectory()
        {
            return Path.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Linq\Join");
        }
    }

    public class Category
    {
        public int Id;
        public string Name;
    }

    public class Product
    {
        public int Id;
        public string Name;
        public int? CategoryId;
    }
}

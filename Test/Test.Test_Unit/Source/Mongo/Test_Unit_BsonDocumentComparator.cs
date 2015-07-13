using System;
using System.IO;
using System.Linq;
using pb.Linq;
using pb.Data.Mongo;
using MongoDB.Bson;

namespace Test.Test_Unit.Mongo
{
    public static class Test_Unit_BsonDocumentComparator
    {
        public static void Test()
        {
            string dir = GetDirectory();
            //Func<BsonDocument, BsonDocument, BsonDocument> resultSelector = (product1, product2) => new BsonDocument { { "product1", product1 ?? new BsonDocument() }, { "product2", product2 ?? new BsonDocument() } };
            Test_Compare_01(Path.Combine(dir, "Products1.txt"), Path.Combine(dir, "Products2.txt"), "_id", "_id", Path.Combine(dir, "Products_Compare_InnerJoin.txt"), JoinType.InnerJoin);
        }

        // Func<BsonDocument, BsonDocument, BsonDocument> resultSelector = null
        public static void Test_Compare_01(string file1, string file2, string key1, string key2, string resultFile, JoinType joinType = JoinType.InnerJoin)
        {
            // .zSaveToJsonFile(resultFile);
            BsonDocumentComparator.CompareBsonDocumentFilesWithKey(file1, file2, key1, key2, joinType).Select(result => result.GetResultDocument()).zSave(resultFile);
        }

        public static void ViewFile(string file)
        {
            string dir = GetDirectory();
            RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_v2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(Path.Combine(dir, file))));
        }

        public static void CreateBasicFiles()
        {
            string dir = GetDirectory();
            // .zSaveToJsonFile
            GetProducts1().zToBsonDocuments().zSave(Path.Combine(dir, "Products1.txt"));
            // .zSaveToJsonFile
            GetProducts2().zToBsonDocuments().zSave(Path.Combine(dir, "Products2.txt"));
            //GetCategories().zToBsonDocuments().zSave(Path.Combine(dir, "Categories.txt"));
        }

        public static Product[] GetProducts1()
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

        public static Product[] GetProducts2()
        {
            return new Product[]
                {
                    new Product { Id = 4, Name = "le point", CategoryId = 3 },
                    new Product { Id = 5, Name = "l'espress", CategoryId = 3 },
                    new Product { Id = 6, Name = "marianne", CategoryId = 3 },
                    new Product { Id = 7, Name = "la bible1", CategoryId = 1 },
                    new Product { Id = 8, Name = "atlas1", CategoryId = 1 },
                    new Product { Id = 9, Name = "histoire", CategoryId = 1 },
                    new Product { Id = 10, Name = "astérix", CategoryId = 4 },
                    new Product { Id = 11, Name = "alix", CategoryId = 4 },
                    new Product { Id = 12, Name = "tintin", CategoryId = 4 },
                    new Product { Id = 13, Name = "mediapart", CategoryId = 2 },
                    new Product { Id = 14, Name = "science et vie", CategoryId = 3 },
                    new Product { Id = 15, Name = "roman", CategoryId = 1 },
                    new Product { Id = 16, Name = "picsou", CategoryId = null }
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
            return Path.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Mongo\BsonDocumentComparator");
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

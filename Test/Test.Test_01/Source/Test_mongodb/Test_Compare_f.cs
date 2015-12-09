using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Linq;

namespace Test.Test_mongodb
{
    public class Test_Compare_f
    {
        public static void Test_Compare_01(string file1, string file2)
        {
            BsonDocumentComparator.CompareBsonDocumentFiles(file1, file2);
        }

        public static void Test_Compare_02()
        {
            string file1 = @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\FindPrint_out_bson.txt";
            string file2 = @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\_archive\FindPrint_v1_SelectPost_02\FindPrint_out_bson.txt";
            string resultFile = @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\test_compare.txt";
            BsonDocumentComparator.CompareBsonDocumentFilesWithKey(file1, file2, "post_title", "post_title", joinType: pb.Linq.JoinType.InnerJoin,
                // "remainText", "warnings"
                //elementsToCompare: new string[] { "category", "file", "print", "title" }).zSaveToJsonFile(resultFile);
                elementsToCompare: new string[] { "findPrint_file" }, comparatorOptions: BsonDocumentComparatorOptions.ReturnNotEqualDocuments)
                //.zSaveToJsonFile(resultFile);
                .Select(result => result.GetResultDocument()).zSave(resultFile);
            RunSource.CurrentRunSource.SetResult(zmongo.FileReader<BsonDocument>(resultFile)
                //.Where(doc => !(doc["result"]["findPrint_file"]["value1"] is BsonNull) && doc["result"]["result"].AsString != "equal")
                .Where(doc => !(doc["result"]["findPrint_file"]["value1"] is BsonNull))
                .Select(doc => new BsonDocument { { "result", doc["result"] } }).zToDataTable2_old());
        }

        public static void Test_Compare_Document_01(string file1, string file2, string resultFile)
        {
            BsonDocument document1 = zmongo.ReadFileAs<BsonDocument>(file1);
            BsonDocument document2 = zmongo.ReadFileAs<BsonDocument>(file2);
            BsonDocumentComparator.CompareBsonDocuments(document1, document2).zSave(resultFile);
        }

        public static void Test_InnerJoin_01(string file1, string file2, string fileResult, Func<BsonDocument, bool> where = null)
        {
            //IEnumerable<BsonDocument> query =
            //    from document1 in zmongo.BsonReader<BsonDocument>(file1)
            //    join document2 in zmongo.BsonReader<BsonDocument>(file2) on document1["postTitle"] equals document2["postTitle"]
            //    select new BsonDocument { { "document1", document1 }, { "document2", document2 } };
            //query.zSaveToJsonFile(fileResult);

            //IEnumerable<BsonDocument> query =
            //    zmongo.BsonReader<BsonDocument>(file1).zFullOuterJoin(
            //        zmongo.BsonReader<BsonDocument>(file2),
            //        document1 => document1["postTitle"],
            //        document2 => document2["postTitle"],
            //        (document1, document2) => new BsonDocument { { "document1", document1 }, { "document2", document2 } });
            IEnumerable<BsonDocument> query =
                zmongo.FileReader<BsonDocument>(file1).zJoin(
                    zmongo.FileReader<BsonDocument>(file2),
                    document1 => document1["postTitle"],
                    document2 => document2["postTitle"],
                    (document1, document2) => new BsonDocument { { "document1", document1 }, { "document2", document2 } },
                    JoinType.FullOuterJoin);

            if (where != null)
                query = query.Where(where);
            //query.zSaveToJsonFile(fileResult);
            query.zSave(fileResult);

            //IEnumerable<BsonDocument> documents1 = null;
            //IEnumerable<BsonDocument> query =
            //    documents1.Join(
            //        categories,
            //        product => product.CategoryId,
            //        category => category.Id,
            //        (product, category) => new ProductCategory { ProductId = product.Id, ProductName = product.Name, CategoryId = category.Id, CategoryName = category.Name }
            //        );
        }

    }
}

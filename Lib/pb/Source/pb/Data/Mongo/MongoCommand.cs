using System;
using System.IO;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using pb.IO;

namespace pb.Data.Mongo
{
    public static class MongoCommand
    {
        public static MongoDatabase GetDatabase(string server, string databaseName)
        {
            if (server == null)
                server = "mongodb://localhost";
            MongoClient mclient = new MongoClient(server);
            MongoServer mserver = mclient.GetServer();
            return mserver.GetDatabase(databaseName);
        }

        public static BsonValue Eval(string code, string databaseName = "admin", string server = null)
        {
            //if (code.StartsWith("{") && code.EndsWith("}"))
            //    code = "db.runCommand( " + code + " )";
            //EvalArgs evalArgs = new EvalArgs { Code = new BsonJavaScript(code) };
            return GetDatabase(server, databaseName).zEval(code.zToEvalArgs());
        }

        public static TDocument FindOneById<TDocument>(string databaseName, string collectionName, object key, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zFindOneById<TDocument>(BsonValue.Create(key));
        }

        public static MongoCursor<BsonDocument> Find(string databaseName, string collectionName, string query, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            return Find<BsonDocument>(databaseName, collectionName, query, sort, fields, limit, options, server);
        }

        public static MongoCursor<TDocument> Find<TDocument>(string databaseName, string collectionName, string query, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zFind<TDocument>(query.zToQueryDocument(), sort.zToSortByWrapper(), fields.zToFieldsWrapper(),
                limit, options.zDeserializeToBsonDocument());
        }

        public static MongoCursor<TDocument> FindAll<TDocument>(string databaseName, string collectionName, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zFindAll<TDocument>(sort.zToSortByWrapper(), fields.zToFieldsWrapper(), limit, options.zDeserializeToBsonDocument());
        }

        public static FindAndModifyResult FindAndModify(string databaseName, string collectionName, string query, string update, bool upsert = false, string sort = null, string fields = null, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zFindAndModify(query.zToQueryDocument(), update.zToUpdateDocument(), upsert, sort.zToSortByWrapper(), fields.zToFieldsWrapper());
        }

        public static WriteConcernResult Insert<TDocument>(string databaseName, string collectionName, TDocument document, MongoInsertOptions options = null, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zInsert(document, options);
        }

        public static long Count(string databaseName, string collectionName, string query = null, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zCount(query.zToQueryDocument());
        }

        public static WriteConcernResult Update(string databaseName, string collectionName, string query, string update, UpdateFlags flags = UpdateFlags.None, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zUpdate(query.zToQueryDocument(), update.zToUpdateDocument(), flags);
        }

        public static void UpdateDocuments(string databaseName, string collectionName, string query, Action<BsonDocument> updateDocument, string sort = null, int limit = 0, string options = null, string server = null)
        {
            MongoCollection collection = GetDatabase(server, databaseName).GetCollection(collectionName);
            QueryDocument queryDoc = query.zToQueryDocument();
            SortByWrapper sortWrapper = sort.zToSortByWrapper();
            BsonDocument optionsDoc = options.zDeserializeToBsonDocument();
            MongoLog.CurrentMongoLog.LogUpdateDocuments(collection, queryDoc, sortWrapper, limit, optionsDoc);
            foreach (BsonDocument document in collection.zFind<BsonDocument>(queryDoc, sort: sortWrapper, limit: limit, options: optionsDoc))
            {
                updateDocument(document);
                collection.zSave(document);
            }
        }

        public static WriteConcernResult Remove(string databaseName, string collectionName, string query, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zRemove(query.zToQueryDocument());
        }

        public static WriteConcernResult RemoveAll(string databaseName, string collectionName, string server = null)
        {
            return GetDatabase(server, databaseName).GetCollection(collectionName).zRemoveAll();
        }

        public static CommandResult RenameCollection(string databaseName, string collectionName, string newCollectionName, string server = null)
        {
            return GetDatabase(server, databaseName).zRenameCollection(collectionName, newCollectionName);
        }

        public static void Export(string databaseName, string collectionName, string file, string query = null, string sort = null, string fields = null, int limit = 0, string options = null,
            Func<BsonDocument, BsonDocument> transformDocument = null, string server = null)
        {
            Export(GetDatabase(server, databaseName).GetCollection(collectionName), file, query, sort, fields, limit, options, transformDocument);
        }

        public static void Export(MongoCollection collection, string file, string query = null, string sort = null, string fields = null, int limit = 0, string options = null,
            Func<BsonDocument, BsonDocument> transformDocument = null)
        {
            QueryDocument queryDoc = query.zToQueryDocument();
            SortByWrapper sortWrapper = sort.zToSortByWrapper();
            FieldsWrapper fieldsWrapper = fields.zToFieldsWrapper();
            BsonDocument optionsDoc = options.zDeserializeToBsonDocument();
            MongoLog.CurrentMongoLog.LogExport(collection, file, queryDoc, sortWrapper, fieldsWrapper, limit, optionsDoc);
            zfile.CreateFileDirectory(file);
            //FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            FileStream fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            //StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            // no bom with new UTF8Encoding()
            StreamWriter sw = new StreamWriter(fs, new UTF8Encoding());
            try
            {
                foreach (BsonDocument document in collection.zFind<BsonDocument>(queryDoc, sort: sortWrapper, fields: fieldsWrapper, limit: limit, options: optionsDoc))
                {
                    BsonDocument document2 = document;
                    if (transformDocument != null)
                        document2 = transformDocument(document2);
                    sw.WriteLine(document2.zToJson());
                }
            }
            finally
            {
                sw.Close();
                fs.Close();
            }
        }

        public static void Import(string databaseName, string collectionName, string file, string server = null, MongoInsertOptions options = null)
        {
            MongoCollection collection = GetDatabase(server, databaseName).GetCollection(collectionName);
            MongoLog.CurrentMongoLog.LogImport(collection, file);
            //foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
            //{
            //    collection.zSave(document);
            //}
            zMongo.BsonRead<BsonDocument>(file).zSave(collection, options);
        }
    }

    //IEnumerable<BsonDocument> Aggregate(AggregateArgs args);
    //AggregateResult Aggregate(IEnumerable<BsonDocument> pipeline);
    //CommandResult AggregateExplain(AggregateArgs args);
    //WriteConcernResult CreateIndex(IMongoIndexKeys keys, IMongoIndexOptions options);
    //IEnumerable<TValue> Distinct<TValue>(string key, IMongoQuery query);
    //CommandResult Drop();
    //CommandResult DropAllIndexes();
    //CommandResult DropIndex(IMongoIndexKeys keys);
    //CommandResult DropIndexByName(string indexName);
    //EnsureIndex(IMongoIndexKeys keys, IMongoIndexOptions options);
    //bool Exists();
    //FindAndModifyResult FindAndRemove(IMongoQuery query, IMongoSortBy sortBy);
    //TDocument FindOneAs<TDocument>(IMongoQuery query);
    //TDocument FindOneByIdAs<TDocument>(BsonValue id);
    //GetIndexesResult GetIndexes();
    //CollectionStatsResult GetStats(GetStatsArgs args);
    //long GetTotalDataSize();
    //long GetTotalStorageSize();
    //IEnumerable<BsonDocument> Group(IMongoQuery query, IMongoGroupBy keys, BsonDocument initial, BsonJavaScript reduce, BsonJavaScript finalize);
    //bool IndexExists(IMongoIndexKeys keys);
    //bool IndexExistsByName(string indexName);
    //IEnumerable<WriteConcernResult> InsertBatch<TNominalType>(IEnumerable<TNominalType> documents, MongoInsertOptions options);
    //bool IsCapped();
    //MapReduceResult MapReduce(IMongoQuery query, BsonJavaScript map, BsonJavaScript reduce, IMongoMapReduceOptions options);
    //ReadOnlyCollection<IEnumerator<TDocument>> ParallelScanAs<TDocument>(ParallelScanArgs args);
    //CommandResult ReIndex();
    //WriteConcernResult Save<TNominalType>(TNominalType document, MongoInsertOptions options);
    //ValidateCollectionResult Validate(ValidateCollectionArgs args);

    public static class TraceMongoCommand
    {
        //public static bool ResultToGrid = false;
        //public static bool ResultToText = false;

        public static void Eval(string code, string databaseName = "admin", string server = null)
        {
            MongoDatabase database = MongoCommand.GetDatabase(server, databaseName);
            //if (code.StartsWith("{") && code.EndsWith("}"))
            //    code = "db.runCommand( " + code + " )";
            //EvalArgs evalArgs = new EvalArgs { Code = new BsonJavaScript(code) };
            EvalArgs evalArgs = code.zToEvalArgs();
            Trace.WriteLine("Eval : {0} {1}", database.zGetFullName(), evalArgs.ToJson());
            // obsolete : 'MongoDB.Driver.MongoDatabase.Eval(MongoDB.Bson.BsonJavaScript, params object[])' is obsolete: 'Use the overload of Eval that has an EvalArgs parameter instead.'
            // BsonValue Eval(BsonJavaScript code, params object[] args);
            BsonValue result = database.zEval(evalArgs);
            Trace.WriteLine(result.zToJson());
        }

        public static void Count(string databaseName, string collectionName, string query = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            QueryDocument queryDoc = query.zToQueryDocument();
            Trace.Write("Count : {0}", collection.zGetFullName());
            if (queryDoc != null)
            {
                Trace.Write(" query ");
                Trace.Write(queryDoc.ToJson());
            }
            long count = collection.zCount(queryDoc);
            Trace.WriteLine(" count {0}", count);
        }

        public static void FindOneById<TDocument>(string databaseName, string collectionName, object key, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            BsonValue bsonKey = BsonValue.Create(key);
            Trace.Write("FindOneByIdAs : {0} key ", collection.zGetFullName());
            Trace.Write(bsonKey.ToJson());
            Trace.Write(" type {0}", typeof(TDocument));
            TDocument result = collection.zFindOneById<TDocument>(bsonKey);
            Trace.WriteLine(result.zToJson());
        }

        public static MongoCursor<BsonDocument> Find(string databaseName, string collectionName, string query, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            //MongoCursor<BsonDocument> cursor = Find<BsonDocument>(databaseName, collectionName, query, sort, fields, limit, options, server);
            //if (ResultToGrid)
            //{
            //    RunSource.CurrentRunSource.SetResult(cursor.zToDataTable2());
            //}
            return Find<BsonDocument>(databaseName, collectionName, query, sort, fields, limit, options, server);
        }

        public static MongoCursor<TDocument> Find<TDocument>(string databaseName, string collectionName, string query, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);

            Trace.Write("FindAs : {0} query ", collection.zGetFullName());

            QueryDocument queryDoc = query.zToQueryDocument();
            if (queryDoc != null)
                Trace.Write(queryDoc.ToJson());
            else
                Trace.Write("null");

            SortByWrapper sortByWrapper = null;
            if (sort != null)
            {
                sortByWrapper = sort.zToSortByWrapper();
                Trace.Write(" sort {0}", sortByWrapper.ToJson());
            }

            FieldsWrapper fieldsWrapper = null;
            if (fields != null)
            {
                fieldsWrapper = fields.zToFieldsWrapper();
                Trace.Write(" fields {0}", fieldsWrapper.ToJson());
            }

            if (limit != 0)
            {
                Trace.Write(" limit {0}", limit);
            }

            BsonDocument optionsDoc = null;
            if (options != null)
            {
                optionsDoc = options.zDeserializeToBsonDocument();
                Trace.Write(" options {0}", optionsDoc.ToJson());
            }

            Trace.WriteLine();

            MongoCursor<TDocument> cursor = collection.zFind<TDocument>(queryDoc, sortByWrapper, fieldsWrapper, limit, optionsDoc);

            //if (ResultToText)
            //{
            //    int i = 0;
            //    foreach (TDocument value in cursor)
            //    {
            //        Trace.WriteLine(value.zToJson(true));
            //        i++;
            //    }
            //    Trace.WriteLine("found {0} document(s)", i);
            //}
            return cursor;
        }

        public static MongoCursor<BsonDocument> FindAll(string databaseName, string collectionName, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            //MongoCursor<BsonDocument> cursor = FindAll<BsonDocument>(databaseName, collectionName, sort, fields, limit, options, server);
            //if (ResultToGrid)
            //{
            //    RunSource.CurrentRunSource.SetResult(cursor.zToDataTable2());
            //}
            return FindAll<BsonDocument>(databaseName, collectionName, sort, fields, limit, options, server); ;
        }

        public static MongoCursor<TDocument> FindAll<TDocument>(string databaseName, string collectionName, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);

            Trace.Write("FindAllAs : {0}", collection.zGetFullName());

            SortByWrapper sortByWrapper = null;
            if (sort != null)
            {
                sortByWrapper = sort.zToSortByWrapper();
                Trace.Write(" sort {0}", sortByWrapper.ToJson());
            }

            FieldsWrapper fieldsWrapper = null;
            if (fields != null)
            {
                fieldsWrapper = fields.zToFieldsWrapper();
                Trace.Write(" fields {0}", fieldsWrapper.ToJson());
            }

            if (limit != 0)
            {
                Trace.Write(" limit {0}", limit);
            }

            BsonDocument optionsDoc = null;
            if (options != null)
            {
                optionsDoc = options.zDeserializeToBsonDocument();
                Trace.Write(" options {0}", optionsDoc.ToJson());
            }

            Trace.WriteLine();

            MongoCursor<TDocument> cursor = collection.zFindAll<TDocument>(sortByWrapper, fieldsWrapper, limit, optionsDoc);

            //if (ResultToText)
            //{
            //    int i = 0;
            //    foreach (TDocument value in cursor)
            //    {
            //        Trace.WriteLine(value.zToJson(true));
            //        i++;
            //    }
            //    Trace.WriteLine("found {0} document(s)", i);
            //}
            return cursor;
        }

        //IEnumerable<TValue> Distinct<TValue>(string key, IMongoQuery query);
        //public static MongoCursor<TDocument> Find<TDocument>(string databaseName, string collectionName, string query, string sort = null, string fields = null, int limit = 0, string options = null, string server = null)
        //{
        //}

        public static void FindAndModify(string databaseName, string collectionName, string query, string update, bool upsert = false, string sort = null, string fields = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);

            QueryDocument queryDoc = query.zToQueryDocument();
            UpdateDocument updateDoc = update.zToUpdateDocument();

            Trace.Write("FindAndModify : {0} query ", collection.zGetFullName());
            Trace.Write(queryDoc.ToJson());
            Trace.Write(" update ");
            Trace.Write(updateDoc.ToJson());
            Trace.Write(" upsert {0}", upsert);

            SortByWrapper sortByWrapper = null;
            if (sort != null)
            {
                sortByWrapper = sort.zToSortByWrapper();
                Trace.Write(" sort {0}", sortByWrapper.ToJson());
            }

            FieldsWrapper fieldsWrapper = null;
            if (fields != null)
            {
                fieldsWrapper = fields.zToFieldsWrapper();
                Trace.Write(" fields {0}", fieldsWrapper.ToJson());
            }

            FindAndModifyResult result;
            try
            {
                result = collection.zFindAndModify(queryDoc, updateDoc, upsert, sortByWrapper, fieldsWrapper);
                TraceResult(result);
            }
            finally
            {
                Trace.WriteLine();
            }
            Trace.WriteLine("document");
            Trace.WriteLine(result.ModifiedDocument.zToJson());
        }

        //public static void FindAndRemove(string databaseName, string collectionName, string query, string sort = null, string fields = null, string server = null)
        //{
        //    MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);

        //    QueryDocument queryDoc = query.zToQueryDocument();

        //    Trace.Write("FindAndRemove : {0} query ", collection.zGetFullName());
        //    Trace.Write(queryDoc.ToJson());

        //    SortByWrapper sortByWrapper = null;
        //    if (sort != null)
        //    {
        //        sortByWrapper = sort.zToSortByWrapper();
        //        Trace.Write(" sort {0}", sortByWrapper.ToJson());
        //    }

        //    FieldsWrapper fieldsWrapper = null;
        //    if (fields != null)
        //    {
        //        fieldsWrapper = fields.zToFieldsWrapper();
        //        Trace.Write(" fields {0}", fieldsWrapper.ToJson());
        //    }

        //    FindAndModifyResult result;
        //    try
        //    {
        //        result = collection.zFindAndRemove(queryDoc, sortByWrapper, fieldsWrapper);
        //        TraceResult(result);
        //    }
        //    finally
        //    {
        //        Trace.WriteLine();
        //    }
        //    Trace.WriteLine("document");
        //    Trace.WriteLine(result.ModifiedDocument.zToJson());
        //}

        public static void Insert(string databaseName, string collectionName, string document, MongoInsertOptions options = null, string server = null)
        {
            Insert(databaseName, collectionName, document.zDeserializeToBsonDocument(), options, server);
        }

        public static void Insert<TDocument>(string databaseName, string collectionName, TDocument document, MongoInsertOptions options = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            Trace.Write("Insert : {0}", collection.zGetFullName());
            Trace.Write(" document ");
            Trace.Write(document.ToJson());
            if (options == null)
                options = new MongoInsertOptions();
            Trace.Write(" check element names {0} flags {1}", options.CheckElementNames, options.Flags);
            try
            {
                WriteConcernResult result = collection.zInsert(document, options);
                TraceResult(result);
            }
            finally
            {
                Trace.WriteLine();
            }
        }

        public static void Update(string databaseName, string collectionName, string query, string update, UpdateFlags flags = UpdateFlags.None, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);

            QueryDocument queryDoc = query.zToQueryDocument();
            UpdateDocument updateDoc = update.zToUpdateDocument();

            Trace.Write("Update : {0} query ", collection.zGetFullName());
            Trace.Write(queryDoc.ToJson());
            Trace.Write(" update ");
            Trace.Write(updateDoc.ToJson());
            Trace.Write(" flags ");
            Trace.Write(flags.ToString());

            try
            {
                WriteConcernResult result = collection.zUpdate(queryDoc, updateDoc, flags);
                TraceResult(result);
            }
            finally
            {
                Trace.WriteLine();
            }
        }

        public static void UpdateDocuments(string databaseName, string collectionName, string query, Action<BsonDocument> updateDocument, string sort = null, int limit = 0, string queryOptions = null,
            MongoInsertOptions saveOptions = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);

            Trace.Write("UpdateDocuments : {0} query ", collection.zGetFullName());

            QueryDocument queryDoc = query.zToQueryDocument();
            Trace.Write(queryDoc.ToJson());

            SortByWrapper sortByWrapper = null;
            if (sort != null)
            {
                sortByWrapper = sort.zToSortByWrapper();
                Trace.Write(" sort {0}", sortByWrapper.ToJson());
            }

            if (limit != 0)
            {
                Trace.Write(" limit {0}", limit);
            }

            BsonDocument queryOptionsDoc = null;
            if (queryOptions != null)
            {
                queryOptionsDoc = queryOptions.zDeserializeToBsonDocument();
                Trace.Write(" options {0}", queryOptionsDoc.ToJson());
            }

            Trace.WriteLine();

            if (saveOptions == null)
                saveOptions = new MongoInsertOptions();

            foreach (BsonDocument document in collection.zFind<BsonDocument>(queryDoc, sort: sortByWrapper, limit: limit, options: queryOptionsDoc))
            {
                updateDocument(document);

                Trace.Write("  Save : {0} document ", collection.zGetFullName());
                Trace.Write(document.ToJson());
                Trace.Write(" options ");
                Trace.Write(saveOptions.ToJson());
                Trace.WriteLine();

                collection.zSave(document, saveOptions);
            }
        }

        public static void Remove(string databaseName, string collectionName, string query, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            QueryDocument queryDoc = query.zToQueryDocument();
            Trace.Write("Remove : {0} query ", collection.zGetFullName());
            Trace.Write(queryDoc.ToJson());
            try
            {
                WriteConcernResult result = collection.zRemove(queryDoc);
                TraceResult(result);
            }
            finally
            {
                Trace.WriteLine();
            }
        }

        public static void RemoveAll(string databaseName, string collectionName, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            Trace.Write("RemoveAll : {0}", collection.zGetFullName());
            try
            {
                WriteConcernResult result = collection.zRemoveAll();
                TraceResult(result);
            }
            finally
            {
                Trace.WriteLine();
            }
        }

        public static void RenameCollection(string databaseName, string collectionName, string newCollectionName, string server = null)
        {
            MongoDatabase database = MongoCommand.GetDatabase(server, databaseName);
            Trace.Write("Rename collection : {0} \"{1}\" to \"{2}\"", database.zGetFullName(), collectionName, newCollectionName);
            try
            {
                CommandResult result = database.zRenameCollection(collectionName, newCollectionName);
                TraceResult(result);
            }
            finally
            {
                Trace.WriteLine();
            }
        }

        public static void ExportDatabase(string databaseName, string directory, string sort = null, string server = null)
        {
            MongoDatabase database = MongoCommand.GetDatabase(server, databaseName);
            Trace.Write("Export database : {0} to directory \"{1}\"", database.zGetFullName(), directory);
            foreach (string collection in database.GetCollectionNames())
            {
                string file = zPath.Combine(directory, "export_" + collection + ".txt");
                Export(databaseName, collection, file, sort: sort, server: server);
            }
        }

        public static void Export(string databaseName, string collectionName, string file, string query = null, string sort = null, string fields = null, int limit = 0, string options = null,
            Func<BsonDocument, BsonDocument> transformDocument = null, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            Trace.Write("Export : {0} to file \"{1}\"", collection.zGetFullName(), file);

            QueryDocument queryDoc = null;
            if (query != null)
            {
                queryDoc = query.zToQueryDocument();
                Trace.Write(queryDoc.ToJson());
            }

            SortByWrapper sortByWrapper = null;
            if (sort != null)
            {
                sortByWrapper = sort.zToSortByWrapper();
                Trace.Write(" sort {0}", sortByWrapper.ToJson());
            }

            FieldsWrapper fieldsWrapper = null;
            if (fields != null)
            {
                fieldsWrapper = fields.zToFieldsWrapper();
                Trace.Write(" fields {0}", fieldsWrapper.ToJson());
            }

            if (limit != 0)
            {
                Trace.Write(" limit {0}", limit);
            }

            BsonDocument optionsDoc = null;
            if (options != null)
            {
                optionsDoc = options.zDeserializeToBsonDocument();
                Trace.Write(" options {0}", optionsDoc.ToJson());
            }

            Trace.WriteLine();

            MongoCommand.Export(databaseName, collectionName, file, query, sort, fields, limit, options, transformDocument, server);
        }

        public static void Import(string databaseName, string collectionName, string file, string server = null)
        {
            MongoCollection collection = MongoCommand.GetDatabase(server, databaseName).GetCollection(collectionName);
            Trace.WriteLine("Import : {0} from file \"{1}\"", collection.zGetFullName(), file);
            MongoCommand.Import(databaseName, collectionName, file, server);
        }

        public static void TraceResult(WriteConcernResult result)
        {
            Trace.Write(" {0}{1} document(s) affected {2}", result.DocumentsAffected, result.UpdatedExisting ? " existing" : "", result.Ok ? "ok" : "not ok");
            if (result.ErrorMessage != null)
                Trace.Write(" error \"{0}\"", result.ErrorMessage);
            if (result.HasLastErrorMessage)
                Trace.Write(" last error \"{0}\"", result.LastErrorMessage);
            //Trace.WriteLine();
        }

        public static void TraceResult(CommandResult result)
        {
            Trace.Write(" {0}", result.Ok ? "ok" : "");
            if (result.ErrorMessage != null)
                Trace.Write(" error \"{0}\"", result.ErrorMessage);
            //Trace.WriteLine();
        }

        //public static void TraceResult(FindAndModifyResult result)
        //{
        //    Trace.Write(" {0}", result.Ok ? "ok" : "");
        //    if (result.ErrorMessage != null)
        //        Trace.Write(" error \"{0}\"", result.ErrorMessage);
        //}
    }

    public static partial class GlobalExtension
    {
        public static EvalArgs zToEvalArgs(this string code)
        {
            if (code != null)
            {
                if (code.StartsWith("{") && code.EndsWith("}"))
                    code = "db.runCommand( " + code + " )";
                return new EvalArgs { Code = new BsonJavaScript(code) };
            }
            else
                return null;
        }

        public static QueryDocument zToQueryDocument(this string query)
        {
            if (query != null)
                return new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(query));
            else
                return null;
        }

        public static SortByWrapper zToSortByWrapper(this string sort)
        {
            if (sort != null)
                return new SortByWrapper(BsonSerializer.Deserialize<BsonDocument>(sort));
            else
                return null;
        }

        public static FieldsWrapper zToFieldsWrapper(this string fields)
        {
            if (fields != null)
                return new FieldsWrapper(BsonSerializer.Deserialize<BsonDocument>(fields));
            else
                return null;
        }

        public static BsonDocument zDeserializeToBsonDocument(this string jsonDocument)
        {
            if (jsonDocument != null)
                return BsonSerializer.Deserialize<BsonDocument>(jsonDocument);
            else
                return null;
        }

        public static UpdateDocument zToUpdateDocument(this string update)
        {
            if (update != null)
                return new UpdateDocument(BsonSerializer.Deserialize<BsonDocument>(update));
            else
                return null;
        }
    }
}

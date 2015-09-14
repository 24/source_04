using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Wrappers;
using MongoDB.Bson;
using pb.IO;
using System.Text;

namespace pb.Data.Mongo
{
    public class MongoLog
    {
        private static MongoLog _currentMongoLog = new MongoLog();
        //private Log_v1 _log;
        private WriteToFile _writeToFile = null;

        public static MongoLog CurrentMongoLog { get { return _currentMongoLog; } set { _currentMongoLog = value; } }

        //public Log_v1 Log { get { return _log; } set { _log = value; } }

        //public void SetLogFile(string file, LogOptions_v1 options)
        //{
        //    _log = new Log_v1(file, options);
        //    //Trace.WriteLine("set mongo log file to \"{0}\"", _log.File);
        //}

        public void SetLogFile(string file, FileOption option, Encoding encoding = null)
        {
            _writeToFile = WriteToFile.Create(file, option, encoding);
            //Trace.WriteLine("set mongo log file to \"{0}\"", _log.File);
        }

        public void LogEval(MongoDatabase database, EvalArgs evalArgs)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Eval : {0} ", database.zGetFullName());
            _writeToFile.Write(evalArgs.ToJson());
            _writeToFile.WriteLine();
        }

        public void LogFindAs(MongoCollection collection, QueryDocument query, SortByWrapper sort = null, FieldsWrapper fields = null, int limit = 0, BsonDocument options = null)
        {
            LogQuery("FindAs", collection, query: query, sort: sort, fields: fields, limit: limit, options: options);
            if (_writeToFile != null)
                _writeToFile.WriteLine();
        }

        public void LogFindAllAs(MongoCollection collection, SortByWrapper sort = null, FieldsWrapper fields = null, int limit = 0, BsonDocument options = null)
        {
            LogQuery("FindAllAs", collection, sort: sort, fields: fields, limit: limit, options: options);
            if (_writeToFile != null)
                _writeToFile.WriteLine();
        }

        public void LogFindOneAs<T>(MongoCollection collection, QueryDocument query)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("FindOneAs : {0} query ", collection.zGetFullName());
            _writeToFile.Write(query.ToJson());
            _writeToFile.Write(" type {0}", typeof(T));
            _writeToFile.WriteLine();
        }

        public void LogFindOneByIdAs<T>(MongoCollection collection, BsonValue bsonKey)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("FindOneByIdAs : {0} key ", collection.zGetFullName());
            _writeToFile.Write(bsonKey.ToJson());
            _writeToFile.Write(" type {0}", typeof(T));
            _writeToFile.WriteLine();
        }

        public void LogFindAndModify(MongoCollection collection, QueryDocument query, UpdateDocument update, bool upsert, SortByWrapper sort, FieldsWrapper fields)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("FindAndModify : {0}", collection.zGetFullName());
            _writeToFile.Write(" query {0}", query.ToJson());
            _writeToFile.Write(" update ");
            _writeToFile.Write(update.ToJson());
            _writeToFile.Write(" upsert {0}", upsert);
            if (sort != null)
                _writeToFile.Write(" sort {0}", sort.ToJson());
            if (fields != null)
                _writeToFile.Write(" fields {0}", fields.ToJson());
            //_log.WriteLine();
        }

        //public void LogFindAndRemove(MongoCollection collection, QueryDocument query, SortByWrapper sort = null, FieldsWrapper fields = null)
        //{
        //    LogQuery("FindAndRemove", collection, sort: sort, fields: fields);
        //}

        public void LogInsert<TDocument>(MongoCollection collection, TDocument document, MongoInsertOptions options)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Insert : {0}", collection.zGetFullName());
            _writeToFile.Write(" document ");
            _writeToFile.Write(document.ToJson());
            _writeToFile.Write(" check element names {0} flags {1}", options.CheckElementNames, options.Flags);
        }

        public void LogCount(MongoCollection collection, IMongoQuery query)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Count : {0}", collection.zGetFullName());
            if (query != null)
            {
                _writeToFile.Write(" query ");
                _writeToFile.Write(query.ToJson());
            }
            _writeToFile.Write(" count ");
        }

        public void LogUpdate(MongoCollection collection, QueryDocument query, UpdateDocument update, UpdateFlags flags)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Update : {0} query ", collection.zGetFullName());
            _writeToFile.Write(query.ToJson());
            _writeToFile.Write(" update ");
            _writeToFile.Write(update.ToJson());
            _writeToFile.Write(" flags ");
            _writeToFile.Write(flags.ToString());
        }

        public void LogRemove(MongoCollection collection, QueryDocument query)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Remove : {0} query ", collection.zGetFullName());
            _writeToFile.Write(query.ToJson());
        }

        public void LogRemoveAll(MongoCollection collection)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("RemoveAll : {0}", collection.zGetFullName());
        }

        public void LogRenameCollection(MongoDatabase database, string collectionName, string newCollectionName)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Rename collection : {0} \"{1}\" to \"{2}\"", database.zGetFullName(), collectionName, newCollectionName);
        }

        public void LogSave(MongoCollection collection, BsonDocument document, MongoInsertOptions options = null)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("Save : {0} document ", collection.zGetFullName());
            _writeToFile.Write(document.ToJson());
            _writeToFile.Write(" options ");
            _writeToFile.Write(options.ToJson());
        }

        public void LogUpdateDocuments(MongoCollection collection, QueryDocument query, SortByWrapper sort = null, int limit = 0, BsonDocument options = null)
        {
            LogQuery("Update documents", collection, query: query, sort: sort, limit: limit, options: options);
        }

        public void LogExport(MongoCollection collection, string file, QueryDocument query, SortByWrapper sort = null, FieldsWrapper fields = null, int limit = 0, BsonDocument options = null)
        {
            LogQuery("Export", collection, file: file, query: query, sort: sort, fields: fields, limit: limit, options: options);
            if (_writeToFile != null)
                _writeToFile.WriteLine();
        }

        public void LogImport(MongoCollection collection, string file)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.WriteLine("Import : {0} file \"{1}\"", collection.zGetFullName(), file);
        }

        private void LogQuery(string label, MongoCollection collection, string file = null, QueryDocument query = null, SortByWrapper sort = null, FieldsWrapper fields = null, int limit = 0, BsonDocument options = null)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write("{0:yyyy-MM-dd HH:mm:ss.ffffff} ", DateTime.Now);
            _writeToFile.Write("{0} : {1}", label, collection.zGetFullName());
            if (file != null)
                _writeToFile.Write(" file \"{0}\"", file);
            if (query != null)
                _writeToFile.Write(" query {0}", query.ToJson());
            if (sort != null)
                _writeToFile.Write(" sort {0}", sort.ToJson());
            if (fields != null)
                _writeToFile.Write(" fields {0}", fields.ToJson());
            if (limit != 0)
                _writeToFile.Write(" limit {0}", limit);
            if (options != null)
                _writeToFile.Write(" options {0}", options.ToJson());
            //_log.WriteLine();
        }


        public T ExecuteAndLogResult<T>(Func<T> command)
        {
            try
            {
                //WriteConcernResult result = command();
                T result = command();

                if (_writeToFile != null)
                {
                    if (result is FindAndModifyResult)
                        LogResult(result as FindAndModifyResult);
                    else if (result is WriteConcernResult)
                        LogResult(result as WriteConcernResult);
                    else if (result is CommandResult)
                        LogResult(result as CommandResult);
                    else if (typeof(T).IsValueType)
                        _writeToFile.WriteLine(result.ToString());
                    //else if (result is BsonValue)
                    //    LogResult(result as BsonValue);
                    else if (typeof(T) != typeof(MongoCursor) && typeof(T).BaseType != typeof(MongoCursor))
                        _writeToFile.WriteLine(result.zToJson());
                }
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        //public void LogResult(BsonValue result)
        //{
        //    if (_log == null)
        //        return;
        //    _log.WriteLine(result.zToJson());
        //}

        public void LogResult(WriteConcernResult result)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write(" result {0}{1} document(s) affected {2}", result.DocumentsAffected, result.UpdatedExisting ? " existing" : "", result.Ok ? "ok" : "not ok");
            if (result.ErrorMessage != null)
                _writeToFile.Write(" error \"{0}\"", result.ErrorMessage);
            if (result.HasLastErrorMessage)
                _writeToFile.Write(" last error \"{0}\"", result.LastErrorMessage);
            _writeToFile.WriteLine();
        }

        public void LogResult(CommandResult result)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write(" result {0}", result.Ok ? "ok" : "");
            if (result.ErrorMessage != null)
                _writeToFile.Write(" error \"{0}\"", result.ErrorMessage);
            _writeToFile.WriteLine();
        }

        public void LogResult(FindAndModifyResult result)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.Write(" result {0}", result.Ok ? "ok" : "");
            if (result.ErrorMessage != null)
                _writeToFile.Write(" error \"{0}\"", result.ErrorMessage);
            _writeToFile.WriteLine();
            _writeToFile.WriteLine("document");
            _writeToFile.WriteLine(result.ModifiedDocument.zToJson());
        }

        public void LogError(Exception ex)
        {
            if (_writeToFile == null)
                return;
            _writeToFile.WriteLine(" error");
            _writeToFile.WriteLine(ex.Message);
            _writeToFile.WriteLine(ex.StackTrace);
        }
    }

    public static partial class GlobalExtension
    {
        public static BsonValue zEval(this MongoDatabase database, EvalArgs evalArgs)
        {
            MongoLog.CurrentMongoLog.LogEval(database, evalArgs);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => database.Eval(evalArgs));
        }

        //public static MongoCursor<BsonDocument> zFind(this MongoCollection collection, QueryDocument query, SortByWrapper sort = null, FieldsWrapper fields = null,
        //    int limit = 0, BsonDocument options = null)
        //{
        //    return collection.zFind<BsonDocument>(query, sort, fields, limit, options);
        //}

        public static MongoCursor<TDocument> zFind<TDocument>(this MongoCollection collection, QueryDocument query, SortByWrapper sort = null, FieldsWrapper fields = null,
            int limit = 0, BsonDocument options = null)
        {
            MongoLog.CurrentMongoLog.LogFindAs(collection, query, sort, fields, limit, options);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(
                () =>
                {
                    MongoCursor<TDocument> cursor = collection.FindAs<TDocument>(query);
                    if (sort != null)
                        cursor.SetSortOrder(sort);
                    if (fields != null)
                        cursor.SetFields(fields);
                    if (limit != 0)
                        cursor.SetLimit(limit);
                    if (options != null)
                        cursor.SetOptions(options);
                    return cursor;
                });
        }

        public static MongoCursor<TDocument> zFindAll<TDocument>(this MongoCollection collection, SortByWrapper sort = null, FieldsWrapper fields = null, int limit = 0, BsonDocument options = null)
        {
            MongoLog.CurrentMongoLog.LogFindAllAs(collection, sort, fields, limit, options);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(
                () =>
                {
                    MongoCursor<TDocument> cursor = collection.FindAllAs<TDocument>();
                    if (sort != null)
                        cursor.SetSortOrder(sort);
                    if (fields != null)
                        cursor.SetFields(fields);
                    if (limit != 0)
                        cursor.SetLimit(limit);
                    if (options != null)
                        cursor.SetOptions(options);
                    return cursor;
                });
        }

        public static TDocument zFindOne<TDocument>(this MongoCollection collection, QueryDocument query)
        {
            MongoLog.CurrentMongoLog.LogFindOneAs<TDocument>(collection, query);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.FindOneAs<TDocument>(query));
        }

        public static TDocument zFindOneById<TDocument>(this MongoCollection collection, BsonValue bsonKey)
        {
            MongoLog.CurrentMongoLog.LogFindOneByIdAs<TDocument>(collection, bsonKey);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.FindOneByIdAs<TDocument>(bsonKey));
        }

        public static FindAndModifyResult zFindAndModify(this MongoCollection collection, QueryDocument query, UpdateDocument update, bool upsert = false, SortByWrapper sort = null, FieldsWrapper fields = null)
        {
            MongoLog.CurrentMongoLog.LogFindAndModify(collection, query, update, upsert, sort, fields);
            FindAndModifyArgs args = new FindAndModifyArgs();
            args.Query = query;
            args.SortBy = sort;
            args.Fields = fields;
            args.Update = update;
            args.Upsert = upsert;
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.FindAndModify(args));
        }

        //public static FindAndModifyResult zFindAndRemove(this MongoCollection collection, QueryDocument query, SortByWrapper sort = null, FieldsWrapper fields = null)
        //{
        //    MongoLog.CurrentMongoLog.LogFindAndRemove(collection, query, sort, fields);
        //    FindAndRemoveArgs args = new FindAndRemoveArgs();
        //    args.Query = query;
        //    args.SortBy = sort;
        //    args.Fields = fields;
        //    return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.FindAndRemove(args));
        //}

        public static WriteConcernResult zInsert<TDocument>(this MongoCollection collection, TDocument document, MongoInsertOptions options = null)
        {
            if (options == null)
                options = new MongoInsertOptions();
            MongoLog.CurrentMongoLog.LogInsert(collection, document, options);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.Insert<TDocument>(document, options));
        }

        public static long zCount(this MongoCollection collection, IMongoQuery query = null)
        {
            MongoLog.CurrentMongoLog.LogCount(collection, query);
            if (query != null)
                return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.Count(query));
            else
                return MongoLog.CurrentMongoLog.ExecuteAndLogResult(collection.Count);
        }

        public static WriteConcernResult zUpdate(this MongoCollection collection, QueryDocument query, UpdateDocument update, UpdateFlags flags)
        {
            MongoLog.CurrentMongoLog.LogUpdate(collection, query, update, flags);
            //return collection.Update(query, update, flags);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.Update(query, update, flags));
        }

        public static WriteConcernResult zRemove(this MongoCollection collection, QueryDocument query)
        {
            MongoLog.CurrentMongoLog.LogRemove(collection, query);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.Remove(query));
        }

        public static WriteConcernResult zRemoveAll(this MongoCollection collection)
        {
            MongoLog.CurrentMongoLog.LogRemoveAll(collection);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.RemoveAll());
        }

        public static CommandResult zRenameCollection(this MongoDatabase database, string collectionName, string newCollectionName)
        {
            MongoLog.CurrentMongoLog.LogRenameCollection(database, collectionName, newCollectionName);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => database.RenameCollection(collectionName, newCollectionName));
        }

        public static WriteConcernResult zSave(this MongoCollection collection, BsonDocument document, MongoInsertOptions options = null)
        {
            if (options == null)
                options = new MongoInsertOptions();
            MongoLog.CurrentMongoLog.LogSave(collection, document, options);
            return MongoLog.CurrentMongoLog.ExecuteAndLogResult(() => collection.Save(document, options));
        }

        public static void zSave(this IEnumerable<BsonDocument> documents, MongoCollection collection, MongoInsertOptions options = null)
        {
            if (options == null)
                options = new MongoInsertOptions();
            foreach (BsonDocument document in documents)
            {
                collection.zSave(document, options);
            }
        }
    }
}

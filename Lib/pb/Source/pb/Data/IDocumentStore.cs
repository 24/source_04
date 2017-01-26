using MongoDB.Bson;
using System;
using System.Collections.Generic;
using pb.Reflection;

namespace pb.Data
{
    // used by WebDataManager_v2.SaveWithKey(), WebDataManager_v2.Refresh(), DownloadAutomateManager_v2.TryDownloadPost(), DocumentStoreExtension.zGetKey_v2()
    public interface IKeyData
    {
        BsonValue GetKey();
    }

    public interface IKeyDetail
    {
        BsonValue GetDetailKey();
    }

    // used by WebDataManager_v2.SetDataId(), WebDataManager_v2.Refresh(), DocumentStoreExtension.zGetId), DocumentStoreExtension.zSetId()
    public interface IIdData
    {
        BsonValue GetId();
        void SetId(BsonValue id);
    }

    public interface IQuery
    {
        //QueryDocument GetQuery();
        //string GetQuery();
        IEnumerable<KeyValuePair<string, object>> GetQueryValues();
    }

    public interface IDocumentStore<TData>
    {
        //string DefaultSort { get; set; }
        Type NominalType { get; set; }
        bool GenerateId { get; }
        bool Exists(BsonValue key);
        BsonValue GetId(BsonValue key);
        BsonValue GetNewId();
        TData LoadFromId(BsonValue id);
        TData LoadFromKey(BsonValue key);
        void SaveWithId(BsonValue id, TData document);
        void SaveWithKey(BsonValue key, TData document);
        IEnumerable<TData> Find(string query, string sort = null, int limit = 0, string options = null);
        int Update(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0);
        IEnumerable<string> Backup(string directory);
    }

    public static class DocumentStoreExtension
    {
        public static BsonValue zGetKey_v2<TData>(this TData data)
        {
            if (data is IKeyData)
                return ((IKeyData)data).GetKey();
            else
                throw new PBException("type {0} is not IKeyData", data.GetType().zGetTypeName());
        }

        public static BsonValue zGetId<TData>(this TData data)
        {
            if (data is IIdData)
                return ((IIdData)data).GetId();
            else
                throw new PBException("type {0} is not IIdData", data.GetType().zGetTypeName());
        }

        public static void zSetId<TData>(this TData data, BsonValue id)
        {
            if (data is IIdData)
                ((IIdData)data).SetId(id);
            else
                throw new PBException("type {0} is not IIdData", data.GetType().zGetTypeName());
        }
    }
}

using System;
using System.Collections.Generic;
using pb.Reflection;

namespace pb.Data
{
    public interface IKeyData<TKey>
    {
        TKey GetKey();
    }

    public interface IDocumentStore<TKey, TData>
    {
        string DefaultSort { get; set; }
        bool DocumentExists(TKey key);
        TData LoadDocument(TKey key);
        void SaveDocument(TKey key, TData document);
        IEnumerable<TData> FindDocuments(string query, string sort = null, int limit = 0, string options = null);
        int UpdateDocuments(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0);
    }

    public static partial class GlobalExtension
    {
        public static TKey zGetKey<TKey, TData>(this TData data)
        {
            if (data is IKeyData<TKey>)
                return ((IKeyData<TKey>)data).GetKey();
            else
                throw new PBException("type {0} is not IKeyData<{1}>", data.GetType().zGetTypeName(), typeof(TKey).zGetTypeName());
        }
    }
}

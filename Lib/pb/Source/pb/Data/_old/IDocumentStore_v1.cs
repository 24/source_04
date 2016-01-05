using System;
using System.Collections.Generic;
using pb.Web.Data;

namespace pb.Data.old
{
    //public interface ILoadDocument<T>
    //{
    //    object Key { get; set; }
    //    T Document { get; set; }
    //    bool DocumentLoaded { get; set; }
    //}

    //public class LoadDocument<T> : ILoadDocument<T>
    //{
    //    private object _key;
    //    private T _document;
    //    private bool _documentLoaded;

    //    public object Key { get { return _key; } set { _key = value; } }
    //    public T Document { get { return _document; } set { _document = value; } }
    //    public bool DocumentLoaded { get { return _documentLoaded; } set { _documentLoaded = value; } }
    //}

    public interface ILoadDocument_v4<TKey, TData>
    {
        TKey Key { get; set; }
        TData Document { get; set; }
        bool DocumentLoaded { get; set; }
    }

    public interface ILoadDocument_v6<TKey>
    {
        IKeyData_v4<TKey> Document { get; set; }
        bool DocumentLoaded { get; set; }
    }

    public interface IDocumentRequest_v1<T>
    {
        bool RefreshDocumentStore { get; set; }
        T Document { get; set; }
        bool DocumentLoaded { get; set; }
        object Key { get; set; }
    }

    public interface IDocumentStore_v1<T>
    {
        bool DocumentExists(IDocumentRequest_v1<T> dataRequest);
        void LoadDocument(IDocumentRequest_v1<T> dataRequest);
        void SaveDocument(IDocumentRequest_v1<T> dataRequest);
    }

    public interface IDocumentStore_v2<T>
    {
        bool DocumentExists(object key);
        T LoadDocument(object key);
        void SaveDocument(object key, T document);
        IEnumerable<T> FindDocuments(string query, string sort = null, int limit = 0, string options = null);
    }

    public interface IDocumentStore_v3<TKey, TData>
    {
        Func<TData, TKey> GetDataKey { get; set; }
        //Func<BsonDocument, TData> Deserialize { get; set; }
        string DefaultSort { get; set; }
        bool DocumentExists(TKey key);
        TData LoadDocument(TKey key);
        void SaveDocument(TKey key, TData document);
        // bool useCursorCache = false
        IEnumerable<TData> FindDocuments(string query, string sort = null, int limit = 0, string options = null);
        int UpdateDocuments(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0);
    }

    public static partial class GlobalExtension
    {
        public static void UpdateDocuments<T>(this IDocumentStore_v2<T> documentStore, Func<T, object> keySelector, Func<T, T> updateDocument = null, string query = null, string sort = null, int limit = 0, string options = null)
        {
            if (query == null)
                query = "{}";
            foreach (T document in documentStore.FindDocuments(query, sort, limit, options))
            {
                object key = keySelector(document);
                T document2 = document;
                if (updateDocument != null)
                    document2 = updateDocument(document);
                documentStore.SaveDocument(key, document2);
            }
        }
    }
}

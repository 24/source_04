using System;

namespace pb.Web.Data
{
    partial class WebDataManager_v4<TData>
    {
        //public int Update(Action<TData> updateDocument, string query = null, string sort = null, int limit = 0)
        //{
        //    //int nb = _documentStore.Update(updateDocument, query, sort, limit);
        //    int nb = 0;
        //    //foreach (BsonDocument document in _Find(query, sort: sort, limit: limit))
        //    //{
        //    //    TData data = Deserialize(document);
        //    //    updateDocument(data);
        //    //    BsonValue id = GetId(document);
        //    //    if (id == null)
        //    //        throw new PBException("can't update document without id {0}", document);
        //    //    Save(id, data);
        //    //    nb++;
        //    //}
        //    foreach (TData data in Find(query, sort: sort, limit: limit))
        //    {
        //        updateDocument(data);
        //        BsonValue id = GetId(document);
        //        if (id == null)
        //            throw new PBException("can't update document without id {0}", document);
        //        Save(id, data);
        //        nb++;
        //    }

        //    Trace.WriteLine("{0} document(s) updated", nb);
        //    return nb;
        //}
    }
}

using System.Collections.Generic;
using System.Data;
using MongoDB.Bson;
using System.Collections;

namespace pb.Data.Mongo
{
    public static class MongoDataTableExtension
    {
        public static DataTable zToDataTable_v3<T>(this T v)
        {
            if (v is BsonDocument)
                return (v as BsonDocument).zToDataTable2();
            // todo v is BsonValue
            //else if (v is BsonValue)
            //{
            //}
            //else if (v is IEnumerable<BsonDocument>)
            //    return (v as IEnumerable<BsonDocument>).zToDataTable2();
            else if (v is IEnumerable<BsonValue>)
                return (v as IEnumerable<BsonValue>).zToDataTable2();
            else if (v is IEnumerable)
            {
                DataTable dt = new DataTable();
                IEnumerator enumerator = ((IEnumerable)v).GetEnumerator();
                while (enumerator.MoveNext())
                    enumerator.Current.ToBsonDocument().zToDataTable2(dt);
                return dt;
            }
            else
            {
                return v.ToBsonDocument().zToDataTable2();
            }
        }
    }
}

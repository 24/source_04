using MongoDB.Bson.Serialization;
using pb.Data;

namespace Download.Print.ZoneEbooks
{
    public static class ZoneEbooks
    {
        public static void InitMongoClassMap()
        {
            // Register all class derived from ZValue to deserialize field NamedValues<ZValue> infos (TelechargementPlus_Base)
            //   ZString, ZStringArray, ZInt
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZString)))
                BsonClassMap.RegisterClassMap<ZString>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZStringArray)))
                BsonClassMap.RegisterClassMap<ZStringArray>();
            if (!BsonClassMap.IsClassMapRegistered(typeof(ZInt)))
                BsonClassMap.RegisterClassMap<ZInt>();
        }

        //public static IEnumerable<TelechargementPlus_PostDetail> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false)
        //{
        //    return from header in TelechargementPlus_LoadHeaderPages.LoadHeaderPages(startPage, maxPage, reloadHeaderPage, loadImage) select LoadDetailItem(header, reloadDetail, loadImage);
        //}

        //public static TelechargementPlus_PostDetail LoadDetailItem(TelechargementPlus_PostHeader header, bool reload = false, bool loadImage = false)
        //{
        //    return TelechargementPlus_LoadDetail.Load(header.urlDetail, reload: reload, loadImage: loadImage);
        //}
    }
}

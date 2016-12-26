using pb.Reflection;

namespace pb.Web.Data
{
    // used by :
    //   - WebLoadImageManager_v1
    public interface ILoadImages
    {
        //void LoadImages(bool refreshImage = false);
        void LoadImages(WebImageRequest imageRequest);
    }

    //[Obsolete]
    // used by :
    //   - WebDataManager<TData>
    //   - WebData<TData>.Load_v1()
    //   - WebData<TData>.Load_v2()
    public static class WebLoadImageManager_v1
    {
        public static void LoadImages<TData>(TData data, WebImageRequest request)
        {
            if (request.LoadImageFromWeb || request.LoadImageToData || request.RefreshImage)
            {
                if (!(data is ILoadImages))
                    throw new PBException($"{typeof(TData).zGetTypeName()} is not ILoadImages");
                ((ILoadImages)data).LoadImages(request);
            }
        }
    }
}

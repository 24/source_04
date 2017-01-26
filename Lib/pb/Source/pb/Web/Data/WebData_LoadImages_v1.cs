namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private void LoadImages_v1(WebImageRequest imageRequest)
        {
            if ((_dataLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                || (!_dataLoadedFromWeb && (imageRequest.LoadImageToData || imageRequest.LoadMissingImageFromWeb)))
            {
                WebLoadImageManager_v1.LoadImages(_data, imageRequest);
            }
        }
    }
}

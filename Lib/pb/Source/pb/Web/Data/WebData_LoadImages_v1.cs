namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private void LoadImages_v1(WebImageRequest imageRequest)
        {
            if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                || (!_documentLoadedFromWeb && (imageRequest.LoadImageToData || imageRequest.LoadMissingImageFromWeb)))
            {
                WebLoadImageManager_v1.LoadImages(_document, imageRequest);
            }
        }
    }
}

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private void LoadImages_v2(WebImageRequest imageRequest)
        {
            WebLoadImageManager_v2<TData> webLoadImageManager = _webDataManager_v4.WebLoadImageManager;

            if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                || (!_documentLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
            {
                if (webLoadImageManager.LoadImagesFromWeb(this))
                {
                    if (_id != null)
                        _documentStore.SaveWithId(_id, _document);
                    else
                        _documentStore.SaveWithKey(_key, _document);
                }
            }

            if (imageRequest.LoadImageToData)
            {
                webLoadImageManager.LoadImagesToData(_document);
            }
        }
    }
}

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private void LoadImages_v2(WebImageRequest imageRequest)
        {
            WebLoadImageManager_v2<TData> webLoadImageManager = _webDataManager_v4.WebLoadImageManager;

            if ((_dataLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                || (!_dataLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
            {
                //if (webLoadImageManager.LoadImagesFromWeb(this))
                if (_data is IGetWebImages)
                {
                    if (webLoadImageManager.LoadImagesFromWeb(imageRequest, ((IGetWebImages)_data).GetWebImages(), _webDataManager_v4.GetImageSubDirectory?.Invoke(this)))
                    {
                        if (_id != null)
                            _documentStore.SaveWithId(_id, _data);
                        else
                            _documentStore.SaveWithKey(_key, _data);
                    }
                }
            }

            if (imageRequest.LoadImageToData)
            {
                webLoadImageManager.LoadImagesToData(_data);
            }
        }
    }
}

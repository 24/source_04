using MongoDB.Bson;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private void LoadImages_v3(WebImageRequest imageRequest)
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
                        BsonDocument data = Serialize();
                        if (_id != null)
                            _dataStore.SaveWithId(_id, data);
                        else
                            _dataStore.SaveWithKey(_key, data);
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

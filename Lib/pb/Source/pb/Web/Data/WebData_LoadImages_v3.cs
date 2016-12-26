using MongoDB.Bson;

namespace pb.Web.Data
{
    partial class WebData<TData>
    {
        private void LoadImages_v3(WebImageRequest imageRequest)
        {
            WebLoadImageManager_v2<TData> webLoadImageManager = _webDataManager_v4.WebLoadImageManager;

            if ((_documentLoadedFromWeb && (imageRequest.LoadImageFromWeb || imageRequest.RefreshImage))
                || (!_documentLoadedFromWeb && imageRequest.LoadMissingImageFromWeb))
            {
                if (webLoadImageManager.LoadImagesFromWeb(this))
                {
                    BsonDocument data = Serialize();
                    if (_id != null)
                        _dataStore.SaveWithId(_id, data);
                    else
                        _dataStore.SaveWithKey(_key, data);
                }
            }

            if (imageRequest.LoadImageToData)
            {
                webLoadImageManager.LoadImagesToData(_document);
            }
        }
    }
}

namespace MyDownloader.Spider
{
    public interface ISpiderResourceFactory
    {
        ISpiderResource CreateSpider(SpiderContext cntx, ISpiderResource parent, string location);
    }
}

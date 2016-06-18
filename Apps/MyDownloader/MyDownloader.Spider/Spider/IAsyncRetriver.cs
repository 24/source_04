namespace MyDownloader.Spider
{
    public interface IAsyncRetriver
    {
        ISpiderResource Resource { get; }

        bool IsCompleted { get; }
    }
}

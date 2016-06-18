using System.Collections.Generic;

namespace MyDownloader.Spider
{
    public interface ISpiderResource
    {
        string Location { get; }

        int Depth { get; }

        ISpiderResource ParentResource { get; }

        List<ISpiderResource> NextResources { get; }

        IAsyncRetriver BeginReceive();

        void EndReceive();
    }
}

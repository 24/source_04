using System;

namespace MyDownloader.Spider
{
    public interface INamingRule
    {
        bool Accept(Uri location, SpiderContext context, ISpiderResource parentSpider);
    }
}

using System;

namespace MyDownloader.Spider.Web.Rules
{
    public class MaxDepthNamingRule: INamingRule
    {
        private int maxDepth;

        public MaxDepthNamingRule(int maxDepth)
        {
            this.maxDepth = maxDepth;
        }

        #region INamingRule Members

        public bool Accept(Uri location, SpiderContext context, ISpiderResource parentSpider)
        {
            if (this.maxDepth > 0 && parentSpider.Depth >= this.maxDepth)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}

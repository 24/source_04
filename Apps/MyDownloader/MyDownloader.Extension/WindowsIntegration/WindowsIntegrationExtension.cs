using MyDownloader.Core.Extensions;

namespace MyDownloader.Extension.WindowsIntegration
{
    public class WindowsIntegrationExtension: IExtension
    {
        #region IExtension Members

        public string Name
        {
            get { return ("Windows Integration"); }
        }

        public IUIExtension UIExtension
        {
            get { return new WindowsIntegrationUIExtension(); }
        }

        #endregion
    }
}

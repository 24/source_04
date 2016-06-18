using System.Windows.Forms;

namespace MyDownloader.Extension.WindowsIntegration.ClipboardMonitor
{
    public interface IClipboardDataHandler
    {
        void HandleClipboardData(IDataObject data);
    }
}

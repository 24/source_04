using MyDownloader.Core.Extensions;

namespace MyDownloader.Extension.SpeedLimit
{
    public interface ISpeedLimitParameters: IExtensionParameters
    {
        bool Enabled { get; set; }

        double MaxRate { get; set; }
    }
}

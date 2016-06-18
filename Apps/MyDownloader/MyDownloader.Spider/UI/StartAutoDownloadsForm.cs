using System.Windows.Forms;
using MyDownloader.Extension.AutoDownloads.UI;

namespace MyDownloader.Spider.UI
{
    public partial class StartAutoDownloadsForm : Form
    {
        public StartAutoDownloadsForm()
        {
            InitializeComponent();
        }

        public ScheduledDownloadEnabler ScheduledDownloadEnabler
        {
            get
            {
                return scheduledDownloadEnabler1;
            }
        }
    }
}
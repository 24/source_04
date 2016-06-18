using System;
using MyDownloader.Core.Extensions;
using MyDownloader.Extension.Notifications.Helpers;

namespace MyDownloader.Extension.Notifications
{
    public class NotificationsExtension: IExtension
    {
        #region IExtension Members

        public string Name
        {
            get { return "Notifications"; }
        }

        public IUIExtension UIExtension
        {
            get { return new NotificationsUIExtension(); }
        }

        #endregion

        #region Constructor

        public NotificationsExtension(INotificationsExtensionParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            SoundHelper.Start(parameters);
            BalloonHelper.Start(parameters);
        }

        public NotificationsExtension():
            this(new NotificationsExtensionParametersSettingsProxy())
        {

        }
 
        #endregion
    }
}

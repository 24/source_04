using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using MyDownloader.App;
using MyDownloader.App.UI;
using MyDownloader.App.SingleInstancing;

using MyDownloader.Core;
using MyDownloader.Core.Extensions;
using MyDownloader.Core.UI;

using MyDownloader.Extension;
using MyDownloader.Extension.AntiVirus;
using MyDownloader.Extension.Protocols;
using MyDownloader.Extension.Notifications;
using MyDownloader.Extension.Video;
using MyDownloader.Extension.AutoDownloads;
using MyDownloader.Extension.SpeedLimit;
using MyDownloader.Extension.PersistedList;
using MyDownloader.Extension.WindowsIntegration;

namespace MyDownloader.App
{
    [Serializable]
    public class App : IApp
    {
        #region Singleton

        private static App instance = new App();

        public static App Instance
        {
            get
            {
                return instance;
            }
        }

        private App()
        {
            AppManager.Instance.Initialize(this);

            extensions = new List<IExtension>();

            extensions.Add(new CoreExtention());
            extensions.Add(new HttpFtpProtocolExtension());
            extensions.Add(new VideoDownloadExtension());
            extensions.Add(new SpeedLimitExtension());
            extensions.Add(new PersistedListExtension());
            extensions.Add(new AntiVirusExtension());
            extensions.Add(new NotificationsExtension());
            extensions.Add(new AutoDownloadsExtension());
            extensions.Add(new WindowsIntegrationExtension());

            try
            {
                Icon = Resource.dragon_fly;
            }
            catch //(System.Exception ex)
            {
                //string msg = ex.Message + "\r\n" + ex.StackTrace;
                //MessageBox.Show(msg, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Fields

        public Icon Icon = null;
        private List<IExtension> extensions;
        private SingleInstanceTracker tracker = null;
        private bool disposed = false;

        #endregion

        #region Properties

        public Form MainForm
        {
            get
            {
                return (MainForm)tracker.Enforcer;
            }
        }

        public NotifyIcon NotifyIcon
        {
            get
            {
                return ((UI.MainForm)MainForm).notifyIcon;
            }
        }

        public List<IExtension> Extensions
        {
            get
            {
                return extensions;
            }
        } 

        #endregion

        #region Methods

        public IExtension GetExtensionByType(Type type)
        {
            for (int i = 0; i < this.extensions.Count; i++)
            {
                if (this.extensions[i].GetType() == type)
                {
                    return this.extensions[i];
                }
            }

            return null;
        }

        private ISingleInstanceEnforcer GetSingleInstanceEnforcer()
        {
            return new MainForm();
        }

        public void InitExtensions()
        {
            for (int i = 0; i < Extensions.Count; i++)
            {
                if (Extensions[i] is IInitializable)
                {
                    ((IInitializable)Extensions[i]).Init();                   
                }
            }

        }
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                for (int i = 0; i < Extensions.Count; i++)
                {
                    if (Extensions[i] is IDisposable)
                    {
                        try
                        {
                            ((IDisposable)Extensions[i]).Dispose();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }
                }
            }
        }

        public void Start(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Attempt to create a tracker
                tracker = new SingleInstanceTracker("SingleInstanceSample", new SingleInstanceEnforcerRetriever(GetSingleInstanceEnforcer));

                // If this is the first instance of the application, run the main form
                if (tracker.IsFirstInstance)
                {
                    try
                    {
                        MainForm form = (MainForm)tracker.Enforcer;
                        
                        //form.downloadList1.AddDownloadURLs(ResourceLocation.FromURLArray(args), 1, null, 0);

                        if (Array.IndexOf<string>(args, "/as") >= 0)
                        {
                            form.WindowState = FormWindowState.Minimized;
                        }

                        form.Load += delegate(object sender, EventArgs e)
                            {
                                InitExtensions();

                                if (form.WindowState == FormWindowState.Minimized)
                                {
                                    form.HideForm();
                                }

                                if (args.Length > 0)
                                {
                                    form.OnMessageReceived(new MessageEventArgs(args));
                                }
                            };

                        form.FormClosing += delegate(object sender, FormClosingEventArgs e)
                            {
                                Dispose();
                            };

                        Application.Run(form);
                    }
                    finally
                    {
                        Dispose();
                    }
                }
                else
                {
                    // This is not the first instance of the application, so do nothing but send a message to the first instance
                    if (args.Length > 0)
                    {
                        tracker.SendMessageToFirstInstance(args);
                    }
                }
            }
            catch (SingleInstancingException ex)
            {
                MessageBox.Show("Could not create a SingleInstanceTracker object:\n" + ex.Message + "\nApplication will now terminate.\n" + ex.InnerException.ToString());

                return;
            }
            finally
            {
                if (tracker != null)
                    tracker.Dispose();
            }
        }

        #endregion
    }
}

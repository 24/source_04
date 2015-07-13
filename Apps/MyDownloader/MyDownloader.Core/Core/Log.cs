
namespace MyDownloader.Core
{
    public enum LogMode
    {
        Error,
        Information
    }

    public delegate void OnLogWriteDelegate(string msg, LogMode mode);

    public static class Log
    {
        public static OnLogWriteDelegate OnLogWrite;

        public static void Write(string msg, LogMode mode, params object[] prm)
        {
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            OnLogWrite(msg, mode);
        }
    }
}

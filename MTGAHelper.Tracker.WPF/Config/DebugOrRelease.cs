using System;
using System.Diagnostics;
using System.IO;

namespace MTGAHelper.Tracker.WPF.Config
{
    public class DebugOrRelease
    {
        public const string LocalServer = "https://localhost:5001";
        const string REMOTE_SERVER = "https://mtgahelper.com";

#if DEBUG && !DEBUGWITHSERVER
        public const string Server = LocalServer;
#else
        public const string Server = REMOTE_SERVER;
#endif

        public DebugOrRelease()
        {
            SetIfDebug();
        }

        public bool IsRelease => !IsDebug;
        public bool IsDebug { get; private set; }

        public string TestServer => LocalServer;

        [Conditional("DEBUG")]
        void SetIfDebug()
        {
            IsDebug = true;
        }

        public string GetConfigFolder()
        {
            return IsDebug
                ? Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper");
        }
    }
}

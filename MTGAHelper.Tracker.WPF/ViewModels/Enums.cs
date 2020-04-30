using System;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public enum WindowContext
    {
        Unknown,
        Welcome,
        Home,
        Drafting,
        Playing,
    }

    [Flags]
    public enum NetworkStatusEnum
    {
        Ready = 0,
        UpToDate = 1,
        Uploading = 2,
        Downloading = 4,
        ProcessingLogFile = 8,
    }

    [Flags]
    public enum ProblemsFlags
    {
        None = 0,
        LogFileNotFound = 1,
        SigninRequired = 2,
        ServerUnavailable = 4,
        GameClientFileNotFound = 8,
        DetailedLogsDisabled = 16,
    }
}

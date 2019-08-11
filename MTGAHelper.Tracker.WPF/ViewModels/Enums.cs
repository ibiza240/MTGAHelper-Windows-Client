using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public enum MainWindowContextEnum
    {
        Unknown,
        Welcome,
        Home,
        InCollection,
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
    }

    [Flags]
    public enum ProblemsFlags
    {
        None = 0,
        LogFileNotFound = 1,
        InvalidUserId = 2,
        ServerUnavailable = 4,
        GameClientFileNotFound = 8,
    }
}

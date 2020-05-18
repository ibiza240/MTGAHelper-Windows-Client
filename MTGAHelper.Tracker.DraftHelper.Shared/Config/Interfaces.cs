using System.Collections.Generic;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Config
{
    public interface IConfigFolderCommunication
    {
        string FolderCommunication { get; }
    }

    public interface IConfigFilepathConfigInput
    {
        string FilepathConfigInput { get; }
    }

    public interface IConfigFilepathConfigOutput
    {
        string FilepathConfigOutput { get; }
    }

    public interface IConfigFolderData
    {
        string FolderData { get; }
    }

    public interface IConfigFolderTemplates
    {
        string FolderTemplates { get; }
    }

    public interface IConfigResolutions
    {
        ICollection<ConfigResolution> ResolutionSettings { get; }
    }

    public interface IEmailProvider
    {
        string Email { get; set; }
    }
}

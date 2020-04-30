namespace MTGAHelper.Lib.Config
{
    public interface IDataPath
    {
        string FolderData { get; }
    }

    public class DataFolderPath : IDataPath
    {
        public DataFolderPath(string folderDataPath)
        {
            FolderData = folderDataPath;
        }

        public string FolderData { get; }
    }
}
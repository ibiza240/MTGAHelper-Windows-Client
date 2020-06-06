using System.Collections.Generic;
using System.IO;
using MTGAHelper.Entity;
using MTGAHelper.Entity.Config.App;
using MTGAHelper.Lib;
using Newtonsoft.Json;

namespace MTGAHelper.Tracker.WPF.Business
{
    //public class CacheLoaderConfigResolutions : ICacheLoader<ICollection<ConfigResolution>>
    //{
    //    private readonly string FolderData;

    //    public CacheLoaderConfigResolutions(IDataPath dataPath)
    //    {
    //        FolderData = dataPath.FolderData;
    //    }

    //    public ICollection<ConfigResolution> LoadData()
    //    {
    //        string configResolutionsContent =
    //            File.ReadAllText(Path.Combine(FolderData, $"{DataFileTypeEnum.ConfigResolutions}.json"));
    //        var configResolutions = JsonConvert.DeserializeObject<ConfigResolutions>(configResolutionsContent);
    //        return configResolutions.ResolutionSettings;
    //    }
    //}
}
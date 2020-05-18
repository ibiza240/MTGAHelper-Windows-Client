using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.Config;
using MTGAHelper.Tracker.DraftHelper.Shared.Config;
using MTGAHelper.Tracker.WPF.Config;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class CacheLoaderConfigResolutions : ICacheLoader<ICollection<ConfigResolution>>
    {
        private readonly string FolderData;

        public CacheLoaderConfigResolutions(IDataPath dataPath)
        {
            FolderData = dataPath.FolderData;
        }

        public ICollection<ConfigResolution> LoadData()
        {
            string configResolutionsContent =
                File.ReadAllText(Path.Combine(FolderData, $"{DataFileTypeEnum.ConfigResolutions}.json"));
            var configResolutions = JsonConvert.DeserializeObject<ConfigResolutions>(configResolutionsContent);
            return configResolutions.ResolutionSettings;
        }
    }
}
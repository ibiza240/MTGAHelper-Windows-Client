using System.Collections.Generic;
using System.IO;
using MTGAHelper.Entity;
using MTGAHelper.Entity.Config.App;
using MTGAHelper.Lib.Logging;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.CacheLoaders
{
    public class CacheLoaderDraftRatings : ICacheLoader<Dictionary<string, DraftRatings>>
    {
        readonly string folderData;

        public CacheLoaderDraftRatings(IDataPath folderData)
        {
            this.folderData = folderData.FolderData;
        }

        public Dictionary<string, DraftRatings> LoadData()
        {
            var fileSets = Path.Combine(folderData, "draftRatings.json");
            LogExt.LogReadFile(fileSets);
            var content = File.ReadAllText(fileSets);
            var ratingsBySource = JsonConvert.DeserializeObject<Dictionary<string, DraftRatings>>(content);

            return ratingsBySource;
        }
    }
}

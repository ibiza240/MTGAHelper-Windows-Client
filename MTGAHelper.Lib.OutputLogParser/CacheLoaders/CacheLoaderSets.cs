using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.Config;
using MTGAHelper.Lib.Logging;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.CacheLoaders
{
    public class CacheLoaderSets : ICacheLoader<Dictionary<int, Set>>
    {
        readonly string folderData;

        public CacheLoaderSets(IDataPath folderData)
        {
            this.folderData = folderData.FolderData;
        }

        public Dictionary<int, Set> LoadData()
        {
            var fileSets = Path.Combine(folderData, "sets.json");
            LogExt.LogReadFile(fileSets);
            var content = File.ReadAllText(fileSets);
            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(content);

            var dataCurated = data
                .Where(i => string.IsNullOrWhiteSpace(i.Key) == false)
                .Where(i => Convert.ToInt32(i.Value.collation) > 0)
                .Select(i => JsonConvert.DeserializeObject<Set>(JsonConvert.SerializeObject(i.Value)))
                .Cast<Set>()
                .ToArray();

            return dataCurated.ToDictionary(i => i.Collation, i => i);
        }
    }
}

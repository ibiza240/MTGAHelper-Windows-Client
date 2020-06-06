using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Entity.Config.App;
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
            var data = JsonConvert.DeserializeObject<ICollection<dynamic>>(content);

            var dataCurated = data
                .Where(i => string.IsNullOrWhiteSpace(i.Code?.ToString()) == false)
                .Where(i => Convert.ToInt32(i.MtgaId ?? 0) > 0)
                .Select(i => new Set
                {
                    Arenacode = i.CodeArena,
                    Collation = i.MtgaId,
                    Code = i.Code,
                    Release = i.ReleaseDate,
                    Scryfall = i.CodeScryfall,
                    Tile = 0
                })
                .ToArray();

            var ret = dataCurated.ToDictionary(i => i.Collation, i => i);
            return ret;
        }
    }
}

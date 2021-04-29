using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Entity.Config.App;
using MTGAHelper.Lib.Logging;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.CacheLoaders
{
    public class CacheLoaderAllCards : ICacheLoader<Dictionary<int, Card>>
    {
        readonly string folderData;

        public CacheLoaderAllCards(IDataPath config)
        {
            folderData = config.FolderData;
        }

        public Dictionary<int, Card> LoadData()
        {
            var fileSets = Path.Combine(folderData, "AllCardsCached2.json");
            LogExt.LogReadFile(fileSets);
            var content = File.ReadAllText(fileSets);
            var dataOldFormat = JsonConvert.DeserializeObject<ICollection<Card2>>(content)
                .Select(c => new Card(c))
                .ToArray();

            // Patch to support multiple cards with an Arena Id of 0
            var iNewId = dataOldFormat.Max(i => i.grpId) + 1;
            foreach (var cardMissingId in dataOldFormat.Where(i => i.grpId == 0).Skip(1))
            {
                cardMissingId.grpId = iNewId++;
            }

            return dataOldFormat.ToDictionary(i => i.grpId);
        }
    }
}

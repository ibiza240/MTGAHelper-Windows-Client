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
            var data = JsonConvert.DeserializeObject<ICollection<Card2>>(content);

            var dataOldFormat = data.Select(c => new Card(c)).ToDictionary(i => i.grpId);
            return dataOldFormat;
        }
    }
}

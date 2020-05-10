using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.IoC;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.Config;
using MTGAHelper.Lib.Logging;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.CacheLoaders
{
    public class CacheLoaderAllCards : ICacheLoader<Dictionary<int, Card>>
    {
        readonly IMapper mapper;

        readonly string folderData;

        public CacheLoaderAllCards(IDataPath config)
        {
            folderData = config.FolderData;
            mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfileOldCardFormat())).CreateMapper();
        }

        public Dictionary<int, Card> LoadData()
        {
            var fileSets = Path.Combine(folderData, "AllCardsCached2.json");
            LogExt.LogReadFile(fileSets);
            var content = File.ReadAllText(fileSets);
            var data = JsonConvert.DeserializeObject<ICollection<Card2>>(content);

            var dataOldFormat = mapper.Map<ICollection<Card>>(data).ToDictionary(i => i.grpId, i => i);
            return dataOldFormat;
        }
    }
}

using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity.Config.Users;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class CollectionResponse
    {
        public string PlayerName { get; set; }
        public string CollectionDate { get; set; }
        public string LastUploadHash { get; set; }
        public ICollection<CollectionCardDto> Cards { get; set; } = new CollectionCardDto[0];
        public InventoryResponseDto Inventory
        { get; set; } = new InventoryResponseDto();

        public RankInfoDto ConstructedRank { get; set; }
        public RankInfoDto LimitedRank { get; set; }
        public Dictionary<string, PlayerProgressDto> PlayerProgress { get; set; }

        public CollectionResponse()
        {
            // For Serialization
        }

        public CollectionResponse(
            string playerName,
            DateTime date,
            string lastUploadHash,
            IMapper mapper,
            ICollection<CardWithAmount> collection,
            Inventory inventory,
            IReadOnlyCollection<ConfigModelRankInfo> ranks,
            IReadOnlyDictionary<string, PlayerProgress> playerProgress)
        {
            PlayerName = playerName;
            CollectionDate = date.ToString("yyyy-MM-dd HH:mm:ss");
            LastUploadHash = lastUploadHash;
            Cards = mapper.Map<ICollection<CollectionCardDto>>(collection);

            //if (Cards.Any(i => i.Color == null))
            //{
            //    var t = Cards.Where(i => i.Color == null).ToArray();
            //    System.Diagnostics.Debugger.Break();
            //}

            Inventory = mapper.Map<InventoryResponseDto>(inventory ?? new Inventory());
            ConstructedRank = mapper.Map<RankInfoDto>(ranks.FirstOrDefault(i => i.Format == RankFormatEnum.Constructed) ?? new ConfigModelRankInfo());
            LimitedRank = mapper.Map<RankInfoDto>(ranks.FirstOrDefault(i => i.Format == RankFormatEnum.Limited) ?? new ConfigModelRankInfo());
            PlayerProgress = mapper.Map<Dictionary<string, PlayerProgressDto>>(playerProgress);
        }
    }

    public class RankInfoDto
    {
        public string Class { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
        public float Percentile { get; set; }
    }

    public class PlayerProgressDto
    {
        public string TrackName { get; set; }
        public int CurrentLevel { get; set; }
        public int CurrentExp { get; set; }
        public int CurrentOrbCount { get; set; }
    }
}

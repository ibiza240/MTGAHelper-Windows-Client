using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Config.Users;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class CollectionResponse
    {
        public string CollectionDate { get; set; }
        public string LastUploadHash { get; set; }
        public ICollection<CollectionCardDto> Cards { get; set; } = new CollectionCardDto[0];
        public InventoryResponseDto MtgaUserProfile { get; set; } = new InventoryResponseDto();

        public RankInfoDto ConstructedRank { get; set; }
        public RankInfoDto LimitedRank { get; set; }
        public Dictionary<string, PlayerProgressDto> PlayerProgress { get; set; }

        public CollectionResponse()
        {
            // For Serialization
        }

        public CollectionResponse(
            DateTime date,
            string lastUploadHash,
            ICollection<CardWithAmount> collection,
            Inventory mtgaUserProfile,
            ICollection<ConfigModelRankInfo> ranks,
            Dictionary<string, PlayerProgress> playerProgress)
        {
            CollectionDate = date.ToString("yyyy-MM-dd HH:mm:ss");
            LastUploadHash = lastUploadHash;
            Cards = Mapper.Map<ICollection<CollectionCardDto>>(collection);

            //if (Cards.Any(i => i.Color == null))
            //{
            //    var t = Cards.Where(i => i.Color == null).ToArray();
            //    System.Diagnostics.Debugger.Break();
            //}

            MtgaUserProfile = mtgaUserProfile == null ? new InventoryResponseDto() : Mapper.Map<InventoryResponseDto>(mtgaUserProfile);
            ConstructedRank = Mapper.Map<RankInfoDto>(ranks.First(i => i.Format == ConfigModelRankInfoFormatEnum.Constructed));
            LimitedRank = Mapper.Map<RankInfoDto>(ranks.First(i => i.Format == ConfigModelRankInfoFormatEnum.Limited));
            PlayerProgress = Mapper.Map<Dictionary<string, PlayerProgressDto>>(playerProgress);
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

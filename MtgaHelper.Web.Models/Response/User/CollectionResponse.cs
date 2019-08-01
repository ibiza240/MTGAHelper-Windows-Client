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
        public Inventory MtgaUserProfile { get; set; } = new Inventory();

        public RankInfoDto ConstructedRank { get; set; }
        public RankInfoDto LimitedRank { get; set; }

        public CollectionResponse()
        {
            // For Serialization
        }

        public CollectionResponse(
            DateTime date,
            string lastUploadHash,
            ICollection<CardWithAmount> collection,
            Inventory mtgaUserProfile,
            ICollection<ConfigModelRankInfo> ranks)
        {
            CollectionDate = date.ToString("yyyy-MM-dd HH:mm:ss");
            LastUploadHash = lastUploadHash;
            Cards = Mapper.Map<ICollection<CollectionCardDto>>(collection);

            //if (Cards.Any(i => i.Color == null))
            //{
            //    var t = Cards.Where(i => i.Color == null).ToArray();
            //    System.Diagnostics.Debugger.Break();
            //}

            MtgaUserProfile = mtgaUserProfile ?? new Inventory();
            ConstructedRank = Mapper.Map<RankInfoDto>(ranks.First(i => i.Format == ConfigModelRankInfoFormatEnum.Constructed));
            LimitedRank = Mapper.Map<RankInfoDto>(ranks.First(i => i.Format == ConfigModelRankInfoFormatEnum.Limited));
        }
    }

    public class RankInfoDto
    {
        public string Class { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
        public float Percentile { get; set; }
    }
}

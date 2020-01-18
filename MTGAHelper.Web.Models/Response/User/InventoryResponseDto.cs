using System.Collections.Generic;
using MTGAHelper.Entity;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class InventoryBoosterDto
    {
        public string Set { get; set; }
        public int Count { get; set; }
    }

    public class InventoryResponseDto
    {
        public string PlayerId { get; set; }
        public int Gold { get; set; }
        public int Gems { get; set; }
        public int DraftTokens { get; set; }
        public int SealedTokens { get; set; }
        public int WcTrackPosition { get; set; }
        public float VaultProgress { get; set; }

        public ICollection<InventoryBoosterDto> Boosters { get; set; }

        public Dictionary<string, int> Wildcards { get; set; } = new Dictionary<string, int>
        {
            {  RarityEnum.Mythic.ToString(), 0 },
            {  RarityEnum.Rare.ToString(), 0 },
            {  RarityEnum.Uncommon.ToString(), 0 },
            {  RarityEnum.Common.ToString(),0 },
        };
    }
}
using MTGAHelper.Entity.OutputLogParsing;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class GetPlayerInventoryRaw
    {
        public string playerId { get; set; }
        public int wcCommon { get; set; }
        public int wcUncommon { get; set; }
        public int wcRare { get; set; }
        public int wcMythic { get; set; }
        public int gold { get; set; }
        public int gems { get; set; }
        public int draftTokens { get; set; }
        public int sealedTokens { get; set; }
        public int wcTrackPosition { get; set; }
        public float vaultProgress { get; set; }
        public ICollection<BoosterDelta> boosters { get; set; }

        /*
            "vanityItems": {
                "pets": [],
                "avatars": [],
                "cardBacks": []
              },
              "vanitySelections": {
                "avatarSelection": null,
                "avatarModSelection": null,
                "cardBackSelection": null,
                "cardBackModSelection": null,
                "petSelection": null,
                "petModSelection": null
              }
        */
    }
}

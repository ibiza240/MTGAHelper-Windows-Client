using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.InventoryUpdated;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser.Readers.MTGAProLogger
{
    public class InventoryUpdateConverter : GenericConverter<InventoryUpdatedResult, PayloadRaw<InventoryUpdatedRaw>>, IMessageReaderMtgaProLogger
    {
        public override string LogTextKey => "**InventoryUpdate**";

        public override IEnumerable<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            //if (part.Contains("Mercantile"))
            //    System.Diagnostics.Debugger.Break();

            var json = GetJson(part);
            var update = JsonConvert.DeserializeObject<PayloadRaw<Update>>(json);
            var payload = update.payload;

            var d = JsonConvert.DeserializeObject<dynamic>(json);
            var aetherizedCards = (JArray)d.Payload.aetherizedCards;
            var vaultProgress = aetherizedCards.Sum(i => (float)i["vaultProgress"] / 1000) * 100;

            payload.delta.vaultProgressDelta = vaultProgress;

            return new[] { new InventoryUpdatedResult
            {
                Timestamp = update.timestamp,
                Raw = new PayloadRaw<InventoryUpdatedRaw>
                {
                    payload = new InventoryUpdatedRaw
                    {
                        timestamp = update.timestamp,
                        context = payload.context.source,
                        updates = new [] { payload }
                    }
                }
            }};
        }
    }
}
using System.Text.RegularExpressions;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class ClientToMatchConverterGeneric : GenericConverter<ClientToMatchResultGeneric, ClientToMatchRawGeneric>, IMessageReaderUnityCrossThreadLogger
    {
        readonly Regex regexClientToMatch = new Regex(@"^([A-Z0-9]+) to Match: (\w+)");
        public override string LogTextKey => " to Match";
        public override bool DoesParse(string part)
        {
            return regexClientToMatch.Match(part).Success;
        }

        readonly ClientToMatchConverter<PayloadSubmitDeckResp> converterSubmitDeck;
        readonly ClientToMatchConverter<PayloadEnterSideboardingReq> converterEnterSideboarding;

        public ClientToMatchConverterGeneric(
            ClientToMatchConverter<PayloadSubmitDeckResp> converterSubmitDeck,
            ClientToMatchConverter<PayloadEnterSideboardingReq> converterEnterSideboarding
        )
        {
            this.converterSubmitDeck = converterSubmitDeck;
            this.converterEnterSideboarding = converterEnterSideboarding;
        }

        public override IMtgaOutputLogPartResult ParseJson(string json)
        {
            if (json.Contains("ClientMessageType_SubmitDeckResp"))
                return converterSubmitDeck.ParseJson(json);
            else if (json.Contains("ClientMessageType_EnterSideboardingReq"))
                return converterEnterSideboarding.ParseJson(json);
            // TODO else if (json.Contains("ClientMessageType_SearchResp"))
            else
                return new IgnoredResult();
        }
    }

    public class ClientToMatchConverter<TPayload> : GenericConverter<ClientToMatchResult<TPayload>, ClientToMatchRaw<TPayload>>
    {
        public override string LogTextKey { get; }
    }
}

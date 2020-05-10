using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch.Raw;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch
{
    public class ClientToMatchConverterGeneric : GenericConverter<ClientToMatchResultGeneric, ClientToMatchRawGeneric>
    {
        private readonly ClientToMatchConverter<PayloadSubmitDeckResp> converterSubmitDeck;
        private readonly ClientToMatchConverter<PayloadEnterSideboardingReq> converterEnterSideboarding;

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
            else
            if (json.Contains("ClientMessageType_EnterSideboardingReq"))
                return converterEnterSideboarding.ParseJson(json);
            else
            return new IgnoredResult();
        }
    }

    public class ClientToMatchConverter<TPayload> : GenericConverter<ClientToMatchResult<TPayload>, ClientToMatchRaw<TPayload>>
    {
    }
}

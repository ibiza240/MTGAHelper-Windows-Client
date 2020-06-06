using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers
{
    public class AuthenticateResponseConverter : GenericConverter<AuthenticateResponseResult, AuthenticateResponseRaw>
    {
        public override string LogTextKey => "AuthenticateResponse";
    }
}

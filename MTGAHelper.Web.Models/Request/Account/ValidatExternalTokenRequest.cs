namespace MTGAHelper.Web.Models.Request.Account
{
    public class ValidatExternalTokenRequest
    {
        public string Token { get; set; }

        public ValidatExternalTokenRequest()
        {
        }

        public ValidatExternalTokenRequest(string token)
        {
            Token = token;
        }
    }
}
namespace WebApplication1.Model.Account
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

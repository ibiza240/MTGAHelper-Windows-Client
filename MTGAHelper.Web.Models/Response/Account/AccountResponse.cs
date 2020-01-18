namespace MTGAHelper.Web.Models.Response.Account
{
    public class AccountResponse : ResponseBase
    {
        public string MtgaHelperUserId { get; set; }
        public string Provider { get; set; }
        public string Email { get; set; }
        //public string SubjectId { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Status { get; set; }

        public AccountResponse WithAuthenticated()
        {
            IsAuthenticated = true;
            return this;
        }
    }
}

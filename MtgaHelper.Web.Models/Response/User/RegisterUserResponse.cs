using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class RegisterUserResponse : ErrorResponse
    {
        public string UserId { get; set; }
        public int NbLogin { get; set; }
        public bool ChangesSinceLastLogin { get; set; }
        public ICollection<string> NotificationsInactive { get; set; } = new string[0];

        public RegisterUserResponse(string userId, int nbLogin, bool changesSinceLastLogin)
        {
            UserId = userId;
            NbLogin = nbLogin;
            ChangesSinceLastLogin = changesSinceLastLogin;
        }
    }
}

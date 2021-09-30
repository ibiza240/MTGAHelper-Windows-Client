using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.User
{
    public class RegisterUserResponse : ErrorResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int NbLogin { get; set; }
        public bool ChangesSinceLastLogin { get; set; }
        public ICollection<string> NotificationsInactive { get; set; } = new string[0];
        public bool IsSupporter { get; set; }

        public RegisterUserResponse(string userId, string email, int nbLogin, bool changesSinceLastLogin, bool isSupporter)
        {
            UserId = userId;
            Email = email;
            NbLogin = nbLogin;
            ChangesSinceLastLogin = changesSinceLastLogin;
            IsSupporter = isSupporter;
        }
    }
}
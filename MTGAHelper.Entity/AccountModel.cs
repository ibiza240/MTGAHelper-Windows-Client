using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public enum AccountStatusEnum
    {
        Active,
        Inactive,
        WaitingForVerificationCode,
        ForgotPassword,
    }

    public class AccountModel
    {
        //public string SubjectId { get; set; }
        //public string Username { get; set; }
        public string PasswordHashed { get; set; }
        public string Salt { get; set; }

        public string Provider { get; set; }
        //public string ProviderSubjectId { get; set; }

        public AccountStatusEnum Status { get; set; }
        public string VerificationCode { get; set; }

        //public List<Claim> Claims { get; set; }

        public string Email { get; set; }
        //public string MtgaUsername { get; set; }
        public string MtgaHelperUserId { get; set; }

        //public DateTime LastLogin { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MTGAHelper.Entity
{
    public class PasswordHasher
    {
        public string Hash(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var passwordHashed = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            return passwordHashed;
        }
    }
}

using System;
using System.Security.Cryptography;

namespace MTGAHelper.Lib
{
    // TODO: move out of Entity
    public class PasswordHasher
    {
        public string Hash(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordHashed = Hash(password, saltBytes);
            return passwordHashed;
        }

        public (string salt, string hash) GenerateSaltAndHash(string password)
        {
            var saltBytes = new byte[64];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetNonZeroBytes(saltBytes);
            }

            var salt = Convert.ToBase64String(saltBytes);
            var hashPassword = Hash(password, saltBytes);

            return (salt, hashPassword);
        }

        static string Hash(string password, byte[] saltBytes)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000))
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
        }
    }
}

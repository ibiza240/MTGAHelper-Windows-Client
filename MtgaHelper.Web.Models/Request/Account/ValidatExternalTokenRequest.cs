using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

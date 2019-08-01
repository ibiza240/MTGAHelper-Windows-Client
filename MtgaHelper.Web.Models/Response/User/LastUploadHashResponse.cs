using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Web.Models.Response.User
{
    public class LastUploadHashResponse
    {
        public string LastUploadHash { get; set; }

        public LastUploadHashResponse(string lastUploadHash)
        {
            LastUploadHash = lastUploadHash;
        }
    }
}

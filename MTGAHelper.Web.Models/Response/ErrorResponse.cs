using System;

namespace MTGAHelper.Web.Models.Response
{
    public interface IResponse
    {
    }

    [Serializable]
    public class ErrorResponse : IResponse
    {
        public string Error { get; set; }

        public ErrorResponse()
        {
        }

        public ErrorResponse(string errorMessage)
        {
            Error = errorMessage;
        }
    }
}
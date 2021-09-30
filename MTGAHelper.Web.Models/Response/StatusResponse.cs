namespace MTGAHelper.Web.Models.Response
{
    public class StatusResponse : IResponse
    {
        public string Status { get; set; }

        public StatusResponse()
        {
        }

        public StatusResponse(string status)
        {
            Status = status;
        }
    }
}
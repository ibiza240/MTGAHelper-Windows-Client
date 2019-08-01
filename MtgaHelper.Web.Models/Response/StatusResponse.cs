namespace MTGAHelper.Web.UI.Model.Response
{
    public class StatusResponse
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

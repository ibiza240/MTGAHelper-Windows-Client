namespace MTGAHelper.Web.UI.Model.Request
{
    public enum ErrorTypeEnum
    {
        Unknown,
        Startup,
        OutputLogParsing,
    }

    public class PostLogRemoteErrorRequest
    {
        public ErrorTypeEnum ErrorType { get; set; }
        public string Exception { get; set; }
        public string UserId { get; set; }
        public string Filename { get; set; }
    }
}

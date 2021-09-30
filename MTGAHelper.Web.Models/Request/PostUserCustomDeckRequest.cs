namespace MTGAHelper.Web.Models.Request
{
    public class PostUserCustomDeckRequest
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string MtgaFormat { get; set; }
    }
}
namespace MTGAHelper.Web.UI.Model.Response
{
    public class UserCustomScraperResponse
    {
        public int NbTotal { get; set; }
        public int NbSkipped { get; set; }

        public UserCustomScraperResponse(int nbTotal, int nbSkipped)
        {
            NbTotal = nbTotal;
            nbSkipped = NbSkipped;
        }
    }
}

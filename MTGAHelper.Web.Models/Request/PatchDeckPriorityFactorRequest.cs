namespace MTGAHelper.Web.Models.Request
{
    public class PatchDeckPriorityFactorRequest
    {
        public string DeckId { get; set; }
        public float Value { get; set; }
    }
}
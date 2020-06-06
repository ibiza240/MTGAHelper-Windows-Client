namespace MTGAHelper.Lib.OutputLogParser.Models
{
    [AddMessageEvenIfDateNull]
    public class PlayerNameResult : MtgaOutputLogPartResultBase<string>, IMtgaOutputLogPartResult
    {
        public string Name { get; set; }
        public string AccountId { get; set; }
    }
}

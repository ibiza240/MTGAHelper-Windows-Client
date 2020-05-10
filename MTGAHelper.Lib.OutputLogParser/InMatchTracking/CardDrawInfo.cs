namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public class CardDrawInfo
    {
        public CardDrawInfo(int grpId, int amount, float drawChance = 0f)
        {
            GrpId = grpId;
            Amount = amount;
            DrawChance = drawChance;
        }

        public int GrpId { get; }
        public int Amount { get; }
        public float DrawChance { get; }
    }
}
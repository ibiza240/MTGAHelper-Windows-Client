namespace MTGAHelper.Entity.OutputLogParsing
{
    public class PayloadRaw<T>
    {
        public int id { get; set; }
        public T payload { get; set; }
    }

}

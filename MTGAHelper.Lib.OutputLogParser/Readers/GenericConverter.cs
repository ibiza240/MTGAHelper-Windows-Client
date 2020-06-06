using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser.Readers
{
    public abstract class GenericConverter<T, TRaw> : ConverterBase<T, TRaw> where T : MtgaOutputLogPartResultBase<TRaw>, new()
    {
        protected override T CreateT(TRaw raw)
        {
            return new T { Raw = raw, LogTextKey = LogTextKey };
        }
    }
}

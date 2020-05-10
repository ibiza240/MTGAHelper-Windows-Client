namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public class GenericConverter<T, TRaw> : ConverterBase<T, TRaw> where T : MtgaOutputLogPartResultBase<TRaw>, new()
    {
        protected override T CreateT(TRaw raw)
        {
            return new T { Raw = raw };
        }
    }
}

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class StateChangedConverter : GenericConverter<MtgaOutputLogPartResultBase<string>, string>
    {
        public StateChangedConverter()
        {
            IsJson = false;
        }
    }
}

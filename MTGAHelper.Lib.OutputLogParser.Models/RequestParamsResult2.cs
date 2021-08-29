using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    public class RequestParamsResult2<T> : MtgaOutputLogPartResultBase<RequestRaw2<T>>
    {
        public override RequestRaw2<T> Raw
        {
            get => base.Raw;
            set
            {
                base.Raw = value;
                RequestParams = value.FetchPayload();
            }
        }

        public T RequestParams { get; private set; }
    }
}
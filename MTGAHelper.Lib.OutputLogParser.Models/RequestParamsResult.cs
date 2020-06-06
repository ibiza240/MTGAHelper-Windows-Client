using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    public class RequestParamsResult<T> : MtgaOutputLogPartResultBase<RequestRaw<T>>
    {
        public override RequestRaw<T> Raw
        {
            get => base.Raw;
            set
            {
                base.Raw = value;
                RequestParams = value.FetchParams();
            }
        }

        public T RequestParams { get; private set; }
    }
}


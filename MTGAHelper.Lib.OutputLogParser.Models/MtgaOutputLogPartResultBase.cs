using System;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    public interface IMtgaOutputLogPartResult
    {
        string Part { get; }
        string Prefix { get; }
        DateTime LogDateTime { get; }
        string SubPart { get; set; }
        long Timestamp { get; set; }
        string MatchId { get; set; }
        string LogTextKey { get; set; }

        IMtgaOutputLogPartResult SetCommonFields(string part, string prefix, DateTime logDateTime);
    }

    public interface IMtgaOutputLogPartResult<T> : IMtgaOutputLogPartResult
    {
        T Raw { get; set; }
    }

    public class MtgaOutputLogPartResultBase<T> : IMtgaOutputLogPartResult<T>
    {
        //public virtual ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.Unknown;
        public string Part { get; private set; }
        public string SubPart { get; set; }
        public string Prefix { get; private set; }
        public long Timestamp { get; set; }
        public string MatchId { get; set; }
        public string LogTextKey { get; set; }
        public DateTime LogDateTime { get; set; }
        public virtual T Raw { get; set; }

        //public bool IsMessageSummarized { get; set; }

        public MtgaOutputLogPartResultBase()
        {
        }

        public MtgaOutputLogPartResultBase(long timestamp)
        {
            Timestamp = timestamp;
        }

        public IMtgaOutputLogPartResult SetCommonFields(string part, string prefix, DateTime logDateTime)
        {
            Part = part;
            Prefix = prefix;
            LogDateTime = logDateTime;
            return this;
        }
    }

    [AddMessageEvenIfDateNull]
    public class DetailedLoggingResult : MtgaOutputLogPartResultBase<string>
    {
        public DetailedLoggingResult(bool isEnabled)
        {
            IsDetailedLoggingEnabled = isEnabled;
        }

        public bool IsDetailedLoggingEnabled { get; }
        public bool IsDetailedLoggingDisabled => !IsDetailedLoggingEnabled;
    }

    public class IgnoredResult : MtgaOutputLogPartResultBase<string>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.Ignored;

        public IgnoredResult(long timestamp = 0)
            : base(timestamp)
        {
        }
    }

    public class IgnoredResultRequestToServer : IgnoredResult
    {
        public IgnoredResultRequestToServer(long timestamp = 0)
            : base(timestamp)
        {
        }
    }

    public class UnknownResult : MtgaOutputLogPartResultBase<string>
    {
        public UnknownResult(long timestamp = 0)
            : base(timestamp)
        {
        }
    }

    public class IgnoredMatchResult : IgnoredResult, ITagMatchResult
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.Ignored;

        public IgnoredMatchResult(long timestamp = 0)
            : base(timestamp)
        {
        }
    }

    public class SummarizedMatchResult : MtgaOutputLogPartResultBase<string>, ITagMatchResult
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.Ignored;

        public SummarizedMatchResult(long timestamp = 0)
            : base(timestamp)
        {
        }
    }

    public class UnknownMatchResult : UnknownResult, ITagMatchResult
    {
        public UnknownMatchResult(long timestamp = 0)
            : base(timestamp)
        {
        }
    }

    public class IgnoredClientToMatchResult : IgnoredMatchResult, ITagMatchResult
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.Ignored;

        public IgnoredClientToMatchResult(long timestamp = 0)
            : base(timestamp)
        {
        }
    }
}

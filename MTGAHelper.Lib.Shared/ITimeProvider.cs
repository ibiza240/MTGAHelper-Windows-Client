using System;

namespace MTGAHelper.Lib
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    public class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

using System;

namespace MTGAHelper.Tracker.WPF.Exceptions
{
    public class DraftHelperMockException : Exception
    {
        public DraftHelperMockException()
            : base("The DraftHelper for Human drafting (Premier or Traditional) is currently available as a perk to supporters. You can check the About window for how to become a supporter. Thanks!")
        {
        }
    }
}

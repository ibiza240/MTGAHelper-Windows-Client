namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftingVMPxpx
    {
        public int PackNumber { get; set; }
        public int PickNumber { get; set; }

        public string Label => $"P{PackNumber + 1} p{PickNumber + 1}";
    }
}

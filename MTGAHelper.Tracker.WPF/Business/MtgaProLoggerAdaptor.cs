using System.Diagnostics;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class MtgaProLoggerAdaptor
    {
        private readonly Process process;

        public MtgaProLoggerAdaptor()
        {
            process = new Process();
            process.StartInfo.FileName = @"getFrontWindow.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
        }

        public void Start()
        {
            process.Start();
        }

        public void Stop()
        {
            process.Kill();
        }
    }
}
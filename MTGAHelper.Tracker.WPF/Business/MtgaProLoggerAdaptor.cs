using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class MtgaProLoggerAdaptor
    {
        public bool IsStarted => process != default;

        private Process process = default;

        public MtgaProLoggerAdaptor()
        {
        }

        public async Task Start()
        {
            if (Process.GetProcessesByName("MTGA").Any())
            {
                await Task.Delay(15000);
                if (IsStarted == false)
                {
                    KillOthers();

                    process = new Process();
                    process.StartInfo.FileName = @"getFrontWindow.exe";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;

                    //File.WriteAllText(Path.GetTempFileName(), $"Starting {Path.GetFullPath(process.StartInfo.FileName)}");

                    process.Start();
                }
            }
        }

        public void Stop()
        {
            if (process != default)
            {
                process.Kill();
                process = default;
            }

            KillOthers();
        }

        private static void KillOthers()
        {
            var others = Process.GetProcessesByName("getFrontWindow");
            foreach (var p in others)
                p.Kill();
        }
    }
}
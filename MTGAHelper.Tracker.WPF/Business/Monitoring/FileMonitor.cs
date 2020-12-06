using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace MTGAHelper.Tracker.WPF.Business.Monitoring
{
    public class FileMonitor
    {
        private readonly object lockFilePath = new object();

        private string filePath;

        private long lastSize;

        private bool initialLoad = true;

        public StringBuilder LogContentToSend { get; private set; } = new StringBuilder();

        public event Action<object, string> OnFileSizeChangedNewText;

        public void SetFilePath(string filePath)
        {
            lock (lockFilePath)
                this.filePath = filePath;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await ContinuallyCheckLogFile(cancellationToken);
        }

        private async Task ContinuallyCheckLogFile(CancellationToken cancellationToken)
        {
            //Task task = null;

            await Task.Run(() =>
            {
                while (true)
                {
                    Task.Delay(1000, cancellationToken).Wait(cancellationToken);

                    lock (lockFilePath)
                    {
                        if (filePath == null) continue;

                        try
                        {
                            if (cancellationToken.IsCancellationRequested)
                                throw new TaskCanceledException();// (task);

                            if (File.Exists(filePath) == false)
                                continue;

                            ReadFile();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }, cancellationToken).ConfigureAwait(false);
        }

        private void ReadFile()
        {
            var fileSize = new FileInfo(filePath).Length;
            if (fileSize == lastSize || fileSize == 0) return;

            string newText;

            if (fileSize < lastSize)
            {
                // New file was created, start at begin
                lastSize = 0;
                LogContentToSend = new StringBuilder();
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var readUntil = fs.Length;
                var readCount = readUntil - lastSize;
                var b = new byte[readCount];

                fs.Seek(lastSize, SeekOrigin.Begin);
                fs.Read(b, 0, (int)readCount);

                newText = Encoding.UTF8.GetString(b);

                lastSize = readUntil;
                LogContentToSend.Append(newText);
            }

            if (initialLoad == false)
            {
                try
                {
                    OnFileSizeChangedNewText?.Invoke(this, newText);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Unexpected error:");
                    //System.Diagnostics.Debugger.Break();
                    throw;
                }
            }

            initialLoad = false;
        }

        public void ResetStringBuilder()
        {
            LogContentToSend = new StringBuilder();
        }
    }
}

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
        private readonly object LockFilePath = new object();

        private string FilePath;

        private long LastSize;

        private bool InitialLoad = true;

        public StringBuilder LogContentToSend { get; private set; } = new StringBuilder();

        public event Action<object, string> OnFileSizeChangedNewText;

        public FileMonitor()
        {
            //filePath = configApp.CurrentValue.LogFilePath;
        }

        public void SetFilePath(string filePath)
        {
            lock (LockFilePath)
                FilePath = filePath;
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

                    lock (LockFilePath)
                    {
                        if (FilePath == null) continue;

                        try
                        {
                            if (cancellationToken.IsCancellationRequested)
                                throw new TaskCanceledException();// (task);

                            if (File.Exists(FilePath) == false)
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
            long fileSize = new FileInfo(FilePath).Length;
            if (fileSize == LastSize) return;

            string newText;

            if (fileSize < LastSize)
            {
                // New file was created
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using var reader = new StreamReader(fs);
                    newText = reader.ReadToEnd();
                }

                LastSize = new FileInfo(FilePath).Length;
                LogContentToSend = new StringBuilder(newText);
            }
            else
            {
                // File got bigger
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long newTextSize = fs.Length - LastSize;
                    var b = new byte[newTextSize];

                    fs.Seek(LastSize, SeekOrigin.Begin);
                    fs.Read(b, 0, (int)(newTextSize));

                    newText = Encoding.UTF8.GetString(b);
                }

                LastSize = fileSize;
                LogContentToSend.Append(newText);
            }

            if (InitialLoad == false)
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

            InitialLoad = false;
        }

        public void ResetStringBuilder()
        {
            LogContentToSend = new StringBuilder();
        }
    }
}

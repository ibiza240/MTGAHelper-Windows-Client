using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Business.Monitoring
{
    public class FileMonitor
    {
        readonly object lockFilePath = new object();
        string filePath = null;
        long lastSize;
        bool initialLoad = true;

        public StringBuilder LogContentToSend { get; private set; } = new StringBuilder();

        public event Action<object, string> OnFileSizeChangedNewText;

        public FileMonitor()
        {
            //filePath = configApp.CurrentValue.LogFilePath;
        }

        public void SetFilePath(string filePath)
        {
            lock (lockFilePath)
                this.filePath = filePath;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await ContinuallyCheckLogFile(cancellationToken);
        }

        async Task ContinuallyCheckLogFile(CancellationToken cancellationToken)
        {
            //Task task = null;

            await Task.Run(() =>
            {
                while (true)
                {
                    Task.Delay(1000).Wait();

                    lock (lockFilePath)
                    {
                        if (filePath != null)
                        {
                            try
                            {
                                if (cancellationToken.IsCancellationRequested == true)
                                    throw new TaskCanceledException();// (task);

                                if (File.Exists(filePath) == false)
                                    continue;

                                ReadFile();
                            }
                            catch (Exception ex)
                            {
                                //Log.Fatal(ex, "Unexpected error:");
                                System.Diagnostics.Debugger.Break();
                            }
                        }
                    }
                }
            }).ConfigureAwait(false);
        }

        void ReadFile()
        {
            var fileSize = new FileInfo(filePath).Length;
            if (fileSize != lastSize)
            {
                var newText = "";

                if (fileSize < lastSize)
                {
                    // New file was created
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        newText = reader.ReadToEnd();
                    }
                    lastSize = new FileInfo(filePath).Length;
                    LogContentToSend = new StringBuilder(newText);
                }
                else
                {
                    // File got bigger
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var newTextSize = fs.Length - lastSize;
                        byte[] b = new byte[newTextSize];

                        fs.Seek(lastSize, SeekOrigin.Begin);
                        fs.Read(b, 0, (int)(newTextSize));

                        newText = Encoding.UTF8.GetString(b);
                    }

                    lastSize = fileSize;
                    LogContentToSend.Append(newText);
                }

                if (initialLoad == false)
                {
                    OnFileSizeChangedNewText?.Invoke(this, newText);
                }

                initialLoad = false;
            }
        }

        public void ResetStringBuilder()
        {
            LogContentToSend = new StringBuilder();
        }
    }
}

using System.IO;
using System.IO.Compression;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class LogFileZipper
    {
        private static byte[] Zip(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new byte[0];

            using var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                ZipArchiveEntry manifest = archive.CreateEntry("output_log.txt");
                using Stream st = manifest.Open();
                using var writerManifest = new StreamWriter(st);
                writerManifest.Write(text);
            }

            return stream.ToArray();
        }

        public byte[] ZipFile(string logFilePath)
        {
            if (File.Exists(logFilePath) == false)
                return new byte[0];

            //var logFileContent = File.ReadAllText(logFilePath);
            string logFileContent = ReadLogFile(logFilePath);

            return Zip(logFileContent);
        }

        public static string ReadLogFile(string logFilePath)
        {
            if (File.Exists(logFilePath) == false)
                return null;

            var logFileContent = "";
            var iTry = 0;
            while (iTry < 10)
            {
                try
                {
                    using FileStream stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sr = new StreamReader(stream);
                    logFileContent = sr.ReadToEnd();
                    // Success, stop trying
                    break;
                }
                catch (IOException)
                {
                    iTry++;
                    System.Threading.Thread.Sleep(1000);
                }
            }

            return logFileContent;
        }

        public byte[] ZipText(string newText)
        {
            return Zip(newText);
        }
    }
}

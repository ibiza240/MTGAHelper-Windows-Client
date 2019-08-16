using MTGAHelper.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGAHelper.Entity
{
    public class LogSplitter
    {
        Util util = new Util();

        public string LastPartWithDate { get; private set; }

        public uint GetLastUploadHash(string logContent)
        {
            using (var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(logContent ?? ""))))
            {
                var builder = new StringBuilder();

                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    var match = util.regexPrefix.Match(line);
                    if (match.Success || line.StartsWith("BIError"))
                    {
                        // New line
                        StoreLastPartWithDate(builder.ToString().Trim());
                        builder = new StringBuilder();
                    }

                    builder.AppendLine(line);
                }

                StoreLastPartWithDate(builder.ToString().Trim());
                return util.To32BitFnv1aHash(LastPartWithDate);
            }
        }

        public void StoreLastPartWithDate(string part)
        {
            //try
            //{
                if (part.Contains(DateTime.Now.Year.ToString()) || part.Contains((DateTime.Now.Year - 1).ToString()))
                    LastPartWithDate = part;
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
        }
    }
}

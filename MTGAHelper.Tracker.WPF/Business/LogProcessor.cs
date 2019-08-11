//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using MTGAHelper.Lib.Exceptions;
//using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
//using Serilog;

//namespace MTGAHelper.Tracker.WPF.Business
//{
//    public class LogProcessor
//    {
//        ReaderMtgaOutputLog reader;

//        public LogProcessor(ReaderMtgaOutputLog reader)
//        {
//            this.reader = reader;
//        }

//        public OutputLogResult Process(string userId, string text)
//        {
//            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text ?? "")))
//            {
//                try
//                {
//                    (OutputLogResult result, Guid? errorId) = reader.LoadFileContent(userId, ms);

//                    //if (result.CollectionByDate.Any(i => i.DateTime == default(DateTime)))
//                    //    SendErrorReport()

//                    //if (errorId.HasValue)
//                    //    SendErrorReport()

//                    return result;
//                }
//                catch (InvalidDataException ex)
//                {
//                    //throw new ParseCollectionInvalidZipFileException(ex);
//                    Log.Error(ex, "Problem processing the log piece:{NewLine}{logError}", text);
//                    return new OutputLogResult();
//                }
//            }
//        }

//        internal ICollection<IMtgaOutputLogPartResult> GetMessages(string newText)
//        {
//            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(newText ?? "")))
//            {
//                var messages = reader.ProcessIntoMessages("local", ms);
//                return messages;
//            }
//        }

//        internal void ResetPreparer()
//        {
//            reader.ResetPreparer();
//        }
//    }
//}

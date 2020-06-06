using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib.Exceptions;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class ZipDeflator
    {
        readonly ReaderMtgaOutputLog reader;

        public ZipDeflator(ReaderMtgaOutputLog reader)
        {
            this.reader = reader;
        }

        public async Task<(OutputLogResult result, Guid? errorId, OutputLogResult2 outputLogResult2)> UnzipAndGetCollection(string userId, Stream fileStream)
        {
            if (fileStream == null)
                throw new ParseCollectionBaseException("No file provided");

            try
            {
                var archive = new ZipArchive(fileStream);

                if (archive.Entries.Count == 0)
                    throw new ParseCollectionEmptyZipContentException();

                if (archive.Entries.Count > 1)
                    throw new ParseCollectionZipContentMultipleFilesException();

                var entry = archive.Entries[0];

                var unzippedEntryStream = await Task.Run(() => entry.Open());
                return await reader.LoadFileContent(userId, unzippedEntryStream);
            }
            catch (InvalidDataException ex)
            {
                throw new ParseCollectionInvalidZipFileException(ex);
            }
        }

    }
}

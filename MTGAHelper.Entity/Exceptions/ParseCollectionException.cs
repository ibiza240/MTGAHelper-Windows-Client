using System;

namespace MTGAHelper.Lib.Exceptions
{
    public class ParseCollectionBaseException : Exception
    {
        public ParseCollectionBaseException(string msg)
            : base(msg)
        {
        }

        public ParseCollectionBaseException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
    public class ParseCollectionEmptyException : ParseCollectionBaseException
    {
        public ParseCollectionEmptyException()
            : base("There was no collection data in the provided file. Try regenerating the output_log.txt file by opening the MTGA game client and browsing your collection.")
        {
        }
    }

    public class ParseCollectionInvalidZipFileException : ParseCollectionBaseException
    {
        public ParseCollectionInvalidZipFileException(Exception ex)
            : base("The provided zip file is corrupted and cannot be decompressed - Maybe the zip file was created while the MTGA game client was running?", ex)
        {
        }
    }

    public class ParseCollectionInvalidZipContentException : ParseCollectionBaseException
    {
        public ParseCollectionInvalidZipContentException()
            : base("The content of the zip file is invalid - The zip file must contain the output_log.txt file only (not HTML logs)")
        {
        }
    }

    public class ParseCollectionEmptyZipContentException : ParseCollectionBaseException
    {
        public ParseCollectionEmptyZipContentException()
            : base("The zip file is empty - The zip file must contain the output_log.txt file")
        {
        }
    }

    public class ParseCollectionZipContentMultipleFilesException : ParseCollectionBaseException
    {
        public ParseCollectionZipContentMultipleFilesException()
            : base("The zip file contains more than one file - The zip file must contain only the output_log.txt file")
        {
        }
    }
    
    public class ParseCollectionInvalidHtmlFoundException : ParseCollectionBaseException
    {
        public ParseCollectionInvalidHtmlFoundException()
            : base("The zip file contains the wrong log file - The output_log.txt file is required, not an HTML log file.")
        {
        }
    }

    public class ParseCollectionInvalidFileException : ParseCollectionBaseException
    {
        public ParseCollectionInvalidFileException()
            : base("The zip file contains the wrong file - The output_log.txt file is required.")
        {
        }
    }
}

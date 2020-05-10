using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.Lib.Config;
using Newtonsoft.Json;

namespace MTGAHelper.Lib
{
    public class PossibleDateFormats : IPossibleDateFormats
    {
        public IReadOnlyList<string> Formats { get; }

        public PossibleDateFormats(IEnumerable<string> formats)
        {
            Formats = formats
                .ToList()
                .AsReadOnly();
        }
    }
    public class DateFormatsFromFile : PossibleDateFormats
    {
        public DateFormatsFromFile(IDataPath folderData)
            : base(GetStringsFromFile(folderData))
        { }

        static IEnumerable<string> GetStringsFromFile(IDataPath folderData)
        {
            var data = File.ReadAllText(Path.Combine(folderData.FolderData, "dateFormats.json"));
            return JsonConvert.DeserializeObject<ICollection<string>>(data);
        }
    }

    public class DateFormatsHardCoded : IPossibleDateFormats
    {
        public IReadOnlyList<string> Formats => new[] {
            "d. M. yyyy H:mm:ss",
            "d. M. yyyy HH:mm:ss",
            "d. M. yyyy. HH:mm:ss",
            "d. MM. yyyy HH:mm:ss",
            "d.M.yyyy H.mm.ss",
            "d.M.yyyy H:mm:ss",
            "d.M.yyyy h:mm:ss tt",
            "d.M.yyyy HH:mm:ss",
            "d.M.yyyy 'г.' H:mm:ss",
            "d.M.yyyy. H:mm:ss",
            "d.M.yyyy. HH:mm:ss",
            "d.MM.yyyy h:mm:ss tt",
            "d.MM.yyyy HH:mm:ss",
            "d.MM.yyyy 'Klock' H.mm:ss",
            "d/M/yy h:mm:ss tt",
            "d/M/yyyy H:mm:ss",
            "d/M/yyyy h:mm:ss tt",
            "d/M/yyyy HH:mm:ss",
            "d/M/yyyy tt h:mm:ss",
            "d/M/yyyy tth:mm:ss",
            "d/MM/yyyy H:mm:ss",
            "d/MM/yyyy h:mm:ss tt",
            "d/MM/yyyy HH:mm:ss",
            "dd.M.yyyy HH:mm:ss",
            "dd.MM.yy H:mm:ss",
            "dd.MM.yy HH:mm:ss",
            "dd.MM.yy 'ý.' HH:mm:ss",
            "dd.MM.yyyy H:mm:ss",
            "dd.MM.yyyy HH:mm:ss",
            "dd.MM.yyyy. H:mm:ss",
            "dd/MM yyyy H:mm:ss",
            "dd/MM yyyy HH:mm:ss",
            "dd/MM/yy HH:mm:ss",
            "dd/MM/yy hh:mm:ss tt",
            "dd/MM/yyyy H.mm.ss",
            "dd/MM/yyyy H:mm:ss",
            "dd/MM/yyyy h:mm:ss tt",
            "dd/MM/yyyy HH.mm.ss",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy tt hh:mm:ss",
            "dd-MM-yy h.mm.ss tt",
            "dd-MM-yy HH.mm.ss",
            "dd-MM-yy HH:mm:ss",
            "dd-MM-yy tt hh:mm:ss",
            "dd-MM-yyyy H:mm:ss",
            "dd-MM-yyyy HH:mm:ss",
            "dd-MM-yyyy tt h:mm:ss",
            "d-MMM yy HH:mm:ss",
            "d-M-yyyy H:mm:ss",
            "d-M-yyyy HH:mm:ss",
            "M/d/yyyy h:mm:ss tt",
            "M/d/yyyy HH:mm:ss",
            "M/d/yyyy tt 'ga' h:mm:ss",
            "MM/dd/yyyy h:mm:ss tt",
            "MM/dd/yyyy HH:mm:ss",
            "yyyy. M. d. tt h:mm:ss",
            "yyyy. MM. dd. H:mm:ss",
            "yyyy.MM.dd HH:mm:ss",
            "yyyy/M/d H:mm:ss",
            "yyyy/M/d h:mm:ss tt",
            "yyyy/M/d HH:mm:ss",
            "yyyy/M/d tt h:mm:ss",
            "yyyy/M/d tt hh:mm:ss",
            "yyyy/MM/dd H:mm:ss",
            "yyyy/MM/dd h:mm:ss tt",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy/MM/dd hh:mm:ss tt",
            "yyyy-M-d H:mm:ss",
            "yyyy-MM-dd h:mm:ss tt",
            "yyyy-MM-dd HH.mm.ss",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd tt h:mm:ss"
        };
    }
}

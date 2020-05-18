using MTGAHelper.Tracker.DraftHelper.Shared.Config;
using MTGAHelper.Tracker.DraftHelper.Shared.Models;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Services
{
    public class DraftRatingsLoader
    {
        private readonly string FilepathDraftRatings;

        public DraftRatingsLoader(IConfigFolderData configFolderData)
        {
            FilepathDraftRatings = Path.Combine(configFolderData.FolderData, "draftRatings.json");
        }

        public ICollection<DraftRating> GetRatingsForSet(string source, string set)
        {
            var fileData = LoadData();
            return fileData[source].RatingsBySet[set].Ratings;
        }

        public Dictionary<string, DraftRatings> LoadData()
        {
            Log.Information("Loading ratings from {filepathDraftRatings}", FilepathDraftRatings);
            string content = File.ReadAllText(FilepathDraftRatings);
            var ratingsBySource = JsonConvert.DeserializeObject<Dictionary<string, DraftRatings>>(content);

            return ratingsBySource;
        }
    }
}
